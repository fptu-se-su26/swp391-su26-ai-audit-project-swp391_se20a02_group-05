# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu          | 2026-06-03T02:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-03T04:30:00.000Z                                                               |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Mục tiêu là hoàn thiện cơ chế quản lý Avatar (AvatarSource Persistence) ngăn chặn việc ghi đè ảnh đại diện tải lên bằng tài khoản Google, xây dựng toàn diện hệ thống quản lý kinh nghiệm làm việc và thành tích (Working Experience & Achievements), và thực hiện chuẩn hóa giao diện (UI Consistency) cho bộ đo mật khẩu (Password Strength Meter) và trường nhập số điện thoại (E.164 PhoneNumberField) trên toàn bộ ứng dụng CVerify.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1 (Fix Google Avatar Overwrite)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-03                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Tách biệt nguồn ảnh đại diện (AvatarSource) để ngăn chặn Google login tự động ghi đè ảnh đại diện tự tải lên của người dùng.                           |
| Phần việc liên quan | Backend / Domain Entities / AuthService / ProfileService / Integration Testing                                                                          |
| Mức độ sử dụng      | Hỗ trợ sinh schema migration, định nghĩa enum AvatarSource, chỉnh sửa logic trong AuthService/ProfileService và thiết lập bộ tích hợp kiểm thử.        |

#### 4.1. Prompt đã sử dụng

```text
Fix the avatar overwrite issue on Google Login. Introduce an AvatarSource enum (Default, Uploaded, Google, GitHub, GitLab) and persist it in the User entity. Modify DbInitializer to run dynamic schema migration for user.avatar_source and set default values. Update AuthService to only overwrite AvatarUrl if the current source is Google. Update ProfileService to set AvatarSource to Uploaded on manual uploads, add SyncAvatarWithProviderAsync, and add DeleteAvatarAsync to physically remove from R2 storage. Write AvatarOwnershipTests integration tests.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất thêm trường AvatarSource dạng enum trong User entity, map thành cột cơ sở dữ liệu `avatar_source` trong DbContext. AI sửa Program/AuthService để chặn việc tự động ghi đè AvatarUrl khi login bằng Google nếu nguồn hiện tại là 'Uploaded'. Viết các API sync và delete avatar, đồng thời sinh 7 ca kiểm thử tích hợp trong AvatarOwnershipTests.cs.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Định nghĩa enum AvatarSource và tích hợp vào User entity ở domain level.
- Thay đổi logic trong ProfileService.UploadAvatar để gán AvatarSource = AvatarSource.Uploaded.
- 7 ca kiểm thử tích hợp trong file AvatarOwnershipTests.cs.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Chỉnh sửa cấu hình migrations trong DbInitializer.InitializeAsync để thêm cột `avatar_source` (default value = 0) và tự động đồng bộ giá trị với linked accounts sẵn có trong hệ thống bằng raw SQL để tránh data anomalies.
- Thiết kế phương thức vật lý dọn dẹp ảnh trên R2 storage khi user xóa avatar bằng cách gọi IStorageService.DeleteFileAsync một cách đồng bộ trước khi đổi nguồn về AvatarSource.Default.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | fix(profile): prevent Google login from overwriting custom uploaded avatar | https://github.com/Kaivian/CVerify/commit/56e141be54441b637e6fcccd4710a8a5af1cebaa |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Giải pháp tách biệt AvatarSource hoạt động chính xác. Nó bảo vệ quyền sở hữu của người dùng đối với ảnh đại diện tùy chỉnh, đồng thời cung cấp tính năng khôi phục (sync) linh hoạt nếu họ muốn đồng bộ lại với nhà cung cấp thứ ba.
```

---

### Lần sử dụng AI số 2 (Working Experience & Achievements Settings)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-03                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Xây dựng thực thể và UI quản lý kinh nghiệm làm việc (Experience) và thành tích học tập/công việc (Achievements).                                       |
| Phần việc liên quan | Fullstack (PostgreSQL DB, EF Core Service, Controller API, React UI Settings Tab)                                                                       |
| Mức độ sử dụng      | Hỗ trợ sinh mã thực thể, service CRUD backend, và cấu trúc component ExperienceAchievementsSection trên frontend.                                       |

#### 4.1. Prompt đã sử dụng

```text
Implement a unified working experience and achievements settings section. Create the Experience and Achievement database schemas, enums, mapping configurations, and EF migrations. Implement CRUD services and controllers with date validation constraints, reordering logic, and user ownership checks. Build the frontend settings section using HeroUI v3 and Tailwind CSS v4 to support dynamic adding, nested achievements, custom tech tags, and link attachments. Integrate unsaved changes dirty checking.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất thực thể WorkExperience và AcademicAchievement, liên kết 1-N. Thiết kế các endpoints CRUD trong Profiles module. Trên client, thiết kế component ExperienceAchievementsSection sử dụng React Hook Form để hỗ trợ thêm mới động và hiển thị nested list thành tích.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Thực thể DB và cấu hình mapping EF Core.
- Logic sắp xếp display order và CRUD ở backend service.
- Interface UI và form layout dynamic items trên settings client.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Viết logic kiểm tra ràng buộc ngày tháng tùy chỉnh ở backend: StartDate phải nhỏ hơn EndDate, và ngăn cấm EndDate trong tương lai nếu trạng thái không phải là IsCurrent.
- Tự triển khai cơ chế reordering re-indexing thông minh tại DB level bằng giao dịch Transaction Scope để tránh race conditions.
- Tích hợp dirty checker UnsavedChangesBar của client với form state của React Hook Form để thông báo cho người dùng khi rời trang mà chưa lưu dữ liệu.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(profile): implement unified working experience and achievements settings | https://github.com/Kaivian/CVerify/commit/4495c7c81472a48413b7839763e6e2aa604a81b0 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Tính năng mang lại giá trị cao cho hồ sơ chuyên nghiệp của kỹ sư trên CVerify. Việc lồng ghép thành tích trực tiếp vào từng mốc kinh nghiệm giúp minh chứng năng lực rõ ràng hơn.
```

---

### Lần sử dụng AI số 3 (Form Standardizations)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-03                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Chuẩn hóa độ đo mật khẩu và cấu hình trường nhập số điện thoại theo chuẩn E.164 (+84).                                                                 |
| Phần việc liên quan | Frontend / Form Components / Validation Schemas / Localizations / Testing                                                                               |
| Mức độ sử dụng      | Hỗ trợ tái cấu trúc PasswordStrengthMeter, xây dựng PhoneNumberField, refactor ProfileTab & ReclaimView, và viết unit tests.                            |

#### 4.1. Prompt đã sử dụng

```text
Audit and standardize password strength meters and phone inputs. Decouple password validation logic in password-policy.ts so Zod schemas in auth.validator.ts use evaluatePasswordPolicy as a single source of truth. Create a reusable PhoneNumberField component supporting E.164 (+84) prefixing, non-digit filtering, and accessibility aria attributes. Refactor ProfileTab and reclaim-view to consume PhoneNumberField. Create unit tests for validation schemas.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất tách hàm evaluatePasswordPolicy trong password-policy.ts. Thiết kế PhoneNumberField với input filtering loại bỏ ký tự không phải số. Viết test case sử dụng Zod schema check boundary mật khẩu và phone format.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Hàm evaluatePasswordPolicy tách biệt khỏi UI layout.
- PhoneNumberField cơ bản với static prefix.
- Mã kiểm thử validation trong auth.validator.test.ts.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Mở rộng PhoneNumberField: Hỗ trợ thuộc tính onBlur để tích hợp mượt mà với React Hook Form và touched validation states của ReclaimView.
- Cấu hình accessibility chi tiết: aria-hidden="true" cho prefix, aria-label động dịch nghĩa mã quốc gia, liên kết aria-describedby tự động tới validation errors.
- Đồng bộ hóa định dạng hiển thị số điện thoại (tự động cắt bỏ số 0 ở đầu nếu nhập local format và chuyển sang +84 khi lưu database).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | refactor(auth): standardize password strength meters and phone inputs | https://github.com/Kaivian/CVerify/commit/07cc984a0410fbb14cff63db69a59a4972e6a558 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Chuẩn hóa trường số điện thoại giúp đảm bảo tính nhất quán định dạng dữ liệu (E.164) gửi lên backend, tránh lỗi validate định dạng trên SMS gateways hay database constraints sau này.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                                          |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------------------ |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích các lỗi và yêu cầu chuẩn hóa form.                                                     |
| Viết user story/use case    |       x       |              |                 |               |                                                                                                  |
| Thiết kế database           |               |      x       |                 |               | Tạo WorkExperience, AcademicAchievement, AvatarSource.                                           |
| Thiết kế kiến trúc hệ thống |               |              |        x        |               | Thiết kế pure policy evaluator cho mật khẩu và cấu trúc component PhoneNumberField.             |
| Thiết kế giao diện          |               |              |        x        |               | Thiết kế UI lồng ghép của phần kinh nghiệm/thành tích và nested form.                             |
| Code frontend               |               |              |        x        |               | PhoneNumberField, ExperienceAchievementsSection, ProfileTab, SignInMethod.                       |
| Code backend                |               |              |        x        |               | AvatarSource logic, WorkExperience CRUD endpoints, và DbInitializer migrations.                   |
| Debug lỗi                   |               |      x       |                 |               | Giải quyết lỗi ghi đè URL avatar của Google và đồng bộ migrations.                               |
| Viết test case              |               |              |        x        |               | Xây dựng AvatarOwnershipTests.cs (integration) và auth.validator.test.ts (unit).                 |
| Kiểm thử sản phẩm           |       x       |              |                 |               | Chạy build bundle Next.js và kiểm thử 7/7 scenarios Avatar tests thành công.                    |
| Tối ưu code                 |       x       |              |                 |               |                                                                                                  |
| Viết báo cáo                |       x       |              |                 |               |                                                                                                  |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                                  |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI thiết kế PhoneNumberField ban đầu thiếu prop `onBlur`, dẫn đến form ReclaimView không thể kích hoạt touched states đúng thời điểm. | Biên dịch client phát hiện lỗi TypeScript trên ReclaimView vì `PhoneNumberField` không nhận prop `onBlur`. | Bổ sung `onBlur` vào interface `PhoneNumberFieldProps` và forward vào thẻ input nguyên bản. |
|   2 | Logic ghi đè Google Avatar lúc đầu dựa vào so sánh chuỗi URL (fragile string check) dễ bị lỗi nếu Google ký mới URL. | Đăng nhập lại Google Account sau khi chỉnh sửa avatar thủ công, avatar vẫn bị ghi đè do URL thay đổi. | Triển khai cờ AvatarSource dạng enum lưu cứng vào DB, kiểm tra điều kiện enum thay vì kiểm tra URL. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả qua các hình thức sau:
1. Chạy thành công 7 integration tests trong AvatarOwnershipTests.cs, bảo đảm logic đồng bộ, xóa, và chống ghi đè avatar hoạt động chính xác 100%.
2. Chạy thành công bộ unit tests auth.validator.test.ts kiểm tra 12 ca kiểm thử password policy và E.164 phone formats.
3. Build thành công bundle client (npm run build) đảm bảo Next.js không có lỗi TypeScript hay routing.
4. Chạy thực tế và lưu số điện thoại ở ProfileTab, kiểm tra Postgres DB lưu chính xác định dạng E.164 (+84).
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Trực tiếp bổ sung prop onBlur và thiết lập cấu hình thuộc tính ARIA hỗ trợ trình đọc màn hình trong PhoneNumberField.
- Thiết kế logic migration database cho avatar_source trong DbInitializer bằng raw SQL để cập nhật an toàn dữ liệu lịch sử.
- Viết 7 integration test cases trong AvatarOwnershipTests.cs bảo đảm bao phủ toàn bộ vòng đời avatar.
- Viết bộ unit tests cho password policy và phone format trong client.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                             | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Sửa lỗi Google Avatar overwrite, triển khai Experience & Achievements, chuẩn hóa form mật khẩu/số điện thoại, kiểm thử. | Có                   | https://github.com/Kaivian/CVerify/commit/56e141be54441b637e6fcccd4710a8a5af1cebaa, https://github.com/Kaivian/CVerify/commit/4495c7c81472a48413b7839763e6e2aa604a81b0, https://github.com/Kaivian/CVerify/commit/07cc984a0410fbb14cff63db69a59a4972e6a558 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm tra thủ công giao diện settings tab mới trên các kích thước màn hình responsive.      | Không                |                     |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp tăng tốc đáng kể tiến độ sinh boilerplate code cho các thực thể mới, endpoints CRUD, và các components React phức tạp, giúp nhóm tập trung thời gian vào xử lý logic nghiệp vụ và tối ưu hóa trải nghiệm người dùng.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng logic kiểm tra avatar dựa trên URL chuỗi của AI. Thiết kế này thiếu tính bền vững vì URL ảnh đại diện của các nhà cung cấp bên thứ ba (như Google/GitHub) là các URL động có thể thay đổi chữ ký (signature) theo thời gian.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?
```text
Nhóm kiểm chứng thông qua việc viết đầy đủ integration test suite ở backend chạy trực tiếp trên database thực tế và viết unit tests độc lập ở frontend.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?
```text
Phần dựng giao diện nested form động (Kinh nghiệm lồng thành tích) trên frontend vì nó đòi hỏi nhiều mã xử lý mảng động phức tạp và đồng bộ state của React Hook Form.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?
```text
Học được cách tổ chức và chuẩn hóa giao diện người dùng theo chuẩn thiết kế nhất quán, cách xử lý đồng bộ dữ liệu với bên thứ ba (OAuth sync) một cách đáng tin cậy.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?
```text
Khi sử dụng AI cho các trường hợp nghiệp vụ đặc thù (như xử lý số điện thoại quốc gia hay đồng bộ ảnh), phải hiểu rõ các tiêu chuẩn thực tế (như E.164) để bổ sung hướng dẫn chi tiết cho AI, tránh để AI tự thiết kế theo chuẩn chung chung.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-03    |
