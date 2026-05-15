// ============================================================================
// Inject User Context — User preferences and personalization
// ============================================================================

import { TravelPlanRequest } from "@/types";

export function injectUserContext(request: TravelPlanRequest): string {
  return [
    "=== USER PREFERENCES ===",
    `Destination: ${request.destination}`,
    `Duration: ${request.durationDays} days`,
    `Travelers: ${request.travelers}`,
    `Budget Level: ${request.budget}`,
    `Travel Styles: ${request.travelStyle.join(", ")}`,
    request.additionalNotes ? `Special Notes: ${request.additionalNotes}` : "",
  ]
    .filter(Boolean)
    .join("\n");
}
