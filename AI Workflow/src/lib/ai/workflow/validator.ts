// ============================================================================
// Workflow Validator — Validates AI output against schemas and business rules
// ============================================================================

import { TravelPlanResponseSchema, ValidatedTravelPlan } from "@/lib/ai/schema/travel-plan.schema";
import { logger } from "@/lib/logger";

export interface ValidationResult {
  valid: boolean;
  data: ValidatedTravelPlan | null;
  errors: string[];
  fieldErrors: Record<string, string[]>;
}

/**
 * Validate parsed JSON against the Zod schema.
 */
export function validateTravelPlan(data: unknown): ValidationResult {
  const result = TravelPlanResponseSchema.safeParse(data);

  if (result.success) {
    logger.validation.success("TravelPlanResponse");
    return {
      valid: true,
      data: result.data,
      errors: [],
      fieldErrors: {},
    };
  }

  const errors: string[] = [];
  const fieldErrors: Record<string, string[]> = {};

  for (const issue of result.error.issues) {
    const path = issue.path.join(".");
    const message = `${path}: ${issue.message}`;
    errors.push(message);

    if (!fieldErrors[path]) fieldErrors[path] = [];
    fieldErrors[path].push(issue.message);
  }

  logger.validation.failure("TravelPlanResponse", errors);

  return {
    valid: false,
    data: null,
    errors,
    fieldErrors,
  };
}

/**
 * Validate business logic constraints on a validated plan.
 */
export function validateBusinessRules(plan: ValidatedTravelPlan): string[] {
  const issues: string[] = [];

  // Budget sum should approximately equal estimatedCost
  const budgetSum =
    plan.budgetSummary.accommodation +
    plan.budgetSummary.food +
    plan.budgetSummary.activities +
    plan.budgetSummary.transport +
    plan.budgetSummary.misc;

  const tolerance = plan.estimatedCost * 0.1; // 10% tolerance
  if (Math.abs(budgetSum - plan.estimatedCost) > tolerance) {
    issues.push(
      `Budget sum ($${budgetSum}) differs from estimatedCost ($${plan.estimatedCost}) by more than 10%`
    );
  }

  // Days must be sequential
  const dayNumbers = plan.days.map((d) => d.day);
  for (let i = 0; i < dayNumbers.length; i++) {
    if (dayNumbers[i] !== i + 1) {
      issues.push(`Day ${dayNumbers[i]} is out of sequence (expected ${i + 1})`);
    }
  }

  // Activity IDs must be unique
  const allIds = plan.days.flatMap((d) => d.activities.map((a) => a.id));
  const duplicateIds = allIds.filter((id, i) => allIds.indexOf(id) !== i);
  if (duplicateIds.length > 0) {
    issues.push(`Duplicate activity IDs found: ${duplicateIds.join(", ")}`);
  }

  // Each day should have activities
  for (const day of plan.days) {
    if (day.activities.length === 0) {
      issues.push(`Day ${day.day} has no activities`);
    }
  }

  if (issues.length > 0) {
    logger.validation.failure("BusinessRules", issues);
  }

  return issues;
}
