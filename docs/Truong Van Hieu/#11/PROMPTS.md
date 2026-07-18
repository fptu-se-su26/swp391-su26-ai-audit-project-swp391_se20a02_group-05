# Prompt Log

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày cập nhật | 2026-07-12 |

---

## 4. Bảng tổng hợp

| STT | Ngày | AI | Mục đích | Prompt tóm tắt | Kết quả | Dùng vào bài? |
|---:|---|---|---|---|---|---|
| 1 | 06/07/2026 | ChatGPT | Phân biệt Mock/Stub/Fake và AAA pattern trong xUnit | "Giải thích Mock, Stub, Fake, Spy... khi nào dùng từng loại... AAA pattern..." | Hiểu phân biệt và best practice | Có (Nguyên lý viết test) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Giải thích sự khác biệt giữa Mock, Stub, Fake và Spy trong unit testing. Khi nào nên dùng
từng loại? Đối với việc test service layer trong ASP.NET Core với Moq, tôi nên mock toàn
bộ repository hay chỉ mock các method cần thiết? Và pattern AAA (Arrange-Act-Assert) nên
tổ chức trong test class như thế nào để test dễ đọc nhất?
```

#### 5.2. Kết quả AI trả về và đã áp dụng

- Chỉ Setup method cần thiết trong từng test (không mock toàn bộ interface).
- Phân biệt dùng Stub (Setup return value) khi test logic, Mock (Verify call) khi test behavior.
- AAA rõ ràng với comment // Arrange, // Act, // Assert.

#### 5.3. Điều quan trọng nhất AI cung cấp

```text
"Quá nhiều Mock.Setup trong 1 test là warning sign — service có quá nhiều dependency."
→ Giúp tôi nhận ra MonitoringController cần refactor thay vì tiếp tục mock nhiều hơn.
```

#### 5.4. Phần không áp dụng từ AI

AI không đề cập TestFixture và [Theory]/[InlineData] — tôi tự tìm hiểu và áp dụng.

---

## 6. Bài học

```text
Hỏi AI về "nguyên tắc" (khi nào dùng Mock vs Stub) hiệu quả hơn hỏi "cách làm" (viết code
test cho tôi). Nguyên tắc áp dụng được cho mọi test case, code mẫu chỉ áp dụng cho 1 case.
```

---

## 9. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 12/07/2026 |
