# AI Learning Reflection

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
| Ngày hoàn thành reflection | 10/06/2026 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong phase thiết kế phân hệ Authentication, tôi chỉ sử dụng Claude ở giai đoạn nghiên
cứu nguyên lý với 2 prompt: một để hiểu Refresh Token Rotation và chiến lược Expiry, một để
hiểu cơ chế phát hiện token reuse attack. Toàn bộ thiết kế schema, thiết kế interface, viết
code ASP.NET Core và kiểm thử đều do tôi tự thực hiện.
```

- **Dùng AI ở giai đoạn nào?** Giai đoạn nghiên cứu nguyên lý (Phase 01).
- **Dùng để hỗ trợ việc gì?** Hiểu cơ chế Refresh Token Rotation và chọn chiến lược Expiry.
- **Công cụ sử dụng nhiều nhất?** Claude.
- **AI có cải thiện chất lượng bài?** Có — giúp tôi hiểu bản chất bảo mật nhanh hơn.
- **Phần AI gợi ý nhưng không sử dụng?** Code mẫu (nếu AI tự sinh ra) vì cần tích hợp với
  kiến trúc ASP.NET Core Identity có sẵn.

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor

### Lý do sử dụng Claude

```text
Claude cho phép tôi hỏi câu hỏi kỹ thuật chuyên sâu và nhận câu trả lời có cấu trúc rõ ràng,
kèm phân tích trade-off giữa các phương án, rất phù hợp cho giai đoạn nghiên cứu nguyên lý.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [ ] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế kiến trúc hệ thống
- [ ] Viết code mẫu
- [ ] Debug lỗi
- [ ] Viết test case

### Mô tả chi tiết

```text
AI hỗ trợ chính ở hai điểm:
1. Giải thích tại sao Refresh Token Rotation quan trọng (bản chất bảo mật) — thay vì tôi chỉ
   biết "nên làm vậy" mà không hiểu lý do.
2. Giải thích cơ chế phát hiện tấn công khi token bị dùng lại — giúp tôi thiết kế đúng phản
   ứng của hệ thống (cascade revoke vs single revoke).
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp học tốt hơn

```text
AI giải thích nguyên lý bảo mật theo kiểu "tại sao" chứ không chỉ "như thế nào", giúp tôi
xây dựng mental model đúng về vòng đời token và các điểm tấn công tiềm ẩn.
```

### 6.2. Những điểm AI chưa giúp tốt

```text
AI không đề cập đến các vấn đề triển khai thực tế như race condition trong concurrent refresh
hay cách tích hợp với ASP.NET Core Identity — những vấn đề này tôi phải tự giải quyết.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [x] Không phụ thuộc
- [ ] Phụ thuộc ít

### Giải thích:

```text
Tôi chỉ dùng AI để hiểu nguyên lý ban đầu. Tất cả các quyết định thiết kế và triển khai
đều do tôi tự thực hiện dựa trên hiểu biết đó.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] So sánh với yêu cầu đề bài
- [x] Tra cứu tài liệu chính thống
- [x] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu

### Mô tả quá trình kiểm chứng

```text
1. Đọc RFC 6749 và RFC 6819 để xác nhận nguyên lý AI mô tả là chính xác.
2. Đọc tài liệu Microsoft Identity Platform về Refresh Token best practices.
3. Triển khai thử nghiệm và chạy JMeter test với 100 concurrent request để kiểm tra
   race condition thực tế.
```

---

## 8. Ví dụ AI gợi ý chưa đủ / cần bổ sung

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Rotation cơ bản: dùng refresh token → cấp mới + revoke cũ. |
| Vì sao chưa đủ? | Không đề cập race condition khi nhiều request đồng thời dùng cùng token. |
| Em/nhóm phát hiện bằng cách nào? | Khi chạy JMeter test với 100 concurrent request thấy có lúc cả hai đều thành công. |
| Em/nhóm đã xử lý như thế nào? | Dùng Redis SETNX để tạo atomic lock, đảm bảo chỉ một request dùng token thành công. |
| Bài học rút ra | Nguyên lý lý thuyết và triển khai thực tế là hai việc khác nhau. AI tốt cho cái trước, tự làm mới giải quyết được cái sau. |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
Đóng góp tự thân của tôi:
1. Tự đặt câu hỏi đúng hướng để hiểu nguyên lý từ AI.
2. Tự thiết kế schema bảng RefreshTokens với TokenFamily.
3. Tự thiết kế ITokenService interface.
4. Tự triển khai toàn bộ code ASP.NET Core với Redis integration.
5. Tự phát hiện và fix race condition bằng SETNX.
6. Tự viết xUnit test cho tất cả kịch bản.
AI hỗ trợ: Chỉ giải thích nguyên lý ở 2 prompt đầu tiên.
```

---

## 10. So sánh trước và sau khi dùng AI

| Area | Before | After | Improvement |
|---|---|---|---|
| Hiểu nguyên lý Rotation | Biết khái niệm nhưng không hiểu tại sao cần | Hiểu rõ bản chất bảo mật và cơ chế phát hiện tấn công | Thiết kế được cơ chế Token Family cascade revoke |
| Thời gian nghiên cứu | Ước tính cần 1-2 ngày đọc RFC và blog | Claude tóm tắt nguyên lý trong 1 buổi | Tiết kiệm ~60% thời gian nghiên cứu cơ bản |

---

## 11. Bài học về môn học

```text
1. Bảo mật phải được thiết kế từ đầu, không phải vá sau — Refresh Token Rotation phức tạp
   hơn nhiều so với đơn giản chỉ cấp access token.
2. Hiểu nguyên lý trước khi code giúp viết code đúng ngay lần đầu, tiết kiệm thời gian debug.
3. Kiểm thử concurrent scenario (race condition) là bắt buộc cho các hệ thống token.
```

---

## 12. Bài học về sử dụng AI có trách nhiệm

```text
1. AI là nguồn học liệu bổ sung, không thay thế tài liệu gốc (RFC, official docs).
2. Hiểu nguyên lý từ AI phải được xác nhận bằng thực hành thực tế — nguyên lý đúng nhưng
   triển khai sai vẫn tạo ra lỗ hổng bảo mật.
3. Không bao giờ copy code bảo mật từ AI mà không hiểu rõ từng dòng.
```

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không dùng code bảo mật từ AI mà không hiểu từng dòng.
- [x] Không bỏ qua kiểm thử thực tế dù AI nói nguyên lý đúng.

---

## 14. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 | Khai báo đúng 2 prompt hỏi nguyên lý |
| Prompt có mục tiêu rõ ràng | 5 | Câu hỏi cụ thể, có bối cảnh hệ thống |
| Kiểm chứng kết quả AI | 5 | Đối chiếu RFC và kiểm thử thực tế |
| Tự chỉnh sửa/cải tiến | 5 | Bổ sung Token Family, xử lý race condition |
| Hiểu nội dung đã nộp | 5 | Làm chủ 100% code và thiết kế |
| Sử dụng AI có trách nhiệm | 5 | Chỉ hỏi kiến thức, không lấy code |

---

## 15. Câu hỏi tự vấn cuối bài

### 15.1. Nếu giảng viên hỏi về phần AI hỗ trợ, em có giải thích được không?

```text
Hoàn toàn được. AI chỉ giúp tôi hiểu nguyên lý Refresh Token Rotation và cơ chế cascade
revoke. Toàn bộ thiết kế và code là tự tôi làm.
```

### 15.2. Nếu không có AI, phần nào khó khăn nhất?

```text
Khâu hiểu bản chất bảo mật của rotation (tại sao cần, không chỉ làm thế nào) sẽ mất nhiều
thời gian đọc RFC và các threat model papers hơn.
```

### 15.3. Phần nào thể hiện rõ nhất năng lực thật sự?

```text
Phát hiện và xử lý race condition trong concurrent token refresh — đây là vấn đề thực tế mà
AI không đề cập, hoàn toàn do tôi tự phát hiện và giải quyết.
```

---

## 16. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 10/06/2026 |
