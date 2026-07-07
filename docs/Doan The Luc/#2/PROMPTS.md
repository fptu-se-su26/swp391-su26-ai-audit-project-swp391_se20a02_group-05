# Prompt Log

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
| Ngày cập nhật gần nhất | 2026-05-15 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 2026-05-16 | ChatGPT | Thiết kế luồng xoay vòng Refresh Token bảo mật và cơ chế Permission-based Authorization. | Thiết kế hệ thống Authenticati... | Đề xuất luồng xử lý: Client gử... | Có |   |
| 2 | 2026-05-16 | Gemini | Viết Identity Repository sử dụng Dapper để tối ưu hiệu năng. | Write full code: Tạo một Ident... | Cung cấp class Repository với ... | Có |   |
| 3 | 2026-05-16 | Claude | Tái cấu trúc file Program.cs để quản lý Dependency Injection sạch sẽ hơn. | Hãy giúp tôi refactor file Pro... | Chia nhỏ code thành các class ... | Có |   |
| 4 | 2026-05-16 | Gemini | Tìm lỗ hổng trong logic lockout tài khoản và failed-attempt handling. | Phân tích đoạn code AccountSer... | Cảnh báo về việc chưa xử lý Ra... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-16 |
| Công cụ AI | ChatGPT |
| Mục đích | Thiết kế luồng xoay vòng Refresh Token bảo mật và cơ chế Permission-based Authorization. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Thiết kế hệ thống Authentication sử dụng JWT cho ASP.NET Core Web API hỗ trợ Refresh Token Rotation. Làm thế nào để lưu Refresh Token an toàn trong HttpOnly Cookie thay vì LocalStorage? Hãy đề xuất cấu trúc phân quyền dựa trên Permission thay vì chỉ dùng Role.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Dự án sử dụng .NET 10, PostgreSQL, và yêu cầu tính bảo mật cao, chống tấn công XSS/CSRF.
```

#### 5.3. Kết quả AI trả về

```text
Đề xuất luồng xử lý: Client gửi credentials -> Server tạo cặp Token -> Lưu Refresh vào Cookie -> Client dùng Access Token trong header. Gợi ý sử dụng RequirementHandler để xử lý phân quyền động.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Logic refresh token và sơ đồ phân quyền.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Tích hợp thêm Redis để lưu trữ trạng thái token giúp thu hồi (revoke) token nhanh chóng khi cần.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-16 |
| Công cụ AI | Gemini |
| Mục đích | Viết Identity Repository sử dụng Dapper để tối ưu hiệu năng. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Write full code: Tạo một IdentityRepository sử dụng Dapper trong C# để lấy thông tin User cùng với danh sách Roles và Permissions tương ứng chỉ trong một truy vấn SQL (single query) trên PostgreSQL.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp schema database gồm các bảng: Users, Roles, Permissions, UserRoles, RolePermissions.
```

#### 5.3. Kết quả AI trả về

```text
Cung cấp class Repository với SQL Query sử dụng LEFT JOIN và kỹ thuật mapping đối tượng phức tạp (Multi-mapping) của Dapper.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Câu lệnh SQL và logic mapping User - Role - Permission.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Thêm các xử lý về LockoutEnd và AccessFailedCount để phục vụ tính năng bảo mật tài khoản.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [ ] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

### Prompt số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-16 |
| Công cụ AI | Claude |
| Mục đích | Tái cấu trúc file Program.cs để quản lý Dependency Injection sạch sẽ hơn. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi debug |

#### 5.1. Prompt nguyên văn

```text
Hãy giúp tôi refactor file Program.cs này bằng cách tách các dịch vụ xác thực, cấu hình database và swagger ra các Extension Methods riêng biệt trong thư mục API/Extensions.
```

#### 5.2. Bối cảnh khi viết prompt

```text
File Program.cs hiện tại đang quá dài (hơn 200 dòng) với nhiều cấu hình đan xen.
```

#### 5.3. Kết quả AI trả về

```text
Chia nhỏ code thành các class như DependencyInjection.cs, IdentityServiceExtensions.cs, SwaggerExtensions.cs.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Ý tưởng tổ chức thư mục và cách đặt tên các Extension methods.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Thêm EnvValidator vào giai đoạn đầu của pipeline để chặn app startup nếu thiếu biến môi trường.
```

#### 5.6. Đánh giá chất lượng prompt

- [ ] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

### Prompt số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-16 |
| Công cụ AI | Gemini |
| Mục đích | Tìm lỗ hổng trong logic lockout tài khoản và failed-attempt handling. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Phân tích đoạn code AccountService sau để tìm các lỗ hổng bảo mật tiềm ẩn liên quan đến Brute Force và Race Condition khi xử lý đếm số lần đăng nhập sai.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp mã nguồn của AccountService.cs xử lý logic AccessFailedCount.
```

#### 5.3. Kết quả AI trả về

```text
Cảnh báo về việc chưa xử lý Race Condition khi cập nhật số lần sai đồng thời và gợi ý thêm cơ chế "Back-off" thời gian giữa các lần thử.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Logic xử lý khóa tài khoản tạm thời (Lockout).
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Áp dụng Distributed Lock với Redis để đảm bảo tính chính xác trong môi trường chạy nhiều instance (multi-node).
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [ ] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [x] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
Thiết kế hệ thống Authentication sử dụng JWT cho ASP.NET Core Web API hỗ trợ Refresh Token Rotation. Làm thế nào để lưu Refresh Token an toàn trong HttpOnly Cookie thay vì LocalStorage? Hãy đề xuất cấu trúc phân quyền dựa trên Permission thay vì chỉ dùng Role.
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt đóng vai trò quyết định vì nó định hình toàn bộ kiến trúc bảo mật của hệ thống ngay từ đầu. Và giúp giải quyết vấn đề kỹ thuật JWT với cookie ngoài đó tư duy về tính mở rộng thông qua phân quyền dựa trên Permission thay vì Role truyền thống. workflow được cải thiện đáng kể khi tôi không phải loay hoay thử sai các phương pháp bảo mật khác nhau, tạo tiền đề để triển khai các Service khác một cách nhất quán và sạch sẽ.
```

### 6.3. Kết quả prompt này mang lại

```text
Đề xuất luồng xử lý: Client gửi credentials -> Server tạo cặp Token -> Lưu Refresh vào Cookie -> Client dùng Access Token trong header. Gợi ý sử dụng RequirementHandler để xử lý phân quyền động.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Logic refresh token và sơ đồ phân quyền.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Tích hợp thêm Redis để lưu trữ trạng thái token giúp thu hồi (revoke) token nhanh chóng khi cần.
```

---

## 7. Prompt chưa hiệu quả

```text
Chưa có prompt chưa hiệu quả được ghi nhận.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
- Tech Stack cụ thể: Ngôn ngữ (C#) ; thư viện/công cụ (Dapper, Redis, PostgreSQL, .NET 8).
- Schema Database: Các bảng liên quan và mối quan hệ giữa chúng để AI viết query SQL chính xác.
- Ràng buộc bảo mật: Nêu rõ yêu cầu về HttpOnly Cookie, Token Rotation.
- Source code hiện tại: file Program.cs hoặc cấu trúc thư mục.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
- Context is King: Cùng một yêu cầu nhưng khi bổ sung thêm ngữ cảnh về refresh token rotation và phân quyền theo Permission, kết quả AI tạo ra trở nên thực tế và chuyên nghiệp hơn nhiều.
- Iterative Prompting: Thay vì yêu cầu AI xử lý toàn bộ logic phức tạp trong một lần, việc chia nhỏ theo từng bước như thiết kế, triển khai repository rồi refactor giúp kết quả chính xác và dễ kiểm soát hơn.
- Role Play: Khi định hướng AI đóng vai trò như “Security Expert” hoặc “Senior Architect”, phản hồi thường có chiều sâu hơn và chú ý đến các edge case quan trọng như race condition hay bảo mật hệ thống.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
- Few-Shot Prompting: Cung cấp trước một vài đoạn code mẫu theo đúng coding convention của dự án giúp AI nhanh chóng bắt đúng phong cách code và cấu trúc mong muốn.
- Kiểm chứng đa nguồn: Kết hợp nhiều công cụ AI khác nhau cho từng nhiệm vụ cụ thể, ví dụ dùng Claude để refactor kiến trúc và Antigravity để rà soát bảo mật, giúp kết quả toàn diện và đáng tin cậy hơn.
- Cung cấp Error Logs: Khi phát sinh lỗi, việc đưa trực tiếp log cho AI phân tích giúp xác định nguyên nhân gốc rễ chính xác hơn, đồng thời hỗ trợ cải thiện kỹ năng debugging và xử lý sự cố.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Coding | 4 |  |

---

## 10. Checklist chất lượng prompt

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | x | |
| Prompt có đủ bối cảnh | x | |
| Tự kiểm tra và chỉnh sửa | x | |

---

## 11. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 16/5/2026 |
