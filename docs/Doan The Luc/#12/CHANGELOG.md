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
| Tên bài tập / Project | CVerify - Avatar Source Persistence, Achievements System & Form Standardizations       |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-06-03T02:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-03T04:30:00.000Z                                                               |

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
| Phase 14            | 2026-06-03 ~ 2026-06-03 | Persisting Avatar Source, Re-engineering Experience/Achievements Settings & Form Consistency   | Completed   |

---

# [Phase 14]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-03 ~ 2026-06-03
- **Mô tả giai đoạn:** Persisting Avatar Source, Re-engineering Experience/Achievements Settings & Form Consistency
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Thêm cột `avatar_source` (integer/enum) trong bảng users hỗ trợ quản lý nguồn gốc avatar. | Đoàn Thế Lực | DbInitializer.cs, User.cs | GitHub Commit |
|   2 | Thêm endpoint `/api/v1/users/profile/avatar/sync` (POST) và `/api/v1/users/profile/avatar` (DELETE) để đồng bộ và xóa ảnh đại diện khỏi R2 storage. | Đoàn Thế Lực | ProfileController.cs, ProfileService.cs | GitHub Commit |
|   3 | Định nghĩa thực thể `WorkExperience` và `AcademicAchievement` phục vụ quản lý kinh nghiệm làm việc và học tập. | Đoàn Thế Lực | WorkExperience.cs, AcademicAchievement.cs | GitHub Commit |
|   4 | Tạo các endpoint CRUD cho kinh nghiệm làm việc và thành tích tương ứng với sắp xếp display order. | Đoàn Thế Lực | ProfileController.cs | GitHub Commit |
|   5 | Tạo component UI `ExperienceAchievementsSection` hiển thị danh sách động các mốc sự nghiệp, công nghệ, thành tích. | Đoàn Thế Lực | client/src/app/(private)/settings/components/ExperienceAchievementsSection.tsx | GitHub Commit |
|   6 | Tạo component `<PhoneNumberField />` hỗ trợ chuẩn E.164 (+84), lọc ký tự phi số và thuộc tính ARIA accessibility. | Đoàn Thế Lực | client/src/components/ui/phone-number-field.tsx | GitHub Commit |
|   7 | Viết bộ unit tests `auth.validator.test.ts` kiểm thử boundary mật khẩu và định dạng số điện thoại. | Đoàn Thế Lực | client/src/features/auth/validators/auth.validator.test.ts | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Nâng cấp `AuthService` và `LinkGoogleAccountAsync` để chỉ ghi đè `AvatarUrl` Google khi nguồn gốc là `AvatarSource.Google`. | Đoàn Thế Lực | AuthService.cs | GitHub Commit |
|   2 | Định vị lại `<PasswordStrengthMeter>` lồng bên trong thẻ `<TextField>` của mật khẩu mới trong phần đổi mật khẩu. | Đoàn Thế Lực | client/src/app/(private)/settings/components/SignInMethod.tsx | GitHub Commit |
|   3 | Cải tiến trường nhập số điện thoại tại `ProfileTab.tsx` và `reclaim-view.tsx` sang `<PhoneNumberField />` dùng E.164. | Đoàn Thế Lực | ProfileTab.tsx, reclaim-view.tsx | GitHub Commit |
|   4 | Sửa đổi hàm validation mật khẩu Zod Zod schemas ở client sang dùng `evaluatePasswordPolicy` tách biệt. | Đoàn Thế Lực | auth.validator.ts | GitHub Commit |

### Testing

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Viết bộ kiểm thử tích hợp `AvatarOwnershipTests.cs` kiểm nghiệm 7 kịch bản hoạt động của cơ chế chống ghi đè ảnh Google. | Đoàn Thế Lực | AvatarOwnershipTests.cs | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | fix(profile): prevent Google login from overwriting custom uploaded avatar | https://github.com/Kaivian/CVerify/commit/56e141be54441b637e6fcccd4710a8a5af1cebaa |
| Commit/PR       | feat(profile): implement unified working experience and achievements settings | https://github.com/Kaivian/CVerify/commit/4495c7c81472a48413b7839763e6e2aa604a81b0 |
| Commit/PR       | refactor(auth): standardize password strength meters and phone inputs | https://github.com/Kaivian/CVerify/commit/07cc984a0410fbb14cff63db69a59a4972e6a558 |

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Tách biệt và bảo vệ được nguồn gốc Avatar (AvatarSource) ngăn việc ghi đè ảnh Google.
- Hoàn thành phần thiết lập Kinh nghiệm làm việc & Thành tích với UI/UX cao cấp, hỗ trợ nested achievements.
- Chuẩn hóa form mật khẩu và định dạng số điện thoại chuẩn E.164 (+84) trên client.
- Tạo bộ kiểm thử tự động Avatar integration tests và client validation unit tests.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Phục hồi ảnh đại diện từ các provider khác ngoài Google (ví dụ GitHub OAuth): Hiện tại hệ thống backend mới tích hợp khôi phục từ Google.
```

---

### 4.3. Cải thiện chính

```text
- Trải nghiệm cập nhật thông tin cá nhân của kỹ sư chuyên nghiệp và nhất quán hơn.
- Form nhập liệu số điện thoại có tính kiểm soát cao và chuẩn hóa ở DB level.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này hoàn thiện hệ thống Profile của người dùng (gồm ảnh đại diện, kinh nghiệm và thành tích) đảm bảo tính toàn vẹn và chống ghi đè dữ liệu, đồng thời hoàn thành cuộc rà soát và chuẩn hóa form nhập liệu trên toàn hệ thống client.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Tích hợp đồng bộ/khôi phục ảnh đại diện cho các OAuth provider khác (GitHub, GitLab).
2. Phát triển màn hình hiển thị Kinh nghiệm và Thành tích trên Public Profile cá nhân.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-03    |
