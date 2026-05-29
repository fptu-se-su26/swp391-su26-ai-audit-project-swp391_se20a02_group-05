# Prompt Log

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
| Ngày cập nhật gần nhất | 2026-05-29 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
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
| 1 | 2026-05-25 | Claude | Chuẩn bị bộ tài liệu nền tảng cho môn Project Course. Tạo bộ tài liệu đặc tả yêu cầu phần mềm. | 



Chuẩn bị file mẫu SRS cho ... | CVerify_SRS_v1.0.docx — Softwa... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-25 |
| Công cụ AI | Claude |
| Mục đích | Chuẩn bị bộ tài liệu nền tảng cho môn Project Course. Tạo bộ tài liệu đặc tả yêu cầu phần mềm. |
| Phần việc liên quan | Report |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

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

#### 5.2. Bối cảnh khi viết prompt

```text
Summary about CVerify project, purpose, requirement
```

#### 5.3. Kết quả AI trả về

```text
CVerify_SRS_v1.0.docx — Software Requirements Specification (~35 trang)

Cover page, TOC tự động, header/footer trang
52 use cases phân chia 6 module (mỗi thành viên ~10 UC)
Bảng actors, UC summary đầy đủ, 6 UC chi tiết (UC table format chuẩn SWP391)
Functional Requirements (FR-01 → FR-06), Non-Functional Requirements (18 NFR với metric cụ thể)
Business Rules (12 BR), Constraints (kỹ thuật + project + regulatory), Database Overview (15 bảng)
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Software Requirements Specification
Project repository README
Research Survey
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
 
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt còn thiếu thông tin
- [ ] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [x] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

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

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

```text
CVerify_SRS_v1.0.docx — Software Requirements Specification (~35 trang)

Cover page, TOC tự động, header/footer trang
52 use cases phân chia 6 module (mỗi thành viên ~10 UC)
Bảng actors, UC summary đầy đủ, 6 UC chi tiết (UC table format chuẩn SWP391)
Functional Requirements (FR-01 → FR-06), Non-Functional Requirements (18 NFR với metric cụ thể)
Business Rules (12 BR), Constraints (kỹ thuật + project + regulatory), Database Overview (15 bảng)
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Software Requirements Specification
Project repository README
Research Survey
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
 
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
Từ kinh nghiệm thực tế trong dự án CVerify, nhóm nhận ra prompt hiệu quả cần có:

1. Mục tiêu rõ ràng: Tôi cần [X] để [làm gì].
2. Bối cảnh dự án: Tên project, core idea, đang ở giai đoạn nào.
3. Tech stack cụ thể: Ngôn ngữ, framework, database, version.
4. Constraints: Team size, timeline, budget, những gì KHÔNG muốn.
5. Format output mong muốn: Code? Text? Diagram? Bảng?
6. Scope giới hạn: Chỉ làm phần X, không làm phần Y.
7. Ví dụ input/output nếu cần (đặc biệt với coding tasks).
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Bài học quan trọng nhất: AI không đọc được tâm trí. Mọi thông tin mà nhóm biết nhưng không đưa vào prompt, AI sẽ không biết và sẽ đưa ra câu trả lời chung chung hoặc không phù hợp. "Hãy thiết kế kiến trúc cho dự án của tôi" và "Hãy thiết kế kiến trúc monolith modular cho team 5 sinh viên, Next.js + ASP.NET Core, 15 tuần, không microservices" cho ra kết quả hoàn toàn khác nhau.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
1. Viết prompt theo template: Context → Goal → Constraints → Output format.
2. Luôn nêu tech stack và version cụ thể khi hỏi về code.
3. Chia nhỏ bài toán lớn thành các câu hỏi nhỏ hơn thay vì hỏi một lần.
4. Thêm "Giải thích reasoning của bạn" để hiểu tại sao AI đề xuất như vậy.
5. Khi AI trả lời chưa tốt, không hỏi lại y nguyên — cần bổ sung thêm context còn thiếu.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Report | 1 |  |

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
| Nguyễn Hoàng Ngọc Ánh | 29/5/2026 |
