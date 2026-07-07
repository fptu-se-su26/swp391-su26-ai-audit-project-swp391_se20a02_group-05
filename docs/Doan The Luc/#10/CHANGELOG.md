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
| Tên bài tập / Project | CVerify - Account Deletion Lifecycle & Modular Monolith Transition                      |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-06-01T09:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-01T15:05:00.000Z                                                               |

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
| Phase 12            | 2026-06-01 ~ 2026-06-01 | Account Deletion Lifecycle & Modular Monolith Transition                                       | Completed   |

---

# [Phase 12]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-01 ~ 2026-06-01
- **Mô tả giai đoạn:** Account Deletion Lifecycle & Modular Monolith Transition
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Thêm giá trị `DELETION_PENDING` vào UserStatus enum với state machine validation matrix trong domain. | Đoàn Thế Lực | User.cs, UserStatus.cs | GitHub Commit |
|   2 | Thêm cột `IsLegalHold` trên thực thể User để hỗ trợ ngăn chặn luồng xóa tài khoản khi có ràng buộc tuân thủ pháp lý. | Đoàn Thế Lực | User.cs | GitHub Commit |
|   3 | Thêm cột `AnonymizedActorHash` trên AuditLog để hỗ trợ ẩn danh hóa vết lịch sử sau khi tài khoản bị xóa cứng. | Đoàn Thế Lực | AuditLog.cs | GitHub Commit |
|   4 | Triển khai 6 API endpoints mới phục vụ xóa tài khoản: GetDeletionRequirements, RequestDeletion, FallbackOtp, ConnectReauth, CallbackReauth, Reactivate. | Đoàn Thế Lực | UserController.cs, DeletionService.cs | GitHub Commit |
|   5 | Tạo trang Reactivate UI client `/auth/reactivate` để người dùng có thể tự khôi phục tài khoản trong grace period. | Đoàn Thế Lực | reactivate-view.tsx | GitHub Commit |
|   6 | Thêm bộ kiểm thử kiến trúc ModularBoundaryTests để tự động kiểm tra ranh giới phụ thuộc giữa các Modules. | Đoàn Thế Lực | ModularBoundaryTests.cs | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Mở rộng TokenCleanupBackgroundJob: Thêm PurgeExpiredSoftDeletedUsersAsync chạy batch-processes định kỳ để xóa cứng tài khoản, dọn dẹp file R2, và ẩn danh audit log. | Đoàn Thế Lực | TokenCleanupBackgroundJob.cs | GitHub Commit |
|   2 | Nâng cấp trang Login: Thêm interceptor phát hiện trạng thái DELETION_PENDING và tự động chuyển hướng người dùng sang trang khôi phục tài khoản. | Đoàn Thế Lực | use-auth.ts, login-view.tsx | GitHub Commit |
|   3 | Đổi tên namespace và cấu trúc thư mục của toàn bộ source code CVerify.Core sang cấu trúc modular: Modules/Shared, Modules/Auth, Modules/Recovery, v.v. | Đoàn Thế Lực | Program.cs, CVerify.Core | GitHub Commit |
|   4 | Cập nhật Program.cs để cấu hình ValidateOnBuild = true giúp kiểm tra scope dependency ngay khi khởi chạy. | Đoàn Thế Lực | Program.cs | GitHub Commit |

### Refactored

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Di chuyển và chuyển đổi namespaces của toàn bộ tệp kiểm thử (hơn 50 file unit/integration/benchmark tests) theo cấu trúc modular monolith mới. | Đoàn Thế Lực | Tests/ | GitHub Commit |

### Security

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Triển khai ẩn danh hóa audit log bằng cơ chế SHA-256 kết hợp ServerSalt, loại bỏ Email/FullName nhưng vẫn giữ trace logs. | Đoàn Thế Lực | TokenCleanupBackgroundJob.cs | GitHub Commit |
|   2 | Thêm SecurityAlertNotice gửi email cảnh báo bảo mật đến hòm thư chính khi OTP fallback được kích hoạt. | Đoàn Thế Lực | DeletionService.cs | GitHub Commit |
|   3 | Ràng buộc thời gian sống (TTL 5 phút) cho OAuth re-authentication token lưu trong cache trước khi xóa. | Đoàn Thế Lực | DeletionService.cs | GitHub Commit |

### Testing

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Viết 5 integration tests cho deletion lifecycle: Password deletion transitions, Legal hold enforcement, Reactivation in grace period, Email uniqueness, và hard purge. | Đoàn Thế Lực | AccountDeletionTests.cs | GitHub Commit |
|   2 | Sửa đổi ModularBoundaryTests.cs để bổ sung các ngoại lệ phụ thuộc hợp lý (Recovery -> Auth, DbContext/User in Shared). | Đoàn Thế Lực | ModularBoundaryTests.cs | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement account deletion lifecycle with soft-delete, reactivation, and automated purge | https://github.com/Kaivian/CVerify/commit/137171f |
| Commit/PR       | refactor(arch): transition codebase to modular monolith architecture | https://github.com/Kaivian/CVerify/commit/1eb5cb7 |

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Hoàn thiện vòng đời xóa tài khoản 14 ngày (Soft-delete & Reactivate).
- Tự động hóa tác vụ xóa cứng dữ liệu người dùng tại background worker (xóa R2 assets, hard-delete user).
- Cơ chế ẩn danh hóa lịch sử log bằng Salted SHA-256 bảo vệ dữ liệu PII của người dùng.
- Tách biệt thành công cấu trúc codebase backend sang dạng Modular Monolith.
- Tích hợp thành công bộ kiểm tra NetArchTest trong CI/CD cục bộ kiểm soát sự cô lập giữa các Modules.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Giao diện Admin quản lý Legal Hold: Hiện tại việc đặt cờ IsLegalHold phải thực hiện trực tiếp bằng DB command hoặc qua API thủ công, chưa có UI quản trị riêng.
```

---

### 4.3. Cải thiện chính

```text
- Đảm bảo tuân thủ bảo mật tuyệt đối về dữ liệu (GDPR Compliance) thông qua cơ chế soft-delete, grace period và ẩn danh hóa lịch sử log.
- Kiến trúc codebase trở nên rõ ràng và dễ bảo trì hơn rất nhiều nhờ cấu trúc Modular Monolith mới, giúp các đội phát triển làm việc song song trên các mô-đun Auth, Profiles, Recovery mà không lo bị spaghetti code.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này giúp hoàn thiện tính năng bảo mật quan trọng về quyền riêng tư dữ liệu người dùng (Account Deletion Lifecycle) và thực hiện cuộc cải cách kiến trúc lớn nhất từ trước đến nay trên CVerify - chuyển đổi thành công sang Modular Monolith để sẵn sàng mở rộng quy mô phát triển dự án.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Phát triển màn hình quản trị Admin để quản lý trạng thái Legal Hold của các tài khoản người dùng đang bị điều tra/tuân thủ.
2. Nâng cao chất lượng kiểm thử kiến trúc bằng cách định nghĩa thêm các quy chuẩn phụ thuộc chi tiết hơn cho từng lớp Layer bên trong mỗi Module nghiệp vụ.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-06-01    |
