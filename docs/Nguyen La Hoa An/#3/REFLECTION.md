# AI Learning Reflection

## 1. Thông tin chung

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
| Ngày hoàn thành reflection | 2026-06-11 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
 
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
Canva AI
```

### Lý do sử dụng công cụ đó

```text
Tiết kiệm thời gian, Hỗ trợ thiết kế UI/UX, Sinh code nhanh
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [ ] Phân tích bài toán
- [ ] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [x] Thiết kế giao diện
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
 
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Tối ưu hóa thời gian xây dựng giao diện: AI hỗ trợ tạo nhanh bộ khung Frontend (HTML/JS) cho các cấu trúc lặp đi lặp lại như dropdown list và bảng dữ liệu, giúp tiết kiệm thời gian gõ code thủ công.

Hiểu rõ hơn về kỹ thuật Prompting nâng cao: Quá trình làm việc giúp em làm quen và áp dụng thành công kỹ thuật "Prompt phủ định" (Negative Prompting) để định hình phạm vi phản hồi của AI một cách chính xác.

Nâng cao tư duy phân rã bài toán: Để AI sinh code chạy được ngay, em học được cách chia nhỏ giao diện thành từng khối chức năng riêng biệt, từ đó rèn luyện tư duy logic tốt hơn khi thiết kế luồng hiển thị.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
Sinh code dư thừa (Boilerplate): AI có xu hướng tự động chèn thêm các đoạn mã mẫu rườm rà (như form login, layout header/footer chung) ngoài ý muốn, làm loãng tệp mã nguồn cốt lõi của dự án CVerify.

Chưa đồng bộ sâu với logic Backend: Các đoạn mã Frontend do AI gợi ý đôi khi sử dụng cấu trúc nhận phản hồi dữ liệu (response data-binding) chung chung, bắt buộc em phải tự tay chỉnh sửa lại rất nhiều để khớp hoàn toàn với API từ Java Servlet và JDBC của hệ thống.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [ ] Phụ thuộc ít
- [x] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Sử dụng AI để tối ưu hóa thời gian nghiên cứu và tạo cấu trúc ban đầu.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [ ] Kiểm tra output
- [ ] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [ ] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Đầu tiên, em tiến hành đọc và đánh giá nhanh cấu trúc mã nguồn (Review code) do AI cung cấp để xem có đúng phạm vi chức năng yêu cầu hay không. Sau đó, em tích hợp đoạn mã Frontend này vào cấu trúc dự án Java Web cục bộ. Em tiến hành chạy thử chương trình trên máy chủ Apache Tomcat để trực tiếp kiểm tra giao diện hiển thị của các dropdown list và bảng dữ liệu. Cuối cùng, thông qua thảo luận với các thành viên khác để đối chiếu sự đồng bộ giữa luồng xử lý giao diện và hệ thống database Backend, đảm bảo toàn bộ tính năng hoạt động nhất quán.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI đã gợi ý trang hiển thị, một đoạn mã HTML và JavaScript (Sử dụng Fetch API) để tự động đổ dữ liệu động lên các thẻ danh sách thả xuống (dropdown list) và hiển thị các dòng kết quả tương ứng khi người dùng tương tác. |
| Em/nhóm đã kiểm tra bằng cách nào? | Em đã nhúng đoạn mã vào trang chức năng của dự án, khởi động máy chủ Tomcat cục bộ để chạy thử chương trình. Đồng thời, em mở công cụ Developer Tools (F12) trên trình duyệt để kiểm tra tab Console và Network xem luồng truyền nhận dữ liệu từ các Servlet Backend có bị lỗi hay không. |
| Kết quả kiểm tra | Kết quả kiểm tra ban đầu là Sai lệch một phần. Giao diện dropdown hiển thị đúng cấu trúc nhưng dữ liệu bị trống do đường dẫn API và cấu trúc đối tượng JSON phản hồi từ Java Servlet thực tế khác với cấu trúc giả định chung chung của AI. |
| Em/nhóm đã xử lý tiếp như thế nào? |   |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

Về Frontend, em là người hỗ trợ, đưa ra định hướng thiết kế tối giản để tập trung hoàn toàn vào chức năng cốt lõi (Functional Readiness). Em đã kiểm soát chặt chẽ quá trình AI sinh code bằng cách loại bỏ các mô-đun rườm rà (login, header, footer), tự tay tích hợp mã nguồn Frontend vào máy chủ Apache Tomcat cục bộ, gỡ lỗi (debug) kết nối truyền nhận dữ liệu và tinh chỉnh lại các đoạn mã JavaScript xử lý vòng lặp đổ dữ liệu lên dropdown list và bảng hiển thị, đảm bảo hệ thống đồng bộ và vận hành mượt mà.

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|

---

## 11. Bài học về môn học

- Tầm quan trọng của làm việc nhóm
- Phân tích yêu cầu đóng vai trò then chốt

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Cần kiểm chứng nội dung AI tạo ra
- Tránh sao chép mù quáng kết quả từ AI
- AI chỉ hỗ trợ, không thay thế tư duy

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

- Đảm bảo tính nhất quán của UI/UX
- Cải thiện quy trình làm việc với Git

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
Có, nhóm đã đọc, kiểm tra và hiểu nội dung trước khi sử dụng.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có, nhưng sẽ mất nhiều thời gian hơn để nghiên cứu và triển khai.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần thiết kế workflow, chỉnh sửa logic và xử lý lỗi thực tế.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Kỹ năng thiết kế hệ thống, viết prompt và kiểm thử phần mềm.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 11/6/2026 |
