# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie AI |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-13T07:28:07.404Z |
| Ngày hoàn thành | 2026-05-13T07:28:07.408Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
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
Research and Requirement analysis, Generate documents.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-15 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Thiết kế một hệ thống AI Agent chuyên cho travel planning với khả năng:  Thu thập requirement từ user qua survey hoặc chat prompt Sinh travel itinerary hoàn chỉnh Generate structured plan dưới dạng Google Sheet Cho phép user chỉnh sửa thủ công AI tự reconcile thay đổi và regenerate plan mới Mở rộng thành ecosystem AI gồm: recommendation engine planner engine chatbot assistant validation system classification/tagging summarization  Mục tiêu thực tế:  Biến AI thành orchestration layer cho travel SaaS Tách AI layer khỏi business logic để scale dễ hơn Chuẩn hóa structured output để FE/BE xử lý deterministic thay vì parsing text tự do |
| Phần việc liên quan | Requirement |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
* Context: Tôi muốn tạo một AI Agent để tạo và lên plan cho khách hàng muốn lên plan đi du lịch(bao gồm chỗ đi chơi, ăn uống, nhà hàng, khách sạn). Agent sẽ đảm nhiệm vai trò lấy nhu cầu của khách hàng thông qua survey hoặc từ prompt input của khách hàng sau đó sẽ tạo ra một implement plan rồi gen ra 1 bảng google sheet. User/Guest có quyền chỉnh sửa các phần trong plan của AI tạo ra và sau đó Agent sẽ sửa lại theo đúng nhu cầu khách hàng.
* Yêu cầu: phân tích, tìm hiểu và đề xuất giải pháp custom AI API cho hệ thống, bao gồm:
   * Lựa chọn API AI phù hợp với bài toán
   * Nghiên cứu cách hoạt động của AI API (request, prompt, response, token, streaming, function calling,…)
   * Thiết kế và custom cấu trúc response từ AI để phù hợp với nghiệp vụ hệ thống
   * Xây dựng cơ chế để front-end phân tích và xử lý response nhằm thực hiện các task cụ thể
   * Đề xuất workflow giao tiếp giữa Front-end ↔ Backend ↔ AI API
   * Thiết kế các AI task mở rộng và tích hợp vào hệ thống như:
      * AI recommendation
      * AI planner/generator
      * AI summarization
      * AI chatbot assistant
      * AI classification/tagging
      * AI validation/checking
   * Đề xuất hướng triển khai thực tế, tối ưu chi phí API, performance và scalability
   * Phân chia hướng implement AI features vào kiến trúc hệ thống hiện tại để đảm bảo khả năng mở rộng về sau.
* Yêu cầu output: đảm bảo giải thích bằng từ ngữ và tạo file json để visualize json data thành dạng graph workflow cho dễ nhìn
* Tài liệu đã nghiên cứu được đính kèm
```

#### 4.2. Kết quả AI gợi ý

```text

EXECUTIVE SUMMARY (ENGLISH)
AI_TRAVEL_PLANNER_SUMMARY.md
A complete English guide covering:

AI API Selection - Why Anthropic Claude (Sonnet 4.6 + Haiku 4.5) is optimal
Core Concepts - Tokens, streaming, function-calling, prompt caching explained
Structured Output Design - How to force JSON schema compliance via "emit_itinerary" terminal tool
Frontend Response Dispatcher - Type-based routing for itinerary_draft, clarification, validation messages
Complete Workflow - Step-by-step: User → FE → Gateway → Orchestrator → Claude → Tools → APIs → Database
AI Microservices - 7 parallel AI tasks (classifier, planner, validator, chatbot, summarizer, recommender)
Cost Optimization - Prompt caching (90% savings), model routing (40% savings), 7 tactics
Scalability - Stateless orchestrator, async queuing, horizontal scaling beyond 5k trips/month
System Layers - 6-tier architecture (Presentation → Gateway → AI → Tools → APIs → State)
Implementation Stack - FastAPI + Next.js + Postgres + Redis + Langfuse
Security & Compliance - Secrets management, OAuth, PCI-DSS, Vietnam PDPL
Success Metrics - KPIs for UX, cost, reliability, adoption
JSON Structure - What's in the workflow file
Quick Start Checklist - 20-step implementation roadmap
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- API selection, tool-use pattern, ReAct flow, 4-phase lifecycle, state management
- Pricing model, security best practices, FastAPI + Next.js stack recommendation
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Terminal tool pattern — emit_itinerary tool to force JSON schema
Response Dispatcher — Type-based FE routing (text/itinerary/clarification/final)
6-layer architecture — Explicit Gateway + AI Orchestration layers
7 parallel AI tasks — Decomposed agents with model routing (Haiku for 70% of work)
Prompt caching math — Concrete ROI calculation
Context compression — Prevents unbounded token growth
Diff-based edit flow — How user edits trigger re-planning
Per-turn Langfuse tracing — trace_id on every message for debugging
PostgreSQL schema — Concrete table design
Redis TTL strategy — Specific cache durations per data type
Fallback orchestration — Recovery paths for API outages
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 12:49:54 | image.png |

#### 4.6. Nhận xét cá nhân/nhóm

```text
- Failure recovery paths (what if Claude is down?)
- User preference learning loop (how to infer user taste)
- Multi-traveler conflict resolution (Alice vs. Bob preferences)
- Offline mode + service worker sync
- A/B testing prompt variants safely
- Explainability (why did AI pick this restaurant?)
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
Research how to integrate APIs into the system, compare criteria, and provide support during the initial planning phase of the project.
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
| Nguyễn Hoàng Ngọc Ánh | 15/5/2026 |
