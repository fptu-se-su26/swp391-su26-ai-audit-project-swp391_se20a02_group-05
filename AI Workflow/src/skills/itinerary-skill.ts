// ============================================================================
// Itinerary Skill — Modular AI Skill
// Generates structured day-by-day itinerary data
// ============================================================================

export interface ItineraryInput {
  destination: string;
  durationDays: number;
  travelStyle: string[];
  travelers: number;
}

export interface ItineraryOutput {
  days: {
    day: number;
    title: string;
    summary: string;
    suggestedActivities: string[];
  }[];
}

export const generateItineraryContext = (input: ItineraryInput): string => {
  const styleContext = input.travelStyle.map((s) => {
    switch (s.toLowerCase()) {
      case "adventure": return "Include hiking, diving, zip-lining, or other adrenaline activities";
      case "relaxation": return "Include spa, beach, yoga, and scenic retreats";
      case "cultural": return "Include museums, temples, historical sites, and local workshops";
      case "foodie": return "Include food tours, cooking classes, market visits, and signature restaurants";
      case "nightlife": return "Include rooftop bars, clubs, night markets, and evening entertainment";
      case "nature": return "Include national parks, waterfalls, wildlife sanctuaries, and scenic trails";
      case "luxury": return "Include premium experiences, private tours, and exclusive venues";
      default: return `Include activities related to ${s}`;
    }
  });

  return [
    `ITINERARY SKILL CONTEXT:`,
    `Destination: ${input.destination}`,
    `Duration: ${input.durationDays} days`,
    `Travelers: ${input.travelers}`,
    `Style Requirements:`,
    ...styleContext.map((s) => `  - ${s}`),
  ].join("\n");
};

export const SKILL_METADATA = {
  name: "itinerary-skill",
  description: "Generates day-by-day travel itineraries based on destination and travel preferences",
  keywords: ["itinerary", "plan", "schedule", "day", "activity", "trip", "visit", "tour", "explore"],
  weight: 1.0,
};
