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
| Ngày bắt đầu | 2026-06-14T00:00:00Z |
| Ngày hoàn thành | 2026-06-14T17:50:00Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 05/06/2026 | Sửa lỗi build hệ thống và bổ sung tùy chọn Career Preferences | Completed |
| Phase 02 | 06/06/2026 | Mở rộng sở thích nghề nghiệp & Chuẩn hóa dữ liệu | Completed |
| Phase 03 | 09/06/2026 | Tinh chỉnh UI/UX, Bố cục lưới Responsive, Tiền tệ động và Thanh thay đổi chưa lưu | Completed |
| Phase 04 | 13/06/2026 | Trang hồ sơ doanh nghiệp công khai, quản lý thư viện ảnh và tích hợp thông tin tổ chức | Completed |
| Phase 05 | 13/06/2026 | Cấu hình Doanh nghiệp và Quản lý Thành viên/Vai trò | Completed |
| Phase 06 | 14/06/2026 | Phát triển Bài đăng và Tin tuyển dụng Doanh nghiệp | Completed |

---

# [Phát triển Bài đăng & Tin tuyển dụng Doanh nghiệp]

## Ngày thực hiện

```text
2026-06-14
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thêm các thực thể `WorkspacePost` và `JobVacancy` đại diện cho các bảng dữ liệu bài viết và tuyển dụng trong cơ sở dữ liệu. | Nguyễn Hoàng Ngọc Ánh | Domain/Entities/ | Commits |
| 2 | Cấu hình quan hệ thực thể, kiểu dữ liệu mảng của thuộc tính trên cơ sở dữ liệu qua `ApplicationDbContext.cs`. | Nguyễn Hoàng Ngọc Ánh | ApplicationDbContext.cs | Commits |
| 3 | Tạo và áp dụng các file Migration nâng cấp DB schema: `20260614071252_AddWorkspacePostsTable` và `20260614100549_AddJobVacanciesTable`. | Nguyễn Hoàng Ngọc Ánh | Migrations/ | Commits |
| 4 | Thêm API tạo và xem bài đăng (`POST/GET /api/workspace/{organizationSlug}/posts`) và API tương ứng cho tin tuyển dụng trong `WorkspaceController.cs`. | Nguyễn Hoàng Ngọc Ánh | WorkspaceController.cs, WorkspaceDtos.cs | Commits |
| 5 | Thêm endpoints `POST /jobs/{jobId}/apply` và `POST /jobs/{jobId}/save` phục vụ người dùng tương tác với tin tuyển dụng. | Nguyễn Hoàng Ngọc Ánh | WorkspaceController.cs | Commits |
| 6 | Phát triển giao diện viết thông báo và danh sách bài đăng có đính kèm lưới ảnh trên trang `posts/page.tsx`. | Nguyễn Hoàng Ngọc Ánh | posts/page.tsx | Commits |
| 7 | Phát triển giao diện danh sách tin tuyển dụng nâng cao trên `jobs/page.tsx` hỗ trợ tìm kiếm từ khóa, bộ lọc vị trí/phòng ban, và Drawer xem chi tiết việc làm. | Nguyễn Hoàng Ngọc Ánh | jobs/page.tsx | Commits |
| 8 | Đồng bộ trạng thái và các tác vụ tương tác API thông qua Zustand store `use-workspace-store.ts`. | Nguyễn Hoàng Ngọc Ánh | use-workspace-store.ts, workspace.service.ts | Commits |
| 9 | Mở rộng định nghĩa kiểu dữ liệu TypeScript cho tin tuyển dụng và bài viết trong `workspace.types.ts`. | Nguyễn Hoàng Ngọc Ánh | workspace.types.ts | Commits |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Commit/Screenshot minh chứng

```text
Commit hash: e0b7e141a3d6e3b1b4be28d78a9d74599e55bef6 (update - migration and backend/frontend logic for posts and jobs)
Commit hash: 4fc92d0f6508e353de29113f94f9f21763e6e6a1 (Add organization follow feature & jobs UI)
```

## Ghi chú

```text
Sự tích hợp này hoàn thiện các tính năng cốt lõi của một trang doanh nghiệp, cho phép tương tác trực tiếp giữa ứng viên (nộp CV, lưu tin) và doanh nghiệp (đăng thông báo, đăng tuyển dụng).
```

---

## 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
1. Hệ thống tin tuyển dụng hoàn chỉnh có bộ lọc nâng cao và Drawer chi tiết.
2. Hệ thống thông báo/bài viết của tổ chức cho phép tạo bài viết có hình ảnh đính kèm từ giao diện người dùng.
3. Database Migration được cấu trúc rõ ràng để lưu vết thực thể bài viết và tuyển dụng trong DB PostgreSQL.
4. Cơ chế đồng bộ hóa dữ liệu Frontend tức thời qua Zustand store.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
1. Chưa tích hợp hệ thống tải CV ứng viên thực tế lên bộ lưu trữ S3 (hiện tại CV ứng tuyển đang liên kết thông tin text mô tả).
```

---

## 4.3. Cải thiện chính

```text
1. Chia nhỏ các cấu phần dữ liệu việc làm (phúc lợi, kỹ năng) thành dạng thẻ (Badge) trực quan trên UI.
2. Xây dựng Drawer chi tiết thay vì Modal giúp giữ nguyên ngữ cảnh trang danh sách, cải thiện độ tiện dụng cho ứng viên.
```

---

## 4.4. Tổng kết project

```text
Việc phát triển trang cá nhân doanh nghiệp với tin bài và tin tuyển dụng giúp CVerify nâng cao khả năng kết nối giữa người lao động và doanh nghiệp, tạo ra giá trị thiết thực và tương tác cao trên nền tảng.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Phát triển hệ thống quản lý ứng viên (Applicant Tracking System - ATS) cơ bản dành riêng cho doanh nghiệp để xét duyệt CV.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 14/06/2026 |
