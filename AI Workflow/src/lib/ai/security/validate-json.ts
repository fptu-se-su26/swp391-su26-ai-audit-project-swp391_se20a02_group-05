// ============================================================================
// JSON Validation — Prompt Security Layer
// Safe JSON parsing with strict structure validation
// ============================================================================

import { logger } from "@/lib/logger";

export interface JsonValidationResult<T = unknown> {
  valid: boolean;
  data: T | null;
  errors: string[];
}

/**
 * Safely parse a raw AI response string into JSON.
 * Handles markdown code fences, trailing commas, and common AI output issues.
 */
export function safeParseJson<T = unknown>(raw: string): JsonValidationResult<T> {
  const errors: string[] = [];

  if (!raw || typeof raw !== "string") {
    return { valid: false, data: null, errors: ["Input is empty or not a string"] };
  }

  let cleaned = raw.trim();

  // Strip markdown code fences
  cleaned = cleaned.replace(/^```(?:json)?\s*\n?/i, "");
  cleaned = cleaned.replace(/\n?\s*```\s*$/i, "");
  cleaned = cleaned.trim();

  // Strip leading prose before first { or [
  const firstBrace = cleaned.indexOf("{");
  const firstBracket = cleaned.indexOf("[");
  let jsonStart = -1;

  if (firstBrace >= 0 && firstBracket >= 0) {
    jsonStart = Math.min(firstBrace, firstBracket);
  } else if (firstBrace >= 0) {
    jsonStart = firstBrace;
  } else if (firstBracket >= 0) {
    jsonStart = firstBracket;
  }

  if (jsonStart > 0) {
    errors.push("Stripped leading non-JSON content");
    cleaned = cleaned.substring(jsonStart);
  }

  // Strip trailing prose after the last } or ]
  const lastBrace = cleaned.lastIndexOf("}");
  const lastBracket = cleaned.lastIndexOf("]");
  const jsonEnd = Math.max(lastBrace, lastBracket);

  if (jsonEnd >= 0 && jsonEnd < cleaned.length - 1) {
    errors.push("Stripped trailing non-JSON content");
    cleaned = cleaned.substring(0, jsonEnd + 1);
  }

  // Remove trailing commas before closing braces/brackets (common AI mistake)
  cleaned = cleaned.replace(/,\s*([}\]])/g, "$1");

  // Attempt parse
  try {
    const parsed = JSON.parse(cleaned) as T;
    if (errors.length > 0) {
      logger.validation.failure("JSON:Parse", errors);
    }
    return { valid: true, data: parsed, errors };
  } catch (e) {
    const message = e instanceof Error ? e.message : "Unknown parse error";
    errors.push(`JSON parse failed: ${message}`);
    logger.validation.failure("JSON:Parse", errors);
    return { valid: false, data: null, errors };
  }
}

/**
 * Verify that the parsed object has all required top-level keys.
 */
export function validateRequiredKeys(obj: Record<string, unknown>, requiredKeys: string[]): string[] {
  const missing: string[] = [];
  for (const key of requiredKeys) {
    if (!(key in obj) || obj[key] === undefined || obj[key] === null) {
      missing.push(key);
    }
  }
  return missing;
}
