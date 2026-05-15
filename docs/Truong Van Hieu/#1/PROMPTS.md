# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software development project |
| Mã môn học | SWP391 |
| Lớp | SE20A06 |
| Học kỳ | SU26 |
| Tên bài tập / Project | GenieTrip |
| Tên sinh viên / Nhóm | Trương Văn Hiếu, Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE190105, DE200147	, DE200523, DE200160	, DE201043 |
| Giảng viên hướng dẫn | Lê Thiện Nhật Quang |
| Ngày bắt đầu | 2026-05-15T07:02:35.302Z |
| Ngày cập nhật gần nhất | 2026-05-15 |

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
| 1 | 2026-05-15 | Gemini | Thiết kế cấu trúc dữ liệu JSON bền vững cho tính năng gợi ý lịch trình du lịch, đảm bảo tương thích với Frontend Next.js. | Tôi đang làm dự án TripGenie b... | AI trả về một mảng JSON chuẩn ... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Gemini |
| Mục đích | Thiết kế cấu trúc dữ liệu JSON bền vững cho tính năng gợi ý lịch trình du lịch, đảm bảo tương thích với Frontend Next.js. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Tôi đang làm dự án TripGenie bằng Next.js. Hãy thiết kế một cấu trúc JSON cho lịch trình du lịch 3 ngày tại Đà Nẵng. Yêu cầu: bao gồm các trường 'id', 'day', 'time_slot' (sáng/chiều/tối), 'activity_name', 'location_coordinates', và 'estimated_cost'. Trả về dữ liệu mẫu là các địa điểm chill cho sinh viên
```

#### 5.2. Bối cảnh khi viết prompt

```text
Cung cấp thông tin về đối tượng người dùng là sinh viên FPT và stack công nghệ đang sử dụng (Next.js, Tailwind)
```

#### 5.3. Kết quả AI trả về

```text
AI trả về một mảng JSON chuẩn định dạng, gợi ý các địa điểm thực tế như Đỉnh Bàn Cờ, Cafe khu An Thượng và các quán ăn đặc sản với mức giá sinh viên
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Toàn bộ cấu trúc các trường (fields) và 80% dữ liệu địa điểm mẫu
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Tự cập nhật lại tọa độ chính xác (coordinates) cho từng địa điểm và điều chỉnh lại chi phí dự kiến dựa trên thực tế năm 2026
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
Tôi đang làm dự án TripGenie bằng Next.js. Hãy thiết kế một cấu trúc JSON cho lịch trình du lịch 3 ngày tại Đà Nẵng. Yêu cầu: bao gồm các trường 'id', 'day', 'time_slot' (sáng/chiều/tối), 'activity_name', 'location_coordinates', và 'estimated_cost'. Trả về dữ liệu mẫu là các địa điểm chill cho sinh viên
```

### 6.2. Vì sao prompt này quan trọng?

```text
Câu lệnh này đóng vai trò quyết định vì nó định hình toàn bộ luồng dữ liệu của ứng dụng. Nếu cấu trúc JSON không chuẩn từ đầu, việc code Frontend sẽ gặp rất nhiều khó khăn. Nó giúp nhóm tiết kiệm 2 ngày làm việc so với việc tự ngồi thiết kế database thủ công
```

### 6.3. Kết quả prompt này mang lại

```text
AI trả về một mảng JSON chuẩn định dạng, gợi ý các địa điểm thực tế như Đỉnh Bàn Cờ, Cafe khu An Thượng và các quán ăn đặc sản với mức giá sinh viên
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Toàn bộ cấu trúc các trường (fields) và 80% dữ liệu địa điểm mẫu
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Tự cập nhật lại tọa độ chính xác (coordinates) cho từng địa điểm và điều chỉnh lại chi phí dự kiến dựa trên thực tế năm 2026
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
Cần cung cấp rõ ràng Stack công nghệ (Next.js/Node.js), đối tượng khách hàng mục tiêu và định dạng đầu ra mong muốn (ví dụ: JSON code block) để AI không trả về văn bản thừa.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Học được kỹ thuật 'Few-shot prompting' và cách giới hạn phạm vi câu trả lời. Việc cung cấp ngữ cảnh cụ thể (Context) quan trọng hơn nhiều so với việc chỉ đặt câu hỏi ngắn
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Sẽ thử nghiệm việc sử dụng 'System Instructions' trong Google AI Studio để kiểm soát tone giọng của AI nhất quán hơn và tạo ra các bộ Prompt có thể tái sử dụng cho cả nhóm
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
| Trương Văn Hiếu | 15/5/2026 |
