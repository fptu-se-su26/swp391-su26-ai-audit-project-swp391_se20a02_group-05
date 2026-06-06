# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu | 2026-06-06T05:00:00Z |
| Ngày hoàn thành | 2026-06-06T06:00:00Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Triển khai mở rộng cấu hình Career Preference (sở thích nghề nghiệp) hỗ trợ Vị trí công việc mong muốn (Desired Job Positions) và Địa điểm làm việc ưa thích; thiết lập cơ chế chuẩn hóa danh sách tag (Skills, Locations, Preferences) trên Backend; nâng cấp Zod schema và giao diện TagChipMultiSelect, CareerTab cùng trang Public Profile trên Frontend.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-06 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Thiết kế phương thức xử lý và chuẩn hóa tag `ValidateAndNormalizeTags` cũng như tích hợp kiểm tra logic nghiệp vụ cho mức lương (min <= max) trên Backend CVerify.Core. |
| Phần việc liên quan | Coding Backend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Write a backend helper function in C# CareerService to validate and normalize a list of string tags. Constraints: maximum 20 items, maximum 100 characters per tag, remove duplicates, ignore empty strings, and return a clean list. Also, add a check to make sure minimum salary is less than or equal to maximum salary."
```

#### 4.2. Kết quả AI gợi ý

```text
Tạo phương thức ValidateAndNormalizeTags trong CareerService.cs sử dụng LINQ để lọc bỏ khoảng trắng, chuyển đổi ký tự, loại bỏ trùng lặp và kiểm tra độ dài/số lượng tag. Ném ngoại lệ ValidationException nếu vi phạm ràng buộc số lượng (>20) hoặc độ dài (>100). Bổ sung kiểm tra logic lương min <= max.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Áp dụng phương thức ValidateAndNormalizeTags và logic kiểm tra lương vào các API Save/Update Career Preference trong CareerService.cs.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tự điều chỉnh việc tích hợp thông điệp lỗi nghiệp vụ vào Dictionary để trả về mã lỗi cụ thể (ValidationError) khớp với Error Normalizer của hệ thống CVerify.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | CareerService.cs | Tích hợp ValidateAndNormalizeTags |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Hàm helper được sinh tối ưu, xử lý chuỗi và kiểm thực ràng buộc chính xác, tránh các payload rác từ Client phá hỏng cơ sở dữ liệu.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-06 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Nâng cấp React Component `TagChipMultiSelect.tsx` hỗ trợ tự định nghĩa tag mới bằng phím Enter/dấu phẩy, thêm nút Add trực quan; xây dựng Zod schema kiểm thực điều kiện lương trên Frontend và hiển thị thông tin lên Public Profile page. |
| Phần việc liên quan | Coding Frontend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"How do I update TagChipMultiSelect.tsx to support custom tag entry on Enter or comma, add an explicit 'Add' button next to the input, and ensure it validates local tags before adding? Also help me write a Zod schema refinement for CareerTab to ensure min salary is <= max salary and notes is capped at 2000 characters."
```

#### 4.2. Kết quả AI gợi ý

```text
- Viết các hàm xử lý sự kiện onKeyDown (kiểm tra key === 'Enter' hoặc key === ',') và onClick của nút Add để thêm tag mới vào mảng trong TagChipMultiSelect.tsx.
- Cung cấp đoạn mã zod schema `.refine((data) => data.minSalary <= data.maxSalary, { message: 'Minimum salary cannot exceed maximum salary', path: ['minSalary'] })` kèm quy tắc giới hạn độ dài ghi chú.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Sử dụng mã nguồn xử lý phím tắt và hiển thị nút Add trong TagChipMultiSelect.tsx.
- Cấu trúc Zod schema cập nhật trong CareerTab.tsx.
- Cách ánh xạ dữ liệu DTO từ Backend hiển thị lên Public Profile `[username]/page.tsx`.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Chỉnh sửa giao diện CSS để các Tag Chip hiển thị đẹp mắt bằng HeroUI v3/Tailwind CSS v4, căn chỉnh vị trí nút Add đồng bộ với các trường input khác.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | CareerTab.tsx & TagChipMultiSelect.tsx | Cập nhật form cài đặt |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Component hoạt động rất mượt mà, hỗ trợ tốt trải nghiệm gõ phím của người dùng khi chọn tag.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Ý tưởng | x |   |   |   |   |
| Phát triển ý tưởng |   | x |   |   |   |
| Review kết quả |   |   | x |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI gợi ý ép kiểu lương trực tiếp bằng Number(val) trong Zod mà không xử lý trường hợp giá trị chuỗi trống, dẫn đến lỗi validation khi người dùng xóa trắng trường lương. | Chạy thử nghiệm phát sinh lỗi validation | Sửa đổi schema Zod sử dụng `.transform()` kết hợp xử lý chuỗi rỗng về `undefined` hoặc `null` trước khi chuyển sang số. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
1. Biên dịch thành công dự án CVerify.Core bằng dotnet build.
2. Kiểm tra lưu thành công cấu hình Career Preferences với thông tin Vị trí công việc mong muốn và các tags được chuẩn hóa sạch sẽ trên database.
3. Kiểm tra các trường hợp validation lỗi lương tối thiểu > lương tối đa và danh sách tag quá giới hạn đều hiển thị thông báo lỗi chính xác trên giao diện người dùng.
4. Truy cập Public Profile ứng viên hiển thị đầy đủ các thẻ thông tin sở thích nghề nghiệp vừa cài đặt.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Người dùng đóng góp: Lên ý tưởng mở rộng cấu hình Career Preferences, thiết lập các trường nghiệp vụ mới, kiểm thử tích hợp toàn diện và kiểm định lỗi trên giao diện.

AI thực hiện: Sinh mã xử lý logic chuẩn hóa tag phức tạp ở Backend, xây dựng các biểu thức Zod schema tương thích và tối ưu hóa phản ứng sự kiện nút nhấn trên Frontend.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trương Văn Hiếu | DE190105 |  | Không |   |
| Đoàn Thế Lực | DE200523 |  | Không |   |
| Nguyễn La Hòa An | DE201043 |  | Không |   |
| Trần Nhất Long | DE200160  |  | Không |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 06/06/2026 |
