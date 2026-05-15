// ============================================================================
// Travel Plan Schema — Zod Validation
// Strict runtime validation for AI-generated travel plan responses
// ============================================================================

import { z } from "zod";

export const ActivityTypeEnum = z.enum(["activity", "food", "transport", "hotel"]);

export const DayActivitySchema = z.object({
  id: z.string().min(1, "Activity ID is required"),
  time: z.string().regex(/^\d{2}:\d{2}$/, "Time must be in HH:MM format"),
  title: z.string().min(1, "Activity title is required"),
  description: z.string().min(1, "Activity description is required"),
  location: z.string().min(1, "Location is required"),
  cost: z.number().min(0, "Cost must be non-negative"),
  type: ActivityTypeEnum,
});

export const DayPlanSchema = z.object({
  day: z.number().int().min(1, "Day must be at least 1"),
  date: z.string().optional(),
  title: z.string().min(1, "Day title is required"),
  summary: z.string().min(1, "Day summary is required"),
  activities: z.array(DayActivitySchema).min(1, "At least one activity per day"),
});

export const BudgetSummarySchema = z.object({
  accommodation: z.number().min(0),
  food: z.number().min(0),
  activities: z.number().min(0),
  transport: z.number().min(0),
  misc: z.number().min(0),
});

export const HotelSchema = z.object({
  name: z.string().min(1),
  pricePerNight: z.number().min(0),
  rating: z.number().min(0).max(5),
});

export const FoodRecommendationSchema = z.object({
  name: z.string().min(1),
  type: z.string().min(1),
  priceRange: z.string().min(1),
});

export const TravelPlanResponseSchema = z.object({
  id: z.string().min(1),
  destination: z.string().min(1),
  summary: z.string().min(10, "Summary must be descriptive"),
  estimatedCost: z.number().min(0),
  budgetSummary: BudgetSummarySchema,
  days: z.array(DayPlanSchema).min(1, "Plan must have at least one day"),
  transportation: z.array(z.string()),
  hotels: z.array(HotelSchema),
  foodRecommendations: z.array(FoodRecommendationSchema),
});

export type ValidatedTravelPlan = z.infer<typeof TravelPlanResponseSchema>;
export type ValidatedDayPlan = z.infer<typeof DayPlanSchema>;
export type ValidatedActivity = z.infer<typeof DayActivitySchema>;
export type ValidatedBudgetSummary = z.infer<typeof BudgetSummarySchema>;
