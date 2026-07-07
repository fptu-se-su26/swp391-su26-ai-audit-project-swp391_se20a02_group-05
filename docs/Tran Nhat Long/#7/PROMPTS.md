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
| Mục đích | Tạo audit documentation package #7 cho commit 10f43487f (re-apply v2 pipeline tasks sau revert) |
| Loại prompt | Workflow execution |

#### Nội dung Prompt

```text
/cverify-code-to-ai-audit tạo audit pack mới cho commit 10f43487f — v2 pipeline task infrastructure
```

#### Kết quả AI trả về (tóm tắt)

```text
AI executed the cverify-code-to-ai-audit skill with audit-only scope for commit 10f43487f:

1. Read the SKILL.md from ~/.claude/skills/ecc/cverify-code-to-ai-audit/SKILL.md
2. Inspected commit 10f43487f via `git show 10f43487f --stat` and `--format`
   - 5 files changed, +1185 insertions
   - Same changeset as faaa877b8 (re-applied after revert b89192101)
3. Confirmed GitHub user: TNL293107 → doc branch: doc/Tran-Nhat-Long
4. Switched to doc/Tran-Nhat-Long branch
5. Identified existing audit folder #6 → created #7
6. Read #6 docs for formatting consistency
7. Generated 4 audit files:
   - AI_AUDIT_LOG.md: 3 AI sessions documented (2 from faaa877b8 original +
     1 for this re-commit audit workflow)
   - CHANGELOG.md: Phase 07 with commit history table (faaa877 → revert → 10f43487f)
   - PROMPTS.md: this file
   - REFLECTION.md: reflection on revert-and-reapply workflow pattern
8. Committed to docs/Tran Nhat Long/#7/ on doc/Tran-Nhat-Long
9. Created audit PR targeting main with labels documentation, audit
   and reviewers nhnnanh, LucFr1746
```

#### Đánh giá

```text
Sử dụng được — AI correctly handled the re-commit scenario:
- Recognized that 10f43487f carries the same changeset as the previously
  audited faaa877b8
- Created a distinct #7 rather than overwriting #6, preserving audit history
- Documented the revert → re-apply sequence in CHANGELOG and REFLECTION
```
