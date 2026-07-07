# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-05-28 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Nhóm sử dụng AI xuyên suốt toàn bộ 6 phase của dự án CVerify, nhưng với mức độ và mục đích khác nhau ở từng giai đoạn.

Giai đoạn đầu (Phase 01-02), AI được dùng chủ yếu để phân tích yêu cầu và viết user stories. Claude giúp nhóm brainstorm nhanh và gợi ý các tính năng nhóm chưa nghĩ đến (trust score decay, consent toggle). Tuy nhiên, nhóm phải loại bỏ ~20% kết quả vì scope quá lớn.

Giai đoạn thiết kế (Phase 03), AI hỗ trợ thiết kế database schema với pgvector và gợi ý API naming convention. ChatGPT đặc biệt hữu ích trong việc giải thích cách dùng HNSW index — công nghệ hoàn toàn mới với nhóm.

Giai đoạn coding (Phase 04), GitHub Copilot và Cursor là hai công cụ được dùng nhiều nhất. AI sinh boilerplate code, CRUD patterns, và EF Core migrations nhanh hơn khoảng 40%. Tuy nhiên, toàn bộ business logic, thuật toán scoring, và integration logic là do nhóm tự viết.

Công cụ được dùng nhiều nhất: GitHub Copilot (coding), Claude (phân tích + debug), ChatGPT (thiết kế + giải thích khái niệm).

AI giúp cải thiện chất lượng bài làm rõ rệt ở khâu debug — thay vì mất hàng giờ tìm nguyên nhân, AI giúp thu hẹp không gian tìm kiếm xuống 2-3 nguyên nhân có thể.

Có một số phần AI gợi ý nhưng nhóm không sử dụng: kiến trúc microservices, event sourcing, và một số user stories quá phức tạp cho scope SWP391.
```

---

## 4. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [x] GitHub Copilot
- [x] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Claude
```

### Lý do sử dụng công cụ đó

```text
Tiết kiệm thời gian, Sinh code nhanh, Kiểm tra lỗi, Tối ưu thuật toán
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [x] Viết test case
- [x] Review code
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [x] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [x] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
 
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
- Hiểu bài nhanh hơn nhờ AI giải thích khái niệm phức tạp bằng ngôn ngữ đơn giản kèm ví dụ cụ thể.
- Biết thêm pattern tổ chức code (Clean Architecture, Repository Pattern, CQRS) qua ví dụ AI sinh ra.
- Debug lỗi nhanh và học được nguyên nhân gốc rễ — không chỉ fix bug mà hiểu tại sao bug xảy ra.
- Biết cách thiết kế API chuẩn RESTful với naming convention nhất quán.
- Tiếp cận được công nghệ AI mới (pgvector, Function Calling) nằm ngoài curriculum trường.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI không biết context môn học SWP391 → thường gợi ý scope quá lớn hoặc giải pháp quá phức tạp cho dự án sinh viên.
- AI đôi khi sinh code không chạy (wrong API endpoint, deprecated method) → cần verify với official docs.
- AI trả lời chung chung khi prompt không đủ context → mất thời gian hỏi lại nhiều lần.
- Dễ bị "AI-brained" — có xu hướng dùng solution AI gợi ý mà không suy nghĩ xem có giải pháp đơn giản hơn không.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm có xu hướng nhờ AI trước khi tự nghĩ, đặc biệt với debug. Tuy nhiên, nhóm đã nhận ra điều này và thực hiện quy tắc: tự debug ít nhất 15 phút trước khi hỏi AI. Kết quả là kỹ năng debugging của cả nhóm được cải thiện rõ rệt trong nửa sau dự án.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [x] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Nhóm áp dụng quy tắc "Zero Trust AI" cho code: không có đoạn code nào từ AI được commit vào repo mà không qua ít nhất 2 bước kiểm chứng: (1) chạy thử locally và xem output, (2) peer review trong PR. Với technical decisions (kiến trúc, database, index), nhóm đối chiếu với official documentation của PostgreSQL, Microsoft, và Next.js trước khi áp dụng.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | ChatGPT gợi ý dùng IVFFlat index cho pgvector embedding search |
| Em/nhóm đã kiểm tra bằng cách nào? | Đọc pgvector GitHub documentation và so sánh IVFFlat vs HNSW |
| Kết quả kiểm tra | Cần chỉnh sửa — HNSW tốt hơn IVFFlat cho dataset <1M rows vì không cần train trước |
| Em/nhóm đã xử lý tiếp như thế nào? | Dùng HNSW thay vì IVFFlat, viết lại migration script |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | GitHub Copilot sinh code gọi sai GitHub API endpoint cho commit comparison (dùng /compare thay vì /commits với sha range) |
| Vì sao gợi ý đó sai/chưa phù hợp? | AI dùng endpoint cũ (deprecated) không còn return đầy đủ data cần thiết |
| Em/nhóm phát hiện bằng cách nào? | Viết unit test và nhận được response thiếu fields — debug bằng Postman và phát hiện endpoint sai |
| Em/nhóm đã sửa như thế nào? | Tra cứu GitHub REST API v3 docs, dùng đúng endpoint /repos/{owner}/{repo}/commits với since/until params |
| Bài học rút ra | Luôn verify API endpoint với official docs, không tin tưởng AI về external API calls |
---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
### [Đóng góp] Review, comment and merge pull request
- **Thành viên:** Trần Nhất Long (DE200160)
- **Minh chứng:** Chưa có
- **Đánh giá AI:** Không sử dụng AI
- **Loại đóng góp thật sự:** AI Assisted (Có sự hỗ trợ của AI)
- **Chi tiết thực hiện & So sánh:** Thực hiện đúng yêu cầu đề tài.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Research | Slow | Fast | Phân tích nhanh nhưng thiếu sâu, bỏ sót edge cases | Phân tích có hệ thống hơn, ít bỏ sót hơn, toàn diện hơn |

---

## 11. Bài học về môn học

```text
- Lập kế hoạch kiến trúc phần mềm tốt hơn
- Tài liệu hóa dự án rất quan trọng
- Kiểm thử sớm giúp giảm thiểu lỗi
- Tầm quan trọng của làm việc nhóm

Sau dự án CVerify, nhóm hiểu sâu hơn nhiều về SDLC thực tế:

1. Requirements là nền tảng: Một user story không rõ ràng sẽ dẫn đến code sai, phải làm lại. Đầu tư thời gian cho requirements không bao giờ là lãng phí.

2. Design trước, code sau: ERD và API design kỹ giúp implementation nhanh và ít bug hơn. Ngược lại, "code trước design sau" tạo ra technical debt khó xử lý.

3. Testing là kỹ năng, không phải việc thêm vào cuối: Nhóm học được rằng viết test case song song với code giúp phát hiện bug sớm và tiết kiệm thời gian tổng thể.

4. Git workflow quan trọng hơn nhóm nghĩ: PR review giúp code quality tốt hơn và là cơ hội học từ code của nhau.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
- Cần kiểm chứng nội dung AI tạo ra
- Kiểm tra kỹ mã nguồn liên quan bảo mật
- Tôn trọng tính trung thực trong học thuật
- Tránh sao chép mù quáng kết quả từ AI

Dự án CVerify là lần đầu tiên nhóm sử dụng AI một cách có hệ thống trong toàn bộ SDLC. Bài học về AI có trách nhiệm:

1. AI là junior developer rất nhanh, không phải senior: AI sinh code nhanh nhưng không hiểu business context, không biết constraint của dự án, và có thể sai về technical facts. Nhóm phải là người review và quyết định.

2. Transparency quan trọng hơn nhóm nghĩ: Ghi nhận việc sử dụng AI (file này) không chỉ là yêu cầu của giảng viên mà thực sự giúp nhóm nhìn lại quá trình và học được nhiều hơn.

3. Không nộp nguyên văn = không nộp mà không hiểu: Nhóm đặt ra quy tắc: nếu không giải thích được đoạn code này làm gì, thì không được commit. Rule này phòng được việc "black box" từ AI.

4. AI tạo ra ảo giác năng lực: Dễ cảm thấy mình giỏi khi có AI hỗ trợ. Nhưng khi không có AI (ví dụ thi), năng lực thật mới lộ ra. Nhóm cần tự học và hiểu, không chỉ dùng AI để "chạy deadline".

5. Bias của AI: AI có xu hướng đề xuất các giải pháp phức tạp và "over-engineered". Nhóm phải tự đặt câu hỏi "Có giải pháp đơn giản hơn không?" trước khi áp dụng.
```

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

```text
- Nâng cao tiêu chuẩn viết code (Coding standards)
- Viết nhiều unit test/integration test hơn
- Đảm bảo tính nhất quán của UI/UX
- Cải thiện quy trình làm việc với Git

1. Viết prompt theo template cố định ngay từ đầu (Context → Goal → Constraints → Output format) thay vì viết tự nhiên.

2. Cung cấp nhiều ngữ cảnh hơn về môn học và scope trong prompt — đặc biệt là constraints về team size và timeline.

3. Ghi log prompt ngay khi sử dụng, không để cuối phase mới ghi hồi ký — nhiều chi tiết bị quên.

4. Thực hiện "AI-free day" mỗi tuần để kiểm tra năng lực thực sự và không bị phụ thuộc.

5. Luôn đối chiếu kết quả AI với official documentation trước khi sử dụng, đặc biệt với external APIs.

6. Thảo luận với nhóm trước khi áp dụng AI suggestion quan trọng — không để một người quyết định một mình.

7. Liên kết log AI với commit hash cụ thể để truy vết dễ hơn.
```

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 |   |
| Prompt có mục tiêu rõ ràng | 5 |   |
| Kiểm chứng kết quả AI | 5 |   |
| Tự chỉnh sửa/cải tiến | 4 |   |
| Hiểu nội dung đã nộp | 5 |   |
| Reflection có chiều sâu | 4 |   |
| Sử dụng AI có trách nhiệm | 5 |   |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm đặt ra quy tắc từ đầu: không commit code mà không giải thích được. Mọi thành viên đều review code của nhau trong PR. Nhóm tự tin giải thích toàn bộ hệ thống CVerify, từ database schema đến business logic đến deployment architecture.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có, với các phần core. Authentication (JWT), CRUD operations, và database queries là những phần nhóm có thể tự viết lại mà không cần AI. Phần khó nhất nếu không có AI là pgvector HNSW configuration và GitHub API integration — những phần này sẽ cần nhiều thời gian nghiên cứu hơn nhưng vẫn có thể tự làm.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần thể hiện rõ nhất năng lực thật sự của em/nhóm là business logic layer: thuật toán tính OwnershipScore, TrustScore model, và logic semantic matching threshold. Đây là những phần AI không thể gợi ý cụ thể vì không biết business context của CVerify — nhóm phải tự phân tích và thiết kế.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Kỹ năng quản lý và thiết kế, lập kế hoạch dự án. Phân bổ nhiệm vụ và nâng cao hiệu quả sử dụng AI và GitHub.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 28/5/2026 |
