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
| Ngày bắt đầu | 2026-06-13T07:30:00Z |
| Ngày hoàn thành | 2026-06-13T14:30:00Z |

---

## 3. Nhật ký Prompt

### Prompt số 1: Xây dựng API PATCH cập nhật thông tin doanh nghiệp nâng cao
*   **Mục đích**: Thiết lập API endpoint để người quản trị cập nhật chi tiết cấu hình doanh nghiệp.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Create a PATCH endpoint `PATCH /api/workspace/{organizationSlug}` in WorkspaceController.cs to allow authorized business representatives to update their profile settings. Implement a helper method MapToWorkspaceDetailsDto to map the updated Organization entity. Update relevant DTOs in WorkspaceDtos.cs to support these changes.
    ```
*   **Kết quả phản hồi**:
    ```csharp
    [HttpPatch("{organizationSlug}")]
    [Authorize]
    public async Task<IActionResult> UpdateWorkspaceDetails(string organizationSlug, [FromBody] UpdateWorkspaceDetailsRequestDto dto)
    {
        // Permission check
        // Org update logic
        // Return MapToWorkspaceDetailsDto(org, ...)
    }
    ```
*   **Mức độ áp dụng**: Tích hợp trực tiếp vào `WorkspaceController.cs` và `WorkspaceDtos.cs`.

---

### Prompt số 2: Mở rộng API Members với phân trang và thông tin Profile chi tiết
*   **Mục đích**: Nâng cấp API lấy danh sách thành viên để hiển thị đầy đủ avatar đã ký số, headline và username của mỗi thành viên kèm cơ chế phân trang.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Enhance the `GET /{organizationSlug}/members` endpoint to support pagination (page, pageSize), search querying, and a publicOnly boolean filter. It should join with the UserProfiles table to fetch the member's Headline, Username, and AvatarUrl. Generate a signed URL for AvatarUrl using GetSignedUrlAsync.
    ```
*   **Kết quả phản hồi**:
    ```csharp
    var baseQuery = (from om in _context.OrganizationMemberships.Where(om => om.OrganizationId == org.Id && om.Status == "active")
                     join up in _context.UserProfiles on om.UserId equals up.UserId into upGroup
                     from up in upGroup.DefaultIfEmpty()
                     select new { om, up }).AsNoTracking();
    ```
*   **Mức độ áp dụng**: Áp dụng trực tiếp trong `WorkspaceController.cs` để tối ưu hóa hiệu năng câu truy vấn cơ sở dữ liệu.

---

### Prompt số 3: Tinh chỉnh Frontend Layout và đổi tên tab danh sách thành viên
*   **Mục đích**: Đồng bộ hóa layout hiển thị trang công khai của doanh nghiệp, đổi tên tab từ People thành Members và gọi API phân trang mới.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Refactor the public workspace frontend layout: rename the "People" tab to "Members" across layouts and navigation structures. Update members/page.tsx to load members from the updated backend API with pagination, rendering their headline, username, and signed avatars. Use the Zustand store to hold this state.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    // React component structure displaying cards of members with dynamic avatars and titles.
    // Zustand action updates to load data asynchronously.
    ```
*   **Mức độ áp dụng**: Áp dụng vào `client/src/app/workspace/[organizationSlug]/(public)/` và các view tương ứng.

---

## 4. Cam kết tính trung thực của Nhật ký Prompt

Sinh viên/nhóm cam kết rằng nhật ký prompt trên đây là chính xác và phản ánh đúng các phiên trao đổi với AI trong quá trình phát triển tính năng này.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
