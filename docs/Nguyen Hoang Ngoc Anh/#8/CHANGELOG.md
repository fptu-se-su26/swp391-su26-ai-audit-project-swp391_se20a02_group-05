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
| Ngày bắt đầu | 2026-06-06T05:00:00Z |
| Ngày hoàn thành | 2026-06-06T06:00:00Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 05/06/2026 | Sửa lỗi build hệ thống và bổ sung tùy chọn Career Preferences | Completed |
| Phase 02 | 06/06/2026 | Mở rộng sở thích nghề nghiệp & Chuẩn hóa dữ liệu (Locations, Job Positions, Tag Validation & Public Profile Integration) | Completed |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Mở rộng sở thích nghề nghiệp & Chuẩn hóa dữ liệu]

## Ngày thực hiện

```text
2026-06-06
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Mở rộng cơ sở dữ liệu và Model CareerPreference hỗ trợ trường `DesiredJobPositions` và địa điểm làm việc ưa thích. | Nguyễn Hoàng Ngọc Ánh | CareerPreference.cs, ProfileSettingsDtos.cs | Commits |
| 2 | Triển khai cơ chế chuẩn hóa và validate danh sách tag `ValidateAndNormalizeTags` loại bỏ tag trùng lặp, giới hạn độ dài và số lượng tag tối đa là 20. | Nguyễn Hoàng Ngọc Ánh | CareerService.cs | Commits |
| 3 | Tích hợp kiểm thực logic nghiệp vụ cho mức lương (lương tối thiểu phải luôn nhỏ hơn hoặc bằng lương tối đa) trên Backend. | Nguyễn Hoàng Ngọc Ánh | CareerService.cs, ProfileService.cs | Commits |
| 4 | Cấu trúc lại Zod schema validation trên Frontend để đồng bộ hóa quy tắc kiểm thực lương min <= max và độ dài ghi chú 2000 ký tự. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx, profile.types.ts | Commits |
| 5 | Nâng cấp component `TagChipMultiSelect.tsx` hỗ trợ gõ nhanh bằng phím Enter/dấu phẩy, thêm nút Add và thực hiện lọc trùng lặp tag tại chỗ. | Nguyễn Hoàng Ngọc Ánh | TagChipMultiSelect.tsx | Commits |
| 6 | Bản đồ hóa lỗi (Error Mapping) trả về từ API Backend hiển thị tương ứng lên các trường nhập liệu của form. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx | Commits |
| 7 | Tích hợp các thẻ thông tin Career Preferences (Vị trí mong muốn, lương, tag kỹ năng/địa điểm, phong cách làm việc) hiển thị trực quan lên trang Public Profile ứng viên. | Nguyễn Hoàng Ngọc Ánh | page.tsx, PreferenceCard.tsx | Commits |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Commit/Screenshot minh chứng

```text
Commit hash: 500e104d20f2c1b0797424d29b7358fe3eb9712a (Add desired job positions & locations to career prefs)
```

## Ghi chú

```text
Cơ chế Validate & Normalization ở cả hai phía giúp nâng cao trải nghiệm nhập liệu của ứng viên và bảo vệ tính toàn vẹn dữ liệu cho hệ thống tuyển dụng.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
1. Mở rộng trọn vẹn dữ liệu sở thích nghề nghiệp của ứng viên với Vị trí mong muốn (Desired Job Positions) và Địa điểm ưa thích (Preferred Locations).
2. Chuẩn hóa hoàn hảo dữ liệu dạng Tag (Skills, Locations, Preferences) ngăn chặn dữ liệu rác (>100 ký tự hoặc >20 tags).
3. Thiết lập thông báo validation đồng bộ lỗi nghiệp vụ giữa Backend (C# Web API) và Frontend (Next.js React Hook Form).
4. Tích hợp hiển thị giao diện sở thích nghề nghiệp sinh động trên trang Hồ sơ công khai (Public Profile) của ứng viên giúp các nhà tuyển dụng dễ dàng đánh giá.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
1. Phát triển hệ thống gợi ý tin tuyển dụng tự động (Job Recommendations) dựa trên các tag vị trí và kỹ năng mới được chuẩn hóa này.
```

---

## 4.3. Cải thiện chính

```text
1. Áp dụng chuẩn hóa mảng tag bằng phương thức ValidateAndNormalizeTags tập trung giúp giảm thiểu mã lặp và tăng tính nhất quán khi xử lý Skills, Locations, và EmploymentPreferences.
2. Nâng cao trải nghiệm người dùng với bàn phím trong TagChipMultiSelect (sử dụng Enter/dấu phẩy để thêm nhanh tag) và nút Add trực quan cho người dùng dùng chuột.
```

---

## 4.4. Tổng kết project

```text
Phase 2 hoàn thành xuất sắc việc mở rộng và chuẩn hóa tính năng Career Preferences trên nền tảng CVerify, tạo bước đệm dữ liệu sạch cực kỳ quan trọng cho các giải pháp AI Matching và Gợi ý việc làm tự động sau này.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Xây dựng thêm API gợi ý từ khóa (Tag Auto-Complete) dựa trên dữ liệu tag phổ biến trong hệ thống.
2. Tối ưu hóa giao diện hiển thị danh sách tag trên thiết bị di động (Responsive UI).
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 06/06/2026 |
