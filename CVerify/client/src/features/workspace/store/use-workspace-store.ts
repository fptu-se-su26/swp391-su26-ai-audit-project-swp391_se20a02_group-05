import { create } from 'zustand';
import { type WorkspaceDetails, type LinkedOrganization } from '../types/workspace.types';
import { workspaceService } from '../services/workspace.service';

interface WorkspaceState {
  workspaces: Record<string, WorkspaceDetails>;
  loading: Record<string, boolean>;
  errors: Record<string, string | null>;
  myOrganizations: LinkedOrganization[] | null;
  fetchWorkspace: (slug: string) => Promise<WorkspaceDetails | null>;
  fetchMyOrganizations: () => Promise<LinkedOrganization[] | null>;
<<<<<<< Updated upstream
=======
  updateWorkspaceDetails: (slug: string, updates: Partial<WorkspaceDetails>) => Promise<boolean>;
  toggleFollowWorkspace: (slug: string) => void;
>>>>>>> Stashed changes
  invalidateCache: (slug?: string) => void;
}

export const useWorkspaceStore = create<WorkspaceState>((set, get) => ({
  workspaces: {},
  loading: {},
  errors: {},
  myOrganizations: null,
  fetchMyOrganizations: async () => {
    try {
      const orgs = await workspaceService.getUserOrganizations();
      set({ myOrganizations: orgs });
      return orgs;
    } catch (err) {
      console.error('[Workspace Store] Failed to fetch user organizations', err);
      return null;
    }
  },
  fetchWorkspace: async (slug: string) => {
    const cached = get().workspaces[slug];
    if (cached) {
      // Trigger background refetch for consistency & freshness without UI blocking
      workspaceService.getWorkspaceDetails(slug)
        .then((details) => {
          set((state) => ({
            workspaces: { ...state.workspaces, [slug]: details }
          }));
        })
        .catch((err) => {
          console.warn('[Workspace Store] Background details refetch failed', err);
        });
      return cached;
    }

    set((state) => ({
      loading: { ...state.loading, [slug]: true },
      errors: { ...state.errors, [slug]: null }
    }));

    try {
      const details = await workspaceService.getWorkspaceDetails(slug);
      set((state) => ({
        workspaces: { ...state.workspaces, [slug]: details },
        loading: { ...state.loading, [slug]: false }
      }));
      return details;
    } catch (err: any) {
      const errMsg = err?.response?.data?.message || err?.message || 'Failed to load workspace';
      set((state) => ({
        errors: { ...state.errors, [slug]: errMsg },
        loading: { ...state.loading, [slug]: false }
      }));
      return null;
    }
  },
<<<<<<< Updated upstream
=======

  updateWorkspaceDetails: async (slug: string, updates: Partial<WorkspaceDetails>) => {
    try {
      const details = await workspaceService.updateWorkspaceDetails(slug, updates);
      set((state) => ({
        workspaces: {
          ...state.workspaces,
          [slug]: {
            ...state.workspaces[slug],
            ...details,
          }
        }
      }));
      return true;
    } catch (err) {
      console.error('[Workspace Store] Failed to update workspace details', err);
      return false;
    }
  },

  toggleFollowWorkspace: (slug: string) => {
    set((state) => {
      const current = state.workspaces[slug];
      if (!current) return state;
      const isFollowing = !current.isFollowing;
      const followersCount = (current.followersCount ?? 0) + (isFollowing ? 1 : -1);
      return {
        workspaces: {
          ...state.workspaces,
          [slug]: {
            ...current,
            isFollowing,
            followersCount,
          }
        }
      };
    });
  },

>>>>>>> Stashed changes
  invalidateCache: (slug?: string) => {
    if (slug) {
      set((state) => {
        const { [slug]: _, ...restWorkspaces } = state.workspaces;
        return { workspaces: restWorkspaces };
      });
    } else {
      set({ workspaces: {} });
    }
  }
}));
