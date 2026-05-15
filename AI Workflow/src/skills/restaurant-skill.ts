// ============================================================================
// Restaurant Skill — Modular AI Skill
// ============================================================================

export interface RestaurantInput {
  destination: string;
  budget: string;
  travelStyle: string[];
}

export const generateRestaurantContext = (input: RestaurantInput): string => {
  const isFoodie = input.travelStyle.some((s) => s.toLowerCase() === "foodie");

  return [
    `RESTAURANT SKILL CONTEXT:`,
    `Destination: ${input.destination}`,
    `Budget Level: ${input.budget}`,
    `Foodie Mode: ${isFoodie ? "YES — prioritize diverse and high-quality food experiences" : "Standard"}`,
    `Must include: restaurant name, cuisine type, price range indicator`,
    isFoodie ? `Include: street food, food markets, cooking classes, local delicacies` : "",
  ].filter(Boolean).join("\n");
};

export const SKILL_METADATA = {
  name: "restaurant-skill",
  description: "Recommends restaurants, street food, and dining experiences",
  keywords: ["food", "restaurant", "eat", "dining", "cuisine", "meal", "lunch", "dinner", "breakfast", "street food"],
  weight: 0.8,
};
