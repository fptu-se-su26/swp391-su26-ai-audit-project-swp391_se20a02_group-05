# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-06-13T01:00:00Z |
| Ngày hoàn thành | 2026-06-13T01:30:00Z |

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
Phát triển và mở rộng tính năng Trang hồ sơ doanh nghiệp công khai (Public Workspace Profile) và quản lý bộ sưu tập ảnh (Media Gallery) trên cả hai phía Backend và Frontend. Sử dụng AI để sinh mã kiểm soát tính hợp lệ của DTO, cấu trúc thực thể DB, sinh liên kết lưu trữ S3 tạm thời (Signed URLs), viết API upload/delete ảnh và xây dựng giao diện hiển thị thư viện ảnh cũng như chỉnh sửa hồ sơ tổ chức ở Frontend.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-13 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Thiết lập các trường hồ sơ tổ chức mới trong thực thể `Organization.cs`, viết mã dịch chuyển cơ sở dữ liệu động trong `DbInitializer.cs`, mở rộng các endpoint và DTO trong `WorkspaceController.cs` và `WorkspaceDtos.cs` để trả về các signed URL cho Banner, Logo và Gallery. |
| Phần việc liên quan | Coding Backend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Expand Organization entity with profile columns (bannerUrl, logoUrl, description, contact details, social links, branchCount, benefits, and list of gallery URLs). Update ApplicationDbContext to handle array configuration for columns. In DbInitializer.cs, write SQL queries to automatically add these columns if they do not exist. In WorkspaceController.cs, add PATCH endpoint to update workspace details, and endpoints to upload/delete gallery media, ensuring permissions are verified. Use storage service to generate GetSignedUrlAsync for logo, banner, and gallery items."
```

#### 4.2. Kết quả AI gợi ý

```text
- Đoạn mã thực thể Organization chứa các thuộc tính chuỗi, số nguyên và mảng chuỗi cùng với cấu trúc ánh xạ DTO.
- Phương thức DbInitializer kiểm tra sự tồn tại của các cột mới trong bảng Organizations bằng SQL Query và gọi ExecuteSqlRaw để thêm cột động nếu thiếu.
- Các API PATCH UpdateWorkspaceDetails và POST/DELETE gallery media trong WorkspaceController sử dụng IStorageService để tạo đường dẫn tạm thời chứa chữ ký bảo mật S3 (signed URLs) và kiểm tra tính hợp lệ của Token người dùng.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc cột thực thể Organization và cấu hình DbContext.
- SQL DDL kiểm tra và tự động cập nhật Database schema trong DbInitializer.cs.
- Cơ chế ký số URLs GetSignedUrlAsync và các DTO chuyển đổi WorkspaceDetailsDto, UpdateWorkspaceDetailsRequestDto.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tối ưu hóa phần kiểm tra quyền sở hữu Workspace (quyền Owner/Admin) trong WorkspaceController để tránh việc người dùng trái phép chỉnh sửa thông tin doanh nghiệp khác.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | Organization.cs, DbInitializer.cs, WorkspaceController.cs | Mở rộng tính năng Workspace ở Backend |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI đề xuất giải pháp xử lý mảng chuỗi (array) trên Database rất tốt và cấu trúc API logic upload/delete chặt chẽ, tối ưu hiệu suất.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-13 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Xây dựng các View hiển thị hồ sơ doanh nghiệp công khai (`workspace-public-profile-view.tsx`), View chỉnh sửa thông tin và Upload ảnh (`workspace-information-view.tsx`), quản lý trạng thái thông tin qua `use-workspace-store.ts`, viết các service API gửi file lên hệ thống lưu trữ ở Frontend. |
| Phần việc liên quan | Coding Frontend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Create a workspace public profile view with sections for banner image, company description, contact metadata, social links, and an interactive media gallery. Also build a workspace information editing view containing inputs for logo, banner, tags, social links, and a gallery manager supporting file uploads and deletion. Write a Zustand store (use-workspace-store.ts) to manage current workspace details, loading states, and actions to upload/delete gallery media."
```

#### 4.2. Kết quả AI gợi ý

```text
- Component JSX hiển thị trang hồ sơ công khai kết hợp bố cục banner tràn viền, thông tin liên hệ định dạng biểu tượng, và lưới ảnh (Gallery Grid).
- Component thông tin chi tiết hỗ trợ chọn tệp tin (File input) kích hoạt hàm gọi service tải lên (Upload) hoặc xóa ảnh thư viện.
- Kho Zustand `use-workspace-store.ts` quản lý state `workspaceDetails`, loading, và đồng bộ hóa danh sách ảnh cục bộ sau khi upload/delete thành công.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Giao diện và cấu trúc layout `workspace-public-profile-view.tsx` và `workspace-information-view.tsx`.
- Cấu trúc Zustand store điều phối dữ liệu Workspace.
- Định nghĩa kiểu dữ liệu trong `workspace.types.ts` đồng bộ với Backend DTO.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Chỉnh sửa giao diện CSS để phần gallery ảnh có tính năng xem trước khi nhấn vào (Light-box preview) và cải thiện responsiveness trên các thiết bị màn hình nhỏ.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | workspace.service.ts, use-workspace-store.ts, views/ | Giao diện hiển thị và quản lý thông tin Workspace |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Hỗ trợ sinh giao diện trực quan và quản lý State bằng Zustand rất gọn gàng, tương tác tốt và hạn chế render thừa.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Ý tưởng | x |   |   |   |   |
| Phát triển ý tưởng |   | x |   |   |   |
| Review kết quả |   |   | x |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI gợi ý sử dụng hàm JavaScript Array.map trực tiếp cho các trường mảng có giá trị null trả về từ API, gây lỗi Crash giao diện khi mới tải trang. | Chạy thử nghiệm phát sinh lỗi undefined | Thêm các toán tử kiểm tra an toàn hoặc gán giá trị mặc định dạng mảng rỗng `|| []` trước khi thực hiện duyệt mảng. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
1. Biên dịch thành công mã nguồn Backend CVerify.Core với dotnet build.
2. Kiểm tra schema tự động cập nhật hoàn thiện trên cơ sở dữ liệu qua DbInitializer.
3. Chạy lệnh kiểm tra TypeScript `npx tsc --noEmit` ở Client đạt kết quả 100% không phát sinh lỗi.
4. Kiểm tra cập nhật thông tin doanh nghiệp (social links, tags, description) lưu trữ thành công thông qua API PATCH.
5. Upload và delete tệp ảnh trong gallery hoạt động đúng logic, lưu trữ thành công trên cloud storage và hiển thị chính xác signed URLs trên trang About/Page công khai.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Người dùng đóng góp: Xác định các nghiệp vụ hiển thị thông tin doanh nghiệp, cấu hình định dạng cột DB, kiểm thử tích hợp upload/delete tệp tin thực tế, cấu hình bảo mật lưu trữ và thực thi audit log.

AI thực hiện: Cung cấp mã nguồn thực thể, các câu lệnh SQL DDL cập nhật Db, dịch vụ S3 signed URLs, các component view, Zustand store, và các hàm gọi API liên quan.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Phát triển Trang hồ sơ doanh nghiệp & Gallery | Có | Commit a34efe358a34e34b17795d3d31d762d56ce05b3b |
| Trương Văn Hiếu | DE190105 |  | Không |   |
| Đoàn Thế Lực | DE200523 |  | Không |   |
| Nguyễn La Hòa An | DE201043 |  | Không |   |
| Trần Nhất Long | DE200160  |  | Không |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
