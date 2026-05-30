# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Gmail Normalization Correction, Multi-Email Support & Password Recovery      |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-05-31                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình khắc phục lỗi chuẩn hóa email Google OAuth và triển khai các chức năng đa email cùng Password Recovery, AI đóng vai trò như một cộng sự hỗ trợ phân tích code, định vị nhanh các thành phần cần thay đổi chính sách chuẩn hóa và sinh mã boilerplate cho các dịch vụ mới. Tuy nhiên, nhóm nắm vai trò then chốt trong việc kiểm soát tính an toàn mật mã học, thiết kế luồng di trú dữ liệu có tính chất bảo vệ xung đột và tích hợp re-authentication bắt buộc đối với tác vụ hoán đổi email để tránh lỗ hổng bảo mật.
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
Antigravity có khả năng đọc hiểu cấu trúc toàn dự án CVerify nhanh chóng, có thể đối chiếu các tệp tin backend với các component frontend Next.js 16 và đưa ra các đề xuất tích hợp có độ tương thích cao.
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
- [ ] Review code
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI hỗ trợ viết cấu trúc di trú cơ sở dữ liệu và hoán đổi email, phác thảo UI cho Connected Accounts và password recovery form ở frontend. Đồng thời đề xuất tách biệt Interface GoogleTokenValidator để thuận tiện cho việc mock và unit test luồng Google OAuth.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu sâu hơn về các vấn đề định danh và chuẩn hóa email trong SSO/OAuth.
- Nắm bắt quy trình thiết kế tính năng liên kết danh tính đa nguồn (Linked Emails) và các nghiệp vụ hoán đổi thông tin định danh chính trong DB an toàn thông qua transactions.
- Học cách cấu trúc lại code (refactoring) bằng cách sử dụng các interfaces để tăng khả năng kiểm thử tự động (testability).
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đề xuất các thay đổi trực tiếp trên dữ liệu DB (data migration) mà không lường trước nguy cơ xung đột khóa độc nhất (unique constraint conflict) khi có email trùng lặp.
- AI bỏ qua các bước kiểm tra xác thực lại mật khẩu (re-authentication) cho các hành động có mức độ nhạy cảm cao như đổi email chính, tạo ra nguy cơ bảo mật lớn cho tài khoản người dùng.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm chủ yếu sử dụng AI để tăng tốc viết mã boilerplate và dựng layout UI ban đầu. Toàn bộ logic kiểm tra bảo mật, logic transaction hoán đổi email phụ và các biện pháp bảo vệ xung đột di trú dữ liệu hoàn toàn do nhóm tự nghiên cứu và cài đặt.
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
Nhóm kiểm chứng thông qua:
1. Chạy bộ kiểm thử tự động của hệ thống `dotnet test` vượt qua toàn bộ 71/71 ca kiểm thử, bao gồm các bài test hồi quy cho email có dấu chấm và subaddressing.
2. Kiểm thử thủ công trên giao diện: Liên kết email phụ mới, nhận và xác thực mã OTP, sau đó hoán đổi làm email chính, kiểm tra dữ liệu thay đổi chuẩn xác trong PostgreSQL.
3. Kiểm tra bảo mật: Xác nhận rằng nếu nhập sai mật khẩu hiện tại, hệ thống lập tức từ chối yêu cầu thăng cấp email phụ làm email chính.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý viết trực tiếp email mới vào users.email khi di trú dữ liệu Google OAuth cũ mà không kiểm tra trùng lặp email. |
| Em/nhóm đã kiểm tra bằng cách nào? | Đọc kỹ mã nguồn và chạy thử trên DB cục bộ có sẵn một số tài khoản password trùng email gốc của tài khoản Google OAuth. |
| Kết quả kiểm tra | Quá trình di trú dữ liệu gặp lỗi ném ngoại lệ Unique Constraint Violation và làm crash ứng dụng khi khởi động. |
| Em/nhóm đã xử lý tiếp như thế nào? | Bổ sung logic kiểm tra sự tồn tại của email đích trên bảng users trước khi thực hiện cập nhật trong DbInitializer.cs, nếu trùng thì in cảnh báo và bỏ qua bản ghi đó. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Cho phép đổi email chính trực tiếp mà không cần người dùng nhập lại mật khẩu xác thực tài khoản. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Gây ra lỗ hổng bảo mật nghiêm trọng (Broken Authentication). Nếu người dùng rời khỏi màn hình mà không khóa máy, kẻ xấu có thể dễ dàng liên kết email phụ của họ và thăng cấp thành email chính để chiếm đoạt tài khoản hoàn toàn. |
| Em/nhóm phát hiện bằng cách nào? | Rà soát thiết kế an toàn thông tin (Security Review) đối với các endpoints đổi thông tin bảo mật. |
| Em/nhóm đã sửa như thế nào? | Bổ sung trường Password vào request MakePrimaryEmailRequest và thực hiện hàm VerifyPassword ở backend trước khi cho phép hoán đổi email. |
| Bài học rút ra | Bất kỳ hành động thay đổi thông tin xác thực chính (email, mật khẩu, 2FA) đều bắt buộc phải yêu cầu re-authentication (nhập lại mật khẩu hoặc OTP) để bảo vệ người dùng. |

---

## 9. Phân đóng góp thật sự của sinh viên/nhóm

```text
- Thiết lập logic hoán đổi danh tính nguyên tử trong Database Transaction.
- Thiết kế cơ chế re-authentication bằng mật khẩu khi promoting email phụ.
- Tự triển khai logic di trú an toàn chống xung đột trùng lặp email trong DbInitializer.cs.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Rút ngắn 50% thời gian dựng form và các API controllers. |
| Code Quality | Good | Excellent | Code sạch hơn nhờ phân tách Interface cho GoogleTokenValidator. |
| Testing | Good | Excellent | Tích hợp thành công nhiều kịch bản test case tự động bao phủ cả email dị biệt. |

---

## 11. Bài học về môn học

- Chuẩn hóa dữ liệu đầu vào (Input Normalization) là con dao hai lưỡi; cần thiết kế bảo toàn tối đa danh tính từ Identity Provider để tránh xung đột danh tính.
- Các nghiệp vụ đổi email và mật khẩu liên quan mật thiết đến bảo mật hệ thống, bắt buộc phải tuân thủ quy chuẩn phòng thủ chiều sâu (defense in depth).

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Tuyệt đối không copy-paste code từ AI mà không hiểu rõ từng dòng lệnh, đặc biệt là các phần code liên quan đến database schema, migrations hay phân quyền bảo mật.
- Luôn đặt câu hỏi phản biện về các lỗ hổng bảo mật tiềm ẩn trong code do AI sinh ra.

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

- Thiết lập cấu trúc unit test tự động chạy ngay sau mỗi lần sinh code bằng AI để kiểm chứng tức thì tính đúng đắn.

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
Có. Nhóm hoàn toàn giải thích được luồng OTP khôi phục mật khẩu, cơ chế hoán đổi email bằng transaction và thuật toán chuẩn hóa email mới.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc viết các endpoints API và tích hợp Entity Framework hoàn toàn nằm trong khả năng của nhóm khi tra cứu tài liệu MSDN chính thống.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-31    |
