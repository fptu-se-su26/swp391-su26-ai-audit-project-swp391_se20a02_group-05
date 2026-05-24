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
| Ngày bắt đầu | 2026-05-13T07:28:07.404Z |
| Ngày hoàn thành | 2026-05-13T07:28:07.408Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [x] Claude
- [x] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Research and Requirement analysis, Generate documents. Checking folders and files in repository automatically. Generate codes and auto testing
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | GitHub Copilot |
| Mục đích sử dụng | Checking and update feature for tool.; Thiết kế PostgreSQL database schema cho CVerify |
| Phần việc liên quan | Requirement |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
[Prompt 1: Checking and update feature for tool.]
Check ExportCenter.tsx in the folder tools to see if the "Download All" button is present. If not, add it and enable its functionality to download all four files (REFLECTION, CHANGELOG, PROMPS, AI_AUDIT_LOG) simultaneously.

---

[Prompt 2: Thiết kế PostgreSQL database schema cho CVerify]
Thiết kế PostgreSQL database schema cho CVerify — nền tảng tuyển dụng IT evidence-based. Các entity chính:
- Users (roles: JobSeeker, Recruiter, Admin)
- CV (sections: experience, education, skills, projects)
- Repository (GitHub link, analysis results: ownership_score, originality_score, commit_frequency)
- JobPosting (by Recruiter, có JD text)
- Application (JobSeeker apply vào JobPosting)
- SkillTag (kỹ năng extract từ CV + repo)
- TrustScore (per JobSeeker)

Yêu cầu kỹ thuật:
- PostgreSQL 16 với pgvector extension
- Cần lưu embedding vectors (1536 dimensions) cho CV và JobPosting để semantic search
- Soft delete (deleted_at)
- Audit fields (created_at, updated_at)
- Scale cho 10,000 users ban đầu

Trả về: SQL DDL + giải thích từng quyết định thiết kế + recommended indexes.
```

#### 4.2. Kết quả AI gợi ý

```text
[Prompt 1] Files modified: tools/AI Log/src/components/export/ExportCenter.tsx

---

[Prompt 2] ChatGPT trả về DDL đầy đủ cho 11 tables, foreign keys, indexes bao gồm HNSW index cho vector columns, và giải thích tại sao dùng JSONB cho một số fields. Gợi ý thêm partial index cho soft delete pattern.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
[Prompt 1] Existing per-file download helper (handleDownload) and markdown generators from @/lib/markdown/generators.

---

[Prompt 2] Cấu trúc cơ bản của 9/11 tables được giữ lại. Naming convention, data types, và HNSW index configuration được sử dụng trực tiếp.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
[Prompt 1] - Added: handleDownloadAll() to invoke downloads for:
AI_AUDIT_LOG.md, PROMPTS.md, CHANGELOG.md, REFLECTION.md
- Added: a header button labeled "Download all files" that calls handleDownloadAll().
- Preserved: existing individual file Download and Copy buttons.
- Ensured: Blob creation + URL handling is safe (created element, clicked, cleaned up).

---

[Prompt 2] - Thêm bảng AuditLog riêng cho compliance logging.
- Tách SkillTag table ra thêm CandidateSkill junction table.
- Chuyển một số JSONB thành typed columns (language_breakdown → LanguageBreakdown table riêng).
- Thêm CHECK constraint cho trust_score BETWEEN 0 AND 100.
- Chuyển toàn bộ từ DDL SQL sang EF Core Code-First migrations.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 12:49:54 | image.png |

#### 4.6. Nhận xét cá nhân/nhóm

```text
- Failure recovery paths (what if Claude is down?)
- User preference learning loop (how to infer user taste)
- Multi-traveler conflict resolution (Alice vs. Bob preferences)
- Offline mode + service worker sync
- A/B testing prompt variants safely
- Explainability (why did AI pick this restaurant?)
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-19 |
| Công cụ AI | GitHub Copilot |
| Mục đích sử dụng | Checking and update feature for tool.; Fix lỗi 401 Unauthorized khi gọi API |
| Phần việc liên quan | Requirement |
| Mức độ sử dụng | Hỗ trợ một phần |

#### 4.1. Prompt đã sử dụng

```text
[Prompt 1: Checking and update feature for tool.]
Check ExportCenter.tsx in the folder tools to see if the "Download All" button is present. If not, add it and enable its functionality to download all four files (REFLECTION, CHANGELOG, PROMPS, AI_AUDIT_LOG) simultaneously.

---

[Prompt 2: Fix lỗi 401 Unauthorized khi gọi API]
Tôi đang bị lỗi 401 Unauthorized khi gọi API từ Next.js 16 đến ASP.NET Core v10.

Setup:
- Frontend: Next.js 16, gọi fetch() với header Authorization: Bearer <token>
- Backend: ASP.NET Core v10 với AddJwtBearer()
- Token được lưu trong cookie httpOnly (Next.js)

Stack trace backend:
Microsoft.AspNetCore.Authentication.JwtBearer: Bearer was not authenticated. Failure message: No SecurityTokenValidator available for token.

Tôi đã config JwtBearerOptions với ValidateIssuer=true, ValidateAudience=true, ValidateIssuerSigningKey=true.

Next.js gửi request như sau (đã check Network tab):
Authorization: Bearer eyJhbGci...

Lỗi này do đâu?
```

#### 4.2. Kết quả AI gợi ý

```text
[Prompt 1] Files modified: tools/AI Log/src/components/export/ExportCenter.tsx

---

[Prompt 2] Claude phân tích 3 nguyên nhân có thể: (1) thiếu AddAuthentication() trước AddJwtBearer(), (2) token audience không match, (3) CORS preflight chặn Authorization header. Hướng dẫn kiểm tra từng case với code snippet cụ thể.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
[Prompt 1] Existing per-file download helper (handleDownload) and markdown generators from @/lib/markdown/generators.

---

[Prompt 2] Nguyên nhân là case (1): nhóm quên gọi app.UseAuthentication() trước app.UseAuthorization() trong middleware pipeline. Fix 1 dòng, bug resolved.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
[Prompt 1] - Added: handleDownloadAll() to invoke downloads for:
AI_AUDIT_LOG.md, PROMPTS.md, CHANGELOG.md, REFLECTION.md
- Added: a header button labeled "Download all files" that calls handleDownloadAll().
- Preserved: existing individual file Download and Copy buttons.
- Ensured: Blob creation + URL handling is safe (created element, clicked, cleaned up).

---

[Prompt 2] Sau khi fix bug, nhóm thêm integration test để verify authentication pipeline hoạt động đúng, tránh lỗi tương tự xảy ra lại sau khi refactor.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Generate documents |   |   |   | x |   |
| Researching and validate information |   | x |   |   |   |
| Checking, generate and testing codes |   | x |   |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | The resulting output does not match user needs 100%. | Check manually | The input prompt is more detailed and includes more context and rules. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Manual verification is combined with the use of another AI to validate the generated content.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Research how to integrate APIs into the system, compare criteria, and provide support during the initial planning phase of the project.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trần Nhất Long | DE200160 | 1. Problem statement và core concept của CVerify: Ý tưởng Proof-of-Work thay thế claim-based CV là ý tưởng gốc của bản thân.  2. Quyết định kiến trúc tổng quan: Chọn monolith modular thay vì microservices, chọn tech stack cụ thể (Next.js 16 + ASP.NET Core v10), từ chối các đề xuất phức tạp của AI.  3. Business logic và thuật toán: Thuật toán tính OwnershipScore dựa trên weighted lines changed, TrustScore decay model, logic semantic matching threshold.  4. Scope management: Quyết định tính năng nào implement, tính năng nào cắt bỏ để phù hợp timeline.  5. Integration và glue code: Code nối giữa các module (GitHub API → RepositoryAnalysis → pgvector → SemanticMatching) là tự viết. | Có | https://docs.google.com/document/d/1K6i3wU3Stycf3q3Jtsft35tII0XdPFchwYhq7Dr6J_s/edit?usp=sharing |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 24/5/2026 |
