# Prompt Log

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày cập nhật | 2026-07-18 |

---

## 4. Bảng tổng hợp

| STT | Ngày | AI | Mục đích | Prompt tóm tắt | Kết quả | Dùng vào bài? |
|---:|---|---|---|---|---|---|
| 1 | 12/07/2026 | Claude | Nguyên lý structured output và multi-step LLM pipeline | "Claude API để phân tích JD và CV... đảm bảo JSON output... pipeline nên tổ chức thế nào..." | Hiểu multi-step, chỉ stream ở step cuối | Có (Thiết kế pipeline) |
| 2 | 13/07/2026 | Claude | Cách viết system prompt cho extraction task | "System prompt cho task extract requirements từ JD text nên có những gì..." | Nguyên lý few-shot, role, output schema | Có (Định hướng viết prompt) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Khi dùng Claude API để phân tích JD và CV của ứng viên, làm thế nào để đảm bảo response
trả về JSON structured đúng format? Tôi có nên dùng system prompt để định nghĩa schema,
hay có cách nào tốt hơn? Ngoài ra, để stream token từ Claude về frontend, pipeline nên
tổ chức như thế nào — xử lý trong một chain hay tách thành nhiều step riêng biệt?
```

#### 5.2. Kết quả AI trả về và áp dụng

- Tách pipeline thành 3 step độc lập (extract → extract → match+stream).
- System prompt định nghĩa rõ output schema cho từng step.
- Chỉ stream ở step cuối (user-facing).

---

### Prompt số 2

#### 5.1. Prompt nguyên văn

```text
System prompt cho task extract requirements từ Job Description text nên có những thành
phần gì để Claude trả về JSON ổn định nhất? Tôi cần extract: required_skills (list),
experience_years (int), education_level (string), responsibilities (list). Có nên dùng
few-shot examples không?
```

#### 5.2. Kết quả AI trả về

AI đề xuất structure: role definition + task description + output schema definition + few-shot
example (1-2 là đủ, không cần nhiều) + instruction "Only return JSON, no explanation."

#### 5.3. Áp dụng

Áp dụng structure này vào system prompt cho JDExtractor step. Tự viết few-shot example và
schema definition. Tự test prompt với nhiều JD sample.

---

## 6. Bài học về cách viết prompt

```text
1. Prompt hỏi AI về nguyên lý ("pipeline nên tổ chức thế nào") cho insight sâu hơn là hỏi
   "viết code pipeline cho tôi."
2. Khi cần chi tiết cụ thể (system prompt structure), hỏi prompt riêng với context đầy đủ
   (list field cần extract) cho kết quả cụ thể hơn.
3. Tự test và iterate prompt thực tế quan trọng hơn thiết kế prompt "hoàn hảo" trên lý thuyết.
```

---

## 9. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 18/07/2026 |
