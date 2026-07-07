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
| Ngày hoàn thành reflection | 2026-06-16                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình triển khai phân hệ liên kết tài khoản lập trình ngoài và dọn dẹp phiên làm việc khi AFK, AI đã hỗ trợ thiết lập khung boilerplate của database migration, cấu trúc gọi API GitHub/GitLab, cấu hình layouts CSS grid và API DatePicker của HeroUI. Sinh viên đóng vai trò kiến trúc sư chính để điều chỉnh các giải pháp kỹ thuật phức tạp bao gồm: cấu hình mã hóa an toàn AES-GCM cho OAuth tokens, tùy chỉnh bộ lọc ngày sinh (Date Validation) ngăn chặn nhập ngày bất hợp lệ, thiết kế hiệu ứng hover vi mô mượt mà khớp tiêu chuẩn UI tĩnh, và phát hiện lỗ hổng bảo mật session logout khi Access Token hết hạn, xử lý triệt để bằng cách chuyển sang AllowAnonymous kết hợp thu hồi refresh token trực tiếp từ cookie.
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
Antigravity cho phép can thiệp đồng thời vào mã nguồn C# ở Backend, Next.js ở Frontend, quản trị migrations của EF Core, và kiểm soát lịch sử commit một cách thống nhất trên cùng một workspace, tăng tốc độ kiểm định tích hợp.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [ ] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
- [x] Review code
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới
- 

### Mô tả chi tiết

```text
AI hỗ trợ viết các lớp gọi REST API của GitHub/GitLab, sinh mã migration thêm bảng ExternalOrganization, cung cấp cách cấu hình thuộc tính của HeroUI DatePicker, và phân tích luồng di chuyển của middleware authentication/authorization để tìm ra nguyên nhân request logout bị từ chối 401 khi Access Token hết hạn.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu rõ cơ chế xử lý phân cấp (Pipeline) của Middleware trong ASP.NET Core: Luồng chạy của Authentication chạy trước Authorization, và tại sao việc gắn attribute [Authorize] sẽ chặn đứng request trước khi chạm đến Controller.
- Tiếp cận cách thiết kế giao diện theo tiêu chuẩn HeroUI (cách import, quản lý state ngày tháng qua các đối tượng Date của HeroUI).
- Ý thức được sự quan trọng của việc mã hóa dữ liệu nhạy cảm của người dùng (OAuth access tokens) trước khi ghi vào cơ sở dữ liệu công cộng.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đề xuất dọn dẹp cookies bằng JavaScript ở phía client khi logout, gợi ý này hoàn toàn bất khả thi vì các cookies bảo mật của hệ thống đều là HttpOnly (trình duyệt cấm JavaScript đọc/ghi).
- AI import sai đường dẫn component của HeroUI và sử dụng các API cũ của thư viện do dữ liệu huấn luyện chưa cập nhật các phiên bản phát hành mới nhất của HeroUI.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm chủ yếu sử dụng AI để sinh code mẫu (boilerplate) và phân tích các trường hợp lỗi phức tạp. Toàn bộ khâu kiểm chứng tính đúng đắn, mã hóa tokens bằng thuật toán AES-GCM, viết validation logic chặn tuổi nhỏ hơn 15 cho DatePicker, và kiểm định bảo mật phiên làm việc hoàn toàn do thành viên nhóm tự triển khai.
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
Quá trình kiểm chứng được thực hiện qua các bước:
1. Thực hiện chạy thử nghiệm AFK auto-kick thực tế: Chờ 15 phút không thao tác, kiểm tra xem cookies có bị dọn sạch khỏi Application Tab trong Chrome Developer Tools sau khi tự động chuyển hướng về trang login hay không. Reload trang để đảm bảo không bị tự động login lại.
2. Thử nghiệm liên kết tài khoản GitHub chứa hơn 80 repositories để kiểm tra tính năng phân trang và khả năng lấy danh sách tổ chức ngoài.
3. Kiểm tra mã nguồn trong Database: Đảm bảo trường lưu trữ Access Token của ứng viên được mã hóa thành các ký tự hex ngẫu nhiên (chỉ được giải mã trong bộ nhớ RAM khi gọi API ngoài).
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý dọn dẹp cookies phiên làm việc bằng cách gán `document.cookie = "access_token=; expires=..."` trực tiếp ở React Hook logout. |
| Em/nhóm đã kiểm tra bằng cách nào? | Chạy thử tính năng logout và kiểm tra danh sách Cookies lưu trên trình duyệt Chrome. |
| Kết quả kiểm tra | Cookies `access_token` và `refresh_token` vẫn còn nguyên do được đánh dấu thuộc tính `HttpOnly` ở server, dẫn đến reload trang vẫn tự động phục hồi phiên. |
| Em/nhóm đã xử lý tiếp như thế nào? | Thay đổi kiến trúc: Chuyển endpoint logout thành `[AllowAnonymous]`, gọi API logout bất kể trạng thái token hết hạn để server gửi phản hồi `Set-Cookie` dọn dẹp sạch sẽ từ phía backend. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Viết hàm dọn dẹp cookies bằng JavaScript ở phía client Next.js. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Vi phạm nguyên tắc bảo mật HttpOnly. JS client không được quyền thao tác với cookies chứa phiên đăng nhập để tránh các cuộc tấn công XSS. |
| Em/nhóm phát hiện bằng cách nào? | Chạy kiểm thử thực tế và thấy cookies không hề bị biến mất ở tab Application. |
| Em/nhóm đã sửa như thế nào? | Cấu hình API logout trên backend xử lý việc phản hồi xóa cookie, và mở rộng endpoint này cho phép truy cập ẩn danh (AllowAnonymous) để xử lý tình huống Access Token đã hết hạn. |
| Bài học rút ra | Các cơ chế bảo mật (như cookie HttpOnly) là chốt chặn quan trọng không được phép phá vỡ. Mọi hành động xóa/sửa phiên làm việc phải được thực hiện thông qua kênh Backend kiểm soát thay vì dựa dẫm vào Client. |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
- Triển khai thuật toán mã hóa AES-GCM bảo vệ khóa bảo mật OAuth của ứng viên.
- Viết Date Validation cho bộ chọn ngày sinh HeroUI DatePicker.
- Phân tích và sửa đổi endpoint logout thành AllowAnonymous kết hợp cơ chế đọc và hủy refresh token cụ thể dựa trên cookies gửi lên.
- Tự căn chỉnh layout Grid 2 cột và hoàn thiện responsive cho các phần tử quản trị CV.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Tiết kiệm ~35% thời gian viết code gọi các APIs tích hợp ngoài và cấu trúc trang giao diện cài đặt. |
| Security Resilience | Basic | Solid | Phát hiện và vá thành công lỗi logic bỏ sót dọn dẹp cookies khi tự động đăng xuất do AFK. |
| UI Standardization | Manual | Unified | Đồng bộ hóa toàn bộ date inputs sang HeroUI DatePicker, tránh lỗi sai định dạng ngày tháng. |

---

## 11. Bài học về môn học

- Khi thiết kế hệ thống bảo mật bằng JWT và Refresh Token qua Cookies, phải luôn đảm bảo rằng API đăng xuất (Logout) có thể tiếp cận được kể cả khi JWT hết hạn để tránh việc kẹt cookies trên client.
- Tích hợp dịch vụ bên thứ ba đòi hỏi quản lý an toàn thông tin nghiêm ngặt, dữ liệu nhạy cảm (OAuth tokens) phải được mã hóa trước khi đi vào bộ lưu trữ vĩnh viễn.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Phải luôn kiểm định các đề xuất liên quan đến dọn dẹp cookies và bảo mật của AI. AI thường đưa ra giải pháp tiện lợi nhất (xóa ở client) thay vì giải pháp bảo mật nhất (xử lý ở server).
- Cần đối chiếu kỹ lưỡng cú pháp thư viện UI với tài liệu chính thống của phiên bản đang sử dụng để tránh lỗi biên dịch do AI sinh mã phiên bản cũ.

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

- Chủ động cung cấp đặc tả bảo mật (như "cookies are HttpOnly") ngay trong prompt dọn dẹp phiên để AI đưa ra kiến trúc xóa cookie từ server thay vì client.
- Yêu cầu AI viết các component UI có chỉ rõ phiên bản thư viện hiện tại để tránh sinh mã lỗi thời.

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
Có. Nhóm giải thích rõ được cơ chế hoạt động của middleware JWT và lý do endpoint logout phải là AllowAnonymous để đảm bảo luồng dọn dẹp cookies diễn ra suôn sẻ khi JWT hết hạn do AFK. Nhóm cũng giải thích rõ được quy trình mã hóa AES-GCM cho OAuth tokens ở backend.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc viết các lớp API Client gọi REST API của GitHub/GitLab, cấu hình Entity Framework, hay thiết lập giao diện Next.js đều là những kỹ năng phát triển phần mềm cơ bản đã được trang bị đầy đủ.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-16    |
