# AI Learning Reflection

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày reflection | 15/07/2026 |

---

## 3. Tóm tắt

```text
Dùng Claude 1 lần để hỏi các điểm cụ thể không chắc trong GitHub Actions: job dependencies,
Environment vs Repository secrets, và add-mask để prevent secret leak. Tự viết toàn bộ YAML
và tự thêm concurrency group, least-privilege permissions.
```

---

## 5. AI hỗ trợ ở điểm nào?

- [x] Tìm ý tưởng giải pháp (về CI/CD security best practices)

```text
add-mask và Environment Protection Rules là hai điểm quan trọng tôi không biết.
AI giúp tiết kiệm thời gian tìm trong docs.
```

---

## 6. Điểm giúp và không giúp

### Giúp tốt

```text
Trả lời nhanh điểm cụ thể (needs vs if:success(), add-mask) giúp tiết kiệm thời gian
đọc docs dài.
```

### Không giúp tốt

```text
AI không đề cập concurrency group (cancel stale runs) và least-privilege permissions
— hai điểm quan trọng cho production-grade CI/CD.
```

### Phụ thuộc AI?

- [x] Không phụ thuộc

---

## 8. Ví dụ AI không đề cập

| Nội dung | Mô tả |
|---|---|
| Không đề cập? | Concurrency group và least-privilege permissions |
| Tại sao quan trọng? | Concurrency cancel stale run tiết kiệm CI minutes. Least-privilege giảm attack surface. |
| Tự xử lý? | Tự thêm `concurrency:` và `permissions: contents: read` sau khi đọc GitHub Security Hardening guide |
| Bài học | CI/CD security là lớp kiến thức riêng. AI biết basic nhưng không biết security hardening guide |

---

## 9. Đóng góp thật sự

```text
1. Thiết kế workflow structure (3 jobs, 3 files).
2. Tự viết toàn bộ YAML.
3. Tự thêm matrix, concurrency, permissions.
4. Test và debug workflow failures.
5. Verify secrets không leak trong log.
```

---

## 16. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 15/07/2026 |
