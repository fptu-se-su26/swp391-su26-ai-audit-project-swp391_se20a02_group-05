# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-06-15T08:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-06-16                                                                             |

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
|   1 | 2026-06-15 | Antigravity | Tích hợp liên kết tài khoản nhà cung cấp mã nguồn ngoài | Add support for external source code providers... | Tạo Migration, các lớp entity/DTO cho tổ chức ngoài, thiết lập source-code clients ở backend và trang cài đặt kết nối ở frontend. | Có | GitHub Commit |
|   2 | 2026-06-15 | Antigravity | Chuẩn hóa bộ chọn ngày sinh HeroUI DatePicker và cấu hình giao diện CV mới | Replace the native Date of Birth input field in the CV Management page with the HeroUI DatePicker... | Căn lề Completeness card lên đầu full-width, tích hợp HeroUI DatePicker có kiểm lỗi ngày tháng, và thêm hoạt ảnh hover vi mô cho CV cards. | Có | GitHub Commit |
|   3 | 2026-06-16 | Antigravity | Khắc phục lỗi bảo mật tự động re-login khi reload trang sau khi bị AFK kick | Recheck the auto kick system when AFK. If a user is already kicked due to inactivity, they are currently automatically logged back in... | Thay đổi endpoint logout từ [Authorize] thành [AllowAnonymous] để dọn dẹp cookies thành công khi Access Token đã hết hạn. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1 (Source-Code Providers & External Organizations Integration)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-15                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Tích hợp phân hệ liên kết tài khoản và tổ chức mã nguồn bên ngoài (GitHub/GitLab).                                                                     |
| Phần việc liên quan | Backend .NET Core (Clients, Services, Controllers, Migrations) & Next.js Frontend (Source Code Providers settings page, service)                       |
| Mức độ sử dụng      | Hỗ trợ sinh mã cấu trúc thực thể, DTOs, khung lớp client GitHub/GitLab, cấu trúc trang cài đặt ở frontend.                                             |

#### 5.1. Prompt nguyên văn

```text
Add support for external source code providers: introduce AddExternalOrganizations EF migration and ExternalOrganization entity/DTO, update ApplicationDbContext and DbInitializer, and apply snapshot changes. Add new source-code clients (GitHubSourceCodeClient, GitLabSourceCodeClient) and ISourceCodeClient interface, and wire them into the SourceCodeProviderService, controller, repository DTOs and entities. Add frontend pages, service and types for source-code providers.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Hệ thống CVerify cần mở rộng tính năng liên kết tài khoản lập trình ngoài của ứng viên để thực hiện phân tích chất lượng/bảo mật.
- Cần tạo các bảng DB lưu trữ dữ liệu liên kết tổ chức ngoài (`ExternalOrganization`) và tích hợp an toàn qua API REST.
```

#### 5.3. Kết quả AI trả về

```text
AI cung cấp chi tiết mã nguồn thiết lập database migration, cài đặt các class client thực thi gọi API của GitHub và GitLab (xác thực qua token và đọc danh sách organizations), và cung cấp trang cài đặt quản trị liên kết ở client.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo thành công tệp migration, áp dụng và đồng bộ DB.
- Tích hợp thành công trang Settings quản lý liên kết kho lưu trữ ngoài.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Bổ sung mã hóa AES-GCM cho các khóa bảo mật OAuth tokens của GitHub/GitLab trước khi ghi vào cơ sở dữ liệu để đảm bảo an toàn thông tin của ứng viên.
- Thêm cơ chế xử lý phân trang tự động khi nạp danh sách repositories lớn.
```

---

### Prompt số 2 (DatePicker Standardization & CV Layout Adjustments)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-15                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Chuẩn hóa date input sang HeroUI DatePicker, định vị Completeness card lên đầu trang và tạo hoạt ảnh hover vi mô cho các danh mục CV.                   |
| Phần việc liên quan | Next.js Frontend (Page Layout, Component Stylings, DatePicker Integration)                                                                             |
| Mức độ sử dụng      | Hỗ trợ tái cấu trúc khối renderOverview, hướng dẫn cách sử dụng API của HeroUI DatePicker để xử lý kiểu dữ liệu ngày tháng.                            |

#### 5.1. Prompt nguyên văn

```text
Replace the Date of Birth input field in the CV Management page with the HeroUI DatePicker component, ensuring proper date parsing, validation, and layout styling according to HeroUI rules. Restructure the Overview page to place the Completeness scorecard at the top spanning full-width, and add subtle transition animations and translate offset to CV section cards on hover.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Các ô chọn ngày sinh của ứng viên đang dùng HTML native date input thô sơ và không đồng bộ định dạng với múi giờ ISO-8601.
- Giao diện CV Overview cần đưa Completeness Card lên đầu full-width để nhấn mạnh trạng thái hoàn thiện hồ sơ của ứng viên.
- Cần bổ sung hiệu ứng hover mượt mà cho các CV section cards.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất sử dụng thư viện `@heroui/react` để import `DatePicker`, cung cấp giải pháp convert dữ liệu ngày tháng và cách định cấu hình tailwind/CSS để tạo hoạt ảnh hover nâng nhẹ thẻ card lên (`hover:-translate-y-0.5`).
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Sử dụng DatePicker mới cho phần BasicInfoForm của CV.
- Điều chỉnh Completeness Card lên trên cùng full-width và Suggested Actions checklist thành 2 cột.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Triển khai bộ xác thực ngày (Validation): Không cho phép chọn ngày sinh lớn hơn ngày hiện tại hoặc tuổi nhỏ hơn 15.
- Loại bỏ các chuyển động thô và giật lag, giữ hoạt ảnh hover siêu mượt với thời gian chuyển tiếp 300ms (`duration-300`).
```

---

### Prompt số 3 (Session Inactivity Cookies Cleanup on Expired Token)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-16                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Sửa lỗi bảo mật không xóa được cookies trình duyệt do API logout bị chặn bởi JWT đã hết hạn khi tự động kick AFK.                                      |
| Phần việc liên quan | Backend .NET Core (AuthController.cs)                                                                                                                  |
| Mức độ sử dụng      | Hỗ trợ phân tích luồng chặn của filter Authorization và xác định giải pháp dọn dẹp cookie an toàn qua AllowAnonymous.                                  |

#### 5.1. Prompt nguyên văn

```text
Recheck the auto kick system when AFK. If a user is already kicked due to inactivity, they are currently automatically logged back in upon reloading the page. This is because their access token has expired, causing the POST request to /api/auth/logout to return a 401 Unauthorized from the server's authorization filter before it executes the logout action (skipping cookie deletion). Change the logout endpoint in AuthController.cs to AllowAnonymous so the browser cookies can be cleaned successfully even with an expired JWT.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Người dùng bị tự động đăng xuất do AFK (hệ thống dọn dẹp trạng thái ở client).
- Nhưng do Access Token (JWT) hết hạn trùng lúc, request gọi đến `/auth/logout` bị backend trả về 401 Unauthorized trước khi đi vào hàm xử lý của Controller.
- Kết quả là cookie trình duyệt không bị xóa. Khi reload trang, hệ thống tự động làm mới token và đăng nhập lại cho người dùng, tạo ra lỗ hổng bảo mật phiên làm việc.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất thay đổi attribute `[Authorize]` thành `[AllowAnonymous]` trên method `Logout` trong `AuthController.cs`.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Thay thế thuộc tính phân quyền của endpoint logout thành `[AllowAnonymous]` giúp dọn dẹp cookies thành công khi Access Token đã hết hạn.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Kiểm tra chặt chẽ điều kiện tồn tại của `refresh_token` trong cookies để thực thi thu hồi phiên cụ thể, ngăn chặn các hành vi gửi request logout giả mạo ảnh hưởng đến các phiên đăng nhập khác của người dùng.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cung cấp chi tiết lỗi nhận được (ví dụ như mã lỗi HTTP 401 khi gọi logout hoặc lỗi biên dịch TypeScript) kết hợp với mong muốn nghiệp vụ rõ ràng giúp AI xác định đúng vị trí cần chỉnh sửa trong cấu trúc mã nguồn phức tạp.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Đặt câu hỏi trực diện vào giải pháp kiến trúc (ví dụ: chuyển endpoint sang AllowAnonymous để dọn dẹp cookies) sẽ mang lại phản hồi chính xác và tối giản nhất so với việc hỏi chung chung về cơ chế hoạt động của cookie.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Design |        2 | Add support for external source code providers... / Replace the Date of Birth input... |
| Prompt Fix    |        1 | Recheck the auto kick system when AFK... |

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
| Đoàn Thế Lực            | 2026-06-16    |
