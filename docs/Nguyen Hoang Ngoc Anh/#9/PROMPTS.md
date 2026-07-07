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
| Ngày bắt đầu | 2026-06-09T01:05:00Z |
| Ngày hoàn thành | 2026-06-09T01:45:00Z |

---

## 3. Nhật ký Prompt

### Prompt số 1: Xây dựng lưới grid responsive 2 cột và thứ tự các thẻ preferences
*   **Mục đích**: Bố trí thẻ Discoverability và AI Trajectory cạnh nhau trên màn hình desktop, đồng thời sắp xếp lại thứ tự của 6 thẻ preference phía dưới.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Place Discoverability and AI-Inferred Career Trajectory on the same horizontal row using a responsive two-column layout in CareerTab.tsx. Then display the remaining cards in this order:
    1. Search Status & Roles
    2. Work Arrangements & Mobility
    3. Compensation & Visibility
    4. Targeted Skills & Growth
    5. Company & Culture Fit
    6. Localization Preferences
    ```
*   **Kết quả phản hồi**:
    ```typescript
    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
      {/* Left Column: AI-Inferred Career Trajectory */}
      <Card>...</Card>
      {/* Right Column: Discoverability Score Widget */}
      <Card>...</Card>
    </div>
    ```
*   **Mức độ áp dụng**: Áp dụng 100% vào tệp `CareerTab.tsx`.

---

### Prompt số 2: Viết mã nguồn hiển thị tiền tệ động với InputGroup
*   **Mục đích**: Thay đổi linh hoạt nhãn, ký tự prefix ($ hoặc ₫), suffix (USD hoặc VND) và placeholder của 2 trường nhập mức lương dựa trên Expected Currency.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    In Compensation & Visibility, dynamically show VND or USD based on expectedSalaryCurrency. Use labels/placeholders/symbols appropriate for the selected currency. Before using InputGroup, Prefix, or Suffix, verify that these components already exist in the project. If they do not exist, use HeroUI Input’s startContent and endContent props instead.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    const isVND = currentValues.expectedSalaryCurrency === "VND";
    const currencySymbol = isVND ? "₫" : "$";
    const currencyCode = isVND ? "VND" : "USD";
    const salaryPlaceholderMin = isVND ? "e.g. 50,000,000" : "e.g. 3000";
    const salaryPlaceholderMax = isVND ? "e.g. 100,000,000" : "e.g. 5000";

    // Markup:
    <InputGroup>
      <InputGroup.Prefix>{currencySymbol}</InputGroup.Prefix>
      <InputGroup.Input placeholder={salaryPlaceholderMin} ... />
      <InputGroup.Suffix>{currencyCode}</InputGroup.Suffix>
    </InputGroup>
    ```
*   **Mức độ áp dụng**: Tích hợp toàn diện vào các trường nhập liệu số lượng của expectedSalary.

---

### Prompt số 3: Thiết kế trạng thái disabled và màu sắc cho các nút Add
*   **Mục đích**: Điều chỉnh nút Add Custom option hiển thị màu trắng khi input rỗng và bị vô hiệu hóa; chuyển sang màu nâu nổi bật khi có giá trị; đè CSS mặc định của HeroUI.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    In TagChipMultiSelect.tsx, make the Add button white/inactive when the Add custom option input is empty. Make the Add button active brown only when the user enters valid text. Keep the existing add custom option logic unchanged. Standardize all Add buttons to the same size and alignment. If HeroUI disabled styles override the inactive white style, adjust className/variant carefully without breaking accessibility.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    className={
      !inputValue.trim()
        ? "bg-white dark:bg-surface border border-border text-muted font-bold shrink-0 opacity-60 cursor-not-allowed disabled:bg-white dark:disabled:bg-surface data-[disabled=true]:bg-white dark:data-[disabled=true]:bg-surface data-[disabled=true]:text-muted data-[disabled=true]:border-border data-[disabled=true]:opacity-60"
        : "bg-accent text-accent-foreground font-bold shrink-0 hover:bg-accent/90 cursor-pointer"
    }
    ```
*   **Mức độ áp dụng**: Áp dụng cho các nút Add trong `TagChipMultiSelect.tsx` và hai khu vực nhập tag/địa điểm của `CareerTab.tsx`.

---

### Prompt số 4: Phục hồi UnsavedChangesBar và loại bỏ ConfirmationModal
*   **Mục đích**: Chuyển đổi từ cơ chế nút bấm lưu tĩnh cùng modal xác nhận về thanh nổi cảnh báo trạng thái form và tự động validate trước khi lưu.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Keep UnsavedChangesBar and remove ConfirmationModal. Make sure Save changes triggers react-hook-form validation inside handleSaveChanges first. Reset must restore the last successfully loaded/saved data from backend without hit backend API.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    const handleSaveChanges = async () => {
      const isValid = await methods.trigger();
      if (!isValid) return;
      // ... Gọi API update
    };

    <UnsavedChangesBar
      message="You have unsaved career preference changes."
      onReset={handleReset}
      onSave={handleSaveChanges}
      isSubmitting={isUpdating}
    />
    ```
*   **Mức độ áp dụng**: Được đưa vào phần điều khiển hành vi thay đổi dữ liệu của `CareerTab.tsx`.

---

## 4. Cam kết tính trung thực của Nhật ký Prompt

Sinh viên/nhóm cam kết rằng nhật ký prompt trên đây là chính xác và phản ánh đúng các phiên trao đổi với AI trong quá trình phát triển tính năng này.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 09/06/2026 |
