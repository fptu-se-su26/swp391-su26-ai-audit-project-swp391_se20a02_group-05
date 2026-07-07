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
| Ngày bắt đầu | 2026-06-04 |
| Ngày cập nhật gần nhất | 2026-06-04 |

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
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 2026-06-04 | Claude Code | Tóm tắt thay đổi trong ngày trên branch | "give me a summary and description of what i changed/created on this branch today" | Phân tích git diff, xác định 3 file thay đổi, mô tả root cause và fix | Có | Commit f9dc89830 |
| 2 | 2026-06-04 | Claude Code | Sinh commit message dạng summary + description để paste vào git | "no, separate between summary and description. I want to paste on my git commit" | Summary 1 dòng + Description block dạng conventional commit | Có | Commit f9dc89830 |
| 3 | 2026-06-04 | Claude Code | Tạo source code PR | `/create-pr` command | PR #54 được tạo/cập nhật với full template | Có | PR #54 |
| 4 | 2026-06-04 | Claude Code | Chọn type of change cho PR | Checklist selection (Frontend + Debug) | Checkbox được chọn đúng | Có | PR #54 body |
| 5 | 2026-06-04 | Claude Code | Thực thi toàn bộ workflow code-to-ai-audit-log | `@"D:\downloads\code-to-ai-audit-log.md" read this workflow and do each steps` | PR updated, reviewer/labels assigned, audit package #5 generated | Có | docs/Tran Nhat Long/#5 |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-04 |
| Công cụ AI | Claude Code (claude-sonnet-4-6) |
| Mục đích | Phân tích git diff để tóm tắt thay đổi trong ngày |
| Phần việc liên quan | Debug / Documentation |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 5.1. Prompt nguyên văn

```text
give me a summary and description of what i changed/created on this branch today
```

#### 5.2. Bối cảnh khi viết prompt

```text
Developer đã sửa xong bug nhưng chưa có commit. Cần tóm tắt các thay đổi để
chuẩn bị viết commit message và PR description.
```

#### 5.3. Kết quả AI trả về

```text
AI phân tích git diff --stat và git diff HEAD, xác định 3 file bị thay đổi:
- types.ts: schema refactor (z.union + .refine)
- ExperienceAchievementsSection.tsx: SelectDropdown binding fix
- PersonalInfoTab.tsx: remove redundant Number() cast

AI giải thích root cause (z.coerce.number() chuyển "" thành 0) và mô tả
từng thay đổi với đường dẫn file cụ thể.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Summary và description được dùng làm commit message cho commit f9dc89830
và làm nội dung cho section Description trong PR #54.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Developer tự viết code fix trước khi hỏi AI. AI chỉ đóng vai trò phân tích
và mô tả lại những gì developer đã làm — không sinh code.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Commit | f9dc89830 |

#### 5.8. Ghi chú thêm

```text
Prompt ngắn nhưng AI hiểu ngữ cảnh vì đang chạy trong Claude Code CLI,
có quyền đọc git log và git diff trực tiếp.
```

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-04 |
| Công cụ AI | Claude Code (claude-sonnet-4-6) |
| Mục đích | Sinh commit message tách thành summary + description để paste git |
| Phần việc liên quan | Documentation |
| Mức độ sử dụng | AI sinh chính nội dung |

#### 5.1. Prompt nguyên văn

```text
no, separate between summary and description. I want to paste on my git commit
```

#### 5.2. Bối cảnh khi viết prompt

```text
AI trả về tóm tắt chung chưa phân tách rõ summary và description.
Developer cần format đúng để copy paste vào git commit message.
```

#### 5.3. Kết quả AI trả về

```text
Summary (1 dòng):
  fix: work experience dropdown binding and validation schema

Description (paragraph):
  Changed experienceCategory and employmentType fields in workExperienceEntrySchema
  from z.coerce.number().min(1) to z.union([z.undefined(), z.number()])...
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Dùng làm commit message cho commit f9dc89830 (có thêm Co-Authored-By footer).
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Không chỉnh sửa — message phản ánh đúng thay đổi thực tế.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Commit | f9dc89830 |

#### 5.8. Ghi chú thêm

```text
Follow-up prompt để clarify format. Cho thấy tầm quan trọng của việc
chỉ định output format ngay từ đầu.
```

---

### Prompt số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-04 |
| Công cụ AI | Claude Code (claude-sonnet-4-6) |
| Mục đích | Thực thi đầy đủ 16 bước workflow audit documentation |
| Phần việc liên quan | Documentation / Audit |
| Mức độ sử dụng | AI sinh chính nội dung |

#### 5.1. Prompt nguyên văn

```text
@"D:\downloads\code-to-ai-audit-log.md"
read this workflow and do each steps
```

#### 5.2. Bối cảnh khi viết prompt

```text
Developer cung cấp file workflow mô tả 16 bước từ validate file changes đến
tạo audit PR. AI cần đọc và thực thi từng bước theo đúng thứ tự.
```

#### 5.3. Kết quả AI trả về

```text
AI thực thi toàn bộ 16 bước:
- Validated file changes (CVerify project ✓)
- Generated commit message: fix: work experience dropdown binding and validation schema
- Branch type: bugfix/
- Committed and pushed to CVerify-uat
- Updated PR #54 with full template body
- Assigned reviewer LucFr1746, assignee TNL293107, labels bug+frontend
- Identified doc identity: doc/Tran-Nhat-Long / docs/Tran Nhat Long/
- Reviewed audit folders #1–#4 for formatting
- Generated audit package #5 (4 files)
- Committed and pushed to doc/Tran-Nhat-Long
- Created audit PR targeting main
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
PR #54 updated, audit package #5 committed to doc/Tran-Nhat-Long branch.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Developer xác nhận documentation identity (branch/folder name).
Developer review từng bước trước khi AI thực thi các hành động ảnh hưởng
đến remote repository (push, PR creation).
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Pull Request | PR #54 — source code |
| Branch | doc/Tran-Nhat-Long |
| Folder | docs/Tran Nhat Long/#5 |

#### 5.8. Ghi chú thêm

```text
Prompt dạng workflow reference (@file) rất hiệu quả cho quy trình lặp lại.
AI đọc file, hiểu đúng 16 bước, và thực thi đúng thứ tự mà không cần hướng
dẫn thêm. Tiết kiệm đáng kể thời gian so với thực hiện thủ công.
```

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
@"D:\downloads\code-to-ai-audit-log.md"
read this workflow and do each steps
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này kích hoạt toàn bộ quy trình từ source code commit đến audit documentation,
bao gồm 16 bước tự động hóa. Nó thể hiện cách dùng AI như một workflow executor
thay vì chỉ là Q&A assistant.
```

### 6.3. Kết quả prompt này mang lại

```text
PR source code đầy đủ template, reviewer/labels assigned, và audit package #5
hoàn chỉnh — tất cả trong một lần thực thi.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Review PR #54 trên GitHub để xác nhận body template đúng.
Kiểm tra docs/Tran Nhat Long/#5/ để xác nhận 4 file được tạo đúng format.
Xác nhận reviewer LucFr1746 và labels bug+frontend được gán.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Developer xác nhận và approve từng action thay vì để AI chạy hoàn toàn tự động.
Điều này đảm bảo không có push ngoài ý muốn đến remote repository.
```

---

## 7. Prompt chưa hiệu quả

```text
Prompt số 1 ban đầu trả về summary + description gộp lại, chưa phân tách rõ.
Cần follow-up prompt số 2 để clarify format. Bài học: specify output format ngay
trong prompt đầu tiên.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
- Output format cụ thể (summary vs description, Vietnamese vs English)
- Scope của task (1 bước hay toàn bộ workflow)
- Context file đính kèm (@file) khi có quy trình phức tạp
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Dùng file reference (@file) để cung cấp workflow dài thay vì paste trực tiếp
giúp prompt gọn hơn và dễ tái sử dụng cho lần sau.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Ngay trong prompt đầu tiên, chỉ định "format: [summary line] + [description block]"
để tránh cần follow-up. Tiết kiệm 1 round-trip với AI.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt phân tích/tóm tắt | 1 | "give me a summary and description..." |
| Prompt format/output | 1 | "no, separate between summary and description..." |
| Prompt workflow execution | 1 | "@file read this workflow and do each steps" |

---

## 10. Checklist chất lượng prompt

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | x | |
| Prompt có đủ bối cảnh | x | Claude Code có quyền đọc git state |
| Tự kiểm tra và chỉnh sửa | x | Review kết quả trước khi push |

---

## 11. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trần Nhất Long | 04/06/2026 |
