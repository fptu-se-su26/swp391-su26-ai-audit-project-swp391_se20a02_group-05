# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày cập nhật gần nhất | 2026-07-16 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 2026-07-16 | ChatGPT | To obtain guidance for selecting, configuring, and using a VPS to deploy the CVerify system, including Docker, Nginx, domain configuration, database services, and deployment troubleshooting. | Please guide me through rentin... | ChatGPT provided step-by-step ... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-16 |
| Công cụ AI | ChatGPT |
| Mục đích | To obtain guidance for selecting, configuring, and using a VPS to deploy the CVerify system, including Docker, Nginx, domain configuration, database services, and deployment troubleshooting. |
| Phần việc liên quan | Other |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Please guide me through renting and configuring a suitable VPS for the CVerify project. The system includes a Next.js frontend, a .NET backend API, a FastAPI AI service, PostgreSQL, Redis, Docker Compose, and Nginx. Help me configure the server, deploy the services, connect the domain, and troubleshoot deployment errors.
```

#### 5.2. Bối cảnh khi viết prompt

```text
The CVerify system uses a multi-service architecture consisting of a Next.js frontend, a .NET backend API, a FastAPI AI service, PostgreSQL, Redis, and Nginx. The target server was an Ubuntu 24.04 VPS with 2 vCPU, 4 GB RAM, and 80 GB NVMe storage. The deployment used Docker Compose, with the domains cverify.io.vn and api.cverify.io.vn. During deployment, several issues occurred involving Nginx upstream configuration, environment variables, Redis connections, PostgreSQL migrations, port conflicts, authentication, and Docker containers.
```

#### 5.3. Kết quả AI trả về

```text
ChatGPT provided step-by-step guidance for comparing VPS providers, selecting an appropriate server configuration, connecting through SSH, installing Docker and Docker Compose, configuring Nginx as a reverse proxy, setting up the domain and API subdomain, and deploying the CVerify services. It also suggested commands and troubleshooting methods for Docker, Redis, PostgreSQL migrations, environment variables, port conflicts, Nginx errors, and service connectivity.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
I used the VPS configuration recommendations, Ubuntu server preparation steps, Docker and Docker Compose installation instructions, Nginx reverse proxy configuration, domain setup guidance, and troubleshooting commands for checking container logs, ports, Redis, PostgreSQL, and API connectivity.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
I adapted the suggested configurations to match the actual CVerify architecture, domain names, container names, ports, and environment variables. I verified each command before execution, adjusted the Nginx upstream and Docker Compose settings, removed unnecessary suggestions, and tested the services using Docker logs, curl, Swagger, and browser access. The final configuration was reviewed and applied manually rather than copied directly.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
Please guide me through renting and configuring a suitable VPS for the CVerify project. The system includes a Next.js frontend, a .NET backend API, a FastAPI AI service, PostgreSQL, Redis, Docker Compose, and Nginx. Help me configure the server, deploy the services, connect the domain, and troubleshoot deployment errors.
```

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

```text
ChatGPT provided step-by-step guidance for comparing VPS providers, selecting an appropriate server configuration, connecting through SSH, installing Docker and Docker Compose, configuring Nginx as a reverse proxy, setting up the domain and API subdomain, and deploying the CVerify services. It also suggested commands and troubleshooting methods for Docker, Redis, PostgreSQL migrations, environment variables, port conflicts, Nginx errors, and service connectivity.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
I used the VPS configuration recommendations, Ubuntu server preparation steps, Docker and Docker Compose installation instructions, Nginx reverse proxy configuration, domain setup guidance, and troubleshooting commands for checking container logs, ports, Redis, PostgreSQL, and API connectivity.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
I adapted the suggested configurations to match the actual CVerify architecture, domain names, container names, ports, and environment variables. I verified each command before execution, adjusted the Nginx upstream and Docker Compose settings, removed unnecessary suggestions, and tested the services using Docker logs, curl, Swagger, and browser access. The final configuration was reviewed and applied manually rather than copied directly.
```

---

## 7. Prompt chưa hiệu quả

```text
Chưa có prompt chưa hiệu quả được ghi nhận.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
The prompt should include the VPS specifications, operating system, project architecture, technologies, domain names, container names, ports, deployment method, expected result, and complete error messages. Configuration files and relevant logs should also be provided so that the AI can identify the problem accurately.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
I learned that a clear and specific prompt produces more useful answers than a general request. Deployment problems should be divided into smaller issues, such as Docker, Nginx, database, Redis, domain, or authentication errors. It is also important to provide the commands already executed, current system status, and actual error output instead of only describing that the deployment failed.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Next time, I will describe the deployment environment and desired outcome first, then provide the relevant configuration, logs, and error messages. I will ask about one problem at a time, specify what I have already tried, and request step-by-step instructions with verification commands. I will also validate and adapt the AI suggestions before applying them to the VPS.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Other | 1 |  |

---

## 10. Checklist chất lượng prompt

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | x | |
| Prompt có đủ bối cảnh | x | |
| Tự kiểm tra và chỉnh sửa | x | |

---

## 11. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 16/7/2026 |
