# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Automatic Username System & Public Profile Routing                            |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-06-02T14:40:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-06-02                                                                             |

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
|   1 | 2026-06-02 | Antigravity | Triển khai hệ thống username tự động và public profile | Implement an automatic username system for CVerify with UsernameService, auth integration, public profile endpoint, database migration, and client page... | Tạo UsernameService, tích hợp Auth/Recovery, migration, public profile API, client [username] page, và tests. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-02                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Triển khai hệ thống username tự động với public profile routing.                                                                                        |
| Phần việc liên quan | Backend / Shared Security Service / Database Migration / Auth Integration / Profile API / Client Routing / Testing                                      |
| Mức độ sử dụng      | Hỗ trợ sinh mã UsernameService, tích hợp đa module, migration DDL, public profile API, client page, và test infrastructure.                             |

#### 5.1. Prompt nguyên văn

```text
Implement an automatic username system for CVerify. Create a UsernameService in Shared/Security that generates usernames from the email local part, validates format (3-30 chars, alphanumeric + underscore/hyphen/period), checks reserved words (matching app routes), enforces a 30-day change cooldown, and handles concurrent uniqueness collisions via optimistic retry with sequential suffixes. Integrate auto-generation into registration and Google OAuth flows. Add a public profile endpoint GET /v1/users/profile/public/{username} with signed avatar URLs. Create database migration for username (CITEXT) and last_username_change_at columns with a unique partial index. Backfill legacy users. Build a Next.js [username] dynamic route page.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- CVerify chưa có hệ thống username: người dùng chỉ được nhận diện bằng email nội bộ, không có identity công khai thân thiện URL.
- Cần username tự động gán khi đăng ký để mỗi user có trang hồ sơ công khai truy cập qua /{username}.
- Username phải unique ở database level (sử dụng CITEXT cho case-insensitive), phải tránh xung đột với application routes, và phải hỗ trợ concurrent registration.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất tạo IUsernameService/UsernameService trong Modules/Shared/Security với 6 phương thức chính:
1. ValidateUsername: kiểm tra format (regex ^[a-zA-Z0-9_\-\.]+$), độ dài 3-30, reserved words.
2. Normalize: lowercase + trim.
3. GenerateBaseUsername: trích xuất email local part, sanitize ký tự không hợp lệ, pad nếu quá ngắn.
4. GenerateUniqueUsernameAsync: sequential suffix check qua database query.
5. RunWithUsernameRetryAsync: optimistic retry bắt PostgresException SqlState 23505 trên constraint chứa "username", tối đa 5 lần.
6. CheckChangeCooldownAsync: kiểm tra LastUsernameChangeAt, enforce 30-day cooldown.

Kèm theo migration DDL (CITEXT column + unique partial index), MigrateLegacyUsernamesAsync cho backfill, public profile endpoint với signed avatar, AuthResponse DTO mở rộng, và [username]/page.tsx client page.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo UsernameService.cs và IUsernameService.cs trong Modules/Shared/Security.
- Tích hợp RunWithUsernameRetryAsync vào AuthService (registration + Google OAuth) và RecoveryExecutionEngine (user provisioning).
- Thêm cột username (CITEXT) và last_username_change_at vào schema users.
- Tạo PublicProfileResponse DTO và endpoint GET /v1/users/profile/public/{username}.
- Tạo client/src/app/[username]/page.tsx dynamic route.
- Mở rộng auth.types.ts và profile.types.ts trên client.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Bổ sung đầy đủ reserved usernames (26 route names) bao phủ toàn bộ application routes từ output npm run build thay vì danh sách tổng quát ban đầu của AI.
- Thiết kế safe migration DDL kiểm tra IF NOT EXISTS cho 3 bảng (users, organizations, user_profiles) để backward compatibility.
- Tích hợp signed avatar URL resolution phân biệt external URL vs internal R2 storage trong public profile.
- Refactor test fixtures: UserBuilder auto-generate username, Level2RecoveryTests dùng UserBuilder pattern, RegistrationFlowTests thêm hardcoded username.
- Viết UsernameServiceTests (unit) và UsernameFlowTests (integration) bao phủ validation, generation, cooldown, uniqueness, reserved words.
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
| Commit          | https://github.com/Kaivian/CVerify/commit/5fbd9d3 |

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Khi yêu cầu AI sinh danh sách cấu hình (reserved words, whitelist, blacklist), cần cung cấp output thực tế của ứng dụng (ví dụ: danh sách routes từ npm run build) thay vì để AI tự suy luận từ tri thức tổng quát. AI có xu hướng sinh danh sách tối thiểu thay vì đầy đủ.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Prompt mô tả tổng thể feature (username system) kết hợp với yêu cầu cụ thể về từng thành phần (validation rules, retry pattern, migration DDL) cho kết quả tốt nhất. Tuy nhiên, phần test fixtures cần được nhắc nhở rõ ràng trong prompt vì AI thường bỏ qua việc cập nhật test data khi thêm trường mới vào entity.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Design |        1 | Implement an automatic username system for CVerify with UsernameService, auth integration, public profile endpoint... |

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
| Đoàn Thế Lực            | 2026-06-02    |
