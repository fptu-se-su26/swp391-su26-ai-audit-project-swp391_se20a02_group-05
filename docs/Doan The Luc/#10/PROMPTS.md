# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Account Deletion Lifecycle & Modular Monolith Transition                      |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-06-01T09:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-06-01                                                                             |

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
|   1 | 2026-06-01 | Antigravity | Triển khai Account Deletion Lifecycle | Design and implement a complete account deletion lifecycle with a 14-day grace period... | Thiết kế các APIs xóa tài khoản, thêm DELETION_PENDING state và tích hợp background purge/anonymize logs. | Có | GitHub Commit |
|   2 | 2026-06-01 | Antigravity | Tái cấu trúc codebase sang Modular Monolith | Refactor the CVerify.Core backend to use a modular monolith architecture... | Di chuyển files sang thư mục Modules, cập nhật namespaces và Program.cs, thiết lập NetArchTest. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-01                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Triển khai Account Deletion Lifecycle (vòng đời xóa tài khoản 14 ngày).                                                                                |
| Phần việc liên quan | Backend / Domain State Machine / Background Workers / Database Migration                                                                               |
| Mức độ sử dụng      | Hỗ trợ định nghĩa trạng thái DELETION_PENDING và xây dựng logic xử lý R2 storage cleanup/audit log anonymization trong background worker.              |

#### 5.1. Prompt nguyên văn

```text
Design and implement a complete account deletion lifecycle with a 14-day grace period. Add DELETION_PENDING status to UserStatus state machine with bidirectional transitions (ACTIVE <-> DELETION_PENDING -> DELETED) and enforce a legal hold check. For background purging, extend TokenCleanupBackgroundJob to batch-process expired DELETION_PENDING users, purge their assets from S3/R2 storage, anonymize their audit logs using SHA-256 + salt, and hard-delete the user record under transaction control. Create APIs to get deletion requirements, request deletion (password/OTP/OAuth re-auth), and reactivate. Implement UI modals and reactivate page.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- CVerify chưa có tính năng xóa tài khoản thực tế, chỉ có soft-delete đơn giản không dọn dẹp dữ liệu lưu trữ vật lý trên R2.
- Để tuân thủ chính sách bảo mật GDPR, hệ thống cần hỗ trợ grace period 14 ngày trước khi thực hiện xóa cứng tài khoản, đồng thời ẩn danh hóa lịch sử log bảo mật thay vì xóa sạch vết để đảm bảo tính forensic.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất thêm trạng thái DELETION_PENDING vào enum UserStatus, cột IsLegalHold trên thực thể User để chặn xóa tài khoản vi phạm pháp lý. Thêm cột AnonymizedActorHash trên AuditLog. Xây dựng tác vụ PurgeExpiredSoftDeletedUsersAsync chạy định kỳ hàng giờ để batch-process xóa user quá hạn, dọn dẹp R2 attachments và cập nhật các audit logs sang trạng thái ẩn danh trong database transaction.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Viết logic chuyển đổi trạng thái trong User.cs với kiểm tra cờ IsLegalHold.
- Thêm cột AnonymizedActorHash vào bảng audit_logs trong DbInitializer.cs.
- Tích hợp PurgeExpiredSoftDeletedUsersAsync vào background worker TokenCleanupBackgroundJob.cs.
- Tạo các endpoint APIs mới cho luồng xóa tài khoản trong UserController.cs.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Triển khai SecurityAlertNotice gửi email cảnh báo bảo mật khi yêu cầu OTP xóa tài khoản fallback được kích hoạt, nâng cao tính bảo mật tránh chiếm đoạt tài khoản.
- Sử dụng ServerSalt kết hợp User.Id để tạo hash ẩn danh actor (AnonymizedActorHash) một cách deterministic.
- Viết 5 kịch bản integration tests tự động bao phủ luồng xóa/phục hồi tài khoản.
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
| Commit          | https://github.com/Kaivian/CVerify/commit/137171f |

---

### Prompt số 2

| Nội dung            | Thông tin                                                                                                                                                 |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-06-01                                                                                                                                                |
| Công cụ AI          | Antigravity                                                                                                                                               |
| Mục đích            | Tái cấu trúc codebase sang Modular Monolith.                                                                                                              |
| Phần việc liên quan | Backend / System Architecture / Dependency Management                                                                                                     |
| Mức độ sử dụng      | Hỗ trợ phân tách thư mục Modules, cập nhật Program.cs và khởi tạo khung kiểm thử NetArchTest.                                                             |

#### 5.1. Prompt nguyên văn

```text
Refactor the CVerify.Core backend to use a modular monolith architecture. Move existing infrastructure, core domain, logic and views into a structured Modules folder: Shared, Auth, Profiles, Recovery, Admin, AiChat. Relocate database persistence context and general middleware to Modules/Shared. Resolve all namespaces and update Program.cs to use the new modular configuration extensions and builders. Add a NetArchTest architecture test project to validate that feature modules do not depend on each other and Shared does not depend on features.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Dự án CVerify ban đầu sử dụng cấu trúc thư mục truyền thống (Core, Infrastructure, API, Application) dẫn đến sự phụ thuộc chéo khó kiểm soát và khó mở rộng khi số lượng nhà phát triển tăng lên.
- Cần tổ chức lại codebase theo cấu trúc Modular Monolith để tách biệt các miền nghiệp vụ thành các mô-đun độc lập (Auth, Profiles, Recovery, v.v.) và Shared mô-đun.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất chia nhỏ cấu trúc dự án thành Modules: Shared, Auth, Profiles, Recovery, Admin, AiChat. Di chuyển DbContext và DB Initializer vào Shared. AI sinh file Program.cs mới sử dụng namespaces CVerify.API.Modules.* và viết ModularBoundaryTests sử dụng thư viện NetArchTest để kiểm tra dependency boundaries.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Di chuyển toàn bộ cấu trúc thư mục nghiệp vụ sang thư mục Modules.
- Thay đổi imports và namespaces trong Program.cs để đăng ký modular services.
- Đổi tên tệp project kiểm thử unit tests để bổ sung gói NetArchTest.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Sửa lỗi nghiêm trọng của NetArchTest: AI cấm Shared phụ thuộc Features tuyệt đối, điều này làm hỏng ApplicationDbContext, DbInitializer, và TokenCleanupBackgroundJob vì chúng cần biết các entities và files để khởi tạo DB và dọn dẹp background. Sinh viên đã thêm điều kiện loại trừ các classes này trong ModularBoundaryTests.cs.
- Sửa đổi thủ công namespace lỗi trên hơn 50 tệp unit và integration tests của dự án để đảm bảo toàn bộ hệ thống kiểm thử tự động xanh trở lại.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [ ] Prompt tạo ra kết quả tốt
- [x] Prompt tạo ra kết quả chưa phù hợp (Gặp lỗi kiểm thử kiến trúc khi chạy thực tế)
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
| --------------- | -------- |
| Commit          | https://github.com/Kaivian/CVerify/commit/1eb5cb7 |

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Khi tái cấu trúc kiến trúc lớn (như Modular Monolith), cần cung cấp cho AI thông tin chi tiết về các class dùng chung bắt buộc phải liên kết với mô-đun nghiệp vụ (như DbContext). Nếu không, AI sẽ áp dụng các quy chuẩn lý thuyết quá cứng nhắc dẫn đến việc sinh các bài test kiến trúc không thể pass trong thực tế.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Nên yêu cầu AI mô tả chi tiết các ngoại lệ nghiệp vụ (exceptions/exclusions) khi thực hiện viết code kiểm thử chất lượng kiến trúc hệ thống bằng các thư viện kiểm tra tĩnh như NetArchTest.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Coding |        1 | Refactor the CVerify.Core backend to use a modular monolith architecture... |
| Prompt Design |        1 | Design and implement a complete account deletion lifecycle with a 14-day grace period... |

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
| Nguyễn Hoàng Ngọc Ánh   | 2026-06-01    |
