# Reflection

## 1. Quy định ghi nhận Reflection

Bản Reflection này ghi lại các bài học kinh nghiệm tự đúc kết của cá nhân hoặc nhóm sau khi hoàn thành phần công việc phát triển tính năng Trang hồ sơ doanh nghiệp công khai và Quản lý thư viện ảnh Gallery trên dự án CVerify.

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

## 3. Nội dung tự đánh giá & phản tư

### 3.1. Các bài học kỹ thuật đúc kết được

```text
1. Về quản lý lưu trữ đám mây và bảo mật tệp tin:
Việc sử dụng Signed URL thay vì đường dẫn bucket tĩnh là cách tiếp cận đúng đắn khi làm việc với lưu trữ đám mây. Signed URL có thời hạn hiệu lực, buộc client phải yêu cầu URL mới từ server sau một khoảng thời gian, từ đó ngăn chặn truy cập trái phép vào ảnh riêng tư. Bài học quan trọng là không bao giờ để lộ đường dẫn bucket trực tiếp trong response API.

2. Về cập nhật Database schema an toàn:
Khi triển khai tính năng mới cần thêm cột vào bảng hiện có, cách an toàn nhất là kiểm tra sự tồn tại của cột trong information_schema trước khi gọi ALTER TABLE. Phương pháp này đảm bảo việc chạy lại DbInitializer nhiều lần không gây lỗi và hoàn toàn tương thích với dữ liệu hiện tại trên môi trường production.

3. Về thiết kế giao diện hồ sơ công ty:
Trang hồ sơ công ty là điểm tiếp xúc đầu tiên giữa tổ chức và ứng viên tiềm năng, nên bố cục cần ưu tiên các thông tin quan trọng nhất (banner, logo, tên, mô tả) ở phần đầu. Lưới ảnh gallery nên hỗ trợ lazy-loading để tối ưu tốc độ tải trang, đặc biệt khi số lượng ảnh lớn.
```

---

### 3.2. Những khó khăn/thử thách đã vượt qua

```text
Thử thách lớn nhất là đảm bảo tính toàn vẹn dữ liệu khi xử lý các trường mảng chuỗi (List<string>) trong Entity Framework với PostgreSQL. Cấu hình HasColumnType("text[]") trong ApplicationDbContext cần phải khớp chính xác với kiểu cột trong database, nếu không EF sẽ không thể deserialize dữ liệu đọc về. Ngoài ra, khi nhận danh sách URL ảnh từ API trả về, cần phòng thủ tốt với trường hợp null hoặc mảng rỗng ở phía Frontend để tránh lỗi crash khi render.
```

---

### 3.3. Đánh giá tính hữu ích của AI

```text
Công cụ AI (Antigravity) hỗ trợ rất hiệu quả trong việc:
- Sinh mã SQL DDL an toàn cho DbInitializer với pattern kiểm tra cột tồn tại trước khi thêm mới.
- Đề xuất cấu trúc DTO đầy đủ và nhất quán giữa Backend và Frontend.
- Xây dựng nhanh các component React có bố cục phức tạp (banner overlay, gallery grid) mà không cần tra cứu tài liệu thủ công.
Nhờ AI, thời gian phát triển tính năng full-stack này giảm ước tính khoảng 50% so với làm thủ công hoàn toàn.
```

---

### 3.4. Kế hoạch cải tiến bản thân/nhóm

```text
1. Về kiến trúc Backend: Nghiên cứu thêm về CQRS pattern để tách biệt rõ ràng các lệnh đọc và ghi dữ liệu, giúp code dễ mở rộng và test hơn trong các tính năng phức tạp tiếp theo.
2. Về Frontend: Tìm hiểu về React Suspense và lazy loading cho các component nặng như gallery ảnh để cải thiện trải nghiệm tải trang ban đầu.
3. Về quy trình: Duy trì thói quen viết audit log ngay sau khi hoàn thành từng tính năng, không để tích lũy nhiều rồi mới ghi một lúc.
```

---

## 4. Cam kết học thuật và phản tư trung thực

Sinh viên/nhóm cam kết rằng những đánh giá và đúc kết trên đây là trung thực, phản ánh đúng trải nghiệm học tập và làm việc thực tế của nhóm trong quá trình thực hiện giai đoạn này của dự án.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
