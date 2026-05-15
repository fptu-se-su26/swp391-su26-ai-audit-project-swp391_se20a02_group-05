# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software development project |
| Mã môn học | SWP391 |
| Lớp | SE20A06 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm |  |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | Trương Văn Hiếu |
| Ngày hoàn thành reflection | 2026-05-15 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Nhóm đã sử dụng AI để hỗ trợ giải quyết các vấn đề kỹ thuật phát sinh trong quá trình sử dụng Git và GitHub Desktop. Cụ thể, AI đã hướng dẫn quy trình xử lý lỗi "Submodule" khi lồng một repository vào thư mục dự án chung. Ngoài ra, AI còn hỗ trợ tối ưu hóa các tham số kỹ thuật (Temperature, System Instructions) để điều khiển model Gemini 3.1 Pro trả về dữ liệu lịch trình du lịch chính xác dưới dạng JSON, giúp chuẩn hóa dữ liệu đầu vào cho dự án TripGenie.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
 Gemini
```

### Lý do sử dụng công cụ đó

```text
 Cung cấp khả năng suy luận sâu (Reasoning/Thoughts), hỗ trợ xuất dữ liệu cấu trúc JSON chuẩn xác và có khả năng đọc hiểu cấu trúc thư mục phức tạp qua hình ảnh chụp màn hình terminal.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [ ] Viết code mẫu
- [ ] Debug lỗi
- [ ] Viết test case
- [ ] Review code
- [ ] Tối ưu code
- [ ] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
 AI đã giúp phân tích cấu trúc folder dự án qua lệnh 'dir' để tìm ra vị trí file package.json bị thất lạc. Đồng thời, AI đóng vai trò một "người dùng thử" để góp ý và hoàn thiện các tính năng cho Guest/Admin của nền tảng TripGenie.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
 Giúp nắm vững cách cấu hình API (như cấu hình Temperature, MaxTokens) và hiểu sâu hơn về cách model AI "suy nghĩ" trước khi đưa ra kết quả. Học được cách quản lý lỗi ENOENT trong môi trường Node.js.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
 Đôi khi AI gợi ý các lệnh nvm không tồn tại (như nvm run) khiến việc khởi động dự án ban đầu gặp chút bối rối.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [x] Không phụ thuộc
- [ ] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều


```text
 Giải thích:
Nhóm sử dụng AI như một trợ lý để kiểm tra lỗi cú pháp và tìm kiếm file nhanh hơn, nhưng logic chính của dự án và việc quản lý luồng dữ liệu vẫn do nhóm chủ động thực hiện.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [ ] Chạy thử chương trình
- [ ] Kiểm tra output
- [ ] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [x] Đối chiếu với tài liệu môn học
- [ ] Review code
- [ ] Hỏi lại giảng viên
- [x] Tra cứu tài liệu chính thống
- [x] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [x] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
 Mỗi khi AI gợi ý lệnh terminal hoặc cấu hình JSON, nhóm đều thực hiện chạy thử trực tiếp trên Postman và Terminal của VS Code. Nếu kết quả trả về đúng định dạng và web khởi chạy thành công trên localhost:3000, kết quả đó mới được chấp nhận.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? |   |
| Em/nhóm đã kiểm tra bằng cách nào? |   |
| Kết quả kiểm tra |   |
| Em/nhóm đã xử lý tiếp như thế nào? |   |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
 Xây dựng cấu trúc dự án, thiết kế luồng người dùng cho TripGenie, trực tiếp viết mã nguồn cho các phần xử lý logic, quản lý Repository trên GitHub và thực hiện tích hợp API vào ứng dụng thực tế.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
Nội dungTrước khi dùng AISau khi dùng AICải thiện đạt đượcDebug lỗi GitMất nhiều giờ tra cứu StackOverflowXử lý xong Submodule trong vài phútTiết kiệm 80% thời gianGen dữ liệu mẫuDữ liệu khô khan, không thực tếDữ liệu hấp dẫn, đúng phong cách Travel BloggerTăng tính chuyên nghiệp của sản phẩm
---

## 11. Bài học về môn học

```text
 Hiểu rõ hơn về quy trình phát triển phần mềm theo nhóm, tầm quan trọng của việc quản lý cấu trúc thư mục rõ ràng và cách tích hợp AI như một tính năng cốt lõi (Core Feature) thay vì chỉ là công cụ hỗ trợ.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
 
```

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

### Giải thích thêm nếu có

```text
 
```

---

## 14. Kế hoạch cải thiện lần sau

```text
 
```
sử dụng AI với prompt tối ưu hơn
---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
 có thể giải thích
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
 Có thể, nhưng sẽ mất nhiều thời gian hơn. Phần quan trọng nhất là thiết kế luồng dữ liệu và xây dựng giao diện người dùng dựa trên framework Next.js. Nếu không có AI, nhóm vẫn có thể tự viết tay các bộ dữ liệu mẫu (mock data) hoặc tra cứu thủ công tài liệu Git để giải quyết lỗi Submodule, dù hiệu suất sẽ giảm xuống đáng kể.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
 Đó là khả năng hiện thực hóa các ý tưởng từ bản thiết kế (System Design) vào code thực tế. Việc thiết kế các chức năng cho Guest/Admin và cách nhóm xử lý cấu trúc thư mục phức tạp để vận hành dự án trên môi trường local chính là minh chứng rõ nhất cho kỹ năng lập trình và giải quyết vấn đề của nhóm.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
 Nhóm muốn cải thiện kỹ năng Prompt Engineering để có thể điều khiển các mô hình AI phức tạp hơn, đồng thời trau dồi thêm kỹ năng quản lý dự án (Project Management) và tối ưu hóa hiệu suất ứng dụng Next.js khi xử lý các dữ liệu thời gian thực từ AI.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|   | 15/5/2026 |
