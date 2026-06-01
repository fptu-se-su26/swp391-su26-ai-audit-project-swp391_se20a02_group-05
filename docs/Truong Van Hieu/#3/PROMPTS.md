# Prompt Log

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
| Ngày cập nhật gần nhất | 2026-06-01 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Sinh viên/nhóm cần ghi lại:

- Đã hỏi AI điều gì.
- Mục đích sử dụng prompt.
- Công cụ AI đã sử dụng.
- AI đã trả lời hoặc gợi ý gì.
- Kết quả đó có được áp dụng vào bài hay không.
- Sinh viên/nhóm đã kiểm tra, chỉnh sửa hoặc cải tiến gì sau khi nhận kết quả từ AI.

---

## 3. Công cụ AI đã sử dụng

Đánh dấu các công cụ AI đã sử dụng.

- [x] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 25/05/2026 | ChatGPT | Tham khảo cấu trúc các trường thông tin và gợi ý bố cục giao diện | "Tôi đang thiết kế form nhập liệu tạo CV/Hồ sơ năng lực cho người dùng trên hệ thống CVerify..." | Cung cấp cấu trúc các trường dữ liệu và gợi ý Multi-step form | Có (Chỉ ý tưởng) | Thiết kế Figma Wireframe |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 25/05/2026 |
| Công cụ AI | ChatGPT |
| Mục đích | Tham khảo cấu trúc các trường thông tin (fields) và gợi ý bố cục giao diện (UX Layout) cho một biểu mẫu nhập liệu CV chuyên nghiệp, phục vụ khâu lên ý tưởng thiết kế phân hệ Hồ sơ năng lực trên CVerify |
| Phần việc liên quan | Requirement / Design |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Tôi đang thiết kế form nhập liệu tạo CV/Hồ sơ năng lực cho người dùng trên hệ thống CVerify. Bạn hãy gợi ý cho tôi một template cấu trúc form chuẩn bao gồm các trường dữ liệu nào (Thông tin cá nhân, Kinh nghiệm, Kỹ năng...) và cách sắp xếp các bước nhập liệu (Step-by-step form) như thế nào để mang lại trải nghiệm tốt nhất, tránh làm người dùng bị ngợp.
```

#### 5.2. Bối cảnh khi viết prompt

Nhóm cần dựng biểu mẫu nhập liệu cho Business User tạo hồ sơ xác thực doanh nghiệp. Để tối ưu hóa trải nghiệm người dùng, tránh gây mệt mỏi khi điền form dài, tôi muốn tham khảo cách phân bố các trường dữ liệu và luồng UX Multi-step từ AI để lấy cảm hứng thiết kế Wireframe trên Figma.

#### 5.3. Kết quả AI trả về

AI đã cung cấp danh mục cấu trúc các trường dữ liệu chuẩn (Personal Info, Education, Experience, Skills, Certifications) và gợi ý giải pháp chia form thành 3 bước (Multi-step form) kèm theo các quy tắc UX như: sử dụng thanh tiến trình (progress bar), nhóm các trường thông tin lồng nhau và cơ chế cho phép nhấn nút "Thêm dòng" động.

#### 5.4. Kết quả đã áp dụng vào bài

Nhóm chỉ tiếp thu phần ý tưởng tổ chức dữ liệu (Schema ý niệm) và giải pháp chia chặng Multi-step form để đưa vào bản vẽ thiết kế Wireframe/Figma. Chúng tôi tuyệt đối không sử dụng bất kỳ dòng mã nguồn nào của AI.

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

Nhóm đã loại bỏ hoàn toàn các trường thông tin không cần thiết mà AI gợi ý (như Sở thích, Người tham chiếu, Mục tiêu cá nhân) để tập trung hoàn toàn vào các trường dữ liệu đặc thù phục vụ khâu xác thực doanh nghiệp/đối tác của hệ thống CVerify (như Mã số thuế, Giấy chứng nhận chất lượng, Giấy phép đăng ký kinh doanh).

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều
- [ ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File liên quan | `docs/Truong Van Hieu/#3/PROMPTS.md` |
| Screenshot | Bản thiết kế Prototype của biểu mẫu đa cấp trên Figma của CVerify |

---

## 6. Prompt quan trọng nhất

Chọn một prompt có ảnh hưởng lớn nhất đến bài tập/project.

### 6.1. Prompt được chọn

```text
"Tôi đang thiết kế form nhập liệu tạo CV/Hồ sơ năng lực cho người dùng trên hệ thống CVerify. Bạn hãy gợi ý cho tôi một template cấu trúc form chuẩn bao gồm các trường dữ liệu nào (Thông tin cá nhân, Kinh nghiệm, Kỹ năng...) và cách sắp xếp các bước nhập liệu (Step-by-step form) như thế nào để mang lại trải nghiệm tốt nhất, tránh làm người dùng bị ngợp."
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này giúp định hình tư duy thiết kế luồng nhập liệu biểu mẫu (multi-step form flow) cho phân hệ hồ sơ xác thực của doanh nghiệp. Việc chia chặng biểu mẫu một cách hợp lý giúp giải quyết triệt để vấn đề quá tải thông tin cho người dùng khi điền hồ sơ năng lực pháp lý dài.
```

### 6.3. Kết quả prompt này mang lại

```text
AI mang lại ý tưởng tổ chức trường dữ liệu cơ bản và gợi ý bố cục Multi-step rõ ràng kèm Progress Bar, giúp nhóm nhanh chóng định hình được cấu trúc tổng quan cho sơ đồ phác thảo wireframe.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Nhóm tiến hành đối chiếu các trường thông tin AI đề xuất với tài liệu quy chuẩn hồ sơ doanh nghiệp thực tế tại Việt Nam, thảo luận nội bộ để gạn lọc và bỏ đi các mục dư thừa.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Chúng tôi đã lược bỏ hoàn toàn các trường cá nhân (như sở thích, tham chiếu) và bổ sung các trường pháp lý bắt buộc phục vụ nghiệp vụ xác thực của CVerify (như Mã số thuế, Giấy đăng ký kinh doanh, Chứng nhận chất lượng ISO).
```

---

## 7. Prompt chưa hiệu quả

Giao đoạn thực hiện này tôi chỉ sử dụng duy nhất prompt trên để brainstorm ý tưởng ban đầu và đã chắt lọc rất kỹ lưỡng, do đó chưa phát sinh thêm prompt chưa hiệu quả nào khác trong phân hệ nghiên cứu ý tưởng này.

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
1. Bối cảnh dự án chi tiết (Hệ thống xác thực doanh nghiệp CVerify).
2. Đối tượng đích sử dụng phân hệ (Business User / Doanh nghiệp).
3. Mục tiêu cụ thể của câu hỏi (Tham khảo cấu trúc trường và bố cục UX Form).
4. Ràng buộc thiết kế mong muốn (Tối giản, tiện dụng, tránh làm người dùng bị ngợp).
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Tôi nhận ra đặt câu hỏi rõ ràng, chi tiết, cung cấp bối cảnh ứng dụng thực tế giúp AI đưa ra các ý tưởng brainstorm sát thực tế hơn, tránh được việc trả lời lan man hoặc lạc đề.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Lần sau tôi sẽ chủ động bổ sung thêm các ràng buộc nghiệp vụ chặt chẽ của hệ thống (ví dụ: giới hạn số lượng bước hoặc mô hình xác thực tích xanh của CVerify) để AI hỗ trợ lọc bớt các trường dữ liệu không sát ngay từ câu trả lời đầu tiên.
```

---

## 9. Phân loại prompt đã sử dụng

Đánh dấu số lượng prompt theo từng nhóm.

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt phân tích yêu cầu | 1 | "Tôi đang thiết kế form nhập liệu tạo CV/Hồ sơ năng lực cho người dùng trên hệ thống CVerify..." |
| Prompt giải thích kiến thức | 0 | |
| Prompt thiết kế giải pháp | 0 | |
| Prompt thiết kế database | 0 | |
| Prompt sinh code mẫu | 0 | |
| Prompt debug lỗi | 0 | |
| Prompt viết test case | 0 | |
| Prompt review code | 0 | |
| Prompt tối ưu code | 0 | |
| Prompt viết báo cáo | 0 | |
| Prompt chuẩn bị thuyết trình | 0 | |
| Prompt khác | 0 | |

---

## 10. Checklist chất lượng prompt

Sinh viên/nhóm tự kiểm tra chất lượng prompt đã dùng.

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | [x] | Mục tiêu tham khảo trường dữ liệu |
| Prompt có đủ bối cảnh | [x] | Ghi rõ hệ thống CVerify |
| Prompt có nêu công nghệ/ngôn ngữ sử dụng | [ ] | Giai đoạn lên ý tưởng thiết kế Figma nên chưa cần ghi |
| Prompt có nêu yêu cầu đầu ra | [x] | Yêu cầu gợi ý các bước nhập liệu |
| Prompt không yêu cầu AI làm toàn bộ bài một cách máy móc | [x] | Chỉ hỏi gợi ý brainstorming ý tưởng |
| Prompt có yêu cầu AI giải thích hoặc phân tích | [x] | Hỏi cách sắp xếp để tránh người dùng bị ngợp |
| Kết quả AI được kiểm tra lại | [x] | Đối chiếu chéo với quy chuẩn thực tế |
| Kết quả AI được chỉnh sửa trước khi sử dụng | [x] | Loại bỏ sở thích, thêm mã số thuế/giấy phép kinh doanh |
| Prompt quan trọng được ghi lại đầy đủ | [x] | Ghi chép trung thực trong tệp tin này |
| Prompt sai/chưa hiệu quả được rút kinh nghiệm | [x] | Rút ra bài học cung cấp thêm ràng buộc nghiệp vụ |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết rằng:

- Các prompt quan trọng đã được ghi lại trung thực.
- Không che giấu việc sử dụng AI trong các phần quan trọng của bài.
- Không nộp nguyên văn kết quả AI nếu chưa kiểm tra và chỉnh sửa.
- Có khả năng giải thích các phần đã sử dụng từ AI.
- Chịu trách nhiệm với sản phẩm cuối cùng.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 01/06/2026 |
