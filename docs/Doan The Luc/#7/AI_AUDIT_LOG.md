# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - Secure OAuth Integration & Settings Change Password Overhaul                  |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-05-30T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-30T23:59:59.000Z                                                               |

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
Tích hợp bảo mật backend và giao diện cài đặt Settings nâng cấp với HeroUI v3 để quản lý liên kết tài khoản OAuth (Google, GitHub, GitLab), kiểm chứng scope validation, unlinking tài khoản và đổi mật khẩu an toàn (Change Password) kèm việc tự động thu hồi session/token của thiết bị khác. Đồng thời tìm kiếm giải pháp khắc phục triệt để lỗi tuần tự hóa khởi tạo schema của database trong DbInitializer.cs khi chạy bộ tích hợp test suite (integration test container).
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-30                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Tìm kiếm nguyên nhân lỗi "relation otp_verifications does not exist" phát sinh khi khởi tạo database container chạy bộ tích hợp kiểm thử.              |
| Phần việc liên quan | Backend / Database / Testing / Debug                                                                                                                   |
| Mức độ sử dụng      | Hỗ trợ ý tưởng và xử lý lỗi                                                                                                                            |

#### 4.1. Prompt đã sử dụng

```text
Chạy lệnh test suite "dotnet test" và phát hiện lỗi database ném ra:
---- Npgsql.PostgresException : 42P01: relation "otp_verifications" does not exist
và
---- Npgsql.PostgresException : 42703: column "recovery_authority" of relation "organizations" does not exist

Hãy phân tích luồng chạy DbInitializer và kiểm tra xem tại sao script SQL không được tạo thành công trên môi trường DB trống.
```

#### 4.2. Kết quả AI gợi ý

```text
AI phân tích thấy tệp DbInitializer.cs chứa một câu lệnh SQL DDL lớn thực thi trong một batch giao dịch duy nhất. Trong khối di trú động DO $$ block (bắt đầu khoảng dòng 430), có đoạn mã kiểm tra và ALTER TABLE bảng user_profiles để thêm cột ai_talent_discovery.
Tuy nhiên, cấu trúc bảng user_profiles (dòng 889) lại được định nghĩa và tạo sau khối lệnh DO $$. Trên một database trống (như container kiểm thử), bảng user_profiles chưa hề tồn tại khi khối DO $$ thực thi, dẫn đến lỗi "relation user_profiles does not exist" làm đổ vỡ toàn bộ batch giao dịch SQL. Vì thế, các bảng và cột được tạo sau đó như otp_verifications và các thay đổi cột của organizations hoàn toàn bị bỏ qua.
AI đề xuất sửa đổi khối DO $$ block để kiểm tra sự tồn tại của bảng user_profiles trước khi thực hiện ALTER TABLE.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Giải pháp bọc điều kiện kiểm tra sự tồn tại của bảng user_profiles trước khi ALTER TABLE trong khối DO $$ của DbInitializer.cs:
IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'user_profiles') THEN ...
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Tự triển khai mã sửa lỗi cụ thể vào tệp DbInitializer.cs tại dòng 626.
- Thực thi lệnh biên dịch cục bộ "dotnet build CVerify.sln" và chạy toàn bộ test suite "dotnet test" kiểm tra và xác nhận 106/106 ca kiểm thử thành công mỹ mãn.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement secure OAuth linking and settings password change | https://github.com/Kaivian/CVerify/commit/a8e67a04e8de3f64d2be76f46c3cde3439a31497 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Khi thiết kế các script khởi tạo cơ sở dữ liệu động hoặc các patch tự động chạy lúc ứng dụng khởi động, việc tuần tự hóa các câu lệnh và kiểm tra sự tồn tại của thực thể (tables, columns, types) là cực kỳ quan trọng để đảm bảo tính an toàn và tính độc lập của môi trường phát triển so với môi trường kiểm thử tự động từ số 0.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                           |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ----------------------------------------------------------------- |
| Phân tích yêu cầu           |               |              |        x        |               | Xem xét các yêu cầu bảo mật mã hóa AES và thu hồi token session   |
| Viết user story/use case    |       x       |              |                 |               |                                                                   |
| Thiết kế database           |               |      x       |                 |               | Thêm bảng oauth_credentials liên kết 1:1 với auth_providers.      |
| Thiết kế kiến trúc hệ thống |               |              |        x        |               | Tổ chức lưu trữ mã hóa riêng tư token tách biệt siêu dữ liệu.    |
| Thiết kế giao diện          |               |              |        x        |               | Overhaul Settings tab và Linked accounts list bằng HeroUI v3.     |
| Code frontend               |               |              |        x        |               | Tích hợp react-hook-form, các hooks useAuth và gọi các api mới.   |
| Code backend                |               |              |        x        |               | Cài đặt các API liên kết tài khoản, đổi mật khẩu và mã hóa token. |
| Debug lỗi                   |               |              |                 |       x       | Tìm và sửa lỗi khởi tạo DbInitializer do lệch thứ tự DDL SQL.     |
| Viết test case              |       x       |              |                 |               |                                                                   |
| Kiểm thử sản phẩm           |               |      x       |                 |               | Chạy bộ test suite 106 bài kiểm thử trên môi trường Postgres container |
| Tối ưu code                 |               |              |        x        |               | Tối ưu hóa truy vấn thông qua các chỉ mục index cơ sở dữ liệu.    |
| Viết báo cáo                |       x       |              |                 |               |                                                                   |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | Khi viết sql di trú động trong DbInitializer, AI bỏ quên kiểm tra sự tồn tại của bảng user_profiles trước khi thực hiện ALTER TABLE. | Chạy bộ tích hợp kiểm thử ném lỗi Postgres Exception: `relation "user_profiles" does not exist` và làm hỏng toàn bộ luồng tạo bảng tiếp theo. | Bổ sung mệnh đề kiểm tra bảng tồn tại `IF EXISTS` bọc ngoài khối thay đổi cấu trúc của bảng `user_profiles`. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả thông qua:
1. Thực hiện lệnh biên dịch backend: `dotnet build CVerify.sln` hoàn tất thành công với 0 lỗi.
2. Chạy bộ kiểm thử tự động của hệ thống: `dotnet test CVerify.sln` hoàn tất thành công 106/106 ca kiểm thử (bao gồm 1 Performance Test, 49 Unit Tests, và 56 Integration Tests), xác nhận tính đúng đắn và an toàn của hệ thống khởi tạo schema mới.
3. Kiểm tra tính năng đổi mật khẩu (Change Password): Xác thực mật khẩu cũ chính xác, mật khẩu mới tuân thủ password policy, cập nhật thành công credentials và revokes toàn bộ refresh tokens cũ trên các session khác của tài khoản đó.
4. Kiểm thử các APIs liên kết tài khoản xã hội (Google, GitHub, GitLab): Kiểm chứng tính năng unlink, kiểm tra scope hoạt động chính xác và mã hóa access/refresh tokens thành công bằng thuật toán AES-256-GCM trước khi lưu xuống PostgreSQL.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Trực tiếp định cấu hình mã hóa khóa TokenEncryptionKey trên Program.cs và thiết kế mô hình Entity Framework cho OAuthCredential.
- Tìm kiếm, định vị và sửa chữa lỗi biên dịch/chạy migration nghiêm trọng của DbInitializer trên container kiểm thử.
- Overhaul lại giao diện UI của Settings tab và liên kết mạng xã hội bằng HeroUI v3 đảm bảo responsive và trải nghiệm người dùng hiện đại.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                            | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai mã hóa, tích hợp APIs OAuth, Change Password, debug di trú DB  | Có                   | https://github.com/Kaivian/CVerify/commit/a8e67a04e8de3f64d2be76f46c3cde3439a31497 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm thử UAT luồng đổi mật khẩu và xác minh liên kết tài khoản trên UI   | Không                |                     |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI hỗ trợ viết nhanh helper mã hóa AES-256-GCM chuẩn chỉ, đề xuất cấu hình DI và phác thảo các endpoints điều phối quản trị liên kết tài khoản ở AuthController.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng đề xuất thiết kế của AI về việc lưu trực tiếp access/refresh token trong bảng auth_providers. Việc này làm lộ lọt thông tin nhạy cảm và vi phạm nguyên lý thiết kế cơ sở dữ liệu tối giản. Nhóm đã tự phân tách ra bảng riêng biệt oauth_credentials và mã hóa hoàn toàn nội dung token.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Sử dụng bộ tích hợp kiểm thử tự động của dự án gồm 106 bài test trên database container, thực hiện build thực tế cả frontend và backend để rà soát lỗi tĩnh (static analysis) và biên dịch.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần thiết kế và viết code cho lớp mã hóa AES-256-GCM với các thao tác băm IV (nonce), sinh tags và copy khối bytes thủ công sẽ mất nhiều thời gian tra cứu và dễ phát sinh lỗi buffer size/lệch offset.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Học được cách áp dụng các nguyên tắc thiết kế bảo mật chuyên nghiệp (security by design) từ việc mã hóa thông tin nhạy cảm đến quản lý thu hồi phiên làm việc (session validation & token revocation) khi tài khoản thay đổi trạng thái bảo mật (đổi mật khẩu).
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Càng tin tưởng vào AI thì càng phải kiểm thử nghiêm ngặt. Lỗi thiếu kiểm tra sự tồn tại của bảng trong DbInitializer cho thấy AI thường bỏ qua các kịch bản môi trường trống (clean environment) hoặc các điều kiện biên.
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
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-30    |
