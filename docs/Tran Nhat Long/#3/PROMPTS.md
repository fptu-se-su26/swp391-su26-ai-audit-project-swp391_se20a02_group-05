# Prompt Log

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
| Ngày cập nhật gần nhất | 2026-05-29 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 2026-05-29 | Gemini | Tối ưu bảo mật và vấn đề quyền sở hữu trí tuệ | Data Privacy: PDPA/GDPR

Portf... | Báo cáo bao gồm 8 chương, full... | Không |   |
| 2 | 2026-05-29 | ChatGPT | Nghiên cứu sâu về vấn đề ownership và contributor identify của repositories trên GitHub | 3.2.1 IP Attestation Flow — Bu... | Ứng viên kết nối GitHub
      ... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-29 |
| Công cụ AI | Gemini |
| Mục đích | Tối ưu bảo mật và vấn đề quyền sở hữu trí tuệ |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi tối ưu |

#### 5.1. Prompt nguyên văn

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

#### 5.2. Bối cảnh khi viết prompt

```text
 
```

#### 5.3. Kết quả AI trả về

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

#### 5.4. Kết quả đã áp dụng vào bài

```text
 
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
 
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-29 |
| Công cụ AI | ChatGPT |
| Mục đích | Nghiên cứu sâu về vấn đề ownership và contributor identify của repositories trên GitHub |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi tối ưu |

#### 5.1. Prompt nguyên văn

```text
3.2.1 IP Attestation Flow — Buộc ứng viên tự xác nhận ownership. Hướng giải pháp của tôi là ép hệ thống chỉ đọc những repo sau: 1. Open source(nếu nhiều contributors). 2. Private repo (nếu contributor chỉ là 1 mình user). Còn những repo khác sẽ đưa vào phần ứng viên tự khai và từ chối trách nhiệm pháp lý.update lại phần 6. implementation map
```

#### 5.2. Bối cảnh khi viết prompt

```text
Một vài edge case cần handle thêm:
Repo public nhưng chỉ có 1 contributor — đây là trường hợp ambiguous. Có thể là personal project push lên public, cũng có thể là ứng viên copy code từ internal repo rồi push lên GitHub để chạy qua CVerify. Nên xử lý case này giống Case 3 thay vì Case 1.
Fork của private repo thành public — GitHub API có fork: true và parent field, nên có thể detect nếu repo là fork. Fork repo cần xử lý riêng vì quyền ownership phức tạp hơn.
Contributor count vs. commit author count — GitHub phân biệt "contributor" (người có merge quyền hoặc được list) với "commit authors". Nên dùng commit author list từ git log thay vì contributor API vì chính xác hơn.
```

#### 5.3. Kết quả AI trả về

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

#### 5.4. Kết quả đã áp dụng vào bài

```text
Sprint 1: Add Repo Classification Engine alongside the security items (since it needs to run at OAuth connect time)
Sprint 2: Replace "IP Attestation flow" with "Self-declaration UI + disclaimer for Case 3 repos" + "License detection"
Sprint 3: Keep DMCA/Takedown but also add "Fork detection + edge case handling"
Sprint 4 and Post-launch: Keep as-is
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
 
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

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

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

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

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
 
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
 
```

---

## 7. Prompt chưa hiệu quả

```text
Chưa có prompt chưa hiệu quả được ghi nhận.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
To get the best AI answers, provide clear context, specific constraints, and explicit goals. High-quality prompts act like a well-briefed assistant, saving you time and follow-up questions.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Prompting is less about "clever wording" and more about clear delegation and context-setting. Treat AI like a brilliant but naive consultant: instead of giving a vague command, provide a specific role, background context, and clear instructions for the output format
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Continuously refine my responses based on your feedback and preferences. Next time, I will improve by personalizing the formatting to my exact needs, providing even more granular, locally specific data
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Design | 2 |  |

---

## 10. Checklist chất lượng prompt

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | x | |
| Prompt có đủ bối cảnh | x | |
| Tự kiểm tra và chỉnh sửa | x | |

---

## 11. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 29/5/2026 |
