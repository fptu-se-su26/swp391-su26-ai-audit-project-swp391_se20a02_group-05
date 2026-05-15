// ============================================================================
// Preference Memory — Persists user preferences across sessions
// ============================================================================

import { create } from "zustand";
import { persist } from "zustand/middleware";

interface UserPreferences {
  favoriteDestinations: string[];
  preferredBudget: string;
  preferredStyles: string[];
  dislikedActivities: string[];
  dietaryRestrictions: string[];
  accessibilityNeeds: string[];
  preferredAccommodationType: string | null;
  travelHistory: { destination: string; date: string }[];
}

interface PreferenceMemoryStore extends UserPreferences {
  addFavoriteDestination: (dest: string) => void;
  setPreferredBudget: (budget: string) => void;
  addPreferredStyle: (style: string) => void;
  addDislikedActivity: (activity: string) => void;
  addDietaryRestriction: (restriction: string) => void;
  addTravelHistory: (destination: string) => void;
  getContext: () => string;
  reset: () => void;
}

const initialPrefs: UserPreferences = {
  favoriteDestinations: [],
  preferredBudget: "moderate",
  preferredStyles: [],
  dislikedActivities: [],
  dietaryRestrictions: [],
  accessibilityNeeds: [],
  preferredAccommodationType: null,
  travelHistory: [],
};

export const usePreferenceMemory = create<PreferenceMemoryStore>()(
  persist(
    (set, get) => ({
      ...initialPrefs,

      addFavoriteDestination: (dest) =>
        set((s) => ({
          favoriteDestinations: [...new Set([...s.favoriteDestinations, dest])],
        })),

      setPreferredBudget: (budget) => set({ preferredBudget: budget }),

      addPreferredStyle: (style) =>
        set((s) => ({
          preferredStyles: [...new Set([...s.preferredStyles, style])],
        })),

      addDislikedActivity: (activity) =>
        set((s) => ({
          dislikedActivities: [...new Set([...s.dislikedActivities, activity])],
        })),

      addDietaryRestriction: (restriction) =>
        set((s) => ({
          dietaryRestrictions: [...new Set([...s.dietaryRestrictions, restriction])],
        })),

      addTravelHistory: (destination) =>
        set((s) => ({
          travelHistory: [
            ...s.travelHistory,
            { destination, date: new Date().toISOString().split("T")[0] },
          ],
        })),

      getContext: () => {
        const prefs = get();
        const lines: string[] = ["USER PREFERENCE MEMORY:"];

        if (prefs.favoriteDestinations.length > 0)
          lines.push(`Favorite destinations: ${prefs.favoriteDestinations.join(", ")}`);
        if (prefs.preferredStyles.length > 0)
          lines.push(`Preferred travel styles: ${prefs.preferredStyles.join(", ")}`);
        if (prefs.dislikedActivities.length > 0)
          lines.push(`AVOID these activities: ${prefs.dislikedActivities.join(", ")}`);
        if (prefs.dietaryRestrictions.length > 0)
          lines.push(`Dietary restrictions: ${prefs.dietaryRestrictions.join(", ")}`);
        if (prefs.accessibilityNeeds.length > 0)
          lines.push(`Accessibility needs: ${prefs.accessibilityNeeds.join(", ")}`);
        if (prefs.travelHistory.length > 0)
          lines.push(`Past trips: ${prefs.travelHistory.map((t) => t.destination).join(", ")}`);

        return lines.join("\n");
      },

      reset: () => set(initialPrefs),
    }),
    {
      name: "travel-planner-preferences",
    }
  )
);
