import { create } from "zustand";
import { TravelPlanResponse } from "@/types";

interface TravelStore {
  currentPlan: TravelPlanResponse | null;
  setPlan: (plan: TravelPlanResponse) => void;
  clearPlan: () => void;
}

export const useTravelStore = create<TravelStore>((set) => ({
  currentPlan: null,
  setPlan: (plan) => set({ currentPlan: plan }),
  clearPlan: () => set({ currentPlan: null }),
}));
