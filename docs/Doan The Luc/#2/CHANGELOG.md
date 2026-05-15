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
| Tên bài tập / Project | TripGenie - Backend |
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
| Phase 01 | 2026-05-15 | Ghi chú quan trọng cho bản Refactor này:
- Setup: Mọi người cần cài đặt/chạy Docker cho Redis và PostgreSQL trước khi khởi chạy ứng dụng. Kiểm tra kỹ EnvValidator để đảm bảo không thiếu biến môi trường.
- Kiến trúc: Hệ thống phân quyền hiện tại dựa trên Policy-based kết hợp với các Permission nguyên tử (Atomic Permissions). Khi tạo Controller mới, có thể sử dụng Attribute [HasPermission(Permission.YourType)].
- Bảo mật: Đã test kỹ luồng Refresh Token Rotation. Lưu ý rằng Access Token sẽ không được lưu ở LocalStorage mà sẽ lấy từ kết quả Login để đảm bảo an toàn.
- Dữ liệu: Các bảng Identity được map thủ công qua Dapper để đạt tốc độ phản hồi nhanh nhất cho middleware check quyền. | In Progress |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Phase 01] 

## Ngày thực hiện

```text
2026-05-15
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Implement JWT Auth & Refresh Token Rotation | Đoàn Thế Lực |   | Auth/Token Service |
| 2 | Develop Account & Permission Services | Đoàn Thế Lực |   | Account lockout logic |
| 3 | Setup Redis Cache & Dapper Identity Repo | Đoàn Thế Lực |   | CacheService/Repo |
| 4 | Configure PostgreSQL Mappings & AppDbContext | Đoàn Thế Lực |   | DB Migrations/Indexes |
| 5 | Refactor API Structure & Env Validation | Đoàn Thế Lực |   | Extensions/Program.cs |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 03:06:08 | image.png |
| Commit/PR | Link commit of auth system | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/f9f2a905c1bdc82519fa5313d84a11d685954d03 |

## Ghi chú

```text
Ghi chú quan trọng cho bản Refactor này:
- Setup: Mọi người cần cài đặt/chạy Docker cho Redis và PostgreSQL trước khi khởi chạy ứng dụng. Kiểm tra kỹ EnvValidator để đảm bảo không thiếu biến môi trường.
- Kiến trúc: Hệ thống phân quyền hiện tại dựa trên Policy-based kết hợp với các Permission nguyên tử (Atomic Permissions). Khi tạo Controller mới, có thể sử dụng Attribute [HasPermission(Permission.YourType)].
- Bảo mật: Đã test kỹ luồng Refresh Token Rotation. Lưu ý rằng Access Token sẽ không được lưu ở LocalStorage mà sẽ lấy từ kết quả Login để đảm bảo an toàn.
- Dữ liệu: Các bảng Identity được map thủ công qua Dapper để đạt tốc độ phản hồi nhanh nhất cho middleware check quyền.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
- Triển khai hệ thống xác thực bảo mật đa lớp sử dụng JWT và cơ chế Refresh Token.
- Tích hợp lưu trữ Refresh Token qua HttpOnly Cookie để ngăn chặn các lỗ hổng bảo mật phía Client.
- Xây dựng hệ thống phân quyền chi tiết (Permission-based Authorization) với Custom Policy Provider.
- Thiết lập bộ kiểm tra và xác thực biến môi trường (Environment Validation) đảm bảo tính nhất quán khi triển khai.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
- Hệ thống thông báo tự động qua Email/SMS khi tài khoản bị khóa do đăng nhập sai nhiều lần.
- Giao diện người dùng (Frontend UI) cho việc quản lý phân quyền và phân vai trò trực quan.
- Tích hợp xác thực hai lớp (2FA) để tăng cường bảo mật cho tài khoản quản trị.
```

---

## 4.3. Cải thiện chính

```text
- Kiến trúc: Tái cấu trúc file Program.cs sang dạng Extension methods, giúp code module hóa và dễ bảo trì.
- Hiệu năng: Tối ưu hóa tốc độ truy vấn quyền hạn bằng cách sử dụng Dapper thay vì ORM truyền thống và kết hợp Redis Cache.
- Database: Chuyển đổi sang kiến trúc Identity Repository tùy chỉnh trên PostgreSQL với đầy đủ chỉ mục (indexes).
```

---

## 4.4. Tổng kết project

```text
Đợt cập nhật này giúp hoàn thiện nền tảng bảo mật của dự án với JWT, Redis và hệ thống phân quyền linh hoạt. Đồng thời, cấu trúc mã nguồn cũng được tối ưu để dễ mở rộng và phù hợp cho các giai đoạn phát triển lớn hơn sau này.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
- Triển khai Unit Test và Integration Test cho toàn bộ luồng Auth để đảm bảo độ tin cậy tuyệt đối.
- Nghiên cứu tích hợp OAuth2 (Google/GitHub Login) để đa dạng hóa phương thức đăng nhập.
- Tối ưu hóa việc ghi log bảo mật (Audit Logs) để theo dõi các hành vi bất thường của người dùng theo thời gian thực.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 16/5/2026 |
