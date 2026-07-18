# AI Learning Reflection

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày hoàn thành reflection | 10/07/2026 |

---

## 3. Tóm tắt

```text
Dùng ChatGPT 1 lần để hiểu các caching pattern và chọn Cache-Aside cho bài toán AI analysis
results. AI cũng gợi ý event-driven invalidation — điểm tôi chưa nghĩ đến. Stampede prevention
và Redis fallback do tôi tự thiết kế và implement.
```

---

## 5. AI hỗ trợ ở điểm nào?

- [x] Tìm ý tưởng giải pháp

```text
Chọn đúng Cache-Aside pattern và biết đến event-driven invalidation (thay vì chỉ dùng TTL).
```

---

## 6. Điểm giúp và không giúp

### Giúp tốt

```text
AI phân tích đúng workload characteristic (read-heavy, expensive computation) để đề xuất
Cache-Aside, không phải Write-Through hay Write-Behind.
```

### Không giúp tốt

```text
AI không đề cập cache stampede — vấn đề phổ biến khi nhiều request đồng thời miss cache.
Phải tự nghiên cứu và implement probabilistic early expiration.
```

### Phụ thuộc AI?

- [x] Không phụ thuộc

---

## 8. Ví dụ AI không đề cập

| Nội dung | Mô tả |
|---|---|
| AI không đề cập? | Cache stampede khi nhiều request đồng thời miss cache |
| Hậu quả nếu không biết? | Burst traffic làm AI service overload vì tất cả request tính lại cùng lúc |
| Tự phát hiện bằng cách nào? | Load test k6 với concurrent request khi cache cold |
| Đã xử lý như thế nào? | Probabilistic early expiration: refresh cache khi còn 10% TTL |
| Bài học | Production caching phức tạp hơn lý thuyết. Phải load test để phát hiện vấn đề. |

---

## 9. Đóng góp thật sự

```text
1. Xác định TTL cụ thể (24h/1h) dựa trên chu kỳ update dữ liệu CVerify.
2. Thiết kế cache key namespace scheme.
3. Implement stampede prevention.
4. Implement Redis down fallback.
5. Load test và đo cache hit rate thực tế (87%).
```

---

## 10. So sánh trước và sau

| Area | Before | After | Improvement |
|---|---|---|---|
| Caching knowledge | Biết Redis nhưng không biết pattern nào dùng khi nào | Hiểu trade-off các pattern, chọn đúng cho workload | Cache hit rate 87%, response time giảm 95% |
| Invalidation | Chỉ biết TTL | Biết event-driven invalidation kết hợp TTL | Dữ liệu fresh hơn khi có update |

---

## 16. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/07/2026 |
