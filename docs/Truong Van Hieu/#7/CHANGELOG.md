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
| Ngày bắt đầu | 2026-06-25 |
| Ngày hoàn thành | 2026-07-02 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 25/06/2026 - 26/06/2026 | Nghiên cứu multi-stage build, thiết kế Dockerfile | Completed |
| Phase 02 | 27/06/2026 - 29/06/2026 | Viết Dockerfile và .dockerignore cho tất cả services | Completed |
| Phase 03 | 30/06/2026 - 01/07/2026 | Tích hợp vào GitHub Actions CI/CD | Completed |
| Phase 04 | 02/07/2026 | Kiểm thử và tài liệu | Completed |

---

# [Phase 01] Nghiên cứu và thiết kế

## Đã hoàn thành

- [x] Hỏi Claude về nguyên lý multi-stage build và best practice
- [x] Đọc tài liệu Docker Best Practices chính thức
- [x] Phác thảo cấu trúc multi-stage cho CVerify.Core (.NET 8) và CVerify.Client (Next.js)
- [x] So sánh aspnet:8.0-runtime vs distroless image

## AI có hỗ trợ không?

- [x] Có

```text
Claude giải thích nguyên lý layer cache và best practice multi-stage build. Không lấy code.
```

---

# [Phase 02] Viết Dockerfile

## Đã hoàn thành

- [x] Viết Dockerfile multi-stage cho CVerify.Core: stage build (sdk:8.0) → stage runtime (aspnet:8.0)
- [x] Viết Dockerfile multi-stage cho CVerify.Client: stage deps → stage build (node:20-alpine) → stage serve (nginx:alpine)
- [x] Viết Dockerfile cho CVerify.AI: stage base (python:3.12-slim) với pip install optimization
- [x] Tạo .dockerignore cho từng service loại bỏ bin/obj/node_modules
- [x] Cấu hình non-root user trong tất cả Dockerfile
- [x] Thêm HEALTHCHECK instruction

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module | Minh chứng |
|---:|---|---|---|---|
| 1 | Dockerfile CVerify.Core multi-stage | Trương Văn Hiếu | `CVerify/CVerify.Core/Dockerfile` | Image size giảm từ 600MB → 180MB |
| 2 | Dockerfile CVerify.Client multi-stage | Trương Văn Hiếu | `CVerify/client/Dockerfile` | Image size giảm từ 1.2GB → 45MB |
| 3 | .dockerignore cho tất cả services | Trương Văn Hiếu | `*/. dockerignore` | Build time giảm đáng kể |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] GitHub Actions Integration

## Đã hoàn thành

- [x] Viết workflow build-and-push.yml cho CI/CD pipeline
- [x] Cấu hình Docker Buildx cho multi-platform build
- [x] Cấu hình cache với actions/cache để tái sử dụng Docker layer cache

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 4. Tổng kết

### 4.1. Kết quả đạt được

| Metric | Trước | Sau | Cải thiện |
|---|---|---|---|
| CVerify.Core image size | ~600 MB | ~180 MB | Giảm 70% |
| CVerify.Client image size | ~1.2 GB | ~45 MB | Giảm 96% |
| CI build time | ~8 phút | ~3 phút (có cache) | Giảm 62% |

---

## 5. Cam kết cập nhật Changelog

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 02/07/2026 |
