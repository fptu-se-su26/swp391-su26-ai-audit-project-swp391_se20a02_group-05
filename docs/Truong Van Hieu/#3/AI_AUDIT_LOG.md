# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - Hệ thống xác thực thông tin và quản lý hồ sơ năng lực dành cho Doanh nghiệp |
| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
| MSSV / Danh sách MSSV | DE190105 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-05-11 |
| Ngày hoàn thành | 2026-06-01 |

---

## 2. Công cụ AI đã sử dụng

Đánh dấu các công cụ AI đã sử dụng trong quá trình thực hiện bài tập/project.

- [x] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Mục tiêu duy nhất của tôi khi sử dụng công cụ AI trong Phase này là hỗ trợ khâu nghiên cứu yêu cầu (Requirement) và phác thảo thiết kế ý tưởng (Design concept). Cụ thể là tham khảo cấu trúc các trường thông tin (fields) và các gợi ý bố cục giao diện (UX Layout) cho biểu mẫu nhập liệu CV chuyên nghiệp, làm nền tảng ý tưởng để nhóm thiết kế phân hệ "Thiết kế và Xuất Hồ sơ năng lực xác thực" dành cho Business User trên hệ thống CVerify. Tôi hoàn toàn không sử dụng AI để sinh mã nguồn (Coding) hay hỗ trợ sửa lỗi lập trình (Debugging).
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-25 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Brainstorming cấu trúc các trường thông tin (fields) và bố cục UX Layout của biểu mẫu nhập liệu Hồ sơ năng lực |
| Phần việc liên quan | Requirement / Design |
| Mức độ sử dụng | Hỗ trợ ý tưởng |

#### 4.1. Prompt đã sử dụng

```text
Tôi đang thiết kế form nhập liệu tạo CV/Hồ sơ năng lực cho người dùng trên hệ thống CVerify. Bạn hãy gợi ý cho tôi một template cấu trúc form chuẩn bao gồm các trường dữ liệu nào (Thông tin cá nhân, Kinh nghiệm, Kỹ năng...) và cách sắp xếp các bước nhập liệu (Step-by-step form) như thế nào để mang lại trải nghiệm tốt nhất, tránh làm người dùng bị ngợp.
```

#### 4.2. Kết quả AI gợi ý

AI đã cung cấp danh mục cấu trúc các trường dữ liệu chuẩn (Personal Info, Education, Experience, Skills, Certifications) và gợi ý giải pháp chia form thành 3 bước (Multi-step form) kèm theo các quy tắc UX như: sử dụng thanh tiến trình (progress bar), nhóm các trường thông tin lồng nhau và cơ chế cho phép nhấn nút "Thêm dòng" động cho kinh nghiệm và học vấn.

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Nhóm chỉ tiếp thu phần ý tưởng tổ chức dữ liệu (Schema ý niệm) và giải pháp chia chặng Multi-step form để đưa vào bản vẽ thiết kế Wireframe/Figma. Chúng tôi hoàn toàn không sử dụng bất kỳ đoạn code nào (HTML/CSS/JS) do AI sinh ra.

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Nhóm đã tiến hành cải tiến sâu sắc ý tưởng của AI để đồng bộ với nghiệp vụ đặc thù của CVerify:
1. Loại bỏ hoàn toàn các trường thông tin phổ thông không cần thiết mà AI gợi ý (như Sở thích, Người tham chiếu, Mục tiêu nghề nghiệp cá nhân).
2. Tập trung bổ sung các trường dữ liệu pháp lý và xác thực đặc thù dành cho Business User (doanh nghiệp) bao gồm: Mã số thuế doanh nghiệp, Giấy phép đăng ký kinh doanh, Chứng nhận chất lượng sản phẩm (ISO, HACCP...), Lịch sử hoạt động pháp lý, Danh sách cổ đông và Trạng thái xác thực kiểm duyệt từ Admin.

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| File liên quan | `docs/Truong Van Hieu/#3/PROMPTS.md` |
| Screenshot | Bản thiết kế Prototype của biểu mẫu đa cấp trên Figma của CVerify |
| Kết quả chạy/test | Sơ đồ phác thảo luồng UX Multi-step được phê duyệt nội bộ |

#### 4.6. Nhận xét cá nhân/nhóm

```text
- Về mặt hiệu quả: AI hoạt động rất tốt trong vai trò một cuốn từ điển bách khoa để "Brainstorming" (gợi ý ý tưởng). Thay vì phải tự ngồi nhớ hoặc đi lướt hàng chục trang web xem một cái form CV cần những gì, AI đã tổng hợp cho nhóm một cái checklist đầy đủ chỉ trong vài giây.
- Bài học rút ra: Tuy nhiên, AI chỉ dừng lại ở mức đưa ra "khung sườn" tham khảo chung chung. Để biến ý tưởng đó thành một thiết kế thực tế chạy được và khớp với nghiệp vụ doanh nghiệp của CVerify, nhóm phải bỏ ra rất nhiều chất xám tự thân để cắt gọt, chỉnh sửa và thiết kế lại dựa trên nghiệp vụ thực tế của hệ thống.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

Đánh dấu mức độ AI hỗ trợ ở từng hạng mục.

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Phân tích yêu cầu |  | [x] |  |  | Tham khảo kịch bản phân bước form nhập liệu |
| Viết user story/use case | [x] |  |  |  | Nhóm tự viết dựa trên đặc tả 83 usecase |
| Thiết kế database | [x] |  |  |  | Nhóm tự thiết kế schema SQL phù hợp với PostgreSQL |
| Thiết kế kiến trúc hệ thống | [x] |  |  |  | Tự thiết kế hệ thống Microservice và API |
| Thiết kế giao diện |  | [x] |  |  | Sử dụng ý niệm Multi-step để tự vẽ wireframe |
| Code frontend | [x] |  |  |  | Tự code Next.js + TailwindCSS 100% |
| Code backend | [x] |  |  |  | Tự code backend APIs 100% |
| Debug lỗi | [x] |  |  |  | Tự gỡ lỗi in ấn và tối ưu hóa DOM bằng chất xám nhóm |
| Viết test case | [x] |  |  |  | Tự lập kịch bản test kiểm thử |
| Kiểm thử sản phẩm | [x] |  |  |  | Thực hiện kiểm thử thủ công và in ấn thực tế |
| Tối ưu code | [x] |  |  |  | Tự tối ưu render bằng react-hook-form |
| Viết báo cáo | [x] |  |  |  | Tự soạn thảo báo cáo học thuật |
| Làm slide thuyết trình | [x] |  |  |  | Tự biên soạn slide thuyết trình nhóm |

---

## 6. Các lỗi hoặc hạn chế từ AI

Ghi lại các trường hợp AI trả lời sai, thiếu, chưa phù hợp hoặc sinh code không chạy.

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI chỉ gợi ý các mẫu form CV xin việc phổ thông (General CV), hoàn toàn chưa sát với nghiệp vụ hồ sơ năng lực pháp lý đặc thù của hệ thống xác thực CVerify. | Đối chiếu trực tiếp ý tưởng của AI với đặc tả 83 Use Cases và 15 nhóm chức năng của hệ thống CVerify. | Loại bỏ các đề xuất dư thừa như "Sở thích", "Người tham chiếu" và tự thiết kế bổ sung các trường pháp lý (Mã số thuế, Giấy phép kinh doanh, Chứng nhận chất lượng). |
| 2 | Đề xuất phân bước Multi-step quá dài dòng và dàn trải, nếu bê nguyên vào sẽ làm giao diện biểu mẫu của Business User bị loãng và khó theo dõi. | Rà soát chéo các yêu cầu về khả năng chịu đựng của giao diện và trải nghiệm người dùng (UX Review). | Tinh lọc, gom nhóm các trường thông tin lồng nhau và giới hạn quy trình nhập liệu trong 3 bước cốt lõi có thanh tiến độ (Progress Bar) kiểm soát trực quan. |

---

## 7. Kiểm chứng kết quả AI

Mô tả cách sinh viên/nhóm kiểm tra lại kết quả do AI gợi ý.

### Nội dung kiểm chứng

```text
Quy trình kiểm chứng ý tưởng từ AI của tôi bao gồm:
1. Đối chiếu nghiệp vụ thực tế (Business Rules Validation): Lấy danh sách các trường thông tin do AI gợi ý, đem đối chiếu chéo với các tài liệu quy chuẩn hồ sơ doanh nghiệp thực tế tại Việt Nam và đặc tả nghiệp vụ của hệ thống CVerify.
2. Thảo luận nội bộ nhóm (Peer Review): Tổ chức họp nhóm ngắn để thảo luận và phản biện xem các trường thông tin nào là thực sự cần thiết cho phân hệ xác thực Business User, từ đó loại bỏ các mục rườm rà.
3. Kiểm chứng qua Wireframe Figma (Prototype Verification): Trực tiếp dựng giao diện biểu mẫu Mockup trên Figma theo bộ khung đã chắt lọc để kiểm tra tính mạch lạc và tiện dụng của luồng UX trước khi tiến hành viết code.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Trong phân hệ Hồ sơ năng lực của Business User trên CVerify, đóng góp của tôi là hoàn toàn tự lực về mặt kỹ thuật, chỉ tham khảo ý tưởng ban đầu của AI ở mức độ sơ khởi:
- Đóng góp thực chất tự thân:
  1. Trực tiếp đưa ra câu hỏi nghiên cứu cấu trúc form và tiếp nhận bộ khung ý tưởng từ AI.
  2. Thực hiện tư duy phản biện để chủ động lọc bỏ 100% các thông tin thừa thãi, không sát nghiệp vụ doanh nghiệp.
  3. Trực tiếp vẽ cấu trúc Form Mockup hoàn chỉnh trên Figma dựa trên bộ khung đã chắt lọc.
  4. Lập trình 100% mã nguồn Next.js + TailwindCSS cho biểu mẫu động, tích hợp bộ đôi react-hook-form + useController để tối ưu hóa hiệu năng render, xử lý in ấn vector chất lượng cao qua CSS Paged Media mà không sử dụng bất kỳ dòng code nào từ AI.
- AI hỗ trợ: Chỉ hỗ trợ "Brainstorming" gợi ý danh mục checklist các trường dữ liệu phổ thông ban đầu.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trương Văn Hiếu | DE190105 | Nghiên cứu, thiết kế giao diện Figma, phát triển và tối ưu hóa phân hệ "Thiết kế và Xuất Hồ sơ năng lực xác thực" | Có (Chỉ hỗ trợ ý tưởng) | Thiết kế Figma, lập trình Front-end Next.js, tối ưu hóa CSS Print và xử lý bảo mật DOMPurify |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Thiết kế sơ đồ Use Case tổng thể gồm 83 UCs chia thành 15 phân vùng chức năng | Có | Bản vẽ XML Draw.io hoàn thiện của CVerify |
| Đoàn Thế Lực | DE200523 | Lập trình Backend APIs và thiết kế database PostgreSQL | Có | Xây dựng API xác thực và lưu trữ dữ liệu doanh nghiệp |
| Nguyễn La Hòa An | DE201043 | Quản lý chất lượng giao diện UX/UI và viết báo cáo SRS | Có | Tài liệu đặc tả yêu cầu phần mềm SRS CVerify |
| Trần Nhất Long | DE200160 | Cấu hình Docker, viết Unit Test cho phân hệ xác thực Business User | Có | Bộ kịch bản kiểm thử và CI/CD pipeline |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI đã hỗ trợ tôi rất tốt trong vai trò một cuốn từ điển bách khoa để "Brainstorming" (gợi ý ý tưởng). AI giúp tổng hợp cho nhóm một cái checklist các trường thông tin cần thiết của một biểu mẫu chuẩn chỉ trong vài giây, giúp tránh thiếu sót các mục cơ bản.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Tôi hoàn toàn không sử dụng bất kỳ đoạn code nào của AI sinh ra và loại bỏ hoàn toàn các trường thông tin cá nhân phổ thông như sở thích, mục tiêu nghề nghiệp, người tham chiếu. Lý do là vì đối tượng sử dụng phân hệ này trên CVerify là Business User (Doanh nghiệp) ứng tuyển xác thực cấp tích xanh, đòi hỏi các thông tin pháp lý doanh nghiệp khắt khe chứ không phải một CV xin việc cá nhân thông thường.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Tôi đã kiểm tra bằng cách đối chiếu chéo danh mục gợi ý của AI với các tài liệu quy chuẩn hồ sơ doanh nghiệp thực tế, đồng thời thảo luận và phản biện nội bộ nhóm để chắt lọc những mục thực sự có giá trị nghiệp vụ cho CVerify.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Nếu không có AI, khâu lên ý tưởng thiết kế form và liệt kê checklist các trường dữ liệu ban đầu sẽ mất nhiều thời gian hơn do phải tự tìm kiếm và phân tích thủ công từ các biểu mẫu trên mạng Internet.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Tôi hiểu rằng việc xây dựng một hệ thống phần mềm quy mô lớn (như CVerify gồm 83 Use Cases và 15 phân vùng) đòi hỏi khâu phân tích yêu cầu cực kỳ chặt chẽ. Thiết kế giao diện và dữ liệu phải bám sát nghiệp vụ thực tế của hệ thống chứ không thể áp dụng máy móc các khuôn mẫu chung chung của AI.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Sử dụng AI có trách nhiệm là chỉ coi AI là một công cụ hỗ trợ gợi ý ý tưởng và tư vấn giải pháp. Lập trình viên phải chịu trách nhiệm hoàn toàn về sản phẩm cuối cùng, có tư duy phản biện sắc bén để sàng lọc thông tin và tự mình làm chủ mọi dòng mã nguồn được đưa vào dự án.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 01/06/2026 |
