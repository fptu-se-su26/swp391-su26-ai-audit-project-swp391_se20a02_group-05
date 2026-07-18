# Changelog

## 2. Thông tin project

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày bắt đầu | 2026-07-06 |
| Ngày hoàn thành | 2026-07-12 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung | Trạng thái |
|---|---|---|---|
| Phase 01 | 06/07/2026 | Nghiên cứu testing patterns với AI | Completed |
| Phase 02 | 07/07/2026 - 10/07/2026 | Viết unit test cho tất cả services | Completed |
| Phase 03 | 11/07/2026 - 12/07/2026 | Fix test isolation, coverage report | Completed |

---

# [Phase 01] Nghiên cứu testing patterns

## Đã hoàn thành

- [x] Hỏi ChatGPT về Mock/Stub/Fake và AAA pattern
- [x] Đọc xUnit docs và Moq docs
- [x] Thiết kế TestFixture base class
- [x] Quyết định dùng [Theory]/[InlineData] cho parametrized tests

## AI có hỗ trợ không?

- [x] Có

```text
ChatGPT giải thích phân biệt Mock/Stub/Fake và nguyên tắc viết test AAA. Không lấy code.
```

---

# [Phase 02] Viết unit test

## Đã hoàn thành

- [x] Test MonitoringAuditService: persist event, normalize severity, broadcast, broadcast fail
- [x] Test HmacSignatureService: verify success, wrong sig, expired timestamp, empty input
- [x] Test MonitoringController: authorized request, unauthorized (wrong sig), timestamp expired
- [x] Test RefreshTokenService: rotation success, token reuse detection, cascade revoke
- [x] Tổng cộng: 47 test cases, tất cả pass

## Thay đổi chi tiết

| STT | Test class | Số test | Pass | Công cụ |
|---:|---|---:|---:|---|
| 1 | MonitoringAuditServiceTests | 15 | 15 | xUnit + Moq |
| 2 | HmacSignatureServiceTests | 12 | 12 | xUnit |
| 3 | MonitoringControllerTests | 8 | 8 | xUnit + Moq |
| 4 | RefreshTokenServiceTests | 12 | 12 | xUnit + Moq |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Fix test isolation và coverage

## Đã hoàn thành

- [x] Fix test isolation issue (test fail khi chạy toàn suite)
- [x] Thêm cleanup trong IDisposable của TestFixture
- [x] Generate coverage report: 78% line coverage
- [x] Identify uncovered paths và thêm edge case tests

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 4. Tổng kết

### Coverage Summary

| Module | Line Coverage | Branch Coverage |
|---|---|---|
| MonitoringAuditService | 92% | 85% |
| HmacSignatureService | 100% | 95% |
| MonitoringController | 78% | 70% |
| RefreshTokenService | 88% | 80% |

---

## 5. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 12/07/2026 |
