# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - Auth System |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-05-23 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình phát triển hệ thống CVerify, AI được sử dụng chủ yếu như một công cụ hỗ trợ phân tích workflow, thiết kế kiến trúc hệ thống xác thực, refactor logic authentication và đánh giá các vấn đề liên quan đến security, scalability và onboarding UX. AI hỗ trợ đề xuất hướng giải quyết, phát hiện edge cases và gợi ý các mô hình kiến trúc phù hợp cho hybrid authentication và trust-based identity system.

Tuy nhiên, các quyết định cuối cùng về kiến trúc, security trade-offs, workflow thực tế và mức độ phù hợp với hệ thống đều được kiểm tra và điều chỉnh thủ công trước khi áp dụng.
```

---

## 4. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Gemini
```

### Lý do sử dụng công cụ đó

```text
Tiết kiệm thời gian, Hỗ trợ brainstorming, Sinh code nhanh, Kiểm tra lỗi, Tạo tài liệu, Hỗ trợ thiết kế UI/UX, Tối ưu thuật toán, Học công nghệ mới, Refactor code, Viết test, Debug hệ thống & Khác: Giúp sức trong các trường hợp như vẫn cần sự review cẩn thận của từng phần
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [ ] Tìm ý tưởng giải pháp
- [x] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
- [ ] Review code
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
 
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp tăng tốc quá trình nghiên cứu kiến trúc hệ thống và giúp tiếp cận các mô hình thiết kế hiện đại nhanh hơn, đặc biệt trong các vấn đề liên quan đến authentication workflow, identity management và scalable onboarding systems.

Những điểm AI giúp em/nhóm học tốt hơn:
- Hỗ trợ phân tích authentication workflows phức tạp
- Gợi ý kiến trúc hybrid authentication và provider linking
- Hỗ trợ phát hiện edge cases trong onboarding flow
- Giúp hiểu rõ hơn về separation of concerns trong backend architecture
- Hỗ trợ đánh giá security risks như account enumeration và cache invalidation
- Đề xuất mô hình scalable identity-state driven authentication
- Giúp cải thiện cách viết prompt kỹ thuật và mô tả workflow hệ thống
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đôi khi đề xuất giải pháp quá phức tạp so với phạm vi thực tế của bài toán
- Một số giải pháp tối ưu cho scalability nhưng chưa phù hợp với giai đoạn MVP
- Một vài đề xuất tạo thêm complexity không cần thiết như Redis caching hoặc distributed auth state quá sớm
- AI đôi lúc chưa cân bằng tốt giữa tính thực tiễn và kiến trúc enterprise-scale
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
AI chủ yếu được sử dụng như công cụ hỗ trợ phân tích và brainstorming kiến trúc. Nhóm vẫn tự đánh giá logic hệ thống, kiểm tra security implications, phân tích workflow và quyết định hướng triển khai cuối cùng. Những phần quan trọng như thiết kế authentication model, đánh giá trade-offs và điều chỉnh kiến trúc đều được thực hiện thủ công thay vì phụ thuộc hoàn toàn vào AI-generated solutions.
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
 
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI đề xuất redesign authentication flow bằng cách tạo:  IdentityStateResolver Redis cache cho auth state State-machine based authentication architecture Separate endpoint để resolve auth state trước khi gửi OTP |
| Em/nhóm đã kiểm tra bằng cách nào? | So sánh complexity với phạm vi bug thực tế Phân tích số lượng network requests phát sinh Kiểm tra khả năng maintainability của frontend state flow Đánh giá security implications Đối chiếu với workflow onboarding thực tế của hệ thống |
| Kết quả kiểm tra | Đúng một phần |
| Em/nhóm đã xử lý tiếp như thế nào? | Nhóm giữ lại:  Identity state concept State-driven UX Provider-aware authentication logic  Đồng thời loại bỏ hoặc trì hoãn:  Redis caching Distributed auth state management Một số abstraction chưa cần thiết ở giai đoạn hiện tại |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI đề xuất sử dụng Redis caching để lưu authentication identity state với TTL và cache invalidation strategy cho toàn bộ email auth resolution flow. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Giải pháp này tạo thêm complexity trong khi authentication state resolution chỉ cần một database query đơn giản và chưa có yêu cầu scale đủ lớn để cần distributed caching. |
| Em/nhóm phát hiện bằng cách nào? | Review kiến trúc tổng thể So sánh implementation cost với actual business value Phân tích mức độ cần thiết của caching trong MVP stage |
| Em/nhóm đã sửa như thế nào? | Chuyển sang resolver đơn giản không dùng Redis Giữ state machine logic nhưng giảm infrastructure complexity Tách rõ MVP solution và future scalability design |
| Bài học rút ra | phạm vi bài toán mức độ scale thực tế complexity budget giai đoạn phát triển của sản phẩm |
---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

Chưa có thông tin đóng góp.

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Coding Speed | Average | Very Fast | Đẳng cấp luôn |
| Documentation | Average | Fast | Hiểu document nhanh |
| Testing | Slow | Basic | Viết test ổn |

---

## 11. Bài học về môn học

- Tầm quan trọng của làm việc nhóm
- Lập kế hoạch kiến trúc phần mềm tốt hơn
- Kiểm thử sớm giúp giảm thiểu lỗi
- Tài liệu hóa dự án rất quan trọng
- Phân tích yêu cầu đóng vai trò then chốt

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Cần kiểm chứng nội dung AI tạo ra
- Tránh sao chép mù quáng kết quả từ AI
- AI chỉ hỗ trợ, không thay thế tư duy
- Tôn trọng tính trung thực trong học thuật
- Kiểm tra kỹ mã nguồn liên quan bảo mật

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

- Nâng cao tiêu chuẩn viết code (Coding standards)
- Viết nhiều unit test/integration test hơn
- Đảm bảo tính nhất quán của UI/UX
- Tối ưu hóa cấu trúc dự án
- Tìm hiểu sâu hơn về thiết kế hệ thống

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 |   |
| Prompt có mục tiêu rõ ràng | 5 |   |
| Kiểm chứng kết quả AI | 5 |   |
| Tự chỉnh sửa/cải tiến | 5 |   |
| Hiểu nội dung đã nộp | 5 |   |
| Reflection có chiều sâu | 5 |   |
| Sử dụng AI có trách nhiệm | 5 |   |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có, nhóm đã đọc, kiểm tra và hiểu nội dung trước khi sử dụng.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có, nhưng sẽ mất nhiều thời gian hơn để nghiên cứu và triển khai.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần thiết kế workflow, chỉnh sửa logic và xử lý lỗi thực tế.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Kỹ năng thiết kế hệ thống, viết prompt và kiểm thử phần mềm.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 24/5/2026 |
