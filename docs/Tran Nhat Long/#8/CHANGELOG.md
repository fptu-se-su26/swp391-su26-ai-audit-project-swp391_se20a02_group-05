# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

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
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-06-17 |
| Ngày hoàn thành | 2026-06-17 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 08 — feat: Line 3 JD Matching Pipeline (L3-005 to L3-015) | 2026-06-17 | Implement toàn bộ 3B/3C/3D của Line 3: Skill/Responsibility/Salary/Culture Fit matching, Match Aggregation, Gap Analysis, Quality Gate, Hiring Recommendation | Completed |

---

# [Phase 08] feat: Line 3 JD Matching Pipeline (L3-005 to L3-015)

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-17
- **Mô tả giai đoạn:** Implement hoàn chỉnh Line 3 — JD Matching Pipeline. Phase này bổ sung L3-005 đến L3-015 (subtasks 3B, 3C, 3D) sau khi L3-001 đến L3-004 (3A. Standardized JD System) đã được implement trong commit cca11ef. Tổng cộng 29 files thay đổi với 8652 insertions.
- **Trạng thái hiện tại:** Completed
- **Commit:** f15e5a7 (feature-business-JD)
- **PR:** #84 → CVerify-uat

## Lịch sử commit liên quan

| Commit | Mô tả | Ngày |
|---|---|---|
| cca11ef | feat: implement 3A Standardized JD System (L3-001 to L3-004) | 2026-06-16 |
| f15e5a7 | feat: implement Line 3 JD Matching Pipeline (L3-005 to L3-015) | 2026-06-17 |

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Implement `JdMatchingService.cs` (371 dòng): SkillAliases, SeniorityLevels, CalculateMatch(), MatchSkills(), MatchResponsibilities(), CalculateSeniorityScore(), CalculateSalaryScore(), CalculateCultureFitScore(), AggregateScore(), ApplyCapRules(), GenerateGapAnalysis(), GenerateHiringRecommendation() | Trần Nhất Long | `CVerify.Core/Modules/Jd/Services/JdMatchingService.cs` | Commit f15e5a7 |
| 2 | Thêm interface `IJdMatchingService.cs` với method `CalculateMatch(JdMatchRequest)` | Trần Nhất Long | `CVerify.Core/Modules/Jd/Services/IJdMatchingService.cs` | Commit f15e5a7 |
| 3 | Tạo entity `StandardizedJd.cs` với các fields: Id, Title, Seniority, RequiredSkills, PreferredSkills, Responsibilities, SalaryMin, SalaryMax, WorkingModel, GeneratedText, CreatedAt | Trần Nhất Long | `CVerify.Core/Modules/Jd/Entities/StandardizedJd.cs` | Commit f15e5a7 |
| 4 | Tạo EF Core migration `AddLine3JdMatching`: thêm bảng StandardizedJds và columns DesiredSalary, MinAcceptableSalary vào CareerPreferences | Trần Nhất Long | `CVerify.Core/Migrations/20260616183936_AddLine3JdMatching.cs` | Commit f15e5a7 |
| 5 | Cập nhật `ApplicationDbContext.cs`: thêm `DbSet<StandardizedJd> StandardizedJds` | Trần Nhất Long | `CVerify.Core/Modules/Shared/Persistence/ApplicationDbContext.cs` | Commit f15e5a7 |
| 6 | Cập nhật `ApplicationDbContextModelSnapshot.cs`: sync với migration mới | Trần Nhất Long | `CVerify.Core/Migrations/ApplicationDbContextModelSnapshot.cs` | Commit f15e5a7 |
| 7 | Cập nhật `CareerPreference.cs`: thêm `DesiredSalary` và `MinAcceptableSalary` properties | Trần Nhất Long | `CVerify.Core/Modules/Profiles/Entities/CareerPreference.cs` | Commit f15e5a7 |
| 8 | Cập nhật `ProfileSettingsDtos.cs`: thêm salary fields vào DTO | Trần Nhất Long | `CVerify.Core/Modules/Profiles/DTOs/ProfileSettingsDtos.cs` | Commit f15e5a7 |
| 9 | Cập nhật `CareerService.cs`: xử lý DesiredSalary và MinAcceptableSalary trong update flow | Trần Nhất Long | `CVerify.Core/Modules/Profiles/Services/CareerService.cs` | Commit f15e5a7 |
| 10 | Cập nhật `JdDtos.cs`: thêm JdMatchRequest, MatchScoreResponse, MatchBreakdown, GapAnalysisResult, HiringRecommendation DTOs | Trần Nhất Long | `CVerify.Core/Modules/Jd/DTOs/JdDtos.cs` | Commit f15e5a7 |
| 11 | Cập nhật `JdController.cs`: thêm POST `/api/jd/match` endpoint, inject IJdMatchingService | Trần Nhất Long | `CVerify.Core/Modules/Jd/Controllers/JdController.cs` | Commit f15e5a7 |
| 12 | Cập nhật `IJdService.cs` và `JdService.cs`: mở rộng service với matching operations | Trần Nhất Long | `CVerify.Core/Modules/Jd/Services/IJdService.cs`, `JdService.cs` | Commit f15e5a7 |
| 13 | Cập nhật `Program.cs`: đăng ký `IJdMatchingService` → `JdMatchingService` trong DI container | Trần Nhất Long | `CVerify.Core/Program.cs` | Commit f15e5a7 |
| 14 | Cập nhật `DbInitializer.cs`: thêm seed data cho salary fields và StandardizedJd examples | Trần Nhất Long | `CVerify.Core/Modules/Shared/Persistence/DbInitializer.cs` | Commit f15e5a7 |
| 15 | Mở rộng AI orchestrator lên 517 dòng: thêm 11 task handlers mới cho L3-005 đến L3-015 | Trần Nhất Long | `CVerify.AI/app/pipelines/jd/orchestrator.py` | Commit f15e5a7 |
| 16 | Tạo `application-quality-gate.tsx`: React component hiển thị warnings và require explicit confirm trước khi apply | Trần Nhất Long | `client/src/modules/business/components/application-quality-gate.tsx` | Commit f15e5a7 |
| 17 | Cập nhật `jd.types.ts`: thêm MatchScoreResponse, MatchBreakdown, GapAnalysis, HiringRecommendation TypeScript types | Trần Nhất Long | `client/src/modules/business/types/jd.types.ts` | Commit f15e5a7 |
| 18 | Cập nhật `jd.service.ts`: thêm `matchCandidate()` API call | Trần Nhất Long | `client/src/modules/business/services/jd.service.ts` | Commit f15e5a7 |
| 19 | Cập nhật `CareerTab.tsx`: thêm Desired Salary và Minimum Acceptable Salary fields trong Settings UI | Trần Nhất Long | `client/src/app/(private)/settings/components/CareerTab.tsx` | Commit f15e5a7 |
| 20 | Cập nhật `profile.types.ts`, `cv/components/types.ts`, `cv/page.tsx`: integrate salary fields và ApplicationQualityGate component | Trần Nhất Long | `client/src/types/profile.types.ts`, `client/src/app/(private)/cv/` | Commit f15e5a7 |
| 21 | Tạo `JdMatchingServiceTests.cs`: unit tests cho JdMatchingService | Trần Nhất Long | `CVerify.Core/tests/CVerify.API.UnitTests/Jd/JdMatchingServiceTests.cs` | Commit f15e5a7 |
| 22 | Tạo `JdMatchingApiTests.cs`: integration tests cho /api/jd/match endpoint | Trần Nhất Long | `CVerify.Core/tests/CVerify.API.IntegrationTests/Jd/JdMatchingApiTests.cs` | Commit f15e5a7 |
| 23 | Tạo `test_line3_pipeline.py`: tests cho AI orchestrator task handlers L3-005 đến L3-015 | Trần Nhất Long | `CVerify.AI/tests/test_line3_pipeline.py` | Commit f15e5a7 |
| 24 | Fix `ScribanTemplateLoader.cs`: minor bug fix trong email template loader | Trần Nhất Long | `CVerify.Core/Modules/Shared/Email/Services/ScribanTemplateLoader.cs` | Commit f15e5a7 |
| 25 | Cập nhật `CVerify.API.csproj`: thêm package references cần thiết | Trần Nhất Long | `CVerify.Core/CVerify.API.csproj` | Commit f15e5a7 |

## AI có hỗ trợ không?

| | Có | Không |
|---|---|---|
| AI hỗ trợ việc này | ✅ | |

**Chi tiết:** Claude (Claude Code) implement toàn bộ JdMatchingService, AI orchestrator handlers, frontend components, DB migration và test suite. Developer review và confirm implementation trước khi commit.

## Lỗi / vấn đề phát sinh

| STT | Lỗi / Vấn đề | Nguyên nhân | Cách xử lý |
|---:|---|---|---|
| 1 | Labels `feature`, `backend` không tồn tại trên repo | Repo chỉ có limited label set | Dùng đúng labels hiện có: `new feature`, `backend`, `frontend`, `audit` |
| 2 | AI_AUDIT_LOG.md và CHANGELOG.md không được tạo ở lần Write đầu tiên | File Write tool path conflict khi switching branch | Tạo lại bằng Write tool lần 2 thành công |

## Kết quả đạt được

```text
- Toàn bộ 15 tasks Line 3 (L3-001 đến L3-015) đã completed
- JD Matching engine với weighted formula: Skill×0.35 + Resp×0.25 + Sen×0.20 + Sal×0.10 + Cult×0.10
- Hard cap rules: salary=0 → cap ≤ 60%; skill<0.4 → flag "Insufficient Skills"
- Gap Analysis với missing skills list và improvement suggestions
- Application Quality Gate UI với explicit confirmation flow
- Hiring Recommendation (Yes/Conditional/No) với natural language explanation
- PR #84 created targeting CVerify-uat
- 29 files changed, 8652 insertions
```
