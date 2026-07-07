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
| Ngày cập nhật gần nhất | 2026-06-01 |

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
| 1 | 2026-05-28 | Gemini | Sửa lỗi cú pháp hình học XML (Could not add object mxGeometry) và tái cấu trúc layout ma trận cho sơ đồ tổng thể gồm 83 Use Cases (chia thành 15 nhóm chức năng) của hệ thống CVerify trên Draw.io. | "Tôi đang thực hiện import sơ ... | AI đã phân tích và phát hiện r... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-28 |
| Công cụ AI | Gemini |
| Mục đích | Sửa lỗi cú pháp hình học XML (Could not add object mxGeometry) và tái cấu trúc layout ma trận cho sơ đồ tổng thể gồm 83 Use Cases (chia thành 15 nhóm chức năng) của hệ thống CVerify trên Draw.io. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi tối ưu |

#### 5.1. Prompt nguyên văn

```text
"Tôi đang thực hiện import sơ đồ Use Case tổng thể cho hệ thống CVerify gồm 83 Use Cases chia làm 15 phân vùng chức năng vào công cụ Draw.io nhưng hệ thống liên tục crash và trả về thông báo lỗi: 'Could not add object mxGeometry'. 

Dưới đây là danh sách chi tiết 83 Use Cases (từ UC-01 đến UC-83), sơ đồ phân bổ 15 nhóm chức năng (Group 01 -> Group 15), danh sách các Actor tương ứng (Guest, User, Business User, Admin, Super Admin, Developer, AI Microservice, System) cùng bảng thống kê mối quan hệ <<include>> và <<extend>> đi kèm.

Hãy phân tích mã lỗi cú pháp XML này, sửa lại toàn bộ cấu trúc hình học, tính toán lại ma trận tọa độ X, Y tự động cho các thẻ <mxGeometry> và xuất lại cho tôi một đoạn mã nguồn XML sạch 100%, bao bọc các Use Cases gọn gàng trong các container swimlane để tôi import thành công vào Draw.io."
```

#### 5.2. Bối cảnh khi viết prompt

```text
Gửi ảnh chụp màn hình thông báo lỗi crash của Draw.io, danh sách phân rã 83 Use Cases (từ UC-01 đến UC-83) chia làm 15 nhóm chức năng, cùng bảng phân bố Actor (Guest, User, Business User, Admin, Super Admin, Developer, AI Microservice, System) và các mối quan hệ <<include>>/<<extend>> đi kèm.
```

#### 5.3. Kết quả AI trả về

```text
AI đã phân tích và phát hiện ra lỗi thiếu/sai lệch cấu trúc thẻ hình học <mxGeometry> trong file XML gốc. AI đã phản hồi bằng cách generate lại toàn bộ một đoạn mã XML sạch 100%, tự động tính toán ma trận tọa độ X, Y, bao bọc 83 Use Cases gọn gàng vào trong 15 container swimlane trực quan giúp Draw.io kết xuất đồ họa thành công.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Sử dụng 100% đoạn mã nguồn XML mới do AI cung cấp để import trực tiếp vào mục Extras -> Edit Diagram trên công cụ Draw.io.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Không cần chỉnh sửa sâu về mặt cấu trúc XML. Chỉ thực hiện căn chỉnh kéo thả lại vị trí của một số Actor hệ thống (như Admin, Super Admin, AI Microservice) trên canvas của Draw.io để các đường mũi tên liên kết quan hệ nhìn thoáng và đẹp mắt hơn.
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
"Tôi đang thực hiện import sơ đồ Use Case tổng thể cho hệ thống CVerify gồm 83 Use Cases chia làm 15 phân vùng chức năng vào công cụ Draw.io nhưng hệ thống liên tục crash và trả về thông báo lỗi: 'Could not add object mxGeometry'. 

Dưới đây là danh sách chi tiết 83 Use Cases (từ UC-01 đến UC-83), sơ đồ phân bổ 15 nhóm chức năng (Group 01 -> Group 15), danh sách các Actor tương ứng (Guest, User, Business User, Admin, Super Admin, Developer, AI Microservice, System) cùng bảng thống kê mối quan hệ <<include>> và <<extend>> đi kèm.

Hãy phân tích mã lỗi cú pháp XML này, sửa lại toàn bộ cấu trúc hình học, tính toán lại ma trận tọa độ X, Y tự động cho các thẻ <mxGeometry> và xuất lại cho tôi một đoạn mã nguồn XML sạch 100%, bao bọc các Use Cases gọn gàng trong các container swimlane để tôi import thành công vào Draw.io."
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này đóng vai trò then chốt vì nó giải quyết trực tiếp lỗi crash hệ thống (Could not add object mxGeometry) khi import sơ đồ có quy mô lớn lên đến 83 Use Cases. Tác động lớn nhất là giúp tiết kiệm hơn 80% thời gian thiết kế; thay vì phải ngồi vẽ tay và căn chỉnh thủ công từng block chức năng trong số 15 nhóm, AI đã tính toán tự động toàn bộ ma trận tọa độ X, Y. Quy trình làm việc được tối ưu hóa hoàn toàn, giúp tiến độ của Phase 02 được hoàn thành đúng hạn mà không bị tắc nghẽn ở khâu kết xuất đồ họa.
```

### 6.3. Kết quả prompt này mang lại

```text
AI đã phân tích và phát hiện ra lỗi thiếu/sai lệch cấu trúc thẻ hình học <mxGeometry> trong file XML gốc. AI đã phản hồi bằng cách generate lại toàn bộ một đoạn mã XML sạch 100%, tự động tính toán ma trận tọa độ X, Y, bao bọc 83 Use Cases gọn gàng vào trong 15 container swimlane trực quan giúp Draw.io kết xuất đồ họa thành công.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Sử dụng 100% đoạn mã nguồn XML mới do AI cung cấp để import trực tiếp vào mục Extras -> Edit Diagram trên công cụ Draw.io.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Không cần chỉnh sửa sâu về mặt cấu trúc XML. Chỉ thực hiện căn chỉnh kéo thả lại vị trí của một số Actor hệ thống (như Admin, Super Admin, AI Microservice) trên canvas của Draw.io để các đường mũi tên liên kết quan hệ nhìn thoáng và đẹp mắt hơn.
```

---

## 7. Prompt chưa hiệu quả

### 7.1. Prompt chưa hiệu quả

```text
"Tôi đang thực hiện import sơ đồ Use Case tổng thể cho hệ thống CVerify gồm 83 Use Cases chia làm 15 phân vùng chức năng vào công cụ Draw.io nhưng hệ thống liên tục crash và trả về thông báo lỗi: 'Could not add object mxGeometry'. 

Dưới đây là danh sách chi tiết 83 Use Cases (từ UC-01 đến UC-83), sơ đồ phân bổ 15 nhóm chức năng (Group 01 -> Group 15), danh sách các Actor tương ứng (Guest, User, Business User, Admin, Super Admin, Developer, AI Microservice, System) cùng bảng thống kê mối quan hệ <<include>> và <<extend>> đi kèm.

Hãy phân tích mã lỗi cú pháp XML này, sửa lại toàn bộ cấu trúc hình học, tính toán lại ma trận tọa độ X, Y tự động cho các thẻ <mxGeometry> và xuất lại cho tôi một đoạn mã nguồn XML sạch 100%, bao bọc các Use Cases gọn gàng trong các container swimlane để tôi import thành công vào Draw.io."
```

### 7.2. Vì sao prompt này chưa hiệu quả?

```text
Câu lệnh ban đầu quá ngắn gọn, mang tính chất giải tỏa cảm xúc cá nhân và hoàn toàn thiếu hụt các thông tin bối cảnh kỹ thuật cần thiết. Do không cung cấp danh sách phân rã Use Case, thông tin về các Actor hay bảng đặc tả mối quan hệ hệ thống, AI không có đủ dữ liệu đầu vào để phân tích cấu trúc thẻ XML bị lỗi. Điều này khiến hệ thống không thể tự động sửa lỗi ngay từ đầu mà phải mất thêm thời gian bổ sung ngữ cảnh chi tiết ở các lượt tương tác sau.
```

### 7.3. Cách cải thiện prompt

```text
 
```

### 7.4. Prompt sau khi cải tiến

```text
"Tôi đang gặp lỗi 'Could not add object mxGeometry' khi import file XML sơ đồ Use Case hệ thống CVerify vào Draw.io. Hệ thống có tổng cộng 83 Use Cases chia làm 15 phân vùng chức năng, tương tác bởi các Actor bao gồm: Guest, User, Business User, Admin, Super Admin, Developer, AI Microservice và System. Hãy phân tích cấu trúc mã nguồn XML, sửa lại các thẻ hình học bị khuyết, tự động tính toán lại ma trận tọa độ lưới cho các container swimlane và xuất lại mã XML sạch hoàn chỉnh để tôi import thành công."
```

### 7.5. Kết quả sau khi cải tiến prompt

```text
 
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Để nhận được phản hồi chính xác và tối ưu từ AI khi xử lý các hệ thống lớn, cần cung cấp đầy đủ các thông tin bối cảnh sau:
Thông số kỹ thuật và quy mô cụ thể: Số lượng phân rã Use Case (83 UC), số lượng nhóm chức năng (15 nhóm) và danh sách chi tiết các Actor tham gia hệ thống.
Dữ liệu đầu vào nguyên bản: Mã nguồn XML hoặc đoạn code lỗi đang gặp phải thay vì chỉ mô tả chung chung.
Thông báo lỗi chi tiết: Cung cấp chính xác mã lỗi hệ thống trả về (ví dụ lỗi hình học Could not add object mxGeometry) kèm hình ảnh minh chứng để AI khoanh vùng phạm vi lỗi.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Tầm quan trọng của Context (Bối cảnh): Prompt không có bối cảnh hoặc chỉ mang tính chất giải tỏa cảm xúc cá nhân sẽ hoàn toàn vô giá trị đối với AI. Chất lượng đầu ra (Output) phụ thuộc trực tiếp vào độ chi tiết của dữ liệu đầu vào (Input).
Tính rõ ràng trong cấu trúc câu lệnh: Việc phân tách rõ ràng giữa: Mục tiêu cần đạt được -> Bối cảnh hiện tại -> Ràng buộc kỹ thuật -> Định dạng đầu ra mong muốn sẽ giúp AI hiểu đúng ý đồ ngay từ lượt tương tác đầu tiên, giảm thiểu tình trạng AI sinh mã bị "ảo giác" hoặc sai lệch cấu trúc lưới tọa độ.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Chuẩn hóa quy trình đặt câu hỏi: Sẽ áp dụng các kỹ thuật cấu trúc prompt nâng cao như Role-playing (định hình AI thành Chuyên gia phân tích hệ thống/Kỹ sư XML) và cung cấp tài liệu đặc tả (UDS) đồng bộ ngay từ đầu.
Cung cấp ngữ cảnh từng bước (Few-shot prompting): Đối với các tác vụ render sơ đồ lớn hoặc sinh cơ sở dữ liệu (Demo Database), tôi sẽ chia nhỏ cấu trúc để AI giải quyết từng phân vùng chức năng thay vì quăng toàn bộ ma trận dữ liệu vào một lượt xử lý, giúp kiểm soát chất lượng mã nguồn sạch hơn.
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
| Nguyễn Hoàng Ngọc Ánh | 1/6/2026 |
