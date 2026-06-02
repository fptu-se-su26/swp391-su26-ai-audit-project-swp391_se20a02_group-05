# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Automatic Username System & Public Profile Routing                            |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-06-02                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình triển khai hệ thống username tự động và public profile routing, AI hỗ trợ sinh bộ khung dịch vụ UsernameService (validation, normalization, generation, retry logic), tích hợp đa module (Auth, Recovery, Profiles), migration DDL, và trang client dynamic route. Sinh viên giữ vai trò chủ đạo trong việc: bổ sung đầy đủ reserved usernames từ application routes thực tế, thiết kế safe migration DDL backward-compatible cho production database, tích hợp signed avatar URL resolution phân biệt external/internal storage, refactor test infrastructure (UserBuilder pattern, hardcoded username fixtures), và viết bộ tests bao phủ validation/generation/cooldown/uniqueness.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Antigravity
```

### Lý do sử dụng công cụ đó

```text
Antigravity cho phép phân tích cấu trúc dự án ASP.NET Core v10 + Next.js 16 đồng thời, hỗ trợ tích hợp xuyên suốt nhiều modules backend (Auth, Recovery, Profiles, Shared) và frontend (types, services, pages) trong cùng một phiên làm việc.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [x] Viết test case
- [ ] Review code
- [ ] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI hỗ trợ sinh toàn bộ bộ khung UsernameService với 6 phương thức chính (ValidateUsername, Normalize, GenerateBaseUsername, GenerateUniqueUsernameAsync, RunWithUsernameRetryAsync, CheckChangeCooldownAsync). AI cũng hỗ trợ tích hợp username generation vào AuthService (registration + Google OAuth), RecoveryExecutionEngine (user provisioning), và ProfileService (public profile lookup). Trên phía client, AI sinh trang [username]/page.tsx và mở rộng type definitions.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu cách thiết kế optimistic retry pattern cho concurrent uniqueness constraint trên PostgreSQL (bắt SqlState 23505 trên constraint cụ thể).
- Nắm vững phương pháp sử dụng CITEXT column type của PostgreSQL cho case-insensitive username comparison thay vì phải LOWER() mọi query.
- Học cách thiết kế safe migration DDL với IF NOT EXISTS check cho backward compatibility trên production database.
- Hiểu cách tích hợp cross-cutting concern (username) xuyên suốt nhiều modules trong kiến trúc Modular Monolith mà không vi phạm ranh giới module.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI sinh danh sách reserved usernames chỉ bao gồm các route tổng quát (admin, login, register, settings) mà thiếu nhiều route đặc thù của ứng dụng (company-onboarding, company-verification, workspace-setup, v.v.), tạo nguy cơ conflict giữa username và application routes.
- AI không tự động cập nhật test fixtures hiện có khi thêm trường mới vào entity (username column), dẫn đến integration tests thất bại hàng loạt do seed data thiếu username.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm sử dụng AI để tăng tốc sinh boilerplate cho dịch vụ username và tích hợp đa module. Các quyết định thiết kế quan trọng (reserved words đầy đủ, safe migration DDL, signed avatar URL, test refactoring) đều do sinh viên tự thực hiện dựa trên kiểm tra thực tế codebase và build output.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [x] Viết test case
- [x] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [x] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Nhóm kiểm chứng bằng cách:
1. Build thành công production bundle Next.js (npm run build) xác nhận 32 routes bao gồm [username] dynamic route.
2. Khởi chạy backend ASP.NET Core (dotnet run) với ValidateOnBuild = true, xác nhận IUsernameService đăng ký đúng trong DI container.
3. Viết UsernameServiceTests (unit) kiểm tra validation, normalization, generation, reserved words, cooldown.
4. Viết UsernameFlowTests (integration) kiểm tra auto-generation khi đăng ký, update với cooldown, uniqueness, validation rules.
5. Kiểm tra thủ công danh sách routes vs reserved usernames để đảm bảo không có conflict.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý danh sách reserved usernames gồm khoảng 15 từ cơ bản (admin, api, login, register, settings, dashboard, profile, privacy, terms, support, help, chat, business, user, organization, auth, system, unauthorized). |
| Em/nhóm đã kiểm tra bằng cách nào? | So sánh với output của `npm run build` liệt kê toàn bộ 32 application routes. |
| Kết quả kiểm tra | Phát hiện thiếu 8 route names: company-onboarding, company-verification, continue-with-email, forgot-password, gateway, reset-password, verify-email, workspace-setup. |
| Em/nhóm đã xử lý tiếp như thế nào? | Bổ sung đầy đủ các route names vào ReservedUsernames HashSet trong UsernameService.cs. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Không tự động cập nhật test seed data (User entities trong Level2RecoveryTests, RegistrationFlowTests) khi thêm username column mới vào User entity. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Khi thêm trường mới vào entity, toàn bộ test fixtures tạo User trực tiếp (không qua Builder) sẽ thiếu trường username, gây lỗi database constraint hoặc assertion failure trong integration tests. |
| Em/nhóm phát hiện bằng cách nào? | Chạy `dotnet test` phát hiện Level2RecoveryTests và RegistrationFlowTests thất bại do User seed data thiếu username. |
| Em/nhóm đã sửa như thế nào? | Refactor Level2RecoveryTests sử dụng UserBuilder pattern (tự sinh username), thêm hardcoded username cho RegistrationFlowTests seed data, và mở rộng UserBuilder với auto-generate username logic. |
| Bài học rút ra | Khi thêm trường bắt buộc (hoặc có unique constraint) vào entity, cần kiểm tra và cập nhật toàn bộ test fixtures đang tạo entity đó. AI cần được nhắc nhở rõ ràng trong prompt về việc cập nhật test data. |

---

## 9. Phân đóng góp thật sự của sinh viên/nhóm

```text
- Bổ sung đầy đủ 26 reserved usernames từ application routes thực tế (npm run build output).
- Thiết kế safe migration DDL với IF NOT EXISTS cho 3 bảng đảm bảo backward compatibility.
- Tích hợp signed avatar URL resolution phân biệt external URL (Google avatar) vs internal R2 key.
- Refactor UserBuilder test helper tự sinh username, chuyển Level2RecoveryTests sang Builder pattern.
- Viết UsernameServiceTests (unit) và UsernameFlowTests (integration).
- Bổ sung hardcoded username cho RegistrationFlowTests seed data.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Rút ngắn ~60% thời gian tích hợp username xuyên suốt 7 modules backend + 4 files client. |
| Code Quality | Good | Excellent | UsernameService encapsulated với validation, retry, cooldown trong một service duy nhất. |
| Testing | Good | Excellent | Bổ sung unit + integration tests bao phủ validation, generation, cooldown, uniqueness. |

---

## 11. Bài học về môn học

- Hệ thống identity công khai (username) đòi hỏi thiết kế đa tầng: application-level validation + database-level uniqueness (CITEXT + partial index) + application-level retry cho concurrent safety.
- Reserved word protection là yêu cầu bắt buộc khi username được sử dụng trong URL routing để tránh conflict với application routes.
- Backward compatibility migration (backfill legacy data) là kỹ năng thiết yếu khi thêm trường mới vào hệ thống đã có dữ liệu production.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- AI sinh danh sách cấu hình (reserved words, whitelist) dựa trên tri thức tổng quát, không dựa trên phân tích codebase thực tế. Sinh viên phải kiểm tra chéo với application state (routes, configs) để đảm bảo tính đầy đủ.
- AI thường bỏ qua tác động của schema change lên test fixtures hiện có. Cần chủ động kiểm tra và cập nhật toàn bộ test data sau mỗi entity modification.

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

---

## 14. Kế hoạch cải thiện lần sau

- Cung cấp cho AI output thực tế của ứng dụng (danh sách routes, schema hiện tại) trong prompt để AI sinh cấu hình đầy đủ hơn.
- Yêu cầu AI kiểm tra và cập nhật toàn bộ test fixtures khi thêm trường mới vào entity trong prompt ban đầu.
- Thiết lập quy trình chạy toàn bộ test suite ngay sau khi AI hoàn thành code generation, trước khi commit.

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
| --- | --- | --- |
| Ghi nhận việc dùng AI trung thực | 5 | |
| Prompt có mục tiêu rõ ràng | 5 | |
| Kiểm chứng kết quả AI | 5 | |
| Tự chỉnh sửa/cải tiến | 5 | |
| Hiểu nội dung đã nộp | 5 | |
| Reflection có chiều sâu | 5 | |
| Sử dụng AI có trách nhiệm | 5 | |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm hoàn toàn giải thích được cách hoạt động của UsernameService (validation regex, CITEXT normalization, sequential suffix generation), optimistic retry pattern (bắt PostgresException 23505 trên username constraint), cooldown enforcement (so sánh LastUsernameChangeAt với TimeProvider), và safe migration DDL (IF NOT EXISTS + ALTER TABLE ADD COLUMN).
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Thiết kế UsernameService với validation/normalization/generation là kiến thức cơ bản. Optimistic retry pattern là kỹ thuật tiêu chuẩn khi xử lý unique constraint violations trên PostgreSQL. Migration DDL và backfill legacy data là kỹ năng DBA cơ bản. AI chỉ giúp tăng tốc implementation.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-02    |
