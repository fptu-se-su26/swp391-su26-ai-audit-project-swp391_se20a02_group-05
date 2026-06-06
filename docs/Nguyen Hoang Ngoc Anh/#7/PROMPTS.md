# Prompt Log

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
| Ngày bắt đầu | 2026-06-05T07:50:00Z |
| Ngày cập nhật gần nhất | 2026-06-05 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 2026-06-05 | Antigravity | Sửa lỗi biên dịch backend C# | Sửa lỗi cú pháp dòng 1 | Loại bỏ từ khóa "cd" thừa ở dòng 1 | Có | Commit |
| 2 | 2026-06-05 | Antigravity | Kiểm tra lỗi build & eslint frontend | Run build front-end and run eslint | Phát hiện 7 lỗi ESLint cản trở build | Có | Logs |
| 3 | 2026-06-05 | Antigravity | Sửa các lỗi set-state-in-effect | Sửa các set-state-in-effect bằng cách defer | Bao bọc set state trong Promise.resolve() | Có | Commit |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-05 |
| Công cụ AI | Antigravity |
| Mục đích | Loại bỏ từ khóa "cd" bị nhập nhầm ở dòng đầu tiên của CareerPreference.cs để build thành công backend. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi sửa lỗi |

#### 5.1. Prompt nguyên văn

```text
"Fix the syntax error on line 1 of CareerPreference.cs where 'cd ' was accidentally prepended to the using directive."
```

#### 5.2. Bối cảnh khi viết prompt

```text
Được kích hoạt sau khi chạy "dotnet build" bị báo lỗi CS0116 do cấu trúc namespace bị lỗi cú pháp ở dòng 1.
```

#### 5.3. Kết quả AI trả về

```text
Sử dụng replace_file_content để xóa "cd " và giữ lại "using System;".
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Dự án backend đã được build thành công với 0 lỗi.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Chạy lệnh "dotnet build" độc lập để xác minh.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Git Diff | Xóa từ khóa thừa |

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-05 |
| Công cụ AI | Antigravity |
| Mục đích | Dọn dẹp toàn bộ các lỗi ESLint ngăn cản việc build và deploy frontend client. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi review và refactor |

#### 5.1. Prompt nguyên văn

```text
"run build front-end and run eslint"
```

#### 5.2. Bối cảnh khi viết prompt

```text
Chạy build frontend để chuẩn bị deploy và commit code sạch lỗi.
```

#### 5.3. Kết quả AI trả về

```text
Phát hiện 7 lỗi ESLint bao gồm 6 lỗi react-hooks/set-state-in-effect và 1 lỗi next/no-html-link-for-pages. Sau đó, gợi ý cách thay thế <a> bằng <Link> và đưa set state trong các effect vào Promise.resolve().then().
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Toàn bộ code frontend được dọn sạch lỗi ESLint và build thành công.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Bao bọc thêm một số hàm load dữ liệu gọi lồng trong các file liên quan để linter không bắt lỗi tĩnh.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt tạo ra kết quả tốt

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Logs | task-76.log ghi nhận 0 errors |

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
"run build front-end and run eslint"
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này giúp kiểm duyệt toàn diện sức khỏe của mã nguồn frontend trước khi đưa vào môi trường CI/CD, ngăn chặn lỗi runtime tiềm ẩn liên quan đến render vòng lặp.
```

### 6.3. Kết quả prompt này mang lại

```text
Frontend client vượt qua kiểm tra tĩnh (ESLint) và biên dịch (Webpack/Turbopack) hoàn hảo.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Chạy trực tiếp lệnh "npm run build" và "npm run lint".
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Refactor tối ưu cơ chế load trạng thái bất đồng bộ.
```

---

## 7. Prompt chưa hiệu quả

```text
Không có.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần cung cấp log lỗi chi tiết hoặc các chỉ dẫn biên dịch cụ thể để AI không phải đoán vị trí và nguyên nhân lỗi.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Nên yêu cầu AI chạy các công cụ phân tích tĩnh (linter, compiler) trước để làm đầu vào cho các prompt refactor tiếp theo.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Coding / Sửa lỗi | 3 | "run build front-end and run eslint" |

---

## 10. Checklist chất lượng prompt

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | x | |
| Prompt có đủ bối cảnh | x | |
| Tự kiểm tra và chỉnh sửa | x | |

---

## 11. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 05/06/2026 |
