# Reflection

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
| Ngày hoàn thành reflection | 2026-07-01 |

---

## 2. Mục đích Reflection
File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI
```text
Trong quá trình thiết kế và triển khai hệ thống Community Forum hoàn chỉnh cho CVerify, AI đã hỗ trợ đắc lực trong việc sinh kiến trúc backend (17 entity classes, service layer, REST API controller), thiết kế database schema phức tạp, và xây dựng 6 trang frontend. Tuy nhiên, AI sinh code frontend với các Button variant types không tương thích với HeroUI typings của project (bordered, solid, light thay vì primary, tertiary, outline), sử dụng isLoading prop không tồn tại, và import Button từ custom wrapper thay vì trực tiếp từ @heroui/react. Sinh viên chịu trách nhiệm chính trong việc phát hiện và sửa tất cả type errors, refactor toàn bộ 6 trang cho HeroUI compliance, thiết kế hệ thống reputation scoring, tích hợp domain events với notification routing hiện có, và chạy pipeline kiểm chứng 4 bước (build, test, lint, type-check).
```

---

## 4. Công cụ AI đã sử dụng
- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất
```text
Antigravity
```

### Lý do sử dụng công cụ đó
```text
Antigravity tích hợp trực tiếp vào IDE workspace, có khả năng đọc và phân tích toàn bộ codebase hiện có, tự động chạy build/lint/test commands, và hỗ trợ MCP tools cho GitHub operations. Đặc biệt hữu ích khi cần tích hợp module mới vào modular monolith architecture phức tạp.
```

---

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [ ] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
- [x] Review code
- [x] Tối ưu code
- [ ] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

```text
AI đã giúp sinh toàn bộ kiến trúc module forum từ entity layer đến frontend pages. Hỗ trợ đặc biệt mạnh trong việc thiết kế 17 entity với quan hệ phức tạp, sinh ForumService với 20+ phương thức, và xây dựng 6 trang frontend với layout phức tạp. Tuy nhiên, code frontend cần refactor đáng kể để tuân thủ HeroUI component typings.
```

---

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn
```text
Có. AI giúp nhóm:
- Hiểu sâu hơn về thiết kế hệ thống forum phức tạp: recursive nested replies với max depth constraint, reputation scoring gamification, policy-based authorization với resource ownership validation.
- Nắm vững cách tổ chức module mới trong modular monolith architecture: đăng ký entities trong shared DbContext, tích hợp domain events với notification system, cấu hình DI container.
- Học cách thiết kế database schema cho social features: voting, reactions, follows, bookmarks, abuse reports với proper indexing strategy.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn
```text
- AI sử dụng sai variant types của HeroUI Button component (bordered, solid, light thay vì primary, tertiary, outline). Đây là lỗi nghiêm trọng vì TypeScript build thất bại hoàn toàn, chỉ phát hiện được khi chạy npm run build.
- AI sử dụng isLoading prop không tồn tại trên HeroUI Button, dẫn đến type error. Nhóm phải tự tìm pattern chuẩn của project (conditional Spinner child + isDisabled).
- AI import Button từ custom wrapper @/components/ui/button thay vì trực tiếp từ @heroui/react, không tuân thủ design system convention của project cho trang forum mới.
- AI sử dụng custom Tailwind animate-pulse skeleton blocks thay vì native HeroUI <Skeleton /> component, vi phạm component reuse principle.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?
- [ ] Không phụ thuộc
- [ ] Phụ thuộc ít
- [x] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
AI sinh phần lớn boilerplate code cho 17 entities, service layer, và 6 frontend pages, tiết kiệm đáng kể thời gian viết mã lặp đi lặp lại. Tuy nhiên, toàn bộ việc phát hiện type errors, refactor cho HeroUI compliance, thiết kế reputation system, tích hợp notification routing, và verification pipeline đều do nhóm tự thực hiện. Nhóm có đủ kiến thức để tự viết từ đầu nhưng sẽ mất nhiều thời gian hơn.
```

---

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [ ] Viết test case
- [x] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [x] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [x] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng
```text
1. Chạy dotnet build CVerify.API.csproj — biên dịch backend thành công không lỗi.
2. Chạy dotnet test CVerify.sln — 152 automated tests (unit, integration, performance) pass.
3. Sinh và áp dụng EF Core migration lên PostgreSQL — schema update thành công.
4. Chạy npm run build (Next.js production build) — ban đầu thất bại do Button variant type errors, sau refactor thành công hoàn toàn.
5. Chạy npm run lint — zero warnings và zero errors trong thư mục forum.
```

### Ví dụ cụ thể về một lần kiểm chứng
| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Sinh code Button với variant="bordered" trong dropdown trigger của moderation actions. |
| Em/nhóm đã kiểm tra bằng cách nào? | Chạy npm run build (Next.js TypeScript production build). |
| Kết quả kiểm tra | Build thất bại: Type '"bordered"' is not assignable to type '"primary" \| "secondary" \| "tertiary" \| "danger" \| "ghost" \| "danger-soft" \| "outline" \| undefined'. |
| Em/nhóm đã xử lý tiếp như thế nào? | Thay variant="bordered" bằng variant="outline" và chạy lại build thành công. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp
| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Sử dụng Button variant="solid"/"light"/"bordered" và isLoading prop trên HeroUI Button component. |
| Vì sao gợi ý đó sai/chưa phù hợp? | HeroUI Button trong project CVerify chỉ chấp nhận variant "primary"/"secondary"/"tertiary"/"danger"/"ghost"/"danger-soft"/"outline". isLoading prop không tồn tại trong type definitions. |
| Em/nhóm phát hiện bằng cách nào? | TypeScript compiler báo type error khi chạy npm run build. |
| Em/nhóm đã sửa như thế nào? | Thay tất cả variant values bằng giá trị hợp lệ. Thay isLoading bằng conditional Spinner child + isDisabled pattern. |
| Bài học rút ra | Khi AI sinh code sử dụng third-party component library, luôn phải kiểm tra typings thực tế trong project vì AI có thể sử dụng API từ phiên bản khác. |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm
```text
- Phát hiện và sửa toàn bộ TypeScript type errors cho HeroUI Button variants trong 6 trang frontend.
- Thiết kế hệ thống reputation scoring (Topic +5, Reply +2, Upvote +10, Accepted Solution +20) và badge awarding logic.
- Tích hợp outbox domain events (FORUM_TOPIC_CREATED, FORUM_REPLY_CREATED, FORUM_ANSWER_ACCEPTED, FORUM_TOPIC_MODERATED) với hệ thống NotificationRecipientResolver hiện có.
- Thiết lập policy-based authorization cho forum endpoints theo đúng convention HasPermission của project.
- Cấu hình database seeding cho default categories và badge definitions.
- Thay thế custom UI patterns (animate-pulse, isLoading, custom wrapper imports) bằng native HeroUI components.
- Chạy pipeline kiểm chứng 4 bước: build, test, lint, type-check.
```

---

## 10. So sánh trước và sau khi dùng AI
| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Architecture Design | Manual entity-by-entity design | Full 17-entity architecture with relationships | Tiết kiệm ~70% thời gian thiết kế schema phức tạp. |
| Backend Implementation | Manual service/controller coding | Complete service + controller + DTOs generated | Tiết kiệm ~60% thời gian viết boilerplate CRUD code. |
| Frontend Development | Manual page-by-page construction | 6 pages generated (nhưng cần significant refactoring) | Tiết kiệm ~40% thời gian sau khi tính cả refactoring effort. |
| Type Safety | N/A | AI code có type errors nghiêm trọng, cần manual fix | AI giảm hiệu quả ở phần này do sinh code không type-safe. |

---

## 11. Bài học về môn học
- Thiết kế hệ thống forum phức tạp đòi hỏi cân nhắc kỹ giữa flexibility và performance: recursive nested replies cần max depth constraint, reputation scoring cần atomic operations, policy-based authorization cần resource ownership validation.
- Modular monolith architecture cho phép tích hợp module mới mà không phá vỡ cấu trúc hiện có, nhưng đòi hỏi hiểu rõ shared infrastructure (DbContext, DI container, notification system).

---

## 12. Bài học về sử dụng AI có trách nhiệm
- Khi AI sinh code sử dụng component library, luôn chạy TypeScript compiler check ngay lập tức vì AI thường sử dụng API từ phiên bản khác hoặc variant names không chính xác.
- Không tin tưởng hoàn toàn vào AI khi làm việc với custom design system — component wrapper imports, variant types, và loading patterns có thể khác biệt đáng kể giữa projects.
- Pipeline kiểm chứng tự động (build → test → lint → type-check) là bắt buộc sau mỗi lần AI sinh code.

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI
- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

---

## 14. Kế hoạch cải thiện lần sau
- Cung cấp rõ ràng danh sách component library variant types và import paths cho AI trước khi yêu cầu sinh frontend code để tránh type errors.
- Chạy TypeScript build ngay sau mỗi batch code generation thay vì đợi đến cuối session.
- Yêu cầu AI đọc component type definitions trước khi sinh code sử dụng chúng.

---

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|---|---|
| Ghi nhận việc dùng AI trung thực | 5 | |
| Prompt có mục tiêu rõ ràng | 5 | |
| Kiểm chứng kết quả AI | 5 | |
| Tự chỉnh sửa/cải tiến | 5 | |
| Hiểu nội dung đã nộp | 5 | |
| Reflection có chiều sâu | 5 | |
| Sử dụng AI có trách nhiệm | 5 | |

---

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?
```text
Có. Nhóm giải thích rõ được kiến trúc 17 entity forum, cách hoạt động của recursive nested replies với max depth constraint, hệ thống reputation scoring gamification, policy-based authorization với HasPermission attributes, cách tích hợp domain events với notification routing, và lý do phải refactor toàn bộ Button variant types.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?
```text
Có. Việc thiết kế entity relationships, viết EF Core Fluent API configurations, triển khai service layer với CRUD operations, và xây dựng frontend pages với HeroUI components đều là kiến thức nền tảng của nhóm. AI chủ yếu giúp tiết kiệm thời gian viết boilerplate code.
```

---

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Đoàn Thế Lực | 2026-07-01 |
