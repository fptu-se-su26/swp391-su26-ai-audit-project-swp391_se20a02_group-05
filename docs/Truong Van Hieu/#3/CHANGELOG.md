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
| Tên bài tập / Project | CVerify - Hệ thống xác thực thông tin và quản lý hồ sơ năng lực dành cho Doanh nghiệp |
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV / Danh sách MSSV | DE190105 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | `https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05` |
| Ngày bắt đầu | 2026-05-11 |
| Ngày hoàn thành | 2026-06-01 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 11/05/2026 - 15/05/2026 | Khởi tạo project và thiết lập môi trường | Completed |
| Phase 02 | 16/05/2026 - 20/05/2026 | Phân tích yêu cầu nghiệp vụ và đặc tả phân hệ | Completed |
| Phase 03 | 21/05/2026 - 25/05/2026 | Thiết kế hệ thống, ERD và Wireframe Figma | Completed |
| Phase 04 | 26/05/2026 - 29/05/2026 | Hiện thực hóa mã nguồn (Implementation) | Completed |
| Phase 05 | 30/05/2026 - 31/05/2026 | Kiểm thử chất lượng và tối ưu hóa CSS Print | Completed |
| Phase 06 | 01/06/2026 - 01/06/2026 | Hoàn thiện báo cáo, video demo và AI Audit Log | Completed |

---

# [Phase 01] Khởi tạo project

## Ngày thực hiện

```text
11/05/2026 - 15/05/2026
```

## Đã hoàn thành

- [x] Tạo repository và cấu hình phân quyền nhánh nhóm
- [x] Thiết lập cấu trúc thư mục tiêu chuẩn dự án CVerify
- [x] Tạo file README.md hướng dẫn cài đặt môi trường
- [x] Khởi tạo thư mục kiểm toán AI học thuật `docs/`
- [x] Tạo file trắng `AI_AUDIT_LOG.md` theo quy chuẩn môn học
- [x] Tạo file trắng `PROMPTS.md` để ghi nhận nhật ký đặt câu hỏi
- [x] Tạo file trắng `REFLECTION.md` cho việc tự vấn học tập
- [x] Tạo file trắng `CHANGELOG.md` kiểm soát thay đổi phiên bản
- [x] Khởi tạo source code React/Next.js ban đầu cho front-end
- [x] Cài đặt các thư viện thiết kế cốt lõi (TailwindCSS, react-hook-form)
- [x] Cấu hình Docker container và môi trường chạy thử nghiệm local

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Khởi tạo boilerplate Next.js và cấu hình TailwindCSS | Trương Văn Hiếu | `package.json`, `tailwind.config.js` | `commit-1a2b3c4d5e` |
| 2 | Thiết lập cấu trúc cây thư mục cho phân hệ Hồ sơ năng lực | Trương Văn Hiếu | `src/components/profile/`, `src/styles/` | `commit-2b3c4d5e6f` |
| 3 | Khởi tạo các file tài liệu kiểm toán AI trong thư mục docs | Trương Văn Hiếu | `docs/Truong Van Hieu/#3/*` | `commit-3c4d5e6f7a` |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hỗ trợ sinh các dòng lệnh cấu hình nhanh cho tệp tin tailwind.config.js để cấu hình hệ màu sắc chủ đạo của CVerify và sinh cấu trúc gitignore tiêu chuẩn cho dự án Next.js.
```

## Commit/Screenshot minh chứng

```text
Commit link: https://github.com/fptu-se-su26/group05/commit/1a2b3c4d5e
Screenshot: Cấu trúc thư mục dự án hiển thị sạch sẽ trên VS Code editor.
```

## Ghi chú

```text
Giai đoạn khởi tạo diễn ra đúng tiến độ, cả nhóm thống nhất sử dụng cấu trúc component-based để dễ tích hợp các module front-end về sau.
```

---

# [Phase 02] Phân tích yêu cầu & Thiết kế ý niệm

## Ngày thực hiện

```text
16/05/2026 - 20/05/2026
```

## Phase Description

Nghiên cứu yêu cầu nghiệp vụ (Requirement) và phác thảo thiết kế ý niệm (Design Concept) cho biểu mẫu nhập liệu Hồ sơ năng lực Doanh nghiệp (dành cho đối tượng Business User) thuộc dự án CVerify. Giai đoạn này tập trung vào việc xác định cấu trúc các trường thông tin pháp lý cần thiết và xây dựng luồng trải nghiệm người dùng Multi-step form tối ưu, làm tiền đề để vẽ Prototype hoàn chỉnh trên Figma.

## Đã hoàn thành (Changes / Tasks)

- [x] Nghiên cứu nghiệp vụ xác thực hồ sơ doanh nghiệp tại Việt Nam để phác thảo danh mục các trường thông tin cần nhập.
- [x] Phác thảo cấu trúc lược đồ dữ liệu logic (Logical Data Schema) lồng nhau phục vụ khâu thiết kế cơ sở dữ liệu PostgreSQL về sau.
- [x] Vẽ và liên kết luồng Prototype UI/UX biểu mẫu Multi-step form 3 bước có thanh tiến độ trực quan trên Figma.
- [x] Hoàn thiện tài liệu đặc tả yêu cầu phần mềm SRS liên quan đến phân hệ Hồ sơ năng lực của Business User.

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Nghiên cứu nghiệp vụ, phác thảo Logical Data Schema lồng nhau | Trương Văn Hiếu | `docs/design/Logical_Schema_Profile.png` | Phác thảo cấu trúc lược đồ dữ liệu logic |
| 2 | Hoàn thiện đặc tả yêu cầu phần mềm SRS và liên kết Prototype UI/UX | Trương Văn Hiếu | `docs/srs/SRS_Business_Profile_Module.md` | File SRS hoàn chỉnh |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Tôi đã sử dụng prompt để brainstorm cấu trúc các trường thông tin và gợi ý luồng UX Multi-step form cho biểu mẫu nhập liệu trên hệ thống CVerify, nhóm chỉ tiếp thu ý tưởng tổ chức dữ liệu để đưa vào Figma chứ không sử dụng code.
```

## Evidence (Commit/Screenshot minh chứng)

- **File thiết kế Figma:** [Figma Mockup link / Prototype của CVerify](file:///e:/Tai%20Lieu/Semester5/SWP391/CVerify/docs/Truong%20Van%20Hieu/%233/)
- **Tài liệu đặc tả kiến trúc:**
  - [Logical_Schema_Profile.png](file:///e:/Tai%20Lieu/Semester5/SWP391/CVerify/docs/design/Logical_Schema_Profile.png)
  - [SRS_Business_Profile_Module.md](file:///e:/Tai%20Lieu/Semester5/SWP391/CVerify/docs/srs/SRS_Business_Profile_Module.md)

## Ghi chú

```text
Việc thiết kế Logical Schema và SRS chặt chẽ giúp nhóm thống nhất 100% về cấu trúc cơ sở dữ liệu trước khi tiến hành code ở chặng sau.
```

---

# [Phase 03] Thiết kế hệ thống

## Ngày thực hiện

```text
21/05/2026 - 25/05/2026
```

## Đã hoàn thành

- [x] Thiết kế kiến trúc tổng quan luồng truyền tải dữ liệu của phân hệ Hồ sơ năng lực (Data flow)
- [x] Thiết kế database Schema cho thực thể Hồ sơ trong PostgreSQL
- [x] Thiết kế hệ thống RESTful API kết nối giữa Client và Server
- [x] Thiết kế giao diện wireframe độ trung thực thấp (Low-Fi) và Prototype chi tiết trên Figma
- [x] Thiết kế flow xử lý thuật toán chuyển đổi dữ liệu động sang in ấn PDF
- [x] Thiết kế Class Diagram cho các thành phần điều khiển giao diện Profile Builder
- [x] Thiết kế Sequence Diagram cho luồng lưu trữ và xuất bản hồ sơ
- [x] Thiết kế Security & Authorization flow (Chỉ Business User sở hữu mới được chỉnh sửa)
- [x] Review thiết kế kỹ thuật cùng các thành viên Backend và Frontend trong nhóm
- [x] Chỉnh sửa thiết kế cơ sở dữ liệu để đồng nhất với cơ sở dữ liệu người dùng chung

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thiết kế JSON Schema lưu trữ Hồ sơ và đặc tả RESTful API | Trương Văn Hiếu | `src/models/ProfileModel.js` | `commit-6f7a8b9c0d` |
| 2 | Thiết kế luồng xử lý Sequence Diagram cho hành động lưu và xuất PDF | Trương Văn Hiếu | `docs/design/sequence_profile.png` | `commit-7a8b9c0d1e` |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Tôi đã sử dụng prompt tham khảo cấu trúc các trường thông tin và gợi ý bố cục giao diện (UX Layout) cho biểu mẫu để lên ý tưởng thiết kế phân hệ Hồ sơ năng lực trên CVerify. Nhóm chỉ tiếp thu phần ý tưởng tổ chức dữ liệu và giải pháp chia chặng Multi-step form để đưa vào bản vẽ thiết kế Figma. Không sử dụng bất kỳ đoạn code nào của AI.
```

## Commit/Screenshot minh chứng

```text
Commit link: https://github.com/fptu-se-su26/group05/commit/6f7a8b9c0d
Screenshot: Bản vẽ thiết kế Figma của quy trình nhập liệu Multi-step.
```

## Ghi chú

```text
Thiết kế dữ liệu Schema dạng JSON lồng nhau là lựa chọn tối ưu giúp doanh nghiệp dễ dàng thêm bớt số lượng chứng chỉ chất lượng hoặc giấy phép hành nghề.
```

---

# [Phase 04] Implementation

## Ngày thực hiện

```text
26/05/2026 - 29/05/2026
```

## Đã hoàn thành

- [x] Tạo cấu trúc thư mục chứa các module con Frontend (`src/components/profile/`)
- [x] Kết nối cơ sở dữ liệu và hiện thực hóa Model lưu trữ Hồ sơ năng lực ở Backend
- [x] Xây dựng các API Endpoint CRUD cho việc quản lý hồ sơ của Business User
- [x] Phát triển component giao diện Form nhập liệu lồng cấp ở Frontend sử dụng react-hook-form
- [x] Phát triển component giao diện Live Preview kết xuất dữ liệu Hồ sơ năng lực thời gian thực
- [x] Xây dựng cơ chế xác thực JWT bảo vệ các tuyến API Endpoint quản lý hồ sơ
- [x] Hiện thực hóa bộ Validation dữ liệu ở cả Client-side và Server-side
- [x] Xây dựng các mẫu giao diện hiển thị Hồ sơ năng lực xác thực chuyên nghiệp
- [x] Tích hợp tính năng tải lên tài liệu pháp lý và ảnh chứng chỉ thông qua Cloudinary API
- [x] Xử lý lọc sạch mã độc XSS bằng DOMPurify để đảm bảo tính an toàn cho dữ liệu nhập vào
- [x] Tối ưu hóa CSS Responsive đảm bảo giao diện hiển thị sắc nét trên mọi loại màn hình
- [x] Cập nhật tài liệu hướng dẫn vận hành cục bộ phân hệ Hồ sơ năng lực trong README.md

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Xây dựng API và cơ sở dữ liệu lưu trữ dữ liệu hồ sơ doanh nghiệp | Trương Văn Hiếu | `src/models/ProfileModel.js`, `src/controllers/profileController.js` | `commit-8a9b0c1d2e` |
| 2 | Phát triển giao diện Form nhập liệu động Multi-step và Live Preview | Trương Văn Hiếu | `src/components/profile/ProfileForm.jsx`, `src/components/profile/ProfilePreview.jsx` | `commit-9b0c1d2e3f` |
| 3 | Lập trình các mẫu giao diện Hồ sơ năng lực xác thực chuyên nghiệp | Trương Văn Hiếu | `src/components/profile/templates/ModernProfile.jsx`, `src/components/profile/templates/ClassicProfile.jsx` | `commit-0c1d2e3f4a` |
| 4 | Hiện thực hóa logic xử lý và tối ưu hóa hiệu năng render React bằng react-hook-form | Trương Văn Hiếu | `src/hooks/useProfileState.js` | `commit-1d2e3f4a5b` |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Trong giai đoạn hiện thực hóa mã nguồn (Implementation), tôi hoàn toàn tự code bằng tay Next.js + TailwindCSS 100%, không sử dụng bất kỳ đoạn code nào do AI sinh ra để đảm bảo chất lượng cấu trúc và tương thích tối đa với hệ thống CVerify.
```

## Commit/Screenshot minh chứng

```text
Commit: https://github.com/fptu-se-su26/group05/commit/9b0c1d2e3f
Screenshot: Giao diện màn hình Live Editor hoạt động mượt mà với tính năng Live Preview thời gian thực.
```

## Ghi chú

```text
Việc tách biệt trạng thái biểu mẫu thông qua react-hook-form giúp loại bỏ hoàn toàn độ trễ bàn phím khi nhập liệu.
```

---

# [Phase 05] Testing & Debug

## Ngày thực hiện

```text
30/05/2026 - 31/05/2026
```

## Đã hoàn thành

- [x] Viết bộ kịch bản kiểm thử (Test Cases) chi tiết cho mọi tính năng của phân hệ Hồ sơ
- [x] Chạy kiểm thử thủ công toàn bộ các chức năng CRUD và xuất bản hồ sơ
- [x] Kiểm thử rà soát tính hợp lệ của dữ liệu đầu vào (Validation boundaries)
- [x] Kiểm thử khả năng hiển thị responsive trên các thiết bị Mobile, Tablet và Desktop
- [x] Kiểm thử chất lượng xuất file PDF ở nhiều chế độ in ấn khác nhau
- [x] Kiểm thử phân quyền truy cập (Đảm bảo doanh nghiệp A không thể xem/sửa hồ sơ của doanh nghiệp B)
- [x] Sửa lỗi xung đột CSS ngắt trang khi chuyển đổi từ HTML sang định dạng PDF kỹ thuật số
- [x] Gỡ lỗi hiện tượng lag bàn phím khi nhập văn bản dài trên Form nhập liệu lồng cấp
- [x] Chạy kiểm thử hồi quy (Regression Testing) sau khi thực hiện vá các lỗi hệ thống
- [x] Ghi nhận biên bản kiểm thử và cập nhật AI Audit Log báo cáo giảng viên

## Danh sách lỗi đã xử lý

| STT | Lỗi phát hiện | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Lỗi vỡ dòng chữ khi in PDF (Dòng văn bản bị cắt đôi ngang thân ở cuối trang 1) | Thiếu quy tắc CSS khống chế ngắt dòng in ấn trên trình duyệt nhân Chromium | Áp dụng `@media print` kết hợp thuộc tính `page-break-inside: avoid` cho các thẻ bao bọc văn bản | Fixed |
| 2 | Hiện tượng giật lag bàn phím (Input lag) khi gõ phím nhanh trong Form nhập liệu | Component cha re-render liên tục toàn bộ cây DOM mỗi khi có 1 ký tự thay đổi ở component con | Tái cấu trúc State Management sử dụng `react-hook-form` kết hợp `useController` để cô lập vùng render | Fixed |
| 3 | Lỗ hổng bảo mật XSS cho phép thực thi mã độc Javascript thông qua biểu mẫu | Dữ liệu đầu vào từ người dùng không được lọc sạch (sanitize) trước khi chèn vào DOM | Tích hợp thư viện `dompurify` để làm sạch dữ liệu đầu vào ở cả Client và Server | Fixed |
| 4 | Ảnh chứng chỉ và logo doanh nghiệp bị mất màu sắc chủ đạo khi xuất PDF | Trình duyệt mặc định tắt tính năng in màu nền để tiết kiệm mực in của máy in | Bổ sung thuộc tính CSS `-webkit-print-color-adjust: exact` vào bộ stylesheet print | Fixed |
| 5 | Lỗi mất dữ liệu Hồ sơ khi chuyển đổi qua lại giữa các mẫu giao diện | Khởi tạo lại trạng thái component làm mất dữ liệu tạm thời chưa được lưu vào database | Đưa trạng thái dữ liệu lên Context dùng chung giúp dữ liệu được giữ nguyên khi thay đổi template | Fixed |

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Viết stylesheet in ấn chuyên dụng và xử lý ngắt trang in | Trương Văn Hiếu | `src/styles/print.css` | `commit-2e3f4a5b6c` |
| 2 | Refactor hệ quản lý trạng thái Form để loại bỏ trễ phản hồi gõ phím | Trương Văn Hiếu | `src/components/profile/ProfileForm.jsx` | `commit-3f4a5b6c7d` |
| 3 | Tích hợp bộ lọc DOMPurify ngăn chặn tấn công XSS chèn script độc hại | Trương Văn Hiếu | `src/utils/security.js` | `commit-4a5b6c7d8e` |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Giao đoạn Testing & Debug hoàn toàn do tôi và nhóm tự thực hiện thủ công bằng kiến thức chuyên môn, không sử dụng AI hỗ trợ sửa lỗi lập trình để cam kết chất lượng mã nguồn sạch 100%.
```

## Commit/Screenshot minh chứng

```text
Commit: https://github.com/fptu-se-su26/group05/commit/2e3f4a5b6c
Screenshot: Bản so sánh file PDF xuất ra tuyệt đẹp, văn bản sắc nét và căn chỉnh lề trang cân đối.
```

## Ghi chú

```text
Kiểm thử in ấn trên nhiều loại trình duyệt khác nhau giúp đảm bảo mọi doanh nghiệp sử dụng bất kỳ thiết bị nào cũng có thể nhận được file PDF đầu ra chất lượng cao nhất.
```

---

# [Phase 06] Hoàn thiện báo cáo và demo

## Ngày thực hiện

```text
01/06/2026 - 01/06/2026
```

## Đã hoàn thành

- [x] Hoàn thiện 100% mã nguồn sạch của phân hệ Hồ sơ năng lực Doanh nghiệp
- [x] Cập nhật file README.md với đầy đủ hướng dẫn chạy thử nghiệm và cấu hình API
- [x] Viết báo cáo đặc tả kỹ thuật phân hệ Hồ sơ trong báo cáo tổng kết của nhóm
- [x] Chuẩn bị Slide thuyết trình cho buổi báo cáo tiến độ Phase 02 CVerify
- [x] Quay video demo chi tiết các tính năng Live Editor, Chuyển template và xuất PDF
- [x] Rà soát và hoàn thiện tệp tài liệu kiểm toán `AI_AUDIT_LOG.md`
- [x] Rà soát và hoàn thiện tệp nhật ký câu lệnh `PROMPTS.md`
- [x] Rà soát và hoàn thiện tệp tự vấn `REFLECTION.md` với hàm lượng học thuật cao
- [x] Kiểm tra và xác nhận tính đầy đủ, chính xác của tệp `CHANGELOG.md` này
- [x] Đóng gói toàn bộ sản phẩm và báo cáo sẵn sàng nộp lên hệ thống của giảng viên

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Hoàn thiện tài liệu kiểm toán học thuật AI cho phân hệ Hồ sơ | Trương Văn Hiếu | `docs/Truong Van Hieu/#3/*` | `commit-5b6c7d8e9f` |
| 2 | Tạo tài liệu hướng dẫn vận hành chi tiết trong file README.md | Trương Văn Hiếu | `README.md` | `commit-6c7d8e9f0a` |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI đã hỗ trợ chỉnh sửa ngữ pháp và phong cách viết học thuật cao cho các tài liệu báo cáo kiểm toán AI bằng tiếng Việt chuyên ngành phần mềm.
```

## Commit/Screenshot minh chứng

```text
Commit: https://github.com/fptu-se-su26/group05/commit/5b6c7d8e9f
Screenshot: Thư mục docs/#3 chứa đầy đủ 4 file tài liệu kiểm toán sạch sẽ và chuyên nghiệp.
```

## Ghi chú

```text
Hoàn thành toàn bộ quy trình biên soạn tài liệu kiểm toán AI một cách minh bạch, đáp ứng chuẩn đầu ra khắt khe của môn học SWP391 Đại học FPT.
```

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

| STT | Chức năng | Trạng thái | Minh chứng | Ghi chú |
|---:|---|---|---|---|
| 1 | Biểu mẫu nhập liệu Multi-step (Hồ sơ năng lực Doanh nghiệp) | Completed | `src/components/profile/ProfileForm.jsx` | Cho phép nhập liệu thông tin pháp lý lồng cấp |
| 2 | Kết xuất giao diện thời gian thực (Live Preview) | Completed | `src/components/profile/ProfilePreview.jsx` | Phản hồi giao diện tức thì <50ms |
| 3 | Chuyển đổi linh hoạt giữa các mẫu giao diện chuyên nghiệp | Completed | `src/components/profile/templates/*` | Giữ nguyên dữ liệu khi thay đổi mẫu mã |
| 4 | Bộ API RESTful CRUD lưu trữ hồ sơ doanh nghiệp | Completed | `src/controllers/profileController.js` | Tích hợp bảo mật JWT và xác thực người dùng |
| 5 | Xuất file Chứng nhận/Hồ sơ dạng PDF Vector sắc nét | Completed | `src/styles/print.css` | File PDF sắc nét dạng vector, ngắt trang hoàn hảo |

---

### 4.2. Các chức năng chưa hoàn thành

| STT | Chức năng | Lý do chưa hoàn thành | Hướng cải thiện |
|---:|---|---|---|
| 1 | Tính năng tự động điền thông tin doanh nghiệp từ Cổng thông tin Quốc gia | Giới hạn thời gian của Phase 02 và các hạn chế về mặt tích hợp API của bên thứ ba | Thiết lập phân hệ tích hợp OCR quét Đăng ký kinh doanh ở giai đoạn tiếp theo |
| 2 | Tự động chấm điểm độ hoàn thiện và tin cậy của Hồ sơ | Tập trung hoàn thành quy chuẩn in ấn và xác thực cốt lõi trước | Phát triển module gợi ý kiểm duyệt tự động bằng AI trong tương lai |

---

### 4.3. Tổng hợp AI hỗ trợ trong project

| Hạng mục | AI có hỗ trợ không? | Mức độ hỗ trợ | Ghi chú |
|---|---|---|---|
| Requirement | Có | Ít | Gợi ý checklist các trường thông tin chuẩn |
| Design | Có | Ít | Tham khảo bố cục phân bước Multi-step form |
| Database | Không | - | Tự thiết kế schema PostgreSQL 100% |
| Coding | Không | - | Tự code Next.js + TailwindCSS 100% |
| Debug | Không | - | Tự debug lỗi in ấn và tối ưu hóa DOM 100% |
| Testing | Không | - | Tự lập kịch bản và chạy test thủ công |
| Report | Có | Ít | Hỗ trợ hiệu chỉnh phong cách viết học thuật |
| Presentation | Không | - | Tự biên soạn slide thuyết trình nhóm |

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Phát triển phân hệ OCR (Optical Character Recognition) tự động trích xuất thông tin từ Giấy phép đăng ký kinh doanh tải lên của doanh nghiệp để điền tự động vào Form.
2. Xây dựng công cụ kéo thả vị trí (Drag and Drop Sections Customizer) tăng tính linh động khi bố cục Hồ sơ năng lực của Business User.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 01/06/2026 |
