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
| Tên bài tập / Project | CVerify - Admin Components System Page                                                 |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05   |
| Ngày bắt đầu          | 2026-05-29T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-29T23:59:59.000Z                                                               |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian               | Nội dung chính                                                                                 | Trạng thái  |
| ------------------- | ----------------------- | ---------------------------------------------------------------------------------------------- | ----------- |
| Phase 01            |                         |                                                               | Not Started |
| Phase 02            |                         |                                                               | Not Started |
| Phase 03            |                         |                                                               | Not Started |
| Phase 04            |                         |                                                               | Not Started |
| Phase 05            |                         |                                                               | Not Started |
| Phase 06            | 2026-05-23 ~ 2026-05-23 | Secure Authentication Refactoring & Super Admin Enhancements                                   | Completed   |
| Phase 07            | 2026-05-28 ~ 2026-05-28 | Reclaim Ownership OTP Verification & Identity Normalization                                    | Completed   |
| Phase 08            | 2026-05-29 ~ 2026-05-29 | Components System Visual Explorer & Workspace Architecture                                     | Completed   |

---

# [Phase 08]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-05-29 ~ 2026-05-29
- **Mô tả giai đoạn:** Components System Visual Explorer & Workspace Architecture
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi                                                                                                                                                                                                               | Người thực hiện | File/Module liên quan                                                       | Minh chứng    |
| --: | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------- | --------------------------------------------------------------------------- | ------------- |
|   1 | Triển khai Workspace Abstraction: Tạo `WorkspaceProvider`, `useWorkspace`, và bộ phân giải Sidebar linh hoạt nhằm cô lập các vùng làm việc quản trị trong trang admin, loại bỏ sự phụ thuộc chặt chẽ vào pathname kiểm tra thủ công. | Đoàn Thế Lực    | workspace-provider.tsx, workspace-layout.tsx, sidebar-link.tsx              | GitHub Commit |
|   2 | Chuẩn hóa mô hình Đồ thị Thành phần: Xây dựng các định nghĩa TypeScript cho `ComponentNode` và `ComponentEdge`, phân tách registry thành các tệp tin chuyên biệt theo phương pháp Atomic Design (`atoms`, `molecules`, `organisms`) hỗ trợ lazy routing. | Đoàn Thế Lực    | registry/atoms.ts, registry/molecules.ts, registry/organisms.ts, types.ts   | GitHub Commit |
|   3 | Lập hộp cát cô lập lỗi (Preview Sandbox Isolation): Triển khai `PreviewSandbox` kết hợp `PreviewErrorBoundary` cùng các stub context ảo (router, i18next) để render runtime an toàn mà không làm sập luồng điều khiển của trang chính. | Đoàn Thế Lực    | preview-sandbox.tsx, preview-error-boundary.tsx, mock-contexts.tsx          | GitHub Commit |
|   4 | Vẽ sơ đồ liên kết động qua React Flow: Tích hợp `@xyflow/react` để trực quan hóa cây phả hệ các thành phần từ trái qua phải (Atom -> Molecule -> Organism), hỗ trợ tương tác Zoom/Pan, tô sáng các quan hệ phụ thuộc lẫn nhau.  | Đoàn Thế Lực    | components-system-graph.tsx, component-node.tsx                             | GitHub Commit |
|   5 | Tìm kiếm Spotlight và Hướng bàn phím (Keyboard-First UX): Thiết kế thanh tìm kiếm Spotlight (`Ctrl+K`) và hỗ trợ điều hướng danh sách bằng các phím mũi tên lên/xuống để tối ưu hóa năng suất làm việc của lập trình viên.      | Đoàn Thế Lực    | components-system-view.tsx, spotlight-search.tsx, use-arrow-navigation.ts   | GitHub Commit |
|   6 | Phân quyền Granular Access Control: Cập nhật registry quyền hạn của hệ thống, bổ sung quyền `components:system:read` và tích hợp middleware bảo vệ route `/admin/components`, chặn truy cập trái phép bằng giao diện "Access Revoked". | Đoàn Thế Lực    | permissions-registry.ts, admin-guard.tsx                                    | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn                                                                 | Nội dung                                                                                                                             |
| --------------- | -------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Commit/PR       | feat(admin): implement Components System visual architecture explorer | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/3cab46522cbbd42171c6670a48b9f71c4c379a52 |

## Ghi chú

```text

```

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Kiến trúc Workspace Abstraction: Tách biệt hoàn toàn sidebar và luồng điều phối của Components System khỏi admin dashboard chính thông qua Context Provider động.
- Trực quan hóa kiến trúc phả hệ: Biểu diễn trực quan dòng chảy phụ thuộc của các component từ nguyên tử (Atoms) đến các phân tử (Molecules) và sinh vật phức tạp (Organisms) bằng sơ đồ cây vẽ qua React Flow.
- Hộp cát Sandbox Isolation: Cho phép nhà phát triển xem trước và cấu hình các thuộc tính giao diện trực tiếp trên trình duyệt một cách an toàn mà không lo lỗi crash component làm ảnh hưởng đến ứng dụng cha.
- Keyboard-First UX và Spotlight Search: Phím tắt điều hướng nhanh qua CMD/CTRL+K và lướt danh sách thành phần bằng bàn phím tiện lợi, nâng cao trải nghiệm nhà phát triển.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- AST Auto-Scanning: Hiện tại các thông số phụ thuộc (metadata) của component đang được định nghĩa thủ công trong các tệp registry. Trong tương lai, cần xây dựng một script chạy tự động quét AST của mã nguồn để tự động trích xuất các mối quan hệ `usedIn`, `composedOf` thực tế.
```

---

### 4.3. Cải thiện chính

```text
- Tạo ra một nền tảng quản trị và khám phá kiến trúc giao diện nâng cao, giải quyết triệt để nguy cơ đổ vỡ runtime do component lỗi gây ra thông qua cơ chế Sandbox Error Boundary, đồng thời mở rộng mô hình phân cấp hệ thống thiết kế (Design System Governance) của CVerify.
```

---

### 4.4. Tổng kết project

```text
Trang quản lý Components System là một mảnh ghép quan trọng giúp đội ngũ lập trình viên CVerify nắm bắt trực quan cấu trúc giao diện lớn, kiểm soát độ phức tạp và mức độ tái sử dụng của các thành phần giao diện. Việc áp dụng các chuẩn thiết kế hiện đại của Linear/Vercel mang lại cảm giác cực kỳ cao cấp, tối ưu hóa tối đa hiệu suất làm việc của lập trình viên.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Tích hợp thư viện phân tích cú pháp mã nguồn (ví dụ: TS-Morph) ở backend để tự động cập nhật registry metadata của component mỗi khi có pull request mới.
2. Bổ sung bảng xếp hạng sức khỏe thành phần (Component Health Score) dựa trên các chỉ số: mức độ tái sử dụng, khả năng responsive, hỗ trợ accessibility (A11y), và độ phủ kiểm thử tự động (test coverage).
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-29    |
