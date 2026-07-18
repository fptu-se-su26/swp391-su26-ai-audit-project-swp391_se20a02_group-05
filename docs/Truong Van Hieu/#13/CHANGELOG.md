# Changelog

## 2. Thông tin project

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày bắt đầu | 2026-07-12 |
| Ngày hoàn thành | 2026-07-18 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung | Trạng thái |
|---|---|---|---|
| Phase 01 | 12/07/2026 | Nghiên cứu, hỏi AI, thiết kế pipeline | Completed |
| Phase 02 | 13/07/2026 - 15/07/2026 | Implement 3-step pipeline và FastAPI endpoints | Completed |
| Phase 03 | 16/07/2026 - 17/07/2026 | Test accuracy và edge cases | Completed |
| Phase 04 | 18/07/2026 | Tài liệu | Completed |

---

# [Phase 01] Nghiên cứu và thiết kế

## Đã hoàn thành

- [x] Hỏi Claude về structured output và multi-step pipeline
- [x] Đọc Anthropic API docs về system prompts và Messages API
- [x] Thiết kế 3-step pipeline: Extract JD → Extract Candidate → Match + Score + Stream
- [x] Thiết kế Pydantic schemas cho output của từng step
- [x] Thiết kế retry logic schema

## AI có hỗ trợ không?

- [x] Có

```text
Claude giải thích nguyên lý multi-step pipeline và khi nào nên stream. Không lấy code.
```

---

# [Phase 02] Implementation

## Đã hoàn thành

- [x] Implement AnalysisPipeline class với 3 step orchestration
- [x] Implement Step 1: JDExtractor — extract requirements, skills, experience từ JD text
- [x] Implement Step 2: CandidateExtractor — extract profile từ CV text
- [x] Implement Step 3: MatchingScorer — compare và stream explanation
- [x] Pydantic validation sau mỗi step
- [x] Retry logic với exponential backoff (max 3 attempts, 1s/2s/4s)
- [x] Token usage logging
- [x] FastAPI endpoints: POST /api/v1/analysis/jd-match (stream SSE)

## Thay đổi chi tiết

| STT | File | Nội dung | Minh chứng |
|---:|---|---|---|
| 1 | `CVerify.AI/app/core/pipeline/analysis_pipeline.py` | 3-step orchestration | Unit test pass |
| 2 | `CVerify.AI/app/api/routes/analysis_router.py` | FastAPI endpoints với SSE streaming | Integration test |
| 3 | `CVerify.AI/app/core/models/analysis_models.py` | Pydantic schemas cho pipeline output | Validation test |

## Lỗi đã xử lý

| STT | Lỗi | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Claude API đôi khi trả JSON không đúng format | LLM non-deterministic output | Retry với exponential backoff, log failure | Fixed |
| 2 | Stream bị ngắt khi client disconnect | Không check disconnect trong generator | Thêm `await request.is_disconnected()` trong generator | Fixed |
| 3 | Token count quá lớn với CV dài | Không có token limit | Thêm preprocessing cắt CV xuống max_tokens trước khi gửi | Fixed |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Testing và accuracy measurement

## Đã hoàn thành

- [x] Test 20 cặp JD-CV thủ công so sánh với AI score
- [x] Accuracy JD matching: 82% match với đánh giá thủ công
- [x] Test edge case: JD mơ hồ, CV thiếu thông tin
- [x] Unit test với 19 test cases cho AnalysisPipeline
- [x] Load test: 10 concurrent analysis requests

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 4. Tổng kết

### Kết quả đạt được

| Metric | Giá trị | Ghi chú |
|---|---|---|
| JD Matching accuracy | 82% | So sánh với đánh giá thủ công 20 cặp |
| p95 response time (full analysis) | 3.2s | Bao gồm 3 Claude API calls |
| p95 time-to-first-token (streaming) | 420ms | Stream step 3 |
| Retry success rate | 97% | Khi step fail, retry thành công |

---

## 5. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 18/07/2026 |
