# AI Audit Log

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
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
VPS research, deployment planning, server configuration, Docker and Docker Compose setup, Nginx reverse proxy configuration, domain setup, troubleshooting deployment errors, log analysis, PostgreSQL migration support, Redis connection troubleshooting, and documentation writing.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-16 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | To support VPS selection, server configuration, CVerify deployment, and troubleshooting of infrastructure-related errors. |
| Phần việc liên quan | Frontend |
| Mức độ sử dụng | Hỗ trợ một phần |

#### 4.1. Prompt đã sử dụng

```text
Requested guidance for selecting and configuring an Ubuntu VPS to deploy CVerify using Docker Compose, Nginx, PostgreSQL, Redis, Next.js, .NET API, and FastAPI.
```

#### 4.2. Kết quả AI gợi ý

```text
VPS configuration recommendations, SSH and server preparation steps, Docker installation commands, Nginx reverse proxy guidance, domain configuration, troubleshooting commands, and deployment verification methods.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
The VPS setup guidance, Docker and Docker Compose installation steps, Nginx reverse proxy configuration, domain and subdomain setup instructions, troubleshooting commands, and deployment verification methods were used.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Adjusted the suggested commands and configurations to match the actual CVerify domains, ports, services, container names, environment variables, and VPS specifications. All changes were manually reviewed and tested before use.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| VPS Deployment |   |   | x |   | ChatGPT was used to support VPS research, Docker and Nginx configuration, deployment troubleshooting, log analysis, and audit documentation. All suggestions were reviewed, modified, and tested manually before being applied. |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | Some AI suggestions were too general and did not fully match the actual CVerify deployment environment. | The issue was detected when several suggested commands and configurations did not match the actual domain names, container names, ports, environment variables, or service architecture. | The suggestions were compared with the real Docker Compose files, Nginx configuration, logs, and VPS environment. Incorrect or unnecessary parts were removed, and the remaining instructions were manually adjusted and tested. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
I verified the AI suggestions by comparing them with the official documentation and the actual CVerify configuration files. I manually checked the VPS through SSH and tested the deployment using commands such as docker ps, docker logs, docker compose config, curl, and port checks. I also verified the frontend, API, Swagger, domain routing, Redis connection, and PostgreSQL migrations through browser testing and service logs. AI-generated instructions were only applied after being reviewed and adjusted to match the real deployment environment.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
My personal contribution was researching and renting a suitable VPS, preparing the Ubuntu Server environment, and supporting the deployment of CVerify. I connected to the VPS through SSH, installed and configured Docker, Docker Compose, Nginx, PostgreSQL, and Redis, and adjusted the deployment configuration to match the project architecture. I also configured the domain and API subdomain, reviewed environment variables, executed deployment commands, checked logs, and troubleshot issues related to ports, Nginx upstreams, Redis, database migrations, authentication, and Docker containers. ChatGPT was used as a support tool for guidance and troubleshooting, while I manually reviewed, modified, executed, and verified the final configuration.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
|  |  |  | Không |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 16/7/2026 |
