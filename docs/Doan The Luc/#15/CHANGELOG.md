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

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-06-17T18:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-18T02:00:00.000Z                                                               |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian               | Nội dung chính                                                                                 | Trạng thái  |
| ------------------- | ----------------------- | ---------------------------------------------------------------------------------------------- | ----------- |
| Phase 01            |                         |                                                                                                | Not Started |
| Phase 02            |                         |                                                                                                | Not Started |
| Phase 03            |                         |                                                                                                | Not Started |
| Phase 04            |                         |                                                                                                | Not Started |
| Phase 05            |                         |                                                                                                | Not Started |
| Phase 06            | 2026-05-23 ~ 2026-05-23 | Secure Authentication Refactoring & Super Admin Enhancements                                   | Completed   |
| Phase 07            | 2026-05-28 ~ 2026-05-28 | Reclaim Ownership OTP Verification & Identity Normalization                                    | Completed   |
| Phase 08            | 2026-05-29 ~ 2026-05-29 | Components System Visual Explorer & Workspace Architecture                                     | Completed   |
| Phase 09            | 2026-05-30 ~ 2026-05-30 | Secure OAuth Integration & Settings Change Password Overhaul                                   | Completed   |
| Phase 10            | 2026-05-31 ~ 2026-05-31 | Email Normalization Correction, Multi-Email Support & Password Recovery Overhaul               | Completed   |
| Phase 11            | 2026-05-31 ~ 2026-05-31 | Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation             | Completed   |
| Phase 12            | 2026-06-01 ~ 2026-06-01 | Account Deletion Lifecycle & Modular Monolith Transition                                       | Completed   |
| Phase 13            | 2026-06-02 ~ 2026-06-02 | Automatic Username System & Public Profile Routing                                             | Completed   |
| Phase 14            | 2026-06-03 ~ 2026-06-03 | Persisting Avatar Source, Re-engineering Experience/Achievements Settings & Form Consistency   | Completed   |
| Phase 15            | 2026-06-05 ~ 2026-06-06 | Repository Analysis Engine with Real-time SSE Progress Streaming                               | Completed   |
| Phase 16            | 2026-06-15 ~ 2026-06-16 | AI CV Assessment, Source-Code Provider Integrations, and Session Inactivity Management         | Completed   |
| Phase 17            | 2026-06-17 ~ 2026-06-18 | Candidate Assessment SSE Streaming, Repository Reset, and Dashboard UI Enhancements           | Completed   |

---

# [Phase 17]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-17 ~ 2026-06-18
- **Mô tả giai đoạn:** Candidate Assessment SSE Streaming, Repository Reset, and Dashboard UI Enhancements
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Thêm endpoint phát triển `v1/candidate-assessments/dev-trigger` để kích hoạt nhanh quy trình đánh giá của ứng viên cho việc phát triển và thử nghiệm cục bộ. | Đoàn Thế Lực | CandidateAssessmentController.cs | GitHub Commit |
|   2 | Bổ sung API đặt lại trạng thái phân tích kho lưu trữ `/repositories/{id}/reset` cho phép phân tích lại từ đầu. | Đoàn Thế Lực | RepositoryAnalysisController.cs, IRepositoryAnalysisService.cs | GitHub Commit |
|   3 | Phát triển phương thức gọi API `resetAnalysis` tích hợp phía client frontend để đặt lại trạng thái phân tích. | Đoàn Thế Lực | repository-analysis.service.ts | GitHub Commit |
|   4 | Tích hợp Zustand store quản lý hàng đợi công việc `use-analysis-job-store.ts` để theo dõi tiến độ phân tích kho lưu trữ. | Đoàn Thế Lực | client/src/app/(private)/settings/components/repository-analysis/stores/ | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Tích hợp quy trình truyền phát sự kiện tiến độ phân tích (SSE Progress Streaming) giữa Python AI pipeline và backend .NET Core với xử lý backfill. | Đoàn Thế Lực | orchestrate_stream.py, composer.py | GitHub Commit |
|   2 | Chuẩn hóa quy trình phân tích và chuẩn hóa số liệu đóng góp Git Metrics của ứng viên để xử lý các schema rỗng hoặc thiếu thông tin email/commits. | Đoàn Thế Lực | repository-analysis.service.ts | GitHub Commit |
|   3 | Nâng cấp trải nghiệm nút `"Open A4 Preview"` chuyển hướng thẳng tới màn hình Live Preview tích hợp trong form thay vì mở popup modal. | Đoàn Thế Lực | client/src/app/(private)/cv/page.tsx | GitHub Commit |
|   4 | Cải tiến trang cài đặt liên kết nguồn mã nguồn với tính năng giám sát tiến độ trực quan và hàng đợi công việc. | Đoàn Thế Lực | settings/source-code-providers/page.tsx, CareerTab.tsx | GitHub Commit |

### Fixed

| STT | Nội dung sửa lỗi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ---------------- | --------------- | --------------------- | ---------- |
|   1 | Khắc phục lỗi căn chỉnh biểu đồ điểm tổng quan của ứng viên (`ProgressCircle`) bị text ghi đè bằng cách ép kích thước track SVG lấp đầy vùng chứa `w-24 h-24`. | Đoàn Thế Lực | AiAssessmentTab.tsx | GitHub Commit |
|   2 | Sửa lỗi căn lề và định dạng danh mục hiển thị độ xác thực của các dự án liên kết. | Đoàn Thế Lực | AiAssessmentTab.tsx | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(vetting): integrate candidate assessment streaming, repository analysis reset, and dashboard UI improvements | https://github.com/Kaivian/CVerify/commit/aee8cce8ce5ea737de7b6a3a4d7db83b924c68a8 |

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Hoàn thiện luồng đồng bộ SSE truyền phát tiến độ và điểm số đánh giá thời gian thực từ AI pipeline về Dashboard.
- Sửa lỗi căn chỉnh vòng tròn điểm số tổng quan bị đè chữ trong digital CV.
- Tích hợp tính năng xem trước CV thời gian thực trực tiếp từ nút bấm Open A4 Preview của giao diện quản trị.
- Bổ sung APIs và giao diện đặt lại trạng thái phân tích, giúp người dùng linh hoạt kích hoạt lại tiến độ phân tích kho lưu trữ.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Chưa hỗ trợ giao diện hủy tiến trình phân tích đang chạy (cancel active analysis job) trực tiếp từ frontend.
```

---

### 4.3. Cải thiện chính

```text
- Tiến trình đánh giá của ứng viên được tối ưu hóa hiển thị thông qua cơ chế SSE mượt mà, phản hồi ngay lập tức khi có thay đổi từ AI pipeline.
- Giao diện CV Management liền mạch hơn nhờ tích hợp chế độ Live Preview trực quan thay cho modal tĩnh.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này mang lại sự đồng bộ toàn diện về trải nghiệm người dùng và khả năng vận hành của hệ thống: từ việc theo dõi chi tiết hàng đợi công việc phân tích, cơ chế đặt lại trạng thái phân tích linh hoạt, đến việc hoàn thiện các biểu đồ trực quan trong CV số của ứng viên, đưa CVerify tiến gần hơn tới trạng thái sẵn sàng thương mại hóa.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Bổ sung nút Hủy tiến trình phân tích trên giao diện hàng đợi công việc.
2. Tối ưu hóa cơ chế cache các số liệu phân tích Git Metrics để giảm tải cho backend API.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-18    |
