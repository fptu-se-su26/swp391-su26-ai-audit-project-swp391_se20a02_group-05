// ============================================================================
// Inject MCP Context — Tool definitions and restrictions
// ============================================================================

const MCP_TOOL_DEFINITIONS = [
  {
    name: "searchHotels",
    description: "Search for hotels at a destination",
    allowedParams: ["city", "budget", "dates", "stars"],
    forbidden: ["file access", "code execution", "environment access", "system access"],
  },
  {
    name: "searchRestaurants",
    description: "Search for restaurants and food venues",
    allowedParams: ["city", "cuisine", "budget", "rating"],
    forbidden: ["file access", "code execution", "environment access", "system access"],
  },
  {
    name: "estimateBudget",
    description: "Estimate travel budget breakdown",
    allowedParams: ["destination", "duration", "budget_level", "travelers"],
    forbidden: ["file access", "code execution", "environment access"],
  },
  {
    name: "generateItinerary",
    description: "Generate day-by-day itinerary structure",
    allowedParams: ["destination", "duration", "style", "activities"],
    forbidden: ["file access", "code execution", "environment access"],
  },
];

export function injectMcpContext(): string {
  const toolDescriptions = MCP_TOOL_DEFINITIONS.map((tool) => {
    return [
      `Tool: ${tool.name}`,
      `  Description: ${tool.description}`,
      `  Allowed parameters: ${tool.allowedParams.join(", ")}`,
      `  Forbidden operations: ${tool.forbidden.join(", ")}`,
    ].join("\n");
  });

  return [
    "=== MCP TOOL DEFINITIONS ===",
    "The following tools are available for grounding your responses:",
    "",
    ...toolDescriptions,
    "",
    "IMPORTANT: These tools provide data context only. Never attempt to execute code or access files.",
  ].join("\n");
}
