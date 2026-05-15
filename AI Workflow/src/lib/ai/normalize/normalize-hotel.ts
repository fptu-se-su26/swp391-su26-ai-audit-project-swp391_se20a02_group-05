/**
 * Normalize hotel data.
 */
export function normalizeHotel(hotel: { name: string; pricePerNight: number; rating: number }) {
  return {
    name: hotel.name.trim(),
    pricePerNight: Math.round(Math.max(0, hotel.pricePerNight) * 100) / 100,
    rating: Math.min(5, Math.max(0, Math.round(hotel.rating * 10) / 10)),
  };
}
