# AI Audit Log

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
| Ngày hoàn thành | 2026-05-14T16:21:03.872Z |

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
 
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-14 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Thiết kế cấu trúc dữ liệu JSON cho lộ trình du lịch và lấy danh sách địa điểm thực tế tại Đà Nẵng |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỗ trợ ý tưởng |

#### 4.1. Prompt đã sử dụng

```text
Tôi cần thiết kế một ứng dụng du lịch cho sinh viên FPT Đà Nẵng. Hãy gợi ý lộ trình du lịch 3 ngày 2 đêm tại Đà Nẵng, bao gồm các địa điểm 'chill', chi phí sinh viên và trả về dưới định dạng JSON
```

#### 4.2. Kết quả AI gợi ý

```text
AI cung cấp một cấu trúc JSON hoàn chỉnh gồm các trường như day, time, location, activity, và cost. Nó cũng gợi ý các địa điểm thực tế như Bán đảo Sơn Trà, các quán cafe tại khu phố An Thượng và các món ăn đặc sản địa phương
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm đã sử dụng nguyên mẫu cấu trúc JSON để xây dựng API cho ứng dụng và sử dụng các địa điểm gợi ý làm dữ liệu mẫu (Seeding data) ban đầu
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
hóm đã điều chỉnh lại phần chi phí cho sát với thực tế hiện tại (vì dữ liệu AI có thể cũ) và bổ sung thêm các hình ảnh thực tế của địa điểm vào Database thay vì chỉ có text như AI trả về
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
| Design & Backend Data Structure |   |   | x |   | AI giúp định hình các trường dữ liệu (fields) cần thiết cho một lộ trình du lịch hoàn chỉnh |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | Dữ liệu về chi phí và địa điểm có thể bị cũ hoặc không còn hoạt động | Kiểm tra chéo với các nguồn tin thực tế từ Google Maps và các hội nhóm du lịch Đà Nẵng | Tự điều chỉnh lại mức giá và cập nhật thêm các địa điểm mới nổi đang hot hiện nay |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Em đã thực hiện kiểm tra thủ công bằng cách tra cứu giá vé và tình trạng hoạt động của các quán cafe, điểm tham quan được gợi ý. Đồng thời, em cũng kiểm tra tính hợp lệ của cấu trúc JSON bằng các công cụ online để đảm bảo không bị lỗi cú pháp khi đưa vào code.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Trong dự án này, em đóng vai trò là người nghiên cứu và tích hợp AI. Em đã trực tiếp thiết kế các câu lệnh (prompts), thực hiện kiểm thử trên Postman để lấy dữ liệu mẫu và chịu trách nhiệm chính trong việc kiểm chứng độ chính xác của nội dung trước khi bàn giao cho nhóm
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
|  |  |  | Có / Không |  |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|   | 14/5/2026 |
