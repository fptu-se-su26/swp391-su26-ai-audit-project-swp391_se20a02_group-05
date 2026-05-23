# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Requirement |
| Mã môn học | SWR302 |
| Lớp | SE20A06 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm |  |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | TamTTT14 |
| Ngày hoàn thành reflection | 2026-05-23 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
ong dự án này, AI được áp dụng như một trợ lý ảo xuyên suốt từ những bước đầu tiên (lên ý tưởng hệ thống CV, xác định tính năng ATS Scoring) cho đến giai đoạn triển khai (hỗ trợ sinh boilerplate code, gợi ý cấu trúc React component, và hỗ trợ viết tài liệu báo cáo). Tuy nhiên, đối với các vấn đề kỹ thuật cốt lõi như thiết kế chuẩn kiến trúc hệ thống (.NET 3 layers, Repository Pattern) và logic nghiệp vụ phức tạp, nhóm vẫn tự đánh giá, đối chiếu và quyết định thủ công để đảm bảo tính chính xác và đáp ứng đúng yêu cầu môn học.
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
 
```

### Lý do sử dụng công cụ đó

```text
 
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
AI giúp đẩy nhanh quá trình tìm hiểu các khái niệm mới (như logic chấm điểm ATS), cung cấp các đoạn boilerplate code React/TypeScript để tham khảo nhanh cú pháp, giúp nhóm có nhiều thời gian hơn để tập trung vào logic nghiệp vụ thay vì phải gõ lại code từ đầu.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
Đôi khi AI đưa ra các kiến trúc hệ thống không đúng chuẩn hoặc sử dụng logic cũ. Việc mù quáng copy code có thể dẫn đến lỗi hệ thống không tương thích, khiến nhóm phải tốn thêm thời gian debug ngược lại.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [ ] Phụ thuộc ít
- [x] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm làm chủ hoàn toàn core logic của dự án. AI chỉ đóng vai trò như một người trợ lý để đẩy nhanh tốc độ (tạo UI mẫu, sinh dữ liệu giả). Các quyết định quan trọng về luồng dữ liệu, áp dụng Design Pattern hay cấu trúc Database đều do nhóm tự thảo luận và quyết định.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [ ] Chạy thử chương trình
- [ ] Kiểm tra output
- [ ] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [ ] Review code
- [x] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Mọi đoạn code hoặc gợi ý thiết kế từ AI đều được mang ra thảo luận nhóm. Sau đó, kết quả được đối chiếu với các yêu cầu kỹ thuật của môn học. Cuối cùng, code được đưa vào môi trường local để chạy thử và các thành viên sẽ tiến hành cross-review (review chéo) lẫn nhau trước khi merge vào nhánh chính.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Khi khởi tạo project .NET cho backend, AI đã gợi ý một cấu trúc thư mục (architecture) cho dự án với thiết kế chỉ bao gồm 2 lớp (layers). |
| Em/nhóm đã kiểm tra bằng cách nào? | Nhóm đã đối chiếu cấu trúc này với yêu cầu chuẩn của môn học và tài liệu về mô hình Clean Architecture/Onion Architecture. |
| Kết quả kiểm tra | Chưa chính xác (Thiếu sót). Cấu trúc chuẩn theo yêu cầu bắt buộc phải có đầy đủ 3 lớp (3 layers) tách biệt rõ ràng trách nhiệm. |
| Em/nhóm đã xử lý tiếp như thế nào? | Nhóm đã từ chối sử dụng cấu trúc do AI tạo ra. Tự tay thiết kế lại toàn bộ solution, tạo đủ 3 project riêng biệt cho 3 layer tương ứng và áp dụng thêm Repository Pattern để đảm bảo việc phân tách tầng truy xuất dữ liệu chuẩn xác nhất. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
Mô tả rõ phần nào là đóng góp chính của sinh viên/nhóm (không copy từ AI): Toàn bộ kiến trúc hệ thống (thiết kế mô hình 3 layers chuẩn cho .NET), cấu trúc cơ sở dữ liệu và việc áp dụng Repository Pattern hoàn toàn do nhóm tự phân tích và triển khai. Các nghiệp vụ cốt lõi như đồng bộ dữ liệu giữa Frontend (React) và Backend, hay việc thiết kế luồng xử lý xuất file PDF cho CV đều do các thành viên tự viết logic và code thủ công để đảm bảo hiệu suất. AI chỉ hỗ trợ sinh boilerplate code và UI component.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Hiểu yêu cầu | ok | tố  |  |

---

## 11. Bài học về môn học

```text
Nắm vững cách xây dựng một phần mềm hoàn chỉnh với luồng xử lý thực tế. Hiểu sâu hơn về cách phân tách các tầng (layers) trong kiến trúc phần mềm, cách quản lý truy xuất dữ liệu an toàn, và quy trình tích hợp các dịch vụ bên ngoài (API) vào ứng dụng mà vẫn đảm bảo tốc độ phản hồi.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
Tuyệt đối không đưa các thông tin nhạy cảm như API Key, chuỗi kết nối cơ sở dữ liệu thật (Database Connection String), hay dữ liệu cá nhân lên các công cụ AI. Nhận thức rõ ràng AI là một trợ lý hỗ trợ tăng tốc công việc, không phải là người ra quyết định thay thế kỹ sư phần mềm. Mọi kết quả đầu ra đều phải được xác minh và chịu trách nhiệm bởi con người.
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
Đầu tư học thêm kỹ năng Prompt Engineering để giao tiếp với AI hiệu quả hơn, cung cấp đủ ngữ cảnh (như framework, pattern sử dụng) ngay từ câu lệnh đầu tiên để tránh AI sinh code sai lệch. Sẽ chia nhỏ các bài toán lớn thành các module nhỏ (micro-tasks) trước khi nhờ AI hỗ trợ thay vì yêu cầu AI giải quyết một luồng nghiệp vụ phức tạp cùng lúc.
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
|   | 23/5/2026 |
