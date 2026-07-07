# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-06-02 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong dự án phân tích thiết kế hệ thống CVerify, nhóm đã sử dụng Claude (Anthropic) làm công cụ AI chính và Antigravity để hỗ trợ đọc mã nguồn.

Quy trình gồm 3 giai đoạn:
(1) Dùng Antigravity để đọc và trích xuất cạn kiệt 84 Use Cases từ toàn bộ source code CVerify (Controller, permissions-registry.json, Frontend pages).
(2) Dùng Claude để phân tích Actor, phân loại Use Cases theo 10 Packages, xác định quan hệ UML (21 include, 25 extend, 3 generalization) và viết Use Case Specification mẫu (UC21, UC26).
(3) Dùng Claude để sinh code Node.js tạo file .docx BA chuyên nghiệp làm tài liệu cho Development Team.

Tổng thể, AI đóng vai trò công cụ phân tích và soạn thảo — mọi quyết định nghiệp vụ và kiểm duyệt nội dung đều do người dùng thực hiện.Trong dự án phân tích thiết kế hệ thống CVerify, nhóm đã sử dụng Claude (Anthropic) làm công cụ AI chính và Antigravity để hỗ trợ đọc mã nguồn.

Quy trình gồm 3 giai đoạn:
(1) Dùng Antigravity để đọc và trích xuất cạn kiệt 84 Use Cases từ toàn bộ source code CVerify (Controller, permissions-registry.json, Frontend pages).
(2) Dùng Claude để phân tích Actor, phân loại Use Cases theo 10 Packages, xác định quan hệ UML (21 include, 25 extend, 3 generalization) và viết Use Case Specification mẫu (UC21, UC26).
(3) Dùng Claude để sinh code Node.js tạo file .docx BA chuyên nghiệp làm tài liệu cho Development Team.

Tổng thể, AI đóng vai trò công cụ phân tích và soạn thảo — mọi quyết định nghiệp vụ và kiểm duyệt nội dung đều do người dùng thực hiện.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
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
Claude (Anthropic) — dùng xuyên suốt 3 phase: phân tích, thiết kế, sinh tài liệu
```

### Lý do sử dụng công cụ đó

```text
Claude hiểu ngữ cảnh phức tạp tốt, phân tích UML chính xác, sinh văn bản kỹ thuật có cấu trúc rõ ràng và hỗ trợ sinh code .docx trực tiếp trong cùng một cuộc trò chuyện.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [ ] Viết code mẫu
- [ ] Debug lỗi
- [x] Viết test case
- [ ] Review code
- [ ] Tối ưu code
- [x] Kiểm tra bảo mật
- [x] Viết báo cáo
- [x] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
 
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
1. Hiểu sâu hơn về chuẩn UML 2.x: AI giải thích rõ sự khác biệt giữa include/extend/generalization bằng ví dụ cụ thể từ chính dự án, không chỉ lý thuyết chung chung.

2. Học được nguyên tắc "tránh nổ Use Case": Không biến mọi thao tác nhỏ thành UC riêng — đây là lỗi phổ biến mà AI đã chỉ ra và giải thích lý do.

3. Nắm được cách viết Use Case Specification chuẩn Cockburn: Cấu trúc Pre-condition → Main Flow → Alt Flows → Post-condition được minh họa qua UC21 và UC26.

4. Hiểu kiến trúc Enterprise-grade: Cơ chế SessionVersion + Force Logout, HMAC handshake, SSE streaming — AI giúp nhóm hiểu tại sao các UC này được thiết kế như vậy.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
1. Giới hạn context window: Không thể nạp toàn bộ 84 UC + source code cùng lúc → phải chia nhiều lần, dễ mất nhất quán giữa các phần.

2. Không tự đọc source code: AI không trực tiếp đọc được file .cs, .ts, .json → cần Antigravity làm bước trung gian trích xuất trước, sau đó mới đưa cho Claude phân tích.

3. Đôi khi phân tích quá chi tiết: Một số Alt Flow trong UC Specification quá kỹ thuật cho tài liệu môn học cơ bản — nhóm phải tự lọc nội dung phù hợp mức độ môn học.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [ ] Phụ thuộc ít
- [x] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm tự hiểu nghiệp vụ và kiểm duyệt output, nhưng dùng AI để tăng tốc phần soạn thảo và đảm bảo chuẩn UML — không phụ thuộc về mặt tư duy phân tích.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [ ] Chạy thử chương trình
- [x] Kiểm tra output
- [x] Viết test case
- [ ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [x] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Sau khi AI trả về danh sách UC và quan hệ UML, nhóm thực hiện kiểm chứng bằng cách:
1. Đối chiếu từng UC với file permissions-registry.json và Controller tương ứng (do Antigravity đã trích xuất) — đảm bảo không thiếu UC nào từ source code.
2. Kiểm tra Actor của mỗi UC: mỗi UC phải có đúng Actor thực hiện nghiệp vụ đó.
3. Kiểm tra quan hệ include: UC gốc không thể hoàn thành nếu thiếu UC được include — xác nhận bằng logic nghiệp vụ thực tế.
4. So sánh UC Specification với chuẩn Cockburn template và slide bài giảng môn Phân tích Thiết kế Hệ thống.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | UC60 (Cập nhật vai trò người dùng) có quan hệ include với UC64 (Force Logout) — vì thay đổi role phải invalidate session ngay lập tức. |
| Em/nhóm đã kiểm tra bằng cách nào? | Tra cứu UsersAdminController.cs dòng 161–163: xác nhận code tăng SessionVersion ngay sau khi update role — logic include là đúng. |
| Kết quả kiểm tra | Đúng — source code xác nhận việc tăng SessionVersion là bắt buộc mỗi khi thay đổi role hoặc trạng thái tài khoản. |
| Em/nhóm đã xử lý tiếp như thế nào? | Giữ nguyên quan hệ include UC60 → UC64 và bổ sung ghi chú "enterprise-grade security pattern" vào tài liệu BA để làm nổi bật kiến trúc. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
1. Cung cấp toàn bộ mô tả nghiệp vụ dự án: Tính năng Ứng viên (CV template, GitHub/GitLab, AI skill tree, job matching) và Doanh nghiệp (Mức 1 & Mức 2, workspace, JD, AI ranking) — AI không tự biết điều này.

2. Phân tích và đưa ra quyết định cuối cùng về scope: Chọn 21 UC (không phải 30 hay 15) sau khi cân nhắc độ phù hợp với môn học.

3. Thu thập và cung cấp 84 UC từ source code CVerify thực tế (qua Antigravity) — AI không tự truy cập được codebase.

4. Kiểm duyệt và phê duyệt từng output: Xác nhận từng bước trước khi đi tiếp, yêu cầu AI tuân thủ ràng buộc "không thêm UC mới".

5. Định hướng toàn bộ cuộc trò chuyện: Quyết định thứ tự phân tích (Bước 1→2→3→4), format output và độ chi tiết cần thiết.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Hiểu yêu cầu | • Chỉ có mô tả nghiệp vụ thô chưa được cấu trúc hóa • Chưa phân loại được Actor Primary/Secondary • Chưa biết số UC phù hợp là bao nhiêu • Không có tài liệu BA theo chuẩn UML | • 9 Actors phân loại rõ ràng Primary/Secondary • 84 UC đầy đủ chia 10 Packages • 21 include + 25 extend + 3 generalization • Tài liệu BA .docx chuyên nghiệp sẵn dùng |  |

---

## 11. Bài học về môn học

```text
1. Use Case Diagram không phải chỉ là vẽ hình — phân tích Actor và quan hệ UML đòi hỏi hiểu sâu nghiệp vụ thực tế, không thể làm tốt nếu chỉ học lý thuyết.

2. Nguyên tắc "đúng hạt nhân" khi xác định UC: Một UC phải mô tả mục tiêu nghiệp vụ hoàn chỉnh, không phải thao tác UI — đây là kỹ năng quan trọng mà môn học nhấn mạnh.

3. Module hóa System Boundary giúp biểu đồ lớn trở nên quản lý được — chia 84 UC thành 10 Packages thay vì vẽ tất cả trong một diagram.

4. Use Case Specification là tài liệu "cầu nối" thực sự giữa BA và Developer — Alt Flows quan trọng không kém Main Flow.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
1. AI là công cụ tăng tốc, không phải thay thế tư duy: Mọi output của AI đều cần được kiểm chứng với source code và yêu cầu đề bài trước khi sử dụng.

2. Prompt rõ ràng = kết quả tốt hơn: Cung cấp ràng buộc cụ thể ("không thêm UC mới ngoài danh sách") cho kết quả chính xác hơn prompt chung chung.

3. Không nộp nguyên văn output AI: Toàn bộ nội dung đã được nhóm kiểm duyệt, điều chỉnh và phê duyệt trước khi đưa vào tài liệu chính thức.

4. Khai báo minh bạch công cụ AI: Ghi rõ Claude và Antigravity trong tài liệu BA, không che giấu việc sử dụng AI.
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
1. Chuẩn bị dữ liệu đầu vào có cấu trúc (JSON/CSV) thay vì plain text để AI phân tích chính xác và nhất quán hơn.

2. Yêu cầu AI sinh PlantUML code song song với tài liệu BA để tiết kiệm thời gian vẽ diagram.

3. Tách biệt rõ prompt "phân tích" và prompt "sinh tài liệu" — không gộp chung để mỗi output được tối ưu hơn.

4. Học thêm về Sequence Diagram và Activity Diagram để bổ sung cho Use Case Diagram trong các dự án tiếp theo.
```

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
Có. Nhóm có thể giải thích từng quyết định: tại sao chọn 21 UC (không vụn vặt), tại sao UC60 include UC64 (SessionVersion), tại sao UC26 có 5 Alt Flows (HMAC fail, content violation, cancel, timeout, token expire). Mọi nội dung đều đã được đọc và hiểu trước khi đưa vào tài liệu.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
 Có thể — nhưng chậm hơn khoảng 3–4 lần. Phần phân tích Actor và phân loại UC nhóm tự làm được vì đã hiểu nghiệp vụ. Phần khó nhất nếu không có AI là viết Use Case Specification đầy đủ cho 84 UC và sinh file .docx có format chuyên nghiệp.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần cung cấp toàn bộ mô tả nghiệp vụ dự án chi tiết (tính năng Ứng viên, DN Mức 1 & 2, AI integration) và việc trích xuất 84 UC từ source code thực tế của CVerify qua Antigravity — đây là phần AI không thể tự làm thay. Khả năng kiểm duyệt output và đưa ra quyết định về scope cũng thể hiện rõ năng lực phân tích nghiệp vụ.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
1. Vẽ thành thạo Use Case Diagram bằng draw.io / PlantUML mà không cần hướng dẫn từng bước. 2. Viết Use Case Specification độc lập cho UC phức tạp (không cần AI soạn thảo). 3. Học thêm Sequence Diagram để bổ sung cho UC26 (AI Chat Streaming) — flow HMAC + SSE rất phù hợp để biểu diễn bằng Sequence Diagram. 4. Cải thiện kỹ năng prompt engineering: chuẩn bị dữ liệu có cấu trúc, tách prompt theo từng mục tiêu cụ thể.
```

---

## 17. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 2/6/2026 |
