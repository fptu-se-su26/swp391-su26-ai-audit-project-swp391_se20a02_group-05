import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { Project, ProjectMetadata, ProjectMember, ChangelogEntry, PromptLogEntry, PromptLessons, AiAuditData, ReflectionData } from '@/types/project';

interface ProjectState {
  projects: Record<string, Project>;
  activeProjectId: string | null;
  
  // Actions
  createProject: (metadata: ProjectMetadata) => string;
  deleteProject: (id: string) => void;
  setActiveProject: (id: string) => void;
  importProject: (projectData: Project) => void;
  
  // Update Actions for the active project
  updateMetadata: (metadata: Partial<ProjectMetadata>) => void;
  updateMembers: (members: ProjectMember[]) => void;
  updateChangelogs: (changelogs: ChangelogEntry[]) => void;
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
  dependencyLevel: '',
  dependencyReason: '',
  verificationMethods: [],
  verificationDescription: '',
  verificationExample: { aiSuggestion: '', checkMethod: '', result: '', followUp: '' },
  wrongSuggestions: [],
  realContributionText: '',
  beforeAfter: [],
  lessonsLearnedText: '',
  responsibilityLessonsText: '',
  commitments: [],
  commitmentExplanation: '',
  improvementPlanText: '',
  selfEvaluation: [],
  finalQuestions: { explainable: '', canReproduce: '', coreCompetency: '', desiredSkill: '' },
};

const initialPromptLessons: PromptLessons = {
  infoNeeded: '',
  lessonsLearned: '',
  futureImprovements: '',
};

export const useProjectStore = create<ProjectState>()(
  persist(
    (set, get) => ({
      projects: {},
      activeProjectId: null,

      createProject: (metadata) => {
        const id = generateId();
        const newProject: Project = {
          id,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          metadata,
          members: [],
          changelogs: [],
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
          const { [id]: _, ...rest } = state.projects;
          return {
            projects: rest,
            activeProjectId: state.activeProjectId === id ? null : state.activeProjectId,
          };
        });
      },

      setActiveProject: (id) => {
        set({ activeProjectId: id });
      },

      importProject: (projectData) => {
        set((state) => ({
          projects: { ...state.projects, [projectData.id]: projectData }
        }));
      },

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
    }
  )
);
