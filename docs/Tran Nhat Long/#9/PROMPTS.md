# Prompts

## 1. Thông tin chung

File này ghi lại các prompt thực tế đã sử dụng với AI trong phiên làm việc này.

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Tên project | CVerify |
| Tên sinh viên | Trần Nhất Long |
| MSSV | DE200160 |
| Ngày | 2026-07-07 |
| Phiên số | #9 |

---

## 2. Danh sách prompt đã sử dụng

### Prompt 1 — Nghiên cứu hướng deploy dựa trên tài liệu tham khảo

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
@"C:\Users\long2\OneDrive\Documents\Claude\Projects\SWP391\CVerify_Deployment_Guide_v1.docx"
tìm hiểu codebase của CVerify và đề xuất hướng deploy phù hợp, có thể tham khảo hướng
mẫu trong file docx đã nghiên cứu từ trước
```

**Context khi dùng prompt:**
```text
- Đã có sẵn tài liệu deployment guide (.docx) nghiên cứu từ trước, đề xuất VPS +
  Docker Compose + Nginx + GitHub Actions
- Chưa có audit nào đối chiếu tài liệu này với codebase thực tế
```

**Kết quả mong đợi:**
```text
- Đọc tài liệu, khảo sát codebase, đề xuất hướng deploy có căn cứ thực tế
```

**Kết quả thực tế:**
```text
- Trích xuất text từ .docx bằng cách unzip + đọc word/document.xml (môi trường không
  có pandoc)
- Dùng Explore agent khảo sát codebase song song
- Tạo bảng đối chiếu tài liệu ↔ thực tế: network segmentation 3 lớp, SignalR hub,
  Cloudflare R2, SendGrid, 3 OAuth provider, chưa có CI/CD, 13+ background worker
  in-process (không dùng Hangfire)
- Đề xuất giữ hướng VPS+Docker nhưng bổ sung Nginx config cho WebSocket/SSE
```

---

### Prompt 2 — Audit toàn diện hạ tầng deploy production

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
# ROLE
You are a Senior DevOps Engineer, Cloud Infrastructure Engineer, and Software Architect.
Your task is to complete the production deployment infrastructure for the CVerify project.
The project already has a working codebase and a partially deployed production environment.
Your job is to audit the existing deployment, validate the uploaded production
configuration, identify gaps, and generate only the missing or improved deployment
artifacts. Never hallucinate. Never guess. Always use the actual repository and
uploaded files as the source of truth. If any information is missing, explicitly
report it instead of inventing values.

[... CURRENT PROJECT STATUS: VPS Ubuntu 22.04, 2 vCPU/4GB/80GB NVMe, domain
cverify.io.vn, frontend deployed, backend/AI/Postgres/Redis/CI-CD/Nginx chưa xong ...]

[... TASK 1-13: Repository & Deployment Audit, Validate Uploaded Configuration,
Complete Production Deployment, Generate docker-compose.prod.yml, Validate Production
Environment Variables, Generate Production Nginx, Generate GitHub Actions, Deployment
Directory Structure, Generate Operational Scripts, Backup Strategy, Monitoring,
Security Review, Deployment Checklist ...]

[kèm nội dung thật của .env và docker-compose.yml production dán trực tiếp trong prompt]
```

**Context khi dùng prompt:**
```text
- Prompt số 1 đã xác định hướng deploy tổng quát
- Người dùng cung cấp trạng thái VPS thật + .env/docker-compose.yml thật để AI audit
  chính xác thay vì suy đoán
```

**Kết quả mong đợi:**
```text
- Audit đầy đủ 13 hạng mục, sinh file trong thư mục deployment/
- Không bịa giá trị, báo cáo rõ khi thiếu thông tin
```

**Kết quả thực tế:**
```text
- Đọc trực tiếp Program.cs, EnvValidator.cs, appsettings.json, CVerify.AI/app/core/config.py
- Phát hiện 3 lỗi CRITICAL: seed test account crash, HMAC secret mismatch, frontend
  build-time API URL sai
- Phát hiện thêm: REDIS_PASSWORD thiếu, SendGrid bị ép dùng với key giả, resource limits
  vượt VPS thật, nhiều secret yếu
- Tạo 18 file trong deployment/: 8 file .md (audit/validation/architecture/security/
  checklist/guide/code-changes), docker-compose.prod.yml, .env.example, nginx/cverify.conf,
  github/deploy.yml, 6 script vận hành
- Không tự sửa lỗi ForwardedHeaders ngay — tạo CODE_CHANGES_REQUIRED.md chờ duyệt vì
  thiếu subnet Docker thật từ VPS
```

---

### Prompt 3 — Đối chiếu file .env dev mới

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
@"D:\downloads\.env"
Update new .env for Docker
```

**Context khi dùng prompt:**
```text
- Đã có deployment/.env.example từ Prompt 2 (bản production)
- Người dùng cung cấp thêm 1 file .env hợp nhất khác, có vẻ là bản dev
  (ASPNETCORE_ENVIRONMENT=Development, DISABLE_RATE_LIMITS=true, DB_PORT=5433)
```

**Kết quả mong đợi:**
```text
- Cập nhật .env.example cho phù hợp với Docker
```

**Kết quả thực tế:**
```text
- AI nhận thấy mục tiêu chưa rõ (dev/prod/cả hai) → dùng AskUserQuestion hỏi lại
  thay vì đoán
- Người dùng chọn "Both — reconcile into one template"
- AI viết lại deployment/.env.example thành 1 file dual-profile [DEV]/[PROD], không
  copy secret thật, cập nhật ENVIRONMENT_VALIDATION.md với addendum ghi nhận
  REDIS_PASSWORD/AI_SERVICE_CLIENT_ID đã có, phát hiện AI_HOST/AI_DEBUG_MODE không
  được code đọc
```

---

### Prompt 4 — Áp dụng fix ForwardedHeaders và commit chọn lọc

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
áp dụng fix ForwardedHeaders vào Program.cs luôn đi, sau đó commit lên github trừ các
secretkey, api key, file .env, các file checklist và các file .md
```

**Context khi dùng prompt:**
```text
- CODE_CHANGES_REQUIRED.md từ Prompt 2 đã đề xuất fix ForwardedHeaders nhưng chưa áp dụng
- Người dùng quyết định áp dụng ngay và chỉ định rõ phạm vi file cần loại trừ khi commit
```

**Kết quả mong đợi:**
```text
- Sửa Program.cs, build kiểm tra, commit + push chỉ những file được phép
```

**Kết quả thực tế:**
```text
- Thêm using Microsoft.AspNetCore.HttpOverrides; và app.UseForwardedHeaders(...) sau
  builder.Build(), giới hạn KnownNetworks vào 172.16.0.0/12 + loopback
- Build lần 1: cảnh báo ASPDEPR005 (API obsolete) → sửa sang System.Net.IPNetwork/
  KnownIPNetworks → build lại: 0 error, 0 warning liên quan
- git status xác nhận .env không bị track (đã gitignore)
- git add chọn lọc: CVerify.Core/Program.cs + 9 file trong deployment/ (compose override,
  nginx, github workflow, 6 script) — loại trừ toàn bộ 8 file .md và .env.example
- Commit "fix: trust reverse-proxy forwarded headers; add production deployment
  artifacts" (58609e7), push lên origin/CVerify-uat thành công
```

---

### Prompt 5 — Tạo audit documentation package

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
chuyển sang nhánh doc/Tran-Nhat-long, tạo audit log mới và tạo PR merge vào main
```

**Context khi dùng prompt:**
```text
- Source code đã commit/push trực tiếp lên CVerify-uat (58609e7) ở Prompt 4, không
  qua branch/PR riêng
- Cần tạo audit documentation theo workflow cverify-code-to-ai-audit, chỉ track
  tài liệu (Step 9-15 của SKILL.md), không cần lặp lại track source code
```

**Kết quả mong đợi:**
```text
- Checkout doc/Tran-Nhat-Long, generate audit pack tiếp theo, tạo PR vào main
```

**Kết quả thực tế:**
```text
- Đọc SKILL.md tại ~/.claude/skills/ecc/cverify-code-to-ai-audit/SKILL.md
- Fetch + checkout doc/Tran-Nhat-Long (khớp mapping GitHub username TNL293107)
- Phát hiện tên thư mục thật là "docs/Tran Nhat Long" (có khoảng trắng), khác với ví dụ
  "Tran-Nhat-Long" trong SKILL.md — dùng đúng tên thật
- Đọc audit pack #8 để giữ format nhất quán, xác định folder kế tiếp là #9
- Generate 4 file: AI_AUDIT_LOG.md, CHANGELOG.md, PROMPTS.md, REFLECTION.md
- Commit lên doc/Tran-Nhat-Long: "docs(audit): add audit package #9"
- Tạo audit PR targeting main với reviewers nhnnanh, LucFr1746
```
