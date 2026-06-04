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
| Ngày bắt đầu | 2026-06-04 |
| Ngày hoàn thành | 2026-06-04 |

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
Debug and fix a form validation bug in the work experience settings page.
AI was used to summarize changes, generate a semantic commit message,
write the pull request description, and execute the full audit documentation workflow.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-04 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích sử dụng | Phân tích và tóm tắt các thay đổi trong ngày trên branch CVerify-uat, xác định lỗi dropdown binding trong form work experience, sinh commit message và mô tả PR. |
| Phần việc liên quan | Debug / Frontend |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
give me a summary and description of what i changed/created on this branch today
```

#### 4.2. Kết quả AI gợi ý

```text
AI analyzed the git diff and identified 3 modified files:
- types.ts: Zod schema refactor for experienceCategory and employmentType
- ExperienceAchievementsSection.tsx: SelectDropdown value/onChange binding fix
- PersonalInfoTab.tsx: Removal of redundant Number() casts

AI generated a clear technical summary explaining the root cause (coercing "" to 0
instead of undefined) and the fix (z.union + .refine guards).
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Root cause analysis: AI correctly identified that z.coerce.number() was converting
  empty string to 0, causing silent validation failures.
- Commit message: Used AI-generated semantic commit message as-is.
- PR description: Used AI-generated body and populated the PR template sections.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- The actual code fix was written by the developer; AI only described what changed.
- PR template sections (Verification, Evidence, Checklist) were reviewed and confirmed
  manually before submission.
- Type of change checkboxes (Frontend + Debug) were selected by the developer.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | f9dc89830 | fix: work experience dropdown binding and validation schema |
| Pull Request | PR #54 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/pull/54 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Claude accurately diagnosed the NaN/0 coercion problem from reading the diff alone.
The suggested schema pattern (z.union + cross-field .refine) is idiomatic Zod and
produces clearer error messages than the previous in-field .min(1) approach.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-04 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích sử dụng | Thực thi workflow tạo source code PR, gán reviewer/label, và sinh tài liệu AI audit theo đúng quy trình code-to-ai-audit-log. |
| Phần việc liên quan | Documentation / Audit |
| Mức độ sử dụng | AI sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
@"D:\downloads\code-to-ai-audit-log.md"
read this workflow and do each steps
```

#### 4.2. Kết quả AI gợi ý

```text
AI executed all 16 steps of the workflow:
- Steps 1-2: Validated invocation and confirmed CVerify project changes.
- Steps 3-4: Generated commit message (fix:) and determined branch type (bugfix/).
- Steps 5-6: Changes already committed to CVerify-uat; commit pushed to remote.
- Steps 7-8: Updated PR #54 with full template body, assigned reviewer LucFr1746,
  assignee TNL293107, labels bug + frontend.
- Steps 9-10: Stored implementation context; resolved doc identity (doc/Tran-Nhat-Long).
- Steps 11-13: Reviewed existing #1–#4 audit folders; generated #5 audit package.
- Steps 14-16: Committed and pushed audit docs; created audit PR targeting main.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Full audit documentation package (AI_AUDIT_LOG.md, CHANGELOG.md, PROMPTS.md, REFLECTION.md)
- PR creation and metadata assignment (reviewer, assignee, labels)
- Commit and push operations on doc/Tran-Nhat-Long branch
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Developer reviewed and approved all steps before execution.
- Confirmation of documentation identity (branch name, folder name) provided by developer.
- Type of change selection and reflection content validated by developer.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Branch | doc/Tran-Nhat-Long | Audit documentation branch |
| Folder | docs/Tran Nhat Long/#5 | This audit package |

#### 4.6. Nhận xét cá nhân/nhóm

```text
The workflow execution was smooth. AI correctly identified the doc branch and
existing folder numbering (#1–#4), then created #5 without gaps or duplicates.
The most valuable part was AI maintaining the Vietnamese document format and
structure consistent with previous audit entries.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Debug / phân tích lỗi |   |   | x |   | AI phân tích từ git diff |
| Viết commit message |   |   |   | x |   |
| Viết PR description |   |   |   | x |   |
| Viết tài liệu audit |   |   |   | x |   |
| Viết code fix | x |   |   |   | Developer tự viết |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI không tự tạo feature branch riêng vì commit đã được push lên CVerify-uat trực tiếp trước khi workflow chạy. | Review bước 5-6 của workflow. | Chấp nhận PR #54 đã tồn tại là source code PR; workflow tiếp tục từ bước assign metadata. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Developer reviewed the generated audit files for accuracy of dates, commit hashes,
PR links, and technical descriptions before pushing to the doc branch.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Trần Nhất Long (DE200160): Fixed the work experience dropdown binding bug independently.
Identified the NaN coercion issue, refactored the Zod schema, corrected the SelectDropdown
binding, and cleaned up the submit handler. Used Claude Code to document and commit the work.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trần Nhất Long | DE200160 | Fix work experience form validation bug | Có | Commit f9dc89830, PR #54 |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trần Nhất Long | 04/06/2026 |
