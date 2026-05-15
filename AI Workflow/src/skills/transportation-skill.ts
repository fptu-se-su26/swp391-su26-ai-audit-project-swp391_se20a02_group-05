// ============================================================================
// Transportation Skill — Modular AI Skill
// ============================================================================

export interface TransportInput {
  destination: string;
  budget: string;
  durationDays: number;
}

export const generateTransportContext = (input: TransportInput): string => {
  return [
    `TRANSPORTATION SKILL CONTEXT:`,
    `Destination: ${input.destination}`,
    `Budget Level: ${input.budget}`,
    `Duration: ${input.durationDays} days`,
    `Must include: recommended transport modes, estimated costs, tips`,
    `Include airport transfers and inter-city travel if applicable`,
  ].join("\n");
};

export const SKILL_METADATA = {
  name: "transportation-skill",
  description: "Recommends local and inter-city transportation options",
  keywords: ["transport", "travel", "bus", "train", "flight", "taxi", "car", "drive", "transfer", "metro"],
  weight: 0.7,
};
