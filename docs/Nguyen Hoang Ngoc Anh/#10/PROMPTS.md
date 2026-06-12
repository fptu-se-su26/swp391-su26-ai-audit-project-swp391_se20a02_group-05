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
| Ngày bắt đầu | 2026-06-13T01:00:00Z |
| Ngày hoàn thành | 2026-06-13T01:30:00Z |

---

## 3. Nhật ký Prompt

### Prompt số 1: Mở rộng thực thể Organization và cập nhật schema Database
*   **Mục đích**: Thêm các trường hồ sơ công ty mới vào thực thể Organization và tự động cập nhật Database schema an toàn khi triển khai.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Expand Organization entity with profile columns: banner_url, logo_url, description, website, company_size, founded_year, industry_tags (array of strings), benefit_tags (array of strings), gallery_urls (array of strings), contact_email, contact_phone, address, linkedin_url, facebook_url, github_url, and branch_count. Configure ApplicationDbContext to map array columns correctly. Write SQL DDL in DbInitializer.cs to check if each column exists before adding it so existing data is not lost.
    ```
*   **Kết quả phản hồi**:
    ```csharp
    // Organization.cs
    public string? BannerUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public List<string> IndustryTags { get; set; } = new();
    public List<string> GalleryUrls { get; set; } = new();
    // ... other fields

    // DbInitializer.cs - Safe column addition
    var checkColumnSql = @"
        SELECT COUNT(*) FROM information_schema.columns
        WHERE table_name = 'Organizations' AND column_name = 'banner_url'";
    var exists = await db.Database.ExecuteSqlRawAsync(checkColumnSql);
    if (exists == 0)
        await db.Database.ExecuteSqlRawAsync("ALTER TABLE Organizations ADD COLUMN banner_url TEXT");
    ```
*   **Mức độ áp dụng**: Áp dụng trực tiếp vào `Organization.cs`, `ApplicationDbContext.cs` và `DbInitializer.cs`.

---

### Prompt số 2: Xây dựng API Workspace endpoints với Signed URL và kiểm soát quyền
*   **Mục đích**: Thêm endpoint PATCH cập nhật thông tin tổ chức và POST/DELETE để quản lý ảnh gallery, tích hợp Signed URL S3 cho bảo mật lưu trữ.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    In WorkspaceController.cs, add:
    1. PATCH /workspaces/{slug}/details - Update workspace profile (description, contact, social links, tags). Verify the caller is Owner or Admin using existing auth middleware.
    2. POST /workspaces/{slug}/gallery - Upload an image to gallery storage. Return the signed URL of the uploaded image.
    3. DELETE /workspaces/{slug}/gallery/{imageKey} - Delete a specific gallery image by key.
    Also update the GET workspace details endpoint to return signed URLs for banner, logo, and all gallery images using GetSignedUrlAsync from IStorageService.
    ```
*   **Kết quả phản hồi**:
    ```csharp
    [HttpPatch("{slug}/details")]
    [Authorize]
    public async Task<IActionResult> UpdateWorkspaceDetails(string slug,
        [FromBody] UpdateWorkspaceDetailsRequestDto dto)
    {
        var org = await _workspaceService.GetBySlugAsync(slug);
        if (!HasPermission(org, "Owner", "Admin")) return Forbid();
        await _workspaceService.UpdateDetailsAsync(org.Id, dto);
        return NoContent();
    }

    [HttpPost("{slug}/gallery")]
    public async Task<IActionResult> UploadGalleryImage(string slug, IFormFile file)
    {
        var key = await _storageService.UploadAsync(file, $"workspace/{slug}/gallery");
        var signedUrl = await _storageService.GetSignedUrlAsync(key);
        return Ok(new { url = signedUrl, key });
    }
    ```
*   **Mức độ áp dụng**: Tích hợp vào `WorkspaceController.cs` với điều chỉnh tên phương thức và xử lý lỗi phù hợp với kiến trúc hiện có của CVerify.

---

### Prompt số 3: Xây dựng giao diện trang hồ sơ công khai và chỉnh sửa thông tin doanh nghiệp
*   **Mục đích**: Tạo các React component hiển thị trang hồ sơ công khai và form chỉnh sửa thông tin tổ chức với gallery upload/delete.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Create two React views for workspace management:
    1. workspace-public-profile-view.tsx: Display public company profile with banner image, company logo, description, contact info (email, phone, address, website), social media links (LinkedIn, Facebook, GitHub), industry tags, benefit tags, and a responsive photo gallery grid.
    2. workspace-information-view.tsx: A settings/editing form that lets the workspace owner update all profile fields, upload/delete banner and logo images, manage gallery by adding/removing photos. Use the workspace.service.ts functions and update use-workspace-store.ts to sync state.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    // workspace-public-profile-view.tsx
    export const WorkspacePublicProfileView = ({ workspace }) => (
      <div className="flex flex-col gap-8">
        <div className="relative">
          <img src={workspace.bannerUrl} className="w-full h-64 object-cover rounded-xl" />
          <img src={workspace.logoUrl} className="absolute bottom-4 left-6 w-20 h-20 rounded-full border-4 border-white" />
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {/* Main info and contact sections */}
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {workspace.galleryUrls?.map((url, i) => (
            <img key={i} src={url} className="rounded-lg aspect-square object-cover" />
          ))}
        </div>
      </div>
    );
    ```
*   **Mức độ áp dụng**: Cấu trúc component và bố cục CSS được áp dụng và điều chỉnh để phù hợp với hệ thống design token của CVerify.

---

### Prompt số 4: Cập nhật Zustand store và TypeScript types
*   **Mục đích**: Đồng bộ hóa state management và định nghĩa type cho workspace chi tiết mới.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Update use-workspace-store.ts Zustand store to include:
    - workspaceDetails state of type WorkspaceDetailsDto
    - isLoadingDetails boolean
    - Actions: fetchWorkspaceDetails(slug), updateWorkspaceDetails(slug, data), uploadGalleryImage(slug, file), deleteGalleryImage(slug, key)
    Also update workspace.types.ts to add WorkspaceDetailsDto with all profile fields and UpdateWorkspaceDetailsRequest type matching the backend DTO.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    // workspace.types.ts
    export interface WorkspaceDetailsDto {
      id: string;
      name: string;
      slug: string;
      bannerUrl?: string;
      logoUrl?: string;
      description?: string;
      galleryUrls?: string[];
      industryTags?: string[];
      benefitTags?: string[];
      contactEmail?: string;
      linkedinUrl?: string;
      // ...
    }

    // use-workspace-store.ts
    interface WorkspaceStore {
      workspaceDetails: WorkspaceDetailsDto | null;
      isLoadingDetails: boolean;
      uploadGalleryImage: (slug: string, file: File) => Promise<void>;
    }
    ```
*   **Mức độ áp dụng**: Tích hợp đầy đủ vào `workspace.types.ts` và `use-workspace-store.ts`.

---

## 4. Cam kết tính trung thực của Nhật ký Prompt

Sinh viên/nhóm cam kết rằng nhật ký prompt trên đây là chính xác và phản ánh đúng các phiên trao đổi với AI trong quá trình phát triển tính năng này.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
