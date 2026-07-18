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
| Ngày hoàn thành reflection | 28/06/2026 |

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Tôi sử dụng AI 2 lần trong phase thiết kế database: lần 1 hỏi ChatGPT về trade-off JSONB vs
normalized, lần 2 hỏi Claude về cách tạo expression index cho partial JSONB path. Toàn bộ
việc viết SQL migration, cấu hình EF Core và benchmark đều tự thực hiện.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database (ở mức nguyên lý)

### Mô tả chi tiết

```text
AI cung cấp framework để đánh giá trade-off JSONB vs normalized và gợi ý hybrid approach.
Đây là loại kiến thức khó tìm nhanh vì cần so sánh nhiều chiều cùng lúc.
```

---

## 6. AI có giúp học tốt hơn không?

### Điểm giúp tốt hơn

```text
AI giải thích "khi nào dùng" thay vì chỉ "dùng như thế nào" — giúp tôi hiểu nguyên tắc
thiết kế database chứ không chỉ học cú pháp.
```

### Điểm chưa giúp tốt

```text
AI không biết data volume thực tế và query pattern cụ thể của CVerify để đưa ra gợi ý
tối ưu nhất. Cần tự benchmark với EXPLAIN ANALYZE để xác nhận quyết định đúng.
```

### Em/nhóm có bị phụ thuộc không?

- [x] Không phụ thuộc

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Tra cứu tài liệu chính thống (PostgreSQL docs về JSONB và GIN)
- [x] Kiểm tra bằng dữ liệu mẫu (EXPLAIN ANALYZE benchmark)

---

## 8. Ví dụ AI gợi ý chưa đủ

| Nội dung | Mô tả |
|---|---|
| AI gợi ý gì? | GIN index cho JSONB column. |
| Vì sao chưa đủ? | GIN toàn cột không optimize được query theo field cụ thể (type). |
| Phát hiện bằng cách nào? | EXPLAIN ANALYZE cho thấy vẫn sequential scan dù có GIN index. |
| Đã xử lý như thế nào? | Tạo expression index trên (certifications -> 'type'). |
| Bài học | Phải đo thực tế với EXPLAIN ANALYZE, không tin tưởng index strategy của AI. |

---

## 9. Phần đóng góp thật sự

```text
1. Tự phân tích bài toán và đặt câu hỏi đúng hướng.
2. Tự quyết định hybrid approach phù hợp.
3. Tự viết toàn bộ SQL migration với constraints và indexes.
4. Tự phát hiện vấn đề GIN index qua EXPLAIN ANALYZE.
5. Tự fix bằng expression index.
6. Tự cấu hình EF Core JSONB mapping.
```

---

## 10. So sánh trước và sau khi dùng AI

| Area | Before | After | Improvement |
|---|---|---|---|
| Hiểu khi nào dùng JSONB | Chỉ biết JSONB linh hoạt hơn | Hiểu rõ trade-off và khi nào hybrid là tốt nhất | Thiết kế schema đúng từ đầu, không phải refactor |
| Index strategy | Không biết cách index JSONB | Biết phân biệt GIN vs expression index | Query performance tốt hơn nhiều |

---

## 15. Câu hỏi tự vấn

### Nếu không có AI, phần nào khó nhất?

```text
Khâu hiểu khi nào hybrid approach là đúng sẽ mất nhiều thời gian đọc blog và docs hơn vì
đây là kiến thức thiết kế cấp cao, không có trong sách giáo khoa cơ bản.
```

### Phần nào thể hiện rõ nhất năng lực thật sự?

```text
Phát hiện vấn đề GIN index qua EXPLAIN ANALYZE và tự tạo expression index — đây là kỹ năng
tối ưu database thực tế mà AI không thể làm thay.
```

---

## 16. Cam kết Reflection

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 28/06/2026 |
