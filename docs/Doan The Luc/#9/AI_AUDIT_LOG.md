# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-05-31T06:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-31T08:38:00.000Z                                                               |

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
Triển khai cơ chế liên kết đa tài khoản OAuth (multi-connection) cho GitHub/GitLab thay vì giới hạn 1 tài khoản trên mỗi provider; xây dựng luồng xác nhận liên kết hai bước (Pending Link Confirmation) với thực thể PendingAuthProvider có TTL 10 phút; nâng cấp SessionValidationMiddleware để kiểm tra phiên đăng nhập ở cấp độ SessionId (per-session revocation) thông qua JWT sid claim; phát triển endpoint thu hồi hàng loạt phiên đăng nhập (Revoke All Other Sessions); cải tiến giao diện Settings UI với ConfirmationModal tái sử dụng và LinkedAccountsList hiển thị metadata chi tiết từng kết nối OAuth.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-31                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Thiết kế cơ chế liên kết đa tài khoản OAuth (multi-connection) và luồng xác nhận liên kết hai bước (Pending Link). |
| Phần việc liên quan | Backend / Authentication / OAuth Linking                                                                                                               |
| Mức độ sử dụng      | Hỗ trợ thiết kế kiến trúc PendingAuthProvider và refactor OAuth callback logic.                                                                         |

#### 4.1. Prompt đã sử dụng

```text
Refactor the OAuth linking flow to support multiple connections per provider (GitHub/GitLab can have multiple linked accounts). Introduce a two-phase confirmation: OAuth callback stores credentials in a PendingAuthProvider table with a 10-minute TTL, then the user confirms via a separate endpoint. Google remains single-connection with a unique index.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất tạo thực thể PendingAuthProvider với các trường lưu trữ tạm thời (provider_key, encrypted tokens, expires_at) và một PendingLinkCleanupService chạy background mỗi 30 phút để xóa bản ghi hết hạn. Đồng thời refactor unique index trên auth_providers: scope uniqueness chỉ áp dụng cho Google (provider_name = 'google'), trong khi GitHub/GitLab cho phép nhiều kết nối. OAuth callback được chia thành hai nhánh: nếu đã có kết nối trùng provider_key thì cập nhật credential, nếu chưa thì lưu vào pending_auth_providers chờ xác nhận.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc Entity PendingAuthProvider và cấu hình EF Core mapping (ToTable, HasKey, HasIndex, ForeignKey).
- Logic chia nhánh trong OAuth callback: cập nhật existing connection vs tạo pending link mới.
- Boilerplate của PendingLinkCleanupService với distributed lock sử dụng ICacheService.AcquireLockAsync.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Bổ sung cơ chế IgnoreQueryFilters khi kiểm tra xung đột provider_key để phát hiện cả các kết nối đã soft-delete và chặn liên kết trùng lặp xuyên tài khoản.
- Thêm audit event PROVIDER_LINK_CONFLICT khi phát hiện xung đột danh tính OAuth giữa các tài khoản.
- Tự thiết kế luồng reactivation: Nếu Google provider đã bị soft-delete, khi liên kết lại sẽ khôi phục (set DeletedAt = null) thay vì tạo bản ghi mới, kèm audit event PROVIDER_LINK_REACTIVATED.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement multi-connection OAuth linking, per-session revocation and pending link confirmation | https://github.com/Kaivian/CVerify/commit/7d34d88 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Kiến trúc hai bước (pending → confirm) giúp ngăn chặn liên kết không mong muốn khi kẻ tấn công chiếm quyền OAuth callback redirect. Người dùng phải chủ động xác nhận liên kết trên giao diện Settings, tăng cường đáng kể tính an toàn cho luồng liên kết OAuth.
```

---

### Lần sử dụng AI số 2

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-31                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Thiết kế cơ chế thu hồi phiên đăng nhập ở cấp độ SessionId (per-session revocation) trong SessionValidationMiddleware. |
| Phần việc liên quan | Backend / Security / Session Management                                                                                                                |
| Mức độ sử dụng      | Hỗ trợ thiết kế kiến trúc middleware và JWT claim mới.                                                                                                  |

#### 4.1. Prompt đã sử dụng

```text
Extend SessionValidationMiddleware to support per-session revocation. Add a 'sid' claim to the JWT token containing the SessionId. The middleware should check if the specific session is still active by querying RefreshTokens. Support a fallback mechanism using the refresh_token cookie for tokens issued before the sid claim was introduced (rolling deployment compatibility).
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất thêm claim "sid" vào JWT token tại TokenService, sau đó trong middleware lấy sid từ context.User.FindFirst("sid"). Nếu sid tồn tại, kiểm tra trạng thái session qua cache (auth:session:{sid}:active) với TTL 30 phút. Nếu cache miss, query DB RefreshTokens để tìm bất kỳ token nào chưa bị revoke cho sessionId đó. Nếu không có sid claim (token cũ chưa chứa sid), fallback sang đọc refresh_token cookie và tra cứu session tương ứng.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Logic middleware hai tầng: kiểm tra SessionVersion (user-wide) trước, sau đó kiểm tra SessionId (per-session).
- Cơ chế fallback qua refresh_token cookie cho khả năng tương thích rolling deployment.
- Cấu trúc cache key auth:session:{sid}:active với TTL 30 phút.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Bổ sung kiểm tra RevokedAt trực tiếp trên refresh_token trong nhánh fallback: Nếu token đã bị thu hồi, lập tức invalidate session mà không cần tra cứu thêm qua SessionId.
- Thêm fail-safe fallback: Nếu Redis gặp sự cố tạm thời (transient failure) khi query cache, middleware vẫn hoạt động bình thường bằng cách truy vấn trực tiếp DB thay vì ném lỗi 500.
- Viết 6 integration tests bao phủ đầy đủ các kịch bản: active sid pass, revoked sid fail, missing sid cookie fallback (active/revoked), invalid sid rejection, và self-revocation guard.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement multi-connection OAuth linking, per-session revocation and pending link confirmation | https://github.com/Kaivian/CVerify/commit/7d34d88 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Per-session revocation là bước tiến lớn so với cơ chế user-wide session version cũ: Giờ đây người dùng có thể thu hồi quyền truy cập của một thiết bị cụ thể mà không ảnh hưởng đến các phiên đăng nhập khác. Cơ chế fallback giúp quá trình nâng cấp không gián đoạn dịch vụ.
```

---

### Lần sử dụng AI số 3

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-05-31                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Xây dựng component ConfirmationModal tái sử dụng và cải tiến LinkedAccountsList hiển thị metadata từng kết nối OAuth. |
| Phần việc liên quan | Frontend / Settings UI / Component Design                                                                                                              |
| Mức độ sử dụng      | Hỗ trợ phác thảo cấu trúc component và logic quản lý trạng thái.                                                                                       |

#### 4.1. Prompt đã sử dụng

```text
Build a reusable ConfirmationModal component with HeroUI v3 Modal primitives supporting: verification text input (user must type exact text to confirm), blocking error state (disables confirm and shows alert), and variant-based styling (danger/warning/primary). Also redesign LinkedAccountsList to display per-connection cards with provider username, avatar, profile URL, and scope validation status.
```

#### 4.2. Kết quả AI gợi ý

```text
AI sinh cấu trúc ConfirmationModal component sử dụng Modal.Backdrop, Modal.Container, Modal.Dialog từ HeroUI v3 với props interface hỗ trợ verificationText, blockingError, và variant. Đồng thời phác thảo LinkedAccountsList hiển thị danh sách kết nối dưới dạng card có avatar, tên provider, và nút disconnect.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc component ConfirmationModal với các props verificationText, blockingError, variant.
- Layout cơ bản của Modal sử dụng HeroUI v3 primitives (Modal.Backdrop, Modal.Header, Modal.Body, Modal.Footer).
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Tự thiết kế logic chặn khóa tài khoản (lockout prevention): Trước khi cho phép ngắt kết nối Google, kiểm tra xem người dùng có mật khẩu hoặc ít nhất một phương thức đăng nhập thay thế. Nếu không, hiển thị blockingError trên modal thay vì cho phép xác nhận.
- Thiết kế lại component SignInMethod để phân biệt luồng "Create Password" (chưa có mật khẩu) và "Change Password" (đã có mật khẩu) dựa trên user.hasPassword.
- Tích hợp modal xác nhận xóa email phụ với thông báo cảnh báo chi tiết về hậu quả (mất khả năng đăng nhập, nhận thông báo, khôi phục mật khẩu qua email đó).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(auth): implement multi-connection OAuth linking, per-session revocation and pending link confirmation | https://github.com/Kaivian/CVerify/commit/7d34d88 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
ConfirmationModal được thiết kế đủ linh hoạt để tái sử dụng cho nhiều tình huống destructive action khác nhau trong tương lai (xóa tổ chức, thu hồi quyền admin, v.v.). Cơ chế blockingError ngăn chặn thao tác vô ý gây khóa tài khoản là lớp bảo vệ quan trọng ở phía UI.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                                          |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------------------ |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích cơ chế multi-connection OAuth và per-session revocation.                                |
| Viết user story/use case    |       x       |              |                 |               |                                                                                                  |
| Thiết kế database           |               |      x       |                 |               | Tạo bảng pending_auth_providers và mở rộng auth_providers.                                        |
| Thiết kế kiến trúc hệ thống |               |              |        x        |               | Thiết kế kiến trúc two-phase link confirmation và per-session middleware.                          |
| Thiết kế giao diện          |               |              |        x        |               | Overhaul Settings UI với ConfirmationModal và LinkedAccountsList.                                 |
| Code frontend               |               |              |        x        |               | Xây dựng ConfirmationModal, cải tiến SignInMethod và LinkedAccountsList.                          |
| Code backend                |               |              |        x        |               | Triển khai PendingAuthProvider, PendingLinkCleanupService, mở rộng AuthService và AuthController. |
| Debug lỗi                   |               |      x       |                 |               | Sửa lỗi password fallback khi không tìm thấy PasswordCredential.                                 |
| Viết test case              |               |              |        x        |               | Viết 6 integration tests cho session revocation middleware.                                       |
| Kiểm thử sản phẩm           |       x       |              |                 |               | Kiểm thử thủ công luồng liên kết/hủy liên kết OAuth và thu hồi phiên đăng nhập.                  |
| Tối ưu code                 |       x       |              |                 |               |                                                                                                  |
| Viết báo cáo                |       x       |              |                 |               |                                                                                                  |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                                  |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI không kiểm tra soft-deleted providers khi xác minh xung đột OAuth linking, dẫn đến khả năng liên kết trùng provider_key với tài khoản khác nếu bản ghi gốc đã soft-delete. | Review mã nguồn phát hiện query chỉ filter `DeletedAt == null`, bỏ sót các bản ghi đã xóa mềm. | Thêm `IgnoreQueryFilters()` khi kiểm tra xung đột provider_key để quét toàn bộ bản ghi bao gồm cả soft-deleted. |
|   2 | AI không tích hợp cơ chế chống khóa tài khoản (lockout prevention) khi ngắt kết nối provider: Cho phép người dùng ngắt kết nối OAuth duy nhất mà không kiểm tra xem có phương thức đăng nhập thay thế. | Kiểm thử thủ công kịch bản: Tạo tài khoản chỉ có Google SSO, ngắt kết nối Google → tài khoản bị khóa vĩnh viễn không thể đăng nhập lại. | Tự thiết kế logic kiểm tra tại frontend: Trước khi hiển thị nút Confirm trên modal ngắt kết nối, kiểm tra user.hasPassword hoặc số lượng provider còn lại > 0. Nếu vi phạm, hiển thị blockingError. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả thông qua:
1. Viết và chạy thành công 6 integration tests mới cho SessionRevocationTests bao phủ các kịch bản: active session pass, revoked session fail, missing sid cookie fallback (active/revoked), invalid sid rejection, và self-revocation guard.
2. Kiểm thử thủ công luồng liên kết OAuth hai bước trên giao diện Settings: Kết nối GitHub → xác nhận pending link → hiển thị connection card → ngắt kết nối thành công.
3. Kiểm thử thu hồi hàng loạt phiên đăng nhập (Revoke All Other Sessions) và xác nhận các phiên bị thu hồi không thể truy cập tiếp.
4. Kiểm tra cơ chế chống khóa tài khoản: Thử ngắt kết nối Google khi chưa có mật khẩu → modal hiển thị blockingError đúng quy cách.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Trực tiếp thiết kế kiến trúc two-phase pending link confirmation với PendingAuthProvider entity và PendingLinkCleanupService.
- Tự tay viết cơ chế per-session revocation trong SessionValidationMiddleware với fallback tương thích rolling deployment.
- Tự thiết kế logic lockout prevention trên frontend khi ngắt kết nối OAuth provider duy nhất.
- Viết 6 integration tests bao phủ đầy đủ các kịch bản session revocation.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                             | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai multi-connection OAuth, per-session revocation, pending link confirmation, Settings UI overhaul | Có                   | https://github.com/Kaivian/CVerify/commit/7d34d88 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm thử tích hợp UI luồng liên kết/hủy liên kết OAuth và thu hồi phiên đăng nhập            | Không                |                     |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp phác thảo nhanh cấu trúc Entity PendingAuthProvider, boilerplate cho PendingLinkCleanupService background worker, và layout cơ bản của ConfirmationModal component.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng logic kiểm tra xung đột OAuth chỉ filter DeletedAt == null của AI, vì bỏ sót các bản ghi đã soft-delete sẽ cho phép liên kết trùng provider_key xuyên tài khoản — một lỗ hổng bảo mật nghiêm trọng. Thay vào đó sử dụng IgnoreQueryFilters() để quét toàn diện.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Kiểm tra bằng cách viết 6 integration tests tự động cho session revocation middleware, kết hợp kiểm thử thủ công trên giao diện Settings cho luồng liên kết/hủy liên kết OAuth hai bước.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần thiết kế ConfirmationModal component có nhiều trạng thái (verification input, blocking error, variant styling) và tích hợp logic quản lý trạng thái phức tạp trong LinkedAccountsList hiển thị per-connection cards.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Hiểu rõ hơn về kiến trúc per-session revocation trong hệ thống authentication hiện đại và tầm quan trọng của rolling deployment compatibility khi thay đổi cấu trúc JWT claims.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI thường bỏ qua các trường hợp edge case liên quan đến soft-delete và lockout prevention. Sinh viên cần chủ động rà soát các kịch bản bảo mật tiêu cực (negative security scenarios) trước khi đưa code vào production.
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
