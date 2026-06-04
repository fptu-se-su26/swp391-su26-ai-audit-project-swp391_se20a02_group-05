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
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-06-04 |
| Ngày hoàn thành | 2026-06-04 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 — Bugfix: Work Experience Form | 2026-06-04 | Fix dropdown binding và Zod schema validation cho form work experience trong Settings | Completed |

---

# [Phase 01] Bugfix: Work Experience Form Dropdown

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-04
- **Mô tả giai đoạn:** Sửa lỗi dropdown binding và validation schema cho trường experienceCategory và employmentType trong form work experience của trang Settings.
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Refactor `workExperienceEntrySchema`: thay `z.coerce.number().min(1)` bằng `z.union([z.undefined(), z.number()])` + `.refine()` guards để cho phép `undefined` là trạng thái xóa chọn | Trần Nhất Long | `settings/components/types.ts` | Commit f9dc89830 |
| 2 | Fix `SelectDropdown` binding cho Experience Category và Employment Type: thêm NaN guard khi hiển thị value, convert string → number (hoặc undefined) trong onChange | Trần Nhất Long | `settings/components/ExperienceAchievementsSection.tsx` | Commit f9dc89830 |
| 3 | Xóa redundant `Number()` cast trong submit handler vì field đã lưu kiểu number trực tiếp | Trần Nhất Long | `settings/components/PersonalInfoTab.tsx` | Commit f9dc89830 |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | f9dc89830 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/f9dc89830 |
| Pull Request | PR #54 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/pull/54 |

## Ghi chú

```text
Lỗi gốc: z.coerce.number() chuyển "" (empty string từ dropdown bị clear) thành 0,
khiến .min(1) pass nhưng API nhận value 0 — không hợp lệ về mặt nghiệp vụ.

Fix: Dùng z.union([z.undefined(), z.number()]) để field chứa number hoặc undefined.
Validation "bắt buộc chọn" chuyển sang .refine() ở cấp schema với NaN guard rõ ràng.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
- Work experience form: dropdown binding cho experienceCategory và employmentType
  hoạt động đúng khi clear selection (trả về undefined thay vì 0/NaN).
- Validation error message lấy từ react-hook-form error object thay vì hardcode.
- Submit handler gọn hơn, không còn redundant Number() cast.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
- Chưa viết test case tự động cho validation schema.
- Chưa có unit test cho SelectDropdown binding logic.
```

---

## 4.3. Cải thiện chính

```text
Tách validation "required" ra khỏi field-level schema vào schema-level .refine()
giúp error path rõ ràng hơn và tránh false-positive khi field là optional trong
trạng thái chưa chọn (undefined).
```

---

## 4.4. Tổng kết project

```text
Phiên làm việc ngắn (bugfix) nhưng fix đúng root cause. Commit clean, PR có đủ
thông tin cho reviewer.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
- Viết integration test cho work experience form submission.
- Xem xét tạo custom SelectDropdown wrapper với type-safe onChange<T extends number>.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trần Nhất Long | 04/06/2026 |
