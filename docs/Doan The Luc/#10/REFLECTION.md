# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Account Deletion Lifecycle & Modular Monolith Transition                      |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-06-01                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình phát triển tính năng Account Deletion Lifecycle và thực hiện tái cấu trúc dự án sang Modular Monolith, AI đóng vai trò hỗ trợ đắc lực trong việc sinh mã boilerplate (domain status transition, dọn dẹp assets trên R2 storage, băm SHA-256 ẩn danh hóa actor) và di chuyển/phân tách cấu trúc thư mục của CVerify.Core backend. Nhóm giữ vai trò chủ đạo trong việc tinh chỉnh logic bảo mật (SecurityAlertNotice khi dùng OTP fallback), khắc phục lỗi ranh giới phụ thuộc nghiêm trọng của NetArchTest trong lớp ModularBoundaryTests.cs (giúp DbContext/User có thể liên kết features hợp lệ), và sửa đổi thủ công namespace lỗi trên hơn 50 tệp kiểm thử tự động để dự án biên dịch và kiểm thử thành công 100%.
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
Antigravity cung cấp khả năng phân tích toàn bộ cấu trúc dự án ASP.NET Core v10 và quản lý tệp tin cục bộ, hỗ trợ đắc lực cho việc di chuyển các file source code, đổi tên namespaces quy mô lớn, và chạy kiểm thử tự động trực tiếp trên máy của người dùng.
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
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI hỗ trợ viết cấu trúc dọn dẹp file R2 storage và tính toán hàm băm actor trong TokenCleanupBackgroundJob.cs. AI cũng hỗ trợ sinh boilerplate cho trang Reactivate UI và phác thảo lớp test kiến trúc ModularBoundaryTests.cs sử dụng thư viện NetArchTest để kiểm soát ranh giới của các modules trong dự án.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu rõ phương pháp thiết kế vòng đời xóa tài khoản an toàn với grace period 14 ngày.
- Nắm vững kiến trúc Modular Monolith và cách cô lập các miền nghiệp vụ để dễ phát triển song song.
- Học cách viết các bài kiểm thử tĩnh (Architecture Tests) bằng thư viện NetArchTest để kiểm soát tính toàn vẹn thiết kế kiến trúc dự án.
- Hiểu sâu hơn về tối ưu hóa dọn dẹp dữ liệu lưu trữ vật lý (R2/S3) và ẩn danh hóa lịch sử (GDPR compliance).
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đề xuất quy tắc NetArchTest cấm Shared phụ thuộc Features tuyệt đối, điều này không thực tế vì trong Modular Monolith sử dụng một DbContext dùng chung (ApplicationDbContext), DbContext này bắt buộc phải import và cấu hình DbSet cho tất cả thực thể thuộc các modules.
- AI bỏ qua mối liên kết nghiệp vụ trực tiếp giữa Recovery và Auth mô-đun trong kiến trúc monolith, khiến bài test features cô lập hoàn toàn thất bại.
- AI không tự động sửa đổi namespaces cho các tệp kiểm thử cũ sau khi refactor cấu trúc thư mục, buộc sinh viên phải tự sửa lỗi biên dịch thủ công ở hàng chục tệp tin.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm sử dụng AI chủ yếu để giải quyết các tác vụ lặp đi lặp lại như sinh mã khung sườn (boilerplate) và di chuyển files. Việc giải quyết các bài toán kiến trúc thực tế (ngoại lệ kiểm thử phụ thuộc), bảo mật gửi thông báo OTP fallback, và phục hồi hàng loạt tệp kiểm thử bị lỗi biên dịch namespaces hoàn toàn do sinh viên tự xử lý.
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
1. Viết và chạy thành công 5 integration tests tự động cho vòng đời xóa tài khoản (AccountDeletionTests.cs).
2. Kích hoạt ValidateOnBuild = true để phát hiện các dependency injection lifetime conflicts ngay khi startup.
3. Chạy kiểm thử kiến trúc bằng NetArchTest để xác nhận các quy tắc cô lập mô-đun hoạt động chính xác sau khi cấu hình ngoại lệ.
4. Chạy toàn bộ 82 integration tests của dự án để đảm bảo không có lỗi runtime sau đợt refactoring lớn.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý bài test `Shared_ShouldNot_DependOnFeatures` cấm tuyệt đối mọi sự tham chiếu từ Shared sang các feature modules. |
| Em/nhóm đã kiểm tra bằng cách nào? | Chạy `dotnet test` và phát hiện bài test thất bại hoàn toàn vì `ApplicationDbContext`, `DbInitializer`, và `User` thực tế nằm ở Shared nhưng phải phụ thuộc vào các features để khai báo DbSet và liên kết nghiệp vụ. |
| Kết quả kiểm tra | Test thất bại, dự án không thể pass CI/CD. |
| Em/nhóm đã xử lý tiếp như thế nào? | Chỉnh sửa `ModularBoundaryTests.cs` để thêm các điều kiện loại trừ (.And().DoNotHaveName(...)) cho các class DB-related và User entity, cho phép chúng phụ thuộc hợp lệ vào features. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Thiết lập bài test kiến trúc Features_ShouldNot_DependOnOtherFeatures cấm tất cả các module tính năng phụ thuộc lẫn nhau mà không có ngoại lệ. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Mô-đun Recovery của hệ thống bắt buộc phải tham chiếu đến Auth.DTOs và Auth.Services để phục vụ việc xác thực danh tính người dùng thực hiện khôi phục tài sản. Nếu cấm tuyệt đối, module Recovery sẽ không hoạt động hoặc phải viết lại toàn bộ logic xác thực trùng lặp (vi phạm DRY). |
| Em/nhóm phát hiện bằng cách nào? | Chạy test kiến trúc tự động phát hiện Recovery Services vi phạm ranh giới. |
| Em/nhóm đã sửa như thế nào? | Bổ sung logic ngoại lệ trong ModularBoundaryTests.cs: nếu module đang quét là Recovery, cho phép loại trừ (không cấm) tham chiếu đến Auth. |
| Bài học rút ra | Các lý thuyết thiết kế kiến trúc sạch (Clean Architecture) luôn cần được tùy biến linh hoạt dựa trên yêu cầu tích hợp nghiệp vụ thực tế của dự án. Không nên áp dụng máy móc 100% các quy tắc lý thuyết do AI sinh ra. |

---

## 9. Phân đóng góp thật sự của sinh viên/nhóm

```text
- Tinh chỉnh lớp kiểm thử kiến trúc ModularBoundaryTests.cs để cân bằng giữa tính cô lập module và tính thực tiễn của cơ sở dữ liệu dùng chung.
- Sửa lỗi namespaces trên hơn 50 tệp kiểm thử unit, integration, và benchmarks bị hỏng do thay đổi vị trí thư mục.
- Triển khai SecurityAlertNotice bảo mật gửi email cảnh báo khi kích hoạt OTP fallback xóa tài khoản.
- Viết 5 integration tests tự động cho deletion lifecycle.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Rút ngắn 50% thời gian thực hiện chuyển đổi cấu trúc thư mục quy mô lớn. |
| Code Quality | Good | Excellent | Code có cấu trúc rõ ràng (Modular Monolith) và được bảo vệ tự động bằng NetArchTest. |
| Testing | Good | Excellent | Tích hợp thêm Architecture Tests giúp kiểm soát thiết kế hệ thống bền vững lâu dài. |

---

## 11. Bài học về môn học

- Xóa tài khoản trong các ứng dụng thực tế đòi hỏi thiết kế quy trình hai giai đoạn (soft-delete → grace period → hard-delete) để bảo vệ quyền lợi người dùng và tuân thủ GDPR.
- Modular Monolith là bước đệm hoàn hảo trước khi chuyển dịch lên Microservices, giúp giữ codebase sạch nhưng vẫn duy trì hiệu năng cao của cơ sở dữ liệu monolith.
- NetArchTest là công cụ cực kỳ mạnh mẽ để tự động hóa việc thực thi các quy chuẩn thiết kế phần mềm trong các dự án lớn.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- AI không thể tự động cấu hình các ngoại lệ phụ thuộc phức tạp trong kiểm thử tĩnh. Sinh viên cần chủ động phân tích ranh giới nghiệp vụ thực tế để viết các quy tắc kiểm thử kiến trúc phù hợp.
- Refactor namespaces quy mô lớn bằng AI cần phải chạy lại toàn bộ unit/integration tests để đảm bảo tính toàn vẹn của mã nguồn.

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

- Yêu cầu AI khai báo các ngoại lệ kiến trúc ngay trong prompt thiết kế Modular Monolith đầu tiên để tối ưu hóa bộ test kiến trúc sinh ra.
- Thiết lập quy trình tự động sửa namespace (bằng scripts hoặc công cụ IDE) trước khi chạy refactor cấu trúc thư mục để tiết kiệm thời gian sửa thủ công.

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
Có. Nhóm hoàn toàn giải thích được luồng băm SHA-256 + salt ẩn danh hóa actor, cơ chế dọn dẹp assets lưu trữ trên R2, kiến trúc Modular Monolith và nguyên lý hoạt động của NetArchTest để kiểm thử boundaries.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc di chuyển files, định nghĩa các endpoints xóa tài khoản, và thiết lập ModularBoundaryTests hoàn toàn nằm trong khả năng của nhóm khi tra cứu tài liệu NetArchTest và Microsoft. AI chỉ đóng vai trò hỗ trợ tăng tốc công việc.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-06-01    |
