# Prompt Log

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
| Ngày cập nhật gần nhất | 2026-06-20 |

---

## 3. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? |
|---:|---|---|---|---|---|---|
| 1 | 10/06/2026 | ChatGPT | So sánh WebSocket/SSE/SignalR để chọn công nghệ realtime | "So sánh WebSocket, SSE và SignalR theo các tiêu chí..." | Bảng so sánh đầy đủ, đề xuất SignalR cho .NET | Có (Chỉ quyết định công nghệ) |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 10/06/2026 |
| Công cụ AI | ChatGPT |
| Mục đích | So sánh WebSocket, SSE và SignalR để chọn công nghệ realtime phù hợp với CVerify |
| Phần việc liên quan | Thiết kế kiến trúc Realtime Notification |
| Mức độ sử dụng | Hỏi kiến thức |

#### 5.1. Prompt nguyên văn

```text
Tôi đang xây dựng tính năng thông báo realtime cho admin dashboard trên hệ thống CVerify
(.NET backend + Next.js frontend). Hãy so sánh WebSocket thuần, Server-Sent Events (SSE)
và SignalR Hub về các tiêu chí: hướng giao tiếp (uni/bi-directional), độ phức tạp triển
khai, khả năng scale horizontal, hỗ trợ fallback khi mạng yếu và phù hợp với từng kịch
bản sử dụng. Kịch bản của tôi là server push thông báo đến nhóm admin khi có sự kiện
monitoring từ microservice AI.
```

#### 5.2. Bối cảnh khi viết prompt

Cần chọn công nghệ realtime cho tính năng push thông báo monitoring từ CVerify.AI đến admin.
Yêu cầu: push theo nhóm (Group), có fallback, dễ tích hợp với .NET và Next.js.

#### 5.3. Kết quả AI trả về

AI cung cấp bảng so sánh:
- SSE: unidirectional (server→client), đơn giản, phù hợp push đơn chiều, không có Group tích
  hợp sẵn.
- WebSocket: bidirectional, phức tạp hơn, cần tự implement Group.
- SignalR: abstraction layer, tự chọn transport (WebSocket ưu tiên, fallback SSE/Long Polling),
  có Group tích hợp sẵn, reconnect tự động, phù hợp nhất với .NET ecosystem.

AI đề xuất SignalR cho kịch bản của tôi vì có Group, reconnect và phù hợp .NET.

#### 5.4. Kết quả đã áp dụng vào bài

Quyết định chọn SignalR Hub. Không dùng code mẫu từ AI.

#### 5.5. Phần đã chỉnh sửa/cải tiến

Tôi bổ sung thêm Redis Backplane (AI không đề cập) và thiết kế JWT auth middleware cho Hub
(AI chỉ đề cập chung chung).

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt tạo ra kết quả tốt

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
"So sánh WebSocket, SSE và SignalR về uni/bi-directional, độ phức tạp, scale horizontal,
fallback... Kịch bản của tôi là server push thông báo đến nhóm admin."
```

### 6.2. Vì sao prompt này quan trọng?

```text
Quyết định công nghệ realtime ảnh hưởng đến toàn bộ kiến trúc phân hệ notification. Chọn
sai ở đây sẽ mất nhiều công refactor sau.
```

### 6.3. Sinh viên kiểm tra kết quả như thế nào?

```text
Đọc tài liệu Microsoft ASP.NET Core SignalR, thử triển khai proof-of-concept và đo thực tế
latency, khả năng group broadcast trước khi áp dụng vào production.
```

---

## 7. Bài học về cách viết prompt

```text
1. Cung cấp bối cảnh stack công nghệ cụ thể (.NET + Next.js) để AI đưa ra gợi ý phù hợp.
2. Liệt kê tiêu chí đánh giá cụ thể thay vì hỏi chung "cái nào tốt hơn".
3. Mô tả kịch bản sử dụng thực tế để AI so sánh đúng trade-off.
```

---

## 8. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ |
|---|---:|---|
| Prompt so sánh công nghệ | 1 | "So sánh WebSocket, SSE và SignalR..." |
| Prompt giải thích kiến thức | 0 | |
| Prompt sinh code mẫu | 0 | |

---

## 9. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 20/06/2026 |
