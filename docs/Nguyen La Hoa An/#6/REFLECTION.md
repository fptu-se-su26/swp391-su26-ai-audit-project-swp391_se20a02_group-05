# AI Learning Reflection

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
| Ngày hoàn thành reflection | 2026-07-16 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
During the CVerify deployment process, I used ChatGPT to assist with researching suitable VPS configurations, preparing the Ubuntu Server environment, installing Docker and Docker Compose, configuring the Nginx reverse proxy, connecting the domain, and troubleshooting deployment issues. AI was also used to analyze logs, explain possible causes of errors, and recommend verification steps. However, I did not directly copy and apply all AI-generated suggestions. I adjusted them according to the actual CVerify architecture, domains, ports, containers, and environment variables, and then manually tested the configurations on the VPS.
```

---

## 4. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
ChatGPT
```

### Lý do sử dụng công cụ đó

```text
Tiết kiệm thời gian, Kiểm tra lỗi
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [ ] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [ ] Thiết kế kiến trúc hệ thống
- [ ] Viết code mẫu
- [ ] Debug lỗi
- [ ] Viết test case
- [ ] Review code
- [ ] Tối ưu code
- [ ] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI supported me in identifying suitable solutions for deploying CVerify on a VPS, learning how to configure Docker, Docker Compose, Nginx, PostgreSQL, and Redis, and troubleshooting deployment issues. It helped analyze errors related to ports, environment variables, container connections, Nginx routing, Redis, and database migrations. I reviewed and adjusted the suggestions to match the actual CVerify architecture before applying them to the VPS.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
AI helped me understand new deployment technologies such as VPS, Docker, Docker Compose, Nginx, PostgreSQL, and Redis more quickly. It provided explanations, suggested troubleshooting steps, and helped me identify possible causes of deployment errors. This allowed me to learn from real technical problems while saving research time.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
Some AI responses were too general or did not fully match the actual CVerify environment. Certain commands and configurations had to be adjusted because the real domain names, ports, container names, environment variables, and service dependencies were different. Therefore, all suggestions still required manual verification before use.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [ ] Phụ thuộc ít
- [x] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
AI was mainly used to reduce research time, provide initial guidance, and suggest troubleshooting directions. However, I still reviewed the project configuration, modified the proposed solutions, executed the commands, analyzed the actual logs, and verified the deployment results manually.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [ ] Kiểm tra output
- [ ] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [ ] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
I verified the AI-generated suggestions by comparing them with the actual CVerify requirements, project configuration files, and official documentation. I tested the deployment directly on the VPS using SSH, Docker commands, service logs, curl requests, Swagger, and browser access. I also checked whether the frontend, backend API, database, Redis, Nginx routing, and domain configuration worked as expected. Any suggestion that did not match the real system environment was modified or rejected.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | The AI suggested configuring Nginx as a reverse proxy to route requests from cverify.io.vn to the frontend container and requests from api.cverify.io.vn to the backend API container. |
| Em/nhóm đã kiểm tra bằng cách nào? | I reviewed the Nginx configuration, restarted the Nginx container, checked the container logs, and tested both domains using a browser, curl, and Swagger. |
| Kết quả kiểm tra | Partially correct. The general reverse proxy approach was correct, but the upstream service names and ports had to be adjusted to match the actual Docker Compose configuration. |
| Em/nhóm đã xử lý tiếp như thế nào? | I updated the Nginx upstream configuration, restarted the related containers, checked the logs again, and retested the frontend and API endpoints until the routing worked correctly. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

My main contribution was researching and renting a suitable VPS for the CVerify project, preparing the Ubuntu Server environment, and supporting the deployment process. I installed and configured Docker, Docker Compose, Nginx, PostgreSQL, and Redis, connected the project domain and API subdomain, and supported the deployment of the frontend, backend API, AI service, database, and cache services. I also analyzed logs and resolved issues related to ports, environment variables, Nginx routing, Redis connections, database migrations, and Docker containers. AI was used only as a support tool, while I manually reviewed, adjusted, executed, and verified the final configurations.

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|

---

## 11. Bài học về môn học

- Tầm quan trọng của làm việc nhóm

I learned that system deployment requires careful coordination between application services, infrastructure, domains, databases, and environment variables. A small configuration error in Docker, Nginx, Redis, or PostgreSQL can affect the entire system, so each service should be checked step by step.

---

## 12. Bài học về sử dụng AI có trách nhiệm

AI should be used as a supporting tool for research, troubleshooting, and documentation. All technical recommendations must be reviewed against the real project configuration, official documentation, and actual test results before being applied.

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

### Giải thích thêm nếu có

```text
 
```

---

## 14. Kế hoạch cải thiện lần sau

In future projects, I will prepare the deployment architecture and environment variables earlier, document all ports and service dependencies, and test each container independently before integrating the full system. I will also improve the Git workflow, add Docker health checks, create automatic backup procedures, strengthen VPS security, and build a clearer deployment checklist. AI prompts will include complete logs, configuration files, expected results, and previous troubleshooting attempts to obtain more accurate support.

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 |   |
| Prompt có mục tiêu rõ ràng | 5 |   |
| Kiểm chứng kết quả AI | 5 |   |
| Tự chỉnh sửa/cải tiến | 5 |   |
| Hiểu nội dung đã nộp | 5 |   |
| Reflection có chiều sâu | 5 |   |
| Sử dụng AI có trách nhiệm | 5 |   |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có, nhóm đã đọc, kiểm tra và hiểu nội dung trước khi sử dụng.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có, nhưng sẽ mất nhiều thời gian hơn để nghiên cứu và triển khai.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần thiết kế workflow, chỉnh sửa logic và xử lý lỗi thực tế.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Kỹ năng thiết kế hệ thống, viết prompt và kiểm thử phần mềm.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 16/7/2026 |
