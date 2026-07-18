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
| Ngày bắt đầu | 2026-07-10 |
| Ngày hoàn thành | 2026-07-15 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là hiểu cách thiết kế GitHub Actions workflow với các giai đoạn build → test →
deploy và cách xử lý secrets an toàn trong CI/CD pipeline cho CVerify. Tôi đã tự viết
workflow draft nhưng không chắc về cấu trúc job dependencies và secrets management.
Dùng AI để hỏi những điểm cụ thể không chắc, không dùng AI để viết workflow.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 10/07/2026 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Hiểu job dependencies (needs), environment protection và secrets rotation |
| Phần việc liên quan | CI/CD GitHub Actions |
| Mức độ sử dụng | Hỏi điểm cụ thể không chắc |

#### 4.1. Prompt đã sử dụng

```text
Trong GitHub Actions, khi tôi có workflow gồm 3 jobs: build → test → deploy, job "deploy"
nên dùng "needs" hay "if" để chỉ chạy khi test pass? Ngoài ra: secrets trong GitHub Actions
nên đặt ở Repository level hay Environment level? Và làm thế nào để ngăn secrets bị log ra
trong output nếu tôi dùng command line tool có thể print environment variables?
```

#### 4.2. Kết quả AI gợi ý

AI giải thích: dùng `needs: [test]` để job deploy chỉ chạy khi job test hoàn thành thành công
(implicit skip on failure). Environment secrets phù hợp hơn Repository secrets khi cần approval
gate (Environment Protection Rules) và phân tách giữa staging và production. Để ngăn secret
leak: dùng `add-mask` để mask giá trị, tránh echo/print trong shell command.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu cách dùng `needs` thay vì `if: success()` (dư thừa), và Environment secrets với
Protection Rules cho production deployment. Áp dụng `echo "::add-mask::$SECRET"` để mask
sensitive values trong log.

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi tự thiết kế workflow với matrix strategy để build parallel trên nhiều OS, và tự cấu hình
concurrency group để cancel in-progress run khi có push mới. AI không đề cập hai điều này.
Ngoài ra tự cấu hình `permissions: contents: read` để giảm attack surface.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: Claude giải thích rõ ràng sự khác biệt Repository vs Environment secrets,
  giúp tôi hiểu khi nào cần từng loại.
- Bài học: `add-mask` là tính năng quan trọng của GitHub Actions mà ít tài liệu đề cập
  nhưng rất cần thiết để tránh secret leak trong log.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | Ghi chú |
|---|:---:|:---:|:---:|---|
| Thiết kế CI/CD pipeline |  | [x] |  | Hỏi điểm cụ thể không chắc |
| Viết YAML workflow | [x] |  |  | Tự viết 100% |
| Cấu hình secrets | [x] |  |  | Tự cấu hình và test |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Hạn chế | Cách xử lý |
|---:|---|---|
| 1 | AI không đề cập concurrency group để cancel in-progress run khi push mới. | Tự tìm hiểu và thêm `concurrency: group: ${{ github.ref }}, cancel-in-progress: true`. |
| 2 | AI không đề cập principle of least privilege cho GitHub Actions permissions. | Tự thêm `permissions: contents: read` ở job level để giảm attack surface. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc tài liệu GitHub Actions chính thức về Encrypted Secrets và Environment Protection.
2. Test workflow trên branch feature và xác nhận job dependencies hoạt động đúng.
3. Kiểm tra log để xác nhận secret không bị expose.
```

---

## 8. Đóng góp cá nhân

```text
- Tự viết toàn bộ YAML workflow cho 3 pipelines (Core, Client, AI service).
- Tự cấu hình matrix strategy.
- Tự thêm concurrency group.
- Tự cấu hình least-privilege permissions.
- Tự test và debug workflow failures.
```

---

## 9. Reflection cuối bài

### 9.1. AI hỗ trợ ở điểm nào?

```text
Làm rõ Environment vs Repository secrets và tính năng `add-mask` để prevent secret leak —
hai điểm quan trọng cho CI/CD security mà tôi không biết trước đó.
```

### 9.2. Học được gì về môn học?

```text
CI/CD pipeline tốt không chỉ là "build and deploy" mà còn phải xem xét security (secrets
management, permissions), efficiency (concurrency, caching) và reliability (retry, timeout).
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 15/07/2026 |
