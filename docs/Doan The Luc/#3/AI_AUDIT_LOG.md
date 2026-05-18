# AI Audit Log

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
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [ ] Claude
- [x] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Code Generation (Backend Auth, Frontend Hooks, API Clients), Prompt Engineering & System Instructions Design (TripGenie Agent rules), Automation Test Generation (Unit/Integration tests, UserBuilder pattern), and CI/CD configuration optimization (Vercel deploy best practices).
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Thiết lập cơ chế bảo mật nâng cao cho hệ thống Authentication bao gồm tự động xoay vòng mã thông báo (Token Rotation) để bảo vệ Refresh Token và chống tấn công giả mạo yêu cầu chéo trang (CSRF) trong môi trường Next.js App Router. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Tôi đang xây dựng hệ thống Auth cho dự án Next.js App Router sử dụng TypeScript. Tôi cần triển khai cơ chế Token Rotation bảo mật cho Refresh Token (phát hiện token cũ bị sử dụng lại để thu hồi toàn bộ session) kết hợp với mã chống CSRF sử dụng HttpOnly Cookie. Hãy viết cho tôi mã nguồn xử lý Middleware backend để validate và API Route Handler /api/auth/refresh xử lý việc xoay vòng token này.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đã sinh ra bộ mã nguồn gồm 2 phần chính: 1 Middleware kiểm tra tính hợp lệ của CSRF Token đính kèm trên header; 1 API Route Handler thực hiện kiểm tra Refresh Token trong DB, áp dụng thuật toán phát hiện token tái sử dụng (Token Reuse Detection) và tiến hành cấp cặp Access/Refresh Token mới an toàn dưới dạng HttpOnly Cookie.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sử dụng khoảng 90% cấu trúc logic xử lý thuật toán phát hiện token tái sử dụng (Token Reuse Detection) tại Backend và khung cấu hình Cookie bảo mật.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tích hợp mã nguồn của AI vào kiến trúc Database hiện tại của dự án (MongoDB/Supabase), tối ưu hóa cách thức bắt lỗi (error handling) để tránh lỗi vòng lặp vô hạn (infinite loop) 401 ở client, đồng thời đồng bộ hóa với mẫu dữ liệu giả lập UserBuilder trong hệ thống bài kiểm thử (Integration Tests).
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
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Xây dựng fixture test linh hoạt bằng cách áp dụng thiết kế Builder Pattern (UserBuilder) để tối ưu hóa việc viết mã kiểm thử đơn vị (Unit Test) và kiểm thử tích hợp (Integration Test) cho phân hệ Auth. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Tôi đang viết Integration Test bằng Jest cho hệ thống Auth trong Next.js. Để tránh việc phải hardcode dữ liệu User trong mỗi file test, hãy viết cho tôi một class UserBuilder bằng TypeScript áp dụng Builder Pattern. Class này cần có các method linh hoạt như withValidEmail(), withExpiredToken(), withAdminRole() và cuối cùng là method build() để sinh ra object user tương
```

#### 4.2. Kết quả AI gợi ý

```text
AI cung cấp trọn vẹn class UserBuilder sử dụng Fluent Interface (gọi hàm chuỗi), cho phép khởi tạo dữ liệu mặc định an toàn và ghi đè (override) bất kỳ trường nào cần thiết cho các kịch bản test đặc thù.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sử dụng 100% cấu trúc class UserBuilder và cách thiết lập dữ liệu mặc định (default values) cho user test.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Bổ sung thêm các phương thức xử lý riêng cho việc ghi đè biến môi trường (env overrides) và đồng bộ hóa cơ chế mã hóa mật khẩu giả lập để khớp với DB Initializer.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

### Lần sử dụng AI số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Chuẩn hóa bộ quy tắc (Agent rules) và chính sách bảo mật (Policies) dưới dạng văn bản hệ thống để nạp vào AI Agent TripGenie, đảm bảo bot không bị ảo tưởng (hallucination) và phản hồi đúng nghiệp vụ. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Tôi đang viết tài liệu cấu hình System Prompt/Rules cho trợ lý ảo du lịch tên là TripGenie. Tôi có một số file quy tắc bằng Markdown nhưng cấu trúc đang bị rời rạc. Hãy giúp tôi tối ưu hóa, viết lại bộ quy tắc này bằng tiếng Anh một cách chuyên nghiệp, phân tầng rõ ràng gồm: Persona, Core Rules (Không leak system prompt, không bàn luận chính trị), và Policy xử lý khi người dùng hỏi ngoài phạm vi du lịch.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đã tái cấu trúc toàn bộ bộ tài liệu, chuyển đổi văn phong thô sang văn phong chỉ thị hệ thống (system instructions) cực kỳ gãy gọn, sử dụng các thẻ tag rõ ràng để LLM dễ hiểu và tuân thủ nghiêm ngặt hơn.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sử dụng toàn bộ khung cấu trúc phân tầng mới và các điều khoản bảo mật lõi (Core Policies) để nạp trực tiếp vào tệp cấu hình tài liệu của Agent.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Thay đổi một số điều kiện ràng buộc về định dạng đầu ra (Output format) thành Markdown chuẩn hóa để phù hợp với giao diện hiển thị thẻ chat trên Frontend của dự án.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

### Lần sử dụng AI số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-18 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Triển khai tầng Client-side cho Auth, viết Custom Hook useAuth để quản lý trạng thái đăng nhập toàn cục, xử lý API Client đồng bộ để tránh lỗi trắng màn hình khi người dùng tải lại trang (reload). |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Trong Next.js App Router, tôi muốn viết một Custom Hook là useAuth kết hợp với React Context để quản lý session của user. Hook này cần tự động kiểm tra trạng thái login khi client-side mount, gọi API Client để làm mới token dưới nền, và cung cấp các trạng thái isLoading, isAuthenticated, user. Hãy viết code tuân thủ React best-practices.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đã viết một React Context hoàn chỉnh cùng với Hook useAuth, có xử lý hiệu quả hiệu ứng phụ (side-effects) trong useEffect để chặn tình trạng gọi API trùng lặp (race condition) khi component re-render.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sử dụng cấu trúc quản lý state (isLoading, user) và logic interceptor chặn bắt lỗi 401 của API Client để kích hoạt luồng làm mới token tự động.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Chỉnh sửa lại logic điều hướng của Next.js Router (useRouter) để sau khi user đăng nhập hoặc refresh token thành công sẽ không bị đẩy về trang login một cách vô lý, giải quyết triệt để lỗi reload bị trắng màn hình.
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
| Code Backend (Auth & Security) |   |   | x |   | Triển khai Token Rotation, CSRF Protection middleware và API Routes phục vụ luồng bảo mật hệ thống. |
| Code Frontend (UI & Hooks) |   | x |   |   | Xây dựng Custom Hook useAuth, Context API và đồng bộ hóa trạng thái ứng dụng chống lỗi trắng màn hình khi reload. |
| Software Testing |   | x |   |   | Sinh cấu trúc lớp UserBuilder linh hoạt và hỗ trợ viết các kịch bản Integration Test, đo chỉ số Benchmarks. |
| AI Prompt Engineering |   |   |   | x | Tái cấu trúc, tối ưu hóa và chuẩn hóa tài liệu System Rules/Policies cho TripGenie AI Agent. |
| DevOps & Deployment |   | x |   |   | Cấu hình tệp loại trừ .gitignore, client env và tối ưu hóa kịch bản tự động hóa deploy qua Vercel. |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | Mã nguồn Token Rotation do AI sinh ra bị lỗi vòng lặp vô hạn (Infinite Loop 401) ở Client khi Refresh Token hết hạn hoàn toàn. | Phát hiện khi chạy thử nghiệm thực tế (Manual Testing) trên trình duyệt, tab Network của Developer Tools gọi API liên tục không dừng. | Chủ động chỉnh sửa lại logic Middleware và cập nhật Axios Interceptor phía Client để hủy ngay request, đồng thời ép điều hướng về trang Login khi nhận mã 401 vĩnh viễn. |
| 2 | Class UserBuilder do AI sinh ra bị lệch kiểu dữ liệu (TypeScript Type Mismatch) so với schema thực tế của database trong dự án. | Trình biên dịch TypeScript (tsc) báo lỗi compile đỏ lòm khi chạy lệnh kiểm thử npm run test. | Thực hiện ép kiểu thủ công (Type casting) và import chính xác các mẫu Interface chuẩn từ Models của Database vào file test. |
| 3 | Tài liệu Agent Rules tiếng Anh do AI viết quá dài dòng, chứa nhiều từ nối hoa mỹ khiến LLM phản hồi chậm (High Latency) do tốn nhiều context tokens. | Thử nghiệm hội thoại thực tế và đo lường thời gian phản hồi (Response Time) trên môi trường nghiệm thu TripGenie-uat. | Cô đọng lại các câu lệnh chỉ thị bằng từ khóa định dạng Markdown (dạng bảng, danh sách gạch đầu dòng), loại bỏ các câu từ mô tả rườm rà không cần thiết. |
| 4 | Luồng điều hướng Router phía Frontend do AI viết sử dụng các hàm client-side cũ, không tương thích với cơ chế Next.js App Router mới. | Trình duyệt crash và báo lỗi Runtime Error liên quan đến việc sử dụng sai thư viện import "next/router". | Thay thế toàn bộ lệnh import sang "next/navigation" và bao bọc các logic điều hướng trang vào trong khối hook useEffect. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Chạy hệ thống kiểm thử tự động toàn diện (npm run test) bao gồm cả Unit test và Integration test để xác minh tính đúng đắn của logic.\n2. Chạy lệnh Build hệ thống (npm run build) để đảm bảo không xảy ra lỗi xung đột kiểu dữ liệu TypeScript.\n3. Tiến hành kiểm thử thủ công kiểm tra luồng xoay vòng token thực tế trên trình duyệt.\n4. Thực hiện các kịch bản bẻ khóa thử nghiệm (Prompt Injection) để đánh giá mức độ tuân thủ quy tắc bảo mật của TripGenie Agent.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Tôi giữ vai trò chủ trì thiết kế phân hệ Kỹ thuật bảo mật và cấu hình Hạ tầng AI Agent của nhóm. Tôi đã trực tiếp thiết kế các câu lệnh prompt (Prompt Engineering) để khai thác giải pháp từ AI, sau đó tự tay tái cấu trúc (refactor), sửa các lỗi bất tương thích mã nguồn để tích hợp mượt mà vào cấu trúc Next.js App Router của dự án. Tôi cũng là người chịu trách nhiệm thiết lập suite kiểm thử tự động để chứng minh tính ổn định của hệ thống trước khi thực hiện merge mã nguồn lên môi trường UAT.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Đoàn Thế Lực | DE200523 | Implement Auth System | Có | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/pull/22/commits |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Review and merge code | Không | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/pull/22/commits |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 18/5/2026 |
