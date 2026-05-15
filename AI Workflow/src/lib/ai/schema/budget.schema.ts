import { z } from "zod";

export const BudgetBreakdownSchema = z.object({
  accommodation: z.number().min(0),
  food: z.number().min(0),
  activities: z.number().min(0),
  transport: z.number().min(0),
  misc: z.number().min(0),
});

export const BudgetValidationSchema = z.object({
  totalBudget: z.number().min(0),
  estimatedCost: z.number().min(0),
  breakdown: BudgetBreakdownSchema,
  overBudget: z.boolean(),
  overBudgetAmount: z.number(),
  overBudgetPercentage: z.number(),
});

export type ValidatedBudgetBreakdown = z.infer<typeof BudgetBreakdownSchema>;
export type ValidatedBudgetValidation = z.infer<typeof BudgetValidationSchema>;
