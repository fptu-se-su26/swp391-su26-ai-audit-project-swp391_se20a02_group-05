// ============================================================================
// Skill Router — Intelligent Skill Selection
// Routes user prompts to relevant skills using keyword + semantic matching
// ============================================================================

import { logger } from "@/lib/logger";
import { SKILL_METADATA as ItineraryMeta, generateItineraryContext } from "@/skills/itinerary-skill";
import { SKILL_METADATA as HotelMeta, generateHotelContext } from "@/skills/hotel-skill";
import { SKILL_METADATA as RestaurantMeta, generateRestaurantContext } from "@/skills/restaurant-skill";
import { SKILL_METADATA as TransportMeta, generateTransportContext } from "@/skills/transportation-skill";
import { estimateBudget } from "@/skills/budget-skill";

interface SkillDefinition {
  name: string;
  description: string;
  keywords: string[];
  weight: number;
  contextGenerator: (input: any) => string;
}

interface SkillScore {
  skill: SkillDefinition;
  score: number;
  matchedKeywords: string[];
}

const ALL_SKILLS: SkillDefinition[] = [
  { ...ItineraryMeta, contextGenerator: generateItineraryContext },
  { ...HotelMeta, contextGenerator: generateHotelContext },
  { ...RestaurantMeta, contextGenerator: generateRestaurantContext },
  { ...TransportMeta, contextGenerator: generateTransportContext },
];

/**
 * Score each skill against the user's prompt and travel styles.
 */
function scoreSkills(prompt: string, travelStyles: string[]): SkillScore[] {
  const input = `${prompt} ${travelStyles.join(" ")}`.toLowerCase();
  const words = input.split(/\s+/);

  return ALL_SKILLS.map((skill) => {
    const matchedKeywords: string[] = [];
    let score = 0;

    for (const keyword of skill.keywords) {
      if (input.includes(keyword.toLowerCase())) {
        matchedKeywords.push(keyword);
        score += skill.weight;
      }
    }

    // Boost for exact word matches
    for (const word of words) {
      if (skill.keywords.some((k) => k.toLowerCase() === word)) {
        score += 0.5;
      }
    }

    return { skill, score, matchedKeywords };
  })
    .sort((a, b) => b.score - a.score);
}

/**
 * Select relevant skills based on user input.
 * Returns skills with confidence > 0 plus always-on skills.
 */
export function routeSkills(
  prompt: string,
  travelStyles: string[],
  threshold = 0.5
): { selected: SkillScore[]; contexts: string[] } {
  const scores = scoreSkills(prompt, travelStyles);

  // Itinerary skill is always included (core skill)
  const selected = scores.filter((s) => s.score >= threshold || s.skill.name === "itinerary-skill");

  // If no skills matched above threshold, include all (fallback for general prompts)
  const finalSelection = selected.length > 0 ? selected : scores;

  logger.workflow.step("SkillRouter", `Selected ${finalSelection.length} skills: ${finalSelection.map((s) => s.skill.name).join(", ")}`);

  return {
    selected: finalSelection,
    contexts: finalSelection.map((s) => s.skill.description),
  };
}

/**
 * Generate full skill context strings for prompt injection.
 */
export function generateSkillContexts(
  input: {
    destination: string;
    budget: string;
    durationDays: number;
    travelers: number;
    travelStyle: string[];
    additionalNotes?: string;
  },
  selectedSkills: SkillScore[]
): string[] {
  return selectedSkills.map((s) => {
    try {
      return s.skill.contextGenerator(input);
    } catch {
      return `[${s.skill.name}]: Context generation failed`;
    }
  });
}
