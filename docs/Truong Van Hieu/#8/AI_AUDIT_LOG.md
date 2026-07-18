# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu | 2026-07-01 |
| Ngày hoàn thành | 2026-07-08 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là hiểu sâu cơ chế HMAC-SHA256 dùng cho service-to-service authentication giữa
CVerify.AI (Python) và CVerify.Core (.NET), đặc biệt là cơ chế timestamp + nonce để ngăn
replay attack. Tôi không dùng AI để viết code — chỉ hỏi để hiểu cơ chế mật mã học trước
khi tự implement.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 01/07/2026 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Hiểu cơ chế HMAC-SHA256 và cách ngăn replay attack bằng timestamp + nonce |
| Phần việc liên quan | Bảo mật service-to-service |
| Mức độ sử dụng | Hỏi kiến thức |

#### 4.1. Prompt đã sử dụng

```text
Giải thích cơ chế HMAC-SHA256 dùng cho service-to-service authentication là gì và tại sao
an toàn hơn API key đơn giản. Cụ thể: làm thế nào để ngăn replay attack bằng timestamp
và nonce? Nếu attacker bắt được request hợp lệ và gửi lại, hệ thống phát hiện bằng cách
nào? Clock skew window bao nhiêu giây là hợp lý?
```

#### 4.2. Kết quả AI gợi ý

AI giải thích: HMAC ký request bằng shared secret, attacker không biết key nên không thể
tạo signature hợp lệ. Timestamp ngăn replay attack vì request cũ sẽ có timestamp ngoài
cửa sổ cho phép (thường 5 phút / 300 giây). Nonce là giá trị ngẫu nhiên dùng một lần để
ngăn duplicate request trong cùng cửa sổ thời gian. Clock skew 300 giây là chuẩn phổ biến
(AWS, Stripe đều dùng). AI cũng đề cập cần dùng constant-time comparison để tránh timing attack.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu nguyên lý timestamp + nonce + clock skew window 300 giây và lý do cần constant-time
comparison. Áp dụng vào thiết kế IHmacSignatureService.VerifySignature và implement trong
CVerify.Core (C# với CryptographicOperations.FixedTimeEquals) và CVerify.AI (Python với
hmac.compare_digest).

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi tự thiết kế công thức HMAC message: `HTTP_METHOD + URL + BODY + TIMESTAMP + NONCE` —
AI không đề cập công thức cụ thể, chỉ giải thích nguyên lý. Ngoài ra tôi tự implement cả
hai phía (C# server verify và Python client sign) đảm bảo consistent algorithm.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: Claude giải thích rất rõ lý do cần từng thành phần (timestamp, nonce,
  constant-time), giúp tôi implement có hiểu biết chứ không chỉ copy pattern.
- Bài học: Constant-time comparison là điểm quan trọng nhất mà AI nhắc đến — nếu không
  biết, tôi có thể đã dùng string == comparison và tạo ra lỗ hổng timing attack.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Thiết kế bảo mật |  | [x] |  |  | Tham khảo nguyên lý HMAC + anti-replay |
| Code C# HMAC | [x] |  |  |  | Tự implement 100% |
| Code Python HMAC | [x] |  |  |  | Tự implement 100% |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI không đề cập công thức nào để build message string cho HMAC (METHOD+URL+BODY+TS+NONCE vs các cách khác). | Nghiên cứu thêm các tiêu chuẩn: AWS Signature V4, Stripe webhook signing. | Tự thiết kế công thức `HTTP_METHOD + URL + BODY + TIMESTAMP + NONCE` dựa trên best practice. |
| 2 | AI không đề cập đến vấn đề body encoding nhất quán giữa Python và C# khi tính HMAC. | Phát hiện khi test cross-service: Python sign → C# verify fail vì encoding khác nhau. | Chuẩn hóa body là UTF-8 string trước khi tính HMAC ở cả hai phía. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc tài liệu AWS Signature Version 4 và Stripe Webhook Signatures để xem họ thiết kế
   HMAC message string như thế nào.
2. Viết unit test đầu đủ bao gồm: verify thành công, sai signature, timestamp hết hạn,
   timestamp ngoài clock skew.
3. Test cross-language: Python sign → C# verify và C# sign → Python verify đều pass.
```

---

## 8. Đóng góp cá nhân

```text
- Tự đặt câu hỏi để hiểu nguyên lý HMAC và anti-replay.
- Tự thiết kế công thức message string.
- Tự implement IHmacSignatureService trong C# với FixedTimeEquals.
- Tự implement MonitoringClient trong Python với hmac.compare_digest.
- Tự phát hiện và fix vấn đề encoding nhất quán.
- Tự viết unit test cho tất cả kịch bản.
```

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ ở điểm nào?

```text
AI nhắc đến constant-time comparison — đây là điểm quan trọng nhất. Nếu không biết điều
này, tôi có thể đã tạo ra lỗ hổng timing attack trong hệ thống.
```

### 9.2. Học được gì về môn học?

```text
Bảo mật service-to-service khó hơn bảo mật user-facing vì không có người dùng để xác nhận
danh tính — chỉ dựa vào shared secret và signature. Từng chi tiết (constant-time, clock skew,
nonce) đều có lý do bảo mật cụ thể.
```

### 9.3. Học được gì về sử dụng AI có trách nhiệm?

```text
Với bảo mật mật mã học, AI cung cấp khái niệm đúng nhưng không biết cách triển khai cụ thể
tránh pitfall như encoding inconsistency. Cần tự test kỹ lưỡng.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
