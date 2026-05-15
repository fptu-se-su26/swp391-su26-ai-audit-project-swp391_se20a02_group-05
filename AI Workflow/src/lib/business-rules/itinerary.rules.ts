export const ITINERARY_RULES = {
  name: "itinerary-rules",
  priority: 5,
  rules: [
    "Each day must have between 3-8 activities",
    "Activities must be ordered chronologically by time within each day",
    "The first day should start no earlier than check-in time if arriving",
    "The last day should end with airport/departure activities",
    "Allow at least 30 minutes travel time between locations",
    "Lunch should be scheduled between 11:00-14:00",
    "Dinner should be scheduled between 17:00-21:00",
    "Do not schedule outdoor activities during extreme heat hours (12:00-15:00) in tropical destinations",
    "Include at least one rest/downtime period per day",
    "Activities should be geographically clustered to minimize travel time",
    "Never schedule the same activity twice on the same day",
    "Day titles must be unique and descriptive",
  ],
};
