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
| Phase 02 | 2026-05-25 ~ 2026-05-28 | Sửa lỗi cấu trúc và tối ưu hóa sơ đồ Use Case tổng thể hệ thống CVerify (83 Use Cases, 15 nhóm chức năng) trên công cụ Draw.io. | Completed |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Phase 02] 

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-05-25 ~ 2026-05-28
- **Mô tả giai đoạn:** Sửa lỗi cấu trúc và tối ưu hóa sơ đồ Use Case tổng thể hệ thống CVerify (83 Use Cases, 15 nhóm chức năng) trên công cụ Draw.io.
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thiết kế, chuẩn hóa và cấu trúc lại sơ đồ Use Case tổng thể cho hệ thống CVerify gồm 83 Use Cases chia làm 15 phân vùng chức năng. | Trương Văn Hiếu |   | https://gemini.google.com/share/76578921ddeb |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Hỗ trợ tạo file XML để tạo sơ đồ Usecase Diagram
```

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 10:57:37 PM | image.png |

## Ghi chú

```text
- Trạng thái bàn giao: Hoàn thành 100% phần sơ đồ Use Case Phase 02 (Khớp chính xác với danh sách 83 UDS).
- Các cụm Actor hệ thống đã được cấu trúc và liên kết: Guest, User, Business User, Admin, Super Admin, Developer, AI Microservice, System.
- Giải pháp kỹ thuật áp dụng: Tái cấu trúc file XML sạch, loại bỏ các thẻ hình học bị khuyết, chia nhóm trực quan theo phân vùng chức năng giúp hệ thống không bị crash và tăng tốc độ kết xuất đồ họa trên trình duyệt.
- Hướng phát triển kế tiếp: Chuyển giao sang thiết kế cấu trúc thực thể Database (Demo Database) dựa trên các Use Case cốt lõi đã được định hình thành công.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
Hệ thống Xác thực Cá nhân & Doanh nghiệp: Hoàn thiện luồng đăng nhập (Email/Password, Google SSO), xác minh OTP qua Email, khôi phục mật khẩu, quản lý phiên làm việc đa tab và Onboarding thiết lập Workspace cho doanh nghiệp (UC-01 đến UC-24).
Phân hệ AI Chat (Streaming): Triển khai thành công tính năng gửi tin nhắn dạng streaming (SSE), quản lý lịch sử/hủy cuộc trò chuyện, gợi ý câu hỏi thông minh (UC-25 đến UC-34).
Hệ thống Quản trị & Phân quyền nâng cao: Hoàn thành giao diện Dashboard phân nhánh (User, Admin, Business); phân hệ quản lý người dùng, vai trò (Roles) và quyền hạn (Permissions) kèm cơ chế xử lý xung đột đồng thời (Concurrency conflict) (UC-35 đến UC-57).
Giám sát & An ninh hệ thống: Đưa vào vận hành cụm Audit Logs tra cứu vết, Email Test (SMTP), kiểm tra sức khỏe hệ thống (Health Check DB, Redis), cảnh báo đánh cắp Token (Token theft detection) và tự động khóa tài khoản (Account lockout) (UC-58 đến UC-83).
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
Tích hợp cổng thanh toán trực tuyến (Payment Gateway): Phần mở rộng xử lý hóa đơn/gói dịch vụ nâng cao cho Business User tạm thời để lại cấu trúc chờ, chưa liên kết API thực tế.
Báo cáo thống kê chuyên sâu (Advanced Analytics): Các biểu đồ trực quan hóa dữ liệu log theo thời gian thực (Real-time charts) trên Dashboard Admin mới chỉ dừng lại ở mức render giao diện mẫu (Mock data).
```

---

## 4.3. Cải thiện chính

```text
Tối ưu hóa cấu trúc sơ đồ hệ thống: Khắc phục triệt để lỗi phân tích cú pháp mã nguồn hình học XML (mxGeometry) trên nền tảng Draw.io, tái cấu trúc thành công layout ma trận trực quan cho toàn bộ 83 Use Cases và 15 phân vùng chức năng giúp dễ dàng bảo trì dữ liệu.
Cải tiến bảo mật và trải nghiệm người dùng: Đồng bộ trạng thái đăng nhập/đăng xuất (Login/Logout Sync) thời gian thực trên đa tab trình duyệt thông qua cơ chế bắt sự kiện LocalStorage/Session.
Tích hợp luồng dữ liệu AI mượt mà: Ổn định hóa kết nối giữa ứng dụng chính và AI Microservice bằng cơ chế xác thực bảo mật HMAC, giảm thiểu latency khi streaming dữ liệu chat.
```

---

## 4.4. Tổng kết project

```text
Dự án CVerify đã hoàn thành thiết kế hệ thống toàn diện với quy mô 83 Use Cases chính thức chia làm 15 nhóm chức năng core. Hệ thống đảm bảo bao phủ đầy đủ tất cả các tác vụ từ nhóm người dùng phổ thông (Guest, User), người dùng doanh nghiệp (Business User) cho đến các cấp quản trị cao cấp (Admin, Super Admin, Developer) và các thực thể tự động (System, AI Microservice). Toàn bộ tài liệu đặc tả (UDS), sơ đồ luồng tương tác quan hệ <<include>>/<<extend>> đã được chuẩn hóa và đồng bộ 100% với tiến độ báo cáo kỹ thuật.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
Mở rộng khả năng AI: Nâng cấp AI Microservice để hỗ trợ xử lý ngữ cảnh dài hơn (Long-context window) và tối ưu hóa bộ nhớ Cache để tăng tốc độ phản hồi tin nhắn streaming.
Triển khai Microservices hóa: Tách biệt hoàn toàn module Quản lý người dùng (Auth Service) và module Giám sát (Monitoring Service) thành các dịch vụ độc lập nhằm tăng khả năng chịu tải và mở rộng hệ thống.
Tối ưu hệ thống lưu trữ: Áp dụng phân vùng Database (Partitioning) đối với bảng Audit Logs và lịch sử Chat để đảm bảo hiệu năng khi dữ liệu phình to.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 1/6/2026 |
