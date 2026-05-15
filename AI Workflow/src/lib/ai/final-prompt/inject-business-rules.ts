// ============================================================================
// Inject Business Rules — Collects and formats all business rules
// ============================================================================

import { BUDGET_RULES } from "@/lib/business-rules/budget.rules";
import { ITINERARY_RULES } from "@/lib/business-rules/itinerary.rules";
import { TRANSPORTATION_RULES } from "@/lib/business-rules/transportation.rules";
import { HOTEL_RULES } from "@/lib/business-rules/hotel.rules";
import { FOOD_RULES } from "@/lib/business-rules/food.rules";
import { SAFETY_RULES } from "@/lib/business-rules/safety.rules";
import { VALIDATION_RULES } from "@/lib/business-rules/validation.rules";
import { OPTIMIZATION_RULES } from "@/lib/business-rules/optimization.rules";

interface RuleSet {
  name: string;
  priority: number;
  rules: string[];
}

const ALL_RULE_SETS: RuleSet[] = [
  SAFETY_RULES,
  VALIDATION_RULES,
  BUDGET_RULES,
  ITINERARY_RULES,
  TRANSPORTATION_RULES,
  HOTEL_RULES,
  FOOD_RULES,
  OPTIMIZATION_RULES,
].sort((a, b) => a.priority - b.priority);

/**
 * Inject all business rules into the prompt, ordered by priority.
 */
export function injectBusinessRules(): string {
  const sections = ALL_RULE_SETS.map((ruleSet) => {
    const header = `[${ruleSet.name.toUpperCase()}] (Priority: ${ruleSet.priority})`;
    const rules = ruleSet.rules.map((r, i) => `  ${i + 1}. ${r}`).join("\n");
    return `${header}\n${rules}`;
  });

  return ["=== CORE BUSINESS RULES ===", "", ...sections].join("\n\n");
}

/**
 * Get only the rule sets relevant to certain travel styles.
 */
export function injectRelevantBusinessRules(travelStyles: string[]): string {
  const styles = travelStyles.map((s) => s.toLowerCase());

  // Always include: safety, validation, budget, itinerary
  const alwaysInclude = ["safety-rules", "validation-rules", "budget-rules", "itinerary-rules"];

  const relevantSets = ALL_RULE_SETS.filter((rs) => {
    if (alwaysInclude.includes(rs.name)) return true;
    if (rs.name === "hotel-rules") return true; // Hotels are always relevant
    if (rs.name === "food-rules" && styles.some((s) => ["foodie", "cultural", "luxury"].includes(s))) return true;
    if (rs.name === "transportation-rules") return true;
    if (rs.name === "optimization-rules") return true;
    return false;
  });

  const sections = relevantSets.map((ruleSet) => {
    const header = `[${ruleSet.name.toUpperCase()}] (Priority: ${ruleSet.priority})`;
    const rules = ruleSet.rules.map((r, i) => `  ${i + 1}. ${r}`).join("\n");
    return `${header}\n${rules}`;
  });

  return ["=== CORE BUSINESS RULES ===", "", ...sections].join("\n\n");
}
