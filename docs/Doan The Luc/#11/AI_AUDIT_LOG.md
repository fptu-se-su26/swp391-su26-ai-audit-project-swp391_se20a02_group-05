# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu          | 2026-06-02T14:40:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-02T15:38:00.000Z                                                               |

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
Triển khai hệ thống username tự động cho nền tảng CVerify, bao gồm: dịch vụ UsernameService quản lý validation/normalization/generation với cơ chế retry optimistic cho concurrent registration, tích hợp tự động gán username vào luồng đăng ký (email + Google OAuth), xây dựng endpoint public profile lookup theo username (GET /v1/users/profile/public/{username}), migration backward-compatible cho legacy users chưa có username, và trang client [username] dynamic route để hiển thị hồ sơ công khai.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-02                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Thiết kế và triển khai hệ thống username tự động: sinh username từ email, validation, normalization, unique generation với retry, và cooldown 30 ngày.    |
| Phần việc liên quan | Backend / Shared Security Service / Database Migration / Auth Integration / Profile API / Client Routing                                                |
| Mức độ sử dụng      | Hỗ trợ sinh mã UsernameService, tích hợp vào AuthService và RecoveryExecutionEngine, tạo migration schema và legacy backfill, xây dựng public profile API và client page. |

#### 4.1. Prompt đã sử dụng

```text
Implement an automatic username system for CVerify. Create a UsernameService in Shared/Security that generates usernames from the email local part, validates format (3-30 chars, alphanumeric + underscore/hyphen/period), checks reserved words (matching app routes), enforces a 30-day change cooldown, and handles concurrent uniqueness collisions via optimistic retry with sequential suffixes. Integrate auto-generation into registration and Google OAuth flows. Add a public profile endpoint GET /v1/users/profile/public/{username} with signed avatar URLs. Create database migration for username (CITEXT) and last_username_change_at columns with a unique partial index. Backfill legacy users. Build a Next.js [username] dynamic route page.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất tạo IUsernameService/UsernameService trong Modules/Shared/Security với các phương thức: ValidateUsername (kiểm tra format, độ dài, reserved words), Normalize (lowercase + trim), GenerateBaseUsername (trích xuất từ email local part, sanitize ký tự), GenerateUniqueUsernameAsync (sequential suffix check), RunWithUsernameRetryAsync (optimistic retry bắt PostgresException 23505 trên constraint username), và CheckChangeCooldownAsync (30-day cooldown). AI cũng sinh schema migration DDL với CITEXT column và unique partial index idx_users_username_active, legacy backfill MigrateLegacyUsernamesAsync, public profile endpoint với signed avatar URL, và trang [username]/page.tsx trên client.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Bộ khung dịch vụ UsernameService với các phương thức validation, normalization, và generation cơ bản.
- Cấu trúc retry logic trong RunWithUsernameRetryAsync bắt PostgresException (SqlState 23505) trên constraint chứa "username".
- Migration DDL cho cột username (CITEXT) và partial unique index trên bảng users.
- Endpoint public profile lookup trong ProfileController và ProfileService.
- Trang [username]/page.tsx client sử dụng Next.js dynamic routing.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Thiết kế danh sách reserved usernames bao phủ toàn bộ các route ứng dụng hiện tại (admin, login, register, settings, chat, business, gateway, v.v.) để ngăn chặn xung đột routing giữa username và application routes.
- Bổ sung safe migration DDL với kiểm tra IF NOT EXISTS cho cả 3 bảng (users, organizations, user_profiles) đảm bảo backward compatibility khi triển khai trên database đã tồn tại.
- Tích hợp signed avatar URL resolution trong public profile sử dụng IEncryptedFileStorageService để hỗ trợ cả external URL (Google avatar) và internal R2 storage.
- Viết UserBuilder test helper tự động sinh username từ email local part cho test fixtures, giúp integration tests không bị vi phạm NOT NULL constraint sau khi thêm username column.
- Refactor Level2RecoveryTests chuyển từ inline User initialization sang UserBuilder pattern thống nhất.
- Viết UsernameServiceTests (unit) và UsernameFlowTests (integration) bao phủ validation, generation, cooldown, uniqueness, và reserved words.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(profile): implement automatic username system with public profile routing | https://github.com/Kaivian/CVerify/commit/5fbd9d3 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Hệ thống username tự động giải quyết bài toán identity công khai cho người dùng CVerify: mỗi user giờ đây có một định danh duy nhất, thân thiện với URL, có thể chia sẻ và truy cập trực tiếp qua /{username}. Cơ chế optimistic retry đảm bảo tính toàn vẹn username ngay cả trong trường hợp concurrent registration, và cooldown 30 ngày ngăn chặn lạm dụng thay đổi username liên tục.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                                          |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------------------ |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích thiết kế hệ thống username và public profile routing.                                   |
| Viết user story/use case    |       x       |              |                 |               |                                                                                                  |
| Thiết kế database           |               |      x       |                 |               | Thêm CITEXT column, partial unique index, và safe migration DDL.                                  |
| Thiết kế kiến trúc hệ thống |               |              |        x        |               | Thiết kế UsernameService trong Shared/Security với optimistic retry pattern.                       |
| Thiết kế giao diện          |               |              |        x        |               | Trang [username] dynamic route hiển thị public profile.                                           |
| Code frontend               |               |              |        x        |               | Xây dựng trang public profile, thêm types và API service.                                         |
| Code backend                |               |              |        x        |               | Triển khai UsernameService, tích hợp Auth/Recovery/Profile, migration legacy backfill.             |
| Debug lỗi                   |               |      x       |                 |               | Sửa lỗi test fixtures thiếu username sau khi thêm column mới.                                    |
| Viết test case              |               |              |        x        |               | Unit tests cho UsernameService và integration tests cho username flows.                            |
| Kiểm thử sản phẩm           |       x       |              |                 |               | Chạy dotnet run, npm run build, và kiểm thử thủ công luồng UI.                                   |
| Tối ưu code                 |       x       |              |                 |               |                                                                                                  |
| Viết báo cáo                |       x       |              |                 |               |                                                                                                  |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                                  |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI không tự động cập nhật các test fixtures hiện có (như Level2RecoveryTests) khi thêm username column mới vào User entity, dẫn đến integration tests thất bại do thiếu required username field. | Chạy `dotnet test` phát hiện các test seed User trực tiếp bị lỗi missing username. | Refactor các test sử dụng UserBuilder pattern với auto-generated username từ email, và bổ sung hardcoded username cho RegistrationFlowTests. |
|   2 | AI sinh danh sách reserved usernames chưa đầy đủ so với toàn bộ routes trong ứng dụng Next.js, có nguy cơ conflict giữa username và application route. | Kiểm tra thủ công danh sách routes từ `npm run build` output và so sánh với ReservedUsernames HashSet. | Bổ sung đầy đủ các route names vào danh sách reserved: company-onboarding, company-verification, continue-with-email, forgot-password, gateway, reset-password, verify-email, workspace-setup. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả thông qua:
1. Build thành công production bundle Next.js client (npm run build) với 32 routes bao gồm dynamic route [username].
2. Khởi chạy thành công backend ASP.NET Core (dotnet run) với ValidateOnBuild = true, xác nhận IUsernameService đã được đăng ký đúng trong DI container.
3. Viết và chạy UsernameServiceTests (unit) kiểm tra validation rules, normalization, reserved words, base generation, và cooldown enforcement.
4. Viết và chạy UsernameFlowTests (integration) kiểm tra auto-generation khi đăng ký, update với cooldown, uniqueness constraint, và validation API rules.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Thiết kế danh sách reserved usernames mapping 1:1 với toàn bộ application routes để ngăn xung đột routing.
- Triển khai safe migration DDL kiểm tra IF NOT EXISTS cho 3 bảng (users, organizations, user_profiles) đảm bảo zero-downtime deployment.
- Tích hợp signed avatar URL resolution phân biệt external URL vs internal R2 storage key trong public profile.
- Refactor UserBuilder pattern cho test infrastructure và viết test coverage cho username flows.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                             | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai UsernameService, tích hợp Auth/Recovery/Profile, migration, public profile API, client page, tests. | Có                   | https://github.com/Kaivian/CVerify/commit/5fbd9d3 |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp sinh nhanh bộ khung UsernameService với các phương thức validation/normalization/generation, retry logic cho concurrent uniqueness, và tích hợp xuyên suốt nhiều modules (Auth, Recovery, Profiles). AI cũng hỗ trợ sinh migration DDL và trang client dynamic route.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng danh sách reserved usernames ban đầu do AI sinh ra vì nó chỉ bao gồm một số route cơ bản (admin, login, register) mà thiếu nhiều route thực tế của ứng dụng (company-onboarding, company-verification, workspace-setup, v.v.). Sinh viên đã tự bổ sung đầy đủ bằng cách kiểm tra output của npm run build.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Nhóm kiểm chứng bằng cách build thành công cả frontend (npm run build) và backend (dotnet run), chạy unit tests cho UsernameService và integration tests cho username flows, đồng thời kiểm tra thủ công trên trình duyệt rằng dynamic route [username] hoạt động đúng.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần khó khăn nhất là tích hợp username auto-generation xuyên suốt nhiều modules (Auth registration, Google OAuth callback, Recovery user provisioning) một cách nhất quán, đồng thời đảm bảo backward compatibility với legacy users thông qua migration backfill.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Học được cách thiết kế hệ thống identity công khai (username) với các yêu cầu: uniqueness (CITEXT + partial index), reserved word protection (route conflict prevention), rate limiting (30-day cooldown), và concurrent safety (optimistic retry pattern).
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI có xu hướng sinh danh sách cấu hình (reserved words) dựa trên tri thức tổng quát thay vì phân tích trực tiếp codebase thực tế. Sinh viên phải luôn kiểm tra chéo kết quả AI với trạng thái thực tế của ứng dụng (ví dụ: danh sách routes từ build output) để đảm bảo tính đầy đủ.
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
| Đoàn Thế Lực            | 2026-06-02    |
