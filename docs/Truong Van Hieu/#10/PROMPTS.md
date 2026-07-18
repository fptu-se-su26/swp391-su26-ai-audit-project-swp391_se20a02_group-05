# Prompt Log

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày cập nhật | 2026-07-08 |

---

## 4. Bảng tổng hợp

| STT | Ngày | AI | Mục đích | Prompt | Kết quả | Dùng vào bài? |
|---:|---|---|---|---|---|---|
| 1 | 03/07/2026 | Claude | Xác nhận kiến thức Clean Architecture layers và Dependency Rule | "Giải thích Clean Architecture... Dependency Rule... Interface ở layer nào... module-based có tương thích không?" | Xác nhận nguyên lý đúng, module-based tương thích | Có (Xác nhận thiết kế) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Giải thích Clean Architecture (còn gọi là Onion Architecture) gồm các layer nào, và Dependency
Rule là gì — tại sao inner layer không được phép biết outer layer? Trong bối cảnh .NET 8 với
EF Core, Repository Pattern nên đặt ở layer nào? Interface nên định nghĩa ở Domain hay
Application layer? Tôi đang tổ chức CVerify.Core theo module (Admin, Auth, Candidate...) —
cách này có tương thích với Clean Architecture không?
```

#### 5.2. Tại sao hỏi prompt này

Tôi đã có hiểu biết về Clean Architecture nhưng không chắc về vị trí của Interface và tính
tương thích với module-based organization. Hỏi AI để xác nhận nhanh thay vì mất thời gian
đọc lại sách.

#### 5.3. Kết quả và áp dụng

AI xác nhận: module-based tương thích với Clean Architecture (Vertical Slice). Interface ở
Application layer, implementation ở Infrastructure. Tôi áp dụng cấu trúc này và viết
ArchitectureTests để enforce.

#### 5.4. Điểm cần cẩn thận từ AI

AI có xu hướng gợi ý Generic Repository Pattern — tôi chủ động không áp dụng vì nó che giấu
EF Core capabilities và tạo ra abstraction không cần thiết.

---

## 6. Bài học

```text
Dùng AI để xác nhận kiến thức ("kiểu này có đúng không?") hiệu quả hơn dùng để học kiến
thức mới ("giải thích cho tôi biết"). Tôi hiểu đúng hơn khi tự học trước rồi dùng AI xác nhận.
```

---

## 9. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
