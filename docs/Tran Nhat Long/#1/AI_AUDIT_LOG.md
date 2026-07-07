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
| Công cụ AI | Claude |
| Mục đích sử dụng | Tôi đang xây dựng một nền tảng tuyển dụng IT tên CVerify. Ý tưởng cốt lõi là thay thế CV truyền thống (claim-based) bằng bằng chứng thực tế từ GitHub/GitLab (Proof-of-Work). Hệ thống có 3 vai trò: Job Seeker, Recruiter, Admin. [Prompt user stories đầy đủ ở Prompt số 1] |
| Phần việc liên quan | Requirement |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
* Context: giờ tôi muốn tạo một web chấm điểm CV có tích hợp AI Agent đọc CV, chấm điểm, cải thiện CV rõ ràng theo từng chỗ, tạo skill tree. Sau đó hệ thông sẽ đánh label cho CV đó dựa theo các kĩ năng và kinh nghiệm. Ngoài ra, người tuyển dụng cũng có thể chủ động đăng job và lọc, tìm người phù hợp với job. Hệ thống được quyền public thông tin của recruiter và job seeker
* Yều cầu: phân tích, tìm hiểu và đề xuất giải pháp custom AI API cho hệ thống, minh họa framework và workflow người dùng(người tuyển dụng, người tìm việc), admin hệ thống bao gồm:
   * Lựa chọn API AI phù hợp với bài toán
   * Framework được chọn như sau:
   * Frontend (client): 
      * Framework: [Next.js 16 (App Router)](https://nextjs.org/)
      * UI Library: [HeroUI v3](https://heroui.com/) - Utilizing Compound Components and React Aria.
      * Styling: [Tailwind CSS v4](https://tailwindcss.com/) with OKLCH color variables.
      * State Management: [Zustand](https://github.com/pmndrs/zustand) for robust local state.
      * Forms: [React Hook Form](https://react-hook-form.com/) + [Zod](https://zod.dev/) for strict validation.
      * Icons: [Lucide React](https://lucide.dev/)
 Backend (server): 
      * [ASP.NET Core v10](https://dotnet.microsoft.com/en-us/apps/aspnet) [download](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
      * [PostgreSQL](https://www.postgresql.org/)
      * [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core)
      * [JWT](https://www.jwt.io/) [download](https://www.nuget.org/packages/JWT)
      * [BCrypt](https://github.com/BcryptNet/bcrypt.net) [download](https://www.nuget.org/packages/BCrypt.Net-Next)
   * Nghiên cứu cách hoạt động của AI API (request, prompt, response, token, streaming, function calling,…)
   * Thiết kế và custom cấu trúc response từ AI để phù hợp với nghiệp vụ hệ thống
   * Xây dựng cơ chế để front-end phân tích và xử lý response nhằm thực hiện các task cụ thể
   * Đề xuất workflow giao tiếp giữa Front-end ↔ Backend ↔ AI API
   * Nghiên cứu kĩ phần LLM.txt; MCP Server; Agent Skills cho phần custom AI, custom thêm yêu cầu dự án, bussiness rule
   * Thiết kế các AI task phù hợp để mở rộng và tích hợp vào hệ thống:
   * Đề xuất hướng triển khai thực tế, tối ưu chi phí API, performance và scalability
   * Phân chia hướng implement AI features vào kiến trúc hệ thống hiện tại để đảm bảo khả năng mở rộng về sau.
* Yêu cầu output: tạo file docx, đọc tài liệu, nghiên cứu thêm nguồn thông tin, xác thực sau đó mở rộng và nghiên cứu chuyên sâu hơn sau đó tổng hợp vào file docx, yêu cầu tạo graph để minh họa các mục như workflow và cách tương tác giữa back end front end với api
* Tài liệu đã nghiên cứu được đính kèm
* Thêm các mục nghiên cứu thủ công được để ở dưới đây:
1. Lựa chọn AI API & Model phù hợp
Với đặc thù cần phân tích mã nguồn (Repository Intelligence), hiểu ngữ nghĩa chuyên sâu (Semantic Hiring) và xử lý dữ liệu CV phức tạp, giải pháp tối ưu là kết hợp các mô hình:
   * Primary Model (Logic & Reasoning): GPT-4o hoặc Claude 3.5 Sonnet.
      * 
Lý do: Khả năng phân tích cấu trúc code, kiến trúc hệ thống và thực hiện Function Calling cực kỳ chính xác để trích xuất dữ liệu có cấu trúc (JSON).
   * Cost-Effective Model (Classification & Labeling): GPT-4o-mini hoặc Gemini 1.5 Flash.
      * 
Lý do: Phù hợp cho các task gắn nhãn (Labels & Chips) , lọc keyword cơ bản và chấm điểm nhanh để tối ưu chi phí API.
   * Embedding Model: text-embedding-3-small.
      * 
Lý do: Chuyển đổi CV và Job Description (JD) thành vector để thực hiện Semantic Matching , tìm kiếm ứng viên theo ngữ nghĩa thay vì chỉ keyword.
2. Thiết kế Cấu trúc Response & Cơ chế Xử lý (Function Calling)
Để hệ thống hoạt động ổn định, AI không nên trả về văn bản tự do (Prose) mà phải trả về Structured Data thông qua cơ chế Function Calling.
Cấu trúc Response đề xuất (JSON Schema):
JSON

```
{
  "assessment": {
    "overall_score": 85,
    "trust_score": 90,
    "technical_depth": "Senior",
    "labels": ["#GoHighConcurrency", "#VerifiedOwnership"]
  },
  "skill_tree": [
    { "skill": "Backend", "level": 4, "sub_skills": ["Kafka", "gRPC"], "evidence_link": "github.com/..." }
  ],
  "improvements": [
    { "section": "Experience", "issue": "Thiếu chỉ số impact", "suggestion": "Thêm metrics về performance optimization." }
  ],
  "agent_action": "TRIGGER_VERIFICATION_ENGINE"
}

```

Cơ chế xử lý tại Front-end (Next.js):
   * Streaming UI: Sử dụng `useChat` từ Vercel AI SDK để hiển thị quá trình AI đang "đọc" và "phân tích" từng phần của CV theo thời gian thực.
   * Client-side Parsing: Dùng Zod để validate lại JSON từ AI API trước khi render Skill Tree hoặc các Label nhằm tránh lỗi runtime khi AI trả về định dạng sai.
3. Workflow Giao tiếp Hệ thống
Giao tiếp giữa Front-end ↔ Backend ↔ AI API được thiết kế theo mô hình Asynchronous Agentic Workflow:
   1. Request: Người dùng upload CV/Link GitHub tại Front-end. Next.js gửi file/URL qua Backend (ASP.NET Core).
   2. Preprocessing: Backend lưu file vào Storage, lưu metadata vào PostgreSQL, sau đó gọi AI Agent.
   3. 
Agent Logic: AI API (sử dụng Function Calling) truy cập vào dữ liệu qua MCP Server (xem mục 4) để lấy context từ Repository.
   4. Response: AI trả về kết quả định dạng JSON. Backend xử lý logic Business Rule (ví dụ: chỉ cho phép Public thông tin nếu User đã xác nhận).
   5. Sync: Kết quả được đẩy về Front-end qua Server-Sent Events (SSE) hoặc SignalR để hiển thị trạng thái "Chấm điểm hoàn tất".
4. Nghiên cứu LLM.txt, MCP Server & Agent Skills
Đây là phần mở rộng để biến AI từ một "Chatbot" thành một "Worker" thực thụ:
   * LLM.txt: Cung cấp một tệp hướng dẫn chuẩn (Context Window) cho Agent. Tệp này sẽ chứa các Business Rules của Veritas như: “Luôn ưu tiên Evidence từ GitHub hơn là Self-declared”.
   * MCP Server (Model Context Protocol): Xây dựng các Server trung gian cho phép AI Agent:
      * Truy cập trực tiếp vào API của GitHub/GitLab để đọc code.
      * Truy vấn cơ sở dữ liệu PostgreSQL của hệ thống để so sánh ứng viên với các JD hiện có.
   * Agent Skills: Tùy chỉnh các bộ "kỹ năng" cho Agent:
      * 
Skill Phân tích Originality: Chuyên detect clone/tutorial repo.
      * 
Skill Chấm điểm Trust: Tính toán độ tin cậy dựa trên lịch sử commit.
5. Thiết kế AI Task & Workflow người dùng
Đối với Người tìm việc (Job Seeker)
   * 
Step 1: Kết nối GitHub/LinkedIn.
   * 
Step 2 (AI Task): AI thực hiện Repository Intelligence để phân tích ownership và kiến trúc.
   * 
Step 3: AI tự động tạo Profile chuẩn hóa (Verified Identity) thay vì bắt đầu bằng CV PDF.
Đối với Người tuyển dụng (Recruiter)
   * Step 1: Đăng JD.
   * 
Step 2 (AI Task): AI dùng Semantic Matching để tìm người phù hợp dựa trên năng lực thực tế thay vì keyword gaming.
   * 
Step 3: Hiển thị Trust Score và các nhãn VERIFIED để giảm rủi ro false positive.
Đối với Admin
   * 
AI Task: Giám sát các tài khoản bot, detect lận đận trong đánh giá công ty (Fraud Detection).
6. Triển khai thực tế & Tối ưu hóa
Tối ưu chi phí & Hiệu suất:
   * Semantic Caching: Sử dụng Redis để lưu kết quả các prompt phổ biến. Nếu hai ứng viên có CV tương tự, hệ thống sẽ dùng lại một phần kết quả phân tích cũ.
   * 
Batch Processing: Các task nặng như phân tích toàn bộ lịch sử GitHub (Source Code Analysis) nên được thực hiện dưới dạng Background Job trong ASP.NET Core (Sử dụng Hangfire).
   * 
Token Management: Chỉ gửi những đoạn code quan trọng nhất (File structure, PR description) cho LLM thay vì toàn bộ repo để tiết kiệm token.
Khả năng mở rộng:
   * 
Modular Agent: Tách các Agent thành các Microservices (Agent Phỏng vấn , Agent Phân tích PoW ). Khi lượng người dùng tăng, chỉ cần scale các Agent cần thiết.
   * Hybrid AI Architecture: Sử dụng mô hình local (như Llama 3) cho các task đơn giản như lọc spam, và dành GPT-4o cho các task phân tích chuyên sâu để tối ưu hóa scalability.
Hệ thống này sẽ chuyển đổi từ quy trình tuyển dụng dựa trên lời khai (Claim-based) sang dựa trên bằng chứng thực tế (Evidence-based).
```

#### 4.2. Kết quả AI gợi ý

```text
12 user stories với acceptance criteria cụ thể, phát hiện thêm 2 tính năng quan trọng (trust score + consent), và giúp nhóm có tài liệu để trình giảng viên review trong tuần đầu tiên.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Repository swp391-su26-ai-audit-project-swp391_se20a02_group-05
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm đọc từng user story và đối chiếu với problem statement gốc. Loại bỏ những story quá phức tạp hoặc không liên quan trực tiếp đến core value của CVerify. Sau đó trình giảng viên review và điều chỉnh theo feedback.Chia nhỏ 2 stories epic thành 6 stories nhỏ để sprint planning dễ hơn. Thêm business constraints cụ thể vào acceptance criteria mà AI không biết (giới hạn số repo, format file CV, v.v.).
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
| Mục đích sử dụng | Checking and update feature for tool. |
| Phần việc liên quan | Testing |
| Mức độ sử dụng | Hỗ trợ một phần |

#### 4.1. Prompt đã sử dụng

```text
Check ExportCenter.tsx in the folder tools to see if the "Download All" button is present. If not, add it and enable its functionality to download all four files (REFLECTION, CHANGELOG, PROMPS, AI_AUDIT_LOG) simultaneously.
```

#### 4.2. Kết quả AI gợi ý

```text
Files modified: tools/AI Log/src/components/export/ExportCenter.tsx
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Existing per-file download helper (handleDownload) and markdown generators from @/lib/markdown/generators.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Added: handleDownloadAll() to invoke downloads for:
AI_AUDIT_LOG.md, PROMPTS.md, CHANGELOG.md, REFLECTION.md
- Added: a header button labeled "Download all files" that calls handleDownloadAll().
- Preserved: existing individual file Download and Copy buttons.
- Ensured: Blob creation + URL handling is safe (created element, clicked, cleaned up).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

### Lần sử dụng AI số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-28 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Thiết kế PostgreSQL database schema cho CVerify |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
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
ChatGPT trả về DDL đầy đủ cho 11 tables, foreign keys, indexes bao gồm HNSW index cho vector columns, và giải thích tại sao dùng JSONB cho một số fields. Gợi ý thêm partial index cho soft delete pattern.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Cấu trúc cơ bản của 9/11 tables được giữ lại. Naming convention, data types, và HNSW index configuration được sử dụng trực tiếp.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Thêm bảng AuditLog riêng cho compliance logging.
- Tách SkillTag table ra thêm CandidateSkill junction table.
- Chuyển một số JSONB thành typed columns (language_breakdown → LanguageBreakdown table riêng).
- Thêm CHECK constraint cho trust_score BETWEEN 0 AND 100.
- Chuyển toàn bộ từ DDL SQL sang EF Core Code-First migrations.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

### Lần sử dụng AI số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-28 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Fix lỗi 401 Unauthorized khi gọi API |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
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
Claude phân tích 3 nguyên nhân có thể: (1) thiếu AddAuthentication() trước AddJwtBearer(), (2) token audience không match, (3) CORS preflight chặn Authorization header. Hướng dẫn kiểm tra từng case với code snippet cụ thể.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nguyên nhân là case (1): nhóm quên gọi app.UseAuthentication() trước app.UseAuthorization() trong middleware pipeline. Fix 1 dòng, bug resolved.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Sau khi fix bug, nhóm thêm integration test để verify authentication pipeline hoạt động đúng, tránh lỗi tương tự xảy ra lại sau khi refactor.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

### Lần sử dụng AI số 5

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-28 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Used Claude Code (claude-sonnet-4-6) to generate the full test suite. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Implements the full multi-layer testing strategy for CVerify as defined in the
project testing specification. This PR introduces all test infrastructure and
test suites across three levels:

**Frontend (client/)**
- Jest + React Testing Library (white-box): unit tests for AuthContent component
  (logo/theme rendering, SSR hydration safety), password policy evaluator
  (boundary/branch coverage), workspace slug validator, and API error normalizer
- Mocha + Chai (black-box): behavioral tests for password policy, workspace slug
  validation, and error normalization — testing public contracts without accessing
  internals
- Cypress (black-box E2E): auth gateway page routing and login UI, company
  verification 3-step wizard (tax code format, company name validation, step
  navigation, already-registered recovery flow, owner identity step)
- Test infrastructure: jest.config.mjs, jest.setup.ts, .mocharc.json,
  cypress.config.ts, HeroUI component mock, TESTING.md guide

**Backend (CVerify.Core/)**
- xUnit + FluentAssertions (white-box): PasswordPolicyServiceTests covering
  Default and Enterprise policies (empty, whitespace, boundary lengths, invalid
  variants, unknown policy fallback); OtpPolicyServiceTests covering valid/invalid
  OTP formats and policy violation exceptions

**Source refactors required by testability**
- Extracted workspace slug logic into a dedicated
  `src/features/auth/security/workspace-slug.ts` module (was inline in the view)
- Minor auth-content.tsx cleanup to support RTL rendering in JSDOM
- company-verification-view.tsx imports updated to use the new slug module
```

#### 4.2. Kết quả AI gợi ý

```text
Test results: Jest 44 passing · Mocha 72 passing · xUnit 71 passing
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
 
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
 
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
| Testing |   |   | x |   |   |

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
| Nguyễn Hoàng Ngọc Ánh | 28/5/2026 |
