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
| Ngày cập nhật gần nhất | 2026-06-11 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
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
| 1 |  |  |  |  |  | Có / Không |  |

---

## 5. Prompt chi tiết

## 6. Prompt quan trọng nhất

```text
Chưa chọn prompt quan trọng nhất.
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
Phạm vi chức năng rõ ràng: Thay vì yêu cầu một giao diện web chung chung, việc cung cấp các yêu cầu cụ thể (ví dụ: tập trung tối đa vào các tính năng cốt lõi như danh sách thả xuống - dropdown list, kết nối cơ sở dữ liệu và loại bỏ hoàn toàn các phần như login, header, footer) giúp AI lọc bỏ nhiễu và tạo ra mã nguồn chính xác hơn.

Công nghệ cốt lõi của dự án (Tech Stack): Nêu rõ các công nghệ đang tương tác ở backend (như Java Web, Servlets, JDBC, JPA và cấu trúc database) để AI đưa ra các giải pháp Frontend đồng bộ và dễ dàng tích hợp với hệ thống hiện tại.

Các ràng buộc cụ thể: Đưa ra mệnh lệnh rõ ràng về trạng thái dự án (ví dụ: "xoá làm lại từ đầu" hoặc chỉ tập trung vào logic hiển thị dữ liệu) để AI không tạo ra các đoạn mã mẫu (template) dư thừa, không phù hợp với giai đoạn phát triển hiện tại.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Sức mạnh của "Prompt phủ định" (Negative Prompting): Việc nói rõ cho AI biết cái gì không cần làm (ví dụ: không làm login, không làm layout chung) cũng quan trọng y như việc yêu cầu những thứ cần làm. Điều này giúp tiết kiệm rất nhiều thời gian lọc và dọn dẹp code thừa.

Chia nhỏ prompt (Incremental Prompting) mang lại hiệu quả cao: Chia nhỏ giao diện Frontend thành từng phần chức năng cốt lõi để hỏi AI逐 từng bước (như xử lý dropdown list trước, hiển thị dòng dữ liệu sau) mang lại kết quả ổn định, dễ debug và chính xác hơn nhiều so với việc yêu cầu một hệ thống UI hoàn chỉnh ngay lập tức.

Thiết lập ngữ cảnh giúp giảm thiểu mơ hồ: Khi AI hiểu rõ mục tiêu chính của giai đoạn này là "ưu tiên chạy được tính năng" (Functional Readiness) hơn là chăm chút ngoại hình, nó sẽ tập trung cung cấp các đoạn code xử lý logic chuẩn xác thay vì các đoạn mã HTML/CSS rườm rà, thổi phồng.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Cung cấp cấu trúc dữ liệu mẫu (Mock Data) sớm hơn: Trong các lần gọi lệnh tới, tôi sẽ chủ động cung cấp trước cấu trúc dữ liệu JSON trả về hoặc schema của database để AI có thể map dữ liệu lên frontend và các thẻ dropdown chuẩn xác ngay từ luồng code đầu tiên.

Định hình sẵn các ràng buộc về giao diện/styling: Dù lần này tập trung vào tính năng cốt lõi, lần tới tôi sẽ đưa thêm các khung framework CSS (như Tailwind hoặc Bootstrap) hoặc quy chuẩn layout tối giản ngay từ prompt đầu để đảm bảo code vừa chạy tốt, vừa gọn gàng về mặt thẩm mỹ.

Đưa các kịch bản ngoại lệ (Edge cases) vào prompt: Tôi sẽ yêu cầu AI xử lý luôn các trạng thái lỗi giao diện (như khi dropdown trống, mất kết nối database) ngay trong quá trình sinh code ban đầu để tăng độ bền bỉ (resilience) cho Frontend.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|

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
| Nguyễn Hoàng Ngọc Ánh | 11/6/2026 |
