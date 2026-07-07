# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

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
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-06-12 |
| Ngày hoàn thành | 2026-06-12 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 07 — feat: Re-apply v2 Pipeline Tasks & Token Debug | 2026-06-12 | Re-apply commit 10f43487f sau khi faaa877b8 bị revert (b89192101) | Completed |

---

# [Phase 07] feat: Re-apply v2 Pipeline Tasks & Token Debug Infrastructure

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-12
- **Mô tả giai đoạn:** Re-application of the v2 pipeline task infrastructure after the original commit (faaa877b8) was reverted via b89192101 due to branch integration issues on AI-Feature-uat. Commit 10f43487f restores the identical 5-file changeset (+1185 lines) at the correct branch history position.
- **Trạng thái hiện tại:** Completed
- **Commit:** 10f43487f2b334656ac9d8b9c6056b0e86c327d9

## Lịch sử commit liên quan

| Commit | Mô tả | Ngày |
|---|---|---|
| faaa877b8 | feat: implement v2 pipeline tasks and token debug infrastructure (original) | 2026-06-12 |
| b89192101 | Revert "feat: implement v2 pipeline tasks and token debug infrastructure" | 2026-06-12 |
| 10f43487f | feat: implement v2 pipeline tasks and token debug infrastructure (re-apply) | 2026-06-12 |

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Re-apply `AI_DEBUG_TOKENS=false` flag vào `.env.example` với hướng dẫn sử dụng và cảnh báo không dùng production | Trần Nhất Long | `CVerify.AI/.env.example` | Commit 10f43487f |
| 2 | Re-apply `ai_debug_tokens: bool` field vào `Settings` class trong `config.py` | Trần Nhất Long | `CVerify.AI/app/core/config.py` | Commit 10f43487f |
| 3 | Re-apply per-job token debug JSONL writer vào `claude_service.py`: khi `AI_DEBUG_TOKENS=true`, mỗi LLM call sẽ append 1 dòng JSONL vào `temp_clones/{job_id}/token_debug.jsonl` | Trần Nhất Long | `CVerify.AI/app/core/services/claude_service.py` | Commit 10f43487f |
| 4 | Re-apply 10 task methods mới trong `github_analysis_orchestrator.py`: `analyze_commit_diff`, `analyze_commit_timeline`, `analyze_commit_intent`, `analyze_complexity`, `analyze_git_blame`, `analyze_clone_detection`, `analyze_ai_generated_code`, `analyze_ownership`, `analyze_skill_graph`, `analyze_trust_score` | Trần Nhất Long | `CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | Commit 10f43487f |
| 5 | Re-apply dispatch routing cho 10 task types mới vào switch trong `run_task()` | Trần Nhất Long | `CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | Commit 10f43487f |
| 6 | Re-apply shared helpers: `_read_meta()`, `_read_task_cache()`, `_clone_dir()`, `_empty_result()` và `_COMPLEXITY_PATTERNS` class-level pattern table (30 entries, L1-L6) | Trần Nhất Long | `CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | Commit 10f43487f |
| 7 | Re-apply `debug/invoke-pipeline.ps1`: HMAC-signed task invoker với token usage summary output (182 lines) | Trần Nhất Long | `CVerify.AI/debug/invoke-pipeline.ps1` | Commit 10f43487f |

## AI có hỗ trợ không?

| | Có | Không |
|---|---|---|
| AI hỗ trợ việc này | ✅ | |

**Chi tiết:** Claude (Claude Code) đã implement toàn bộ changeset trong phiên trước (faaa877b8) và generate audit documentation pack #7 cho re-commit này. Developer tự quyết định re-apply sau revert và xác nhận changeset không thay đổi.

## Lỗi / vấn đề phát sinh

| STT | Lỗi / Vấn đề | Nguyên nhân | Cách xử lý |
|---:|---|---|---|
| 1 | Commit faaa877b8 bị revert (b89192101) | Branch integration conflict trên AI-Feature-uat khi merge với upstream changes | Re-apply cùng changeset sau khi upstream conflicts được giải quyết |
| 2 | Audit pack #6 đã tồn tại cho faaa877b8 | Re-commit sau revert tạo ra commit hash mới (10f43487f) cần audit riêng | Tạo audit pack #7 riêng biệt để duy trì lịch sử audit đầy đủ |

## Kết quả đạt được

```text
- Changeset được re-apply thành công tại commit 10f43487f
- 10/10 v2 pipeline tasks available on AI-Feature-uat branch
- Token debug infrastructure restored: AI_DEBUG_TOKENS=true viết JSONL per job
- HMAC-signed debug invoker script available at debug/invoke-pipeline.ps1
- Audit pack #7 documenting the re-application decision and history
```
