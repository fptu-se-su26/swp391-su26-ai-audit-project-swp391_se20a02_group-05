# AI Audit Log

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
| Ngày hoàn thành | 2026-07-01T16:30:00.000Z |

---

## 2. Công cụ AI đã sử dụng
- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

```text
Mục tiêu là thiết kế và triển khai một hệ thống Community Forum hoàn chỉnh, production-ready cho CVerify. Bao gồm:
- Thiết kế kiến trúc backend với 17 entity (categories, topics, replies lồng tối đa 3 cấp, voting, emoji reactions, follows, bookmarks, abuse reports, reputation, badges).
- Xây dựng ForumService tích hợp hệ thống điểm reputation (Topic +5, Reply +2, Upvote +10, Accepted Solution +20) và cấp badge tự động.
- Triển khai REST API với policy-based authorization (forum:category:manage, forum:topic:moderate, forum:moderation:queue) và cho phép đọc công khai.
- Sinh migration Entity Framework Core và áp dụng lên PostgreSQL.
- Xây dựng 6 trang frontend (forum explore, category filter, create topic, topic detail thread, topic edit, moderation queue) sử dụng trực tiếp HeroUI Button, Skeleton, Spinner, TagGroup, Tag.
- Refactor toàn bộ frontend để tuân thủ nghiêm ngặt kiểu dữ liệu TypeScript của HeroUI Button component.
```

---

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-01 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Thiết kế kiến trúc dữ liệu forum với 17 entity và quan hệ nhiều-nhiều, cấu hình DbContext, seeding |
| Phần việc liên quan | Backend (ForumEntities.cs, ApplicationDbContext.cs, DbInitializer.cs) |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implementation Prompt — AI-Native Community Forum System: Build a modern, AI-native discussion platform where candidates can communicate with other candidates, businesses can interact with candidates, organizations can publish discussions. Design 17 entities including categories with visibility scopes, topics, recursive nested replies (max depth 3), voting, emoji reactions, follows, bookmarks, abuse reports, reputation, badges.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất thiết kế 17 entity với các quan hệ phức tạp: ForumCategory (hỗ trợ visibility Public/Private/OrganizationOnly và tenant scope), ForumTopic (với slug tự động và unique prefix), ForumReply (đệ quy với MaxDepth=3), ForumVote, ForumReaction, ForumFollow, ForumBookmark, ForumAbuseReport, ForumReputation, ForumBadge, ForumUserBadge cùng bảng trung gian ForumTopicTag. Đồng thời cấu hình toàn bộ DbSet, Fluent API indices và many-to-many mappings trong ApplicationDbContext.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Cấu trúc 17 entity classes trong ForumEntities.cs.
- Cấu hình DbContext với DbSet properties, composite indices, và Fluent API relationship mappings.
- Logic seeding mặc định cho categories (DevOps, Frontend, Announcements, Hiring, v.v.) và badge definitions trong DbInitializer.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Xác định và thiết kế hệ thống điểm reputation chính xác (Topic +5, Reply +2, Upvote +10, Accepted Solution +20) dựa trên mô hình gamification của platform.
- Thiết lập outbox domain events (FORUM_TOPIC_CREATED, FORUM_REPLY_CREATED, FORUM_ANSWER_ACCEPTED, FORUM_TOPIC_MODERATED) để tích hợp với hệ thống notification hiện có.
- Bổ sung drop-table routines trong DbInitializer cho soft-reset database.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | feat(forum): implement AI-native community forum module with full-stack architecture | https://github.com/Kaivian/CVerify/commit/c9154ed |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Thiết kế 17 entity với quan hệ lồng ghép phức tạp đòi hỏi sự cẩn thận trong việc cấu hình indices và foreign key constraints. AI hỗ trợ tốt trong việc sinh boilerplate nhưng nhóm phải tự kiểm soát tính toàn vẹn dữ liệu và logic nghiệp vụ.
```

---

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-01 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Xây dựng ForumService và ForumController với policy-based authorization |
| Phần việc liên quan | Backend (ForumService.cs, IForumService.cs, ForumController.cs, ForumDtos.cs) |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement ForumService with slugification, reputation scoring, badge awarding, and outbox domain events. Expose REST API via ForumController with policy-based authorization attributes.
```

#### 4.2. Kết quả AI gợi ý

```text
AI triển khai ForumService đầy đủ với các phương thức CRUD cho categories, topics, tags, replies, voting, reactions, bookmarks, follows, reports, moderation. Tự động slugify tiêu đề topic với UUID prefix đảm bảo uniqueness. ForumController expose 20+ REST endpoints với các attributes [HasPermission("forum:category:manage")], [HasPermission("forum:topic:moderate")], [HasPermission("forum:moderation:queue")].
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Toàn bộ service layer với các phương thức CRUD, search, filter, pagination.
- DTO definitions cho request/response mapping.
- Controller endpoints với policy-based authorization attributes.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Tích hợp với hệ thống NotificationRecipientResolver hiện có để định tuyến thông báo cho topic authors, parent-reply authors, topic followers, và administrators.
- Đăng ký IForumService/ForumService trong DI container của Program.cs theo đúng convention của project.
- Bổ sung các activity event types mới vào ActivityEventTypes.cs theo naming convention hiện tại.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | feat(forum): implement AI-native community forum module with full-stack architecture | https://github.com/Kaivian/CVerify/commit/c9154ed |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc tích hợp policy-based authorization đòi hỏi hiểu rõ hệ thống quyền hiện tại của CVerify để gắn đúng permission strings. AI sinh code nhanh nhưng nhóm cần review từng endpoint để đảm bảo an toàn.
```

---

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-07-01 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Xây dựng 6 trang frontend forum với HeroUI components |
| Phần việc liên quan | Frontend (forum.service.ts, 6 page.tsx files) |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Build forum frontend pages: explore stream with category sidebar and tag filtering, category-filtered listing, topic creation form, topic detail thread with markdown rendering and nested replies, topic edit form, moderation queue dashboard. Use direct HeroUI Button, Skeleton, Spinner, TagGroup, Tag components.
```

#### 4.2. Kết quả AI gợi ý

```text
AI xây dựng 6 trang frontend hoàn chỉnh với forum.service.ts làm API client. Trang explore có category sidebar, search input, tag filtering qua TagGroup selectionMode="single". Trang topic detail render markdown content, AI summary card, nested reply feed (max depth 3), voting/reaction buttons, và two-column skeleton loading layout.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- API client forum.service.ts với Axios interceptors.
- 6 page components với layout, search, filter, và form logic.
- Markdown-to-HTML parser an toàn cho nội dung topic.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Phát hiện và sửa lỗi TypeScript type mismatch: Button component của HeroUI trong project chỉ chấp nhận variant "primary" | "secondary" | "tertiary" | "danger" | "ghost" | "danger-soft" | "outline", AI ban đầu sinh code với variant "bordered", "solid", "light" không hợp lệ.
- Thay thế custom isLoading prop (không tồn tại trên HeroUI Button) bằng pattern chuẩn: render Spinner child component conditionally + isDisabled={loading}.
- Chuyển đổi tất cả import Button từ custom wrapper (@/components/ui/button) sang import trực tiếp từ @heroui/react.
- Thay thế tất cả custom animate-pulse skeleton blocks bằng native HeroUI <Skeleton /> component.
- Chuyển đổi tag chip lists và input chips sang native HeroUI <TagGroup> và <Tag> components.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | feat(forum): implement AI-native community forum module with full-stack architecture | https://github.com/Kaivian/CVerify/commit/c9154ed |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Đây là phần đòi hỏi nhiều chỉnh sửa thủ công nhất. AI sinh code frontend nhanh nhưng sử dụng sai variant types của HeroUI Button component khiến TypeScript build thất bại. Nhóm phải kiểm tra kỹ typings của từng component và refactor toàn bộ 6 trang để đảm bảo type safety.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI
| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Phân tích yêu cầu | | x | | | Phân tích yêu cầu forum system từ prompt mô tả chi tiết. |
| Viết user story/use case | x | | | | |
| Thiết kế database | | | x | | Thiết kế 17 entity với quan hệ phức tạp, indices, và many-to-many mappings. |
| Thiết kế kiến trúc hệ thống | | | x | | Service layer, controller, DTO, domain events, notification routing. |
| Thiết kế giao diện | | x | | | Layout tổng thể dựa trên design system HeroUI hiện có. |
| Code frontend | | | x | | 6 trang frontend + API client, nhưng cần refactor nghiêm ngặt cho HeroUI compliance. |
| Code backend | | | x | | 17 entities, service, controller, DTOs, migration. |
| Debug lỗi | | | x | | Sửa TypeScript type errors cho Button variants, Skeleton, TagGroup. |
| Viết test case | x | | | | |
| Kiểm thử sản phẩm | | x | | | Chạy npm run build, npm run lint, dotnet test để kiểm chứng. |
| Tối ưu code | | x | | | Refactor component imports và loading patterns theo chuẩn project. |
| Viết báo cáo | x | | | | |
| Làm slide thuyết trình | x | | | | |

---

## 6. Các lỗi hoặc hạn chế từ AI
| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|--:|---|---|---|
| 1 | AI sinh Button component với variant="bordered", "solid", "light" không tồn tại trong typings của HeroUI Button trong project CVerify. | TypeScript compiler báo lỗi: Type '"bordered"' is not assignable to type '"primary" \| "secondary" \| "tertiary" \| "danger" \| "ghost" \| "danger-soft" \| "outline" \| undefined'. | Thay thế toàn bộ variant values bằng các giá trị hợp lệ: "outline", "tertiary", "primary", "danger". |
| 2 | AI sử dụng isLoading prop trên HeroUI Button component, nhưng prop này không tồn tại trong type definitions. | TypeScript build failure. | Thay bằng pattern render conditional Spinner child + isDisabled={loading}. |
| 3 | AI import Button từ custom wrapper @/components/ui/button thay vì trực tiếp từ @heroui/react. | Review code thấy import path không đúng chuẩn project cho trang forum. | Chuyển tất cả import sang @heroui/react. |
| 4 | AI sử dụng custom Tailwind animate-pulse divs thay vì native HeroUI Skeleton component. | Review code manual phát hiện không tuân thủ design system. | Thay thế bằng <Skeleton /> từ @heroui/react. |

---

```text
Kiểm chứng kết quả qua các hình thức sau:
1. Chạy dotnet build CVerify.API.csproj thành công không lỗi biên dịch.
2. Chạy dotnet test CVerify.sln — toàn bộ 152 tests (unit, integration, performance) pass.
3. Sinh và áp dụng EF Core migration (20260701142634_AddForumModule) lên PostgreSQL thành công.
4. Chạy npm run build (Next.js production build) thành công với zero TypeScript type errors.
5. Chạy npm run lint (ESLint) — zero warnings và zero errors trong thư mục forum (client/src/app/forum).
```

---

### 8.1. Đối với bài cá nhân
```text
- Phát hiện và sửa toàn bộ lỗi TypeScript type mismatch cho HeroUI Button variant trong 6 trang frontend.
- Thiết kế hệ thống reputation scoring và badge awarding logic.
- Tích hợp domain events với hệ thống notification routing hiện có.
- Thiết lập policy-based authorization cho forum endpoints.
- Cấu hình database seeding cho default categories và badge definitions.
```

### 8.2. Đối với bài nhóm
| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Đoàn Thế Lực | DE200523 | Thiết kế kiến trúc forum, triển khai backend + frontend, refactor HeroUI components, verification | Có | GitHub Commits |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Review thiết kế entity, kiểm tra tính nhất quán UI | Có | GitHub Commits |

---

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?
```text
AI hỗ trợ sinh nhanh boilerplate code cho 17 entity classes, service layer với 20+ phương thức CRUD, 6 trang frontend với layout phức tạp, và API client. Đặc biệt hữu ích trong việc thiết kế cấu trúc dữ liệu đệ quy cho nested replies và cấu hình Fluent API relationship mappings.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?
```text
Nhóm không sử dụng các Button variant types mà AI đề xuất (bordered, solid, light) vì chúng không tương thích với HeroUI Button typings trong project. Nhóm cũng không dùng isLoading prop và custom animate-pulse skeleton mà AI sinh ra, thay bằng các pattern chuẩn của project (Spinner child component và native HeroUI Skeleton).
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?
```text
Chạy pipeline kiểm chứng 4 bước: (1) dotnet build để kiểm tra biên dịch backend, (2) dotnet test để chạy 152 automated tests, (3) npm run build để kiểm tra TypeScript type safety của frontend, (4) npm run lint để đảm bảo ESLint compliance.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?
```text
Việc viết thủ công 17 entity classes với đầy đủ các properties, navigation properties, indices, và Fluent API configuration sẽ tốn rất nhiều thời gian và dễ mắc lỗi typo hoặc thiếu sót quan hệ.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?
```text
Hiểu sâu hơn về thiết kế hệ thống forum phức tạp với reputation scoring, nested reply architecture, và policy-based authorization. Nắm vững cách tích hợp module mới vào modular monolith architecture mà không phá vỡ cấu trúc hiện có.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?
```text
Khi AI sinh code sử dụng third-party component library (như HeroUI), luôn phải kiểm tra typings thực tế của component trong project vì AI có thể sử dụng API từ phiên bản khác hoặc variant names không chính xác. Việc chạy TypeScript compiler check là bắt buộc trước khi commit.
```

---

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Đoàn Thế Lực | 2026-07-01 |
