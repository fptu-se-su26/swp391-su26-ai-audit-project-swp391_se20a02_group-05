# Changelog

## 1. Thông tin project

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
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-07-19 |
| Ngày hoàn thành | 2026-07-19 |

---

## 2. Tổng quan giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| xUnit2002 test fix | 2026-07-19 | Sửa cảnh báo xUnit2002 (`Assert.NotNull` trên value type `Guid`) và giải quyết conflict khi merge vào `CVerify-uat` | Completed |

---

## 3. Chi tiết thay đổi

### 2026-07-19 — Fix xUnit2002 warning trên `ExtendedRepositorySyncQueueTests`

| Mục | Nội dung |
|---|---|
| Người thực hiện | Trần Nhất Long (với hỗ trợ Claude Code) |
| Loại chi thay đổi | Test |
| Mô tả | `Guid` là value type nên `Assert.NotNull(queueId)` luôn pass và bị analyzer xUnit gắn cảnh báo xUnit2002 (assert vô nghĩa). Thay bằng `Assert.NotEqual(Guid.Empty, queueId)` để assert có ý nghĩa thật: xác nhận queue ID được cấp phát khác giá trị rỗng. |
| Files | `CVerify/CVerify.Core/tests/CVerify.API.UnitTests/BackgroundWorkers/ExtendedRepositorySyncQueueTests.cs` |
| AI hỗ trợ | Có — Claude Code hỗ trợ xác định lỗi analyzer và viết lại assertion |
| Minh chứng | Commit `dd56860` trên `CVerify-uat` |

### 2026-07-19 — Merge conflict resolution khi đồng bộ `CVerify-uat`

| Mục | Nội dung |
|---|---|
| Người thực hiện | Trần Nhất Long (với hỗ trợ Claude Code) |
| Loại thay đổi | Merge/Fix |
| Mô tả | Khi merge nhánh `dd56860` với các thay đổi song song khác trên `CVerify-uat` (PR #110, PR #111), phát sinh conflict. Giữ lại fix `Assert.NotEqual` cho xUnit2002 và hợp nhất các thay đổi khác (test mở rộng, tài liệu kiến trúc, audit packages của thành viên khác) không xung đột về mặt logic. |
| Files | `CVerify/CVerify.Core/Modules/Shared/Security/UsernameService.cs` và các file test/tài liệu đi kèm merge |
| AI hỗ trợ | Có — Claude Code hỗ trợ rà soát và giải quyết conflict |
| Minh chứng | Commit `97139ee` trên `CVerify-uat` |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit | dd56860 | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/dd56860 |
| Commit | 97139ee | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/97139ee |
| Branch | CVerify-uat | Cả 2 commit được push lên CVerify-uat |

## Ghi chú

```text
xUnit2002 là cảnh báo analyzer của bộ test xUnit: Assert.NotNull/Assert.Null chỉ có
ý nghĩa với reference type hoặc nullable value type. Guid là struct (value type)
nên Assert.NotNull(queueId) không bao giờ fail — assertion vô nghĩa, che giấu khả
năng phát hiện lỗi thật. Assert.NotEqual(Guid.Empty, queueId) diễn đạt đúng ý định
kiểm thử: queue ID được cấp phát phải khác giá trị mặc định/rỗng.
```

---

# 4. Tổng kết thay đổi cuối phase

## 4.1. Các chức năng đã hoàn thành

```text
- Sửa xong cảnh báo xUnit2002 trên ExtendedRepositorySyncQueueTests.
- Merge fix vào CVerify-uat không mất thay đổi song song của thành viên khác.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
- Không rà soát toàn bộ codebase để tìm các trường hợp Assert.NotNull/Assert.Null
  khác trên value type có thể mắc cùng lỗi xUnit2002 — phạm vi pack này chỉ giới
  hạn ở file được báo cáo.
```

---

## 4.3. Cải thiện chính

```text
Sửa đúng gốc cảnh báo analyzer thay vì suppress cảnh báo, giữ cho bộ test có giá
trị kiểm thử thật thay vì chỉ để pass CI.
```

---

## 4.4. Tổng kết phase

```text
Thay đổi nhỏ, tập trung, không ảnh hưởng logic nghiệp vụ — thuộc nhóm dọn dẹp chất
lượng test trước khi tiếp tục các thay đổi khác trên CVerify-uat.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
- Quét toàn bộ test suite để tìm các cảnh báo xUnit2002 tương tự trên value type
  khác (ví dụ enum, struct tùy chỉnh).
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trần Nhất Long | 19/07/2026 |
