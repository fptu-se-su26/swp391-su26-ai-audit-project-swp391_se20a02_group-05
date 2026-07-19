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
| Phase 06 — feat: v2 Pipeline Tasks & Token Debug | 2026-06-12 | Implement 10 v2 pipeline task methods (L1-003…L1-018) và token debug infrastructure cho CVerify.AI | Completed |

---

# [Phase 06] feat: v2 Pipeline Tasks & Token Debug Infrastructure

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-12
- **Mô tả giai đoạn:** Rewrites the repository scanning pipeline to align the Python AI service with the v2 18-task DAG defined in CVerify.Core DagScheduler. Adds 10 new task method implementations (L1-003 through L1-018) and a token debug observability system.
- **Trạng thái hiện tại:** Completed
- **Commit:** faaa877b8926e1290b8ff858da1efa6b6d6b324b

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thêm `AI_DEBUG_TOKENS=false` flag vào `.env.example` với hướng dẫn sử dụng và cảnh báo không dùng production | Trần Nhất Long | `CVerify.AI/.env.example` | Commit faaa877 |
| 2 | Thêm `ai_debug_tokens: bool` field vào `Settings` class trong `config.py` | Trần Nhất Long | `CVerify.AI/app/core/config.py` | Commit faaa877 |
| 3 | Thêm per-job token debug JSONL writer vào `claude_service.py`: khi `AI_DEBUG_TOKENS=true`, mỗi LLM call sẽ append 1 dòng JSONL vào `temp_clones/{job_id}/token_debug.jsonl` | Trần Nhất Long | `CVerify.AI/app/core/services/claude_service.py` | Commit faaa877 |
| 4 | Implement 10 task methods mới trong `github_analysis_orchestrator.py`: `analyze_commit_diff`, `analyze_commit_timeline`, `analyze_commit_intent`, `analyze_complexity`, `analyze_git_blame`, `analyze_clone_detection`, `analyze_ai_generated_code`, `analyze_ownership`, `analyze_skill_graph`, `analyze_trust_score` | Trần Nhất Long | `CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | Commit faaa877 |
| 5 | Thêm dispatch routing cho 10 task types mới vào switch trong `run_task()` | Trần Nhất Long | `CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | Commit faaa877 |
| 6 | Thêm shared helpers: `_read_meta()`, `_read_task_cache()`, `_clone_dir()`, `_empty_result()` và `_COMPLEXITY_PATTERNS` class-level pattern table (30 entries, L1-L6) | Trần Nhất Long | `CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | Commit faaa877 |
| 7 | Tạo `debug/invoke-pipeline.ps1`: HMAC-signed task invoker với token usage summary output (182 lines) | Trần Nhất Long | `CVerify.AI/debug/invoke-pipeline.ps1` | Commit faaa877 |

## AI có hỗ trợ không?

| | Có | Không |
|---|---|---|
| AI hỗ trợ việc này | ✅ | |

**Chi tiết:** Claude (Claude Code) hỗ trợ implement toàn bộ 10 task methods (~900 lines), shared helpers, token debug writer, PowerShell debug script, và audit documentation. Developer tự thiết kế DAG spec và kiểm tra alignment với `CVerify.Core DagScheduler`.

## Lỗi / vấn đề phát sinh

| STT | Lỗi / Vấn đề | Nguyên nhân | Cách xử lý |
|---:|---|---|---|
| 1 | CloneDetection false positive trên educational repos | Thresholds ban đầu quá thấp cho commit bomb detection | Tune lại threshold sau khi test với sample repos |
| 2 | token_debug.jsonl cần append-only thay vì JSON | Parallel task execution có thể race-condition nếu dùng JSON rewrite | Dùng JSONL format với `open("a")` append mode |

## Kết quả đạt được

```text
- 10/10 v2 pipeline tasks implemented và mapped vào DAG dispatcher
- Token debug infrastructure hoạt động: AI_DEBUG_TOKENS=true viết JSONL per job
- HMAC-signed debug invoker script sẵn sàng cho dev/staging testing
- CVerify.AI Python service align với v2 18-task DAG của CVerify.Core
- Total additions: 1,185 lines across 5 files
```
