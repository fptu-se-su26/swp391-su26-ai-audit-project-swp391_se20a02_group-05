import os
import shutil
import tempfile
import json
import logging
import asyncio
import time
from abc import ABC, abstractmethod
from typing import AsyncGenerator, Any, Tuple, List, Literal, Optional, Union
import redis.asyncio as redis
from pydantic import BaseModel, Field, ValidationError, field_validator
from app.core.config import settings
from app.pipelines.repository.orchestrators.unified_evidence import UnifiedEvidenceEngine

_SKILL_CONFIDENCE_FLOOR = 40.0
_CROSS_CUTTING_SKILL_PATTERNS = ["state management", "clean architecture", "rest api design", "dependency injection"]



from app.pipelines.repository.github.technology_detector import TechnologyDetector
from app.pipelines.repository.github.code_sampler import CodeSampler, CodeSamplingOptions
from app.pipelines.shared.ai.prompts.github_prompt_factory import GitHubPromptFactory
from app.pipelines.shared.ai.prompts.cv_prompt_factory import CvPromptFactory
from app.core.services.claude_service import ClaudeService
from app.pipelines.repository.github.repo_classifier import classify_repository

from app.core.monitoring.observability import trace_stage, TraceContext, persist_trace_logs

class ClassificationV2(BaseModel):
    primaryDomain: str
    subDomain: str
    confidence: float = Field(..., ge=0.0, le=1.0)
    isVerified: bool
    trustScore: float = Field(..., ge=0.0, le=1.0)

class SectionItemV2(BaseModel):
    title: str
    content: str

class SectionV2(BaseModel):
    type: Literal["engineering_practices", "security_findings", "architecture_insights"]
    items: List[Union[str, SectionItemV2]]

class RiskV2(BaseModel):
    score: float = Field(..., ge=0.0, le=100.0)
    level: Literal["low", "medium", "high"]
    reasons: List[str]

class CvHighlight(BaseModel):
    signal: str
    impact: str

class CvSynthesisContract(BaseModel):
    schemaVersion: Literal["v2"] = "v2"
    title: str
    skills: List[str]
    summary: str
    highlights: List[CvHighlight]
    ownershipProfile: Literal["High contribution profile", "Standard contribution profile", "Low contribution profile", "External contributor context"]

    @field_validator("summary")
    @classmethod
    def validate_summary_length(cls, v: str) -> str:
        # Hard validation bounds to reject extreme outliers, keeping the orchestrator retry loop safe.
        if len(v) < 100:
            raise ValueError("CV summary is too short (must be at least 100 characters).")
        if len(v) > 550:
            raise ValueError("CV summary is too long (must be under 550 characters).")
        return v

class EvidenceStrengthContract(BaseModel):
    score: float
    label: str

class ReportV2Contract(BaseModel):
    schemaVersion: Literal["v2"]
    repoId: str
    classification: ClassificationV2
    sections: List[SectionV2]
    risk: RiskV2
    cvSynthesis: Optional[CvSynthesisContract] = None
    evidenceStrength: Optional[EvidenceStrengthContract] = None

    model_config = {
        "extra": "allow"
    }

logger = logging.getLogger("github_analysis_orchestrator")

class IGitHubAnalysisOrchestrator(ABC):
    @abstractmethod
    async def orchestrate_async(
        self,
        repository_id: Any,
        repo_name: str,
        repo_owner: str,
        encrypted_token: str,
        default_branch: str,
        correlation_id: str
    ) -> AsyncGenerator[dict, None]:
        ...

class GitHubAnalysisOrchestrator(IGitHubAnalysisOrchestrator):
    def __init__(self):
        self.tech_detector = TechnologyDetector()
        self.code_sampler = CodeSampler()
        self.prompt_factory = GitHubPromptFactory()
        self.cv_prompt_factory = CvPromptFactory()
        self.claude_service = ClaudeService()
        self.redis_client = redis.from_url(settings.redis_url, decode_responses=True)

    async def publish_task_event(self, job_id: str, task_type: str, message: str, level: str = "Info"):
        logger_func = logger.info if level.lower() == "info" else logger.warning if level.lower() == "warning" else logger.error
        logger_func(f"[{task_type}] {message}", extra={"job_id": job_id, "task_type": task_type})
        
        try:
            from app.core.monitoring.observability import UIStreamingManager
            await UIStreamingManager().enqueue_ui_event(
                job_id=job_id,
                task_type=task_type,
                task_status="Running",
                level=level,
                message=message
            )
        except Exception as e:
            logger.warning(f"Failed to publish progress log to Redis: {e}")

    async def orchestrate_async(
        self,
        repository_id: Any,
        repo_name: str,
        repo_owner: str,
        encrypted_token: str,
        default_branch: str,
        correlation_id: str = "system"
    ) -> AsyncGenerator[dict, None]:
        # Legacy monolithic stream endpoint - mapped for compatibility
        # Runs classification and loops through tasks yielding progress
        extra_log = {"correlation_id": correlation_id}
        start_time = time.perf_counter()

        logger.info(f"Starting legacy repository analysis stream for {repo_owner}/{repo_name}", extra=extra_log)
        yield {
            "status": "Preparing",
            "step": "Preparing",
            "progress": 10.0,
            "message": "Initializing analysis pipeline workspace..."
        }
        
        # Stub implementing fallback - C# core uses discrete execute_task endpoints in UAT
        try:
            classification = await classify_repository(repo_owner, repo_name, encrypted_token, correlation_id)
            yield {
                "status": "Completed",
                "step": "Completed",
                "progress": 100.0,
                "message": "Legacy stream completed. Discrete task orchestrator is active."
            }
        except Exception as e:
            logger.error(f"Legacy stream runner failed: {e}", extra=extra_log)
            yield {
                "status": "Failed",
                "step": "Failed",
                "progress": 0.0,
                "message": str(e)
            }

    async def execute_task(
        self,
        task_type: str,
        job_id: str,
        repository_id: str,
        repo_owner: str,
        repo_name: str,
        encrypted_token: str,
        default_branch: str,
        correlation_id: str = "system"
    ) -> dict:
        extra_log = {"correlation_id": correlation_id, "job_id": job_id, "task_type": task_type}
        
        # Determine if debug mode is active
        debug_mode = os.getenv("AI_DEBUG_MODE", "false").lower() == "true"
        TraceContext.set(
            pipeline_stage=task_type,
            is_sampled=True, # Always log 100% of discrete task executions
            extra={
                "jobId": job_id,
                "taskType": task_type,
                "correlationId": correlation_id,
                "debug": debug_mode
            }
        )
        
        logger.info(f"Executing discrete analysis task {task_type} for job {job_id}", extra=extra_log)
        start_time = time.perf_counter()
        
        try:
            if task_type == "RepoStructure":
                result = await self.analyze_structure(job_id, repository_id, repo_owner, repo_name, encrypted_token, default_branch, correlation_id)
            elif task_type == "CommitIntelligence":
                result = await self.analyze_commits(job_id, encrypted_token, correlation_id)
            elif task_type == "SkillExtraction":
                result = await self.analyze_skills(job_id, encrypted_token, correlation_id)
            elif task_type == "ArchitectureAnalysis":
                result = await self.analyze_architecture(job_id, encrypted_token, correlation_id)
            elif task_type == "CodeQuality":
                result = await self.analyze_quality(job_id, encrypted_token, correlation_id)
            elif task_type == "SecurityAnalysis":
                result = await self.analyze_security(job_id, encrypted_token, correlation_id)
            elif task_type == "RepositoryClassification":
                result = await self.analyze_classification(job_id, encrypted_token, correlation_id)
            elif task_type == "RepositorySummary":
                result = await self.analyze_summary(job_id, encrypted_token, correlation_id)
            elif task_type == "CvSynthesis":
                result = await self.analyze_cv_synthesis(job_id, encrypted_token, correlation_id)
            # ── Line 1 v2 pipeline tasks (DagScheduler L1-003 … L1-018) ──
            elif task_type in ("CommitDiff", "L1-003"):
                result = await self.analyze_commit_diff(job_id, encrypted_token, correlation_id)
            elif task_type in ("CommitTimeline", "L1-007"):
                result = await self.analyze_commit_timeline(job_id, encrypted_token, correlation_id)
            elif task_type in ("CommitIntent", "L1-009"):
                result = await self.analyze_commit_intent(job_id, encrypted_token, correlation_id)
            elif task_type in ("Complexity", "L1-010"):
                result = await self.analyze_complexity(job_id, encrypted_token, correlation_id)
            elif task_type in ("GitBlame", "L1-012"):
                result = await self.analyze_git_blame(job_id, encrypted_token, correlation_id)
            elif task_type in ("CloneDetection", "L1-013"):
                result = await self.analyze_clone_detection(job_id, encrypted_token, correlation_id)
            elif task_type in ("AiGeneratedCode", "L1-014"):
                result = await self.analyze_ai_generated_code(job_id, encrypted_token, correlation_id)
            elif task_type in ("Ownership", "L1-015"):
                result = await self.analyze_ownership(job_id, encrypted_token, correlation_id)
            elif task_type in ("SkillGraph", "L1-017"):
                result = await self.analyze_skill_graph(job_id, encrypted_token, correlation_id)
            elif task_type in ("TrustScore", "L1-018"):
                result = await self.analyze_trust_score(job_id, encrypted_token, correlation_id)
            else:
                raise ValueError(f"Unknown task type: {task_type}")

            # Save result data to local workspace file for downstream tasks (like CvSynthesis)
            try:
                temp_dir_base = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones"))
                job_dir = os.path.join(temp_dir_base, job_id)
                if os.path.exists(job_dir):
                    task_file = os.path.join(job_dir, f"{task_type}_result.json")
                    with open(task_file, "w", encoding="utf-8") as f:
                        f.write(json.dumps(result.get("data")))
            except Exception as e:
                logger.warning(f"Failed to write task result cache to workspace: {e}", extra=extra_log)

            # Persist trace logs asynchronously
            persist_trace_logs(job_id, debug_mode)

            return {
                "status": "Completed",
                "errorMessage": None,
                "schemaVersion": "2.0.0",
                "resultData": json.dumps(result.get("data")),
                "telemetry": result.get("telemetry"),
                "events": result.get("events", [])
            }
        except Exception as e:
            duration = int((time.perf_counter() - start_time) * 1000)
            logger.exception(f"Error executing task {task_type} for job {job_id}: {e}", extra=extra_log)
            
            # Extract accumulated execution records from TraceContext
            executions = TraceContext.get().get("executions", [])
            task_prompt_tokens = sum(ev.get("promptTokens", 0) for ev in executions)
            task_completion_tokens = sum(ev.get("completionTokens", 0) for ev in executions)
            task_cache_read = sum(ev.get("cacheReadTokens", 0) for ev in executions)
            task_cache_write = sum(ev.get("cacheWriteTokens", 0) for ev in executions)
            task_cost = sum(ev.get("estimatedCostUsd", 0) for ev in executions)
            
            # Clear context executions
            TraceContext.set(executions=[])
            
            telemetry = None
            if executions:
                telemetry = {
                    "promptTokens": task_prompt_tokens,
                    "completionTokens": task_completion_tokens,
                    "totalTokens": task_prompt_tokens + task_completion_tokens,
                    "cacheReadTokens": task_cache_read,
                    "cacheWriteTokens": task_cache_write,
                    "estimatedCostUsd": float(task_cost),
                    "durationMs": duration,
                    "modelName": executions[0].get("model") if executions else settings.claude_model,
                    "provider": "Anthropic"
                }
            
            err_str = str(e).lower()
            error_code = "UNKNOWN_ERROR"
            retryable = True
            
            if "rate limit" in err_str or "429" in err_str:
                error_code = "RATE_LIMIT_EXCEEDED"
                retryable = True
            elif "timeout" in err_str or "time out" in err_str:
                error_code = "TIMEOUT"
                retryable = True
            elif "connection" in err_str or "dns" in err_str:
                error_code = "SERVICE_UNAVAILABLE"
                retryable = True
            elif "json" in err_str or "parse" in err_str or "format" in err_str:
                error_code = "PARSING_ERROR"
                retryable = False
            elif "invalid_prompt" in err_str or "bad request" in err_str:
                error_code = "INVALID_REQUEST"
                retryable = False
            
            # Persist logs even on failure to capture error traceback
            persist_trace_logs(job_id, debug_mode)
            
            return {
                "status": "Failed",
                "errorMessage": str(e),
                "errorCode": error_code,
                "retryable": retryable,
                "taskId": task_type,
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "schemaVersion": "2.0.0",
                "resultData": None,
                "telemetry": telemetry,
                "events": [
                    {
                        "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                        "level": "Error",
                        "eventType": "AI_TASK_FAILED",
                        "message": str(e)
                    }
                ]
            }

    async def _get_meta_and_sample(self, job_id: str, encrypted_token: str, correlation_id: str) -> tuple[dict, Any]:
        temp_dir_base = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones"))
        job_dir = os.path.join(temp_dir_base, job_id)
        clone_dir = os.path.join(job_dir, "repo")
        meta_path = os.path.join(job_dir, "meta.json")

        if not os.path.exists(meta_path):
            raise Exception("Workspace metadata not found. Repository Structure task must run first.")

        with open(meta_path, "r", encoding="utf-8") as f_in:
            meta = json.load(f_in)

        if not meta.get("is_cloned"):
            from app.pipelines.repository.github.code_sampler import CodeSample
            return meta, CodeSample(file_content=[], file_names=[])

        options = CodeSamplingOptions(max_files=10, max_lines_per_file=100)
        sample = await self.code_sampler.sample_async(clone_dir, encrypted_token, options)
        return meta, sample

    def _repair_json_string(self, candidate: str) -> str:
        chars = []
        stack = []
        in_string = False
        i = 0
        n = len(candidate)
        
        while i < n:
            c = candidate[i]
            
            if not in_string:
                if c == '"':
                    in_string = True
                    chars.append(c)
                    i += 1
                elif c == '{':
                    stack.append('}')
                    chars.append(c)
                    i += 1
                elif c == '[':
                    stack.append(']')
                    chars.append(c)
                    i += 1
                elif c in ('}', ']'):
                    if stack and stack[-1] == c:
                        stack.pop()
                    chars.append(c)
                    i += 1
                elif c == ',':
                    # Check for trailing comma
                    next_idx = i + 1
                    while next_idx < n and candidate[next_idx].isspace():
                        next_idx += 1
                    if next_idx < n and candidate[next_idx] in ('}', ']'):
                        # Skip the comma, move directly to brace/bracket
                        i = next_idx
                    else:
                        chars.append(c)
                        i += 1
                else:
                    chars.append(c)
                    i += 1
            else: # in_string is True
                if c == '\\':
                    # If it's a valid escape sequence for quote or backslash, preserve it as-is
                    if i + 1 < n and candidate[i + 1] in ('"', '\\'):
                        chars.append(c)
                        chars.append(candidate[i + 1])
                        i += 2
                    else:
                        # Escape the backslash
                        chars.append('\\')
                        chars.append('\\')
                        i += 1
                elif c == '"':
                    # Check if this is the end of the string
                    next_idx = i + 1
                    while next_idx < n and candidate[next_idx].isspace():
                        next_idx += 1
                    # If we've reached the end of the candidate string, or it is followed by structural JSON chars,
                    # it's a valid closing quote.
                    if next_idx >= n or candidate[next_idx] in (',', '}', ']', ':'):
                        in_string = False
                        chars.append(c)
                        i += 1
                    else:
                        # Inner unescaped double quote - escape it
                        chars.append('\\')
                        chars.append('"')
                        i += 1
                elif c == '\n':
                    chars.append('\\')
                    chars.append('n')
                    i += 1
                elif c == '\r':
                    chars.append('\\')
                    chars.append('r')
                    i += 1
                elif c == '\t':
                    chars.append('\\')
                    chars.append('t')
                    i += 1
                else:
                    chars.append(c)
                    i += 1
                    
        # If we ended inside a string literal, close it
        if in_string:
            chars.append('"')
            
        # Clean up any trailing comma at the end of the reconstructed characters
        while chars and (chars[-1].isspace() or chars[-1] == ','):
            if chars[-1] == ',':
                chars.pop()
                break
            chars.pop()
            
        # Close any open braces or brackets
        while stack:
            chars.append(stack.pop())
            
        return "".join(chars)

    def _extract_json(self, text: str, correlation_id: str) -> dict:
        text = text.strip()
        first_brace = text.find('{')
        last_brace = text.rfind('}')
        
        if first_brace != -1:
            if last_brace != -1 and last_brace > first_brace:
                json_candidate = text[first_brace:last_brace + 1]
                try:
                    return json.loads(json_candidate)
                except Exception as e:
                    logger.warning(f"Failed to parse raw extracted JSON block: {e}. Attempting repair fallback.", extra={"correlation_id": correlation_id})
            
            try:
                # Scan from first_brace to the absolute end of the generated text to capture and heal truncated suffix
                full_candidate = text[first_brace:]
                repaired = self._repair_json_string(full_candidate)
                return json.loads(repaired)
            except Exception as retry_err:
                logger.error(f"Failed to parse repaired JSON block: {retry_err}", extra={"correlation_id": correlation_id})
                raise Exception(f"Claude output returned invalid JSON inside block. Sanitization failed: {retry_err}")
        
        try:
            return json.loads(text)
        except Exception as e:
            logger.error(f"Failed to parse Claude output as JSON. Error: {e}", extra={"correlation_id": correlation_id})
            raise Exception("Claude output did not return a valid JSON format.")

    @trace_stage("RepoStructure")
    async def analyze_structure(self, job_id: str, repository_id: str, repo_owner: str, repo_name: str, encrypted_token: str, default_branch: str, correlation_id: str) -> dict:
        extra_log = {"correlation_id": correlation_id}
        temp_dir_base = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones"))
        job_dir = os.path.join(temp_dir_base, job_id)
        clone_dir = os.path.join(job_dir, "repo")
        meta_path = os.path.join(job_dir, "meta.json")

        await self.publish_task_event(job_id, "RepoStructure", f"Creating workspace directory: {job_dir}")
        os.makedirs(job_dir, exist_ok=True)

        await self.publish_task_event(job_id, "RepoStructure", "Classifying repository: checking stats (stars, forks) and history...")
        classification = await classify_repository(
            repo_owner=repo_owner,
            repo_name=repo_name,
            encrypted_token=encrypted_token,
            correlation_id=correlation_id
        )
        await self.publish_task_event(job_id, "RepoStructure", f"Repository classified. Type: {classification.repo_type}. Stars: {classification.stars_count}. Forks: {classification.forks_count}.")

        filenames = []
        all_techs = []
        is_cloned = False

        if classification.repo_type == "FORK_NO_CONTRIBUTION":
            await self.publish_task_event(job_id, "RepoStructure", "Fork with no contributions. Skipping clone.")
            pass
        else:
            clone_owner = repo_owner
            clone_name = repo_name
            if classification.repo_type == "FORK_UPSTREAM_CONTRIBUTION":
                clone_owner = classification.analysis_target_owner
                clone_name = classification.analysis_target_name

            clone_url = f"https://{encrypted_token}@github.com/{clone_owner}/{clone_name}.git"

            if not os.path.exists(os.path.join(clone_dir, ".git")):
                shutil.rmtree(clone_dir, ignore_errors=True)
                os.makedirs(clone_dir, exist_ok=True)
                env = os.environ.copy()
                env["GIT_TERMINAL_PROMPT"] = "0"
                import subprocess

                def clone_with_branch():
                    return subprocess.run(
                        ["git", "-c", "credential.helper=", "clone", "--depth", "100", "--branch", default_branch, clone_url, clone_dir],
                        env=env,
                        capture_output=True
                    )

                await self.publish_task_event(job_id, "RepoStructure", f"Cloning branch '{default_branch}' from GitHub...")
                proc = await asyncio.to_thread(clone_with_branch)
                if proc.returncode != 0:
                    shutil.rmtree(clone_dir, ignore_errors=True)
                    def clone_default_branch():
                        return subprocess.run(
                            ["git", "-c", "credential.helper=", "clone", "--depth", "100", clone_url, clone_dir],
                            env=env,
                            capture_output=True
                        )
                    await self.publish_task_event(job_id, "RepoStructure", "Cloning default branch (fallback method)...")
                    proc_retry = await asyncio.to_thread(clone_default_branch)
                    if proc_retry.returncode != 0:
                        stderr_retry = proc_retry.stderr
                        err_msg = stderr_retry.decode("utf-8", errors="ignore").strip()
                        raise Exception(f"Git clone failed: {err_msg}")
            
            is_cloned = True
            await self.publish_task_event(job_id, "RepoStructure", "Cloning completed successfully. Scanning workspace directory...")

            package_contents = []
            package_names = {"package.json", "requirements.txt", "go.mod", "pom.xml", "cargo.toml", "docker-compose.yml"}

            for root, dirs, files in os.walk(clone_dir):
                dirs[:] = [d for d in dirs if d not in {".git", "node_modules", "bin", "obj", "dist", "vendor", "venv", "packages", "__pycache__"}]
                for f in files:
                    filenames.append(f)
                    if f.lower() in package_names or f.endswith(".csproj"):
                        try:
                            with open(os.path.join(root, f), "r", encoding="utf-8", errors="ignore") as f_in:
                                package_contents.append(f_in.read(2000))
                        except OSError:
                            pass

            techs_from_files = self.tech_detector.detect_from_filenames(filenames)
            techs_from_package = self.tech_detector.detect_from_package_files(package_contents)
            all_techs = list(set(techs_from_files + techs_from_package))
            await self.publish_task_event(job_id, "RepoStructure", f"Scanned {len(filenames)} files. Detected languages/frameworks: {', '.join(all_techs)}.")

        # Retrieve sampled files list
        sampled_files_names = []
        if is_cloned:
            await self.publish_task_event(job_id, "RepoStructure", "Sampling files for content analysis...")
            options = CodeSamplingOptions(max_files=10, max_lines_per_file=100)
            sample = await self.code_sampler.sample_async(clone_dir, encrypted_token, options)
            sampled_files_names = sample.file_names

        # Compute extended quality metrics
        files_scanned = len(filenames)
        files_sampled = len(sampled_files_names)
        skipped_files = max(0, files_scanned - files_sampled)
        coverage_pct = round(files_sampled / files_scanned * 100, 1) if files_scanned > 0 else 100.0

        meta_data = {
            "job_id": job_id,
            "repository_id": repository_id,
            "repo_owner": repo_owner,
            "repo_name": repo_name,
            "default_branch": default_branch,
            "repo_type": classification.repo_type,
            "confidence_ceiling": classification.confidence_ceiling,
            "confidence_modifier": classification.confidence_modifier,
            "classification_rationale": classification.classification_rationale,
            "analysis_target_owner": classification.analysis_target_owner,
            "analysis_target_name": classification.analysis_target_name,
            "red_flags": classification.red_flags,
            "technologies": all_techs,
            "filenames": filenames,
            "sampled_files": sampled_files_names,
            "is_cloned": is_cloned,
            
            # Save classifier stats
            "branches_count": classification.branches_count,
            "prs_count": classification.prs_count,
            "issues_count": classification.issues_count,
            "stars_count": classification.stars_count,
            "forks_count": classification.forks_count,
            "total_commits": classification.total_commits,
            "user_commit_ratio": classification.user_commit_ratio,
            "is_primary_author": classification.is_primary_author,
            "contributor_distribution": classification.contributor_distribution,
            "bus_factor": classification.bus_factor,
            "active_contributors": classification.active_contributors,
            
            # Quality stats
            "files_scanned": files_scanned,
            "files_sampled": files_sampled,
            "skipped_files": skipped_files,
            "coverage_pct": coverage_pct,
            
            # Trust and Adversarial metrics
            "unverified_commits_count": getattr(classification, "unverified_commits_count", 0),
            "timestamp_compression_ratio": getattr(classification, "timestamp_compression_ratio", 0.0),
            "uncalibrated_identities_count": getattr(classification, "uncalibrated_identities_count", 0)
        }

        with open(meta_path, "w", encoding="utf-8") as f_out:
            json.dump(meta_data, f_out)

        result_data = {
            "repo": {
                "id": repository_id,
                "name": repo_name,
                "full_name": f"{repo_owner}/{repo_name}",
                "url": f"https://github.com/{repo_owner}/{repo_name}",
                "description": "Fork with no contributions" if classification.repo_type == "FORK_NO_CONTRIBUTION" else None,
                "fork": classification.repo_type in ("FORK_NO_CONTRIBUTION", "FORK_UPSTREAM_CONTRIBUTION"),
                "languages": {t: round(100.0/len(all_techs), 1) for t in all_techs} if all_techs else {"Other": 100.0},
                "repo_type": classification.repo_type,
                "confidence_ceiling": classification.confidence_ceiling,
                
                # Real counts from classifier API
                "stars": classification.stars_count,
                "forks": classification.forks_count,
                "branches": classification.branches_count,
                "open_prs": classification.prs_count
            },
            "classification": {
                "primary_type": "Fork" if classification.repo_type == "FORK_NO_CONTRIBUTION" else "Unclassified",
                "all_types": ["Fork"] if classification.repo_type == "FORK_NO_CONTRIBUTION" else [],
                "complexity": "low" if classification.repo_type == "FORK_NO_CONTRIBUTION" else "medium",
                "benchmark_group": "forks" if classification.repo_type == "FORK_NO_CONTRIBUTION" else "unclassified",
                "classification_rationale": classification.classification_rationale,
                "sampled_files": sampled_files_names,
                "ignored_files_count": skipped_files,
                "confidence_factors": classification.red_flags or ["authentic_history"]
            },
            "confidence_meta": {
                "confidence_score": 100.0,
                "completeness_ratio": 1.0,
                "evidence_coverage_count": 0
            }
        }

        return {
            "data": result_data,
            "telemetry": None,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": f"Structure analysis completed. Classified as {classification.repo_type}."
                }
            ]
        }

    @trace_stage("CommitIntelligence")
    async def analyze_commits(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        
        # Setup workspace paths
        temp_dir_base = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones"))
        clone_dir = os.path.join(temp_dir_base, job_id, "repo")
        
        # Local Git history auditing
        local_total_commits = 0
        local_user_commit_ratio = 1.0
        local_bus_factor = 1
        local_contrib_counts = {}
        local_contributor_distribution = []

        await self.publish_task_event(job_id, "CommitIntelligence", "Reading local Git history logs...")
        if meta.get("is_cloned") and os.path.exists(os.path.join(clone_dir, ".git")):
            try:
                import subprocess
                proc = subprocess.run(
                    ["git", "log", "--format=%ae|%an", "--all"],
                    cwd=clone_dir,
                    capture_output=True,
                    text=True,
                    errors="ignore"
                )
                if proc.returncode == 0:
                    lines = [line.strip() for line in proc.stdout.strip().split("\n") if line.strip()]
                    local_commits = [line.split("|", 1) for line in lines if "|" in line]
                    local_total_commits = len(local_commits)
                    await self.publish_task_event(job_id, "CommitIntelligence", f"Parsed local Git log: {local_total_commits} total commits found. Computing distributions...")
                    
                    for email, name in local_commits:
                        key = email.lower().strip() if email else name.strip()
                        local_contrib_counts[key] = local_contrib_counts.get(key, 0) + 1
                    
                    # Compute user commits matching details
                    user_email = meta.get("user_email", "")
                    username = meta.get("username", "")
                    user_local_commits = 0
                    
                    for email, name in local_commits:
                        email_match = user_email and email.lower().strip() == user_email.lower().strip()
                        name_match = username and (username.lower().strip() in name.lower().strip() or name.lower().strip() in username.lower().strip())
                        if email_match or name_match:
                            user_local_commits += 1

                    local_user_commit_ratio = user_local_commits / local_total_commits if local_total_commits > 0 else 1.0
                    
                    for key, count in local_contrib_counts.items():
                        local_contributor_distribution.append({
                            "username": key,
                            "commit_ratio": round(count / local_total_commits, 4)
                        })
                        
                    sorted_local_contribs = sorted(list(local_contrib_counts.values()), reverse=True)
                    running_sum = 0
                    local_bus_factor = 0
                    half_local_commits = local_total_commits / 2
                    for c_commits in sorted_local_contribs:
                        running_sum += c_commits
                        local_bus_factor += 1
                        if running_sum >= half_local_commits:
                            break
                    if local_bus_factor == 0:
                        local_bus_factor = 1
            except Exception as e:
                logger.warning(f"Local git history parsing failed: {e}", extra={"correlation_id": correlation_id})

        # Deterministic facts calculation
        final_total_commits = local_total_commits if local_total_commits > 0 else meta.get("total_commits", 1)
        final_user_commit_ratio = local_user_commit_ratio if local_total_commits > 0 else meta.get("user_commit_ratio", 1.0)
        final_bus_factor = local_bus_factor if local_total_commits > 0 else meta.get("bus_factor", 1)
        final_active_contributors = len(local_contrib_counts) if local_total_commits > 0 else meta.get("active_contributors", 1)
        final_distribution = local_contributor_distribution if local_total_commits > 0 else meta.get("contributor_distribution", [])

        await self.publish_task_event(job_id, "CommitIntelligence", f"Git metrics computed: Bus Factor={final_bus_factor}, Active Contributors={final_active_contributors}, User Contribution Ratio={final_user_commit_ratio*100:.1f}%.")

        if not meta.get("is_cloned"):
            # Return fork stats structure directly
            await self.publish_task_event(job_id, "CommitIntelligence", "Ecosystem evaluation only (repo is a fork with no contributions).")
            return {
                "data": {
                    "ownership": {
                        "user_commit_ratio": 0.0,
                        "total_commits": 0,
                        "is_primary_author": False,
                        "architectural_ownership_pct": 0.0,
                        "critical_path_ownership_pct": 0.0,
                        "maintenance_duration_months": 0,
                        "explanation": "No contributions were found in this repository by the current user.",
                        "contributor_distribution": [],
                        "bus_factor": 1,
                        "active_contributors": 1
                    },
                    "trust": {
                        "classification": "template_dump",
                        "confidence": 30,
                        "rule_flags": ["fork_no_contributions"],
                        "ai_findings": ["Ecosystem familiarity evaluation only. Code belongs to parent author."],
                        "explanation": "Ecosystem familiarity evaluation only. Code belongs to parent author."
                    },
                    "confidence_meta": {
                        "confidence_score": 100.0,
                        "completeness_ratio": 1.0,
                        "evidence_coverage_count": 0
                    }
                },
                "telemetry": None,
                "events": []
            }

        files_str = "".join([f"--- FILE: {name} ---\n{content}\n\n" for name, content in zip(sample.file_names, sample.file_content)])
        input_payload = {
            "repo_name": meta.get("repo_name"),
            "repo_owner": meta.get("repo_owner"),
            "red_flags": meta.get("red_flags", []),
            "repo_type": meta.get("repo_type"),
            "files_str": files_str,
            # Ingest factual git history metadata for Claude context grounding
            "factual_total_commits": final_total_commits,
            "factual_user_commit_ratio": final_user_commit_ratio,
            "factual_bus_factor": final_bus_factor,
            "factual_active_contributors": final_active_contributors
        }
        system_prompt = self.prompt_factory.get_system_prompt()
        user_prompt = self.prompt_factory.get_commits_user_prompt(input_payload)
        
        await self.publish_task_event(job_id, "CommitIntelligence", "Invoking AI reasoning to evaluate repository trust and author patterns...")
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
        parsed = self._extract_json(raw_text, correlation_id)
        await self.publish_task_event(job_id, "CommitIntelligence", "AI reasoning complete. Parsed response successfully.")
        
        # Override Claude's generated values with real deterministic facts
        if "ownership" not in parsed:
            parsed["ownership"] = {}
        parsed["ownership"]["total_commits"] = final_total_commits
        parsed["ownership"]["user_commit_ratio"] = round(final_user_commit_ratio, 4)
        parsed["ownership"]["is_primary_author"] = (final_user_commit_ratio >= 0.50)
        parsed["ownership"]["bus_factor"] = final_bus_factor
        parsed["ownership"]["active_contributors"] = final_active_contributors
        parsed["ownership"]["contributor_distribution"] = final_distribution
        
        # Inject task-level confidence metadata
        parsed["confidence_meta"] = {
            "confidence_score": parsed.get("trust", {}).get("confidence", 80.0),
            "completeness_ratio": round(meta.get("coverage_pct", 100.0) / 100.0, 2),
            "evidence_coverage_count": len(parsed.get("trust", {}).get("ai_findings", []))
        }

        return {
            "data": parsed.get("data", parsed),
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": "Commit intelligence and Git trust analysis complete."
                }
            ]
        }

    @trace_stage("SkillExtraction")
    async def analyze_skills(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        await self.publish_task_event(job_id, "SkillExtraction", "Extracting skill signatures and technology stack details...")
        if not meta.get("is_cloned"):
            await self.publish_task_event(job_id, "SkillExtraction", "Ecosystem evaluation only (repo is a fork with no contributions).")
            return {
                "data": {
                    "skills": [],
                    "confidence_meta": {
                        "confidence_score": 100.0,
                        "completeness_ratio": 1.0,
                        "evidence_coverage_count": 0
                    }
                },
                "telemetry": None,
                "events": []
            }

        files_str = "".join([f"--- FILE: {name} ---\n{content}\n\n" for name, content in zip(sample.file_names, sample.file_content)])
        input_payload = {
            "repo_name": meta.get("repo_name"),
            "repo_owner": meta.get("repo_owner"),
            "technologies": meta.get("technologies", []),
            "files_str": files_str
        }
        system_prompt = self.prompt_factory.get_system_prompt()
        user_prompt = self.prompt_factory.get_skills_user_prompt(input_payload)
        
        await self.publish_task_event(job_id, "SkillExtraction", "Invoking AI Skill Extraction model...")
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
        parsed = self._extract_json(raw_text, correlation_id)
        await self.publish_task_event(job_id, "SkillExtraction", "AI reasoning complete. Parsed response successfully.")
        
        parsed_data = parsed.get("data", parsed)
        parsed_data["confidence_meta"] = {
            "confidence_score": 90.0,
            "completeness_ratio": round(meta.get("coverage_pct", 100.0) / 100.0, 2),
            "evidence_coverage_count": len(parsed_data.get("skills", []))
        }

        return {
            "data": parsed_data,
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": "Technical skill extraction complete."
                }
            ]
        }

    @trace_stage("ArchitectureAnalysis")
    async def analyze_architecture(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        await self.publish_task_event(job_id, "ArchitectureAnalysis", "Scanning codebase layout for architectural patterns...")
        if not meta.get("is_cloned"):
            await self.publish_task_event(job_id, "ArchitectureAnalysis", "Ecosystem evaluation only (repo is a fork with no contributions).")
            return {
                "data": {
                    "patterns": [],
                    "explanation": "Short-circuit repo classification",
                    "confidence_meta": {
                        "confidence_score": 100.0,
                        "completeness_ratio": 1.0,
                        "evidence_coverage_count": 0
                    }
                },
                "telemetry": None,
                "events": []
            }

        files_str = "".join([f"--- FILE: {name} ---\n{content}\n\n" for name, content in zip(sample.file_names, sample.file_content)])
        input_payload = {
            "repo_name": meta.get("repo_name"),
            "repo_owner": meta.get("repo_owner"),
            "technologies": meta.get("technologies", []),
            "files_str": files_str
        }
        system_prompt = self.prompt_factory.get_system_prompt()
        user_prompt = self.prompt_factory.get_architecture_user_prompt(input_payload)
        
        await self.publish_task_event(job_id, "ArchitectureAnalysis", "Invoking AI Architecture Pattern Scan...")
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
        parsed = self._extract_json(raw_text, correlation_id)
        await self.publish_task_event(job_id, "ArchitectureAnalysis", "AI reasoning complete. Parsed response successfully.")
        
        parsed_data = parsed.get("data", parsed)
        parsed_data["confidence_meta"] = {
            "confidence_score": 85.0,
            "completeness_ratio": round(meta.get("coverage_pct", 100.0) / 100.0, 2),
            "evidence_coverage_count": len(parsed_data.get("patterns", []))
        }

        return {
            "data": parsed_data,
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": "Architecture pattern evaluation complete."
                }
            ]
        }

    @trace_stage("CodeQuality")
    async def analyze_quality(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        await self.publish_task_event(job_id, "CodeQuality", "Inspecting code styling, testing configurations, and observability hooks...")
        if not meta.get("is_cloned"):
            await self.publish_task_event(job_id, "CodeQuality", "Ecosystem evaluation only (repo is a fork with no contributions).")
            return {
                "data": {
                    "testing": {"frameworks": [], "has_tests": False, "confidence": 0, "evidence": [], "detail": "N/A"},
                    "observability": {"logging_configured": False, "metrics_configured": False, "confidence": 0, "evidence": [], "detail": "N/A"},
                    "cicd": {"configured": False, "providers": [], "confidence": 0, "evidence": []},
                    "findings": [],
                    "confidence_meta": {
                        "confidence_score": 100.0,
                        "completeness_ratio": 1.0,
                        "evidence_coverage_count": 0
                    }
                },
                "telemetry": None,
                "events": []
            }

        files_str = "".join([f"--- FILE: {name} ---\n{content}\n\n" for name, content in zip(sample.file_names, sample.file_content)])
        input_payload = {
            "repo_name": meta.get("repo_name"),
            "repo_owner": meta.get("repo_owner"),
            "files_str": files_str
        }
        system_prompt = self.prompt_factory.get_system_prompt()
        user_prompt = self.prompt_factory.get_quality_user_prompt(input_payload)
        
        await self.publish_task_event(job_id, "CodeQuality", "Invoking AI Code Quality model...")
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
        parsed = self._extract_json(raw_text, correlation_id)
        await self.publish_task_event(job_id, "CodeQuality", "AI reasoning complete. Parsed response successfully.")
        
        parsed_data = parsed.get("data", parsed)
        parsed_data["confidence_meta"] = {
            "confidence_score": 80.0,
            "completeness_ratio": round(meta.get("coverage_pct", 100.0) / 100.0, 2),
            "evidence_coverage_count": len(parsed_data.get("findings", []))
        }

        return {
            "data": parsed_data,
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": "Code quality scan complete."
                }
            ]
        }

    @trace_stage("SecurityAnalysis")
    async def analyze_security(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        await self.publish_task_event(job_id, "SecurityAnalysis", "Auditing dependencies and code for potential vulnerabilities...")
        if not meta.get("is_cloned"):
            await self.publish_task_event(job_id, "SecurityAnalysis", "Ecosystem evaluation only (repo is a fork with no contributions).")
            return {
                "data": {
                    "vulnerabilities": [], "confidence": 100, "evidence": "N/A", "findings": [],
                    "confidence_meta": {
                        "confidence_score": 100.0,
                        "completeness_ratio": 1.0,
                        "evidence_coverage_count": 0
                    }
                },
                "telemetry": None,
                "events": []
            }

        files_str = "".join([f"--- FILE: {name} ---\n{content}\n\n" for name, content in zip(sample.file_names, sample.file_content)])
        input_payload = {
            "repo_name": meta.get("repo_name"),
            "repo_owner": meta.get("repo_owner"),
            "files_str": files_str
        }
        system_prompt = self.prompt_factory.get_system_prompt()
        user_prompt = self.prompt_factory.get_security_user_prompt(input_payload)
        
        await self.publish_task_event(job_id, "SecurityAnalysis", "Invoking AI Security audit model...")
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
        parsed = self._extract_json(raw_text, correlation_id)
        await self.publish_task_event(job_id, "SecurityAnalysis", "AI reasoning complete. Parsed response successfully.")
        
        parsed_data = parsed.get("data", parsed)
        parsed_data["confidence_meta"] = {
            "confidence_score": 95.0,
            "completeness_ratio": round(meta.get("coverage_pct", 100.0) / 100.0, 2),
            "evidence_coverage_count": len(parsed_data.get("findings", []))
        }

        return {
            "data": parsed_data,
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": "Security scan complete."
                }
            ]
        }

    @trace_stage("RepositorySummary")
    async def analyze_summary(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        await self.publish_task_event(job_id, "RepositorySummary", "Compiling repository narrative summary and suggestions...")
        if not meta.get("is_cloned"):
            await self.publish_task_event(job_id, "RepositorySummary", "Ecosystem evaluation only (repo is a fork with no contributions).")
            return {
                "data": {
                    "recruiter_summary": "This repository is a fork of the parent codebase with no detected user contributions.",
                    "top_strengths": [{"strength": "Ecosystem Familiarity", "rationale": "Familiarity with the parent codebase ecosystem.", "evidence": ["Forked metadata"]}],
                    "limitations": [{"limitation": "No Direct Contributions", "rationale": "No direct code modifications were verified.", "evidence": ["No commits on parent"]}],
                    "confidence_meta": {
                        "confidence_score": 100.0,
                        "completeness_ratio": 1.0,
                        "evidence_coverage_count": 0
                    }
                },
                "telemetry": None,
                "events": []
            }

        files_str = "".join([f"--- FILE: {name} ---\n{content}\n\n" for name, content in zip(sample.file_names, sample.file_content)])
        input_payload = {
            "repo_name": meta.get("repo_name"),
            "repo_owner": meta.get("repo_owner"),
            "technologies": meta.get("technologies", []),
            "files_str": files_str
        }
        system_prompt = self.prompt_factory.get_system_prompt()
        user_prompt = self.prompt_factory.get_summary_user_prompt(input_payload)
        
        await self.publish_task_event(job_id, "RepositorySummary", "Invoking AI Narrative engine...")
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
        parsed = self._extract_json(raw_text, correlation_id)
        await self.publish_task_event(job_id, "RepositorySummary", "AI reasoning complete. Parsed response successfully.")
        
        parsed_data = parsed.get("data", parsed)
        parsed_data["confidence_meta"] = {
            "confidence_score": 85.0,
            "completeness_ratio": round(meta.get("coverage_pct", 100.0) / 100.0, 2),
            "evidence_coverage_count": len(parsed_data.get("top_strengths", [])) + len(parsed_data.get("limitations", []))
        }

        return {
            "data": parsed_data,
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": "Narrative summary evaluation complete."
                }
            ]
        }

    @trace_stage("RepositoryClassification")
    async def analyze_classification(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        await self.publish_task_event(job_id, "RepositoryClassification", "Classifying repository's semantic domain...")
        
        if not meta.get("is_cloned"):
            await self.publish_task_event(job_id, "RepositoryClassification", "Ecosystem evaluation only (repo is a fork with no contributions).")
            result_data = {
                "primary_type": "Unknown",
                "all_types": [],
                "confidence": 1.0,
                "evidence": ["Repository is a fork with no contributions and has not been cloned for code analysis."],
                "schema_version": "1.0",
                "classifier_version": "2026.06",
                "confidence_meta": {
                    "confidence_score": 100.0,
                    "completeness_ratio": 1.0,
                    "evidence_coverage_count": 0
                }
            }
            return {
                "data": result_data,
                "telemetry": None,
                "events": [
                    {
                        "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                        "level": "Info",
                        "eventType": "StepCompleted",
                        "message": "Repository classification complete (Short-circuited)."
                    }
                ]
            }

        files_str = "".join([f"--- FILE: {name} ---\n{content}\n\n" for name, content in zip(sample.file_names, sample.file_content)])
        input_payload = {
            "repo_name": meta.get("repo_name"),
            "repo_owner": meta.get("repo_owner"),
            "technologies": meta.get("technologies", []),
            "files_str": files_str
        }
        system_prompt = self.prompt_factory.get_system_prompt()
        user_prompt = self.prompt_factory.get_classification_user_prompt(input_payload)
        
        await self.publish_task_event(job_id, "RepositoryClassification", "Invoking AI Repository Classification model...")
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
        parsed = self._extract_json(raw_text, correlation_id)
        await self.publish_task_event(job_id, "RepositoryClassification", "AI reasoning complete. Parsed response successfully.")
        
        parsed_data = parsed.get("data", parsed)
        parsed_data["confidence_meta"] = {
            "confidence_score": round(parsed_data.get("confidence", 0.8) * 100.0, 1),
            "completeness_ratio": round(meta.get("coverage_pct", 100.0) / 100.0, 2),
            "evidence_coverage_count": len(parsed_data.get("evidence", []))
        }

        return {
            "data": parsed_data,
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": f"Repository classified as: {parsed_data.get('primary_type', 'Unknown')}."
                }
            ]
        }

    @trace_stage("CvSynthesis")
    async def analyze_cv_synthesis(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        meta, sample = await self._get_meta_and_sample(job_id, encrypted_token, correlation_id)
        await self.publish_task_event(job_id, "CvSynthesis", "Synthesizing professional CV content from repository intelligence...")

        # Setup paths
        temp_dir_base = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones"))
        job_dir = os.path.join(temp_dir_base, job_id)

        # 1. Deterministic inputs from meta and local cache files
        repo_name = meta.get("repo_name", "unknown")
        classification = "Unknown"
        skills = []
        user_commit_ratio = 1.0
        total_commits = 1
        general_repo_summary = ""
        findings = []
        ownership_explanation = ""

        # Read RepositoryClassification result
        class_file = os.path.join(job_dir, "RepositoryClassification_result.json")
        if os.path.exists(class_file):
            try:
                with open(class_file, "r", encoding="utf-8") as f:
                    class_data = json.load(f)
                    classification = class_data.get("primary_type", classification)
            except Exception as e:
                logger.warning(f"Failed to read classification cache: {e}", extra={"correlation_id": correlation_id})

        # Read SkillExtraction result
        skills_file = os.path.join(job_dir, "SkillExtraction_result.json")
        if os.path.exists(skills_file):
            try:
                with open(skills_file, "r", encoding="utf-8") as f:
                    skills_data = json.load(f)
                    skills = [s.get("skill") for s in skills_data.get("skills", []) if s.get("skill")]
            except Exception as e:
                logger.warning(f"Failed to read skills cache: {e}", extra={"correlation_id": correlation_id})

        # Fallback to technologies from meta if skills are empty
        if not skills:
            skills = meta.get("technologies", [])

        # Read CommitIntelligence result
        commits_file = os.path.join(job_dir, "CommitIntelligence_result.json")
        if os.path.exists(commits_file):
            try:
                with open(commits_file, "r", encoding="utf-8") as f:
                    commits_data = json.load(f)
                    ownership = commits_data.get("ownership", {})
                    user_commit_ratio = ownership.get("user_commit_ratio", user_commit_ratio)
                    total_commits = ownership.get("total_commits", total_commits)
                    ownership_explanation = ownership.get("explanation", "")
            except Exception as e:
                logger.warning(f"Failed to read commits cache: {e}", extra={"correlation_id": correlation_id})

        # Read RepositorySummary result
        summary_file = os.path.join(job_dir, "RepositorySummary_result.json")
        if os.path.exists(summary_file):
            try:
                with open(summary_file, "r", encoding="utf-8") as f:
                    summary_data = json.load(f)
                    general_repo_summary = summary_data.get("recruiter_summary", "")
                    for strength in summary_data.get("top_strengths", []):
                        if strength.get("strength"):
                            findings.append({"finding": strength.get("strength"), "impact": "positive"})
                    for limitation in summary_data.get("limitations", []):
                        if limitation.get("limitation"):
                            findings.append({"finding": limitation.get("limitation"), "impact": "warning"})
            except Exception as e:
                logger.warning(f"Failed to read summary cache: {e}", extra={"correlation_id": correlation_id})

        # Deterministic ownership profile mapping
        if user_commit_ratio >= 0.70:
            ownership_profile = "High contribution profile"
        elif user_commit_ratio >= 0.20:
            ownership_profile = "Standard contribution profile"
        elif user_commit_ratio >= 0.05:
            ownership_profile = "Low contribution profile"
        else:
            ownership_profile = "External contributor context"

        # Safe default values builder function (no LLM fallback path)
        def compile_deterministic_fallback() -> dict:
            fallback_title = f"{classification} Developer" if classification != "Unknown" else "Software Developer"
            fallback_highlights = [{"signal": f.get("finding", ""), "impact": f.get("impact", "positive")} for f in findings]
            if not fallback_highlights:
                fallback_highlights = [{"signal": "Contributed to repository codebase.", "impact": "positive"}]
            return {
                "schemaVersion": "v2",
                "title": fallback_title,
                "skills": skills,
                "summary": f"Verified codebase contributions targeting a {classification} application.",
                "highlights": fallback_highlights,
                "ownershipProfile": ownership_profile
            }

        # Short-circuit logic for forks with no contributions
        if meta.get("repo_type") == "FORK_NO_CONTRIBUTION":
            await self.publish_task_event(job_id, "CvSynthesis", "Ecosystem familiarity evaluation only. Using deterministic fallback.")
            result_data = compile_deterministic_fallback()
            result_data["summary"] = "Repository is a fork with no detected direct developer contributions."
            result_data["ownershipProfile"] = "External contributor context"
            return {
                "data": result_data,
                "telemetry": None,
                "events": []
            }

        # Prepare input payload for Claude (excluding general summary to prevent leakage)
        input_payload = {
            "repo_name": repo_name,
            "classification": classification,
            "skills": skills,
            "ownershipProfile": ownership_profile,
            "ownership_explanation": ownership_explanation,
            "findings": findings
        }

        system_prompt = self.cv_prompt_factory.get_system_prompt()
        user_prompt = self.cv_prompt_factory.get_user_prompt(input_payload)

        # Single-retry orchestration loop
        parsed_data = None
        telemetry = None
        max_attempts = 2
        attempt = 0

        while attempt < max_attempts and parsed_data is None:
            attempt += 1
            await self.publish_task_event(job_id, "CvSynthesis", f"Invoking AI CV Synthesis model (Attempt {attempt})...")
            try:
                raw_text, attempt_telemetry = await self.claude_service.analyze_repo_with_telemetry(system_prompt, user_prompt, correlation_id)
                telemetry = attempt_telemetry
                parsed = self._extract_json(raw_text, correlation_id)
                
                # Validation check using Pydantic model (enforces hard validation boundaries: [100, 550])
                CvSynthesisContract.model_validate(parsed)
                cv_summary = parsed.get("summary", "")

                # Similarity guard using SequenceMatcher
                import difflib
                similarity = difflib.SequenceMatcher(None, general_repo_summary.lower(), cv_summary.lower()).ratio()

                # Soft warning range: Target is [250, 450] characters.
                is_invalid_length = not (250 <= len(cv_summary) <= 450)
                is_too_similar = similarity > 0.6

                if (is_invalid_length or is_too_similar) and attempt < max_attempts:
                    reasons = []
                    if is_invalid_length:
                        reasons.append(f"length of {len(cv_summary)} characters is outside target range [250, 450]")
                    if is_too_similar:
                        reasons.append(f"similarity ratio of {similarity:.2f} is too high (> 0.6)")
                    
                    err_msg = f"Soft validation warning: " + " and ".join(reasons)
                    logger.warning(err_msg, extra={"correlation_id": correlation_id})
                    raise ValueError(err_msg)

                # Log warnings on final attempt if minor violations remain but accept it
                if is_invalid_length or is_too_similar:
                    reasons = []
                    if is_invalid_length:
                        reasons.append(f"length {len(cv_summary)} is outside [250, 450]")
                    if is_too_similar:
                        reasons.append(f"similarity {similarity:.2f} is high")
                    logger.warning(
                        f"CV Synthesis final attempt accepted with minor violations: " + ", ".join(reasons),
                        extra={"correlation_id": correlation_id}
                    )

                parsed_data = parsed
            except Exception as e:
                logger.warning(f"CV Synthesis attempt {attempt} failed validation: {e}", extra={"correlation_id": correlation_id})
                if attempt < max_attempts:
                    # Append error details to user prompt for self-correction retry
                    user_prompt += f"\n\nYOUR PREVIOUS RESPONSE FAILED VALIDATION: {str(e)}\n"
                    user_prompt += "Please fix the structure, adjust length to 250-450 characters, and ensure CV summary uses completely different phrasing than any general summary. Return ONLY valid JSON."
                else:
                    await self.publish_task_event(job_id, "CvSynthesis", "Validation failed twice. Falling back to deterministic builder.", "Warning")

        if parsed_data is None:
            # Execute deterministic fallback builder
            parsed_data = compile_deterministic_fallback()

        # Enforce deterministic constraints over LLM response to ensure total accuracy
        parsed_data["schemaVersion"] = "v2"
        parsed_data["skills"] = skills
        parsed_data["ownershipProfile"] = ownership_profile
        parsed_data["title"] = f"{classification} Developer" if classification != "Unknown" else "Software Developer"

        return {
            "data": parsed_data,
            "telemetry": telemetry,
            "events": [
                {
                    "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                    "level": "Info",
                    "eventType": "StepCompleted",
                    "message": "CV Synthesis complete."
                }
            ]
        }

    # ── Shared helper ────────────────────────────────────────────────────────

    def _read_meta(self, job_id: str) -> dict:
        meta_path = os.path.join(
            os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones")),
            job_id, "meta.json"
        )
        if not os.path.exists(meta_path):
            raise Exception("Workspace metadata not found. RepoStructure / L1-001 must run first.")
        with open(meta_path, "r", encoding="utf-8") as f:
            return json.load(f)

    def _read_task_cache(self, job_id: str, task_type: str) -> dict:
        cache_path = os.path.join(
            os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones")),
            job_id, f"{task_type}_result.json"
        )
        if not os.path.exists(cache_path):
            return {}
        try:
            with open(cache_path, "r", encoding="utf-8") as f:
                return json.load(f)
        except Exception:
            return {}

    def _clone_dir(self, job_id: str) -> str:
        return os.path.join(
            os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones")),
            job_id, "repo"
        )

    def _empty_result(self, data: dict) -> dict:
        return {"data": data, "telemetry": None, "events": []}

    # ── File path → complexity level taxonomy (research doc §5.3) ────────────

    # Ordered by specificity: most specific first so that overlapping patterns
    # resolve to the highest applicable level.
    _COMPLEXITY_PATTERNS: list[tuple[list[str], str, int]] = [
        # (path fragments, capability label, L-level)
        (["k8s", "kubernetes", "helm", "service-mesh", "istio", "operator"], "Platform Engineering", 6),
        (["saga", "cqrs", "event-sourcing", "distributed", "choreography"], "Distributed Systems", 5),
        (["kafka", "rabbitmq", "queue", "messaging", "pubsub", "event-bus"], "Event-Driven Architecture", 5),
        (["terraform", "ansible", "pulumi", "iac"], "Infrastructure as Code", 5),
        (["rbac", "abac", "permission-graph", "policy"], "Access Control & Policy", 4),
        (["oauth", "oidc", "openid", "sso"], "OAuth / OIDC", 4),
        (["payment", "billing", "checkout", "stripe", "paypal"], "Payment Processing", 4),
        (["graphql", "grpc", "protobuf", "thrift"], "Advanced API Design", 4),
        (["auth", "jwt", "token", "session", "security", "encrypt", "crypto"], "Authentication & Security", 3),
        (["migration", "schema", "flyway", "liquibase"], "Database Management", 3),
        (["monitoring", "observability", "tracing", "metrics", "prometheus", "grafana"], "System Observability", 3),
        (["docker", "container", "compose"], "Containerization", 3),
        (["ci", "cd", "pipeline", "workflow", "github/workflows", "jenkins"], "CI/CD", 3),
        (["test", "spec", "__test__", "unittest", "integration-test", "e2e"], "Testing & Quality", 2),
        (["order", "cart", "catalog", "product", "inventory", "user"], "Business Domain CRUD", 2),
        (["controller", "router", "endpoint", "handler", "api"], "API Endpoint", 2),
    ]

    def _classify_file_complexity(self, filepath: str) -> tuple[str, int]:
        """Return (capability_label, L_level) for a file path. Default is L1 (trivial)."""
        fp_lower = filepath.lower().replace("\\", "/")
        for fragments, label, level in self._COMPLEXITY_PATTERNS:
            if any(frag in fp_lower for frag in fragments):
                return label, level
        return "General Code", 1

    # ── L1-003 CommitDiff ────────────────────────────────────────────────────

    @trace_stage("CommitDiff")
    async def analyze_commit_diff(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Diff-First pipeline: parse git diffs and map file paths to capability signals.
        Intentionally avoids using commit messages as the primary signal (§3.6)."""
        import subprocess, re
        extra_log = {"correlation_id": correlation_id}
        meta = self._read_meta(job_id)
        clone_dir = self._clone_dir(job_id)

        if not meta.get("is_cloned") or not os.path.exists(os.path.join(clone_dir, ".git")):
            await self.publish_task_event(job_id, "CommitDiff", "No cloned repo — skipping diff parsing.")
            return self._empty_result({"commits": [], "total_parsed": 0, "capability_signals": []})

        await self.publish_task_event(job_id, "CommitDiff", "Reading commit hashes (top 50 non-merge commits)...")

        log_proc = await asyncio.to_thread(subprocess.run,
            ["git", "log", "--no-merges", "--format=%H|%ae|%s", "-n", "50"],
            cwd=clone_dir, capture_output=True, text=True, errors="ignore"
        )
        commit_entries = []
        if log_proc.returncode == 0:
            for line in log_proc.stdout.strip().split("\n"):
                if "|" in line:
                    parts = line.split("|", 2)
                    commit_entries.append({
                        "hash": parts[0].strip(),
                        "email": parts[1].strip() if len(parts) > 1 else "",
                        "message": parts[2].strip() if len(parts) > 2 else ""
                    })

        analyzed_commits = []
        all_signals: dict[str, int] = {}

        for entry in commit_entries[:30]:
            h = entry["hash"]

            # Files changed (structural diff — not message)
            files_proc = await asyncio.to_thread(subprocess.run,
                ["git", "diff-tree", "--no-commit-id", "-r", "--name-only", h],
                cwd=clone_dir, capture_output=True, text=True, errors="ignore"
            )
            if files_proc.returncode != 0:
                continue
            files_changed = [f.strip() for f in files_proc.stdout.strip().split("\n") if f.strip()]

            # Diff line-count stats
            stat_proc = await asyncio.to_thread(subprocess.run,
                ["git", "diff-tree", "--no-commit-id", "-r", "--shortstat", h],
                cwd=clone_dir, capture_output=True, text=True, errors="ignore"
            )
            stat_line = stat_proc.stdout.strip()
            lines_added = int((re.search(r"(\d+) insertion", stat_line) or ["", 0])[1] or 0)
            lines_deleted = int((re.search(r"(\d+) deletion", stat_line) or ["", 0])[1] or 0)

            # Map files → capability (Diff-First, §3.6 approach)
            commit_capabilities = []
            seen_caps: set[str] = set()
            for f in files_changed:
                cap_label, level = self._classify_file_complexity(f)
                if cap_label not in seen_caps and cap_label != "General Code":
                    seen_caps.add(cap_label)
                    commit_capabilities.append({"capability": cap_label, "complexity_level": f"L{level}", "evidence_file": f})
                    all_signals[cap_label] = all_signals.get(cap_label, 0) + 1

            # Infer type from structural patterns only (not message text)
            inferred_type = "feature"
            if any(any(p in f.lower() for p in ["test", "spec", "__test__"]) for f in files_changed):
                inferred_type = "test"
            elif any(any(p in f.lower() for p in ["migration", "schema"]) for f in files_changed):
                inferred_type = "db_migration"
            elif lines_added == 0 and lines_deleted > 5:
                inferred_type = "cleanup"
            elif lines_added > 0 and lines_deleted > lines_added * 0.8:
                inferred_type = "refactor"

            # Cross-validate message vs inferred type (flag mismatch, §3.6)
            msg_lower = entry["message"].lower()
            message_type_hint = (
                "bugfix" if any(w in msg_lower for w in ["fix", "bug", "hotfix", "patch"]) else
                "refactor" if any(w in msg_lower for w in ["refactor", "cleanup", "clean up", "chore"]) else
                "feature" if any(w in msg_lower for w in ["feat", "add", "implement", "new"]) else
                "unknown"
            )
            intent_conflict = message_type_hint != "unknown" and message_type_hint != inferred_type

            analyzed_commits.append({
                "hash": h[:8],
                "email": entry["email"],
                "message": entry["message"],
                "files_changed": files_changed[:10],
                "lines_added": lines_added,
                "lines_deleted": lines_deleted,
                "capabilities": commit_capabilities[:5],
                "inferred_type": inferred_type,
                "message_type_hint": message_type_hint,
                "intent_conflict": intent_conflict,
            })

        capability_signals = sorted(
            [{"capability": cap, "frequency": cnt} for cap, cnt in all_signals.items()],
            key=lambda x: x["frequency"], reverse=True
        )

        await self.publish_task_event(job_id, "CommitDiff",
            f"Parsed {len(analyzed_commits)} commits — {len(capability_signals)} capability signals detected.")

        return {
            "data": {
                "commits": analyzed_commits,
                "total_parsed": len(analyzed_commits),
                "capability_signals": capability_signals,
                "approach": "diff_first",
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"CommitDiff complete. {len(capability_signals)} capabilities mapped from diffs."
            }]
        }

    # ── L1-007 CommitTimeline ────────────────────────────────────────────────

    @trace_stage("CommitTimeline")
    async def analyze_commit_timeline(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Temporal analysis: commit frequency, working patterns, evolution signals (§3.5)."""
        import subprocess, re
        from datetime import datetime, timezone
        meta = self._read_meta(job_id)
        clone_dir = self._clone_dir(job_id)

        if not meta.get("is_cloned") or not os.path.exists(os.path.join(clone_dir, ".git")):
            return self._empty_result({"commit_frequency_score": 0, "working_patterns": {}, "timeline_signals": []})

        await self.publish_task_event(job_id, "CommitTimeline", "Extracting commit timestamps for temporal analysis...")

        log_proc = await asyncio.to_thread(subprocess.run,
            ["git", "log", "--no-merges", "--format=%aI|%ae|%s"],
            cwd=clone_dir, capture_output=True, text=True, errors="ignore"
        )

        timestamps: list[datetime] = []
        authors: list[str] = []
        messages: list[str] = []

        if log_proc.returncode == 0:
            for line in log_proc.stdout.strip().split("\n"):
                if "|" not in line:
                    continue
                parts = line.split("|", 2)
                try:
                    dt = datetime.fromisoformat(parts[0].strip().replace("Z", "+00:00"))
                    timestamps.append(dt)
                    authors.append(parts[1].strip() if len(parts) > 1 else "")
                    messages.append(parts[2].strip() if len(parts) > 2 else "")
                except ValueError:
                    continue

        if not timestamps:
            return self._empty_result({"commit_frequency_score": 0, "working_patterns": {}, "timeline_signals": []})

        total = len(timestamps)
        # Commit span in days
        span_days = max(1, (timestamps[0] - timestamps[-1]).days)
        commits_per_week = round(total / max(1, span_days / 7), 2)

        # Working hour distribution (UTC hour buckets 0-23)
        hour_dist: dict[int, int] = {}
        weekday_dist: dict[int, int] = {}
        for dt in timestamps:
            hour_dist[dt.hour] = hour_dist.get(dt.hour, 0) + 1
            weekday_dist[dt.weekday()] = weekday_dist.get(dt.weekday(), 0) + 1

        peak_hour = max(hour_dist, key=hour_dist.__getitem__, default=12)
        peak_weekday = max(weekday_dist, key=weekday_dist.__getitem__, default=1)
        weekday_names = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]

        # Burst detection: >10 commits in a single day
        day_counts: dict[str, int] = {}
        for dt in timestamps:
            day_key = dt.strftime("%Y-%m-%d")
            day_counts[day_key] = day_counts.get(day_key, 0) + 1
        burst_days = [day for day, cnt in day_counts.items() if cnt > 10]

        # Commit type ratios from messages (secondary cross-validation)
        msg_lower_list = [m.lower() for m in messages]
        bugfix_count = sum(1 for m in msg_lower_list if any(w in m for w in ["fix", "bug", "patch", "hotfix"]))
        refactor_count = sum(1 for m in msg_lower_list if any(w in m for w in ["refactor", "cleanup", "chore"]))
        feature_count = sum(1 for m in msg_lower_list if any(w in m for w in ["feat", "add", "implement", "new"]))

        bug_to_fix_ratio = round(bugfix_count / max(1, total), 3)
        refactor_initiative = round(refactor_count / max(1, total), 3)

        # Commit frequency score: reward consistent cadence, penalise single-burst
        consistency_penalty = min(30.0, len(burst_days) * 5.0)
        base_freq = min(100.0, commits_per_week * 10)
        commit_frequency_score = round(max(0.0, base_freq - consistency_penalty), 1)

        # Feature complexity growth: rough proxy — ratio of commits touching >5 files
        multi_file_ratio = 0.0  # populated later by CommitDiff if available
        diff_cache = self._read_task_cache(job_id, "CommitDiff")
        if diff_cache.get("commits"):
            multi_file = sum(1 for c in diff_cache["commits"] if len(c.get("files_changed", [])) > 5)
            multi_file_ratio = round(multi_file / max(1, len(diff_cache["commits"])), 3)

        timeline_signals = [
            {"signal": "commit_frequency_score", "value": commit_frequency_score,
             "note": f"{commits_per_week:.1f} commits/week over {span_days} days"},
            {"signal": "bug_to_fix_ratio", "value": bug_to_fix_ratio,
             "note": f"{bugfix_count}/{total} commits are bug-fixes"},
            {"signal": "refactor_initiative", "value": refactor_initiative,
             "note": f"{refactor_count}/{total} commits are voluntary refactors"},
            {"signal": "multi_file_commit_ratio", "value": multi_file_ratio,
             "note": "Proxy for architectural / cross-cutting commits"},
            {"signal": "burst_days_count", "value": len(burst_days),
             "note": f"Days with >10 commits: {burst_days[:3]}"},
        ]

        await self.publish_task_event(job_id, "CommitTimeline",
            f"Timeline: {total} commits over {span_days}d. Freq score={commit_frequency_score}. BugRatio={bug_to_fix_ratio}.")

        return {
            "data": {
                "commit_frequency_score": commit_frequency_score,
                "commits_per_week": commits_per_week,
                "span_days": span_days,
                "total_commits": total,
                "bug_to_fix_ratio": bug_to_fix_ratio,
                "refactor_initiative": refactor_initiative,
                "working_patterns": {
                    "peak_hour_utc": peak_hour,
                    "peak_weekday": weekday_names[peak_weekday],
                    "hour_distribution": {str(h): c for h, c in sorted(hour_dist.items())},
                    "weekday_distribution": {weekday_names[d]: c for d, c in sorted(weekday_dist.items())},
                },
                "burst_days": burst_days[:5],
                "timeline_signals": timeline_signals,
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"Commit timeline analysis complete. {len(timeline_signals)} signals extracted."
            }]
        }

    # ── L1-009 CommitIntent ──────────────────────────────────────────────────

    @trace_stage("CommitIntent")
    async def analyze_commit_intent(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """AI inference of developer intent from diff content only (Diff-First §3.6)."""
        meta = self._read_meta(job_id)
        if not meta.get("is_cloned"):
            return self._empty_result({"intents": [], "dominant_intent": "unknown", "capability_profile": []})

        diff_cache = self._read_task_cache(job_id, "CommitDiff")
        commits = diff_cache.get("commits", [])
        cap_signals = diff_cache.get("capability_signals", [])

        if not commits:
            return self._empty_result({"intents": [], "dominant_intent": "unknown", "capability_profile": cap_signals})

        await self.publish_task_event(job_id, "CommitIntent", "Inferring developer intent from diff patterns (AI)...")

        # Compact diff summary for LLM — capped to avoid token bloat
        diff_summary_lines = []
        for c in commits[:15]:
            files = ", ".join(c.get("files_changed", [])[:5])
            caps = ", ".join(cap.get("capability", "") for cap in c.get("capabilities", []))
            inferred = c.get("inferred_type", "feature")
            diff_summary_lines.append(
                f"- [{inferred}] +{c.get('lines_added',0)}/-{c.get('lines_deleted',0)} lines | files: {files} | caps: {caps or 'none'}"
            )
        diff_summary = "\n".join(diff_summary_lines)

        top_caps = ", ".join(s["capability"] for s in cap_signals[:8])
        user_prompt = (
            f"Repository: {meta.get('repo_owner','?')}/{meta.get('repo_name','?')}\n"
            f"Tech stack: {', '.join(meta.get('technologies', []))}\n\n"
            f"COMMIT DIFF SUMMARY (top 15 commits, classified from file paths — NOT from messages):\n{diff_summary}\n\n"
            f"Detected capability signals (by frequency): {top_caps}\n\n"
            "Based ONLY on the file paths, lines changed, and capability signals above "
            "(ignore commit messages — they are unreliable), produce a JSON report:\n"
            "{\n"
            '  "dominant_intent": "<feature_builder|system_designer|problem_solver|maintenance|performance|research>",\n'
            '  "confidence": <0.0-1.0>,\n'
            '  "capability_profile": [{"capability": "...", "evidence_strength": "strong|moderate|weak"}],\n'
            '  "engineering_maturity_signals": ["..."],\n'
            '  "intent_conflict_flags": ["<hash>: message says X but diff shows Y"]\n'
            "}"
        )

        system_prompt = self.prompt_factory.get_system_prompt()
        raw_text, telemetry = await self.claude_service.analyze_repo_with_telemetry(
            system_prompt, user_prompt, correlation_id
        )
        parsed = self._extract_json(raw_text, correlation_id)

        await self.publish_task_event(job_id, "CommitIntent", "CommitIntent inference complete.")
        return {
            "data": {
                "intents": parsed.get("capability_profile", []),
                "dominant_intent": parsed.get("dominant_intent", "feature_builder"),
                "confidence": parsed.get("confidence", 0.7),
                "capability_profile": parsed.get("capability_profile", cap_signals),
                "engineering_maturity_signals": parsed.get("engineering_maturity_signals", []),
                "intent_conflict_flags": parsed.get("intent_conflict_flags", []),
            },
            "telemetry": telemetry,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"CommitIntent analysis complete. Dominant: {parsed.get('dominant_intent', 'unknown')}."
            }]
        }

    # ── L1-010 Complexity ────────────────────────────────────────────────────

    @trace_stage("Complexity")
    async def analyze_complexity(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Classify repository files into 6-level complexity taxonomy (§5.3)."""
        meta = self._read_meta(job_id)
        filenames = meta.get("filenames", [])

        level_counts = {f"L{i}": 0 for i in range(1, 7)}
        level_examples: dict[str, list[str]] = {f"L{i}": [] for i in range(1, 7)}
        total_power_score = 0.0

        # Power score ranges per level (mid-point used as representative value)
        LEVEL_POWER = {"L1": 3, "L2": 20, "L3": 100, "L4": 275, "L5": 750, "L6": 2000}

        for fname in filenames:
            _, level_int = self._classify_file_complexity(fname)
            key = f"L{level_int}"
            level_counts[key] += 1
            if len(level_examples[key]) < 3:
                level_examples[key].append(fname)
            total_power_score += LEVEL_POWER.get(key, 3)

        total_files = max(1, sum(level_counts.values()))
        level_distribution = {
            k: {"count": v, "pct": round(v / total_files * 100, 1), "examples": level_examples[k]}
            for k, v in level_counts.items()
        }

        # Dominant complexity tier
        dominant_level = max(level_counts, key=lambda k: level_counts[k] * int(k[1]))

        # Trivial file ratio (§5.4 — L1 capped at 5% of Power Score)
        l1_power = level_counts["L1"] * LEVEL_POWER["L1"]
        l1_ratio = round(l1_power / max(1.0, total_power_score), 3)
        trivial_capped = l1_ratio > 0.05

        await self.publish_task_event(job_id, "Complexity",
            f"Complexity taxonomy: dominant={dominant_level}, total_power={total_power_score:.0f}, L1_ratio={l1_ratio:.1%}.")

        return {
            "data": {
                "dominant_level": dominant_level,
                "level_distribution": level_distribution,
                "total_power_score": round(total_power_score, 1),
                "trivial_file_ratio": l1_ratio,
                "trivial_capped": trivial_capped,
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"Complexity analysis complete. Dominant tier: {dominant_level}. Power score: {total_power_score:.0f}."
            }]
        }

    # ── L1-012 GitBlame ──────────────────────────────────────────────────────

    @trace_stage("GitBlame")
    async def analyze_git_blame(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Per-file authorship via git blame on the most-changed files (§4.2 cregit approach)."""
        import subprocess
        meta = self._read_meta(job_id)
        clone_dir = self._clone_dir(job_id)

        if not meta.get("is_cloned") or not os.path.exists(os.path.join(clone_dir, ".git")):
            return self._empty_result({"file_authorship": [], "overall_author_ratio": 0.0})

        await self.publish_task_event(job_id, "GitBlame", "Running git blame on top-changed files...")

        # Find top 10 most-changed files
        log_proc = await asyncio.to_thread(subprocess.run,
            ["git", "log", "--no-merges", "--name-only", "--format=", "-n", "200"],
            cwd=clone_dir, capture_output=True, text=True, errors="ignore"
        )
        file_freq: dict[str, int] = {}
        if log_proc.returncode == 0:
            for line in log_proc.stdout.strip().split("\n"):
                line = line.strip()
                if line and not line.startswith("diff"):
                    file_freq[line] = file_freq.get(line, 0) + 1

        top_files = sorted(file_freq, key=file_freq.__getitem__, reverse=True)[:10]

        # Get committer identity (email-based matching)
        user_email = meta.get("user_email", "").lower().strip()
        username = meta.get("username", "").lower().strip()

        def is_user_author(author_email: str, author_name: str) -> bool:
            e = author_email.lower().strip()
            n = author_name.lower().strip()
            if user_email and e == user_email:
                return True
            if username and (username in n or n in username):
                return True
            return False

        file_authorship = []
        total_lines = 0
        user_lines = 0

        for rel_path in top_files:
            abs_path = os.path.join(clone_dir, rel_path.replace("/", os.sep))
            if not os.path.isfile(abs_path):
                continue

            blame_proc = await asyncio.to_thread(subprocess.run,
                ["git", "blame", "--line-porcelain", rel_path],
                cwd=clone_dir, capture_output=True, text=True, errors="ignore", timeout=15
            )
            if blame_proc.returncode != 0:
                continue

            author_lines: dict[str, int] = {}
            cur_email = ""
            cur_name = ""
            for bline in blame_proc.stdout.split("\n"):
                if bline.startswith("author ") and not bline.startswith("author-"):
                    cur_name = bline[7:].strip()
                elif bline.startswith("author-mail "):
                    cur_email = bline[12:].strip().strip("<>")
                elif bline.startswith("\t"):  # actual code line
                    key = cur_email or cur_name
                    author_lines[key] = author_lines.get(key, 0) + 1
                    total_lines += 1
                    if is_user_author(cur_email, cur_name):
                        user_lines += 1

            file_total = max(1, sum(author_lines.values()))
            top_author = max(author_lines, key=author_lines.__getitem__, default="unknown")
            file_authorship.append({
                "file": rel_path,
                "total_lines": file_total,
                "user_lines": author_lines.get(
                    next((k for k in author_lines if is_user_author(
                        k if "@" in k else "", k if "@" not in k else ""
                    )), ""), 0
                ),
                "top_author": top_author,
                "author_count": len(author_lines),
            })

        overall_author_ratio = round(user_lines / max(1, total_lines), 4)
        await self.publish_task_event(job_id, "GitBlame",
            f"Git blame on {len(file_authorship)} files. User lines: {user_lines}/{total_lines} ({overall_author_ratio:.1%}).")

        return {
            "data": {
                "file_authorship": file_authorship,
                "overall_author_ratio": overall_author_ratio,
                "total_lines_blamed": total_lines,
                "user_lines_blamed": user_lines,
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"Git blame authorship complete. Author ratio: {overall_author_ratio:.1%}."
            }]
        }

    # ── L1-013 CloneDetection ────────────────────────────────────────────────

    @trace_stage("CloneDetection")
    async def analyze_clone_detection(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Heuristic detection of tutorial clones, commit dumps, and fork inflation (§6.5)."""
        import subprocess
        meta = self._read_meta(job_id)
        clone_dir = self._clone_dir(job_id)

        flags: list[str] = []
        risk_score = 0.0

        # Already classified as fork by repo_classifier
        if meta.get("repo_type") in ("FORK_NO_CONTRIBUTION", "FORK_UPSTREAM_CONTRIBUTION"):
            flags.append("known_fork")
            risk_score += 30.0

        if meta.get("is_cloned") and os.path.exists(os.path.join(clone_dir, ".git")):
            # Heuristic 1: single large initial commit (tutorial clone)
            log_proc = await asyncio.to_thread(subprocess.run,
                ["git", "log", "--no-merges", "--format=%H", "--reverse"],
                cwd=clone_dir, capture_output=True, text=True, errors="ignore"
            )
            hashes = [h.strip() for h in log_proc.stdout.strip().split("\n") if h.strip()]
            if hashes:
                first_stat = await asyncio.to_thread(subprocess.run,
                    ["git", "diff-tree", "--no-commit-id", "-r", "--shortstat", hashes[0]],
                    cwd=clone_dir, capture_output=True, text=True, errors="ignore"
                )
                import re
                first_added = int((re.search(r"(\d+) insertion", first_stat.stdout) or ["", 0])[1] or 0)
                total_commits = len(hashes)

                if first_added > 1000 and total_commits <= 5:
                    flags.append("tutorial_clone_suspected")
                    risk_score += 50.0
                elif first_added > 500 and total_commits <= 3:
                    flags.append("large_initial_commit")
                    risk_score += 30.0

            # Heuristic 2: commit bomb (>100 commits in a single day)
            day_proc = await asyncio.to_thread(subprocess.run,
                ["git", "log", "--no-merges", "--format=%ad", "--date=short"],
                cwd=clone_dir, capture_output=True, text=True, errors="ignore"
            )
            day_counts: dict[str, int] = {}
            for d in day_proc.stdout.strip().split("\n"):
                d = d.strip()
                if d:
                    day_counts[d] = day_counts.get(d, 0) + 1
            max_day_commits = max(day_counts.values(), default=0)
            if max_day_commits > 100:
                flags.append(f"commit_bomb_detected:{max_day_commits}_commits_in_one_day")
                risk_score += 40.0
            elif max_day_commits > 50:
                flags.append(f"high_commit_velocity:{max_day_commits}_commits_in_one_day")
                risk_score += 20.0

            # Heuristic 3: no development history (single commit or empty after initial)
            if len(hashes) <= 2 and first_added > 200:
                flags.append("no_development_history")
                risk_score += 25.0

        risk_score = min(100.0, risk_score)
        classification = "clean" if risk_score < 25 else "suspicious" if risk_score < 60 else "high_risk"

        await self.publish_task_event(job_id, "CloneDetection",
            f"Clone detection: {classification} (score={risk_score:.0f}). Flags: {flags or ['none']}.")

        return {
            "data": {
                "clone_risk_score": round(risk_score, 1),
                "classification": classification,
                "flags": flags,
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info" if classification == "clean" else "Warning",
                "eventType": "StepCompleted",
                "message": f"Clone detection complete: {classification}."
            }]
        }

    # ── L1-014 AiGeneratedCode ───────────────────────────────────────────────

    @trace_stage("AiGeneratedCode")
    async def analyze_ai_generated_code(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Detect AI-generated code indicators: large single-burst commits, no revision history (§6.5)."""
        meta = self._read_meta(job_id)
        diff_cache = self._read_task_cache(job_id, "CommitDiff")
        commits = diff_cache.get("commits", [])

        flags: list[dict] = []
        ai_risk_score = 0.0

        # Signal 1: individual commits > 500 lines added in one shot
        large_single_commits = [
            c for c in commits
            if c.get("lines_added", 0) > 500 and c.get("lines_deleted", 0) < 50
        ]
        if large_single_commits:
            ai_risk_score += min(40.0, len(large_single_commits) * 15.0)
            for c in large_single_commits[:3]:
                flags.append({
                    "flag": "large_single_burst",
                    "commit": c["hash"],
                    "lines_added": c.get("lines_added", 0),
                    "note": "Large addition with minimal revision — potential AI dump"
                })

        # Signal 2: zero revision on large features (added, never touched again)
        if commits:
            file_touch_count: dict[str, int] = {}
            for c in commits:
                for f in c.get("files_changed", []):
                    file_touch_count[f] = file_touch_count.get(f, 0) + 1
            single_touch_large = [f for f, cnt in file_touch_count.items() if cnt == 1]
            single_touch_ratio = round(len(single_touch_large) / max(1, len(file_touch_count)), 3)
            if single_touch_ratio > 0.7:
                ai_risk_score += 20.0
                flags.append({
                    "flag": "high_single_touch_ratio",
                    "ratio": single_touch_ratio,
                    "note": f"{single_touch_ratio:.0%} of files touched only once — suggests copy-paste or AI generation"
                })

        # Signal 3: intent conflicts from CommitDiff (message ≠ diff)
        intent_conflicts = [c for c in commits if c.get("intent_conflict")]
        if len(intent_conflicts) > 3:
            ai_risk_score += min(20.0, len(intent_conflicts) * 4.0)
            flags.append({
                "flag": "intent_message_conflicts",
                "count": len(intent_conflicts),
                "note": "Commit messages don't match diff content — possible obfuscation"
            })

        ai_risk_score = min(100.0, ai_risk_score)
        risk_level = "low" if ai_risk_score < 25 else "medium" if ai_risk_score < 60 else "high"

        await self.publish_task_event(job_id, "AiGeneratedCode",
            f"AI code detection: {risk_level} ({ai_risk_score:.0f}). Flags: {len(flags)}.")

        return {
            "data": {
                "ai_risk_score": round(ai_risk_score, 1),
                "risk_level": risk_level,
                "flags": flags,
                "large_single_commits_count": len(large_single_commits),
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info" if risk_level == "low" else "Warning",
                "eventType": "StepCompleted",
                "message": f"AI generated code detection complete: {risk_level} risk."
            }]
        }

    # ── L1-015 Ownership ────────────────────────────────────────────────────

    @trace_stage("Ownership")
    async def analyze_ownership(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Aggregate ownership evidence from GitBlame + CommitHistory (§4.3 formula)."""
        meta = self._read_meta(job_id)
        blame_cache = self._read_task_cache(job_id, "GitBlame")
        commit_cache = self._read_task_cache(job_id, "CommitIntelligence")

        # Pull deterministic facts
        blame_ratio = blame_cache.get("overall_author_ratio", meta.get("user_commit_ratio", 1.0))
        commit_ratio = (commit_cache.get("ownership") or {}).get("user_commit_ratio", meta.get("user_commit_ratio", 1.0))
        total_commits = (commit_cache.get("ownership") or {}).get("total_commits", meta.get("total_commits", 1))
        bus_factor = (commit_cache.get("ownership") or {}).get("bus_factor", meta.get("bus_factor", 1))

        # Ownership Score formula (§4.3):
        # weighted average of blame-line ratio and commit ratio, penalised for high bus-factor
        raw_ownership = (blame_ratio * 0.6) + (commit_ratio * 0.4)
        bus_factor_penalty = min(0.3, max(0.0, (bus_factor - 2) * 0.05))
        ownership_score = round(max(0.0, raw_ownership - bus_factor_penalty), 4)

        # Module-level ownership from GitBlame file list
        module_ownership: dict[str, float] = {}
        for fa in blame_cache.get("file_authorship", []):
            f = fa.get("file", "")
            user_lines = fa.get("user_lines", 0)
            total_lines = max(1, fa.get("total_lines", 1))
            module = f.split("/")[0] if "/" in f else "root"
            module_ownership[module] = module_ownership.get(module, 0) + user_lines / total_lines

        # Normalize per-module
        module_count: dict[str, int] = {}
        for fa in blame_cache.get("file_authorship", []):
            module = (fa.get("file") or "root").split("/")[0]
            module_count[module] = module_count.get(module, 0) + 1
        normalized_module = {
            mod: round(module_ownership[mod] / module_count[mod], 4)
            for mod in module_ownership if module_count.get(mod, 0) > 0
        }

        is_primary = ownership_score >= 0.50

        await self.publish_task_event(job_id, "Ownership",
            f"Ownership score={ownership_score:.2f}. Primary author={is_primary}. Bus factor={bus_factor}.")

        return {
            "data": {
                "ownership_score": ownership_score,
                "is_primary_author": is_primary,
                "blame_line_ratio": blame_ratio,
                "commit_ratio": commit_ratio,
                "bus_factor": bus_factor,
                "total_commits": total_commits,
                "module_ownership": normalized_module,
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"Ownership score computed: {ownership_score:.2f}."
            }]
        }

    # ── L1-017 SkillGraph ────────────────────────────────────────────────────

    @trace_stage("SkillGraph")
    async def analyze_skill_graph(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Build Skill Evidence Graph: nodes=skills, edges=evidence links (§8.3 schema)."""
        meta = self._read_meta(job_id)
        if not meta.get("is_cloned"):
            return self._empty_result({"nodes": [], "edges": [], "skill_count": 0})

        diff_cache = self._read_task_cache(job_id, "CommitDiff")
        ownership_cache = self._read_task_cache(job_id, "Ownership")
        skill_cache = self._read_task_cache(job_id, "SkillExtraction")

        cap_signals = diff_cache.get("capability_signals", [])
        ownership_score = ownership_cache.get("ownership_score", meta.get("user_commit_ratio", 1.0))
        technologies = meta.get("technologies", [])
        extracted_skills = [s.get("skill") for s in skill_cache.get("skills", []) if s.get("skill")]

        # Merge capability signals + extracted skills + detected tech
        all_skills: dict[str, dict] = {}
        for tech in technologies:
            all_skills[tech] = {"source": "tech_detector", "frequency": 1, "evidence_strength": "moderate"}
        for s in extracted_skills:
            if s not in all_skills:
                all_skills[s] = {"source": "llm_extraction", "frequency": 1, "evidence_strength": "moderate"}
            else:
                all_skills[s]["evidence_strength"] = "strong"
        for cap in cap_signals:
            cap_name = cap.get("capability", "")
            if cap_name and cap_name not in all_skills:
                all_skills[cap_name] = {
                    "source": "diff_analysis",
                    "frequency": cap.get("frequency", 1),
                    "evidence_strength": "strong" if cap.get("frequency", 0) >= 3 else "moderate"
                }
            elif cap_name:
                all_skills[cap_name]["frequency"] = all_skills[cap_name].get("frequency", 0) + cap.get("frequency", 0)
                all_skills[cap_name]["evidence_strength"] = "strong"

        # Build graph nodes and edges
        nodes = [
            {
                "id": f"dev-{meta.get('repo_owner', 'dev')}",
                "type": "developer",
                "label": meta.get("repo_owner", "developer")
            },
            {
                "id": f"repo-{meta.get('repo_name', 'repo')}",
                "type": "repository",
                "label": f"{meta.get('repo_owner','?')}/{meta.get('repo_name','?')}"
            }
        ]
        edges = [{
            "source": f"dev-{meta.get('repo_owner', 'dev')}",
            "target": f"repo-{meta.get('repo_name', 'repo')}",
            "label": "CONTRIBUTED_TO",
            "weight": round(ownership_score, 3)
        }]

        for skill_name, attrs in all_skills.items():
            node_id = f"skill-{skill_name.lower().replace(' ', '-')}"
            nodes.append({
                "id": node_id,
                "type": "skill",
                "label": skill_name,
                "evidence_strength": attrs["evidence_strength"],
                "source": attrs["source"],
                "frequency": attrs.get("frequency", 1)
            })
            edges.append({
                "source": f"dev-{meta.get('repo_owner', 'dev')}",
                "target": node_id,
                "label": "DEMONSTRATES",
                "weight": round(ownership_score * (1.0 if attrs["evidence_strength"] == "strong" else 0.6), 3)
            })

        await self.publish_task_event(job_id, "SkillGraph",
            f"Skill Evidence Graph built: {len(all_skills)} skills, {len(nodes)} nodes, {len(edges)} edges.")

        return {
            "data": {
                "nodes": nodes,
                "edges": edges,
                "skill_count": len(all_skills),
                "skills_summary": {k: v["evidence_strength"] for k, v in all_skills.items()},
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"Skill Evidence Graph complete: {len(all_skills)} skills."
            }]
        }

    # ── L1-018 TrustScore ────────────────────────────────────────────────────

    @trace_stage("TrustScore")
    async def analyze_trust_score(self, job_id: str, encrypted_token: str, correlation_id: str) -> dict:
        """Three-dimensional Trust Score: Evidence × 0.40 + Ownership × 0.35 + Consistency × 0.25 (§6.4)."""
        meta = self._read_meta(job_id)

        intent_cache = self._read_task_cache(job_id, "CommitIntent")
        quality_cache = self._read_task_cache(job_id, "CodeQuality")
        ownership_cache = self._read_task_cache(job_id, "Ownership")
        timeline_cache = self._read_task_cache(job_id, "CommitTimeline")
        clone_cache = self._read_task_cache(job_id, "CloneDetection")
        ai_code_cache = self._read_task_cache(job_id, "AiGeneratedCode")

        # Dimension 1: Evidence Verification Score (0-100)
        # Does the code actually match claimed skills?
        intent_confidence = float(intent_cache.get("confidence", 0.7)) * 100
        quality_has_tests = 1.0 if quality_cache.get("testing", {}).get("has_tests") else 0.5
        evidence_score = round(min(100.0, intent_confidence * quality_has_tests), 1)

        # Dimension 2: Ownership Confidence (0-100)
        ownership_score = float(ownership_cache.get("ownership_score", meta.get("user_commit_ratio", 1.0))) * 100
        clone_penalty = float(clone_cache.get("clone_risk_score", 0)) * 0.5
        ai_penalty = float(ai_code_cache.get("ai_risk_score", 0)) * 0.4
        ownership_confidence = round(max(0.0, min(100.0, ownership_score - clone_penalty - ai_penalty)), 1)

        # Dimension 3: Consistency Score (0-100)
        freq_score = float(timeline_cache.get("commit_frequency_score", 50))
        burst_count = len(timeline_cache.get("burst_days", []))
        consistency_penalty = min(30.0, burst_count * 5.0)
        consistency_score = round(max(0.0, min(100.0, freq_score - consistency_penalty)), 1)

        # Weighted trust score (§6.4 formula)
        raw_trust = (
            evidence_score * 0.40 +
            ownership_confidence * 0.35 +
            consistency_score * 0.25
        )

        # Apply repo classification confidence ceiling
        confidence_ceiling = float(meta.get("confidence_ceiling", 1.0))
        trust_score = round(min(raw_trust * confidence_ceiling, 100.0), 1)

        # Adversarial risk adjustments
        adversarial_flags: list[str] = clone_cache.get("flags", []) + [
            f.get("flag", "") for f in ai_code_cache.get("flags", []) if f.get("flag")
        ]
        if adversarial_flags:
            trust_score = max(0.0, trust_score - len(adversarial_flags) * 5.0)

        trust_level = "high" if trust_score >= 70 else "medium" if trust_score >= 40 else "low"

        await self.publish_task_event(job_id, "TrustScore",
            f"Trust score={trust_score:.1f} ({trust_level}). E={evidence_score} O={ownership_confidence} C={consistency_score}.")

        return {
            "data": {
                "trust_score": round(trust_score, 1),
                "trust_level": trust_level,
                "dimensions": {
                    "evidence_verification": evidence_score,
                    "ownership_confidence": ownership_confidence,
                    "consistency_score": consistency_score,
                },
                "raw_score_before_ceiling": round(raw_trust, 1),
                "confidence_ceiling_applied": confidence_ceiling,
                "adversarial_flags": adversarial_flags,
            },
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"Trust Score computed: {trust_score:.1f}/100 ({trust_level})."
            }]
        }

    async def aggregate_results(
        self,
        job_id: str,
        repository_id: str,
        repo_owner: str,
        repo_name: str,
        partial_results: dict,
        delete_workspace: bool = False,
        correlation_id: str = "system"
    ) -> dict:
        extra_log = {"correlation_id": correlation_id}
        temp_dir_base = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "temp_clones"))
        job_dir = os.path.join(temp_dir_base, job_id)
        meta_path = os.path.join(job_dir, "meta.json")
        clone_dir = os.path.join(job_dir, "repo")

        if not os.path.exists(meta_path):
            raise Exception("Workspace metadata not found. Repository Structure task must run first.")

        with open(meta_path, "r", encoding="utf-8") as f_in:
            meta = json.load(f_in)

        structure_data = partial_results.get("RepoStructure", {})
        commits_data = partial_results.get("CommitIntelligence", {})
        skills_data = partial_results.get("SkillExtraction", {})
        arch_data = partial_results.get("ArchitectureAnalysis", {})
        quality_data = partial_results.get("CodeQuality", {})
        security_data = partial_results.get("SecurityAnalysis", {})
        class_task_data = partial_results.get("RepositoryClassification", {})
        summary_data = partial_results.get("RepositorySummary", {})
        cv_synthesis_data = partial_results.get("CvSynthesis", {})

        # Strict file-based CI/CD detection
        cicd_files_exist = False
        if meta.get("is_cloned") and os.path.exists(clone_dir):
            workflows_dir = os.path.join(clone_dir, ".github", "workflows")
            if os.path.isdir(workflows_dir):
                try:
                    w_files = os.listdir(workflows_dir)
                    if any(os.path.isfile(os.path.join(workflows_dir, f)) for f in w_files):
                        cicd_files_exist = True
                except Exception:
                    pass
            if not cicd_files_exist:
                for f in ("Jenkinsfile", "jenkinsfile"):
                    if os.path.isfile(os.path.join(clone_dir, f)):
                        cicd_files_exist = True
                        break
            if not cicd_files_exist:
                for f in (".gitlab-ci.yml", ".gitlab-ci.yaml"):
                    if os.path.isfile(os.path.join(clone_dir, f)):
                        cicd_files_exist = True
                        break

        cicd_ok = cicd_files_exist

        repo_type = meta.get("repo_type", "ORIGINAL_WORK")
        confidence_ceiling = meta.get("confidence_ceiling", 1.0)
        confidence_modifier = meta.get("confidence_modifier", 1.0)
        classification_rationale = meta.get("classification_rationale", "")
        technologies = meta.get("technologies", [])
        filenames = meta.get("filenames", [])

        repo_info = structure_data.get("repo", {})
        classification_info = structure_data.get("classification", {})

        # Segregated classification and authenticity data structures
        authenticity_info = {
            "type": repo_type,
            "confidence_ceiling": confidence_ceiling,
            "confidence_modifier": confidence_modifier,
            "rationale": classification_rationale,
            "red_flags": meta.get("red_flags", [])
        }

        class_primary_type = class_task_data.get("primary_type", "Unknown")
        class_all_types = class_task_data.get("all_types", [])
        class_confidence = class_task_data.get("confidence", 0.8)
        class_evidence = class_task_data.get("evidence", [])
        class_schema_version = class_task_data.get("schema_version", "1.0")
        class_classifier_version = class_task_data.get("classifier_version", "2026.06")

        classification_dict = {
            "schema_version": class_schema_version,
            "classifier_version": class_classifier_version,
            "primary_type": class_primary_type,
            "all_types": class_all_types,
            "confidence": class_confidence,
            "evidence": class_evidence,
            
            # Legacy fallbacks for backward compatibility
            "classification_rationale": " | ".join(class_evidence) if class_evidence else classification_rationale,
            "complexity": class_task_data.get("complexity", "medium"),
            "benchmark_group": class_task_data.get("benchmark_group", class_primary_type.lower().replace(" ", "_") + "s"),
            "sampled_files": meta.get("sampled_files", []),
            "ignored_files_count": max(0, len(filenames) - len(meta.get("sampled_files", []))),
            "confidence_factors": meta.get("red_flags") or ["authentic_history"]
        }

        ownership_info = commits_data.get("ownership", {
            "user_commit_ratio": 1.0,
            "total_commits": 1,
            "is_primary_author": True,
            "architectural_ownership_pct": 100.0,
            "critical_path_ownership_pct": 100.0,
            "maintenance_duration_months": 1,
            "explanation": "Authentic original codebase.",
            "contributor_distribution": [],
            "bus_factor": 1,
            "active_contributors": 1
        })
        trust_info = commits_data.get("trust", {
            "classification": "personal_authentic",
            "confidence": 100,
            "rule_flags": [],
            "ai_findings": [],
            "explanation": ""
        })

        trust_confidence = trust_info.get("confidence", 100)
        modified_confidence = min(100.0, float(trust_confidence) * confidence_modifier)
        max_conf = confidence_ceiling * 100.0
        if modified_confidence > max_conf:
            modified_confidence = max_conf
        trust_info["confidence"] = round(modified_confidence, 1)

        profile_info = {
            "technologies": [{"name": t, "type": "language" if t in ("Python", "JavaScript", "TypeScript", "C#", "Java", "Go", "Rust", "Ruby", "PHP") else "framework"} for t in technologies],
            "skills": {},
            "architecture": {
                "patterns": [p.get("pattern") for p in arch_data.get("patterns", [])] if "patterns" in arch_data else [],
                "explanation": arch_data.get("explanation", "")
            },
            "engineering_practices": {
                "testing": {
                    "frameworks": quality_data.get("testing", {}).get("frameworks", []),
                    "has_tests": quality_data.get("testing", {}).get("has_tests", False),
                    "detail": quality_data.get("testing", {}).get("detail", "")
                },
                "observability": {
                    "logging_configured": quality_data.get("observability", {}).get("logging_configured", False),
                    "metrics_configured": quality_data.get("observability", {}).get("metrics_configured", False),
                    "detail": quality_data.get("observability", {}).get("detail", "")
                },
                "cicd": {
                    "configured": cicd_ok,
                    "providers": quality_data.get("cicd", {}).get("providers", []) if cicd_ok else []
                }
            }
        }

        skills_dict = {}
        for s_item in skills_data.get("skills", []):
            cat = s_item.get("category", "backend")
            skill_name = s_item.get("skill")
            if cat not in skills_dict:
                skills_dict[cat] = []
            skills_dict[cat].append(skill_name)
        profile_info["skills"] = skills_dict

        # Extract, normalize, validate, and deduplicate findings using UnifiedEvidenceEngine
        raw_items = []
        raw_items.extend(UnifiedEvidenceEngine.normalize_quality(quality_data))
        raw_items.extend(UnifiedEvidenceEngine.normalize_security(security_data))
        raw_items.extend(UnifiedEvidenceEngine.normalize_architecture(arch_data))

        unique_items = UnifiedEvidenceEngine.deduplicate_and_validate(raw_items, filenames)
        ev_strength = UnifiedEvidenceEngine.calculate_evidence_strength(unique_items)

        # Convert back to legacy findings structure for downstream compatibility
        findings = []
        for item in unique_items:
            if item.type in ("engineering_practices", "security_findings"):
                findings.append({
                    "finding": item.title,
                    "title": item.title,
                    "category": "quality" if item.type == "engineering_practices" else "security",
                    "explanation": item.content,
                    "impact": "critical" if item.severity == "critical" else "warning" if item.severity == "medium" else "positive",
                    "confidence": int(item.confidence * 100),
                    "evidence_signals": item.evidence_signals
                })

        narrative_info = {
            "recruiter_summary": summary_data.get("recruiter_summary", ""),
            "top_strengths": [{"strength": s.get("strength"), "rationale": s.get("rationale")} for s in summary_data.get("top_strengths", [])],
            "limitations": [{"limitation": l.get("limitation"), "rationale": l.get("rationale")} for l in summary_data.get("limitations", [])]
        }

        # Determine risk level and score based on configuration-driven weights
        def load_risk_policy() -> dict:
            policy_path = os.path.join(os.path.dirname(__file__), "..", "scoring", "risk_policy.json")
            if os.path.exists(policy_path):
                try:
                    with open(policy_path, "r", encoding="utf-8") as f:
                        return json.load(f).get("weights", {})
                except Exception:
                    pass
            return {}

        policy = load_risk_policy()
        sec_cfg = policy.get("security", {"base_score": 15.0, "sec_critical": 35.0, "sec_warning": 15.0, "vuln_critical": 20.0, "vuln_warning": 10.0})
        maint_cfg = policy.get("maintainability", {"base_low": 15.0, "base_medium": 35.0, "base_high": 60.0, "qual_critical": 20.0, "qual_warning": 10.0})
        arch_cfg = policy.get("architecture", {"base_score": 15.0, "arch_critical": 30.0, "arch_warning": 15.0})
        op_cfg = policy.get("operational", {"base_score": 10.0, "no_cicd": 25.0, "no_tests": 20.0, "no_logging": 20.0, "no_metrics": 15.0, "op_critical": 20.0, "op_warning": 10.0})
        dep_cfg = policy.get("dependency", {"base_score": 15.0, "dep_critical": 20.0, "dep_warning": 10.0})
        unc_cfg = policy.get("evidence_uncertainty", {"low_commits": 30.0, "low_user_ratio": 25.0})
        overall_cfg = policy.get("overall", {"max_weight": 0.7, "avg_weight": 0.3})

        critical_sec = sum(1 for f in findings if f.get("category") == "security" and f.get("impact") == "critical")
        warning_sec = sum(1 for f in findings if f.get("category") == "security" and f.get("impact") == "warning")
        vuln_critical = sum(1 for v in security_data.get("vulnerabilities", []) if v.get("severity") == "critical")
        vuln_warning = sum(1 for v in security_data.get("vulnerabilities", []) if v.get("severity") in ("warning", "medium"))
        
        complexity = classification_dict.get("complexity", "medium")
        qual_critical = sum(1 for f in findings if f.get("category") == "quality" and f.get("impact") == "critical")
        qual_warning = sum(1 for f in findings if f.get("category") == "quality" and f.get("impact") == "warning")

        arch_critical = sum(1 for f in findings if ("architecture" in f.get("finding", "").lower() or "structure" in f.get("finding", "").lower()) and f.get("impact") == "critical")
        arch_warning = sum(1 for f in findings if ("architecture" in f.get("finding", "").lower() or "structure" in f.get("finding", "").lower()) and f.get("impact") == "warning")

        has_tests = quality_data.get("testing", {}).get("has_tests", False)
        logging_ok = quality_data.get("observability", {}).get("logging_configured", False)
        metrics_ok = quality_data.get("observability", {}).get("metrics_configured", False)
        op_crit = sum(1 for f in findings if f.get("category") == "quality" and "test" in f.get("finding", "").lower() and f.get("impact") == "critical")
        op_warn = sum(1 for f in findings if f.get("category") == "quality" and "test" in f.get("finding", "").lower() and f.get("impact") == "warning")

        total_commits = ownership_info.get("total_commits", 1)
        user_ratio = ownership_info.get("user_commit_ratio", 1.0)
        confidence_ceiling = meta.get("confidence_ceiling", 1.0)

        # 6-dimensional risk score calculations
        security_score = min(100.0, sec_cfg["base_score"] + sec_cfg["sec_critical"] * critical_sec + sec_cfg["sec_warning"] * warning_sec + sec_cfg["vuln_critical"] * vuln_critical + sec_cfg["vuln_warning"] * vuln_warning)
        
        complexity_base = maint_cfg["base_high"] if complexity == "high" else maint_cfg["base_medium"] if complexity == "medium" else maint_cfg["base_low"]
        maintainability_score = min(100.0, complexity_base + maint_cfg["qual_critical"] * qual_critical + maint_cfg["qual_warning"] * qual_warning)
        
        architecture_score = min(100.0, arch_cfg["base_score"] + arch_cfg["arch_critical"] * arch_critical + arch_cfg["arch_warning"] * arch_warning)
        
        operational_score = op_cfg["base_score"]
        if not cicd_ok: operational_score += op_cfg["no_cicd"]
        if not has_tests: operational_score += op_cfg["no_tests"]
        if not logging_ok: operational_score += op_cfg["no_logging"]
        if not metrics_ok: operational_score += op_cfg["no_metrics"]
        operational_score = min(100.0, operational_score + op_cfg["op_critical"] * op_crit + op_cfg["op_warning"] * op_warn)

        dep_score = dep_cfg["base_score"]
        for f in findings:
            desc = f.get("explanation", "").lower() or ""
            title = f.get("finding", "").lower() or ""
            if "dependency" in desc or "dependency" in title or "package" in desc or "package" in title:
                dep_score += dep_cfg["dep_critical"] if f.get("impact") == "critical" else dep_cfg["dep_warning"]
        dep_score = min(100.0, dep_score)

        uncertainty_score = 100.0 - (confidence_ceiling * 100.0)
        if total_commits < 5: uncertainty_score += unc_cfg["low_commits"]
        if user_ratio < 0.2: uncertainty_score += unc_cfg["low_user_ratio"]
        uncertainty_score = min(100.0, uncertainty_score)

        # Overall risk level mapping
        max_dim = max(security_score, maintainability_score, architecture_score, operational_score, dep_score, uncertainty_score)
        avg_dim = (security_score + maintainability_score + architecture_score + operational_score + dep_score + uncertainty_score) / 6.0
        overall_score = round(overall_cfg["max_weight"] * max_dim + overall_cfg["avg_weight"] * avg_dim, 1)

        risk_level = "High" if overall_score >= 70.0 else "Medium" if overall_score >= 35.0 else "Low"

        # Determine Contributing Factors
        top_factors = []
        if security_score >= 70.0: top_factors.append("Vulnerable code patterns / dependencies")
        if maintainability_score >= 70.0: top_factors.append("High structural complexity / tech debt")
        if operational_score >= 70.0: top_factors.append("Missing automated testing or CI/CD pipelines")
        if dep_score >= 70.0: top_factors.append("Supply chain / outdated package warnings")
        if uncertainty_score >= 70.0: top_factors.append("Sparse commit history or low authorship ratio")
        
        if not top_factors:
            if overall_score >= 35.0: top_factors.append("Moderate warnings in quality/security indicators")
            else: top_factors.append("Project demonstrates solid engineering practices")

        risk_assessment = {
            "risk_level": risk_level,
            "risk_score": overall_score,
            "critical_findings_count": critical_sec + qual_critical,
            "warning_findings_count": warning_sec + qual_warning,
            "explanation": f"Overall risk is {risk_level} due to: " + ", ".join(top_factors) + ".",
            "top_factors": top_factors,
            "dimensions": {
                "security": security_score,
                "maintainability": maintainability_score,
                "architecture": architecture_score,
                "operational": operational_score,
                "dependency": dep_score,
                "evidence_uncertainty": uncertainty_score
            }
        }

        # 1. TRUTH CALIBRATION LAYER & CONFLICT RESOLUTION
        conflict_resolution_log = []
        
        # Authority Ranking: Git Logs (S_git: highest) -> GitHub API (S_api: medium) -> LLM Inference (S_llm: advisory)
        calibrated_skills = {}
        
        # Reconcile LLM Inferred Skills (S_llm) with Configuration Files (S_git)
        # S_git holds technologies from technology detector config files walk
        detected_tech_set = {t.lower() for t in technologies}
        
        # If it was a list of dicts from skills_data, handle it:
        if isinstance(skills_data.get("skills"), list):
            calibrated_skills_list = []
            for s_item in skills_data.get("skills", []):
                cat = s_item.get("category", "backend")
                skill = s_item.get("skill", "")
                skill_lower = skill.lower()
                has_config_support = False
                for tech in detected_tech_set:
                    if tech in skill_lower or skill_lower in tech:
                        has_config_support = True
                        break
                
                if has_config_support or skill in ("Git", "GitHub", "CI/CD"):
                    calibrated_skills_list.append(s_item)
                else:
                    msg = f"Rejected LLM skill inference '{skill}': Unsupported by configuration files dependency references."
                    conflict_resolution_log.append(msg)
                    logger.info(msg, extra=extra_log)
            
            # Rebuild profile skills
            skills_dict = {}
            for s_item in calibrated_skills_list:
                c_cat = s_item.get("category", "backend")
                s_name = s_item.get("skill")
                if c_cat not in skills_dict:
                    skills_dict[c_cat] = []
                skills_dict[c_cat].append(s_name)
            profile_info["skills"] = skills_dict

        # Reconcile Git logs (S_git) vs GitHub API (S_api) for user ownership
        user_git_ratio = ownership_info.get("user_commit_ratio", 1.0)
        is_primary_git = ownership_info.get("is_primary_author", True)
        
        # Check scenario: API claims user is primary, but Git log says mismatch
        # If Git log has low contribution ratio (<0.20), override API's claims
        if user_git_ratio < 0.20 and is_primary_git:
            ownership_info["is_primary_author"] = False
            msg = "Ownership Conflict Resolved: Overrode primary author flag to False. Git logs indicate contribution ratio is under 20% despite API metrics."
            conflict_resolution_log.append(msg)
            logger.info(msg, extra=extra_log)

        # 2. EXPLICIT UNCERTAINTY & ADVERSARIAL METRICS DECOMPOSITION
        unverified_commits = meta.get("unverified_commits_count", 0)
        compression_ratio = meta.get("timestamp_compression_ratio", 0.0)
        uncalibrated_identities = meta.get("uncalibrated_identities_count", 0)
        
        # Calculate Variance (Stability parameter)
        # Stable: High commits count, long age. Variable: Small commits count.
        total_c = ownership_info.get("total_commits", 1)
        variance = round(100.0 / (total_c + 1), 2)
        
        # Calculate Sampling Bias Risk (ratio of skipped to scanned files)
        files_sc = meta.get("files_scanned", 1)
        files_sa = meta.get("files_sampled", 1)
        sampling_bias = round(1.0 - (files_sa / max(1, files_sc)), 4)
        
        # Calculate Adversarial Risk
        # Triggered by timestamp compression, unverified commits, or mismatch author emails
        adv_score = 0.0
        if compression_ratio > 0.1:
            adv_score += 40.0
        if uncalibrated_identities > 2:
            adv_score += 30.0
        if unverified_commits > 0:
            # Scale with unverified percentage
            pct_unverified = unverified_commits / max(1, total_c)
            adv_score += pct_unverified * 30.0
        adversarial_risk = min(100.0, round(adv_score, 1))

        # Adjust main trust confidence based on adversarial risk
        final_trust_confidence = float(trust_info.get("confidence", 100))
        if adversarial_risk > 30.0:
            final_trust_confidence = max(10.0, final_trust_confidence - (adversarial_risk * 0.5))
            trust_info["confidence"] = round(final_trust_confidence, 1)

        # Force isVerified confidence adjustments if CI/CD not configured
        if not cicd_ok:
            final_trust_confidence = max(10.0, final_trust_confidence - 20.0)
            trust_info["confidence"] = round(final_trust_confidence, 1)

        is_verified = final_trust_confidence >= 50.0

        # 3. STRUCTURED CALIBRATED TRUST GRAPH GENERATION
        # Nodes: Developer, Repository, Skill, Evidence
        # Edges: Owns, Demonstrates, Uses, Verifies, Attributes
        nodes = [
            {"id": "developer", "type": "developer", "data": {"label": f"Developer ({repo_owner})", "ratio": user_git_ratio}},
            {"id": "repository", "type": "repository", "data": {"label": f"Repository ({repo_owner}/{repo_name})", "type": repo_type}}
        ]
        edges = [
            {"id": "dev-owns-repo", "source": "developer", "target": "repository", "label": "Owns", "weight": user_git_ratio}
        ]
        
        # Add verified skill nodes (calibrated)
        skill_index = 0
        for cat, list_skills in profile_info.get("skills", {}).items():
            for s_name in list_skills:
                node_id = f"skill-{skill_index}"
                nodes.append({"id": node_id, "type": "skill", "data": {"label": s_name, "category": cat}})
                edges.append({"id": f"dev-has-{node_id}", "source": "developer", "target": node_id, "label": "Demonstrates", "weight": round(final_trust_confidence / 100.0, 2)})
                edges.append({"id": f"repo-uses-{node_id}", "source": "repository", "target": node_id, "label": "Uses"})
                skill_index += 1
                
        # Add verified evidence nodes (signed commits / verified code files)
        evidence_index = 0
        for f in findings:
            finding_name = f.get("finding", "Practice Citation")
            node_id = f"evidence-{evidence_index}"
            nodes.append({"id": node_id, "type": "evidence", "data": {"label": finding_name, "category": f.get("category", "quality")}})
            # Link evidence to developer/repo
            edges.append({"id": f"ev-supports-{node_id}", "source": node_id, "target": "repository", "label": "Verifies"})
            evidence_index += 1

        trust_graph = {
            "nodes": nodes,
            "edges": edges
        }

        # Build sections for v2 schema using unique, validated evidence
        testing_frameworks = quality_data.get("testing", {}).get("frameworks", [])
        cicd_providers = quality_data.get("cicd", {}).get("providers", []) if cicd_ok else []
        
        eng_items = [
            {
                "title": "Testing",
                "content": f"{', '.join(testing_frameworks) if testing_frameworks else 'None'} ({'configured' if has_tests else 'not configured'})"
            },
            {
                "title": "Observability",
                "content": f"Logging is {'configured' if logging_ok else 'not configured'}, Metrics are {'configured' if metrics_ok else 'not configured'}"
            },
            {
                "title": "CI/CD",
                "content": f"{'Configured (' + ', '.join(cicd_providers) + ')' if cicd_ok else 'Not configured'}"
            }
        ]
        for item in unique_items:
            if item.type == "engineering_practices":
                eng_items.append({
                    "title": item.title,
                    "content": item.content
                })

        sec_items = []
        for item in unique_items:
            if item.type == "security_findings":
                sec_items.append({
                    "title": item.title,
                    "content": item.content
                })
        if not sec_items:
            sec_items.append({
                "title": "Security Findings",
                "content": "No critical security vulnerabilities or warning findings detected."
            })

        arch_items = []
        for item in unique_items:
            if item.type == "architecture_insights":
                arch_items.append({
                    "title": item.title,
                    "content": item.content
                })
        if arch_data.get("explanation"):
            arch_items.append({
                "title": "Architecture Explanation",
                "content": arch_data.get('explanation')
            })
        if not arch_items:
            arch_items.append({
                "title": "Architectural Insights",
                "content": "No specialized architectural insights detected."
            })

        sections = [
            {"type": "engineering_practices", "items": eng_items},
            {"type": "security_findings", "items": sec_items},
            {"type": "architecture_insights", "items": arch_items}
        ]

        classification_v2 = {
            "primaryDomain": class_primary_type if class_primary_type else "Unknown",
            "subDomain": ", ".join(technologies[:2]) if technologies else "General",
            "confidence": min(1.0, max(0.0, float(class_confidence))),
            "isVerified": is_verified,
            "trustScore": min(1.0, max(0.0, float(final_trust_confidence) / 100.0))
        }

        risk_v2 = {
            "score": min(100.0, max(0.0, float(overall_score))),
            "level": risk_level.lower(),
            "reasons": top_factors
        }

        # Structure the payload strictly satisfying the v2 contract, plus legacy fields
        report_dict = {
            "schemaVersion": "v2",
            "repoId": str(repository_id),
            "classification": classification_v2,
            "sections": sections,
            "risk": risk_v2,
            "cvSynthesis": cv_synthesis_data if cv_synthesis_data else None,
            "evidenceStrength": ev_strength,
            
            # Legacy fields for backward compatibility
            "facts": {
                "repo": repo_info,
                "git_metrics": {
                    "total_commits": ownership_info.get("total_commits", 1),
                    "user_commit_ratio": ownership_info.get("user_commit_ratio", 1.0),
                    "is_primary_author": ownership_info.get("is_primary_author", True),
                    "bus_factor": ownership_info.get("bus_factor", 1),
                    "active_contributors": ownership_info.get("active_contributors", 1),
                    "contributor_distribution": ownership_info.get("contributor_distribution", [])
                },
                "quality_metrics": {
                    "files_scanned": meta.get("files_scanned", 0),
                    "files_sampled": meta.get("files_sampled", 0),
                    "skipped_files": meta.get("skipped_files", 0),
                    "coverage_pct": meta.get("coverage_pct", 100.0),
                    "prompt_cache_efficiency": 0.82
                }
            },
            "ai_conclusions": {
                "authenticity": authenticity_info,
                "classification": classification_dict,
                "evidence_points": {
                    "total": len(findings) * 5,
                    "breakdown": {f.get("category", "quality"): 5 for f in findings}
                },
                "trust": trust_info,
                "risk_assessment": risk_assessment,
                "positioning": {
                    "benchmark_group": classification_dict.get("benchmark_group", "unclassified"),
                    "peer_group_size": 100,
                    "relative_strengths": [s.get("strength") for s in summary_data.get("top_strengths", [])][:3]
                },
                "profile": profile_info,
                "findings": findings,
                "narrative": narrative_info
            },
            "trust_intelligence": {
                "uncertainty_metrics": {
                    "variance": variance,
                    "sampling_bias_risk": sampling_bias,
                    "adversarial_manipulation_risk": adversarial_risk,
                    "unverified_commits": unverified_commits,
                    "timestamp_compression_ratio": compression_ratio,
                    "uncalibrated_identities": uncalibrated_identities
                },
                "conflict_resolution_log": conflict_resolution_log,
                "trust_graph": trust_graph
            }
        }

        # Pydantic v2 contract enforcement validation check
        try:
            ReportV2Contract.model_validate(report_dict)
        except ValidationError as val_err:
            logger.error(f"Report V2 Contract validation failure: {val_err}", extra=extra_log)
            raise ValueError(f"Report V2 Contract validation failure: {val_err}")

        # Workspace lifecycle clean up tracking
        if delete_workspace:
            try:
                shutil.rmtree(job_dir, ignore_errors=True)
                logger.info(f"Workspace lifecycle audit: Cleaned up workspace folder for job {job_id}", extra=extra_log)
            except Exception as cleanup_err:
                logger.warning(f"Workspace lifecycle audit: Cleanup failed: {cleanup_err}", extra=extra_log)

        return report_dict
