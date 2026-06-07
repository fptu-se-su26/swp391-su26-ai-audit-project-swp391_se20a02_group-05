# Reflection

## 1. Quy định ghi nhận Reflection

Bản Reflection này ghi lại các bài học kinh nghiệm tự đúc kết của cá nhân hoặc nhóm sau khi hoàn thành phần công việc phát triển tính năng Career Preferences mở rộng và chuẩn hóa dữ liệu của dự án CVerify.

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
| Ngày bắt đầu | 2026-06-06T05:00:00Z |
| Ngày hoàn thành | 2026-06-06T06:00:00Z |

---

## 3. Nội dung tự đánh giá & phản tư

### 3.1. Các bài học kỹ thuật đúc kết được

```text
1. Về thiết kế Backend và CSDL:
Khi làm việc với các kiểu dữ liệu mảng tag (danh sách kỹ năng, địa điểm, hình thức làm việc), việc lưu trữ thô trực tiếp xuống CSDL rất dễ phát sinh dữ liệu rác. Giải pháp chuẩn hóa dữ liệu bằng hàm tập trung (ValidateAndNormalizeTags) trên Server là bắt buộc để đảm bảo an toàn hệ thống thông tin. Nó giúp loại bỏ hoàn toàn các chuỗi rỗng, chuẩn hóa viết hoa viết thường, loại bỏ khoảng trắng dư thừa và khống chế độ dài ký tự tối đa của tag.

2. Về tương tác UI và Validation trên Frontend:
Việc kiểm thực dữ liệu (Data Validation) bằng Zod kết hợp React Hook Form giúp đồng bộ hóa các lỗi nghiệp vụ ngay từ phía client. Zod schema `.refine()` là một công cụ mạnh mẽ để viết các điều kiện logic phức tạp (như so sánh lương tối thiểu không được lớn hơn lương tối đa) thay vì chỉ sử dụng các thuộc tính validate mặc định.

3. Nâng cấp trải nghiệm người dùng bằng phím tắt:
Với các component chọn tag dạng chip, việc cho phép người dùng nhấn phím Enter hoặc phím dấu phẩy (,) để thêm nhanh từ khóa giúp thao tác diễn ra liên tục, tạo cảm giác chuyên nghiệp như một ứng dụng web cao cấp.
```

---

### 3.2. Những khó khăn/thử thách đã vượt qua

```text
Thử thách lớn nhất là ánh xạ các thông điệp lỗi chi tiết từ Web API Backend phản hồi về giao diện Next.js Client. Nếu chỉ báo lỗi chung chung 400 Bad Request, người dùng sẽ không biết trường nào nhập sai. Chúng tôi đã xây dựng cơ chế phân loại lỗi nghiệp vụ rõ ràng, từ đó lọc mảng lỗi trả về và gọi phương thức `setError` của react-hook-form tương ứng cho các trường nhập liệu cụ thể như `minSalary` hay `skills`.
```

---

### 3.3. Đánh giá tính hữu ích của AI

```text
Công cụ AI (Antigravity) cực kỳ hữu ích trong việc sinh các cấu trúc mã có tính rập khuôn và logic chuẩn hóa chuỗi hoặc kiểm thử điều kiện trong Zod. Nhờ AI sinh mẫu schema refinement và hàm LINQ chuẩn hóa tag, chúng tôi đã tiết kiệm được 60% thời gian nghiên cứu và viết code thử nghiệm, tập trung nhiều hơn vào việc tối ưu giao diện Public Profile và kiểm chứng nghiệp vụ.
```

---

### 3.4. Kế hoạch cải tiến bản thân/nhóm

```text
1. Về kỹ năng lập trình: Sẽ tiếp tục học hỏi các mẫu thiết kế (design patterns) nâng cao để xử lý các nghiệp vụ phức tạp ở Backend, giữ các Controller luôn mỏng và tập trung logic vào Service Layer.
2. Về quản lý mã nguồn: Đảm bảo kiểm tra kỹ linter (npm run lint) trước khi commit để tránh việc làm gián đoạn luồng CI/CD của nhóm.
```

---

## 4. Cam kết học thuật và phản tư trung thực

Sinh viên/nhóm cam kết rằng những đánh giá và đúc kết trên đây là trung thực, phản ánh đúng trải nghiệm học tập và làm việc thực tế của nhóm trong quá trình thực hiện giai đoạn này của dự án.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 06/06/2026 |
