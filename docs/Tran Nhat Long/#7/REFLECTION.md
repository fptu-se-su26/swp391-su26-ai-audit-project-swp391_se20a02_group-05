# AI Learning Reflection

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Trần Nhất Long, Nguyễn La Hòa An |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE200160, DE201043 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày hoàn thành reflection | 2026-06-12 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và phát triển phần mềm.

---

## 3. Tóm tắt quá trình sử dụng AI

```text
Phiên làm việc này (audit pack #7) là phiên audit documentation cho một re-commit:
commit 10f43487f là re-application của toàn bộ v2 pipeline task changeset sau khi
commit faaa877b8 (đã được audit ở #6) bị revert qua b89192101.

Trong phiên này, AI (Claude Code) được sử dụng để:
1. Trace lịch sử commit (faaa877 → revert → 10f43487f)
2. Xác định audit pack tiếp theo (#7, vì #6 đã cover faaa877)
3. Generate 4 audit documentation files cho #7 với context re-commit chính xác

Nội dung kỹ thuật (pipeline tasks, token debug) không thay đổi so với #6 —
phiên này chỉ document quyết định re-apply và context lịch sử.
```

---

## 4. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [ ] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity

---

## 5. Phân tích chi tiết

### 5.1. Những điều AI làm tốt

```text
1. Commit history tracing: AI xác định chính xác chuỗi faaa877 → b89192101 (revert)
   → 10f43487f (re-apply) từ git log và git show, không nhầm lẫn giữa các commit.

2. Sequential audit numbering: AI nhận ra #6 đã tồn tại cho faaa877b8 và tạo #7
   cho 10f43487f, không overwrite hoặc skip số.

3. Re-commit context trong documentation: CHANGELOG.md được bổ sung bảng lịch sử
   commit 3 dòng (faaa877 → revert → 10f43487f) để người đọc hiểu ngữ cảnh
   mà không cần tra git log.

4. Consistent formatting: AI duy trì đúng format tiếng Việt + table structure
   từ #6 sang #7, bao gồm header, section numbering, và checkbox style.
```

### 5.2. Những điều AI làm chưa tốt / cần điều chỉnh

```text
1. Audit pack #7 phần lớn là mirror của #6 vì changeset không thay đổi.
   Trong tương lai, nếu re-commit có thay đổi thực sự (bug fix, algorithm change),
   cần differentiate kỹ hơn phần "Thay đổi chi tiết" thay vì chỉ note "Re-apply".

2. Lý do revert (b89192101) không được ghi rõ trong commit message của revert.
   AI không thể determine chính xác nguyên nhân — chỉ note "branch integration issues"
   dựa trên context. Developer cần bổ sung lý do cụ thể nếu cần audit trail đầy đủ.
```

### 5.3. Quyết định kỹ thuật quan trọng

```text
1. Tạo audit pack mới (#7) thay vì amend #6:
   Decision: Tạo #7 riêng biệt cho 10f43487f
   Reasoning: Mỗi commit hash có audit riêng để đảm bảo traceability.
   Nếu audit pack gắn với faaa877 (đã revert), auditor sẽ không tìm thấy
   documentation cho commit đang hiện diện trên branch.
   Audit trail phải follow commit hash, không follow content.

2. Ghi lại toàn bộ commit chain trong CHANGELOG:
   Decision: Thêm bảng "Lịch sử commit liên quan" với 3 entries
   Reasoning: Revert → re-apply là một pattern cần transparent trong audit log.
   Nếu chỉ ghi commit cuối cùng, reviewer sẽ không hiểu tại sao có phase 07
   với content giống phase 06.

3. Giữ lại "Lỗi / vấn đề phát sinh" section với revert là lỗi:
   Decision: Ghi revert là một "vấn đề phát sinh" trong CHANGELOG
   Reasoning: Revert là signal rằng có issue với lần commit trước.
   Audit log nên reflect thực tế này thay vì chỉ document happy path.
```

### 5.4. Bài học rút ra

```text
1. Audit trail phải theo commit hash, không theo content:
   Hai commit với content giống nhau (faaa877 và 10f43487f) vẫn cần audit pack
   riêng biệt vì chúng có hash khác nhau và nằm tại vị trí khác nhau trong
   branch history. Auditor track theo hash, không theo diff.

2. Revert → re-apply là pattern bình thường trong Git workflow:
   Không cần "xin lỗi" hay giảm thiểu trong documentation. Ghi thẳng:
   "original commit bị revert, re-applied sau khi integration issue resolved."
   Transparent documentation có giá trị cao hơn clean-looking audit log.

3. Sequential audit numbering phải được enforce nghiêm:
   Nếu AI đã tạo #6, #7 phải là bước tiếp theo — dù #7 có content tương tự #6.
   Gaps hoặc overwrite trong audit numbering sẽ gây confusion khi review.

4. AI documentation workflow scale tốt với re-commit scenarios:
   cverify-code-to-ai-audit skill handle được cả trường hợp re-commit
   vì nó trace từ commit hash, không từ working tree diff.
   Skill có thể invoke với commit hash argument bất kỳ.
```

---

## 6. Tự đánh giá

| Tiêu chí | Mức độ | Ghi chú |
|---|---|---|
| Hiểu rõ code AI sinh ra | ✅ Tốt | Code được review từ phiên #6; #7 re-apply cùng changeset |
| Kiểm tra kết quả AI | ✅ Tốt | Verified commit hash và file list trước khi approve audit PR |
| Không phụ thuộc mù quáng vào AI | ✅ Tốt | Developer quyết định tạo #7 riêng; AI chỉ execute documentation |
| Ghi lại quá trình đầy đủ | ✅ Tốt | Revert history được document rõ ràng trong CHANGELOG và REFLECTION |
| Học hỏi từ AI | ✅ Tốt | AI nhắc nhở audit trail theo commit hash — adopt làm practice |

---

## 7. Kế hoạch cải thiện

```text
1. Document lý do revert trong commit message:
   Commit b89192101 revert message không giải thích lý do.
   Cải thiện: Luôn thêm body vào revert commit messages với nguyên nhân cụ thể
   (ví dụ: "reverts due to merge conflict with upstream AppLayout refactor").

2. Xem xét squash revert + re-apply:
   Pattern revert → re-apply tạo noise trong git log.
   Alternative: dùng interactive rebase để squash nếu không cần preserve history
   trung gian. Trade-off: rebase rewrite history — chỉ safe trên feature branch
   chưa share.

3. Automation check: nếu re-commit có diff với original commit:
   Thêm git diff faaa877 10f43487f vào workflow để detect nếu re-commit có thay đổi.
   Nếu có diff, CHANGELOG cần section riêng cho delta, không chỉ note "re-apply".
```
