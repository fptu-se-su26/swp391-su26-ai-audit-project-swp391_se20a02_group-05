# Prompts

## 1. Thông tin chung
| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - AI-Native Community Forum Module |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-07-01T14:00:00.000Z |
| Ngày cập nhật gần nhất | 2026-07-01 |

---

## 2. Mục đích của file Prompt Log
File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng
- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng
| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|--:|---|---|---|---|---|---|---|
| 1 | 2026-07-01 | Antigravity | Thiết kế kiến trúc forum toàn diện | Implementation Prompt — AI-Native Community Forum System... | Sinh 17 entity, service layer, REST API, migration, 6 frontend pages. | Có | GitHub Commit |
| 2 | 2026-07-01 | Antigravity | Mở rộng plan với permission matrix, admin capabilities, org forums | Please introduce a dedicated Forum Permission Matrix... | Bổ sung permission policies, category/tag CRUD, organization-specific forums. | Có | GitHub Commit |
| 3 | 2026-07-01 | Antigravity | Build và lint verification | run build and lint | Phát hiện và sửa TypeScript type errors cho Button variants. | Có | GitHub Commit |
| 4 | 2026-07-01 | Antigravity | Refactor UI components sang HeroUI | Button thì dùng nút của heroUI, Skeleton cũng dùng của HeroUI, tag group dùng của HeroUI | Chuyển đổi toàn bộ 6 trang sang native HeroUI components. | Có | GitHub Commit |

---

### Prompt số 1 (Full-Stack Forum Architecture Design)
| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-01 |
| Công cụ AI | Antigravity |
| Mục đích | Thiết kế và triển khai hệ thống Community Forum hoàn chỉnh cho CVerify. |
| Phần việc liên quan | Backend (Entities, Services, Controller, DTOs, Migration) + Frontend (6 pages, API client) |
| Mức độ sử dụng | Hỗ trợ sinh toàn bộ kiến trúc module từ entity đến frontend pages. |

#### 5.1. Prompt nguyên văn

```text
Implementation Prompt — AI-Native Community Forum System

Your task is to design and implement a complete, production-ready Community Forum module for CVerify.

Before writing or modifying any code, you must first perform a comprehensive analysis of the existing architecture, authentication system, authorization model, organization hierarchy, UI design system, backend services, database schema, and reusable components. The implementation must integrate naturally with the current platform instead of introducing isolated or duplicated solutions.

Do not immediately begin implementation. First evaluate the existing system, identify reusable infrastructure, potential architectural conflicts, security concerns, and integration opportunities, then produce an implementation plan before making changes.

Objectives: Build a modern, AI-native discussion platform where candidates can communicate with other candidates, businesses can interact with candidates, organizations can publish discussions.

[Detailed specifications for entities, services, API, frontend pages, reputation system, moderation queue, etc.]
```

#### 5.2. Bối cảnh khi viết prompt

```text
- CVerify là nền tảng quản lý CV và đánh giá năng lực ứng viên. Platform cần mở rộng sang cộng đồng để tăng engagement.
- Cần tích hợp tự nhiên với hệ thống authentication, authorization, và notification hiện có.
- Yêu cầu modular monolith architecture, không tạo microservice riêng.
```

#### 5.3. Kết quả AI trả về

```text
AI sinh implementation plan chi tiết bao gồm:
- 17 entity classes với đầy đủ quan hệ phức tạp.
- ForumService với 20+ phương thức CRUD, search, filter, reputation scoring.
- ForumController với policy-based authorization.
- 6 trang frontend với layout, search, filter, form logic.
- forum.service.ts API client.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Toàn bộ backend module (entities, DTOs, service, controller, migration).
- Toàn bộ frontend pages sau khi refactor cho HeroUI compliance.
- Tích hợp với existing systems (DbContext, DbInitializer, Program.cs, ActivityEventTypes, NotificationRecipientResolver).
```

---

### Prompt số 2 (Architecture Expansion — Permission Matrix & Admin Capabilities)
| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-01 |
| Công cụ AI | Antigravity |
| Mục đích | Mở rộng plan với permission matrix, admin capabilities cho categories/tags, organization forums. |
| Phần việc liên quan | Backend Architecture Design |
| Mức độ sử dụng | Hỗ trợ thiết kế chi tiết hơn cho permission system và admin features. |

#### 5.1. Prompt nguyên văn

```text
Please introduce a dedicated Forum Permission Matrix instead of relying only on role descriptions. Define explicit permission policies (e.g. create/edit/delete own content, moderate content, manage categories/tags, pin/lock topics) and ensure they integrate with the existing policy-based authorization system and resource ownership validation.

The plan should also include administration capabilities for categories and tags (CRUD, ordering, visibility, archive/private categories), as well as a scalable architecture for Organization-specific forums.

Since CVerify is positioned as an AI-native platform, the AI integration should be described in greater detail.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Implementation plan ban đầu chỉ mô tả role-based permissions một cách tổng quát.
- Cần chi tiết hóa permission matrix để tích hợp với hệ thống HasPermission attributes hiện có.
- Cần admin CRUD cho categories/tags và architecture cho organization-scoped forums.
```

#### 5.3. Kết quả AI trả về

```text
AI bổ sung chi tiết:
- Permission matrix với 4 permission levels: forum:topic:create, forum:topic:moderate, forum:category:manage, forum:moderation:queue.
- Category admin capabilities với visibility scopes (Public, Private, OrganizationOnly).
- Organization-specific forum architecture với tenant scoping.
- AI integration roadmap: topic summarization, duplicate detection, toxicity screening.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Permission attributes trên ForumController endpoints.
- Category visibility scopes trong ForumEntities.
- Organization tenant scoping trong ForumCategory entity.
```

---

### Prompt số 3 (Build & Lint Verification)
| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-01 |
| Công cụ AI | Antigravity |
| Mục đích | Kiểm tra tính toàn vẹn của code sau implementation. |
| Phần việc liên quan | Build Pipeline |
| Mức độ sử dụng | Hỗ trợ chạy và phân tích kết quả build/lint. |

#### 5.1. Prompt nguyên văn

```text
run build and lint
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Sau khi hoàn thành implementation, cần kiểm tra xem code có compile thành công và tuân thủ lint rules hay không.
```

#### 5.3. Kết quả AI trả về

```text
AI chạy npm run build và phát hiện TypeScript type error: Button variant="bordered" không hợp lệ. Tiến hành sửa và chạy lại build thành công.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Sửa Button variant="bordered" thành variant="outline" trong topic/[topicSlug]/page.tsx.
```

---

### Prompt số 4 (HeroUI Component Migration)
| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-01 |
| Công cụ AI | Antigravity |
| Mục đích | Refactor toàn bộ forum frontend sang native HeroUI components. |
| Phần việc liên quan | Frontend (6 page.tsx files) |
| Mức độ sử dụng | Hỗ trợ chuyển đổi imports và props cho Button, Skeleton, TagGroup, Tag. |

#### 5.1. Prompt nguyên văn

```text
recheck lại page:
- Button thì dùng nút của heroUI, không hardcode màu, không hardcode animation
- Skeleton cũng dùng của HeroUI
- Và tag group dùng của HeroUI luôn
```

#### 5.2. Bối cảnh khi viết prompt

```text
- AI ban đầu import Button từ custom wrapper @/components/ui/button và sử dụng custom animate-pulse skeleton blocks.
- CVerify project yêu cầu sử dụng trực tiếp HeroUI components từ @heroui/react.
- Cần đảm bảo tất cả variant props khớp với TypeScript type definitions của project.
```

#### 5.3. Kết quả AI trả về

```text
AI refactor toàn bộ 6 trang:
- Chuyển Button import sang @heroui/react.
- Thay variant="solid"/"light"/"bordered" bằng "primary"/"tertiary"/"outline".
- Thay custom isLoading bằng conditional Spinner child.
- Thay animate-pulse divs bằng native <Skeleton />.
- Thay custom tag chips bằng <TagGroup>/<Tag>.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Toàn bộ 6 trang frontend forum sử dụng trực tiếp HeroUI components.
- Zero TypeScript type errors sau refactor.
- Zero ESLint warnings trong thư mục forum.
```

---

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?
```text
Cần cung cấp chi tiết về design system hiện có (HeroUI variant types, component imports), TypeScript configuration, và existing project patterns. Khi yêu cầu AI thiết kế module mới, cần mô tả rõ hệ thống authorization và notification hiện có để AI tích hợp đúng cách.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?
```text
Khi prompt ban đầu tạo ra code không tuân thủ design system, cần đặt prompt follow-up cụ thể về component library ("dùng nút của heroUI", "Skeleton cũng dùng của HeroUI") thay vì yêu cầu chung chung. Việc chỉ định rõ ràng library và component cụ thể giúp AI sinh code chính xác hơn.
```

---

## 9. Phân loại prompt đã sử dụng
| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Architecture | 1 | Implementation Prompt — AI-Native Community Forum System... |
| Prompt Design | 1 | Please introduce a dedicated Forum Permission Matrix... |
| Prompt Verification | 1 | run build and lint |
| Prompt Fix | 1 | Button thì dùng nút của heroUI, Skeleton cũng dùng của HeroUI... |

---

## 10. Checklist chất lượng prompt
| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | x | |
| Prompt có đủ bối cảnh | x | |
| Tự kiểm tra và chỉnh sửa | x | |

---

## 11. Cam kết sử dụng prompt minh bạch
Sinh viên/nhóm cam kết sử dụng prompt minh bạch và ghi nhận đúng đóng góp của AI.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Đoàn Thế Lực | 2026-07-01 |
