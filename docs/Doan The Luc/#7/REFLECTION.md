# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Secure OAuth Integration & Settings Change Password Overhaul                  |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-05-30                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong đợt phát triển tính năng liên kết tài khoản OAuth an toàn và đổi mật khẩu trên trang Cài đặt, AI đóng vai trò là một trợ lý đắc lực hỗ trợ sinh boilerplate mã hóa AES-256-GCM ở backend và dựng giao diện Settings hiện đại với HeroUI v3 ở frontend. Tuy nhiên, nhóm đóng vai trò cốt lõi trong việc rà soát kiến trúc bảo mật (loại bỏ phương án sinh khóa ngẫu nhiên khi thiếu cấu hình), debug khắc phục lỗi tuần tự khởi tạo schema database trong DbInitializer.cs khi chạy bộ integration tests tự động, và quản lý vòng đời refresh tokens để thu hồi toàn bộ session thiết bị khác khi thay đổi mật khẩu.
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
Antigravity có khả năng phân tích ngữ cảnh codebase rất nhanh, hỗ trợ đọc hiểu luồng di trú dữ liệu phức tạp của DbInitializer và sinh mã nguồn mẫu cho lớp mã hóa AES-256-GCM chuẩn xác hơn các mô hình chat thông thường.
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
AI hỗ trợ viết cấu trúc mã nguồn mã hóa đối xứng AES-256-GCM trong TokenEncryptionService.cs, dựng giao diện Settings Profile với cấu trúc component Tab hiện đại của HeroUI v3 ở frontend. Đặc biệt, AI giúp nhanh chóng định vị lỗi tuần tự hóa câu lệnh ALTER TABLE bảng user_profiles khi bảng chưa được tạo trong DbInitializer.cs.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu và áp dụng thành thạo thuật toán mã hóa đối xứng AES-256-GCM kết hợp tách biệt lưu trữ cipherText, initialization vector (nonce) và authentication tag trong .NET Core.
- Tiếp cận kỹ năng quản lý phiên làm việc bảo mật cao (Active Session Invalidation) thông qua cơ chế thu hồi refresh tokens khi đổi mật khẩu tài khoản.
- Nắm bắt phương pháp sửa lỗi tuần tự hóa khởi tạo schema cơ sở dữ liệu trên các container kiểm thử sạch từ đầu.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đôi khi đề xuất các giải pháp thiết kế "tiện lợi" nhưng vi phạm nguyên tắc bảo mật. Ví dụ: AI gợi ý sinh ngẫu nhiên khóa mã hóa tạm thời khi thiếu khóa cấu hình TokenEncryptionKey ở khởi chạy, dẫn đến việc mất toàn bộ khả năng giải mã dữ liệu sau khi ứng dụng khởi động lại.
- AI không bao quát hết thứ tự khởi tạo của các script DDL SQL lớn chạy lúc runtime, dẫn đến lỗi crash di trú cơ sở dữ liệu trên DB trống khi chạy integration test suite.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm chủ yếu dùng AI để phác thảo các khung boilerplate mật mã học và UI layout. Các bước kiểm chứng logic nghiệp vụ đổi mật khẩu, xử lý thu hồi session trên các thiết bị khác, sửa lỗi tuần tự DDL và cấu hình bảo mật ứng dụng hoàn toàn do nhóm nghiên cứu thực hiện.
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
1. Chạy 106 bài kiểm thử tự động của hệ thống `dotnet test CVerify.sln` để đảm bảo cơ sở dữ liệu của các integration test container được tạo thành công và DbInitializer không còn ném ra ngoại lệ.
2. Kiểm tra tính năng mã hóa OAuth Token: Liên kết tài khoản Google/GitHub rồi truy cập PostgreSQL xem dữ liệu trong bảng oauth_credentials, xác nhận access_token và refresh_token đều được mã hóa dưới dạng chuỗi Base64 thay vì plaintext.
3. Kiểm thử Change Password: Đăng nhập tài khoản trên hai trình duyệt khác nhau, thực hiện đổi mật khẩu ở trình duyệt một và kiểm tra xem trình duyệt hai có bị tự động đăng xuất do refresh token cũ bị thu hồi hay không.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Viết script ALTER TABLE thêm cột cho bảng user_profiles trong DbInitializer.cs mà không kiểm tra xem bảng đó đã được định nghĩa và tạo trước đó hay chưa. |
| Em/nhóm đã kiểm tra bằng cách nào? | Chạy lệnh `dotnet test CVerify.sln` để kiểm tra kết quả bộ test suite tích hợp. |
| Kết quả kiểm tra | Thất bại nghiêm trọng: Trình khởi tạo ném ra Postgres Exception thông báo relation "user_profiles" does not exist và làm sập toàn bộ các bước tạo bảng phía sau như otp_verifications. |
| Em/nhóm đã xử lý tiếp như thế nào? | Bổ sung mệnh đề kiểm tra `IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'user_profiles')` bọc quanh khối ALTER TABLE, đưa câu lệnh ALTER TABLE vào đúng thứ tự sau khi bảng đã tồn tại, và chạy lại bộ test thành công 106/106 bài. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Trong TokenEncryptionService, AI viết logic: nếu TokenEncryptionKey trống hoặc null, ứng dụng sẽ tự động sinh khóa ngẫu nhiên mới bằng `RandomNumberGenerator` để tiếp tục chạy. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Cực kỳ nguy hiểm. Khóa mã hóa sinh ngẫu nhiên trên RAM ở mỗi lần khởi chạy sẽ thay đổi liên tục. Khi ứng dụng restart hoặc scale-out sang nhiều instances, việc giải mã các tokens OAuth đã lưu trong database bằng khóa cũ sẽ bị lỗi giải mã (CryptographicException), làm hỏng luồng hoạt động của người dùng. |
| Em/nhóm phát hiện bằng cách nào? | Đọc kỹ mã nguồn do AI gợi ý trong TokenEncryptionService.cs và rà soát thiết kế bảo mật trước khi tích hợp. |
| Em/nhóm đã sửa như thế nào? | Loại bỏ hoàn toàn logic sinh khóa ngẫu nhiên. Thay vào đó, kiểm tra chặt chẽ cấu hình TokenEncryptionKey lúc startup tại Program.cs, nếu không tìm thấy khóa thì ném ngoại lệ dừng khởi động ứng dụng kèm thông báo bảo mật rõ ràng. |
| Bài học rút ra | Các hệ thống mật mã học bắt buộc phải sử dụng các khóa mã hóa tĩnh, bảo mật và đồng nhất (đọc từ Secrets Manager hoặc Environment Variables), tuyệt đối không dùng khóa sinh ngẫu nhiên động tại runtime cho mã hóa dữ liệu cần lưu trữ lâu dài. |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
- Định hình mô hình dữ liệu oauth_credentials liên kết 1:1 bảo mật với auth_providers thay vì gộp chung plaintext vào bảng chính.
- Khắc phục triệt để lỗi tuần tự hóa DDL trong DbInitializer.cs, bảo đảm môi trường kiểm thử CI/CD của dự án hoạt động ổn định.
- Tự triển khai cơ chế thu hồi refresh tokens trên các thiết bị khác bằng việc vô hiệu hóa khóa của người dùng trong TokenService.cs khi đổi mật khẩu thành công.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Rút ngắn 60% thời gian triển khai lớp mã hóa phức tạp và dựng UI tab Settings. |
| Architecture | Good | Excellent | Đảm bảo tính cô lập và bảo mật cao thông qua mã hóa AES-256-GCM ứng dụng. |
| UX Design | Average | Premium | Giao diện cài đặt và quản lý liên kết tài khoản mượt mà, đồng bộ chuẩn HeroUI v3. |

---

## 11. Bài học về môn học

- Các khía cạnh bảo mật như mã hóa dữ liệu nhạy cảm (Tokens) và quản lý phiên (Session Revocation) phải được thiết kế đồng bộ từ đầu (Security by Design).
- Thứ tự thực thi DDL (Data Definition Language) rất nhạy cảm và dễ đổ vỡ trong môi trường database trống của các bài kiểm thử tích hợp nếu không được kiểm soát tuần tự.
- Việc đồng bộ hóa thư viện UI (HeroUI v3) giúp giao diện nhất quán, giảm thiểu độ lệch mã nguồn giữa các màn hình của dự án.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Không bao giờ tin tưởng hoàn toàn vào các đề xuất tự động của AI, đặc biệt là các giải pháp liên quan đến bảo mật và mật mã học.
- Luôn rà soát kỹ logic sinh khóa, quản lý tài nguyên và cấu hình môi trường để tránh nợ kỹ thuật hoặc lỗ hổng bảo mật nghiêm trọng.
- Rà soát tính chính xác của mã nguồn sinh ra bằng các bộ công cụ kiểm thử tự động (tests) trên môi trường sạch trước khi bàn giao mã nguồn.

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

- Thiết kế các prompt định rõ tiêu chuẩn an ninh và ràng buộc kiểm thử ngay từ đầu để AI đưa ra các đoạn mã an toàn hơn.
- Tiếp tục viết thêm các bộ Unit Test phủ kín các dịch vụ mã hóa và đổi mật khẩu để phát hiện sớm các thay đổi không tương thích.

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
Có. Nhóm hoàn toàn giải thích được cách thức hoạt động của TokenEncryptionService (sử dụng AesGcm với IV, cipherText và tag đóng gói Base64), logic kiểm tra TokenEncryptionKey lúc khởi chạy ứng dụng, và cách DbInitializer.cs được sửa lỗi thông qua việc kiểm tra cấu trúc bảng.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Phần mã hóa AES-256-GCM và cấu hình DbInitializer nhóm hoàn toàn có thể tự triển khai bằng cách tra cứu tài liệu mật mã chuẩn của .NET và cú pháp SQL của PostgreSQL, dù thời gian viết code sẽ lâu hơn.
```

### 16.3. Phân nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần thiết lập kiến trúc mã hóa an toàn ở tầng ứng dụng (.NET application layer), thiết kế mô hình cơ sở dữ liệu tách biệt để tránh rò rỉ token, và trực tiếp sửa chữa logic tuần tự trong DbInitializer.cs giúp hệ thống integration tests chạy thông suốt.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Nhóm muốn nâng cao kỹ năng triển khai các giải pháp bảo mật nâng cao khác như mã hóa khóa bất đối xứng, quản lý khóa thông qua Azure Key Vault / AWS KMS, và tối ưu hóa hiệu năng truy vấn dữ liệu đã được mã hóa.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-30    |
