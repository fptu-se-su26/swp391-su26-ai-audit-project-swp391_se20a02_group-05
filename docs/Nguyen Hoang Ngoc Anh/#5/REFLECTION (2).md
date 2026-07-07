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
| Ngày hoàn thành reflection | 2026-05-22 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong dự án này, tôi sử dụng AI như một trợ lý kỹ thuật đắc lực để tối ưu hóa quy trình phát triển theo Clean Architecture. Cụ thể, AI hỗ trợ phác thảo cấu trúc Database, gợi ý các mẫu thiết kế (Design Patterns) như Repository Pattern để quản lý dữ liệu, và hỗ trợ tạo các khung mã nguồn (boilerplate code) cho các tầng nghiệp vụ. Tôi đóng vai trò chủ chốt trong việc kiểm soát logic, tinh chỉnh cấu trúc theo chuẩn dự án, debug các vấn đề phát sinh, và tích hợp các module vào hệ thống thống nhất để đảm bảo tính ổn định và khả năng mở rộng.
```

---

## 4. Công cụ AI đã sử dụng

- [x] ChatGPT
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
Gemini
```

### Lý do sử dụng công cụ đó

```text
Tiết kiệm thời gian, Tạo tài liệu
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
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
 
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
AI hỗ trợ đẩy nhanh tốc độ nghiên cứu tài liệu, gợi ý các giải pháp kỹ thuật (Clean Architecture, Design Patterns) giúp nhóm có định hướng triển khai rõ ràng ngay từ đầu, giảm thiểu các lỗi logic cơ bản khi lập trình và tạo môi trường thảo luận nhanh để giải quyết bế tắc kỹ thuật.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
AI đôi khi đưa ra các đoạn mã thiếu tính tùy biến cao cho các nghiệp vụ đặc thù hoặc chưa hiểu hết logic nghiệp vụ phức tạp của dự án, đòi hỏi con người phải can thiệp sâu để hiệu chỉnh (human-in-the-loop). Đôi khi AI có thể đưa ra các thư viện hoặc cách tiếp cận không tương thích với version .NET mà nhóm đang sử dụng.
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

- [ ] Chạy thử chương trình
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
Nhóm thực hiện kiểm chứng thông qua các bước: (1) Chạy thử chương trình để đảm bảo code biên dịch thành công; (2) Review code thủ công để đối chiếu với tiêu chuẩn 3 tầng (3-layer architecture); (3) Kiểm tra bằng dữ liệu mẫu (Unit Testing) để đảm bảo logic tính toán/truy vấn khớp với yêu cầu nghiệp vụ; (4) Thảo luận nhóm để thống nhất các đoạn code AI cung cấp trước khi áp dụng vào dự án thực tế.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI đề xuất cấu trúc project .NET với 2 tầng (Presentation và Data Access) để đơn giản hóa việc thực hiện bài tập. |
| Em/nhóm đã kiểm tra bằng cách nào? | So sánh với yêu cầu kỹ thuật của môn học và nguyên lý Clean Architecture mà tôi đang áp dụng cho dự án. |
| Kết quả kiểm tra | Chưa phù hợp (Sai so với yêu cầu). Vì cấu trúc 2 tầng không đảm bảo tính tách biệt của Business Logic, gây khó khăn cho việc mở rộng và bảo trì sau này. |
| Em/nhóm đã xử lý tiếp như thế nào? | Tôi đã tự điều chỉnh, tách biệt thêm tầng Business Logic (Service Layer) để đảm bảo mô hình 3 lớp, sau đó cấu hình lại Dependency Injection để các lớp giao tiếp với nhau đúng chuẩn. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

Phát triển tính năng: Xây dựng các module phức tạp như theo dõi lịch trình sinh hoạt và quản lý dữ liệu sức khỏe trẻ em, sử dụng Repository Pattern để tối ưu hóa truy vấn.

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Planning | Average | Fast |  |

---

## 11. Bài học về môn học

- Tầm quan trọng của làm việc nhóm
- Kiểm thử sớm giúp giảm thiểu lỗi

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Cần kiểm chứng nội dung AI tạo ra
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

- Tìm hiểu sâu hơn về thiết kế hệ thống

Sẽ làm tốt hơn lần sau

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
| Nguyễn Hoàng Ngọc Ánh | 22/5/2026 |
