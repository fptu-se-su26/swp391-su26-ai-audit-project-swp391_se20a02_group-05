# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - Reclaim Organization Ownership                                               |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-05-28T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-28T23:59:59.000Z                                                               |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
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
Phân tích nguyên nhân lỗi xác thực OTP, thiết kế giải pháp chuẩn hóa (normalization) email/tax code trên backend, tối ưu hóa state management trên frontend cho wizard multi-step và sinh code kiểm thử tích hợp (integration testing).
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung            | Thông tin                                                                                                                                                                                 |
| ------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-28                                                                                                                                                                                |
| Công cụ AI          | Gemini, Antigravity                                                                                                                                                                       |
| Mục đích sử dụng    | Fix Reclaim Organization Ownership OTP verification bug (token invalid/expired on submission) and improve robustness of multi-step reclaim flows with proper email/taxCode normalization. |
| Phần việc liên quan | Backend / Frontend / Testing / Debug                                                                                                                                                      |
| Mức độ sử dụng      | Sinh chính nội dung                                                                                                                                                                       |

#### 4.1. Prompt đã sử dụng

```text
# Fix Reclaim Ownership OTP Verification Bug

Investigate and fix the bug in the **Reclaim Organization Ownership** workflow where the final reclaim submission fails with:

> "Email OTP Verification token is invalid or has expired."

even when the OTP was recently verified successfully.

The issue occurs after:

* OTP email is sent successfully
* User enters correct OTP
* Verification succeeds
* User uploads legal ownership evidence
* Final reclaim submission fails with token invalid/expired

Perform a full investigation across both frontend and backend to identify the real root cause.

Check:

* OTP verification lifecycle
* verification session persistence
* token expiration handling
* frontend state resets/re-renders
* multi-step workflow state management
* file upload side effects
* request payload construction
* backend validation flow
* Redis/cache/session storage
* auth middleware
* race conditions
* navigation/back button behavior
* refresh handling
* React StrictMode effects
* stale state or invalidated sessions

Fix the issue properly without hacks or bypasses.

Requirements:

* Ensure verification state persists correctly through the entire reclaim workflow
* Ensure uploaded files or page transitions do not invalidate verification state
* Ensure the final reclaim request uses the correct verified session/token
* Ensure expiration timing is calculated correctly and consistently
* Prevent premature invalidation of verification state
* Improve reliability of multi-step OTP flows
* Add proper validation/error handling
* Add logging for verification lifecycle failures
* Add automated tests covering the reclaim verification workflow

Do not hardcode tokens, disable expiration checks, or implement temporary workarounds.
```

#### 4.2. Kết quả AI gợi ý

```text
AI proposed a structured resolution:
1. Identifying a mismatch between the raw email/taxCode values inputted during initial representative identification vs those sent/signed inside the verification token. Whitespace differences, subaddressing (+tag), and casing caused string comparison mismatches in OrganizationReclaimService.cs and RecoveryService.cs.
2. Recommending a centralized normalization class (RecoveryTokenHelper) implementing Trim, Lowercase, and Gmail subaddressing stripping.
3. Suggesting frontend updates in reclaim-view.tsx: caching the verified OTP token in React/Zustand state so that moving between wizard steps does not reset or prematurely invalidate the token.
4. Adding an integration test to simulate and assert successful submission under normalization rules.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Email and tax code normalization helpers (casing, spaces, Gmail subaddressing parsing).
- Integration test scenario for submitting organization claims using non-normalized emails.
- Logic pattern for skip-on-verified step navigation in React components.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Refined the Gmail subaddressing stripping logic in C# to strictly apply to 'gmail.com' domain while retaining full domain names for other email providers.
- Improved backend logging on token mismatch by logging both the decrypted parameters and the incoming request parameters to help diagnose future casing/formatting issues.
- Optimized frontend ReclaimView by ensuring the "Previous" button does not clear the filled state of RepresentativeInfo fields, maintaining a smooth user navigation path.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn                                             | Nội dung                                                                                                                             |
| --------------- | ------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------ |
| Commit/PR       | Enhance OTP handling, rate limits & file uploads | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/7b272853130f6c668a40df73d4aec346d6611c99 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Sử dụng các giá trị chuẩn hóa (normalized values) cho email và mã số thuế khi mã hóa/ký token là bắt buộc để ngăn ngừa các lỗi so khớp giả lập. Đồng thời, việc tối ưu hóa state lưu giữ token trên frontend giúp cải thiện rõ rệt UX, tránh việc người dùng phải nhận lại OTP khi chỉ quay lại sửa đổi thông tin người đại diện.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                              |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | -------------------------------------------------------------------- |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích các nguyên nhân tiềm ẩn gây lỗi token hết hạn/không hợp lệ |
| Viết user story/use case    |       x       |              |                 |               |                                                                      |
| Thiết kế database           |       x       |              |                 |               | Không có thay đổi schema cho bug fix này                             |
| Thiết kế kiến trúc hệ thống |               |              |        x        |               | Thiết kế lớp normalization trung gian                                |
| Thiết kế giao diện          |               |      x       |                 |               | Điều chỉnh luồng điều hướng trong wizard                             |
| Code frontend               |               |              |        x        |               | Cải tiến reclaim-view.tsx                                            |
| Code backend                |               |              |                 |       x       | Viết RecoveryTokenHelper.cs và tích hợp vào các services             |
| Debug lỗi                   |               |              |        x        |               | Lập luận tìm ra lỗi so khớp chuỗi thô                                |
| Viết test case              |               |              |                 |       x       | Tạo RecoveryFlowTests.cs integration test case                       |
| Kiểm thử sản phẩm           |               |      x       |                 |               | Chạy dotnet test và kiểm chứng thủ công                              |
| Tối ưu code                 |               |      x       |                 |               | Thêm logs chẩn đoán lỗi                                              |
| Viết báo cáo                |       x       |              |                 |               |                                                                      |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                      |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI                                                                                                 | Cách phát hiện                                                                                                                                                                                 | Cách xử lý/cải tiến                                                                                                     |
| --: | ----------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------- |
|   1 | AI gợi ý loại bỏ dấu '+' và phần đuôi subaddressing trên mọi tên miền email.                                      | Code review cho thấy điều này không đúng vì chỉ có Gmail/Google Workspace chính thức bỏ qua subaddressing bằng '+'. Các mail server khác có thể coi địa chỉ có dấu '+' là khác biệt hoàn toàn. | Sửa lại logic trong RecoveryTokenHelper để chỉ áp dụng cắt bỏ subaddressing cho tên miền gmail.com.                     |
|   2 | AI đề xuất xóa sạch state của biểu mẫu người đại diện khi nhấn nút quay lại ("Previous") từ bước Upload tài liệu. | Thử nghiệm thủ công trên UI phát hiện người dùng phải điền lại toàn bộ thông tin.                                                                                                              | Giữ nguyên dữ liệu biểu mẫu trên React state, chỉ chuyển ReclaimStep và giữ token đã xác thực nếu email không thay đổi. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả thông qua:
1. Viết Integration Test (RecoveryFlowTests.cs) giả lập việc gửi email chứa khoảng trắng, ký tự viết hoa và nhãn '+tag'. Test chạy thành công và trả về mã 200 OK cùng trạng thái Claim Pending.
2. Chạy thử nghiệm runtime trên client (Next.js) và backend (ASP.NET Core v10). Thực hiện quy trình điền thông tin, nhận OTP, xác thực thành công, nhấn Back, nhấn Next (không cần nhập lại OTP), tải tài liệu lên và nộp thành công mà không bị lỗi Token Invalid.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Phát hiện lỗ hổng logic chuẩn hóa của AI đối với các email không thuộc Gmail và sửa đổi kịp thời.
- Thiết kế trải nghiệm quay lại (Back navigation) mượt mà cho ReclaimView mà không làm mất dữ liệu đã điền của người đại diện.
- Trực tiếp chạy và kiểm tra kết quả test suite, tối ưu cấu trúc các file trong codebase.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                | Có sử dụng AI không? | Minh chứng đóng góp                                                                                                                  |
| --------------------- | -------- | ------------------------------------------------------------- | -------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Đoàn Thế Lực          | DE200523 | Sửa lỗi OTP verification workflow trên cả frontend và backend | Có                   | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/7b272853130f6c668a40df73d4aec346d6611c99 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm tra tính đúng đắn của logic chuẩn hóa và test suite      | Không                |                                                                                                                                      |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp định vị nhanh lỗi so khớp chuỗi thô (mismatch do khoảng trắng, casing) thay vì lỗi hết hạn token thực tế, và đề xuất cấu trúc boilerplate cho helper cùng integration test.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng logic loại bỏ nhãn '+' cho các domain không phải Gmail vì nó vi phạm tiêu chuẩn kỹ thuật email của các nhà cung cấp khác. Không sử dụng hành vi reset form khi nhấn nút "Previous" để tối ưu hóa trải nghiệm người dùng.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Bằng cách chạy Integration Tests thông qua lệnh `dotnet test` và chạy kiểm thử hộp đen (black-box manual testing) trực tiếp trên trình duyệt Chrome qua môi trường UAT.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần viết code kiểm thử tích hợp (integration tests) mô phỏng lại luồng yêu cầu phức tạp (nhận OTP -> sinh token -> submit claim với R2 mock) sẽ tốn nhiều thời gian tra cứu API của HttpClient/EF Core hơn.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Học được rằng trong một hệ thống phân tán đa bước (multi-step distributed workflow), tính nhất quán của dữ liệu nhận diện (identity parameters) giữa các bước là cực kỳ quan trọng và cần được chuẩn hóa sớm nhất có thể.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI có thể tạo ra các giải pháp chung chung (như chuẩn hóa mọi email bằng cùng một công thức). Lập trình viên phải có kiến thức nền tảng tốt (như tiêu chuẩn RFC của email) để phát hiện và điều chỉnh các điểm thiếu sót đó.
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
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-29    |
