// ============================================================================
// AI Orchestrator — The Master Controller
// Runs the full pipeline from raw input to validated normalized output
// ============================================================================

import { TravelPlanRequest } from "@/types";
import { logger } from "@/lib/logger";
import { firewallInput, firewallOutput, safeParseJson } from "@/lib/ai/security/prompt-firewall";
import { routeSkills, generateSkillContexts } from "@/lib/ai/skill-router";
import { assembleFinalPrompt } from "@/lib/ai/final-prompt/assemble-final-prompt";
import { executeGeneration } from "./executor";
import { validateTravelPlan, validateBusinessRules } from "./validator";
import { attemptRepair, generateFallbackPlan } from "./recovery";
import { normalizePlan } from "@/lib/ai/normalize/normalize-plan";
import { contextMemory } from "@/lib/ai/memory/context-memory";
import { ValidatedTravelPlan } from "@/lib/ai/schema/travel-plan.schema";

export interface OrchestrationResult {
  success: boolean;
  plan: ValidatedTravelPlan | null;
  error?: string;
  metadata: {
    durationMs: number;
    tokensEstimated: number;
    retries: number;
    repairs: number;
  };
}

export async function orchestrateTravelPlan(
  request: TravelPlanRequest,
  sessionMemoryContext = "",
  preferenceMemoryContext = ""
): Promise<OrchestrationResult> {
  const startTime = Date.now();
  let tokensEstimated = 0;
  let repairs = 0;

  try {
    logger.workflow.step("Orchestrator", `Starting orchestration for ${request.destination}`);

    // 1. Security Scan (Input)
    const rawInputStr = JSON.stringify(request);
    const inputScan = firewallInput(rawInputStr);
    if (!inputScan.allowed) {
      throw new Error(`Security policy violation: ${inputScan.injectionReport.reasons.join(", ")}`);
    }

    // 2. Skill Selection
    const { selected: selectedSkills } = routeSkills(request.destination, request.travelStyle);
    const skillContexts = generateSkillContexts(request, selectedSkills);

    // 3. Assemble Final Prompt
    const assembled = assembleFinalPrompt({
      userRequest: request,
      skillContexts,
      sessionMemory: sessionMemoryContext,
      preferenceMemory: preferenceMemoryContext,
      workflowState: contextMemory.compile(),
    });
    tokensEstimated = assembled.tokenEstimate;

    // 4. Generate Content
    let rawResponse = await executeGeneration(assembled.fullPrompt);

    // 5. Output Security Scan
    let outputScan = firewallOutput(rawResponse);
    if (!outputScan.allowed) {
      logger.workflow.error("Orchestrator", "Initial response failed security scan, attempting generation again.");
      rawResponse = await executeGeneration(assembled.fullPrompt, "gemini-2.5-flash", 0.5);
      outputScan = firewallOutput(rawResponse);
      if (!outputScan.allowed) {
        throw new Error("AI continuously generated unsafe output.");
      }
    }

    // 6. JSON Parsing & Schema Validation
    let parsedResult = safeParseJson(outputScan.sanitizedOutput);
    let validationResult = parsedResult.valid ? validateTravelPlan(parsedResult.data) : null;
    let businessErrors: string[] = [];

    // 7. Auto-Repair Loop
    if (!parsedResult.valid || !validationResult?.valid) {
      logger.workflow.step("Orchestrator", "Validation failed. Entering repair loop.");
      const errorsToFix = [
        ...parsedResult.errors,
        ...(validationResult ? validationResult.errors : []),
      ];
      
      const repairedData = await attemptRepair(assembled.fullPrompt, rawResponse, errorsToFix);
      if (repairedData) {
        repairs++;
        parsedResult = { valid: true, data: repairedData, errors: [] };
        validationResult = validateTravelPlan(repairedData);
      }
    }

    if (!validationResult?.valid || !parsedResult.data) {
      logger.workflow.error("Orchestrator", "Unrecoverable validation error. Using fallback.");
      const fallback = generateFallbackPlan(request.destination, request.durationDays) as ValidatedTravelPlan;
      return {
        success: false,
        plan: fallback,
        error: "Failed to generate valid plan. Using fallback.",
        metadata: { durationMs: Date.now() - startTime, tokensEstimated, retries: 0, repairs },
      };
    }

    // 8. Business Rule Validation
    const validatedPlan = validationResult.data as ValidatedTravelPlan;
    businessErrors = validateBusinessRules(validatedPlan);
    
    if (businessErrors.length > 0) {
      logger.warn("Orchestrator", `Business rules violated: ${businessErrors.join("; ")}`);
      // We could repair here too, but for now we'll accept it and log the warning
    }

    // 9. Normalization
    const finalPlan = normalizePlan(validatedPlan);

    logger.workflow.complete(Date.now() - startTime);

    return {
      success: true,
      plan: finalPlan,
      metadata: { durationMs: Date.now() - startTime, tokensEstimated, retries: 0, repairs },
    };

  } catch (error) {
    logger.workflow.error("Orchestrator", `Fatal error: ${error}`);
    const fallback = generateFallbackPlan(request.destination, request.durationDays) as ValidatedTravelPlan;
    return {
      success: false,
      plan: fallback,
      error: error instanceof Error ? error.message : "Unknown error",
      metadata: { durationMs: Date.now() - startTime, tokensEstimated, retries: 0, repairs },
    };
  }
}
