# Changelog

## 2. Thông tin project

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày bắt đầu | 2026-07-03 |
| Ngày hoàn thành | 2026-07-08 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung | Trạng thái |
|---|---|---|---|
| Phase 01 | 03/07/2026 | Xác nhận kiến trúc Clean Architecture với AI, thiết kế cấu trúc | Completed |
| Phase 02 | 04/07/2026 - 06/07/2026 | Refactor cấu trúc module, enforce Dependency Rule | Completed |
| Phase 03 | 07/07/2026 - 08/07/2026 | Architecture test và tài liệu | Completed |

---

# [Phase 01] Xác nhận kiến trúc

## Đã hoàn thành

- [x] Hỏi Claude xác nhận Clean Architecture layers và Dependency Rule
- [x] Đọc "Clean Architecture" Robert C. Martin và eShopOnContainers reference
- [x] Thiết kế cấu trúc thư mục module-based: `Modules/{Name}/Controllers/Services/Models/DTOs`
- [x] Quyết định không dùng Generic Repository

## AI có hỗ trợ không?

- [x] Có

```text
Claude xác nhận nguyên lý Dependency Rule và tương thích của module-based organization với
Clean Architecture. Chỉ dùng để xác nhận kiến thức, không thiết kế thay.
```

---

# [Phase 02] Refactor và enforce Dependency Rule

## Đã hoàn thành

- [x] Refactor cấu trúc thư mục theo module-based Clean Architecture
- [x] Di chuyển interfaces vào đúng layer (Domain/Application)
- [x] Di chuyển EF Core implementation vào Infrastructure layer
- [x] Tách DTOs khỏi Domain entities
- [x] Viết architecture unit test với NetArchTest để enforce Dependency Rule

## Thay đổi chi tiết

| STT | Nội dung | Người thực hiện | File | Minh chứng |
|---:|---|---|---|---|
| 1 | Refactor cấu trúc module Admin, Auth, Candidate | Trương Văn Hiếu | `CVerify.Core/Modules/` | Commit refactor |
| 2 | Viết ArchitectureTests với NetArchTest | Trương Văn Hiếu | `tests/ArchitectureTests/` | Test pass |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Architecture Test

## Đã hoàn thành

- [x] ArchitectureTest: Domain không depend vào Infrastructure → pass
- [x] ArchitectureTest: Controllers chỉ depend vào Application → pass
- [x] Code review nội bộ để xác nhận Dependency Rule

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 5. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
