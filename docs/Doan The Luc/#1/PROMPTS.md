# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project Test |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | AI Workflow Logger |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | Quang Lê |
| Ngày bắt đầu | 2026-05-12T16:15:00.532Z |
| Ngày cập nhật gần nhất | 2026-05-14 |

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
| 1 | 2026-05-12 | ChatGPT | Triển khai ý tưởng rộng hơn | Dưới đây là bản phác thảo toàn... | AI đọc file, đánh giá ý tưởng,... | Có |   |
| 2 | 2026-05-12 | ChatGPT | Tạo prompt chi tiết từ ý tưởng ban đầu | Rồi bây giờ từ những ý tưởng t... | Create a full-stack modern web... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-12 |
| Công cụ AI | ChatGPT |
| Mục đích | Triển khai ý tưởng rộng hơn |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Dưới đây là bản phác thảo toàn bộ ý tưởng của website, được thiết kế để trở thành một "Nhật ký cộng tác AI" (AI-Human Collaboration Ledger) chuyên nghiệp, giúp bạn quản lý quy trình làm việc một cách minh bạch và khoa học. 1. Tên dự án & Tinh thần thiết kế Tên dự án: PowD AI-Audit System Phong cách: Minimalist, Tech-focused (Sử dụng Dark mode, font chữ monospace cho code và bảng biểu). Mục tiêu: Chuyển đổi các tệp Markdown khô khan thành một quy trình nhập liệu trực quan, dễ theo dõi và có thể xuất ngược lại định dạng chuẩn để nộp bài hoặc lưu trữ. 2. Page 1: AI Workspace (Hệ thống nhập liệu Step-by-Step) Trang này được thiết kế như một "Survey Flow" (luồng khảo sát) gồm 5 bước để đảm bảo bạn không bỏ sót bất kỳ thông tin nào từ các tệp mẫu. Step 1: Khởi động & Bối cảnh (Project Setup) Nguồn dữ liệu: Lấy từ CHANGELOG.md và PROMPTS.md. Nội dung: Chọn Phase đang thực hiện (từ Phase 01 đến Phase 06). Xác định Ngày thực hiện và Nhiệm vụ chính (Checklist các đầu việc đã hoàn thành). Step 2: Công cụ & Mục tiêu (Strategy) Nguồn dữ liệu: Lấy từ AI_AUDIT_LOG.md. Nội dung: Chọn Công cụ AI đã sử dụng (ChatGPT, Gemini, Claude, Cursor...). Xác định Mục tiêu sử dụng (Thiết kế database, Viết code, Debug, Tối ưu...). Step 3: Tương tác chi tiết (Prompting) Nguồn dữ liệu: Lấy từ PROMPTS.md. Nội dung: Prompt nguyên văn: Ô nhập liệu (textarea) cho câu lệnh bạn đã dùng. Bối cảnh: Tại sao bạn cần dùng prompt này? Kết quả AI: Tóm tắt phản hồi từ AI. Đánh giá: Checklist chất lượng prompt (Rõ ràng? Có lỗi? Cần chỉnh sửa nhiều?). Step 4: Thực thi & Minh chứng (Implementation) Nguồn dữ liệu: Lấy từ AI_AUDIT_LOG.md và CHANGELOG.md. Nội dung: Mức độ sử dụng: (Hỗ trợ ý tưởng / Sinh chính nội dung / Tự chỉnh sửa). Phần tự cải tiến: Mô tả chi tiết những gì bạn đã sửa từ code của AI. Minh chứng: Ô nhập Link commit, upload screenshot hoặc kết quả test. Step 5: Tự vấn & Hoàn thiện (Reflection) Nguồn dữ liệu: Lấy từ REFLECTION.md. Nội dung: Kiểm chứng: Bạn đã test kết quả AI như thế nào? (Chạy thử, đối chiếu tài liệu...). Bài học: Những gì bạn học được về kiến thức chuyên môn và cách dùng AI có trách nhiệm. Cam kết: Nút xác nhận tính trung thực của dữ liệu. 3. Page 2: Overview (Kho lưu trữ & Dashboard) Nơi quản lý toàn bộ lịch sử làm việc của bạn. Bộ lọc thông minh (Smart Filters): Lọc theo thời gian: Hôm nay, tuần này, tháng này. Lọc theo Phase: Xem lại quá trình từ lúc phân tích yêu cầu đến khi code xong. Giao diện Dashboard: Hiển thị danh sách các "Entry" (Lần nhập liệu) dưới dạng thẻ (Card). Mỗi thẻ tóm tắt: Ngày, Phase, Công cụ AI dùng chính, và một đoạn ngắn Prompt quan trọng nhất. Chức năng Xuất bản (Export Engine): Tính năng cốt lõi: Chọn một hoặc nhiều Entry để Export sang Markdown. Hệ thống sẽ tự động ghép dữ liệu vào đúng template của 4 file: PROMPTS.md, AI_AUDIT_LOG.md, CHANGELOG.md, REFLECTION.md. 4. Công nghệ đề xuất (Tech Stack) Framework: Next.js (Phục vụ tốt cho SEO và render Markdown). Cơ sở dữ liệu: Supabase (Lưu trữ các entry và quản lý xác thực người dùng). UI Library: Tailwind CSS + shadcn/ui (Tạo giao diện hiện đại, sạch sẽ). Tiện ích: Lucide React (Bộ icon cho các phase và công cụ AI). 5. Giá trị mang lại Website này không chỉ là công cụ nhập liệu, mà là một AI Workflow Manager. Nó giúp bạn: Chống lười: Việc chia nhỏ các tệp Markdown phức tạp thành các bước khảo sát giúp việc ghi chép bớt nhàm chán. Chuyên nghiệp hóa: Có minh chứng rõ ràng về việc bạn "làm chủ" AI chứ không phải "phụ thuộc" vào AI. Tối ưu hóa học tập: Thông qua phần Reflection, bạn thực sự đúc kết được kiến thức sau mỗi lần copy-paste code. Phân tích xem đã đầy đủ theo các file tôi gửi chưa
```

#### 5.2. Bối cảnh khi viết prompt

```text
4 files Markdown trong docs
```

#### 5.3. Kết quả AI trả về

```text
AI đọc file, đánh giá ý tưởng, chỉ ra chỗ nào tốt chỗ nào cần cải thiện, triển khai ý tưởng web sâu hơn,...
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
All of of the result (with review)
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Chỉnh sửa lại một vài chỗ còn chưa hợp lí, chỉnh lại các tools, framework sử dụng, confirm lại ý tưởng và các câu hỏi mở của AI
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

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-12 |
| Công cụ AI | ChatGPT |
| Mục đích | Tạo prompt chi tiết từ ý tưởng ban đầu |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Rồi bây giờ từ những ý tưởng trên, hệ thống lại toàn bộ chức năng, để tạo một prompt hoàn chỉnh để tạo project từ đầu đến cuối đầy đủ tính năng và chi tiết theo file md tôi đã gửi.
```

#### 5.2. Bối cảnh khi viết prompt

```text
4 file markdown trong doc
```

#### 5.3. Kết quả AI trả về

```text
Create a full-stack modern web application called “AI Workflow Logger”.

IMPORTANT:
This project must strictly use HeroUI v3 components and design patterns.

Documentation reference:
https://heroui.com/docs/react/components

The UI architecture, layout system, forms, navigation, tables, cards, modals, tabs, drawers, dropdowns, steppers, accordions, and interactive elements must follow HeroUI v3 best practices and component APIs.

Do NOT use shadcn/ui.
Do NOT mix multiple UI libraries.
Use HeroUI v3 as the primary and complete UI system.

==================================================
PROJECT PURPOSE
===============

The application helps students and developers document AI-assisted software development workflows and automatically export structured Markdown documentation.

The app replaces manual editing of markdown files with a guided workflow interface.

The generated output must map correctly to these markdown files:

* PROMPTS.md
* AI_AUDIT_LOG.md
* CHANGELOG.md
* REFLECTION.md

IMPORTANT:
Before implementing the workflow, carefully analyze ALL markdown templates and documentation files inside the `.docs` folder.

The application workflow, forms, sections, export structure, and field mapping must strictly follow the markdown structure and requirements defined in those files.

Do NOT invent arbitrary workflow structures.
The workflow must be derived directly from the markdown templates and documentation rules.

==================================================
TECH STACK
==========

Frontend:

* Next.js App Router
* TypeScript
* Tailwind CSS
* HeroUI v3
* React Hook Form
* Zod
* Lucide React

Backend:

* Supabase
* PostgreSQL
* Supabase Auth
* Supabase Storage

Markdown:

* remark
* gray-matter
* markdown generation utilities

==================================================
DESIGN STYLE
============

Design style:

* dark mode
* minimal
* clean developer workflow aesthetic
* modern productivity app
* structured documentation-focused interface

Inspired by:

* Linear
* Notion
* GitHub Projects
* developer tooling dashboards

Use:

* HeroUI Cards
* HeroUI Tabs
* HeroUI Accordion
* HeroUI Table
* HeroUI Textarea
* HeroUI Select
* HeroUI Input
* HeroUI Modal
* HeroUI Drawer
* HeroUI Chip
* HeroUI Navbar
* HeroUI Sidebar patterns
* HeroUI Progress
* HeroUI Tooltip

Typography:

* Inter
* Geist
* JetBrains Mono for prompt/markdown sections

Avoid:

* overly futuristic visuals
* heavy analytics dashboards
* unnecessary charts
* cluttered UI

==================================================
APPLICATION GOAL
================

Main goals:

* simplify AI workflow logging
* guide users through structured documentation
* generate markdown automatically
* preserve markdown structure
* reduce manual editing effort
* support academic AI audit requirements

==================================================
APPLICATION STRUCTURE
=====================

The application only contains 3 core pages:

1. Dashboard
2. Workflow Workspace
3. Export Center

==================================================
PAGE 1 — DASHBOARD
==================

Create a clean dashboard using HeroUI components.

Features:

* Project cards
* Recent entries
* Quick actions
* Current phase display

Use:

* HeroUI Card
* HeroUI Chip
* HeroUI Button
* HeroUI Dropdown
* HeroUI Divider

Project Card fields:

* project name
* course
* semester
* phase
* last updated

Quick actions:

* Create Project
* Continue Workflow
* Export Markdown

==================================================
PAGE 2 — WORKFLOW WORKSPACE
===========================

This is the core page.

Design it as a guided multi-step workflow system.

Use HeroUI components for:

* step navigation
* forms
* tabs
* collapsible sections
* inputs
* file uploads
* markdown previews

Layout:

* left sidebar stepper
* centered form workspace
* sticky navigation
* responsive layout

Workflow Steps:

1. Project Information
2. Changelog / Phase Update
3. Prompt Log
4. AI Audit Entry
5. Reflection
6. Markdown Preview

The workflow and fields MUST be derived directly from the markdown templates inside `.docs`.

==================================================
STEP 1 — PROJECT INFORMATION
============================

Fields:

* project name
* course
* class
* semester
* lecturer
* repository URL
* members
* student IDs

Use:

* HeroUI Input
* HeroUI Textarea
* HeroUI Select
* HeroUI Card
* HeroUI Chip

==================================================
STEP 2 — CHANGELOG / PHASE UPDATE
=================================

This step maps directly to CHANGELOG.md.

Read the markdown structure carefully and generate matching forms.

Features:

* phase selector
* task checklist
* change logs
* implementation notes
* commit links
* screenshots
* testing notes

Use:

* HeroUI Accordion
* HeroUI CheckboxGroup
* HeroUI Textarea
* HeroUI Input
* HeroUI Tabs

==================================================
STEP 3 — PROMPT LOG
===================

This step maps directly to PROMPTS.md.

Support:

* multiple prompt entries
* categorized prompts
* prompt evaluation
* evidence attachment

Each entry:

* AI tool
* prompt
* context
* AI response summary
* modifications
* result quality
* lessons learned

Use:

* HeroUI Textarea
* HeroUI Select
* HeroUI Tabs
* HeroUI Card
* HeroUI Chip
* HeroUI Textarea with monospace font

==================================================
STEP 4 — AI AUDIT ENTRY
=======================

This step maps directly to AI_AUDIT_LOG.md.

Features:

* AI usage tracking
* contribution logging
* verification methods
* issue tracking
* human modifications
* evidence uploads

Use:

* HeroUI Table
* HeroUI Card
* HeroUI Checkbox
* HeroUI Accordion
* HeroUI Tabs

==================================================
STEP 5 — REFLECTION
===================

This step maps directly to REFLECTION.md.

Features:

* long-form reflections
* AI evaluation
* verification notes
* lessons learned
* responsible AI usage confirmation

Use:

* HeroUI Textarea
* HeroUI Card
* HeroUI CheckboxGroup
* HeroUI Divider

==================================================
STEP 6 — MARKDOWN PREVIEW
=========================

Create a markdown export interface.

Features:

* markdown preview tabs
* syntax-highlighted preview
* copy markdown
* download markdown
* export ZIP

Preview tabs:

* PROMPTS.md
* AI_AUDIT_LOG.md
* CHANGELOG.md
* REFLECTION.md

Use:

* HeroUI Tabs
* HeroUI Snippet
* HeroUI Button
* HeroUI Card

==================================================
IMPORTANT WORKFLOW REQUIREMENTS
===============================

IMPORTANT:
The workflow must NOT be generic.

The implementation must:

* analyze every markdown file in `.docs`
* derive sections dynamically from markdown structure
* preserve headings
* preserve tables
* preserve checklist formats
* preserve markdown hierarchy
* preserve template formatting

The generated markdown output must look like professionally handwritten markdown files.

==================================================
DATABASE DESIGN
===============

Create relational database tables for:

* users
* projects
* members
* prompt_entries
* audit_entries
* changelog_entries
* reflections
* evidences

Use:

* proper relationships
* timestamps
* clean schema design

==================================================
FEATURES
========

Required Features:

* authentication
* create/edit/delete projects
* autosave forms
* markdown generation
* markdown export
* file uploads
* responsive layout
* dark mode
* local draft saving
* markdown preview

Optional Features:

* markdown import
* export ZIP
* duplicate project
* project search

==================================================
CODE QUALITY
============

Requirements:

* production-ready architecture
* reusable HeroUI components
* modular forms
* scalable structure
* full TypeScript typing
* proper validation
* loading states
* error handling
* clean folder structure

==================================================
FOLDER STRUCTURE
================

Generate a clean scalable project structure:

* app/
* components/
* features/
* lib/
* hooks/
* types/
* services/
* markdown/
* export/
* utils/

==================================================
FINAL EXPECTATION
=================

Generate:

* full frontend
* backend integration
* Supabase schema
* markdown generation system
* export engine
* responsive HeroUI interface
* workflow derived from `.docs` markdown templates

The final application should feel like:

“A minimal developer-focused AI workflow documentation tool that converts structured workflow input into clean markdown audit files.”

```

#### 5.4. Kết quả đã áp dụng vào bài

```text
All of it with review
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Local file storage
- Workflow Forms → Structured JSON → Markdown Export Engine
- Phase 1 please ignore markdown import and focus entirely on the workflow-to-markdown export pipeline.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
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
Dưới đây là bản phác thảo toàn bộ ý tưởng của website, được thiết kế để trở thành một "Nhật ký cộng tác AI" (AI-Human Collaboration Ledger) chuyên nghiệp, giúp bạn quản lý quy trình làm việc một cách minh bạch và khoa học. 1. Tên dự án & Tinh thần thiết kế Tên dự án: PowD AI-Audit System Phong cách: Minimalist, Tech-focused (Sử dụng Dark mode, font chữ monospace cho code và bảng biểu). Mục tiêu: Chuyển đổi các tệp Markdown khô khan thành một quy trình nhập liệu trực quan, dễ theo dõi và có thể xuất ngược lại định dạng chuẩn để nộp bài hoặc lưu trữ. 2. Page 1: AI Workspace (Hệ thống nhập liệu Step-by-Step) Trang này được thiết kế như một "Survey Flow" (luồng khảo sát) gồm 5 bước để đảm bảo bạn không bỏ sót bất kỳ thông tin nào từ các tệp mẫu. Step 1: Khởi động & Bối cảnh (Project Setup) Nguồn dữ liệu: Lấy từ CHANGELOG.md và PROMPTS.md. Nội dung: Chọn Phase đang thực hiện (từ Phase 01 đến Phase 06). Xác định Ngày thực hiện và Nhiệm vụ chính (Checklist các đầu việc đã hoàn thành). Step 2: Công cụ & Mục tiêu (Strategy) Nguồn dữ liệu: Lấy từ AI_AUDIT_LOG.md. Nội dung: Chọn Công cụ AI đã sử dụng (ChatGPT, Gemini, Claude, Cursor...). Xác định Mục tiêu sử dụng (Thiết kế database, Viết code, Debug, Tối ưu...). Step 3: Tương tác chi tiết (Prompting) Nguồn dữ liệu: Lấy từ PROMPTS.md. Nội dung: Prompt nguyên văn: Ô nhập liệu (textarea) cho câu lệnh bạn đã dùng. Bối cảnh: Tại sao bạn cần dùng prompt này? Kết quả AI: Tóm tắt phản hồi từ AI. Đánh giá: Checklist chất lượng prompt (Rõ ràng? Có lỗi? Cần chỉnh sửa nhiều?). Step 4: Thực thi & Minh chứng (Implementation) Nguồn dữ liệu: Lấy từ AI_AUDIT_LOG.md và CHANGELOG.md. Nội dung: Mức độ sử dụng: (Hỗ trợ ý tưởng / Sinh chính nội dung / Tự chỉnh sửa). Phần tự cải tiến: Mô tả chi tiết những gì bạn đã sửa từ code của AI. Minh chứng: Ô nhập Link commit, upload screenshot hoặc kết quả test. Step 5: Tự vấn & Hoàn thiện (Reflection) Nguồn dữ liệu: Lấy từ REFLECTION.md. Nội dung: Kiểm chứng: Bạn đã test kết quả AI như thế nào? (Chạy thử, đối chiếu tài liệu...). Bài học: Những gì bạn học được về kiến thức chuyên môn và cách dùng AI có trách nhiệm. Cam kết: Nút xác nhận tính trung thực của dữ liệu. 3. Page 2: Overview (Kho lưu trữ & Dashboard) Nơi quản lý toàn bộ lịch sử làm việc của bạn. Bộ lọc thông minh (Smart Filters): Lọc theo thời gian: Hôm nay, tuần này, tháng này. Lọc theo Phase: Xem lại quá trình từ lúc phân tích yêu cầu đến khi code xong. Giao diện Dashboard: Hiển thị danh sách các "Entry" (Lần nhập liệu) dưới dạng thẻ (Card). Mỗi thẻ tóm tắt: Ngày, Phase, Công cụ AI dùng chính, và một đoạn ngắn Prompt quan trọng nhất. Chức năng Xuất bản (Export Engine): Tính năng cốt lõi: Chọn một hoặc nhiều Entry để Export sang Markdown. Hệ thống sẽ tự động ghép dữ liệu vào đúng template của 4 file: PROMPTS.md, AI_AUDIT_LOG.md, CHANGELOG.md, REFLECTION.md. 4. Công nghệ đề xuất (Tech Stack) Framework: Next.js (Phục vụ tốt cho SEO và render Markdown). Cơ sở dữ liệu: Supabase (Lưu trữ các entry và quản lý xác thực người dùng). UI Library: Tailwind CSS + shadcn/ui (Tạo giao diện hiện đại, sạch sẽ). Tiện ích: Lucide React (Bộ icon cho các phase và công cụ AI). 5. Giá trị mang lại Website này không chỉ là công cụ nhập liệu, mà là một AI Workflow Manager. Nó giúp bạn: Chống lười: Việc chia nhỏ các tệp Markdown phức tạp thành các bước khảo sát giúp việc ghi chép bớt nhàm chán. Chuyên nghiệp hóa: Có minh chứng rõ ràng về việc bạn "làm chủ" AI chứ không phải "phụ thuộc" vào AI. Tối ưu hóa học tập: Thông qua phần Reflection, bạn thực sự đúc kết được kiến thức sau mỗi lần copy-paste code. Phân tích xem đã đầy đủ theo các file tôi gửi chưa
```

### 6.2. Vì sao prompt này quan trọng?

```text
 
```

### 6.3. Kết quả prompt này mang lại

```text
AI đọc file, đánh giá ý tưởng, chỉ ra chỗ nào tốt chỗ nào cần cải thiện, triển khai ý tưởng web sâu hơn,...
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
All of of the result (with review)
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Chỉnh sửa lại một vài chỗ còn chưa hợp lí, chỉnh lại các tools, framework sử dụng, confirm lại ý tưởng và các câu hỏi mở của AI
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
Mục tiêu rõ ràng: muốn AI làm gì
Context: project, tech stack, tình huống hiện tại
Yêu cầu cụ thể: dùng framework gì, style gì, best practice không
Constraint: không dùng gì, giới hạn gì
Output mong muốn: code, roadmap, bảng, markdown, prompt,...
Ví dụ input/output nếu có
Thông tin hiện tại: lỗi gì, đã thử gì rồi
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Prompt tốt thường có:
- Mục tiêu rõ ràng
- Context đầy đủ
- Yêu cầu cụ thể
- Constraint / giới hạn
- Format output mong muốn
- Ví dụ nếu có
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Cung cấp nhiều context hơn
Nói rõ output muốn nhận
Đưa constraint ngay từ đầu
Thêm ví dụ thực tế
Chia task lớn thành nhiều bước
Mention tech stack/version cụ thể
Mô tả lỗi và expected behavior rõ hơn
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Design | 2 |  |

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
| Nguyễn Hoàng Ngọc Ánh | 14/5/2026 |
