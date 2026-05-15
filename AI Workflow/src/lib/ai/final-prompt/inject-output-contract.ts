// ============================================================================
// Inject Output Contract — Strict JSON schema the AI must follow
// ============================================================================

export function injectOutputContract(): string {
  return `=== OUTPUT JSON CONTRACT ===
You MUST return a JSON object that EXACTLY matches this schema:

{
  "id": "string (UUID format, e.g. '550e8400-e29b-41d4-a716-446655440000')",
  "destination": "string (must match user's requested destination)",
  "summary": "string (2-3 sentence overview, minimum 10 characters)",
  "estimatedCost": "number (total in USD, non-negative)",
  "budgetSummary": {
    "accommodation": "number (USD, non-negative)",
    "food": "number (USD, non-negative)",
    "activities": "number (USD, non-negative)",
    "transport": "number (USD, non-negative)",
    "misc": "number (USD, non-negative)"
  },
  "days": [
    {
      "day": "number (sequential starting from 1)",
      "date": "string (optional, YYYY-MM-DD format)",
      "title": "string (unique, descriptive day title)",
      "summary": "string (brief day overview)",
      "activities": [
        {
          "id": "string (unique activity ID)",
          "time": "string (HH:MM 24-hour format)",
          "title": "string (activity name)",
          "description": "string (activity details)",
          "location": "string (specific location)",
          "cost": "number (USD, non-negative, 0 for free activities)",
          "type": "enum: 'activity' | 'food' | 'transport' | 'hotel'"
        }
      ]
    }
  ],
  "transportation": ["string (list of recommended transport options)"],
  "hotels": [
    {
      "name": "string (real hotel name)",
      "pricePerNight": "number (USD, non-negative)",
      "rating": "number (0-5, one decimal place)"
    }
  ],
  "foodRecommendations": [
    {
      "name": "string (real restaurant/food place name)",
      "type": "string (cuisine type)",
      "priceRange": "string ($ to $$$$)"
    }
  ]
}

=== ZOD VALIDATION CONTRACT ===
- All string fields: minimum 1 character unless marked optional
- All number fields: minimum 0
- "days" array: minimum 1 element
- "activities" per day: minimum 1 element
- "time" field: must match /^\\d{2}:\\d{2}$/ regex
- "rating" field: 0 <= rating <= 5
- "type" field: must be exactly one of: "activity", "food", "transport", "hotel"
- "id" fields: must be unique across the entire response

=== STRICT RULES ===
- Return ONLY the raw JSON object
- Do NOT wrap in markdown code blocks
- Do NOT include any text outside the JSON
- Do NOT omit any required field
- Do NOT use null for required fields
- ALL currency values must be in USD
- ALL times must be in 24-hour HH:MM format`;
}
