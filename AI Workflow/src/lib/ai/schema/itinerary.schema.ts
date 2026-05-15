import { z } from "zod";

export const ItineraryRowSchema = z.object({
  day: z.number().int().min(1),
  time: z.string().regex(/^\d{2}:\d{2}$/, "Time must be HH:MM"),
  activity: z.string().min(1),
  location: z.string().min(1),
  cost: z.number().min(0),
  type: z.enum(["activity", "food", "transport", "hotel"]),
});

export const FinalItinerarySchema = z.object({
  tripTable: z.array(ItineraryRowSchema).min(1),
  budgetSummary: z.object({
    accommodation: z.number().min(0),
    food: z.number().min(0),
    activities: z.number().min(0),
    transport: z.number().min(0),
    misc: z.number().min(0),
    total: z.number().min(0),
  }),
  travelTips: z.array(z.string()),
  packingList: z.array(z.string()),
});

export type ValidatedItineraryRow = z.infer<typeof ItineraryRowSchema>;
export type ValidatedFinalItinerary = z.infer<typeof FinalItinerarySchema>;
