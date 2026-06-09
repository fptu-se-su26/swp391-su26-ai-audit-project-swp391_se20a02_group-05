# Reflection

## 1. Quy định ghi nhận Reflection

Bản Reflection này ghi lại các bài học kinh nghiệm tự đúc kết của cá nhân hoặc nhóm sau khi hoàn thành phần công việc tinh chỉnh giao diện, tiền tệ động và tích hợp cơ chế thanh trạng thái thay đổi chưa lưu trên dự án CVerify.

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
| Ngày bắt đầu | 2026-06-09T01:05:00Z |
| Ngày hoàn thành | 2026-06-09T01:45:00Z |

---

## 3. Nội dung tự đánh giá & phản tư

### 3.1. Các bài học kỹ thuật đúc kết được

```text
1. Về thiết kế giao diện responsive và thứ tự cấu trúc:
Việc sắp xếp các thẻ thông tin tổng quan, đặc biệt là các widget thông tin phân tích do AI sinh ra cạnh nhau giúp trang web trở nên cân đối và tận dụng tối đa chiều rộng màn hình máy tính để bàn (Desktop). Bố cục này cũng đòi hỏi cấu trúc CSS Grid responsive (`grid-cols-1 md:grid-cols-2`) để đảm bảo giao diện hiển thị gọn gàng, tự động xếp dọc trơn tru khi người dùng truy cập từ điện thoại.

2. Về linh hoạt hóa dữ liệu tiền tệ động:
Sử dụng các component InputGroup lồng ghép Prefix và Suffix cho phép giao diện linh động đổi dạng tiền tệ (USD/VND) mà không ảnh hưởng tới dữ liệu số lưu dưới cơ sở dữ liệu. Điều này mang lại trải nghiệm chuyên nghiệp cho người dùng ở mọi quốc gia mà vẫn đảm bảo DTO Backend giữ nguyên tính nhất quán, không tốn thêm tài nguyên chuyển đổi tỷ giá phức tạp.

3. Về kiểm soát CSS Override từ thư viện ngoài:
Một số thư viện UI (như HeroUI/NextUI) có cơ chế thiết lập thuộc tính mặc định cho các nút ở trạng thái disabled rất mạnh (độ ưu tiên cao). Việc hiểu rõ cách hoạt động của Tailwind CSS custom state (như `data-[disabled=true]`) giúp giải quyết các lỗi giao diện không thể ghi đè bằng class `disabled:` thông thường.
```

---

### 3.2. Những khó khăn/thử thách đã vượt qua

```text
Thử thách lớn nhất trong đợt cập nhật này là tinh chỉnh thanh sticky UnsavedChangesBar hoạt động đồng bộ với React Hook Form. Khác với các nút bấm tĩnh, UnsavedChangesBar là một component con sử dụng ngữ cảnh (Context) để tự theo dõi trạng thái dirty của form. Để đảm bảo tính chính xác, chúng tôi đã cấu trúc hàm handleSaveChanges và handleReset chuẩn chỉ, tự động chạy kiểm thử tính hợp lệ của toàn bộ form (`methods.trigger()`) trước khi gửi request lưu. Khó khăn thứ hai là việc đè màu xám mặc định của nút Add khi bị disabled thành màu trắng, đã được giải quyết triệt để nhờ viết chuỗi selector Tailwind chi tiết.
```

---

### 3.3. Đánh giá tính hữu ích của AI

```text
Hệ thống AI (Antigravity) chứng tỏ vai trò quan trọng trong việc nhanh chóng tìm ra các giải pháp CSS selector chính xác để giải quyết lỗi ghi đè giao diện nút bấm disabled, đồng thời hỗ trợ sinh mẫu JSX bọc InputGroup động một cách nhanh chóng. Sự hỗ trợ từ AI giảm thiểu đáng kể thời gian gõ code boilerplate lặp đi lặp lại và các lỗi đánh máy, nâng cao đáng kể hiệu suất lập trình của chúng tôi.
```

---

### 3.4. Kế hoạch cải tiến bản thân/nhóm

```text
1. Về kỹ năng thiết kế UI/UX: Tiếp tục tìm hiểu sâu hơn các chuẩn thiết kế và tương tác cao cấp, hạn chế việc sử dụng quá nhiều các ô cửa sổ modal xác nhận không cần thiết giúp người dùng thao tác nhanh hơn.
2. Về phong cách lập trình: Duy trì viết code tường minh, có chú thích đầy đủ cho các khối mã nghiệp vụ quan trọng và đảm bảo linter luôn sạch sẽ.
```

---

## 4. Cam kết học thuật và phản tư trung thực

Sinh viên/nhóm cam kết rằng những đánh giá và đúc kết trên đây là trung thực, phản ánh đúng trải nghiệm học tập và làm việc thực tế của nhóm trong quá trình thực hiện giai đoạn này của dự án.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 09/06/2026 |
