# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV | DE190105 |
| Môn học | SWP391 - SE20A02 - SU26 |
| Ngày cập nhật | 2026-07-02 |

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Claude

---

## 4. Bảng tổng hợp

| STT | Ngày | AI | Mục đích | Prompt tóm tắt | Kết quả | Có dùng vào bài? |
|---:|---|---|---|---|---|---|
| 1 | 25/06/2026 | Claude | Nguyên lý multi-stage build và best practice | "Giải thích Docker multi-stage build... best practice cho .NET 8 và Next.js..." | Hiểu layer cache, chọn base image | Có (Nguyên lý) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Giải thích Docker multi-stage build là gì và tại sao nó giúp giảm kích thước image
production. Đối với dịch vụ .NET 8 ASP.NET Core và Next.js, các best practice nào quan
trọng nhất để giảm image size và tăng tốc độ build trong CI/CD pipeline? Ví dụ như:
chọn base image nào, copy file nào vào từng stage, thứ tự COPY để tận dụng layer cache.
```

#### 5.2. Bối cảnh

CVerify có 3 services cần container hóa: .NET 8 backend, Next.js frontend và Python FastAPI.
Image size lớn làm chậm CI/CD và tốn chi phí Registry. Cần hiểu nguyên lý trước khi viết.

#### 5.3. Kết quả AI trả về

Giải thích: multi-stage dùng image sdk đầy đủ để build, chỉ copy artifact sang image runtime
nhỏ. Best practice: thứ tự COPY dependency trước source để tận dụng cache, dùng alpine/slim
base image, .dockerignore loại bỏ file không cần thiết.

#### 5.4. Đã áp dụng như thế nào

Áp dụng nguyên lý thứ tự COPY và chọn base image đúng. Tự viết toàn bộ Dockerfile.

#### 5.5. Phần đã chỉnh sửa/cải tiến

Bổ sung non-root user và HEALTHCHECK instruction mà AI không đề cập.

#### 5.6. Đánh giá

- [x] Prompt rõ ràng, có ngữ cảnh công nghệ cụ thể
- [x] Kết quả tốt

---

## 6. Bài học về cách viết prompt

```text
Prompt có ví dụ cụ thể về những gì muốn biết (thứ tự COPY, chọn base image) giúp AI tập
trung vào đúng điểm quan trọng thay vì giải thích tổng quan.
```

---

## 9. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 02/07/2026 |
