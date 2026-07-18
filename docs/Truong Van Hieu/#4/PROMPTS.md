# Prompt Log

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
| Ngày cập nhật gần nhất | 2026-06-10 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
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
| 1 | 02/06/2026 | Claude | Hiểu nguyên lý Refresh Token Rotation và chiến lược Expiry | "Giải thích Refresh Token Rotation là gì, tại sao cần rotation và so sánh Sliding vs Absolute Expiry..." | Hiểu nguyên lý rotation, chọn được chiến lược expiry phù hợp cho từng nhóm user | Có (Chỉ ý tưởng thiết kế) | Sequence diagram luồng Auth |
| 2 | 03/06/2026 | Claude | Làm rõ cách phát hiện tấn công khi cùng refresh token bị dùng hai lần | "Nếu cùng một refresh token được dùng hai lần thì hệ thống nên xử lý như thế nào..." | Hiểu cơ chế phát hiện và cascade revoke | Có (Chỉ nguyên lý thiết kế) | Thiết kế TokenFamily |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 02/06/2026 |
| Công cụ AI | Claude |
| Mục đích | Hiểu nguyên lý Refresh Token Rotation và chọn chiến lược Expiry cho CVerify |
| Phần việc liên quan | Thiết kế kiến trúc Authentication |
| Mức độ sử dụng | Hỏi kiến thức |

#### 5.1. Prompt nguyên văn

```text
Bạn hãy giải thích cho tôi cơ chế Refresh Token Rotation trong JWT Authentication là gì,
tại sao cần rotation thay vì chỉ dùng một refresh token dài hạn, và sự khác biệt giữa
Sliding Session Expiry và Absolute Expiry. Hệ thống của tôi có hai nhóm người dùng là
Business User (đăng nhập dài hạn) và Admin (yêu cầu bảo mật cao hơn) — tôi nên áp dụng
chiến lược nào cho từng nhóm?
```

#### 5.2. Bối cảnh khi viết prompt

Tôi cần thiết kế phân hệ Authentication cho CVerify với hai nhóm người dùng có nhu cầu bảo
mật khác nhau. Trước khi quyết định chiến lược, tôi muốn hiểu rõ nguyên lý của Refresh Token
Rotation để không chọn sai cơ chế.

#### 5.3. Kết quả AI trả về

AI giải thích rõ ràng:
- Rotation: mỗi lần dùng refresh token → server cấp token mới + vô hiệu hóa token cũ → nếu
  kẻ tấn công dùng token cũ sẽ bị phát hiện vì token đã bị revoke.
- Sliding Expiry: thời gian hết hạn bị đẩy lùi mỗi khi người dùng hoạt động → phù hợp Business
  User cần phiên làm việc liên tục.
- Absolute Expiry: token hết hạn bất kể hoạt động → phù hợp Admin vì giới hạn thời gian tối đa
  một phiên.

#### 5.4. Kết quả đã áp dụng vào bài

Tôi sử dụng nguyên lý trên để quyết định: Business User dùng Sliding Expiry 7 ngày; Admin dùng
Absolute Expiry 8 giờ. Không dùng code từ AI.

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

AI không đề cập đến Token Family và cascade revoke — tôi tự bổ sung thiết kế này để có thể
vô hiệu hóa toàn bộ phiên của một người dùng khi phát hiện tấn công.

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Claude |
| Mục đích | Hiểu cách hệ thống phát hiện và phản ứng khi cùng refresh token bị dùng hai lần |
| Phần việc liên quan | Thiết kế xử lý tấn công token theft |
| Mức độ sử dụng | Hỏi kiến thức |

#### 5.1. Prompt nguyên văn

```text
Nếu cùng một refresh token bị dùng hai lần (ví dụ kẻ tấn công dùng token đánh cắp trước
khi người dùng hợp lệ dùng nó), hệ thống nên phát hiện và xử lý như thế nào? Có cần
revoke toàn bộ phiên không hay chỉ revoke token đó?
```

#### 5.2. Bối cảnh khi viết prompt

Sau khi hiểu rotation cơ bản, tôi cần làm rõ phản ứng của hệ thống khi phát hiện token bị
sử dụng lại để thiết kế xử lý đúng.

#### 5.3. Kết quả AI trả về

AI giải thích: khi phát hiện token reuse (cùng token được dùng lần 2), đây là dấu hiệu chắc
chắn của token theft → nên revoke toàn bộ token family (tức là revoke phiên của người dùng
đó) và yêu cầu đăng nhập lại, không chỉ revoke một token.

#### 5.4. Kết quả đã áp dụng vào bài

Hiểu rõ hơn về thiết kế token family để cascade revoke. Tôi tự thiết kế bảng TokenFamilies
và logic revoke trong RefreshTokenService.

#### 5.5. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt tạo ra kết quả tốt

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
"Giải thích Refresh Token Rotation là gì, tại sao cần rotation và so sánh Sliding vs
Absolute Expiry. Hệ thống của tôi có Business User và Admin — tôi nên áp dụng chiến
lược nào cho từng nhóm?"
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này định hình toàn bộ quyết định thiết kế kiến trúc xác thực của CVerify: loại token,
thời hạn, chiến lược phân biệt theo nhóm user. Sai ở đây sẽ dẫn đến lỗ hổng bảo mật nghiêm
trọng sau này.
```

### 6.3. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Đối chiếu với RFC 6749, đọc tài liệu Microsoft Identity Platform và kiểm thử thực tế bằng
JMeter với kịch bản concurrent refresh để xác nhận thiết kế không bị race condition.
```

---

## 7. Bài học về cách viết prompt

### 7.1. Khi viết prompt cần cung cấp gì?

```text
1. Bối cảnh hệ thống cụ thể (CVerify với hai nhóm user có nhu cầu bảo mật khác nhau).
2. Câu hỏi có tính quyết định (chọn chiến lược nào) thay vì hỏi chung chung.
3. Thông tin về ràng buộc (cần cân bằng bảo mật và trải nghiệm người dùng).
```

### 7.2. Bài học rút ra

```text
Prompt tốt là prompt giúp mình hiểu nguyên lý để tự quyết định, không phải prompt xin
AI quyết định thay. "Nên dùng A hay B?" tốt hơn "Hãy làm X cho tôi".
```

---

## 8. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt giải thích kiến thức | 2 | "Giải thích Refresh Token Rotation..." |
| Prompt thiết kế giải pháp | 0 | |
| Prompt sinh code mẫu | 0 | |
| Prompt debug lỗi | 0 | |

---

## 9. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/06/2026 |
