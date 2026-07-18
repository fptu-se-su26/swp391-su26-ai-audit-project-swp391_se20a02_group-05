# AI Learning Reflection

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày reflection | 18/07/2026 |

---

## 3. Tóm tắt

```text
Dùng Claude 2 lần để hiểu nguyên lý multi-step LLM pipeline và structure của system prompt
cho extraction task. Tự thiết kế 3-step pipeline, tự viết tất cả prompt, tự implement
Pydantic validation, retry logic và token usage tracking. Điểm quan trọng nhất là nhận ra
LLM output không deterministic — phải luôn có validation và retry.
```

---

## 5. AI hỗ trợ ở điểm nào?

- [x] Tìm ý tưởng giải pháp

```text
Nguyên lý multi-step pipeline thay vì 1 mega-prompt và system prompt structure cho structured
output. Giúp thiết kế đúng từ đầu thay vì trial-and-error.
```

---

## 6. Điểm giúp và không giúp

### Giúp tốt

```text
Giải thích tại sao multi-step tốt hơn mega-prompt: isolation (debug từng step), caching
(cache output step 1-2 nếu JD không đổi), và độ tin cậy (prompt ngắn hơn → output nhất quán hơn).
```

### Không giúp tốt

```text
AI không đề cập Pydantic validation cho pipeline steps, retry logic khi output fail validation,
và token tracking. Đây là các yêu cầu production engineering mà AI không tự nghĩ đến.
```

### Phụ thuộc AI?

- [x] Không phụ thuộc

---

## 8. Ví dụ AI không đề cập

| Nội dung | Mô tả |
|---|---|
| Không đề cập? | LLM output đôi khi không đúng schema (non-deterministic) và cần retry |
| Hậu quả nếu không biết? | Pipeline crash khi LLM trả output không parse được |
| Tự phát hiện khi nào? | Khi test với 20 cặp JD-CV thấy ~3% request fail parse |
| Đã xử lý như thế nào? | Retry exponential backoff max 3 lần, log failure, fallback graceful |
| Bài học | Production LLM system phải luôn defensive — không tin tưởng output 100% |

---

## 9. Đóng góp thật sự

```text
1. Tự thiết kế 3-step pipeline architecture.
2. Tự viết system prompt và few-shot examples cho từng step.
3. Tự implement Pydantic validation.
4. Tự implement retry logic và token tracking.
5. Tự measure accuracy với 20 cặp test.
6. Tự handle edge cases (JD mờ, CV thiếu thông tin).
```

---

## 10. So sánh trước và sau

| Area | Before | After | Improvement |
|---|---|---|---|
| Thiết kế LLM pipeline | Nghĩ gọi 1 API call là đủ | Biết tách thành multi-step với validation | Pipeline reliable hơn, debug dễ hơn |
| System prompt | Không biết structure tốt | Biết role + task + schema + few-shot | Output JSON ổn định hơn đáng kể |
| Production readiness | Không nghĩ đến retry | Retry + validation + fallback | 97% success rate thay vì crash |

---

## 11. Bài học về môn học

```text
1. LLM integration là software engineering task, không chỉ là "gọi API."
2. Reliability của AI feature phụ thuộc vào engineering xung quanh (validation, retry, monitoring)
   nhiều hơn là chất lượng model.
3. Accuracy measurement với ground truth là bắt buộc để biết feature có hoạt động đúng không.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
Ironically, khi build AI feature thì phải áp dụng đúng tư duy "không tin tưởng AI hoàn toàn"
— luôn validate, luôn có fallback. Đây chính là cách dùng AI có trách nhiệm: AI là tool hỗ trợ,
con người/engineer chịu trách nhiệm về chất lượng sản phẩm cuối.
```

---

## 15. Câu hỏi tự vấn

### Phần nào thể hiện năng lực thật sự?

```text
Thiết kế retry logic + Pydantic validation + token tracking — những engineering concern mà AI
không tự đề xuất. Và tự measure accuracy 82% bằng ground truth so với đánh giá thủ công.
```

### Điều muốn cải thiện lần sau?

```text
Thêm observability (trace từng step trong pipeline) để debug production issues nhanh hơn.
Và thử fine-tune prompt để đẩy accuracy từ 82% lên 90%+.
```

---

## 16. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 18/07/2026 |
