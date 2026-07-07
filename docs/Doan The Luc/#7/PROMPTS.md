# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project  | CVerify - Secure OAuth Integration & Settings Change Password Overhaul                  |
| Tên sinh viên / Nhóm   | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV  | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn   | QuangLTN3                                                                              |
| Ngày bắt đầu           | 2026-05-30T00:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-05-30                                                                             |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày       | Công cụ AI  | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
| --: | ---------- | ----------- | -------- | -------------- | ------------- | ------------------------- | ---------- |
|   1 | 2026-05-30 | Antigravity | Tìm nguyên nhân lỗi khởi tạo database khi chạy integration test | Chạy lệnh test suite "dotnet test" và phát hiện lỗi database ném ra... | Chỉ ra lỗi thứ tự thực thi DDL SQL trong DbInitializer.cs khi ALTER TABLE user_profiles chạy trước CREATE TABLE. | Có | GitHub Commit |
|   2 | 2026-05-30 | Antigravity | Triển khai mã hóa OAuth token AES-256-GCM bảo mật | Design a secure OAuth token encryption module in .NET using AES-256-GCM... | Sinh code lớp mã hóa TokenEncryptionService và thực thể lưu trữ OAuthCredential. | Có | GitHub Commit |
|   3 | 2026-05-30 | Antigravity | Overhaul giao diện Cài đặt (Settings) và liên kết Social Accounts | Redesign the user settings profile tab and social accounts list using HeroUI v3... | Thiết kế các component giao diện Profile, ChangePasswordForm và LinkedAccounts sử dụng HeroUI v3. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-30                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Tìm kiếm nguyên nhân lỗi "relation otp_verifications does not exist" phát sinh khi khởi tạo database container chạy bộ tích hợp kiểm thử.              |
| Phần việc liên quan | Backend / Database / Testing / Debug                                                                                                                   |
| Mức độ sử dụng      | Hỗ trợ ý tưởng và xử lý lỗi                                                                                                                            |

#### 5.1. Prompt nguyên văn

```text
Chạy lệnh test suite "dotnet test" và phát hiện lỗi database ném ra:
---- Npgsql.PostgresException : 42P01: relation "otp_verifications" does not exist
và
---- Npgsql.PostgresException : 42703: column "recovery_authority" of relation "organizations" does not exist

Hãy phân tích luồng chạy DbInitializer và kiểm tra xem tại sao script SQL không được tạo thành công trên môi trường DB trống.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Khi chạy bộ kiểm thử tự động của hệ thống (dotnet test), cơ sở dữ liệu trên Docker container kiểm thử được khởi tạo sạch hoàn toàn.
- Tuy nhiên, script DDL SQL trong DbInitializer.cs đột ngột thất bại làm sập toàn bộ các ca kiểm thử tích hợp DB.
- Cần AI hỗ trợ rà soát xem lỗi logic tuần tự hóa nào trong tệp SQL thô khiến các bảng otp_verifications và thay đổi trong organizations không thể thực thi.
```

#### 5.3. Kết quả AI trả về

```text
AI phân tích thấy tệp DbInitializer.cs chứa một câu lệnh SQL DDL lớn thực thi trong một batch giao dịch duy nhất. Trong khối di trú động DO $$ block (bắt đầu khoảng dòng 430), có đoạn mã kiểm tra và ALTER TABLE bảng user_profiles để thêm cột ai_talent_discovery.
Tuy nhiên, cấu trúc bảng user_profiles (dòng 889) lại được định nghĩa và tạo sau khối lệnh DO $$. Trên một database trống (như container kiểm thử), bảng user_profiles chưa hề tồn tại khi khối DO $$ thực thi, dẫn đến lỗi "relation user_profiles does not exist" làm đổ vỡ toàn bộ batch giao dịch SQL. Vì thế, các bảng và cột được tạo sau đó như otp_verifications và các thay đổi cột của organizations hoàn toàn bị bỏ qua.
AI đề xuất sửa đổi khối DO $$ block để kiểm tra sự tồn tại của bảng user_profiles trước khi thực hiện ALTER TABLE.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Bọc điều kiện kiểm tra sự tồn tại của bảng user_profiles trước khi ALTER TABLE trong khối DO $$ của DbInitializer.cs:
IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'user_profiles') THEN ...
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Tự triển khai đoạn SQL bọc điều kiện trên vào đúng vị trí dòng 626 của DbInitializer.cs.
- Biên dịch lại backend và chạy thành công 106/106 bài test suite tự động, xác minh lỗi tuần tự hóa khởi tạo schema đã được loại bỏ hoàn toàn.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
| --------------- | -------- |
| Commit          | https://github.com/Kaivian/CVerify/commit/a8e67a04e8de3f64d2be76f46c3cde3439a31497 |

#### 5.8. Ghi chú thêm

```text
```

---

### Prompt số 2

| Nội dung            | Thông tin                                                                                                                                                 |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-30                                                                                                                                                |
| Công cụ AI          | Antigravity                                                                                                                                               |
| Mục đích            | Tạo cấu trúc mã hóa/giải mã AES-256-GCM bảo vệ access/refresh tokens của OAuth trước khi lưu xuống PostgreSQL.                                            |
| Phần việc liên quan | Backend / Cryptography / Security                                                                                                                         |
| Mức độ sử dụng      | Hỏi sinh code mẫu và gợi ý giải pháp mã hóa                                                                                                               |

#### 5.1. Prompt nguyên văn

```text
Design a secure OAuth token encryption module in .NET using AES-256-GCM. 
We need to encrypt sensitive information (AccessToken, RefreshToken) in the `OAuthCredential` entity before writing to the database. 
Provide an IEncryptionService interface and a TokenEncryptionService implementation that reads a TokenEncryptionKey configured via appsettings/environment variables. 
Ensure standard validation is implemented and throw cryptographic exceptions on tampering.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Tích hợp liên kết tài khoản OAuth (Google, GitHub, GitLab) yêu cầu lưu trữ AccessToken và RefreshToken của người dùng để thực thi các tác vụ hậu kỳ (Background jobs, Sync profile).
- Việc lưu trữ plaintext tokens vi phạm các nguyên lý bảo mật cơ bản. Nhóm quyết định mã hóa dữ liệu nhạy cảm bằng thuật toán mã hóa đối xứng mạnh AES-256-GCM.
```

#### 5.3. Kết quả AI trả về

```text
- Bản thiết kế giao diện IEncryptionService chứa hai phương thức: Encrypt và Decrypt.
- Lớp TokenEncryptionService triển khai sử dụng thư viện mật mã chuẩn của .NET (System.Security.Cryptography.AesGcm).
- Tách biệt lưu trữ IV (nonce), ciphertext, và authentication tag, sau đó đóng gói thành một chuỗi Base64 thống nhất để dễ lưu trữ dưới dạng text.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tạo các tệp IEncryptionService.cs, TokenEncryptionService.cs và cấu hình Dependency Injection trong Program.cs.
- Tích hợp lớp mã hóa này vào tầng logic liên kết tài khoản của AuthService trước khi lưu dữ liệu thực thể OAuthCredential xuống PostgreSQL.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Nhóm đã sửa đổi phần cấu hình bí mật: AI đề xuất sinh ngẫu nhiên một khóa mã hóa tạm thời nếu không tìm thấy cấu hình bí mật. Nhóm đã loại bỏ đề xuất này vì nó làm hỏng việc giải mã dữ liệu sau khi ứng dụng khởi động lại (khóa thay đổi). Nhóm triển khai logic ném ngoại lệ dừng khởi động ứng dụng nếu TokenEncryptionKey không được cung cấp trong cấu hình môi trường.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
| --------------- | -------- |
| Commit          | https://github.com/Kaivian/CVerify/commit/a8e67a04e8de3f64d2be76f46c3cde3439a31497 |

#### 5.8. Ghi chú thêm

```text
```

---

### Prompt số 3

| Nội dung            | Thông tin                                                                                                                                                 |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-30                                                                                                                                                |
| Công cụ AI          | Antigravity                                                                                                                                               |
| Mục đích            | Overhaul giao diện Cài đặt (Settings UI) và cấu trúc liên kết mạng xã hội bằng HeroUI v3 & Tailwind CSS v4.                                               |
| Phần việc liên quan | Frontend / UI Components                                                                                                                                  |
| Mức độ sử dụng      | Hỏi sinh khung giao diện mẫu                                                                                                                              |

#### 5.1. Prompt nguyên văn

```text
Redesign the user settings profile tab and social accounts connection list using HeroUI v3 and Tailwind CSS v4. 
We need a responsive tab layout: Personal Info, Security (Change Password), and Connected Accounts.
In Connected Accounts, list Google, GitHub, and GitLab with their connection status, account email if linked, and 'Connect' / 'Disconnect' buttons that call the APIs.
Include clean animations using Framer Motion and modern typography.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Trang cài đặt cũ của hệ thống trông khá đơn giản và sử dụng các thư viện css tùy biến lộn xộn.
- Nhóm muốn đồng bộ hóa toàn diện UI của CVerify sang chuẩn HeroUI v3 mới được ban hành để nâng tầm thẩm mỹ và trải nghiệm của người dùng.
```

#### 5.3. Kết quả AI trả về

```text
- Các đoạn mã JSX mẫu cho cấu trúc Tab, Input, Button, Card của HeroUI v3.
- Giao diện danh sách Social Accounts gọn gàng với hiệu ứng hover và các icon đại diện.
- Biểu mẫu đổi mật khẩu tích hợp kiểm tra quy chuẩn password strength.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Thiết kế lại các tệp cài đặt tại `settings/page.tsx`, `social-connections.tsx` và `change-password-form.tsx`.
- Giao diện mới hiển thị mượt mà trạng thái liên kết động và hỗ trợ chuyển đổi tab linh hoạt.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Nhóm tự tích hợp các hooks xác thực `useAuth` toàn cục và bộ quản lý trạng thái qua Zustand để cập nhật lập tức danh sách liên kết trên UI khi người dùng ấn liên kết/hủy liên kết, đảm bảo trải nghiệm real-time và không cần tải lại trang.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
| --------------- | -------- |
| Commit          | https://github.com/Kaivian/CVerify/commit/a8e67a04e8de3f64d2be76f46c3cde3439a31497 |

#### 5.8. Ghi chú thêm

```text
```

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
Chạy lệnh test suite "dotnet test" và phát hiện lỗi database ném ra: 
---- Npgsql.PostgresException : 42P01: relation "otp_verifications" does not exist ... (Nguyên văn Prompt số 1)
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này giải quyết một lỗi nghẽn cổ chai nghiêm trọng trong hệ thống kiểm thử tự động (integration test container). Nó ảnh hưởng trực tiếp đến quy trình kiểm thử liên tục (CI/CD) của dự án. Nếu không phát hiện ra nguyên nhân do thứ tự ALTER TABLE chạy trước CREATE TABLE trong DbInitializer.cs, nhóm sẽ mất rất nhiều thời gian mò mẫm trong tệp di trú SQL khổng lồ chứa hơn 1000 dòng lệnh DDL thô.
```

### 6.3. Kết quả prompt này mang lại

```text
Chỉ ra chính xác mâu thuẫn tuần tự hóa trong DbInitializer.cs, đề xuất giải pháp bọc kiểm tra `IF EXISTS` giúp toàn bộ 106 bài kiểm thử chạy trơn tru ngay lập tức.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Nhóm trực tiếp chèn mã kiểm tra bảng user_profiles tồn tại vào DbInitializer.cs và kích hoạt chạy test container thông qua lệnh `dotnet test CVerify.sln`.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Đảm bảo cú pháp SQL tương thích hoàn toàn với PostgreSQL phiên bản đang chạy trong Docker container, tránh viết các câu lệnh DDL không đặc thù.
```

---

## 7. Prompt chưa hiệu quả

### 7.1. Prompt chưa hiệu quả

```text
How to decrypt OAuth tokens in PostgreSQL using AES decryption?
```

### 7.2. Vì sao prompt này chưa hiệu quả?

```text
Prompt hỏi về việc giải mã trực tiếp ở mức cơ sở dữ liệu PostgreSQL. Gợi ý từ AI tập trung vào việc cài đặt phần mở rộng `pgcrypto` trong Postgres. Tuy nhiên, việc này làm phân tán khóa bí mật mã hóa xuống cả tầng DB, đi ngược lại nguyên lý bảo mật ứng dụng (không chia sẻ khóa mã hóa với hệ quản trị cơ sở dữ liệu để phòng ngừa SQL injection trích xuất dữ liệu thô).
```

### 7.3. Cách cải thiện prompt

```text
Định hình rõ ràng việc mã hóa/giải mã phải được xử lý ở tầng ứng dụng (.NET Core application layer) sử dụng các dịch vụ Cryptography bảo mật của C#, thay vì xử lý trực tiếp ở tầng dữ liệu SQL.
```

### 7.4. Prompt sau khi cải tiến

```text
Write a C# encryption helper service for .NET Core where the database ONLY stores the encrypted bytes (Base64 representation) and all cryptographic operations (AES-256-GCM encryption/decryption) take place in the application memory, using keys injected from environment variables.
```

### 7.5. Kết quả sau khi cải tiến prompt

```text
AI cung cấp lớp dịch vụ TokenEncryptionService xử lý hoàn hảo trên bộ nhớ ứng dụng .NET, PostgreSQL chỉ nhận và lưu trữ ciphertext thô, cô lập hoàn toàn khóa mã hóa khỏi cơ sở dữ liệu.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần cung cấp bối cảnh kiến trúc hệ thống hiện tại, các ràng buộc thiết kế bảo mật (như không xử lý mã hóa tại tầng DB), các thư viện công nghệ đích (HeroUI v3, ASP.NET Core v10), và các logs lỗi cụ thể từ trình biên dịch hay hệ thống kiểm thử.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Nên đặt câu hỏi đi kèm điều kiện biên và các quy tắc bảo mật thiết kế (Security-by-design) để AI không đưa ra các giải pháp "tiện lợi nhưng kém bảo mật" (như sinh khóa ngẫu nhiên khi thiếu cấu hình hoặc lưu khóa dưới DB).
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Đính kèm các nguyên tắc bảo mật và công nghệ cụ thể của dự án vào prompt chỉ dẫn ban đầu của hệ thống để AI tự động lọc bỏ các phương án thiết kế vi phạm nguyên tắc này.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Coding |        2 | Design a secure OAuth token encryption module in .NET using AES-256-GCM... |
| Prompt Design |        1 | Redesign the user settings profile tab and social accounts connection list... |

---

## 10. Checklist chất lượng prompt

| Tiêu chí                   | Đã đạt? | Ghi chú |
| -------------------------- | :-----: | ------- |
| Prompt có mục tiêu rõ ràng |    x    |         |
| Prompt có đủ bối cảnh      |    x    |         |
| Tự kiểm tra và chỉnh sửa   |    x    |         |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết sử dụng prompt minh bạch và ghi nhận đúng đóng góp của AI.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-30    |
