# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm |  |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | Quang |
| Ngày bắt đầu | 2026-05-15T01:39:40.001Z |
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
| 1 | 2026-05-15 | Gemini | hỏi ý tưởng về chủ đề | Hãy giữ lời giải thích ngắn gọ... | AI đã đề xuất một danh sách cá... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Gemini |
| Mục đích | hỏi ý tưởng về chủ đề |
| Phần việc liên quan | Report |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Hãy giữ lời giải thích ngắn gọn, chỉ vài câu, và đừng quá miêu tả chi tiết. Đề bài không nêu rõ cần dùng bao nhiêu câu và nên dùng phong cách nào.
```

#### 5.2. Bối cảnh khi viết prompt

```text
"Ứng dụng du lịch hướng tới đối tượng khách hàng là [Ví dụ: Gen Z thích đi phượt / Gia đình đi nghỉ dưỡng].

Dữ liệu hệ thống đang có:

Địa điểm: Danh sách 500 điểm đến tại Việt Nam (có thông tin giá vé, giờ mở cửa).

User Profile: Sở thích của người dùng hiện tại là [Ví dụ: thích ăn uống, sợ leo núi].

Thời tiết: Dự báo trời sẽ mưa vào 2 ngày tới tại điểm đến.

Nhiệm vụ của AI: Đóng vai trò là một hướng dẫn viên địa phương am hiểu sâu sắc. Khi trả lời, chỉ sử dụng dữ liệu địa điểm đã cho và kết hợp với sở thích người dùng để đưa ra gợi ý thực tế. Trả về kết quả dưới dạng văn bản tự nhiên nhưng súc tích."
```

#### 5.3. Kết quả AI trả về

```text
AI đã đề xuất một danh sách các ý tưởng về lộ trình du lịch cá nhân hóa dựa trên sở thích người dùng. Cụ thể, AI gợi ý các địa điểm tham quan ngoài trời vào buổi sáng để tránh mưa (theo dữ liệu thời tiết cung cấp) và các quán ăn địa phương phù hợp với khẩu vị người dùng, kèm theo các "Action Tags" để hệ thống tự động hiển thị bản đồ.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
sử dụng danh sách các địa điểm cụ thể và thứ tự sắp xếp lộ trình mà AI đề xuất. Đặc biệt là phần logic "ưu tiên hoạt động ngoài trời trước" dựa trên ngữ cảnh thời tiết trong phần Context.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Về kỹ thuật: Tôi đã tinh chỉnh lại prompt để ép AI trả về dữ liệu theo cấu trúc Object JSON thay vì văn bản tự do. Việc này giúp code ở Backend có thể tự động bóc tách tên địa điểm và tọa độ để hiển thị trực tiếp lên bản đồ của ứng dụng.

Về logic: Tôi đã bổ sung thêm cơ chế "Fallback". Trong trường hợp AI gợi ý một địa điểm đã đóng cửa (dựa trên dữ liệu thời gian thực của hệ thống), ứng dụng sẽ tự động yêu cầu AI thay thế bằng một địa điểm tương đương trong bán kính 2km.

Về trải nghiệm: Tôi đã rút ngắn các đoạn mô tả dài dòng của AI, chỉ giữ lại các từ khóa quan trọng (như: "view đẹp", "giá rẻ", "đặc sản") để người dùng dễ dàng lướt xem nhanh khi đang di chuyển.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [ ] Prompt có đủ bối cảnh
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
Hãy giữ lời giải thích ngắn gọn, chỉ vài câu, và đừng quá miêu tả chi tiết. Đề bài không nêu rõ cần dùng bao nhiêu câu và nên dùng phong cách nào.
```

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

```text
AI đã đề xuất một danh sách các ý tưởng về lộ trình du lịch cá nhân hóa dựa trên sở thích người dùng. Cụ thể, AI gợi ý các địa điểm tham quan ngoài trời vào buổi sáng để tránh mưa (theo dữ liệu thời tiết cung cấp) và các quán ăn địa phương phù hợp với khẩu vị người dùng, kèm theo các "Action Tags" để hệ thống tự động hiển thị bản đồ.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
sử dụng danh sách các địa điểm cụ thể và thứ tự sắp xếp lộ trình mà AI đề xuất. Đặc biệt là phần logic "ưu tiên hoạt động ngoài trời trước" dựa trên ngữ cảnh thời tiết trong phần Context.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Về kỹ thuật: Tôi đã tinh chỉnh lại prompt để ép AI trả về dữ liệu theo cấu trúc Object JSON thay vì văn bản tự do. Việc này giúp code ở Backend có thể tự động bóc tách tên địa điểm và tọa độ để hiển thị trực tiếp lên bản đồ của ứng dụng.

Về logic: Tôi đã bổ sung thêm cơ chế "Fallback". Trong trường hợp AI gợi ý một địa điểm đã đóng cửa (dựa trên dữ liệu thời gian thực của hệ thống), ứng dụng sẽ tự động yêu cầu AI thay thế bằng một địa điểm tương đương trong bán kính 2km.

Về trải nghiệm: Tôi đã rút ngắn các đoạn mô tả dài dòng của AI, chỉ giữ lại các từ khóa quan trọng (như: "view đẹp", "giá rẻ", "đặc sản") để người dùng dễ dàng lướt xem nhanh khi đang di chuyển.
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
Hãy trình bày chi tiết và cụ thể, cung cấp ví dụ hoặc hướng dẫn.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
 việc gợi ý về cơ bản là nghệ thuật giảm thiểu sự mơ hồ. Bởi vì tôi xử lý thông tin dựa trên các mẫu và xác suất, nên gợi ý càng "tốt" thì càng ít khả năng tôi lạc vào những vấn đề chung chung hoặc không liên quan.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Tôi sẽ cải thiện bằng cách tập trung vào độ chính xác và khả năng dự đoán. Vì chúng ta đã xác định rằng câu trả lời tốt hơn từ AI đến từ việc coi các câu hỏi như một bản tóm tắt dự án, mục tiêu của tôi là đáp ứng nhu cầu của bạn bằng cách chủ động hơn thay vì chỉ phản ứng thụ động.
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
|   | 15/5/2026 |
