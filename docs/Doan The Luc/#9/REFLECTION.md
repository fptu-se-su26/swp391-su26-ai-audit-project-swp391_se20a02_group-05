# AI Learning Reflection

## 1. Thông tin chung

| Thông tin                  | Nội dung                                                                               |
| -------------------------- | -------------------------------------------------------------------------------------- |
| Môn học                    | Software Development Project                                                           |
| Mã môn học                 | SWP391                                                                                 |
| Lớp                        | SE20A02                                                                                |
| Học kỳ                     | SU26                                                                                   |
| Tên bài tập / Project      | CVerify - Multi-Connection OAuth Linking, Per-Session Revocation & Pending Link Confirmation |
| Tên sinh viên / Nhóm       | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV      | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn       | QuangLTN3                                                                              |
| Ngày hoàn thành reflection | 2026-05-31                                                                             |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển hệ thống CVerify.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Trong quá trình triển khai multi-connection OAuth linking, per-session revocation middleware và pending link confirmation, AI đóng vai trò hỗ trợ phác thảo kiến trúc entity mới (PendingAuthProvider), sinh boilerplate cho background worker (PendingLinkCleanupService), và dựng layout cơ bản cho ConfirmationModal component. Nhóm đóng vai trò chủ đạo trong việc thiết kế các lớp bảo vệ an ninh: IgnoreQueryFilters để phát hiện xung đột soft-deleted providers, cơ chế lockout prevention khi ngắt kết nối OAuth, fail-safe fallback cho Redis transient failure trong middleware, và viết toàn bộ 6 integration tests bao phủ các kịch bản session revocation.
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
Antigravity có khả năng đọc và phân tích toàn bộ codebase CVerify (backend ASP.NET Core + frontend Next.js 16) đồng thời, hỗ trợ refactor xuyên suốt từ Entity → Service → Controller → UI component trong cùng một phiên làm việc.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

- [x] Hiểu yêu cầu đề bài
- [x] Phân tích bài toán
- [x] Tìm ý tưởng giải pháp
- [x] Thiết kế database
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
AI hỗ trợ thiết kế cấu trúc PendingAuthProvider entity, phác thảo logic background cleanup với distributed lock, sinh boilerplate middleware cho per-session validation, và dựng layout ConfirmationModal component với HeroUI v3 primitives. AI cũng hỗ trợ phân tích cách thêm sid claim vào JWT token và thiết kế cơ chế cache cho session status.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
Có. AI giúp nhóm:
- Hiểu rõ hơn về kiến trúc per-session revocation và sự khác biệt so với user-wide session invalidation.
- Nắm bắt cách thiết kế two-phase confirmation flow để bảo vệ chống OAuth callback redirect attack.
- Học cách tích hợp distributed lock cho background workers trong môi trường multi-instance.
- Hiểu sâu hơn về IgnoreQueryFilters trong EF Core và ứng dụng trong kiểm tra xung đột soft-deleted entities.
```

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
- AI không kiểm tra soft-deleted records khi xác minh xung đột OAuth linking, tạo ra lỗ hổng cho phép liên kết trùng provider_key xuyên tài khoản.
- AI không tích hợp lockout prevention logic khi ngắt kết nối OAuth provider: Cho phép người dùng ngắt phương thức đăng nhập duy nhất mà không cảnh báo, dẫn đến khóa tài khoản vĩnh viễn.
- AI không thêm fail-safe fallback cho Redis transient failure trong middleware, có thể gây lỗi 500 khi cache service tạm thời không khả dụng.
```

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [x] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm sử dụng AI chủ yếu để tăng tốc sinh boilerplate code và phác thảo layout component. Toàn bộ logic bảo mật (IgnoreQueryFilters, lockout prevention, concurrency guard, fail-safe fallback), thiết kế integration tests, và các quyết định kiến trúc quan trọng (two-phase confirmation, per-session revocation fallback) hoàn toàn do nhóm tự nghiên cứu và triển khai.
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
- [x] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [x] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI

### Mô tả quá trình kiểm chứng

```text
Nhóm kiểm chứng thông qua:
1. Viết và chạy thành công 6 integration tests mới cho SessionRevocationTests, bao phủ cả kịch bản active session, revoked session, missing sid fallback, invalid sid, và self-revocation guard.
2. Kiểm thử thủ công trên giao diện: Liên kết GitHub qua OAuth callback → xác nhận pending link → hiển thị connection card → ngắt kết nối thành công.
3. Kiểm tra lockout prevention: Thử ngắt kết nối Google khi tài khoản chưa có mật khẩu → modal hiển thị blockingError chặn hành động.
4. Kiểm tra thu hồi hàng loạt phiên đăng nhập: Gọi DELETE /auth/sessions → xác nhận các phiên bị thu hồi không thể truy cập API.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Kiểm tra xung đột OAuth linking chỉ filter `DeletedAt == null`, bỏ qua các bản ghi soft-deleted. |
| Em/nhóm đã kiểm tra bằng cách nào? | Tạo kịch bản thử nghiệm: Liên kết Google provider → soft-delete → liên kết lại từ tài khoản khác với cùng provider_key. |
| Kết quả kiểm tra | Hệ thống cho phép liên kết trùng provider_key xuyên tài khoản vì query chỉ tìm bản ghi active (DeletedAt == null). |
| Em/nhóm đã xử lý tiếp như thế nào? | Thay thế query bằng `IgnoreQueryFilters()` để quét toàn bộ bản ghi bao gồm cả soft-deleted, chặn xung đột và ghi audit event PROVIDER_LINK_CONFLICT. |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

| Nội dung | Mô tả |
| --- | --- |
| AI đã gợi ý gì? | Cho phép ngắt kết nối OAuth provider mà không kiểm tra xem người dùng có phương thức đăng nhập thay thế hay không. |
| Vì sao gợi ý đó sai/chưa phù hợp? | Gây ra kịch bản khóa tài khoản vĩnh viễn (account lockout): Nếu người dùng chỉ có Google SSO mà không có mật khẩu, ngắt kết nối Google sẽ khiến tài khoản không thể truy cập lại được. |
| Em/nhóm phát hiện bằng cách nào? | Kiểm thử thủ công kịch bản edge case: Tạo tài khoản chỉ bằng Google SSO → thử ngắt kết nối Google → xác nhận tài khoản bị khóa vĩnh viễn. |
| Em/nhóm đã sửa như thế nào? | Thiết kế logic lockout prevention ở frontend: Trước khi hiển thị nút Confirm trên ConfirmationModal, kiểm tra user.hasPassword và số lượng active providers. Nếu ngắt kết nối sẽ gây lockout, hiển thị blockingError thay vì cho phép xác nhận. |
| Bài học rút ra | Mọi hành động hủy kết nối phương thức xác thực đều phải kiểm tra xem người dùng còn ít nhất một phương thức đăng nhập thay thế. Đây là nguyên tắc bảo mật cơ bản mà AI thường bỏ qua khi chỉ tập trung vào logic chức năng. |

---

## 9. Phân đóng góp thật sự của sinh viên/nhóm

```text
- Thiết kế logic IgnoreQueryFilters để kiểm tra xung đột OAuth provider bao gồm soft-deleted records.
- Thiết kế cơ chế lockout prevention khi ngắt kết nối OAuth provider duy nhất.
- Viết fail-safe fallback cho Redis transient failure trong SessionValidationMiddleware.
- Thiết kế concurrency guard trong PasswordRecoveryService chống provisioning song song.
- Viết toàn bộ 6 integration tests cho session revocation middleware.
- Thiết kế luồng reactivation cho Google provider đã soft-delete.
- Bọc logic unlink provider trong database transaction đảm bảo tính nguyên tử.
```

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
| --- | --- | --- | --- |
| Coding Speed | Average | Fast | Rút ngắn 40% thời gian dựng entity, background worker và UI component boilerplate. |
| Code Quality | Good | Excellent | Code bảo mật hơn nhờ bổ sung IgnoreQueryFilters, lockout prevention, và fail-safe fallback. |
| Testing | Good | Excellent | Tăng độ bao phủ kiểm thử với 6 integration tests mới cho session revocation edge cases. |

---

## 11. Bài học về môn học

- Per-session revocation là tiêu chuẩn bảo mật hiện đại cho hệ thống authentication; user-wide session invalidation quá thô sơ cho các ứng dụng multi-device.
- Two-phase confirmation (pending → confirm) là pattern thiết yếu cho các hành động liên kết danh tính OAuth để chống OAuth callback redirect attack.
- Rolling deployment compatibility phải được cân nhắc khi thay đổi cấu trúc JWT claims; cơ chế fallback đảm bảo không gián đoạn dịch vụ cho người dùng hiện tại.

---

## 12. Bài học về sử dụng AI có trách nhiệm

- AI có xu hướng bỏ qua soft-deleted records trong các query kiểm tra xung đột. Sinh viên cần chủ động rà soát xem hệ thống có sử dụng soft-delete pattern hay không và yêu cầu AI xử lý đúng.
- AI không tự động thiết kế lockout prevention. Bất kỳ tính năng nào cho phép hủy kết nối phương thức xác thực đều cần sinh viên tự thiết kế lớp bảo vệ chống khóa tài khoản.

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

- Yêu cầu AI kiểm tra soft-delete awareness ngay trong prompt đầu tiên để tránh phải sửa lỗi xung đột danh tính ở giai đoạn review.
- Bổ sung kịch bản kiểm thử lockout prevention vào checklist review bắt buộc trước khi merge bất kỳ tính năng nào liên quan đến quản lý phương thức xác thực.

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
Có. Nhóm hoàn toàn giải thích được kiến trúc two-phase pending link confirmation, cơ chế per-session revocation qua JWT sid claim với cookie fallback, logic IgnoreQueryFilters cho soft-delete awareness, và nguyên lý lockout prevention khi ngắt kết nối OAuth.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. Việc thiết kế per-session revocation middleware và PendingAuthProvider entity hoàn toàn nằm trong khả năng của nhóm khi tra cứu tài liệu Microsoft ASP.NET Core và Entity Framework Core chính thống. AI chỉ giúp tăng tốc quá trình viết boilerplate code.
```

---

## 17. Cam kết Reflection

Sinh viên/nhóm cam kết nội dung reflection phản ánh chân thực quá trình làm việc.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-31    |
