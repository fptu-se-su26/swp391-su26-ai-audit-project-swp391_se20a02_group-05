# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Sinh mã nguồn, tái cấu trúc code, triển khai logic và tối ưu hóa chức năng cho phần Frontend của CVerify.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-11 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Triển khai cấu trúc giao diện  cho dự án CVerify, |
| Phần việc liên quan | Frontend |
| Mức độ sử dụng | Hỗ trợ một phần |

#### 4.1. Prompt đã sử dụng

```text
"Hãy viết mã nguồn Frontend (HTML/JS) để tạo các dropdown list động và bảng hiển thị dữ liệu lấy từ cơ sở dữ liệu. Yêu cầu: Xóa hết để làm lại từ đầu, tập trung hoàn toàn vào chức năng cốt lõi này, không bao gồm hệ thống đăng nhập (login) và không cần làm phần giao diện chung như header hay footer."
```

#### 4.2. Kết quả AI gợi ý

```text
AI đã cung cấp một đoạn mã Frontend sạch, tối giản đúng theo yêu cầu. Đoạn mã bao gồm cấu trúc các thẻ <select> cho dropdown, bảng mã HTML động để render dữ liệu, đi kèm các đoạn script xử lý nhận dữ liệu (fetch/response) để ánh xạ (data-binding) trực tiếp với Backend (Servlets/JDBC) mà không bị loãng bởi các thành phần giao diện khác.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sử dụng toàn bộ cấu trúc logic xử lý dropdown động, các thẻ hiển thị dữ liệu cốt lõi và các đoạn script kết nối dữ liệu.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tiến hành tích hợp đoạn mã vào cấu trúc thư mục của dự án Java Web, tinh chỉnh lại các đường dẫn API kết nối tới Servlet cho chuẩn xác, và tối ưu hóa cách hiển thị giao diện thả xuống (dropdown) để đảm bảo dữ liệu tải mượt mà, sẵn sàng cho việc kiểm thử tính năng xác thực của CVerify.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| hỗ trợ làm Frontend |   |   | x |   | thiết kế cấu trúc database, viết truy vấn SQL và cấu hình kết nối JDBC/Servlet phía Backend để tạo API trả dữ liệu cho Frontend. Đồng thời tự gỡ lỗi (debug) kết nối server để đảm bảo luồng dữ liệu chạy ổn định. |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI thường có xu hướng tự động sinh thêm các đoạn mã mẫu (boilerplate code) rườm rà không cần thiết như hệ thống layout chung (Header/Footer), các file CSS định dạng mặc định đồ sộ và form Login/Register, dù đã được yêu cầu tập trung vào tính năng cốt lõi. | Phát hiện trong quá trình kiểm tra mã nguồn (Code review) sau khi AI trả phản hồi. Nhận thấy tệp mã nguồn bị loãng, chứa nhiều thành phần giao diện thừa thãi không khớp với định hướng đơn giản hóa cấu trúc của dự án CVerify hiện tại. | Sử dụng kỹ thuật "Prompt phủ định" (Negative Prompting), ra lệnh một cách dứt khoát: "Xóa hết làm lại từ đầu, tuyệt đối không thêm login, header hay footer". Sau đó, tự tay lọc bỏ các đoạn CSS dư thừa để giữ lại đúng logic xử lý dropdown và bảng hiển thị dữ liệu cốt lõi. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Code Review (Đánh giá mã nguồn): Đọc và kiểm tra trực quan từng dòng code HTML/JS do AI sinh ra để đảm bảo không chứa các thành phần thừa như form đăng nhập, header, footer, đồng thời kiểm tra tính gọn gàng của cấu trúc thẻ dữ liệu.

Local Integration Testing (Kiểm thử tích hợp cục bộ): Nhúng trực tiếp đoạn mã Frontend vào project, chạy máy chủ Apache Tomcat cục bộ để kiểm tra xem các dropdown list có hiển thị đúng giao diện và kích hoạt đúng sự kiện (event) mong muốn hay không.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
AI đóng vai trò trợ lý tăng tốc độ viết mã (Code Generation), hỗ trợ dựng nhanh bộ khung cấu trúc Frontend tối giản gồm các thẻ dropdown động, bảng hiển thị dữ liệu và gợi ý các đoạn script xử lý nhận phản hồi (fetch/response) từ backend.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
|  |  |  | Có / Không |  |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 11/6/2026 |
