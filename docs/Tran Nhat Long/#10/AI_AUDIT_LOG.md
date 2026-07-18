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
| Ngày bắt đầu | 2026-07-11 |
| Ngày hoàn thành | 2026-07-13 |

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
Chẩn đoán và sửa một loạt lỗi production phát sinh trong quá trình vận hành CVerify
trên VPS (email OTP gửi thất bại, đăng nhập/liên kết OAuth GitHub-GitLab thất bại
không rõ nguyên nhân, AI repository analysis bị lỗi JSON, giao diện forum bị lệch),
sau đó thực hiện di dời toàn bộ hạ tầng production từ AWS EC2 sang GCP Compute Engine
khi domain/nhà cung cấp hạ tầng thay đổi. AI được dùng để đọc log/database trực tiếp
trên VPS, xác định nguyên nhân gốc, viết code fix, và cập nhật tài liệu vận hành.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1 — Email OTP thất bại do provider bị hardcode

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-11 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Kiểm tra lý do gửi OTP email thất bại trên production (`cverify.com.vn`), xác định nguyên nhân và sửa. |
| Phần việc liên quan | Backend / Configuration |
| Mức độ sử dụng | AI hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Kiểm tra lại google client ID, tại sao chưa dùng được chức năng đăng nhập bằng google
và chức năng đăng nhập bằng email và gửi otp có hoạt động không?
```

#### 4.2. Kết quả AI gợi ý

```text
AI đọc trực tiếp log production và file .env trên VPS, phát hiện SENDGRID_API_KEY
rỗng và log thực tế cho thấy SendGrid trả về "Permission denied, wrong credentials"
từ 2 lần người dùng tự thử gửi OTP trước đó. Đào sâu thêm, AI phát hiện nguyên nhân
gốc nằm ở docker-compose.yml: biến EmailSettings__Provider bị hardcode cứng thành
"SendGrid" ở cấp container, ghi đè lên default "Smtp" trong appsettings.json —
trong khi production chỉ có SMTP Gmail thật, còn SendGrid API key vẫn là placeholder.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Chẩn đoán nguyên nhân gốc (docker-compose.yml override appsettings.json) được
  dùng nguyên vẹn.
- Code fix: thêm biến EMAIL_PROVIDER (mặc định Smtp) vào docker-compose.yml và
  .env.example để có thể chọn provider mà không cần sửa compose file.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Cung cấp giá trị SMTP Gmail thật (app password) và xác nhận provider cần dùng
  (Smtp) cho VPS — đây là thông tin AI không thể tự có.
- Xác nhận lại qua việc gửi OTP thật trên cverify.com.vn sau khi deploy.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | 5d0dd10 | fix: make email provider configurable instead of hardcoded SendGrid |
| File | `CVerify/docker-compose.yml`, `CVerify/.env.example` | 2 file thay đổi, 5 dòng |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI không dừng lại ở việc coi "SendGrid lỗi" là nguyên nhân cuối cùng, mà tiếp tục
đọc docker-compose.yml để tìm ra vì sao app không tự rơi về SMTP mặc định như đã
cấu hình trong appsettings.json — đây là bước đào sâu quan trọng, nếu dừng sớm sẽ
chỉ sửa nhầm sang việc đi tìm SendGrid API key thật thay vì sửa đúng chỗ.
```

---

### Lần sử dụng AI số 2 — Giao diện forum bị lệch trái

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-11 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Sửa lỗi dòng empty-state trong trang forum bị lệch trái thay vì căn giữa. |
| Phần việc liên quan | Frontend / UI |
| Mức độ sử dụng | AI sinh chính |

#### 4.1. Prompt đã sử dụng

```text
Dòng "No discussions have been started here yet. Be the first to start a thread!"
đang bị lệch sang bên trái, hãy allign lại cho ở giữa khung và đối xứng theo trục
dọc của khung. Các dòng "No results match your active filters. Try broadening your
query or clear the filters." cũng bị lỗi tương tự
```

#### 4.2. Kết quả AI gợi ý

```text
AI xác định nguyên nhân nằm trong component Card dùng chung (client/src/components/
ui/card.tsx:47): wrapper nội bộ là "flex flex-col w-full" không kế thừa items-center
từ class truyền vào ngoài, khiến đoạn <p className="max-w-sm"> bị giới hạn chiều
rộng nhưng không được căn giữa dưới align-items: stretch mặc định — text-center chỉ
căn giữa chữ trong hộp đã bị neo trái đó, không căn giữa chính hộp. AI chọn không
sửa component Card dùng chung (rủi ro ảnh hưởng toàn app), mà thêm mx-auto cục bộ
tại forum/page.tsx cho cả 2 dòng bị lỗi.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ chẩn đoán CSS và code fix (mx-auto) được dùng nguyên vẹn.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Xác nhận trực tiếp trên cverify.com.vn/forum sau khi deploy rằng cả 2 dòng đã
  căn giữa đúng.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | 4f5dbff | fix: center forum empty-state text within its max-w-sm box |
| File | `CVerify/client/src/app/forum/page.tsx` | 2 dòng thay đổi |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI tự giới hạn phạm vi sửa đổi đúng cách — nhận ra Card là component dùng chung và
chủ động tránh sửa nó để không ảnh hưởng các trang khác, thay vào đó chọn fix cục bộ
tại nơi phát sinh lỗi. Đây là cách tiếp cận an toàn hơn so với "sửa tận gốc" khi gốc
đó lại được nhiều nơi khác phụ thuộc vào hành vi hiện tại.
```

---

### Lần sử dụng AI số 3 — Liên kết OAuth GitHub/GitLab thất bại âm thầm

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-11 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Điều tra lỗi "Failed to load pending connection details" khi liên kết tài khoản GitHub/GitLab, không có exception hay log nào giải thích nguyên nhân. |
| Phần việc liên quan | Backend / Debug |
| Mức độ sử dụng | AI sinh chính |

#### 4.1. Prompt đã sử dụng

```text
Đã thử xong, kiểm tra log giúp tôi, hiện Failed to load pending connection details.
cho cả 2 liên kết
```

#### 4.2. Kết quả AI gợi ý

```text
AI đọc log production và xác nhận request callback hoàn tất với 302 nhưng không hề
có dấu vết PROVIDER_LINK_INITIATED, không hit endpoint pending-details, không có
exception nào được ghi — nghĩa là backend đã redirect qua một trong các nhánh
early-return "im lặng" (state_mismatch, unauthenticated, limit_reached,
token_exchange_failed, profile_fetch_failed, provider_already_linked,
encryption_key_missing) vốn không log gì cả. Thay vì đoán mù, AI thêm LogWarning
vào từng nhánh, kèm response status/body thực tế từ GitHub/GitLab/Google API cho
2 nhánh token-exchange và profile-fetch, để lần thử tiếp theo chỉ ra đúng nguyên
nhân thay vì phải đoán tiếp.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ code logging bổ sung vào AuthController.OAuthCallback được dùng nguyên vẹn.
- Sau khi log hoạt động, AI dùng chính log mới này để xác nhận GitHub đã chạy đúng
  (PROVIDER_LINK thành công) còn GitLab lỗi invalid_grant do secret không khớp — dẫn
  tới việc phát hiện thêm GITHUB_CLIENT_ID/GITLAB_CLIENT_ID cũng trống trên VPS.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Thực hiện lại thao tác "Connect GitHub/GitLab" trên web nhiều lần theo yêu cầu
  của AI để tạo request mới cho log bắt được.
- Cung cấp lại Client ID/Secret GitHub, GitLab sau khi AI xác nhận chúng trống
  trên VPS.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | bf1cd47 | diag: log every silent early-exit branch in OAuth callback |
| File | `CVerify/CVerify.Core/Modules/Auth/Controllers/AuthController.cs` | +25 dòng |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Điểm đáng chú ý nhất: AI từng có một lần chẩn đoán sai (kết luận nhầm code mới chưa
được deploy dựa trên kiểm tra strings trên binary, trong khi thực chất là do .NET
dùng UTF-16 nên strings không bắt được chuỗi) — và tự phát hiện, tự sửa sai bằng
cách đổi sang grep -a trước khi báo cáo lại người dùng. Việc AI minh bạch báo lại
"lần kiểm tra trước là false negative, không phải lỗi deploy thật" thay vì im lặng
sửa và không nhắc tới, là điều đáng ghi nhận cho tính tin cậy của audit trail.
```

---

### Lần sử dụng AI số 4 — AI repository analysis lỗi parse JSON

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-11 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Sửa lỗi "Task ArchitectureAnalysis failed: Claude output returned invalid JSON" khiến toàn bộ repository analysis pipeline bị chặn ở stage Architecture & Modularity. |
| Phần việc liên quan | AI Service (Python) / Bugfix |
| Mức độ sử dụng | AI sinh chính |

#### 4.1. Prompt đã sử dụng

```text
các repo analysis đều bị lỗi ở phần architecture & modularity, Task
ArchitectureAnalysis failed: Claude output returned invalid JSON inside block.
Sanitization failed
```

#### 4.2. Kết quả AI gợi ý

```text
AI đọc log lỗi thực tế ("Extra data: line 1 column 2984") và xác định đây là lỗi
JSON kinh điển: có một object JSON hợp lệ, hoàn chỉnh, nhưng còn dữ liệu thừa theo
sau. Đọc code _extract_json thì thấy hàm này dùng text[first_brace:text.rfind('}')+1]
— tức tìm dấu '}' CUỐI CÙNG trong toàn bộ chuỗi trả về, kể cả khi Claude vô tình
thêm một đoạn văn ngắn sau JSON (ví dụ khi đoạn văn đó nhắc tới code có chứa ký tự
'}' như interface{}), khiến vùng cắt bị kéo dài quá điểm kết thúc thật của JSON. AI
thay bằng json.JSONDecoder().raw_decode(), chỉ parse đúng 1 JSON value đầu tiên và
bỏ qua phần còn lại — đồng thời giữ lại cơ chế repair bounded-slice cũ làm fallback
cho trường hợp JSON bị cắt cụt thật sự, để không phá hành vi hiện có.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ fix _extract_json (raw_decode + fallback repair) được dùng nguyên vẹn.
- Bộ 3 test case standalone (JSON có trailing prose, JSON bị cắt cụt, JSON hợp lệ
  thông thường) do AI tự viết để xác nhận fix trước khi deploy được giữ lại làm
  bằng chứng kiểm thử.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Yêu cầu build lại bằng --no-cache sau sự cố cache Docker không nhận code mới ở
  lần sửa OAuth trước đó, để đảm bảo chắc chắn fix lần này thực sự được deploy.
- Chạy lại repository analysis thật trên cverify.com.vn để xác nhận pipeline không
  còn dừng ở ArchitectureAnalysis (pipeline sau đó tiến xa hơn tới CommitIntelligence
  trước khi dừng vì hết credit Anthropic — một lỗi khác, không liên quan tới code).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | 84d0798 | fix: harden Claude JSON extraction against trailing prose |
| File | `CVerify/CVerify.AI/app/pipelines/repository/orchestrators/github_analysis_orchestrator.py` | +14/-2 dòng |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI chọn đúng công cụ chuẩn của thư viện (json.JSONDecoder().raw_decode()) thay vì
tự viết thêm heuristic phức tạp hơn để "vá" hàm rfind cũ — giải pháp tổng quát hơn
vì giải quyết đúng lớp lỗi (bất kỳ nội dung gì theo sau JSON) thay vì chỉ vá đúng
trường hợp cụ thể gặp phải (chữ interface{}).
```

---

### Lần sử dụng AI số 5 — Di dời hạ tầng production từ AWS EC2 sang GCP Compute Engine

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-13 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Dựng lại toàn bộ hạ tầng production (VM, Nginx, SSL, Docker Compose, GitHub Actions) trên GCP Compute Engine để thay thế VPS AWS EC2 cũ, và cập nhật tài liệu/script vận hành cho đúng thực tế mới. |
| Phần việc liên quan | DevOps / Documentation |
| Mức độ sử dụng | AI sinh chính |

#### 4.1. Prompt đã sử dụng

```text
(Chuỗi prompt trong phiên "Google VPS deployment guide", trích các mốc chính)
- Hướng dẫn tạo VM, firewall, Nginx, SSL trên GCP cho domain cverify.com.vn
- "Merge toàn bộ và đưa tôi link để tìm google client secret"
- "client secret mới: GOCSPX-... và commit luôn để không cần chờ nữa, trừ các secret ra"
- Xác nhận thực hiện deploy thủ công phát hiện script vẫn dùng path ec2-user cũ,
  yêu cầu commit các sửa đổi deploy.sh/health-check.sh/DEPLOYMENT_GUIDE.md lên GitHub
  để lần deploy tự động qua GitHub Actions sau này không gặp lại lỗi tương tự
```

#### 4.2. Kết quả AI gợi ý

```text
Trong lúc chạy thử deploy thủ công trên VM GCP mới, AI phát hiện script deploy trên
GitHub vẫn dùng đường dẫn cứng /home/ec2-user/... (di sản từ VPS AWS EC2 cũ), trong
khi VM GCP dùng username khác (long2) — phải ghi đè tạm bằng biến COMPOSE_DIR mới
chạy được. AI đề xuất và thực hiện: thay toàn bộ path cứng ec2-user trong deploy.yml
và 4 script (backup-db.sh, deploy.sh, health-check.sh, restore-db.sh) bằng $HOME để
chạy đúng bất kể VM dùng username nào, đồng thời viết lại các phần đặc thù AWS trong
DEPLOYMENT_GUIDE.md (provisioning VM, layout Nginx, firewall, GitHub secrets) cho
đúng quy trình GCP Compute Engine thực tế vừa thực hiện.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ thao tác dựng hạ tầng GCP (VM, firewall, Nginx nginx.org, Certbot, GitHub
  Actions secrets) do AI thực hiện qua SSH/gcloud CLI.
- Code fix path ec2-user → $HOME trong deploy.yml và 4 script vận hành.
- Nội dung viết lại của DEPLOYMENT_GUIDE.md (212 dòng thay đổi) cho quy trình GCP.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Tự thực hiện các bước không thể giao cho AI: đăng nhập Google Cloud Console để
  lấy Client Secret, cập nhật 3 bản ghi DNS A sang IP VM mới, xác nhận thứ tự ưu
  tiên loại trừ secret khi commit ("trừ các secret ra").
- Xác nhận trực tiếp cverify.com.vn và api.cverify.com.vn trả về 200/Healthy sau
  khi AI hoàn tất deploy.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | 16797a4 | docs: migrate deployment runbook and scripts from AWS EC2 to GCP Compute Engine |
| File | `.github/workflows/deploy.yml`, `CVerify/deployment/DEPLOYMENT_GUIDE.md`, `CVerify/deployment/scripts/*.sh` | 6 file, +188/-40 dòng |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI tự phát hiện ra sự bất nhất giữa hạ tầng thật (GCP, username long2) và tài liệu/
script cũ (viết cho AWS EC2, username ec2-user) trong lúc vận hành thực tế, thay vì
chờ được yêu cầu — và chủ động cảnh báo rằng nếu không sửa, GitHub Actions tự động
sẽ gặp lại đúng lỗi đó ở lần deploy kế tiếp. Đây là ví dụ về AI giữ tài liệu vận
hành đồng bộ với hạ tầng thật thay vì để tài liệu "đóng băng" theo lần viết đầu tiên.
```

---

### Lần sử dụng AI số 6 — Sinh tài liệu AI audit theo quy trình code-to-ai-audit

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-13 |
| Công cụ AI | Claude (Claude Code — claude-sonnet-5) |
| Mục đích sử dụng | Rà soát các commit chưa được audit kể từ audit package #9, sinh audit package #10 đúng vị trí docs/Tran Nhat Long (repo root), không đặt trong CVerify/docs. |
| Phần việc liên quan | Documentation / Audit |
| Mức độ sử dụng | AI sinh chính |

#### 4.1. Prompt đã sử dụng

```text
làm audit pack mới, đẩy vào nhánh docs/Tran-Nhat-Long và đặt pack mới trong
https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/
tree/doc/Tran-Nhat-Long/docs/Tran%20Nhat%20Long chứ không đặt vào CVerify/docs
```

#### 4.2. Kết quả AI gợi ý

```text
AI xác nhận repo có 2 thư mục docs song song (docs/ ở repo root — nơi lưu audit
theo tên sinh viên, và CVerify/docs — tài liệu kỹ thuật của source code), đối chiếu
git log kể từ commit cuối được audit ở #9 (58609e7) để xác định đúng 5 commit của
Trần Nhất Long chưa có audit (5d0dd10, 4f5dbff, bf1cd47, 84d0798, 16797a4), tra cứu
lại 2 phiên làm việc gốc qua session transcript để lấy đúng prompt/ngữ cảnh thật,
rồi sinh audit package #10 tại docs/Tran Nhat Long/#10 trên nhánh doc/Tran-Nhat-Long.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ 4 file tài liệu audit (AI_AUDIT_LOG.md, CHANGELOG.md, PROMPTS.md,
  REFLECTION.md) do AI sinh dựa trên git log và transcript phiên làm việc thật.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Xác nhận vị trí lưu tài liệu (repo-root docs/, không phải CVerify/docs) và nhánh
  đích (doc/Tran-Nhat-Long) trước khi AI thực hiện.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Branch | doc/Tran-Nhat-Long | Nhánh tài liệu audit |
| Folder | docs/Tran Nhat Long/#10 | Audit package này |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI không giả định vị trí commit cuối đã audit, mà tự đối chiếu git log với nội
dung CHANGELOG.md của package #9 để xác định chính xác 5 commit còn thiếu — tránh
tạo audit trùng lặp hoặc bỏ sót.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Chẩn đoán lỗi production (log/DB) |   |   |   | x | Đọc log/DB trực tiếp trên VPS |
| Sửa cấu hình (email provider) |   |   | x |   | Cần thông tin thật từ người dùng |
| Sửa CSS/UI | | | | x | |
| Sửa code backend (logging, JSON parsing) | | | | x | |
| Dựng hạ tầng GCP + viết lại tài liệu deploy | | | | x | |
| Viết tài liệu audit | | | | x | |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI từng kết luận sai rằng code logging mới chưa được deploy, do dùng lệnh `strings` trên binary .NET (không bắt được chuỗi UTF-16), dẫn tới nghi ngờ nhầm việc build cache. | AI tự phát hiện khi thử lại bằng `grep -a` và thấy 19/19 match thay vì 0. | AI tự sửa và báo cáo minh bạch lại là false negative, không lặp lại kết luận sai đó ở các bước sau. |
| 2 | AI không tự tạo branch/PR riêng cho 5 commit trong pack này — tất cả đều được commit thẳng lên `CVerify-uat` trong các phiên xử lý sự cố production trực tiếp, theo đúng thực tế thao tác (ưu tiên khắc phục nhanh trên VPS) thay vì theo quy trình feature-branch chuẩn. | Đối chiếu `git log` không thấy branch riêng cho từng commit. | Chấp nhận theo đúng precedent của package #9 — track tài liệu audit vẫn có PR riêng vào `main`, tách biệt với track source code. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Người dùng xác nhận trực tiếp trên production (cverify.com.vn) sau mỗi lần deploy:
gửi OTP thành công qua SMTP, forum hiển thị đúng căn giữa, liên kết GitHub thành
công (GitLab cần thêm bước cấp lại secret), repository analysis chạy qua được
stage ArchitectureAnalysis, và cuối cùng cverify.com.vn/api.cverify.com.vn trả
về 200/Healthy trên hạ tầng GCP mới.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Trần Nhất Long (DE200160): Vận hành và khắc phục sự cố production độc lập cho
CVerify trong 3 ngày (11-13/07/2026) — từ debug cấu hình email/OAuth, sửa lỗi AI
pipeline, sửa UI, đến di dời toàn bộ hạ tầng sang nhà cung cấp mới (GCP). Dùng
Claude Code trong toàn bộ quá trình để đọc log/DB thật trên VPS, viết fix, và
đồng bộ lại tài liệu vận hành.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trần Nhất Long | DE200160 | Vận hành production, sửa lỗi email/OAuth/AI pipeline/UI, di dời hạ tầng GCP | Có | Commit 5d0dd10, 4f5dbff, bf1cd47, 84d0798, 16797a4 |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trần Nhất Long | 13/07/2026 |
