import { Project, EvidenceItem } from "@/types/project";

const evidenceTypeLabels: Record<EvidenceItem['type'], string> = {
  commit: 'Commit/PR',
  screenshot: 'Screenshot',
  demo: 'Demo Link',
  test: 'Test Result',
  file: 'Related File',
  note: 'Note',
};

export const generateChangelog = (project: Project): string => {
  const { metadata, members, changelogs, changelogSummary } = project;
  
  let markdown = `# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Môn học | ${metadata.course || ''} |
| Mã môn học | ${metadata.courseCode || ''} |
| Lớp | ${metadata.class || ''} |
| Học kỳ | ${metadata.semester || ''} |
| Tên bài tập / Project | ${metadata.name || ''} |
| Tên sinh viên / Nhóm | ${members.map(m => m.name).join(', ') || ''} |
| MSSV / Danh sách MSSV | ${members.map(m => m.studentId).join(', ') || ''} |
| Giảng viên hướng dẫn | ${metadata.lecturer || ''} |
| Repository URL | ${metadata.repoUrl || ''} |
| Ngày bắt đầu | ${metadata.startDate || ''} |
| Ngày hoàn thành | ${metadata.endDate || ''} |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
`;

  // General phases overview
  const defaultPhases = ["Phase 01", "Phase 02", "Phase 03", "Phase 04", "Phase 05", "Phase 06"];
  defaultPhases.forEach(defaultPhase => {
    const existing = changelogs.find(c => c.phaseName.includes(defaultPhase));
    if (existing) {
      markdown += `| ${existing.phaseName} | ${existing.date} | ${existing.notes.split('\\n')[0] || existing.phaseName} | ${existing.status} |\n`;
    } else {
      markdown += `| ${defaultPhase} |  |  | Not Started |\n`;
    }
  });

  markdown += `\n---\n\n`;

  // Details for each phase
  changelogs.forEach((phase) => {
    markdown += `# [${phase.phaseName}] \n\n`;
    markdown += `## Ngày thực hiện\n\n\`\`\`text\n${phase.date}\n\`\`\`\n\n`;
    markdown += `## Thay đổi chi tiết\n\n`;
    markdown += `| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |\n`;
    markdown += `|---:|---|---|---|---|\n`;
    
    if (phase.changes && phase.changes.length > 0) {
      phase.changes.forEach((c, i) => {
        markdown += `| ${i + 1} | ${c.content || ' '} | ${c.author || ' '} | ${c.files || ' '} | ${c.evidence || ' '} |\n`;
      });
    } else {
      markdown += `| 1 |  |  |  |  |\n`;
    }

    markdown += `\n## AI có hỗ trợ không?\n\n`;
    markdown += `- [${phase.aiSupport?.used ? 'x' : ' '}] Có\n`;
    markdown += `- [${!phase.aiSupport?.used ? 'x' : ' '}] Không\n\n`;
    
    if (phase.aiSupport?.used) {
      markdown += `Nếu có, mô tả AI đã hỗ trợ phần nào:\n\n\`\`\`text\n${phase.aiSupport.description || ' '}\n\`\`\`\n\n`;
    }

    markdown += `## Commit/Screenshot minh chứng\n\n\`\`\`text\n${phase.evidenceLink || ' '}\n\`\`\`\n\n`;
    markdown += `## Ghi chú\n\n\`\`\`text\n${phase.notes || ' '}\n\`\`\`\n\n`;
    markdown += `---\n\n`;
  });

  // End sections — populated from changelogSummary
  const summary = changelogSummary || { completedFeatures: '', unfinishedFeatures: '', majorImprovements: '', overallSummary: '', futureImprovements: '' };

  markdown += `# 4. Tổng kết thay đổi cuối project\n\n`;
  markdown += `## 4.1. Các chức năng đã hoàn thành\n\n\`\`\`text\n${summary.completedFeatures || 'Chưa có thông tin.'}\n\`\`\`\n\n---\n\n`;
  markdown += `## 4.2. Các chức năng chưa hoàn thành\n\n\`\`\`text\n${summary.unfinishedFeatures || 'Chưa có thông tin.'}\n\`\`\`\n\n---\n\n`;
  markdown += `## 4.3. Cải thiện chính\n\n\`\`\`text\n${summary.majorImprovements || 'Chưa có thông tin.'}\n\`\`\`\n\n---\n\n`;
  markdown += `## 4.4. Tổng kết project\n\n\`\`\`text\n${summary.overallSummary || 'Chưa có thông tin.'}\n\`\`\`\n\n---\n\n`;
  markdown += `## 4.5. Hướng cải thiện tiếp theo\n\n\`\`\`text\n${summary.futureImprovements || 'Chưa có thông tin.'}\n\`\`\`\n\n---\n\n`;
  markdown += `# 5. Cam kết cập nhật Changelog\n\n`;
  markdown += `Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.\n\n`;
  markdown += `| Đại diện sinh viên/nhóm | Ngày xác nhận |\n`;
  markdown += `|---|---|\n`;
  markdown += `| ${members[0]?.name || ' '} | ${new Date().toLocaleDateString('vi-VN')} |\n`;

  return markdown;
};

export const generatePrompts = (project: Project): string => {
  const { metadata, members, prompts, promptLessons } = project;
  
  let markdown = `# Prompt Log\n\n`;
  markdown += `## 1. Thông tin chung\n\n`;
  markdown += `| Thông tin | Nội dung |\n|---|---|\n`;
  markdown += `| Môn học | ${metadata.course || ''} |\n`;
  markdown += `| Mã môn học | ${metadata.courseCode || ''} |\n`;
  markdown += `| Lớp | ${metadata.class || ''} |\n`;
  markdown += `| Học kỳ | ${metadata.semester || ''} |\n`;
  markdown += `| Tên bài tập / Project | ${metadata.name || ''} |\n`;
  markdown += `| Tên sinh viên / Nhóm | ${members.map(m => m.name).join(', ') || ''} |\n`;
  markdown += `| MSSV / Danh sách MSSV | ${members.map(m => m.studentId).join(', ') || ''} |\n`;
  markdown += `| Giảng viên hướng dẫn | ${metadata.lecturer || ''} |\n`;
  markdown += `| Ngày bắt đầu | ${metadata.startDate || ''} |\n`;
  markdown += `| Ngày cập nhật gần nhất | ${new Date().toISOString().split('T')[0]} |\n\n---\n\n`;

  markdown += `## 2. Mục đích của file Prompt Log\n\nFile này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.\n\n---\n\n`;

  markdown += `## 3. Công cụ AI đã sử dụng\n\n`;
  const toolsList = ["ChatGPT", "Gemini", "Claude", "GitHub Copilot", "Cursor", "Antigravity", "Microsoft Copilot", "Perplexity"];
  const usedTools = Array.from(new Set(prompts.map(p => p.aiTool)));
  toolsList.forEach(tool => {
    markdown += `- [${usedTools.includes(tool) ? 'x' : ' '}] ${tool}\n`;
  });
  markdown += `- [ ] Công cụ khác: ....................................\n\n---\n\n`;

  markdown += `## 4. Bảng tổng hợp prompt đã sử dụng\n\n`;
  markdown += `| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |\n`;
  markdown += `|---:|---|---|---|---|---|---|---|\n`;
  if (prompts.length === 0) {
    markdown += `| 1 |  |  |  |  |  | Có / Không |  |\n`;
  } else {
    prompts.forEach((p, i) => {
      markdown += `| ${i + 1} | ${p.date} | ${p.aiTool} | ${p.purpose} | ${(p.promptText || '').substring(0, 30)}... | ${(p.aiResponse || '').substring(0, 30)}... | ${p.appliedResult ? 'Có' : 'Không'} | ${p.evidence?.[0]?.content || ' '} |\n`;
    });
  }
  markdown += `\n---\n\n`;

  markdown += `## 5. Prompt chi tiết\n\n`;
  
  prompts.forEach((p, i) => {
    markdown += `### Prompt số ${i + 1}\n\n`;
    markdown += `| Nội dung | Thông tin |\n|---|---|\n`;
    markdown += `| Ngày sử dụng | ${p.date} |\n`;
    markdown += `| Công cụ AI | ${p.aiTool} |\n`;
    markdown += `| Mục đích | ${p.purpose} |\n`;
    markdown += `| Phần việc liên quan | ${p.category} |\n`;
    markdown += `| Mức độ sử dụng | ${p.usageLevel} |\n\n`;
    
    markdown += `#### 5.1. Prompt nguyên văn\n\n\`\`\`text\n${p.promptText || ' '}\n\`\`\`\n\n`;
    markdown += `#### 5.2. Bối cảnh khi viết prompt\n\n\`\`\`text\n${p.context || ' '}\n\`\`\`\n\n`;
    markdown += `#### 5.3. Kết quả AI trả về\n\n\`\`\`text\n${p.aiResponse || ' '}\n\`\`\`\n\n`;
    markdown += `#### 5.4. Kết quả đã áp dụng vào bài\n\n\`\`\`text\n${p.appliedResult || ' '}\n\`\`\`\n\n`;
    markdown += `#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến\n\n\`\`\`text\n${p.improvements || ' '}\n\`\`\`\n\n`;
    
    markdown += `#### 5.6. Đánh giá chất lượng prompt\n\n`;
    const check = p.evaluationChecklist || [];
    markdown += `- [${check.includes("Prompt rõ ràng") ? 'x' : ' '}] Prompt rõ ràng\n`;
    markdown += `- [${check.includes("Prompt có đủ bối cảnh") ? 'x' : ' '}] Prompt có đủ bối cảnh\n`;
    markdown += `- [${check.includes("Prompt còn thiếu thông tin") ? 'x' : ' '}] Prompt còn thiếu thông tin\n`;
    markdown += `- [${check.includes("Prompt tạo ra kết quả tốt") ? 'x' : ' '}] Prompt tạo ra kết quả tốt\n`;
    markdown += `- [${check.includes("Prompt tạo ra kết quả chưa phù hợp") ? 'x' : ' '}] Prompt tạo ra kết quả chưa phù hợp\n`;
    markdown += `- [${check.includes("Cần hỏi lại AI nhiều lần") ? 'x' : ' '}] Cần hỏi lại AI nhiều lần\n`;
    markdown += `- [${check.includes("Cần tự kiểm tra và chỉnh sửa nhiều") ? 'x' : ' '}] Cần tự kiểm tra và chỉnh sửa nhiều\n\n`;

    markdown += `#### 5.7. Minh chứng liên quan\n\n| Loại minh chứng | Nội dung |\n|---|---|\n| File/Link | ${p.evidence?.[0]?.content || ' '} |\n\n`;
    markdown += `#### 5.8. Ghi chú thêm\n\n\`\`\`text\n${p.notes || ' '}\n\`\`\`\n\n---\n\n`;
  });

  const mostImportant = prompts.find(p => p.isMostImportant) || prompts[0];
  markdown += `## 6. Prompt quan trọng nhất\n\n`;
  if (mostImportant) {
    markdown += `### 6.1. Prompt được chọn\n\n\`\`\`text\n${mostImportant.promptText || ' '}\n\`\`\`\n\n`;
    markdown += `### 6.2. Vì sao prompt này quan trọng?\n\n\`\`\`text\n${mostImportant.importanceExplanation || mostImportant.notes || ' '}\n\`\`\`\n\n`;
    markdown += `### 6.3. Kết quả prompt này mang lại\n\n\`\`\`text\n${mostImportant.aiResponse || ' '}\n\`\`\`\n\n`;
    markdown += `### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?\n\n\`\`\`text\n${mostImportant.appliedResult || ' '}\n\`\`\`\n\n`;
    markdown += `### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?\n\n\`\`\`text\n${mostImportant.improvements || ' '}\n\`\`\`\n\n---\n\n`;
  } else {
    markdown += `\`\`\`text\nChưa chọn prompt quan trọng nhất.\n\`\`\`\n\n---\n\n`;
  }

  const ineffective = prompts.find(p => p.isIneffective);
  markdown += `## 7. Prompt chưa hiệu quả\n\n`;
  if (ineffective) {
    markdown += `### 7.1. Prompt chưa hiệu quả\n\n\`\`\`text\n${ineffective.promptText || ' '}\n\`\`\`\n\n`;
    markdown += `### 7.2. Vì sao prompt này chưa hiệu quả?\n\n\`\`\`text\n${ineffective.ineffectiveDetails?.reason || ' '}\n\`\`\`\n\n`;
    markdown += `### 7.3. Cách cải thiện prompt\n\n\`\`\`text\n${ineffective.ineffectiveDetails?.improvementMethod || ' '}\n\`\`\`\n\n`;
    markdown += `### 7.4. Prompt sau khi cải tiến\n\n\`\`\`text\n${ineffective.ineffectiveDetails?.improvedPrompt || ' '}\n\`\`\`\n\n`;
    markdown += `### 7.5. Kết quả sau khi cải tiến prompt\n\n\`\`\`text\n${ineffective.ineffectiveDetails?.newResult || ' '}\n\`\`\`\n\n---\n\n`;
  } else {
    markdown += `\`\`\`text\nChưa có prompt chưa hiệu quả được ghi nhận.\n\`\`\`\n\n---\n\n`;
  }

  markdown += `## 8. Bài học về cách viết prompt\n\n`;
  markdown += `### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?\n\n\`\`\`text\n${promptLessons?.infoNeeded || ' '}\n\`\`\`\n\n`;
  markdown += `### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?\n\n\`\`\`text\n${promptLessons?.lessonsLearned || ' '}\n\`\`\`\n\n`;
  markdown += `### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?\n\n\`\`\`text\n${promptLessons?.futureImprovements || ' '}\n\`\`\`\n\n---\n\n`;

  // Categories
  markdown += `## 9. Phân loại prompt đã sử dụng\n\n| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |\n|---|---:|---|\n`;
  const categoriesMap = prompts.reduce((acc, p) => { acc[p.category] = (acc[p.category] || 0) + 1; return acc; }, {} as Record<string, number>);
  Object.keys(categoriesMap).forEach(cat => {
    markdown += `| Prompt ${cat} | ${categoriesMap[cat]} |  |\n`;
  });
  markdown += `\n---\n\n`;

  markdown += `## 10. Checklist chất lượng prompt\n\n`;
  markdown += `| Tiêu chí | Đã đạt? | Ghi chú |\n|---|:---:|---|\n`;
  markdown += `| Prompt có mục tiêu rõ ràng | x | |\n`;
  markdown += `| Prompt có đủ bối cảnh | x | |\n`;
  markdown += `| Tự kiểm tra và chỉnh sửa | x | |\n\n---\n\n`;

  markdown += `## 11. Cam kết sử dụng prompt minh bạch\n\n`;
  markdown += `| Đại diện sinh viên/nhóm | Ngày xác nhận |\n|---|---|\n| ${members[0]?.name || ' '} | ${new Date().toLocaleDateString('vi-VN')} |\n`;

  return markdown;
};

export const generateAiAudit = (project: Project): string => {
  const { metadata, members, aiAudit } = project;
  
  let markdown = `# AI Audit Log\n\n## 1. Thông tin chung\n\n| Thông tin | Nội dung |\n|---|---|\n`;
  markdown += `| Môn học | ${metadata.course || ''} |\n`;
  markdown += `| Mã môn học | ${metadata.courseCode || ''} |\n`;
  markdown += `| Lớp | ${metadata.class || ''} |\n`;
  markdown += `| Học kỳ | ${metadata.semester || ''} |\n`;
  markdown += `| Tên bài tập / Project | ${metadata.name || ''} |\n`;
  markdown += `| Tên sinh viên / Nhóm | ${members.map(m => m.name).join(', ') || ''} |\n`;
  markdown += `| MSSV / Danh sách MSSV | ${members.map(m => m.studentId).join(', ') || ''} |\n`;
  markdown += `| Giảng viên hướng dẫn | ${metadata.lecturer || ''} |\n`;
  markdown += `| Ngày bắt đầu | ${metadata.startDate || ''} |\n`;
  markdown += `| Ngày hoàn thành | ${metadata.endDate || ''} |\n\n---\n\n`;

  markdown += `## 2. Công cụ AI đã sử dụng\n\n`;
  const toolsList = ["ChatGPT", "Gemini", "Claude", "GitHub Copilot", "Cursor", "Antigravity", "Perplexity", "Microsoft Copilot"];
  toolsList.forEach(tool => {
    markdown += `- [${aiAudit.toolsUsed?.includes(tool) ? 'x' : ' '}] ${tool}\n`;
  });
  markdown += `- [ ] Công cụ khác: ....................................\n\n---\n\n`;

  markdown += `## 3. Mục tiêu sử dụng AI\n\n### Mô tả mục tiêu sử dụng AI\n\n\`\`\`text\n${aiAudit.usageTargetsText || ' '}\n\`\`\`\n\n`;
  
  markdown += `## 4. Nhật ký sử dụng AI chi tiết\n\n---\n\n`;
  
  (aiAudit.auditEntries || []).forEach((e, i) => {
    markdown += `### Lần sử dụng AI số ${i + 1}\n\n`;
    markdown += `| Nội dung | Thông tin |\n|---|---|\n`;
    markdown += `| Ngày sử dụng | ${e.date} |\n`;
    markdown += `| Công cụ AI | ${e.aiTool} |\n`;
    markdown += `| Mục đích sử dụng | ${e.purpose} |\n`;
    markdown += `| Phần việc liên quan | ${e.category} |\n`;
    markdown += `| Mức độ sử dụng | ${e.usageLevel} |\n\n`;
    
    markdown += `#### 4.1. Prompt đã sử dụng\n\n\`\`\`text\n${e.prompt || ' '}\n\`\`\`\n\n`;
    markdown += `#### 4.2. Kết quả AI gợi ý\n\n\`\`\`text\n${e.aiResponseSummary || ' '}\n\`\`\`\n\n`;
    markdown += `#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI\n\n\`\`\`text\n${e.usedContent || ' '}\n\`\`\`\n\n`;
    markdown += `#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến\n\n\`\`\`text\n${e.modifications || ' '}\n\`\`\`\n\n`;
    
    markdown += `#### 4.5. Minh chứng\n\n| Loại minh chứng | Nhãn | Nội dung |\n|---|---|---|\n`;
    if (e.evidence && e.evidence.length > 0) {
      e.evidence.forEach((ev) => {
        const typeLabel = evidenceTypeLabels[ev.type as keyof typeof evidenceTypeLabels] || ev.type;
        markdown += `| ${typeLabel} | ${ev.label || ' '} | ${ev.content || ev.fileName || ' '} |\n`;
      });
    } else {
      markdown += `| File/Commit |  |  |\n`;
    }
    markdown += `\n`;

    markdown += `#### 4.6. Nhận xét cá nhân/nhóm\n\n\`\`\`text\n${e.lessonsLearned || ' '}\n\`\`\`\n\n---\n\n`;
  });

  markdown += `## 5. Bảng tổng hợp mức độ sử dụng AI\n\n`;
  markdown += `| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |\n`;
  markdown += `|---|:---:|:---:|:---:|:---:|---|\n`;
  (aiAudit.usageMatrix || []).forEach(m => {
    const l1 = m.usageLevel === 'Không dùng AI' ? 'x' : ' ';
    const l2 = m.usageLevel === 'AI hỗ trợ ít' ? 'x' : ' ';
    const l3 = m.usageLevel === 'AI hỗ trợ nhiều' ? 'x' : ' ';
    const l4 = m.usageLevel === 'AI sinh chính' ? 'x' : ' ';
    markdown += `| ${m.category} | ${l1} | ${l2} | ${l3} | ${l4} | ${m.notes || ' '} |\n`;
  });
  markdown += `\n---\n\n`;

  markdown += `## 6. Các lỗi hoặc hạn chế từ AI\n\n`;
  markdown += `| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |\n|---:|---|---|---|\n`;
  if (!aiAudit.issues || aiAudit.issues.length === 0) {
    markdown += `| 1 |  |  |  |\n`;
  } else {
    aiAudit.issues.forEach((issue, i) => {
      markdown += `| ${i + 1} | ${issue.description || ' '} | ${issue.detectionMethod || ' '} | ${issue.resolution || ' '} |\n`;
    });
  }
  markdown += `\n---\n\n`;

  markdown += `## 7. Kiểm chứng kết quả AI\n\n### Nội dung kiểm chứng\n\n\`\`\`text\n${aiAudit.verificationMethodsText || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 8. Đóng góp cá nhân hoặc đóng góp nhóm\n\n`;
  markdown += `### 8.1. Đối với bài cá nhân\n\n\`\`\`text\n${aiAudit.personalContributionText || ' '}\n\`\`\`\n\n`;
  markdown += `### 8.2. Đối với bài nhóm\n\n`;
  markdown += `| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |\n|---|---|---|---|---|\n`;
  if (!aiAudit.groupContributions || aiAudit.groupContributions.length === 0) {
    markdown += `|  |  |  | Có / Không |  |\n`;
  } else {
    aiAudit.groupContributions.forEach(g => {
      markdown += `| ${g.memberName} | ${g.memberId} | ${g.tasks} | ${g.aiUsed ? 'Có' : 'Không'} | ${g.evidence || ' '} |\n`;
    });
  }
  markdown += `\n---\n\n`;

  markdown += `## 9. Reflection cuối bài\n\n### Xem chi tiết tại REFLECTION.md\n\n---\n\n`;

  markdown += `## 10. Cam kết học thuật\n\n| Đại diện sinh viên/nhóm | Ngày xác nhận |\n|---|---|\n| ${members[0]?.name || ' '} | ${new Date().toLocaleDateString('vi-VN')} |\n`;

  return markdown;
};

export const generateReflection = (project: Project): string => {
  const { metadata, members, reflection } = project;
  
  let markdown = `# AI Learning Reflection\n\n## 1. Thông tin chung\n\n| Thông tin | Nội dung |\n|---|---|\n`;
  markdown += `| Môn học | ${metadata.course || ''} |\n`;
  markdown += `| Mã môn học | ${metadata.courseCode || ''} |\n`;
  markdown += `| Lớp | ${metadata.class || ''} |\n`;
  markdown += `| Học kỳ | ${metadata.semester || ''} |\n`;
  markdown += `| Tên bài tập / Project | ${metadata.name || ''} |\n`;
  markdown += `| Tên sinh viên / Nhóm | ${members.map(m => m.name).join(', ') || ''} |\n`;
  markdown += `| MSSV / Danh sách MSSV | ${members.map(m => m.studentId).join(', ') || ''} |\n`;
  markdown += `| Giảng viên hướng dẫn | ${metadata.lecturer || ''} |\n`;
  markdown += `| Ngày hoàn thành reflection | ${new Date().toISOString().split('T')[0]} |\n\n---\n\n`;

  markdown += `## 2. Mục đích Reflection\n\nFile này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập...\n\n---\n\n`;

  markdown += `## 3. Tóm tắt quá trình sử dụng AI\n\n\`\`\`text\n${reflection.summaryText || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 4. Công cụ AI đã sử dụng\n\n`;
  const toolsList = ["ChatGPT", "Gemini", "Claude", "GitHub Copilot", "Cursor", "Antigravity", "Microsoft Copilot", "Perplexity"];
  toolsList.forEach(tool => {
    markdown += `- [${reflection.toolsUsed?.includes(tool) ? 'x' : ' '}] ${tool}\n`;
  });
  markdown += `- [ ] Công cụ khác: ....................................\n\n`;
  markdown += `### Công cụ được sử dụng nhiều nhất\n\n\`\`\`text\n${reflection.mostUsedTool || ' '}\n\`\`\`\n\n`;
  markdown += `### Lý do sử dụng công cụ đó\n\n\`\`\`text\n${reflection.mostUsedReason || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 5. AI đã hỗ trợ em/nhóm ở điểm nào?\n\n`;
  const supportAreaOptions = ["Hiểu yêu cầu đề bài", "Phân tích bài toán", "Tìm ý tưởng giải pháp", "Thiết kế database", "Thiết kế giao diện", "Thiết kế kiến trúc hệ thống", "Viết code mẫu", "Debug lỗi", "Viết test case", "Review code", "Tối ưu code", "Kiểm tra bảo mật", "Viết báo cáo", "Chuẩn bị thuyết trình", "Tìm hiểu công nghệ mới"];
  supportAreaOptions.forEach(area => {
    markdown += `- [${reflection.supportAreas?.includes(area) ? 'x' : ' '}] ${area}\n`;
  });
  markdown += `\n### Mô tả chi tiết\n\n\`\`\`text\n${reflection.supportDetails || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 6. AI có giúp em/nhóm học tốt hơn không?\n\n`;
  markdown += `### 6.1. Những điểm AI giúp em/nhóm học tốt hơn\n\n\`\`\`text\n${reflection.helpfulPoints || ' '}\n\`\`\`\n\n`;
  markdown += `### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn\n\n\`\`\`text\n${reflection.unhelpfulPoints || ' '}\n\`\`\`\n\n`;
  
  markdown += `### 6.3. Em/nhóm có bị phụ thuộc vào AI không?\n\n`;
  const dLevels = ["Không phụ thuộc", "Phụ thuộc ít", "Phụ thuộc trung bình", "Phụ thuộc nhiều"];
  dLevels.forEach(lvl => {
    markdown += `- [${reflection.dependencyLevel === lvl ? 'x' : ' '}] ${lvl}\n`;
  });
  markdown += `\nGiải thích:\n\n\`\`\`text\n${reflection.dependencyReason || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?\n\n`;
  const vMethods = ["Chạy thử chương trình", "Kiểm tra output", "Viết test case", "So sánh với yêu cầu đề bài", "Đối chiếu với tài liệu môn học", "Review code", "Hỏi lại giảng viên", "Tra cứu tài liệu chính thống", "Thảo luận với thành viên nhóm", "Kiểm tra bằng dữ liệu mẫu", "So sánh trước và sau khi dùng AI"];
  vMethods.forEach(m => {
    markdown += `- [${reflection.verificationMethods?.includes(m) ? 'x' : ' '}] ${m}\n`;
  });
  markdown += `\n### Mô tả quá trình kiểm chứng\n\n\`\`\`text\n${reflection.verificationDescription || ' '}\n\`\`\`\n\n`;
  markdown += `### Ví dụ cụ thể về một lần kiểm chứng\n\n| Nội dung | Mô tả |\n|---|---|\n`;
  markdown += `| AI đã gợi ý gì? | ${reflection.verificationExample?.aiSuggestion || ' '} |\n`;
  markdown += `| Em/nhóm đã kiểm tra bằng cách nào? | ${reflection.verificationExample?.checkMethod || ' '} |\n`;
  markdown += `| Kết quả kiểm tra | ${reflection.verificationExample?.result || ' '} |\n`;
  markdown += `| Em/nhóm đã xử lý tiếp như thế nào? | ${reflection.verificationExample?.followUp || ' '} |\n\n---\n\n`;

  markdown += `## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp\n\n`;
  if (!reflection.wrongSuggestions || reflection.wrongSuggestions.length === 0) {
    markdown += `\`\`\`text\nTrong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.\n\`\`\`\n\n`;
  } else {
    markdown += `| Nội dung | Mô tả |\n|---|---|\n`;
    reflection.wrongSuggestions.forEach(w => {
      markdown += `| AI đã gợi ý gì? | ${w.suggestion} |\n`;
      markdown += `| Vì sao gợi ý đó sai/chưa phù hợp? | ${w.reason} |\n`;
      markdown += `| Em/nhóm phát hiện bằng cách nào? | ${w.detectionMethod} |\n`;
      markdown += `| Em/nhóm đã sửa như thế nào? | ${w.fixMethod} |\n`;
      markdown += `| Bài học rút ra | ${w.lesson} |\n`;
    });
  }
  markdown += `---\n\n`;

  markdown += `## 9. Phần đóng góp thật sự của sinh viên/nhóm\n\n\`\`\`text\n${reflection.realContributionText || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 10. So sánh trước và sau khi dùng AI\n\n`;
  markdown += `| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |\n|---|---|---|---|\n`;
  (reflection.beforeAfter || []).forEach(ba => {
    markdown += `| ${ba.area} | ${ba.before} | ${ba.after} | ${ba.improvement} |\n`;
  });
  markdown += `\n---\n\n`;

  markdown += `## 11. Bài học về môn học\n\n\`\`\`text\n${reflection.lessonsLearnedText || ' '}\n\`\`\`\n\n---\n\n`;
  markdown += `## 12. Bài học về sử dụng AI có trách nhiệm\n\n\`\`\`text\n${reflection.responsibilityLessonsText || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 13. Điều em/nhóm sẽ không làm khi sử dụng AI\n\n`;
  const commitments = ["Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.", "Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.", "Không che giấu việc sử dụng AI trong các phần quan trọng.", "Không dùng AI để tạo nội dung sai lệch hoặc gian lận.", "Không dùng AI thay thế hoàn toàn quá trình học.", "Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên."];
  commitments.forEach(c => {
    markdown += `- [${reflection.commitments?.includes(c) ? 'x' : ' '}] ${c}\n`;
  });
  markdown += `\n### Giải thích thêm nếu có\n\n\`\`\`text\n${reflection.commitmentExplanation || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 14. Kế hoạch cải thiện lần sau\n\n\`\`\`text\n${reflection.improvementPlanText || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 15. Tự đánh giá mức độ hoàn thành\n\n| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |\n|---|:---:|---|\n`;
  (reflection.selfEvaluation || []).forEach(e => {
    markdown += `| ${e.criteria} | ${e.score} | ${e.notes || ' '} |\n`;
  });
  markdown += `\n---\n\n`;

  markdown += `## 16. Câu hỏi tự vấn cuối bài\n\n`;
  markdown += `### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?\n\n\`\`\`text\n${reflection.finalQuestions?.explainable || ' '}\n\`\`\`\n\n`;
  markdown += `### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?\n\n\`\`\`text\n${reflection.finalQuestions?.canReproduce || ' '}\n\`\`\`\n\n`;
  markdown += `### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?\n\n\`\`\`text\n${reflection.finalQuestions?.coreCompetency || ' '}\n\`\`\`\n\n`;
  markdown += `### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?\n\n\`\`\`text\n${reflection.finalQuestions?.desiredSkill || ' '}\n\`\`\`\n\n---\n\n`;

  markdown += `## 17. Cam kết Reflection\n\n| Đại diện sinh viên/nhóm | Ngày xác nhận |\n|---|---|\n| ${members[0]?.name || ' '} | ${new Date().toLocaleDateString('vi-VN')} |\n`;

  return markdown;
};
