# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie - Backend |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-05-15 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
- Thiết kế & Phân tích: Sử dụng ChatGPT và Claude để xây dựng kiến trúc xác thực với Refresh Token Rotation và Permission-based Authorization.
- Thực thi & Refactor: Gemini hỗ trợ sinh code cho Dapper Repository, Redis Cache và refactor Program.cs sang Extension methods để tăng tốc phát triển backend.
- Kiểm định & Tối ưu: Antigravity được dùng để rà soát bảo mật và race condition, kết hợp với manual review để đảm bảo mã nguồn ổn định và phù hợp với PostgreSQL cũng như yêu cầu thực tế của dự án.
```

---

## 4. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Gemini
```

### Lý do sử dụng công cụ đó

```text
Gemini được chọn làm công cụ AI chính nhờ khả năng xử lý ngữ cảnh dài và hỗ trợ tốt cho hệ sinh thái .NET. Công cụ này đặc biệt hiệu quả trong việc gợi ý các truy vấn SQL phức tạp cho Dapper, mapping dữ liệu giữa PostgreSQL và Domain Entities, đồng thời hỗ trợ triển khai mã nguồn nhanh với ít lỗi logic hơn trong quá trình phát triển.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [x] Viết test case
- [x] Review code
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [x] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
- Thiết kế & Phân tích: AI hỗ trợ xây dựng kiến trúc xác thực nhiều lớp với JWT, Refresh Token Rotation và Permission-based Authorization.
- Coding & Refactor: Hỗ trợ sinh boilerplate code bằng Dapper và refactor Program.cs sang Extension methods để mã nguồn gọn gàng hơn.
- Bảo mật & Tối ưu: Hỗ trợ kiểm tra race condition, rà soát bảo mật và xây dựng EnvValidator cho hệ thống.
- Kiểm thử & Báo cáo: Hỗ trợ viết test case, tổng hợp thay đổi mã nguồn và tạo tài liệu kỹ thuật nhanh chóng.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
- Tiếp cận kiến thức nhanh hơn: AI giúp em hiểu các khái niệm như Refresh Token Rotation và Permission-based Authorization thông qua ví dụ thực tế thay vì chỉ đọc lý thuyết.
- Học hỏi Best Practices: Qua các gợi ý refactor và tổ chức mã nguồn, em học được cách xây dựng hệ thống sạch sẽ, dễ mở rộng và dễ bảo trì hơn.
- Nâng cao kỹ năng Debug: AI hỗ trợ phân tích race condition và các lỗi tiềm ẩn, giúp em hiểu sâu hơn về cách hệ thống vận hành thực tế và tối ưu bằng Redis.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- Thông tin chưa luôn cập nhật: Một số gợi ý về thư viện hoặc cách triển khai đôi khi chưa phù hợp với phiên bản .NET mới nhất, nên vẫn cần tự kiểm chứng và điều chỉnh thêm.
- Giải pháp mang tính tham khảo: AI thường tạo boilerplate code ở mức cơ bản, vì vậy cần có kiến thức nền để đánh giá và tối ưu lại về bảo mật cũng như hiệu năng.
- Phụ thuộc vào Prompting: Chất lượng kết quả phụ thuộc khá nhiều vào cách đặt câu hỏi; prompt chưa rõ ràng dễ khiến AI trả về thông tin chung chung hoặc thiếu chính xác.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [ ] Phụ thuộc ít
- [x] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Em chọn mức độ này vì AI đóng vai trò như một trợ lý hỗ trợ tăng tốc quá trình phát triển, đặc biệt ở các phần boilerplate code và gợi ý hướng giải quyết vấn đề. Tuy nhiên, toàn bộ kết quả từ AI đều được em tự rà soát, chỉnh sửa và kiểm chứng lại để phù hợp với kiến trúc cũng như yêu cầu bảo mật của dự án. AI hỗ trợ tiết kiệm thời gian, nhưng quyết định cuối cùng vẫn do em thực hiện.
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
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
- Static Review: Rà soát thủ công mã nguồn AI tạo ra để đảm bảo đúng logic nghiệp vụ và không vi phạm các nguyên tắc bảo mật như hard-code secret.
- Dynamic Testing: Chạy hệ thống local và dùng Postman để kiểm tra các luồng như Login, Refresh Token và Authorization với dữ liệu thực tế trên PostgreSQL và Redis.
- Edge-case Testing: Kiểm thử các tình huống như đăng nhập sai nhiều lần, token hết hạn hoặc token giả để xác nhận cơ chế lockout và JWT validation hoạt động đúng.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Gợi ý cấu hình lưu trữ Refresh Token vào LocalStorage phía Client để dễ dàng truy xuất. |
| Em/nhóm đã kiểm tra bằng cách nào? | Đối chiếu với các tài liệu bảo mật chính thống (OWASP) và thảo luận nhóm về rủi ro tấn công XSS. |
| Kết quả kiểm tra | Sai/Chưa phù hợp. Lưu vào LocalStorage khiến hệ thống dễ bị đánh cắp token qua mã độc Javascript. |
| Em/nhóm đã xử lý tiếp như thế nào? | Yêu cầu AI viết lại logic để lưu Refresh Token vào HttpOnly Cookie và cấu hình flag Secure, SameSite để tối ưu bảo mật. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Gợi ý đoạn code tăng biến AccessFailedCount bằng cách: Đọc giá trị từ DB -> Cộng 1 -> Lưu lại DB. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Nếu 2 yêu cầu đăng nhập sai đến cùng lúc, giá trị lưu lại có thể chỉ tăng 1 thay vì 2. |
| Em/nhóm phát hiện bằng cách nào? | Qua việc Review code và phân tích logic luồng xử lý đa luồng (Concurrency). |
| Em/nhóm đã sửa như thế nào? | Triển khai Distributed Lock với Redis để đảm bảo quá trình đọc-ghi số lần đăng nhập sai là (atomic). |
| Bài học rút ra | Không nên tin tưởng các logic cập nhật trạng thái đơn giản của AI trong môi trường phân tán; luôn cần cơ chế khóa (locking). |
| AI đã gợi ý gì? | Gợi ý câu lệnh SQL JOIN và mapping tự động của Dapper cho kiểu dữ liệu List<Permission>. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Dapper không tự động hiểu cách map một chuỗi kết quả phẳng (flat result) thành một cấu trúc phân cấp (Nested Object) nếu không dùng hàm multi-mapping cụ thể. |
| Em/nhóm phát hiện bằng cách nào? | Khi chạy thử chương trình, danh sách Permission luôn trả về rỗng (null) mặc dù SQL chạy đúng. |
| Em/nhóm đã sửa như thế nào? | Tự viết lại hàm mapping sử dụng một Dictionary tạm để gộp các Permission vào đúng User/Role tương ứng trong quá trình lặp kết quả. |
| Bài học rút ra | AI thường mạnh về SQL nhưng thường bỏ qua các chi tiết kỹ thuật nhỏ trong việc xử lý kiểu dữ liệu đặc thù của thư viện. |
---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
Đóng góp chính của tôi tập trung vào thiết kế kiến trúc hệ thống và tích hợp hạ tầng hơn là chỉ viết code chức năng. trực tiếp xây dựng cấu trúc Extension Methods để module hóa hệ thống, triển khai Environment Validator, tối ưu truy vấn Dapper cho PostgreSQL và đưa ra quyết định sử dụng HttpOnly Cookie cho Refresh Token nhằm tăng cường bảo mật. Những phần này đều được tùy chỉnh dựa trên yêu cầu thực tế của dự án
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Thiết lập hạ tầng (Infrastructure Setup) | Mất nhiều thời gian để tự cấu hình Redis, Dapper và JWT Middleware, dễ gặp lỗi cấu hình. | AI hỗ trợ tạo nhanh boilerplate code và cấu hình chuẩn cho backend .NET. | Tiết kiệm đáng kể thời gian setup và giảm lỗi cơ bản trong quá trình triển khai. |
| Hệ thống phân quyền (Authorization) | Chỉ sử dụng Role-based Authorization đơn giản do khó triển khai policy phức tạp. | AI hỗ trợ xây dựng Permission-based Authorization với các handler và policy riêng. | Hệ thống phân quyền linh hoạt hơn và phù hợp với mô hình enterprise. |
| Debug & Tối ưu hóa | Việc debug race condition hoặc lỗi mapping dữ liệu khá mất thời gian và khó phát hiện. | AI hỗ trợ phân tích mã nguồn và gợi ý các edge case tiềm ẩn trong hệ thống. | Tăng độ ổn định, phát hiện sớm lỗi kiến trúc và cải thiện chất lượng mã nguồn trước khi deploy. |

---

## 11. Bài học về môn học

```text
 
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
| Nguyễn Hoàng Ngọc Ánh | 16/5/2026 |
