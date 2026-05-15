// ============================================================================
// Session Memory — Tracks current session state
// ============================================================================

import { create } from "zustand";

interface SessionState {
  sessionId: string;
  currentDestination: string | null;
  generationCount: number;
  approvedPlanIds: string[];
  rejectedReasons: string[];
  lastActivity: string | null;
}

interface SessionMemoryStore extends SessionState {
  setDestination: (dest: string) => void;
  incrementGeneration: () => void;
  approvePlan: (planId: string) => void;
  addRejection: (reason: string) => void;
  setLastActivity: (activity: string) => void;
  reset: () => void;
  getContext: () => string;
}

const initialState: SessionState = {
  sessionId: crypto.randomUUID(),
  currentDestination: null,
  generationCount: 0,
  approvedPlanIds: [],
  rejectedReasons: [],
  lastActivity: null,
};

export const useSessionMemory = create<SessionMemoryStore>((set, get) => ({
  ...initialState,

  setDestination: (dest) => set({ currentDestination: dest }),
  incrementGeneration: () => set((s) => ({ generationCount: s.generationCount + 1 })),
  approvePlan: (planId) => set((s) => ({ approvedPlanIds: [...s.approvedPlanIds, planId] })),
  addRejection: (reason) => set((s) => ({ rejectedReasons: [...s.rejectedReasons, reason] })),
  setLastActivity: (activity) => set({ lastActivity: activity }),
  reset: () => set(initialState),

  getContext: () => {
    const state = get();
    const lines: string[] = ["SESSION MEMORY:"];
    if (state.currentDestination) lines.push(`Current destination: ${state.currentDestination}`);
    lines.push(`Generation attempts this session: ${state.generationCount}`);
    if (state.rejectedReasons.length > 0) {
      lines.push(`User previously rejected plans for: ${state.rejectedReasons.join("; ")}`);
      lines.push(`IMPORTANT: Avoid repeating these issues in the new plan.`);
    }
    if (state.approvedPlanIds.length > 0) {
      lines.push(`User has approved ${state.approvedPlanIds.length} plan(s) this session`);
    }
    return lines.join("\n");
  },
}));
