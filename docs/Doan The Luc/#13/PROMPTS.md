# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Repository Analysis Engine with Real-time SSE Progress Streaming             |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-06-05T19:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-06-06                                                                             |

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

| STT | Ngày       | Công cụ AI  | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
| --: | ---------- | ----------- | -------- | -------------- | ------------- | ------------------------- | ---------- |
|   1 | 2026-06-05 | Antigravity | Giải quyết lỗi Windows Subprocess & Claude JSON | Resolve the NotImplementedError when running asyncio.create_subprocess_exec on Windows... | Chuyển sang dùng subprocess.run trong asyncio.to_thread và dùng giải thuật brace-matching lọc JSON. | Có | GitHub Commit |
|   2 | 2026-06-05 | Antigravity | Đồng bộ trạng thái tiến trình SSE lên Next.js UI | Build a React Hook or state controller in Next.js to stream progress events... | Xây dựng EventSource listener ở frontend, map đúng trạng thái phân tích để UI không bị reset. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1 (FastAPI Subprocess Execution & Claude Resiliency)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-05                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Khắc phục lỗi tương thích môi trường Windows khi clone Git và lỗi phân tách cấu trúc JSON đầu ra từ Claude.                                            |
| Phần việc liên quan | AI FastAPI Microservice / subprocess / Git integration / Claude API client                                                                              |
| Mức độ sử dụng      | Hỗ trợ tái cấu trúc cơ chế gọi tiến trình con (subprocess orchestrator) và thiết lập giải thuật bóc tách chuỗi JSON an toàn (brace-matching extractor). |

#### 5.1. Prompt nguyên văn

```text
Resolve the NotImplementedError when running asyncio.create_subprocess_exec on Windows under Uvicorn's SelectorEventLoop policy. Replace the async git clone helper with a synchronous subprocess.run execution wrapped in asyncio.to_thread. Additionally, design a resilient JSON extraction method in the Claude Service using a brace-matching algorithm to prevent failures when Claude prepends conversational text or markdown blocks before the JSON payload.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- FastAPI chạy trên Windows dưới Uvicorn sử dụng SelectorEventLoop làm mặc định, dẫn đến lỗi NotImplementedError khi gọi các câu lệnh shell bất đồng bộ như git clone thông qua asyncio.create_subprocess_exec.
- Claude đôi khi trả về đoạn văn mở đầu hoặc kết luận trước/sau khối JSON ```json ... ```, làm gãy hàm phân tích cú pháp json.loads tiêu chuẩn của Python.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất chuyển sang chạy subprocess.run đồng bộ nhưng bọc trong asyncio.to_thread để chạy không gây nghẽn luồng chính. Đồng thời đề xuất thuật toán brace-matching duyệt qua chuỗi ký tự để tìm vị trí ngoặc nhọn mở đầu `{` và ngoặc nhọn đóng `}` tương thích để cắt chuỗi.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Áp dụng giải pháp asyncio.to_thread cho lệnh git clone giúp chạy ổn định trên Windows.
- Áp dụng hàm brace-matching bóc tách thành công JSON thô từ phản hồi của Claude 3.5 Sonnet.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Sinh viên bổ sung việc vô hiệu hóa nhắc nhở đăng nhập tài khoản bằng cách chèn biến môi trường GIT_TERMINAL_PROMPT=0 trong subprocess.run để hệ thống tự động báo lỗi ngay thay vì bị treo vô hạn.
- Mở rộng hàm brace-matching để hỗ trợ kiểm tra dấu ngoặc vuông `[` và `]` phòng trường hợp Claude trả về một mảng JSON các thẻ công nghệ.
```

---

### Prompt số 2 (Next.js Frontend Progress UI & SSE State Sync)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-05                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Tạo kết nối SSE để hiển thị tiến trình phân tích thời gian thực và quản lý trạng thái hiển thị của UI Settings.                                         |
| Frontend UI / SSE Services / React State Management |
| Mức độ sử dụng      | Hỗ trợ sinh React Hook lắng nghe EventSource và ánh xạ danh mục trạng thái phân tích từ backend.                                                       |

#### 5.1. Prompt nguyên văn

```text
Build a React Hook or state controller in Next.js to stream progress events from the Repository Analysis SSE endpoint. The UI must display a detailed progress bar, current operation text, and real-time terminal logs. Ensure that intermediate states returned from the backend (like RunningAgents, RunningMetrics, ProcessingReport) are mapped correctly so that the UI does not collapse back to the initial "Analyze" button during execution.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Phân tích mã nguồn là tiến trình lâu dài (lên đến vài phút). Cần có cơ chế streaming SSE (Server-Sent Events) để thông báo tiến độ trực quan.
- Nếu không mapping trạng thái tốt, các giá trị trạng thái mới từ backend đổ về có thể làm state ở React bị chuyển sang null hoặc idle, khiến giao diện bị sập nút bấm và reset tiến độ.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất mã khởi tạo `new EventSource()` trong React component, lắng nghe sự kiện `message` để set các state: progress, status, và logs.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo mã kết nối EventSource nhận dữ liệu.
- Thiết kế thanh tiến trình động tương ứng với phần trăm nhận về.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Do EventSource mặc định của trình duyệt không hỗ trợ custom Headers để gửi JWT token xác thực, sinh viên đã sửa đổi cách truyền token thông qua URL Query Parameter `?token=xxx` và cấu hình Middleware ở Backend để đón nhận.
- Tự tay tinh chỉnh phần layout console log: Thêm tính năng Auto-scroll xuống dòng cuối cùng để người dùng thấy log mới nhất lập tức, tăng trải nghiệm nhập vai.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần chỉ định rõ hệ điều hành mục tiêu (Windows) và các thư viện máy chủ (Uvicorn, FastAPI) đang chạy để tránh AI gợi ý các hàm bất đồng bộ không tương thích, đồng thời đưa ra các mẫu phản hồi lỗi cụ thể để AI khoanh vùng giải pháp nhanh hơn.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Việc phân rã các yêu cầu lớn thành các tác vụ kỹ thuật cụ thể (ví dụ: yêu cầu thuật toán bóc tách JSON thay vì chỉ nói "sửa lỗi phân tích JSON") giúp AI sinh mã tối ưu hơn và tránh viết lan man.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Design |        2 | Resolve the NotImplementedError... / Build a React Hook... |

---

## 10. Checklist chất lượng prompt

| Tiêu chí                   | Đã đạt? | Ghi chú |
| -------------------------- | :-----: | ------- |
| Prompt có mục tiêu rõ ràng |    x    |         |
| Prompt có đủ bối cảnh      |    x    |         |
| Tự kiểm tra và chỉnh sửa   |    x    |         |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết sử dụng prompt minh bạch và ghi nhận đúng đóng góp của AI.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-06    |
