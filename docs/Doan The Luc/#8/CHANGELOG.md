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
| Tên bài tập / Project | CVerify - Gmail Normalization Correction, Multi-Email Support & Password Recovery      |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-05-31T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-31T02:00:00.000Z                                                               |

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

---

# [Phase 10]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-05-31 ~ 2026-05-31
- **Mô tả giai đoạn:** Email Normalization Correction, Multi-Email Support & Password Recovery Overhaul
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Khắc phục lỗi chuẩn hóa email Google OAuth: Thay thế chính sách chuẩn hóa Gmail phá hủy cấu trúc (loại bỏ dấu chấm và phần subaddressing) bằng chính sách bảo toàn danh tính (Trim, NFC normalization, lowercase). | Đoàn Thế Lực | AuthService.cs, IdentityStateResolver.cs, RecoveryTokenHelper.cs | GitHub Commit |
|   2 | Triển khai lớp tương thích ngược LegacyEmailCompatibilityHelper: Hỗ trợ tìm kiếm tài khoản fallback khi đăng nhập bằng email chuẩn hóa kiểu cũ để giữ kết nối cho tài khoản cũ chưa di trú. | Đoàn Thế Lực | LegacyEmailCompatibilityHelper.cs | GitHub Commit |
|   3 | Xây dựng chương trình di trú dữ liệu tự động lúc startup: Thực hiện quét và phục hồi email gốc cho các tài khoản Google từ provider_account_id trong DbInitializer.cs kèm theo cơ chế bảo vệ xung đột. | Đoàn Thế Lực | DbInitializer.cs | GitHub Commit |
|   4 | Triển khai tính năng quản lý Đa email (Multi-Email): Định nghĩa thực thể `UserEmail` và thiết lập các endpoint API link/unlink/make-primary cho phép liên kết tối đa 3 email phụ và hoán đổi email chính mượt mà. | Đoàn Thế Lực | UserEmail.cs, AuthController.cs, ApplicationDbContext.cs | GitHub Commit |
|   5 | Triển khai luồng Khôi phục mật khẩu (Password Recovery): Xây dựng `PasswordRecoveryService` và `PasswordRecoveryController` sử dụng OTP để cho phép khôi phục mật khẩu an toàn và cập nhật đồng bộ PasswordHash. | Đoàn Thế Lực | PasswordRecoveryService.cs, PasswordRecoveryController.cs | GitHub Commit |
|   6 | Cải tiến giao diện cài đặt Settings UI: Thiết kế lại component `SignInMethod.tsx` và `AccountTab.tsx` tích hợp quản lý email phụ và đổi mật khẩu an toàn theo chuẩn HeroUI v3. | Đoàn Thế Lực | SignInMethod.tsx, AccountTab.tsx | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): fix email normalization, add multi-email support and password recovery | https://github.com/Kaivian/CVerify/commit/caed6cc966c813a3036495db34ff3db89d554a93 |

## Ghi chú

```text
```

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Sửa lỗi chuẩn hóa email Google OAuth: Bảo toàn nguyên vẹn email Google chứa dấu chấm và subaddressing.
- Đăng nhập tương thích ngược: Cho phép người dùng cũ đăng nhập bình thường nhờ cơ chế fallback.
- Quản lý đa email: Người dùng liên kết được tối đa 3 email đã xác thực và thăng cấp email phụ làm email chính.
- Khôi phục mật khẩu OTP: Khôi phục mật khẩu thông qua mã OTP nhận qua email chính xác.
- Đồng bộ hóa thông tin bảo mật: PasswordHash được cập nhật đồng nhất trên cả User và PasswordCredentials.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Quản lý thiết bị đang hoạt động (Active Sessions Management): Người dùng chưa thể hủy phiên làm việc của từng thiết bị cụ thể.
```

---

### 4.3. Cải thiện chính

```text
- Bảo mật danh tính: Giải quyết dứt điểm lỗi biến đổi email của Identity Provider, nâng cao độ tin cậy của tích hợp OAuth.
- Linh hoạt tài khoản: Thêm tính năng phụ trợ liên kết nhiều email giúp người dùng dễ dàng chuyển đổi phương thức đăng nhập và khôi phục tài khoản.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này hoàn thành việc chuẩn hóa toàn diện chính sách email của hệ thống, củng cố tính năng quản lý danh tính đa nguồn (Linked Emails) và khôi phục mật khẩu an toàn, đưa hệ thống lên chuẩn chất lượng cao hơn cả về frontend lẫn backend.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Phát triển bảng quản lý Active Sessions hiển thị danh sách thiết bị kèm IP/User-Agent chi tiết.
2. Gửi mail cảnh báo bảo mật khi phát hiện đổi mật khẩu hoặc liên kết email mới từ địa chỉ IP lạ.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-31    |
