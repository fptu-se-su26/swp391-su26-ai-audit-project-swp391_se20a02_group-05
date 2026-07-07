# AI Audit Log

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
| Ngày hoàn thành | 2026-05-15T07:02:35.302Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Thiết kế System Architecture, xây dựng Database Schema (JSON), Debug lỗi môi trường (Node.js/Git) và khởi tạo dữ liệu mẫu cho ứng dụng GenieTrip
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Thiết kế cấu trúc dữ liệu JSON bền vững cho tính năng gợi ý lịch trình du lịch, đảm bảo tương thích với Frontend Next.js. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỗ trợ ý tưởng |

#### 4.1. Prompt đã sử dụng

```text
Tôi đang làm dự án TripGenie bằng Next.js. Hãy thiết kế một cấu trúc JSON cho lịch trình du lịch 3 ngày tại Đà Nẵng. Yêu cầu: bao gồm các trường 'id', 'day', 'time_slot' (sáng/chiều/tối), 'activity_name', 'location_coordinates', và 'estimated_cost'. Trả về dữ liệu mẫu là các địa điểm chill cho sinh viên
```

#### 4.2. Kết quả AI gợi ý

```text
AI trả về một mảng JSON chuẩn định dạng, gợi ý các địa điểm thực tế như Đỉnh Bàn Cờ, Cafe khu An Thượng và các quán ăn đặc sản với mức giá sinh viên
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Toàn bộ cấu trúc các trường (fields) và 80% dữ liệu địa điểm mẫu
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tự cập nhật lại tọa độ chính xác (coordinates) cho từng địa điểm và điều chỉnh lại chi phí dự kiến dựa trên thực tế năm 2026
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 2:27:38 PM | image.png |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Very good
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Design & Backend Data Structure |   | x |   |   | AI giúp định hình các trường dữ liệu (fields) cần thiết cho một lộ trình du lịch hoàn chỉnh |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | Dữ liệu về chi phí và địa điểm có thể bị cũ hoặc không còn hoạt động | Kiểm tra chéo với các nguồn tin thực tế từ Google Maps và các hội nhóm du lịch Đà Nẵng | Tự điều chỉnh lại mức giá và cập nhật thêm các địa điểm mới nổi đang hot hiện nay |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
- Kiểm tra tính hợp lệ của cấu trúc JSON bằng công cụ JSONLint để đảm bảo không có lỗi cú pháp trước khi đưa vào code.
- Sử dụng Postman để test thử dữ liệu mẫu (Seeding data) xem có hiển thị đúng các trường (id, day, cost...) hay không.
- Đối soát thủ công các địa điểm du lịch (ví dụ: Cafe An Thượng, Bán đảo Sơn Trà) trên Google Maps để cập nhật tọa độ và tình trạng hoạt động thực tế năm 2026.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Trực tiếp thiết kế và tối ưu hóa hệ thống Prompts cho tính năng gen lịch trình du lịch (Itinerary Generation).
- Chịu trách nhiệm quản lý mã nguồn trên GitHub Desktop, xử lý triệt để các lỗi xung đột (Merge Conflict) phát sinh khi làm việc nhóm.
- Cấu hình môi trường chạy Local (Next.js/Node.js) và thực hiện tài liệu hóa toàn bộ quá trình sử dụng AI cho nhóm.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trương Văn Hiếu	 | DE190105 | rực tiếp thiết kế và tối ưu hóa hệ thống Prompts cho tính năng gen lịch trình du lịch (Itinerary Generation). | Có |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 15/5/2026 |
