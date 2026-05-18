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
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 |  |  | Not Started |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 | 2026-05-18 | "Giai đoạn này đã giải quyết triệt để khối lượng công việc Full-stack bao gồm: Cài đặt hệ thống bảo mật Auth nâng cao (Google Login, Token Rotation, CSRF), chuẩn hóa bộ quy tắc vận hành cho TripGenie AI Agent, đồng thời bao phủ đầy đủ Unit/Integration Test và chạy nghiệm thu UAT." | In Progress |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Phase 04] 

## Ngày thực hiện

```text
2026-05-18
```

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Bổ sung hạ tầng gửi email, thiết lập các luồng xác thực tài khoản (auth flows) và cấu hình CI tự động. | Đoàn Thế Lực |   | Commit / PR Link |
| 2 | Viết mã kiểm thử tích hợp (auth integration tests), kiểm thử đơn vị (unit tests) và chạy đo hiệu năng (benchmarks) cho phân hệ auth. | Đoàn Thế Lực |   | Commit / PR Link |
| 3 | Cấu hình bộ khởi tạo cơ sở dữ liệu (DB Initializer), thiết lập các endpoint kiểm tra trạng thái hệ thống (System Health) và sửa các lỗi liên quan đến test. | Đoàn Thế Lực |   | Commit / PR Link |
| 4 | Áp dụng các quy chuẩn React best-practices để tối ưu frontend và cấu hình kỹ năng deploy dự án lên Vercel. | Đoàn Thế Lực |   | Commit / PR Link |
| 5 | Xây dựng và định nghĩa tập hợp các quy tắc ứng xử (agent rules) và chính sách (policies) cho trợ lý ảo TripGenie. | Đoàn Thế Lực |   | Commit / PR Link |
| 6 | Chuẩn hóa lại cấu trúc tài liệu quy tắc của TripGenie agent (gỡ bỏ các file markdown cũ và cập nhật bộ tài liệu quy tắc mới). | Đoàn Thế Lực |   | Commit / PR Link |
| 7 | Thực hiện gộp các pull request (#18, #20) nhánh tính năng AI (feature/ai-enhancement) và hoàn thiện tích hợp lên môi trường nghiệm thu (TripGenie-uat qua PR #19, #21). | Đoàn Thế Lực |   | Commit / PR Link |
| 8 | Mở rộng phần kiểm thử Auth: Triển khai mẫu thiết kế UserBuilder để tạo dữ liệu test, cấu hình ghi đè biến môi trường (env overrides) và các liên kết kiểm thử. | Đoàn Thế Lực |   | Commit / PR Link |
| 9 | Tăng cường bảo mật hệ thống Auth: Tích hợp đăng nhập bằng Google (Google login), cơ chế tự động xoay vòng mã thông báo (Token Rotation) và chống tấn công giả mạo CSRF. | Đoàn Thế Lực |   | Commit / PR Link |
| 10 | Hiện thực hóa giao diện người dùng (Auth UI), xây dựng các custom hooks, API client và cài đặt các thư viện phụ thuộc (dependencies) liên quan cho luồng Auth. | Đoàn Thế Lực |   | Commit / PR Link |
| 11 | Bổ sung các tệp tài liệu hướng dẫn (READMEs), thiết lập biến môi trường phía client (client env) và cập nhật cấu hình loại trừ của .gitignore. | Đoàn Thế Lực |   | Commit / PR Link |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | Commits phát triển hệ thống Auth (17 - 18/05/2026) | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/pull/22/commits |

## Ghi chú

```text
"Giai đoạn này đã giải quyết triệt để khối lượng công việc Full-stack bao gồm: Cài đặt hệ thống bảo mật Auth nâng cao (Google Login, Token Rotation, CSRF), chuẩn hóa bộ quy tắc vận hành cho TripGenie AI Agent, đồng thời bao phủ đầy đủ Unit/Integration Test và chạy nghiệm thu UAT."
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
- Hệ thống Xác thực & Bảo mật Toàn diện (Full-stack Authentication): Tích hợp đăng nhập bằng Google (Google OAuth2), hoàn thiện giao diện Auth UI, Custom Hooks quản lý trạng thái đăng nhập và API Client. Triển khai cơ chế bảo mật nâng cao gồm Token Rotation (xoay vòng mã thông báo) và chống tấn công giả mạo CSRF.

- Trợ lý ảo TripGenie AI Agent: Định nghĩa bộ quy tắc vận hành (Agent rules), chính sách nghiệp vụ (Policies), gỡ bỏ tài liệu cũ để chuẩn hóa hệ thống prompt/knowledge. Đã tích hợp và kiểm thử thành công trên môi trường nghiệm thu TripGenie-uat.

- Hạ tầng lõi & Kiểm thử: Cấu hình hạ tầng gửi Email tự động, bộ khởi tạo dữ liệu DB Initializer, và endpoint theo dõi trạng thái hệ thống System Health Check.

- Tự động hóa CI/CD: Thiết lập thành công luồng CI tự động và tích hợp công cụ deploy liên tục lên nền tảng Vercel theo chuẩn React best-practices.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
- Giao diện quản lý phân quyền chi tiết (Granular RBAC UI): Phần logic bảo mật tuyến đường (Route Guard) đã xong, nhưng giao diện trực quan để Admin cấu hình, gán quyền chi tiết cho từng tài khoản người dùng đang trong quá trình hoàn thiện.

- Dashboard phân tích lịch sử TripGenie (AI Analytics Dashboard): Hệ thống lưu trữ log của AI Agent đã chạy trên UAT, nhưng màn hình trực quan hóa dữ liệu (biểu đồ thống kê tần suất gọi bot, tỷ lệ hài lòng của user) chưa được bao phủ hoàn toàn trong phase này.
```

---

## 4.3. Cải thiện chính

```text
- Nâng cấp Kiến trúc Bảo mật: Chuyển đổi thành công từ luồng Auth cơ bản sang mô hình bảo mật chuẩn Production với cơ chế Token Rotation, giảm thiểu tối đa rủi ro lộ mã Refresh Token và ngăn chặn CSRF.

- Nâng cao Chất lượng và Hiệu năng Code: Áp dụng nghiêm ngặt các quy chuẩn React best-practices để tối ưu hóa hiệu năng render phía Frontend. Tách biệt rõ ràng biến môi trường hệ thống (client env) và cập nhật bộ lọc .gitignore để bảo mật source code.

- Quy trình Kiểm thử Chuẩn hóa (Testing Rigor): Tối ưu hóa bộ mã nguồn test thông qua thiết kế pattern UserBuilder (linh hoạt tạo dữ liệu giả lập). Bổ sung kiểm thử tích hợp (Integration tests), kiểm thử đơn vị (Unit tests) và chạy Benchmarks để cam kết hiệu năng cho luồng Auth dưới áp lực tải lớn.
```

---

## 4.4. Tổng kết project

```text
"Giai đoạn này đánh dấu bước chuyển mình quan trọng của dự án từ một sản phẩm prototype sang mô hình Production-ready. Hệ thống đã thiết lập xong nền móng hạ tầng vững chắc bao gồm cơ chế bảo mật nghiêm ngặt (Google Auth, Token Rotation, CSRF) kết hợp với năng lực xử lý thông minh từ lõi TripGenie AI Agent. Việc áp dụng quy trình kiểm thử tự động (Unit/Integration/Benchmark), bàn giao sản phẩm lên môi trường kiểm thử người dùng (UAT)."
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
- Tích hợp Hệ thống Giám sát & Cảnh báo (Telemetry & Monitoring): Cài đặt các công cụ như Sentry hoặc Prometheus để chủ động phát hiện sớm các bất thường trong luồng xoay vòng token (Token Rotation Anomalies) hoặc lỗi timeout của AI Agent.

- Tối ưu hóa Chi phí & Tốc độ AI (LLM Caching): Triển khai cơ chế lưu bộ nhớ đệm (như Redis Cache) cho các câu hỏi trùng lặp gửi đến TripGenie Agent nhằm giảm thiểu độ trễ (latency) và tiết kiệm chi phí gọi API của mô hình ngôn ngữ lớn.

- Mở rộng năng lực Agent bằng Advanced RAG: Nâng cấp tài liệu quy tắc của TripGenie từ file tĩnh sang hệ thống cơ sở dữ liệu vector (Vector Database) để Agent có khả năng truy xuất thông tin động thông minh và chính xác hơn khi dự án mở rộng quy mô.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 18/5/2026 |
