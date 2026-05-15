// Mocked MCP Server Implementation

export const searchHotels = async (location: string, style: string) => {
  console.log(`[MCP] Searching hotels in ${location} with style ${style}`);
  return [
    { name: "Grand Plaza Hotel", rating: 4.8, pricePerNight: 200, amenities: ["Pool", "Spa"] },
    { name: "City Center Inn", rating: 4.2, pricePerNight: 80, amenities: ["Free WiFi", "Breakfast"] },
  ];
};

export const searchRestaurants = async (location: string, cuisine: string) => {
  console.log(`[MCP] Searching restaurants in ${location} for cuisine ${cuisine}`);
  return [
    { name: "The Local Spot", rating: 4.5, priceRange: "$$" },
    { name: "Fine Dining Experiences", rating: 4.9, priceRange: "$$$$" },
  ];
};

// In a real implementation, this would start an HTTP server or connect via stdio
export const startMcpServer = () => {
  console.log("Mock MCP Server started with tools: searchHotels, searchRestaurants");
};
