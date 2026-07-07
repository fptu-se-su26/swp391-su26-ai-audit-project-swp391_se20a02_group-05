# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu          | 2026-06-01T09:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-01T15:05:00.000Z                                                               |

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
Triển khai toàn diện vòng đời xóa tài khoản (Account Deletion Lifecycle) bao gồm thời gian chờ 14 ngày (grace period), cơ chế xác thực đa dạng theo profile người dùng, khôi phục tài khoản (reactivation), gửi cảnh báo bảo mật khi dùng OTP fallback, và tự động hóa tác vụ dọn dẹp chạy background (Tokens/Assets/Logs) kết hợp anonymization log (bằng SHA-256 + salt). Đồng thời, thực hiện tái cấu trúc toàn bộ mã nguồn backend từ kiến trúc legacy sang Modular Monolith (Modules/Shared và các modules Auth, Profiles, Recovery, Admin, AiChat) và bổ sung bộ kiểm thử kiến trúc (ModularBoundaryTests) để tự động hóa việc phát hiện vi phạm ranh giới phụ thuộc.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-01                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Thiết kế vòng đời xóa tài khoản (Account Deletion Lifecycle) và cơ chế tự động hóa purge/anonymize ở background.                                        |
| Phần việc liên quan | Backend / Domain State Machine / Database Migration / Background Workers                                                                                |
| Mức độ sử dụng      | Hỗ trợ định nghĩa trạng thái DELETION_PENDING, thiết kế database index, và sinh mã xử lý R2 storage cleanup/audit log anonymization trong background worker. |

#### 4.1. Prompt đã sử dụng

```text
Design and implement a complete account deletion lifecycle with a 14-day grace period. Add DELETION_PENDING status to UserStatus state machine with bidirectional transitions (ACTIVE <-> DELETION_PENDING -> DELETED) and enforce a legal hold check. For background purging, extend TokenCleanupBackgroundJob to batch-process expired DELETION_PENDING users, purge their assets from S3/R2 storage, anonymize their audit logs using SHA-256 + salt, and hard-delete the user record under transaction control. Create APIs to get deletion requirements, request deletion (password/OTP/OAuth re-auth), and reactivate. Implement UI modals and reactivate page.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý định nghĩa thêm giá trị DELETION_PENDING trong UserStatus enum, bổ sung cột IsLegalHold trên thực thể User và cột AnonymizedActorHash trên AuditLog. Thiết kế các API endpoint kiểm tra cấu hình auth của user để đưa ra luồng xác thực phù hợp (password-based vs OAuth re-auth vs OTP fallback). Trong TokenCleanupBackgroundJob, AI đề xuất phương thức PurgeExpiredSoftDeletedUsersAsync chạy tuần tự: lấy batch 50 user quá hạn 14 ngày, duyệt qua danh sách file đính kèm của họ để xóa trên R2, tính toán actor hash ẩn danh, cập nhật hàng loạt log của họ sang dạng ẩn danh (xóa PII, cập nhật AnonymizedActorHash) và thực thi lệnh Remove User trong một Transaction Scope.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- State transition validation matrix trong User.TransitionTo() cho trạng thái DELETION_PENDING.
- Cấu trúc vòng lặp batching dọn dẹp file đính kèm R2 qua IStorageService.DeleteFileAsync.
- Cấu hình PostgreSQL enum type migration và partial unique index idx_users_email_active trên database.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Thiết kế cơ chế Salted SHA-256 hashing để tạo AnonymizedActorHash nhằm bảo toàn tính ẩn danh tuyệt đối (GDPR-compliant) nhưng vẫn duy trì tính liên kết trace logs hệ thống sau khi user đã bị xóa cứng.
- Bổ sung SecurityAlertNotice gửi email cảnh báo bảo mật đến hòm thư chính khi OTP fallback được yêu cầu gửi tới hòm thư phụ, ngăn chặn kẻ xấu chiếm đoạt quyền xóa tài khoản.
- Tự thiết kế luồng chuyển đổi trạng thái khi Login: chặn đăng nhập với người dùng ở trạng thái DELETION_PENDING và chuyển hướng họ sang trang /auth/reactivate để phục hồi tài khoản nếu muốn.
- Viết 5 integration tests tự động kiểm thử toàn bộ luồng: password deletion, legal hold guard, 14-day reactivation, email uniqueness và hard purge.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement account deletion lifecycle with soft-delete, reactivation, and automated purge | https://github.com/Kaivian/CVerify/commit/137171f |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Quy trình xóa tài khoản kết hợp tự động dọn dẹp background giúp giải phóng dung lượng đĩa hiệu quả và đảm bảo tuân thủ nghiêm ngặt quyền được quên (GDPR). Việc ẩn danh hóa audit log thay vì xóa hoàn toàn log giúp giữ lại vết lịch sử bảo mật phục vụ công tác điều tra số khi cần thiết.
```

---

### Lần sử dụng AI số 2

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-01                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Tái cấu trúc codebase backend sang kiến trúc Modular Monolith để tách biệt các miền nghiệp vụ (Auth, Recovery, Profiles, Admin, AiChat, Shared).        |
| Phần việc liên quan | Backend / System Architecture / Dependency Management                                                                                                   |
| Mức độ sử dụng      | Hỗ trợ tổ chức lại namespace, cập nhật Program.cs và khởi tạo khung kiểm thử NetArchTest.                                                              |

#### 4.1. Prompt đã sử dụng

```text
Refactor the CVerify.Core backend to use a modular monolith architecture. Move existing infrastructure, core domain, logic and views into a structured Modules folder: Shared, Auth, Profiles, Recovery, Admin, AiChat. Relocate database persistence context and general middleware to Modules/Shared. Resolve all namespaces and update Program.cs to use the new modular configuration extensions and builders. Add a NetArchTest architecture test project to validate that feature modules do not depend on each other and Shared does not depend on features.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất chia nhỏ cấu trúc dự án thành thư mục Modules. Di chuyển thực thể cơ bản, persistence (DbContext/DbInitializer), config và logging vào Modules/Shared. Nhóm các nghiệp vụ còn lại vào các thư mục Auth, Profiles, Recovery, Admin và AiChat tương ứng. AI sinh cấu trúc Program.cs mới sử dụng namespaces CVerify.API.Modules.* và viết lớp ModularBoundaryTests sử dụng thư viện NetArchTest để kiểm tra dependency boundaries.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Bộ khung cấu trúc thư mục mới và ánh xạ namespace của các Services/Controllers.
- Boilerplate đăng ký Middleware và Hub mới trong file Program.cs.
- Lớp kiểm thử kiến trúc ModularBoundaryTests ban đầu.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Khắc phục lỗi kiểm thử kiến trúc (Architecture Tests): AI viết test quá lý thuyết dẫn đến biên dịch và chạy test bị lỗi (do ApplicationDbContext bắt buộc phải biết tất cả Entity DbSet từ các Module tính năng, User entity trong Shared có liên kết Many-to-Many hoặc Navigation Property với PasswordCredential thuộc Auth, và TokenCleanupBackgroundJob phải quét ProfileAttachment thuộc Profiles). 
- Sinh viên đã điều chỉnh lại ModularBoundaryTests.cs để bổ sung các ngoại lệ cần thiết: cho phép module Recovery tham chiếu đến Auth, loại bỏ ApplicationDbContext, DbInitializer, TokenCleanupBackgroundJob, và User khỏi quy tắc kiểm tra 'Shared không được phép tham chiếu Features'. 
- Sửa đổi toàn bộ namespace lỗi trong 25 file unit tests và 30 file integration tests để phục hồi khả năng chạy test thành công.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | refactor(arch): transition codebase to modular monolith architecture | https://github.com/Kaivian/CVerify/commit/1eb5cb7 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc chuyển đổi sang Modular Monolith cải thiện đáng kể khả năng bảo trì, giúp phân tách rõ trách nhiệm giữa các mô-đun nghiệp vụ độc lập. Sự hỗ trợ từ bộ công cụ kiểm thử NetArchTest đóng vai trò thiết yếu trong việc giám sát chất lượng thiết kế kiến trúc, ngăn chặn việc tái lập các liên kết chéo (spaghetti dependencies) sau này.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                                          |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------------------ |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích thiết kế luồng xóa tài khoản và ranh giới Modular Monolith.                             |
| Viết user story/use case    |       x       |              |                 |               |                                                                                                  |
| Thiết kế database           |               |      x       |                 |               | Thêm AnonymizedActorHash, IsLegalHold và PostgreSQL enum migration.                              |
| Thiết kế kiến trúc hệ thống |               |              |        x        |               | Thiết kế phân rã modular monolith và quy tắc NetArchTest.                                        |
| Thiết kế giao diện          |               |              |        x        |               | Dựng trang Reactivate UI và modal xác nhận 3 bước xóa tài khoản.                                 |
| Code frontend               |               |              |        x        |               | Redesign AccountTab modal, viết reactivate view và use-auth login interceptor.                    |
| Code backend                |               |              |        x        |               | Triển khai dọn dẹp background, ẩn danh hóa dữ liệu, di chuyển modules và fix namespace.           |
| Debug lỗi                   |               |              |        x        |               | Sửa lỗi phụ thuộc vòng tròn và biên dịch namespace của test projects sau refactor.               |
| Viết test case              |               |              |        x        |               | Thiết lập ModularBoundaryTests và 5 integration tests cho deletion lifecycle.                     |
| Kiểm thử sản phẩm           |       x       |              |                 |               | Chạy thử toàn diện 82 integration tests backend và kiểm thử thủ công luồng UI.                   |
| Tối ưu code                 |       x       |              |                 |               |                                                                                                  |
| Viết báo cáo                |       x       |              |                 |               |                                                                                                  |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                                  |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI sinh mã kiểm thử kiến trúc (Architecture Tests) quá cứng nhắc, dẫn đến test Shared_ShouldNot_DependOnFeatures thất bại vì DbContext, DbInitializer, và User thực tế bắt buộc phải có sự liên kết với các feature entities để hoạt động. | Chạy lệnh `dotnet test` phát hiện ModularBoundaryTests thất bại với danh sách 4 kiểu dữ liệu vi phạm. | Thêm các điều kiện loại trừ (.And().DoNotHaveName(...)) cho các class DB-related và User entity trong quy tắc kiểm thử NetArchTest. |
|   2 | AI bỏ qua liên kết phụ thuộc trực tiếp giữa mô-đun Recovery và Auth trong quá trình phân tách modules, dẫn đến test Features_ShouldNot_DependOnOtherFeatures thất bại. | Chạy test phát hiện Recovery Services phụ thuộc vào Auth.DTOs và Auth.Services để xác minh tài khoản khôi phục. | Điều chỉnh quy tắc kiểm thử để cấp phép ngoại lệ: cho phép Recovery tham chiếu đến Auth do tính chất liên kết nghiệp vụ. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả thông qua:
1. Viết và chạy thành công 5 integration tests mới kiểm tra luồng xóa tài khoản: kích hoạt trạng thái DELETION_PENDING, chặn xóa khi có legal hold, khôi phục tài khoản trong grace period, bảo toàn email duy nhất và dọn dẹp cứng (hard-delete) sau 14 ngày.
2. Kiểm thử tự động với NetArchTest: hai kiểm thử ModularBoundaryTests chạy thành công 100% sau khi bổ sung các ngoại lệ phụ thuộc hợp lệ.
3. Chạy thành công toàn bộ suite gồm 82 integration tests của CVerify.API.IntegrationTests và 51 unit tests của CVerify.API.UnitTests mà không phát sinh bất kỳ lỗi biên dịch hay runtime nào.
4. Chạy thực tế dự án (dotnet run) và kiểm tra bootstrap thành công với service validation bật (ValidateOnBuild = true).
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Trực tiếp cấu hình thuật toán băm SHA-256 kết hợp server-side salt để mã hóa ẩn danh hóa người dùng trong audit logs.
- Giải quyết triệt để các xung đột phụ thuộc trong bộ kiểm thử kiến trúc ModularBoundaryTests.
- Khắc phục thủ công lỗi biên dịch namespace trên tổng số hơn 50 file kiểm thử (integration/unit tests).
- Triển khai cảnh báo email bảo mật SecurityAlertNotice khi có yêu cầu OTP fallback gửi tới secondary email.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                             | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai Account Deletion Lifecycle, tái cấu trúc Modular Monolith, sửa lỗi biên dịch tests, điều chỉnh NetArchTest. | Có                   | https://github.com/Kaivian/CVerify/commit/137171f, https://github.com/Kaivian/CVerify/commit/1eb5cb7 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm thử tích hợp UI luồng khôi phục tài khoản đăng nhập (reactivate page).                 | Không                |                     |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp tự động hóa việc di chuyển hàng chục ngàn dòng code cũ sang cấu trúc thư mục Modules và đổi tên namespaces nhanh chóng. Nó cũng sinh khung sườn mã nguồn dọn dẹp file lưu trữ trên R2 và mẫu mã hóa log bảo mật.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng quy tắc phân tách tuyệt đối của AI trong NetArchTest đối với DbContext và User. Việc thiết kế database monolith dùng chung bắt buộc DbContext phải phụ thuộc vào thực thể của các mô-đun nghiệp vụ khác. Việc tuân thủ máy móc theo gợi ý của AI sẽ làm hệ thống không thể build được hoặc buộc phải chia tách cơ sở dữ liệu vật lý (điều chưa cần thiết ở giai đoạn hiện tại).
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Nhóm kiểm chứng bằng cách kích hoạt cơ chế tự động validate service scope tại startup (ValidateOnBuild = true) và thực thi toàn bộ hệ thống kiểm thử unit, integration, và architecture.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần khó khăn nhất là thực hiện đổi tên namespaces và chỉnh sửa đường dẫn thư mục cho hàng trăm file trong dự án một cách thủ công, điều này rất dễ gây ra sai sót cú pháp hoặc thiếu file tham chiếu.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Học được cách tổ chức dự án theo mô hình Modular Monolith, cách cân bằng giữa tính cô lập của mô-đun nghiệp vụ và sự tiện lợi của cơ sở dữ liệu dùng chung (ApplicationDbContext), và tầm quan trọng của việc kiểm thử kiến trúc (Architecture Tests).
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI thường đề xuất các thiết kế kiến trúc lý tưởng lý thuyết (ví dụ: cấm Shared phụ thuộc Feature) nhưng không chạy thử được trong thực tế. Nhà phát triển phải luôn kiểm thử tính thực tiễn và tùy biến các quy tắc AI sinh ra cho phù hợp với đặc thù dự án.
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
| Nguyễn Hoàng Ngọc Ánh   | 2026-06-01    |
