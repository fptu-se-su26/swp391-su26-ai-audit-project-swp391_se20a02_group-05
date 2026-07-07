# Prompt Log

## 1. Thông tin chung

| Thông tin              | Nội dung                                                                               |
| ---------------------- | -------------------------------------------------------------------------------------- |
| Môn học                | Software Development Project                                                           |
| Mã môn học             | SWP391                                                                                 |
| Lớp                    | SE20A02                                                                                |
| Học kỳ                 | SU26                                                                                   |
| Tên bài tập / Project | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-06-22T19:43:00.000Z                                                               |
| Ngày cập nhật gần nhất | 2026-06-22                                                                             |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày       | Công cụ AI  | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
| --: | ---------- | ----------- | -------- | -------------- | ------------- | ------------------------- | ---------- |
|   1 | 2026-06-22 | Antigravity | Redesign Bảng tin tuyển dụng thành dạng Accordion Group | xóa phần này của side bar đi / tạo group là Job Board chứa các items | Loại bỏ section Jobs cũ và khởi tạo node group `job-board-group` chứa các item con: Explore, Recommended, Saved và Applied. | Có | GitHub Commit |
|   2 | 2026-06-22 | Antigravity | Đồng bộ hóa tab của trang Jobs với Sidebar | đổi tab trong page cũng cập nhật side bar luôn | Viết lại sự kiện `onSelectionChange` của component Tabs để đẩy tham số truy vấn lên URL qua `router.push`, đồng bộ hóa highlight. | Có | GitHub Commit |
|   3 | 2026-06-22 | Antigravity | Nhấp Group Header cũng chuyển hướng & Chuyển đổi My CV thành Accordion Group | chỉnh lại ấn vào job board cũng navigate được / thêm tiếp group cho My CV | Bổ sung trường `href: '/jobs'` cho Job Board group và tái cấu trúc `candidate-cv` thành group chứa 8 item con của CV editor. | Có | GitHub Commit |
|   4 | 2026-06-22 | Antigravity | Dọn dẹp cấu trúc điều hướng (Evidence/Repositories) | xóa phần project dưới evidence đi, và chuyển repositories lên phần intelligence luôn | Chuyển Repositories lên section Intelligence, xóa bỏ item Projects cũ và xóa hoàn toàn section trống Evidence khỏi sidebar. | Có | GitHub Commit |

---

## 5. Prompt chi tiết

### Prompt số 1 (Redesign Sidebar Navigation Structure to Collapsible Groups)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Gom nhóm các mục con điều hướng Jobs cồng kềnh thành Accordion Group gọn gàng.                                                                         |
| Phần việc liên quan | Next.js Frontend (client/src/config/navigation-config.ts)                                                                                              |
| Mức độ sử dụng      | Hỗ trợ sinh mã mảng node children mới và nhập thêm các icon lucide thích hợp.                                                                          |

#### 5.1. Prompt nguyên văn

```text
xóa phần này của side bar đi (kèm ảnh chụp màn hình phần Jobs hiển thị "Recommended Jobs" và "Applications")
tạo group là Job Board chứa các items
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Thanh sidebar hiển thị một section riêng biệt tên là Jobs chứa trực tiếp hai mục Recommended Jobs và Applications.
- Nhóm muốn gom gọn lại thành một Accordion Group tên là "Job Board" và hiển thị đầy đủ 4 tab chức năng của Jobs bao gồm cả Explore và Saved.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất xóa section cũ, import các icon lucide và cấu hình `job-board-group` dạng `'group'` lồng các liên kết con tương ứng bên trong.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Thêm cấu trúc group Job Board vào `navigation-config.ts`.
```

---

### Prompt số 2 (Bi-directional State Synchronization)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Đồng bộ hóa giao diện tabs trên trang với highlight của sidebar để tránh bất đồng bộ trạng thái.                                                       |
| Phần việc liên quan | Next.js Frontend (client/src/app/jobs/page.tsx, client/src/lib/navigation-utils.ts)                                                                    |
| Mức độ sử dụng      | Hỗ trợ viết logic chuyển đổi URL trong onSelectionChange của component Tabs và logic so khớp tab động.                                                 |

#### 5.1. Prompt nguyên văn

```text
đổi tab trong page cũng cập nhật side bar luôn
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Khi nhấp chuyển đổi tab trực tiếp trên trang Jobs, component Tabs chuyển đổi dữ liệu và state hoàn hảo, tuy nhiên URL không đổi khiến cho phần highlight trên thanh sidebar bị đứng im ở mục cũ.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất cập nhật sự kiện `onSelectionChange` để đẩy query parameter `tab` lên URL thông qua router và sửa đổi logic khớp của `isActiveRoute` dựa trên `searchParams.tab`.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Tích hợp thành công cơ chế chuyển hướng URL cho component `<Tabs>` trong trang `JobsPage`.
```

---

### Prompt số 3 (Navigate Group Header & Create My CV Group Accordion)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Cho phép click tiêu đề Group Job Board cũng điều hướng và chuyển đổi My CV thành Group Accordion.                                                      |
| Phần việc liên quan | Next.js Frontend (navigation-config.ts, SidebarGroup.tsx, client/src/app/(private)/cv/page.tsx)                                                        |
| Mức độ sử dụng      | Hỗ trợ viết cấu trúc Accordion group mới cho My CV và viết hàm đẩy trạng thái URL cho các hành động biên soạn CV.                                      |

#### 5.1. Prompt nguyên văn

```text
chỉnh lại ấn vào job board cũng navigate được, mặc định là tab explore
thêm tiếp group cho My CV, mặc định là page management này, còn items là các tab trong ảnh 2 (kèm ảnh chụp màn hình trang CV Editor)
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Người dùng muốn click vào dòng chữ lớn "Job Board" của group cũng dẫn thẳng đến trang `/jobs`.
- Đồng thời muốn cấu trúc tương tự cho "My CV" để người dùng click đi thẳng vào từng mục con (Basic Info, Skills, Projects, Experience, v.v.).
```

#### 5.3. Kết quả AI trả về

```text
AI cung cấp cấu hình `href` cho group và cung cấp cấu trúc `candidate-cv-group` lồng 8 mục con của CV, đồng thời viết logic đồng bộ URL khi đổi tab/click card trong CV Management.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Áp dụng cấu hình group mới cho My CV và Job Board, cập nhật các hàm click điều hướng trong `cv/page.tsx`.
```

---

### Prompt số 4 (Clean up Evidence Section & Move Repositories)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích            | Tinh giản hóa cấu trúc thanh điều hướng sidebar bằng cách dọn dẹp các liên kết trùng lặp và trống.                                                     |
| Phần việc liên quan | Next.js Frontend (navigation-config.ts, sidebar-content.tsx, navigation-utils.ts)                                                                      |
| Mức độ sử dụng      | Hỗ trợ viết lại cấu trúc config sau dọn dẹp.                                                                                                           |

#### 5.1. Prompt nguyên văn

```text
xóa phần project dưới evidence đi, và chuyển repositories lên phần intelligence luôn
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Mục Projects dưới Evidence bị trùng lặp với mục con của My CV. Section Evidence chỉ còn duy nhất mục Repositories nên trở nên khá dư thừa. Nhóm muốn gom Repositories lên section Intelligence và xóa bỏ Evidence.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất dọn dẹp cấu hình, chuyển Repositories lên Intelligence section với tên ID mới `intelligence-repositories` và loại bỏ phần Evidence.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Xóa và cập nhật lại mảng config trong file `navigation-config.ts`.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần cung cấp hình ảnh minh họa thực tế giao diện và phân tích hành vi nghiệp vụ rõ ràng (ví dụ: làm rõ việc click nút thì thay đổi URL để tác động lên thanh điều hướng). Điều này giúp tránh việc AI chỉ chỉnh sửa giao diện thô mà bỏ qua các logic xử lý nghiệp vụ sâu.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Khi AI đề xuất giải pháp chưa đầy đủ hoặc sai hướng, ta cần đặt câu hỏi phản biện trực diện, khoanh vùng tệp tin liên quan và cung cấp thêm ảnh minh họa để AI nhanh chóng hiệu chỉnh.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt   | Số lượng | Ví dụ prompt tiêu biểu |
| ------------- | -------: | ---------------------- |
| Prompt Design |        2 | tạo group là Job Board chứa các items / thêm tiếp group cho My CV... |
| Prompt Fix    |        2 | đổi tab trong page cũng cập nhật side bar luôn / xóa phần project dưới evidence đi... |

---

## 10. Checklist chất lượng prompt

| Tiêu chí                   | Đã đạt? | Ghi chú |
| -------------------------- | :-----: | ------- |
| Prompt có mục tiêu rõ ràng |    x    |         |
| Prompt có đủ bối cảnh      |    x    |         |
| Tự kiểm tra và chỉnh sửa   |    x    |         |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết sử dụng prompt minh bạch và ghi nhận đúng đóng góp của AI.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-22    |
