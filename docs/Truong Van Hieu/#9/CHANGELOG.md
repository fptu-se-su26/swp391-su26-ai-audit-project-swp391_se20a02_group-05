# Changelog

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày bắt đầu | 2026-07-05 |
| Ngày hoàn thành | 2026-07-10 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 05/07/2026 | Nghiên cứu caching patterns, chọn Cache-Aside | Completed |
| Phase 02 | 06/07/2026 - 08/07/2026 | Thiết kế và implement caching layer | Completed |
| Phase 03 | 09/07/2026 - 10/07/2026 | Load test và tài liệu | Completed |

---

# [Phase 01] Nghiên cứu

## Đã hoàn thành

- [x] Hỏi ChatGPT so sánh Cache-Aside, Write-Through, Write-Behind
- [x] Quyết định Cache-Aside + event-driven invalidation
- [x] Xác định TTL: JD analysis 24h, candidate score 1h
- [x] Thiết kế cache key namespace: `cverify:analysis:{type}:{id}`

## AI có hỗ trợ không?

- [x] Có

```text
ChatGPT so sánh các caching pattern và gợi ý event-driven invalidation. Không lấy code.
```

---

# [Phase 02] Implementation

## Đã hoàn thành

- [x] Implement ICacheService interface wrapping IDistributedCache
- [x] Implement CacheService với StackExchange.Redis
- [x] Tích hợp cache vào AnalysisService (Cache-Aside pattern)
- [x] Implement event-driven invalidation khi JD hoặc candidate CV thay đổi
- [x] Implement stampede prevention với probabilistic early expiration
- [x] Implement Redis down fallback (degraded mode)

## Lỗi đã xử lý

| STT | Lỗi | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Cache stampede khi load test | Tất cả request miss cùng lúc và tính lại | Probabilistic early expiration: refresh khi còn 10% TTL | Fixed |
| 2 | Redis timeout làm request hang | Connection timeout không được set | Thêm timeout config và fallback mode | Fixed |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Load Test

## Đã hoàn thành

- [x] Load test với k6: 50 concurrent users, 5 phút
- [x] Cache hit rate: 87% sau warm-up
- [x] Average response time: giảm từ 2.3s → 120ms khi cache hit
- [x] Redis down scenario: fallback hoạt động, degraded mode response time 2.4s

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 4. Tổng kết

### Kết quả đạt được

| Metric | Trước cache | Sau cache | Cải thiện |
|---|---|---|---|
| Response time (cache hit) | 2.3s | 120ms | Giảm 95% |
| Cache hit rate (sau warm-up) | N/A | 87% | |
| Throughput | ~20 req/s | ~150 req/s | Tăng 7.5x |

---

## 5. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/07/2026 |
