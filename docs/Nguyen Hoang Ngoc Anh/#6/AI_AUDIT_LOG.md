# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 2026-06-02T01:35:15.986Z |
| Ngày hoàn thành | 2026-06-02T01:35:15.986Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
System Analysis & Design, Use Case Identification, UML Relationship Analysis, Business Analysis Documentation, Use Case Specification Writing, Package Architecture, Code Generation (.docx output), Technical Documentation
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-02 |
| Công cụ AI | Claude |
| Mục đích sử dụng | Xác định toàn bộ quan hệ include/extend/generalization cho 84UC, phân chia Packages để vẽ diagram, và viết Use Case Specification mẫu cho UC phức tạp nhất. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Hãy dựa đúng vào danh sách 21 Use Case và các Actors ở trên để thực hiện Bước 3 và Bước 4 theo chuẩn UML... BƯỚC 3: XÁC ĐỊNH MỐI QUAN HỆ... BƯỚC 4: ĐÓNG GÓI & ĐẶC TẢ USE CASE..."
```

#### 4.2. Kết quả AI gợi ý

```text
Toàn bộ bảng include/extend, bảng Package, UC Specification UC21 đầy đủ — dùng trực tiếp vào tài liệu BA.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Toàn bộ bảng include/extend, bảng Package, UC Specification UC21 đầy đủ — dùng trực tiếp vào tài liệu BA.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Giữ nguyên cấu trúc, chỉ yêu cầu không thêm UC mới để đảm bảo nhất quán với danh sách gốc.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 8:55:06 AM | image.png |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Group 5 members
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Ý tưởng | x |   |   |   |   |
| Phát triển ý tưởng |   | x |   |   |   |
| Review kết quả |   |   | x |   |   |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | 1. Context window limit: Không thể paste toàn bộ 84 UC + source code một lần → phải chia thành nhiều message (UC01–35 và UC36–84 riêng biệt). | Tự reviw |   |
| 2 | 2. UC01–35 bị thiếu lần đầu: Prompt ban đầu chỉ có UC36–84, AI phải yêu cầu bổ sung thêm → mất 1 lượt hỏi thêm. | Phân tích và bổ sung thêm |   |
| 3 | 3. Không tự sinh PlantUML code: Tài liệu BA đã có hướng dẫn vẽ diagram nhưng chưa sinh file .puml tự động → cần prompt riêng. | Tự làm thủ công |   |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
1. Đối chiếu từng UC với file tham chiếu source code (Controller, permissions-registry.json, Frontend pages) do Antigravity đã trích xuất.
2. Kiểm tra tính nhất quán Actor ↔ UC: mỗi UC phải có Actor hợp lệ thực hiện.
3. Xác nhận quan hệ include/extend logic đúng với nghiệp vụ thực tế (ví dụ: UC60 update user → bắt buộc UC64 force logout).
4. Review cấu trúc Use Case Specification UC26 theo chuẩn Cockburn UC template.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Người dùng (bạn) cung cấp: toàn bộ mô tả nghiệp vụ dự án, dữ liệu 84 UC từ source code CVerify (qua Antigravity), định hướng yêu cầu đầu ra, kiểm duyệt và phê duyệt từng bước trước khi đi tiếp.

AI (Claude) thực hiện: phân tích, phân loại, viết spec, sinh code tài liệu .docx.
AI (Antigravity) thực hiện: đọc source code và trích xuất 84 UC chi tiết kèm file tham chiếu
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Trương Văn Hiếu | DE190105 |  | Không |   |
| Đoàn Thế Lực | DE200523 |  | Không |   |
| Nguyễn La Hòa An | DE201043 |  | Không |   |
| Trần Nhất Long | DE200160  |  | Không |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 2/6/2026 |
