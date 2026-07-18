# Changelog

## 2. Thông tin project

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
| Repository URL | `https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05` |
| Ngày bắt đầu | 2026-06-18 |
| Ngày hoàn thành | 2026-06-28 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 18/06/2026 - 19/06/2026 | Nghiên cứu JSONB vs normalized, quyết định schema | Completed |
| Phase 02 | 20/06/2026 - 23/06/2026 | Thiết kế và viết SQL migration | Completed |
| Phase 03 | 24/06/2026 - 27/06/2026 | Triển khai EF config, repository, kiểm thử | Completed |
| Phase 04 | 28/06/2026 | Hoàn thiện tài liệu | Completed |

---

# [Phase 01] Nghiên cứu và quyết định schema

## Đã hoàn thành

- [x] Hỏi ChatGPT về trade-off JSONB vs normalized tables
- [x] Đọc tài liệu PostgreSQL JSONB và GIN index
- [x] Quyết định hybrid approach: normalized columns + JSONB cho certifications
- [x] Phác thảo ERD cho bảng BusinessProfiles

## AI có hỗ trợ không?

- [x] Có

```text
ChatGPT giải thích trade-off JSONB vs normalized tables và gợi ý hybrid approach.
Chỉ dùng để hiểu nguyên lý, không lấy SQL từ AI.
```

---

# [Phase 02] Thiết kế và viết SQL migration

## Đã hoàn thành

- [x] Thiết kế schema bảng business_profiles với columns chuẩn hóa cốt lõi
- [x] Thêm cột certifications JSONB cho dữ liệu chứng chỉ linh hoạt
- [x] Viết CHECK constraint validate cấu trúc JSONB tối thiểu
- [x] Tạo GIN index trên certifications JSONB
- [x] Tạo expression index trên certifications -> 'type' cho query theo loại chứng chỉ
- [x] Viết EF Core migration file

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Viết migration tạo bảng business_profiles với JSONB column | Trương Văn Hiếu | `CVerify.Core/Migrations/` | Migration file |
| 2 | Tạo GIN index và expression index cho JSONB | Trương Văn Hiếu | `CVerify.Core/Migrations/` | Index definition |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Triển khai EF config và kiểm thử

## Đã hoàn thành

- [x] Cấu hình EF Core HasColumnType("jsonb") cho certifications column
- [x] Triển khai BusinessProfileRepository với query JSONB bằng EF.Functions.JsonContains
- [x] Benchmark EXPLAIN ANALYZE trước/sau khi có GIN index
- [x] Kiểm thử CRUD certification data với JSONB

## Danh sách lỗi đã xử lý

| STT | Lỗi phát hiện | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Query theo type chứng chỉ vẫn chậm dù có GIN index | GIN index trên toàn cột không optimize được partial path query | Tạo expression index trên (certifications -> 'type') | Fixed |
| 2 | EF Core serialize JSONB không đúng kiểu | EF dùng text thay vì jsonb | Thêm HasColumnType("jsonb") và configure JsonDocument converter | Fixed |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 4. Tổng kết

### 4.1. Các chức năng đã hoàn thành

| STT | Chức năng | Trạng thái | Ghi chú |
|---:|---|---|---|
| 1 | Schema bảng business_profiles hybrid JSONB | Completed | Normalized core + JSONB certifications |
| 2 | GIN index và expression index | Completed | Query theo type nhanh |
| 3 | CHECK constraint validate JSONB | Completed | Đảm bảo data integrity |
| 4 | EF Core JSONB mapping và repository | Completed | |

### 4.2. Tổng hợp AI hỗ trợ

| Hạng mục | AI có hỗ trợ không? | Mức độ | Ghi chú |
|---|---|---|---|
| Thiết kế database | Có | Ít | Nguyên lý JSONB vs normalized |
| Viết SQL | Không | - | Tự viết 100% |
| Coding EF | Không | - | Tự code 100% |

---

## 5. Cam kết cập nhật Changelog

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 28/06/2026 |
