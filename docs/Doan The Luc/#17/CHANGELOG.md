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
| Tên bài tập / Project | CVerify - AI-Native Community Forum Module |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | https://github.com/Kaivian/CVerify |
| Ngày bắt đầu | 2026-07-01T14:00:00.000Z |
| Ngày hoàn thành | 2026-07-01T16:30:00.000Z |

---

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | | | Not Started |
| Phase 02 | | | Not Started |
| Phase 03 | | | Not Started |
| Phase 04 | | | Not Started |
| Phase 05 | | | Not Started |
| Phase 06 | 2026-05-23 ~ 2026-05-23 | Secure Authentication Refactoring & Super Admin Enhancements | Completed |
| Phase 07 | 2026-05-28 ~ 2026-05-28 | Reclaim Ownership OTP Verification & Identity Normalization | Completed |
| Phase 08 | 2026-05-29 ~ 2026-05-29 | Components System Visual Explorer & Workspace Architecture | Completed |
| Phase 09 | 2026-05-30 ~ 2026-05-30 | Secure OAuth Integration & Settings Change Password Overhaul | Completed |
| Phase 10 | 2026-05-31 ~ 2026-05-31 | Email Normalization Correction, Multi-Email Support & Password Recovery Overhaul | Completed |
| Phase 11 | 2026-05-31 ~ 2026-05-31 | Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation | Completed |
| Phase 12 | 2026-06-01 ~ 2026-06-01 | Account Deletion Lifecycle & Modular Monolith Transition | Completed |
| Phase 13 | 2026-06-02 ~ 2026-06-02 | Automatic Username System & Public Profile Routing | Completed |
| Phase 14 | 2026-06-03 ~ 2026-06-03 | Persisting Avatar Source, Re-engineering Experience/Achievements Settings & Form Consistency | Completed |
| Phase 15 | 2026-06-05 ~ 2026-06-06 | Repository Analysis Engine with Real-time SSE Progress Streaming | Completed |
| Phase 16 | 2026-06-15 ~ 2026-06-16 | AI CV Assessment, Source-Code Provider Integrations, and Session Inactivity Management | Completed |
| Phase 17 | 2026-06-17 ~ 2026-06-18 | Candidate Assessment SSE Streaming, Repository Reset, and Dashboard UI Enhancements | Completed |
| Phase 18 | 2026-06-22 ~ 2026-06-22 | Sidebar Navigation Redesign, Accordion Groups & Bi-directional URL Tab Synchronization | Completed |
| Phase 19 | 2026-07-01 ~ 2026-07-01 | AI-Native Community Forum Module — Full-Stack Architecture | Completed |

---

- **Thời gian thực hiện:** 2026-07-01 ~ 2026-07-01
- **Mô tả giai đoạn:** AI-Native Community Forum Module — Full-Stack Architecture with Backend Entities, Services, REST API, Database Migration, and 6 Frontend Pages
- **Trạng thái hiện tại:** Completed

### Added
| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|--:|---|---|---|---|
| 1 | Thiết kế và triển khai 17 entity classes cho hệ thống forum (categories, topics, replies, voting, reactions, follows, bookmarks, reports, reputation, badges). | Đoàn Thế Lực | CVerify.Core/Modules/Forum/Entities/ForumEntities.cs | GitHub Commit |
| 2 | Xây dựng ForumService với CRUD operations, slug generation, reputation scoring (+5/+2/+10/+20), badge awarding, và outbox domain events. | Đoàn Thế Lực | CVerify.Core/Modules/Forum/Services/ForumService.cs, IForumService.cs | GitHub Commit |
| 3 | Triển khai ForumController với 20+ REST endpoints và policy-based authorization attributes. | Đoàn Thế Lực | CVerify.Core/Modules/Forum/Controllers/ForumController.cs | GitHub Commit |
| 4 | Tạo ForumDtos với typed request/response objects cho categories, topics, replies, tags, reports. | Đoàn Thế Lực | CVerify.Core/Modules/Forum/DTOs/ForumDtos.cs | GitHub Commit |
| 5 | Sinh và áp dụng EF Core migration (20260701142634_AddForumModule) lên PostgreSQL. | Đoàn Thế Lực | CVerify.Core/Migrations/20260701142634_AddForumModule.cs | GitHub Commit |
| 6 | Xây dựng forum.service.ts API client với Axios cho frontend. | Đoàn Thế Lực | client/src/services/forum.service.ts | GitHub Commit |
| 7 | Tạo trang forum explore với category sidebar, search, tag filtering qua HeroUI TagGroup. | Đoàn Thế Lực | client/src/app/forum/page.tsx | GitHub Commit |
| 8 | Tạo trang category-filtered topic listing. | Đoàn Thế Lực | client/src/app/forum/[categorySlug]/page.tsx | GitHub Commit |
| 9 | Tạo trang topic creation form với title validation, category selection, HeroUI TagGroup tag input. | Đoàn Thế Lực | client/src/app/forum/new/page.tsx | GitHub Commit |
| 10 | Tạo trang topic detail thread với markdown rendering, AI summary card, nested reply feed, voting, reactions, two-column skeleton loading. | Đoàn Thế Lực | client/src/app/forum/topic/[topicSlug]/page.tsx | GitHub Commit |
| 11 | Tạo trang topic edit form với pre-populated fields và native TagGroup. | Đoàn Thế Lực | client/src/app/forum/topic/[topicSlug]/edit/page.tsx | GitHub Commit |
| 12 | Tạo trang moderation queue dashboard cho reviewing abuse reports. | Đoàn Thế Lực | client/src/app/forum/moderation/page.tsx | GitHub Commit |

### Changed
| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|--:|---|---|---|---|
| 1 | Cấu hình DbSet properties, composite indices, và Fluent API many-to-many relationship mappings cho 17 forum entities. | Đoàn Thế Lực | CVerify.Core/Modules/Shared/Persistence/ApplicationDbContext.cs | GitHub Commit |
| 2 | Tích hợp drop-table routines, default category seeding, và badge definition seeding. | Đoàn Thế Lực | CVerify.Core/Modules/Shared/Persistence/DbInitializer.cs | GitHub Commit |
| 3 | Bổ sung forum-specific activity event types (FORUM_TOPIC_CREATED, FORUM_REPLY_CREATED, FORUM_ANSWER_ACCEPTED, FORUM_TOPIC_MODERATED). | Đoàn Thế Lực | CVerify.Core/Modules/Shared/Domain/Constants/ActivityEventTypes.cs | GitHub Commit |
| 4 | Cấu hình notification routing rules cho topic authors, parent-reply authors, topic followers, administrators. | Đoàn Thế Lực | CVerify.Core/Modules/Shared/Domain/Resolvers/NotificationRecipientResolver.cs | GitHub Commit |
| 5 | Đăng ký IForumService/ForumService trong DI container. | Đoàn Thế Lực | CVerify.Core/Program.cs | GitHub Commit |

### Fixed
| STT | Nội dung sửa lỗi | Người thực hiện | File/Module liên quan | Minh chứng |
|--:|---|---|---|---|
| 1 | Sửa TypeScript type error: Button variant="bordered" không hợp lệ, thay bằng variant="outline". | Đoàn Thế Lực | client/src/app/forum/topic/[topicSlug]/page.tsx | GitHub Commit |
| 2 | Sửa TypeScript type error: Button variant="solid"/"light" không hợp lệ, thay bằng variant="primary"/"tertiary". | Đoàn Thế Lực | 6 forum page.tsx files | GitHub Commit |
| 3 | Thay thế custom isLoading prop không tồn tại bằng conditional Spinner child + isDisabled pattern. | Đoàn Thế Lực | 6 forum page.tsx files | GitHub Commit |

### Removed
| STT | Nội dung xóa bỏ | Người thực hiện | File/Module liên quan | Minh chứng |
|--:|---|---|---|---|
| 1 | Xóa legacy placeholder forum page tại (private)/forum/page.tsx. | Đoàn Thế Lực | client/src/app/(private)/forum/page.tsx | GitHub Commit |

## AI có hỗ trợ không?
- [x] Có
- [ ] Không

## Minh chứng liên quan
| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | feat(forum): implement AI-native community forum module with full-stack architecture | https://github.com/Kaivian/CVerify/commit/c9154ed |

---

### 4.1. Các chức năng đã hoàn thành
```text
- 17 entity classes với quan hệ phức tạp (nhiều-nhiều, đệ quy, tenant-scoped).
- ForumService hoàn chỉnh với CRUD, search, filter, pagination, reputation scoring, badge awarding.
- REST API với 20+ endpoints và policy-based authorization.
- EF Core migration thành công trên PostgreSQL.
- 6 trang frontend tuân thủ nghiêm ngặt HeroUI component typings.
- API client forum.service.ts với full CRUD operations.
- Markdown-to-HTML parser an toàn cho nội dung topic.
- Two-column skeleton loading layout cho trang topic detail.
```

---

### 4.2. Các chức năng chưa hoàn thành
```text
- AI background workers cho automatic topic summarization, duplicate detection, toxicity screening.
- Real-time SignalR notifications cho new replies/reactions.
- Full-text search integration cho forum content.
```

---

### 4.3. Cải thiện chính
```text
- CVerify platform mở rộng từ CV management thuần túy sang nền tảng cộng đồng với discussion forum đầy đủ tính năng.
- Hệ thống reputation và badge gamification tăng engagement và khuyến khích đóng góp chất lượng.
- Policy-based authorization đảm bảo phân quyền chặt chẽ theo vai trò (candidate, moderator, admin).
```

---

### 4.4. Tổng kết project
```text
Giai đoạn này đánh dấu bước mở rộng quan trọng của CVerify từ một nền tảng quản lý CV sang một hệ sinh thái cộng đồng hoàn chỉnh. Forum module được thiết kế với kiến trúc modular monolith, tích hợp tự nhiên với hệ thống authentication, authorization, và notification hiện có mà không tạo ra technical debt.
```

---

### 4.5. Hướng cải thiện tiếp theo
```text
1. Triển khai AI background workers cho topic summarization và toxicity screening.
2. Tích hợp real-time SignalR push notifications cho new replies.
3. Thêm full-text search integration (Elasticsearch/PostgreSQL tsvector).
4. Xây dựng organization-specific forum spaces.
```

---

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Đoàn Thế Lực | 2026-07-01 |
