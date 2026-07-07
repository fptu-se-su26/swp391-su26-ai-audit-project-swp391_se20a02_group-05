# Reflection

## 1. Quy định ghi nhận Reflection

Bản Reflection này ghi lại các bài học kinh nghiệm tự đúc kết của cá nhân hoặc nhóm sau khi hoàn thành phần công việc phát triển tính năng Bài đăng và Tin tuyển dụng Doanh nghiệp trên dự án CVerify.

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
| Ngày bắt đầu | 2026-06-14T00:00:00Z |
| Ngày hoàn thành | 2026-06-14T17:50:00Z |

---

## 3. Nội dung tự đánh giá & phản tư

### 3.1. Các bài học kỹ thuật đúc kết được

```text
1. Về quản lý cơ sở dữ liệu EF Core Migrations:
Khi tạo thực thể DB mới có quan hệ phức tạp, việc tạo Migration tường minh và kiểm tra kỹ file model snapshot giúp kiểm soát tính đúng đắn của schema trước khi chạy lệnh update DB. Bài học đúc kết được là luôn kiểm tra kiểu dữ liệu các cột mảng (array) đặc trưng của PostgreSQL như text[].

2. Về thiết kế luồng tương tác trên ứng dụng:
Việc sử dụng Drawer trượt từ cạnh phải màn hình thay vì Modal ở trung tâm giúp nâng cao UX rõ rệt cho trang tuyển dụng. Ứng viên vừa đọc được chi tiết công việc cụ thể, vừa dễ dàng đối chiếu hoặc chuyển đổi sang các tin tuyển dụng khác mà không phải đóng/mở cửa sổ nhiều lần.

3. Về phản hồi trạng thái ứng dụng:
Tích hợp Toast notification thông báo kết quả tức thời khi ứng viên click "Apply" hoặc "Save" giúp giao diện có phản hồi trực quan, tạo cảm giác an tâm cho người dùng rằng thao tác của họ đã được xử lý.
```

---

### 3.2. Những khó khăn/thử thách đã vượt qua

```text
Khó khăn lớn nhất là sửa lỗi vòng lặp vô hạn `getSnapshot` do Zustand sinh ra khi đăng ký lắng nghe state trong Next.js (lỗi call stack 21). Việc giải quyết bằng cách ghi nhớ (cache) kết quả lấy snapshot hoặc tối ưu dependency array trong hook `useSyncExternalStore` đã khắc phục triệt để lỗi crash giao diện, đảm bảo website tải trang mượt mà.
```

---

### 3.3. Đánh giá tính hữu ích của AI

```text
AI hỗ trợ cực kỳ hiệu quả ở các phần việc:
- Tự động sinh mã Migration đầy đủ cho 2 bảng dữ liệu lớn.
- Đề xuất layout giao diện Drawer tuyển dụng và Modal tạo thông báo trực quan, Responsive tốt.
- Định hình nhanh các Zustand store action đồng bộ với API Backend.
Rút ngắn khoảng 60% thời gian thực thi của lập trình viên và nâng cao chất lượng code.
```

---

### 3.4. Kế hoạch cải tiến bản thân/nhóm

```text
1. Nghiên cứu sâu về phân quyền theo vai trò động (Dynamic Role-based Access Control) để kiểm soát quyền đăng tin/đăng bài linh hoạt hơn.
2. Tích hợp thư viện tải lên file CV trực tiếp lên AWS S3 và quét mã độc trước khi lưu trữ để bảo vệ hệ thống.
```

---

## 4. Cam kết học thuật và phản tư trung thực

Sinh viên/nhóm cam kết rằng những đánh giá và đúc kết trên đây là trung thực, phản ánh đúng trải nghiệm học tập và làm việc thực tế của nhóm trong quá trình thực hiện giai đoạn này của dự án.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 14/06/2026 |
