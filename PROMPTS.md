# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie AI |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-13T07:28:07.404Z |
| Ngày cập nhật gần nhất | 2026-05-14 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

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
| 1 | 2026-05-13 | Claude | Thiết kế một hệ thống AI Agent chuyên cho travel planning với khả năng:  Thu thập requirement từ user qua survey hoặc chat prompt Sinh travel itinerary hoàn chỉnh Generate structured plan dưới dạng Google Sheet Cho phép user chỉnh sửa thủ công AI tự reconcile thay đổi và regenerate plan mới Mở rộng thành ecosystem AI gồm: recommendation engine planner engine chatbot assistant validation system classification/tagging summarization  Mục tiêu thực tế:  Biến AI thành orchestration layer cho travel SaaS Tách AI layer khỏi business logic để scale dễ hơn Chuẩn hóa structured output để FE/BE xử lý deterministic thay vì parsing text tự do | * Context: Tôi muốn tạo một AI... | ... | Không |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-13 |
| Công cụ AI | Claude |
| Mục đích | Thiết kế một hệ thống AI Agent chuyên cho travel planning với khả năng:  Thu thập requirement từ user qua survey hoặc chat prompt Sinh travel itinerary hoàn chỉnh Generate structured plan dưới dạng Google Sheet Cho phép user chỉnh sửa thủ công AI tự reconcile thay đổi và regenerate plan mới Mở rộng thành ecosystem AI gồm: recommendation engine planner engine chatbot assistant validation system classification/tagging summarization  Mục tiêu thực tế:  Biến AI thành orchestration layer cho travel SaaS Tách AI layer khỏi business logic để scale dễ hơn Chuẩn hóa structured output để FE/BE xử lý deterministic thay vì parsing text tự do |
| Phần việc liên quan | Report |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
* Context: Tôi muốn tạo một AI Agent để tạo và lên plan cho khách hàng muốn lên plan đi du lịch(bao gồm chỗ đi chơi, ăn uống, nhà hàng, khách sạn). Agent sẽ đảm nhiệm vai trò lấy nhu cầu cảu khách hàng thông qua survey hoặc từ prompt input của khách hàng sau đó sẽ tạo ra một implement plan rồi gen ra 1 bảng google sheet. User/Guest có quyền chỉnh sửa các phần trong plan của AI tạo ra và sau đó Agent sẽ sửa lại theo đúng nhu cầu khách hàng.
* Yều cầu: phân tích, tìm hiểu và đề xuất giải pháp custom AI API cho hệ thống, bao gồm:
   * Lựa chọn API AI phù hợp với bài toán
   * Nghiên cứu cách hoạt động của AI API (request, prompt, response, token, streaming, function calling,…)
   * Thiết kế và custom cấu trúc response từ AI để phù hợp với nghiệp vụ hệ thống
   * Xây dựng cơ chế để front-end phân tích và xử lý response nhằm thực hiện các task cụ thể
   * Đề xuất workflow giao tiếp giữa Front-end ↔ Backend ↔ AI API
   * Thiết kế các AI task mở rộng và tích hợp vào hệ thống như:
      * AI recommendation
      * AI planner/generator
      * AI summarization
      * AI chatbot assistant
      * AI classification/tagging
      * AI validation/checking
   * Đề xuất hướng triển khai thực tế, tối ưu chi phí API, performance và scalability
   * Phân chia hướng implement AI features vào kiến trúc hệ thống hiện tại để đảm bảo khả năng mở rộng về sau.
* Yêu cầu output: đảm bảo giải thích bằng từ ngữ và tạo file json để visualize json data thành dạng graph workflow cho dễ nhìn
* Tài liệu đã nghiên cứu được đính kèm
```

#### 5.2. Bối cảnh khi viết prompt

```text
Business Context

Hệ thống là:

AI-powered travel planning platform
Có editable collaborative itinerary
Google Sheet là output chính
User có thể override AI decisions
AI phải regenerate theo feedback loop
```

#### 5.3. Kết quả AI trả về

```text
 
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

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
* Context: Tôi muốn tạo một AI Agent để tạo và lên plan cho khách hàng muốn lên plan đi du lịch(bao gồm chỗ đi chơi, ăn uống, nhà hàng, khách sạn). Agent sẽ đảm nhiệm vai trò lấy nhu cầu cảu khách hàng thông qua survey hoặc từ prompt input của khách hàng sau đó sẽ tạo ra một implement plan rồi gen ra 1 bảng google sheet. User/Guest có quyền chỉnh sửa các phần trong plan của AI tạo ra và sau đó Agent sẽ sửa lại theo đúng nhu cầu khách hàng.
* Yều cầu: phân tích, tìm hiểu và đề xuất giải pháp custom AI API cho hệ thống, bao gồm:
   * Lựa chọn API AI phù hợp với bài toán
   * Nghiên cứu cách hoạt động của AI API (request, prompt, response, token, streaming, function calling,…)
   * Thiết kế và custom cấu trúc response từ AI để phù hợp với nghiệp vụ hệ thống
   * Xây dựng cơ chế để front-end phân tích và xử lý response nhằm thực hiện các task cụ thể
   * Đề xuất workflow giao tiếp giữa Front-end ↔ Backend ↔ AI API
   * Thiết kế các AI task mở rộng và tích hợp vào hệ thống như:
      * AI recommendation
      * AI planner/generator
      * AI summarization
      * AI chatbot assistant
      * AI classification/tagging
      * AI validation/checking
   * Đề xuất hướng triển khai thực tế, tối ưu chi phí API, performance và scalability
   * Phân chia hướng implement AI features vào kiến trúc hệ thống hiện tại để đảm bảo khả năng mở rộng về sau.
* Yêu cầu output: đảm bảo giải thích bằng từ ngữ và tạo file json để visualize json data thành dạng graph workflow cho dễ nhìn
* Tài liệu đã nghiên cứu được đính kèm
```

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

```text
 
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
 
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
 
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
 
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Report | 1 |  |

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
| Nguyễn Hoàng Ngọc Ánh | 14/5/2026 |
