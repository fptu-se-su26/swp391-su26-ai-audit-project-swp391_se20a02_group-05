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
| Ngày bắt đầu | 2026-06-09T01:05:00Z |
| Ngày hoàn thành | 2026-06-09T01:45:00Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 05/06/2026 | Sửa lỗi build hệ thống và bổ sung tùy chọn Career Preferences | Completed |
| Phase 02 | 06/06/2026 | Mở rộng sở thích nghề nghiệp & Chuẩn hóa dữ liệu | Completed |
| Phase 03 | 09/06/2026 | Tinh chỉnh UI/UX, Bố cục lưới Responsive, Tiền tệ động và Thanh thay đổi chưa lưu | Completed |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Tinh chỉnh UI/UX, Lưới Responsive, Tiền tệ động và Thanh thay đổi chưa lưu]

## Ngày thực hiện

```text
2026-06-09
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Sắp xếp Discoverability và AI-Inferred Career Trajectory song song bằng lưới responsive `grid grid-cols-1 md:grid-cols-2`. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx | Commits |
| 2 | Sắp xếp lại thứ tự của các thẻ preferences còn lại theo đúng chuẩn yêu cầu. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx | Commits |
| 3 | Tích hợp thành công InputGroup cho mức lương min/max hiển thị động ký hiệu tiền tệ ($ / ₫) và mã tiền (USD / VND) tùy theo Expected Currency được chọn. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx | Commits |
| 4 | Cải tiến hành vi thanh `UnsavedChangesBar`: tự động hiển thị khi form có thay đổi và ẩn đi khi nhấn Save/Reset; gỡ bỏ các modal xác nhận rườm rà. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx | Commits |
| 5 | Tích hợp kiểm thực form validation (React Hook Form `trigger()`) chạy trực tiếp trước khi gửi dữ liệu lưu trong handleSaveChanges. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx | Commits |
| 6 | Cập nhật hàm `handleReset` khôi phục chính xác các trường dữ liệu mặc định đã lấy về từ database thay vì đưa form về rỗng. | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx | Commits |
| 7 | Đồng bộ kích thước nút Add (size="md"), căn lề `max-w-sm` và tùy biến trạng thái disabled (vẫn giữ nguyên màu trắng, không bị xám hóa). | Nguyễn Hoàng Ngọc Ánh | CareerTab.tsx, TagChipMultiSelect.tsx | Commits |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Commit/Screenshot minh chứng

```text
Commit hash: 4595aeecd937c44a7290475f1d776e878fbbe780 (Refactor CareerTab UI and improve inputs)
```

## Ghi chú

```text
Việc loại bỏ các modal xác nhận giúp cải thiện đáng kể tốc độ lưu thông tin của người dùng trong khi UnsavedChangesBar vẫn đảm bảo ngăn chặn việc mất mát dữ liệu do điều hướng nhầm.
```

---

## 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
1. Hoàn thiện layout lưới responsive 2 cột chuyên nghiệp cho các thẻ AI phân tích nghề nghiệp ở đầu trang.
2. Thiết lập cấu hình hiển thị tiền tệ động thông minh giúp người dùng nhập mức lương rõ ràng và chính xác theo USD/VND.
3. Đồng bộ hóa kích thước nút thêm tag (Skills & Locations) trực quan và đồng điệu với TagChipMultiSelect.
4. Quản lý trạng thái form thay đổi chưa lưu an toàn bằng thanh sticky UnsavedChangesBar tích hợp trực tiếp validation và reset dữ liệu thực tế.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
1. Chưa áp dụng cơ chế định dạng số (Number formatting) ngăn chặn người dùng nhập thủ công các ký tự đặc biệt như dấu phẩy vào trường lương.
```

---

## 4.3. Cải thiện chính

```text
1. Đè các selector disabled mặc định của thư viện bằng Tailwind custom state (`data-[disabled=true]`) giải quyết triệt để lỗi ghi đè màu sắc.
2. Trực tiếp thực thi logic validation bằng API react-hook-form tích hợp trước khi gửi yêu cầu lưu giúp tránh lãng phí request lên API backend.
```

---

## 4.4. Tổng kết project

```text
Phase 3 tập trung cao vào cải thiện tính trực quan và tối ưu trải nghiệm tương tác của phần Cài đặt nghề nghiệp trên CVerify, mang lại giao diện phản ứng nhanh nhạy, hiện đại và chuẩn mực.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Nghiên cứu cơ chế tự động chuyển đổi tỷ giá hiển thị (VND sang USD và ngược lại) chỉ mang tính chất tham khảo khi người dùng click xem trước.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 09/06/2026 |
