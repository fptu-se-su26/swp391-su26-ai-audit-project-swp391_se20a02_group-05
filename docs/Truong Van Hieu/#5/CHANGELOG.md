# Changelog

## 1. Quy định ghi Changelog

File này ghi lại các thay đổi trong quá trình thực hiện task realtime notification với SignalR.

---

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - Hệ thống xác thực thông tin và quản lý hồ sơ năng lực dành cho Doanh nghiệp |
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV / Danh sách MSSV | DE190105 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | `https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05` |
| Ngày bắt đầu | 2026-06-10 |
| Ngày hoàn thành | 2026-06-20 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 10/06/2026 - 11/06/2026 | Nghiên cứu và chọn công nghệ realtime | Completed |
| Phase 02 | 12/06/2026 - 14/06/2026 | Thiết kế kiến trúc Hub và event pipeline | Completed |
| Phase 03 | 15/06/2026 - 18/06/2026 | Triển khai code backend và frontend | Completed |
| Phase 04 | 19/06/2026 - 20/06/2026 | Kiểm thử và tài liệu | Completed |

---

# [Phase 01] Nghiên cứu và chọn công nghệ

## Đã hoàn thành

- [x] Hỏi ChatGPT so sánh WebSocket, SSE và SignalR
- [x] Đọc tài liệu Microsoft ASP.NET Core SignalR
- [x] Quyết định chọn SignalR Hub
- [x] Phác thảo kiến trúc tổng quan phân hệ realtime

## AI có hỗ trợ không?

- [x] Có

```text
ChatGPT so sánh WebSocket/SSE/SignalR theo các tiêu chí: hướng giao tiếp, độ phức tạp,
khả năng scale, fallback. Chỉ dùng làm tài liệu tham khảo ra quyết định, không lấy code.
```

---

# [Phase 02] Thiết kế kiến trúc

## Đã hoàn thành

- [x] Thiết kế IAdminClient interface định nghĩa các method client-side
- [x] Thiết kế AdminHub kế thừa Hub<IAdminClient>
- [x] Thiết kế cơ chế emit event từ MonitoringAuditService → AdminHub
- [x] Thiết kế cơ chế xác thực JWT cho SignalR connection
- [x] Thiết kế Group "admins" để broadcast đến tất cả admin đang online

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thiết kế IAdminClient interface | Trương Văn Hiếu | `CVerify.Core/Modules/Admin/Hubs/IAdminClient.cs` | Interface definition |
| 2 | Thiết kế cơ chế JWT auth cho SignalR | Trương Văn Hiếu | `CVerify.Core/Program.cs` | Auth config |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Triển khai code

## Đã hoàn thành

- [x] Triển khai AdminHub với phương thức JoinAdminGroup, ReceiveMonitoringAlert
- [x] Cấu hình JWT auth middleware cho SignalR (OnMessageReceived từ query string)
- [x] Tích hợp IHubContext<AdminHub> vào MonitoringAuditService để emit event
- [x] Cấu hình Redis Backplane cho SignalR (scale horizontal)
- [x] Triển khai client Next.js với @microsoft/signalr
- [x] Triển khai AdminMonitoringProvider tự động kết nối khi admin login
- [x] Triển khai toast notification khi nhận ReceiveMonitoringAlert

## Danh sách lỗi đã xử lý

| STT | Lỗi phát hiện | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Client không nhận được thông báo khi deploy nhiều instance | Thiếu Redis Backplane | Thêm AddStackExchangeRedis cho SignalR | Fixed |
| 2 | JWT token không được nhận trong SignalR Hub | Hub không đọc Authorization header chuẩn | Cấu hình OnMessageReceived để đọc token từ query string `?access_token=` | Fixed |
| 3 | CORS error khi client Next.js kết nối Hub | Thiếu cấu hình CORS cho SignalR endpoint | Thêm WithOrigins cho SignalR endpoint riêng | Fixed |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 04] Kiểm thử và tài liệu

## Đã hoàn thành

- [x] Kiểm thử kết nối SignalR từ nhiều client đồng thời
- [x] Kiểm thử nhận thông báo realtime khi có monitoring event
- [x] Kiểm thử reconnect tự động khi mất kết nối
- [x] Hoàn thiện 4 file tài liệu kiểm toán AI

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 4. Tổng kết

### 4.1. Các chức năng đã hoàn thành

| STT | Chức năng | Trạng thái | Ghi chú |
|---:|---|---|---|
| 1 | AdminHub với Group-based broadcast | Completed | Group "admins" nhận toàn bộ admin online |
| 2 | JWT auth cho SignalR connection | Completed | Token từ query string |
| 3 | Redis Backplane cho horizontal scale | Completed | Multi-instance support |
| 4 | Client auto-connect và toast notification | Completed | AdminMonitoringProvider |

### 4.2. Tổng hợp AI hỗ trợ

| Hạng mục | AI có hỗ trợ không? | Mức độ | Ghi chú |
|---|---|---|---|
| Chọn công nghệ | Có | Ít | So sánh WebSocket/SSE/SignalR |
| Thiết kế | Không | - | Tự thiết kế |
| Coding | Không | - | Tự code 100% |
| Testing | Không | - | Tự kiểm thử |

---

## 5. Cam kết cập nhật Changelog

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 20/06/2026 |
