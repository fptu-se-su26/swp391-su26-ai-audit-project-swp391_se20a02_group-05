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
Phiên làm việc nhỏ, tập trung vào chất lượng test:

1. Analyzer xUnit báo cảnh báo xUnit2002 trên ExtendedRepositorySyncQueueTests.cs
   vì Assert.NotNull được gọi trên Guid — một value type luôn khác null nên
   assertion này không bao giờ có thể fail, làm mất ý nghĩa kiểm thử.
2. Dùng Claude Code để xác nhận nguyên nhân cảnh báo và thay bằng
   Assert.NotEqual(Guid.Empty, queueId) — assertion thật sự kiểm tra queue ID
   được cấp phát khác giá trị rỗng.
3. Khi push fix lên, CVerify-uat đã có các merge song song khác (PR #110, #111)
   nên phát sinh conflict. Dùng Claude Code rà soát các file conflict, xác nhận
   giữ lại đúng fix xUnit2002 trong khi hợp nhất các thay đổi khác không liên quan.

Bài học: cảnh báo analyzer tưởng nhỏ (xUnit2002) thường chỉ ra một assertion vô
nghĩa che giấu khả năng phát hiện lỗi thật của test; sửa đúng gốc (đổi assertion)
tốt hơn suppress cảnh báo.
```

---

## 3. Công cụ AI đã sử dụng

- [x] Claude Code

### Công cụ được sử dụng nhiều nhất

```text
Claude Code
```

### Lý do sử dụng

```text
Công việc gồm cả sửa code C# và thao tác git (merge conflict) trên cùng một phiên
làm việc; Claude Code phù hợp để thực hiện end-to-end trong terminal mà không cần
chuyển đổi công cụ.
```

---

## 4. Đánh giá hiệu quả

| Tiêu chí | Nhận xét |
|---|---|
| Độ chính xác | Cao — cảnh báo analyzer xác định rõ vị trí và nguyên nhân |
| Rủi ro | Thấp — thay đổi chỉ nằm trong test, không ảnh hưởng logic nghiệp vụ |
| Việc sinh viên vẫn kiểm soát | Xác nhận thủ công phần giữ lại khi giải quyết merge conflict, không để AI tự động chọn |
