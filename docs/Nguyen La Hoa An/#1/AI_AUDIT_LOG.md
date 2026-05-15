# AI Audit Log

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
| Ngày hoàn thành | 2026-05-15T01:39:40.002Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
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
research and analyze
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Yêu cầu AI phân tích và làm rõ các yêu cầu kỹ thuật/nghiệp vụ của dự án để đảm bảo hiểu đúng phạm vi tính năng cần phát triển cho ứng dụng du lịch. |
| Phần việc liên quan | Report |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Đóng vai trò là một Business Analyst (BA), hãy phân tích các yêu cầu chức năng (Functional Requirements) và phi chức năng (Non-functional Requirements) cho ứng dụng du lịch tích hợp AI. Tập trung vào tính năng gợi ý lộ trình dựa trên thời tiết và sở thích cá nhân
```

#### 4.2. Kết quả AI gợi ý

```text
liệt kê các yêu cầu trọng tâm bao gồm: tính năng gợi ý lộ trình thông minh, tích hợp dữ liệu thời tiết thực tế, quản lý hồ sơ sở thích người dùng và cơ chế tương tác thời gian thực thông qua chatbot hướng dẫn viên. AI cũng gợi ý các ràng buộc về hiệu năng và bảo mật dữ liệu người dùng.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
trích xuất danh sách các yêu cầu chức năng (Functional Requirements) và các tiêu chí đánh giá trải nghiệm người dùng (UX Metrics) từ phản hồi của AI để đưa vào tài liệu đặc tả (SRS) của dự án.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
cụ thể hóa các yêu cầu chung của AI bằng cách gắn chúng với các công nghệ thực tế sẽ sử dụng (như Google Maps API cho bản đồ, Supabase cho cơ sở dữ liệu). Đồng thời, tôi đã loại bỏ các tính năng quá phức tạp không phù hợp với giai đoạncủa dự án để tập trung vào tính năng cốt lõi.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Qua lượt tương tác này, việc đưa ra một câu lệnh chung chung như 'explain requirements' chỉ mang lại kết quả ở mức cơ bản. Tuy nhiên, khi bổ sung thêm ngữ cảnh về đối tượng khách hàng và các ràng buộc về công nghệ, AI đã cung cấp những gợi ý rất sát với thực tế dự án. Tôi đã học được rằng việc xác định rõ vai trò cho AI là chìa khóa để có được bảng phân tích yêu cầu chất lượng.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Requirement Analysis | x |   |   |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI gợi ý các địa điểm du lịch không tồn tại hoặc đã đóng cửa lâu năm | Kiểm tra chéo (Cross-check) với dữ liệu thực tế từ Google Maps và API chính thống. | Thiết lập "Grounding" bằng cách cung cấp danh sách địa điểm cố định trong phần Context để AI chỉ được phép chọn trong đó. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
sử dụng phương pháp "Human-in-the-loop". Mọi phản hồi từ AI về logic nghiệp vụ đều được đối chiếu với tài liệu chuyên ngành du lịch và quy trình chuẩn của hệ thống Backend. Với các mã JSON do AI tạo ra, tôi đã chạy thử qua các công cụ Validate JSON để đảm bảo không có lỗi cú pháp trước khi đưa vào code chính thức.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
viết logic xử lý tại Backend bằng Java/C# và xây dựng bộ lọc để kiểm soát dữ liệu đầu ra của AI. Còn AI Đóng vai trò là một "Trợ lý ảo" giúp tra cứu nhanh các mẫu thiết kế (Design Patterns), viết các đoạn code boilerplate (như các class DTO/Entity) và gợi ý các hướng giải quyết vấn đề khi gặp lỗi logic phức tạp
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
|   | 15/5/2026 |
