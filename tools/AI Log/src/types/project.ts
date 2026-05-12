export interface ProjectMetadata {
  name: string;
  course: string;
  courseCode: string;
  class: string;
  semester: string;
  lecturer: string;
  repoUrl: string;
  startDate: string;
  endDate: string;
}

export interface ProjectMember {
  id: string;
  name: string;
  studentId: string;
}

export interface ChangelogEntry {
  id: string;
  phaseName: string;
  date: string;
  status: 'Not Started' | 'In Progress' | 'Completed';
  completedChecklist: string[];
  changes: Array<{ id: string; content: string; author: string; files: string; evidence: string }>;
  aiSupport: { used: boolean; description: string };
  evidenceLink: string;
  notes: string;
}

export interface PromptLogEntry {
  id: string;
  date: string;
  aiTool: string;
  purpose: string;
  category: string; 
  usageLevel: string; 
  promptText: string;
  context: string;
  aiResponse: string;
  appliedResult: string;
  improvements: string;
  evaluationChecklist: string[];
  evidence: Array<{ id: string; type: string; content: string }>;
  notes: string;
  isMostImportant: boolean; 
  isIneffective: boolean; 
  ineffectiveDetails?: { reason: string; improvementMethod: string; improvedPrompt: string; newResult: string };
}

export interface PromptLessons {
  infoNeeded: string;
  lessonsLearned: string;
  futureImprovements: string;
}

export interface AiAuditLogEntry {
  id: string;
  date: string;
  aiTool: string;
  purpose: string;
  category: string;
  usageLevel: string;
  prompt: string;
  aiResponseSummary: string;
  usedContent: string;
  modifications: string;
  evidence: Array<{ id: string; type: string; content: string }>;
  lessonsLearned: string;
}

export interface AiAuditData {
  toolsUsed: string[];
  usageTargetsText: string;
  auditEntries: AiAuditLogEntry[];
  usageMatrix: Array<{ id: string; category: string; usageLevel: string; notes: string }>;
  issues: Array<{ id: string; description: string; detectionMethod: string; resolution: string }>;
  verificationMethodsText: string;
  personalContributionText: string;
  groupContributions: Array<{ id: string; memberName: string; memberId: string; tasks: string; aiUsed: boolean; evidence: string }>;
}

export interface ReflectionData {
  summaryText: string;
  toolsUsed: string[];
  mostUsedTool: string;
  mostUsedReason: string;
  supportAreas: string[];
  supportDetails: string;
  helpfulPoints: string;
  unhelpfulPoints: string;
  dependencyLevel: string;
  dependencyReason: string;
  verificationMethods: string[];
  verificationDescription: string;
  verificationExample: { aiSuggestion: string; checkMethod: string; result: string; followUp: string };
  wrongSuggestions: Array<{ id: string; suggestion: string; reason: string; detectionMethod: string; fixMethod: string; lesson: string }>;
  realContributionText: string;
  beforeAfter: Array<{ id: string; area: string; before: string; after: string; improvement: string }>;
  lessonsLearnedText: string;
  responsibilityLessonsText: string;
  commitments: string[];
  commitmentExplanation: string;
  improvementPlanText: string;
  selfEvaluation: Array<{ id: string; criteria: string; score: number; notes: string }>;
  finalQuestions: { explainable: string; canReproduce: string; coreCompetency: string; desiredSkill: string };
}

export interface Project {
  id: string;
  createdAt: string;
  updatedAt: string;
  metadata: ProjectMetadata;
  members: ProjectMember[];
  changelogs: ChangelogEntry[];
  prompts: PromptLogEntry[];
  promptLessons: PromptLessons;
  aiAudit: AiAuditData;
  reflection: ReflectionData;
}
