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
| Ngày bắt đầu | 2026-07-07 |
| Ngày hoàn thành | 2026-07-07 |

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
Audit toàn bộ hạ tầng triển khai production của CVerify (VPS cverify.io.vn, Docker
Compose, Nginx, CI/CD) dựa trên tài liệu tham khảo đã có sẵn (CVerify_Deployment_Guide_v1.docx)
và trạng thái thực tế của codebase + file .env/docker-compose.yml đã upload. Từ đó:

1. Đối chiếu tài liệu tham khảo với codebase thật, chỉ ra điểm sai lệch.
2. Audit toàn bộ 13 hạng mục triển khai (repo audit, config validation, environment
   validation, architecture, docker-compose.prod.yml, nginx, GitHub Actions, scripts
   vận hành, backup, monitoring, security review, deployment checklist) và sinh ra
   bộ tài liệu deployment/ đầy đủ, không được bịa giá trị.
3. Áp dụng 1 code fix thật đã được audit xác nhận (ForwardedHeaders middleware) để
   sửa lỗi rate-limit bị vô hiệu hoá khi có Nginx reverse proxy.
4. Commit code fix + các artifact triển khai (không gồm secret, .env, file checklist,
   file .md) lên GitHub.
```

---

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-07 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Đọc tài liệu deployment guide (.docx) đã nghiên cứu trước đó và đối chiếu với codebase CVerify thực tế để đề xuất hướng deploy |
| Phần việc liên quan | DevOps / Infrastructure / Research |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
@"C:\Users\long2\OneDrive\Documents\Claude\Projects\SWP391\CVerify_Deployment_Guide_v1.docx"
tìm hiểu codebase của CVerify và đề xuất hướng deploy phù hợp, có thể tham khảo hướng
mẫu trong file docx đã nghiên cứu từ trước
```

#### 4.2. Kết quả AI gợi ý

```text
AI trích xuất nội dung .docx (unpack zip, đọc word/document.xml trực tiếp vì môi trường
không có pandoc), xác nhận tài liệu đề xuất hướng VPS + Docker Compose + Nginx +
GitHub Actions SSH deploy cho 5 service (Next.js, ASP.NET Core, FastAPI, Postgres+pgvector,
Redis). Song song, AI dùng agent Explore khảo sát codebase thật và phát hiện nhiều điểm
lệch so với tài liệu:
  - Docker network đã phân đoạn 3 lớp (frontend/backend/infra), không chỉ 1 mạng như docx
  - Có SignalR hub (/hubs/admin, /hubs/notifications) — docx không đề cập
  - Có Cloudflare R2, SendGrid, 3 OAuth provider (Google/GitHub/GitLab) — docx không có
  - Chưa có CI/CD nào tồn tại trong repo (docx ngụ ý đã có sẵn)
  - 13+ background worker chạy in-process trong container Core — không dùng Hangfire
    như docx mô tả
AI tổng hợp bảng đối chiếu tài liệu ↔ thực tế và đề xuất giữ hướng VPS+Docker nhưng
điều chỉnh Nginx/compose theo đúng cấu trúc thật của repo.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Bảng đối chiếu tài liệu tham khảo vs. codebase thực tế
- Đề xuất cấu hình Nginx xử lý SSE + WebSocket (SignalR) mà tài liệu gốc chưa có
- Danh sách 3 việc cần xử lý trước khi deploy (retry logic Anthropic, dọn dẹp
  git-clone workspace, đảm bảo đủ biến môi trường)
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Cung cấp file .docx tham khảo đã chuẩn bị từ trước
- Yêu cầu AI xác minh lại bằng cách đọc codebase thật thay vì tin hoàn toàn vào tài liệu
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp cao — AI không chỉ lặp lại tài liệu tham khảo mà chủ động verify từng điểm
bằng cách đọc codebase, phát hiện đúng các sai lệch quan trọng (SignalR, R2, CI/CD
chưa tồn tại) mà nếu chỉ dựa vào tài liệu gốc sẽ bị bỏ sót.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-07 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Audit toàn diện hạ tầng deploy production (13 task: repo audit, config validation, env validation, architecture, docker-compose.prod.yml, nginx, CI/CD, scripts, backup, monitoring, security review, checklist) dựa trên `.env`/`docker-compose.yml` thật đã upload cho VPS `cverify.io.vn` (2 vCPU/4GB) |
| Phần việc liên quan | DevOps / Infrastructure / Security |
| Mức độ sử dụng | AI hỗ trợ toàn bộ |

#### 4.1. Prompt đã sử dụng

```text
# ROLE
You are a Senior DevOps Engineer, Cloud Infrastructure Engineer, and Software Architect.
Your task is to complete the production deployment infrastructure for the CVerify project.
[... đầy đủ 13 TASK: Repository & Deployment Audit, Validate Uploaded Configuration,
Complete Production Deployment, Generate docker-compose.prod.yml, Validate Production
Environment Variables, Generate Production Nginx, Generate GitHub Actions, Deployment
Directory Structure, Generate Operational Scripts, Backup Strategy, Monitoring, Security
Review, Deployment Checklist ...]
Never hallucinate. Never guess. Always use the actual repository and uploaded files as
the source of truth. If any information is missing, explicitly report it instead of
inventing values.
[kèm theo nội dung .env và docker-compose.yml thật cho production tại cverify.io.vn]
```

#### 4.2. Kết quả AI gợi ý

```text
AI đọc trực tiếp Program.cs, EnvValidator.cs, appsettings.json, CVerify.AI/app/core/config.py,
.env và docker-compose.yml thật, phát hiện 3 lỗi CRITICAL sẽ khiến production không chạy
được nếu deploy nguyên trạng:
  1. SEED_TEST_ACCOUNTS=true + ASPNETCORE_ENVIRONMENT=Production → Program.cs:104-107
     throw exception → container Core crash-loop ngay khi start
  2. AI service đọc biến SHARED_SECRET/CLIENT_ID (config.py) nhưng .env chỉ có
     AI_SERVICE_SHARED_SECRET → xác thực HMAC giữa Core↔AI luôn thất bại
  3. NEXT_PUBLIC_API_URL bị bake sai lúc build (docker-compose build arg trỏ
     localhost:5247 thay vì domain thật) — Next.js inline biến này lúc build time
Ngoài ra phát hiện: REDIS_PASSWORD bị tham chiếu nhưng không định nghĩa (Redis chạy
không mật khẩu), EmailSettings bị ép SendGrid với API key giả trong khi Gmail SMTP
thật đã có sẵn, resource limits (4.5 vCPU/6.1GB) vượt quá VPS thật (2 vCPU/4GB), và
nhiều secret yếu (DB_PASSWORD=123123...).

AI tạo thư mục deployment/ gồm 18 file: REPOSITORY_AUDIT.md, CONFIG_VALIDATION.md,
ENVIRONMENT_VALIDATION.md, ARCHITECTURE.md, SECURITY_REVIEW.md, DEPLOYMENT_CHECKLIST.md,
DEPLOYMENT_GUIDE.md, CODE_CHANGES_REQUIRED.md, docker-compose.prod.yml, .env.example,
nginx/cverify.conf, github/deploy.yml, và 6 script (deploy.sh, backup-db.sh,
restore-db.sh, cleanup-workspaces.sh, renew-ssl.sh, health-check.sh).

Đặc biệt, AI KHÔNG tự sửa lỗi ForwardedHeaders (rate-limit bị vô hiệu hoá sau Nginx)
ngay lập tức mà tạo CODE_CHANGES_REQUIRED.md để chờ duyệt, vì cần subnet Docker thật
từ VPS (docker network inspect) mà AI không truy cập được — tránh áp dụng cấu hình
"trust mọi proxy" gây lỗ hổng giả mạo IP.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ 18 file trong deployment/ (audit, validation, architecture, compose override,
  nginx config, CI/CD pipeline, 6 script vận hành)
- Bảng tổng hợp mức độ nghiêm trọng (CRITICAL/HIGH/MEDIUM/LOW) cho từng lỗi phát hiện
- Khuyến nghị rotate toàn bộ secret vì đã đi qua phiên chat/generation
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Cung cấp .env và docker-compose.yml thật của VPS production để AI đối chiếu
- Xác nhận thông tin VPS thật (2 vCPU/4GB, domain cverify.io.vn, trạng thái từng service)
- Review từng finding trước khi quyết định áp dụng fix nào
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp cao — AI phát hiện đúng 3 lỗi CRITICAL bằng cách đọc chéo code thật (Program.cs,
config.py, EnvValidator.cs) thay vì suy đoán, và biết dừng lại đúng lúc (không tự ý sửa
code khi thiếu thông tin từ VPS thật) thay vì bịa giá trị.
```

---

### Lần sử dụng AI số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-07 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Đối chiếu thêm 1 file `.env` hợp nhất (bản dev) mới được cung cấp, cập nhật `deployment/.env.example` thành template 2 profile (dev/production) |
| Phần việc liên quan | DevOps / Configuration |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
@"D:\downloads\.env"
Update new .env for Docker
```

#### 4.2. Kết quả AI gợi ý

```text
AI nhận diện file .env mới là bản Development (ASPNETCORE_ENVIRONMENT=Development,
DISABLE_RATE_LIMITS=true, DB_PORT=5433, host=localhost) khác với bản Production đã
audit trước đó. Vì mục tiêu người dùng chưa rõ (dev, prod, hay cả hai), AI đặt câu hỏi
làm rõ trước khi sửa thay vì đoán. Sau khi người dùng chọn "Both — reconcile into one
template", AI cập nhật deployment/.env.example thành 1 file duy nhất có comment [DEV]/[PROD]
rõ ràng trên từng dòng khác biệt, đồng thời cập nhật ENVIRONMENT_VALIDATION.md với 1 addendum
mới: xác nhận REDIS_PASSWORD và AI_SERVICE_CLIENT_ID nay đã có (đóng 2 gap cũ), phát hiện
thêm biến AI_HOST/AI_DEBUG_MODE được khai báo nhưng không được code nào đọc.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- deployment/.env.example dạng dual-profile (dev/production) với comment giải thích
- Addendum trong ENVIRONMENT_VALIDATION.md ghi nhận thay đổi
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Cung cấp file .env mới
- Trả lời câu hỏi làm rõ mục tiêu (dev/prod/cả hai) khi AI hỏi thay vì để AI tự đoán
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp — AI không tự đoán ngữ cảnh khi thông tin mơ hồ (dùng AskUserQuestion), và khi
có câu trả lời thì thực hiện đúng yêu cầu, không copy secret thật vào file mẫu.
```

---

### Lần sử dụng AI số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-07 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Áp dụng fix `ForwardedHeaders` đã đề xuất trong `CODE_CHANGES_REQUIRED.md` vào `Program.cs`, sau đó commit + push chọn lọc file lên GitHub |
| Phần việc liên quan | Backend / Security / Git |
| Mức độ sử dụng | AI hỗ trợ toàn bộ |

#### 4.1. Prompt đã sử dụng

```text
áp dụng fix ForwardedHeaders vào Program.cs luôn đi, sau đó commit lên github trừ các
secretkey, api key, file .env, các file checklist và các file .md
```

#### 4.2. Kết quả AI gợi ý

```text
AI thêm using Microsoft.AspNetCore.HttpOverrides; và block app.UseForwardedHeaders(...)
ngay sau builder.Build(), giới hạn KnownIPNetworks vào dải mạng bridge mặc định của
Docker (172.16.0.0/12) + loopback thay vì mở hoàn toàn (tránh giả mạo X-Forwarded-For).
Build lần 1 dùng API IPNetwork/KnownNetworks (Microsoft.AspNetCore.HttpOverrides) →
cảnh báo obsolete (ASPDEPR005) vì .NET 10 khuyến nghị System.Net.IPNetwork/KnownIPNetworks
→ AI tự sửa lại và build lại lần 2, xác nhận 0 error, 0 warning liên quan đến
ForwardedHeaders. Sau đó AI kiểm tra git status, xác nhận .env không bị track (đã có
trong .gitignore), stage chọn lọc CVerify.Core/Program.cs + 9 file trong deployment/
(docker-compose.prod.yml, nginx/cverify.conf, github/deploy.yml, 6 script) — loại trừ
toàn bộ 8 file .md và .env.example — commit và push thẳng lên nhánh CVerify-uat
(không tạo branch/PR riêng vì không được yêu cầu ở bước này).
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Code fix ForwardedHeaders trong CVerify.Core/Program.cs
- Lệnh git add chọn lọc từng file cụ thể (không dùng git add -A)
- Commit message mô tả đúng nội dung thay đổi
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Yêu cầu rõ ràng những gì cần loại trừ khi commit (secret, .env, checklist, .md)
- Xác nhận build thành công trước khi đồng ý commit
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp cao — AI build thử và tự sửa cảnh báo obsolete trước khi commit thay vì
commit code có warning, đồng thời tuân thủ chính xác danh sách loại trừ file mà
người dùng yêu cầu (kiểm tra lại bằng git status trước khi push).
```

---

### Lần sử dụng AI số 5

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-07 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Tạo audit documentation package #9 cho phiên làm việc deployment audit + ForwardedHeaders fix |
| Phần việc liên quan | Documentation / Workflow |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
chuyển sang nhánh doc/Tran-Nhat-long, tạo audit log mới và tạo PR merge vào main
```

#### 4.2. Kết quả AI gợi ý

```text
AI đọc SKILL.md tại ~/.claude/skills/ecc/cverify-code-to-ai-audit/SKILL.md, xác định
yêu cầu chỉ nhắm vào audit track (Step 9-15), vì source code đã được commit/push trực
tiếp lên CVerify-uat ở bước trước (không qua PR riêng theo yêu cầu người dùng lúc đó).
AI fetch + checkout doc/Tran-Nhat-Long (branch thật khớp với mapping GitHub username
TNL293107), xác nhận thư mục thật là "docs/Tran Nhat Long" (có khoảng trắng, không phải
"Tran-Nhat-Long" như ví dụ trong SKILL.md), đọc audit pack #8 để giữ đúng format/style,
xác định folder kế tiếp là #9, generate 4 file audit, commit lên doc/Tran-Nhat-Long,
tạo PR audit targeting main với reviewers nhnnanh, LucFr1746.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Audit documentation package #9 (4 file)
- Xác nhận đúng tên thư mục thật trên branch doc/Tran-Nhat-Long thay vì dùng tên mẫu
  trong SKILL.md
- Audit PR creation với reviewers nhnnanh, LucFr1746
```

#### 4.4. Phần sinh viên/nhóm đã tự làm / kiểm tra

```text
- Trigger workflow bằng lệnh /cverify-code-to-ai-audit với arguments cụ thể
- Review nội dung audit trước khi approve merge
```

#### 4.5. Đánh giá mức độ phù hợp của kết quả AI

```text
Phù hợp — AI phát hiện đúng tên thư mục thật (có khoảng trắng) thay vì làm theo mẫu
generic trong SKILL.md, giữ format nhất quán với #6/#7/#8.
```
