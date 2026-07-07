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
| Tên bài tập / Project | CVerify - Reclaim Organization Ownership                                               |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Repository URL        | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05   |
| Ngày bắt đầu          | 2026-05-28T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-28T23:59:59.000Z                                                               |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian               | Nội dung chính                                               | Trạng thái  |
| ------------------- | ----------------------- | ------------------------------------------------------------ | ----------- |
| Phase 01            |                         |                                                              | Not Started |
| Phase 02            |                         |                                                              | Not Started |
| Phase 03            |                         |                                                              | Not Started |
| Phase 04            |                         |                                                              | Not Started |
| Phase 05            |                         |                                                              | Not Started |
| Phase 06            | 2026-05-23 ~ 2026-05-23 | Secure Authentication Refactoring & Super Admin Enhancements | Completed   |
| Phase 07            | 2026-05-28 ~ 2026-05-28 | Reclaim Ownership OTP Verification & Identity Normalization  | Completed   |

---

# [Phase 07]

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-05-28 ~ 2026-05-28
- **Mô tả giai đoạn:** Reclaim Ownership OTP Verification & Identity Normalization
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi                                                                                                                                                                                                               | Người thực hiện | File/Module liên quan                             | Minh chứng    |
| --: | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------- | ------------------------------------------------- | ------------- |
|   1 | Standardized Identity Normalization layer: Created `NormalizeEmail` and `NormalizeTaxCode` in `RecoveryTokenHelper.cs` to handle trim, lowercasing, and domain-scoped Gmail subaddress parsing (stripping dots and '+' tags).   | Đoàn Thế Lực    | RecoveryTokenHelper.cs                            | GitHub Commit |
|   2 | Enforced Normalized Verification: Updated `OrganizationReclaimService.cs` and `RecoveryService.cs` to compare normalized emails and tax codes during claim submission, preventing token rejection due to formatting variations. | Đoàn Thế Lực    | OrganizationReclaimService.cs, RecoveryService.cs | GitHub Commit |
|   3 | Diagnostics and Logging: Enhanced mismatch error logs in the backend to display both raw and normalized parameters, simplifying UAT troubleshooting.                                                                            | Đoàn Thế Lực    | OrganizationReclaimService.cs, RecoveryService.cs | GitHub Commit |
|   4 | Wizard Token Re-usability: Modified `reclaim-view.tsx` on the client to cache verified OTP tokens in component state, allowing users to navigate back and edit details without re-triggering OTP requests.                      | Đoàn Thế Lực    | reclaim-view.tsx                                  | GitHub Commit |
|   5 | Seamless Back Navigation: Corrected "Previous" button behavior from step 4 (Documents Upload) to link back to Representative Info without destroying filled fields.                                                             | Đoàn Thế Lực    | reclaim-view.tsx                                  | GitHub Commit |
|   6 | Integration Testing suite: Added `SubmitClaim_With_Email_Normalization_Should_Succeed` in `RecoveryFlowTests.cs` to assert successful claim submission using mixed-cased, subaddressed emails.                                  | Đoàn Thế Lực    | RecoveryFlowTests.cs                              | GitHub Commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn                                             | Nội dung                                                                                                                             |
| --------------- | ------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------ |
| Commit/PR       | Enhance OTP handling, rate limits & file uploads | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/7b272853130f6c668a40df73d4aec346d6611c99 |

## Ghi chú

```text

```

---

## 4. Tổng kết thay đổi cuối project

### 4.1. Các chức năng đã hoàn thành

```text
- Trình quản lý xác thực OTP và chuẩn hóa thông tin: Xây dựng bộ chuẩn hóa email và mã số thuế chuyên dụng (RecoveryTokenHelper) giúp loại bỏ khoảng trắng, đổi thành chữ thường, loại bỏ nhãn phụ (+tag) và dấu chấm cho Gmail để đảm bảo tính duy nhất.
- Hỗ trợ lưu trữ trạng thái xác thực trong wizard đa bước: Nâng cấp Client-side ReclaimView để ghi nhớ token OTP đã xác thực thành công, cho phép người dùng chuyển đổi giữa các bước RepresentativeInfo và DocumentsUpload mà không bị bắt buộc xác thực lại từ đầu.
```

---

### 4.2. Các chức năng chưa hoàn thành

```text
- Tự động chuẩn hóa trên toàn bộ ứng dụng: Hiện tại cơ chế chuẩn hóa mới được áp dụng cho luồng Reclaim/Recovery, cần nhân rộng ra toàn bộ các luồng đăng ký (Register) và đăng nhập (Login) thông thường.
```

---

### 4.3. Cải thiện chính

```text
- Loại bỏ hoàn toàn lỗi "Email OTP Verification token is invalid or has expired" phát sinh do sự khác biệt định dạng chữ viết hoặc khoảng trắng giữa lúc đăng ký thông tin và lúc nộp tài liệu chứng minh.
```

---

### 4.4. Tổng kết project

```text
Các thay đổi cải thiện đáng kể độ tin cậy và trải nghiệm sử dụng (UX) của luồng Reclaim Organization. Bằng cách kết hợp cơ chế chuẩn hóa chặt chẽ ở backend cùng với khả năng ghi nhớ trạng thái ở frontend, ứng dụng đã đạt chuẩn chất lượng sản xuất và sẵn sàng cho môi trường UAT.
```

---

### 4.5. Hướng cải thiện tiếp theo

```text
1. Mở rộng bộ chuẩn hóa (Normalization helper) để dùng chung cho mọi thực thể User/Email trong hệ thống Auth Core của CVerify.
2. Thêm cảnh báo giao diện (UI tooltip) cho người dùng biết email của họ đã được chuẩn hóa về dạng tối giản nhằm tránh nhầm lẫn.
```

---

## 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-29    |
