# Prompt Log

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày cập nhật | 2026-07-10 |

---

## 4. Bảng tổng hợp

| STT | Ngày | AI | Mục đích | Prompt tóm tắt | Kết quả | Có dùng? |
|---:|---|---|---|---|---|---|
| 1 | 05/07/2026 | ChatGPT | So sánh caching patterns và TTL strategy cho AI results | "So sánh Cache-Aside, Write-Through, Write-Behind... Bài toán cache AI analysis results 2-3s..." | Chọn Cache-Aside, event-driven invalidation | Có (Pattern decision) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Giải thích sự khác biệt giữa các caching pattern: Cache-Aside (Lazy Loading), Write-Through
và Write-Behind trong Redis. Khi nào nên dùng pattern nào? Bài toán của tôi là cache kết quả
phân tích AI (JD matching score, candidate analysis) trên CVerify — kết quả này tốn ~2-3 giây
để tính. TTL nên đặt bao nhiêu và làm thế nào để invalidate cache khi có dữ liệu mới?
```

#### 5.2. Kết quả áp dụng

- Chọn Cache-Aside (read-heavy, dữ liệu có thể stale).
- TTL tự xác định: 24h cho JD analysis, 1h cho candidate score.
- Áp dụng thêm event-driven invalidation khi data thay đổi.
- Tự thiết kế stampede prevention (AI không đề cập).

#### 5.3. Đánh giá

- [x] Prompt có bối cảnh cụ thể (thời gian tính ~2-3s)
- [x] Kết quả tốt, AI đề xuất đúng pattern

---

## 6. Bài học về prompt

```text
Mô tả "tốn ~2-3 giây để tính" giúp AI hiểu đây là expensive computation và đề xuất đúng
caching pattern. Số liệu cụ thể trong prompt giúp AI phân tích trade-off chính xác hơn.
```

---

## 9. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/07/2026 |
