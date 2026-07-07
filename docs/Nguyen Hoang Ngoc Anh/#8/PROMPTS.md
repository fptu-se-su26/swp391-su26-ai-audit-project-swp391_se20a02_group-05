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
| Ngày bắt đầu | 2026-06-06T05:00:00Z |
| Ngày hoàn thành | 2026-06-06T06:00:00Z |

---

## 3. Nhật ký Prompt

### Prompt số 1: Xây dựng helper chuẩn hóa danh sách tag ở Backend C#
*   **Mục đích**: Viết phương thức lọc trùng, giới hạn số lượng và độ dài của tag đầu vào trước khi lưu vào DB.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Write a backend helper function in C# CareerService to validate and normalize a list of string tags. Constraints: maximum 20 items, maximum 100 characters per tag, remove duplicates, ignore empty strings, and return a clean list. Also, add a check to make sure minimum salary is less than or equal to maximum salary.
    ```
*   **Kết quả phản hồi**:
    ```csharp
    private List<string> ValidateAndNormalizeTags(List<string>? tags)
    {
        if (tags == null) return new List<string>();

        if (tags.Count > 20)
        {
            throw new ValidationException("Maximum of 20 tags is allowed.");
        }

        var normalized = new List<string>();
        foreach (var tag in tags)
        {
            var trimmed = tag?.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (trimmed.Length > 100)
            {
                throw new ValidationException("Tag length cannot exceed 100 characters.");
            }

            if (!normalized.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
            {
                normalized.Add(trimmed);
            }
        }

        return normalized;
    }
    ```
*   **Mức độ áp dụng**: 100% áp dụng vào `CareerService.cs` để chuẩn hóa dữ liệu đầu vào.

---

### Prompt số 2: Viết Zod schema và logic kiểm thực điều kiện lương ở Frontend
*   **Mục đích**: Tạo quy tắc validate mức lương tối thiểu <= tối đa và giới hạn ghi chú 2000 ký tự trên giao diện client.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    Write a Zod schema validation rules for a form containing minSalary, maxSalary, and careerNotes. minSalary and maxSalary must be numbers, minSalary must be less than or equal to maxSalary, and careerNotes must be capped at 2000 characters. Handle cases where the fields can be empty or null.
    ```
*   **Kết quả phản hồi**:
    ```typescript
    const careerPreferenceSchema = z.object({
      minSalary: z.union([z.number(), z.nan(), z.null()]).optional(),
      maxSalary: z.union([z.number(), z.nan(), z.null()]).optional(),
      careerNotes: z.string().max(2000, "Notes cannot exceed 2000 characters").optional()
    }).refine((data) => {
      if (data.minSalary && data.maxSalary) {
        return data.minSalary <= data.maxSalary;
      }
      return true;
    }, {
      message: "Minimum salary cannot exceed maximum salary",
      path: ["minSalary"]
    });
    ```
*   **Mức độ áp dụng**: Áp dụng vào schema cấu hình form trong file `CareerTab.tsx`.

---

### Prompt số 3: Tương tác bàn phím và nút Add trong TagChipMultiSelect
*   **Mục đích**: Giúp người dùng thêm tag dễ dàng bằng cách nhấn phím Enter, dấu phẩy hoặc click nút Add.
*   **Hệ thống AI sử dụng**: Antigravity
*   **Prompt gốc**:
    ```text
    How do I update TagChipMultiSelect.tsx to support custom tag entry on Enter or comma, add an explicit 'Add' button next to the input, and ensure it validates local tags before adding?
    ```
*   **Kết quả phản hồi**:
    ```typescript
    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
      if (e.key === 'Enter' || e.key === ',') {
        e.preventDefault();
        handleAddTag();
      }
    };

    const handleAddTag = () => {
      const trimmed = inputValue.trim();
      if (trimmed && !selectedTags.includes(trimmed)) {
        if (trimmed.length > 100) {
          showToast("Tag length cannot exceed 100 characters", "error");
          return;
        }
        if (selectedTags.length >= 20) {
          showToast("Maximum of 20 tags is allowed", "error");
          return;
        }
        onChange([...selectedTags, trimmed]);
        setInputValue('');
      }
    };
    ```
*   **Mức độ áp dụng**: Áp dụng hoàn toàn vào `TagChipMultiSelect.tsx` để tối ưu hóa tương tác chọn từ khóa của ứng viên.

---

## 4. Cam kết tính trung thực của Nhật ký Prompt

Sinh viên/nhóm cam kết rằng nhật ký prompt trên đây là chính xác và phản ánh đúng các phiên trao đổi với AI trong quá trình phát triển tính năng này.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 06/06/2026 |
