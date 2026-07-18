# AI Learning Reflection

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày reflection | 12/07/2026 |

---

## 3. Tóm tắt

```text
Dùng ChatGPT 1 lần để hiểu phân biệt Mock/Stub/Fake và AAA pattern. Phần quan trọng nhất
là nhận ra "quá nhiều Setup = warning sign về design." Tự viết 47 test cases và tự thiết
kế TestFixture. Tự phát hiện và fix test isolation issue.
```

---

## 5. AI hỗ trợ ở điểm nào?

- [x] Tìm ý tưởng giải pháp

```text
Làm rõ sự khác biệt Mock vs Stub — điểm quan trọng để viết assertion đúng chỗ.
Gợi ý nguyên tắc "chỉ mock method cần thiết" giúp test focused hơn.
```

---

## 6. Điểm giúp và không giúp

### Giúp tốt

```text
Insight "quá nhiều Setup = warning sign về design" — giúp tôi nhận ra cần refactor
MonitoringController thay vì tiếp tục mock nhiều hơn.
```

### Không giúp tốt

```text
AI không đề cập test isolation issue với static/singleton. Tự phát hiện khi test fail khi
chạy toàn suite. Cũng không đề cập [Theory]/[InlineData] để giảm boilerplate.
```

### Phụ thuộc AI?

- [x] Không phụ thuộc

---

## 8. Ví dụ AI không đề cập

| Nội dung | Mô tả |
|---|---|
| Không đề cập gì? | Test isolation issue và [Theory]/[InlineData] parametrized test |
| Hậu quả? | Test pass riêng nhưng fail khi chạy toàn suite (isolation issue) |
| Tự xử lý như thế nào? | TestFixture với IDisposable cleanup, dùng [Theory] giảm boilerplate |
| Bài học | AI biết pattern phổ biến nhưng không biết những vấn đề phát sinh khi chạy real test suite |

---

## 9. Đóng góp thật sự

```text
1. Tự thiết kế TestFixture base class.
2. Tự viết 47 test cases cho 4 service/controller.
3. Tự áp dụng [Theory]/[InlineData] cho parametrized cases.
4. Tự phát hiện và fix test isolation issue.
5. Generate và analyze coverage report, thêm edge case tests.
```

---

## 10. So sánh trước và sau

| Area | Before | After | Improvement |
|---|---|---|---|
| Hiểu Mock vs Stub | Nhầm lẫn, hay dùng Verify cho data assertion | Biết dùng đúng: Stub cho data, Mock cho behavior | Test assertion đúng chỗ |
| Test structure | Không có base class, nhiều boilerplate | TestFixture base class, [Theory] | Test ít boilerplate hơn, dễ thêm case |

---

## 16. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 12/07/2026 |
