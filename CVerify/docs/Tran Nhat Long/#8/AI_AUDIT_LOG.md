# AI Audit Log

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
| Ngày bắt đầu | 2026-06-17 |
| Ngày hoàn thành | 2026-06-17 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Implement toàn bộ Line 3 — JD Matching Pipeline (L3-005 đến L3-015) cho hệ thống CVerify.

Line 3 là pipeline match ứng viên với công việc, bao gồm:
- 3B: Skill & Responsibility Matching (L3-005, L3-006, L3-007)
- 3C: Salary & Culture Fit Matching (L3-008, L3-009, L3-010)
- 3D: Match Aggregation & Output (L3-011 đến L3-015)

AI được dùng để implement toàn bộ backend engine (.NET), AI orchestrator (Python),
frontend components (React/TypeScript), database migration, và test suite.
```

---

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-17 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích sử dụng | Kiểm tra trạng thái tasks Line 3 từ file Excel task breakdown và commit các tasks còn lại (L3-005 đến L3-015) |
| Phần việc liên quan | Backend / AI Pipeline / Frontend / Database |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
@"C:\Users\long2\OneDrive\Documents\Claude\Projects\SWP391\CVerify_AI_Pipeline_v2_Task_Breakdown.xlsx"
đọc file và kiểm tra các task line 3 xem đã hoàn thành chưa, nếu rồi thì tạo commit, push và tạo PR để merge vào CVerify-uat
```

#### 4.2. Kết quả AI gợi ý

```text
AI đọc file Excel CVerify_AI_Pipeline_v2_Task_Breakdown.xlsx, phân tích sheet
"Line 3 — JD Matching" và xác định:

- L3-001 đến L3-004 (3A. Standardized JD System) đã được implement trong commit cca11ef
- L3-005 đến L3-015 (3B/3C/3D) đã được implement trong working tree chưa commit

AI kiểm tra các file chưa commit:
  - CVerify.Core/Modules/Jd/Services/JdMatchingService.cs (371 dòng)
  - CVerify.Core/Modules/Jd/Services/IJdMatchingService.cs
  - CVerify.Core/Modules/Jd/Entities/StandardizedJd.cs
  - CVerify.Core/Migrations/20260616183936_AddLine3JdMatching.cs
  - CVerify.AI/app/pipelines/jd/orchestrator.py (517 dòng, 11 task handlers mới)
  - client/src/modules/business/components/application-quality-gate.tsx
  - Tests: JdMatchingServiceTests.cs, JdMatchingApiTests.cs, test_line3_pipeline.py

Xác nhận tất cả 15 tasks đã implement đầy đủ, sau đó:
  - Stage 29 files cụ thể
  - Commit: "feat: implement Line 3 JD Matching Pipeline (L3-005 to L3-015)"
  - Push lên origin/feature-business-JD
  - Tạo PR #84 targeting CVerify-uat với đầy đủ mô tả
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ implementation của JdMatchingService.cs (371 dòng):
  + SkillAliases dictionary (semantic normalization)
  + SeniorityLevels mapping
  + CalculateMatch() — entry point
  + MatchSkills(), MatchResponsibilities(), CalculateSeniorityScore()
  + CalculateSalaryScore(), CalculateCultureFitScore()
  + AggregateScore() với weighted formula
  + ApplyCapRules() — hard override rules
  + GenerateGapAnalysis() — missing skills + improvement suggestions
  + GenerateHiringRecommendation() — Yes/Conditional/No logic

- AI orchestrator expansion (orchestrator.py):
  + _skill_match_calculator() — L3-005
  + _responsibility_match_engine() — L3-006
  + _seniority_match_calculator() — L3-007
  + _candidate_salary_fields() — L3-008
  + _salary_match_calculator() — L3-009
  + _culture_role_fit_analyzer() — L3-010
  + _match_score_aggregator() — L3-011
  + _match_score_cap_rule() — L3-012
  + _gap_analysis_engine() — L3-013
  + _application_quality_gate() — L3-014
  + _hiring_recommendation_generator() — L3-015

- application-quality-gate.tsx (React component):
  + Pre-apply warning UI với salary/skill/seniority warnings
  + Explicit confirmation flow trước khi apply

- DB migration: AddLine3JdMatching
  + StandardizedJd entity
  + DesiredSalary + MinAcceptableSalary fields cho CareerPreference

- Test suite:
  + JdMatchingServiceTests.cs (unit tests)
  + JdMatchingApiTests.cs (integration tests)
  + test_line3_pipeline.py (AI orchestrator tests)

- Commit message, PR body, reviewer assignment
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Xem xét file Excel task breakdown để xác định scope của Line 3
- Xác nhận các task L3-001 đến L3-004 đã commit trước đó (cca11ef)
- Verify implementation có đủ cho tất cả 15 tasks trước khi commit
- Kiểm tra git diff --stat để confirm scope thay đổi (29 files, 8652 insertions)
- Quyết định PR target là CVerify-uat (không phải main)
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp cao — AI đọc đúng Excel file, xác định đúng tasks còn lại, verify code
đã implement, commit đúng files, và tạo PR với description đầy đủ cho tất cả 15 tasks.
Weighted formula (Skill×0.35 + Resp×0.25 + Sen×0.20 + Sal×0.10 + Cult×0.10) đúng
với spec trong task breakdown.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-17 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-4-6) |
| Mục đích sử dụng | Tạo audit documentation package #8 cho Line 3 JD Matching Pipeline |
| Phần việc liên quan | Documentation / Workflow |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
/cverify-code-to-ai-audit update for today's audit work
```

#### 4.2. Kết quả AI gợi ý

```text
AI thực hiện workflow cverify-code-to-ai-audit:
- Xác nhận PR #84 đã tạo, assign reviewer LucFr1746, assignee TNL293107
- Apply labels: new feature, backend, frontend, audit
- Đọc audit packs #6, #7 để maintain formatting consistency
- Xác định next audit folder là #8
- Generate 4 files: AI_AUDIT_LOG.md, CHANGELOG.md, PROMPTS.md, REFLECTION.md
- Commit lên doc/Tran-Nhat-Long branch
- Tạo audit PR targeting main
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Complete audit documentation package #8 (4 files)
- Branch switching và git operations trên doc/Tran-Nhat-Long
- Audit PR creation với reviewers nhnnanh, LucFr1746
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Trigger workflow bằng lệnh /cverify-code-to-ai-audit
- Review audit content trước khi approve
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp — AI duy trì formatting nhất quán với #6 và #7, nội dung phản ánh
đúng implementation thực tế của Line 3 JD Matching Pipeline.
```
