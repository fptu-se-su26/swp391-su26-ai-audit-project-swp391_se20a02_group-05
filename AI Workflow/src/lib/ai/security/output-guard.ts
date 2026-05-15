// ============================================================================
// Output Guard — Prompt Security Layer
// Validates AI output doesn't contain leaked system prompts or harmful content
// ============================================================================

import { logger } from "@/lib/logger";

export interface OutputGuardResult {
  safe: boolean;
  issues: string[];
}

const OUTPUT_DANGER_PATTERNS = [
  { pattern: /system\s*prompt\s*[:=]/i, reason: "AI leaked system prompt" },
  { pattern: /\bAPI[_\s]?KEY\b/i, reason: "API key reference in output" },
  { pattern: /AIzaSy[A-Za-z0-9_-]{33}/i, reason: "Exposed Google API key pattern" },
  { pattern: /sk-[A-Za-z0-9]{40,}/i, reason: "Exposed OpenAI API key pattern" },
  { pattern: /password\s*[:=]\s*["'][^"']+["']/i, reason: "Password in output" },
  { pattern: /\bsecret\b.*[:=]\s*["'][^"']+["']/i, reason: "Secret value in output" },
  { pattern: /<script[^>]*>/i, reason: "Script tag in output" },
  { pattern: /javascript:/i, reason: "JavaScript URI in output" },
  { pattern: /on(?:load|error|click)\s*=/i, reason: "Event handler in output" },
];

/**
 * Guard against AI producing dangerous or leaking output content.
 */
export function guardOutput(output: string): OutputGuardResult {
  const issues: string[] = [];

  if (!output || typeof output !== "string") {
    return { safe: true, issues: [] };
  }

  for (const rule of OUTPUT_DANGER_PATTERNS) {
    if (rule.pattern.test(output)) {
      issues.push(rule.reason);
    }
  }

  const safe = issues.length === 0;

  if (!safe) {
    logger.security.block("Output guard blocked response", { issues });
  }

  return { safe, issues };
}

/**
 * Strip dangerous content from AI output while preserving the rest.
 */
export function sanitizeOutput(output: string): string {
  let cleaned = output;

  // Remove any script tags
  cleaned = cleaned.replace(/<script[^>]*>[\s\S]*?<\/script>/gi, "");
  
  // Remove event handlers
  cleaned = cleaned.replace(/\bon\w+\s*=\s*["'][^"']*["']/gi, "");

  // Redact anything that looks like an API key
  cleaned = cleaned.replace(/AIzaSy[A-Za-z0-9_-]{33}/g, "[REDACTED_KEY]");
  cleaned = cleaned.replace(/sk-[A-Za-z0-9]{40,}/g, "[REDACTED_KEY]");

  return cleaned;
}
