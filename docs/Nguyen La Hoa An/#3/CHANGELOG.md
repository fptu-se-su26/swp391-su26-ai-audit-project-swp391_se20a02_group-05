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
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 |  |  | Not Started |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
Xây dựng giao diện cốt lõi (Core UI): Thiết kế và triển khai giao diện người dùng hiển thị danh sách thả xuống (Dropdown lists), các bảng dữ liệu động và khu vực tương tác chính của hệ thống CVerify.

Kết nối dữ liệu (Data Binding): Xử lý hiển thị dữ liệu trả về từ hệ thống Backend (Java Web/Servlet/JDBC) lên giao diện một cách chính xác.

Tối giản hóa cấu trúc: Loại bỏ các thành phần bổ trợ chưa cần thiết (như Layout chung Header/Footer, hệ thống Đăng nhập phức tạp) để tập trung tối đa nguồn lực vào việc hiển thị luồng tính năng cốt lõi (Core Functionality).
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
Hệ thống xác thực người dùng (Authentication): Phần đăng nhập, phân quyền người dùng (Role-based access control) tạm thời được lược bỏ hoặc cấu hình cứng (hardcode) để ưu tiên chạy thử nghiệm tính năng xác thực dữ liệu của CVerify.

Giao diện hoàn chỉnh (Full Layout): Header định hướng và Footer thông tin chưa được tích hợp vào các trang chức năng.
```

---

## 4.3. Cải thiện chính

```text
Tối ưu hóa Luồng code (Code Refactoring): Tái cấu trúc lại mã nguồn Frontend từ đầu theo yêu cầu khắt khe của dự án, đảm bảo mã nguồn sạch sẽ, không bị loãng bởi các thành phần dư thừa.

Trải nghiệm người dùng: Tối ưu hóa tốc độ tải và phản hồi của các dropdown list khi truy vấn dữ liệu từ database thông qua việc tinh chỉnh logic nhận phản hồi (response).
```

---

## 4.4. Tổng kết project

```text
Quá trình hỗ trợ phát triển Frontend cho CVerify tập trung mạnh mẽ vào tính thực dụng và hiệu năng. Thay vì sa đà vào thiết kế giao diện bên ngoài, AI đã đồng hành cùng bạn tập trung tối đa vào phần lõi: lấy dữ liệu từ database, xử lý luồng hiển thị, và tối ưu hóa trải nghiệm tương tác trực tiếp với tính năng "Verify". Dự án hiện tại đã đạt trạng thái sẵn sàng về mặt chức năng cốt lõi (Functional Readiness).
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
Tích hợp lại hệ thống Header, Footer và thiết kế giao diện chuẩn Responsive cho mọi thiết bị.

Xây dựng hoàn chỉnh mô-đun Login/Register bảo mật để chuẩn bị cho giai đoạn bàn giao hoặc chạy thực tế.

Kết nối Frontend với các công nghệ nâng cao ở Backend để xử lý các tác vụ kiểm thử/xác thực thời gian thực mượt mà hơn.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 11/6/2026 |
