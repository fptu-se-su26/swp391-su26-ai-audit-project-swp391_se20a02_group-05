# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-07-19 |

---

## 2. Tóm tắt quá trình sử dụng AI

```text
Phiên làm việc tập trung vào tài liệu pháp lý (licensing), không thay đổi
chức năng phần mềm:

1. Xác định LICENSE hiện tại của CVerify là GPL-3.0 — mâu thuẫn với mục tiêu
   Proprietary All Rights Reserved.
2. Dùng Cursor để thay LICENSE, thêm LEGAL.md / NOTICE / COPYRIGHT, và tách
   thành nhiều commit logic theo yêu cầu review.
3. Giữ nguyên README.md và toàn bộ source code theo ràng buộc của prompt.
4. Hoàn tất push, PR checklist, và gán reviewer.

Bài học: tài liệu pháp lý cần nhất quán giữa LICENSE gốc repo và LICENSE
trong thư mục sản phẩm; third-party dependencies vẫn giữ license riêng.
```

---

## 3. Công cụ AI đã sử dụng

- [x] Cursor

### Công cụ được sử dụng nhiều nhất

```text
Cursor (Composer)
```

### Lý do sử dụng

```text
Prompt yêu cầu thao tác git nhiều bước (checkout/pull, 4–5 commit, push, PR,
assign reviewer) trên Windows PowerShell; Cursor phù hợp để thực thi end-to-end
trong khi vẫn giữ ràng buộc không sửa README.md / source code.
```

---

## 4. Đánh giá hiệu quả

| Tiêu chí | Nhận xét |
|---|---|
| Độ chính xác | Cao đối với phạm vi documentation-only |
| Rủi ro | Thấp — không đụng runtime code |
| Việc sinh viên vẫn kiểm soát | Prompt ràng buộc commit tách bước, reviewer, và không merge PR |
