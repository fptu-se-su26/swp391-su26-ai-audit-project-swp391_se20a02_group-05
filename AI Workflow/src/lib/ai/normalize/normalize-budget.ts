import { ValidatedBudgetSummary } from "@/lib/ai/schema/travel-plan.schema";

/**
 * Normalize budget values to consistent USD formatting.
 */
export function normalizeBudget(budget: ValidatedBudgetSummary): ValidatedBudgetSummary {
  return {
    accommodation: roundCurrency(budget.accommodation),
    food: roundCurrency(budget.food),
    activities: roundCurrency(budget.activities),
    transport: roundCurrency(budget.transport),
    misc: roundCurrency(budget.misc),
  };
}

function roundCurrency(value: number): number {
  return Math.round(Math.max(0, value) * 100) / 100;
}
