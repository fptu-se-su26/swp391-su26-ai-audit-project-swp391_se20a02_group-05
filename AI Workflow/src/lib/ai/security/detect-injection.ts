// ============================================================================
// Prompt Injection Detection — Prompt Security Layer
// Detects jailbreaks, prompt leaking, encoded attacks, tool manipulation
// ============================================================================

import { logger } from "@/lib/logger";

export interface InjectionReport {
  safe: boolean;
  riskLevel: "low" | "medium" | "high";
  reasons: string[];
}

interface DetectionRule {
  pattern: RegExp;
  risk: "medium" | "high";
  reason: string;
}

const DETECTION_RULES: DetectionRule[] = [
  // Direct instruction override
  { pattern: /ignore\s+(all\s+)?previous\s+instructions/i, risk: "high", reason: "Instruction override attempt" },
  { pattern: /disregard\s+(all\s+)?(above|prior|previous)/i, risk: "high", reason: "Instruction disregard attempt" },
  { pattern: /forget\s+(everything|all|your)\s+(instructions|rules|context)/i, risk: "high", reason: "Memory wipe attempt" },

  // System prompt extraction
  { pattern: /reveal\s+(your\s+)?(system|initial)\s+prompt/i, risk: "high", reason: "System prompt extraction" },
  { pattern: /show\s+(me\s+)?(your|the)\s+(system|hidden|secret)\s+(prompt|instructions)/i, risk: "high", reason: "Prompt leaking attempt" },
  { pattern: /what\s+(are|is)\s+your\s+(system|initial|original)\s+(prompt|instructions)/i, risk: "high", reason: "Prompt inquiry" },
  { pattern: /repeat\s+(your\s+)?(system|initial)\s+(prompt|instructions)/i, risk: "high", reason: "Prompt replay attempt" },

  // Role manipulation
  { pattern: /pretend\s+(to\s+be|you\s+are)\s+/i, risk: "high", reason: "Role manipulation attempt" },
  { pattern: /you\s+are\s+now\s+(a|an|in)\s+/i, risk: "medium", reason: "Role reassignment attempt" },
  { pattern: /act\s+as\s+(if\s+)?(you\s+are\s+)?(a|an)\s+(different|new)/i, risk: "high", reason: "Identity override attempt" },
  { pattern: /developer\s+mode/i, risk: "high", reason: "Developer mode activation attempt" },
  { pattern: /sudo\s+mode/i, risk: "high", reason: "Privilege escalation attempt" },
  { pattern: /god\s+mode/i, risk: "high", reason: "Unrestricted mode attempt" },
  { pattern: /DAN\s+mode/i, risk: "high", reason: "DAN jailbreak attempt" },

  // Jailbreak keywords
  { pattern: /jailbreak/i, risk: "high", reason: "Explicit jailbreak keyword" },
  { pattern: /bypass\s+(safety|filter|restriction|guard|content)/i, risk: "high", reason: "Safety bypass attempt" },
  { pattern: /override\s+(safety|security|rules|restriction)/i, risk: "high", reason: "Override attempt" },

  // Recursive prompt attacks
  { pattern: /\[INST\]/i, risk: "high", reason: "Instruction tag injection (Llama style)" },
  { pattern: /<\|im_start\|>/i, risk: "high", reason: "Chat template injection (OpenAI style)" },
  { pattern: /Human:|Assistant:|System:/i, risk: "medium", reason: "Role prefix injection" },

  // Tool manipulation
  { pattern: /execute\s+(code|command|script|shell)/i, risk: "high", reason: "Code execution attempt" },
  { pattern: /access\s+(file|system|env|environment|database)/i, risk: "high", reason: "System access attempt" },
  { pattern: /read\s+(file|env|secret|key|password)/i, risk: "high", reason: "Data exfiltration attempt" },
  { pattern: /call\s+(api|endpoint|url|webhook)\s+/i, risk: "medium", reason: "External call attempt" },

  // Encoded injection
  { pattern: /&#x[0-9a-f]+;/i, risk: "medium", reason: "HTML entity encoded content" },
  { pattern: /%[0-9a-f]{2}%[0-9a-f]{2}/i, risk: "medium", reason: "URL encoded content detected" },
  { pattern: /\\u[0-9a-f]{4}/i, risk: "medium", reason: "Unicode escape sequence detected" },
  { pattern: /base64[:\s]/i, risk: "medium", reason: "Base64 reference detected" },
];

export function detectInjection(input: string): InjectionReport {
  const reasons: string[] = [];
  let maxRisk: "low" | "medium" | "high" = "low";

  if (!input || typeof input !== "string") {
    return { safe: true, riskLevel: "low", reasons: [] };
  }

  // Decode common encodings to check underlying content
  let decoded = input;
  try {
    decoded = decodeURIComponent(input);
  } catch {
    // keep original
  }

  const targets = [input];
  if (decoded !== input) targets.push(decoded);

  for (const target of targets) {
    for (const rule of DETECTION_RULES) {
      if (rule.pattern.test(target)) {
        reasons.push(rule.reason);
        if (rule.risk === "high") maxRisk = "high";
        else if (rule.risk === "medium" && maxRisk !== "high") maxRisk = "medium";
      }
    }
  }

  // Check for suspiciously long inputs (possible prompt stuffing)
  if (input.length > 10000) {
    reasons.push("Suspiciously long input (possible prompt stuffing)");
    if (maxRisk === "low") maxRisk = "medium";
  }

  // Check for excessive special characters (possible obfuscation)
  const specialCharRatio = (input.replace(/[a-zA-Z0-9\s.,!?'-]/g, "").length) / input.length;
  if (specialCharRatio > 0.4 && input.length > 50) {
    reasons.push("High special character ratio (possible obfuscation)");
    if (maxRisk === "low") maxRisk = "medium";
  }

  const safe = maxRisk !== "high";

  if (!safe) {
    logger.security.injection("Prompt injection detected", { riskLevel: maxRisk, reasons });
  } else if (reasons.length > 0) {
    logger.security.warn("Suspicious input detected", { riskLevel: maxRisk, reasons });
  }

  return { safe, riskLevel: maxRisk, reasons };
}
