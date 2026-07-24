# AI Audit Log

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
| Ngày hoàn thành | 2026-07-19 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [x] Claude Code
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Sửa cảnh báo analyzer xUnit2002 trong ExtendedRepositorySyncQueueTests (Assert.NotNull
dùng sai trên value type Guid), sau đó merge fix vào CVerify-uat mà không mất các
thay đổi song song khác đang được merge cùng lúc (PR #110, PR #111).
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-19 |
| Công cụ AI | Claude Code |
| Mục đích | Xác định nguyên nhân cảnh báo xUnit2002 và sửa assertion cho đúng |
| Phần việc liên quan | Testing (Unit Test) |
| Mức độ sử dụng | AI hỗ trợ chẩn đoán lỗi analyzer và đề xuất assertion thay thế |

#### Prompt tóm tắt

```text
Sửa cảnh báo xUnit2002 trên test Guid trong ExtendedRepositorySyncQueueTests.cs.
```

#### Kết quả

```text
- Thay Assert.NotNull(queueId) bằng Assert.NotEqual(Guid.Empty, queueId)
- Cảnh báo xUnit2002 được giải quyết, assertion có ý nghĩa kiểm thử thật
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-19 |
| Công cụ AI | Claude Code |
| Mục đích | Giải quyết merge conflict giữa fix xUnit2002 và các thay đổi song song trên CVerify-uat |
| Phần việc liên quan | Git / Merge |
| Mức độ sử dụng | AI hỗ trợ rà soát các file conflict và xác nhận giữ đúng thay đổi cần thiết |

#### Prompt tóm tắt

```text
Merge nhánh chứa fix Assert.NotEqual vào CVerify-uat, giữ nguyên fix xUnit2002 và
không làm mất các thay đổi khác đang merge cùng lúc.
```

#### Kết quả

```text
- Conflict được giải quyết, giữ lại fix Assert.NotEqual cho xUnit2002
- Các thay đổi song song khác (test mở rộng, tài liệu, audit packages của thành
  viên khác) được hợp nhất không mất mát
- Commit merge 97139ee hoàn tất, CVerify-uat ở trạng thái build/test pass
```

---

## 5. Đánh giá nhanh

| Tiêu chí | Đánh giá |
|---|---|
| Đúng nguyên nhân gốc (không chỉ suppress warning) | Đạt |
| Không ảnh hưởng logic nghiệp vụ | Đạt |
| Merge không mất thay đổi song song | Đạt |
| Sinh viên vẫn kiểm soát nội dung merge | Đạt — xác nhận thủ công phần giữ lại trong conflict |
