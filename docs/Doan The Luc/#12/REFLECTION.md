# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Avatar Source Persistence, Achievements System & Form Standardizations       |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-06-03                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình phát triển Avatar Source Persistence, hệ thống Achievements và chuẩn hóa form nhập liệu, AI hỗ trợ sinh nhanh cấu trúc các thực thể DB mới, service CRUD, bộ khung component PhoneNumberField, và boilerplates cho test cases. Sinh viên đóng vai trò thiết kế chính trong việc: triển khai migrations an toàn cho dữ liệu lịch sử bằng raw SQL, quản lý xóa vật lý tệp ảnh trên Cloudflare R2, viết logic validate ngày tháng tùy chỉnh ở backend, tích hợp dirty checking với form state của React Hook Form, mở rộng PhoneNumberField hỗ trợ onBlur và accessibility attributes chi tiết, và hoàn thiện bộ tích hợp kiểm thử.
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
Antigravity cung cấp khả năng chỉnh sửa và liên kết mã nguồn đồng thời trên nhiều ngôn ngữ (C# backend, TypeScript/React frontend) và các thư mục cấu hình một cách mượt mà và trực tiếp trong workspace.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [x] Viết test case
- [ ] Review code
- [ ] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI hỗ trợ sinh enum AvatarSource và logic phân nhánh trong ProfileService/AuthService. Sinh khung CRUD service và controller backend cho Kinh nghiệm & Thành tích. Sinh boilerplate PhoneNumberField trên client và bộ test case mẫu trong AvatarOwnershipTests.cs và auth.validator.test.ts.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu phương pháp tách biệt nguồn gốc tài nguyên (như AvatarSource) để kiểm soát việc đồng bộ dữ liệu OAuth thay vì so sánh chuỗi URL động.
- Nắm vững cách xây dựng form nhập liệu lồng nhau động (nested form array) với React Hook Form trên Next.js.
- Học cách chuẩn hóa dữ liệu số điện thoại theo chuẩn quốc tế E.164 từ client trước khi đưa vào database.
- Hiểu rõ phương pháp gán ARIA attributes cho các custom form fields để tuân thủ WCAG accessibility standards.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI gợi ý logic kiểm tra avatar dựa trên URL chuỗi (fragile string check), thiết kế này rất không an toàn khi Google ký mới (expires) ảnh đại diện của user.
- Component PhoneNumberField ban đầu do AI đề xuất thiếu prop `onBlur`, làm gãy touched validation states của trang ReclaimView sử dụng local state.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm chỉ dùng AI để tạo boilerplate và sinh khung kiểm thử nhanh. Toàn bộ logic lõi như database transactions, R2 storage purging, custom date validation ở backend, custom copy paste handler trên PhoneNumberField và accessibility details đều do sinh viên tự tay lập trình và tối ưu hóa.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [x] Viết test case
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
Nhóm kiểm chứng bằng cách:
1. Viết và chạy thành công 7 integration test cases trong AvatarOwnershipTests.cs bao phủ toàn bộ vòng đời avatar (tải lên, xóa, khôi phục, Google login sync).
2. Viết và chạy thành công unit test suite auth.validator.test.ts kiểm chứng password/phone validation.
3. Build thành công client bundle Next.js (npm run build).
4. Kiểm tra thủ công lưu số điện thoại ở ProfileTab và ReclaimView trên giao diện web, kiểm nghiệm DB lưu đúng E.164 format.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý logic kiểm tra avatar thay đổi bằng cách so sánh URL mới của Google với URL hiện tại lưu trong DB. |
| Em/nhóm đã kiểm tra bằng cách nào? | Đăng nhập lại Google sau khi upload avatar thủ công, Google ký mới URL hình ảnh và hệ thống tự động ghi đè liên kết Google vào DB mặc dù user đã tải lên ảnh tùy chọn. |
| Kết quả kiểm tra | Thiết kế so sánh URL chuỗi của AI bị lỗi vì Google signed URL thay đổi liên tục. |
| Em/nhóm đã xử lý tiếp như thế nào? | Chuyển đổi sang lưu cờ AvatarSource enum trong User entity để phân biệt rõ ràng nguồn Uploaded vs Google. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | PhoneNumberField component ban đầu thiếu prop `onBlur` chuyển tiếp từ input gốc. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Khiến ReclaimView không thể kích hoạt touched states đúng thời điểm (người dùng bấm ra ngoài ô nhập liệu mà không gõ gì thì lỗi validate không hiện). |
| Em/nhóm phát hiện bằng cách nào? | Test thủ công trang ReclaimView và thấy lỗi không xuất hiện khi bấm click out khỏi trường số điện thoại. |
| Em/nhóm đã sửa như thế nào? | Bổ sung `onBlur` vào `PhoneNumberFieldProps` và cắm `onBlur={onBlur}` vào thẻ `InputGroup.Input` gốc. |
| Bài học rút ra | Các custom form component tự viết phải luôn hỗ trợ đầy đủ các standard event props (`onChange`, `onBlur`, `onFocus`) để tương thích tốt với các form libraries hay custom validation handlers. |

---

## 9. Phân đóng góp thật sự của sinh viên/nhóm

```text
- Viết logic dynamic schema upgrades và data backfill bằng raw SQL trong DbInitializer.cs.
- Lập trình hàm xóa vật lý file ảnh trên Cloudflare R2 sử dụng IStorageService.
- Triển khai custom date validation ràng buộc StartDate/EndDate ở backend.
- Bổ sung prop onBlur, custom copy-paste formatter, và accessibility attributes trong PhoneNumberField.
- Viết 7 integration test scenarios cho avatar và bộ unit tests cho validation rules.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Rút ngắn ~50% thời gian tạo CRUD endpoints và nested client forms. |
| Code Quality | Good | Excellent | Các form nhập liệu được chuẩn hóa, PhoneNumberField độc lập với accessibility cao. |
| Testing | Good | Excellent | Bổ sung thêm integration tests cho avatar lifecycle và unit tests cho client. |

---

## 11. Bài học về môn học

- Thiết kế cơ sở dữ liệu và API cho các thực thể lồng nhau (như Kinh nghiệm & Thành tích) cần có sắp xếp DisplayOrder nhất quán ở DB level để tránh lộn xộn giao diện.
- Đồng bộ dữ liệu OAuth profile (như avatar, email) phải có thuộc tính trạng thái (AvatarSource) quản lý tường minh để tránh ghi đè dữ liệu tùy chỉnh của người dùng.
- Việc chuẩn hóa form và input format (E.164) giúp hệ thống hoạt động ổn định và nhất quán định dạng.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Phải kiểm tra chéo các giả định của AI về các tham số động bên ngoài (như signed URLs).
- custom components do AI sinh ra thường thiếu các chuẩn tiếp cận (accessibility) và event handlers đầy đủ; nhà phát triển cần chủ động rà soát lại.

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

- Chủ động cung cấp đặc tả accessibility (như WCAG standards) ngay trong prompt ban đầu khi thiết kế custom UI components.
- Yêu cầu AI sử dụng các giải pháp trạng thái bền vững (enum/database flags) thay vì so sánh chuỗi động.

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
Có. Nhóm giải thích rõ ràng được cơ chế chống ghi đè bằng cờ AvatarSource enum, luồng CRUD và reordering display order của WorkExperience/AcademicAchievement ở backend EF Core, và cách PhoneNumberField tự động lọc định dạng số và prependen mã quốc gia E.164 +84.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc map enum EF Core, viết CRUD logic, xử lý mảng động trong React Hook Form, và thiết kế validation regex Zod đều là kỹ năng lập trình Fullstack cơ bản. AI đóng vai trò như một trợ lý tăng tốc độ gõ code.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-03    |
