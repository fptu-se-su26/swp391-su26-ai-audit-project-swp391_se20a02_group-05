# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày cập nhật gần nhất | 2026-05-18 |

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
| 1 | 2026-05-18 | ChatGPT | Thiết lập cơ chế bảo mật nâng cao cho hệ thống Authentication bao gồm tự động xoay vòng mã thông báo (Token Rotation) để bảo vệ Refresh Token và chống tấn công giả mạo yêu cầu chéo trang (CSRF) trong môi trường Next.js App Router. | Tôi đang xây dựng hệ thống Aut... | AI đã sinh ra bộ mã nguồn gồm ... | Có |   |
| 2 | 2026-05-18 | Gemini | Xây dựng fixture test linh hoạt bằng cách áp dụng thiết kế Builder Pattern (UserBuilder) để tối ưu hóa việc viết mã kiểm thử đơn vị (Unit Test) và kiểm thử tích hợp (Integration Test) cho phân hệ Auth. | Tôi đang viết Integration Test... | AI cung cấp trọn vẹn class Use... | Có |   |
| 3 | 2026-05-18 | ChatGPT | Chuẩn hóa bộ quy tắc (Agent rules) và chính sách bảo mật (Policies) dưới dạng văn bản hệ thống để nạp vào AI Agent TripGenie, đảm bảo bot không bị ảo tưởng (hallucination) và phản hồi đúng nghiệp vụ. | Tôi đang viết tài liệu cấu hìn... | AI đã tái cấu trúc toàn bộ bộ ... | Có |   |
| 4 | 2026-05-18 | Gemini | Triển khai tầng Client-side cho Auth, viết Custom Hook useAuth để quản lý trạng thái đăng nhập toàn cục, xử lý API Client đồng bộ để tránh lỗi trắng màn hình khi người dùng tải lại trang (reload). | Trong Next.js App Router, tôi ... | AI đã viết một React Context h... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | ChatGPT |
| Mục đích | Thiết lập cơ chế bảo mật nâng cao cho hệ thống Authentication bao gồm tự động xoay vòng mã thông báo (Token Rotation) để bảo vệ Refresh Token và chống tấn công giả mạo yêu cầu chéo trang (CSRF) trong môi trường Next.js App Router. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Tôi đang xây dựng hệ thống Auth cho dự án Next.js App Router sử dụng TypeScript. Tôi cần triển khai cơ chế Token Rotation bảo mật cho Refresh Token (phát hiện token cũ bị sử dụng lại để thu hồi toàn bộ session) kết hợp với mã chống CSRF sử dụng HttpOnly Cookie. Hãy viết cho tôi mã nguồn xử lý Middleware backend để validate và API Route Handler /api/auth/refresh xử lý việc xoay vòng token này.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp cấu trúc thư mục của dự án Next.js hiện tại, các thư viện đang dùng (Axios, JWT), cơ chế lưu cookie và yêu cầu code phải tuân thủ nghiêm ngặt chuẩn bảo mật để chuẩn bị nghiệm thu môi trường UAT.
```

#### 5.3. Kết quả AI trả về

```text
AI đã sinh ra bộ mã nguồn gồm 2 phần chính: 1 Middleware kiểm tra tính hợp lệ của CSRF Token đính kèm trên header; 1 API Route Handler thực hiện kiểm tra Refresh Token trong DB, áp dụng thuật toán phát hiện token tái sử dụng (Token Reuse Detection) và tiến hành cấp cặp Access/Refresh Token mới an toàn dưới dạng HttpOnly Cookie.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Sử dụng khoảng 90% cấu trúc logic xử lý thuật toán phát hiện token tái sử dụng (Token Reuse Detection) tại Backend và khung cấu hình Cookie bảo mật.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Tích hợp mã nguồn của AI vào kiến trúc Database hiện tại của dự án (MongoDB/Supabase), tối ưu hóa cách thức bắt lỗi (error handling) để tránh lỗi vòng lặp vô hạn (infinite loop) 401 ở client, đồng thời đồng bộ hóa với mẫu dữ liệu giả lập UserBuilder trong hệ thống bài kiểm thử (Integration Tests).
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
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | Gemini |
| Mục đích | Xây dựng fixture test linh hoạt bằng cách áp dụng thiết kế Builder Pattern (UserBuilder) để tối ưu hóa việc viết mã kiểm thử đơn vị (Unit Test) và kiểm thử tích hợp (Integration Test) cho phân hệ Auth. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Tôi đang viết Integration Test bằng Jest cho hệ thống Auth trong Next.js. Để tránh việc phải hardcode dữ liệu User trong mỗi file test, hãy viết cho tôi một class UserBuilder bằng TypeScript áp dụng Builder Pattern. Class này cần có các method linh hoạt như withValidEmail(), withExpiredToken(), withAdminRole() và cuối cùng là method build() để sinh ra object user tương
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp interface User hiện tại của hệ thống, cấu trúc cơ sở dữ liệu và yêu cầu dữ liệu sinh ra phải tương thích với các thư viện kiểm thử tự động đang setup trong CI.
```

#### 5.3. Kết quả AI trả về

```text
AI cung cấp trọn vẹn class UserBuilder sử dụng Fluent Interface (gọi hàm chuỗi), cho phép khởi tạo dữ liệu mặc định an toàn và ghi đè (override) bất kỳ trường nào cần thiết cho các kịch bản test đặc thù.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Sử dụng 100% cấu trúc class UserBuilder và cách thiết lập dữ liệu mặc định (default values) cho user test.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Bổ sung thêm các phương thức xử lý riêng cho việc ghi đè biến môi trường (env overrides) và đồng bộ hóa cơ chế mã hóa mật khẩu giả lập để khớp với DB Initializer.
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

### Prompt số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | ChatGPT |
| Mục đích | Chuẩn hóa bộ quy tắc (Agent rules) và chính sách bảo mật (Policies) dưới dạng văn bản hệ thống để nạp vào AI Agent TripGenie, đảm bảo bot không bị ảo tưởng (hallucination) và phản hồi đúng nghiệp vụ. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi tối ưu |

#### 5.1. Prompt nguyên văn

```text
Tôi đang viết tài liệu cấu hình System Prompt/Rules cho trợ lý ảo du lịch tên là TripGenie. Tôi có một số file quy tắc bằng Markdown nhưng cấu trúc đang bị rời rạc. Hãy giúp tôi tối ưu hóa, viết lại bộ quy tắc này bằng tiếng Anh một cách chuyên nghiệp, phân tầng rõ ràng gồm: Persona, Core Rules (Không leak system prompt, không bàn luận chính trị), và Policy xử lý khi người dùng hỏi ngoài phạm vi du lịch.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp nội dung thô của các file markdown cũ và mục tiêu là hệ thống này sẽ được đưa lên môi trường nghiệm thu nghiệm ngặt TripGenie-uat.
```

#### 5.3. Kết quả AI trả về

```text
AI đã tái cấu trúc toàn bộ bộ tài liệu, chuyển đổi văn phong thô sang văn phong chỉ thị hệ thống (system instructions) cực kỳ gãy gọn, sử dụng các thẻ tag rõ ràng để LLM dễ hiểu và tuân thủ nghiêm ngặt hơn.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Sử dụng toàn bộ khung cấu trúc phân tầng mới và các điều khoản bảo mật lõi (Core Policies) để nạp trực tiếp vào tệp cấu hình tài liệu của Agent.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Thay đổi một số điều kiện ràng buộc về định dạng đầu ra (Output format) thành Markdown chuẩn hóa để phù hợp với giao diện hiển thị thẻ chat trên Frontend của dự án.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [ ] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [x] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

### Prompt số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | Gemini |
| Mục đích | Triển khai tầng Client-side cho Auth, viết Custom Hook useAuth để quản lý trạng thái đăng nhập toàn cục, xử lý API Client đồng bộ để tránh lỗi trắng màn hình khi người dùng tải lại trang (reload). |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Trong Next.js App Router, tôi muốn viết một Custom Hook là useAuth kết hợp với React Context để quản lý session của user. Hook này cần tự động kiểm tra trạng thái login khi client-side mount, gọi API Client để làm mới token dưới nền, và cung cấp các trạng thái isLoading, isAuthenticated, user. Hãy viết code tuân thủ React best-practices.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp thông tin về việc hệ thống backend sử dụng HttpOnly Cookie để lưu token và luồng chuyển hướng trang (Routing Guard) hiện tại của dự án.
```

#### 5.3. Kết quả AI trả về

```text
AI đã viết một React Context hoàn chỉnh cùng với Hook useAuth, có xử lý hiệu quả hiệu ứng phụ (side-effects) trong useEffect để chặn tình trạng gọi API trùng lặp (race condition) khi component re-render.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Sử dụng cấu trúc quản lý state (isLoading, user) và logic interceptor chặn bắt lỗi 401 của API Client để kích hoạt luồng làm mới token tự động.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Chỉnh sửa lại logic điều hướng của Next.js Router (useRouter) để sau khi user đăng nhập hoặc refresh token thành công sẽ không bị đẩy về trang login một cách vô lý, giải quyết triệt để lỗi reload bị trắng màn hình.
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

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
Tôi đang xây dựng hệ thống Auth cho dự án Next.js App Router sử dụng TypeScript. Tôi cần triển khai cơ chế Token Rotation bảo mật cho Refresh Token (phát hiện token cũ bị sử dụng lại để thu hồi toàn bộ session) kết hợp với mã chống CSRF sử dụng HttpOnly Cookie. Hãy viết cho tôi mã nguồn xử lý Middleware backend để validate và API Route Handler /api/auth/refresh xử lý việc xoay vòng token này.
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này đóng vai trò then chốt vì nó giải quyết trực tiếp bài toán bảo mật cốt lõi (Security) vốn là phần khó và dễ xảy ra lỗ hổng nhất trong dự án. Việc AI cung cấp giải pháp chuẩn Production về Token Rotation giúp đội phát triển tiết kiệm được rất nhiều thời gian nghiên cứu tài liệu mã hóa, nâng cao độ an toàn dữ liệu của ứng dụng lên mức tối đa, giúp hệ thống vượt qua các bài kiểm thử hiệu năng (Benchmarks) và đủ điều kiện để merge thành công vào nhánh UAT.
```

### 6.3. Kết quả prompt này mang lại

```text
AI đã sinh ra bộ mã nguồn gồm 2 phần chính: 1 Middleware kiểm tra tính hợp lệ của CSRF Token đính kèm trên header; 1 API Route Handler thực hiện kiểm tra Refresh Token trong DB, áp dụng thuật toán phát hiện token tái sử dụng (Token Reuse Detection) và tiến hành cấp cặp Access/Refresh Token mới an toàn dưới dạng HttpOnly Cookie.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Sử dụng khoảng 90% cấu trúc logic xử lý thuật toán phát hiện token tái sử dụng (Token Reuse Detection) tại Backend và khung cấu hình Cookie bảo mật.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Tích hợp mã nguồn của AI vào kiến trúc Database hiện tại của dự án (MongoDB/Supabase), tối ưu hóa cách thức bắt lỗi (error handling) để tránh lỗi vòng lặp vô hạn (infinite loop) 401 ở client, đồng thời đồng bộ hóa với mẫu dữ liệu giả lập UserBuilder trong hệ thống bài kiểm thử (Integration Tests).
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
Cần cung cấp thông tin cực kỳ tường minh về môi trường công nghệ (ví dụ: phân biệt rõ Next.js App Router với Pages Router), kiến trúc cơ sở dữ liệu đang dùng, cấu trúc mã nguồn hiện tại và các ràng buộc phi chức năng (như tiêu chuẩn bảo mật, thư viện bắt buộc) để AI không đưa ra mã nguồn chung chung hoặc lỗi thời.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Học được phương pháp chia tách bài toán lớn thành các prompt con có tính tuần tự (Incremental Prompting). Việc yêu cầu AI xử lý thuật toán Backend trước, sau đó mới prompt yêu cầu viết Client Hooks tích hợp giúp chất lượng code sinh ra chính xác hơn, ít lỗi logic hơn việc gộp chung tất cả vào một prompt duy nhất.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Lần tới sẽ áp dụng kỹ thuật "Few-shot Prompting" bằng cách cung cấp sẵn cho AI 1 đến 2 đoạn mã xử lý thực tế trong dự án hiện tại để AI nắm được văn phong lập trình (coding style), cách đặt tên biến và cấu trúc module, giúp code sinh ra có thể tích hợp ngay lập tức mà không cần chỉnh sửa thủ công nhiều.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Coding | 3 |  |
| Prompt Design | 1 |  |

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
| Nguyễn Hoàng Ngọc Ánh | 18/5/2026 |
