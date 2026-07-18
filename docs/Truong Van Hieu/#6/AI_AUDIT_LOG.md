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
| Ngày bắt đầu | 2026-06-18 |
| Ngày hoàn thành | 2026-06-28 |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor

---

## 3. Mục tiêu sử dụng AI

```text
Mục tiêu là tìm hiểu khi nào nên dùng cột JSONB trong PostgreSQL thay vì thiết kế bảng
chuẩn hóa (normalized tables), để áp dụng cho việc lưu trữ thông tin pháp lý linh hoạt
của Business User trên CVerify. Không dùng AI để viết SQL hay code — chỉ hỏi nguyên lý
thiết kế database.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 18/06/2026 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Hiểu khi nào dùng JSONB vs normalized tables trong PostgreSQL |
| Phần việc liên quan | Thiết kế database schema |
| Mức độ sử dụng | Hỏi kiến thức |

#### 4.1. Prompt đã sử dụng

```text
Trong PostgreSQL, khi nào nên dùng cột JSONB thay vì thiết kế bảng chuẩn hóa (normalized
tables)? Hãy giải thích trade-off về: hiệu năng query, khả năng index, tính linh hoạt
schema và khó khăn khi maintain. Bài toán của tôi là lưu thông tin pháp lý doanh nghiệp
(giấy phép, chứng chỉ) có cấu trúc thay đổi tùy loại doanh nghiệp — mỗi loại có thể có
số lượng và loại giấy phép khác nhau.
```

#### 4.2. Kết quả AI gợi ý

AI giải thích: JSONB phù hợp khi schema không cố định (semi-structured data), cần lưu
metadata linh hoạt. Normalized tables phù hợp khi cần JOIN, aggregate, index cụ thể và
query phức tạp. PostgreSQL 15+ hỗ trợ index trên JSONB với GIN index nhưng vẫn chậm hơn
B-tree trên column thông thường. AI gợi ý hybrid approach: cột chuẩn hóa cho thông tin cốt
lõi (mã số thuế, tên doanh nghiệp), JSONB cho metadata linh hoạt (danh sách giấy phép đặc
thù theo ngành).

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Tiếp thu nguyên lý hybrid approach để thiết kế schema: tách bảng BusinessProfiles với các
cột chuẩn hóa cho thông tin cốt lõi, thêm cột Certifications JSONB cho danh sách chứng chỉ
linh hoạt. Không dùng SQL từ AI.

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Tôi bổ sung thêm GIN index trên cột JSONB cho trường type (loại chứng chỉ) vì đây là trường
query thường xuyên. AI đề cập GIN index nhưng không nói rõ cách áp dụng cho partial path.
Ngoài ra tôi thiết kế migration riêng để validate JSONB schema bằng CHECK constraint.

#### 4.5. Nhận xét cá nhân/nhóm

```text
- Về hiệu quả: ChatGPT giải thích rõ ràng với ví dụ cụ thể, giúp tôi hiểu bản chất trade-off
  thay vì chỉ biết "JSONB linh hoạt hơn".
- Bài học: Hybrid approach là lựa chọn thực tế tốt nhất — cả JSONB thuần và normalized thuần
  đều có nhược điểm riêng trong bài toán này.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Thiết kế database |  | [x] |  |  | Tham khảo nguyên lý JSONB vs normalized |
| Viết SQL migration | [x] |  |  |  | Tự viết SQL 100% |
| Code backend | [x] |  |  |  | Tự code Entity Framework config |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI không đề cập cách dùng GIN index trên partial path JSONB (chỉ JSONB cả cột). | Phát hiện khi query theo type chứng chỉ vẫn chậm dù đã có GIN index. | Tạo expression index: CREATE INDEX ON business_profiles USING GIN ((certifications -> 'type')). |
| 2 | AI không đề cập cách validate JSONB schema ở DB level. | Phát hiện khi có dữ liệu không nhất quán trong cột certifications. | Thêm CHECK constraint validate cấu trúc JSONB bắt buộc có field 'type' và 'issuedAt'. |

---

## 7. Kiểm chứng kết quả AI

```text
1. Đọc tài liệu PostgreSQL về JSONB và GIN index.
2. Benchmark query performance với EXPLAIN ANALYZE so sánh có/không có GIN index.
3. Thảo luận với thành viên backend để đảm bảo schema đáp ứng yêu cầu query thực tế.
```

---

## 8. Đóng góp cá nhân

```text
- Tự đặt câu hỏi đúng hướng để hiểu trade-off JSONB vs normalized.
- Tự quyết định hybrid approach phù hợp với bài toán CVerify.
- Tự viết SQL migration với GIN index và CHECK constraint.
- Tự cấu hình Entity Framework để mapping cột JSONB.
- Tự benchmark và tối ưu query performance.
```

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ ở điểm nào?

```text
AI cung cấp framework tư duy để đánh giá trade-off JSONB vs normalized, giúp tôi ra quyết
định thiết kế schema đúng hướng ngay từ đầu.
```

### 9.2. Học được gì về môn học?

```text
Thiết kế database tốt phải cân bằng giữa tính linh hoạt và hiệu năng query. Không có giải
pháp "tốt nhất" — chỉ có giải pháp phù hợp nhất với bài toán cụ thể.
```

### 9.3. Học được gì về sử dụng AI có trách nhiệm?

```text
AI đưa ra nguyên lý chung nhưng không biết chi tiết constraint của bài toán cụ thể. Cần tự
benchmark và validate thực tế.
```

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Trương Văn Hiếu | 28/06/2026 |
