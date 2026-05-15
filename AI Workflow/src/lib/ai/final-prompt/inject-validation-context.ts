// ============================================================================
// Inject Validation Context — Runtime validation expectations
// ============================================================================

export function injectValidationContext(): string {
  return [
    "=== RESPONSE QUALITY RULES ===",
    "- Every activity must have realistic, locally-accurate pricing",
    "- Hotel names must be real hotels that exist at the destination",
    "- Restaurant names must be real restaurants that exist at the destination",
    "- Times must follow logical daily patterns (breakfast before lunch before dinner)",
    "- The total of budgetSummary fields should approximately equal estimatedCost",
    "- Activity descriptions should be informative and at least 20 characters",
    "",
    "=== FAILURE RECOVERY RULES ===",
    "- If you are unsure about a specific cost, provide your best realistic estimate",
    "- If you cannot find a real hotel name, use a plausible generic name with the city",
    "- If a field is required but you lack data, provide a reasonable default",
    "- NEVER leave required fields empty, null, or undefined",
    "- NEVER use placeholder text like 'TBD', 'N/A', or 'Lorem ipsum'",
  ].join("\n");
}
