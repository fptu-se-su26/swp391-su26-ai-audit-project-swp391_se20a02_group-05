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
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-06-15T08:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-16T02:00:00.000Z                                                               |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian               | Nội dung chính                                                                                 | Trạng thái  |
| ------------------- | ----------------------- | ---------------------------------------------------------------------------------------------- | ----------- |
| Phase 01            |                         |                                                                                                | Not Started |
| Phase 02            |                         |                                                                                                | Not Started |
| Phase 03            |                         |                                                                                                | Not Started |
| Phase 04            |                         |                                                                                                | Not Started |
| Phase 05            |                         |                                                                                                | Not Started |
| Phase 06            | 2026-05-23 ~ 2026-05-23 | Secure Authentication Refactoring & Super Admin Enhancements                                   | Completed   |
| Phase 07            | 2026-05-28 ~ 2026-05-28 | Reclaim Ownership OTP Verification & Identity Normalization                                    | Completed   |
| Phase 08            | 2026-05-29 ~ 2026-05-29 | Components System Visual Explorer & Workspace Architecture                                     | Completed   |
| Phase 09            | 2026-05-30 ~ 2026-05-30 | Secure OAuth Integration & Settings Change Password Overhaul                                   | Completed   |
| Phase 10            | 2026-05-31 ~ 2026-05-31 | Email Normalization Correction, Multi-Email Support & Password Recovery Overhaul               | Completed   |
| Phase 11            | 2026-05-31 ~ 2026-05-31 | Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation             | Completed   |
| Phase 12            | 2026-06-01 ~ 2026-06-01 | Account Deletion Lifecycle & Modular Monolith Transition                                       | Completed   |
| Phase 13            | 2026-06-02 ~ 2026-06-02 | Automatic Username System & Public Profile Routing                                             | Completed   |
| Phase 14            | 2026-06-03 ~ 2026-06-03 | Persisting Avatar Source, Re-engineering Experience/Achievements Settings & Form Consistency   | Completed   |
| Phase 15            | 2026-06-05 ~ 2026-06-06 | Repository Analysis Engine with Real-time SSE Progress Streaming                               | Completed   |
| Phase 16            | 2026-06-15 ~ 2026-06-16 | AI CV Assessment, Source-Code Provider Integrations, and Session Inactivity Management         | Completed   |

---

# [Phase 16]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-15 ~ 2026-06-16
- **Mô tả giai đoạn:** AI CV Assessment, Source-Code Provider Integrations, and Session Inactivity Management
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Cung cấp thực thể `ExternalOrganization` và DTO quản lý các tổ chức mã nguồn ngoài trong hệ thống CVerify. | Đoàn Thế Lực | ExternalOrganization.cs | GitHub Commit |
|   2 | Thêm migration `AddExternalOrganizations` thiết lập các bảng liên kết tổ chức ngoài trong DB. | Đoàn Thế Lực | 20260615141609_AddExternalOrganizations.cs | GitHub Commit |
|   3 | Phát triển GitHub và GitLab API Clients phục vụ truy vấn repository và organization metadata. | Đoàn Thế Lực | GitHubSourceCodeClient.cs, GitLabSourceCodeClient.cs | GitHub Commit |
|   4 | Thêm trang cài đặt kết nối tài khoản mã nguồn ngoài Settings Page ở Frontend. | Đoàn Thế Lực | settings/source-code-providers/page.tsx | GitHub Commit |
|   5 | Thêm cột `last_profile_update_at` lưu trữ dấu vết thời gian cập nhật hồ sơ cá nhân trong `user_profiles`. | Đoàn Thế Lực | DbInitializer.cs | GitHub Commit |
|   6 | Thêm các form cấu hình thông tin dự án CV (`ProjectsForm.tsx`, `BasicInfoForm.tsx`, `CVPreview.tsx`). | Đoàn Thế Lực | client/src/app/(private)/cv/components/ | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Cải tiến giao diện CV Overview: Đưa Completeness Card lên đầu trang hiển thị full-width, tổ chức Suggested Actions checklist dạng lưới 2 cột. | Nguyễn Hoàng Ngọc Ánh | client/src/app/(private)/cv/page.tsx | GitHub Commit |
|   2 | Tích hợp biểu cảm hover động (shadow-md và -translate-y-0.5) cho các thẻ danh mục CV. | Nguyễn Hoàng Ngọc Ánh | client/src/app/(private)/cv/page.tsx | GitHub Commit |
|   3 | Chuẩn hóa các ô chọn ngày sinh của ứng viên sang sử dụng HeroUI DatePicker thay vì native date input. | Nguyễn Hoàng Ngọc Ánh | client/src/app/(private)/cv/components/BasicInfoForm.tsx | GitHub Commit |
|   4 | Cập nhật luồng xử lý tương tác bài đăng (optimistic like/share count updates) trên Dashboard. | Đoàn Thế Lực | posts/page.tsx | GitHub Commit |

### Fixed

| STT | Nội dung sửa lỗi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ---------------- | --------------- | --------------------- | ---------- |
|   1 | Thay đổi endpoint `/api/auth/logout` từ `[Authorize]` sang `[AllowAnonymous]` để cho phép dọn dẹp cookies khi Access Token đã hết hạn do AFK. | Đoàn Thế Lực | AuthController.cs | GitHub Commit |
|   2 | Khắc phục các lỗi biên dịch TypeScript và ESLint cảnh báo trên client Next.js. | Đoàn Thế Lực | client/src/app/(private)/cv/page.tsx | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | fix(auth): allow anonymous access to logout endpoint | https://github.com/Kaivian/CVerify/commit/599de424c5b160b73c24f2b960cd42be8169fe5a |
| Commit/PR       | Add source-code providers and external orgs | https://github.com/Kaivian/CVerify/commit/55e14804779a6d6f28cb33ee890991c3e2a30eaf |

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Tích hợp thành công kết nối OAuth ngoài với hai nhà cung cấp GitHub và GitLab, hỗ trợ lấy thông tin tổ chức ngoài.
- Nâng cấp giao diện CV: Giao diện Completeness Card full-width nổi bật, chuẩn hóa HeroUI DatePicker cho đầu vào ngày tháng, và hoạt ảnh hover vi mô mượt mà.
- Thiết lập cơ chế đánh giá ứng viên tổng hợp từ kết quả phân tích kho lưu trữ mã nguồn và CV Projects.
- Khắc phục lỗi bảo mật rò rỉ phiên AFK auto-kick thông qua việc cho phép dọn dẹp cookies ẩn danh trên backend.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Chưa hỗ trợ đầy đủ việc lọc và chọn lựa nhánh cụ thể (Branch selection) để phân tích khi liên kết repository ngoài (mặc định phân tích nhánh main/default).
```

---

### 4.3. Cải thiện chính

```text
- Đồng bộ hóa chặt chẽ và an toàn thông tin của các kho lưu trữ bên ngoài với quy trình đánh giá ứng viên.
- Trải nghiệm bảo mật và đăng xuất tự động hoàn thiện, không bị tự động khôi phục phiên do lỗi cookies tồn đọng.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này hoàn thành kết nối End-to-End quan trọng từ dữ liệu lập trình thực tế ngoài (GitHub/GitLab) của ứng viên đi thẳng vào dữ liệu hồ sơ CV và hệ thống đánh giá tự động trên CVerify, tạo ra một trải nghiệm ứng tuyển minh bạch, chuyên nghiệp và có tính xác thực cao.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Phát triển cơ chế phân quyền chi tiết cho các thành viên trong External Organizations.
2. Thêm bộ lọc cảnh báo sớm cho ứng viên nếu họ chọn ngày sinh không phù hợp hoặc sai lệch lớn so với dữ liệu lịch sử làm việc.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-16    |
