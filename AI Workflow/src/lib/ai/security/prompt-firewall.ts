// ============================================================================
// Prompt Firewall — Unified Security Entry Point
// Orchestrates sanitize → detect → validate → guard pipeline
// ============================================================================

import { sanitizeInput } from "./sanitize-input";
import { detectInjection, type InjectionReport } from "./detect-injection";
import { guardOutput, sanitizeOutput, type OutputGuardResult } from "./output-guard";
import { logger } from "@/lib/logger";

export interface FirewallInputResult {
  allowed: boolean;
  sanitizedInput: string;
  injectionReport: InjectionReport;
  originalInput: string;
}

export interface FirewallOutputResult {
  allowed: boolean;
  sanitizedOutput: string;
  guardReport: OutputGuardResult;
}

/**
 * Run the full input security pipeline:
 *   sanitize → detect injection → allow/deny
 */
export function firewallInput(rawInput: string): FirewallInputResult {
  const originalInput = rawInput;

  // Step 1: Sanitize
  const sanitizedInput = sanitizeInput(rawInput);

  // Step 2: Detect injection
  const injectionReport = detectInjection(sanitizedInput);

  const allowed = injectionReport.safe;

  if (!allowed) {
    logger.security.block("Prompt firewall BLOCKED input", {
      riskLevel: injectionReport.riskLevel,
      reasons: injectionReport.reasons,
    });
  } else {
    logger.security.clean("Prompt firewall PASSED input");
  }

  return { allowed, sanitizedInput, injectionReport, originalInput };
}

/**
 * Run the full output security pipeline:
 *   guard → sanitize output
 */
export function firewallOutput(rawOutput: string): FirewallOutputResult {
  // Step 1: Guard
  const guardReport = guardOutput(rawOutput);

  // Step 2: Sanitize output regardless (defense in depth)
  const sanitizedOutput = sanitizeOutput(rawOutput);

  const allowed = guardReport.safe;

  if (!allowed) {
    logger.security.block("Output firewall flagged response", {
      issues: guardReport.issues,
    });
  }

  return { allowed, sanitizedOutput, guardReport };
}

// Re-export all security modules
export { sanitizeInput } from "./sanitize-input";
export { detectInjection, type InjectionReport } from "./detect-injection";
export { safeParseJson, validateRequiredKeys } from "./validate-json";
export { guardOutput, sanitizeOutput } from "./output-guard";
