// ============================================================================
// Workflow Recovery — Auto-repair and retry logic for failed generations
// ============================================================================

import { generateWithRetry } from "@/lib/ai/google";
import { safeParseJson } from "@/lib/ai/security/validate-json";
import { logger } from "@/lib/logger";

/**
 * Build a repair prompt that tells the AI to fix specific validation errors.
 */
export function buildRepairPrompt(
  originalPrompt: string,
  invalidJson: string,
  errors: string[]
): string {
  return [
    "The previous AI response had validation errors. Fix them and return a corrected JSON.",
    "",
    "ERRORS FOUND:",
    ...errors.map((e, i) => `  ${i + 1}. ${e}`),
    "",
    "INVALID RESPONSE (fix this):",
    invalidJson.substring(0, 2000), // Truncate to avoid token explosion
    "",
    "ORIGINAL REQUIREMENTS:",
    originalPrompt.substring(0, 3000),
    "",
    "Return ONLY the corrected, complete JSON object. No markdown. No explanation.",
  ].join("\n");
}

/**
 * Attempt to auto-repair a failed AI response.
 */
export async function attemptRepair<T>(
  originalPrompt: string,
  invalidResponse: string,
  errors: string[],
  maxRepairAttempts = 2
): Promise<T | null> {
  for (let attempt = 1; attempt <= maxRepairAttempts; attempt++) {
    logger.workflow.step("Recovery", `Repair attempt ${attempt}/${maxRepairAttempts}`);

    try {
      const repairPrompt = buildRepairPrompt(originalPrompt, invalidResponse, errors);
      const repairedRaw = await generateWithRetry(repairPrompt, {
        temperature: 0.3, // Lower temperature for more deterministic repair
        maxRetries: 1,
      });

      const parsed = safeParseJson<T>(repairedRaw);
      if (parsed.valid && parsed.data) {
        logger.workflow.step("Recovery", `Repair succeeded on attempt ${attempt}`);
        return parsed.data;
      }

      // Use the new errors for next repair attempt
      invalidResponse = repairedRaw;
      errors = parsed.errors;
    } catch (error) {
      logger.workflow.error("Recovery", `Repair attempt ${attempt} failed: ${error}`);
    }
  }

  logger.workflow.error("Recovery", "All repair attempts exhausted");
  return null;
}

/**
 * Generate a safe fallback response when all else fails.
 */
export function generateFallbackPlan(destination: string, days: number): object {
  const activities = [];
  for (let d = 1; d <= days; d++) {
    activities.push({
      day: d,
      title: `Day ${d} in ${destination}`,
      summary: `Explore ${destination} on day ${d}`,
      activities: [
        {
          id: `fallback-${d}-1`,
          time: "09:00",
          title: "Morning Exploration",
          description: `Explore the local area of ${destination}`,
          location: destination,
          cost: 0,
          type: "activity",
        },
        {
          id: `fallback-${d}-2`,
          time: "12:00",
          title: "Local Lunch",
          description: "Enjoy local cuisine at a nearby restaurant",
          location: destination,
          cost: 15,
          type: "food",
        },
        {
          id: `fallback-${d}-3`,
          time: "14:00",
          title: "Afternoon Activity",
          description: `Visit a popular attraction in ${destination}`,
          location: destination,
          cost: 20,
          type: "activity",
        },
      ],
    });
  }

  return {
    id: crypto.randomUUID(),
    destination,
    summary: `A ${days}-day trip to ${destination}. This is a basic fallback plan — please regenerate for a personalized itinerary.`,
    estimatedCost: days * 100,
    budgetSummary: {
      accommodation: days * 40,
      food: days * 25,
      activities: days * 20,
      transport: days * 10,
      misc: days * 5,
    },
    days: activities,
    transportation: ["Local public transport"],
    hotels: [{ name: `${destination} Central Hotel`, pricePerNight: 40, rating: 3.5 }],
    foodRecommendations: [{ name: "Local Restaurant", type: "Local Cuisine", priceRange: "$$" }],
  };
}
