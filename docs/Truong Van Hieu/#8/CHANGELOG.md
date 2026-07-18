# Changelog

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày bắt đầu | 2026-07-01 |
| Ngày hoàn thành | 2026-07-08 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 01/07/2026 - 02/07/2026 | Nghiên cứu nguyên lý HMAC và anti-replay | Completed |
| Phase 02 | 03/07/2026 - 05/07/2026 | Implement C# và Python HMAC service | Completed |
| Phase 03 | 06/07/2026 - 07/07/2026 | Viết test và kiểm thử cross-language | Completed |
| Phase 04 | 08/07/2026 | Tài liệu | Completed |

---

# [Phase 01] Nghiên cứu

## Đã hoàn thành

- [x] Hỏi Claude về cơ chế HMAC và anti-replay (timestamp + nonce)
- [x] Đọc AWS Signature V4 và Stripe webhook signing
- [x] Thiết kế công thức message string: `METHOD + URL + BODY + TIMESTAMP + NONCE`
- [x] Thiết kế IHmacSignatureService interface

## AI có hỗ trợ không?

- [x] Có

```text
Claude giải thích nguyên lý HMAC, timestamp/nonce anti-replay và constant-time comparison.
Không lấy code từ AI.
```

---

# [Phase 02] Implementation

## Đã hoàn thành

- [x] Implement HmacSignatureService trong C# với HMACSHA256 và CryptographicOperations.FixedTimeEquals
- [x] Implement MonitoringClient trong Python với hmac.new + hmac.compare_digest
- [x] Chuẩn hóa UTF-8 encoding cho body string ở cả hai phía
- [x] Tích hợp vào middleware xác thực inbound request từ CVerify.AI

## Thay đổi chi tiết

| STT | Nội dung | Người thực hiện | File | Minh chứng |
|---:|---|---|---|---|
| 1 | HmacSignatureService C# | Trương Văn Hiếu | `CVerify.Core/Modules/Shared/System/Services/HmacSignatureService.cs` | Unit test pass |
| 2 | MonitoringClient Python | Trương Văn Hiếu | `CVerify.AI/app/core/clients/monitoring_client.py` | Unit test pass |

## Lỗi đã xử lý

| STT | Lỗi | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Cross-language verify fail | Encoding khác nhau (Python default UTF-8, C# default UTF-16) | Explicit UTF-8 encode ở cả hai phía | Fixed |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Testing

## Đã hoàn thành

- [x] xUnit test: VerifySignature thành công
- [x] xUnit test: Sai signature → false
- [x] xUnit test: Timestamp hết hạn (>300s) → false
- [x] xUnit test: Signature rỗng → false
- [x] pytest test Python: Sign và verify thành công
- [x] Integration test cross-language: Python sign → C# verify pass

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 5. Cam kết cập nhật Changelog

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
