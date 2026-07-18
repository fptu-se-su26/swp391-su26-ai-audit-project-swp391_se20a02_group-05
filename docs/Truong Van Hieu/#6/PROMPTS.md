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
| Ngày cập nhật gần nhất | 2026-06-28 |

---

## 3. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài? |
|---:|---|---|---|---|---|---|
| 1 | 18/06/2026 | ChatGPT | Trade-off JSONB vs normalized PostgreSQL | "Khi nào nên dùng JSONB thay vì normalized tables? Trade-off về query, index, maintain..." | Hiểu nguyên lý, gợi ý hybrid approach | Có (Nguyên lý thiết kế) |
| 2 | 19/06/2026 | Claude | Làm rõ cách tạo GIN index cho partial path JSONB | "Cách tạo index trên một field cụ thể trong JSONB column PostgreSQL?" | Hiểu expression index với GIN | Có (Định hướng index strategy) |

---

## 5. Prompt chi tiết

### Prompt số 1

#### 5.1. Prompt nguyên văn

```text
Trong PostgreSQL, khi nào nên dùng cột JSONB thay vì thiết kế bảng chuẩn hóa (normalized
tables)? Hãy giải thích trade-off về: hiệu năng query, khả năng index, tính linh hoạt
schema và khó khăn khi maintain. Bài toán của tôi là lưu thông tin pháp lý doanh nghiệp
(giấy phép, chứng chỉ) có cấu trúc thay đổi tùy loại doanh nghiệp — mỗi loại có thể có
số lượng và loại giấy phép khác nhau.
```

#### 5.2. Kết quả AI trả về

AI giải thích trade-off và đề xuất hybrid approach: dùng columns chuẩn hóa cho thông tin
cốt lõi ổn định, JSONB cho metadata linh hoạt. Đề cập đến GIN index cho JSONB nhưng không
chi tiết về partial path.

#### 5.3. Đã áp dụng như thế nào

Thiết kế schema hybrid: business_profiles table với cột normalized (tax_code, company_name,
registration_number) + cột certifications JSONB.

---

### Prompt số 2

#### 5.1. Prompt nguyên văn

```text
Tôi có cột JSONB tên certifications trong PostgreSQL lưu array như:
[{"type": "ISO9001", "issuedAt": "2025-01-01"}, ...].
Làm thế nào để tạo index hiệu quả để query theo field "type"? GIN index thông thường
có đủ không hay cần expression index?
```

#### 5.2. Kết quả AI trả về

Claude giải thích sự khác biệt: GIN index trên toàn cột giúp containment query (@>),
nhưng để query theo path cụ thể cần expression index: `CREATE INDEX ON table USING GIN
((column -> 'type'))`. Có thể kết hợp cả hai tùy use case.

#### 5.3. Đã áp dụng như thế nào

Tạo thêm expression index trên `(certifications -> 'type')` song song với GIN index toàn cột.

---

## 6. Bài học về cách viết prompt

```text
1. Mô tả rõ bài toán cụ thể (cấu trúc data thay đổi theo loại doanh nghiệp) thay vì hỏi chung.
2. Khi cần hiểu sâu hơn một điểm cụ thể (GIN index cho partial path), hỏi prompt riêng
   với ví dụ cụ thể về data structure.
3. Prompt có ví dụ cụ thể cho kết quả chính xác hơn prompt trừu tượng.
```

---

## 9. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 28/06/2026 |
