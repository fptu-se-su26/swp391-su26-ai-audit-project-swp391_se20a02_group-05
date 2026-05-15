// ============================================================================
// Token Optimizer — Compress and optimize prompt token usage
// ============================================================================

import { logger } from "@/lib/logger";

const CRITICAL_SECTIONS = [
  "SECURITY RULES",
  "PROMPT INJECTION DEFENSE",
  "OUTPUT JSON CONTRACT",
  "ZOD VALIDATION CONTRACT",
  "CORE BUSINESS RULES",
  "MCP TOOL DEFINITIONS",
];

/**
 * Optimize a compiled prompt to reduce token usage while preserving critical content.
 */
export function optimizeTokens(fullPrompt: string, maxTokens = 8000): string {
  const estimatedTokens = Math.ceil(fullPrompt.length / 4);

  if (estimatedTokens <= maxTokens) {
    logger.debug("TokenOptimizer", `Prompt within budget: ~${estimatedTokens} tokens`);
    return fullPrompt;
  }

  logger.warn("TokenOptimizer", `Prompt exceeds budget: ~${estimatedTokens} > ${maxTokens}. Compressing...`);

  let optimized = fullPrompt;

  // Step 1: Remove redundant whitespace (preserving readability)
  optimized = optimized.replace(/\n{3,}/g, "\n\n");
  optimized = optimized.replace(/[ \t]{3,}/g, "  ");

  // Step 2: Compress repeated separator patterns
  optimized = optimized.replace(/(---\n\n){2,}/g, "---\n\n");

  // Step 3: Shorten non-critical sections (memory, preferences)
  // Only if still over budget
  const afterBasicOptimization = Math.ceil(optimized.length / 4);
  if (afterBasicOptimization > maxTokens) {
    optimized = compressNonCriticalSections(optimized);
  }

  const finalTokens = Math.ceil(optimized.length / 4);
  logger.info("TokenOptimizer", `Optimized: ~${estimatedTokens} → ~${finalTokens} tokens`);

  return optimized;
}

/**
 * Compress non-critical sections while preserving critical ones.
 */
function compressNonCriticalSections(prompt: string): string {
  const sections = prompt.split("---\n\n");

  const compressed = sections.map((section) => {
    // Never compress critical sections
    const isCritical = CRITICAL_SECTIONS.some((cs) => section.includes(cs));
    if (isCritical) return section;

    // For non-critical sections, compress verbose descriptions
    let s = section;

    // Shorten bullet point lists if they're very long
    const lines = s.split("\n");
    if (lines.length > 20) {
      // Keep first 15 lines and add a summary
      const kept = lines.slice(0, 15);
      const removedCount = lines.length - 15;
      kept.push(`  [... ${removedCount} additional items summarized for brevity]`);
      s = kept.join("\n");
    }

    return s;
  });

  return compressed.join("---\n\n");
}
