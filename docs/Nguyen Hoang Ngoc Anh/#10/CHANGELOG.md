# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

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
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05.git |
| Ngày bắt đầu | 2026-06-13T01:00:00Z |
| Ngày hoàn thành | 2026-06-13T01:30:00Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 05/06/2026 | Sửa lỗi build hệ thống và bổ sung tùy chọn Career Preferences | Completed |
| Phase 02 | 06/06/2026 | Mở rộng sở thích nghề nghiệp & Chuẩn hóa dữ liệu | Completed |
| Phase 03 | 09/06/2026 | Tinh chỉnh UI/UX, Bố cục lưới Responsive, Tiền tệ động và Thanh thay đổi chưa lưu | Completed |
| Phase 04 | 13/06/2026 | Trang hồ sơ doanh nghiệp công khai, quản lý thư viện ảnh và tích hợp thông tin tổ chức | Completed |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Trang hồ sơ doanh nghiệp công khai & Quản lý thư viện ảnh Gallery]

## Ngày thực hiện

```text
2026-06-13
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Mở rộng thực thể `Organization` thêm các trường mới: bannerUrl, logoUrl, mô tả công ty, thông tin liên hệ, liên kết mạng xã hội, số lượng chi nhánh, danh sách tag ngành nghề và phúc lợi, và mảng URL ảnh gallery. | Nguyễn Hoàng Ngọc Ánh | Organization.cs | Commits |
| 2 | Cập nhật `ApplicationDbContext` cấu hình kiểu cột mảng (array) cho các trường lưu trữ danh sách chuỗi động. | Nguyễn Hoàng Ngọc Ánh | ApplicationDbContext.cs | Commits |
| 3 | Bổ sung logic kiểm tra và tạo cột mới động trong `DbInitializer.cs` bằng SQL DDL an toàn, không phá vỡ các bản ghi hiện có. | Nguyễn Hoàng Ngọc Ánh | DbInitializer.cs | Commits |
| 4 | Mở rộng `WorkspaceDtos.cs` với DTO mới `WorkspaceDetailsDto` chứa permissions, workspaces, media và metadata công ty; thêm `UpdateWorkspaceDetailsRequestDto` cho yêu cầu PATCH. | Nguyễn Hoàng Ngọc Ánh | WorkspaceDtos.cs | Commits |
| 5 | Thêm endpoint PATCH cập nhật thông tin doanh nghiệp và hai endpoint POST/DELETE quản lý ảnh gallery trong `WorkspaceController.cs`, tích hợp kiểm tra quyền Owner/Admin và sinh URL lưu trữ tạm thời có ký số (Signed URL). | Nguyễn Hoàng Ngọc Ánh | WorkspaceController.cs | Commits |
| 6 | Xây dựng giao diện trang hồ sơ công khai `workspace-public-profile-view.tsx` với bố cục banner, thông tin liên hệ có biểu tượng, và lưới thư viện ảnh (Gallery Grid). | Nguyễn Hoàng Ngọc Ánh | workspace-public-profile-view.tsx | Commits |
| 7 | Xây dựng giao diện chỉnh sửa thông tin doanh nghiệp `workspace-information-view.tsx` hỗ trợ upload Logo, Banner, quản lý tag ngành nghề, liên kết mạng xã hội và thư viện ảnh. | Nguyễn Hoàng Ngọc Ánh | workspace-information-view.tsx | Commits |
| 8 | Cập nhật Zustand store `use-workspace-store.ts` quản lý trạng thái workspace chi tiết, loading state và các action upload/delete đồng bộ với UI. | Nguyễn Hoàng Ngọc Ánh | use-workspace-store.ts | Commits |
| 9 | Cập nhật workspace.service.ts và workspace.types.ts mở rộng định nghĩa kiểu dữ liệu và các hàm gọi API tương ứng. | Nguyễn Hoàng Ngọc Ánh | workspace.service.ts, workspace.types.ts | Commits |
| 10 | Thêm trang công khai `about/page.tsx`, layout `layout.tsx` và trang chủ `page.tsx` trong thư mục `workspace/[organizationSlug]/(public)`. | Nguyễn Hoàng Ngọc Ánh | app/workspace/[organizationSlug]/(public)/ | Commits |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Commit/Screenshot minh chứng

```text
Commit hash: a34efe358a34e34b17795d3d31d762d56ce05b3b (Add public workspace profile and gallery support)
```

## Ghi chú

```text
Tính năng hồ sơ công khai giúp các nhà tuyển dụng và ứng viên dễ dàng tìm hiểu về tổ chức trước khi ứng tuyển, nâng cao uy tín và tính chuyên nghiệp của doanh nghiệp trên nền tảng CVerify.
```

---

## 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
1. Trang hồ sơ doanh nghiệp công khai đầy đủ thông tin: banner, logo, mô tả, thông tin liên hệ và mạng xã hội.
2. Hệ thống quản lý thư viện ảnh (Gallery): upload, xem trước và xóa ảnh với lưu trữ bảo mật sử dụng Signed URL.
3. API PATCH cập nhật thông tin tổ chức kết hợp kiểm tra quyền hạn (Owner/Admin only).
4. Cơ sở dữ liệu tự động cập nhật schema an toàn qua DbInitializer khi triển khai phiên bản mới.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
1. Chưa thêm tính năng tìm kiếm và lọc doanh nghiệp theo ngành nghề, khu vực địa lý hay mức độ phúc lợi.
```

---

## 4.3. Cải thiện chính

```text
1. Sử dụng Signed URL S3 để bảo vệ ảnh gallery, chỉ cho phép truy cập tạm thời có xác thực thay vì expose đường dẫn bucket trực tiếp.
2. Xây dựng SQL DDL kiểm tra sự tồn tại của cột trước khi thêm mới giúp triển khai an toàn trên môi trường production mà không làm mất dữ liệu.
```

---

## 4.4. Tổng kết project

```text
Phase 4 hoàn thiện tính năng hồ sơ công khai cho tổ chức/doanh nghiệp trên CVerify, tạo ra kênh giao tiếp chuyên nghiệp giữa nhà tuyển dụng và ứng viên tiềm năng với đầy đủ thông tin về văn hóa, môi trường làm việc và phúc lợi công ty.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Tích hợp hệ thống đánh giá doanh nghiệp (Company Review) dựa trên phản hồi của ứng viên và nhân viên.
2. Tối ưu hóa tải ảnh gallery bằng kỹ thuật lazy-loading và CDN caching.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
