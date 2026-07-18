# AI Learning Reflection

## 1. Thông tin chung

| Tên sinh viên / Nhóm | Trương Văn Hiếu / Nhóm SE20A02 - Group 05 |
|---|---|
| MSSV | DE190105 |
| Ngày reflection | 08/07/2026 |

---

## 3. Tóm tắt

```text
Dùng Claude 1 lần để xác nhận kiến thức về Clean Architecture layers và Dependency Rule.
Tôi đã tự đọc sách trước, dùng AI để xác nhận nhanh 2-3 điểm không chắc thay vì dùng AI
để học từ đầu. Toàn bộ thiết kế và refactor cấu trúc code do tôi tự thực hiện.
```

---

## 5. AI hỗ trợ ở điểm nào?

- [x] Hiểu yêu cầu đề bài (xác nhận hiểu biết)

```text
AI xác nhận nhanh chóng rằng module-based organization tương thích với Clean Architecture —
điểm tôi không chắc sau khi đọc sách.
```

---

## 6. Điểm giúp và không giúp

### Giúp tốt

```text
Trả lời câu hỏi "có tương thích không?" nhanh hơn nhiều so với tự tìm câu trả lời từ sách.
Giúp tôi tự tin tiếp tục thay vì nghi ngờ kiến trúc.
```

### Không giúp tốt

```text
AI có xu hướng đề xuất Generic Repository — đây là pattern phổ biến nhưng không phải lúc
nào cũng tốt. Cần tự đánh giá trade-off.
```

### Phụ thuộc AI?

- [x] Không phụ thuộc

---

## 8. Ví dụ AI gợi ý không phù hợp

| Nội dung | Mô tả |
|---|---|
| AI gợi ý gì? | Generic Repository Pattern (`IRepository<T>`) |
| Vì sao không phù hợp? | Che giấu EF Core capabilities (Include, GroupBy...), tạo abstraction không cần thiết |
| Tự quyết định như thế nào? | Dùng Specific Repository hoặc trực tiếp IDbContext ở Application layer |
| Bài học | AI đề xuất pattern phổ biến nhưng không phải lúc nào cũng đúng cho bài toán cụ thể |

---

## 9. Đóng góp thật sự

```text
1. Tự học Clean Architecture từ sách Robert C. Martin trước.
2. Tự thiết kế cấu trúc module-based cụ thể.
3. Tự quyết định không dùng Generic Repository.
4. Tự viết ArchitectureTests để enforce Dependency Rule.
5. Tự refactor toàn bộ cấu trúc CVerify.Core.
```

---

## 15. Câu hỏi tự vấn

### Cách dùng AI hiệu quả nhất trong phase này?

```text
Dùng AI để xác nhận kiến thức ("kiểu này có đúng không?") sau khi đã tự học — hiệu quả
hơn dùng AI để học từ đầu. Kết quả xác nhận giúp tôi tự tin hơn và tiết kiệm thời gian.
```

---

## 16. Cam kết

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 08/07/2026 |
