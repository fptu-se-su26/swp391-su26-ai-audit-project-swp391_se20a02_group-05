# Prompts

## 1. Thông tin chung

File này ghi lại các prompt thực tế đã sử dụng với AI trong phiên làm việc này.

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Tên project | CVerify |
| Tên sinh viên | Trần Nhất Long |
| MSSV | DE200160 |
| Ngày | 2026-06-17 |
| Phiên số | #8 |

---

## 2. Danh sách prompt đã sử dụng

### Prompt 1 — Kiểm tra tasks Line 3 và commit

**Công cụ:** Claude Code (claude-sonnet-4-6)

**Prompt:**
```text
@"C:\Users\long2\OneDrive\Documents\Claude\Projects\SWP391\CVerify_AI_Pipeline_v2_Task_Breakdown.xlsx"
đọc file và kiểm tra các task line 3 xem đã hoàn thành chưa, nếu rồi thì tạo commit, push và tạo PR để merge vào CVerify-uat
```

**Context khi dùng prompt:**
```text
- Đang ở branch feature-business-JD
- Đã có nhiều files thay đổi chưa commit liên quan đến Line 3 JD Matching
- File Excel chứa task breakdown với 15 tasks (L3-001 đến L3-015) đều marked TODO
- Commit trước (cca11ef) đã implement 3A (L3-001 đến L3-004)
```

**Kết quả mong đợi:**
```text
- AI đọc Excel, xác định tasks đã implement
- Commit và push toàn bộ thay đổi
- Tạo PR với đầy đủ description
```

**Kết quả thực tế:**
```text
- AI đọc sheet "Line 3 — JD Matching" từ Excel
- Xác nhận L3-001 đến L3-004 đã committed (cca11ef)
- Kiểm tra working tree: JdMatchingService.cs (371 dòng), orchestrator.py (517 dòng),
  application-quality-gate.tsx, migrations, tests
- Xác nhận tất cả 15 tasks đã implement
- git add 29 files cụ thể (không dùng git add -A)
- Commit: "feat: implement Line 3 JD Matching Pipeline (L3-005 to L3-015)"
  (8652 insertions, 29 files changed)
- Push lên origin/feature-business-JD
- gh pr create với title và body đầy đủ mô tả từng task
- PR URL: https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/pull/84
```

---

### Prompt 2 — Tạo audit documentation package

**Công cụ:** Claude Code (claude-sonnet-4-6)

**Prompt:**
```text
/cverify-code-to-ai-audit update for today's audit work
```

**Context khi dùng prompt:**
```text
- PR #84 đã được tạo
- Cần tạo audit documentation theo workflow cverify-code-to-ai-audit
```

**Kết quả mong đợi:**
```text
- Assign reviewer/assignee/labels cho PR #84
- Generate audit pack #8 (4 files)
- Commit lên doc/Tran-Nhat-Long
- Tạo audit PR targeting main
```

**Kết quả thực tế:**
```text
- Đọc SKILL.md từ ~/.claude/skills/ecc/cverify-code-to-ai-audit/SKILL.md
- Assign reviewer LucFr1746, assignee TNL293107 cho PR #84
- Apply labels: new feature, backend, frontend, audit
- Fetch origin/doc/Tran-Nhat-Long, xác định folder hiện tại: #6 và #7
- Đọc audit packs #7 để maintain formatting
- Generate audit pack #8: AI_AUDIT_LOG.md, CHANGELOG.md, PROMPTS.md, REFLECTION.md
- Commit lên doc/Tran-Nhat-Long: "docs(audit): add audit package #8"
- Tạo audit PR targeting main với reviewers nhnnanh, LucFr1746
```
