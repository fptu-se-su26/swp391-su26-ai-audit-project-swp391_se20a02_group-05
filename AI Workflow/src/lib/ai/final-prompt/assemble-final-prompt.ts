// ============================================================================
// Assemble Final Prompt — The Master Prompt Pipeline
// Combines ALL context layers into a single, deterministic prompt string
// 
// ORDER IS MANDATORY:
//   1. System Role
//   2. Core Business Rules
//   3. Security Rules
//   4. Prompt Injection Defense Rules
//   5. Output JSON Contract
//   6. Zod Validation Contract
//   7. Active Agent Skills
//   8. MCP Tool Definitions
//   9. Memory Context
//  10. User Preferences
//  11. Travel Constraints
//  12. Optimization Rules
//  13. Response Quality Rules
//  14. Failure Recovery Rules
//  15. Active Workflow State
//  16. Current User Request
// ============================================================================

import { TravelPlanRequest } from "@/types";
import { buildSystemPrompt } from "@/lib/ai/prompt/build-system-prompt";
import { buildUserPrompt } from "@/lib/ai/prompt/build-user-prompt";
import { injectBusinessRules } from "./inject-business-rules";
import { injectSecurityContext } from "./inject-security-context";
import { injectOutputContract } from "./inject-output-contract";
import { injectSkillContext } from "./inject-skill-context";
import { injectMcpContext } from "./inject-mcp-context";
import { injectMemoryContext } from "./inject-memory-context";
import { injectUserContext } from "./inject-user-context";
import { injectValidationContext } from "./inject-validation-context";
import { injectTravelRules } from "./inject-travel-rules";
import { logger } from "@/lib/logger";

export interface FinalPromptRequest {
  userRequest: TravelPlanRequest;
  skillContexts: string[];
  sessionMemory: string;
  preferenceMemory: string;
  workflowState?: string;
}

export interface AssembledPrompt {
  systemPrompt: string;
  userPrompt: string;
  fullPrompt: string;
  compiledBusinessRules: string;
  compiledSkills: string;
  compiledTools: string;
  validationContracts: string;
  securityRules: string;
  memoryContext: string;
  tokenEstimate: number;
}

/**
 * Assemble the final, complete AI execution prompt.
 * This guarantees ALL business rules, security rules, validation schemas,
 * skill contexts, MCP tools, memory, and user context are included.
 */
export function assembleFinalPrompt(req: FinalPromptRequest): AssembledPrompt {
  const startTime = Date.now();

  // 1. System Role
  const systemPrompt = buildSystemPrompt();

  // 2. Core Business Rules (priority-ordered)
  const compiledBusinessRules = injectBusinessRules();

  // 3-4. Security Rules + Prompt Injection Defense
  const securityRules = injectSecurityContext();

  // 5-6. Output JSON Contract + Zod Validation Contract
  const validationContracts = injectOutputContract();

  // 7. Active Agent Skills
  const compiledSkills = injectSkillContext(req.skillContexts);

  // 8. MCP Tool Definitions
  const compiledTools = injectMcpContext();

  // 9. Memory Context
  const memoryContext = injectMemoryContext(req.sessionMemory, req.preferenceMemory);

  // 10. User Preferences
  const userPreferences = injectUserContext(req.userRequest);

  // 11-12. Travel Constraints + Optimization Rules
  const travelRules = injectTravelRules(
    req.userRequest.destination,
    req.userRequest.budget
  );

  // 13-14. Response Quality + Failure Recovery
  const qualityRules = injectValidationContext();

  // 15. Workflow State
  const workflowState = req.workflowState
    ? `=== ACTIVE WORKFLOW STATE ===\n${req.workflowState}`
    : "";

  // 16. Current User Request (always last)
  const userPrompt = buildUserPrompt(req.userRequest);

  // Assemble in MANDATORY order
  const fullPromptParts = [
    systemPrompt,
    compiledBusinessRules,
    securityRules,
    validationContracts,
    compiledSkills,
    compiledTools,
    memoryContext,
    userPreferences,
    travelRules,
    qualityRules,
    workflowState,
    userPrompt,
  ].filter((part) => part.trim().length > 0);

  const fullPrompt = fullPromptParts.join("\n\n---\n\n");

  // Estimate tokens (~4 chars per token)
  const tokenEstimate = Math.ceil(fullPrompt.length / 4);

  logger.ai.request("Final prompt assembled", {
    sections: fullPromptParts.length,
    totalChars: fullPrompt.length,
    tokenEstimate,
    assemblyTime: Date.now() - startTime,
  });

  return {
    systemPrompt,
    userPrompt,
    fullPrompt,
    compiledBusinessRules,
    compiledSkills,
    compiledTools,
    validationContracts,
    securityRules,
    memoryContext,
    tokenEstimate,
  };
}
