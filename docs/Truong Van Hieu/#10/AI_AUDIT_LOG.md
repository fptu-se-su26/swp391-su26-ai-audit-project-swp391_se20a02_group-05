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
| Ngày bắt đầu | 2026-07-03 |
| Ngày hoàn thành | 2026-07-08 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là hiểu nguyên lý Clean Architecture / Onion Architecture và cách tổ chức các
layer (Domain, Application, Infrastructure, Presentation) trong dự án .NET để áp dụng
cho cấu trúc CVerify.Core. Tôi không dùng AI để thiết kế thay mà chỉ hỏi để kiểm tra
xem hiểu biết của mình về Clean Architecture có đúng không.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/07/2026 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Kiểm tra hiểu biết về Clean Architecture layers và Dependency Rule |
| Phần việc liên quan | Thiết kế kiến trúc CVerify.Core |
| Mức độ sử dụng | Hỏi để xác nhận kiến thức |

#### 4.1. Prompt đã sử dụng

```text
Giải thích Clean Architecture (còn gọi là Onion Architecture) gồm các layer nào, và Dependency
Rule là gì — tại sao inner layer không được phép biết outer layer? Trong bối cảnh .NET 8 với
EF Core, Repository Pattern nên đặt ở layer nào? Interface nên định nghĩa ở Domain hay
Application layer? Tôi đang tổ chức CVerify.Core theo module (Admin, Auth, Candidate...) —
cách này có tương thích với Clean Architecture không?
```

#### 4.2. Kết quả AI gợi ý

AI giải thích: Clean Architecture gồm Domain (entities, interfaces) → Application (use cases,
services) → Infrastructure (EF Core, repositories, external services) → Presentation (controllers,
API). Dependency Rule: dependency chỉ hướng vào trong, outer biết inner nhưng không ngược lại.
Repository Interface định nghĩa ở Domain/Application, implementation ở Infrastructure. Tổ chức
theo module (feature slice) hoàn toàn tương thích — mỗi module có thể có đủ các layer bên trong.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu xác nhận rằng cách tổ chức theo module của CVerify.Core là đúng hướng (Vertical Slice
Architecture kết hợp Clean Architecture). Xác nhận vị trí đặt Interface vs Implementation.

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi tự quyết định cấu trúc cụ thể: `Modules/{ModuleName}/Controllers/`, `Services/`, `Models/`,
`DTOs/`. AI chỉ xác nhận nguyên lý, không thiết kế cấu trúc thư mục thực tế. Ngoài ra tôi tự
quyết định không dùng Generic Repository mà dùng Specific Repository để tránh over-abstraction.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: Claude xác nhận nhanh chóng hiểu biết của tôi về Clean Architecture, giúp
  tôi tự tin đi tiếp mà không mất thời gian nghi ngờ.
- Bài học: Clean Architecture là nguyên lý, không phải quy định cứng nhắc. Điều chỉnh
  cho phù hợp với bài toán (module-based) là hoàn toàn hợp lý.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | Ghi chú |
|---|:---:|:---:|:---:|---|
| Thiết kế kiến trúc |  | [x] |  | Xác nhận nguyên lý Clean Architecture |
| Tổ chức cấu trúc thư mục | [x] |  |  | Tự quyết định |
| Code | [x] |  |  | Tự code 100% |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Hạn chế | Cách xử lý |
|---:|---|---|
| 1 | AI có xu hướng đề xuất Generic Repository pattern — đây là anti-pattern trong nhiều trường hợp vì che giấu EF Core capabilities. | Tự quyết định dùng Specific Repository với EF Core trực tiếp ở Application layer khi cần. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc "Clean Architecture" của Robert C. Martin.
2. Tham khảo eShopOnContainers Microsoft reference architecture.
3. Code review nội bộ để đảm bảo không vi phạm Dependency Rule.
```

---

## 8. Đóng góp cá nhân

```text
- Tự quyết định cấu trúc module-based kết hợp Clean Architecture.
- Tự thiết kế cấu trúc thư mục cụ thể cho từng module.
- Tự quyết định không dùng Generic Repository.
- Tự enforce Dependency Rule bằng .NET architecture unit test.
```

---

## 9. Reflection cuối bài

### 9.1. AI hỗ trợ ở điểm nào?

```text
Xác nhận nhanh chóng rằng module-based organization tương thích với Clean Architecture —
giúp tôi tự tin tiếp tục mà không mất thời gian nghi ngờ.
```

### 9.2. Học được gì về sử dụng AI có trách nhiệm?

```text
AI có thể đề xuất pattern phổ biến (Generic Repository) mà không phải lúc nào cũng phù hợp.
Cần tự đánh giá trade-off thay vì áp dụng máy móc.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
