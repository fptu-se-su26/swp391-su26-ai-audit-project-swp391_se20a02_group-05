// ============================================================================
// Inject Security Context — For final prompt assembly
// ============================================================================

import { buildSecurityContext } from "@/lib/ai/prompt/build-security-context";

export function injectSecurityContext(): string {
  return buildSecurityContext();
}
