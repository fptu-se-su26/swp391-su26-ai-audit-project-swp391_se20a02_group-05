# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-06-22T19:43:00.000Z                                                               |
| Ngày hoàn thành       | 2026-06-22T20:13:00.000Z                                                               |

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
Mục tiêu là cải tiến cấu trúc thanh điều hướng sidebar bằng cách gom nhóm các liên kết có liên quan thành các Group thu gọn (Accordion) cho Job Board và My CV, đồng thời tối ưu hóa luồng tương tác đồng bộ hai chiều (bi-directional synchronization) thông qua các tham số truy vấn URL (?tab=...) giúp đồng bộ hóa trực quan giữa giao diện tabs trên trang và các mục highlight trên thanh điều hướng sidebar. Gom gọn cấu trúc sidebar bằng cách chuyển Repositories lên section Intelligence và loại bỏ hoàn toàn section Evidence đã trống.
```

---

## 4. Nhật ký sử dụng AI chi tiết

### Lần sử dụng AI số 1 (Redesign Sidebar Navigation Structure to Collapsible Groups)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Chuyển đổi "Job Board" và "My CV" thành các Accordion Group thu gọn chứa các item con chi tiết.                                                        |
| Phần việc liên quan | Next.js Frontend (navigation-config.ts, navigation.types.ts)                                                                                           |
| Mức độ sử dụng      | Hỗ trợ tái cấu trúc mảng định nghĩa node navigationConfig từ dạng 'item' sang dạng 'group', cấu hình các link con bên trong.                           |

#### 4.1. Prompt đã sử dụng

```text
xóa phần này của side bar đi (kèm ảnh chụp màn hình phần Jobs hiển thị "Recommended Jobs" và "Applications")
tạo group là Job Board chứa các items
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất dọn dẹp các mục cũ và định nghĩa lại phần Job Board thành một node có type: 'group' với icon là Briefcase, chứa các node children con là Explore, Recommended, Saved và Applied. Đồng thời, import thêm các icon liên quan từ thư viện lucide-react.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Đoạn mã cấu hình của node 'job-board-group' trong file navigation-config.ts.
- Các icon tương ứng của từng liên kết con.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Thiết lập thuộc tính `exactMatch: true` cho item Explore để tránh việc sáng cả tab khi đang ở các tab con.
- Đảm bảo tính tương thích kiểu dữ liệu (NavigationNode) theo quy chuẩn của dự án.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(navigation): redesign sidebar navigation with collapsible Job Board and My CV groups | https://github.com/Kaivian/CVerify/commit/aea4e82b9c397c731860878cb2ae847f87cf2805 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Sự thay đổi này giúp thanh điều hướng sidebar gọn gàng hơn nhiều, người dùng dễ dàng truy cập nhanh các mục con của Bảng tin tuyển dụng thay vì cấu trúc cồng kềnh trước đây.
```

---

### Lần sử dụng AI số 2 (Bi-directional State Synchronization & Active Route Update)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Đồng bộ hóa việc click đổi tab trên trang Jobs với highlight của sidebar và ngược lại.                                                                 |
| Phần việc liên quan | Next.js Frontend (client/src/app/jobs/page.tsx, client/src/lib/navigation-utils.ts)                                                                    |
| Mức độ sử dụng      | Hỗ trợ sinh mã cập nhật sự kiện đổi tab của component Tabs và logic khớp route con trong isActiveRoute.                                                 |

#### 4.1. Prompt đã sử dụng

```text
đổi tab trong page cũng cập nhật side bar luôn
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất cập nhật sự kiện `onSelectionChange` của component `<Tabs>` trong trang Jobs để thực hiện `router.push('/jobs?tab=' + tab)` mỗi khi người dùng nhấp đổi tab trên trang, đồng thời bổ sung logic so khớp tab trong isActiveRoute của navigation-utils.ts.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Hàm cập nhật URL trong onSelectionChange của Tabs.
- Cấu trúc khớp route theo tab trong isActiveRoute.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Bổ sung khối lệnh `else { setActiveTab("explore"); }` trong useEffect đồng bộ tabParam ở JobsPage để khi URL chuyển về `/jobs` (không có query param), giao diện sẽ tự động reset về tab Explore một cách mượt mà.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(navigation): redesign sidebar navigation with collapsible Job Board and My CV groups | https://github.com/Kaivian/CVerify/commit/aea4e82b9c397c731860878cb2ae847f87cf2805 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Đồng bộ hóa URL giúp trải nghiệm người dùng tăng cao, cho phép lưu trữ trạng thái trang (bookmark) và chia sẻ liên kết trực tiếp đến từng tab cụ thể trên bảng tin tuyển dụng.
```

---

### Lần sử dụng AI số 3 (Navigate Group Header & Create My CV Group)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Cho phép click tiêu đề Group Job Board cũng điều hướng và chuyển đổi My CV thành Group Accordion tương tự.                                              |
| Phần việc liên quan | Next.js Frontend (navigation-config.ts, SidebarGroup.tsx, client/src/app/(private)/cv/page.tsx)                                                        |
| Mức độ sử dụng      | Hỗ trợ sinh cấu trúc group con cho My CV, viết các sự kiện đồng bộ chuyển tiếp URL cho các nút form editor.                                            |

#### 4.1. Prompt đã sử dụng

```text
chỉnh lại ấn vào job board cũng navigate được, mặc định là tab explore
thêm tiếp group cho My CV, mặc định là page management này, còn items là các tab trong ảnh 2 (kèm ảnh chụp màn hình trang CV Editor có các section ở thanh bên trái)
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất bổ sung trường `href: '/jobs'` trực tiếp cho node `job-board-group` trong file config để SidebarGroup tự động điều hướng khi click header. Với My CV, AI tạo node group `candidate-cv-group` chứa các item con: Overview, Basic Info, Skills, Projects, Experience, Education, Achievements, Preferences.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Đoạn mã cấu hình `candidate-cv-group` và các icon lucide đi kèm.
- Logic đồng bộ URL cho trang CV.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Nhóm đã chỉnh sửa logic sự kiện click nút quay lại Overview (`Back to Overview`) trong editor để thực hiện `router.push('/cv')`, làm sạch query parameter nhằm đưa sidebar trở lại highlight mục Overview.
- Nhóm đã tối ưu hóa component SidebarLink để gỡ bỏ logic so khớp tab tĩnh cũ và ủy quyền hoàn toàn cho isActiveRoute.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(navigation): redesign sidebar navigation with collapsible Job Board and My CV groups | https://github.com/Kaivian/CVerify/commit/aea4e82b9c397c731860878cb2ae847f87cf2805 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc phân rã các tiểu mục của CV ra sidebar giúp giảm tải độ phức tạp của giao diện quản lý CV, tăng tính điều hướng trực quan khi người dùng muốn đi thẳng vào sửa một phần hồ sơ cụ thể.
```

---

### Lần sử dụng AI số 4 (Clean up Evidence Section & Move Repositories to Intelligence)

| Nội dung            | Thông tin                                                                                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Ngày sử dụng        | 2026-06-22                                                                                                                                             |
| Công cụ AI          | Antigravity                                                                                                                                            |
| Mục đích sử dụng    | Loại bỏ section Evidence đã trống và di chuyển Repositories lên section Intelligence.                                                                  |
| Phần việc liên quan | Next.js Frontend (navigation-config.ts, sidebar-content.tsx, navigation-utils.ts)                                                                      |
| Mức độ sử dụng      | Hỗ trợ loại bỏ mã định nghĩa Evidence section cũ và dọn dẹp các logic lọc liên quan trong sidebar-content.                                             |

#### 4.1. Prompt đã sử dụng

```text
xóa phần project dưới evidence đi, và chuyển repositories lên phần intelligence luôn
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất chuyển item `Repositories` sang phần cuối của mảng con `children` của `intelligence-section` với ID mới là `intelligence-repositories` và loại bỏ hoàn toàn `evidence-section` cùng item `Projects` khỏi config.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Đoạn mã cập nhật của mảng cấu hình trong navigation-config.ts.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Tiến hành dọn dẹp logic lọc section của `sidebar-content.tsx` (gỡ bỏ kiểm tra `node.id !== "evidence-section"`).
- Cập nhật hàm so khớp đường dẫn active trong `navigation-utils.ts` sang ID mới `intelligence-repositories` và xóa bỏ case khớp cho `evidence-projects`.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(navigation): redesign sidebar navigation with collapsible Job Board and My CV groups | https://github.com/Kaivian/CVerify/commit/aea4e82b9c397c731860878cb2ae847f87cf2805 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Việc tối giản hóa các phần của sidebar giúp giao diện thân thiện hơn, loại bỏ các mục trùng lặp như Projects (nay đã nằm gọn trong My CV).
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                                                     |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------------------------------- |
| Phân tích yêu cầu           |               |      x       |                 |               | Xác định cấu trúc sidebar hợp lý và cơ chế hoạt động cho Accordion Groups.                 |
| Viết user story/use case    |       x       |              |                 |               |                                                                                             |
| Thiết kế database           |       x       |              |                 |               |                                                                                             |
| Thiết kế kiến trúc hệ thống |               |      x       |                 |               | Tái thiết kế luồng đồng bộ trạng thái URL query parameter hai chiều cho các trang tabs.     |
| Thiết kế giao diện          |               |      x       |                 |               | Gom nhóm trực quan các biểu tượng và liên kết điều hướng.                                    |
| Code frontend               |               |              |        x        |               | Tạo cấu trúc Accordion Groups, viết các handler đồng bộ đổi tab với URL.                     |
| Code backend                |       x       |              |                 |               | Không có thay đổi backend trong đợt này.                                                     |
| Debug lỗi                   |               |      x       |                 |               | Khắc phục lỗi so khớp route active của các mục con thông qua hàm `isActiveRoute`.           |
| Viết test case              |       x       |              |                 |               |                                                                                             |
| Kiểm thử sản phẩm           |               |      x       |                 |               | Chạy kiểm tra type-checking cục bộ và điều hướng thủ công trên trình duyệt.                 |
| Tối ưu code                 |               |      x       |                 |               | Ủy quyền toàn bộ kiểm tra active route từ SidebarLink về hàm dùng chung `isActiveRoute`.     |
| Viết báo cáo                |       x       |              |                 |               |                                                                                             |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                                             |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | Khi đổi tab trên trang Jobs, AI chỉ đề xuất đổi ở sự kiện chọn tab của component mà chưa đồng bộ đầy đủ các sự kiện click nút bấm hoặc reset form tìm kiếm khác về đúng tab `explore`. | Kiểm tra hành động tìm kiếm thấy URL không tự động cập nhật về `?tab=explore` khi đang ở tab khác làm lệch highlight sidebar. | Tự bổ sung lệnh điều hướng `router.push("/jobs?tab=explore")` vào trong hàm `handleSearchSubmit` khi chuyển tab sang explore. |
|   2 | AI đề xuất thay đổi cấu trúc `candidate-cv` thành group chứa các tab con nhưng chưa cập nhật hàm reset `setViewState("overview")` khi click nút quay lại, khiến người dùng bị kẹt ở chế độ edit trên thanh sidebar. | Bấm nút quay lại (Back arrow) trên trang CV thấy URL vẫn giữ `?tab=basic-info` và mục sidebar tương ứng vẫn sáng dù giao diện đã về overview. | Bổ sung `router.push("/cv")` vào sự kiện `onClick` của nút quay lại trong file `cv/page.tsx`. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả qua các hình thức sau:
1. Chạy thành công ứng dụng frontend và quan sát sidebar: Các mục "Job Board" và "My CV" được hiển thị dưới dạng nhóm Accordion có nút mũi tên thu gọn mượt mà.
2. Click chọn "Job Board" hoặc "Explore", trang chuyển hướng thành công đến `/jobs` và highlight mục con Explore. Đổi sang tab "Recommended" trên trang, URL đổi thành `/jobs?tab=recommended` và mục con Recommended trên sidebar tự động sáng lên.
3. Click "My CV", trang hiển thị đúng giao diện CV Management dạng Overview. Bấm chọn card "Basic Information", URL chuyển thành `/cv?tab=basic-info` và thanh sidebar tự động mở rộng nhóm My CV đồng thời highlight mục Basic Information tương ứng.
4. Chạy trình typecheck `npx tsc --noEmit` hoàn tất thành công mà không phát sinh lỗi kiểu dữ liệu.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Cải tiến logic khớp route trong `navigation-utils.ts` để tối ưu hóa khả năng so khớp tab động thông qua search parameters.
- Trực tiếp bổ sung các hàm xử lý đồng bộ URL khi người dùng thực hiện các hành động nút bấm đặc thù (Reset Filters, Back to Overview) để đảm bảo trạng thái sidebar luôn chính xác.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                                                 | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ---------------------------------------------------------------------------------------------- | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Triển khai cấu hình Accordion groups, sửa đổi logic so khớp route active, đồng bộ URL, tsc.     | Có                   | GitHub Commits      |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm tra tính nhất quán giao diện và kiểm thử luồng điều hướng của hai trang Jobs và CV.        | Có                   | GitHub Commits      |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI hỗ trợ đắc lực trong việc sinh nhanh các khối định nghĩa cấu trúc dữ liệu group mới cho sidebar, đề xuất các icon phù hợp và giúp đẩy nhanh tốc độ viết logic chuyển tiếp URL bằng next/navigation.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?
```text
Nhóm không phụ thuộc hoàn toàn vào cơ chế cập nhật state cô lập của AI đề xuất cho Tabs mà chủ động tích hợp thêm các logic đồng bộ router trên toàn bộ các nút hành động bổ trợ khác để tránh tình trạng bất đồng bộ trạng thái giữa URL và thanh sidebar.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Chạy thử nghiệm Next.js trực tiếp trên trình duyệt, tương tác với mọi mục trên sidebar và click mọi tab trên trang Jobs & CV để kiểm chứng sự thay đổi đồng nhất, đồng thời chạy trình biên dịch kiểm tra kiểu TypeScript để đảm bảo chất lượng code sạch.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Việc thiết lập và viết thủ công toàn bộ cấu trúc Accordion lồng ghép các item con cho mảng định nghĩa node của My CV và Job Board có thể tốn khá nhiều thời gian viết mã lặp đi lặp lại.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Hiểu rõ hơn về cơ chế vận hành của hệ thống định tuyến (Routing) trong Next.js App Router, tầm quan trọng của việc lấy URL làm nguồn chân lý duy nhất (single source of truth) cho trạng thái ứng dụng nhằm duy trì tính nhất quán của giao diện điều hướng.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Khi hướng dẫn AI viết mã điều hướng trang, việc kiểm soát chặt chẽ toàn bộ các luồng rẽ nhánh phụ (như reset bộ lọc hay nút quay lại) là nhiệm vụ bắt buộc của lập trình viên để tránh các lỗi logic tiềm ẩn mà AI thường bỏ sót.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-22    |
