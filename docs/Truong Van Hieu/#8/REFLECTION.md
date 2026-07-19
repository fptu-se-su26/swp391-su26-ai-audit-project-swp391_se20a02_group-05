# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày hoàn thành reflection | 08/07/2026 |

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Tôi dùng Claude 1 lần để hiểu nguyên lý HMAC-SHA256 và cơ chế anti-replay. Phần quan
trọng nhất AI đóng góp là thông tin về constant-time comparison — điều tôi không tự nghĩ
ra. Toàn bộ implementation, thiết kế message format và testing đều tự thực hiện.
```

---

## 5. AI đã hỗ trợ ở điểm nào?

- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế kiến trúc (ở mức nguyên lý bảo mật)

```text
Điểm quan trọng nhất: AI nhắc đến constant-time comparison để tránh timing attack. Nếu
không hỏi AI, tôi có thể đã dùng string comparison bình thường và để lộ lỗ hổng bảo mật.
```

---

## 6. Điểm giúp và không giúp tốt

### Giúp tốt

```text
AI trả lời đầy đủ các thành phần của HMAC anti-replay (timestamp, nonce, constant-time)
và giải thích tại sao từng thành phần cần thiết.
```

### Không giúp tốt

```text
AI không đề cập vấn đề encoding inconsistency khi implement cross-language (Python UTF-8
vs C# UTF-16). Đây là bug thực tế tôi gặp và phải tự fix.
```

### Phụ thuộc AI?

- [x] Không phụ thuộc

---

## 8. Ví dụ AI không đề cập

| Nội dung | Mô tả |
|---|---|
| AI không đề cập gì? | Vấn đề encoding khi implement HMAC cross-language. |
| Hậu quả? | Python sign → C# verify fail ban đầu do encoding khác nhau. |
| Tự fix như thế nào? | Explicit encode body sang UTF-8 bytes trước khi tính HMAC ở cả hai phía. |
| Bài học | Cross-language implementation cần chú ý encoding. AI không biết pitfall này. |

---

## 9. Đóng góp thật sự

```text
1. Thiết kế message format: HTTP_METHOD + URL + BODY + TIMESTAMP + NONCE.
2. Implement HmacSignatureService C# với FixedTimeEquals.
3. Implement MonitoringClient Python với hmac.compare_digest.
4. Phát hiện và fix vấn đề encoding cross-language.
5. Viết đầy đủ unit test và integration test.
```

---

## 10. So sánh trước và sau

| Area | Before | After | Improvement |
|---|---|---|---|
| Hiểu HMAC anti-replay | Biết HMAC nhưng không biết tại sao cần timestamp/nonce | Hiểu đầy đủ nguyên lý và implement đúng | Implement có lý do chứ không copy pattern |
| Bảo mật implementation | Không biết constant-time comparison | Áp dụng FixedTimeEquals và compare_digest | Tránh được timing attack vulnerability |

---

## 15. Câu hỏi tự vấn

### Phần nào thể hiện năng lực thật sự?

```text
Phát hiện và fix vấn đề encoding cross-language (Python UTF-8 vs C# default encoding) và
thiết kế message format chuẩn theo AWS Signature V4 — đây là điểm AI không làm thay tôi.
```

---

## 16. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
