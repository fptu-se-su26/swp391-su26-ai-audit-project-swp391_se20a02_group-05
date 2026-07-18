# Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Tên project | CVerify |
| Tên sinh viên | Trần Nhất Long |
| MSSV | DE200160 |
| Ngày | 2026-07-07 |
| Phiên số | #9 |
| Tính năng | Production Deployment Audit + ForwardedHeaders Fix |

---

## 2. Quyết định kỹ thuật quan trọng

### 2.1. Ưu tiên đọc code thật thay vì tin vào tài liệu tham khảo

**Quyết định:** Dùng tài liệu `.docx` deployment guide chỉ như điểm khởi đầu, mọi kết luận
cuối cùng đều phải được xác nhận lại bằng cách đọc `Program.cs`, `EnvValidator.cs`,
`appsettings.json`, `config.py` thật.

**Lý do:**
- Tài liệu tham khảo được viết trước, có thể lệch với codebase đã phát triển thêm
  (SignalR, R2, SendGrid, OAuth 3 provider không có trong tài liệu gốc)
- "Never hallucinate, never guess" là yêu cầu tường minh của người dùng cho phase này

**Trade-off:**
- Tốn nhiều lượt đọc file hơn so với việc chỉ paraphrase tài liệu có sẵn
- Đổi lại: phát hiện đúng 3 lỗi CRITICAL mà nếu chỉ dựa vào tài liệu sẽ bỏ sót hoàn toàn

---

### 2.2. Không tự sửa `.env` production thật

**Quyết định:** Chỉ report các lỗi trong `.env` production (seed test account, thiếu
REDIS_PASSWORD, SendGrid key giả...) trong `CONFIG_VALIDATION.md`, không tự động sửa
file `.env` thật đang chạy trên VPS.

**Lý do:**
- AI không có quyền truy cập trực tiếp vào VPS để verify thay đổi có an toàn không
- Sửa sai một biến production (ví dụ đổi `TOKEN_ENCRYPTION_KEY` đang dùng) có thể làm
  mất khả năng giải mã dữ liệu cũ — hậu quả không thể hoàn tác dễ dàng

**Trade-off:**
- Người dùng phải tự áp dụng các thay đổi vào `.env` thật theo checklist
- Đổi lại: tránh rủi ro phá vỡ production do AI hành động ngoài phạm vi quan sát được

---

### 2.3. Không tự áp dụng fix ForwardedHeaders ngay khi phát hiện

**Quyết định:** Ở phase audit, chỉ ghi lỗi ForwardedHeaders vào `CODE_CHANGES_REQUIRED.md`
và chờ người dùng xác nhận, thay vì tự sửa `Program.cs` ngay lập tức.

**Lý do:**
- Cấu hình `KnownNetworks`/`KnownIPNetworks` cần subnet Docker thật từ VPS
  (`docker network inspect`) mà AI không truy cập được trong phiên audit
- Áp dụng `ForwardedHeaders.All` không giới hạn mạng tin cậy sẽ mở lỗ hổng giả mạo
  `X-Forwarded-For`, nguy hiểm hơn cả lỗi ban đầu

**Trade-off:**
- Lỗi vẫn tồn tại thêm 1 phiên làm việc trước khi được sửa
- Đổi lại: khi thực sự áp dụng (Prompt 4), dùng dải mạng mặc định hợp lý
  (`172.16.0.0/12` + loopback) có ghi chú rõ cần verify lại trên VPS thật, thay vì
  một cấu hình mở toang được tạo vội trong lúc chưa có xác nhận từ người dùng

---

### 2.4. Sửa API `IPNetwork`/`KnownNetworks` cũ sang `System.Net.IPNetwork`/`KnownIPNetworks`

**Quyết định:** Sau khi build lần đầu cho ra warning `ASPDEPR005`, chủ động sửa sang
API mới thay vì để lại warning obsolete trong code.

**Lý do:**
- `.NET 10` đã đánh dấu `Microsoft.AspNetCore.HttpOverrides.IPNetwork` và
  `ForwardedHeadersOptions.KnownNetworks` là obsolete, khuyến nghị dùng
  `System.Net.IPNetwork`/`KnownIPNetworks`
- Không nên commit code có warning mới phát sinh từ chính thay đổi của mình

**Outcome:**
- Build lại xác nhận 0 error, 0 warning liên quan đến ForwardedHeaders trước khi commit

---

### 2.5. Commit chọn lọc thay vì `git add -A`

**Quyết định:** Stage từng file cụ thể (`Program.cs` + 9 file trong `deployment/`),
loại trừ hoàn toàn 8 file `.md` và `.env.example` theo đúng yêu cầu người dùng.

**Lý do:**
- Người dùng chỉ định rõ phạm vi loại trừ (secret, `.env`, checklist, `.md`)
- `deployment/*.md` chứa phân tích chi tiết về lỗ hổng bảo mật thật của hệ thống —
  không nên đẩy công khai lên nhánh chính trước khi được review riêng

**Outcome:**
- `git status` sau khi add xác nhận đúng 10 file thay đổi (1 modified + 9 added),
  8 file `.md` vẫn ở trạng thái untracked

---

## 3. Quan sát kỹ thuật

### 3.1. Rate limiter dựa trên `RemoteIpAddress` là điểm mù phổ biến khi thêm reverse proxy

`Program.cs` có logic rate-limit khá đầy đủ (5+ policy theo từng endpoint nhạy cảm),
nhưng toàn bộ đều partition theo `context.Connection.RemoteIpAddress`. Đây là lỗi kinh
điển khi một ứng dụng được thiết kế chạy trực tiếp (không proxy) sau đó được đưa ra sau
Nginx/Load Balancer mà không cập nhật middleware — dễ bị bỏ sót vì ứng dụng vẫn "chạy
được", chỉ có tác dụng bảo vệ là biến mất một cách âm thầm.

### 3.2. Env var naming mismatch giữa 2 service khác ngôn ngữ (C#/Python) là rủi ro triển khai thực tế

`AI_SERVICE_SHARED_SECRET` (phía .NET) và `SHARED_SECRET` (phía Python/pydantic-settings)
mang cùng ý nghĩa nhưng khác tên — loại lỗi này không xuất hiện trong unit test riêng lẻ
của từng service (mỗi service tự đọc file `.env.example` của chính nó và pass), chỉ lộ
ra khi audit chéo cả 2 phía cùng lúc với cùng 1 file `.env` gộp chung.

### 3.3. Next.js `NEXT_PUBLIC_*` inline tại build time là nguồn lỗi dễ bị bỏ sót khi containerize

Biến môi trường đúng trong `.env` không đảm bảo runtime đúng nếu framework inline giá trị
lúc build (`next build`). Docker build arg tại thời điểm build image mới là nguồn sự thật
thực sự cho các biến `NEXT_PUBLIC_*`, không phải biến runtime của container khi chạy.

---

## 4. Điều sẽ làm khác nếu làm lại

```text
1. Nên yêu cầu quyền truy cập read-only vào VPS (hoặc output của `docker network
   inspect`) ngay từ đầu phase audit, để có thể áp dụng fix ForwardedHeaders với
   subnet chính xác trong cùng 1 lượt thay vì phải tách thành 2 phiên làm việc.

2. Có thể viết thêm 1 script nhỏ kiểm tra "biến .env nào được định nghĩa nhưng không
   được bất kỳ code nào đọc tới" (như AI_HOST, AI_DEBUG_MODE phát hiện thủ công ở
   Prompt 3) để tự động hoá việc phát hiện biến môi trường mồ côi trong các lần audit sau.

3. Nên tạo sẵn 1 test tích hợp nhỏ giả lập request qua reverse proxy (set header
   X-Forwarded-For) để xác nhận rate limiter thực sự phân biệt được client thật sau
   khi thêm ForwardedHeaders, thay vì chỉ xác nhận bằng build thành công.
```

---

## 5. Kết luận

Phiên làm việc này khác với các phiên trước ở chỗ trọng tâm không phải là implement
tính năng mới, mà là audit hạ tầng đã tồn tại và tìm ra khoảng cách giữa "trông như đang
chạy" và "sẽ thực sự chạy đúng khi lên production". Ba lỗi CRITICAL phát hiện được đều là
loại lỗi im lặng — không gây lỗi biên dịch, không xuất hiện khi test từng service riêng
lẻ, chỉ lộ ra khi đối chiếu chéo giữa nhiều file cấu hình và nhiều ngôn ngữ (C#/Python)
với nhau. Việc AI chủ động dừng lại để hỏi (khi mục tiêu `.env` mới chưa rõ) và dừng lại
để chờ duyệt (khi thiếu thông tin subnet thật của VPS để sửa ForwardedHeaders) quan trọng
không kém việc tìm ra lỗi — tránh biến một audit hữu ích thành một thay đổi rủi ro không
được kiểm soát.
