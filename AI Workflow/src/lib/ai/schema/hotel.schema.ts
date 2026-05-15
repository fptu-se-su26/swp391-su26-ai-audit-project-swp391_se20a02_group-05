import { z } from "zod";

export const HotelSearchResultSchema = z.object({
  name: z.string().min(1),
  pricePerNight: z.number().min(0),
  rating: z.number().min(0).max(5),
  amenities: z.array(z.string()).optional(),
  location: z.string().optional(),
  checkIn: z.string().optional(),
  checkOut: z.string().optional(),
});

export const HotelSearchResponseSchema = z.object({
  results: z.array(HotelSearchResultSchema),
  totalFound: z.number().min(0),
  searchCity: z.string(),
});

export type ValidatedHotelSearchResult = z.infer<typeof HotelSearchResultSchema>;
