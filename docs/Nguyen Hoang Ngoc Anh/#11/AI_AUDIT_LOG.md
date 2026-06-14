# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu | 2026-06-13T07:30:00Z |
| Ngày hoàn thành | 2026-06-13T14:30:00Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Phát triển tính năng Cấu hình Doanh nghiệp (Business Settings) và Quản lý Thành viên/Vai trò (Role Business Management). Sử dụng AI để sinh mã API PATCH cập nhật thông tin chi tiết tổ chức, nâng cấp endpoint truy vấn danh sách thành viên hỗ trợ phân trang, lọc công khai, tải kèm thông tin tiêu đề (headline), username và avatar đã ký số (signed URLs). Đồng thời tinh chỉnh giao diện Frontend hiển thị thông tin thành viên (Members tab) và đồng bộ trạng thái qua Zustand store.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-13 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Phát triển endpoint PATCH cập nhật hồ sơ doanh nghiệp và bổ sung cơ chế phân trang, lọc public, định dạng DTO cho API lấy danh sách thành viên trong `WorkspaceController.cs`. |
| Phần việc liên quan | Coding Backend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Update PATCH endpoint in WorkspaceController to handle workspace updates for organizations. Implement central mapping to WorkspaceDetailsDto. Enhance members endpoint to support paging, search, publicOnly filtering, and include profile fields (headline, username, signed avatar) from UserProfiles."
```

#### 4.2. Kết quả AI gợi ý

```text
- API endpoint `PATCH /workspace/{organizationSlug}` cập nhật thông tin và trả về DTO đã cập nhật.
- API endpoint `GET /{organizationSlug}/members` thực hiện truy vấn kết hợp (Join) giữa bảng `OrganizationMemberships` và `UserProfiles` để lấy `Headline`, `Username`, và `AvatarUrl`.
- Cơ chế ký số avatar URL bằng `GetSignedUrlAsync` trước khi trả về danh sách thành viên.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Đoạn mã xử lý Join Entity và map dữ liệu từ DB sang `MemberDto`.
- Cấu hình phân trang `page` và `pageSize` với logic giới hạn tối đa bản ghi trả về.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Thêm cơ chế kiểm tra quyền truy cập của người dùng thực hiện yêu cầu (kiểm tra quyền xem thành viên nội bộ đối với tài khoản không phải là khách vãng lai).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | WorkspaceController.cs | Nâng cấp API Settings & Members |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI đề xuất giải pháp Join giữa Membership và Profile rất tối ưu, giải quyết triệt để vấn đề truy vấn thông tin thành viên đi kèm.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-13 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Tinh chỉnh giao diện thiết lập thông tin doanh nghiệp, đổi tên tab hiển thị từ People thành Members, cấu hình liên kết điều hướng và cập nhật Zustand store để đồng bộ hóa danh sách thành viên mới. |
| Phần việc liên quan | Coding Frontend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Refactor public workspace layout and views: change People tab to Members, update members/page.tsx to call the updated members endpoint with paging, display user headline, username, and signed avatar. Integrate Zustand store use-workspace-store.ts to handle member states."
```

#### 4.2. Kết quả AI gợi ý

```text
- Cấu trúc layout JSX mới hiển thị danh sách thành viên bằng thẻ Card và Avatar có link đến profile cá nhân.
- Zustand store action `fetchWorkspaceMembers` gọi api của workspace.service với các tham số phân trang.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Giao diện CSS và bố cục responsive hiển thị danh sách Members.
- Các hàm gọi API tương tác trong service và store.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tối ưu hóa layout hiển thị ảnh đại diện và thêm nút "Back to Dashboard" giúp doanh nghiệp quay lại màn hình quản trị nhanh chóng.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | members/page.tsx, use-workspace-store.ts | Đồng bộ giao diện danh sách thành viên |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Giao diện hiển thị rõ ràng, chuyên nghiệp và hoạt động mượt mà với Zustand store.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Ý tưởng | x |   |   |   |   |
| Phát triển ý tưởng |   | x |   |   |   |
| Review kết quả |   |   | x |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI quên xử lý trường hợp UserProfile chưa được tạo cho một số tài khoản thành viên cũ (gây null reference khi map). | Kiểm tra thử nghiệm API bị lỗi null pointer | Sử dụng DefaultIfEmpty() khi thực hiện Join bảng ở phía Backend để tránh crash. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
1. Biên dịch thành công mã nguồn Backend CVerify.Core với dotnet build.
2. Kiểm tra cập nhật thông tin doanh nghiệp qua API PATCH thành công từ trang Settings.
3. Tab Members (trước đây là People) hiển thị đầy đủ thông tin thành viên (Avatar, Tên, Headline) và hoạt động phân trang chuẩn xác.
4. Quyền Owner/Admin được kiểm soát chặt chẽ đối với các tính năng thay đổi thông tin cấu hình.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Người dùng đóng góp: Nghiệp vụ quản lý quyền hạn trong doanh nghiệp, đề xuất cấu trúc phân trang cho Members, thực hiện test tích hợp API và UI.

AI thực hiện: Sinh mã API cập nhật và lấy danh sách thành viên nâng cao, cập nhật giao diện JSX/CSS cho layout và trang thành viên.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Phát triển Cấu hình doanh nghiệp & Quản lý vai trò | Có | Commit c27c5c2424f6b2f8cf77cecbf15df0849f155d53 |
| Trương Văn Hiếu | DE190105 |  | Không |   |
| Đoàn Thế Lực | DE200523 |  | Không |   |
| Nguyễn La Hòa An | DE201043 |  | Không |   |
| Trần Nhất Long | DE200160  |  | Không |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
