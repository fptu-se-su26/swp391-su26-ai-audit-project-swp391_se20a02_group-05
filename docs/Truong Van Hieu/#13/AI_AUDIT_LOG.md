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
| Ngày bắt đầu | 2026-07-12 |
| Ngày hoàn thành | 2026-07-18 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là hiểu cách thiết kế pipeline phân tích CV ứng viên (JD Matching, Candidate
Scoring) sử dụng Large Language Model (Claude API) và cách tổ chức các bước xử lý trong
FastAPI service của CVerify.AI. Tôi muốn hỏi AI để hiểu nguyên lý prompt engineering cho
structured output và streaming, không dùng AI để viết code.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 12/07/2026 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Hiểu prompt engineering cho structured JSON output từ LLM |
| Phần việc liên quan | Thiết kế AI analysis pipeline |
| Mức độ sử dụng | Hỏi kiến thức |

#### 4.1. Prompt đã sử dụng

```text
Khi dùng Claude API để phân tích JD và CV của ứng viên, làm thế nào để đảm bảo response
trả về JSON structured đúng format? Tôi có nên dùng system prompt để định nghĩa schema,
hay có cách nào tốt hơn? Ngoài ra, để stream token từ Claude về frontend, pipeline nên
tổ chức như thế nào — xử lý trong một chain hay tách thành nhiều step riêng biệt?
```

#### 4.2. Kết quả AI gợi ý

AI giải thích: để có structured JSON output ổn định, nên dùng system prompt định nghĩa rõ
schema và dùng JSON mode (nếu API hỗ trợ) hoặc yêu cầu output format cụ thể trong prompt.
Với pipeline phức tạp nên tách thành nhiều step: extract → analyze → score — mỗi step có
prompt riêng và output là input của step sau. Streaming phù hợp cho step cuối hiển thị kết
quả cho user, không phải intermediate steps.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu nguyên lý tách pipeline thành nhiều step thay vì 1 mega-prompt, và chỉ stream ở
step cuối. Áp dụng vào thiết kế AnalysisPipeline: Step 1 (extract JD requirements) → Step
2 (extract candidate profile) → Step 3 (match + score + stream explanation).

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi tự thiết kế schema validation bằng Pydantic để validate output từ mỗi step trước khi
truyền sang step tiếp theo — AI không đề cập. Ngoài ra tự thiết kế retry logic với exponential
backoff cho trường hợp Claude API trả về output không đúng schema.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: Claude giải thích rõ tại sao nên tách pipeline thành nhiều step (isolation,
  debuggability, caching từng step) thay vì 1 mega-prompt dễ fail.
- Bài học: Khi dùng LLM trong production, phải luôn validate output và có retry logic vì
  LLM không đảm bảo output nhất quán 100%.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | Ghi chú |
|---|:---:|:---:|:---:|---|
| Thiết kế AI pipeline |  | [x] |  | Tham khảo nguyên lý multi-step và streaming |
| Viết prompt cho Claude API | [x] |  |  | Tự viết và test prompt |
| Code FastAPI service | [x] |  |  | Tự code 100% |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Hạn chế | Cách xử lý |
|---:|---|---|
| 1 | AI không đề cập Pydantic validation cho structured output. | Tự thêm Pydantic model cho output của mỗi step và validate trước khi truyền sang step tiếp. |
| 2 | AI không đề cập retry logic khi LLM output không đúng schema. | Tự implement retry với exponential backoff (tối đa 3 lần) khi validation fail. |
| 3 | AI không đề cập cost tracking khi dùng Claude API theo token. | Tự thêm token usage logging trong response metadata để track chi phí. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc Anthropic API documentation về system prompts và structured output.
2. Test từng step của pipeline riêng lẻ với nhiều JD và CV sample.
3. Test edge case: JD không rõ ràng, CV thiếu thông tin — đảm bảo pipeline không crash.
4. Đo accuracy của JD matching score bằng cách so sánh với đánh giá thủ công.
```

---

## 8. Đóng góp cá nhân

```text
- Tự thiết kế 3-step pipeline (extract JD → extract candidate → match+score+stream).
- Tự viết system prompt cho từng step.
- Tự implement Pydantic validation cho output từng step.
- Tự implement retry logic với exponential backoff.
- Tự implement token usage logging.
- Tự test và measure accuracy.
```

---

## 9. Reflection cuối bài

### 9.1. AI hỗ trợ ở điểm nào?

```text
Nguyên lý tách pipeline thành nhiều step thay vì 1 mega-prompt — giúp thiết kế đúng từ đầu
và dễ debug khi có vấn đề ở step nào đó.
```

### 9.2. Học được gì về môn học?

```text
Tích hợp LLM vào production system cần nhiều engineering hơn là chỉ gọi API: validate output,
retry logic, cost tracking, caching. LLM là một component trong pipeline, không phải toàn bộ giải pháp.
```

### 9.3. Học được gì về sử dụng AI có trách nhiệm?

```text
Ironically, khi build hệ thống AI thì phải apply đúng kỹ thuật engineering để đảm bảo
reliability — AI không tự làm đúng mọi lúc, cần validation và fallback. Tương tự như việc
dùng AI tool: không tin tưởng hoàn toàn kết quả, phải kiểm tra.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 18/07/2026 |
