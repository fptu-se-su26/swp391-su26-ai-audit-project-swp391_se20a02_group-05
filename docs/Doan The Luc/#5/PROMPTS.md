# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Reclaim Organization Ownership                                               |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-05-28T00:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-05-29                                                                             |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày       | Công cụ AI          | Mục đích                                                | Prompt tóm tắt                                | Kết quả chính                                                    | Có sử dụng vào bài không? | Minh chứng    |
| --: | ---------- | ------------------- | ------------------------------------------------------- | --------------------------------------------- | ---------------------------------------------------------------- | ------------------------- | ------------- |
|   1 | 2026-05-28 | Gemini, Antigravity | Fix Reclaim Organization Ownership OTP verification bug | Fix Reclaim Ownership OTP Verification Bug... | AI proposed identity normalization helpers & client wizard cache | Có                        | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung            | Thông tin                                                                                                                                                                                 |
| ------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-28                                                                                                                                                                                |
| Công cụ AI          | Gemini, Antigravity                                                                                                                                                                       |
| Mục đích            | Fix Reclaim Organization Ownership OTP verification bug (token invalid/expired on submission) and improve robustness of multi-step reclaim flows with proper email/taxCode normalization. |
| Phần việc liên quan | Backend / Frontend / Testing / Debug                                                                                                                                                      |
| Mức độ sử dụng      | Hỏi sinh code                                                                                                                                                                             |

#### 5.1. Prompt nguyên văn

```text
# Fix Reclaim Ownership OTP Verification Bug

Investigate and fix the bug in the **Reclaim Organization Ownership** workflow where the final reclaim submission fails with:

> "Email OTP Verification token is invalid or has expired."

even when the OTP was recently verified successfully.

The issue occurs after:

* OTP email is sent successfully
* User enters correct OTP
* Verification succeeds
* User uploads legal ownership evidence
* Final reclaim submission fails with token invalid/expired

Perform a full investigation across both frontend and backend to identify the real root cause.

Check:

* OTP verification lifecycle
* verification session persistence
* token expiration handling
* frontend state resets/re-renders
* multi-step workflow state management
* file upload side effects
* request payload construction
* backend validation flow
* Redis/cache/session storage
* auth middleware
* race conditions
* navigation/back button behavior
* refresh handling
* React StrictMode effects
* stale state or invalidated sessions

Fix the issue properly without hacks or bypasses.

Requirements:

* Ensure verification state persists correctly through the entire reclaim workflow
* Ensure uploaded files or page transitions do not invalidate verification state
* Ensure the final reclaim request uses the correct verified session/token
* Ensure expiration timing is calculated correctly and consistently
* Prevent premature invalidation of verification state
* Improve reliability of multi-step OTP flows
* Add proper validation/error handling
* Add logging for verification lifecycle failures
* Add automated tests covering the reclaim verification workflow

Do not hardcode tokens, disable expiration checks, or implement temporary workarounds.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Luồng Reclaim của CVerify lưu giữ thông tin người đại diện gồm email và mã số thuế.
- Token xác thực được sinh ở bước 2 chứa email và mã số thuế, ký bằng HMAC-SHA256.
- Ở bước submit cuối cùng, backend thực hiện so khớp email và mã số thuế trong token với email và mã số thuế gửi lên trong request body.
- Lỗi phát sinh do người dùng nhập email có ký tự viết hoa hoặc khoảng trắng hoặc địa chỉ Gmail có nhãn phụ (+tag) ở bước 1, nhưng hệ thống gửi lên định dạng thô khác biệt ở bước cuối, gây lệch chuỗi so khớp.
```

#### 5.3. Kết quả AI trả về

```text
- Đề xuất tạo lớp chuẩn hóa dữ liệu đầu vào.
- Thiết kế helper RecoveryTokenHelper thực hiện chuẩn hóa viết thường, xóa khoảng trắng và loại bỏ nhãn phụ của Gmail.
- Đề xuất lưu trữ token trong React/Zustand state của client để tránh mất token khi chuyển step trong wizard.
- Cung cấp khung kiểm thử tích hợp (integration tests) cho luồng chuẩn hóa.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Lớp chuẩn hóa email và mã số thuế tích hợp vào dịch vụ OrganizationReclaimService và RecoveryService ở backend.
- Cơ chế ghi nhớ token đã xác minh ở frontend để tối ưu UX khi nhấn Back/Next.
- Test case kiểm thử tích hợp trong file RecoveryFlowTests.cs.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Giới hạn logic loại bỏ nhãn '+' chỉ cho gmail.com (AI ban đầu áp dụng cho mọi domain, điều này có thể làm mất thông tin hòm thư hợp lệ của các nhà cung cấp khác).
- Sửa nút Back trên client để giữ nguyên dữ liệu form đã điền của RepresentativeInfo thay vì xóa sạch theo đề xuất ban đầu của AI.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung                                                                                                                             |
| --------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Commit          | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/7b272853130f6c668a40df73d4aec346d6611c99 |

#### 5.8. Ghi chú thêm

```text

```

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
# Fix Reclaim Ownership OTP Verification Bug ... (Nguyên văn prompt số 1)
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này giải quyết một bug logic quan trọng cản trở quá trình nghiệm thu (UAT) của tính năng Reclaim Organization. Nó buộc AI phải tìm ra nguyên nhân cốt lõi từ sự sai khác dữ liệu đầu vào giữa các bước trong một quy trình đa bước phức tạp, thay vì chỉ đề xuất các giải pháp tạm thời (hacks).
```

### 6.3. Kết quả prompt này mang lại

```text
Tạo ra một giải pháp chuẩn hóa dữ liệu nhất quán trên cả frontend và backend, đồng thời xây dựng được bộ tích hợp kiểm thử tự động vững chắc.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Kiểm tra thông qua tích hợp test suite và kiểm thử hộp đen thủ công trên môi trường chạy thực tế của hệ thống.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Sửa đổi logic subaddressing để chỉ áp dụng cho Gmail, và cải tiến hành vi giao diện khi nhấn Back/Next để bảo toàn dữ liệu người dùng điền.
```

---

## 7. Prompt chưa hiệu quả

### 7.1. Prompt chưa hiệu quả

```text
How to normalize email addresses in C#?
```

### 7.2. Vì sao prompt này chưa hiệu quả?

```text
Prompt quá chung chung và thiếu bối cảnh hệ thống. AI phản hồi bằng cách đưa ra các hàm regex loại bỏ dấu '+' và dấu '.' cho toàn bộ các nhà cung cấp email. Điều này sai lệch tiêu chuẩn vì các email như `user+tag@outlook.com` có thể bị hỏng địa chỉ nhận tin nếu xóa dấu '+'.
```

### 7.3. Cách cải thiện prompt

```text
Bổ sung bối cảnh cụ thể của nhà cung cấp dịch vụ và ràng buộc kỹ thuật.
```

### 7.4. Prompt sau khi cải tiến

```text
Write a C# method to normalize email addresses where subaddressing tags (anything after '+') and dots before the '@' are only stripped if the domain is 'gmail.com'. For other domains, only trim and lowercase the string.
```

### 7.5. Kết quả sau khi cải tiến prompt

```text
AI sinh ra code C# chính xác phân tách phần nội bộ (local part) và tên miền (domain part), chỉ thực hiện thay thế/cắt chuỗi trên local part khi domain khớp với 'gmail.com', giữ nguyên cho các domain khác.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần cung cấp bối cảnh đầy đủ của quy trình nghiệp vụ (đặc biệt là các luồng đa bước), các lỗi nhận được cụ thể kèm theo log stack trace, và các ràng buộc về mặt công nghệ/tiêu chuẩn kỹ thuật (ví dụ: quy tắc xử lý email của từng nhà cung cấp).
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Không nên đặt câu hỏi quá ngắn hoặc quá chung chung. Việc chia nhỏ câu hỏi thành các mục "Vấn đề", "Yêu cầu", "Ràng buộc" và cung cấp mã nguồn hiện tại giúp AI đưa ra giải pháp bám sát thực tế nhất.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Nhóm sẽ luôn đính kèm tài liệu đặc tả API và mô hình dữ liệu liên quan để AI hiểu rõ hơn các mối quan hệ thực thể, đồng thời đưa ra các biên giới nghiệm thu (acceptance criteria) rõ ràng cho AI.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu                        |
| ------------- | -------: | --------------------------------------------- |
| Prompt Coding |        1 | Fix Reclaim Ownership OTP Verification Bug... |
| Prompt Design |        0 |                                               |

---

## 10. Checklist chất lượng prompt

| Tiêu chí                   | Đã đạt? | Ghi chú |
| -------------------------- | :-----: | ------- |
| Prompt có mục tiêu rõ ràng |    x    |         |
| Prompt có đủ bối cảnh      |    x    |         |
| Tự kiểm tra và chỉnh sửa   |    x    |         |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết sử dụng prompt minh bạch và ghi nhận đúng đóng góp của AI.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-29    |
