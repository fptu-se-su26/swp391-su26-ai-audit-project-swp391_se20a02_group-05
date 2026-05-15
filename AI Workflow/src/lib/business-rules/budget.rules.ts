export const BUDGET_RULES = {
  name: "budget-rules",
  priority: 4,
  rules: [
    "Total trip cost must not exceed the user's stated budget by more than 5%",
    "Hotel/accommodation cost should not exceed 40% of total budget for non-luxury trips",
    "Daily activity cost must be realistic for the destination country",
    "Luxury hotels (5-star) cannot appear in budget-level plans",
    "Budget hostels and guesthouses cannot appear in luxury-level plans",
    "All costs must be in USD unless the user specifies another currency",
    "Each day's spending must be distributed realistically across meals, activities, and transport",
    "Include a 5-10% miscellaneous buffer in the budget breakdown",
    "Free activities should have cost: 0, never negative values",
    "Airport transfer costs must be included in the transportation budget",
  ],
};
