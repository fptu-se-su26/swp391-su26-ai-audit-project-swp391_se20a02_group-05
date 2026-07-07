# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-06-12 |
| Ngày cập nhật gần nhất | 2026-06-12 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity

---

## 4. Danh sách Prompt đã sử dụng

---

### Prompt số 1

| Thông tin | Nội dung |
|---|---|
| Ngày sử dụng | 2026-06-12 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích | Implement 10 v2 pipeline task methods vào `github_analysis_orchestrator.py` |
| Loại prompt | Code generation |

#### Nội dung Prompt

```text
Implement v2 pipeline tasks for the CVerify AI service. The tasks should align
with the 18-task DAG defined in CVerify.Core DagScheduler. New tasks needed:
CommitDiff (L1-003), CommitTimeline (L1-007), CommitIntent (L1-009),
Complexity (L1-010), GitBlame (L1-012), CloneDetection (L1-013),
AiGeneratedCode (L1-014), Ownership (L1-015), SkillGraph (L1-017),
TrustScore (L1-018).

For CommitDiff: Diff-First analysis — file paths → capability taxonomy;
cross-validate commit messages vs structural diff patterns to detect intent conflicts.

For CommitTimeline: commit frequency score, bug-to-fix ratio, refactor initiative,
working hour distribution.

For CommitIntent: LLM inference using diff cache only — never commit messages
as primary signal.

For Complexity: deterministic L1-L6 taxonomy from file paths, compute Power Score.

For GitBlame: git blame top-changed files, per-file author ratios,
overall user authorship %.

For CloneDetection: heuristic — tutorial clones, commit bombs, no-dev-history.

For AiGeneratedCode: large single-burst commits, high single-touch file ratio,
intent conflicts.

For Ownership: blame×0.6 + commit_ratio×0.4 − bus_factor_penalty; module breakdown.

For SkillGraph: Skill Evidence Graph from tech stack + diff capability + LLM.

For TrustScore: Evidence×0.40 + Ownership×0.35 + Consistency×0.25
with adversarial flag penalties.
```

#### Kết quả AI trả về (tóm tắt)

```text
AI implemented all 10 analyze_* methods inside GitHubAnalysisOrchestrator:

1. analyze_commit_diff(): reads diff cache from meta.json, maps file paths
   through capability taxonomy, calls LLM to detect intent conflicts.

2. analyze_commit_timeline(): aggregates commit history by date/hour,
   classifies commits by type (bug/feat/refactor), computes temporal scores.

3. analyze_commit_intent(): reads CommitDiff result cache + diff text,
   calls LLM to infer developer intent without reading commit messages.

4. analyze_complexity(): iterates file paths through _COMPLEXITY_PATTERNS
   (priority-ordered, 30 entries), assigns L1-L6 level + capability label,
   computes weighted Power Score.

5. analyze_git_blame(): runs git blame on top-N changed files,
   parses author lines, computes per-file and aggregate ownership ratios.

6. analyze_clone_detection(): checks commit count, file similarity heuristics,
   burst commit patterns; flags repo as likely tutorial clone if triggered.

7. analyze_ai_generated_code(): detects single large commit introducing
   majority of codebase, high single-touch ratio, intent conflict from L1-003.

8. analyze_ownership(): combines blame output with commit ratio,
   applies bus_factor_penalty for single-author concentration,
   returns module-level breakdown dict.

9. analyze_skill_graph(): combines tech stack from meta.json with capability
   signals from CommitDiff result, calls LLM to produce Skill Evidence Graph nodes.

10. analyze_trust_score(): reads Evidence (from SkillGraph), Ownership,
    Consistency scores; applies formula with adversarial penalties;
    returns final TrustScore with component breakdown.

Added shared helpers and _COMPLEXITY_PATTERNS class attribute.
```

#### Đánh giá

```text
Sử dụng được — AI generated all 10 methods aligned with the spec.
Minor tuning needed for CloneDetection thresholds after manual testing.
```

---

### Prompt số 2

| Thông tin | Nội dung |
|---|---|
| Ngày sử dụng | 2026-06-12 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích | Implement token debug infrastructure |
| Loại prompt | Code generation |

#### Nội dung Prompt

```text
Add token debug infrastructure to CVerify.AI:

1. Add AI_DEBUG_TOKENS=false to .env.example under a new section
   "# 8. TOKEN COUNT DEBUGGING" with comments explaining:
   - Each LLM call writes a token_debug.jsonl line to temp_clones/{job_id}/
   - Readable via GET /api/v1/debug/tokens/{job_id}
   - Only enable in dev/staging, never production

2. Add ai_debug_tokens: bool = Field(False, validation_alias="AI_DEBUG_TOKENS")
   to Settings in config.py

3. In claude_service.py after telemetry calculation, add a writer block:
   - Only runs when settings.ai_debug_tokens=true and job_id is present
   - Resolves path: temp_clones/{job_id}/token_debug.jsonl
   - Appends JSONL record: ts, task, model, prompt_tokens, completion_tokens,
     cache_read_tokens, cache_write_tokens, total_tokens, estimated_cost_usd,
     duration_ms, mismatch_flag, prompt_preview (first 300 chars if debug_mode)
   - Wrap in try/except so failures are non-critical (log debug only)

4. Create debug/invoke-pipeline.ps1:
   - Accepts -JobId and -TaskType parameters
   - Reads HMAC secret from environment
   - Signs the request with HMAC-SHA256
   - Calls local API endpoint
   - Prints token usage summary table after response
```

#### Kết quả AI trả về (tóm tắt)

```text
AI implemented all 4 items:
- .env.example: 6 lines added with full comments and safety warning
- config.py: 1 line added to Settings class
- claude_service.py: 30-line token debug writer block with JSONL append,
  path resolution, and non-critical try/except error handling
- debug/invoke-pipeline.ps1: 182-line script with HMAC signing,
  param block, API call, and token summary table formatting
```

#### Đánh giá

```text
Sử dụng được — implementation matched the spec exactly.
JSONL append mode correctly handles concurrent writes from parallel tasks.
```

---

### Prompt số 3

| Thông tin | Nội dung |
|---|---|
| Ngày sử dụng | 2026-06-12 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích | Thực thi audit documentation workflow cho commit faaa877 |
| Loại prompt | Workflow execution |

#### Nội dung Prompt

```text
/cverify-code-to-ai-audit bỏ, không audit những phần liên quan tới commit của Line 2
mà chỉ làm cho commit faaa877
```

#### Kết quả AI trả về (tóm tắt)

```text
AI executed the audit-only portion of the cverify-code-to-ai-audit skill:
- Inspected commit faaa877 diff (5 files, +1185 lines)
- Identified the next sequential audit folder as #6
- Read previous audit packages (#5) for formatting consistency
- Generated AI_AUDIT_LOG.md, CHANGELOG.md, PROMPTS.md, REFLECTION.md
- Committed all 4 files to docs/Tran Nhat Long/#6/ on doc/Tran-Nhat-Long branch
- Created audit PR targeting main with reviewers nhnnanh, LucFr1746
```

#### Đánh giá

```text
Sử dụng được — AI correctly scoped to single commit faaa877,
maintained consistent formatting with previous audit packages.
```
