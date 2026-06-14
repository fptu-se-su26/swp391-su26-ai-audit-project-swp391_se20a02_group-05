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
| Ngày bắt đầu | 2026-06-13T07:30:00Z |
| Ngày hoàn thành | 2026-06-13T14:30:00Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 05/06/2026 | Sửa lỗi build hệ thống và bổ sung tùy chọn Career Preferences | Completed |
| Phase 02 | 06/06/2026 | Mở rộng sở thích nghề nghiệp & Chuẩn hóa dữ liệu | Completed |
| Phase 03 | 09/06/2026 | Tinh chỉnh UI/UX, Bố cục lưới Responsive, Tiền tệ động và Thanh thay đổi chưa lưu | Completed |
| Phase 04 | 13/06/2026 | Trang hồ sơ doanh nghiệp công khai, quản lý thư viện ảnh và tích hợp thông tin tổ chức | Completed |
| Phase 05 | 13/06/2026 | Cấu hình Doanh nghiệp và Quản lý Thành viên/Vai trò | Completed |
| Phase 06 |  |  | Not Started |

---

# [Cấu hình Doanh nghiệp & Quản lý Thành viên/Vai trò]

## Ngày thực hiện

```text
2026-06-13
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thêm endpoint PATCH `PATCH /api/workspace/{organizationSlug}` cập nhật chi tiết cấu hình doanh nghiệp (thông tin liên hệ, social links, tags). | Nguyễn Hoàng Ngọc Ánh | WorkspaceController.cs | Commits |
| 2 | Cấu hình hàm helper `MapToWorkspaceDetailsDto` tập trung hóa logic map thực thể tổ chức sang DTO phản hồi. | Nguyễn Hoàng Ngọc Ánh | WorkspaceController.cs | Commits |
| 3 | Mở rộng API members hỗ trợ phân trang (`page`, `pageSize`), tìm kiếm (`search`), và cờ lọc tài khoản công khai (`publicOnly`). | Nguyễn Hoàng Ngọc Ánh | WorkspaceController.cs | Commits |
| 4 | Cải tiến truy vấn DB bằng phép Join thực hiện lấy thông tin `Headline`, `Username` từ bảng `UserProfiles` và `AvatarUrl` đã được ký số cho mỗi thành viên. | Nguyễn Hoàng Ngọc Ánh | WorkspaceController.cs, WorkspaceDtos.cs | Commits |
| 5 | Xóa trang `about/page.tsx` công khai cũ và tinh chỉnh các trang/layout công khai để đồng bộ cấu trúc mới. | Nguyễn Hoàng Ngọc Ánh | client/src/app/workspace/[organizationSlug] | Commits |
| 6 | Đổi tên tab People sang Members trên UI hiển thị thông tin thành viên của doanh nghiệp. | Nguyễn Hoàng Ngọc Ánh | layout.tsx, people/page.tsx | Commits |
| 7 | Cập nhật Zustand store và service gọi API tích hợp cơ chế phân trang và đồng bộ trạng thái thành viên. | Nguyễn Hoàng Ngọc Ánh | use-workspace-store.ts, workspace.service.ts | Commits |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Commit/Screenshot minh chứng

```text
Commit hash: c27c5c2424f6b2f8cf77cecbf15df0849f155d53 (Add workspace update and expand profile data)
```

## Ghi chú

```text
Chức năng này giúp các doanh nghiệp trên hệ thống CVerify chủ động quản lý thông tin thương hiệu của mình và kiểm soát danh sách thành viên nội bộ một cách bảo mật, chuyên nghiệp.
```

---

## 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
1. API PATCH cập nhật thông tin doanh nghiệp được tối ưu hóa bảo mật theo quyền hạn.
2. Danh sách thành viên (Members) hiển thị kèm các thông tin mở rộng (Headline, Username, Avatar đã ký số) hỗ trợ phân trang đầy đủ.
3. Đồng bộ hóa trạng thái qua Zustand store giúp UI mượt mà, phản hồi lập tức.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
1. Chưa thêm tính năng phân quyền trực tiếp (chỉ định/thay đổi vai trò của thành viên trong tổ chức từ giao diện).
```

---

## 4.3. Cải thiện chính

```text
1. Sử dụng Left Join với UserProfiles giúp tránh việc truy vấn lẻ từng thành viên, tối ưu hiệu suất truy cập DB.
2. Chuyển đổi tab People thành Members giúp định nghĩa rõ ràng hơn về thành viên tổ chức.
```

---

## 4.4. Tổng kết project

```text
Giai đoạn này củng cố nền tảng quản trị của doanh nghiệp trên CVerify, mang lại trải nghiệm chuyên nghiệp cho quản trị viên khi quản lý thông tin tổ chức và thông tin nhân sự tham gia hệ thống.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Xây dựng form chỉnh sửa vai trò (Role Management) trực tiếp trên trang cấu hình doanh nghiệp cho Owner/Admin.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
