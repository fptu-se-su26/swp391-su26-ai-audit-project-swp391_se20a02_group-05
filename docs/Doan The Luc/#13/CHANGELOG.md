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
| Tên bài tập / Project | CVerify - Repository Analysis Engine with Real-time SSE Progress Streaming             |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/Kaivian/CVerify                                                     |
| Ngày bắt đầu          | 2026-06-05T19:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-06T03:00:00.000Z                                                               |

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

---

# [Phase 15]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-06-05 ~ 2026-06-06
- **Mô tả giai đoạn:** Repository Analysis Engine with Real-time SSE Progress Streaming
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

### Added

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Thêm các thực thể cơ sở dữ liệu `AnalysisJob`, `AnalysisJobEvent` và `AnalysisReport` quản lý lịch sử và sự kiện phân tích. | Đoàn Thế Lực | AnalysisJob.cs, AnalysisJobEvent.cs, AnalysisReport.cs | GitHub Commit |
|   2 | Triển khai worker chạy ngầm `BackgroundRepositoryAnalysisProcessor` để kéo các tác vụ phân tích từ hàng đợi Redis/Memory. | Đoàn Thế Lực | BackgroundRepositoryAnalysisProcessor.cs | GitHub Commit |
|   3 | Xây dựng bộ quét dọn phục hồi hàng đợi `AnalysisQueueRecoverySweeper` xử lý các tác vụ bị treo do hệ thống khởi động lại đột ngột. | Đoàn Thế Lực | AnalysisQueueRecoverySweeper.cs | GitHub Commit |
|   4 | Xây dựng endpoint SSE `GET /api/v1/analysis/orchestrate/stream` để truyền dữ liệu trạng thái phân tích thời gian thực. | Đoàn Thế Lực | RepositoryAnalysisController.cs | GitHub Commit |
|   5 | Phát triển API AI Microservice `POST /api/v1/analysis/orchestrate/stream` điều phối quá trình phân tích đa bước. | Đoàn Thế Lực | app/routes/analysis_router.py | GitHub Commit |
|   6 | Thiết lập module bóc tách công nghệ `technology_detector.py` và lấy mẫu code `code_sampler.py` để tối ưu hóa context gửi AI. | Đoàn Thế Lực | technology_detector.py, code_sampler.py | GitHub Commit |
|   7 | Viết unit tests `DecryptTokensTest.cs` kiểm nghiệm giải mã token nguồn khóa từ DB an toàn. | Đoàn Thế Lực | DecryptTokensTest.cs | GitHub Commit |

### Changed

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ----------------- | --------------- | --------------------- | ---------- |
|   1 | Chuyển đổi cơ chế gọi luồng con clone Git trên Windows sang `asyncio.to_thread` kết hợp `subprocess.run` đồng bộ. | Đoàn Thế Lực | github_analysis_orchestrator.py | GitHub Commit |
|   2 | Refactor API Claude Service sử dụng bộ phân tích cú pháp brace-matching bóc tách chuỗi JSON thô từ LLM. | Đoàn Thế Lực | claude_service.py | GitHub Commit |
|   3 | Refactor trang quản lý nhà cung cấp mã nguồn `page.tsx` ở frontend để tích hợp EventSource listener hiển thị tiến trình. | Đoàn Thế Lực | client/src/app/(private)/settings/source-code-providers/page.tsx | GitHub Commit |
|   4 | Nâng cấp API Service của client hỗ trợ đăng ký nhận sự kiện SSE thời gian thực từ Backend. | Đoàn Thế Lực | repository-analysis.service.ts | GitHub Commit |

### Fixed

| STT | Nội dung sửa lỗi | Người thực hiện | File/Module liên quan | Minh chứng |
| --: | ---------------- | --------------- | --------------------- | ---------- |
|   1 | Khắc phục lỗi reset UI ở frontend bằng cách gom nhóm chính xác các trạng thái trung gian SSE vào cờ `"analyzing"`. | Đoàn Thế Lực | client/src/app/(private)/settings/source-code-providers/page.tsx | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(analysis): implement repository analysis engine with real-time SSE progress streaming | https://github.com/Kaivian/CVerify/commit/09a81b3cc30e9d6d37df90209df32ab1b54a7df2 |

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Hoàn thành cơ chế hàng đợi xử lý ngầm (Background Worker) và khôi phục tác vụ phân tích mã nguồn ở Backend C#.
- Phát triển luồng điều phối đa bước (orchestrator stream) kết xuất tiến trình real-time sử dụng SSE trên FastAPI.
- Xây dựng giao diện điều khiển, hiển thị console logs, progress bar, và dashboard hiển thị kết quả báo cáo ở Frontend.
- Sửa lỗi tương thích subprocess trên hệ điều hành Windows và giải quyết xung đột phân tích cú pháp JSON từ Claude.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Khả năng phân tích sâu (Deep static analysis) cho các tệp nhị phân hoặc mã nguồn quá lớn (vượt quá giới hạn context window của Claude).
```

---

### 4.3. Cải thiện chính

```text
- Trải nghiệm người dùng cực kỳ trực quan khi theo dõi tiến trình phân tích trực tiếp tương tự như công cụ CI/CD.
- Kiến trúc hàng đợi xử lý tách biệt giúp Backend chính không bị nghẽn tài nguyên khi phân tích các kho lưu trữ lớn.
```

---

### 4.4. Tổng kết project

```text
Giai đoạn này hoàn thành một trong những tính năng cốt lõi của CVerify - Khả năng tự động phân tích và đánh giá chất lượng/bảo mật của kho lưu trữ nguồn mở hoặc đóng thông qua công nghệ AI, hỗ trợ đầy đủ luồng tương tác thời gian thực SSE.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Bổ sung bộ nhớ đệm (Caching) báo cáo phân tích cho các commit giống nhau để tiết kiệm chi phí gọi Claude API.
2. Tích hợp hỗ trợ thêm các nhà cung cấp mã nguồn khác như GitLab và Bitbucket.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-06    |
