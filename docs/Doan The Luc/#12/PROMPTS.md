# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Avatar Source Persistence, Achievements System & Form Standardizations       |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-06-03T02:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-06-03                                                                             |

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
|   1 | 2026-06-03 | Antigravity | Ngăn chặn ghi đè Avatar Google | Fix the avatar overwrite issue on Google Login. Introduce an AvatarSource enum... | Định nghĩa enum AvatarSource, chỉnh sửa AuthService/ProfileService, và viết integration tests. | Có | GitHub Commit |
|   2 | 2026-06-03 | Antigravity | Quản lý Kinh nghiệm & Thành tích | Implement a unified working experience and achievements settings section... | Tạo DB schemas, Endpoints CRUD, UI dynamic nested form, và reordering. | Có | GitHub Commit |
|   3 | 2026-06-03 | Antigravity | Chuẩn hóa form Mật khẩu & Số điện thoại | Audit and standardize password strength meters and phone inputs... | Tách evaluatePasswordPolicy, xây dựng PhoneNumberField, refactor UI và viết unit tests. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1 (Avatar Source Persistence)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-03                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Ngăn chặn việc Google Login tự động ghi đè ảnh đại diện tải lên thủ công của người dùng.                                                               |
| Phần việc liên quan | Backend / User Entity / AuthService / ProfileService / Integration Tests                                                                                |
| Mức độ sử dụng      | Hỗ trợ sinh khung enum, sửa đổi logic đồng bộ trong auth và profile service, sinh boilerplate integration tests.                                        |

#### 5.1. Prompt nguyên văn

```text
Fix the avatar overwrite issue on Google Login. Introduce an AvatarSource enum (Default, Uploaded, Google, GitHub, GitLab) and persist it in the User entity. Modify DbInitializer to run dynamic schema migration for user.avatar_source and set default values. Update AuthService to only overwrite AvatarUrl if the current source is Google. Update ProfileService to set AvatarSource to Uploaded on manual uploads, add SyncAvatarWithProviderAsync, and add DeleteAvatarAsync to physically remove from R2 storage. Write AvatarOwnershipTests integration tests.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Khi người dùng đăng nhập bằng Google OAuth, hệ thống tự động ghi đè liên kết `AvatarUrl` của Google vào DB của user, làm mất ảnh đại diện tùy chỉnh mà họ đã tải lên trước đó.
- Cần có cơ chế xác định nguồn ảnh đại diện (User.AvatarSource) để kiểm soát việc đồng bộ, xóa vật lý trên Cloudflare R2, và khôi phục ảnh từ provider.
```

#### 5.3. Kết quả AI trả về

```text
AI sinh enum `AvatarSource` và thuộc tính `AvatarSource` trên thực thể User. Sửa `AuthService` kiểm tra `user.AvatarSource == AvatarSource.Google` trước khi cập nhật avatar khi đăng nhập Google. Tạo phương thức `SyncAvatarWithProviderAsync` gán nguồn về provider, và `DeleteAvatarAsync` gán về Default. Viết `AvatarOwnershipTests.cs` kiểm tra 7 kịch bản.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo enum `AvatarSource` và map nó vào ApplicationDbContext.
- Thêm logic gán `AvatarSource = AvatarSource.Uploaded` khi upload avatar trong ProfileService.
- Thêm file `AvatarOwnershipTests.cs` chạy kiểm thử thành công.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Viết lệnh DDL update dữ liệu cũ trong `DbInitializer.InitializeAsync` để map provider tương ứng với các user đã tồn tại (legacy accounts), tránh gán bừa bãi nguồn về Uploaded hay Default.
- Tự thêm logic gọi API xóa vật lý file ảnh trên R2 của user khi họ bấm nút DELETE avatar, tránh rác lưu trữ (storage leakage).
```

---

### Prompt số 2 (Kinh nghiệm & Thành tích)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-03                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Tạo phần quản lý Kinh nghiệm làm việc & Thành tích tích hợp trên trang thiết lập cá nhân của kỹ sư.                                                    |
| Phần việc liên quan | Backend DB / Mapping / CRUD / Frontend Form / Dynamic nesting                                                                                          |
| Mức độ sử dụng      | Hỗ trợ sinh thực thể DB, service CRUD và khung component ExperienceAchievementsSection trên client.                                                     |

#### 5.1. Prompt nguyên văn

```text
Implement a unified working experience and achievements settings section. Create the Experience and Achievement database schemas, enums, mapping configurations, and EF migrations. Implement CRUD services and controllers with date validation constraints, reordering logic, and user ownership checks. Build the frontend settings section using HeroUI v3 and Tailwind CSS v4 to support dynamic adding, nested achievements, custom tech tags, and link attachments. Integrate unsaved changes dirty checking.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Kỹ sư trên CVerify cần có phần để cập nhật lịch sử công việc và các chứng nhận/thành tích kèm theo.
- Thành tích phải lồng trong kinh nghiệm (nested achievements) và hỗ trợ tự động reorder display order, tech tags, và links.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất thực thể `WorkExperience` và `AcademicAchievement` ở backend, liên kết 1-N. endpoints CRUD ở ProfileController hỗ trợ reorder. Trên client, sinh UI component `ExperienceAchievementsSection` cho phép điền các trường, nested list và tag list.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Thêm database schema và EF Core configuration cho 2 thực thể mới.
- endpoints CRUD và reorder logic được áp dụng ở backend.
- UI settings tab mới hoạt động ổn định trên client.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Tự viết logic validate ngày tháng: Ngăn cản StartDate lớn hơn EndDate, chặn EndDate trong tương lai nếu IsCurrent = false.
- Tích hợp dirty check của form với component `UnsavedChangesBar` trên Next.js để cảnh báo khi rời trang chưa lưu.
- Khắc phục lỗi reordering re-indexing DB level tránh race conditions bằng PostgreSQL TRANSACTION.
```

---

### Prompt số 3 (Form Standardizations)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-03                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Chuẩn hóa độ đo mật khẩu và cấu hình trường nhập số điện thoại theo chuẩn E.164 (+84).                                                                 |
| Phần việc liên quan | Frontend UI / Zod Validation / Phone Input Components / Unit testing                                                                                   |
| Mức độ sử dụng      | Hỗ trợ tách evaluatePasswordPolicy, xây dựng component PhoneNumberField, refactor các view liên quan và viết unit tests.                               |

#### 5.1. Prompt nguyên văn

```text
Audit and standardize password strength meters and phone inputs. Decouple password validation logic in password-policy.ts so Zod schemas in auth.validator.ts use evaluatePasswordPolicy as a single source of truth. Create a reusable PhoneNumberField component supporting E.164 (+84) prefixing, non-digit filtering, and accessibility aria attributes. Refactor ProfileTab and reclaim-view to consume PhoneNumberField. Create unit tests for validation schemas.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- UI mật khẩu của CVerify bị bất đồng bộ giữa cài đặt (SignInMethod) và các trang onboarding (Password Strength Meter nằm ngoài TextField).
- Số điện thoại ở client đang nhập thủ công với đầu số cứng `+084`, vi phạm chuẩn E.164 lưu trữ.
- Validation Zod password schema và Password Strength UI bị lệch yêu cầu cho tài khoản Enterprise.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất code PhoneNumberField với Prefix hiển thị tĩnh, tự động gộp prefix và số khi onChange. Tách evaluatePasswordPolicy và viết bộ kiểm thử validation bằng Zod.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo PhoneNumberField.tsx, lồng PasswordStrengthMeter vào trong TextField của SignInMethod.tsx.
- Refactor ProfileTab.tsx và reclaim-view.tsx dùng PhoneNumberField.
- Tạo file auth.validator.test.ts kiểm chứng thành công.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Mở rộng PhoneNumberField: Thêm prop `onBlur` để cắm mốc validate chuẩn React Hook Form/ReclaimView touched states.
- Accessibility: Bổ sung aria-hidden="true" cho prefix, aria-label động, tự động phối hợp lỗi validation qua aria-describedby.
- Xử lý các tiền tố thay thế khi người dùng copy paste số điện thoại (ví dụ: gõ 084 hoặc gõ số 0 ở đầu sẽ tự cắt và format thành E.164 +84).
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần cung cấp bối cảnh cụ thể của framework UI đang dùng (như HeroUI v3 hay React Hook Form) và các ràng buộc đặc thù quốc gia (như chuẩn định dạng số điện thoại di động/cố định của Việt Nam là 9-10 số sau mã +84) để tránh AI sinh regex không phù hợp.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Khi đặt câu hỏi, việc yêu cầu tách biệt logic (pure functions) khỏi UI presentation (như password validation policy) giúp tăng tính tái sử dụng và khả năng viết unit tests độc lập dễ dàng hơn nhiều.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Design |        3 | Fix the avatar overwrite issue on Google Login... / Implement a unified working experience... / Audit and standardize password... |

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
| Đoàn Thế Lực            | 2026-06-03    |
