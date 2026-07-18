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
| Ngày bắt đầu | 2026-07-11 |
| Ngày hoàn thành | 2026-07-13 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 10a — Production incident fixes | 2026-07-11 | Sửa email provider hardcode, giao diện forum lệch trái, thêm log chẩn đoán OAuth callback, sửa lỗi parse JSON của AI pipeline | Completed |
| Phase 10b — AWS EC2 → GCP Compute Engine migration | 2026-07-13 | Cập nhật script deploy và DEPLOYMENT_GUIDE.md để phản ánh đúng hạ tầng GCP mới | Completed |

---

# [Phase 10a] Production Incident Fixes

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-07-11
- **Mô tả giai đoạn:** Trong lúc vận hành production sau khi domain đổi sang `cverify.com.vn`, phát hiện và sửa 4 lỗi độc lập: (1) OTP email gửi thất bại do provider bị hardcode `SendGrid` trong khi chỉ có SMTP thật; (2) 2 dòng empty-state trong trang forum bị lệch trái do component `Card` dùng chung không truyền `items-center`; (3) liên kết tài khoản GitHub/GitLab thất bại với thông báo "Failed to load pending connection details" nhưng không để lại log nào; (4) AI repository analysis bị chặn ở stage `ArchitectureAnalysis` do lỗi parse JSON khi Claude thỉnh thoảng thêm văn bản thừa sau JSON hợp lệ.
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thêm biến `EMAIL_PROVIDER` (mặc định `Smtp`) để chọn provider gửi email qua env thay vì hardcode `SendGrid` trong compose | Trần Nhất Long | `CVerify/docker-compose.yml`, `CVerify/.env.example` | Commit 5d0dd10 |
| 2 | Thêm `mx-auto` cho 2 khối text empty-state trong trang forum để căn giữa đúng bên trong box `max-w-sm` | Trần Nhất Long | `CVerify/client/src/app/forum/page.tsx` | Commit 4f5dbff |
| 3 | Thêm `LogWarning` vào toàn bộ nhánh early-return im lặng của OAuth callback (state_mismatch, unauthenticated, limit_reached, token_exchange_failed, profile_fetch_failed, provider_already_linked, encryption_key_missing), kèm response status/body thật từ GitHub/GitLab/Google | Trần Nhất Long | `CVerify/CVerify.Core/Modules/Auth/Controllers/AuthController.cs` | Commit bf1cd47 |
| 4 | Thay `text[first_brace:text.rfind('}')+1]` bằng `json.JSONDecoder().raw_decode()` trong `_extract_json`, giữ fallback bounded-slice-repair cho trường hợp JSON bị cắt cụt thật | Trần Nhất Long | `CVerify/CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | Commit 84d0798 |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | 5d0dd10 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/5d0dd10 |
| Commit | 4f5dbff | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/4f5dbff |
| Commit | bf1cd47 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/bf1cd47 |
| Commit | 84d0798 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/84d0798 |
| Branch | CVerify-uat | Cả 4 commit được push thẳng lên CVerify-uat trong lúc xử lý sự cố production trực tiếp |

## Ghi chú

```text
Lỗi 1 (email): docker-compose.yml override cứng EmailSettings__Provider=SendGrid,
bất kể appsettings.json default là "Smtp" — production chỉ có SMTP Gmail thật,
SendGrid API key vẫn là placeholder, khiến mọi OTP thất bại "Permission denied,
wrong credentials".

Lỗi 2 (UI): Card component (card.tsx:47) có wrapper nội bộ "flex flex-col w-full"
không kế thừa items-center từ class truyền vào ngoài, nên phần tử con max-w-sm bị
stretch rồi neo trái thay vì được căn giữa.

Lỗi 3 (OAuth): Toàn bộ nhánh lỗi sớm của OAuthCallback redirect với ?error= nhưng
không log gì, khiến lỗi thật (invalid_grant từ GitLab do secret không khớp, thiếu
GITHUB_CLIENT_ID/GITLAB_CLIENT_ID trên VPS) không thể quan sát được từ log.

Lỗi 4 (AI JSON): _extract_json dùng rfind('}') tìm dấu ngoặc đóng CUỐI CÙNG trong
toàn văn bản trả về, bị kéo dài quá điểm kết thúc JSON thật khi Claude thêm văn bản
thừa chứa ký tự '}' phía sau (ví dụ nhắc tới interface{}).
```

---

# [Phase 10b] AWS EC2 → GCP Compute Engine Migration

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-07-13
- **Mô tả giai đoạn:** Domain/nhà cung cấp hạ tầng đổi sang GCP Compute Engine (VM `cverify-vps`, dự án `cverify-production`). Trong lúc chạy thử deploy thủ công trên VM mới, phát hiện toàn bộ script/tài liệu vận hành vẫn còn path cứng `/home/ec2-user/...` từ thời VPS AWS EC2 cũ, không tương thích với username thật trên VM GCP. Cập nhật lại script và tài liệu để phản ánh đúng hạ tầng hiện tại.
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thay path cứng `/home/ec2-user` bằng `$HOME` để script chạy đúng bất kể username VM | Trần Nhất Long | `.github/workflows/deploy.yml`, `CVerify/deployment/scripts/backup-db.sh`, `deploy.sh`, `health-check.sh`, `restore-db.sh` | Commit 16797a4 |
| 2 | Viết lại các phần đặc thù AWS (VM provisioning, layout Nginx, firewall, GitHub secrets) trong runbook cho đúng quy trình GCP Compute Engine thực tế | Trần Nhất Long | `CVerify/deployment/DEPLOYMENT_GUIDE.md` (+212/-... dòng) | Commit 16797a4 |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | 16797a4 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/16797a4 |
| Branch | CVerify-uat | Commit thẳng lên CVerify-uat sau khi hạ tầng GCP đã chạy healthy |

## Ghi chú

```text
Phát hiện trong lúc thao tác thật (không phải rà soát chủ động trước): chạy deploy
thủ công trên VM GCP mới bị lỗi vì script vẫn tham chiếu /home/ec2-user, phải ghi
đè tạm bằng biến COMPOSE_DIR để deploy được ngay. Nếu không sửa lại source, lần
deploy tự động kế tiếp qua GitHub Actions (kích hoạt bởi chính commit sửa lỗi khác
trong ngày) sẽ gặp lại lỗi y hệt.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
- Email OTP: chuyển sang cấu hình provider qua biến môi trường, gửi thành công qua
  SMTP thật trên production.
- Forum: 2 dòng empty-state căn giữa đúng trên cverify.com.vn/forum.
- OAuth callback: đầy đủ log chẩn đoán cho mọi nhánh lỗi, xác nhận GitHub link
  thành công qua log thật.
- AI repository analysis: vượt qua được stage ArchitectureAnalysis, không còn lỗi
  parse JSON do văn bản thừa.
- Hạ tầng production: chuyển hoàn toàn sang GCP Compute Engine, script/tài liệu
  vận hành đồng bộ với hạ tầng thật.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
- Liên kết tài khoản GitLab vẫn còn lỗi "Failed to load pending connection details"
  ở bước sau khi backend đã xử lý callback thành công (xác nhận qua database) —
  nguyên nhân chính xác ở bước lấy pending-link detail từ frontend chưa được xác
  định trong phạm vi các commit của pack này.
- SENDGRID_API_KEY trên production vẫn là giá trị thật nhưng provider mặc định
  đang dùng SMTP/Failover, chưa xác nhận SendGrid tự nó hoạt động độc lập.
```

---

## 4.3. Cải thiện chính

```text
Thay vì chỉ sửa lỗi hiển bên ngoài (ví dụ "SendGrid báo lỗi" → tìm SendGrid key
thật), cả 2 phase đều đào tới đúng lớp nguyên nhân gốc: config override ẩn trong
docker-compose.yml, wrapper CSS ẩn trong component dùng chung, nhánh code im lặng
không log, và path hạ tầng cũ không được cập nhật theo nhà cung cấp mới. Cách tiếp
cận "thêm log trước khi sửa" ở Phase 10a (lỗi OAuth) đặc biệt hiệu quả vì biến một
lỗi hoàn toàn không thể quan sát thành một lỗi có bằng chứng cụ thể.
```

---

## 4.4. Tổng kết project

```text
3 ngày vận hành production liên tục với nhiều lớp sự cố không liên quan tới nhau
(email, UI, OAuth, AI pipeline, hạ tầng) nhưng đều được xử lý bằng cùng một cách
tiếp cận: đọc log/database thật trước, xác nhận nguyên nhân trước khi sửa, xác nhận
lại bằng test thật trên production sau khi deploy.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
- Điều tra tiếp lỗi pending-connection-details của GitLab (backend đã xác nhận xử
  lý callback thành công qua database nhưng bước tiếp theo vẫn lỗi).
- Viết test tích hợp cho _extract_json với các biến thể trailing-prose khác nhau
  để tránh hồi quy.
- Cân nhắc script kiểm tra tự động các path cứng còn sót lại (ec2-user hoặc tương
  tự) trong deployment/ sau mỗi lần đổi nhà cung cấp hạ tầng.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trần Nhất Long | 13/07/2026 |
