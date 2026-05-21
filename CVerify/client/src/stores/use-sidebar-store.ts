"use client";

import { create } from 'zustand';

interface SidebarState {
  isCollapsed: boolean;
  isMobileOpen: boolean;
  expandedGroups: Record<string, boolean>;
  toggleCollapsed: () => void;
  setMobileOpen: (open: boolean) => void;
  toggleGroup: (id: string) => void;
  setGroupExpanded: (id: string, expanded: boolean) => void;
  resetMobile: () => void;
}

export const useSidebarStore = create<SidebarState>((set) => ({
  isCollapsed: false,
  isMobileOpen: false,
  expandedGroups: {},
  
  toggleCollapsed: () => set((state) => ({ isCollapsed: !state.isCollapsed })),
  
  setMobileOpen: (open) => set({ isMobileOpen: open }),
  
  toggleGroup: (id) =>
    set((state) => ({
      expandedGroups: {
        ...state.expandedGroups,
        [id]: !state.expandedGroups[id],
      },
    })),
    
  setGroupExpanded: (id, expanded) =>
    set((state) => ({
      expandedGroups: {
        ...state.expandedGroups,
        [id]: expanded,
      },
    })),
    
  resetMobile: () => set({ isMobileOpen: false }),
}));
