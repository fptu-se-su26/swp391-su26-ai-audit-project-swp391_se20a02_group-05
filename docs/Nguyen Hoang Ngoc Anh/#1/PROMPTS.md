# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software development project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Trương Văn Hiếu, Đoàn Thế Lực, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE190105, DE200523, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-15T05:40:41.033Z |
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
| 1 | 2026-05-12 | Gemini | Clear my idea about project. I want AI suggest some fearture and support writend document | Tôi đang mong muốn hệ thống sẽ... | Đưa ra được nội dung dựa trên ... | Không |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-12 |
| Công cụ AI | Gemini |
| Mục đích | Clear my idea about project. I want AI suggest some fearture and support writend document |
| Phần việc liên quan | Report |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Tôi đang mong muốn hệ thống sẽ gợi ý các địa điểm tham quan, ăn uống, nhu cầu của khách dụ lịch thông qua việc khảo sát dự án, số ngày đi, đi bao nhiêu ngày. Thiết kế tour Bao gồm Đà Nẵng, Hội An, gợi ý các địa điểm phù hợp với hành trình. Gợi ý booking tài xế đưa đón theo suốt hành trình, đặt bàn ăn nhà hàng, khảo sát quán địa điểm ăn nhậu, cà phê cho khách du lịch follow.

Hãy giúp tôi xây dựng document cho dự án

```

#### 5.2. Bối cảnh khi viết prompt

```text

Tôi muốn có 9 part chính như sau:

1. Giới thiệu tổng quan (Introduction & Overview)

Đây là phần mở đầu để bất kỳ ai (Sếp, Dev, hay Nhà đầu tư) đọc vào cũng hiểu ngay dự án là gì.



Tên dự án: (Ví dụ: SmartTravel AI).

Mục tiêu dự án: Tóm tắt ngắn gọn trong 2-3 câu về sản phẩm.

Phạm vi dự án (Project Scope): App này làm gì và không làm gì (để tránh việc Dev làm quá đà hoặc sai hướng).

2. Bối cảnh & Lý do chọn dự án (Why we chose this project / Business Value)

Đây là phần em vừa nhắc tới. Trong thực tế, phần này giúp thuyết phục mọi người rằng dự án này "đáng làm".



Vấn đề hiện tại (Problem Statement): Người dùng mất quá nhiều thời gian để lên lịch trình, thông tin du lịch bị phân mảnh, lịch trình mẫu không cá nhân hóa.

Giải pháp (The Solution): Dùng AI để tạo lịch trình tức thì, cá nhân hóa theo túi tiền và sở thích.

Giá trị mang lại: Tiết kiệm thời gian, trải nghiệm du lịch mượt mà hơn.

3. Các vai trò trong hệ thống (User Roles & Personas)

Dựa trên hình ảnh em gửi lúc trước, chúng ta sẽ làm rõ 3 nhóm đối tượng:



Guest/User: Họ cần gì? (Tìm kiếm, tạo lịch trình, quản lý chuyến đi).

Admin: Họ cần gì? (Quản lý dữ liệu địa điểm, quản lý người dùng, xem báo cáo).

AI System (System Actor): Nó đóng vai trò gì? (Xử lý ngôn ngữ tự nhiên, gợi ý địa điểm, tối ưu hóa lộ trình).

4. Yêu cầu chức năng (Functional Requirements)

Đây là phần dài nhất và quan trọng nhất. Chúng ta sẽ chia theo các nhóm tính năng lớn (Epics):



Nhóm Quản lý tài khoản: Đăng ký, đăng nhập, hồ sơ cá nhân.

Nhóm AI Planner (Linh hồn của app): Nhập yêu cầu -> AI gen lịch trình -> Chỉnh sửa lịch trình.

Nhóm Khám phá (Discovery): Tìm kiếm địa điểm, xem đánh giá.

Nhóm Quản lý (Admin Dashboard): Thống kê, duyệt nội dung.

(Mỗi tính năng nhỏ trong này sẽ được viết dưới dạng User Story và Acceptance Criteria như anh đã ví dụ ở câu trước).

5. Luồng nghiệp vụ (Business Process / User Flow)

Dùng sơ đồ để mô tả cách người dùng đi từ lúc mở app đến khi có được lịch trình trên tay.



Ví dụ: Luồng "Tạo lịch trình" -> User chọn điểm đến -> AI xử lý -> User lưu lịch trình -> User xem bản đồ.

6. Yêu cầu phi chức năng (Non-Functional Requirements)

Phần này Tester (môn SWT của em) sẽ soi rất kỹ:



Hiệu năng: AI phải phản hồi kết quả trong vòng bao nhiêu giây? App chịu được bao nhiêu người dùng cùng lúc?

Bảo mật: Mã hóa thông tin thanh toán, bảo mật tài khoản.

Tính khả dụng (Usability): Giao diện phải thân thiện, dễ dùng trên cả mobile và desktop.

7. Giao diện người dùng (UI/UX - Wireframes)

Phần này em sẽ nhúng các link thiết kế từ Figma hoặc các hình ảnh phác thảo sơ bộ (Wireframes) để Dev dễ hình dung bố cục.



8. Lộ trình phát triển (Roadmap & Phases)

Đừng tham làm tất cả mọi thứ cùng lúc. Hãy chia giai đoạn:



Giai đoạn 1 (MVP - Minimum Viable Product): Chỉ làm tính năng tạo lịch trình bằng AI cơ bản và đăng nhập.

Giai đoạn 2: Thêm tính năng đặt vé máy bay, khách sạn, chia sẻ lịch trình với bạn bè.

9. LỜI KHUYÊN KIẾN TRÚC & CÔNG NGHỆ (DÀNH CHO TECH LEAD/DEV)
```

#### 5.3. Kết quả AI trả về

```text
Đưa ra được nội dung dựa trên yêu cầu của tôi
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
 
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Chỉnh sửa, thêm ,xóa các nội dung để phù hợp ví dụ: Tính năng , chỉnh sửa công việc thành viên, phạm vi dự án,...
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
Tôi đang mong muốn hệ thống sẽ gợi ý các địa điểm tham quan, ăn uống, nhu cầu của khách dụ lịch thông qua việc khảo sát dự án, số ngày đi, đi bao nhiêu ngày. Thiết kế tour Bao gồm Đà Nẵng, Hội An, gợi ý các địa điểm phù hợp với hành trình. Gợi ý booking tài xế đưa đón theo suốt hành trình, đặt bàn ăn nhà hàng, khảo sát quán địa điểm ăn nhậu, cà phê cho khách du lịch follow.

Hãy giúp tôi xây dựng document cho dự án

```

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

```text
Đưa ra được nội dung dựa trên yêu cầu của tôi
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
 
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Chỉnh sửa, thêm ,xóa các nội dung để phù hợp ví dụ: Tính năng , chỉnh sửa công việc thành viên, phạm vi dự án,...
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
Description detail our project
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Hiểu rõ hơn dự án
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Hoàn thiện hơn
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
| Nguyễn Hoàng Ngọc Ánh | 15/5/2026 |
