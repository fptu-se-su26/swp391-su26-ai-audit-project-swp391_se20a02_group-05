# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu | 2026-06-10 |
| Ngày hoàn thành | 2026-06-20 |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu của tôi là tìm hiểu và so sánh các công nghệ real-time: WebSocket, Server-Sent Events
(SSE) và SignalR Hub để quyết định công nghệ nào phù hợp với tính năng thông báo realtime trên
admin dashboard của CVerify. Tôi không dùng AI để viết code — toàn bộ implementation do tôi
tự thực hiện sau khi đã hiểu ưu/nhược điểm của từng công nghệ.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 10/06/2026 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | So sánh WebSocket, SSE và SignalR để chọn công nghệ realtime cho CVerify |
| Phần việc liên quan | Thiết kế kiến trúc Realtime Notification |
| Mức độ sử dụng | Hỏi kiến thức, không lấy code |

#### 4.1. Prompt đã sử dụng

```text
Tôi đang xây dựng tính năng thông báo realtime cho admin dashboard trên hệ thống CVerify
(.NET backend + Next.js frontend). Hãy so sánh WebSocket thuần, Server-Sent Events (SSE)
và SignalR Hub về các tiêu chí: hướng giao tiếp (uni/bi-directional), độ phức tạp triển
khai, khả năng scale horizontal, hỗ trợ fallback khi mạng yếu và phù hợp với từng kịch
bản sử dụng.
```

#### 4.2. Kết quả AI gợi ý

AI so sánh rõ ràng: SSE là unidirectional (server → client) phù hợp thông báo đơn chiều,
WebSocket là bidirectional nhưng phức tạp hơn, còn SignalR Hub là lớp abstraction bên trên
WebSocket/SSE/Long Polling tự động chọn transport tốt nhất và có sẵn các tính năng như
Group, reconnect tự động, phù hợp với .NET ecosystem.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Chỉ tiếp thu phần so sánh trade-off để đưa ra quyết định: chọn SignalR Hub vì đã có trong
.NET stack, hỗ trợ Group (gửi thông báo đến nhóm admin), tự động fallback và reconnect — phù
hợp hơn với kiến trúc CVerify so với SSE thuần hay WebSocket tự triển khai.

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi bổ sung cơ chế xác thực JWT cho SignalR Hub (AI không đề cập chi tiết), thiết kế tên
Group theo role ("admins") và thiết kế cơ chế emit từ MonitoringAuditService khi có sự kiện.
Ngoài ra tôi tự thiết kế AdminHub kế thừa Hub<IAdminClient> thay vì Hub thô.

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| File liên quan | `docs/Truong Van Hieu/#5/PROMPTS.md` |
| Kết quả áp dụng | AdminHub với SignalR, JWT auth middleware, Group "admins" |

#### 4.6. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: ChatGPT giải thích đầy đủ và có bảng so sánh trực quan, giúp tôi ra quyết
  định công nghệ nhanh chóng mà không mất nhiều thời gian đọc docs của từng công nghệ.
- Bài học: Sau khi chọn SignalR, việc triển khai thực tế (JWT auth trong Hub, scale với Redis
  backplane, xử lý reconnect client) phức tạp hơn nhiều so với lý thuyết AI mô tả.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Phân tích yêu cầu | [x] |  |  |  | Tự phân tích từ đặc tả |
| Thiết kế kiến trúc hệ thống |  | [x] |  |  | Tham khảo so sánh công nghệ từ AI |
| Code backend | [x] |  |  |  | Tự code AdminHub, SignalR config 100% |
| Code frontend | [x] |  |  |  | Tự code client kết nối SignalR |
| Debug lỗi | [x] |  |  |  | Tự debug reconnect issue |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI không đề cập đến việc cần Redis Backplane khi scale SignalR horizontal trên nhiều instance. | Phát hiện khi thử deploy lên nhiều pod — thông báo không broadcast được. | Tích hợp SignalR Redis Backplane với AddStackExchangeRedis. |
| 2 | AI không đề cập chi tiết cách xác thực JWT trong SignalR Hub (Hub không có HTTP context thông thường). | Phát hiện khi thử kết nối từ client với JWT bearer token. | Cấu hình AddAuthentication với OnMessageReceived để đọc token từ query string. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc tài liệu chính thức của Microsoft về ASP.NET Core SignalR để xác nhận so sánh AI đúng.
2. Đọc so sánh SignalR vs SSE trên blog Microsoft Developer.
3. Thực hành triển khai thực tế và kiểm thử với nhiều client đồng thời kết nối.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Tự đặt câu hỏi so sánh công nghệ và đưa ra quyết định kiến trúc.
- Tự thiết kế AdminHub<IAdminClient> với typed clients.
- Tự triển khai JWT auth middleware cho SignalR.
- Tự thiết kế MonitoringAuditService emit event đến Hub.
- Tự viết client-side connection code trong Next.js với @microsoft/signalr.
- Tự debug và fix các vấn đề reconnect và CORS trong SignalR.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trương Văn Hiếu | DE190105 | Thiết kế và triển khai phân hệ realtime với SignalR | Có (Chỉ hỏi kiến thức) | AdminHub, SignalR config, client code |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Thiết kế Use Case thông báo realtime | Có | Bản vẽ UC |
| Đoàn Thế Lực | DE200523 | Tích hợp realtime events vào backend services | Có | Backend event emit |
| Nguyễn La Hòa An | DE201043 | Viết tài liệu SRS phân hệ thông báo | Có | Tài liệu SRS |
| Trần Nhất Long | DE200160 | Viết test cho Hub và emit service | Có | Test cases |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI cung cấp bảng so sánh có cấu trúc giữa ba công nghệ realtime, giúp tôi ra quyết định
chọn SignalR trong vài phút thay vì mất nhiều giờ đọc docs của từng công nghệ.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Tôi không sử dụng code mẫu nếu AI có cung cấp vì cần tích hợp với cấu trúc Clean Architecture
của CVerify Core và cần thêm JWT auth, Redis backplane mà AI không đề cập.
```

### 9.3. Sau bài này học được gì về môn học?

```text
Lựa chọn công nghệ realtime phải dựa trên trade-off thực tế của hệ thống (scalability, auth,
ecosystem) chứ không phải chọn công nghệ "tốt nhất" về lý thuyết.
```

### 9.4. Sau bài này học được gì về sử dụng AI có trách nhiệm?

```text
AI giỏi so sánh tổng quan nhưng không biết chi tiết triển khai thực tế của hệ thống cụ thể.
Cần tự đọc docs chính thức và tự thử nghiệm.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 20/06/2026 |
