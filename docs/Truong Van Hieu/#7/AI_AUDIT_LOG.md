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
| Ngày bắt đầu | 2026-06-25 |
| Ngày hoàn thành | 2026-07-02 |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là hiểu nguyên lý Docker multi-stage build và các best practice để tối ưu image
size cho dịch vụ .NET 8 và Next.js trong môi trường production của CVerify. Tôi không dùng
AI để viết Dockerfile — chỉ hỏi nguyên lý và kiểm tra lại hiểu biết của mình.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 25/06/2026 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Hiểu nguyên lý Docker multi-stage build và best practice tối ưu image size |
| Phần việc liên quan | DevOps / CI-CD |
| Mức độ sử dụng | Hỏi kiến thức |

#### 4.1. Prompt đã sử dụng

```text
Giải thích Docker multi-stage build là gì và tại sao nó giúp giảm kích thước image production.
Đối với dịch vụ .NET 8 ASP.NET Core và Next.js, các best practice nào quan trọng nhất để
giảm image size và tăng tốc độ build trong CI/CD pipeline? Ví dụ như: chọn base image nào,
copy file nào vào từng stage, thứ tự COPY để tận dụng layer cache.
```

#### 4.2. Kết quả AI gợi ý

AI giải thích rõ: multi-stage build cho phép dùng image build đầy đủ (sdk) để compile rồi
chỉ copy artifact vào image runtime (aspnet/runtime) nhỏ hơn. Best practice: dùng alpine
hoặc slim base image, copy package.json trước khi copy source để tận dụng npm install cache,
đặt COPY . . sau cùng, dùng .dockerignore để loại bỏ node_modules và bin/obj.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu nguyên lý về thứ tự COPY để tận dụng layer cache và lý do chọn runtime image nhỏ.
Áp dụng để tối ưu Dockerfile cho CVerify.Core (dùng mcr.microsoft.com/dotnet/aspnet:8.0 thay
vì sdk image) và CVerify.Client (dùng node:20-alpine cho build stage, nginx:alpine cho serve).

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi bổ sung thêm: build argument để inject environment variables lúc build, cấu hình non-root
user trong container (AI không đề cập) và cấu hình health check. Ngoài ra tôi tự research
thêm về distroless image cho .NET để so sánh với aspnet runtime.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: Claude giải thích rõ lý do tại sao thứ tự COPY ảnh hưởng đến cache,
  giúp tôi hiểu nguyên lý thay vì chỉ copy template Dockerfile.
- Bài học: .dockerignore quan trọng không kém Dockerfile — quên file này làm mất toàn bộ
  lợi ích của layer cache vì context gửi lên Docker daemon quá lớn.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Thiết kế kiến trúc DevOps |  | [x] |  |  | Tham khảo nguyên lý multi-stage |
| Viết Dockerfile | [x] |  |  |  | Tự viết 100% |
| Cấu hình CI/CD | [x] |  |  |  | Tự cấu hình GitHub Actions |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI không đề cập đến việc chạy container với non-root user (security best practice). | Đọc CIS Docker Benchmark sau khi hoàn thiện Dockerfile. | Thêm RUN addgroup/adduser và chuyển sang user đó trước CMD. |
| 2 | AI không đề cập đến health check trong Dockerfile cho orchestration. | Khi deploy lên GCP Cloud Run thấy không có health status. | Thêm HEALTHCHECK instruction và endpoint /health trong ASP.NET Core. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc tài liệu Docker Best Practices chính thức.
2. Đo kích thước image thực tế trước/sau khi áp dụng multi-stage (docker image ls).
3. Đo thời gian build có/không có layer cache trong CI/CD pipeline.
```

---

## 8. Đóng góp cá nhân

```text
- Tự viết Dockerfile cho CVerify.Core và CVerify.Client.
- Tự cấu hình .dockerignore cho từng service.
- Tự phát hiện và thêm non-root user và health check.
- Tự đo và so sánh image size.
- Tự cấu hình GitHub Actions để build và push image.
```

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ ở điểm nào?

```text
AI giải thích lý do tại sao thứ tự instruction trong Dockerfile ảnh hưởng đến layer cache —
đây là kiến thức quan trọng mà nhiều tutorial không giải thích rõ.
```

### 9.2. Học được gì về môn học?

```text
DevOps là phần tích hợp quan trọng của SDLC. Dockerfile tối ưu giúp giảm thời gian deploy
và chi phí lưu trữ image trên Container Registry một cách đáng kể.
```

### 9.3. Học được gì về sử dụng AI có trách nhiệm?

```text
AI biết best practice phổ biến nhưng không biết security requirement của tổ chức
(non-root user, health check). Luôn phải đối chiếu với security standards.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 02/07/2026 |
