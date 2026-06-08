# AI Learning Reflection

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
| Ngày hoàn thành reflection | 2026-06-05 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển dự án.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong đợt audit này, nhóm đã sử dụng Antigravity làm trợ lý viết code và sửa lỗi build trực tiếp trên cả backend và frontend. AI hỗ trợ phát hiện nhanh lỗi cú pháp dư thừa trong CareerPreference.cs và đề xuất giải pháp refactor hiệu quả cho lỗi set-state-in-effect trong React 19 bằng cách đưa các lệnh cập nhật state đồng bộ vào microtask hàng đợi (Promise.resolve().then()). Tất cả thay đổi cuối cùng đều được kiểm chứng độc lập bằng lệnh build và linter.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Antigravity (IDE Agentic Coding Assistant) - dùng xuyên suốt việc tìm lỗi, sửa đổi file và tự động kiểm thử.
```

### Lý do sử dụng công cụ đó

```text
Antigravity tích hợp trực tiếp vào môi trường làm việc cục bộ của IDE, có khả năng chạy lệnh, tìm kiếm grep, đọc và sửa file trực tiếp giúp giảm thiểu sai lệch do sao chép thủ công.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [ ] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [ ] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [ ] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
- [x] Review code
- [ ] Tối ưu code
- [ ] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI đã hỗ trợ đắc lực trong việc quét toàn bộ file lỗi linter và thực hiện các chỉnh sửa mã nguồn có độ chính xác cao đối với các hooks React, giúp đẩy nhanh tiến trình tích hợp code.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
1. Hiểu sâu về cơ chế render của React 19: Nhận biết tại sao việc gọi setState đồng bộ trong useEffect lại bị cấm và cách giải quyết thông qua trì hoãn tác vụ (Promise.resolve().then).
2. Học cách sử dụng Next.js Link: Tối ưu hóa điều hướng Client-side SPA thay vì dùng thẻ anchor truyền thống.
3. Kỹ năng gỡ lỗi biên dịch nhanh.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
1. Linter tĩnh đôi khi báo lỗi giả hoặc lặp lại nếu mã nguồn lồng nhau quá phức tạp. Nhóm phải tự phân tích thêm cấu trúc callback của hooks để sắp xếp lại Promise.resolve() hợp lý.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm làm chủ toàn bộ logic của các component. AI chỉ hỗ trợ thực hiện nhanh các tác vụ sửa cú pháp và refactor lặp lại để dọn lỗi linter.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [ ] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Sau khi AI thực hiện chỉnh sửa mã nguồn, nhóm chạy "dotnet build" trên backend, cùng với "npm run build" và "npm run lint" trên client để đảm bảo mọi lỗi đều được giải quyết triệt để và không gây ra lỗi hồi quy (regression).
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Bao bọc lệnh gọi `loadConnections()` trong `LinkedAccountsList.tsx` bằng Promise.resolve() |
| Em/nhóm đã kiểm tra bằng cách nào? | Chạy `npm run lint` sau khi sửa |
| Kết quả kiểm tra | Đạt — ESLint không còn báo lỗi tại dòng 92. |
| Em/nhóm đã xử lý tiếp như thế nào? | Tiếp tục triển khai cho tất cả các useEffect có gọi set state đồng bộ tương tự. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong đợt audit này, AI không đưa ra gợi ý nào sai lệch nghiêm trọng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
1. Phát triển toàn bộ logic nghiệp vụ cho tính năng Career Preferences trên cả backend và frontend.
2. Kiểm tra độc lập và cấu hình các môi trường chạy ứng dụng để xác thực các lỗi build.
3. Định hướng cho AI sửa lỗi bằng cách cung cấp các lệnh build/lint liên tục.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Trạng thái mã nguồn | Có 1 lỗi build backend, 7 lỗi build frontend cản trở commit | 0 lỗi build backend, 0 lỗi linter frontend | Dự án sẵn sàng merge vào CVerify-uat |

---

## 11. Bài học về môn học

```text
1. Quản lý trạng thái và chu kỳ render trong React/Next.js là rất quan trọng để đảm bảo tính mượt mà của UI.
2. Quá trình kiểm thử tĩnh (Linting) và biên dịch liên tục giúp phát hiện sớm các lỗi cú pháp sơ đẳng trước khi commit.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
1. Phải luôn tự mình chạy lại linter và compiler sau mỗi lần AI can thiệp để đảm bảo tính đúng đắn, không tin cậy mù quáng vào kết quả sinh ra.
```

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.

---

## 14. Kế hoạch cải thiện lần sau

```text
1. Tiếp tục cải tiến các prompt xử lý lỗi bằng cách cung cấp ngữ cảnh code xung quanh rõ ràng hơn.
2. Thiết lập cơ chế tự động lint trước khi yêu cầu AI commit.
```

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 |   |
| Prompt có mục tiêu rõ ràng | 5 |   |
| Kiểm chứng kết quả AI | 5 |   |
| Tự chỉnh sửa/cải tiến | 5 |   |
| Hiểu nội dung đã nộp | 5 |   |
| Reflection có chiều sâu | 5 |   |
| Sử dụng AI có trách nhiệm | 5 |   |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm hoàn toàn giải thích được lý do tại sao phải dùng Promise.resolve() để defer set state, cũng như cơ cấu hoạt động của các React hooks trong settings.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có, nhóm tự làm lại được vì đây là các lỗi linter và cú pháp cơ bản, tuy nhiên sẽ mất nhiều thời gian tra cứu và tìm vị trí lỗi hơn.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 05/06/2026 |
