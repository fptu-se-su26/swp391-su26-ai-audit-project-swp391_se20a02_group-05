// ============================================================================
// Workflow Executor — Executes the AI generation pipeline
// ============================================================================

import { generateWithRetry } from "@/lib/ai/google";
import { logger } from "@/lib/logger";

/**
 * Execute the AI generation step using the fully assembled prompt.
 */
export async function executeGeneration(
  fullPrompt: string,
  modelName = "gemini-2.5-flash",
  temperature = 0.7
): Promise<string> {
  const startTime = Date.now();
  logger.workflow.step("Executor", `Starting AI generation with ${modelName}`);

  try {
    const response = await generateWithRetry(fullPrompt, {
      model: modelName,
      temperature,
    });

    const duration = Date.now() - startTime;
    logger.workflow.step("Executor", `Generation completed in ${duration}ms`);

    return response;
  } catch (error) {
    logger.workflow.error("Executor", `Generation failed: ${error}`);
    throw error;
  }
}
