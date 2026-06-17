# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-06-15T08:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-16T02:00:00.000Z                                                               |

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
Mục tiêu là hoàn thiện và tối ưu hóa hệ thống CVerify thông qua các hạng mục: tái cấu trúc giao diện quản lý CV (đưa Completeness Card lên đầu trang full-width, thêm hiệu ứng hover chuyển động cho thẻ danh mục), chuẩn hóa các ô chọn ngày sinh bằng HeroUI DatePicker, phát triển phân hệ liên kết tài khoản nhà cung cấp mã nguồn ngoài (GitHub/GitLab clients & External Organizations support), tích hợp đánh giá ứng viên toàn diện (Repository Assessments và CV Projects), và giải quyết lỗi bảo mật auto kick khi AFK (mở rộng endpoint logout cho phép truy cập ẩn danh để dọn dẹp cookie khi JWT hết hạn).
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1 (Source-Code Providers & External Organizations Integration)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-15                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Tích hợp các nhà cung cấp mã nguồn ngoài (GitHub/GitLab) và thực thể tổ chức ngoài (ExternalOrganization).                                             |
| Phần việc liên quan | Backend .NET Core (Clients, Services, Controllers, Migrations) & Next.js Frontend (Source Code Providers settings page, service)                       |
| Mức độ sử dụng      | Hỗ trợ sinh mã cấu trúc thực thể, DTOs, khung lớp client GitHub/GitLab, cấu trúc trang cài đặt ở frontend.                                             |

#### 4.1. Prompt đã sử dụng

```text
Add support for external source code providers: introduce AddExternalOrganizations EF migration and ExternalOrganization entity/DTO, update ApplicationDbContext and DbInitializer, and apply snapshot changes. Add new source-code clients (GitHubSourceCodeClient, GitLabSourceCodeClient) and ISourceCodeClient interface, and wire them into the SourceCodeProviderService, controller, repository DTOs and entities. Add frontend pages, service and types for source-code providers.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất kiến trúc thực thể `ExternalOrganization` kết hợp khóa ngoại liên kết với `SourceCodeRepository`. Sinh mã khung cho `GitHubSourceCodeClient` và `GitLabSourceCodeClient` thực hiện gọi API Octokit/GitLab REST API, và viết trang cài đặt `source-code-providers/page.tsx` sử dụng Next.js.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc Migration `AddExternalOrganizations` và mô hình thực thể DB.
- Cấu trúc gọi API của `GitHubSourceCodeClient` và `GitLabSourceCodeClient`.
- Khung giao diện quản trị source-code-providers ở Settings.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Mã hóa tokens: Bổ sung logic mã hóa an toàn AES-GCM cho các access token của GitHub/GitLab trước khi lưu vào DB.
- Tự cấu hình logic xử lý phân trang (Pagination) khi nạp danh sách repositories từ GitHub/GitLab để xử lý được các tài khoản có số lượng repository lớn (>100).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | Add source-code providers and external orgs | https://github.com/Kaivian/CVerify/commit/55e14804779a6d6f28cb33ee890991c3e2a30eaf |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Phân hệ liên kết kho lưu trữ ngoài đóng vai trò quyết định đến tính ứng dụng của CVerify. Việc tổ chức các client ngoài dưới một interface `ISourceCodeClient` giúp hệ thống dễ dàng mở rộng thêm các provider khác như Bitbucket trong tương lai.
```

---

### Lần sử dụng AI số 2 (Candidate Assessment & CV Projects Integration)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-15                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Tích hợp đánh giá kho lưu trữ và các dự án CV vào giao diện đánh giá ứng viên toàn diện.                                                               |
| Phần việc liên quan | Next.js Frontend (Forms, Components, Preview, Stores, Services)                                                                                        |
| Mức độ sử dụng      | Hỗ trợ sinh mã cho `BasicInfoForm.tsx`, `ProjectsForm.tsx`, `CVPreview.tsx`, và hook quản lý dữ liệu dự án.                                            |

#### 4.1. Prompt đã sử dụng

```text
Re-architect candidate assessment to integrate repository assessments and CV projects. Create custom form views BasicInfoForm, ProjectsForm, CVPreview, and hook them to zustand stores. Provide detailed UI elements showing evaluated repository scores, verified commits, and parsed CV details side-by-side.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất giao diện Next.js phân tách thành 3 tab: Thông tin cơ bản, Dự án liên kết và Xem trước CV. Sinh mã boilerplate cho các component form cập nhật dữ liệu ứng viên thông qua API và Zustand.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Boilerplate của form cơ bản và form quản lý dự án (`ProjectsForm`).
- Cấu trúc layout chia cột (Grid) hiển thị điểm đánh giá dự án và liên kết minh chứng.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Triển khai bộ đồng bộ dữ liệu tự động giữa form nhập liệu của ứng viên và Live Preview (CV Live Preview render theo thời gian thực khi người dùng gõ phím).
- Tối ưu hóa UI/UX: Định dạng lại điểm số đánh giá ứng viên bằng biểu đồ đo lường chất lượng tròn (Progress Circle) trực quan thay vì hiển thị số thô.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | refactor(assessment): integrate repository assessments and CV projects into candidate evaluation | https://github.com/Kaivian/CVerify/commit/d185d87178cfcb2b2ee766324d26210214a1a367 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Tích hợp trực tiếp các đánh giá phân tích mã nguồn từ AI vào hồ sơ CV giúp nhà tuyển dụng có cái nhìn thực tế và chính xác nhất về năng lực lập trình của ứng viên.
```

---

### Lần sử dụng AI số 3 (CV Layout Enhancements & HeroUI DatePicker Standardization)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-15                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Đưa Completeness Card lên đầu trang full-width, chuẩn hóa Date Input sang HeroUI DatePicker và cấu hình hiệu ứng chuyển động khi hover thẻ.            |
| Phần việc liên quan | Next.js Frontend (Page Layout, Component Stylings)                                                                                                     |
| Mức độ sử dụng      | Hỗ trợ tái cấu trúc khối renderOverview, hướng dẫn cách sử dụng API của HeroUI DatePicker để xử lý kiểu dữ liệu ngày tháng.                            |

#### 4.1. Prompt đã sử dụng

```text
Replace the native Date of Birth input field in the CV Management page with the HeroUI DatePicker component, ensuring proper date parsing, validation, and layout styling according to HeroUI rules. Restructure the Overview page to place the Completeness scorecard at the top spanning full-width, and add subtle transition animations and translate offset to CV section cards on hover.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất sử dụng import `@heroui/react` và component `DatePicker`. Hướng dẫn sửa CSS grid để đưa Completeness Card lên đầu, đồng thời đề xuất bổ sung các class Tailwind/CSS `hover:shadow-md hover:-translate-y-0.5` vào thẻ card.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc tích hợp HeroUI DatePicker và xử lý chuyển đổi múi giờ ISO-8601 sang đối tượng ngày của HeroUI.
- Cách chia grid layout cho Overview Page.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Thiết lập cơ chế kiểm lỗi (Date Validation): Ngăn chặn việc chọn ngày sinh trong tương lai hoặc tuổi dưới 15 trực tiếp trên bộ DatePicker của HeroUI.
- Kiểm soát hiệu ứng chuyển động: Ban đầu loại bỏ toàn bộ hoạt ảnh để tối ưu tốc độ kết xuất, sau đó bổ sung chọn lọc các hiệu ứng dịch chuyển vi mô cực kỳ mượt mà khi hover để nâng cao trải nghiệm thị giác mà không gây rối mắt.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(cv): layout Completeness card at top of Overview page full-width | https://github.com/Kaivian/CVerify/commit/5e21a8efddb28d8e4c0775eebcf30508425e5291 |
| Commit/PR       | refactor(cv): adjust section cards layout and enable hover animations | https://github.com/Kaivian/CVerify/commit/06e56d9c303cab20dc2bcec4f1d0acbdc4883a59 |
| Commit/PR       | refactor(cv): replace native date input with heroui datepicker | https://github.com/Kaivian/CVerify/commit/8313424e03075c0fca20b5ad1ca11855a40a8501 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc chuẩn hóa DatePicker giúp hệ thống đồng bộ được định dạng ngày tháng đầu vào từ người dùng, tránh các lỗi phân tích ngày tháng trên backend. Giao diện có hiệu ứng hover giúp trang CV sinh động hơn.
```

---

### Lần sử dụng AI số 4 (Session Inactivity Cookies Cleanup on Expired Token)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-16                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Cho phép dọn dẹp cookies trình duyệt của người dùng thành công khi họ bị tự động kick do AFK (tránh tự động đăng nhập lại khi reload).                 |
| Phần việc liên quan | Backend .NET Core (AuthController.cs)                                                                                                                  |
| Mức độ sử dụng      | Hỗ trợ phân tích luồng chặn của filter Authorization và xác định giải pháp dọn dẹp cookie an toàn qua AllowAnonymous.                                  |

#### 4.1. Prompt đã sử dụng

```text
Recheck the auto kick system when AFK. If a user is already kicked due to inactivity, they are currently automatically logged back in upon reloading the page. This is because their access token has expired, causing the POST request to /api/auth/logout to return a 401 Unauthorized from the server's authorization filter before it executes the logout action (skipping cookie deletion). Change the logout endpoint in AuthController.cs to AllowAnonymous so the browser cookies can be cleaned successfully even with an expired JWT.
```

#### 4.2. Kết quả AI gợi ý

```text
AI xác định việc thay đổi annotation `[Authorize]` thành `[AllowAnonymous]` trên endpoint `Logout` trong `AuthController.cs` là giải pháp tối ưu nhất, cho phép request đi tiếp vào service xử lý dọn dẹp cookie và thu hồi refresh token trong cơ sở dữ liệu.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Giải pháp cấu trúc sửa đổi thuộc tính phân quyền của endpoint logout.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Xác minh an ninh: Đảm bảo hàm `LogoutAsync()` thực hiện kiểm tra thực tế `refresh_token` trong Cookie để vô hiệu hóa trên DB, tránh việc lạm dụng API logout ẩn danh để phá hoại các phiên đăng nhập khác của người dùng.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | fix(auth): allow anonymous access to logout endpoint | https://github.com/Kaivian/CVerify/commit/599de424c5b160b73c24f2b960cd42be8169fe5a |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Lỗi cookies không được xóa do API logout bị chặn bởi JWT hết hạn là một lỗi logic bảo mật rất tinh tế nhưng cực kỳ phổ biến. Việc chuyển sang chế độ AllowAnonymous kết hợp thu hồi token thủ công trên backend giải quyết triệt để lỗi trải nghiệm này.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                              |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------ |
| Phân tích yêu cầu           |               |      x       |                 |               | Xác định các vấn đề hiển thị và bảo mật phiên.                                       |
| Viết user story/use case    |       x       |              |                 |               |                                                                                      |
| Thiết kế database           |               |      x       |                 |               | Cấu trúc bảng ExternalOrganization, cột last_profile_update_at.                      |
| Thiết kế kiến trúc hệ thống |               |      x       |                 |               | Thiết kế hệ thống tích hợp client GitHub/GitLab.                                     |
| Thiết kế giao diện          |               |              |        x        |               | Thiết kế Live Preview cho CV và căn chỉnh Completeness Card.                         |
| Code frontend               |               |              |        x        |               | Viết các forms quản lý dự án, DatePicker, và các component trong CV settings.        |
| Code backend                |               |              |        x        |               | Viết GitHub/GitLab clients, SourceCodeProviderService, và cập nhật AuthController.   |
| Debug lỗi                   |               |              |        x        |               | Sửa lỗi biên dịch TypeScript, lỗi chặn 401 khi gọi API logout khi Access Token hết hạn. |
| Viết test case              |       x       |              |                 |               |                                                                                      |
| Kiểm thử sản phẩm           |               |      x       |                 |               | Kiểm thử thủ công quy trình AFK auto kick và liên kết tài khoản GitHub.               |
| Tối ưu code                 |               |      x       |                 |               |                                                                                      |
| Viết báo cáo                |       x       |              |                 |               |                                                                                      |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                      |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI gợi ý viết hàm dọn dẹp cookie bằng JavaScript trên Client (`document.cookie = ...`) trong hàm logout. | Khi chạy thực tế, cookies `access_token` và `refresh_token` không bị xóa vì chúng được đánh dấu `HttpOnly`. | Chuyển logic dọn dẹp cookie về backend thông qua API ẩn danh để phản hồi `Set-Cookie` dọn dẹp. |
|   2 | AI viết sai import component DatePicker của HeroUI hoặc sử dụng sai API định dạng ngày tháng của `@heroui/react`. | Quá trình biên dịch Next.js (npm run build) ném ra lỗi cú pháp import và không tìm thấy module. | Kiểm tra lại tài liệu chính thức của HeroUI để sử dụng đúng namespace và phương thức gán giá trị mặc định. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả qua các hình thức sau:
1. Chạy thành công ứng dụng frontend và backend. Kiểm tra tính năng AFK auto-kick: Sau 15 phút không hoạt động, màn hình countdown hiện ra, hệ thống tự động gọi API logout thành công, cookies bị xóa sạch, reload trang không còn tự động đăng nhập.
2. Kiểm tra trang Settings -> Source Code Providers: Liên kết thành công tài khoản GitHub/GitLab, lấy được danh sách repositories và thực thể tổ chức hiển thị chính xác.
3. Kiểm tra trang CV: Completeness Card nằm trên cùng, các card CV hiển thị mượt mà với hoạt ảnh hover dịch chuyển tinh tế. Bộ chọn DatePicker của HeroUI hoạt động chuẩn xác, không lỗi định dạng.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Tự cấu hình thuật toán mã hóa AES-GCM bảo vệ Access Token của GitHub/GitLab trên cơ sở dữ liệu.
- Tự viết logic đồng bộ dữ liệu thời gian thực giữa form thông tin cơ bản / dự án và CV Live Preview.
- Phát hiện lỗi logic bảo mật của endpoint logout khi Access Token hết hạn và điều chỉnh sang AllowAnonymous an toàn.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                                 | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ---------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai clients tích hợp bên thứ ba, fix lỗi bảo mật logout, thiết kế database & migration. | Có                   | GitHub Commits      |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Thiết kế layouts CV Overview, chuẩn hóa HeroUI DatePicker, thêm hiệu ứng hover cho các cards.   | Có                   | GitHub Commits      |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI hỗ trợ viết nhanh các tệp tin cấu hình migration, khung client gọi API ngoài, cấu trúc CSS/Tailwind cho layouts grid ở frontend, giúp tiết kiệm đáng kể thời gian dựng boilerplate.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không dùng code xóa cookie bằng JS client do AI gợi ý vì không thể xóa được cookie HttpOnly. Luôn phải gọi API backend để đảm bảo bảo mật và xóa cookies triệt để.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Nhóm tiến hành biên dịch ứng dụng (`dotnet build` và `npm run build`), chạy kiểm thử tích hợp thực tế (End-to-End) các luồng đăng xuất tự động, nạp dữ liệu từ GitHub, và kiểm tra dữ liệu thực tế lưu trữ trong cơ sở dữ liệu PostgreSQL.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần viết các cấu trúc gọi API phức tạp của GitHub/GitLab (OAuth & Repositories fetch) do có rất nhiều mô hình dữ liệu phản hồi (DTOs) cần phải định nghĩa chính xác.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Học được cách tích hợp dịch vụ bên thứ ba an toàn (mã hóa credentials trước khi lưu), hiểu sâu sắc cơ chế hoạt động của middleware authentication/authorization trong ASP.NET Core, và cách tối ưu hóa trải nghiệm người dùng bằng các hoạt ảnh vi mô (micro-animations).
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI là trợ thủ tuyệt vời nhưng cần phải có sự kiểm định liên tục từ con người, đặc biệt là các vấn đề liên quan đến bảo mật (HttpOnly cookies) và cấu hình phiên bản thư viện UI để tránh các lỗi biên dịch đáng tiếc.
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
| Đoàn Thế Lực            | 2026-06-16    |
