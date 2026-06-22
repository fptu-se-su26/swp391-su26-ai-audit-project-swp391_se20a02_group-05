# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-06-17T18:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-18T02:00:00.000Z                                                               |

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
Mục tiêu là khắc phục lỗi hiển thị vòng tròn biểu đồ điểm số tổng quan bị đè chữ trong digital CV, chuẩn hóa và tái tương tác hành động của nút bấm "Open A4 Preview" trên trang CV Management để chuyển hướng mượt mà sang chế độ Live Preview tích hợp trong form biên tập, đồng thời tối ưu hóa cơ chế normalize dữ liệu Git Metrics và REST APIs quản lý hàng đợi phân tích/đặt lại trạng thái kho lưu trữ ở phía backend.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1 (Overall Score Circle Overlap Fix)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-18                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Sửa lỗi ProgressCircle.Track hiển thị quá nhỏ và bị số điểm cùng nhãn VERIFIED ghi đè/cắt qua.                                                         |
| Phần việc liên quan | Next.js Frontend (AiAssessmentTab.tsx, component ProgressCircle)                                                                                       |
| Mức độ sử dụng      | Hỗ trợ chẩn đoán nguyên nhân do class size-9 mặc định khống chế kích thước SVG track, đề xuất cách ghi đè bằng class Tailwind.                          |

#### 4.1. Prompt đã sử dụng

```text
design lại card này, số hiển thị đang bị đè vòng tròn? (kèm ảnh chụp màn hình dial score bị đè)
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất sử dụng thuộc tính `className="!w-full !h-full"` trực tiếp trên thẻ `<ProgressCircle.Track>` để buộc SVG chiếm trọn không gian `w-24 h-24` của thẻ cha, giúp biểu đồ co giãn chính xác theo thiết kế.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Giải pháp ghi đè CSS cho SVG track của ProgressCircle bằng lớp Tailwind.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Định dạng lại code để đảm bảo thụt lề thụt dòng chuẩn xác theo quy tắc ESLint và Next.js của dự án.
- Giữ nguyên độ dày đường viền `strokeWidth={4}` để duy trì tính nhất quán thị giác với các biểu đồ khác.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(vetting): integrate candidate assessment streaming, repository analysis reset, and dashboard UI improvements | https://github.com/Kaivian/CVerify/commit/aee8cce8ce5ea737de7b6a3a4d7db83b924c68a8 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Sửa đổi giao diện nhỏ nhưng mang lại cải thiện trực quan rất lớn, giúp sản phẩm trông hoàn thiện và chuyên nghiệp hơn rất nhiều khi hiển thị hồ sơ năng lực của ứng viên.
```

---

### Lần sử dụng AI số 2 (Identify Component Path for Digital CV)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-18                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Định vị chính xác tệp tin nguồn chịu trách nhiệm kết xuất giao diện digital CV đang bị lỗi.                                                           |
| Phần việc liên quan | Cấu trúc thư mục Next.js (client/src/app/[username]/components/AiAssessmentTab.tsx)                                                                    |
| Mức độ sử dụng      | Hỗ trợ tìm kiếm theo từ khóa văn bản và lọc danh sách các file liên quan để khoanh vùng.                                                               |

#### 4.1. Prompt đã sử dụng

```text
card đó nằm trong digital CV của người dùng
```

#### 5.2. Kết quả AI gợi ý

```text
AI định vị được từ khóa "AI-Verified Talent Assessment" nằm trong component `AiAssessmentTab.tsx` thuộc trang hiển thị hồ sơ cá nhân công khai `/[username]`.
```

#### 5.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Đường dẫn file `AiAssessmentTab.tsx`.
```

#### 5.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Không có. Sinh viên tự mở file và thực hiện phân tích cấu trúc DOM để xác minh.
```

#### 5.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(vetting): integrate candidate assessment streaming, repository analysis reset, and dashboard UI improvements | https://github.com/Kaivian/CVerify/commit/aee8cce8ce5ea737de7b6a3a4d7db83b924c68a8 |

#### 5.6. Nhận xét cá nhân/nhóm

```text
Việc định vị file nhanh chóng giúp rút ngắn thời gian chuẩn bị và khoanh vùng sửa lỗi giao diện một cách chính xác.
```

---

### Lần sử dụng AI số 3 (CV Management Preview Redirect Button)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-18                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Thay đổi cơ chế hành động của nút "Open A4 Preview" để chuyển người dùng sang tab xem trước trực tiếp Live Preview trong form.                          |
| Phần việc liên quan | Next.js Frontend (cv/page.tsx, navigation và state management)                                                                                         |
| Mức độ sử dụng      | Hỗ trợ viết các câu lệnh chuyển đổi trạng thái và logic gán sự kiện.                                                                                   |

#### 4.1. Prompt đã sử dụng

```text
trong CV management này đổi lại một cái nữa là open A4 preview thành live preview đi
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất đổi tên nút bấm trong `cv/page.tsx` thành "Live Preview" và liên kết hàm chuyển tiếp trạng thái. Tuy nhiên giải pháp này chỉ dừng lại ở việc đổi tên nhãn hiển thị thô.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Khung sườn của button và cách viết prop onPress.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Nhóm phát hiện việc đổi tên nhãn nút bấm không giải quyết đúng nghiệp vụ nên đã thực hiện điều chỉnh ở lần gọi tiếp theo để khôi phục tên nhãn gốc và viết lại logic onPress.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(vetting): integrate candidate assessment streaming, repository analysis reset, and dashboard UI improvements | https://github.com/Kaivian/CVerify/commit/aee8cce8ce5ea737de7b6a3a4d7db83b924c68a8 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Tích hợp trải nghiệm Live Preview trực tiếp giúp cải thiện đáng kể tốc độ biên soạn hồ sơ của ứng viên, giảm thiểu số lần phải mở các cửa sổ popup phiền toái.
```

---

### Lần sử dụng AI số 4 (Form Editor Live Preview Logic Correction)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-18                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Sửa đổi đúng cơ chế chuyển đổi Live Preview cho nút bấm thay vì chỉ đổi tên nhãn.                                                                      |
| Phần việc liên quan | Next.js Frontend (cv/page.tsx, variables state viewState & editorMode)                                                                                 |
| Mức độ sử dụng      | Hỗ trợ sinh mã cập nhật đồng thời hai biến state `viewState` và `editorMode` để chuyển đổi giao diện chính xác.                                         |

#### 4.1. Prompt đã sử dụng

```text
không phải đổi tên mà là đổi qua phần live preview này nè ko phải mở modal A4 (kèm ảnh mô tả Live Preview tab)
```

#### 4.2. Kết quả AI gợi ý

```text
AI phản hồi bằng cách khôi phục lại tên nhãn nút gốc là "Open A4 Preview" và thay đổi onPress thành:
```
```tsx
onPress={() => {
  setViewState("editor");
  setEditorMode("preview");
}}
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc gọi hàm thay đổi state đồng thời.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Độc lập xác nhận việc nút xem bản in A4 tĩnh (VIEW A4) ở góc trên bên phải Live Preview vẫn tiếp tục hoạt động chính xác để không làm mất tính năng xuất bản in của người dùng.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(vetting): integrate candidate assessment streaming, repository analysis reset, and dashboard UI improvements | https://github.com/Kaivian/CVerify/commit/aee8cce8ce5ea737de7b6a3a4d7db83b924c68a8 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc tương tác trực tiếp qua ảnh chụp màn hình giúp AI nhận diện chính xác các thành phần điều hướng trên UI và đưa ra câu lệnh thay đổi state chính xác nhất.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                              |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------ |
| Phân tích yêu cầu           |               |      x       |                 |               | Xác định lỗi CSS và luồng tương tác mong muốn.                                       |
| Viết user story/use case    |       x       |              |                 |               |                                                                                      |
| Thiết kế database           |       x       |              |                 |               |                                                                                      |
| Thiết kế kiến trúc hệ thống |               |      x       |                 |               | Thiết kế luồng chuyển đổi trạng thái của CV page.                                    |
| Thiết kế giao diện          |               |      x       |                 |               | Căn chỉnh kích thước ProgressCircle.                                                 |
| Code frontend               |               |      x       |                 |               | Viết hàm onPress cập nhật state hiển thị và class ghi đè kích thước track SVG.       |
| Code backend                |       x       |              |                 |               | Các endpoints đã có sẵn trong dự án và chỉ được đưa vào commit.                      |
| Debug lỗi                   |               |              |        x        |               | Sửa lỗi ProgressCircle bị đè chữ do giới hạn size-9 của SVG.                        |
| Viết test case              |       x       |              |                 |               |                                                                                      |
| Kiểm thử sản phẩm           |               |      x       |                 |               | Chạy kiểm thử TypeScript compiler và kiểm tra giao diện digital CV trên trình duyệt. |
| Tối ưu code                 |       x       |              |                 |               |                                                                                      |
| Viết báo cáo                |       x       |              |                 |               |                                                                                      |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                      |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI chỉ thực hiện đổi tên nút bấm Standard A4 CV thành "Live Preview" thay vì thay đổi cơ chế onPress như mong muốn. | Kiểm tra mã nguồn thấy nút bấm vẫn gọi `setIsA4PreviewOpen(true)` mở modal tĩnh. | Yêu cầu AI khôi phục nhãn cũ và thay đổi logic onPress để cập nhật state điều hướng `setViewState("editor")` và `setEditorMode("preview")`. |
|   2 | AI đề xuất sử dụng style inline cho SVG để bỏ qua các class mặc định của HeroUI. | Đọc đề xuất thấy vi phạm quy tắc thiết kế nhất quán và tính tái sử dụng class của hệ thống. | Thay đổi sang sử dụng class Tailwind quan trọng (`!w-full !h-full`) trên component con `<ProgressCircle.Track>`. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả qua các hình thức sau:
1. Chạy thành công ứng dụng frontend và backend. Kiểm tra trang Digital CV: Vòng tròn điểm số Thế Lực Đoàn (60 VERIFIED) hiển thị cân đối, bao quanh điểm số rõ ràng và sắc nét.
2. Kiểm tra trang CV Management: Bấm chọn nút Open A4 Preview, giao diện ngay lập tức chuyển sang chế độ biên tập và hiển thị Live Preview bên trong trang. Nút VIEW A4 nhỏ góc phải vẫn mở modal A4 thành công khi cần in ấn.
3. Chạy lệnh type checking `npx tsc --noEmit` hoàn thành thành công không phát sinh bất kỳ cảnh báo hay lỗi kiểu dữ liệu TypeScript nào.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Tự cấu hình luồng chuyển đổi trạng thái của form editor (`setViewState("editor")` và `setEditorMode("preview")`) để tối ưu hóa tương tác xem trước CV.
- Phát hiện lỗi CSS Specificity của component ProgressCircle và ghi đè bằng class Tailwind quan trọng (`!w-full !h-full`).
- Triển khai normalize logic an toàn cho dữ liệu Git Metrics để chống sập giao diện khi gặp dữ liệu không đồng nhất.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                                 | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ---------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai cấu hình hiển thị vòng tròn điểm số, sửa đổi logic chuyển hướng nút bấm, biên dịch. | Có                   | GitHub Commits      |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm tra tính nhất quán giao diện CV và hỗ trợ xác định lỗi đè chữ trên trình duyệt.            | Có                   | GitHub Commits      |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI hỗ trợ chẩn đoán nhanh lỗi ghi đè kích thước track SVG của HeroUI và cung cấp nhanh mã điều chỉnh state tương tác cho nút bấm.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không đổi tên nút bấm thành "Live Preview" thô thiển và không sử dụng thuộc tính style trực tiếp cho SVG để tránh phá vỡ tính đồng nhất trong thiết kế của dự án Next.js.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Thực hiện chạy thử nghiệm trực tiếp trên trình duyệt Chrome, quan sát vị trí hiển thị của vòng tròn điểm số, kiểm thử nút bấm và chạy trình biên dịch kiểm tra kiểu TypeScript để đảm bảo chất lượng.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Việc tìm ra nguyên nhân vòng tròn ProgressCircle bị đè chữ có thể mất nhiều thời gian do phải lục lọi qua các tệp CSS đóng gói trong node_modules của HeroUI để tìm ra class size-9 quy định kích thước track.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Hiểu rõ hơn về cách điều chỉnh tương tác UI linh hoạt bằng cách thay đổi đồng thời nhiều biến trạng thái (state), và cách ghi đè CSS của các thư viện UI hiện đại một cách có hệ thống.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
AI có thể hiểu nhầm các chỉ dẫn bằng chữ đơn thuần, việc kết hợp hình ảnh trực quan và mô tả kỹ thuật cụ thể là cách tốt nhất để sử dụng AI một cách hiệu quả và an toàn.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-18    |
