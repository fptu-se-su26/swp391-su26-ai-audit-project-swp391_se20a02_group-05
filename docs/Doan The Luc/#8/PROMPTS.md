# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Gmail Normalization Correction, Multi-Email Support & Password Recovery      |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-05-31T00:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-05-31                                                                             |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày       | Công cụ AI  | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
| --: | ---------- | ----------- | -------- | -------------- | ------------- | ------------------------- | ---------- |
|   1 | 2026-05-31 | Antigravity | Tìm lỗi biến đổi email Google OAuth | Investigate Google OAuth registration/login flow for email mutation... | Xác định hàm NormalizeEmailPolicy và đề xuất chính sách chuẩn hóa mới bảo toàn dấu chấm. | Có | GitHub Commit |
|   2 | 2026-05-31 | Antigravity | Thiết kế CSDL và APIs cho tính năng đa email phụ | Design the multi-email database schema and link/unlink/make-primary endpoints... | Sinh cấu trúc bảng user_emails và logic các api đổi email chính/phụ an toàn. | Có | GitHub Commit |
|   3 | 2026-05-31 | Antigravity | Xây dựng luồng khôi phục mật khẩu (Password Recovery) bằng OTP | Create PasswordRecoveryService and PasswordRecoveryController with OTP verification... | Sinh logic gửi và kiểm tra OTP khôi phục mật khẩu, tự động thu hồi session khác. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-31                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Tìm lỗi biến đổi email Google OAuth (mất dấu chấm) và định hình chính sách chuẩn hóa mới.                                                              |
| Phần việc liên quan | Backend / Security / Identity Validation                                                                                                               |
| Mức độ sử dụng      | Hỗ trợ định vị lỗi và đề xuất helper tương thích ngược.                                                                                                |

#### 5.1. Prompt nguyên văn

```text
Investigate the Google OAuth registration/login flow and identify why the email address returned by Google is being modified before being persisted to the database.
Example: actual Google email "theluc.1746@gmail.com" becomes "theluc1746@gmail.com".
Review: LoginWithGoogleAsync, OAuth payload parsing, email normalization helpers, and suggest a fix that preserves the original email while keeping backward compatibility for legacy accounts.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Email từ Google OAuth trả về có chứa dấu chấm, nhưng khi lưu vào bảng users dấu chấm bị biến mất do logic chuẩn hóa Gmail cũ.
- Việc này gây lỗi không nhất quán danh tính và cần sửa đổi mà không làm gián đoạn đăng nhập của các tài khoản cũ.
```

#### 5.3. Kết quả AI trả về

```text
AI định vị lỗi nằm ở hàm NormalizeEmailPolicy trong AuthService.cs và NormalizeEmail trong IdentityStateResolver.cs và RecoveryTokenHelper.cs.
AI đề xuất đổi sang chính sách chuẩn hóa bảo toàn nguyên gốc: Trim() + ToLowerInvariant().
Đồng thời, AI thiết kế LegacyEmailCompatibilityHelper để bọc logic cũ và thực hiện fallback lookup khi người dùng đăng nhập bằng email chuẩn hóa kiểu cũ.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Sửa đổi toàn bộ các hàm chuẩn hóa email sang chính sách mới.
- Thêm lớp LegacyEmailCompatibilityHelper.cs chứa logic chuẩn hóa Gmail cũ và tích hợp logic tìm kiếm fallback vào AuthService.cs và IdentityStateResolver.cs.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Nhóm đã xây dựng thêm một script migration tự động chạy một lần lúc startup (MigrateLegacyGoogleEmailsAsync) trong DbInitializer.cs để sửa đổi trực tiếp dữ liệu users.email cũ từ auth_providers.provider_account_id của Google OAuth.
- Bổ sung bước kiểm tra xung đột trùng lặp email (Conflict Protection) trước khi update dữ liệu.
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
| --------------- | -------- |
| Commit          | https://github.com/Kaivian/CVerify/commit/caed6cc966c813a3036495db34ff3db89d554a93 |

---

### Prompt số 2

| Nội dung            | Thông tin                                                                                                                                                 |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-31                                                                                                                                                |
| Công cụ AI          | Antigravity                                                                                                                                               |
| Mục đích            | Dựng cấu trúc cơ sở dữ liệu và APIs quản lý đa email liên kết.                                                                                            |
| Phần việc liên quan | Backend / Database / APIs                                                                                                                                 |
| Mức độ sử dụng      | Hỗ trợ sinh mã nguồn EF mapping và logic transaction hoán đổi email.                                                                                       |

#### 5.1. Prompt nguyên văn

```text
Design the multi-email database schema and link/unlink/make-primary API endpoints. Users can link up to 3 verified email addresses. Make-primary swaps the current primary email with the selected secondary email inside a database transaction to ensure atomicity.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- CVerify cần cho phép một người dùng liên kết tối đa 3 email để tăng tính linh hoạt và nâng cao khả năng khôi phục tài khoản.
- Hoán đổi email chính - phụ là tác vụ nhạy cảm, cần được thực hiện an toàn và có tính nguyên tử trong database.
```

#### 5.3. Kết quả AI trả về

```text
- Lớp Entity `UserEmail` ánh xạ sang bảng `user_emails`.
- Các API endpoints cho luồng liên kết (link), hủy liên kết (unlink), và thăng cấp email phụ làm email chính (make-primary).
- Sử dụng EF Core DbContextTransaction để bọc logic swap email.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo tệp UserEmail.cs và định cấu hình index, khóa ngoại trong ApplicationDbContext.cs.
- Tích hợp logic API make-primary trong AuthController.cs thực hiện hoán đổi email an toàn.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Nhóm đã bổ sung thêm lớp kiểm tra bảo mật: Bắt buộc người dùng phải nhập mật khẩu hiện tại (re-authentication) trước khi cho phép thăng cấp email phụ làm email chính để tránh chiếm đoạt tài khoản khi máy tính bị mở sẵn session.
- Bổ sung logic kiểm tra số lượng email phụ (không quá 2 email phụ).
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

| Loại minh chứng | Nội dung |
| --------------- | -------- |
| Commit          | https://github.com/Kaivian/CVerify/commit/caed6cc966c813a3036495db34ff3db89d554a93 |

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần cung cấp đầy đủ thông tin về cấu trúc thực thể hiện tại, kịch bản lỗi xảy ra, và các ràng buộc bảo mật (như kiểm tra re-authentication khi làm hành vi nhạy cảm).
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Đặt câu hỏi phải đi kèm yêu cầu bảo mật nghiêm ngặt. Nếu không yêu cầu rõ, AI sẽ bỏ qua các bước kiểm tra mật khẩu hay kiểm tra xung đột dữ liệu, làm giảm tính an toàn của hệ thống.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Coding |        2 | Design the multi-email database schema and link/unlink/make-primary API endpoints... |
| Prompt Debug  |        1 | Investigate the Google OAuth registration/login flow and identify why the email address returned by Google is being modified... |

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
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-31    |
