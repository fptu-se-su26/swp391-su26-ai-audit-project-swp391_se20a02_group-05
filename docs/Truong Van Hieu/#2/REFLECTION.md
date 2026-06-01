# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | TripGenie |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-06-01 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Tổng quan quá trình sử dụng AI
Mục đích sử dụng: Khắc phục triệt để lỗi cú pháp hình học XML (Could not add object mxGeometry) và tự động hóa việc tái cấu trúc, phân bổ layout lưới tọa độ cho sơ đồ Use Case tổng thể quy mô lớn của dự án.
Công cụ AI áp dụng: ChatGPT / Gemini.
Vai trò của AI: Đóng vai trò trợ lý kỹ thuật chuyên sâu (Kỹ sư XML & Phân tích hệ thống), chịu trách nhiệm sinh chính nội dung mã nguồn XML sạch sau khi được cung cấp đầy đủ ngữ cảnh lỗi và danh sách thực thể.
Các bước triển khai chi tiết
[Bước 1: Phát hiện lỗi] ──> [Bước 2: Cung cấp bối cảnh] ──> [Bước 3: AI xử lý & sửa lỗi] ──> [Bước 4: Kiểm chứng & Tinh chỉnh]
Bước 1: Phát hiện lỗi & Thu thập thông tin
Xác định lỗi hệ thống khiến Draw.io bị crash khi import sơ đồ lớn. Thu thập chính xác mã lỗi hiển thị và bóc tách file XML gốc để tìm nguyên nhân (thiếu hụt thuộc tính hình học trong thẻ <mxGeometry>).
Bước 2: Cung cấp bối cảnh kỹ thuật cho AI
Đưa ra câu lệnh (Prompt) cải tiến có cấu trúc rõ ràng. Cung cấp toàn bộ dữ liệu nền tảng bao gồm: Thông báo lỗi trực quan, quy mô hệ thống (83 Use Cases chia làm 15 phân vùng chức năng) và danh sách các Actor hệ thống liên quan.
Bước 3: AI phân tích và tái cấu trúc mã nguồn
AI tiến hành quét toàn bộ cú pháp, sửa các thẻ cell bị lỗi đóng/mở và tự động tính toán ma trận tọa độ lưới (Grid Layout). Kết quả, AI xuất ra một đoạn mã XML mới hoàn chỉnh, bao bọc gọn gàng các nhóm chức năng vào các container swimlane.
Bước 4: Kiểm chứng và Tinh chỉnh thủ công
Import trực tiếp mã XML mới vào Draw.io để xác minh tính toàn vẹn (Sơ đồ hiển thị mượt mà, không còn lỗi đồ họa). Tiến hành kéo thả và căn chỉnh thủ công vị trí của các Actor chính để tối ưu hóa các đường mũi tên liên kết quan hệ <<include>> và <<extend>> đạt độ trực quan cao nhất.
Bài học kinh nghiệm & Đóng góp
Hiệu quả đạt được: Tiết kiệm hơn 80% thời gian thiết kế sơ đồ, đảm bảo tiến độ Phase 02 hoàn thành đúng hạn và khớp 100% với tài liệu đặc tả hệ thống.
Hạn chế của AI: Các khu vực có mật độ Use Case dày đặc dễ bị chồng chéo các đường liên kết mũi tên, bắt buộc phải có sự can thiệp và rà soát logic lại từ con người.
Kế hoạch cải tiến: Lần tới sẽ áp dụng cấu trúc prompt nâng cao (Few-shot prompting), chia nhỏ hệ thống để AI xử lý từng phân vùng chức năng ngay từ đầu nhằm kiểm soát chất lượng mã nguồn tốt hơn.
```

---

## 4. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
ChatGPT
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
- [ ] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [ ] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
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
Hiệu quả đạt được: Tiết kiệm hơn 80% thời gian thiết kế sơ đồ, đảm bảo tiến độ Phase 02 hoàn thành đúng hạn và khớp 

```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
100% với tài liệu đặc tả hệ thống.
Hạn chế của AI: Các khu vực có mật độ Use Case dày đặc dễ bị chồng chéo các đường liên kết mũi tên, bắt buộc phải có sự can thiệp và rà soát logic lại từ con người.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [ ] Phụ thuộc ít
- [x] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Sử dụng AI để tối ưu hóa thời gian nghiên cứu và tạo cấu trúc ban đầu.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [ ] Chạy thử chương trình
- [ ] Kiểm tra output
- [x] Viết test case
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
- Quá trình kiểm chứng kết quả từ AI được thực hiện qua 3 bước nghiêm ngặt:
1. Đối chiếu tài liệu: Lấy danh mục phân rã chức năng gồm đầy đủ các nhóm Use Case của hệ thống TripGenie ra để làm tiêu chuẩn đối chiếu trực tiếp với cấu trúc các thẻ XML do AI sinh ra.
2. Kiểm tra Output & Dữ liệu mẫu: Sử dụng trực tiếp tệp mã nguồn XML từ AI làm dữ liệu mẫu để import thử nghiệm vào công cụ Draw.io thông qua tính năng Extras -> Edit Diagram.
3. So sánh trước/sau: So sánh trạng thái render đồ họa trước khi dùng AI (hệ thống liên tục crash, báo lỗi mã hình học) và sau khi dùng AI (sơ đồ được kết xuất thành công, hiển thị trực quan các phân vùng chức năng).
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI đã cung cấp một đoạn mã nguồn cấu trúc XML mới, trong đó tự động tái cấu trúc layout ma trận, bổ sung các thuộc tính hình học <mxGeometry> bị khuyết thiếu và phân bổ các Use Cases vào 15 container swimlane chức năng. |
| Em/nhóm đã kiểm tra bằng cách nào? | Nhóm tiến hành import tệp XML của AI vào Draw.io để kiểm tra khả năng kết xuất đồ họa. Đồng thời rà soát thủ công số lượng Use Cases hiển thị trên canvas xem có khớp hoàn toàn với danh mục thiết kế ban đầu hay không. |
| Kết quả kiểm tra | Đúng về mặt cấu trúc cú pháp XML giúp hệ thống hết crash hoàn toàn. Tuy nhiên, sai lệch nhỏ về mặt hiển thị thẩm mỹ do một số đường mũi tên quan hệ bị xếp chồng lên nhau tại các khu vực có mật độ Use Case dày đặc. |
| Em/nhóm đã xử lý tiếp như thế nào? | Nhóm giữ nguyên phần khung XML xử lý core của AI, sau đó thực hiện can thiệp thủ công bằng cách kéo thả, căn chỉnh lại vị trí của các Actor và dãn khoảng cách các luồng liên kết trên canvas để sơ đồ thoáng, dễ đọc hơn. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Trong lượt sinh mã XML đầu tiên, AI đã tự động sắp xếp các Actor hệ thống (Guest, User, Business User, Admin...) tập trung hoàn toàn ở một phía bên trái của sơ đồ canvas. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Cách sắp xếp này chưa phù hợp với nguyên tắc thiết kế sơ đồ quy mô lớn. Nó khiến cho các đường mũi tên liên kết quan hệ <> và <> từ các Actor đi tới 83 Use Cases ở các nhóm chức năng bên phải bị kéo quá dài, cắt chéo qua nhau gây rối mắt và mất tính trực quan. |
| Em/nhóm phát hiện bằng cách nào? | Phát hiện ngay sau khi import file XML vào Draw.io và quan sát giao diện tổng thể trực quan (Visual Inspection) bằng mắt thường. |
| Em/nhóm đã sửa như thế nào? | Thay vì bắt AI sinh lại, nhóm đã tự tay phân bổ lại các Actor theo sơ đồ đa hướng: Đẩy các Actor phổ thông sang cánh trái, Actor quản trị (Admin, Super Admin) lên phía trên và các thực thể tự động (System, AI Microservice) sang cánh phải để tối ưu luồng mũi tên. |
| Bài học rút ra | Không nên kỳ vọng AI tối ưu hoàn hảo 100% phần giao diện thẩm mỹ đối với các sơ đồ kiến trúc phức tạp. AI chỉ mạnh về xử lý cấu trúc dữ liệu nền tảng (XML), còn con người phải chịu trách nhiệm tinh chỉnh bố cục để đảm bảo tính trực quan và quy chuẩn tài liệu kỹ thuật. |
---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

- Đóng góp cốt lõi: Chủ động nghiên cứu cấu trúc mã nguồn XML của công cụ Draw.io, phát hiện lỗi khuyết thiếu thuộc tính trong thẻ cấu trúc hình học khiến hệ thống bị crash khi import ma trận sơ đồ lớn.
- Vai trò xử lý: Trực tiếp xây dựng câu lệnh điều hướng (Prompt) có cấu trúc kỹ thuật rõ ràng để phối hợp cùng AI sinh lại tệp XML sạch, phân chia layout lưới tự động cho toàn bộ 83 Use Cases và bao bọc gọn gàng trong 15 phân vùng chức năng hệ thống TripGenie.
- Hoàn thiện sản phẩm: Thực hiện quy trình kiểm chứng độc lập, import tệp XML thành công và tự tay căn chỉnh thủ công vị trí các Actor đa hướng trên canvas để đảm bảo sơ đồ đạt độ trực quan, đúng quy chuẩn báo cáo kỹ thuật của nhóm.

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Planning | Very Slow | Fast | thời gian và hiệu suất  |

---

## 11. Bài học về môn học

- Tài liệu hóa dự án rất quan trọng
- Lập kế hoạch kiến trúc phần mềm tốt hơn
- Tầm quan trọng của làm việc nhóm

Việc thiết kế và quản lý các sơ đồ kiến trúc phần mềm có quy mô lớn (hơn 80 Use Cases) đòi hỏi tính nhất quán cực kỳ cao giữa tài liệu đặc tả yêu cầu (UDS) và tệp tin kết xuất đồ họa. Nhóm rút ra bài học là cần phải xây dựng một bộ quy chuẩn đặt tên (Naming Convention) cho các cấu trúc ID của cấu phần hệ thống ngay từ đầu, tránh việc phát sinh lỗi cú pháp cấu trúc hình học khi tích hợp hoặc chia sẻ file làm việc chung giữa các thành viên.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Cần kiểm chứng nội dung AI tạo ra

Tuyệt đối không sử dụng các đoạn mã XML hoặc source code do AI sinh ra mà không qua khâu rà soát logic thủ công. Khi AI hỗ trợ giải quyết các bài toán crash hệ thống hoặc tối ưu lưới tọa độ, vai trò của người kỹ sư là phải hiểu rõ bản chất lỗi cú pháp (như lỗi khuyết thiếu thẻ mxGeometry) để kiểm soát được chất lượng và tính toàn vẹn của sản phẩm, đảm bảo sự trung thực và minh bạch trong báo cáo kiểm toán AI.

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

Tuyệt đối không sử dụng các đoạn mã XML hoặc source code do AI sinh ra mà không qua khâu rà soát logic thủ công. Khi AI hỗ trợ giải quyết các bài toán crash hệ thống hoặc tối ưu lưới tọa độ, vai trò của người kỹ sư là phải hiểu rõ bản chất lỗi cú pháp (như lỗi khuyết thiếu thẻ mxGeometry) để kiểm soát được chất lượng và tính toàn vẹn của sản phẩm, đảm bảo sự trung thực và minh bạch trong báo cáo kiểm toán AI.

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 |   |
| Prompt có mục tiêu rõ ràng | 3 |   |
| Kiểm chứng kết quả AI | 4 |   |
| Tự chỉnh sửa/cải tiến | 3 |   |
| Hiểu nội dung đã nộp | 5 |   |
| Reflection có chiều sâu | 2 |   |
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
| Nguyễn Hoàng Ngọc Ánh | 1/6/2026 |
