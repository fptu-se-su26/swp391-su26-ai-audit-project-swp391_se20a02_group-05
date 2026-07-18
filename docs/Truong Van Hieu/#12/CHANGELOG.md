# Changelog

## 2. Thông tin project

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày bắt đầu | 2026-07-10 |
| Ngày hoàn thành | 2026-07-15 |

---

## 3. Tổng quan giai đoạn

| Giai đoạn | Thời gian | Nội dung | Trạng thái |
|---|---|---|---|
| Phase 01 | 10/07/2026 | Nghiên cứu, hỏi AI, thiết kế workflow | Completed |
| Phase 02 | 11/07/2026 - 13/07/2026 | Viết và test workflow YAML | Completed |
| Phase 03 | 14/07/2026 - 15/07/2026 | Security hardening và tài liệu | Completed |

---

# [Phase 01] Nghiên cứu và thiết kế

## Đã hoàn thành

- [x] Hỏi Claude về job dependencies, Environment secrets, add-mask
- [x] Thiết kế workflow: build → test → deploy với 3 jobs riêng biệt
- [x] Quyết định dùng Environment Protection Rules cho production deployment
- [x] Thiết kế concurrency group để cancel stale runs

## AI có hỗ trợ không?

- [x] Có

```text
Claude giải thích job dependencies (needs) vs Environment secrets vs Repository secrets
và tính năng add-mask. Không lấy YAML từ AI.
```

---

# [Phase 02] Viết workflow

## Đã hoàn thành

- [x] Workflow `build-test.yml`: trigger on PR, chạy build + unit test
- [x] Workflow `deploy-staging.yml`: trigger on merge to CVerify-uat, deploy lên staging GCP
- [x] Workflow `deploy-production.yml`: trigger manual với Environment approval
- [x] Matrix strategy: test trên ubuntu-latest và windows-latest
- [x] Docker layer cache với actions/cache
- [x] Concurrency group cancel in-progress

## Thay đổi chi tiết

| STT | File | Nội dung | Minh chứng |
|---:|---|---|---|
| 1 | `.github/workflows/build-test.yml` | PR validation workflow | CI pass trên tất cả PRs |
| 2 | `.github/workflows/deploy-staging.yml` | Staging deployment | Auto deploy khi merge CVerify-uat |
| 3 | `.github/workflows/deploy-production.yml` | Production với manual approval | Require reviewer approval |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

# [Phase 03] Security hardening

## Đã hoàn thành

- [x] Thêm `permissions: contents: read` ở tất cả jobs
- [x] Cấu hình `add-mask` cho sensitive values trong scripts
- [x] Review workflow để không leak secrets trong log
- [x] Test: xác nhận log không chứa secret values

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

---

## 5. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 15/07/2026 |
