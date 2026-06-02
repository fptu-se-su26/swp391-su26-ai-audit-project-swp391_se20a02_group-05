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
| Tên bài tập / Project | CVerify - Automatic Username System & Public Profile Routing                            |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-06-02T14:40:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-02T15:38:00.000Z                                                               |

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
| Phase 13            | 2026-06-02 ~ 2026-06-02 | Automatic Username System & Public Profile Routing                                             | Completed   |

---

# [Phase 13]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-02 ~ 2026-06-02
- **Mô tả giai đoạn:** Automatic Username System & Public Profile Routing
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Tạo dịch vụ `UsernameService` trong Shared/Security với các phương thức: ValidateUsername, Normalize, GenerateBaseUsername, GenerateUniqueUsernameAsync, RunWithUsernameRetryAsync, CheckChangeCooldownAsync. | Đoàn Thế Lực | UsernameService.cs, IUsernameService.cs | GitHub Commit |
|   2 | Thêm cột `username` (CITEXT) và `last_username_change_at` vào bảng users với unique partial index `idx_users_username_active`. | Đoàn Thế Lực | DbInitializer.cs, User.cs | GitHub Commit |
|   3 | Triển khai endpoint public profile lookup `GET /v1/users/profile/public/{username}` với signed avatar URL resolution. | Đoàn Thế Lực | ProfileController.cs, ProfileService.cs, PublicProfileResponse.cs | GitHub Commit |
|   4 | Tạo trang client `[username]` dynamic route hiển thị hồ sơ công khai người dùng. | Đoàn Thế Lực | client/src/app/[username]/page.tsx | GitHub Commit |
|   5 | Thêm `PublicProfileResponse` type và `fetchPublicProfile` API method trên client. | Đoàn Thế Lực | profile.types.ts, profile.service.ts | GitHub Commit |
|   6 | Triển khai `MigrateLegacyUsernamesAsync` để backfill username cho legacy users dựa trên email hoặc user_profiles.username cũ. | Đoàn Thế Lực | DbInitializer.cs | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Mở rộng `AuthResponse` DTO thêm trường Username, cập nhật toàn bộ constructor calls trong Auth/Recovery modules. | Đoàn Thế Lực | AuthDtos.cs, AuthService.cs, CandidateRecoveryService.cs, OrganizationRecoveryService.cs, RecoveryExecutionEngine.cs | GitHub Commit |
|   2 | Tích hợp username auto-generation vào luồng đăng ký email và Google OAuth callback trong AuthService thông qua `RunWithUsernameRetryAsync`. | Đoàn Thế Lực | AuthService.cs | GitHub Commit |
|   3 | Tích hợp `RunWithUsernameRetryAsync` vào RecoveryExecutionEngine cho user provisioning trong quá trình recovery. | Đoàn Thế Lực | RecoveryExecutionEngine.cs | GitHub Commit |
|   4 | Cập nhật `ProfileResponse` mapping để fallback sang `User.Username` khi `UserProfile.Username` null. | Đoàn Thế Lực | ProfileService.cs | GitHub Commit |
|   5 | Mở rộng client auth types thêm trường `username` trên User, LoginResponseData, và UserProfileResponseData. | Đoàn Thế Lực | auth.types.ts | GitHub Commit |
|   6 | Đăng ký `IUsernameService` trong DI container và truyền vào `DbInitializer.InitializeAsync`. | Đoàn Thế Lực | Program.cs | GitHub Commit |

### Refactored

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Chuyển đổi Level2RecoveryTests từ inline User initialization sang UserBuilder pattern thống nhất. | Đoàn Thế Lực | Level2RecoveryTests.cs | GitHub Commit |
|   2 | Mở rộng UserBuilder helper thêm `WithUsername()` và auto-generate username từ email local part. | Đoàn Thế Lực | UserBuilder.cs | GitHub Commit |

### Security

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Danh sách reserved usernames bao phủ toàn bộ application routes ngăn chặn xung đột routing và username squatting. | Đoàn Thế Lực | UsernameService.cs | GitHub Commit |
|   2 | Unique partial index trên CITEXT column đảm bảo uniqueness ở database level, bổ sung cho application-level optimistic retry. | Đoàn Thế Lực | DbInitializer.cs | GitHub Commit |
|   3 | Cooldown 30 ngày ngăn chặn lạm dụng thay đổi username liên tục. | Đoàn Thế Lực | UsernameService.cs | GitHub Commit |

### Testing

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Viết UsernameServiceTests (unit): validation rules, normalization, base generation từ email, reserved word detection, và cooldown enforcement. | Đoàn Thế Lực | UsernameServiceTests.cs | GitHub Commit |
|   2 | Viết UsernameFlowTests (integration): auto-generation khi đăng ký, update với cooldown, uniqueness constraint, và validation rules qua API. | Đoàn Thế Lực | UsernameFlowTests.cs | GitHub Commit |
|   3 | Bổ sung hardcoded username cho RegistrationFlowTests seed data để tương thích với username column mới. | Đoàn Thế Lực | RegistrationFlowTests.cs | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(profile): implement automatic username system with public profile routing | https://github.com/Kaivian/CVerify/commit/5fbd9d3 |

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Hệ thống tự động sinh username từ email local part khi đăng ký (email + Google OAuth).
- Endpoint public profile lookup theo username với signed avatar URL.
- Trang client [username] dynamic route hiển thị hồ sơ công khai.
- Migration backward-compatible cho cột username với backfill legacy users.
- Validation, normalization, reserved word protection, và 30-day cooldown cho username.
- Optimistic retry pattern cho concurrent username allocation.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Giao diện Settings cho phép user tự thay đổi username (backend API CheckChangeCooldownAsync đã sẵn sàng, chưa có UI form).
- Trang [username] chưa hiển thị đầy đủ thông tin nghề nghiệp (career preferences, attachments).
```

---

### 4.3. Cải thiện chính

```text
- Mỗi user CVerify giờ có một identity công khai duy nhất, thân thiện URL, có thể chia sẻ trực tiếp.
- Cơ chế optimistic retry đảm bảo zero-collision username allocation trong high-concurrency scenarios.
- Safe migration DDL cho phép triển khai trên database production đã tồn tại mà không gây downtime.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này bổ sung tính năng identity công khai quan trọng cho nền tảng CVerify, cho phép người dùng có username duy nhất và trang hồ sơ công khai có thể truy cập qua URL thân thiện. Hệ thống được thiết kế với tính toàn vẹn cao (database-level uniqueness + application-level retry) và backward compatibility tốt (legacy migration backfill).
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Phát triển giao diện Settings cho phép người dùng tự thay đổi username với UI hiển thị cooldown timer.
2. Mở rộng trang [username] hiển thị thêm career preferences, skills, và portfolio attachments.
3. Bổ sung SEO metadata (og:title, og:description, og:image) cho trang public profile.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-02    |
