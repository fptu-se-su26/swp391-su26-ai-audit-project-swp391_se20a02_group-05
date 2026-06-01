# AI Audit Log

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
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
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
Sửa lỗi cú pháp mã nguồn XML, tái cấu trúc hệ thống layout hình học và phân vùng tọa độ lưới cho sơ đồ Use Case tổng thể trên Draw.io.
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-28 |
| Công cụ AI | Gemini |
| Mục đích sử dụng | Sửa lỗi cú pháp hình học XML (Could not add object mxGeometry) và tái cấu trúc layout ma trận cho sơ đồ tổng thể gồm 83 Use Cases (chia thành 15 nhóm chức năng) của hệ thống CVerify trên Draw.io. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Tôi đang thực hiện import sơ đồ Use Case tổng thể cho hệ thống CVerify gồm 83 Use Cases chia làm 15 phân vùng chức năng vào công cụ Draw.io nhưng hệ thống liên tục crash và trả về thông báo lỗi: 'Could not add object mxGeometry'. 

Dưới đây là danh sách chi tiết 83 Use Cases (từ UC-01 đến UC-83), sơ đồ phân bổ 15 nhóm chức năng (Group 01 -> Group 15), danh sách các Actor tương ứng (Guest, User, Business User, Admin, Super Admin, Developer, AI Microservice, System) cùng bảng thống kê mối quan hệ <<include>> và <<extend>> đi kèm.

Hãy phân tích mã lỗi cú pháp XML này, sửa lại toàn bộ cấu trúc hình học, tính toán lại ma trận tọa độ X, Y tự động cho các thẻ <mxGeometry> và xuất lại cho tôi một đoạn mã nguồn XML sạch 100%, bao bọc các Use Cases gọn gàng trong các container swimlane để tôi import thành công vào Draw.io."
```

#### 4.2. Kết quả AI gợi ý

```text
AI đã phân tích và phát hiện ra lỗi thiếu/sai lệch cấu trúc thẻ hình học <mxGeometry> trong file XML gốc. AI đã phản hồi bằng cách generate lại toàn bộ một đoạn mã XML sạch 100%, tự động tính toán ma trận tọa độ X, Y, bao bọc 83 Use Cases gọn gàng vào trong 15 container swimlane trực quan giúp Draw.io kết xuất đồ họa thành công.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sử dụng 100% đoạn mã nguồn XML mới do AI cung cấp để import trực tiếp vào mục Extras -> Edit Diagram trên công cụ Draw.io.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Không cần chỉnh sửa sâu về mặt cấu trúc XML. Chỉ thực hiện căn chỉnh kéo thả lại vị trí của một số Actor hệ thống (như Admin, Super Admin, AI Microservice) trên canvas của Draw.io để các đường mũi tên liên kết quan hệ nhìn thoáng và đẹp mắt hơn.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Screenshot | Screenshot 11:07:39 PM | image.png |

#### 4.6. Nhận xét cá nhân/nhóm

```text
- Về mặt hiệu quả: Việc áp dụng AI vào quy trình sửa lỗi cấu trúc XML là một quyết định hoàn toàn đúng đắn của nhóm. AI thể hiện khả năng phân tích cú pháp thẻ lỗi hình học và tính toán ma trận tọa độ rất nhanh, giúp cứu vãn sơ đồ Use Case phức tạp của hệ thống TripGenie mà nếu làm thủ công sẽ mất rất nhiều ngày để mò lỗi cấu trúc.
- Bài học đúc kết: Tuy nhiên, nhóm nhận ra không nên phụ thuộc 100% vào AI. Do sơ đồ hệ thống có quy mô lớn, các đường mũi tên liên kết quan hệ (<<include>>, <<extend>>) sinh ra tự động ban đầu bị chồng chéo, gây rối mắt. Vai trò kiểm chứng và tinh chỉnh thủ công của thành viên trong nhóm vẫn là yếu tố quyết định để đưa ra sản phẩm sơ đồ cuối cùng đạt chuẩn trực quan và logic. Nhóm cần duy trì việc cung cấp bối cảnh kỹ thuật thật chi tiết cho AI ở các giai đoạn sau.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Design Usecase Diagram Structure |   |   |   | x | AI giúp định hình các trường dữ liệu (fields) cần thiết cho một bảng hoàn chỉnh |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI thỉnh thoảng xếp chồng các mối quan hệ mũi tên liên kết (<<include>>/<<extend>>) tại các khu vực có mật độ Use Case dày đặc, đòi hỏi người dùng phải can thiệp thủ công để kéo dãn layout trực quan. | Kiểm tra chéo với các nguồn tin thực tế và can thiệp thủ công từ Trương Văn Hiếu | Sơ đồ sau khi sửa lỗi đã đồng bộ 100% với danh sách phân rã chức năng trong tài liệu đặc tả hệ thống và kết xuất đồ họa mượt mà trên trình duyệt. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
- Kiểm thử trực tiếp bằng công cụ (Tool Integration Verification): Import file XML do AI sinh ra vào tính năng Edit Diagram của Draw.io để kiểm tra xem hệ thống còn báo lỗi 'Could not add object mxGeometry' hay không. Kết quả sơ đồ render thành công, không bị crash đồ họa.
- Rà soát thủ công (Manual Functional Review): Đối chiếu trực tiếp sơ đồ Use Case trên canvas với danh mục phân rã chức năng để đảm bảo đầy đủ số lượng Use Cases, các Actor hệ thống được định hình chính xác và luồng liên kết quan hệ <<include>>/<<extend>> không bị sai lệch logic.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Phần việc cá nhân tự thực hiện: Phát hiện lỗi cấu trúc XML khi import sơ đồ lớn, thu thập mã lỗi hình học của hệ thống. Nghiên cứu danh mục phân rã chức năng, bóc tách cấu trúc luồng của các Actor và cung cấp bối cảnh kỹ thuật chính xác cho AI. Sau khi nhận kết quả từ AI, tôi trực tiếp kiểm thử tính toàn vẹn của tệp XML và thực hiện tinh chỉnh kéo thả thủ công layout trên canvas Draw.io để tối ưu hóa tính thẩm mỹ và trực quan cho sơ đồ.
- Phần AI hỗ trợ: Tự động phân tích cú pháp thẻ đóng/mở XML, debug lỗi khởi tạo mxGeometry và tự động hóa việc tính toán ma trận tọa độ lưới để phân bổ đều các Use Cases vào các container swimlane chức năng.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
|  |  |  | Có / Không |  |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 1/6/2026 |
