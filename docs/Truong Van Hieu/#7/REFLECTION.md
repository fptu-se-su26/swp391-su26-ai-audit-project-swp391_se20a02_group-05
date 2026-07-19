# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày hoàn thành reflection | 02/07/2026 |

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Tôi dùng Claude 1 lần để hiểu nguyên lý Docker multi-stage build và best practice tối ưu
image. Toàn bộ Dockerfile, .dockerignore và GitHub Actions workflow đều tự viết.
```

---

## 5. AI đã hỗ trợ ở điểm nào?

- [x] Tìm ý tưởng giải pháp

```text
AI giải thích cụ thể tại sao thứ tự instruction trong Dockerfile ảnh hưởng đến layer cache,
giúp tôi hiểu nguyên lý thay vì chỉ copy Dockerfile template từ internet.
```

---

## 6. AI có giúp học tốt hơn không?

### Điểm giúp tốt

```text
Hiểu nguyên lý layer cache giúp tôi tự optimize Dockerfile thay vì chỉ copy theo template.
Kết quả: image size giảm 70-96% tùy service.
```

### Điểm chưa giúp tốt

```text
AI không nhắc đến security best practice (non-root user, health check). Phải tự đọc
CIS Docker Benchmark.
```

### Phụ thuộc AI?

- [x] Không phụ thuộc

---

## 7. Kiểm tra kết quả AI

- [x] Tra cứu tài liệu chính thống (Docker official docs)
- [x] Kiểm tra bằng dữ liệu mẫu (docker image ls, thời gian build)

---

## 8. Ví dụ AI gợi ý thiếu

| Nội dung | Mô tả |
|---|---|
| Thiếu gì? | Non-root user và HEALTHCHECK instruction |
| Phát hiện bằng cách nào? | Đọc CIS Docker Benchmark và yêu cầu của GCP Cloud Run |
| Đã thêm gì? | RUN addgroup/adduser, USER instruction, HEALTHCHECK trong Dockerfile |
| Bài học | Security hardening Docker không có trong best practice phổ biến AI biết |

---

## 9. Đóng góp thật sự

```text
1. Tự quyết định chiến lược multi-stage cho từng service.
2. Tự viết Dockerfile cho CVerify.Core, CVerify.Client, CVerify.AI.
3. Tự bổ sung non-root user và health check.
4. Tự cấu hình GitHub Actions với Docker Buildx.
5. Đo và xác nhận kết quả tối ưu thực tế.
```

---

## 10. So sánh trước và sau

| Area | Before | After | Improvement |
|---|---|---|---|
| Hiểu layer cache | Không biết thứ tự COPY quan trọng | Biết đặt COPY dependency trước source | Build time giảm đáng kể khi có cache |
| Image security | Chạy root user | Non-root user + health check | Đáp ứng security requirement |

---

## 15. Câu hỏi tự vấn

### Phần nào thể hiện năng lực thật sự?

```text
Tự phát hiện thiếu non-root user và health check qua đọc CIS Benchmark, và tự cấu hình
GitHub Actions với layer cache optimization — đây là những việc AI không làm thay tôi.
```

---

## 16. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 02/07/2026 |
