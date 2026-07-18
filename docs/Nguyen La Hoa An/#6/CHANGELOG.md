# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

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
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 2026-07-16 ~ 2026-07-16 | Research, rent, and configure a suitable VPS to prepare the production deployment environment for CVerify. | In Progress |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 |  |  | Not Started |

---

# [Phase 01] 

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-07-16 ~ 2026-07-16
- **Mô tả giai đoạn:** Research, rent, and configure a suitable VPS to prepare the production deployment environment for CVerify.
- **Trạng thái hiện tại:** In Progress

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Researched and compared VPS providers based on CPU, RAM, storage, server location, operating system support, and rental cost. |   |   |   |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI was used to research VPS configurations, review Linux and Docker commands, troubleshoot deployment errors, and suggest Nginx reverse proxy configurations.
```

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| File/Commit |  |  |

## Ghi chú

```text
The VPS was successfully rented and prepared for the CVerify deployment. Basic server tools and required services were installed. The deployment process is still in progress because several environment variables, authentication configurations, database migrations, and service connection issues require further testing.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
Researched and compared suitable VPS providers and server configurations.
```

---

## 4.2. Các chức năng chưa hoàn thành

```text
Complete production testing for authentication and Google OAuth
```

---

## 4.3. Cải thiện chính

```text
Migrated CVerify from a local development environment to a centralized VPS environment
```

---

## 4.4. Tổng kết project

```text
During the project, I was responsible for researching, renting, and configuring the VPS used to deploy the CVerify system. I prepared the Ubuntu Server environment, installed the required deployment tools, configured Docker services, set up the domain and Nginx reverse proxy, and supported the deployment of the frontend, backend API, AI service, PostgreSQL database, and Redis cache.

I also assisted in troubleshooting infrastructure and deployment issues involving ports, environment variables, Docker containers, Nginx routing, Redis connections, and database migrations. The CVerify system was successfully moved from a local-only environment to a publicly accessible VPS environment. However, several production-level configurations and integration tests still require further completion.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
Implement an automated CI/CD pipeline using GitHub Actions.
Configure automatic PostgreSQL database backups and recovery procedures.
Add monitoring for CPU, RAM, disk usage, container health, and service availability.
Configure Docker health checks and automatic restart policies.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 16/7/2026 |
