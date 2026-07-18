# Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Tên project | CVerify |
| Tên sinh viên | Trần Nhất Long |
| MSSV | DE200160 |
| Ngày | 2026-06-17 |
| Phiên số | #8 |
| Tính năng | Line 3 — JD Matching Pipeline (L3-005 đến L3-015) |

---

## 2. Quyết định kỹ thuật quan trọng

### 2.1. Weighted Formula cho Match Score Aggregation

**Quyết định:** Sử dụng công thức `Skill×0.35 + Resp×0.25 + Sen×0.20 + Sal×0.10 + Cult×0.10`

**Lý do:**
- Skill matching (35%) là yếu tố quan trọng nhất vì technical fit là tiêu chí cốt lõi tuyển dụng
- Responsibility match (25%) phản ánh khả năng thực hiện công việc thực tế
- Seniority (20%) quan trọng nhưng có thể linh hoạt (senior có thể làm mid-level role)
- Salary (10%) và Culture fit (10%) là yếu tố secondary — mismatch có thể thương lượng

**Trade-off:**
- Weighted average có thể "che khuất" mismatch nghiêm trọng ở một dimension
- Giải quyết bằng Cap Rules (L3-012): hard override khi salary=0 hoặc skill<0.4

---

### 2.2. Hard Cap Rules thay vì Blocking

**Quyết định:** Khi salary=0 (hard mismatch), cap Match Score ≤ 60% thay vì reject hoàn toàn

**Lý do:**
- Ứng viên vẫn có quyền apply kể cả khi có mismatch (confirmed bởi task spec L3-014)
- Cap ≤ 60% signal rõ ràng cho recruiter mà không block candidate
- Approach này nhất quán với UX philosophy của Quality Gate: warn + confirm, không block

**Trade-off:**
- Recruiter cần đọc kỹ warnings thay vì dựa hoàn toàn vào score
- Giải quyết bằng Gap Analysis (L3-013) với mô tả cụ thể cho từng warning

---

### 2.3. Semantic Skill Matching với SkillAliases Dictionary

**Quyết định:** Sử dụng static alias dictionary thay vì embedding-based semantic search

**Lý do:**
- Embedding search cần vector DB (overhead cho MVP)
- Alias dictionary bao phủ 80%+ common tech stack variations ("reactjs"→"react", "nodejs"→"node.js")
- Deterministic và dễ debug hơn embedding similarity

**Trade-off:**
- Không handle aliases chưa có trong dictionary
- Có thể bổ sung thêm aliases dần dần hoặc migrate sang embedding search ở phase sau

---

### 2.4. Tách JdMatchingService khỏi JdService

**Quyết định:** Tạo `JdMatchingService` và `IJdMatchingService` riêng biệt thay vì gộp vào `JdService`

**Lý do:**
- Single Responsibility Principle: JdService quản lý CRUD; JdMatchingService chỉ xử lý matching logic
- JdMatchingService có thể mock độc lập trong unit tests
- Dễ scale riêng nếu matching cần compute intensive optimization sau này

---

### 2.5. Application Quality Gate là UI Component, không phải Backend Block

**Quyết định:** Quality Gate (L3-014) implement ở frontend với explicit confirm flow

**Lý do:**
- Task spec L3-014 nói rõ: "Ứng viên vẫn có thể apply sau confirm — không block hoàn toàn"
- Backend không nên block apply request ngay cả khi có mismatch
- Frontend warning đủ để inform ứng viên về risks

**Outcome:**
- `application-quality-gate.tsx` show warnings (salary, skills gap, seniority mismatch)
- Require confirm button nếu có warnings
- Không disable apply button

---

## 3. Quan sát kỹ thuật

### 3.1. AI Orchestrator Task Routing

Mỗi task trong Line 3 được route qua `execute_task()` dispatcher trong `orchestrator.py`.
Pattern nhất quán: mỗi task handler nhận `(job_id, inputs, correlation_id)` và trả về
standardized `_ok()` hoặc `_err()` response. Pattern này inherited từ Line 1 và Line 2,
giúp monitoring và error handling nhất quán across toàn bộ pipeline.

### 3.2. Database Migration Strategy

Migration `AddLine3JdMatching` thêm cả bảng mới (`StandardizedJds`) và columns mới
(`DesiredSalary`, `MinAcceptableSalary` vào `CareerPreferences`). Không dùng nullable
với default value để tránh breaking existing data — columns mới là nullable.

### 3.3. Excel-Driven Task Verification

Workflow đọc trực tiếp từ Excel file task breakdown để xác định scope. Điều này tạo
một "source of truth" rõ ràng cho tasks cần implement, giảm risk missing tasks.
AI có thể đọc structured data từ Excel và cross-reference với code implementation.

---

## 4. Điều sẽ làm khác nếu làm lại

```text
1. Implement salary currency normalization (VND/USD) đầy đủ hơn — hiện tại chỉ
   validate range, chưa handle conversion rate.

2. Culture Fit scoring (L3-010) dùng heuristic đơn giản. Nếu có thêm data về
   candidate working style preference, scoring sẽ chính xác hơn.

3. Responsibility Match Engine (L3-006) dùng keyword coverage. Có thể cải thiện
   bằng NLP sentence embedding để match semantic meaning thay vì keyword overlap.
```

---

## 5. Kết luận

Line 3 JD Matching Pipeline là pipeline phức tạp nhất trong CVerify với 5 matching
dimensions và nhiều business rules. Việc phân chia rõ ràng thành 3A/3B/3C/3D giúp
implement từng phần độc lập và test riêng biệt.

Weighted formula kết hợp với hard cap rules tạo ra hệ thống scoring vừa nuanced
(tính trọng số từng dimension) vừa có guardrails (không cho score cao khi có hard mismatch).

Gap Analysis và Hiring Recommendation (L3-013, L3-015) sử dụng LLM để tạo natural
language explanation, giúp recruiter và candidate hiểu lý do match/mismatch mà
không cần đọc raw numbers.
