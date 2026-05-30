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
| Tên bài tập / Project | CVerify - Secure OAuth Integration & Settings Change Password Overhaul                  |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-05-30T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-30T23:59:59.000Z                                                               |

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

---

# [Phase 09]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-05-30 ~ 2026-05-30
- **Mô tả giai đoạn:** Secure OAuth Integration & Settings Change Password Overhaul
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Triển khai cấu trúc mã hóa bảo mật token OAuth (AES-256-GCM): Xây dựng thực thể `OAuthCredential` và helper mã hóa/giải mã access/refresh tokens trước khi lưu trữ xuống PostgreSQL bằng khóa bí mật lấy từ cấu hình hệ thống. | Đoàn Thế Lực | OAuthCredential.cs, TokenEncryptionService.cs, IEncryptionService.cs | GitHub Commit |
|   2 | Cấu hình xác thực khóa TokenEncryptionKey lúc khởi chạy ứng dụng: Thêm logic kiểm tra khóa cấu hình trong `Program.cs` để ngăn chặn ứng dụng chạy khi thiếu biến môi trường mã hóa nhạy cảm. | Đoàn Thế Lực | Program.cs | GitHub Commit |
|   3 | Tích hợp các endpoints OAuth Linking và Unlinking: Triển khai các API trong `AuthController` cho phép người dùng đăng nhập liên kết thêm tài khoản mạng xã hội (Google, GitHub, GitLab) hoặc hủy liên kết một cách an toàn. | Đoàn Thế Lực | AuthController.cs, AuthService.cs | GitHub Commit |
|   4 | Nâng cấp luồng API đổi mật khẩu (Change Password): Thực hiện kiểm tra chính sách mật khẩu nghiêm ngặt, cập nhật credentials và thực hiện thu hồi (revoke) toàn bộ Refresh Tokens hoạt động của tài khoản đó trên các thiết bị khác. | Đoàn Thế Lực | AuthController.cs, AuthService.cs, TokenService.cs | GitHub Commit |
|   5 | Overhaul giao diện Cài đặt (Settings Tab) & Social Connections: Sử dụng HeroUI v3 và Tailwind CSS v4 để thiết kế lại trang Settings, hiển thị danh sách tài khoản liên kết trực quan cùng biểu mẫu đổi mật khẩu hiện đại, đáp ứng chuẩn thiết kế cao cấp. | Đoàn Thế Lực | settings/page.tsx, social-connections.tsx, change-password-form.tsx | GitHub Commit |
|   6 | Khắc phục lỗi khởi tạo cơ sở dữ liệu chạy test container (`DbInitializer.cs` sequencing fix): Sửa lỗi thiếu kiểm tra sự tồn tại của bảng `user_profiles` trước khi ALTER TABLE trong DbInitializer, giúp toàn bộ 106/106 ca kiểm thử chạy thành công trơn tru. | Đoàn Thế Lực | DbInitializer.cs | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement secure OAuth linking and settings password change | https://github.com/Kaivian/CVerify/commit/a8e67a04e8de3f64d2be76f46c3cde3439a31497 |

## Ghi chú

```text
```

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Mã hóa tokens bảo mật: Toàn bộ access và refresh token từ Google/GitHub/GitLab được mã hóa đối xứng AES-256-GCM trước khi lưu xuống PostgreSQL, ngăn chặn rò rỉ token khi database bị truy cập trái phép.
- Quản lý liên kết tài khoản OAuth: Cho phép liên kết và hủy liên kết tài khoản linh hoạt ngay trên giao diện cài đặt Settings.
- Đổi mật khẩu an toàn và hủy phiên đăng nhập: Đổi mật khẩu kèm cơ chế tự động hủy toàn bộ tokens/sessions đang hoạt động trên các thiết bị khác, bảo vệ tài khoản tối đa.
- Giao diện Settings cao cấp: Sử dụng HeroUI v3 mang lại giao diện mượt mà, phản hồi nhanh nhạy và thiết kế hiện đại.
- Sửa lỗi tuần tự hóa khởi tạo schema: Bộ kiểm thử tự động 106/106 tests vượt qua ổn định trên các test containers sạch.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Hệ thống gửi email thông báo bảo mật: Chưa gửi email cảnh báo bảo mật tức thì khi người dùng đổi mật khẩu hoặc thực hiện liên kết tài khoản mới. Tính năng này được lên lịch triển khai ở phase tiếp theo.
```

---

### 4.3. Cải thiện chính

```text
- Nâng tầm bảo mật hệ thống thông qua việc áp dụng chuẩn mã hóa dữ liệu nhạy cảm AES-256-GCM ở backend và cơ chế bảo vệ phiên làm việc (Session Invalidation). Đồng thời nâng cấp thiết kế Settings UI đồng nhất với thư viện HeroUI v3 cao cấp của dự án CVerify.
```

---

### 4.4. Tổng kết project

```text
Việc kết hợp hoàn hảo giữa giao diện người dùng HeroUI v3 trực quan và kiến trúc backend ASP.NET Core v10 bảo mật cao (mã hóa token, revoke token khác thiết bị) đã hoàn tất một cột mốc quan trọng về mặt an ninh thông tin cho dự án CVerify, tăng tính sẵn sàng của hệ thống thông qua kiểm thử tích hợp tự động hoàn chỉnh.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Triển khai gửi email thông báo bảo mật cho người dùng ngay khi mật khẩu thay đổi thành công hoặc có liên kết OAuth mới.
2. Xây dựng trang hiển thị các thiết bị/phiên hoạt động đang đăng nhập (Active Sessions Manager) để người dùng có quyền chủ động hủy phiên của từng thiết bị cụ thể thay vì hủy tất cả cùng lúc.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-30    |
