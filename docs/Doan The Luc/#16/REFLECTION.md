# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - CV Management, Source-Code Provider Integration & Session Inactivity Lock    |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-06-22                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình triển khai tái cấu trúc thanh điều hướng sidebar thành các nhóm Accordion Group thu gọn (Job Board, My CV), di chuyển Repositories lên section Intelligence và loại bỏ Evidence section, đồng thời thiết lập cơ chế đồng bộ hóa URL hai chiều dựa trên tham số query `tab`, AI đã hỗ trợ đắc lực trong việc sinh nhanh các mảng định nghĩa node group, đề xuất các icon phù hợp và viết các handler thay đổi URL. Sinh viên chịu trách nhiệm chính trong việc kiểm chứng logic active route tập trung, tối ưu hóa sự kiện click nút quay lại, và thực hiện dọn dẹp logic lọc của sidebar-content để code sạch hoàn toàn.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

### Công cụ được sử dụng nhiều nhất

```text
Antigravity
```

### Lý do sử dụng công cụ đó

```text
Antigravity tích hợp trực tiếp vào workspace và cung cấp các công cụ kiểm tra kiểu dữ liệu tự động (TypeScript compiler check), giúp tăng tốc đáng kể việc viết code và kiểm chứng chất lượng code sạch.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ ] Thiết kế giao diện
- [x] Thiết kế kiến trúc hệ thống
- [x] Viết code mẫu
- [x] Debug lỗi
- [ ] Viết test case
- [x] Review code
- [x] Tối ưu code
- [ ] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ ] Tìm hiểu công nghệ mới

### Mô tả chi tiết

```text
AI đã giúp sinh boilerplate code cấu hình group mới cho Job Board và My CV, gợi ý sử dụng router.push trong sự kiện onSelectionChange của component Tabs, và viết cấu trúc so khớp đường dẫn active trong file navigation-utils.ts.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu sâu sắc hơn về cơ chế định tuyến (Routing) của Next.js và cách đồng bộ hóa trạng thái URL (query parameters) để làm nguồn chân lý duy nhất (single source of truth) cho trạng thái highlight của thanh điều hướng.
- Nắm bắt tốt hơn cách tổ chức cấu trúc dữ liệu đệ quy (recursive navigation node) để tạo ra các menu nhiều cấp thu gọn (Accordion) linh hoạt trên giao diện.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI đề xuất chuyển đổi tab qua sự kiện onSelectionChange của component Tabs trên trang Jobs nhưng quên đồng bộ ở các hành động khác (như nút submit tìm kiếm), dẫn đến lệch trạng thái highlight của sidebar khi người dùng tìm kiếm ở tab khác. Nhóm đã tự bổ sung router.push trong handleSearchSubmit để sửa lỗi này.
- Khi tạo group My CV, AI không cập nhật sự kiện click của nút quay lại (Back arrow) để xóa tham số truy vấn trên URL, làm cho mục con trên sidebar vẫn sáng khi giao diện đã quay về Overview. Nhóm đã tự hiệu chỉnh bằng cách thêm router.push('/cv').
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
AI được sử dụng chủ yếu như một trợ lý viết mã cấu trúc lặp đi lặp lại để tiết kiệm thời gian gõ phím. Toàn bộ logic kiểm chứng, rà soát lỗi rẽ nhánh và dọn dẹp các logic lọc cũ đều do nhóm tự lập kế hoạch và trực tiếp thực hiện.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

- [x] Chạy thử chương trình
- [x] Kiểm tra output
- [ ] Viết test case
- [x] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [x] Review code
- [ ] Hỏi lại giảng viên
- [x] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
1. Khởi chạy ứng dụng Next.js và nhấp chọn các tab chức năng trên trang Jobs để xem URL thay đổi và kiểm tra xem mục con tương ứng trên sidebar có sáng lên đồng bộ hay không.
2. Tương tác với CV Management, click vào các mục chỉnh sửa chi tiết và kiểm tra xem group My CV trên sidebar có tự động mở rộng và highlight chính xác không. Nhấp nút Back và kiểm tra xem sidebar có tự thu gọn về Overview không.
3. Chạy lệnh biên dịch TypeScript (`npx tsc --noEmit`) để chắc chắn toàn bộ code sạch lỗi kiểu dữ liệu.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Cấu hình group My CV với các item con nhưng không đổi logic của nút Back trên giao diện CV Editor. |
| Em/nhóm đã kiểm tra bằng cách nào? | Chạy giao diện thử nghiệm, click chỉnh sửa phần "Basic Information" rồi bấm vào nút Back (mũi tên trái). |
| Kết quả kiểm tra | Giao diện quay về Overview thành công nhưng mục "Basic Information" trên sidebar vẫn sáng do URL vẫn giữ `?tab=basic-info`. |
| Em/nhóm đã xử lý tiếp như thế nào? | Bổ sung `router.push("/cv")` vào sự kiện `onClick` của nút quay lại trong file `cv/page.tsx` để làm sạch query param, giúp đồng bộ hóa hoàn toàn. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Gợi ý cấu hình chuyển đổi tab cho component Tabs trên trang Jobs nhưng bỏ quên sự kiện submit tìm kiếm `handleSearchSubmit`. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Thiếu sót các luồng rẽ nhánh phụ làm mất đi tính nhất quán hiển thị giữa trang và thanh sidebar. |
| Em/nhóm phát hiện bằng cách nào? | Đang ở tab "Recommended" và gõ tìm kiếm, trang tự chuyển về "Explore" nhưng mục "Recommended" trên sidebar vẫn sáng. |
| Em/nhóm đã sửa như thế nào? | Thêm `router.push("/jobs?tab=explore")` vào sự kiện submit của form tìm kiếm. |
| Bài học rút ra | Các luồng tương tác thay đổi trạng thái gián tiếp (như submit form tự động chuyển tab) luôn cần được rà soát kỹ lưỡng để đồng bộ URL đầy đủ. |

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

```text
- Nhóm đã chỉnh sửa logic sự kiện click nút quay lại Overview (`Back to Overview`) trong editor để thực hiện `router.push('/cv')`, làm sạch query parameter nhằm đưa sidebar trở lại highlight mục Overview.
- Nhóm đã tối ưu hóa component SidebarLink để gỡ bỏ logic so khớp tab tĩnh cũ và ủy quyền hoàn toàn cho isActiveRoute.
- Dọn dẹp logic lọc `evidence-section` cũ trong file sidebar-content.tsx để code sạch hoàn toàn.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Tiết kiệm ~50% thời gian thiết lập cấu trúc mảng định nghĩa node group lặp đi lặp lại. |
| Navigation UX | Basic | premium | Người dùng dễ dàng truy cập sâu vào từng phần của CV và Job Board từ sidebar một cách trực quan, đồng bộ. |
| Code Cleanliness | Basic | Clean | Gom gọn cấu trúc sidebar bằng cách gỡ bỏ các mục trùng lặp và các section thừa. |

---

## 11. Bài học về môn học

- Việc lấy URL làm nguồn chân lý duy nhất (single source of truth) là mô hình thiết kế tối ưu nhất khi phát triển các thanh điều hướng phức tạp chứa nhiều cấp liên kết con tương quan với tab hiển thị trên trang.
- Thiết kế hệ thống điều hướng tốt cần đi đôi với dọn dẹp các tài nguyên/section dư thừa (như Evidence section) để giữ trải nghiệm người dùng luôn tập trung và rõ ràng.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- Tuyệt đối không được chủ quan trước các gợi ý mã nguồn của AI, lập trình viên phải luôn kiểm soát toàn bộ các sự kiện thay đổi trạng thái bổ trợ để đảm bảo tính bao phủ (code coverage) và đồng bộ trạng thái chính xác.
- Luôn kiểm chứng sản phẩm trực tiếp trên môi trường chạy thực tế và chạy trình biên dịch kiểm tra kiểu trước khi nộp hoặc commit.

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

- Cung cấp mô tả chi tiết hơn về các sự kiện đi kèm (như nút quay lại hay submit form) khi yêu cầu AI viết mã đồng bộ hóa trang để AI đề xuất đầy đủ giải pháp ngay từ đầu.
- Rà soát trước các mục trùng lặp trong thiết kế giao diện để đưa ra cấu hình chuẩn ngay từ lần đầu tiên.

---

## 15. Tự đánh giá mức độ hoàn thành

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
| --- | --- | --- |
| Ghi nhận việc dùng AI trung thực | 5 | |
| Prompt có mục tiêu rõ ràng | 5 | |
| Kiểm chứng kết quả AI | 5 | |
| Tự chỉnh sửa/cải tiến | 5 | |
| Hiểu nội dung đã nộp | 5 | |
| Reflection có chiều sâu | 5 | |
| Sử dụng AI có trách nhiệm | 5 | |

---

## 16. Câu hỏi tự vấn cuối bài

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Có. Nhóm giải thích rõ được cơ chế hoạt động của Accordion Groups trong sidebar, cách sử dụng các sự kiện router.push để cập nhật URL động giúp đồng bộ hóa trực quan giữa tabs trang và sidebar, và logic so khớp tab trong isActiveRoute.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc viết cấu trúc cấu hình mảng, xử lý logic sự kiện click trong React, và tối ưu hóa so khớp URL đều là những kiến thức nền tảng vững vàng của nhóm.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Đoàn Thế Lực            | 2026-06-22    |
