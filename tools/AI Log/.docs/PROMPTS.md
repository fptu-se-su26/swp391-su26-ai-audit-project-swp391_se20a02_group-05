# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software development project |
| Mã môn học | SWP391 |
| Lớp | SE20A06 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm |  |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | Trương Văn Hiếu |
| Ngày bắt đầu | 2026-05-14T16:21:03.872Z |
| Ngày cập nhật gần nhất | 2026-05-14 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
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
| 1 | 2026-05-14 | Gemini | hiết kế cấu trúc dữ liệu JSON cho lộ trình du lịch và lấy danh sách địa điểm thực tế tại Đà Nẵng | Tôi muốn đi Đà Lạt 3 ngày budg... | AI trả về một đối tượng JSON c... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-14 |
| Công cụ AI | Gemini |
| Mục đích | hiết kế cấu trúc dữ liệu JSON cho lộ trình du lịch và lấy danh sách địa điểm thực tế tại Đà Nẵng |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Tôi muốn đi Đà Lạt 3 ngày budget 5 triệu thích chill. Trả về duy nhất định dạng JSON gồm các trường: day, activity, location, estimated_cost
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp vai trò là "Travel Planner AI", giới hạn ngân sách (5 triệu), địa điểm (Đà Lạt), thời gian (3 ngày) và phong cách mong muốn (chill)
```

#### 5.3. Kết quả AI trả về

```text
AI trả về một đối tượng JSON chứa lộ trình chi tiết từng ngày, mỗi hoạt động đi kèm địa điểm và chi phí dự kiến phù hợp với ngân sách
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm sử dụng cấu trúc các trường dữ liệu (fields) để thiết kế Database và sử dụng các địa điểm gợi ý làm dữ liệu mẫu cho hệ thống
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm đã kiểm tra lại tính xác thực của các quán cafe chill trên thực tế và điều chỉnh lại chi phí (estimated_cost) cho chính xác với giá thị trường hiện tại
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
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

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
Tôi muốn đi Đà Lạt 3 ngày budget 5 triệu thích chill. Trả về duy nhất định dạng JSON gồm các trường: day, activity, location, estimated_cost
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này cực kỳ quan trọng vì nó giúp nhóm định hình được cấu trúc dữ liệu JSON chuẩn cho tính năng cốt lõi của ứng dụng (lên lộ trình du lịch). Thay vì mất cả buổi để tự ngồi liệt kê địa điểm và chi phí, AI đã thực hiện việc này trong vài giây, giúp tăng tốc độ làm việc của nhóm lên rất nhiều
```

### 6.3. Kết quả prompt này mang lại

```text
AI trả về một đối tượng JSON chứa lộ trình chi tiết từng ngày, mỗi hoạt động đi kèm địa điểm và chi phí dự kiến phù hợp với ngân sách
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Nhóm sử dụng cấu trúc các trường dữ liệu (fields) để thiết kế Database và sử dụng các địa điểm gợi ý làm dữ liệu mẫu cho hệ thống
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Nhóm đã kiểm tra lại tính xác thực của các quán cafe chill trên thực tế và điều chỉnh lại chi phí (estimated_cost) cho chính xác với giá thị trường hiện tại
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
Cần cung cấp bối cảnh cụ thể (Context) như: đối tượng người dùng, ngân sách và địa điểm rõ ràng. Đặc biệt, phải yêu cầu định dạng đầu ra (Output format) cụ thể như JSON kèm theo danh sách các trường (fields) dữ liệu mong muốn để dễ dàng tích hợp vào code
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Học được rằng việc giới hạn phạm vi trả lời của AI (như câu lệnh 'chỉ trả về duy nhất JSON') là cực kỳ quan trọng để tránh việc AI giải thích dông dài, gây khó khăn cho việc xử lý dữ liệu tự động. Ngoài ra, việc đưa ra ngân sách cụ thể giúp AI lọc địa điểm thực tế hơn
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Lần tới nhóm sẽ thử áp dụng kỹ thuật 'Few-shot prompting' (đưa ra các mẫu dữ liệu chuẩn trước) để AI bắt chước chính xác hơn. Đồng thời, nhóm sẽ chia nhỏ các yêu cầu phức tạp thành nhiều prompt phụ để tối ưu hóa kết quả cho từng tính năng của ứng dụng
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
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
|   | 14/5/2026 |
