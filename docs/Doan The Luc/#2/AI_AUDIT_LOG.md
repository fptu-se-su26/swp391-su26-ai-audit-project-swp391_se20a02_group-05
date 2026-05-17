# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie - Backend |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [x] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
System Architecture Design; Infrastructure Setup; Code Generation & Refactoring; Security Review & Debugging; Documentation Automation
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Thiết kế luồng xoay vòng Refresh Token bảo mật và cơ chế Permission-based Authorization. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Thiết kế hệ thống Authentication sử dụng JWT cho ASP.NET Core Web API hỗ trợ Refresh Token Rotation. Làm thế nào để lưu Refresh Token an toàn trong HttpOnly Cookie thay vì LocalStorage? Hãy đề xuất cấu trúc phân quyền dựa trên Permission thay vì chỉ dùng Role.
```

#### 4.2. Kết quả AI gợi ý

```text
Đề xuất luồng xử lý: Client gửi credentials -> Server tạo cặp Token -> Lưu Refresh vào Cookie -> Client dùng Access Token trong header. Gợi ý sử dụng RequirementHandler để xử lý phân quyền động.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Logic refresh token và sơ đồ phân quyền.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tích hợp thêm Redis để lưu trữ trạng thái token giúp thu hồi (revoke) token nhanh chóng khi cần.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 03:27:19 | image.png |
| Screenshot | Screenshot 03:28:38 | image.png |
| Screenshot | Screenshot 03:28:54 | image.png |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Đợt refactor này không chỉ giúp nâng cấp hệ thống bảo mật theo hướng chuyên nghiệp hơn với JWT, Redis và Permission-based Authorization, mà còn giúp cả nhóm hiểu rõ hơn về cách xây dựng một backend thực tế theo chuẩn enterprise. Một thành viên tập trung triển khai chính, trong khi thành viên còn lại đảm nhận việc review, kiểm thử và học hỏi quy trình thiết kế hệ thống, từ đó cải thiện khả năng teamwork và tư duy kiến trúc phần mềm.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Viết Identity Repository sử dụng Dapper để tối ưu hiệu năng. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Write full code: Tạo một IdentityRepository sử dụng Dapper trong C# để lấy thông tin User cùng với danh sách Roles và Permissions tương ứng chỉ trong một truy vấn SQL (single query) trên PostgreSQL.
```

#### 4.2. Kết quả AI gợi ý

```text
Cung cấp class Repository với SQL Query sử dụng LEFT JOIN và kỹ thuật mapping đối tượng phức tạp (Multi-mapping) của Dapper.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Câu lệnh SQL và logic mapping User - Role - Permission.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Thêm các xử lý về LockoutEnd và AccessFailedCount để phục vụ tính năng bảo mật tài khoản.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Đợt refactor này không chỉ giúp nâng cấp hệ thống bảo mật theo hướng chuyên nghiệp hơn với JWT, Redis và Permission-based Authorization, mà còn giúp cả nhóm hiểu rõ hơn về cách xây dựng một backend thực tế theo chuẩn enterprise. Một thành viên tập trung triển khai chính, trong khi thành viên còn lại đảm nhận việc review, kiểm thử và học hỏi quy trình thiết kế hệ thống, từ đó cải thiện khả năng teamwork và tư duy kiến trúc phần mềm.
```

---

### Lần sử dụng AI số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Tái cấu trúc file Program.cs để quản lý Dependency Injection sạch sẽ hơn. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Hãy giúp tôi refactor file Program.cs này bằng cách tách các dịch vụ xác thực, cấu hình database và swagger ra các Extension Methods riêng biệt trong thư mục API/Extensions.
```

#### 4.2. Kết quả AI gợi ý

```text
Chia nhỏ code thành các class như DependencyInjection.cs, IdentityServiceExtensions.cs, SwaggerExtensions.cs.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Ý tưởng tổ chức thư mục và cách đặt tên các Extension methods.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Thêm EnvValidator vào giai đoạn đầu của pipeline để chặn app startup nếu thiếu biến môi trường.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Đợt refactor này không chỉ giúp nâng cấp hệ thống bảo mật theo hướng chuyên nghiệp hơn với JWT, Redis và Permission-based Authorization, mà còn giúp cả nhóm hiểu rõ hơn về cách xây dựng một backend thực tế theo chuẩn enterprise. Một thành viên tập trung triển khai chính, trong khi thành viên còn lại đảm nhận việc review, kiểm thử và học hỏi quy trình thiết kế hệ thống, từ đó cải thiện khả năng teamwork và tư duy kiến trúc phần mềm.
```

---

### Lần sử dụng AI số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Tìm lỗ hổng trong logic lockout tài khoản và failed-attempt handling. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Phân tích đoạn code AccountService sau để tìm các lỗ hổng bảo mật tiềm ẩn liên quan đến Brute Force và Race Condition khi xử lý đếm số lần đăng nhập sai.
```

#### 4.2. Kết quả AI gợi ý

```text
Cảnh báo về việc chưa xử lý Race Condition khi cập nhật số lần sai đồng thời và gợi ý thêm cơ chế "Back-off" thời gian giữa các lần thử.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Logic xử lý khóa tài khoản tạm thời (Lockout).
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Áp dụng Distributed Lock với Redis để đảm bảo tính chính xác trong môi trường chạy nhiều instance (multi-node).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Đợt refactor này không chỉ giúp nâng cấp hệ thống bảo mật theo hướng chuyên nghiệp hơn với JWT, Redis và Permission-based Authorization, mà còn giúp cả nhóm hiểu rõ hơn về cách xây dựng một backend thực tế theo chuẩn enterprise. Một thành viên tập trung triển khai chính, trong khi thành viên còn lại đảm nhận việc review, kiểm thử và học hỏi quy trình thiết kế hệ thống, từ đó cải thiện khả năng teamwork và tư duy kiến trúc phần mềm.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Backend (Auth Logic) |   | x |   |   | Sử dụng AI để thiết kế luồng JWT Rotation nhưng tự viết logic xử lý HttpOnly Cookie để đảm bảo an toàn bảo mật tuyệt đối. |
| Database (Dapper/SQL) |   |   | x |   | AI hỗ trợ viết các câu lệnh SQL JOIN phức tạp và Mapping đa bảng; tự tối ưu hóa các Index trong PostgreSQL. |
| Infrastructure (Redis/Env) |   | x |   |   | Dùng AI tham khảo cấu hình Redis Cache; tự triển khai EnvValidator để kiểm soát chặt chẽ các biến môi trường của dự án. |
| Refactoring (Structure) |   |   |   | x | Sử dụng AI để gợi ý cách tách Extension Methods giúp làm sạch file Program.cs; tự sắp xếp lại cấu trúc thư mục API/Extensions theo ý đồ cá nhân. |
| Documentation (Logs/PR) |   | x |   |   | AI giúp tổng hợp các thay đổi (Changelog) và viết mô tả Pull Request chuyên nghiệp dựa trên các đoạn code đã thực hiện. |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | Hallucination về thư viện Dapper: AI gợi ý một số hàm extension của Dapper không tồn tại trong phiên bản .NET 10 đang dùng. | Khi biên dịch code (Compile-time error) thấy báo lỗi thiếu method. | Tra cứu tài liệu chính thống của Dapper và tự viết lại hàm Mapping đa bảng (Multi-mapping) cho đúng cú pháp. |
| 2 | Bỏ qua Race Condition: AI cung cấp code cập nhật AccessFailedCount (đăng nhập sai) mà không có cơ chế khóa (locking), dẫn đến dữ liệu sai khi có nhiều yêu cầu cùng lúc. | Thông qua việc tự review code và phân tích logic luồng xử lý đồng thời (Concurrency). | Triển khai thêm Distributed Lock bằng Redis để đảm bảo việc cập nhật số lần đăng nhập sai luôn chính xác trong môi trường multi-instance. |
| 3 | Cấu hình Security không tối ưu: AI mặc định gợi ý lưu JWT Secret trực tiếp trong code hoặc file cấu hình không qua kiểm tra. | Khi chạy thử nghiệm, hệ thống không khởi động được do thiếu biến môi trường cần thiết. | Xây dựng thêm class EnvValidator để kiểm tra và bắt buộc tất cả các thông tin nhạy cảm phải được lấy từ biến môi trường (.env) trước khi app chạy. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
- Manual Code Review: Rà soát lại toàn bộ code do AI sinh ra để đảm bảo đúng coding convention, loại bỏ logic dư thừa và hạn chế các rủi ro bảo mật tiềm ẩn.
- Unit Testing: Xây dựng test cho các service quan trọng như AuthService và TokenService nhằm kiểm tra độ chính xác của JWT và luồng xác thực.
- Integration Testing: Sử dụng Postman để kiểm tra thực tế các API endpoint, bao gồm refresh token rotation và cơ chế lưu HttpOnly Cookie.
- Security Audit: Thực hiện kiểm tra bảo mật bằng các công cụ phân tích tĩnh để phát hiện sớm các vấn đề như SQL Injection hoặc race condition trong hệ thống.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Phần việc cá nhân: Tôi trực tiếp thiết kế cấu trúc API/Extensions, xây dựng hệ thống Permission, cấu hình Redis và PostgreSQL, đồng thời xử lý các logic phức tạp như đồng bộ dữ liệu giữa các service và quản lý lỗi tập trung.
- Sự hỗ trợ từ AI: AI được sử dụng như một trợ lý để sinh boilerplate code, gợi ý các truy vấn SQL phức tạp và tham khảo best practices về bảo mật, giúp tăng tốc quá trình phát triển và tập trung hơn vào tối ưu kiến trúc hệ thống.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Đoàn Thế Lực | DE200523 | Đảm nhiệm toàn bộ quá trình Refactor backend và infrastructure. Thiết kế và triển khai JWT Auth, Refresh Token Rotation, Permission-based Authorization, Redis Cache, Dapper Repository, PostgreSQL mappings và cấu trúc API Extensions. | Có | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/f9f2a905c1bdc82519fa5313d84a11d685954d03 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Phụ trách kiểm thử (Review code), đảm bảo chất lượng mã nguồn tuân thủ coding convention, thực hiện quy trình Merge code và kiểm tra tính nhất quán của hệ thống sau khi refactor. | Không |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 16/5/2026 |
