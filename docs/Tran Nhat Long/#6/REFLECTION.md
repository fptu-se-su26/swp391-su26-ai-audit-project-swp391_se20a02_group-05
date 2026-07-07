# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-06-12 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển phần mềm.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Phiên làm việc 2026-06-12 tập trung vào hai mục tiêu chính:

1. Implement v2 pipeline tasks (L1-003 through L1-018) cho CVerify.AI Python service,
   align với 18-task DAG đã được định nghĩa sẵn trong CVerify.Core DagScheduler.
   Developer đã có sẵn DAG spec và formula weights từ research document trước đó.
   AI (Claude Code) được dùng để sinh code implement cho 10 task methods (~900 lines)
   dựa trên spec chi tiết.

2. Build token debug infrastructure để hỗ trợ observability trong dev/staging:
   AI_DEBUG_TOKENS flag, JSONL writer per-job, HMAC-signed PowerShell invoker.

Vai trò của AI trong phiên này: code generator và workflow executor.
Developer đã có sẵn architectural decisions và spec — AI chịu trách nhiệm
translate spec thành code implementation.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity

---

## 5. Phân tích chi tiết

### 5.1. Những điều AI làm tốt

```text
1. Pattern table generation: _COMPLEXITY_PATTERNS với 30 entries, ordered by
   specificity (most specific first) — AI correctly understood the "most specific
   wins" resolution rule and ordered accordingly.

2. Formula implementation: TrustScore formula (Evidence×0.40 + Ownership×0.35 +
   Consistency×0.25 with adversarial penalties) được AI implement chính xác
   theo spec. AI không tự ý thay đổi weights.

3. CommitIntent signal isolation: AI correctly avoided reading commit messages
   as primary signal in analyze_commit_intent(), chỉ sử dụng diff cache.
   Đây là một constraint tinh tế nhưng AI đã hiểu và tuân thủ đúng.

4. JSONL over JSON for concurrent writes: AI chủ động chọn JSONL append mode
   thay vì JSON rewrite, phù hợp với môi trường parallel task execution.

5. Non-critical error handling: Token debug writer được wrap trong try/except
   với logger.debug() — AI hiểu rằng observability code không được break
   the pipeline nếu thất bại.
```

### 5.2. Những điều AI làm chưa tốt / cần điều chỉnh

```text
1. CloneDetection thresholds: AI dùng threshold tương đối conservative cho
   commit bomb detection. Sau khi test với educational repos thực tế,
   cần tune threshold cao hơn để giảm false positive.

2. analyze_git_blame() git subprocess: AI hardcode git binary path và không
   handle trường hợp git không available trong Docker environment.
   Cần thêm fallback logic.
```

### 5.3. Quyết định kỹ thuật quan trọng

```text
1. JSONL vs JSON cho token_debug:
   Decision: Dùng JSONL (append-only, one record per line)
   Reasoning: Parallel pipeline tasks có thể ghi đồng thời vào cùng job workspace.
   JSON rewrite sẽ require locking hoặc gây race condition.
   JSONL append là atomic ở file-system level cho small writes.

2. Shared helpers thay vì inline logic:
   Decision: Extract _read_meta(), _read_task_cache(), _clone_dir(), _empty_result()
   thành instance methods.
   Reasoning: Cả 10 task methods cần access cùng workspace paths.
   Centralize path resolution tránh path drift khi Docker mount points thay đổi.

3. Class-level _COMPLEXITY_PATTERNS:
   Decision: Define pattern table tại class level, không phải module level.
   Reasoning: Giữ pattern table gần với methods sử dụng nó.
   Class-level attribute không allocate memory per-instance.

4. Priority ordering trong _COMPLEXITY_PATTERNS:
   Decision: Most specific patterns first (kubernetes/helm trước cloud/aws).
   Reasoning: First-match semantics — specific patterns phải win trước generic
   platform patterns. Ordering là load-bearing, documented trong comment.

5. Adversarial flag penalties trong TrustScore:
   Decision: Apply penalties (−0.15 mỗi flag) nhưng floor TrustScore tại 0.0.
   Reasoning: Negative trust score không có nghĩa semantic. Floor tại 0
   để preserve ordering (cùng 0 = cùng mức flagged) mà không gây confusion.
```

### 5.4. Bài học rút ra

```text
1. Spec-driven AI generation hiệu quả hơn open-ended generation:
   Khi có sẵn formula weights, pattern categories, và signal sources cụ thể,
   AI có thể generate code chính xác với ít iteration. Phiên này chứng minh
   việc có research document trước khi code giúp AI output quality tốt hơn.

2. Constraint có thể được truyền đạt ngắn gọn nhưng hiệu quả:
   "never commit messages as primary signal" — một constraint ngắn nhưng AI
   hiểu và tuân thủ đúng trong implementation. AI không cần constraint dài dòng.

3. Infrastructure code (debug tools) benefit từ AI nhiều nhất:
   HMAC-signed PowerShell invoker là boilerplate điển hình — AI generate nhanh,
   chính xác. Developer tập trung vào business logic thay vì viết debug tooling.

4. Audit documentation phản ánh thực tế thay vì lý tưởng:
   REFLECTION này ghi nhận cả CloneDetection threshold issue và git blame
   subprocess limitation — không chỉ ghi thành công. Honest audit log
   có giá trị học tập cao hơn.
```

---

## 6. Tự đánh giá

| Tiêu chí | Mức độ | Ghi chú |
|---|---|---|
| Hiểu rõ code AI sinh ra | ✅ Tốt | Developer review từng method, hiểu formula và pattern logic |
| Kiểm tra kết quả AI | ✅ Tốt | Test CloneDetection với sample repos, tune thresholds |
| Không phụ thuộc mù quáng vào AI | ✅ Tốt | AI chỉ translate spec đã có; design decisions là của developer |
| Ghi lại quá trình đầy đủ | ✅ Tốt | 3 lần sử dụng AI được ghi chi tiết |
| Học hỏi từ AI | ✅ Tốt | JSONL over JSON decision được adopt từ AI recommendation |

---

## 7. Kế hoạch cải thiện

```text
1. Test suite cho v2 pipeline tasks:
   Viết unit tests cho từng task method với mock git blame output và mock
   LLM responses. Đặc biệt test CloneDetection với edge cases.

2. Fix git blame subprocess:
   Thêm fallback khi git binary không available (Docker environment).
   Có thể dùng GitPython library thay vì subprocess.

3. Expose token debug endpoint:
   GET /api/v1/debug/tokens/{job_id} endpoint đang được mention trong
   .env.example nhưng chưa implemented. Cần implement để hoàn thiện
   token debug infrastructure.
```
