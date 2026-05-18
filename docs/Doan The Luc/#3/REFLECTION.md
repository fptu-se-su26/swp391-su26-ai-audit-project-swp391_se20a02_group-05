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
| Ngày hoàn thành reflection | 2026-05-18 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...

---

## 3. Tóm tắt quá trình sử dụng AI

```text
- Sinh mã nguồn và Cấu hình hạ tầng (Generation): Nhóm đã sử dụng GitHub Copilot để sinh nhanh các đoạn mã lặp (boilerplate), xây dựng khung giao diện Auth UI, tạo Custom Hook useAuth và thiết lập mẫu thiết kế UserBuilder cho tầng kiểm thử. Song song đó, ChatGPT được khai thác để thiết kế các giải thuật bảo mật phức tạp như xoay vòng token (Token Rotation), chống CSRF, và chuẩn hóa các file Markdown quy tắc (Agent rules) thành tập lệnh hệ thống gãy gọn cho TripGenie.

- Rà soát và Sửa lỗi bất tương thích (Refactoring): Do AI có những giới hạn về mặt cập nhật phiên bản (ảo tưởng mã nguồn của Next.js Pages Router cũ thay vì App Router mới) và thiếu sót trong việc xử lý điều kiện biên (gây lỗi vòng lặp vô hạn 401 khi Refresh Token hết hạn), nhóm đã chủ động can thiệp thủ công. Toàn đội đã refactor lại toàn bộ logic điều hướng, sửa lỗi xung đột kiểu dữ liệu (TypeScript Type Mismatch) và tối ưu hóa các câu chỉ thị prompt để giảm độ trễ (latency) cho AI Agent.
```

---

## 4. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [x] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
ChatGPT; Gemini; Antigravity
```

### Lý do sử dụng công cụ đó

```text
Ngon bổ và đẳng cấp
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
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [x] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
- Nghiên cứu & Thiết kế hệ thống (Hiểu đề bài, Phân tích bài toán, Ý tưởng, Công nghệ mới, Kiến trúc, Database & UI): AI hỗ trợ nhóm nhanh chóng tiếp cận và làm chủ kiến trúc Next.js App Router. Từ các yêu cầu nghiệp vụ của đề bài, AI đã cùng nhóm phân tích, định hình kiến trúc phân tầng cho hệ thống Authentication bảo mật cao và luồng xử lý prompt của TripGenie Agent. AI cũng gợi ý thiết kế các thực thể database (như cấu trúc lưu log Refresh Token để phục vụ cơ chế hoảng loạn khi phát hiện token reuse) và phác thảo luồng trải nghiệm giao diện người dùng (Auth UI & Chatbot layout) liền mạch.

- Hiện thực hóa mã nguồn & Bảo mật (Viết code mẫu, Tối ưu code, Kiểm tra bảo mật): AI đóng vai trò như một trợ lý đắc lực cung cấp các đoạn mã mẫu chuẩn Production về thuật toán xoay vòng mã thông báo (Token Rotation) và cơ chế mã hóa HttpOnly Cookie chống tấn công CSRF. Đồng thời, AI giúp nhóm tối ưu hóa hiệu năng render phía frontend bằng các React best-practices và tinh chỉnh tài liệu cấu hình prompt cho AI Agent nhằm giảm thiểu tối đa độ trễ (latency), tích hợp các quy tắc bảo mật nghiêm ngặt để chống tấn công Prompt Injection trước khi đưa lên môi trường nghiệm thu TripGenie-uat.

- Kiểm định chất lượng sản phẩm (Viết test case, Debug lỗi, Review code): Trong khâu kiểm thử, AI đã gợi ý áp dụng mẫu thiết kế UserBuilder giúp tối ưu hóa việc tạo dữ liệu giả lập cho hệ thống Unit và Integration Tests, bao phủ toàn diện các kịch bản kiểm thử hiệu năng (Benchmarks). Khi hệ thống gặp lỗi runtime phức tạp như lỗi vòng lặp vô hạn (Infinite Loop 401) hay lỗi bất tương thích kiểu dữ liệu (TypeScript Type Mismatch), AI đã hỗ trợ rà soát chéo mã nguồn (Review code) và định hướng giải pháp xử lý triệt để (Debug).
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
- Tinh thông kiến trúc bảo mật chuẩn Production: Qua việc phản biện và tối ưu hóa mã nguồn do AI gợi ý, nhóm đã tiếp thu sâu sắc tư duy thiết kế hệ thống xác thực nâng cao, hiểu rõ bản chất vận hành của cơ chế xoay vòng mã thông báo (Token Rotation), HttpOnly Cookie, và cách thức phòng ngừa tấn công giả mạo CSRF trên môi trường thực tế.

- Nâng cao tư duy kiểm thử tự động (Automation Testing): AI đã giúp nhóm tiếp cận và ứng dụng thành thạo mẫu thiết kế UserBuilder (Builder Pattern), từ đó học được cách tổ chức các bộ test suite (Unit Test, Integration Test, Benchmarks) một cách khoa học để cam kết chất lượng phần mềm trước khi merge code.

- Phát triển kỹ năng Kỹ nghệ câu lệnh (Prompt Engineering): Nhóm học được cách viết và thiết kế các tập lệnh hệ thống (System Instructions) có cấu trúc, phân tầng rõ ràng (Persona, Core Rules, Policies) để định hình chính xác hành vi và giới hạn nghiệp vụ cho AI Agent (TripGenie) thay vì chỉ giao tiếp bằng câu lệnh rời rạc.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- Ảo tưởng về phiên bản công nghệ (Version Hallucination): AI thường xuyên cung cấp các đoạn mã nguồn lỗi thời thuộc cấu trúc Next.js Pages Router (gói next/router) thay vì cấu trúc App Router hiện đại (next/navigation), buộc nhóm phải mất thêm thời gian để tự tra cứu tài liệu chính thống của Next.js nhằm refactor lại.

- Bỏ sót các kịch bản lỗi phức tạp (Edge Cases): Các giải pháp logic bất đồng bộ liên quan đến Middleware và Axios Interceptor do AI sinh ra thường quá lý tưởng hóa, không xử lý tốt các trường hợp biên như mạng chập chờn hoặc mã Refresh Token hết hạn hoàn toàn, gây ra các lỗi runtime nghiêm trọng (như lỗi vòng lặp vô hạn 401) khiến nhóm gặp nhiều khó khăn khi debug sâu.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Sử dụng AI để tối ưu hóa thời gian nghiên cứu và tạo cấu trúc ban đầu.
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
- Rà soát tĩnh và Đối chiếu tài liệu (Static Review): Ngay sau khi AI sinh mã nguồn cho hệ thống Authentication và bộ quy tắc của TripGenie Agent, toàn đội tiến hành Code Review thủ công. Đối chiếu trực tiếp mã nguồn với tài liệu chính thức của Next.js App Router để phát hiện và thay thế ngay các hàm điều hướng cũ (gói next/router) sang cấu trúc mới (next/navigation), tránh lỗi crash hệ thống khi chạy.

- Kiểm thử tự động với Dữ liệu mẫu (Automated Testing): Nhóm sử dụng mẫu thiết kế UserBuilder do AI gợi ý để viết các ca kiểm thử tích hợp (Integration Tests) và kiểm thử đơn vị (Unit Tests). Tiến hành chạy lệnh 'npm run test' để xác minh tính đúng đắn của giải thuật bảo mật và thực hiện đo hiệu năng tải (Benchmarks) nhằm cam kết hệ thống không bị thắt nút cổ chai dưới áp lực lớn.

- Kiểm thử động và Giám sát Runtime (Dynamic Testing): Khởi chạy ứng dụng trong môi trường local, thực hiện giả lập các kịch bản thực tế như hết hạn Access Token và Refresh Token. Qua việc giám sát tab Network và Console trong Developer Tools, nhóm đã phát hiện kịp thời lỗi vòng lặp vô hạn (Infinite Loop 401) do AI viết thiếu điều kiện biên, từ đó chủ động bổ sung logic Interceptor để xử lý triệt để lỗi.

- Nghiệm thu môi trường (UAT Deployment): Tiến hành biên dịch hệ thống (npm run build) để đảm bảo không còn lỗi xung đột kiểu dữ liệu TypeScript, sau đó deploy trực tiếp lên Vercel để chạy thử nghiệm và nghiệm thu hiệu năng thực tế của TripGenie AI Agent trên nhánh 'TripGenie-uat'.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI đã gợi ý cấu trúc lớp UserBuilder áp dụng thiết kế Builder Pattern bằng TypeScript để sinh dữ liệu giả lập (mock data) linh hoạt cho các kịch bản kiểm thử đơn vị (Unit Test) và kiểm thử tích hợp (Integration Test) của phân hệ Auth. |
| Em/nhóm đã kiểm tra bằng cách nào? | Nhóm đã tích hợp trực tiếp lớp này vào tệp tin auth.test.ts, viết thử nghiệm các ca kiểm thử ghi đè trạng thái (như withAdminRole(), withExpiredToken()) và chạy lệnh thực thi toàn bộ test suite thông qua câu lệnh npm run test. |
| Kết quả kiểm tra | Đúng. Mã nguồn vận hành chính xác, sinh cấu trúc đối tượng khớp hoàn toàn với Interface hệ thống, chạy qua toàn bộ các xác thực kiểm thử mà không gặp lỗi runtime. |
| Em/nhóm đã xử lý tiếp như thế nào? | Nhóm đã đưa UserBuilder làm chuẩn chung cho toàn bộ hạ tầng viết test của hệ thống Auth, tiến hành cấu hình ghi đè biến môi trường (env overrides) và tích hợp vào luồng chạy CI tự động hóa của dự án. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | AI gợi ý đoạn mã xử lý tuyến đường bảo vệ (Route Guard) để chuyển hướng người dùng chưa đăng nhập bằng cách sử dụng gói thư viện import { useRouter } from 'next/router'. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Gợi ý bị lỗi thời. Dự án đang chạy trên kiến trúc Next.js App Router, việc sử dụng gói next/router (chỉ dành cho Pages Router cũ) sẽ khiến ứng dụng bị crash runtime ngay khi render. |
| Em/nhóm phát hiện bằng cách nào? | Hệ thống báo lỗi đỏ (Runtime Error) ngay trên màn hình trình duyệt với thông báo: "NextRouter was not mounted". |
| Em/nhóm đã sửa như thế nào? | Thay đổi câu lệnh import thành import { useRouter } from 'next/navigation' theo đúng chuẩn Next.js mới và bao bọc logic điều hướng vào trong Hook useEffect. |
| Bài học rút ra | Phải luôn khai báo cực kỳ tường minh phiên bản công nghệ mới nhất (Next.js App Router) ngay trong câu lệnh chỉ thị prompt đầu tiên để ép AI không đưa ra mã nguồn lỗi thời. |
| AI đã gợi ý gì? | AI gợi ý đoạn mã cấu hình Middleware xử lý Token Rotation (xoay vòng mã thông báo) và cơ chế Axios Interceptor ở phía Client để tự động làm mới Access Token khi nhận mã lỗi 401. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Logic do AI viết thiếu điều kiện chặn request khi Refresh Token đã hết hạn hoàn toàn, dẫn đến việc Client liên tục gửi yêu cầu làm mới vô hạn mà không dừng lại. |
| Em/nhóm phát hiện bằng cách nào? | Phát hiện thông qua việc kiểm thử thủ công (Manual Testing) luồng hết hạn session trên trình duyệt, tab Network của Developer Tools hiển thị hàng trăm request gọi API /api/auth/refresh liên tục gây treo hệ thống. |
| Em/nhóm đã sửa như thế nào? | Đã chủ động viết lại Axios Interceptor phía Client, bổ sung logic kiểm tra nếu Refresh Token đã hết hạn vĩnh viễn thì hủy ngay request và lập tức ép hướng người dùng về trang /login. |
| Bài học rút ra | Các logic bất đồng bộ và xử lý trạng thái bảo mật mạng (Network States) phức tạp của AI cần phải được kiểm thử biên một cách thủ công nghiêm ngặt trên trình duyệt, không nên quá tin tưởng vào mã nguồn sinh tự động. |
---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
Đoàn Thế Lực - Auth System
Nguyễn Hoàng Ngọc Ánh - Review
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Tốc độ viết mã & Khởi tạo (Coding Velocity) | So sánh trước và sau khi dùng AI | Tận dụng AI sinh nhanh phôi code, cấu hình nền tảng và các khung xương thư mục chuẩn chỉ trong vài phút, giúp rút ngắn từ 40% đến 50% thời gian triển khai thô. | Giải phóng hoàn toàn thời gian khỏi các đầu việc thủ công, giúp nhóm tập trung sớm vào việc tối ưu logic bảo mật hệ thống và kiến trúc nghiệp vụ phức tạp. |
| Chất lượng kiểm thử (Testing Quality & Coverage) | Việc viết mã kiểm thử (Unit/Integration Test) gặp khó khăn do dữ liệu giả lập (mock data) thường bị hardcode cố định, khó bao phủ hết các kịch bản lỗi biên và thiếu các bài đo hiệu năng (Benchmarks). | Ứng dụng thành thạo pattern UserBuilder do AI gợi ý giúp linh hoạt sinh các object test, chạy ổn định trên luồng CI tự động và hoàn thiện hệ thống đo chỉ số tải Benchmarks. | Nâng cao độ bao phủ test suite, đưa hệ thống Auth đạt chuẩn Production-ready và giảm thiểu tối đa các lỗi tiềm ẩn (regression bugs) trước khi merge vào nhánh UAT. |
| Kỹ nghệ câu lệnh cho Agent (Prompt Engineering) | Bộ quy tắc và chính sách vận hành của TripGenie Agent được viết rời rạc dưới dạng văn bản thô, dễ khiến mô hình ngôn ngữ lớn (LLM) bị ảo tưởng và phản hồi chậm do tốn nhiều context tokens. | Chuẩn hóa toàn bộ tài liệu thành cấu trúc chỉ thị hệ thống (System Instructions) dạng Markdown gãy gọn, phân tầng nghiêm ngặt (Persona, Core Rules, Boundary Policies). | AI Agent TripGenie tuân thủ tuyệt đối các quy định nghiệp vụ, chống leak system prompt hiệu quả và tốc độ phản hồi (latency) nhanh hơn rõ rệt trên môi trường kiểm thử. |

---

## 11. Bài học về môn học

```text
Dự án này đã giúp nhóm hiểu rõ tầm quan trọng của việc xây dựng một kiến trúc phân tầng vững chắc và quy trình quản lý mã nguồn (Git Workflow) nghiêm ngặt trước khi triển khai các tính năng nâng cao. Việc hiện thực hóa một hệ thống Auth chuẩn Production-ready đòi hỏi tư duy toàn diện từ tầng Middleware bảo mật, API Route Handlers backend cho đến tầng Client-side Hooks. Đồng thời, nhóm nhận thức sâu sắc rằng việc bao phủ mã nguồn bằng hệ thống kiểm thử tự động (Unit/Integration Test) và chạy đo lường hiệu năng (Benchmarks) là điều kiện kiên quyết để cam kết tính ổn định của sản phẩm trước khi bàn giao lên môi trường nghiệm thu (UAT).
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
Sử dụng AI có trách nhiệm đồng nghĩa với việc không lạm dụng theo kiểu sao chép mù quáng mã nguồn tự động mà thiếu đi khâu kiểm định độc lập. Nhóm nhận ra AI vẫn tồn tại các giới hạn lớn như ảo tưởng phiên bản công nghệ cũ và bỏ sót các kịch bản lỗi biên phức tạp (như lỗi vòng lặp vô hạn trong cơ chế Token Rotation). Việc sinh viên trực tiếp làm chủ kiến trúc, thực hiện rà soát chéo thủ công (Peer Review), tự viết các bộ test suite kiểm chứng runtime và chủ động thiết lập hàng rào chính sách (Policies) để chống Prompt Injection cho TripGenie Agent chính là biểu hiện cốt lõi của việc ứng dụng AI một cách thông minh, đạo đức và có trách nhiệm.
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
- Áp dụng Few-shot Prompting: Trong các phân hệ tiếp theo, nhóm sẽ chủ động cung cấp sẵn cho AI từ 1 đến 2 module mã nguồn mẫu đạt chuẩn hiện tại của dự án để làm ngữ cảnh đầu vào, giúp code sinh ra đạt độ nhất quán cao về phong cách lập trình (Coding style) và cấu trúc kiểu dữ liệu TypeScript.

- Ràng buộc chặt chẽ bối cảnh công nghệ: Khai báo tường minh các thông số công nghệ cốt lõi (Next.js App Router, TypeScript 5, HttpOnly Cookie) ngay trong câu lệnh chỉ thị đầu tiên nhằm triệt tiêu hoàn toàn tình trạng AI gợi ý các đoạn mã lỗi thời.

- Xây dựng kho lưu trữ prompt chung (Prompt Registry): Tập hợp và quản lý các câu chỉ thị hệ thống hiệu quả của cả nhóm để tái sử dụng cho các chặng phát triển sau, giúp tối ưu hóa thời gian tương tác và kiểm soát tốt hơn độ trễ (latency) khi cấu hình AI Agent.
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
| Nguyễn Hoàng Ngọc Ánh | 18/5/2026 |
