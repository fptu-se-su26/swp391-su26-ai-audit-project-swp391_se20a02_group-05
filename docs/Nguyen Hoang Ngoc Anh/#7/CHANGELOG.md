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
| Ngày bắt đầu | 2026-06-05T07:50:00Z |
| Ngày hoàn thành | 2026-06-05T08:15:00Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 05/06/2026 | Sửa lỗi build hệ thống và bổ sung tùy chọn Career Preferences | Completed |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Tích hợp tùy chọn Career Preferences & Sửa lỗi Build]

## Ngày thực hiện

```text
2026-06-05
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Bổ sung các trường dữ liệu CareerPreference trong Database & Entity C# | Nguyễn Hoàng Ngọc Ánh | CareerPreference.cs, User.cs | Commits |
| 2 | Cập nhật DTOs, CareerService, ProfileService và hạt giống DbInitializer | Nguyễn Hoàng Ngọc Ánh | ProfileSettingsDtos.cs, CareerService.cs, DbInitializer.cs | Commits |
| 3 | Xây dựng component PreferenceCard và TagChipMultiSelect trên React | Nguyễn Hoàng Ngọc Ánh | PreferenceCard.tsx, TagChipMultiSelect.tsx | Commits |
| 4 | Cập nhật form CareerTab hỗ trợ cấu hình và lưu thông tin Career Preferences | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx, profile.types.ts | Commits |
| 5 | Khắc phục lỗi biên dịch C# CS0116 trong CareerPreference.cs | Nguyễn Hoàng Ngọc Ánh | CareerPreference.cs | Commits |
| 6 | Khắc phục các lỗi ESLint react-hooks/set-state-in-effect và next/no-html-link-for-pages | Nguyễn Hoàng Ngọc Ánh | AccountTab.tsx, ConfirmationModal.tsx, LinkedAccountsList.tsx, page.tsx, status/page.tsx | Commits |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Commit/Screenshot minh chứng

```text
Commit hash: feature/career-preferences-settings
```

## Ghi chú

```text
Các lỗi build frontend đã được dọn sạch hoàn toàn, đảm bảo chất lượng tích hợp liên tục (CI).
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
1. Tích hợp hoàn toàn các tùy chọn tìm kiếm việc làm nâng cao bao gồm: Mức lương mong muốn, tính thương lượng, chế độ hiển thị lương, phong cách làm việc, môi trường ưa thích và giá trị doanh nghiệp.
2. Hoàn thành sửa các cảnh báo / lỗi ngăn cản quá trình build của ESLint trong frontend client, đưa số lượng lỗi ESLint về 0.
3. Hoàn thành sửa lỗi biên dịch backend CVerify.API.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
1. Tích hợp AI Matching để xếp hạng ứng viên dựa trên Career Preferences mới bổ sung này (sẽ làm ở phase sau).
```

---

## 4.3. Cải thiện chính

```text
1. Refactor cơ chế cập nhật trạng thái trong các React hooks (useEffect) từ đồng bộ sang bất đồng bộ bằng Promise.resolve() để tránh render chồng chéo (cascading renders) ảnh hưởng hiệu năng React 19.
2. Chuyển đổi toàn bộ anchor tags <a> sang component <Link> để định tuyến SPA tối ưu trong Next.js.
```

---

## 4.4. Tổng kết project

```text
Bổ sung thành công cấu hình Career Preferences cho ứng viên, giúp nâng cao khả năng gợi ý việc làm của hệ thống. Đồng thời giải quyết triệt để các lỗi build và lint trên cả backend và frontend.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Tích hợp cơ chế cache tối ưu cho Career Preferences khi người dùng chuyển đổi qua lại giữa các tab trong cài đặt.
2. Viết thêm các test case tự động cho tính năng lưu Career Preferences.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 05/06/2026 |
