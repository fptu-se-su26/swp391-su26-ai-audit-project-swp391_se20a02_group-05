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

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-05-31T06:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-31T08:38:00.000Z                                                               |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian               | Nội dung chính                                                                                 | Trạng thái  |
| ------------------- | ----------------------- | ---------------------------------------------------------------------------------------------- | ----------- |
| Phase 01            |                         |                                                                                                | Not Started |
| Phase 02            |                         |                                                                                                | Not Started |
| Phase 03            |                         |                                                                                                | Not Started |
| Phase 04            |                         |                                                                                                | Not Started |
| Phase 05            |                         |                                                                                                | Not Started |
| Phase 06            | 2026-05-23 ~ 2026-05-23 | Secure Authentication Refactoring & Super Admin Enhancements                                   | Completed   |
| Phase 07            | 2026-05-28 ~ 2026-05-28 | Reclaim Ownership OTP Verification & Identity Normalization                                    | Completed   |
| Phase 08            | 2026-05-29 ~ 2026-05-29 | Components System Visual Explorer & Workspace Architecture                                     | Completed   |
| Phase 09            | 2026-05-30 ~ 2026-05-30 | Secure OAuth Integration & Settings Change Password Overhaul                                   | Completed   |
| Phase 10            | 2026-05-31 ~ 2026-05-31 | Email Normalization Correction, Multi-Email Support & Password Recovery Overhaul               | Completed   |
| Phase 11            | 2026-05-31 ~ 2026-05-31 | Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation             | Completed   |

---

# [Phase 11]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-05-31 ~ 2026-05-31
- **Mô tả giai đoạn:** Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Tạo thực thể PendingAuthProvider và bảng pending_auth_providers lưu trữ tạm thời thông tin liên kết OAuth chờ xác nhận (TTL 10 phút). | Đoàn Thế Lực | PendingAuthProvider.cs, ApplicationDbContext.cs, DbInitializer.cs | GitHub Commit |
|   2 | Triển khai PendingLinkCleanupService: Background worker chạy mỗi 30 phút xóa các bản ghi pending hết hạn, sử dụng distributed lock để tránh thực thi song song trong môi trường multi-instance. | Đoàn Thế Lực | PendingLinkCleanupService.cs, Program.cs | GitHub Commit |
|   3 | Thêm JWT claim "sid" (SessionId) vào token để hỗ trợ per-session revocation. | Đoàn Thế Lực | TokenService.cs, ITokenService.cs | GitHub Commit |
|   4 | Triển khai các endpoint mới: GetLinkedConnections, GetPendingLinkDetails, ConfirmLink, UnlinkProviderConnection, RevokeAllOtherSessions. | Đoàn Thế Lực | AuthController.cs, AuthService.cs, IAuthService.cs | GitHub Commit |
|   5 | Thêm trường ProviderDisplayName và ProviderProfileUrl vào thực thể AuthProvider với safe DB migration. | Đoàn Thế Lực | AuthProvider.cs, DbInitializer.cs | GitHub Commit |
|   6 | Tạo component ConfirmationModal tái sử dụng với hỗ trợ verification text, blocking error, và variant styling. | Đoàn Thế Lực | ConfirmationModal.tsx | GitHub Commit |
|   7 | Thêm các phương thức API client: fetchConnections, fetchPendingLinkDetails, confirmLink, unlinkConnection, revokeOtherSessions. | Đoàn Thế Lực | auth.service.ts | GitHub Commit |
|   8 | Thêm types LinkedProviderConnection, PendingLinkDetailsResponseData, và trường hasPassword vào User. | Đoàn Thế Lực | auth.types.ts | GitHub Commit |
|   9 | Hiển thị Google Unlinked Error với hướng dẫn khôi phục trên trang đăng nhập. | Đoàn Thế Lực | login-view.tsx | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Mở rộng SessionValidationMiddleware: Thêm tầng kiểm tra per-session revocation qua sid claim với cơ chế fallback qua refresh_token cookie cho rolling deployment compatibility. | Đoàn Thế Lực | SessionValidationMiddleware.cs | GitHub Commit |
|   2 | Refactor OAuth callback logic để hỗ trợ multi-connection per provider: Cập nhật credential cho kết nối hiện tại hoặc tạo pending link cho kết nối mới. | Đoàn Thế Lực | AuthController.cs, AuthService.cs | GitHub Commit |
|   3 | Refactor unique index trên auth_providers: Scope uniqueness chỉ áp dụng cho Google (provider_name = 'google'), thêm index lookup chung cho multi-connection. | Đoàn Thế Lực | ApplicationDbContext.cs, DbInitializer.cs | GitHub Commit |
|   4 | Overhaul LinkedAccountsList: Hiển thị per-connection cards với provider username, avatar, profile URL, và scope validation status. | Đoàn Thế Lực | LinkedAccountsList.tsx | GitHub Commit |
|   5 | Overhaul SignInMethod: Phân biệt luồng "Create Password" và "Change Password" dựa trên user.hasPassword. | Đoàn Thế Lực | SignInMethod.tsx | GitHub Commit |
|   6 | Thêm /settings vào danh sách protected pages trong auth interceptor. | Đoàn Thế Lực | interceptors.ts | GitHub Commit |

### Fixed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Sửa lỗi password change fallback: Khi không tìm thấy PasswordCredential record, giờ đây fallback sang user.PasswordHash thay vì ném lỗi. | Đoàn Thế Lực | AuthService.cs, PasswordRecoveryService.cs | GitHub Commit |

### Security

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Tăng cường Google linking: Kiểm tra email đã xác thực (EmailVerified), sử dụng IgnoreQueryFilters để phát hiện xung đột soft-deleted providers xuyên tài khoản. | Đoàn Thế Lực | AuthService.cs | GitHub Commit |
|   2 | Thêm audit events bảo mật: PROVIDER_LINK_CONFLICT, PROVIDER_LINK_REACTIVATED, PROVIDER_LINK_CONFIRMED. | Đoàn Thế Lực | AuthService.cs | GitHub Commit |
|   3 | Bổ sung concurrency guard trong PasswordRecoveryService: Chặn provisioning song song khi credential bị thay đổi giữa hai request đồng thời. | Đoàn Thế Lực | PasswordRecoveryService.cs | GitHub Commit |
|   4 | Thêm audit trail logging chi tiết trong PasswordRecoveryService cho các sự kiện: PASSWORD_RECOVERY_FAILED, PASSWORD_SETUP_FAILED, PASSWORD_REUSE_BLOCKED, PASSWORD_SETUP_CONCURRENT_BLOCKED. | Đoàn Thế Lực | PasswordRecoveryService.cs | GitHub Commit |
|   5 | Bọc logic unlink provider trong database transaction để đảm bảo tính nguyên tử khi xóa credential và soft-delete provider. | Đoàn Thế Lực | AuthService.cs | GitHub Commit |

### Testing

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Viết 6 integration tests mới cho SessionRevocationTests: ValidSid_ActiveSession_Should_Pass, ValidSid_RevokedSession_Should_Fail, MissingSid_ActiveCookieFallback_Should_Pass, MissingSid_RevokedCookieFallback_Should_Fail, InvalidSid_Should_Fail, RevokeCurrentSession_Should_Return_BadRequest_And_Fail. | Đoàn Thế Lực | SessionRevocationTests.cs | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement multi-connection OAuth linking, per-session revocation and pending link confirmation | https://github.com/Kaivian/CVerify/commit/7d34d88 |

## Ghi chú

```text
```

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Multi-connection OAuth: Hỗ trợ liên kết nhiều tài khoản GitHub/GitLab với cùng một CVerify profile.
- Two-phase link confirmation: Luồng xác nhận liên kết hai bước qua PendingAuthProvider với TTL 10 phút.
- Per-session revocation: Thu hồi quyền truy cập ở cấp độ phiên đăng nhập cụ thể qua JWT sid claim.
- Revoke All Other Sessions: Endpoint thu hồi hàng loạt tất cả phiên đăng nhập ngoại trừ phiên hiện tại.
- ConfirmationModal: Component tái sử dụng cho destructive actions với verification text và blocking error.
- LinkedAccountsList overhaul: Hiển thị metadata chi tiết từng kết nối OAuth (username, avatar, scopes).
- SignInMethod adaptive UI: Tự động chuyển đổi giữa "Create Password" và "Change Password" dựa trên trạng thái tài khoản.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Pending link confirmation UI flow: Giao diện xác nhận liên kết hai bước chưa có trang chuyên biệt, hiện tại chỉ có API endpoints.
- Email notification khi liên kết/hủy liên kết OAuth: Chưa gửi email cảnh báo bảo mật khi có thay đổi provider.
```

---

### 4.3. Cải thiện chính

```text
- Bảo mật phiên đăng nhập: Nâng cấp từ user-wide session version lên per-session revocation, cho phép quản lý thiết bị chi tiết hơn.
- Linh hoạt OAuth: Cho phép liên kết nhiều tài khoản cùng provider (GitHub/GitLab) phục vụ nhu cầu quản lý nhiều workspace.
- An toàn liên kết: Two-phase confirmation ngăn chặn liên kết OAuth không mong muốn qua OAuth callback redirect attack.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này hoàn thành việc nâng cấp toàn diện hệ thống quản lý phiên đăng nhập và liên kết OAuth, đưa CVerify lên chuẩn bảo mật per-session revocation hiện đại và hỗ trợ multi-connection OAuth linh hoạt.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Xây dựng trang Pending Link Confirmation chuyên biệt hiển thị thông tin provider và cho phép người dùng xác nhận/từ chối liên kết.
2. Gửi email cảnh báo bảo mật khi phát hiện liên kết hoặc hủy liên kết OAuth provider.
3. Hiển thị bảng Active Sessions với thông tin IP/User-Agent/Thiết bị chi tiết trên giao diện Settings.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-31    |
