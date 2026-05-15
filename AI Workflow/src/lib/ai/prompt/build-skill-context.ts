// ============================================================================
// Build Skill Context — Injects active skill descriptions into prompt
// ============================================================================

export function buildSkillContext(skillContexts: string[]): string {
  if (skillContexts.length === 0) return "";

  return [
    "=== ACTIVE AGENT SKILLS ===",
    "The following skills are active for this request. Use their guidelines:",
    "",
    ...skillContexts,
  ].join("\n");
}
