# Reflection

## 1. Quy định ghi nhận Reflection

Bản Reflection này ghi lại các bài học kinh nghiệm tự đúc kết của cá nhân hoặc nhóm sau khi hoàn thành phần công việc phát triển tính năng Cấu hình Doanh nghiệp và Quản lý Thành viên/Vai trò trên dự án CVerify.

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
| Ngày bắt đầu | 2026-06-13T07:30:00Z |
| Ngày hoàn thành | 2026-06-13T14:30:00Z |

---

## 3. Nội dung tự đánh giá & phản tư

### 3.1. Các bài học kỹ thuật đúc kết được

```text
1. Về truy vấn liên kết cơ sở dữ liệu:
Khi một thực thể cần hiển thị thông tin đi kèm từ thực thể khác (như Membership đi kèm Headline/Username từ Profile), việc sử dụng truy vấn Linq Join thay vì truy vấn tuần tự (N+1 query problem) là cực kỳ quan trọng để đảm bảo thời gian phản hồi của hệ thống.

2. Về đồng bộ hóa DTO:
Sử dụng các phương thức chuyển đổi (MapToWorkspaceDetailsDto) tập trung giúp code controller sạch hơn, dễ bảo trì và tránh viết lặp đi lặp lại logic gán thuộc tính thủ công ở nhiều endpoint khác nhau.

3. Về trải nghiệm người dùng quản trị:
Nút quay lại bảng điều khiển ("Back to Dashboard") và tab điều hướng trực quan mang lại luồng thao tác logic và mượt mà cho đại diện doanh nghiệp khi chuyển đổi giữa xem hồ sơ công khai và quay lại quản trị nội bộ.
```

---

### 3.2. Những khó khăn/thử thách đã vượt qua

```text
Khó khăn lớn nhất là cấu hình cơ chế Left Join trong Linq (dùng DefaultIfEmpty) để API thành viên không bị crash khi có tài khoản chưa điền thông tin Profile (dữ liệu Profile bị null). Việc gán các giá trị mặc định cho avatar và headline khi các giá trị này không tồn tại đã giải quyết triệt để vấn đề này, đảm bảo giao diện hiển thị ổn định.
```

---

### 3.3. Đánh giá tính hữu ích của AI

```text
AI hỗ trợ đắc lực trong việc:
- Đề xuất câu truy vấn Left Join tối ưu bằng C# Linq.
- Sinh cấu trúc các DTO mở rộng nhất quán và chính xác.
- Tái cấu trúc nhanh component Frontend từ People sang Members, tối ưu hóa CSS cho bố cục hiển thị dạng thẻ.
Nhờ đó thời gian phát triển được rút ngắn đáng kể, đặc biệt ở khâu thiết kế query DB phức tạp.
```

---

### 3.4. Kế hoạch cải tiến bản thân/nhóm

```text
1. Tìm hiểu sâu hơn về cơ chế tối ưu hóa bộ nhớ đệm (Caching) cho các danh sách thành viên doanh nghiệp ít thay đổi để giảm tải cho DB.
2. Cải tiến giao diện quản lý vai trò giúp phân quyền trực quan hơn trên giao diện Client.
```

---

## 4. Cam kết học thuật và phản tư trung thực

Sinh viên/nhóm cam kết rằng những đánh giá và đúc kết trên đây là trung thực, phản ánh đúng trải nghiệm học tập và làm việc thực tế của nhóm trong quá trình thực hiện giai đoạn này của dự án.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 13/06/2026 |
