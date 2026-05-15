// ============================================================================
// Hotel Skill — Modular AI Skill
// ============================================================================

export interface HotelInput {
  destination: string;
  budget: string;
  travelers: number;
  durationDays: number;
}

export const generateHotelContext = (input: HotelInput): string => {
  const budgetGuide: Record<string, string> = {
    budget: "Focus on hostels, guesthouses, and 2-3 star hotels under $50/night",
    moderate: "Recommend 3-4 star hotels in the $50-150/night range",
    luxury: "Recommend 4-5 star hotels and resorts in the $150+/night range",
    any: "Include a range of options from budget to luxury",
  };

  return [
    `HOTEL SKILL CONTEXT:`,
    `Destination: ${input.destination}`,
    `Budget Level: ${input.budget}`,
    `Travelers: ${input.travelers}`,
    `Duration: ${input.durationDays} nights`,
    `Guidelines: ${budgetGuide[input.budget] || budgetGuide.any}`,
    `Must include: hotel name, price per night, star rating`,
  ].join("\n");
};

export const SKILL_METADATA = {
  name: "hotel-skill",
  description: "Searches and recommends hotels matching budget and destination",
  keywords: ["hotel", "stay", "accommodation", "resort", "hostel", "lodge", "inn", "sleep"],
  weight: 0.9,
};
