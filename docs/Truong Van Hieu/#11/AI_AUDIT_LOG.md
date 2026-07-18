# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - Hệ thống xác thực thông tin và quản lý hồ sơ năng lực dành cho Doanh nghiệp |
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV / Danh sách MSSV | DE190105 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-07-06 |
| Ngày hoàn thành | 2026-07-12 |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là hiểu các testing pattern trong xUnit: AAA (Arrange-Act-Assert), Test Doubles
(Mock/Stub/Fake/Spy), và cách tổ chức test class hiệu quả. Tôi có nền tảng về unit test
cơ bản nhưng chưa quen với Moq và cách mock dependency injection trong ASP.NET Core.
Không dùng AI để viết test — chỉ hỏi nguyên lý để tự viết đúng cách.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 06/07/2026 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Hiểu sự khác biệt giữa Mock, Stub, Fake và khi nào dùng từng loại |
| Phần việc liên quan | Viết unit test cho service layer |
| Mức độ sử dụng | Hỏi kiến thức |

#### 4.1. Prompt đã sử dụng

```text
Giải thích sự khác biệt giữa Mock, Stub, Fake và Spy trong unit testing. Khi nào nên dùng
từng loại? Đối với việc test service layer trong ASP.NET Core với Moq, tôi nên mock toàn
bộ repository hay chỉ mock các method cần thiết? Và pattern AAA (Arrange-Act-Assert) nên
tổ chức trong test class như thế nào để test dễ đọc nhất?
```

#### 4.2. Kết quả AI gợi ý

AI giải thích rõ ràng:
- Stub: trả về dữ liệu cố định, không verify behavior.
- Mock: verify behavior (method được gọi bao nhiêu lần, với argument gì).
- Fake: implementation thật nhưng đơn giản hóa (ví dụ in-memory DB).
- Spy: wrap object thật, ghi lại behavior.
Trong Moq: dùng Setup để stub, Verify để mock assertion. Chỉ mock method cần thiết trong test
đó. AAA: mỗi test có 3 phần rõ ràng, không nên quá 3-4 Mock.Setup trong 1 test.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu phân biệt Stub vs Mock và nguyên tắc "chỉ mock method cần thiết" — giúp tôi viết
test focused hơn thay vì mock toàn bộ interface. Áp dụng pattern AAA rõ ràng trong mọi test.

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi tự thiết kế TestFixture base class để share common setup giữa các test class cùng module.
AI không đề cập đến này. Ngoài ra tôi tự viết custom IEqualityComparer cho assert phức tạp
trên domain object.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: ChatGPT giải thích rõ ràng sự khác biệt Stub vs Mock — điểm nhiều người
  nhầm lẫn. Giúp tôi viết test assert đúng chỗ.
- Bài học: Quá nhiều Mock.Setup trong 1 test là dấu hiệu service đó có quá nhiều dependency
  — nên refactor thay vì tiếp tục mock.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | Ghi chú |
|---|:---:|:---:|:---:|---|
| Hiểu testing patterns |  | [x] |  | Phân biệt Mock/Stub/Fake |
| Viết test cases | [x] |  |  | Tự viết 100% |
| Thiết kế test structure | [x] |  |  | Tự thiết kế TestFixture |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Hạn chế | Cách xử lý |
|---:|---|---|
| 1 | AI không đề cập đến vấn đề test isolation khi dùng static member hoặc singleton. | Tự phát hiện khi test chạy pass riêng nhưng fail khi chạy toàn bộ suite. Thêm TestFixture và cleanup trong Dispose. |
| 2 | AI không đề cập đến parametrized test với [Theory] và [InlineData] trong xUnit. | Tự tìm hiểu và áp dụng để tránh viết nhiều test method tương tự nhau. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc tài liệu xUnit và Moq chính thức.
2. Đọc "The Art of Unit Testing" của Roy Osherove.
3. Thực hành viết test và review với thành viên nhóm.
```

---

## 8. Đóng góp cá nhân

```text
- Tự thiết kế test structure với TestFixture base class.
- Tự viết tất cả test case cho MonitoringAuditService, HmacSignatureService.
- Tự áp dụng [Theory]/[InlineData] cho parametrized tests.
- Tự phát hiện và fix test isolation issue.
- Tự viết custom assert helper cho domain object.
```

---

## 9. Reflection cuối bài

### 9.1. AI hỗ trợ ở điểm nào?

```text
Làm rõ sự khác biệt Mock vs Stub — điểm nhiều lập trình viên nhầm lẫn và dẫn đến viết
assert sai chỗ (assert data thay vì assert behavior hoặc ngược lại).
```

### 9.2. Học được gì về môn học?

```text
Unit test tốt không chỉ là "test pass" mà còn là test dễ đọc, isolated và fail với thông
báo rõ ràng. Quá nhiều Setup trong 1 test là warning sign về design.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 12/07/2026 |
