# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Admin Components System Page                                                 |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-05-29                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong đợt phát triển tính năng trang quản trị Components System và trực quan hóa kiến trúc này, AI đóng vai trò quan trọng trong việc đề xuất các mô hình kiến trúc Workspace Abstraction và cơ chế cô lập lỗi trong Sandbox xem trước, giúp nhóm nhanh chóng vượt qua giai đoạn phác thảo và viết boilerplate thô. Tuy nhiên, nhóm vẫn đóng vai trò cốt lõi trong việc rà soát, phát hiện và sửa các lỗi cú pháp nghiêm trọng do sự khác biệt giữa các phiên bản React 19 và TypeScript, kiểm thử bảo mật route thủ công và tối ưu hóa hiệu năng render thực tế.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Gemini
```

### Lý do sử dụng công cụ đó

```text
Gemini được tích hợp trong hệ thống Antigravity hỗ trợ đọc hiểu codebase React/Next.js vô cùng chính xác, có tốc độ phản hồi nhanh và khả năng đề xuất các cấu trúc phân lớp modular, clean code rất phù hợp với phong cách kiến trúc hệ thống hiện tại của CVerify.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [x] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [x] Viết test case
- [ ] Review code
- [x] Tối ưu code
- [x] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI hỗ trợ đắc lực trong việc sinh mã nguồn mẫu cho WorkspaceProvider, useWorkspace để cô lập thanh sidebar quản trị; phác thảo cấu trúc React Flow nodes/edges và layout Generator tự động tính toán tọa độ; cung cấp khung Sandbox Preview bọc trong Error Boundary cùng các mock contexts để xem trước component một cách an toàn mà không làm sụp đổ trang chính.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Tiếp cận và áp dụng mô hình Workspace Abstraction nâng cao, tách rời hoàn toàn logic điều phối sidebar để sẵn sàng nhân rộng cho các nền tảng con của quản trị viên về sau.
- Hiểu được cách triển khai hộp cát cô lập lỗi (Sandbox Isolation) bằng React Error Boundary để đảm bảo tính sẵn sàng cao (High Availability) cho các trang quản trị chứa mã nguồn động.
- Học cách sử dụng thư viện React Flow (@xyflow/react) một cách chuyên nghiệp, từ việc thiết lập node/edge tùy biến đến thuật toán sắp xếp tọa độ theo cột phả hệ Atomic Design.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đôi khi sinh ra các cú pháp lỗi thời hoặc không tương thích với phiên bản thư viện hiện tại. Ví dụ: AI đề xuất cách truy cập raw children trong Class Component theo cú pháp cũ của React 18, dẫn đến lỗi biên dịch nghiêm trọng khi chạy Next.js Turbopack trên môi trường React 19.
- AI thỉnh thoảng bỏ quên các import thư viện hoặc icon thiết yếu (như Settings icon của Lucide), khiến quá trình production build của Next.js bị crash trắng nếu sinh viên không kiểm tra thủ công.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm sử dụng AI làm trợ lý đắc lực để phác thảo boilerplate và cấu trúc thuật toán tọa độ thô cho React Flow. Các quyết định kiến trúc, tối ưu trải nghiệm (Spotlight search, arrow-key navigation), sửa lỗi cú pháp React 19/TypeScript và phân quyền bảo vệ route hoàn toàn do nhóm tự nghiên cứu, kiểm soát thông qua code review và kiểm thử thực tế.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [x] Viết test case
- [x] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Nhóm kiểm chứng bằng cách:
1. Chạy lệnh production build nghiêm ngặt `npm run build` trên thư mục client để rà soát lỗi biên dịch TypeScript, đảm bảo không có cảnh báo kiểu hay lỗi thiếu import.
2. Kiểm thử bảo mật (Granular Access Control): Sử dụng tài khoản có vai trò USER truy cập thẳng route `/admin/components`, xác nhận hệ thống chặn thành công và hiển thị giao diện "Access Revoked". Đăng nhập bằng tài khoản ADMIN có quyền `components:system:read` thì truy cập mượt mà.
3. Kiểm thử độ chịu lỗi Sandbox: Đưa vào một component cố ý ném lỗi runtime trong lúc render, kiểm tra xem `PreviewErrorBoundary` có bắt lỗi thành công và chỉ làm hỏng cục bộ thẻ card đó mà không ảnh hưởng tới hoạt động của phần còn lại trên trang.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung                           | Mô tả                                                                                                                                                                             |
| ---------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| AI đã gợi ý gì?                    | Đề xuất viết class component `PreviewErrorBoundary` có phương thức render trả về `this.children` trực tiếp để xuất nội dung con ra ngoài.                                          |
| Em/nhóm đã kiểm tra bằng cách nào? | Đọc kỹ lỗi biên dịch ném ra từ Next.js Turbopack compiler khi chạy build: `Property 'children' does not exist on type 'PreviewErrorBoundary'.`                                    |
| Kết quả kiểm tra                   | Lỗi cú pháp nghiêm trọng. Trên React 19 kết hợp TypeScript, các thuộc tính của Component (bao gồm cả `children`) bắt buộc phải được truy cập thông qua đối tượng props (`this.props.children`). |
| Em/nhóm đã xử lý tiếp như thế nào? | Thực hiện sửa đổi thủ công dòng code bị lỗi từ `this.children` thành `this.props.children` để đảm bảo tương thích tuyệt đối với React 19 và vượt qua trình biên dịch.             |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung                          | Mô tả                                                                                                                                                             |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| AI đã gợi ý gì?                   | Sử dụng các thẻ icon Lucide động bằng cách truy cập chuỗi string động thông qua import chung, ví dụ: `const IconComponent = Lucide[iconName]`.                     |
| Vì sao gợi ý đó sai/chưa phù hợp? | Cách làm này phá vỡ cơ chế Tree-Shaking của Webpack/Turbopack, buộc Next.js phải đóng gói toàn bộ thư viện Lucide khổng lồ vào bundle client, gây giảm hiệu năng nghiêm trọng và tăng dung lượng tải trang. |
| Em/nhóm phát hiện bằng cách nào?  | Đọc code review ban đầu và kiểm chứng dung lượng tệp tin JS sau khi build thử nghiệm.                                                                             |
| Em/nhóm đã sửa như thế nào?       | Thay thế bằng cách import tĩnh (static import) trực tiếp các icon cần thiết (Settings, Folder, Key, v.v.) từ thư viện `lucide-react` để tối ưu hóa bundle size.    |
| Bài học rút ra                    | Tuyệt đối tránh việc import động toàn bộ thư viện trên các nền tảng web hiện đại để giữ cho bundle size luôn tối giản và tốc độ tải trang nhanh chóng.            |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
- Tự tay debug các lỗi biên dịch type-checking nghiêm trọng của AI liên quan đến React 19 class components và các imports bị thiếu.
- Thiết lập cơ chế bảo mật Git remote để bảo vệ mã nguồn, tự động xóa Personal Access Token ra khỏi tệp plain-text config ngay sau khi đẩy code lên repository.
- Thiết kế trải nghiệm tìm kiếm phím tắt Spotlight CMD/Ctrl+K kết hợp hook điều hướng bằng bàn phím mũi tên cực kỳ trực quan và mượt mà cho các nhà phát triển.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung      | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được                                                          |
| ------------- | ----------------- | --------------- | --------------------------------------------------------------------------- |
| Coding Speed  | Average           | Very Fast       | Rút ngắn 70% thời gian dựng khung sườn React Flow phức tạp và các Registry. |
| Architecture  | Good              | Excellent       | Tiếp cận và tích hợp nhanh mô hình Workspace Provider và Sandbox cô lập.    |
| UX Design     | Average           | Premium         | Thiết kế giao diện tối giản, tối ưu theo phong cách hiện đại của Linear.   |

---

## 11. Bài học về môn học

- Hiểu được tầm quan trọng của việc tách biệt và cô lập các vùng chức năng (Workspaces) để nâng cao độ tin cậy và khả năng mở rộng hệ thống.
- Hộp cát Sandbox Isolation là tiêu chuẩn bắt buộc khi trực quan hóa hoặc chạy thử mã nguồn động để bảo toàn độ ổn định của ứng dụng chính.
- Việc kiểm soát bundle size và Tree-shaking vô cùng quan trọng khi phát triển các hệ thống web quy mô doanh nghiệp.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Phải luôn chạy các trình biên dịch nghiêm ngặt (`npm run build`, `dotnet build`) để rà soát các lỗi cú pháp ẩn của AI trước khi bàn giao.
- Luôn kiểm soát bảo mật thông tin, tuyệt đối không để rò rỉ mã bảo mật Personal Access Token (PAT) trong plain-text config khi cấu hình git đẩy code.
- Phải hiểu rõ bản chất dòng code AI viết ra để có thể tự tay tinh chỉnh, tối ưu hóa theo các tiêu chuẩn kỹ thuật hiện hành.

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

- [x] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [x] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [x] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [x] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [x] Không dùng AI thay thế hoàn toàn quá trình học.
- [x] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

---

## 14. Kế hoạch cải thiện lần sau

- Sẽ nghiên cứu viết các prompt chặt chẽ hơn, định rõ phiên bản React 19 và TypeScript ngay từ đầu để hạn chế AI sinh sai cú pháp class component.
- Thử nghiệm tích hợp AI để sinh tự động các bộ dữ liệu mock đa dạng phục vụ cho việc kiểm thử hiệu năng render chịu tải lớn (stress test).

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí                         | Điểm tự đánh giá 1-5 | Ghi chú |
| -------------------------------- | :------------------: | ------- |
| Ghi nhận việc dùng AI trung thực |          5           |         |
| Prompt có mục tiêu rõ ràng       |          5           |         |
| Kiểm chứng kết quả AI            |          5           |         |
| Tự chỉnh sửa/cải tiến            |          5           |         |
| Hiểu nội dung đã nộp             |          5           |         |
| Reflection có chiều sâu          |          5           |         |
| Sử dụng AI có trách nhiệm        |          5           |         |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm nắm rõ cách thức hoạt động của WorkspaceProvider, thuật toán sắp xếp tọa độ đỉnh/cạnh React Flow, cơ chế bắt lỗi Error Boundary để hiển thị giao diện fallback trong Preview Sandbox.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Tuy nhiên việc thiết lập thủ công các mối quan hệ đồ thị phả hệ phức tạp trên React Flow canvas và căn chỉnh CSS/Framer Motion sẽ tiêu tốn thêm rất nhiều thời gian tự nghiên cứu tài liệu.
```

### 16.3. Phân nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Phần thiết kế giải pháp Workspace Provider động tách biệt hoàn toàn sidebar, trực tiếp debug sửa đổi cú pháp class component lỗi của AI để tương thích hoàn toàn với React 19 và cấu hình bảo mật Git remote loại bỏ PAT.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Nhóm muốn nâng cao kỹ năng thiết kế kiến trúc vi dịch vụ (Micro-frontends) và khả năng tối ưu hóa đồ họa tương tác hiệu năng cao trên trình duyệt web.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-29    |
