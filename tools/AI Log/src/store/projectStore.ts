import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { 
  Project, ProjectMetadata, ProjectMember, ChangelogEntry, 
  PromptLogEntry, PromptLessons, AiAuditData, ReflectionData, 
  ChangelogSummary 
} from '@/types/project';
import { 
  saveProjectToFile, saveProjectAs as saveProjectAsFile, 
  openProjectFile, getPersistenceMode, migrateProject,
  type PersistenceMode 
} from '@/lib/fileService';
import { DEFAULT_PROJECT_METADATA, DEFAULT_PROJECT_MEMBERS } from '@/lib/defaults';

interface ProjectState {
  projects: Record<string, Project>;
  activeProjectId: string | null;
  fileHandles: Record<string, FileSystemFileHandle | null>;
  lastSavedAt: Record<string, string | null>;
  persistenceMode: PersistenceMode;
  
  // Core Actions
  createProject: (metadata: Partial<ProjectMetadata> & { name: string; courseCode: string }) => string;
  deleteProject: (id: string) => void;
  setActiveProject: (id: string) => void;
  importProject: (projectData: Project) => void;
  
  // File Actions
  saveProject: (id: string) => Promise<boolean>;
  saveProjectAs: (id: string) => Promise<boolean>;
  openProject: () => Promise<string | null>;
  setFileHandle: (id: string, handle: FileSystemFileHandle | null) => void;
  
  // Update Actions for the active project
  updateMetadata: (metadata: Partial<ProjectMetadata>) => void;
  updateMembers: (members: ProjectMember[]) => void;
  updateChangelogs: (changelogs: ChangelogEntry[]) => void;
  updateChangelogSummary: (summary: Partial<ChangelogSummary>) => void;
  updatePrompts: (prompts: PromptLogEntry[], lessons?: Partial<PromptLessons>) => void;
  updateAiAudit: (aiAudit: Partial<AiAuditData>) => void;
  updateReflection: (reflection: Partial<ReflectionData>) => void;
}

const generateId = () => Math.random().toString(36).substring(2, 9);

const initialAiAuditData: AiAuditData = {
  toolsUsed: [],
  usageTargetsText: '',
  auditEntries: [],
  usageMatrix: [],
  issues: [],
  verificationMethodsText: '',
  personalContributionText: '',
  groupContributions: [],
};

const initialReflectionData: ReflectionData = {
  summaryText: '',
  toolsUsed: [],
  mostUsedTool: '',
  mostUsedReason: '',
  supportAreas: [],
  supportDetails: '',
  helpfulPoints: '',
  unhelpfulPoints: '',
  dependencyLevel: 'Phụ thuộc ít',
  dependencyReason: 'Sử dụng AI để tối ưu hóa thời gian nghiên cứu và tạo cấu trúc ban đầu.',
  verificationMethods: [],
  verificationDescription: '',
  verificationExample: { aiSuggestion: '', checkMethod: '', result: '', followUp: '' },
  wrongSuggestions: [],
  realContributionText: '',
  beforeAfter: [],
  lessonsLearnedText: '',
  responsibilityLessonsText: '',
  commitments: [
    "Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.",
    "Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.",
    "Không che giấu việc sử dụng AI trong các phần quan trọng.",
    "Không dùng AI để tạo nội dung sai lệch hoặc gian lận.",
    "Không dùng AI thay thế hoàn toàn quá trình học.",
    "Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên."
  ],
  commitmentExplanation: '',
  improvementPlanText: '',
  selfEvaluation: [],
  finalQuestions: { 
    explainable: 'Có, nhóm đã đọc, kiểm tra và hiểu nội dung trước khi sử dụng.', 
    canReproduce: 'Có, nhưng sẽ mất nhiều thời gian hơn để nghiên cứu và triển khai.', 
    coreCompetency: 'Phần thiết kế workflow, chỉnh sửa logic và xử lý lỗi thực tế.', 
    desiredSkill: 'Kỹ năng thiết kế hệ thống, viết prompt và kiểm thử phần mềm.' 
  },
};

const initialPromptLessons: PromptLessons = {
  infoNeeded: '',
  lessonsLearned: '',
  futureImprovements: '',
};

const initialChangelogSummary: ChangelogSummary = {
  completedFeatures: '',
  unfinishedFeatures: '',
  majorImprovements: '',
  overallSummary: '',
  futureImprovements: '',
};

export const useProjectStore = create<ProjectState>()(
  persist(
    (set, get) => ({
      projects: {},
      activeProjectId: null,
      fileHandles: {},
      lastSavedAt: {},
      persistenceMode: typeof window !== 'undefined' ? getPersistenceMode() : 'fallback',

      createProject: (metadata: Partial<ProjectMetadata> & { name: string; courseCode: string }) => {
        const id = generateId();
        const newProject: Project = {
          id,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          metadata: { ...DEFAULT_PROJECT_METADATA, ...metadata } as ProjectMetadata,
          members: DEFAULT_PROJECT_MEMBERS,
          changelogs: [],
          changelogSummary: initialChangelogSummary,
          prompts: [],
          promptLessons: initialPromptLessons,
          aiAudit: initialAiAuditData,
          reflection: initialReflectionData,
        };
        
        set((state) => ({
          projects: { ...state.projects, [id]: newProject },
          activeProjectId: id,
        }));
        return id;
      },

      deleteProject: (id) => {
        set((state) => {
          // Create new copies without the deleted key
          const newProjects = { ...state.projects };
          delete newProjects[id];
          const newHandles = { ...state.fileHandles };
          delete newHandles[id];
          const newSavedAt = { ...state.lastSavedAt };
          delete newSavedAt[id];
          return {
            projects: newProjects,
            fileHandles: newHandles,
            lastSavedAt: newSavedAt,
            activeProjectId: state.activeProjectId === id ? null : state.activeProjectId,
          };
        });
      },

      setActiveProject: (id) => {
        set({ activeProjectId: id });
      },

      importProject: (projectData) => {
        const migrated = migrateProject(projectData);
        set((state) => ({
          projects: { ...state.projects, [migrated.id]: migrated }
        }));
      },

      // ─── File Actions ─────────────────────────────────────

      saveProject: async (id) => {
        const { projects, fileHandles } = get();
        const project = projects[id];
        if (!project) return false;

        try {
          const handle = await saveProjectToFile(project, fileHandles[id]);
          set((state) => ({
            fileHandles: { ...state.fileHandles, [id]: handle },
            lastSavedAt: { ...state.lastSavedAt, [id]: new Date().toISOString() },
          }));
          return true;
        } catch (err) {
          console.error("Failed to save project:", err);
          return false;
        }
      },

      saveProjectAs: async (id) => {
        const { projects } = get();
        const project = projects[id];
        if (!project) return false;

        try {
          const handle = await saveProjectAsFile(project);
          if (handle) {
            set((state) => ({
              fileHandles: { ...state.fileHandles, [id]: handle },
              lastSavedAt: { ...state.lastSavedAt, [id]: new Date().toISOString() },
            }));
          } else {
            // Fallback mode or cancelled - still mark as saved if content was downloaded
            set((state) => ({
              lastSavedAt: { ...state.lastSavedAt, [id]: new Date().toISOString() },
            }));
          }
          return true;
        } catch (err) {
          console.error("Failed to save project:", err);
          return false;
        }
      },

      openProject: async () => {
        try {
          const result = await openProjectFile();
          if (!result) return null;

          const { project, handle } = result;
          set((state) => ({
            projects: { ...state.projects, [project.id]: project },
            activeProjectId: project.id,
            fileHandles: { ...state.fileHandles, [project.id]: handle },
            lastSavedAt: { ...state.lastSavedAt, [project.id]: new Date().toISOString() },
          }));
          return project.id;
        } catch (err) {
          console.error("Failed to open project:", err);
          throw err;
        }
      },

      setFileHandle: (id, handle) => {
        set((state) => ({
          fileHandles: { ...state.fileHandles, [id]: handle },
        }));
      },

      // ─── Update Actions ───────────────────────────────────

      updateMetadata: (metadata) => {
        const { activeProjectId, projects } = get();
        if (!activeProjectId || !projects[activeProjectId]) return;

        set((state) => ({
          projects: {
            ...state.projects,
            [activeProjectId]: {
              ...state.projects[activeProjectId],
              metadata: { ...state.projects[activeProjectId].metadata, ...metadata },
              updatedAt: new Date().toISOString(),
            }
          }
        }));
      },

      updateMembers: (members) => {
        const { activeProjectId, projects } = get();
        if (!activeProjectId || !projects[activeProjectId]) return;

        set((state) => ({
          projects: {
            ...state.projects,
            [activeProjectId]: {
              ...state.projects[activeProjectId],
              members,
              updatedAt: new Date().toISOString(),
            }
          }
        }));
      },

      updateChangelogs: (changelogs) => {
        const { activeProjectId, projects } = get();
        if (!activeProjectId || !projects[activeProjectId]) return;

        set((state) => ({
          projects: {
            ...state.projects,
            [activeProjectId]: {
              ...state.projects[activeProjectId],
              changelogs,
              updatedAt: new Date().toISOString(),
            }
          }
        }));
      },

      updateChangelogSummary: (summary) => {
        const { activeProjectId, projects } = get();
        if (!activeProjectId || !projects[activeProjectId]) return;

        set((state) => ({
          projects: {
            ...state.projects,
            [activeProjectId]: {
              ...state.projects[activeProjectId],
              changelogSummary: { ...state.projects[activeProjectId].changelogSummary, ...summary },
              updatedAt: new Date().toISOString(),
            }
          }
        }));
      },

      updatePrompts: (prompts, lessons) => {
        const { activeProjectId, projects } = get();
        if (!activeProjectId || !projects[activeProjectId]) return;

        set((state) => ({
          projects: {
            ...state.projects,
            [activeProjectId]: {
              ...state.projects[activeProjectId],
              prompts,
              ...(lessons ? { promptLessons: { ...state.projects[activeProjectId].promptLessons, ...lessons } } : {}),
              updatedAt: new Date().toISOString(),
            }
          }
        }));
      },

      updateAiAudit: (aiAudit) => {
        const { activeProjectId, projects } = get();
        if (!activeProjectId || !projects[activeProjectId]) return;

        set((state) => ({
          projects: {
            ...state.projects,
            [activeProjectId]: {
              ...state.projects[activeProjectId],
              aiAudit: { ...state.projects[activeProjectId].aiAudit, ...aiAudit },
              updatedAt: new Date().toISOString(),
            }
          }
        }));
      },

      updateReflection: (reflection) => {
        const { activeProjectId, projects } = get();
        if (!activeProjectId || !projects[activeProjectId]) return;

        set((state) => ({
          projects: {
            ...state.projects,
            [activeProjectId]: {
              ...state.projects[activeProjectId],
              reflection: { ...state.projects[activeProjectId].reflection, ...reflection },
              updatedAt: new Date().toISOString(),
            }
          }
        }));
      },
    }),
    {
      name: 'ai-workflow-logger-storage',
      // Only persist lightweight data — project data is cached here but
      // the .data.json file is the canonical source of truth.
      // fileHandles cannot be serialized, so we exclude them.
      partialize: (state) => ({
        projects: state.projects,
        activeProjectId: state.activeProjectId,
        lastSavedAt: state.lastSavedAt,
      }),
    }
  )
);
