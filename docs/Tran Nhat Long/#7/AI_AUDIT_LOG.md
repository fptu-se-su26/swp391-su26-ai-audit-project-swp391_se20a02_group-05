# AI Audit Log

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
| Ngày hoàn thành | 2026-06-12 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Re-apply v2 pipeline task infrastructure (commit 10f43487f) after the previous
implementation (faaa877b8) was reverted via commit b89192101.

The revert was necessitated by branch integration issues on AI-Feature-uat.
This re-commit restores the identical 5-file changeset (+1185 lines) to the
correct point in the branch history.

AI was used to generate the audit documentation for this re-commit,
reflecting the re-application decision and the audit context correctly.
```

---

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-12 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích sử dụng | Tạo audit documentation package #7 cho commit 10f43487f — re-application của v2 pipeline task infrastructure sau khi bị revert. |
| Phần việc liên quan | Documentation / Workflow |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
/cverify-code-to-ai-audit tạo audit pack mới cho commit 10f43487f — v2 pipeline task infrastructure
```

#### 4.2. Kết quả AI gợi ý

```text
AI executed the audit documentation workflow for commit 10f43487f:

- Inspected commit 10f43487f diff (same 5 files, +1185 lines as faaa877b8)
- Identified that audit pack #6 already existed for faaa877b8 (the original commit)
- Identified the next sequential audit folder as #7
- Read previous audit packages (#6) for formatting consistency
- Generated AI_AUDIT_LOG.md, CHANGELOG.md, PROMPTS.md, REFLECTION.md for #7
- Noted the revert-and-reapply context (faaa877 → revert b89192101 → 10f43487f)
- Committed all 4 files to docs/Tran Nhat Long/#7/ on doc/Tran-Nhat-Long branch
- Created audit PR targeting main with reviewers nhnnanh, LucFr1746
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Complete audit documentation package (#7): AI_AUDIT_LOG.md, CHANGELOG.md,
  PROMPTS.md, REFLECTION.md
- Commit message and PR creation for the audit PR
- Branch management: switching to doc/Tran-Nhat-Long, staging, push
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Decided to create a separate audit pack (#7) for the re-commit rather than
  amending audit pack #6, to preserve a complete historical record in the audit log.
- Verified that commit 10f43487f carries the same changeset as faaa877b8
  (same 5 files, +1185 lines) so the implementation description is identical.
- Confirmed the revert commit hash (b89192101) and the reason for revert before
  including it in the audit documentation.
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp — AI correctly identified the re-commit scenario, noted the prior audit
pack #6, and generated a distinct #7 that accurately reflects the workflow:
original implementation → revert → re-application.
```
