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
| Ngày bắt đầu | 2026-07-07 |
| Ngày hoàn thành | 2026-07-07 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 09 — chore/fix: Production Deployment Audit + ForwardedHeaders Fix | 2026-07-07 | Audit toàn diện hạ tầng deploy production (VPS cverify.io.vn), sinh bộ tài liệu deployment/ (18 file), và fix lỗi rate-limit bị vô hiệu hoá sau reverse proxy trong Program.cs | Completed |

---

# [Phase 09] chore/fix: Production Deployment Audit + ForwardedHeaders Fix

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-07-07
- **Mô tả giai đoạn:** Audit toàn bộ hạ tầng triển khai production của CVerify dựa trên `.env`/`docker-compose.yml` thật của VPS `cverify.io.vn` (2 vCPU/4GB), phát hiện 3 lỗi CRITICAL (seed test account crash, Core↔AI HMAC auth mismatch, frontend build-time API URL sai) và nhiều lỗi HIGH/MEDIUM khác. Sinh bộ tài liệu `deployment/` đầy đủ (audit, validation, kiến trúc, compose override, nginx, CI/CD, 6 script vận hành, security review, checklist). Sau đó áp dụng 1 code fix thật đã được duyệt (ForwardedHeaders middleware) vào `Program.cs` để khắc phục rate-limit bị vô hiệu hoá khi có Nginx đứng trước, và commit chọn lọc lên GitHub.
- **Trạng thái hiện tại:** Completed
- **Commit:** `58609e7` (CVerify-uat)
- **PR:** Không tạo PR riêng cho phase này — commit thẳng lên `CVerify-uat` theo yêu cầu người dùng; audit documentation này sẽ được PR riêng vào `main` theo track tài liệu.

## Lịch sử commit liên quan

| Commit | Mô tả | Ngày |
|---|---|---|
| `58609e7` | fix: trust reverse-proxy forwarded headers; add production deployment artifacts | 2026-07-07 |

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thêm `using Microsoft.AspNetCore.HttpOverrides;` và block `app.UseForwardedHeaders(...)` ngay sau `builder.Build()`, giới hạn `KnownIPNetworks` vào `172.16.0.0/12` (dải bridge mặc định Docker) + loopback để tránh giả mạo `X-Forwarded-For` | Trần Nhất Long | `CVerify.Core/Program.cs` | Commit `58609e7` |
| 2 | Tạo `REPOSITORY_AUDIT.md`: audit kiến trúc, network, volume, thứ tự khởi động, background worker, SSE/SignalR, OAuth, storage, safety guard | Trần Nhất Long | `deployment/REPOSITORY_AUDIT.md` | Không commit (loại trừ theo yêu cầu — file `.md`) |
| 3 | Tạo `CONFIG_VALIDATION.md`: phát hiện 3 lỗi CRITICAL (seed test account crash, HMAC secret mismatch, frontend build-time URL) + các lỗi HIGH/MEDIUM/LOW khác | Trần Nhất Long | `deployment/CONFIG_VALIDATION.md` | Không commit (file `.md`) |
| 4 | Tạo `ENVIRONMENT_VALIDATION.md`: liệt kê biến bắt buộc/tùy chọn, biến GitHub Secrets, biến không được commit; bổ sung addendum sau khi đối chiếu file `.env` dev mới | Trần Nhất Long | `deployment/ENVIRONMENT_VALIDATION.md` | Không commit (file `.md`) |
| 5 | Tạo `ARCHITECTURE.md`: sơ đồ luồng HTTP/HTTPS/SignalR/SSE/AI communication | Trần Nhất Long | `deployment/ARCHITECTURE.md` | Không commit (file `.md`) |
| 6 | Tạo `docker-compose.prod.yml`: override sửa 3 lỗi CRITICAL, right-size resource limits cho VPS 2vCPU/4GB | Trần Nhất Long | `deployment/docker-compose.prod.yml` | Commit `58609e7` |
| 7 | Tạo `deployment/.env.example` dạng dual-profile ([DEV]/[PROD]) sau khi đối chiếu thêm file `.env` dev mới | Trần Nhất Long | `deployment/.env.example` | Không commit (loại trừ — file `.env`) |
| 8 | Tạo `nginx/cverify.conf`: HTTPS, HTTP redirect, SSE (`proxy_buffering off`), SignalR WebSocket upgrade, rate limiting, security headers | Trần Nhất Long | `deployment/nginx/cverify.conf` | Commit `58609e7` |
| 9 | Tạo `github/deploy.yml`: pipeline build/test/deploy qua SSH, không fabricate test project không tồn tại | Trần Nhất Long | `deployment/github/deploy.yml` | Commit `58609e7` |
| 10 | Tạo 6 script vận hành: `deploy.sh`, `backup-db.sh`, `restore-db.sh`, `cleanup-workspaces.sh`, `renew-ssl.sh`, `health-check.sh` | Trần Nhất Long | `deployment/scripts/*.sh` | Commit `58609e7` |
| 11 | Tạo `SECURITY_REVIEW.md`: phân loại CRITICAL/HIGH/MEDIUM/LOW, khuyến nghị rotate secret đã lộ | Trần Nhất Long | `deployment/SECURITY_REVIEW.md` | Không commit (file `.md`) |
| 12 | Tạo `DEPLOYMENT_CHECKLIST.md`: 14 bước triển khai có purpose/command/verification/rollback/risk | Trần Nhất Long | `deployment/DEPLOYMENT_CHECKLIST.md` | Không commit (file `.md`, checklist) |
| 13 | Tạo `DEPLOYMENT_GUIDE.md`: cấu trúc thư mục VPS, backup strategy, monitoring nhẹ | Trần Nhất Long | `deployment/DEPLOYMENT_GUIDE.md` | Không commit (file `.md`) |
| 14 | Tạo `CODE_CHANGES_REQUIRED.md`: báo cáo lỗi ForwardedHeaders trước khi áp dụng, chờ duyệt | Trần Nhất Long | `deployment/CODE_CHANGES_REQUIRED.md` | Không commit (file `.md`) — nội dung sau đó được áp dụng vào Program.cs ở mục 1 |

## AI có hỗ trợ không?

| | Có | Không |
|---|---|---|
| AI hỗ trợ việc này | ✅ | |

**Chi tiết:** Claude (Claude Code) thực hiện toàn bộ việc đọc code thật để audit (Program.cs, EnvValidator.cs, appsettings.json, config.py), sinh 18 file trong `deployment/`, áp dụng code fix ForwardedHeaders, build kiểm tra 2 lần, và thực hiện git add/commit/push chọn lọc. Developer cung cấp file `.env`/`docker-compose.yml` thật, xác nhận thông tin VPS, và quyết định phạm vi file cần loại trừ khi commit.

## Lỗi / vấn đề phát sinh

| STT | Lỗi / Vấn đề | Nguyên nhân | Cách xử lý |
|---:|---|---|---|
| 1 | `SEED_TEST_ACCOUNTS=true` cùng `ASPNETCORE_ENVIRONMENT=Production` sẽ làm Core crash ngay khi khởi động | `Program.cs:104-107` throw `InvalidOperationException` theo thiết kế fail-fast | Ghi rõ trong `CONFIG_VALIDATION.md`, không tự sửa `.env` production thật (không có quyền truy cập VPS), chỉ cập nhật `.env.example` mẫu |
| 2 | AI service không xác thực được HMAC signature từ Core | `config.py` đọc `SHARED_SECRET`/`CLIENT_ID` nhưng `.env`/compose chỉ có `AI_SERVICE_SHARED_SECRET` | Thêm mapping `environment:` trong `docker-compose.prod.yml` (không cần sửa code) |
| 3 | Build lần 1 của fix ForwardedHeaders bị cảnh báo `ASPDEPR005` (obsolete API) | Dùng `Microsoft.AspNetCore.HttpOverrides.IPNetwork`/`KnownNetworks` — API cũ trong .NET 10 | Sửa sang `System.Net.IPNetwork`/`KnownIPNetworks`, build lại xác nhận 0 warning liên quan |
| 4 | Rate limiter dùng `RemoteIpAddress` sẽ luôn nhận IP của Nginx sau khi có reverse proxy | Thiếu middleware `UseForwardedHeaders` trong `Program.cs` | Thêm `app.UseForwardedHeaders(...)` với `KnownIPNetworks` giới hạn (không mở hoàn toàn để tránh giả mạo) |

## Kết quả đạt được

```text
- Phát hiện và ghi nhận 3 lỗi CRITICAL + nhiều lỗi HIGH/MEDIUM/LOW trong cấu hình
  production thật, không tự ý sửa .env production (ngoài quyền hạn/truy cập)
- Sinh đầy đủ bộ tài liệu deployment/ (18 file): audit, validation, kiến trúc,
  compose override, nginx, CI/CD, 6 script vận hành, security review, checklist
- Áp dụng thành công 1 code fix thật: ForwardedHeaders middleware trong Program.cs,
  build xác nhận 0 error/0 warning liên quan
- Commit 58609e7 lên CVerify-uat: chỉ gồm code fix + artifact triển khai không phải
  .md/.env/checklist, đúng theo yêu cầu loại trừ của người dùng
- Push thành công lên origin/CVerify-uat
```
