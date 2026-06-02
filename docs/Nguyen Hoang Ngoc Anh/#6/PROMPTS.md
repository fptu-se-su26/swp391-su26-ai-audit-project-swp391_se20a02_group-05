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
| Ngày bắt đầu | 2026-06-02T01:35:15.986Z |
| Ngày cập nhật gần nhất | 2026-06-02 |

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
| 1 | 2026-05-28 | Other | Xác định toàn bộ Actors và Use Cases cho hệ thống web kết nối Ứng viên – Doanh nghiệp có tích hợp AI. Cần phân loại Primary/Secondary Actor và định lượng số UC phù hợp không bị vụn vặt. | "Vai trò: Hãy đóng vai là một ... | Claude xác định được 9 Actors ... | Có |   |
| 2 | 2026-05-28 | Claude | Xác định toàn bộ quan hệ include/extend/generalization cho 84UC, phân chia Packages để vẽ diagram, và viết Use Case Specification mẫu cho UC phức tạp nhất. | "Hãy dựa đúng vào danh sách 21... | Toàn bộ bảng include/extend, b... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-28 |
| Công cụ AI | Other |
| Mục đích | Xác định toàn bộ Actors và Use Cases cho hệ thống web kết nối Ứng viên – Doanh nghiệp có tích hợp AI. Cần phân loại Primary/Secondary Actor và định lượng số UC phù hợp không bị vụn vặt. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi debug |

#### 5.1. Prompt nguyên văn

```text
"Vai trò: Hãy đóng vai là một Software Architect dày dặn kinh nghiệm... Hãy bắt đầu với Bước 1 và Bước 2 trước, chờ tôi phản hồi rồi mới đi tiếp các bước sau."
```

#### 5.2. Bối cảnh khi viết prompt

```text
Mô tả đầy đủ dự án: tính năng Ứng viên (CV template, GitHub/GitLab integration, AI skill tree, job search, CV matching), tính năng Doanh nghiệp Mức 1 & Mức 2 (workspace, JD, AI ranking, advanced search), cùng các lưu ý về "tránh nổ UC" và module hóa.
```

#### 5.3. Kết quả AI trả về

```text
Claude xác định được 9 Actors (4 Primary + 5 Secondary), liệt kê demo 84 UC chia theo Actor 10 UC Ứng viên, 7 UC DN Mức 1, 4 UC DN Mức 2 bổ sung). Giải thích rõ nguyên tắc phân loại và tại sao 18–25 UC là con số lý tưởng cho hệ thống này.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Toàn bộ — bảng Actor (9 actor), danh sách 84UC theo Actor, giải thích nguyên tắc "tránh nổ UC" và nhận xét về Secondary Actor.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Hỏi thêm về việc có cần tách Actor Admin không, và xác nhận lại danh sách trước khi đi tiếp Bước 3.
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
| Ngày sử dụng | 2026-05-28 |
| Công cụ AI | Claude |
| Mục đích | Xác định toàn bộ quan hệ include/extend/generalization cho 84UC, phân chia Packages để vẽ diagram, và viết Use Case Specification mẫu cho UC phức tạp nhất. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi review |

#### 5.1. Prompt nguyên văn

```text
"Hãy dựa đúng vào danh sách 21 Use Case và các Actors ở trên để thực hiện Bước 3 và Bước 4 theo chuẩn UML... BƯỚC 3: XÁC ĐỊNH MỐI QUAN HỆ... BƯỚC 4: ĐÓNG GÓI & ĐẶC TẢ USE CASE..."
```

#### 5.2. Bối cảnh khi viết prompt

```text
Phân tích 15 include, 6 extend (kèm extension point rõ ràng), 1 cặp generalization Actor (L2→L1). Chia 6 Packages. Viết UC Specification UC21 với Main Flow 8 bước, 4 Alt Flows (AI timeout, không tìm thấy ứng viên, job post đóng, không đủ quyền).
```

#### 5.3. Kết quả AI trả về

```text
Toàn bộ bảng include/extend, bảng Package, UC Specification UC21 đầy đủ — dùng trực tiếp vào tài liệu BA.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Toàn bộ bảng include/extend, bảng Package, UC Specification UC21 đầy đủ — dùng trực tiếp vào tài liệu BA.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Giữ nguyên cấu trúc, chỉ yêu cầu không thêm UC mới để đảm bảo nhất quán với danh sách gốc.
```

#### 5.6. Đánh giá chất lượng prompt

- [ ] Prompt rõ ràng
- [ ] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [ ] Prompt tạo ra kết quả tốt
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
"Vai trò: Hãy đóng vai là một Software Architect dày dặn kinh nghiệm... Hãy bắt đầu với Bước 1 và Bước 2 trước, chờ tôi phản hồi rồi mới đi tiếp các bước sau."
```

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

```text
Claude xác định được 9 Actors (4 Primary + 5 Secondary), liệt kê demo 84 UC chia theo Actor 10 UC Ứng viên, 7 UC DN Mức 1, 4 UC DN Mức 2 bổ sung). Giải thích rõ nguyên tắc phân loại và tại sao 18–25 UC là con số lý tưởng cho hệ thống này.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Toàn bộ — bảng Actor (9 actor), danh sách 84UC theo Actor, giải thích nguyên tắc "tránh nổ UC" và nhận xét về Secondary Actor.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Hỏi thêm về việc có cần tách Actor Admin không, và xác nhận lại danh sách trước khi đi tiếp Bước 3.
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
1. Cung cấp toàn bộ mô tả nghiệp vụ ngay từ đầu, không chia nhỏ quá nhiều lần. 2. Nêu rõ ràng ràng buộc: "không thêm UC mới ngoài danh sách" — giúp AI tuân thủ scope. 3. Chỉ định cụ thể cấu trúc output mong muốn (bảng, đặc tả, package) thay vì nói chung chung "đầy đủ". 4. Kèm file tham chiếu source code giúp AI hiểu ngữ cảnh kỹ thuật sâu hơn.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
1. Prompt theo bước (Bước 1→2→3→4) giúp kiểm soát chất lượng từng phần thay vì yêu cầu tất cả cùng lúc. 2. Đặt vai trò rõ ràng ("Software Architect dày dặn kinh nghiệm") nâng cao chất lượng phân tích đáng kể. 3. Ví dụ cụ thể trong prompt (ví dụ: "đừng tạo UC Toggle project, gộp vào Quản lý hiển thị CV") định hướng AI chính xác hơn lý thuyết trừu tượng. 4. Context window có giới hạn — nên chia dữ liệu lớn (84 UC) thành nhiều paste thay vì một lần.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
1. Chuẩn bị toàn bộ UC list trước khi bắt đầu cuộc trò chuyện, tránh phải paste nhiều lần. 2. Yêu cầu AI sinh PlantUML code song song với tài liệu BA để tiết kiệm thời gian vẽ. 3. Dùng format JSON/CSV cấu trúc để truyền dữ liệu UC thay vì plain text — AI parse chính xác hơn. 4. Tách riêng prompt "Phân tích UC" và prompt "Sinh tài liệu" để mỗi output được tối ưu hơn.
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
| Nguyễn Hoàng Ngọc Ánh | 2/6/2026 |
