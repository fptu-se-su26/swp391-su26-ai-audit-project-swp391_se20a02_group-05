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
| Ngày bắt đầu | 2026-07-05 |
| Ngày hoàn thành | 2026-07-10 |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là hiểu các caching pattern phổ biến (Cache-Aside, Write-Through, Write-Behind) và
khi nào nên dùng TTL như thế nào cho việc cache kết quả phân tích AI của CVerify. Việc
cache sai pattern sẽ làm dữ liệu stale hoặc miss cache liên tục — cần hiểu nguyên lý trước
khi thiết kế. Không dùng AI để viết code.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 05/07/2026 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | So sánh các caching pattern và khi nào nên dùng TTL cho AI analysis results |
| Phần việc liên quan | Thiết kế caching layer |
| Mức độ sử dụng | Hỏi kiến thức |

#### 4.1. Prompt đã sử dụng

```text
Giải thích sự khác biệt giữa các caching pattern: Cache-Aside (Lazy Loading), Write-Through
và Write-Behind trong Redis. Khi nào nên dùng pattern nào? Bài toán của tôi là cache kết quả
phân tích AI (JD matching score, candidate analysis) trên CVerify — kết quả này tốn ~2-3 giây
để tính. TTL nên đặt bao nhiêu và làm thế nào để invalidate cache khi có dữ liệu mới?
```

#### 4.2. Kết quả AI gợi ý

AI so sánh: Cache-Aside phù hợp cho read-heavy, dữ liệu có thể stale — app đọc cache trước,
miss thì tính và lưu. Write-Through đảm bảo cache luôn đồng bộ DB — phù hợp khi cần consistency.
Write-Behind (async) phù hợp write-heavy nhưng phức tạp. Với AI analysis results tốn thời gian
tính, Cache-Aside là phù hợp nhất. TTL nên dựa trên chu kỳ dữ liệu thay đổi. AI cũng đề cập
event-driven invalidation thay vì chỉ dùng TTL.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu quyết định dùng Cache-Aside pattern và ý tưởng về event-driven invalidation. Tự xác
định TTL cụ thể dựa trên chu kỳ update dữ liệu của CVerify: JD analysis 24 giờ (JD không thay
đổi thường xuyên), candidate score 1 giờ (vì candidate có thể cập nhật CV).

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi bổ sung cache key design (namespace:resource:id) và stampede prevention bằng probabilistic
early expiration — AI không đề cập. Ngoài ra thiết kế fallback khi Redis down: tính trực tiếp
không cache thay vì throw error.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: ChatGPT giải thích rõ trade-off giữa các pattern, giúp tôi chọn đúng
  Cache-Aside cho bài toán này.
- Bài học: TTL không phải là cách invalidation duy nhất — event-driven invalidation kết hợp
  TTL là giải pháp tốt hơn cho dữ liệu có thể thay đổi như candidate score.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Thiết kế caching layer |  | [x] |  |  | Tham khảo caching patterns |
| Code Redis integration | [x] |  |  |  | Tự code 100% |
| Thiết kế cache key | [x] |  |  |  | Tự thiết kế namespace scheme |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế | Cách phát hiện | Cách xử lý |
|---:|---|---|---|
| 1 | AI không đề cập cache stampede (nhiều request đồng thời miss cache và tất cả cùng tính) | Phát hiện khi load test với nhiều concurrent request khi cache miss | Implement probabilistic early expiration: refresh cache khi còn 10% TTL thay vì đợi expire hoàn toàn |
| 2 | AI không đề cập Redis down fallback | Thiết kế chủ động dựa trên yêu cầu reliability | Implement try/catch: nếu Redis error thì tính trực tiếp (degraded mode), không throw |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc tài liệu Redis về caching best practices.
2. Load test với k6 để phát hiện stampede và xác nhận cache hit rate.
3. Test Redis down scenario để xác nhận fallback hoạt động.
```

---

## 8. Đóng góp cá nhân

```text
- Tự quyết định Cache-Aside pattern và event-driven invalidation.
- Tự thiết kế TTL cụ thể (24h/1h) dựa trên bài toán CVerify.
- Tự thiết kế cache key namespace scheme.
- Tự implement stampede prevention.
- Tự implement Redis down fallback.
- Tự load test để đo cache hit rate thực tế.
```

---

## 9. Reflection cuối bài

### 9.1. AI hỗ trợ ở điểm nào?

```text
AI giúp chọn đúng caching pattern (Cache-Aside) thay vì áp dụng sai pattern (Write-Through
cho read-heavy workload) và gợi ý event-driven invalidation — điểm quan trọng mà tôi chưa nghĩ đến.
```

### 9.2. Học được gì?

```text
Caching pattern phải phù hợp với workload (read-heavy vs write-heavy) và tính chất dữ liệu
(consistency requirement). Không có one-size-fits-all. TTL kết hợp event invalidation tốt hơn
chỉ TTL đơn thuần.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/07/2026 |
