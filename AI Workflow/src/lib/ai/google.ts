import { GoogleGenerativeAI } from "@google/generative-ai";
import { TravelPlanRequest, TravelPlanResponse } from "@/types";

const apiKey = process.env.GOOGLE_API_KEY || "AIzaSyDQCp1a-vPQYVFJcJ9UwbQqhqmoIBaQps8";
const genAI = new GoogleGenerativeAI(apiKey);

export const getGeminiModel = (modelName = "gemini-2.5-flash") => {
  return genAI.getGenerativeModel({ model: modelName });
};

export const generateTravelPlan = async (
  request: TravelPlanRequest
): Promise<TravelPlanResponse> => {
  const model = getGeminiModel();

  const prompt = `
    You are an expert AI Travel Agent.
    Generate a highly structured JSON travel itinerary for the following request:
    Destination: ${request.destination}
    Duration: ${request.durationDays} days
    Travelers: ${request.travelers}
    Budget Level: ${request.budget}
    Travel Style: ${request.travelStyle.join(", ")}
    Additional Notes: ${request.additionalNotes || "None"}

    You MUST strictly return a JSON object that matches this TypeScript interface:
    {
      "id": "string (generate a random uuid)",
      "destination": "string",
      "summary": "string",
      "estimatedCost": "number",
      "budgetSummary": {
        "accommodation": "number",
        "food": "number",
        "activities": "number",
        "transport": "number",
        "misc": "number"
      },
      "transportation": ["string array"],
      "hotels": [
        { "name": "string", "pricePerNight": "number", "rating": "number" }
      ],
      "foodRecommendations": [
        { "name": "string", "type": "string", "priceRange": "string" }
      ],
      "days": [
        {
          "day": "number",
          "title": "string",
          "summary": "string",
          "activities": [
            {
              "id": "string",
              "time": "string (e.g. 09:00)",
              "title": "string",
              "description": "string",
              "location": "string",
              "cost": "number",
              "type": "activity | food | transport | hotel"
            }
          ]
        }
      ]
    }

    Return ONLY the raw JSON object. Do not wrap it in markdown block.
  `;

  try {
    // Generate content using Gemini
    const result = await model.generateContent(prompt);
    const responseText = result.response.text();
    
    // Clean potential markdown wrapping
    const cleanJson = responseText.replace(/```json\n?/g, '').replace(/```\n?/g, '').trim();
    
    return JSON.parse(cleanJson) as TravelPlanResponse;
  } catch (error) {
    console.error("Error generating travel plan:", error);
    throw new Error("Failed to generate travel plan");
  }
};
