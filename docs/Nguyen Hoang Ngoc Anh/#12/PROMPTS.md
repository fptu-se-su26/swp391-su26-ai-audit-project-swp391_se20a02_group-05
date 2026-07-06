# Hệ thống prompt sử dụng

## 1. Quy định ghi nhận Prompt

File này ghi lại danh sách các câu lệnh (prompts) đã gửi cho AI trong quá trình thực hiện bài tập, lab, assignment hoặc project để giảng viên kiểm tra mức độ tự chủ và hiệu quả sử dụng AI.

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
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05.git |
| Ngày bắt đầu | 2026-06-14T00:00:00Z |
| Ngày hoàn thành | 2026-06-14T17:50:00Z |

---

## 3. Nhật ký Prompt

### Prompt số 1: Khởi tạo thực thể DB và Migration cho bài đăng và tuyển dụng
*   **Mục đích**: Sinh cấu trúc các thực thể `WorkspacePost`, `JobVacancy` và tạo các file Migration tương ứng cho EF Core.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Generate C# entity classes WorkspacePost.cs (columns: Content, Category, ImageUrls) and JobVacancy.cs (columns: Title, Description, Requirements, Benefits, Salary, Locations, Skills, CoverImageUrl) in the shared folder. Set up entity mapping inside ApplicationDbContext.cs and generate Entity Framework migrations to create workspace_posts and job_vacancies tables.
    ```
*   **Kết quả phản hồi**:
    ```csharp
    // JobVacancy.cs
    public class JobVacancy {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        // ... other properties
    }
    // ApplicationDbContext.cs configurations and migration structures.
    ```
*   **Mức độ áp dụng**: Áp dụng trực tiếp vào cấu trúc Domain Entities và tạo file Migration của dự án.

---

### Prompt số 2: Viết các API Endpoints tương tác tin tuyển dụng và thông báo bài viết
*   **Mục đích**: Phát triển các API CRUD cho bài viết, tin bài tuyển dụng có kèm kiểm tra xác thực quyền sở hữu doanh nghiệp.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    In WorkspaceController.cs, add POST/GET endpoints for workspace posts (under route '/posts') and job vacancies (under route '/jobs'). Ensure only the owner or admin of the organization can post, and public visitors can only retrieve/get posts or jobs. Also write the apply and save endpoints for jobs.
    ```
*   **Kết quả phản hồi**:
    ```csharp
    [HttpPost("{organizationSlug}/posts")]
    [Authorize]
    public async Task<IActionResult> CreatePost(string organizationSlug, [FromBody] CreatePostRequestDto dto) { ... }

    [HttpPost("{organizationSlug}/jobs")]
    [Authorize]
    public async Task<IActionResult> CreateJobVacancy(string organizationSlug, [FromBody] CreateJobVacancyRequestDto dto) { ... }
    ```
*   **Mức độ áp dụng**: Tích hợp toàn bộ vào `WorkspaceController.cs` và `WorkspaceDtos.cs`.

---

### Prompt số 3: Xây dựng giao diện tin tức đăng bài và tin tuyển dụng phân trang/lọc
*   **Mục đích**: Thiết kế giao diện Frontend cho các trang công khai bài đăng (posts) và tuyển dụng (jobs) với các tính năng tương tác lọc/drawer.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Create React pages for public jobs and posts.
    1. jobs/page.tsx: render lists of jobs with search bar, filters for job types and departments, and a detailed sliding Drawer. Integrate Apply and Save actions using toast messages.
    2. posts/page.tsx: display a feed of company posts, with a 'Write an Announcement' modal for workspace owners to create posts with multiple image links.
    Sync state management using Zustand store.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    // React components code utilizing vanilla CSS variables and responsive styles.
    // Zustand store action functions for posting, applying, and saving.
    ```
*   **Mức độ áp dụng**: Áp dụng cho UI/UX client-side và store quản lý trạng thái.

---

## 4. Cam kết tính trung thực của Nhật ký Prompt

Sinh viên/nhóm cam kết rằng nhật ký prompt trên đây là chính xác và phản ánh đúng các phiên trao đổi với AI trong quá trình phát triển tính năng này.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 14/06/2026 |
