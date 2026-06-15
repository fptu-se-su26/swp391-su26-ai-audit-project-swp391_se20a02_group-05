import json
import logging
import time
from typing import Any, Dict

from app.core.services.claude_service import ClaudeService
from app.pipelines.shared.ai.prompts.candidate_prompt_factory import CandidatePromptFactory
from app.core.monitoring.observability import trace_stage, TraceContext

logger = logging.getLogger("candidate_evaluation_orchestrator")

_CANDIDATE_L2_TASK_NAMES = {
    "SkillTaxonomyMapper", "SkillProficiencyEstimator", "StrengthWeaknessAnalyzer",
    "CareerLevelMapper", "CareerLevelCalibrator", "CareerLevelGate",
    "EngineeringMaturityAssessor", "ProblemSolvingAnalyzer", "TechnicalTendencyClassifier",
    "WorkingStyleClassifier", "ExperienceConfidenceMultiplier", "MultiRoleRecommendationEngine",
    "CandidateSummaryGenerator", "CandidateProfileComposer",
}

TASK_ALIASES: dict[str, str] = {
    "L2-001": "SkillTaxonomyMapper",
    "L2-002": "SkillProficiencyEstimator",
    "L2-003": "StrengthWeaknessAnalyzer",
    "L2-004": "CareerLevelMapper",
    "L2-005": "CareerLevelCalibrator",
    "L2-006": "CareerLevelGate",
    "L2-007": "EngineeringMaturityAssessor",
    "L2-008": "ProblemSolvingAnalyzer",
    "L2-009": "TechnicalTendencyClassifier",
    "L2-010": "WorkingStyleClassifier",
    "L2-011": "ExperienceConfidenceMultiplier",
    "L2-012": "MultiRoleRecommendationEngine",
    "L2-013": "CandidateSummaryGenerator",
    "L2-014": "CandidateProfileComposer",
}


def is_line2_task(task_type: str) -> bool:
    return task_type in TASK_ALIASES or task_type in _CANDIDATE_L2_TASK_NAMES


class CandidateEvaluationOrchestrator:
    def __init__(self):
        self.claude_service = ClaudeService()
        self.prompt_factory = CandidatePromptFactory()

    def _extract_json(self, text: str, correlation_id: str) -> dict:
        text = text.strip()
        first_brace = text.find('{')
        last_brace = text.rfind('}')
        if first_brace != -1 and last_brace != -1 and last_brace > first_brace:
            candidate = text[first_brace:last_brace + 1]
            try:
                return json.loads(candidate)
            except Exception:
                pass
        try:
            import json_repair
            repaired = json_repair.repair_json(text[first_brace:] if first_brace != -1 else text)
            return json.loads(repaired)
        except Exception as e:
            raise ValueError(f"Failed to parse Claude JSON output: {e}")

    def _ok(self, data: dict, telemetry: Any, task_type: str) -> dict:
        return {
            "status": "Completed",
            "errorMessage": None,
            "schemaVersion": "2.0.0",
            "resultData": json.dumps(data),
            "telemetry": telemetry,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Info",
                "eventType": "StepCompleted",
                "message": f"Line 2 task {task_type} completed successfully."
            }]
        }

    def _err(self, task_type: str, job_id: str, e: Exception) -> dict:
        err_str = str(e).lower()
        if "rate limit" in err_str or "429" in err_str:
            code, retry = "RATE_LIMIT_EXCEEDED", True
        elif "timeout" in err_str:
            code, retry = "TIMEOUT", True
        elif "json" in err_str or "parse" in err_str:
            code, retry = "PARSING_ERROR", False
        else:
            code, retry = "UNKNOWN_ERROR", True
        return {
            "status": "Failed",
            "errorMessage": str(e),
            "errorCode": code,
            "retryable": retry,
            "taskId": task_type,
            "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
            "schemaVersion": "2.0.0",
            "resultData": None,
            "telemetry": None,
            "events": [{
                "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
                "level": "Error",
                "eventType": "AI_TASK_FAILED",
                "message": str(e)
            }]
        }

    async def execute_task(
        self,
        task_type: str,
        job_id: str,
        inputs: Dict[str, Any],
        correlation_id: str = "system"
    ) -> dict:
        normalized = TASK_ALIASES.get(task_type, task_type)
        extra = {"correlation_id": correlation_id, "job_id": job_id, "task_type": normalized}
        logger.info(f"Executing Line 2 task {normalized} for job {job_id}", extra=extra)

        TraceContext.set(pipeline_stage=normalized, is_sampled=True, extra={
            "jobId": job_id, "taskType": normalized, "correlationId": correlation_id
        })

        try:
            if normalized == "SkillTaxonomyMapper":
                result = await self._skill_taxonomy_mapper(job_id, inputs, correlation_id)
            elif normalized == "SkillProficiencyEstimator":
                result = await self._skill_proficiency_estimator(job_id, inputs, correlation_id)
            elif normalized == "StrengthWeaknessAnalyzer":
                result = await self._strength_weakness_analyzer(job_id, inputs, correlation_id)
            elif normalized == "CareerLevelMapper":
                result = await self._career_level_mapper(job_id, inputs, correlation_id)
            elif normalized == "CareerLevelCalibrator":
                result = await self._career_level_calibrator(job_id, inputs, correlation_id)
            elif normalized == "CareerLevelGate":
                result = await self._career_level_gate(job_id, inputs, correlation_id)
            elif normalized == "EngineeringMaturityAssessor":
                result = await self._engineering_maturity_assessor(job_id, inputs, correlation_id)
            elif normalized == "ProblemSolvingAnalyzer":
                result = await self._problem_solving_analyzer(job_id, inputs, correlation_id)
            elif normalized == "TechnicalTendencyClassifier":
                result = await self._technical_tendency_classifier(job_id, inputs, correlation_id)
            elif normalized == "WorkingStyleClassifier":
                result = await self._working_style_classifier(job_id, inputs, correlation_id)
            elif normalized == "ExperienceConfidenceMultiplier":
                result = await self._experience_confidence_multiplier(job_id, inputs, correlation_id)
            elif normalized == "MultiRoleRecommendationEngine":
                result = await self._multi_role_recommendation(job_id, inputs, correlation_id)
            elif normalized == "CandidateSummaryGenerator":
                result = await self._candidate_summary_generator(job_id, inputs, correlation_id)
            elif normalized == "CandidateProfileComposer":
                result = await self._candidate_profile_composer(job_id, inputs, correlation_id)
            else:
                raise ValueError(f"Unknown Line 2 task type: {task_type}")
            return result
        except Exception as e:
            logger.exception(f"Error in Line 2 task {normalized}: {e}", extra=extra)
            return self._err(task_type, job_id, e)

    # ── L2-001 ────────────────────────────────────────────────────────────────

    @trace_stage("SkillTaxonomyMapper")
    async def _skill_taxonomy_mapper(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_skill_taxonomy_mapper_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "SkillTaxonomyMapper")

    # ── L2-002 ────────────────────────────────────────────────────────────────

    @trace_stage("SkillProficiencyEstimator")
    async def _skill_proficiency_estimator(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_skill_proficiency_estimator_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "SkillProficiencyEstimator")

    # ── L2-003 ────────────────────────────────────────────────────────────────

    @trace_stage("StrengthWeaknessAnalyzer")
    async def _strength_weakness_analyzer(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_strength_weakness_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "StrengthWeaknessAnalyzer")

    # ── L2-004 ────────────────────────────────────────────────────────────────

    @trace_stage("CareerLevelMapper")
    async def _career_level_mapper(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_career_level_mapper_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "CareerLevelMapper")

    # ── L2-005 ────────────────────────────────────────────────────────────────

    @trace_stage("CareerLevelCalibrator")
    async def _career_level_calibrator(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_career_level_calibrator_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "CareerLevelCalibrator")

    # ── L2-006 ────────────────────────────────────────────────────────────────

    @trace_stage("CareerLevelGate")
    async def _career_level_gate(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        # Rule-based gate with AI explanation
        calibrated_level = inputs.get("calibratedLevel", "L2")
        calibrated_score = inputs.get("calibratedScore", 0.0)
        level_evidence = inputs.get("levelEvidence", {})
        repo_report = inputs.get("repoIntelligenceReport", {})

        # Deterministic gate rules
        gate_violations = []
        final_level = calibrated_level

        patterns = repo_report.get("patterns", [])
        has_architecture_evidence = any(
            p.get("patternName", p.get("pattern", "")).lower() not in ("", "none", "unknown")
            for p in (patterns if isinstance(patterns, list) else [])
        )

        if calibrated_level == "L3" and not has_architecture_evidence:
            gate_violations.append("Senior level requires architecture evidence (design patterns, DI, system design). None detected.")
            final_level = "L2"

        if calibrated_level == "L4":
            l4_evidence = level_evidence.get("L4", [])
            if not l4_evidence:
                gate_violations.append("Staff level requires platform/infrastructure or cross-service evidence.")
                final_level = "L3"

        gate_passed = len(gate_violations) == 0

        level_map = {"L1": "Junior", "L2": "Middle", "L3": "Senior", "L4": "Staff", "L5": "Principal"}

        if gate_passed:
            rationale = f"All gate requirements met for {calibrated_level} ({level_map.get(calibrated_level, calibrated_level)})."
        else:
            rationale = f"Gate downgrade from {calibrated_level} to {final_level}: " + " ".join(gate_violations)

        # Generate AI explanation for the gate decision
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_career_level_gate_prompt({**inputs, "gateViolations": gate_violations})
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        ai_data = self._extract_json(raw, correlation_id)

        # Override AI verdict with deterministic gate result
        ai_data["gatePassed"] = gate_passed
        ai_data["finalLevel"] = final_level
        ai_data["finalLevelLabel"] = level_map.get(final_level, final_level)
        ai_data["finalScore"] = calibrated_score
        ai_data["gateViolations"] = gate_violations
        if not gate_passed:
            ai_data["gateRationale"] = rationale

        return self._ok(ai_data, telemetry, "CareerLevelGate")

    # ── L2-007 ────────────────────────────────────────────────────────────────

    @trace_stage("EngineeringMaturityAssessor")
    async def _engineering_maturity_assessor(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_engineering_maturity_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "EngineeringMaturityAssessor")

    # ── L2-008 ────────────────────────────────────────────────────────────────

    @trace_stage("ProblemSolvingAnalyzer")
    async def _problem_solving_analyzer(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_problem_solving_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "ProblemSolvingAnalyzer")

    # ── L2-009 ────────────────────────────────────────────────────────────────

    @trace_stage("TechnicalTendencyClassifier")
    async def _technical_tendency_classifier(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_technical_tendency_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "TechnicalTendencyClassifier")

    # ── L2-010 ────────────────────────────────────────────────────────────────

    @trace_stage("WorkingStyleClassifier")
    async def _working_style_classifier(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_working_style_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "WorkingStyleClassifier")

    # ── L2-011 (deterministic rule-based) ────────────────────────────────────

    async def _experience_confidence_multiplier(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        working_experience = inputs.get("workingExperience", [])

        total_months = 0
        is_leadership = False
        for exp in (working_experience if isinstance(working_experience, list) else []):
            duration = exp.get("durationMonths", 0)
            total_months += duration
            if exp.get("isLeadership", False):
                is_leadership = True

        years = total_months / 12.0

        # Confidence multiplier formula (1.0x base, max 1.25x)
        if years >= 5:
            multiplier = 1.25
        elif years >= 3:
            multiplier = 1.20
        elif years >= 1:
            multiplier = 1.10
        else:
            multiplier = 1.0

        if is_leadership:
            multiplier = min(multiplier + 0.05, 1.25)

        data = {
            "confidenceMultiplier": round(multiplier, 2),
            "totalExperienceMonths": total_months,
            "totalExperienceYears": round(years, 1),
            "hasLeadershipExperience": is_leadership,
            "multiplierRationale": (
                f"{round(years, 1)} years of experience"
                f"{' including leadership roles' if is_leadership else ''}. "
                f"Confidence boost: {round((multiplier - 1.0) * 100, 0):.0f}%."
            )
        }
        return self._ok(data, None, "ExperienceConfidenceMultiplier")

    # ── L2-012 ────────────────────────────────────────────────────────────────

    @trace_stage("MultiRoleRecommendationEngine")
    async def _multi_role_recommendation(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_multi_role_recommendation_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "MultiRoleRecommendationEngine")

    # ── L2-013 ────────────────────────────────────────────────────────────────

    @trace_stage("CandidateSummaryGenerator")
    async def _candidate_summary_generator(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        system = self.prompt_factory.get_system_prompt()
        user = self.prompt_factory.get_candidate_summary_prompt(inputs)
        raw, telemetry = await self.claude_service.analyze_repo_with_telemetry(system, user, correlation_id)
        data = self._extract_json(raw, correlation_id)
        return self._ok(data, telemetry, "CandidateSummaryGenerator")

    # ── L2-014 (rule-based aggregation) ──────────────────────────────────────

    async def _candidate_profile_composer(self, job_id: str, inputs: dict, correlation_id: str) -> dict:
        # Aggregate all L2 task outputs into the final Candidate Profile
        skill_score = float(inputs.get("skillDepthScore", 0.0))
        ownership_score = float(inputs.get("ownershipScore", 0.0))
        architecture_score = float(inputs.get("architectureScore", 0.0))
        problem_solving_score = float(inputs.get("problemSolvingScore", 0.0))
        impact_score = float(inputs.get("impactScore", 0.0))

        # Formula: Skill×35% + Ownership×25% + Architecture×20% + ProblemSolving×12% + Impact×8%
        candidate_score = (
            skill_score * 0.35
            + ownership_score * 0.25
            + architecture_score * 0.20
            + problem_solving_score * 0.12
            + impact_score * 0.08
        )
        candidate_score = round(min(max(candidate_score, 0.0), 100.0), 2)

        confidence_multiplier = float(inputs.get("confidenceMultiplier", 1.0))

        data = {
            "schemaVersion": "candidate-profile-v1",
            "candidateScore": candidate_score,
            "candidateScoreLabel": _score_to_label(candidate_score),
            "careerLevel": inputs.get("finalLevel", "L2"),
            "careerLevelLabel": inputs.get("finalLevelLabel", "Middle"),
            "careerLevelConfidence": inputs.get("confidenceInLevel", 0.8),
            "confidenceMultiplier": confidence_multiplier,
            "displayConfidence": round(min(inputs.get("confidenceInLevel", 0.8) * confidence_multiplier, 1.0), 2),
            "primaryTendency": inputs.get("primaryTendency", ""),
            "tendencyConfidence": inputs.get("primaryConfidence", 0.0),
            "primaryWorkingStyle": inputs.get("primaryWorkingStyle", ""),
            "strongestDomains": inputs.get("strongestDomains", []),
            "skillGaps": inputs.get("skillGaps", []),
            "skillProficiencies": inputs.get("skillProficiencies", []),
            "engineeringMaturityScore": inputs.get("engineeringMaturityScore", 0.0),
            "problemSolvingScore": problem_solving_score,
            "topRoleMatch": inputs.get("topMatch", {}),
            "suggestedRoles": inputs.get("suggestedRoles", []),
            "recruiterHeadline": inputs.get("recruiterHeadline", ""),
            "fullSummary": inputs.get("fullSummary", ""),
            "keyStrengths": inputs.get("keyStrengths", []),
            "watchPoints": inputs.get("watchPoints", []),
            "scoreBreakdown": {
                "skillDepth": {"score": skill_score, "weight": 0.35},
                "ownership": {"score": ownership_score, "weight": 0.25},
                "architecture": {"score": architecture_score, "weight": 0.20},
                "problemSolving": {"score": problem_solving_score, "weight": 0.12},
                "impact": {"score": impact_score, "weight": 0.08},
            }
        }
        return self._ok(data, None, "CandidateProfileComposer")


def _score_to_label(score: float) -> str:
    if score >= 93:
        return "Principal"
    if score >= 83:
        return "Staff"
    if score >= 66:
        return "Senior"
    if score >= 46:
        return "Middle"
    return "Junior"
