# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Repository Analysis Engine with Real-time SSE Progress Streaming             |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-06-06                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình phát triển hệ thống phân tích mã nguồn thời gian thực, AI đã hỗ trợ đắc lực trong việc sinh mã khung cho hàng đợi xử lý ngầm (Background Worker) bằng C#, đề xuất giải pháp xử lý đa luồng non-blocking trên FastAPI, và thiết lập cấu trúc EventSource lắng nghe SSE trên Next.js. Sinh viên đóng vai trò thiết kế kiến trúc chính và xử lý các vấn đề thực tế phức tạp bao gồm: sửa lỗi tương thích SelectorEventLoop của Windows thông qua chạy subprocess đồng bộ trong luồng phụ (asyncio.to_thread), xây dựng giải thuật brace-matching tự chống lỗi định dạng JSON từ Claude, triển khai cơ chế truyền Bearer Token an toàn qua URL param kết hợp Gateway Auth, và tối ưu hóa trải nghiệm Terminal Logs auto-scroll ở frontend.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Antigravity
```

### Lý do sử dụng công cụ đó

```text
Antigravity cho phép sửa lỗi và đồng bộ mã nguồn trực tiếp trên cả 3 phân hệ (C# backend, Python AI, và TypeScript frontend) trong cùng một workspace một cách liền mạch, giúp giải quyết các bài toán tích hợp End-to-End phức tạp cực kỳ nhanh chóng.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [ ] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [x] Viết test case
- [ ] Review code
- [ ] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI giúp sinh nhanh cấu trúc các thực thể DB (AnalysisJob, AnalysisReport), khung sườn của Background Repository Analysis Processor ở backend, viết các API endpoints điều phối phân tích ở Python, và sinh khung kết nối EventSource SSE ở client.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu sâu về cách phân bổ tài nguyên xử lý tác vụ nặng (git clone, AI analysis) thông qua kiến trúc hàng đợi bất đồng bộ kết hợp Redis Pub/Sub.
- Tiếp cận giải pháp Server-Sent Events (SSE) để truyền dữ liệu một chiều thời gian thực với chi phí tối thiểu so với WebSocket hay Web Polling.
- Nhận thức được các hạn chế hệ thống liên quan đến Event Loop trên hệ điều hành Windows và cách khắc phục bằng asyncio.to_thread.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đề xuất sử dụng asyncio.create_subprocess_exec trên FastAPI để thực thi git clone, tuy nhiên gợi ý này bị sập hoàn toàn trên Windows do chính sách SelectorEventLoop của Uvicorn không hỗ trợ chạy subprocess bất đồng bộ.
- AI đề xuất bóc tách phản hồi JSON từ Claude bằng biểu thức chính quy (Regex), dẫn đến lỗi runtime khi Claude định dạng phản hồi không chuẩn hoặc sinh văn bản tự do ngoài thẻ code markdown.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm chỉ sử dụng AI cho mục đích boilerplate code và thiết kế schema JSON ban đầu. Các phần sửa lỗi hệ điều hành Windows, thiết kế giải thuật brace-matching tự chế, cơ chế bảo mật token cho cổng SSE, và tối ưu giao diện Terminal hoàn toàn do sinh viên tự tay lập trình và kiểm thử.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [x] Viết test case
- [x] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [x] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Quá trình kiểm chứng được thực hiện qua các bước:
1. Chạy thực tế tiến trình phân tích trên web, theo dõi log in ra terminal console của Next.js khớp hoàn toàn với các bước đang chạy ngầm trong Python AI service.
2. Kiểm tra xem tệp tin mã nguồn có bị rò rỉ hay không: Git clone tải về được dọn dẹp sạch sẽ trong thư mục tạm ngay sau khi phân tích kết thúc.
3. Chạy unit tests DecryptTokensTest.cs đảm bảo việc mã hóa và giải mã token để clone repos private hoạt động chính xác.
4. Kiểm tra cấu trúc dữ liệu JSON được lưu vào bảng AnalysisReport khớp đúng với đặc tả báo cáo.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý sử dụng `asyncio.create_subprocess_exec` để chạy tiến trình git clone bất đồng bộ. |
| Em/nhóm đã kiểm tra bằng cách nào? | Khởi chạy ứng dụng FastAPI trên Windows và bấm kích hoạt phân tích kho lưu trữ. |
| Kết quả kiểm tra | Tiến trình sập ngay lập tức và ném ra ngoại lệ `NotImplementedError` do SelectorEventLoop của Windows không hỗ trợ. |
| Em/nhóm đã xử lý tiếp như thế nào? | Thay đổi sang sử dụng `subprocess.run` đồng bộ nhưng ủy quyền chạy trên một Worker Thread riêng bằng `asyncio.to_thread` giúp giữ luồng chính luôn non-blocking. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Sử dụng biểu thức chính quy (Regex) để trích xuất JSON từ chuỗi văn bản của Claude. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Cực kỳ thiếu bền vững (fragile). Chỉ cần Claude thêm một vài ký tự văn bản ngẫu nhiên hoặc quên viết thẻ markdown ` ```json ` là Regex sẽ không tìm thấy khớp, làm chết toàn bộ tiến trình phân tích. |
| Em/nhóm phát hiện bằng cách nào? | Chạy phân tích nhiều lần với các repos khác nhau, có những lần Claude sinh phản hồi có text giải thích bên ngoài làm hàm parse ném lỗi. |
| Em/nhóm đã sửa như thế nào? | Thiết kế hàm `extract_json_block` sử dụng brace-matching (đếm số ngoặc `{` và `}` mở đóng tương ứng) để cắt chuỗi JSON thô chính xác bất kể text bọc bên ngoài. |
| Bài học rút ra | Khi làm việc với LLMs, lập trình viên luôn phải chuẩn bị sẵn tư duy thiết kế phòng vệ (defensive design), dự phòng các trường hợp dữ liệu đầu ra bị biến dạng để hệ thống không bị crash đột ngột. |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
- Tự triển khai cơ chế bypass Git interactive prompts bằng cấu hình config nền, tránh nghẽn thread.
- Thiết kế và phát triển giải thuật bóc tách JSON brace-matching cho Claude Service.
- Viết logic mapping trạng thái phân tích đầu cuối ở frontend Next.js tránh lỗi sập UI.
- Viết Background Queue Recovery Sweeper để dọn dẹp và khôi phục các tác vụ phân tích cũ bị treo.
- Cấu hình truyền token an toàn và xác thực cổng stream SSE.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Tiết kiệm ~40% thời gian dựng khung các API controllers và cấu hình Background Workers. |
| System Architecture | Standard | Complex/Real-time | Triển khai được cơ chế stream SSE thời gian thực kết nối 3 nền tảng khác nhau cực kỳ mượt mà. |
| Error Resilience | Basic | Excellent | Xây dựng được các bộ lọc lỗi phòng vệ tốt trước các ngoại lệ từ hệ điều hành và mô hình AI. |

---

## 11. Bài học về môn học

- Thiết kế ứng dụng phân tán cần chú ý đến sự tương thích hệ điều hành (như hành vi Event Loop của Windows vs Linux) ngay từ bước lập kế hoạch.
- Sử dụng Server-Sent Events (SSE) là giải pháp truyền tải tiến trình một chiều tuyệt vời cho các tiến trình nền lâu dài mà không cần duy trì kết nối hai chiều phức tạp như WebSockets.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Không được phó mặc chất lượng mã nguồn hoặc tính ổn định hệ thống cho AI. Lập trình viên phải hiểu rõ cơ chế hoạt động của code AI sinh ra để chủ động gỡ lỗi khi môi trường chạy thực tế thay đổi.
- Thiết kế hệ thống tích hợp AI phải luôn có kiểm soát lỗi chặt chẽ (defensive programming) đối với dữ liệu do mô hình ngôn ngữ sinh ra.

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

---

## 14. Kế hoạch cải thiện lần sau

- Cung cấp các thông tin ràng buộc môi trường (như Windows OS, Uvicorn server) ngay khi hỏi về subprocess để tránh AI gợi ý các API bất tương thích.
- Yêu cầu AI viết mã kiểm thử bao phủ (test coverage) nhiều trường hợp ngoại lệ từ LLM hơn để tăng độ bền bỉ cho ứng dụng.

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
| --- | --- | --- |
| Ghi nhận việc dùng AI trung thực | 5 | |
| Prompt có mục tiêu rõ ràng | 5 | |
| Kiểm chứng kết quả AI | 5 | |
| Tự chỉnh sửa/cải tiến | 5 | |
| Hiểu nội dung đã nộp | 5 | |
| Reflection có chiều sâu | 5 | |
| Sử dụng AI có trách nhiệm | 5 | |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm hoàn toàn giải thích được cơ chế hoạt động của Background Processor nhận thông tin từ hàng đợi, cơ chế bắn Pub/Sub Redis đến cổng Web API và chuyển tiếp thành luồng stream SSE gửi về frontend Next.js, cũng như giải thuật brace-matching tự viết để bóc tách JSON của Claude.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc viết các background service trong .NET Core, xử lý đa luồng Python, gọi subprocess, và sử dụng API EventSource của trình duyệt đều là những kiến thức lập trình cơ bản có đầy đủ tài liệu hướng dẫn chính thống trên mạng.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-06    |
