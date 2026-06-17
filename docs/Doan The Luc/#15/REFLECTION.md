# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-06-18                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình triển khai cơ chế đồng bộ SSE tiến độ đánh giá, API đặt lại trạng thái phân tích, và khắc phục lỗi hiển thị vòng tròn điểm số tổng quan của ứng viên, AI đã hỗ trợ thiết lập khung API reset ở Backend .NET Core, viết các hàm xử lý gộp trạng thái của Git Metrics và tối ưu hóa CSS cho ProgressCircle. Sinh viên chịu trách nhiệm chính trong việc kiểm chứng bảo mật API reset, thiết kế luồng chuyển tiếp giao diện của nút bấm preview sang chế độ Live Preview tích hợp trong form editor, và tìm ra nguyên nhân sâu xa của việc đè đứt vòng tròn SVG do độ đặc hiệu CSS (specificity) trong cấu hình lớp size-9 của HeroUI.
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
Antigravity
```

### Lý do sử dụng công cụ đó

```text
Antigravity cung cấp khả năng tích hợp chặt chẽ với workspace thực tế, giúp nhanh chóng rà soát mã nguồn ở cả Frontend và Backend, đồng thời kiểm tra tự động các lỗi kiểu dữ liệu TypeScript và định dạng ESLint trong quá trình làm việc.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
- [x] Review code
- [x] Tối ưu code
- [ ] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI đã hỗ trợ sinh mã boilerplate cho các thuộc tính Git Metrics normalizer, gợi ý phương án đặt lại trạng thái phân tích ở controller backend, hướng dẫn cách sử dụng class Tailwind quan trọng (!) để ghi đè các cấu hình CSS lồng nhau của HeroUI, và viết khung store quản lý các công việc phân tích của kho lưu trữ.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu sâu sắc hơn về tính ưu việt của cơ chế truyền tải Server-Sent Events (SSE) so với Polling truyền thống trong việc cập nhật trạng thái thời gian thực.
- Nhận thức rõ về độ đặc hiệu trong CSS (CSS Specificity) và cách thức Tailwind xử lý ghi đè thuộc tính khi làm việc với các component thư viện (HeroUI) được đóng gói sẵn.
- Tiếp cận giải pháp xử lý dữ liệu mềm dẻo (graceful fallback) khi đọc cấu trúc dữ liệu JSON không đồng bộ từ phía AI pipeline trả về.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đề xuất đổi tên nút bấm trong trang CV Management thay vì thay đổi luồng xử lý tương tác của nút. Điều này không đúng với mong muốn thực tế là chuyển trạng thái màn hình sang Live Preview.
- AI đề xuất sử dụng thuộc tính style trực tiếp (`style={{ width: '100%' }}`) để cưỡng bức kích hoạt kích thước SVG, giải pháp này phá vỡ tính nhất quán của hệ thống Tailwind CSS của dự án.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm chủ yếu sử dụng AI để tăng tốc độ phát triển các cấu trúc gọi API cơ bản và đề xuất phương án khắc phục lỗi hiển thị. Mọi quyết định về mặt kiến trúc tương tác, luồng chuyển đổi trạng thái của form editor, và kiểm thử độ ổn định dữ liệu đều do thành viên nhóm trực tiếp phân tích và thực hiện.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [ ] Viết test case
- [x] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [x] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Quá trình kiểm chứng được thực hiện qua các bước:
1. Chạy thử nghiệm biên dịch Next.js và kiểm tra các lỗi type checking (`npx tsc --noEmit`).
2. Mở trực tiếp trang Digital CV của ứng viên Thế Lực Đoàn tại local để quan sát sự thay đổi kích thước của vòng tròn điểm số: Vòng tròn đã bao quanh chữ 60 VERIFIED một cách cân đối, không còn đè lên chữ.
3. Nhấp chọn nút Open A4 Preview trên dashboard để xác nhận trang tự động chuyển đổi sang giao diện Form Editor ở trạng thái Live Preview thay vì mở popup modal như trước.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý thay thế nhãn hiển thị nút bấm thành "Live Preview" để tương ứng với tính năng mới. |
| Em/nhóm đã kiểm tra bằng cách nào? | Đọc kỹ lại yêu cầu tương tác và kiểm tra luồng sử dụng trên trang CV Management. |
| Kết quả kiểm tra | Việc đổi tên nhãn không giải quyết được mong muốn là chuyển đổi hành động của nút từ mở modal sang kích hoạt Live Preview bên trong trang biên tập. |
| Em/nhóm đã xử lý tiếp như thế nào? | Hoàn tác việc đổi tên nhãn về lại "Open A4 Preview", đồng thời cấu hình lại sự kiện onPress để thiết lập `setViewState("editor")` và `setEditorMode("preview")`. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Thay đổi nhãn hiển thị của nút bấm thành "Live Preview" thay vì giữ nguyên nhãn và chuyển hướng hành động. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Hiểu sai ngữ cảnh mong muốn của người dùng (người dùng muốn đổi cơ chế hoạt động của nút chứ không muốn đổi tên nút). |
| Em/nhóm phát hiện bằng cách nào? | Dựa vào phản hồi trực tiếp từ người dùng và kiểm tra giao diện CV. |
| Em/nhóm đã sửa như thế nào? | Khôi phục lại nhãn nút bấm gốc và thay đổi logic onPress để chuyển đổi tab hiển thị Live Preview trực tiếp trong trang biên soạn. |
| Bài học rút ra | Cần phải lắng nghe kỹ và phân tích kỹ các phản hồi phản biện của người dùng, tránh tự ý thay đổi các nhãn hiển thị đã được định nghĩa thống nhất trên hệ thống. |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
- Tự cấu hình luồng chuyển đổi trạng thái của form editor (`setViewState("editor")` và `setEditorMode("preview")`) để tối ưu hóa tương tác xem trước CV.
- Phát hiện lỗi CSS Specificity của component ProgressCircle và ghi đè bằng class Tailwind quan trọng (`!w-full !h-full`).
- Triển khai normalize logic an toàn cho dữ liệu Git Metrics để chống sập giao diện khi gặp dữ liệu không đồng nhất.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Tiết kiệm ~40% thời gian thiết lập các APIs reset và cấu trúc dữ liệu Normalizer. |
| UI Alignment | Basic | Premium | Khắc phục triệt để lỗi đè chữ trên biểu đồ tròn, mang lại cảm giác thiết kế cao cấp và nhất quán. |
| Logic Resilience | Basic | Solid | Khắc phục được các lỗi xử lý schema rỗng hoặc không đồng bộ của Git Metrics. |

---

## 11. Bài học về môn học

- Các component giao diện phức tạp từ thư viện (như HeroUI) thường đi kèm với các class CSS lồng nhau có độ ưu tiên cao. Để ghi đè kích thước một cách an toàn và nhất quán, việc hiểu rõ CSS Specificity là cực kỳ quan trọng.
- Khi truyền dữ liệu thời gian thực từ AI engine về Client qua SSE, cần thiết lập một Normalizer vững chắc ở Client để tránh lỗi phá vỡ cấu trúc giao diện khi nhận dữ liệu thô chưa qua làm sạch.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Không được phụ thuộc hoàn toàn vào gợi ý đầu tiên của AI. AI có thể hiểu sai mục đích sử dụng (như việc đề xuất đổi tên nút thay vì đổi hành động bấm nút).
- Phải luôn kiểm tra các cảnh báo biên dịch kiểu dữ liệu (TypeScript) và chất lượng code (ESLint) sau khi AI sinh mã để đảm bảo sản phẩm sạch hoàn toàn.

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

---

## 14. Kế hoạch cải thiện lần sau

- Cung cấp mô tả chi tiết hơn về mặt tương tác UI (ví dụ: mô tả rõ việc chuyển đổi viewState thay vì chỉ đề cập đến tên nút) để AI hiểu đúng ngữ cảnh nghiệp vụ ngay từ đầu.
- Rà soát kỹ lưỡng CSS Specificity trước khi yêu cầu AI sửa các lỗi hiển thị liên quan đến component thư viện.

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
| --- | --- | --- |
| Ghi nhận việc dùng AI trung thực | 5 | |
| Prompt có mục tiêu rõ ràng | 5 | |
| Kiểm chứng kết quả AI | 5 | |
| Tự chỉnh sửa/cải tiến | 5 | |
| Hiểu nội dung đã nộp | 5 | |
| Reflection có chiều sâu | 5 | |
| Sử dụng AI có trách nhiệm | 5 | |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm giải thích rõ được nguyên lý hoạt động của Server-Sent Events (SSE) trong việc truyền phát tiến độ phân tích, lý do áp dụng class Tailwind quan trọng (!) để sửa lỗi ghi đè kích thước track của SVG ProgressCircle, và cách thức hoạt động của Live Preview trong form editor của CV Management.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc cấu hình CSS ghi đè kích thước, tối ưu hóa các store quản lý dữ liệu trong React (Zustand), và thiết lập các endpoint REST API trong ASP.NET Core đều là những kỹ năng chuyên môn cốt lõi đã được kiểm chứng của nhóm.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-18    |
