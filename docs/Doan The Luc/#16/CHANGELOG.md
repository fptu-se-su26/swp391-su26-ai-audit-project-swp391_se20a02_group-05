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
| Ngày bắt đầu          | 2026-06-22T19:43:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-22T20:13:00.000Z                                                               |

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
| Phase 17            | 2026-06-17 ~ 2026-06-18 | Candidate Assessment SSE Streaming, Repository Reset, and Dashboard UI Enhancements           | Completed   |
| Phase 18            | 2026-06-22 ~ 2026-06-22 | Sidebar Navigation Redesign, Accordion Groups & Bi-directional URL Tab Synchronization         | Completed   |

---

# [Phase 18]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-22 ~ 2026-06-22
- **Mô tả giai đoạn:** Sidebar Navigation Redesign, Accordion Groups & Bi-directional URL Tab Synchronization
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Thêm cấu trúc Accordion Groups lồng các mục liên kết con trực quan vào cấu trúc thanh điều hướng sidebar. | Đoàn Thế Lực | client/src/config/navigation-config.ts | GitHub Commit |
|   2 | Khởi tạo các dịch thuật i18n mới cho tiếng Anh và tiếng Việt cho các tab con của Job Board và My CV. | Đoàn Thế Lực | client/src/locales/ | GitHub Commit |
|   3 | Xây dựng layouts dùng chung `PlatformShell` và `PublicPageShell` giúp tái sử dụng các wrapper và sidebar/header shell. | Đoàn Thế Lực | client/src/components/layouts/ | GitHub Commit |
|   4 | Tích hợp SSR token verification và state hydration giúp tải trước thông tin user đăng nhập trên server để giảm thiểu giật trang. | Đoàn Thế Lực | client/src/app/layout.tsx, providers.tsx | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Gom nhóm các liên kết của Job Board và My CV thành các cấu trúc Accordion thu gọn. | Đoàn Thế Lực | client/src/config/navigation-config.ts | GitHub Commit |
|   2 | Thiết lập cơ chế đồng bộ hóa URL hai chiều dựa trên tham số query `tab` giúp đồng bộ tab trang với highlight của sidebar. | Đoàn Thế Lực | JobsPage, CvManagementCenter | GitHub Commit |
|   3 | Di chuyển mục `Repositories` lên phần Intelligence và đổi ID thành `intelligence-repositories`. | Đoàn Thế Lực | client/src/config/navigation-config.ts | GitHub Commit |
|   4 | Cải tiến so khớp active route của SidebarLink bằng việc ủy quyền hoàn toàn cho hàm tập trung `isActiveRoute`. | Đoàn Thế Lực | sidebar-link.tsx, navigation-utils.ts | GitHub Commit |

### Removed

| STT | Nội dung xóa bỏ | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | --------------- | --------------- | --------------------- | ---------- |
|   1 | Xóa bỏ liên kết dự án trùng lặp `Projects` dưới phần Evidence cũ. | Đoàn Thế Lực | client/src/config/navigation-config.ts | GitHub Commit |
|   2 | Loại bỏ hoàn toàn section trống `Evidence` khỏi thanh điều hướng sidebar và logic lọc liên quan. | Đoàn Thế Lực | navigation-config.ts, sidebar-content.tsx | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(navigation): redesign sidebar navigation with collapsible Job Board and My CV groups | https://github.com/Kaivian/CVerify/commit/aea4e82b9c397c731860878cb2ae847f87cf2805 |

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Tái cấu trúc sidebar với các Accordion Groups tiện ích thu gọn cho Job Board và My CV.
- Đồng bộ hóa tab hai chiều trang-sidebar mượt mà thông qua URL query parameter.
- Loại bỏ hoàn toàn section trống Evidence và di chuyển Repositories lên section Intelligence.
- Triệt tiêu lỗi nhấp nháy màn hình khi tải trang (hydration flashing) bằng SSR state hydration cho AuthStore.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Chưa lưu trữ trạng thái đóng/mở (expand/collapse) của từng Group trong localStorage để khôi phục khi reload trang.
```

---

### 4.3. Cải thiện chính

```text
- Giao diện thanh sidebar trở nên tinh gọn, chuyên nghiệp và có tính định hướng người dùng cao hơn rất nhiều.
- Trải nghiệm chuyển đổi tab và điều hướng được tối ưu hóa mượt mà nhờ liên kết trực tiếp với URL.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này mang lại sự hoàn thiện vượt bậc về mặt thiết kế trải nghiệm người dùng điều hướng (Navigation UX) và tối ưu hóa hiệu năng tải ban đầu (Initial Load Performance) của hệ thống CVerify. Việc tổ chức các liên kết thông minh giúp ứng viên dễ dàng tương tác và quản lý tốt hơn hồ sơ năng lực của mình.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Lưu trữ trạng thái đóng/mở của Accordion Groups vào LocalStorage.
2. Tối ưu hóa hiệu ứng chuyển động trượt mở rộng (slide-down/slide-up) của Accordion con trên sidebar.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-22    |
