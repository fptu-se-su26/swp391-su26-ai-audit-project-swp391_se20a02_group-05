// ============================================================================
// Inject Skill Context — Dynamic skill injection into final prompt
// ============================================================================

export function injectSkillContext(skillContexts: string[]): string {
  if (skillContexts.length === 0) return "";

  return [
    "=== ACTIVE AGENT SKILLS ===",
    "The following specialized skills are active. Follow their guidelines:",
    "",
    ...skillContexts.map((ctx, i) => `--- Skill ${i + 1} ---\n${ctx}`),
  ].join("\n");
}
