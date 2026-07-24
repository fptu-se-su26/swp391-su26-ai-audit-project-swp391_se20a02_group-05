# Prompt Log

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
| Ngày bắt đầu | 2026-07-19 |
| Ngày cập nhật gần nhất | 2026-07-19 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [x] Claude Code
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 3. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? |
|---:|---|---|---|---|---|---|
| 1 | 2026-07-19 | Claude Code | Sửa cảnh báo xUnit2002 | Sửa cảnh báo xUnit2002 trên test Guid trong ExtendedRepositorySyncQueueTests.cs | Assert.NotNull → Assert.NotEqual(Guid.Empty, ...) | Có |
| 2 | 2026-07-19 | Claude Code | Giải quyết merge conflict | Merge fix xUnit2002 vào CVerify-uat, giữ nguyên các thay đổi song song khác | Commit merge 97139ee | Có |

---

## 4. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-19 |
| Công cụ AI | Claude Code |
| Mục đích | Fix xUnit2002 analyzer warning |
| Mức độ sử dụng | AI thực hiện theo yêu cầu sửa cảnh báo cụ thể của sinh viên |

#### Prompt nguyên văn (tóm tắt)

```text
Sửa cảnh báo xUnit2002 trên test Guid trong ExtendedRepositorySyncQueueTests.cs.
```

#### Kết quả chính

```text
Assert.NotNull(queueId) (vô nghĩa với value type Guid) được thay bằng
Assert.NotEqual(Guid.Empty, queueId).
```

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-19 |
| Công cụ AI | Claude Code |
| Mục đích | Merge conflict resolution |
| Mức độ sử dụng | AI hỗ trợ rà soát conflict, sinh viên xác nhận phần giữ lại |

#### Prompt nguyên văn (tóm tắt)

```text
Merge nhánh chứa fix Assert.NotEqual vào CVerify-uat, giữ nguyên fix xUnit2002 và
không làm mất các thay đổi khác đang merge cùng lúc (PR #110, PR #111).
```

#### Kết quả chính

```text
Conflict resolved, giữ lại fix xUnit2002, hợp nhất thành công các thay đổi song
song khác vào commit merge 97139ee.
```
