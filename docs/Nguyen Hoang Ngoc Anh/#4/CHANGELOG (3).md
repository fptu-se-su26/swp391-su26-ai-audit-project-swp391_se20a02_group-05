# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Requirement |
| Mã môn học | SWR302 |
| Lớp | SE20A06 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm |  |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | TamTTT14 |
| Repository URL |  |
| Ngày bắt đầu | 2026-05-23T02:35:09.457Z |
| Ngày hoàn thành | 2026-05-23T02:35:09.457Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 2026-05-19 | Phase 01 | Completed |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Phase 01] 

## Ngày thực hiện

```text
2026-05-19
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Xây dựng idea ý tưởng mới cho dự án môn học chủ đề áp dụng AI trong phát triển CV và tìm việc làm | Nguyễn Hoàng Ngọc Ansh |   |   |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Sử dụng AI (Gemini) để brainstorm và phát triển các ý tưởng cốt lõi cho dự án. AI đã hỗ trợ gợi ý các tính năng chính cần có, ví dụ như: hệ thống chấm điểm CV theo chuẩn ATS, thuật toán phân tích mức độ phù hợp (matching) giữa kỹ năng của ứng viên và Job Description, và tính năng tự động tạo bản nháp Cover Letter. Quá trình này giúp thu hẹp phạm vi đề tài, đánh giá tính khả thi và định hình rõ hướng đi ban đầu cho sản phẩm.
```

## Commit/Screenshot minh chứng

```text
https://docs.google.com/document/d/1ZQGdnrWGuIo-i0d8rY5gn6EARG2BQ_CSC2jjNV9H_Pc/edit?tab=t.co2eh4x7rtet#heading=h.r91qcyiy0gc
```

## Ghi chú

```text
 
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
Hệ thống Quản lý Tài khoản (Identity): Đăng ký/đăng nhập an toàn cho ứng viên và nhà tuyển dụng.

Trình tạo CV thông minh (CV Builder): Giao diện kéo thả trực quan được xây dựng bằng React và TypeScript, cho phép xuất file PDF chuẩn ATS.

AI ATS Scoring: Tính năng cốt lõi sử dụng AI để phân tích, chấm điểm CV và đưa ra gợi ý cải thiện chi tiết.

Smart Job Matching: Thuật toán phân tích mức độ phù hợp giữa kỹ năng trên CV của ứng viên và Job Description (JD).

AI Cover Letter Generator: Tự động tạo bản nháp thư xin việc dựa trên thông tin CV và JD cụ thể.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
AI Mock Interview: Tính năng phỏng vấn giả lập với AI bằng giọng nói (bị hoãn lại do giới hạn thời gian triển khai và resource của AI model).

Cổng thanh toán (Payment Gateway): Tích hợp thanh toán VNPay/Momo cho các gói tính năng Premium của người dùng.
```

---

## 4.3. Cải thiện chính

```text
Kiến trúc Hệ thống: Chuyển đổi và chuẩn hóa backend (.NET) sang mô hình Clean Architecture (với đầy đủ 3 layers tiêu chuẩn), giúp phân tách nghiệp vụ rõ ràng và dễ dàng bảo trì.

Tối ưu truy xuất dữ liệu: Áp dụng Repository Pattern giúp chuẩn hóa các thao tác với database và tăng tốc độ phản hồi của API.

Trải nghiệm người dùng (UX): Tối ưu hóa thời gian chờ khi AI xử lý phản hồi bằng cách thêm các trạng thái loading/skeleton UI trên frontend.
```

---

## 4.4. Tổng kết project

```text
Dự án đã hoàn thành thành công phiên bản MVP, đáp ứng được mục tiêu ban đầu là mang đến một công cụ thực tiễn giúp sinh viên và người tìm việc tối ưu hóa hồ sơ của mình. Quá trình phát triển diễn ra suôn sẻ nhờ sự phối hợp chặt chẽ của toàn team trong việc thống nhất Tech Stack và áp dụng các design pattern chuẩn xác, giúp giải quyết tốt các bài toán về hiệu suất khi tích hợp với các API của AI.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
Mở rộng hệ sinh thái: Phát triển thêm phiên bản Mobile App (có thể dùng React Native hoặc Unity) để tăng mức độ tiếp cận.

Nâng cấp AI: Fine-tune model AI để phân tích sâu hơn các CV thuộc các ngành nghề đặc thù (như đồ họa, kỹ thuật cơ khí).

Real-time Chat: Thêm tính năng nhắn tin trực tiếp giữa nhà tuyển dụng và ứng viên ngay trên nền tảng.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|   | 23/5/2026 |
