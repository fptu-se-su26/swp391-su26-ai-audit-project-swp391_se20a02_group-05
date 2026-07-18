# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày cập nhật | 2026-07-08 |

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Claude

---

## 4. Bảng tổng hợp

| STT | Ngày | AI | Mục đích | Prompt tóm tắt | Kết quả | Có dùng? |
|---:|---|---|---|---|---|---|
| 1 | 01/07/2026 | Claude | Nguyên lý HMAC và anti-replay attack | "Giải thích HMAC-SHA256 cho service-to-service auth... ngăn replay attack bằng timestamp/nonce..." | Hiểu nguyên lý, clock skew, constant-time | Có (Nguyên lý) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Giải thích cơ chế HMAC-SHA256 dùng cho service-to-service authentication là gì và tại sao
an toàn hơn API key đơn giản. Cụ thể: làm thế nào để ngăn replay attack bằng timestamp
và nonce? Nếu attacker bắt được request hợp lệ và gửi lại, hệ thống phát hiện bằng cách
nào? Clock skew window bao nhiêu giây là hợp lý?
```

#### 5.2. Bối cảnh

CVerify.AI (Python) cần gọi CVerify.Core (.NET) với authentication an toàn. API key đơn
giản dễ bị replay attack. Cần hiểu cơ chế HMAC trước khi implement.

#### 5.3. Kết quả AI trả về

- HMAC ký bằng shared secret → attacker không có key, không tạo được signature hợp lệ.
- Timestamp ngăn replay vì request cũ có timestamp ngoài clock skew (300s là chuẩn).
- Nonce là random value một lần để ngăn duplicate trong cùng cửa sổ.
- **Quan trọng nhất:** phải dùng constant-time comparison để tránh timing attack.

#### 5.4. Đã áp dụng như thế nào

Áp dụng nguyên lý thiết kế VerifySignature với clock skew 300s và constant-time comparison.
Tự viết toàn bộ code.

#### 5.5. Điểm quan trọng nhất từ AI

Constant-time comparison (CryptographicOperations.FixedTimeEquals trong C#, hmac.compare_digest
trong Python) — điểm tôi không biết nếu không hỏi AI.

#### 5.6. Đánh giá

- [x] Prompt rõ ràng, có bối cảnh cụ thể
- [x] Kết quả tốt, trả lời đúng câu hỏi

---

## 6. Bài học về cách viết prompt

```text
Câu hỏi "attacker bắt được request hợp lệ và gửi lại, hệ thống phát hiện bằng cách nào?"
rất cụ thể và giúp AI giải thích đúng cơ chế timestamp chứ không giải thích chung chung.
Đặt câu hỏi theo attack scenario giúp hiểu nguyên lý bảo mật sâu hơn.
```

---

## 9. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
