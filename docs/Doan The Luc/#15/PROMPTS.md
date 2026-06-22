# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-06-17T18:00:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-06-18                                                                             |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày       | Công cụ AI  | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
| --: | ---------- | ----------- | -------- | -------------- | ------------- | ------------------------- | ---------- |
|   1 | 2026-06-18 | Antigravity | Sửa lỗi đè hiển thị vòng tròn điểm số tổng quan | design lại card này, số hiển thị đang bị đè vòng tròn? (kèm ảnh chụp màn hình) | Xác định lỗi do kích thước của ProgressCircle.Track bị khống chế ở size-9 (36px). Thêm class `!w-full !h-full` để SVG tự co giãn chiếm trọn vùng chứa `w-24 h-24`. | Có | GitHub Commit |
|   2 | 2026-06-18 | Antigravity | Xác nhận vị trí của card lỗi | card đó nằm trong digital CV của người dùng | Hướng dẫn định vị thành phần hiển thị thuộc file `AiAssessmentTab.tsx` trong giao diện CV công khai của người dùng. | Có | GitHub Commit |
|   3 | 2026-06-18 | Antigravity | Yêu cầu đổi tên nhãn nút xem A4 preview | trong CV management này đổi lại một cái nữa là open A4 preview thành live preview đi | Đổi nhãn hiển thị nút bấm Standard A4 CV từ "Open A4 Preview" thành "Live Preview". | Có (Sau đó hoàn tác) | GitHub Commit |
|   4 | 2026-06-18 | Antigravity | Làm rõ yêu cầu nghiệp vụ: Thay đổi cơ chế hành động của nút chứ không phải đổi tên | không phải đổi tên mà là đổi qua phần live preview này nè ko phải mở modal A4 (kèm ảnh minh họa tab Live Preview) | Hoàn tác việc đổi tên nhãn nút bấm, thay đổi thuộc tính onPress để kích hoạt trực tiếp trạng thái `setViewState("editor")` và `setEditorMode("preview")`. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1 (Overall Score Circle Overlap Fix)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-18                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Sửa lỗi biểu đồ tròn ProgressCircle cắt qua số điểm hiển thị tổng quan của ứng viên Thế Lực Đoàn.                                                       |
| Phần việc liên quan | Next.js Frontend (AiAssessmentTab.tsx, component ProgressCircle)                                                                                       |
| Mức độ sử dụng      | Hỗ trợ phân tích kích thước mặc định của track SVG và áp dụng CSS class Tailwind quan trọng (`!`) để ép kích thước.                                    |

#### 5.1. Prompt nguyên văn

```text
design lại card này, số hiển thị đang bị đè vòng tròn? (kèm ảnh chụp màn hình dial score bị đè)
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Trang đánh giá ứng viên (AI Verified Assessment) hiển thị một biểu đồ tròn (ProgressCircle) chứa số điểm tổng quan (ví dụ: 60) và chữ VERIFIED.
- Điểm hiển thị cỡ chữ 3xl (30px) trong khi vòng tròn thực tế hiển thị ở kích thước 36px do thuộc tính size="lg" của HeroUI chỉ định kích thước track là size-9.
- Đường tròn cắt ngang qua chữ số "0" và đè lên nhãn VERIFIED.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất thêm thuộc tính class `className="!w-full !h-full"` trực tiếp vào `<ProgressCircle.Track>` để cưỡng bức SVG chiếm trọn vùng chứa `w-24 h-24` của thẻ cha `<ProgressCircle>`, giúp căn giữa hoàn hảo.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Thêm class `className="!w-full !h-full"` cho `<ProgressCircle.Track>` trong `AiAssessmentTab.tsx`.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Can thiệp căn chỉnh lại thụt lề thụt dòng (indentation) cho các khối code liên quan trong file để giữ tính nhất quán theo quy chuẩn ESLint của dự án.
```

---

### Prompt số 2 (Redirect A4 Preview to Embedded Live Preview)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-18                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Thay đổi tương tác của nút "Open A4 Preview" để chuyển trực tiếp sang tab xem trước Live Preview của editor thay vì hiển thị modal popup.              |
| Phần việc liên quan | Next.js Frontend (cv/page.tsx)                                                                                                                         |
| Mức độ sử dụng      | Hỗ trợ viết các lệnh cập nhật state tương tác (`setViewState` và `setEditorMode`).                                                                     |

#### 5.1. Prompt nguyên văn

```text
không phải đổi tên mà là đổi qua phần live preview này nè ko phải mở modal A4 (kèm ảnh minh họa giao diện Live Preview trong Form Editor)
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Ban đầu, AI hiểu nhầm yêu cầu và đổi tên nút từ "Open A4 Preview" thành "Live Preview" nhưng vẫn giữ nguyên cơ chế mở modal A4.
- Người dùng muốn giữ nguyên tên nút bấm nhưng khi bấm vào nút, hệ thống phải chuyển tiếp màn hình hiển thị sang tab biên tập của CV và kích hoạt chế độ xem trước trực tiếp (Live Preview).
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất khôi phục lại nhãn hiển thị nút là "Open A4 Preview", đồng thời đổi hàm gọi onPress từ `setIsA4PreviewOpen(true)` thành khối cập nhật trạng thái `setViewState("editor")` và `setEditorMode("preview")`.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Thay thế thành công hàm onPress của nút bấm trong `client/src/app/(private)/cv/page.tsx`.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Xác minh tính đúng đắn của hành động: Đảm bảo nút "VIEW A4" trên thanh công cụ của Live Preview vẫn tiếp tục gọi `setIsA4PreviewOpen(true)` để người dùng có thể in hoặc xem bản in chuẩn bất cứ lúc nào.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần đính kèm hình ảnh mô tả trực quan các trạng thái giao diện và chỉ rõ cơ chế hành động của hệ thống (ví dụ: dùng các biến state quản lý hiển thị như viewState hay editorMode) để AI hiểu đúng nghiệp vụ cần xử lý.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Khi AI làm sai, việc phủ nhận trực tiếp giải pháp sai và cung cấp ngay hình ảnh minh họa mong muốn thực tế là cách nhanh nhất để AI nhận diện được vấn đề và đề xuất đúng giải pháp khắc phục.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Design |        2 | design lại card này, số hiển thị đang bị đè vòng tròn? / trong CV management này đổi lại một cái nữa... |
| Prompt Fix    |        2 | card đó nằm trong digital CV của người dùng / không phải đổi tên mà là đổi qua phần live preview này nè... |

---

## 10. Checklist chất lượng prompt

| Tiêu chí                   | Đã đạt? | Ghi chú |
| -------------------------- | :-----: | ------- |
| Prompt có mục tiêu rõ ràng |    x    |         |
| Prompt có đủ bối cảnh      |    x    |         |
| Tự kiểm tra và chỉnh sửa   |    x    |         |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết sử dụng prompt minh bạch và ghi nhận đúng đóng góp của AI.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-18    |
