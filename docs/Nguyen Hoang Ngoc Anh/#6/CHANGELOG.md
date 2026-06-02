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
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL |  |
| Ngày bắt đầu | 2026-06-02T01:35:15.986Z |
| Ngày hoàn thành | 2026-06-02T01:35:15.986Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 |  |  | Not Started |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Phân tích Actors & Use Cases (Bước 1 & 2)] 

## Ngày thực hiện

```text
2026-05-28
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Xác định 9 Actors (4 Primary + 5 Secondary) | Nguyễn Hoàng Ngọc Ansh |   |   |
| 2 | iệt kê 21 Use Cases cho hệ thống tuyển dụng AI | Nguyễn Hoàng Ngọc Ansh |   |   |
| 3 | Xác định số lượng UC phù hợp (~84 UC) | Nguyễn Hoàng Ngọc Ansh |   |   |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

## Commit/Screenshot minh chứng

```text
folder CVerify.drawio
```

## Ghi chú

```text
 
```

---

# [Mở rộng sang hệ thống CVerify (84 UC)] 

## Ngày thực hiện

```text
2026-05-28
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | BA Document toàn hệ thống CVerify – 84 Use Cases | Nguyễn Hoàng Ngọc Ansh |   |   |
| 2 | Tạo tài liệu BA .docx hoàn chỉnh | Nguyễn Hoàng Ngọc Ansh |   |   |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

## Commit/Screenshot minh chứng

```text
SWP Document SRS
```

## Ghi chú

```text
 
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
1. Xác định đầy đủ 9 Actors (4 Primary: Anonymous User, Candidate/Developer, Business Partner, System Admin; 5 Secondary: AI Engine, Google OAuth, Email Service, Redis, PostgreSQL)

2. Xây dựng danh sách 84 Use Cases trích xuất toàn bộ từ mã nguồn CVerify, phân nhóm vào 10 Packages nghiệp vụ

3. Phân tích 21 quan hệ <<include>>, 25 quan hệ <<extend>>, 3 nhóm Generalization theo chuẩn UML 2.x

4. Viết Use Case Specification đầy đủ cho UC26 (AI Chat Streaming) gồm: Pre-conditions, Main Flow 14 bước, 5 Alternative Flows, Post-conditions

5. Xuất tài liệu BA hoàn chỉnh (CVerify_BA_UseCaseDocument.docx) có thể dùng ngay cho Development Team

6. Hướng dẫn vẽ Use Case Diagram bằng draw.io / PlantUML / StarUML

```

---

## 4.2. Các chức năng chưa hoàn thành

```text
. Chưa vẽ Use Case Diagram trực quan (diagram hình ảnh thực tế chưa được render)

3. Chưa viết Use Case Specification chi tiết cho các UC còn lại
```

---

## 4.3. Cải thiện chính

```text
1. Phát hiện và áp dụng nguyên tắc "tránh nổ Use Case" — gộp các thao tác nhỏ (toggle, click) vào UC cấp cao hơn

2. Module hóa thành 10 Packages giúp BA vẽ diagram theo từng phần riêng biệt, tránh biểu đồ quá rối

3. Xác định UC26 (AI Chat Streaming) là UC phức tạp nhất với kiến trúc HMAC + SSE + Content Moderation — đây là điểm khác biệt lớn của CVerify so với hệ thống thông thường

4. Cơ chế SessionVersion để Force Logout (UC64) được ghi nhận là kiến trúc bảo mật hiếm gặp, enterprise-grade
```

---

## 4.4. Tổng kết project

```text
Dự án hoàn thành phân tích thiết kế hệ thống (System Analysis & Design) cho CVerify — một nền tảng kết nối kỹ thuật số kết hợp xác thực năng lực lập trình (Evidence Graph), tuyển dụng IT và lập kế hoạch du lịch, được hỗ trợ bởi AI Engine (Claude Sonnet 4.6 + Claude Haiku 4.5).

Toàn bộ 84 Use Cases được trích xuất cạn kiệt từ source code, bao gồm Backend (ASP.NET Core), Frontend (Next.js), AI Service (FastAPI/Python) và hệ thống phân quyền (permissions-registry.json).
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Sinh code PlantUML tự động cho từng Package để team vẽ diagram nhanh hơn

2. Bổ sung Use Case Specification cho tất cả UC Priority HIGH (52 UC)

3. Thêm Sequence Diagram cho UC26 (AI Chat Streaming) để lập trình viên hiểu flow HMAC + SSE

4. Tích hợp tài liệu BA vào wiki nội bộ (Confluence / Notion) kèm link tham chiếu đến từng file source cod
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 2/6/2026 |
