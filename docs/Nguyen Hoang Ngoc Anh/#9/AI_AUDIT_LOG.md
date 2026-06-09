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
| Ngày bắt đầu | 2026-06-09T01:05:00Z |
| Ngày hoàn thành | 2026-06-09T01:45:00Z |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
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
Tối ưu hóa giao diện và trải nghiệm người dùng (UI/UX) cho mục Career Preferences trong CareerTab: sắp xếp layout dạng lưới hai cột cho các thẻ AI insights, chuẩn hóa thứ tự các thẻ sở thích, định cấu hình động cho thông tin tiền tệ (VND/USD), chuẩn hóa kích thước nút thêm tag và tùy biến giao diện thanh UnsavedChangesBar thay cho việc mở cửa sổ modal xác nhận.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-06-09 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Sắp xếp lại thứ tự các thẻ preferences, định cấu hình layout responsive 2 cột cho Discoverability và AI-Inferred Career Trajectory, tích hợp InputGroup hỗ trợ tiền tệ động VND/USD (prefix, suffix, placeholder và label), thiết lập logic nút Add custom option và xử lý trạng thái bị vô hiệu hóa (disabled). |
| Phần việc liên quan | Coding Frontend |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
"Update CareerTab.tsx:
1. Place Discoverability and AI-Inferred Career Trajectory side-by-side inside a responsive grid layout. Enforce ordering of the remaining preferences cards.
2. Replace Input with InputGroup for Expected Salary Min/Max fields, dynamically updating label, prefix ($ or ₫), suffix (USD or VND), and placeholder depending on expectedSalaryCurrency.
3. Standardize 'Add' buttons size to 'md' and layout max-w-sm. Apply bg color overrides (white border-border when empty, bg-accent when text is present) and ensure they remain white when disabled using data-[disabled=true] selector.
4. Keep UnsavedChangesBar at the bottom, removing ConfirmationModal. Validation must run in handleSaveChanges before calling save API; reset must restore cached backend preferences."
```

#### 4.2. Kết quả AI gợi ý

```text
- Viết mã JSX điều chỉnh layout trong CareerTab.tsx sử dụng `grid grid-cols-1 md:grid-cols-2 gap-6` cho 2 thẻ AI.
- Cập nhật JSX trường lương bằng cách bọc thẻ InputGroup.Prefix và InputGroup.Suffix xung quanh InputGroup.Input. Sử dụng các biến currencyCode, currencySymbol, salaryPlaceholder để gán động.
- Áp dụng logic className có điều kiện cho các nút Add dựa trên độ dài chuỗi đầu vào (newLocation.trim() hay newSkill.trim()), thêm các thuộc tính data-[disabled=true] để đè thuộc tính mặc định của HeroUI.
- Khôi phục component UnsavedChangesBar, loại bỏ các state của ConfirmationModal, điều chỉnh handleSaveChanges chạy `await methods.trigger()` trước khi xử lý gọi API lưu dữ liệu.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Áp dụng cấu trúc lưới 2 cột và thứ tự các thẻ preferences trong CareerTab.tsx.
- Cấu trúc InputGroup cùng các thuộc tính động cho VND/USD.
- Class định dạng có điều kiện cho các nút Add tại CareerTab.tsx và TagChipMultiSelect.tsx.
- Xử lý lưu và reset kết hợp thanh trạng thái thay đổi chưa lưu (UnsavedChangesBar).
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Điều chỉnh lại các khoảng cách padding và margin trên giao diện di động để đảm bảo các trường nhập liệu không bị lệch hàng khi thay đổi giữa các đồng tiền VND và USD.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Source Code | CareerTab.tsx & TagChipMultiSelect.tsx | Tái cấu trúc layout và đồng bộ cơ chế thay đổi |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI đề xuất giải pháp viết class Tailwind đè HeroUI disabled state rất khéo léo, giao diện mượt mà và trực quan hơn hẳn phiên bản cũ.
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
| 1 | Mặc dù đã thêm class `disabled:bg-white` nhưng lớp nền của nút Add khi bị disabled vẫn bị HeroUI v3 ghi đè thành màu xám mặc định. | Kiểm tra trực quan trên giao diện | Thêm selector `data-[disabled=true]:bg-white` và các class tương tự vào className để đè hoàn toàn CSS của thư viện HeroUI. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
1. Chạy lệnh kiểm tra TypeScript `npx tsc --noEmit` thành công mà không phát sinh lỗi kiểu dữ liệu.
2. Kiểm tra giao diện responsive: Discoverability và AI-Inferred Career Trajectory hiển thị song song trên desktop, xếp chồng trên mobile.
3. Thay đổi Expected Salary Currency cập nhật lập tức label, symbol, placeholder và suffix động (USD -> $, VND -> ₫).
4. Nhập chuỗi vào ô Custom Option chuyển màu nút Add từ trắng sang nâu. Xóa trắng chuyển nút Add về màu trắng.
5. Sửa đổi form kích hoạt UnsavedChangesBar hiển thị ở góc dưới màn hình. Click Reset khôi phục đúng dữ liệu ban đầu, click Save changes lưu thành công và ẩn thanh UnsavedChangesBar đi.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Người dùng đóng góp: Đưa ra các yêu cầu tinh chỉnh chi tiết về UI/UX, hướng dẫn giữ lại UnsavedChangesBar thay vì ConfirmationModal, chạy các câu lệnh biên dịch và kiểm tra trạng thái Git.

AI thực hiện: Viết mã nguồn JSX tái cấu trúc lưới, sinh class điều kiện disabled cho các nút Add, xây dựng component InputGroup linh hoạt tiền tệ và dọn dẹp các modal không dùng tới.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Phát triển Frontend Career Preferences | Có | Commit 4595aeecd937c44a7290475f1d776e878fbbe780 |
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
| Nguyễn Hoàng Ngọc Ánh | 09/06/2026 |
