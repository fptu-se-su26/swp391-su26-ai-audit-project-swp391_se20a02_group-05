# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - Hệ thống xác thực thông tin và quản lý hồ sơ năng lực dành cho Doanh nghiệp |
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV / Danh sách MSSV | DE190105 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-06-02 |
| Ngày hoàn thành | 2026-06-10 |

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
Mục tiêu của tôi khi sử dụng AI trong phase này là tìm hiểu và làm rõ kiến thức về cơ chế
Refresh Token Rotation trong JWT Authentication: hiểu tại sao cần rotation, sự khác biệt giữa
Sliding Session Expiry và Absolute Expiry, và khi nào nên áp dụng từng cơ chế trong hệ thống
CVerify. Tôi hoàn toàn không dùng AI để viết code — toàn bộ mã nguồn xác thực do tôi tự triển
khai sau khi đã hiểu rõ nguyên lý.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 02/06/2026 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Giải thích cơ chế Refresh Token Rotation và so sánh Sliding vs Absolute Expiry |
| Phần việc liên quan | Thiết kế kiến trúc Authentication |
| Mức độ sử dụng | Hỏi kiến thức, không lấy code |

#### 4.1. Prompt đã sử dụng

```text
Bạn hãy giải thích cho tôi cơ chế Refresh Token Rotation trong JWT Authentication là gì,
tại sao cần rotation thay vì chỉ dùng một refresh token dài hạn, và sự khác biệt giữa
Sliding Session Expiry và Absolute Expiry. Hệ thống của tôi có hai nhóm người dùng là
Business User (đăng nhập dài hạn) và Admin (yêu cầu bảo mật cao hơn) — tôi nên áp dụng
chiến lược nào cho từng nhóm?
```

#### 4.2. Kết quả AI gợi ý

AI giải thích rõ ràng rằng Refresh Token Rotation nghĩa là mỗi lần dùng refresh token để lấy
access token mới, server sẽ đồng thời cấp một refresh token mới và vô hiệu hóa cái cũ, giúp
phát hiện token bị đánh cắp (nếu cùng lúc có hai request dùng cùng refresh token thì một trong
hai là tấn công). AI cũng giải thích Sliding Expiry (thời gian hết hạn bị đẩy lại mỗi khi
người dùng hoạt động) phù hợp với Business User cần phiên làm việc liên tục, trong khi
Absolute Expiry (token hết hạn bất kể hoạt động) phù hợp với Admin cần kiểm soát phiên chặt chẽ.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tôi chỉ sử dụng phần giải thích nguyên lý để định hình quyết định thiết kế: áp dụng Sliding
Expiry 7 ngày cho Business User và Absolute Expiry 8 giờ cho Admin. Không có dòng code nào
được lấy từ AI.

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi bổ sung thêm cơ chế lưu token family (nhóm các refresh token cùng phiên) vào Redis để
có thể revoke toàn bộ phiên khi phát hiện tấn công — đây là điểm mà AI không đề cập. Ngoài
ra tôi thêm fingerprint thiết bị vào payload để tăng độ tin cậy khi kiểm tra token.

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| File liên quan | `docs/Truong Van Hieu/#4/PROMPTS.md` |
| Kết quả áp dụng | Hệ thống Authentication với Refresh Token Rotation + Redis token family |

#### 4.6. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: Claude giải thích rất mạch lạc và có ví dụ minh hoạ tấn công token theft,
  giúp tôi hiểu sâu hơn tại sao rotation quan trọng chứ không chỉ học thuộc khái niệm.
- Bài học rút ra: Sau khi hiểu nguyên lý, việc tự triển khai code thực tế trong ASP.NET Core
  với Entity Framework và Redis yêu cầu rất nhiều xử lý chi tiết (concurrency, atomic update)
  mà AI không thể giải quyết thay tôi được.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Phân tích yêu cầu | [x] |  |  |  | Tự phân tích từ đặc tả CVerify |
| Thiết kế database | [x] |  |  |  | Tự thiết kế bảng RefreshTokens |
| Thiết kế kiến trúc hệ thống |  | [x] |  |  | Tham khảo nguyên lý Rotation từ AI |
| Code backend | [x] |  |  |  | Tự code ASP.NET Core 100% |
| Debug lỗi | [x] |  |  |  | Tự debug concurrency issue trong token rotation |
| Viết test case | [x] |  |  |  | Tự viết xUnit test |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI mô tả rotation ở mức lý thuyết, không đề cập vấn đề race condition khi nhiều request đồng thời dùng cùng refresh token. | Phát hiện khi test với JMeter gửi nhiều request song song. | Tự triển khai atomic update với Redis SETNX để đảm bảo chỉ một request dùng token thành công. |
| 2 | AI không nhắc đến việc cần lưu token family để có thể revoke toàn bộ phiên khi phát hiện tấn công. | Tự nhận ra khi thiết kế threat model cho CVerify. | Thiết kế thêm bảng TokenFamily và cơ chế cascade revoke. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc RFC 6749 (OAuth 2.0) và tài liệu Microsoft Identity Platform để xác nhận nguyên lý
   AI mô tả là chính xác.
2. Thực hành triển khai thử nghiệm trên môi trường local và kiểm thử các kịch bản tấn công
   (token reuse, concurrent refresh) để xác nhận thiết kế hoạt động đúng.
3. So sánh với cách triển khai của các dự án mã nguồn mở (IdentityServer4) để đảm bảo không
   bỏ sót edge case quan trọng.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Đóng góp thực chất tự thân:
  1. Tự nghiên cứu và đặt câu hỏi để hiểu nguyên lý Refresh Token Rotation.
  2. Tự thiết kế schema bảng RefreshTokens với token family ID.
  3. Tự triển khai toàn bộ code xác thực trong ASP.NET Core, bao gồm middleware,
     service layer và Redis integration.
  4. Tự phát hiện và xử lý race condition trong atomic token rotation.
- AI hỗ trợ: Chỉ giải thích khái niệm lý thuyết Refresh Token Rotation và Expiry strategies.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trương Văn Hiếu | DE190105 | Thiết kế và triển khai phân hệ Authentication với JWT + Refresh Token Rotation | Có (Chỉ hỏi kiến thức) | Code ASP.NET Core, schema DB, Redis integration |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Thiết kế Use Case cho phân hệ xác thực người dùng | Có | Bản vẽ UC Authentication |
| Đoàn Thế Lực | DE200523 | Thiết kế API contract và tích hợp với frontend | Có | API documentation |
| Nguyễn La Hòa An | DE201043 | Viết tài liệu SRS phân hệ Authentication | Có | Tài liệu SRS |
| Trần Nhất Long | DE200160 | Viết integration test cho luồng đăng nhập | Có | Bộ test cases |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giải thích rõ ràng và có cấu trúc về nguyên lý Refresh Token Rotation, giúp tôi nhanh
chóng hiểu bản chất bảo mật của cơ chế này thay vì phải đọc nhiều bài blog dài dòng.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Tôi không sử dụng code mẫu của AI (nếu có) vì cần phải tích hợp với kiến trúc ASP.NET Core
Identity có sẵn của CVerify, và cần xử lý các edge case như race condition mà AI không đề cập.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Đối chiếu với RFC 6749, tài liệu Microsoft và kiểm thử thực tế với JMeter để xác nhận
cơ chế hoạt động đúng trong điều kiện concurrent request.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Khâu hiểu tại sao cần rotation (chứ không chỉ "nên làm vậy") sẽ mất nhiều thời gian đọc
tài liệu hơn. AI giúp tôi hiểu bản chất trong vài phút.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Hiểu sâu về vòng đời token trong hệ thống phân tán và tầm quan trọng của việc thiết kế
cơ chế xác thực an toàn ngay từ đầu thay vì vá lỗi sau.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI là nguồn học liệu tốt để hiểu khái niệm, nhưng không thể thay thế việc tự đọc RFC
và tự triển khai để thực sự hiểu sâu và làm chủ công nghệ.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/06/2026 |
