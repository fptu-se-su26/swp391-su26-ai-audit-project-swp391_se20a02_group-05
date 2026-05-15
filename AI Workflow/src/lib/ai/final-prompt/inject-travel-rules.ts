// ============================================================================
// Inject Travel Rules — Destination-specific constraints
// ============================================================================

export function injectTravelRules(destination: string, budget: string): string {
  return [
    "=== TRAVEL CONSTRAINTS ===",
    `Target destination: ${destination}`,
    `Budget category: ${budget}`,
    "",
    "Constraints:",
    "- All suggested locations must actually exist at the destination",
    "- Respect local opening hours and seasonal availability",
    "- Consider weather patterns for outdoor activities",
    "- Account for local holidays and peak seasons",
    "- Use local currency equivalents converted to USD",
    "- Consider visa and entry requirements if applicable",
    "",
    "=== OPTIMIZATION RULES ===",
    "- Cluster geographically close activities together",
    "- Minimize transit time between activities",
    "- Balance high-energy and relaxing activities throughout the day",
    "- Place the most popular attractions early to avoid crowds",
    "- Distribute costs evenly across days when possible",
  ].join("\n");
}
