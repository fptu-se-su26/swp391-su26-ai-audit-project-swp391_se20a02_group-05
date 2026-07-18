# Prompt Log

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày cập nhật | 2026-07-15 |

---

## 4. Bảng tổng hợp

| STT | Ngày | AI | Mục đích | Prompt tóm tắt | Kết quả | Dùng vào bài? |
|---:|---|---|---|---|---|---|
| 1 | 10/07/2026 | Claude | Job dependencies, secrets management, prevent log leak | "needs vs if:success()... Environment vs Repository secrets... ngăn secrets leak trong log..." | Hiểu needs, Environment Protection, add-mask | Có (Điểm cụ thể không chắc) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Trong GitHub Actions, khi tôi có workflow gồm 3 jobs: build → test → deploy, job "deploy"
nên dùng "needs" hay "if" để chỉ chạy khi test pass? Ngoài ra: secrets trong GitHub Actions
nên đặt ở Repository level hay Environment level? Và làm thế nào để ngăn secrets bị log ra
trong output nếu tôi dùng command line tool có thể print environment variables?
```

#### 5.2. Bối cảnh

Tôi đã draft workflow nhưng không chắc về `needs` vs `if: success()`, và không biết Environment
Protection Rules để require manual approval khi deploy production.

#### 5.3. Kết quả AI trả về

- `needs: [test]` đủ, không cần thêm `if: success()` (implicit).
- Environment secrets + Protection Rules cho production để require approval.
- `echo "::add-mask::$VALUE"` để mask secret trong log.

#### 5.4. Điều quan trọng nhất

`add-mask` — tính năng ít tài liệu đề cập nhưng quan trọng để prevent secret leak.

#### 5.5. Phần không áp dụng từ AI

AI không đề cập concurrency group và least-privilege permissions — tôi tự tìm hiểu và thêm.

---

## 6. Bài học

```text
Hỏi AI về điểm cụ thể mình không chắc ("needs vs if:success()") hiệu quả hơn hỏi chung
"viết CI/CD workflow cho tôi". Có background knowledge trước giúp đặt đúng câu hỏi.
```

---

## 9. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 15/07/2026 |
