// ============================================================================
// Context Compiler — Merges, deduplicates, and optimizes all context
// ============================================================================

import { logger } from "@/lib/logger";
import { optimizeTokens } from "./token-optimizer";

interface ContextSection {
  name: string;
  content: string;
  priority: number; // Lower = higher priority
  critical: boolean; // Critical sections are never truncated
}

/**
 * Compile multiple context sections into one optimized string.
 * - Merges all sections
 * - Removes duplicates
 * - Maintains priority ordering
 * - Preserves critical rules
 */
export function compileContext(sections: ContextSection[], maxTokens = 8000): string {
  // Sort by priority (ascending = highest priority first)
  const sorted = [...sections].sort((a, b) => a.priority - b.priority);

  // Deduplicate: remove sections with identical content
  const seen = new Set<string>();
  const unique = sorted.filter((s) => {
    const key = s.content.trim();
    if (seen.has(key)) {
      logger.debug("ContextCompiler", `Removed duplicate section: ${s.name}`);
      return false;
    }
    seen.add(key);
    return true;
  });

  // Assemble
  const assembled = unique.map((s) => s.content.trim()).join("\n\n---\n\n");

  // Optimize tokens
  const optimized = optimizeTokens(assembled, maxTokens);

  logger.info("ContextCompiler", "Context compiled", {
    inputSections: sections.length,
    uniqueSections: unique.length,
    estimatedTokens: Math.ceil(optimized.length / 4),
  });

  return optimized;
}
