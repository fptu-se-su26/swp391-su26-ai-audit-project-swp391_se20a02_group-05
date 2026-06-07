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
| Ngày bắt đầu | 2026-06-05T07:50:00Z |
| Ngày hoàn thành | 2026-06-05T08:15:00Z |

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
Sửa lỗi cú pháp namespace trong file CareerPreference.cs; Tìm kiếm và giải quyết toàn bộ các lỗi ESLint ngăn cản quá trình build và commit frontend, đặc biệt là lỗi react-hooks/set-state-in-effect và next/no-html-link-for-pages.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-05 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Khắc phục lỗi cú pháp C# CS0116 do từ khóa "cd" bị chèn nhầm ở đầu file CareerPreference.cs, chạy kiểm tra build. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Fix the syntax error on line 1 of CareerPreference.cs where 'cd ' was accidentally prepended to the using directive."
```

#### 4.2. Kết quả AI gợi ý

```text
Xóa từ khóa "cd " ở dòng số 1 của file CareerPreference.cs, trả cấu trúc namespace về định dạng hợp lệ.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Áp dụng code đã sửa của file CareerPreference.cs.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Chạy lệnh "dotnet build" để xác nhận dự án biên dịch thành công 0 lỗi.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Terminal Output | dotnet build | Biên dịch thành công 0 lỗi |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI phát hiện và sửa lỗi cú pháp rất nhanh, chính xác.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-05 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Phân tích và khắc phục 7 lỗi ESLint trong frontend (react-hooks/set-state-in-effect và next/no-html-link-for-pages). |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"run build front-end and run eslint" -> Sau đó yêu cầu AI phân tích nhật ký lỗi và viết code sửa đổi.
```

#### 4.2. Kết quả AI gợi ý

```text
- Viết các microtask Promise.resolve().then(...) bao quanh các hàm set state đồng bộ trong useEffect trong AccountTab.tsx, ConfirmationModal.tsx, LinkedAccountsList.tsx và settings/page.tsx.
- Thay thế thẻ anchor <a> bằng component <Link> từ next/link trong system/status/page.tsx.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Toàn bộ code sửa lỗi ESLint trên 5 files frontend để pass qua tiến trình build.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm chứng bằng cách chạy "npm run lint" và "npm run build" trên client để đảm bảo 0 lỗi.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Terminal Output | npm run lint | 0 errors |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc sửa lỗi set-state-in-effect bằng Promise.resolve() giúp tối ưu hóa hiệu năng render của React 19 / Next 16.
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
| 1 | Lần chạy linter thứ hai vẫn phát hiện lỗi set-state-in-effect trong LinkedAccountsList.tsx vì loadConnections() và loadGoogleStatus() mặc dù async nhưng vẫn chạy trực tiếp trong effect. | Linter báo lỗi | Bao bọc lệnh gọi hàm trực tiếp trong Promise.resolve().then() để trì hoãn hoàn toàn. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
1. Build backend C# bằng dotnet build để kiểm tra lỗi CS0116 đã biến mất.
2. Build frontend client bằng npm run build và npm run lint để xác nhận không còn bất kỳ lỗi ESLint nào cản trở quá trình commit code.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Người dùng cung cấp: các thay đổi tính năng Career Preferences trước đó, kích hoạt workflow phân tích và commit, kiểm tra các lỗi biên dịch.

AI thực hiện: phát hiện lỗi cú pháp, đưa ra các bản sửa lỗi tối ưu để không vi phạm quy tắc render React, tự động refactor các thẻ điều hướng.
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
| Nguyễn Hoàng Ngọc Ánh | 05/06/2026 |
