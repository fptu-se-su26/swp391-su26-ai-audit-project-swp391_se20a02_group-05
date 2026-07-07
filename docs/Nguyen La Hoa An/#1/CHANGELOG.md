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
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm |  |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | Quang |
| Repository URL |  |
| Ngày bắt đầu | 2026-05-15T01:39:40.001Z |
| Ngày hoàn thành | 2026-05-15T01:39:40.002Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 2026-05-15 | Giai đoạn này tập trung vào việc định hình "xương sống" cho ứng dụng. Nhóm đã sử dụng AI để mở rộng tư duy về các tính năng thông minh, nhưng đã thực hiện lọc bỏ những yêu cầu không khả thi để đảm bảo tiến độ dự án. Trọng tâm của Phase 01 là đạt được sự thống nhất về cách Backend sẽ gửi Context cho AI. | In Progress |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Phase 01] 

## Ngày thực hiện

```text
2026-05-15
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Phân tích các tính năng tích hợp AI | an |   |   |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI đã hỗ trợ trong việc liệt kê các yêu cầu chức năng (Functional Requirements) cho ứng dụng du lịch, gợi ý cấu trúc dữ liệu JSON để giao tiếp giữa Backend và AI API, đồng thời giúp giải thích các khái niệm kỹ thuật mới như Function Calling và Streaming.
```

## Commit/Screenshot minh chứng

```text
This report consolidates research on how to integrate external APIs into Large Language Model (LLM)-powered AI agents, with a concrete target: a travel application that can take a user from a fuzzy idea ("a chill 4-day beach trip from Hanoi") all the way to a paid, calendar-synced booking. It covers the agent architecture, the tool-use / function-calling pattern, the catalogue of travel APIs worth integrating, a reference end-to-end workflow, and the operational concerns - state, security, latency, fallback - that decide whether an agent is a demo or a product. Revision 2 adds Chapter 8: a concrete API-key recommendation for a web app whose orchestrator is Anthropic's Claude, including a side-by-side comparison of capability, suitability, and 2026 pricing. The central design lesson, drawn from both engineering practice and from comparing five production AI travel planners, is simple: an agent must not call third-party APIs directly. It must call internal tools that wrap those APIs, behind a clear schema, with authentication, normalization, error handling, and observability owned by the backend. The LLM contributes language understanding and planning; the tools contribute reliable, auditable execution.
```

## Ghi chú

```text
Giai đoạn này tập trung vào việc định hình "xương sống" cho ứng dụng. Nhóm đã sử dụng AI để mở rộng tư duy về các tính năng thông minh, nhưng đã thực hiện lọc bỏ những yêu cầu không khả thi để đảm bảo tiến độ dự án. Trọng tâm của Phase 01 là đạt được sự thống nhất về cách Backend sẽ gửi Context cho AI.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
Chưa có thông tin.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
Chưa có thông tin.
```

---

## 4.3. Cải thiện chính

```text
Chưa có thông tin.
```

---

## 4.4. Tổng kết project

```text
Chưa có thông tin.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
Chưa có thông tin.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|   | 15/5/2026 |
