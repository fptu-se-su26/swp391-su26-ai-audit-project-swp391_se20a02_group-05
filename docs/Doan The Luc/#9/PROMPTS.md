# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-05-31T06:00:00.000Z                                                               |
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
|   1 | 2026-05-31 | Antigravity | Thiết kế kiến trúc multi-connection OAuth và pending link confirmation | Refactor OAuth linking to support multi-connection per provider... | Tạo PendingAuthProvider entity, refactor index và callback logic. | Có | GitHub Commit |
|   2 | 2026-05-31 | Antigravity | Thiết kế per-session revocation middleware | Extend SessionValidationMiddleware to support per-session revocation... | Thêm sid claim vào JWT và logic kiểm tra session cụ thể trong middleware. | Có | GitHub Commit |
|   3 | 2026-05-31 | Antigravity | Xây dựng ConfirmationModal và cải tiến LinkedAccountsList | Build a reusable ConfirmationModal component with HeroUI v3... | Sinh component modal tái sử dụng và redesign danh sách kết nối OAuth. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-31                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Thiết kế cơ chế liên kết đa tài khoản OAuth và luồng xác nhận hai bước.                                                                                |
| Phần việc liên quan | Backend / Authentication / OAuth Linking                                                                                                               |
| Mức độ sử dụng      | Hỗ trợ thiết kế kiến trúc entity mới và refactor callback logic.                                                                                       |

#### 5.1. Prompt nguyên văn

```text
Refactor the OAuth linking flow to support multiple connections per provider (GitHub/GitLab can have multiple linked accounts). Introduce a two-phase confirmation: OAuth callback stores credentials in a PendingAuthProvider table with a 10-minute TTL, then the user confirms via a separate endpoint. Google remains single-connection with a unique index. Add a background cleanup service to prune expired pending links.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- CVerify trước đó chỉ cho phép liên kết 1 tài khoản duy nhất cho mỗi OAuth provider, không đáp ứng nhu cầu người dùng có nhiều tài khoản GitHub cho các workspace khác nhau.
- Liên kết OAuth trực tiếp qua callback không có bước xác nhận tạo ra rủi ro bảo mật khi kẻ tấn công có thể lợi dụng OAuth redirect để liên kết tài khoản không mong muốn.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất tạo Entity PendingAuthProvider với bảng pending_auth_providers lưu trữ thông tin liên kết tạm thời (provider_key, encrypted tokens, expires_at). OAuth callback được refactor: nếu kết nối đã tồn tại thì cập nhật credential, nếu chưa thì lưu vào pending. PendingLinkCleanupService chạy background mỗi 30 phút với distributed lock. Unique index trên auth_providers được scope lại chỉ áp dụng cho Google.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo PendingAuthProvider.cs với EF Core mapping trong ApplicationDbContext.cs.
- Tạo PendingLinkCleanupService.cs với distributed lock qua ICacheService.AcquireLockAsync.
- Refactor OAuth callback trong AuthController.cs để phân nhánh existing connection update vs pending link creation.
- Tạo DDL cho bảng pending_auth_providers trong DbInitializer.cs.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Bổ sung IgnoreQueryFilters() khi kiểm tra xung đột provider_key để phát hiện cả soft-deleted providers.
- Thêm audit events PROVIDER_LINK_CONFLICT và PROVIDER_LINK_REACTIVATED.
- Thiết kế logic reactivation cho Google provider đã soft-delete: khôi phục DeletedAt = null thay vì tạo bản ghi mới.
- Bọc logic unlink provider trong database transaction để đảm bảo tính nguyên tử.
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
| Commit          | https://github.com/Kaivian/CVerify/commit/7d34d88 |

---

### Prompt số 2

| Nội dung            | Thông tin                                                                                                                                                 |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-31                                                                                                                                                |
| Công cụ AI          | Antigravity                                                                                                                                               |
| Mục đích            | Thiết kế per-session revocation middleware và JWT sid claim.                                                                                                |
| Phần việc liên quan | Backend / Security / Session Management                                                                                                                   |
| Mức độ sử dụng      | Hỗ trợ thiết kế kiến trúc middleware hai tầng và cơ chế fallback.                                                                                          |

#### 5.1. Prompt nguyên văn

```text
Extend SessionValidationMiddleware to support per-session revocation. Add a 'sid' claim to the JWT token containing the SessionId. The middleware should check if the specific session is still active by querying RefreshTokens. Support a fallback mechanism using the refresh_token cookie for tokens issued before the sid claim was introduced (rolling deployment compatibility).
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Middleware hiện tại chỉ kiểm tra SessionVersion ở cấp user-wide: Khi thu hồi một phiên, tất cả phiên khác cũng bị ảnh hưởng.
- Cần cơ chế thu hồi chính xác từng phiên đăng nhập cụ thể mà không ảnh hưởng đến các phiên còn lại.
- Đang trong giai đoạn rolling deployment nên cần tương thích ngược với JWT token cũ chưa chứa sid claim.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất thêm claim "sid" vào JWT tại TokenService. Middleware thêm tầng kiểm tra thứ hai sau SessionVersion: lấy sid từ JWT, kiểm tra cache auth:session:{sid}:active (TTL 30 phút), nếu cache miss thì query DB. Nếu không có sid (token cũ), fallback đọc refresh_token cookie và tra cứu SessionId tương ứng.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Thêm tham số sessionId vào GenerateJwtToken trong TokenService.cs.
- Mở rộng SessionValidationMiddleware.cs với logic kiểm tra per-session revocation hai tầng.
- Tạo endpoint DELETE /auth/sessions để thu hồi hàng loạt phiên đăng nhập.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Bổ sung kiểm tra RevokedAt trực tiếp trong nhánh fallback: Nếu refresh_token đã bị thu hồi, invalidate ngay mà không cần tra cứu SessionId.
- Thêm try-catch fail-safe: Nếu Redis transient failure, middleware fallback sang query DB thay vì ném lỗi 500.
- Viết 6 integration tests cho SessionRevocationTests bao phủ toàn bộ kịch bản edge case.
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
| Commit          | https://github.com/Kaivian/CVerify/commit/7d34d88 |

---

### Prompt số 3

| Nội dung            | Thông tin                                                                                                                                                 |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-31                                                                                                                                                |
| Công cụ AI          | Antigravity                                                                                                                                               |
| Mục đích            | Xây dựng ConfirmationModal tái sử dụng và redesign LinkedAccountsList.                                                                                     |
| Phần việc liên quan | Frontend / Settings UI / Component Design                                                                                                                 |
| Mức độ sử dụng      | Hỗ trợ sinh boilerplate component và phác thảo layout.                                                                                                     |

#### 5.1. Prompt nguyên văn

```text
Build a reusable ConfirmationModal component with HeroUI v3 Modal primitives supporting: verification text input (user must type exact text to confirm), blocking error state (disables confirm and shows alert), and variant-based styling (danger/warning/primary). Also redesign LinkedAccountsList to display per-connection cards with provider username, avatar, profile URL, and scope validation status.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Hệ thống thiếu modal xác nhận chung cho các hành động nguy hiểm (ngắt kết nối OAuth, xóa email, v.v.).
- LinkedAccountsList hiện tại chỉ hiển thị tên provider mà không cung cấp metadata chi tiết về từng kết nối.
- Cần hỗ trợ multi-connection: hiển thị nhiều card cho cùng một provider (ví dụ 2 tài khoản GitHub).
```

#### 5.3. Kết quả AI trả về

```text
AI sinh ConfirmationModal component với Modal.Backdrop, Modal.Container, Modal.Dialog từ HeroUI v3. Props interface hỗ trợ verificationText, blockingError, variant, isPending. LinkedAccountsList được redesign hiển thị per-connection cards với avatar, username, và nút disconnect.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo ConfirmationModal.tsx với đầy đủ chức năng verification text, blocking error, variant styling.
- Tích hợp ConfirmationModal vào SignInMethod.tsx cho Google unlink và email delete confirmation.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Thiết kế logic lockout prevention: Kiểm tra user.hasPassword và số lượng provider trước khi cho phép ngắt kết nối, hiển thị blockingError nếu vi phạm.
- Chuyển đổi SignInMethod giữa "Create Password" và "Change Password" dựa trên user.hasPassword với điều kiện validation khác nhau.
- Bổ sung Google Unlinked Error UI trong login-view.tsx với hướng dẫn khôi phục chi tiết cho người dùng.
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
| Commit          | https://github.com/Kaivian/CVerify/commit/7d34d88 |

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần nêu rõ các ràng buộc bảo mật (soft-delete awareness, lockout prevention) và yêu cầu tương thích ngược (rolling deployment) trong prompt. Nếu không chỉ định, AI sẽ sinh code cho trường hợp lý tưởng (happy path) mà bỏ qua các kịch bản edge case quan trọng.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Nên chia nhỏ các yêu cầu phức tạp thành nhiều prompt riêng biệt theo từng lớp kiến trúc (middleware → service → controller → UI) thay vì yêu cầu AI xử lý toàn bộ trong một prompt duy nhất. Điều này giúp kiểm soát chất lượng từng phần và dễ dàng phát hiện lỗi thiết kế sớm hơn.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Coding |        2 | Refactor OAuth linking to support multi-connection per provider... |
| Prompt Design |        1 | Build a reusable ConfirmationModal component with HeroUI v3... |

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
