# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
<<<<<<< Updated upstream
| Tên bài tập / Project | TripGenie AI |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-05-15 |
=======
| Tên bài tập / Project | CVerify 1 |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-05-29 |
>>>>>>> Stashed changes

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Summary of the AI ​​usage process in the project.
1. **Input**: Upload tài liệu AI research 20+ trang → Claude đọc bằng `bash_tool`
2. **Analysis**: Claude phân tích 17 chapters, xác định nội dung có thể dùng được
3. **Design**: Claude thiết kế 6-layer architecture, bổ sung các concepts không có trong tài liệu
4. **Output**: Gen 3 files (Summary.md, workflow.json, reflection.md) dùng `create_file`

## Cách Claude Sử Dụng AI
- **Tool-use**: Gọi bash/create_file tools để đọc file, tạo output (giống AI Agent pattern)
- **ReAct loop**: Thought → Action (bash_tool) → Observation → Output
- **Structured output**: Mọi output có schema rõ ràng (JSON, Markdown sections)

## Nội Dung Từ Tài Liệu (80%)
✅ Chapters 4-6, 8-10, 12, 15-16 được sử dụng trực tiếp
✅ Tool-use pattern, 4-phase lifecycle, API stack, security, MVP roadmap

## Cải Thiện Claude Thêm Vào (20%)
- Terminal tool (emit_itinerary) để force JSON output
- Response Dispatcher layer cho FE routing
- 7 parallel AI microservices breakdown
- Concrete cost calculations & Redis TTL strategies
- Edit diff workflow cụ thể

## Gaps Được Identify
❌ Failure recovery paths
❌ User preference learning
❌ Multi-traveler conflict resolution
❌ Offline mode support
❌ A/B testing prompts safely
❌ Explainability (why this choice?)

## Giá Trị Mang Lại
⏱- 2-3 tuần research → 1 giờ gen document
- 14 sections covered (LLM selection, cost, security, scalability, risks)
- Actionable (20-step checklist, concrete schema, specific TTL values)- Production-ready blueprint chứ không phải lý thuyết

---
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
Gemini
Claude
```

### Lý do sử dụng công cụ đó

```text
Understanding the correct context and producing the output that best suits the user's requirements.
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
- [x] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
 
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
The data source is larger and faster to query, resulting in quicker data generation compared to manual methods.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
Sometimes misunderstanding the context and user requirements can lead to outputs that deviate from what was intended.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
AI only assists in searching for documents and information, saving time, and does not participate fully in the research process.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [ ] Chạy thử chương trình
- [x] Kiểm tra output
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
 
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Provide links to documents from official sources for verification. |
| Em/nhóm đã kiểm tra bằng cách nào? | Manually comparing and contrasting generated information with information from official websites on the internet. |
| Kết quả kiểm tra | Đúng |
| Em/nhóm đã xử lý tiếp như thế nào? | Accept the information generated by AI and develop further ideas from it. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | During the research process using AI, the team has not observed any serious information errors from the AI. |
| Vì sao gợi ý đó sai/chưa phù hợp? | none |
| Em/nhóm phát hiện bằng cách nào? | none |
| Em/nhóm đã sửa như thế nào? | none |
| Bài học rút ra | none |
---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

<<<<<<< Updated upstream
```text
Students manually researched the topic requirements to master business rules, along with creating their own documentation and academic evidence to provide context for AI to expand the scope of their research.
```
=======
### [Đóng góp] Review, comment and merge pull request
- **Thành viên:** Trần Nhất Long (DE200160)
- **Minh chứng:** Chưa có
- **Đánh giá AI:** Không sử dụng AI
- **Loại đóng góp thật sự:** AI Assisted (Có sự hỗ trợ của AI)
- **Chi tiết thực hiện & So sánh:** Thực hiện đúng yêu cầu đề tài.
>>>>>>> Stashed changes

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Understanding requirements | Partially understanding the project's requirements and lack of information | Improve the depth of research by ensuring the literature is accompanied by more accurate information. | Understand the project's business structure. |

---

## 11. Bài học về môn học

<<<<<<< Updated upstream
```text
Learn how to use GitHub effectively and how to use AI efficiently for project design.
```
=======
- Lập kế hoạch kiến trúc phần mềm tốt hơn
- Tài liệu hóa dự án rất quan trọng
- Kiểm thử sớm giúp giảm thiểu lỗi
- Tầm quan trọng của làm việc nhóm

Sau dự án CVerify, nhóm hiểu sâu hơn nhiều về SDLC thực tế:

1. Requirements là nền tảng: Một user story không rõ ràng sẽ dẫn đến code sai, phải làm lại. Đầu tư thời gian cho requirements không bao giờ là lãng phí.

2. Design trước, code sau: ERD và API design kỹ giúp implementation nhanh và ít bug hơn. Ngược lại, "code trước design sau" tạo ra technical debt khó xử lý.

3. Testing là kỹ năng, không phải việc thêm vào cuối: Nhóm học được rằng viết test case song song với code giúp phát hiện bug sớm và tiết kiệm thời gian tổng thể.

4. Git workflow quan trọng hơn nhóm nghĩ: PR review giúp code quality tốt hơn và là cơ hội học từ code của nhau.
>>>>>>> Stashed changes

---

## 12. Bài học về sử dụng AI có trách nhiệm

<<<<<<< Updated upstream
```text
Include maintaining human oversight to avoid bias, protecting data privacy, verifying AI-generated content to avoid inaccuracies, and being transparent when AI is used.
```
=======
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
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
```text
Provide context, reasonable and complete requirements for AI. Do not let AI generate code or documentation from scratch; manual testing and verification are mandatory afterward.
```
=======
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
>>>>>>> Stashed changes

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 |   |
| Prompt có mục tiêu rõ ràng | 5 |   |
| Kiểm chứng kết quả AI | 5 |   |
| Tự chỉnh sửa/cải tiến | 4 |   |
| Hiểu nội dung đã nộp | 4 |   |
| Reflection có chiều sâu | 4 |   |
| Sử dụng AI có trách nhiệm | 5 |   |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Được
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Được
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Designing project frameworks and workflows for engineering business processes.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Management and design skills, project planning. Task allocation and improved use of AI and GitHub.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
<<<<<<< Updated upstream
| Nguyễn Hoàng Ngọc Ánh | 15/5/2026 |
=======
| Nguyễn Hoàng Ngọc Ánh | 29/5/2026 |
>>>>>>> Stashed changes
