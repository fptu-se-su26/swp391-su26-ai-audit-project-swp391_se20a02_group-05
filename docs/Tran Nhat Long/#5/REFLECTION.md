# AI Learning Reflection

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
| Ngày hoàn thành reflection | 2026-06-04 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển phần mềm.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Phiên làm việc 2026-06-04 tập trung vào debug và bugfix form work experience:

1. Developer tự phát hiện và sửa bug (z.coerce.number() chuyển "" thành 0).
2. Dùng Claude Code để phân tích git diff và mô tả lại thay đổi bằng ngôn ngữ rõ ràng.
3. Dùng Claude Code để sinh commit message theo conventional commit format.
4. Dùng Claude Code thực thi workflow 16 bước: commit → push → PR → assign → audit docs.

Vai trò của AI trong phiên này: documentation assistant và workflow executor,
không phải code generator. Developer là người phát hiện và sửa bug.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Claude (Claude Code CLI — claude-sonnet-4-6)
```

### Lý do sử dụng công cụ đó

```text
Claude Code tích hợp trực tiếp với git, GitHub CLI, và file system — cho phép
AI đọc git diff, tạo commit, push branch, và tạo PR mà không cần copy-paste thủ công.
Phù hợp với workflow phát triển phần mềm hơn so với chatbot web.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [ ] Hiểu yêu cầu đề bài
- [ ] Phân tích bài toán
- [ ] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [ ] Thiết kế kiến trúc hệ thống
- [ ] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
- [ ] Review code
- [ ] Tối ưu code
- [ ] Kiểm tra bảo mật
- [x] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
Debug: AI phân tích git diff và xác định root cause (z.coerce.number() coercion)
từ code đã được sửa, giúp articulate vấn đề rõ ràng hơn.

Viết báo cáo: AI sinh toàn bộ audit documentation package (4 files) dựa trên
implementation context được lưu trong session.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
- Articulation: AI giúp diễn đạt vấn đề kỹ thuật (NaN coercion, Zod schema pattern)
  bằng ngôn ngữ rõ ràng, giúp developer hiểu sâu hơn về quyết định mình đưa ra.
- Workflow automation: AI thực thi quy trình lặp lại (commit → PR → audit) nhanh
  và nhất quán, để developer tập trung vào logic nghiệp vụ.
- Documentation habit: Workflow bắt buộc ghi lại prompts và decisions — thói quen
  tốt cho long-term learning.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI không thể tự tạo feature branch riêng khi commit đã push lên CVerify-uat trực tiếp.
  Workflow bị adjust để dùng PR #54 đã có thay vì tạo branch mới.
- AI không biết business constraint (enum value bắt đầu từ 1 hay 0) nếu không được
  cung cấp — cần developer confirm.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Developer tự phát hiện và sửa bug. AI chỉ hỗ trợ documentation và workflow execution.
Nếu không có AI, developer vẫn có thể commit và tạo PR thủ công — chỉ tốn nhiều thời gian hơn.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [ ] Chạy thử chương trình
- [x] Kiểm tra output
- [ ] Viết test case
- [x] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
1. Review git diff để xác nhận AI phân tích đúng 3 file thay đổi.
2. Đọc commit message AI sinh ra và xác nhận phản ánh đúng thay đổi thực tế.
3. Kiểm tra PR #54 trên GitHub: body template, reviewer, labels.
4. Kiểm tra docs/Tran Nhat Long/#5/ có đủ 4 file với format đúng.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Schema dùng z.union([z.undefined(), z.number()]) + .refine() thay vì z.coerce.number().min(1) |
| Em/nhóm đã kiểm tra bằng cách nào? | Review git diff và so sánh với Zod documentation |
| Kết quả kiểm tra | Đúng — pattern này là idiomatic Zod cho optional enum fields |
| Em/nhóm đã xử lý tiếp như thế nào? | Sử dụng trực tiếp trong commit; không cần chỉnh sửa |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Prompt đầu tiên trả về summary+description gộp, không phân tách rõ |
| Vì sao gợi ý đó sai/chưa phù hợp? | Developer cần 2 phần tách biệt để paste vào git commit message |
| Em/nhóm phát hiện bằng cách nào? | Đọc output và nhận ra format không dùng được trực tiếp |
| Em/nhóm đã sửa như thế nào? | Gửi follow-up prompt clarify format |
| Bài học rút ra | Specify output format ngay trong prompt đầu tiên |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

### [Đóng góp] Fix work experience dropdown validation bug
- **Thành viên:** Trần Nhất Long (DE200160)
- **Minh chứng:** Commit f9dc89830, PR #54
- **Đánh giá AI:** AI hỗ trợ documentation, không sinh code fix
- **Loại đóng góp thật sự:** Developer-led (AI chỉ hỗ trợ tài liệu hóa)
- **Chi tiết thực hiện:** Phân tích root cause, refactor Zod schema, fix SelectDropdown binding, cleanup submit handler.

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Viết commit message | Tự viết mất 5–10 phút, có thể không đúng conventional format | AI sinh trong vài giây, đúng format | Tiết kiệm thời gian, nhất quán hơn |
| Tạo PR + assign metadata | Thủ công trên GitHub UI, dễ bỏ sót reviewer/label | AI thực hiện qua gh CLI, đầy đủ và đúng | Giảm human error |
| Viết audit documentation | Mất 30–60 phút điền 4 file template | AI sinh trong vài phút | Tăng tốc đáng kể, giữ được format nhất quán |

---

## 11. Bài học về môn học

```text
- Debugging form validation cần hiểu rõ cách library xử lý type coercion (Zod, react-hook-form).
- Zod schema design: field-level validation vs schema-level .refine() có trade-off khác nhau.
- Git workflow quan trọng: commit trực tiếp lên branch chia sẻ (CVerify-uat) thay vì
  tạo feature branch riêng gây khó khăn khi áp dụng workflow PR-based.
- Documentation workflow nên được tích hợp ngay vào development loop, không phải làm sau.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
- AI là documentation assistant hiệu quả khi developer đã hiểu rõ thay đổi của mình.
- Không để AI sinh code fix rồi commit mà không review — đặc biệt với form validation
  vì bug có thể ảnh hưởng data integrity.
- Ghi lại prompts ngay khi dùng, không để cuối ngày mới ghi — nhiều chi tiết bị quên.
- Luôn review output của AI (commit message, PR body, audit docs) trước khi push.
- Workflow automation (@file) tiết kiệm thời gian nhưng developer vẫn phải approve
  từng action có side effect (push, PR creation).
```

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

### Giải thích thêm nếu có

```text
Trong phiên này, AI không sinh code fix — developer tự viết và AI chỉ mô tả lại.
Đây là cách dùng AI có trách nhiệm nhất: AI amplify productivity, không replace thinking.
```

---

## 14. Kế hoạch cải thiện lần sau

```text
1. Tạo feature branch trước khi commit thay vì commit trực tiếp lên CVerify-uat.
   Giúp workflow áp dụng đúng 100% theo quy trình code-to-ai-audit-log.

2. Specify output format ngay trong prompt đầu tiên để tránh follow-up không cần thiết.

3. Viết test case cho Zod schema validation để catch regression sớm.

4. Ghi prompt log ngay khi dùng, không để cuối phiên mới ghi.
```

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 | Ghi đúng AI dùng để làm gì, không làm gì |
| Prompt có mục tiêu rõ ràng | 4 | Prompt 1 cần follow-up để clarify format |
| Kiểm chứng kết quả AI | 5 | Review diff, PR, và audit files trước khi push |
| Tự chỉnh sửa/cải tiến | 4 | Code fix do developer viết; docs do AI sinh |
| Hiểu nội dung đã nộp | 5 | Developer có thể giải thích toàn bộ thay đổi |
| Reflection có chiều sâu | 4 | |
| Sử dụng AI có trách nhiệm | 5 | AI không sinh code; developer review mọi action |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Được. AI hỗ trợ: phân tích git diff, sinh commit message, update PR, gán reviewer/labels,
và sinh 4 file audit documentation. Code fix do developer tự viết.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Được. Bug fix (Zod schema + SelectDropdown binding) là công việc của developer.
Documentation và PR có thể làm thủ công — chỉ tốn nhiều thời gian hơn.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phát hiện root cause (z.coerce.number() → 0) và chọn đúng pattern fix
(z.union + .refine thay vì .min(1)) — đây là technical judgment của developer,
không phải AI.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
- Viết test case cho form validation schema (Zod + react-hook-form).
- Git branching discipline: luôn tạo feature/bugfix branch thay vì commit trực tiếp
  lên shared branch.
- Prompt engineering: specify format constraints ngay từ đầu.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trần Nhất Long | 04/06/2026 |
