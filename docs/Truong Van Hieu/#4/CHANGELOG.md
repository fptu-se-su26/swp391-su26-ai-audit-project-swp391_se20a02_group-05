# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.

---

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - Hệ thống xác thực thông tin và quản lý hồ sơ năng lực dành cho Doanh nghiệp |
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV / Danh sách MSSV | DE190105 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | `https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05` |
| Ngày bắt đầu | 2026-06-02 |
| Ngày hoàn thành | 2026-06-10 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 02/06/2026 - 03/06/2026 | Nghiên cứu nguyên lý JWT Refresh Token Rotation | Completed |
| Phase 02 | 04/06/2026 - 06/06/2026 | Thiết kế schema và kiến trúc Authentication module | Completed |
| Phase 03 | 07/06/2026 - 09/06/2026 | Triển khai mã nguồn và kiểm thử | Completed |
| Phase 04 | 10/06/2026 | Hoàn thiện tài liệu kiểm toán AI | Completed |

---

# [Phase 01] Nghiên cứu nguyên lý

## Ngày thực hiện

```text
02/06/2026 - 03/06/2026
```

## Đã hoàn thành

- [x] Đọc RFC 6749 (OAuth 2.0) và RFC 6819 (OAuth 2.0 Threat Model)
- [x] Hỏi Claude để làm rõ nguyên lý Refresh Token Rotation
- [x] So sánh Sliding Expiry vs Absolute Expiry cho hai nhóm user
- [x] Phác thảo threat model cho phân hệ Authentication của CVerify
- [x] Vẽ sequence diagram luồng Refresh Token Rotation

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

```text
Claude giải thích nguyên lý Refresh Token Rotation và so sánh Sliding vs Absolute Expiry,
giúp tôi hiểu bản chất bảo mật của cơ chế này. Không lấy code từ AI.
```

---

# [Phase 02] Thiết kế schema và kiến trúc

## Ngày thực hiện

```text
04/06/2026 - 06/06/2026
```

## Đã hoàn thành

- [x] Thiết kế bảng RefreshTokens với các cột: Id, UserId, TokenFamily, Token (hashed), ExpiresAt, CreatedAt, RevokedAt, ReplacedByToken
- [x] Thiết kế bảng TokenFamilies để hỗ trợ cascade revoke toàn bộ phiên
- [x] Thiết kế ITokenService interface với các phương thức GenerateTokenPair, RefreshTokens, RevokeFamily
- [x] Thiết kế Redis key schema cho atomic token rotation (SETNX pattern)
- [x] Xác định chiến lược expiry: Business User → Sliding 7 ngày; Admin → Absolute 8 giờ

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thiết kế schema bảng RefreshTokens và TokenFamilies | Trương Văn Hiếu | `CVerify.Core/Modules/Auth/Models/` | Database migration file |
| 2 | Thiết kế ITokenService interface | Trương Văn Hiếu | `CVerify.Core/Modules/Auth/Services/ITokenService.cs` | Interface definition |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

```text
Giai đoạn thiết kế schema và interface hoàn toàn tự làm dựa trên hiểu biết tích lũy từ
phase nghiên cứu, không sử dụng AI.
```

---

# [Phase 03] Triển khai và kiểm thử

## Ngày thực hiện

```text
07/06/2026 - 09/06/2026
```

## Đã hoàn thành

- [x] Triển khai JwtService: GenerateAccessToken, GenerateRefreshToken với TokenFamily
- [x] Triển khai RefreshTokenService với atomic rotation dùng Redis SETNX
- [x] Triển khai AuthController với endpoint POST /api/auth/refresh
- [x] Triển khai middleware kiểm tra fingerprint thiết bị trong token payload
- [x] Viết xUnit test kiểm tra kịch bản rotation thành công
- [x] Viết xUnit test kiểm tra kịch bản phát hiện token reuse attack
- [x] Viết xUnit test kiểm tra cascade revoke khi phát hiện tấn công
- [x] Kiểm thử concurrent refresh với JMeter (100 request đồng thời)

## Danh sách lỗi đã xử lý

| STT | Lỗi phát hiện | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Race condition: hai request đồng thời dùng cùng refresh token, cả hai đều thành công | Không có cơ chế khóa khi cập nhật trạng thái token | Áp dụng Redis SETNX để đảm bảo chỉ một request dùng token thành công, request còn lại nhận 401 | Fixed |
| 2 | Cascade revoke không hoạt động khi token family quá lớn | Query không dùng index trên cột TokenFamilyId | Thêm composite index (TokenFamilyId, RevokedAt) vào bảng RefreshTokens | Fixed |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

```text
Giai đoạn triển khai code và kiểm thử hoàn toàn tự thực hiện, không sử dụng AI.
```

---

# [Phase 04] Hoàn thiện tài liệu

## Ngày thực hiện

```text
10/06/2026
```

## Đã hoàn thành

- [x] Hoàn thiện AI_AUDIT_LOG.md
- [x] Hoàn thiện PROMPTS.md ghi lại prompt đã hỏi AI
- [x] Hoàn thiện REFLECTION.md
- [x] Hoàn thiện CHANGELOG.md này

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

| STT | Chức năng | Trạng thái | Ghi chú |
|---:|---|---|---|
| 1 | JWT Access Token + Refresh Token generation | Completed | Sliding 7d / Absolute 8h theo nhóm user |
| 2 | Refresh Token Rotation với atomic Redis lock | Completed | Race-condition proof |
| 3 | Token Family cascade revoke | Completed | Revoke toàn bộ phiên khi phát hiện token reuse |
| 4 | Device fingerprint trong token payload | Completed | Tăng cường bảo mật |

### 4.2. Tổng hợp AI hỗ trợ

| Hạng mục | AI có hỗ trợ không? | Mức độ | Ghi chú |
|---|---|---|---|
| Nghiên cứu nguyên lý | Có | Ít | Giải thích khái niệm Rotation và Expiry strategies |
| Thiết kế | Không | - | Tự thiết kế schema và interface |
| Coding | Không | - | Tự code 100% |
| Testing | Không | - | Tự viết và chạy test |

---

## 5. Cam kết cập nhật Changelog

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/06/2026 |
