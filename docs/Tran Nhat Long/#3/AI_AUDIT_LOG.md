# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
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
Design and Generate documents
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-29 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Tối ưu bảo mật và vấn đề quyền sở hữu trí tuệ |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Data Privacy: PDPA/GDPR

Portfolio của developer không phải chỉ là "thông tin cá nhân" thông thường — nó chứa source code, và source code có thể chứa rất nhiều thứ nhạy cảm mà ứng viên không nhận ra khi upload.
Thực tế phổ biến nhất là hardcoded credentials. Developer hay để API keys, database connection strings, hoặc JWT secrets trong code rồi quên xóa trước khi push. Khi CVerify harvest repo đó và lưu vào database của mình, platform trở thành nơi lưu trữ credentials của bên thứ ba — và nếu database bị breach, thiệt hại không chỉ là thông tin ứng viên mà còn là toàn bộ hệ thống mà credentials đó trỏ tới.
Vấn đề thứ hai là personal data embedded trong code. File config có thể chứa tên server nội bộ, IP address của hạ tầng công ty cũ, hoặc thậm chí dữ liệu test là real customer data mà developer copy từ production để test local. Đây không phải trường hợp hiếm — đây là antipattern cực kỳ phổ biến ở các công ty nhỏ và startup.
Về mặt pháp lý, Việt Nam có Nghị định 13/2023/NĐ-CP về bảo vệ dữ liệu cá nhân, có hiệu lực từ tháng 7/2023. Nếu CVerify lưu trữ dữ liệu ứng viên mà không có consent rõ ràng, không có data retention policy, và không có cơ chế "right to be forgotten" — platform có thể vi phạm ngay từ ngày đầu launch. Trong môi trường SWP391 thì ít ai kiểm tra, nhưng nếu sản phẩm thực sự được triển khai ra ngoài thì đây là rủi ro pháp lý thật sự.
IP Ownership: Ai sở hữu code trong portfolio?

Đây là vùng xám pháp lý mà hầu hết developer Việt Nam không biết mình đang đứng trong đó.
Theo luật lao động và hầu hết hợp đồng IT ở Việt Nam, code viết trong giờ làm việc, dùng tài nguyên công ty, hoặc liên quan đến business của công ty là tài sản trí tuệ của công ty, không phải của cá nhân developer. Điều này có nghĩa là khi ứng viên upload code từ dự án công ty cũ lên CVerify — dù họ là người viết ra — họ đang chia sẻ tài sản không thuộc về họ.
Từ góc nhìn của CVerify, khi platform nhận và lưu trữ code đó, platform trở thành bên thứ ba nhận tài sản sở hữu trí tuệ bị leak. Nếu công ty cũ của ứng viên phát hiện, họ có thể kiện không chỉ ứng viên mà còn kiện cả platform. Đây không phải lý thuyết — LinkedIn đã đối mặt với các tranh chấp tương tự ở quy mô nhỏ, và GitHub có chính sách DMCA takedown để xử lý chính xác loại vấn đề này.
Vấn đề còn phức tạp hơn với private repositories. Nếu ứng viên grant CVerify quyền đọc private repo qua OAuth, và trong repo đó có code của client mà công ty ký NDA — CVerify đang tiếp cận thông tin bảo mật của bên thứ tư mà không có authorization. Chuỗi liability lúc này có tới bốn bên: ứng viên, công ty cũ, client của công ty, và CVerify.
Deepfake & Impersonation

Vấn đề này sẽ xuất hiện ngay khi CVerify có đủ lượng user để ai đó thấy worth it để cheat.
Security của platform bản thân

Tách biệt với data privacy, bản thân hệ thống CVerify có một số attack surface đặc biệt do tính chất của platform.
Vì platform chạy code analysis, nếu không được sandbox đúng cách thì có nguy cơ code execution attacks. Ứng viên có thể submit một repository chứa script được thiết kế để chạy trong quá trình analysis — ví dụ một package.json với postinstall script độc hại, hoặc Makefile với arbitrary commands. Nếu analysis pipeline không isolate môi trường thực thi, attacker có thể compromise server.
OAuth token management cũng là điểm yếu. Khi ứng viên kết nối GitHub, CVerify nhận access token và cần lưu để sync định kỳ. Token này nếu bị leak cho phép attacker đọc toàn bộ repo của ứng viên — bao gồm private repo. Với ASP.NET Core, cần encrypt token at rest, rotate định kỳ, và revoke ngay khi ứng viên disconnect account. Dựa vào các thông tin đã thu thập được từ product CVerify, hãy phân tích và lên hướng giải pháp gợi ý để xử lý các vấn đề đấy
```

#### 4.2. Kết quả AI gợi ý

```text
Báo cáo bao gồm 8 chương, full TOC, header/footer với page number, và color-coded tables. Dưới đây là tóm tắt các hướng giải pháp chính:
Data Privacy (P1 — Ngay lập tức)
Pipeline bắt buộc chạy secret scanning (Gitleaks/TruffleHog) trước khi lưu bất cứ thứ gì vào DB. Implement 4 trụ cột pháp lý của Nghị định 13/2023: consent flow, data retention policy, right-to-erasure endpoint, và DPA với AI vendors. Nguyên tắc core: không persist raw code — chỉ lưu metrics.
IP Ownership (P1 — Ngay lập tức)
Mandatory IP Attestation step trước khi ứng viên kết nối GitHub — với ngôn ngữ pháp lý rõ ràng và lưu timestamp. Trang DMCA/Takedown với SLA 48h acknowledge. Default OAuth scope chỉ public_repo; private repo analysis yêu cầu explicit opt-in thêm.
Deepfake & Impersonation (P2 — Trước launch)
Behavioral consistency analysis (commit pattern, coding style fingerprint, timeline coherence) thay vì verify danh tính trực tiếp. Challenge-response system dựa trên code của chính ứng viên. AI-generated code detection với labeling transparent — không block mà disclose.
Platform Security (P1 — Ngay lập tức)
Sandboxed Docker containers với --network none, read-only filesystem, non-root user cho mọi analysis job. Tuyệt đối không chạy code — chỉ parse AST với tree-sitter. OAuth tokens encrypt AES-256 + Azure Key Vault, không bao giờ lưu plaintext.
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

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-29 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Nghiên cứu sâu về vấn đề ownership và contributor identify của repositories trên GitHub |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
3.2.1 IP Attestation Flow — Buộc ứng viên tự xác nhận ownership. Hướng giải pháp của tôi là ép hệ thống chỉ đọc những repo sau: 1. Open source(nếu nhiều contributors). 2. Private repo (nếu contributor chỉ là 1 mình user). Còn những repo khác sẽ đưa vào phần ứng viên tự khai và từ chối trách nhiệm pháp lý.update lại phần 6. implementation map
```

#### 4.2. Kết quả AI gợi ý

```text
Ứng viên kết nối GitHub
        ↓
CVerify scan danh sách repo
        ↓
   Classify từng repo:
   ├─ Public + license rõ + >1 commit author  →  Auto-include (Case 1)
   ├─ Private + chỉ 1 commit author = user    →  Auto-include (Case 2)  
   ├─ Public + chỉ 1 commit author            →  Self-declaration required (Case 3)
   ├─ Fork của repo khác                      →  Self-declaration required (Case 3)
   └─ Private + nhiều contributors            →  Self-declaration required (Case 3)
        ↓ (Case 3)
   Hiển thị: "Repo này có thể là tài sản của bên thứ ba.
              Bạn xác nhận bạn có quyền chia sẻ?"
   + Platform disclaimer rõ ràng trong ToS
Section 6 đã được update với 2 phần mới:
6.1 Repo Classification Logic — bảng 3 case rõ ràng (xanh/xanh/cam) với điều kiện, hành động, và lý do cho từng loại repo.
6.2 Sprint Breakdown — roadmap được điều chỉnh:

Sprint 1 thêm Repo Classification Engine (item 4) — vì đây là foundation cho toàn bộ IP flow
Sprint 2 thay "IP Attestation flow" bằng Self-declaration UI cho Case 3 + license detection
Sprint 3 thêm Fork chain detection để handle edge case repo fork từ private org
Post-launch thêm item re-classify repos định kỳ khi contributor pattern thay đổi

Và có callout note nhắc dependency: classifier phải xong trước khi build UI.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sprint 1: Add Repo Classification Engine alongside the security items (since it needs to run at OAuth connect time)
Sprint 2: Replace "IP Attestation flow" with "Self-declaration UI + disclaimer for Case 3 repos" + "License detection"
Sprint 3: Keep DMCA/Takedown but also add "Fork detection + edge case handling"
Sprint 4 and Post-launch: Keep as-is
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
| Generate documents |   |   | x |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | The resulting output does not match user needs 100%. | Check manually | The input prompt is more detailed and includes more context and rules. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Verifying AI output requires active oversight and systematic fact-checking to ensure accuracy, prevent misinformation, and catch fabrications. It is best to treat AI as a preliminary draft or a research assistant rather than a flawless authority
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
 
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Đoàn Thế Lực | DE200523 | Further research and contributions on the issue of intellectual property identification. | Có |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 29/5/2026 |
