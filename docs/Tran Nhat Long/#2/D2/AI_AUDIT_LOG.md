# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify 2 |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-29T06:05:37.062Z |
| Ngày hoàn thành | 2026-05-29T06:05:37.062Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
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
 
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-29 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Chuẩn bị bộ tài liệu nền tảng cho môn Project Course. Tạo bộ tài liệu đặc tả yêu cầu phần mềm. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text




Chuẩn bị file mẫu SRS cho dự án.
Trong đó có:
giới thiệu hệ thống
functional requirements
non-functional requirements
use cases
actors
business rules
constraint
architecture overview
database overview
v.v. Thường đây là file Word chuẩn để cả nhóm viết yêu cầu hệ thống.
README.md Yêu cầu README của project phải có: a) “Project có sử dụng hàm lượng nghiên cứu (RBL)” RBL = Research Based Learning. Tức: Dự án không được chỉ là CRUD cơ bản. Phải có phần nghiên cứu kỹ thuật hoặc nghiên cứu công nghệ. Ví dụ:
nghiên cứu thuật toán AI
recommendation system
tối ưu routing
streaming architecture
vector database
RAG
event-driven architecture
caching
distributed system
agentic workflow
multimodal AI
security architecture
model orchestration
forecasting
NLP
v.v. Giảng viên muốn biết: “Điểm nghiên cứu” của project nằm ở đâu. Ví dụ với TripGenie:
AI itinerary planning
weather-aware planning
streaming AI response
prompt caching
structured JSON contract
multi-agent orchestration Đó chính là “hàm lượng nghiên cứu”. b) Hướng dẫn README phải có:
cách setup project
cách chạy backend
cách chạy frontend
env variables
database setup
API keys
docker/run commands
project structure Tức là ai clone repo về cũng chạy được.
Nhóm phải có tài liệu nghiên cứu / survey / paper reference. Có thể là:
paper liên quan

research summary
tài liệu tham khảo học thuật
giải thích hướng nghiên cứu của nhóm Ví dụ:
AI travel planning papers
itinerary optimization
weather prediction impact
recommendation systems
LLM orchestration papers
Trong README.md phải có link Jira project. Ví dụ:
## Project Management
Jira: https://yourteam.atlassian.net
Trong Jira phải:
chia task
assign member
có sprint/task management Ví dụ:
Backend auth
AI planner
Weather service
Frontend itinerary UI
Streaming API
Database schema
Testing
DeploymentProject phải đủ lớn. Nếu nhóm 5 người:
mỗi người chịu trách nhiệm khoảng 10 use cases
tổng hệ thống khoảng 50 use cases Ví dụ use case
đăng ký
đăng nhập
tạo trip
edit trip
AI generate itinerary
save itinerary
share itinerary
budget calculation
weather integration
export PDF
chat assistant
booking integration
review places
notifications
collaborative planning
```

#### 4.2. Kết quả AI gợi ý

```text
CVerify_SRS_v1.0.docx — Software Requirements Specification (~35 trang)

Cover page, TOC tự động, header/footer trang
52 use cases phân chia 6 module (mỗi thành viên ~10 UC)
Bảng actors, UC summary đầy đủ, 6 UC chi tiết (UC table format chuẩn SWP391)
Functional Requirements (FR-01 → FR-06), Non-Functional Requirements (18 NFR với metric cụ thể)
Business Rules (12 BR), Constraints (kỹ thuật + project + regulatory), Database Overview (15 bảng)
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Software Requirements Specification
Project repository README
Research Survey
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
 
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
| Generate documents |   |   |   | x |   |
| Researching and validate information |   | x |   |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | The resulting output does not match user needs 100%. | Check manually | The input prompt is more detailed and includes more context and rules. |
| 2 | Required multiple edits to match requirements | Check manually | Manually checking and improvising more context to AI |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Manual verification is combined with the use of another AI to validate the generated content.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Research and generate requirements and research paper for the project's structure
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
|  |  |  | Có / Không |  |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 29/5/2026 |
