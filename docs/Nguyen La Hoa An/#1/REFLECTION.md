# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm |  |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | Quang |
| Ngày hoàn thành reflection | 2026-05-15 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
sử dụng AI như công cụ hỗ trợ trong quá trình làm
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
gemini
```

### Lý do sử dụng công cụ đó

```text
nhanh
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [ ] Phân tích bài toán
- [ ] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [ ] Thiết kế kiến trúc hệ thống
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
AI đã giúp em hiểu được những điều cần thiết cho dự án, không quá lan man, dài dòng
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
AI đã giúp em rất nhiều trong việc lên kế hoạch, thiết kế, và hiểu về các vấn đề trong dự án
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
 
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
đôi khi phải phụ thuộc vào AI vì có những vấn đề không hiểu
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [x] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Sau khi nhận phản hồi từ AI, nhóm thực hiện kiểm chứng qua 3 lớp: Đầu tiên là Review code để đảm bảo logic an toàn. Tiếp theo, đưa mã nguồn vào môi trường sandbox để Chạy thử chương trình. Cuối cùng, Viết các test case biên (như nhập sai ngày tháng, địa điểm không có trong DB) để Kiểm tra output xem AI có xử lý lỗi đúng như mong đợi hay không.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI đề xuất một đoạn mã Java sử dụng thư viện JSON Simple để phân tích dữ liệu thời tiết và gợi ý lộ trình du lịch |
| Em/nhóm đã kiểm tra bằng cách nào? | Nhóm đã copy đoạn mã vào dự án, tạo một file JSON mẫu với dữ liệu thời tiết xấu (bão) và chạy unit test để xem AI có tự động đổi lộ trình sang các hoạt động trong nhà hay không. |
| Kết quả kiểm tra | Đúng về mặt logic tính toán, nhưng Sai về mặt thư viện (thư viện AI dùng đã cũ, không tương thích với phiên bản Spring Boot hiện tại của nhóm). |
| Em/nhóm đã xử lý tiếp như thế nào? | Nhóm giữ lại logic gợi ý của AI nhưng yêu cầu AI viết lại đoạn code sử dụng thư viện Jackson để đồng bộ với project. Sau đó, nhóm tự tay chỉnh sửa lại các endpoint để kết nối chính xác với database của hệ thống. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
Thiết kế Database Schema: Xây dựng cấu trúc bảng dữ liệu trên SQL Server/Supabase để lưu trữ thông tin địa điểm, người dùng và lịch sử chuyến đi.

Xây dựng Workflow: Thiết lập luồng đi của dữ liệu từ Frontend qua Backend, cách xử lý xác thực (Authentication) và phân quyền trước khi gửi dữ liệu đến AI API.

Xử lý Context Injection: Viết code Java/C# để tự động truy xuất dữ liệu từ Database (sở thích, thời tiết) và "nhồi" vào câu lệnh gửi cho AI.

Hệ thống Filter & Security: Xây dựng bộ lọc để kiểm soát nội dung đầu ra của AI, đảm bảo AI không trả về thông tin sai lệch hoặc vi phạm chính sách của ứng dụng.

Kết nối API: Tự cấu hình và quản lý các kết nối API, xử lý các trường hợp mất mạng hoặc lỗi server (Error Handling).
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Hiểu yêu cầu | lan man, không hiểu | hiểu | hiểu |

---

## 11. Bài học về môn học

```text
hiểu hơn 
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
sử dụng vừa phải, không được lạm dụng
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
khi thực hiện dự án phải lên kế hoạch cụ thể
```

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 |   |
| Prompt có mục tiêu rõ ràng | 5 |   |
| Kiểm chứng kết quả AI | 5 |   |
| Tự chỉnh sửa/cải tiến | 5 |   |
| Hiểu nội dung đã nộp | 5 |   |
| Reflection có chiều sâu | 5 |   |
| Sử dụng AI có trách nhiệm | 5 |   |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
có
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
có
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
thảo luận nhóm
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
thiết lập kế hoạch
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|   | 15/5/2026 |
