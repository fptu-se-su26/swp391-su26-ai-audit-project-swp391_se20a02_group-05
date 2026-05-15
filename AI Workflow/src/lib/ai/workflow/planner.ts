// ============================================================================
// Workflow Planner — Determines which pipeline steps to execute
// ============================================================================

import { TravelPlanRequest } from "@/types";
import { logger } from "@/lib/logger";

export interface WorkflowStep {
  name: string;
  required: boolean;
  order: number;
}

const ALL_STEPS: WorkflowStep[] = [
  { name: "security-scan", required: true, order: 1 },
  { name: "skill-selection", required: true, order: 2 },
  { name: "mcp-tool-selection", required: true, order: 3 },
  { name: "prompt-composition", required: true, order: 4 },
  { name: "token-optimization", required: true, order: 5 },
  { name: "ai-generation", required: true, order: 6 },
  { name: "json-validation", required: true, order: 7 },
  { name: "schema-validation", required: true, order: 8 },
  { name: "business-rule-validation", required: true, order: 9 },
  { name: "normalization", required: true, order: 10 },
  { name: "output-security-scan", required: true, order: 11 },
];

/**
 * Plan the workflow steps for a given request.
 */
export function planWorkflow(request: TravelPlanRequest): WorkflowStep[] {
  const steps = ALL_STEPS.filter((s) => s.required).sort((a, b) => a.order - b.order);

  logger.workflow.step("Planner", `Planned ${steps.length} steps for ${request.destination}`);

  return steps;
}
