export const VALIDATION_RULES = {
  name: "validation-rules",
  priority: 2,
  rules: [
    "ALL responses must be valid JSON — no markdown, no prose, no explanation outside JSON",
    "Every required field in the schema MUST be present and non-null",
    "All time values must be in HH:MM 24-hour format",
    "All cost values must be non-negative numbers",
    "All arrays must contain at least one element unless explicitly optional",
    "Day numbers must be sequential starting from 1",
    "Activity IDs must be unique across the entire plan",
    "Hotel ratings must be between 0 and 5",
    "The estimatedCost must equal the sum of budgetSummary values (within 5% tolerance)",
    "Destination name must match the user's requested destination",
  ],
};
