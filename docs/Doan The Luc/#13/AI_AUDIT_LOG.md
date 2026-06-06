# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - Repository Analysis Engine with Real-time SSE Progress Streaming             |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-06-05T19:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-06T03:00:00.000Z                                                               |

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
Mục tiêu là xây dựng hoàn chỉnh cơ chế phân tích mã nguồn kho lưu trữ (Repository Analysis Engine) tích hợp AI để kiểm tra mức độ tin cậy của mã nguồn thông qua ba phân hệ chính: Backend .NET Core (Xử lý hàng đợi nền, Redis Pub/Sub và Streaming tiến trình thực tế qua SSE), FastAPI AI Service (Orchestrator clone, phân tích công nghệ, lấy mẫu mã nguồn và phân tích bảo mật thông qua mô hình Claude 3.5 Sonnet), và Next.js Frontend (SSE listener tương tác hiển thị tiến trình, phần trăm, console logs chi tiết và report dashboard).
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1 (FastAPI Subprocess Execution & Claude Resiliency)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-05                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Giải quyết lỗi NotImplementedError khi chạy git clone bất đồng bộ trên Windows và khắc phục lỗi phân tích JSON từ phản hồi của Claude.                  |
| Phần việc liên quan | AI FastAPI Microservice / subprocess / Git integration / Claude API client                                                                              |
| Mức độ sử dụng      | Hỗ trợ tái cấu trúc cơ chế gọi tiến trình con (subprocess orchestrator) và thiết lập giải thuật bóc tách chuỗi JSON an toàn (brace-matching extractor). |

#### 4.1. Prompt đã sử dụng

```text
Resolve the NotImplementedError when running asyncio.create_subprocess_exec on Windows under Uvicorn's SelectorEventLoop policy. Replace the async git clone helper with a synchronous subprocess.run execution wrapped in asyncio.to_thread. Additionally, design a resilient JSON extraction method in the Claude Service using a brace-matching algorithm to prevent failures when Claude prepends conversational text or markdown blocks before the JSON payload.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất sử dụng asyncio.to_thread để ủy quyền tiến trình git clone đồng bộ sang một luồng làm việc riêng, tránh xung đột Event Loop trên Windows. Thiết lập giải thuật đếm ngoặc nhọn '{' và '}' để xác định vị trí chuỗi JSON hợp lệ đầu tiên và cuối cùng trong phản hồi văn bản thô của Claude.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Đoạn mã cấu hình asyncio.to_thread để gọi subprocess.run của Git.
- Thuật toán tìm ngoặc nhọn và phân tích chuỗi JSON động trong Claude Service.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Bổ sung cấu hình bypass Git Credential Manager thông qua biến môi trường GIT_TERMINAL_PROMPT=0 và cấu hình git config --global credential.helper "" trong tiến trình chạy luồng con để ngăn chặn tiến trình bị đứng vĩnh viễn (hang) do pop-up đăng nhập Windows.
- Tinh chỉnh thuật toán brace-matching để xử lý thêm trường hợp Claude trả về mảng JSON `[...]` thay vì đối tượng `{...}` tại một số tác vụ phân tích danh mục công nghệ.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(analysis): implement repository analysis engine with real-time SSE progress streaming | https://github.com/Kaivian/CVerify/commit/09a81b3cc30e9d6d37df90209df32ab1b54a7df2 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc Uvicorn ép buộc sử dụng SelectorEventLoop trên Windows là một điểm hạn chế kinh điển của môi trường phát triển cục bộ. Sự kết hợp giữa asyncio.to_thread và subprocess.run giúp giải quyết triệt để lỗi nền mà vẫn duy trì tính chất non-blocking của API FastAPI.
```

---

### Lần sử dụng AI số 2 (Next.js Frontend Progress UI & SSE State Sync)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-05                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Đồng bộ trạng thái tiến trình phân tích theo thời gian thực (SSE EventSource) trên giao diện Settings và ngăn chặn việc UI tự động quay lại trạng thái Idle. |
| Phần việc liên quan | Next.js Frontend / SSE Services / React State Management                                                                                                |
| Mức độ sử dụng      | Hỗ trợ sinh React Hook lắng nghe EventSource và ánh xạ danh mục trạng thái phân tích từ backend.                                                       |

#### 4.1. Prompt đã sử dụng

```text
Build a React Hook or state controller in Next.js to stream progress events from the Repository Analysis SSE endpoint. The UI must display a detailed progress bar, current operation text, and real-time terminal logs. Ensure that intermediate states returned from the backend (like RunningAgents, RunningMetrics, ProcessingReport) are mapped correctly so that the UI does not collapse back to the initial "Analyze" button during execution.
```

#### 4.2. Kết quả AI gợi ý

```text
AI cung cấp cấu trúc EventSource listener trong service frontend và viết logic chuyển đổi trạng thái trong component React để phân loại tiến trình từ 0% đến 100% dựa trên các chuỗi Event được đẩy xuống.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Code boilerplate của EventSource listener và gắn token Authorization vào tham số truy vấn URL (do EventSource gốc không hỗ trợ custom headers).
- Thiết kế thanh tiến trình (progress bar) và khu vực console hiển thị log dòng lệnh thời gian thực.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Khắc phục lỗi reset UI: Tự xây dựng cấu trúc ánh xạ phân cấp, gom nhóm toàn bộ các trạng thái trung gian (`Queued`, `Cloning`, `DetectingTechnology`, `SamplingCode`, `RunningAgents`, `GeneratingReport`) vào trạng thái `"analyzing"` của component để giao diện không bị giật lag hoặc tự sập về nút ban đầu.
- Tự động cuộn khu vực log console (`scrollToBottom`) khi có bản ghi log mới đổ về để cải thiện trải nghiệm người dùng tương tự như một terminal thực sự.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(analysis): implement repository analysis engine with real-time SSE progress streaming | https://github.com/Kaivian/CVerify/commit/09a81b3cc30e9d6d37df90209df32ab1b54a7df2 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Kết nối SSE mang lại trải nghiệm mượt mà hơn nhiều so với việc gọi API polling liên tục. Việc tùy biến luồng dữ liệu log giúp giao diện CVerify mang lại cảm giác chuyên nghiệp cao.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                                     |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------------- |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích thiết kế hệ thống hàng đợi phân tán tích hợp AI.                                  |
| Viết user story/use case    |       x       |              |                 |               |                                                                                             |
| Thiết kế database           |               |      x       |                 |               | Tạo bảng AnalysisJob, AnalysisJobEvent, AnalysisReport.                                     |
| Thiết kế kiến trúc hệ thống |               |              |        x        |               | Kiến trúc Redis Pub/Sub chuyển tiếp log từ Worker sang Web API và SSE client.               |
| Thiết kế giao diện          |               |              |        x        |               | Giao diện Terminal console logs hiển thị tiến trình chạy agent của Claude.                  |
| Code frontend               |               |              |        x        |               | Xây dựng trang thiết lập, EventSource listener, xử lý mapping trạng thái SSE.                |
| Code backend                |               |              |        x        |               | Viết BackgroundRepositoryAnalysisProcessor, RepositoryAnalysisService và API SSE endpoints. |
| Debug lỗi                   |               |              |        x        |               | Sửa lỗi SelectorEventLoop trên Windows và lỗi định dạng JSON từ Claude.                     |
| Viết test case              |               |      x       |                 |               | Viết bộ test giải mã token an toàn DecryptTokensTest.cs.                                     |
| Kiểm thử sản phẩm           |       x       |              |                 |               | Kiểm thử thực tế quá trình phân tích SSE từ đầu đến khi sinh báo cáo thành công.            |
| Tối ưu code                 |       x       |              |                 |               |                                                                                             |
| Viết báo cáo                |       x       |              |                 |               |                                                                                             |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                             |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI sử dụng `asyncio.create_subprocess_exec` để gọi git clone trong FastAPI, dẫn đến gãy tiến trình trên môi trường Windows. | Ứng dụng AI Microservice tung ra lỗi `NotImplementedError` ngay lập tức khi nhận lệnh phân tích. | Chuyển đổi sang `subprocess.run` chạy đồng bộ bên trong `asyncio.to_thread` để tránh block main loop. |
|   2 | AI đề xuất sử dụng regex để trích xuất JSON nằm giữa thẻ ```json ... ``` từ Claude, cách này bị lỗi nếu Claude viết sai cú pháp markdown. | Quá trình phân tích ném ngoại lệ: `Claude output did not return a valid JSON format` khi Claude dán text giải thích trước thẻ code block. | Triển khai thuật toán brace-matching duyệt chuỗi để tìm điểm khởi đầu và kết thúc của JSON thô. |
|   3 | AI thiết kế EventSource client bằng thư viện chuẩn của trình duyệt nhưng bỏ qua việc truyền Bearer Token của user. | API Web trả về mã lỗi 401 Unauthorized do thiếu JWT token trong request Header của SSE. | Chuyển token qua query parameter `/stream?token=xxx` và xác thực an toàn tại API Gateway/Middleware. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả qua các hình thức sau:
1. Thực hiện chạy phân tích thành công 100% kho lưu trữ GitHub công khai và riêng tư trên môi trường Windows.
2. Kiểm tra log stream trên Next.js console chạy mượt mà, tự động cập nhật phần trăm và logs chi tiết từng bước mà không bị reset trạng thái.
3. Chạy thành công bộ unit tests bảo mật thông tin DecryptTokensTest.cs kiểm định quá trình giải mã token AES-GCM.
4. Đảm bảo toàn bộ backend, frontend, và AI microservice biên dịch thành công mà không có lỗi TypeScript hay EF Core configuration.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Thiết kế giải pháp bypass Git credentials bằng global configs và biến môi trường nền để tránh bị đứng luồng chạy ngầm.
- Triển khai thuật toán bóc tách JSON brace-matching đảm bảo xử lý cả mảng JSON và đối tượng JSON bị bọc văn bản ngẫu nhiên.
- Refactor logic mapping trạng thái SSE ở frontend Next.js đảm bảo tính ổn định và tính tương tác của giao diện người dùng.
- Cấu hình Redis Pub/Sub và background sweeper xử lý dọn dẹp và khôi phục các tác vụ phân tích bị treo do lỗi đột ngột.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                                     | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | -------------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai Backend Queue/SSE, FastAPI AI Orchestrator, sửa lỗi Windows Subprocess & Claude JSON.   | Có                   | https://github.com/Kaivian/CVerify/commit/09a81b3cc30e9d6d37df90209df32ab1b54a7df2 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm tra thủ công giao diện kết quả phân tích (Report Cards) và giao diện console log responsive. | Không                |                     |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp giảm thiểu thời gian viết code mẫu cho việc cấu hình SSE controller trên .NET Core, xây dựng cấu trúc schema JSON phân tích mã nguồn cho Claude prompt, và dựng khung cơ bản cho giao diện Next.js Settings Page.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng bộ phân tích JSON bằng biểu thức chính quy (Regex) do AI đề xuất. Regex rất kém linh hoạt và dễ bị gãy nếu mô hình AI (Claude) thay đổi văn bản mô tả đầu ra hoặc sử dụng các ký tự đặc biệt lồng nhau.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Nhóm kiểm chứng bằng cách kiểm thử luồng hoạt động tích hợp thực tế (End-to-End integration testing) từ việc kích hoạt nút phân tích trên web, quan sát dữ liệu thay đổi trong DB, các sự kiện bắn qua Redis Pub/Sub, tiến trình chạy của agent trên Python, cho tới khi dữ liệu SSE cập nhật thời gian thực trên giao diện.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần xây dựng prompt chi tiết (System Prompts) định nghĩa các luật phân tích bảo mật, phát hiện kiến trúc mã nguồn và đánh giá điểm tin cậy cho Claude 3.5 Sonnet vì đòi hỏi cách hành văn chuyên nghiệp và các khuôn dạng JSON nghiêm ngặt.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Hiểu sâu sắc về cơ chế vận hành của lập trình hướng sự kiện (Event-driven architecture) sử dụng Redis Pub/Sub và SSE, cách xử lý đa luồng non-blocking ở cả C# và Python, và các vấn đề tương thích hệ điều hành (Windows vs Linux) trong thực tế.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Không nên tin tưởng tuyệt đối vào khả năng trả về đúng cấu trúc định dạng của LLM. Luôn luôn phải xây dựng các bộ lọc lỗi (defensive programming) và bộ phân tách cú pháp an toàn để bảo vệ hệ thống khỏi các ngoại lệ phân tích dữ liệu không mong muốn.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-06    |
