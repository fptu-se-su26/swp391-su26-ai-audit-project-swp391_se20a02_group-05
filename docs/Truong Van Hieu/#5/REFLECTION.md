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
| Ngày hoàn thành reflection | 20/06/2026 |

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Tôi chỉ sử dụng ChatGPT 1 lần duy nhất để so sánh ba công nghệ realtime (WebSocket, SSE,
SignalR) nhằm ra quyết định lựa chọn công nghệ. Toàn bộ thiết kế kiến trúc Hub, triển khai
code, xử lý JWT auth và Redis Backplane đều do tôi tự thực hiện.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế kiến trúc hệ thống
- [ ] Viết code mẫu

### Mô tả chi tiết

```text
AI cung cấp bảng so sánh có cấu trúc giữa ba công nghệ theo các tiêu chí rõ ràng, giúp tôi
ra quyết định chọn SignalR mà không mất nhiều thời gian đọc docs từng công nghệ từ đầu.
```

---

## 6. AI có giúp học tốt hơn không?

### 6.1. Những điểm AI giúp tốt hơn

```text
AI giải thích tại sao SignalR phù hợp hơn SSE cho kịch bản Group broadcast, giúp tôi hiểu
nguyên lý trade-off thay vì chỉ "nghe nói SignalR tốt".
```

### 6.2. Những điểm AI chưa giúp tốt

```text
AI không biết chi tiết về các vấn đề thực tế như: JWT auth trong Hub khác với controller
thế nào, hay khi deploy nhiều instance cần Redis Backplane. Những điều này tôi phải tự học
từ docs Microsoft và StackOverflow.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [x] Không phụ thuộc

```text
Chỉ dùng AI 1 lần cho quyết định ban đầu. Toàn bộ implementation và debug hoàn toàn tự làm.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Tra cứu tài liệu chính thống
- [x] Kiểm tra bằng dữ liệu mẫu

```text
Đọc tài liệu Microsoft chính thức và triển khai proof-of-concept để xác nhận SignalR đáp ứng
được yêu cầu trước khi áp dụng vào codebase thực tế.
```

---

## 8. Ví dụ AI gợi ý chưa đủ

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | SignalR phù hợp cho .NET, có Group, fallback tự động. |
| Vì sao chưa đủ? | Không đề cập Redis Backplane cho horizontal scale. |
| Em phát hiện bằng cách nào? | Khi deploy 2 instance thấy client chỉ nhận được broadcast từ đúng instance xử lý request. |
| Em đã xử lý như thế nào? | Thêm AddStackExchangeRedis() cho SignalR config. |
| Bài học | Lý thuyết về SignalR không bao gồm kiến trúc production. Phải tự nghiên cứu thêm. |

---

## 9. Phần đóng góp thật sự

```text
1. Tự quyết định kiến trúc Hub sau khi hiểu trade-off từ AI.
2. Tự thiết kế IAdminClient interface và AdminHub.
3. Tự giải quyết JWT auth trong SignalR context.
4. Tự phát hiện và fix Redis Backplane cho multi-instance.
5. Tự triển khai client-side SignalR trong Next.js.
```

---

## 10. So sánh trước và sau khi dùng AI

| Area | Before | After | Improvement |
|---|---|---|---|
| Chọn công nghệ | Không biết SSE/WebSocket/SignalR khác nhau thế nào | Hiểu rõ trade-off, chọn đúng SignalR | Tiết kiệm ~3 giờ đọc docs từng công nghệ |
| Hiểu Group concept | Không biết SignalR có sẵn Group | Hiểu và áp dụng Group "admins" | Design đơn giản hơn, không cần tự implement |

---

## 15. Câu hỏi tự vấn cuối bài

### Nếu không có AI, phần nào khó khăn nhất?

```text
Khâu so sánh và chọn công nghệ sẽ mất nhiều thời gian hơn vì cần đọc docs của cả ba công
nghệ rồi mới ra quyết định. AI rút ngắn quá trình này đáng kể.
```

### Phần nào thể hiện rõ nhất năng lực thật sự?

```text
Phát hiện và fix vấn đề Redis Backplane khi multi-instance, và thiết kế JWT auth middleware
cho SignalR Hub — đây là các vấn đề AI không đề cập, hoàn toàn do tôi tự xử lý.
```

---

## 16. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 20/06/2026 |
