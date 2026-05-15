// ============================================================================
// Normalize Plan — Response Normalization
// Ensures consistent formatting of the complete travel plan
// ============================================================================

import { ValidatedTravelPlan } from "@/lib/ai/schema/travel-plan.schema";
import { normalizeBudget } from "./normalize-budget";
import { normalizeHotel } from "./normalize-hotel";

/**
 * Normalize and clean the entire travel plan response.
 */
export function normalizePlan(plan: ValidatedTravelPlan): ValidatedTravelPlan {
  const normalized = { ...plan };

  // Normalize budget
  normalized.budgetSummary = normalizeBudget(normalized.budgetSummary);
  normalized.estimatedCost = Math.round(normalized.estimatedCost * 100) / 100;

  // Normalize hotels
  normalized.hotels = normalized.hotels.map(normalizeHotel);

  // Normalize days
  normalized.days = normalized.days.map((day, idx) => ({
    ...day,
    day: idx + 1, // Ensure sequential
    activities: day.activities
      .map((act) => ({
        ...act,
        cost: Math.round(act.cost * 100) / 100,
        time: normalizeTime(act.time),
        title: act.title.trim(),
        description: act.description.trim(),
        location: act.location.trim(),
      }))
      // Sort activities by time
      .sort((a, b) => a.time.localeCompare(b.time))
      // Remove duplicates by title within same day
      .filter((act, i, arr) => i === arr.findIndex((a) => a.title === act.title)),
  }));

  // Normalize transportation — deduplicate
  normalized.transportation = [...new Set(normalized.transportation.map((t) => t.trim()))];

  // Normalize food recommendations — deduplicate
  const seenFoods = new Set<string>();
  normalized.foodRecommendations = normalized.foodRecommendations.filter((f) => {
    const key = f.name.toLowerCase().trim();
    if (seenFoods.has(key)) return false;
    seenFoods.add(key);
    return true;
  });

  return normalized;
}

/**
 * Normalize a time string to HH:MM format.
 */
function normalizeTime(time: string): string {
  // Already in correct format
  if (/^\d{2}:\d{2}$/.test(time)) return time;

  // Handle "9:00" → "09:00"
  if (/^\d{1}:\d{2}$/.test(time)) return `0${time}`;

  // Handle "9:00 AM" → "09:00"
  const match12 = time.match(/^(\d{1,2}):(\d{2})\s*(AM|PM)$/i);
  if (match12) {
    let hours = parseInt(match12[1]);
    const minutes = match12[2];
    const period = match12[3].toUpperCase();
    if (period === "PM" && hours !== 12) hours += 12;
    if (period === "AM" && hours === 12) hours = 0;
    return `${hours.toString().padStart(2, "0")}:${minutes}`;
  }

  // Fallback — return as-is
  return time;
}
