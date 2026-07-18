# Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Tên project | CVerify |
| Tên sinh viên | Trần Nhất Long |
| MSSV | DE200160 |
| Ngày | 2026-07-11 đến 2026-07-13 |
| Phiên số | #10 |
| Tính năng | Production Incident Fixes + AWS EC2 → GCP Compute Engine Migration |

---

## 2. Quyết định kỹ thuật quan trọng

### 2.1. Thêm logging trước khi sửa, thay vì đoán nguyên nhân

**Quyết định:** Khi lỗi liên kết OAuth GitHub/GitLab không để lại dấu vết log nào,
AI chọn thêm `LogWarning` vào toàn bộ 7 nhánh early-return trước, deploy, rồi mới
chờ log thật để xác định nguyên nhân — thay vì đoán và sửa mù dựa trên đọc code.

**Lý do:**
- Đọc code chỉ cho thấy *khả năng* xảy ra (ví dụ dòng `FindFirst(ClaimTypes.
  NameIdentifier)` có thể null), không xác nhận được nhánh nào thực sự bị kích hoạt
  trong lần thử thật của người dùng.

**Trade-off:**
- Cần thêm 1 vòng deploy + thử lại trước khi có câu trả lời thật, chậm hơn so với
  sửa ngay theo suy đoán hợp lý nhất.
- Đổi lại: khi log thật xuất hiện, xác định đúng ngay GitLab lỗi `invalid_grant`
  (secret không khớp) chứ không phải lỗi authentication như suy đoán ban đầu —
  tránh sửa nhầm chỗ.

---

### 2.2. Không tự sửa component `Card` dùng chung

**Quyết định:** Khi phát hiện nguyên nhân lệch UI nằm trong `Card` component dùng
chung, AI chỉ thêm `mx-auto` cục bộ tại `forum/page.tsx`, không sửa hành vi
`items-center` của `Card`.

**Lý do:**
- `Card` được dùng ở nhiều trang khác; đổi hành vi stretch mặc định của nó có thể
  làm lệch layout ở những nơi đang phụ thuộc vào hành vi hiện tại mà không được
  kiểm tra trong phạm vi task này.

**Trade-off:**
- Nếu lỗi tương tự lặp lại ở trang khác dùng `Card` + `max-w-*`, sẽ phải vá lại
  cục bộ thêm lần nữa thay vì được sửa tận gốc một lần.
- Đổi lại: không có rủi ro hồi quy ở các trang không nằm trong phạm vi task.

---

### 2.3. Dùng `json.JSONDecoder().raw_decode()` thay vì vá heuristic `rfind`

**Quyết định:** Thay hoàn toàn cơ chế tìm dấu `}` cuối cùng bằng `raw_decode()` của
thư viện chuẩn, thay vì thêm điều kiện đặc biệt để bỏ qua trường hợp `interface{}`
cụ thể vừa gặp.

**Lý do:**
- `rfind('}')` sai về bản chất với *bất kỳ* nội dung nào chứa `}` xuất hiện sau JSON
  hợp lệ, không riêng gì `interface{}` — vá theo từng trường hợp cụ thể sẽ không
  bao giờ hết case mới.
- `raw_decode()` là công cụ built-in được thiết kế đúng cho chính bài toán này:
  parse một JSON value và dừng lại, bất kể phần còn lại là gì.

**Outcome:**
- 3 test case standalone (trailing prose, JSON cắt cụt, JSON hợp lệ) đều pass; cơ
  chế repair bounded-slice cũ được giữ nguyên làm fallback cho JSON cắt cụt thật.

---

### 2.4. Không tự merge `.env` production khi người dùng chỉ yêu cầu "đưa cho tôi xem"

**Quyết định:** Khi người dùng chỉ yêu cầu xem lại secret cũ trong một file `.env`
local, AI dừng lại đúng ở việc hiển thị bảng giá trị (không in ra phần thật sự nhạy
cảm dạng plaintext không cần thiết) thay vì tự động merge thẳng vào `.env` trên VPS
production dù có thể suy luận đó là bước tiếp theo hợp lý.

**Lý do:**
- Yêu cầu gốc chỉ là "đưa cho tôi" (xem), chưa phải "merge vào VM" — hai việc có
  mức độ rủi ro khác nhau (xem vs. thay đổi cấu hình production đang chạy).

**Outcome:**
- Khi phát hiện đã lỡ đẩy file tạm lên VM trước khi có xác nhận, AI dọn sạch ngay
  lập tức thay vì để lại và chờ được yêu cầu dọn.

---

### 2.5. Cập nhật tài liệu vận hành ngay khi phát hiện lệch so với hạ tầng thật

**Quyết định:** Khi phát hiện `deploy.yml` và các script vận hành vẫn dùng path
`/home/ec2-user` (di sản từ AWS EC2) trong khi VM GCP mới dùng username khác, AI
chủ động đề xuất sửa toàn bộ path bằng `$HOME` và viết lại các phần AWS-specific
trong `DEPLOYMENT_GUIDE.md`, thay vì chỉ ghi đè tạm bằng biến môi trường cho xong
việc trước mắt.

**Lý do:**
- Ghi đè tạm (`COMPOSE_DIR=...`) chỉ giải quyết cho lần deploy thủ công hiện tại;
  lần deploy tự động tiếp theo qua GitHub Actions sẽ dùng lại script gốc trên GitHub
  và gặp lại đúng lỗi đó.

**Outcome:**
- Sau khi commit, `$HOME` hoạt động đúng bất kể VM/username nào được dùng trong
  tương lai, không còn phụ thuộc ngầm vào giả định "chạy trên AWS EC2".

---

## 3. Quan sát kỹ thuật

### 3.1. Kiểm tra binary bằng `strings` không đáng tin với .NET do encoding UTF-16

Khi xác minh code mới đã thực sự được compile vào container hay chưa, lệnh `strings`
trên file DLL trả về 0 match dù code đã đúng — vì .NET dùng chuỗi UTF-16 nội bộ,
không phải ASCII/UTF-8 mà `strings` mặc định tìm. `grep -a` (binary-safe, không giả
định encoding) mới cho kết quả đúng (19/19 match). Đây là một lớp false-negative dễ
gặp khi audit ngược container .NET đã build, cần ghi nhớ cho các lần kiểm tra tương tự.

### 3.2. Lỗi cấu hình override ẩn giữa nhiều lớp (compose → appsettings) khó phát hiện qua unit test

`docker-compose.yml` override `EmailSettings__Provider` ở cấp container, trong khi
`appsettings.json` có default khác — cả hai file đều "đúng" khi đọc riêng lẻ, lỗi
chỉ xuất hiện khi 2 lớp cấu hình chồng lên nhau lúc container thật khởi động. Loại
lỗi này không thể bắt được bằng unit test cấp ứng dụng thông thường, chỉ lộ ra khi
kiểm tra hành vi runtime thật trên production.

### 3.3. Tài liệu/script vận hành dễ "đóng băng" theo hạ tầng ban đầu khi nhà cung cấp thay đổi

`DEPLOYMENT_GUIDE.md` và các script được viết đúng cho AWS EC2 lúc khởi tạo, nhưng
không có cơ chế nào tự động cảnh báo khi hạ tầng chuyển sang GCP với giả định khác
(username VM). Lỗi chỉ lộ ra khi thực sự chạy deploy thủ công trên hạ tầng mới —
gợi ý cần một bước rà soát rõ ràng ("audit lại toàn bộ tài liệu vận hành") mỗi khi
đổi nhà cung cấp hạ tầng, thay vì chỉ tin tài liệu cũ vẫn còn đúng.

---

## 4. Điều sẽ làm khác nếu làm lại

```text
1. Thêm một bước rà soát "grep toàn bộ deployment/ và .github/workflows/ tìm path
   cứng hoặc giả định về username/OS" ngay sau khi xác nhận hạ tầng mới đã chạy
   healthy, thay vì chỉ phát hiện tình cờ trong lúc chạy deploy thủ công.

2. Với lỗi OAuth GitLab "Failed to load pending connection details" vẫn còn tồn
   đọng sau pack này (đã xác nhận backend xử lý callback thành công qua database),
   nên thêm log ở đúng bước lấy pending-link detail từ frontend ngay trong cùng đợt
   thêm logging này, thay vì chỉ log phía OAuthCallback rồi phải điều tra tiếp ở
   một phiên khác.

3. Viết sẵn 1 script kiểm tra nhanh "container đang chạy code mới hay cũ" dùng
   grep -a thay vì strings, để không lặp lại false-negative đã gặp khi audit
   AuthController.cs.
```

---

## 5. Kết luận

Ba ngày làm việc này khác với một phiên phát triển tính năng thông thường ở chỗ mọi
việc đều bắt đầu từ một triệu chứng quan sát được trên production (OTP không gửi
được, giao diện lệch, liên kết tài khoản thất bại, AI pipeline lỗi, deploy thủ công
gặp lỗi path), không phải từ một yêu cầu tính năng có sẵn. Điểm chung xuyên suốt cả
5 commit là AI luôn tìm bằng chứng thật (log, database, response API, kết quả build)
trước khi kết luận nguyên nhân, kể cả khi việc đó có nghĩa là phải thêm log và chờ
thêm một vòng thử lại thay vì trả lời ngay. Việc AI tự phát hiện và minh bạch báo
lại sai lầm của chính mình (false negative từ `strings`) cũng là một tín hiệu tốt về
độ tin cậy của các kết luận còn lại trong pack này.
