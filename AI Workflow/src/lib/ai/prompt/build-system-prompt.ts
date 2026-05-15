// ============================================================================
// Build System Prompt — Core system identity and behavior rules
// ============================================================================

import * as fs from "fs";
import * as path from "path";

/**
 * Build the core system prompt from LLM.txt + hardcoded identity.
 */
export function buildSystemPrompt(): string {
  let llmTxt = "";
  try {
    const llmPath = path.join(process.cwd(), "LLM.txt");
    llmTxt = fs.readFileSync(llmPath, "utf-8");
  } catch {
    // Fallback if LLM.txt not readable
    llmTxt = "";
  }

  return [
    "=== SYSTEM ROLE ===",
    "You are an expert AI Travel Planning Agent built by AI Travel Planner.",
    "You generate highly structured, valid JSON travel itineraries.",
    "You NEVER produce markdown, prose, or explanations outside of JSON.",
    "You ALWAYS return a single raw JSON object as your entire response.",
    "",
    llmTxt ? "=== LLM.txt RULES ===" : "",
    llmTxt,
  ]
    .filter(Boolean)
    .join("\n");
}
