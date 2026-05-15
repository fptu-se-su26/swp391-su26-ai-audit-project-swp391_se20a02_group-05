import { ProjectMetadata, ProjectMember } from "@/types/project";

export const DEFAULT_PROJECT_METADATA: ProjectMetadata = {
  name: "TripGenie",
  course: "Software Development Project",
  courseCode: "SWP391",
  class: "SE20A02",
  semester: "SU26",
  lecturer: "QuangLTN3",
  repoUrl: "https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05",
  startDate: "2026-05-11T00:00:00.000Z",
  endDate: "2026-07-19T00:00:00.000Z",
};

export const DEFAULT_PROJECT_MEMBERS: ProjectMember[] = [
  { id: "1", studentId: "DE200147", name: "Nguyễn Hoàng Ngọc Ánh" },
  { id: "2", studentId: "DE200523", name: "Đoàn Thế Lực" },
  { id: "3", studentId: "DE190105", name: "Trương Văn Hiếu" },
  { id: "4", studentId: "DE201043", name: "Nguyễn La Hòa An" },
  { id: "5", studentId: "DE200160", name: "Trần Nhất Long" },
];
