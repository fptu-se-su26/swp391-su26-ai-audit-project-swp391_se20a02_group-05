export const OPTIMIZATION_RULES = {
  name: "optimization-rules",
  priority: 6,
  rules: [
    "Minimize unnecessary travel between locations by clustering nearby activities",
    "Suggest the most time-efficient routes between activities",
    "Prioritize activities with highest user satisfaction for the selected travel style",
    "Balance the daily schedule — avoid overloading mornings or evenings",
    "Suggest free alternatives when the budget is tight",
    "Include early morning activities for travelers who specified 'Adventure' style",
    "Include late-night activities only if user selected 'Nightlife' style",
    "Optimize hotel location to minimize daily commute to activity clusters",
  ],
};
