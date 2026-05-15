// ============================================================================
// Build User Prompt — Formats user request into structured prompt section
// ============================================================================

import { TravelPlanRequest } from "@/types";

export function buildUserPrompt(request: TravelPlanRequest): string {
  return [
    "=== CURRENT USER REQUEST ===",
    `Destination: ${request.destination}`,
    `Duration: ${request.durationDays} days`,
    `Number of Travelers: ${request.travelers}`,
    `Budget Level: ${request.budget}`,
    `Travel Styles: ${request.travelStyle.join(", ")}`,
    request.additionalNotes
      ? `Additional Requirements: ${request.additionalNotes}`
      : "",
    "",
    "Generate the travel plan NOW based on ALL the rules and constraints above.",
    "Return ONLY a raw JSON object. No markdown. No explanation.",
  ]
    .filter(Boolean)
    .join("\n");
}
