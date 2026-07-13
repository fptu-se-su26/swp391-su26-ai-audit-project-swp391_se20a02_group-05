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
| Ngày | 2026-07-11 đến 2026-07-13 |
| Phiên số | #10 |

---

## 2. Danh sách prompt đã sử dụng

### Prompt 1 — Kiểm tra Google login và email OTP

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
Kiểm tra lại google client ID, tại sao chưa dùng được chức năng đăng nhập bằng google
và chức năng đăng nhập bằng email và gửi otp có hoạt động không?
```

**Context khi dùng prompt:**
```text
- Production vừa đổi sang domain cverify.com.vn, một số biến môi trường (Google
  Client ID, SMTP/SendGrid) nghi ngờ chưa được set đúng trên VPS mới.
```

**Kết quả mong đợi:**
```text
- Xác định rõ Google login và gửi OTP có hoạt động không, nếu không thì vì sao.
```

**Kết quả thực tế:**
```text
- Phát hiện GOOGLE_CLIENT_ID/SECRET và NEXT_PUBLIC_GOOGLE_CLIENT_ID đều trống trên
  VPS.
- Phát hiện SENDGRID_API_KEY trống, log thật cho thấy SendGrid trả "Permission
  denied, wrong credentials" từ 2 lần người dùng tự thử gửi OTP trước đó.
```

---

### Prompt 2 — Cập nhật env và commit fix email provider

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
@"D:\downloads\docker env" @"D:\downloads\AI env" @"D:\downloads\core env"
@"D:\downloads\client env"
update latest env
```

**Context khi dùng prompt:**
```text
- Đã có sẵn file .env local cũ (từ lần deploy domain cverify.io.vn trước đây) chứa
  giá trị SMTP/OAuth thật, cần đối chiếu và cập nhật vào .env production mới.
```

**Kết quả mong đợi:**
```text
- Merge đúng các giá trị thiếu/sai vào .env production, không phá vỡ cấu hình khác.
```

**Kết quả thực tế:**
```text
- Trong lúc đối chiếu, AI phát hiện nguyên nhân sâu hơn: docker-compose.yml hardcode
  EmailSettings__Provider=SendGrid, ghi đè default "Smtp" của appsettings.json — sửa
  thành biến EMAIL_PROVIDER cấu hình được (commit 5d0dd10), set SMTP Gmail thật vào
  .env, rebuild và xác nhận Google login trả 200, dữ liệu Postgres không mất qua
  lần recreate container.
```

---

### Prompt 3 — Sửa giao diện forum bị lệch trái

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
Dòng No discussions have been started here yet. Be the first to start a thread!
đang bị lệch sang bên trái, hãy allign lại cho ở giữa khung và đối xứng theo trục
dọc của khung. Các dòng No results match your active filters. Try broadening your
query or clear the filters. cũng bị lỗi tương tự
```

**Context khi dùng prompt:**
```text
- Người dùng phát hiện lỗi UI trong lúc kiểm tra thủ công trang forum trên production.
```

**Kết quả mong đợi:**
```text
- Sửa để 2 dòng text căn giữa đúng trong khung của chúng.
```

**Kết quả thực tế:**
```text
- Xác định nguyên nhân là component Card dùng chung (card.tsx:47) không truyền
  items-center xuống wrapper nội bộ, thêm mx-auto cục bộ tại forum/page.tsx (commit
  4f5dbff), build + deploy + xác nhận trực tiếp trên cverify.com.vn/forum.
```

---

### Prompt 4 — Điều tra lỗi liên kết GitHub/GitLab

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
Đã thử xong, kiểm tra log giúp tôi, hiện Failed to load pending connection details.
cho cả 2 liên kết
```

**Context khi dùng prompt:**
```text
- Trước đó đã đổi GitLab Client Secret và sửa callback URL GitHub, nhưng liên kết
  tài khoản vẫn báo lỗi trên frontend.
```

**Kết quả mong đợi:**
```text
- Xem log để biết chính xác vì sao liên kết thất bại.
```

**Kết quả thực tế:**
```text
- Log hoàn toàn im lặng ở mọi nhánh lỗi. AI thêm LogWarning vào toàn bộ 7 nhánh
  early-return của OAuthCallback (commit bf1cd47) kèm response chi tiết từ
  GitHub/GitLab/Google, sau đó dùng chính log mới để xác nhận GitHub thành công còn
  GitLab lỗi invalid_grant do secret không khớp, và phát hiện thêm
  GITHUB_CLIENT_ID/GITLAB_CLIENT_ID cũng trống trên VPS.
```

---

### Prompt 5 — Sửa lỗi AI repository analysis parse JSON

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
các repo analysis đều bị lỗi ở phần architecture & modularity, Task
ArchitectureAnalysis failed: Claude output returned invalid JSON inside block.
Sanitization failed
```

**Context khi dùng prompt:**
```text
- Chuyển sang kiểm tra tính năng AI Analysis sau khi Google/GitHub OAuth đã ổn định.
```

**Kết quả mong đợi:**
```text
- Sửa lỗi để repository analysis chạy qua được stage Architecture & Modularity.
```

**Kết quả thực tế:**
```text
- Xác định lỗi "Extra data: line 1 column 2984" do _extract_json dùng rfind('}')
  bắt nhầm dấu ngoặc đóng của văn bản thừa Claude thêm sau JSON hợp lệ. Thay bằng
  json.JSONDecoder().raw_decode() (commit 84d0798), viết 3 test case standalone xác
  nhận không hồi quy, build --no-cache để chắc chắn deploy đúng code mới, xác nhận
  pipeline chạy qua ArchitectureAnalysis tới tận CommitIntelligence.
```

---

### Prompt 6 — Yêu cầu commit các sửa đổi deploy khi phát hiện path ec2-user cũ

**Công cụ:** Claude Code (claude-sonnet-5)

**Prompt:**
```text
(Trong lúc AI chạy deploy thủ công trên VM GCP mới và tự phát hiện script vẫn dùng
path /home/ec2-user cũ, AI hỏi lại người dùng có muốn commit các sửa đổi deploy.sh/
health-check.sh/DEPLOYMENT_GUIDE.md lên GitHub ngay không, để lần deploy tự động
qua GitHub Actions sau đó không gặp lại lỗi tương tự — người dùng xác nhận đồng ý.)
client secret mới: GOCSPX-... và commit luôn để không cần chờ nữa, trừ các secret ra
```

**Context khi dùng prompt:**
```text
- Đang trong quá trình dựng lại toàn bộ hạ tầng production trên GCP Compute Engine
  để thay thế VPS AWS EC2 cũ (domain, VM, Nginx, SSL, GitHub Actions).
```

**Kết quả mong đợi:**
```text
- Commit các thay đổi cần thiết lên GitHub, không để lộ secret nào trong git.
```

**Kết quả thực tế:**
```text
- Thay toàn bộ path cứng ec2-user bằng $HOME trong deploy.yml và 4 script vận
  hành, viết lại các phần đặc thù AWS trong DEPLOYMENT_GUIDE.md cho đúng GCP (commit
  16797a4), xác nhận .env vốn đã nằm trong .gitignore nên không secret nào lọt vào
  git trước khi commit.
```

---

## 3. Ghi chú chung

```text
Phần lớn các prompt trong pack này là báo cáo triệu chứng lỗi thực tế trên production
(dạng "đã thử xong, kiểm tra log giúp tôi" hoặc dán nguyên văn thông báo lỗi), không
phải mô tả yêu cầu kỹ thuật chi tiết — AI phải tự đọc log/database/code để xác định
nguyên nhân trước khi đề xuất fix, thay vì được chỉ định sẵn hướng sửa.
```
