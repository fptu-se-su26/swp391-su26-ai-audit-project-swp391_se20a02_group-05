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
| Ngày bắt đầu | 2026-06-14T00:00:00Z |
| Ngày hoàn thành | 2026-06-14T17:50:00Z |

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
Phát triển hoàn thiện hệ thống Trang cá nhân doanh nghiệp (Business Public Profile) bao gồm hai cấu phần chính: Đăng tin tức/thông báo (Workspace Announcements/Posts) và Đăng tuyển dụng (Job Vacancies). Sử dụng AI để thiết lập các thực thể database mới, tạo các file dịch chuyển DB (Migrations), viết các API Endpoint CRUD cho bài viết và tin tuyển tuyển dụng kèm kiểm tra phân quyền sở hữu. Đồng thời, xây dựng giao diện tương tác phía Client hỗ trợ tạo/hiển thị bài đăng, đăng ký ứng tuyển/lưu công việc và quản lý dữ liệu qua Zustand store.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-14 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Thiết lập cấu trúc thực thể `WorkspacePost` và `JobVacancy`, viết các file dịch chuyển cơ sở dữ liệu EF Core, và mở rộng API endpoints trong `WorkspaceController.cs` để hỗ trợ tạo, lấy tin bài và tin tuyển dụng. |
| Phần việc liên quan | Coding Backend & DB |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Create WorkspacePost and JobVacancy entities in CVerify.Core. Configure them in ApplicationDbContext. Add EF Core migrations to create workspace_posts and job_vacancies tables. In WorkspaceController, implement:
1. POST and GET /api/workspace/{organizationSlug}/posts to write and list organization announcements.
2. POST and GET /api/workspace/{organizationSlug}/jobs to publish and filter job listings.
3. POST /{organizationSlug}/jobs/{jobId}/apply and save endpoints.
Add proper Owner/Admin permission checks using active claims."
```

#### 4.2. Kết quả AI gợi ý

```text
- Các thực thể `WorkspacePost` (chứa `Content`, `Category`, `ImageUrls`) và `JobVacancy` (chứa `Title`, `Description`, `Requirements`, `Salary`, `Locations`, `CoverImageUrl`).
- Hai file Migration: `20260614071252_AddWorkspacePostsTable` và `20260614100549_AddJobVacanciesTable` định nghĩa bảng dữ liệu.
- Các API endpoints tương tác trong `WorkspaceController.cs` kiểm tra quyền của tài khoản doanh nghiệp sở hữu workspace trước khi cho phép ghi.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Định nghĩa schema thực thể và file Migration đi kèm.
- Logic kiểm tra token quyền hạn và ghi nhận lịch sử ứng tuyển/lưu việc làm.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Tối ưu hóa các thuộc tính mảng lưu trữ địa điểm tuyển dụng (`Locations`) và kỹ năng yêu cầu (`Skills`) trên PostgreSQL sử dụng định dạng kiểu mảng an toàn.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | WorkspacePost.cs, JobVacancy.cs, WorkspaceController.cs | Thiết lập thực thể và API ở Backend |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI sinh mã Migration chuẩn xác và đồng bộ hoàn toàn với snapshot cơ sở dữ liệu hiện tại, các API xử lý logic phân quyền chặt chẽ.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-14 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Phát triển giao diện người dùng tương tác cho trang tin tức và tuyển dụng của doanh nghiệp, viết Zustand store quản lý state và các action API tương tác. |
| Phần việc liên quan | Coding Frontend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Revamp the public workspace jobs and posts views in frontend. For posts/page.tsx, add a 'Write an Announcement' modal and list posts dynamically with media grid. For jobs/page.tsx, implement jobs list with search inputs, filters for job types and departments, detailed vacancy drawer/modal, and Apply/Save vacancy actions. Map these to Zustand store actions."
```

#### 4.2. Kết quả AI gợi ý

```text
- Component JSX danh sách bài viết hỗ trợ lưới hiển thị hình ảnh kèm modal đăng bài của đại diện doanh nghiệp.
- Giao diện tìm kiếm việc làm nâng cao hỗ trợ bộ lọc và chi tiết việc làm dạng Drawer tiện dụng.
- Zustand store quản lý state bài viết/tuyển dụng riêng biệt, bao gồm loading state và thông báo Toast khi lưu/ứng tuyển.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc giao diện layout các trang `jobs/page.tsx` và `posts/page.tsx`.
- Zustand store action `fetchPosts`, `createPost`, `fetchJobs`, `createJob`, `applyJob` và `saveJob`.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Cải tiến hiệu ứng mở rộng Drawer thông tin việc làm và giao diện hiển thị badge các phúc lợi xã hội (benefits) đi kèm tin tuyển dụng.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | posts/page.tsx, jobs/page.tsx, use-workspace-store.ts | Xây dựng UI/UX trang cá nhân doanh nghiệp |

#### 4.6. Nhận xét cá nhân/nhóm

```text
UI hiển thị rất đẹp mắt, đồng bộ tốt với các token thiết kế vanilla CSS có sẵn trong hệ thống CVerify.
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
| 1 | AI gợi ý hàm lưu bài viết có vòng lặp vô hạn `getSnapshot` khi đăng ký lắng nghe state trong Zustand. | Kiểm tra console log client báo lỗi loop call stack | Thêm cơ chế ghi nhớ (cache) kết quả `getSnapshot` hoặc tối ưu dependency array trong hook `useSyncExternalStore`. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
1. Biên dịch thành công dự án và chạy migration thành công trên cơ sở dữ liệu PostgreSQL.
2. Kiểm tra tạo bài đăng (Post) doanh nghiệp lưu thành công vào DB và tự động prepend vào danh sách hiển thị Frontend.
3. Chức năng đăng tuyển (Post a Job) hoạt động chuẩn, bộ lọc tuyển dụng lọc theo vị trí và phòng ban chuẩn xác.
4. Tương tác Ứng tuyển (Apply) và Lưu việc làm (Save) trả về phản hồi Toast tức thời và cập nhật DB tương ứng.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Người dùng đóng góp: Cung cấp nghiệp vụ đăng tin tuyển dụng, cấu trúc lưu trữ tập tin chứng chỉ/CV ứng viên, kiểm thử các luồng tương tác trên giao diện.

AI thực hiện: Cung cấp code thực thể DB, các file migration, viết API CRUD ở Backend, xây dựng layout trang hiển thị tin tức, tin tuyển dụng ở Frontend và kết nối Zustand store.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Phát triển bài đăng và tin tuyển dụng doanh nghiệp | Có | Commits 4fc92d0f6508e353de29113f94f9f21763e6e6a1, e0b7e141a3d6e3b1b4be28d78a9d74599e55bef6 |
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
| Nguyễn Hoàng Ngọc Ánh | 14/06/2026 |
