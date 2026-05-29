# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Reclaim Organization Ownership                                               |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-05-29                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong đợt sửa lỗi (bug fix) luồng Reclaim Organization này, AI đóng vai trò quan trọng trong việc hỗ trợ phân tích tìm lỗi logic so khớp chuỗi thô. AI đề xuất thiết lập lớp chuẩn hóa (normalization) dữ liệu đầu vào và hỗ trợ sinh các đoạn mã kiểm thử tích hợp (integration tests) giúp rút ngắn thời gian lập trình. Tuy nhiên, nhóm vẫn đóng vai trò chính trong việc kiểm thử chất lượng thực tế và điều chỉnh các logic chưa tối ưu của AI liên quan đến tiêu chuẩn xử lý email và thiết kế trải nghiệm người dùng (UX) trên UI wizard.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [ ] Claude
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
Hỗ trợ phân tích lỗi logic nhanh chóng, có khả năng đọc và hiểu tốt cấu trúc codebase C# và React, đồng thời sinh code kiểm thử tích hợp chuẩn xác theo kiến trúc xUnit/IntegrationTest hiện tại của CVerify.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [ ] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [x] Viết test case
- [ ] Review code
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI hỗ trợ viết các hàm chuẩn hóa email và mã số thuế trong RecoveryTokenHelper.cs, điều chỉnh luồng state quản lý token trong reclaim-view.tsx để ghi nhớ token xác thực giữa các bước của wizard, và sinh ra lớp kiểm thử SubmitClaim_With_Email_Normalization_Should_Succeed để kiểm chứng tính đúng đắn của logic chuẩn hóa.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu rõ tầm quan trọng của việc chuẩn hóa các định dạng định danh (email, tax code) trước khi dùng chúng làm khóa hoặc dữ liệu so khớp trong token.
- Tiếp cận cách thiết lập và cấu trúc các bài kiểm thử tích hợp (integration tests) phức tạp liên quan đến giải mã token và mô phỏng tải tệp lên mock service một cách nhanh chóng.
- Học cách tối ưu hóa React state cho wizard multi-step nhằm nâng cao trải nghiệm người dùng (UX).
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI có xu hướng đơn giản hóa hoặc áp dụng các logic chung chung (ví dụ: loại bỏ nhãn phụ của mọi hòm thư điện tử mà không quan tâm đến tiêu chuẩn kỹ thuật riêng biệt của từng nhà cung cấp dịch vụ). Điều này có thể dẫn tới lỗi mất thư hoặc sai lệch thông tin liên lạc của người dùng trên các hệ thống email khác ngoài Gmail.
- Đôi khi code frontend do AI sinh ra chưa tối ưu hóa việc giữ lại dữ liệu form cũ khi người dùng bấm quay lại (Back), làm giảm trải nghiệm sử dụng thực tế.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm sử dụng AI như một trợ lý viết code mẫu và viết test case tự động. Các quyết định kiến trúc, tối ưu trải nghiệm và sửa đổi chi tiết kỹ thuật cho đúng chuẩn RFC/email của các bên thứ ba hoàn toàn được thực hiện thủ công bởi nhóm thông qua code review và kiểm thử thực tế.
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
Nhóm kiểm chứng bằng cách:
1. Chạy lệnh dotnet test để thực thi toàn bộ integration test suite, đặc biệt là test case mới liên quan đến chuẩn hóa email.
2. Build và khởi chạy cả client (Next.js) lẫn server (ASP.NET Core). Tiến hành điền dữ liệu chứa khoảng trắng và chữ in hoa, xác thực OTP, nhấn Back/Next để sửa đổi thông tin người đại diện, sau đó upload file chứng minh tài liệu và bấm nộp thành công.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung                           | Mô tả                                                                                                                                                                                                                                 |
| ---------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| AI đã gợi ý gì?                    | AI đề xuất hàm chuẩn hóa email bằng cách loại bỏ toàn bộ chuỗi ký tự sau dấu '+' (subaddressing) cho bất kỳ email nào được nhập vào.                                                                                                  |
| Em/nhóm đã kiểm tra bằng cách nào? | Đọc và kiểm chứng lại tiêu chuẩn kỹ thuật (RFC) của các nhà cung cấp hòm thư phổ biến. Kiểm tra thực tế xem MailKit/SMTP có gửi được thư đến địa chỉ Outlook/Yahoo bị cắt dấu '+' không.                                              |
| Kết quả kiểm tra                   | Sai lệch thực tế đối với các email không phải Gmail. Việc cắt bỏ nhãn phụ trên Outlook/Yahoo khiến email đích không thể nhận được thư (vì mail server của họ coi nhãn phụ là một phần của tên hòm thư chính thức).                    |
| Em/nhóm đã xử lý tiếp như thế nào? | Chỉnh sửa lại RecoveryTokenHelper.cs sao cho việc loại bỏ nhãn phụ và dấu chấm chỉ được thực hiện nếu phần tên miền (domain part) là 'gmail.com'. Với các tên miền khác, chỉ thực hiện đổi thành chữ thường và cắt khoảng trắng thừa. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung                          | Mô tả                                                                                                                                                                                                              |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| AI đã gợi ý gì?                   | Đề xuất reset hoàn toàn biểu mẫu của RepresentativeInfo khi người dùng chuyển về từ bước Upload tài liệu, nhằm đảm bảo dữ liệu mới nhất.                                                                           |
| Vì sao gợi ý đó sai/chưa phù hợp? | Làm suy giảm UX nghiêm trọng vì người dùng mất công nhập lại toàn bộ thông tin người đại diện từ đầu chỉ vì muốn xem lại tài liệu hoặc sửa một lỗi nhỏ.                                                            |
| Em/nhóm phát hiện bằng cách nào?  | Kiểm thử luồng thao tác thực tế (manual black-box testing) trên trình duyệt sau khi tích hợp code của AI.                                                                                                          |
| Em/nhóm đã sửa như thế nào?       | Giữ nguyên dữ liệu form cũ trong state của React, chỉ chuyển hướng ReclaimStep và thực hiện so khớp email mới nhập với email cũ. Nếu email không thay đổi thì cho phép tái sử dụng token OTP đã xác thực trước đó. |
| Bài học rút ra                    | Luôn đặt trải nghiệm người dùng thực tế và tính bảo toàn dữ liệu lên hàng đầu khi thiết kế các form wizard nhiều bước.                                                                                             |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
- Hoàn thiện RecoveryTokenHelper.cs với các quy tắc chuẩn hóa phù hợp cho Gmail và các nhà cung cấp khác.
- Cấu trúc lại state management của ReclaimView trên client để đảm bảo trải nghiệm Back/Next mượt mà và bảo toàn dữ liệu.
- Phân tích log, xác minh tính đúng đắn của token xác thực và trực tiếp chạy integration test suite.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung      | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được                           |
| ------------- | ----------------- | --------------- | -------------------------------------------- |
| Coding Speed  | Average           | Very Fast       | Viết code chuẩn hóa và test case nhanh chóng |
| Documentation | Average           | Fast            | Ghi chép tài liệu rõ ràng, chuyên nghiệp     |
| Testing       | Slow              | Basic           | Tạo lập test case tự động chuẩn xác          |

---

## 11. Bài học về môn học

- Tầm quan trọng của việc chuẩn hóa dữ liệu định danh sớm.
- Kiểm thử tích hợp tự động giúp phát hiện nhanh các lỗi mismatch chuỗi ở các lớp nghiệp vụ.
- Thiết kế luồng đi qua các wizard cần cân bằng giữa tính bảo mật (token expire) và trải nghiệm người dùng (UX).

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Tuyệt đối không copy-paste code từ AI mà không hiểu rõ tác động của nó đối với các chuẩn kỹ thuật liên quan (như RFC email).
- Phải luôn viết các ca kiểm thử độc lập để đối chiếu kết quả do AI đề xuất.
- Đánh giá mã nguồn do AI sinh ra dưới góc độ bảo mật thông tin (logging, rò rỉ token).

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

- Tận dụng AI để sinh nhiều kịch bản test case biên (edge-case testing) hơn nữa.
- Nâng cao kỹ năng viết prompt có cấu trúc chặt chẽ để giảm thiểu lỗi sai lệch kỹ thuật từ AI.
- Đảm bảo tính nhất quán của giao diện và logic xác thực trên tất cả các luồng đăng ký/đăng nhập.

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí                         | Điểm tự đánh giá 1-5 | Ghi chú |
| -------------------------------- | :------------------: | ------- |
| Ghi nhận việc dùng AI trung thực |          5           |         |
| Prompt có mục tiêu rõ ràng       |          5           |         |
| Kiểm chứng kết quả AI            |          5           |         |
| Tự chỉnh sửa/cải tiến            |          5           |         |
| Hiểu nội dung đã nộp             |          5           |         |
| Reflection có chiều sâu          |          5           |         |
| Sử dụng AI có trách nhiệm        |          5           |         |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm nắm rõ cách thức hoạt động của RecoveryTokenHelper, luồng giải mã token trên backend và state management lưu giữ token xác thực ở client.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Tuy nhiên quá trình viết integration test mô phỏng payload multipart-form-data và mock R2 upload sẽ mất nhiều thời gian tra cứu tài liệu hơn.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần phát hiện lỗi logic chuẩn hóa email chung chung của AI và sửa đổi nó để tuân thủ đúng tiêu chuẩn của các nhà cung cấp hòm thư khác nhau, cùng với việc tinh chỉnh UX nút Back/Next trên client.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Kỹ năng viết Integration Test chất lượng cao và thiết kế các hệ thống phân tán chịu lỗi tốt.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-29    |
