import { z } from "zod";

export const travelPlanSchema = z.object({
  destination: z.string().min(2, "Destination is required"),
  budget: z.enum(["budget", "moderate", "luxury", "any"]),
  budgetAmount: z.number().optional(),
  travelStyle: z.array(z.string()).min(1, "Select at least one travel style"),
  travelers: z.number().min(1, "Must have at least 1 traveler").max(20),
  durationDays: z.number().min(1, "Must be at least 1 day").max(30),
  startDate: z.date().optional(),
  additionalNotes: z.string().optional(),
});

export type TravelPlanRequest = z.infer<typeof travelPlanSchema>;

export interface DayActivity {
  id: string;
  time: string;
  title: string;
  description: string;
  location: string;
  cost: number;
  type: "activity" | "food" | "transport" | "hotel";
}

export interface DayPlan {
  day: number;
  date?: string;
  title: string;
  summary: string;
  activities: DayActivity[];
}

export interface TravelPlanResponse {
  id: string;
  destination: string;
  summary: string;
  days: DayPlan[];
  estimatedCost: number;
  transportation: string[];
  hotels: any[];
  foodRecommendations: any[];
  budgetSummary: {
    accommodation: number;
    food: number;
    activities: number;
    transport: number;
    misc: number;
  };
}
