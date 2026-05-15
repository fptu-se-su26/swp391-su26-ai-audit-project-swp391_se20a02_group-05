// ============================================================================
// Inject Memory Context — Session + preference memory
// ============================================================================

export function injectMemoryContext(
  sessionContext: string,
  preferenceContext: string
): string {
  const parts: string[] = ["=== MEMORY CONTEXT ==="];

  if (sessionContext && sessionContext.trim().length > 0) {
    parts.push(sessionContext);
  }

  if (preferenceContext && preferenceContext.trim().length > 0) {
    parts.push("");
    parts.push(preferenceContext);
  }

  if (parts.length === 1) {
    parts.push("No prior memory context available.");
  }

  return parts.join("\n");
}
