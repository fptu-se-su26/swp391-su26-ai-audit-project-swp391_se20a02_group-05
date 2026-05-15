import { NextResponse } from "next/server";
import { generateTravelPlan } from "@/lib/ai/google";
import { TravelPlanRequest } from "@/types";

export async function POST(req: Request) {
  try {
    const body: TravelPlanRequest = await req.json();
    
    // In a real app, validate body with Zod here
    
    const plan = await generateTravelPlan(body);
    
    return NextResponse.json({ success: true, plan });
  } catch (error) {
    console.error("API Error:", error);
    return NextResponse.json(
      { success: false, error: "Failed to generate travel plan" },
      { status: 500 }
    );
  }
}
