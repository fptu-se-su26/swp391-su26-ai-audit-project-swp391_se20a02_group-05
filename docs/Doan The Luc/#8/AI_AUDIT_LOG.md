# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - Gmail Normalization Correction, Multi-Email Support & Password Recovery      |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-05-31T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-31T02:00:00.000Z                                                               |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Khắc phục lỗi chuẩn hóa email (Gmail normalization policy) làm biến đổi email Google OAuth; thiết lập cơ chế quản lý đa email (multi-email) cho phép liên kết, hủy liên kết và thăng cấp email phụ lên email chính; xây dựng luồng khôi phục mật khẩu (Password Recovery) bằng mã OTP và đồng bộ hóa password hash của User với PasswordCredentials.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-31                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Phân tích lỗi chuẩn hóa Gmail làm mất dấu chấm trong chuỗi email Google OAuth và tìm cách chỉnh sửa chính sách chuẩn hóa.                              |
| Phần việc liên quan | Backend / Authentication / Normalization                                                                                                               |
| Mức độ sử dụng      | Hỗ trợ giải pháp chuẩn hóa mới và thiết kế lớp tương thích ngược.                                                                                      |

#### 4.1. Prompt đã sử dụng

```text
Google OAuth Email format: theluc.1746@gmail.com được lưu thành theluc1746@gmail.com trong CVerify. Dấu chấm bị loại bỏ do chính sách chuẩn hóa cũ của Gmail. Hãy tìm nơi email bị biến đổi và thiết kế chính sách chuẩn hóa email mới bảo toàn nguyên vẹn email của Identity Provider, đồng thời tạo cơ chế tương thích ngược (fallback) cho các tài khoản cũ.
```

#### 4.2. Kết quả AI gợi ý

```text
AI xác định việc chuẩn hóa Gmail (loại bỏ dấu chấm và phần subaddressing sau dấu cộng) diễn ra ở ba nơi: AuthService.NormalizeEmailPolicy, IdentityStateResolver.NormalizeEmail, và RecoveryTokenHelper.NormalizeEmail. 
AI đề xuất đổi chính sách chuẩn hóa chung thành: trim() + Unicode NFC normalization + lowercase. Đồng thời, xây dựng một lớp LegacyEmailCompatibilityHelper chứa thuật toán chuẩn hóa cũ để hỗ trợ tìm kiếm tài khoản fallback khi đăng nhập nếu không tìm thấy email gốc.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Thuật toán chuẩn hóa mới: Trim().Normalize(NormalizationForm.FormC).ToLowerInvariant()
- Cấu trúc lớp LegacyEmailCompatibilityHelper chứa hàm ApplyOldGmailNormalization để áp dụng chuẩn hóa cũ khi cần tìm kiếm dự phòng.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Viết một script chạy migration tự động một lần trong DbInitializer.cs có tên MigrateLegacyGoogleEmailsAsync để cập nhật lại email gốc cho các tài khoản Google OAuth từ cột provider_account_id trong bảng auth_providers.
- Thêm cơ chế kiểm tra xung đột (conflict protection): Nếu email gốc của tài khoản Google đã được đăng ký bởi một tài khoản mật khẩu khác, hệ thống sẽ bỏ qua không cập nhật để tránh vi phạm ràng buộc độc nhất (unique constraint) trên database.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): fix email normalization, add multi-email support and password recovery | https://github.com/Kaivian/CVerify/commit/caed6cc966c813a3036495db34ff3db89d554a93 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc thay đổi chính sách định danh tài khoản giữa chừng luôn ẩn chứa rủi ro xung đột dữ liệu. Giải pháp di trú dữ liệu kết hợp cơ chế kiểm tra xung đột là cách tối ưu để bảo vệ tính toàn vẹn của dữ liệu hiện tại mà vẫn sửa đổi được lỗi hệ thống.
```

---

### Lần sử dụng AI số 2

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-31                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Thiết kế cấu trúc cơ sở dữ liệu và endpoints API cho tính năng quản lý nhiều email phụ (multi-email support).                                          |
| Phần việc liên quan | Backend / Database / Web API                                                                                                                           |
| Mức độ sử dụng      | Hỗ trợ phác thảo schema cơ sở dữ liệu và logic controllers.                                                                                            |

#### 4.1. Prompt đã sử dụng

```text
Design the multi-email database schema and link/unlink/make-primary API endpoints. Users can link up to 3 verified email addresses. Promoted primary email swaps with the old primary.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất bảng user_emails chứa thông tin email phụ (id, user_id, email, is_verified, verified_at, created_at) với index độc nhất trên cột email. 
Đồng thời phác thảo các endpoint:
- POST api/auth/emails/link: Gửi OTP, xác thực và lưu email phụ.
- POST api/auth/emails/make-primary: Xác thực mật khẩu, swap email phụ lên làm email chính trong bảng users, chuyển email chính cũ xuống bảng user_emails.
- DELETE api/auth/emails/{id}: Xóa liên kết email phụ.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Schema bảng user_emails và cấu hình liên kết Entity Framework Core (1-N giữa User và UserEmail).
- Logic hoán đổi email chính - phụ sử dụng Database Transaction để đảm bảo tính nguyên tử (atomic swap).
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Bổ sung kiểm tra TOCTOU trước khi ghi nhận email phụ: Thực hiện kiểm tra lại sự tồn tại của email trên cả bảng users và user_emails trước khi xác thực OTP thành công để tránh đăng ký trùng lặp do độ trễ mạng.
- Áp dụng kiểm tra số lượng email tối đa (giới hạn tối đa 2 email phụ, tức tổng cộng 3 email bao gồm cả email chính).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): fix email normalization, add multi-email support and password recovery | https://github.com/Kaivian/CVerify/commit/caed6cc966c813a3036495db34ff3db89d554a93 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Thực thi các tác vụ đổi email định danh chính đòi hỏi tính an toàn cực cao. Việc bọc toàn bộ quá trình hoán đổi trong một Transaction đảm bảo nếu có bất cứ lỗi nào xảy ra ở giữa, hệ thống sẽ khôi phục lại trạng thái cũ, tránh tình trạng tài khoản bị mất email chính.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                           |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ----------------------------------------------------------------- |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích lỗi Gmail normalization và thiết kế khôi phục mật khẩu. |
| Viết user story/use case    |       x       |              |                 |               |                                                                   |
| Thiết kế database           |               |      x       |                 |               | Tạo bảng user_emails quản lý các email phụ.                        |
| Thiết kế kiến trúc hệ thống |               |      x       |                 |               | Tích hợp cơ chế liên kết danh tính đa nguồn (Linked Emails).      |
| Thiết kế giao diện          |               |              |        x        |               | Overhaul trang Settings tích hợp email phụ và khôi phục mật khẩu. |
| Code frontend               |               |              |        x        |               | Xây dựng component SignInMethod và các form đổi mật khẩu.         |
| Code backend                |               |              |        x        |               | Triển khai PasswordRecoveryService và AuthController API mới.     |
| Debug lỗi                   |               |      x       |                 |               | Sửa lỗi chuẩn hóa email trong tích hợp kiểm thử.                  |
| Viết test case              |               |              |        x        |               | Viết thêm EmailManagementTests và PasswordRecoveryTests.          |
| Kiểm thử sản phẩm           |       x       |              |                 |               | Chạy test suite tích hợp với 71 tests passed.                     |
| Tối ưu code                 |       x       |              |                 |               |                                                                   |
| Viết báo cáo                |       x       |              |                 |               |                                                                   |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI gợi ý logic di trú dữ liệu thô mà không kiểm tra trùng lặp email trên database thực tế. | Kiểm tra mã nguồn thấy nếu một email Google OAuth gốc trùng khớp với một tài khoản password sẵn có, di trú thô sẽ kích hoạt lỗi trùng khóa chính (unique key constraint error). | Thêm bước kiểm tra sự tồn tại của email đích trên bảng `users` trước khi cho phép cập nhật dữ liệu trong vòng lặp di trú. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả thông qua:
1. Chạy thành công bộ test suite kiểm thử tích hợp: `dotnet test` vượt qua toàn bộ 71/71 ca kiểm thử, bao gồm cả các ca kiểm thử hồi quy mới về lưu trữ email có ký tự đặc biệt và đăng nhập fallback.
2. Kiểm chứng di trú dữ liệu thành công trên môi trường DB khi khởi chạy: Google OAuth email khôi phục lại các dấu chấm nguyên bản từ auth_providers.
3. Thử nghiệm trên UI: Người dùng liên kết email phụ thành công, thực hiện xác thực mã OTP gửi về email phụ, và hoán đổi email phụ lên làm email chính mượt mà.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Trực tiếp sửa đổi chính sách NormalizeEmailPolicy tại AuthService.cs và thiết kế cơ chế fallback của IdentityStateResolver.cs.
- Tự tay viết mã di trú dữ liệu an toàn MigrateLegacyGoogleEmailsAsync trong DbInitializer.cs.
- Thiết kế giao diện quản lý liên kết Email phụ với HeroUI v3 đảm bảo tính responsive và thân thiện với người dùng.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                            | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Sửa lỗi email normalization, phát triển đa email và Password Recovery API | Có                   | https://github.com/Kaivian/CVerify/commit/caed6cc966c813a3036495db34ff3db89d554a93 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm thử tích hợp UI luồng đổi mật khẩu và quản lý email phụ               | Không                |                     |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp định vị nhanh các file chứa hàm chuẩn hóa email cũ và sinh khung boilerplate cho PasswordRecoveryService một cách nhanh chóng.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng đề xuất di trú dữ liệu trực tiếp không kiểm tra xung đột của AI, do nó sẽ làm sập DB khởi động nếu có dữ liệu trùng lặp từ trước.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Kiểm tra bằng cách chạy dotnet test cục bộ kết hợp viết bổ sung các test cases kiểm thử hồi quy (regression testing) bao phủ các kịch bản lỗi Gmail cũ.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần dựng form quản lý đa email ở frontend với các trạng thái liên kết động và thiết kế các animation chuyển trạng thái của HeroUI v3.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Hiểu rõ hơn về tầm quan trọng của việc tôn trọng định danh của Identity Provider (IdP) trong thiết kế hệ thống SSO/OAuth.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI luôn có xu hướng đề xuất các phương án "happy path". Sinh viên cần chịu trách nhiệm rà soát các điều kiện biên và bảo mật dữ liệu trước khi đưa code vào sản xuất.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-31    |
