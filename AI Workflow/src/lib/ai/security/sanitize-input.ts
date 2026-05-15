// ============================================================================
// Input Sanitization — Prompt Security Layer
// Removes hidden chars, normalizes unicode, strips injection patterns
// ============================================================================

import { logger } from "@/lib/logger";

const CONTROL_CHAR_REGEX = /[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F]/g;
const PROMPT_SEPARATOR_REGEX = /[-=]{5,}|[#]{3,}\s*(system|instruction|prompt|context)/gi;
const XML_INJECTION_REGEX = /<\/?(?:system|instruction|context|prompt|role|assistant|user|tool)[^>]*>/gi;
const MARKDOWN_JAILBREAK_REGEX = /```(?:system|instruction|override|jailbreak|hack)[^`]*```/gi;
const EXCESSIVE_WHITESPACE_REGEX = /[ \t]{4,}/g;
const EXCESSIVE_NEWLINES_REGEX = /\n{4,}/g;
const UNICODE_HOMOGLYPH_MAP: Record<string, string> = {
  "\u0410": "A", "\u0412": "B", "\u0421": "C", "\u0415": "E",
  "\u041D": "H", "\u041A": "K", "\u041C": "M", "\u041E": "O",
  "\u0420": "P", "\u0422": "T", "\u0425": "X",
  "\u0430": "a", "\u0435": "e", "\u043E": "o", "\u0440": "p",
  "\u0441": "c", "\u0443": "y", "\u0445": "x",
};
const ZERO_WIDTH_REGEX = /[\u200B\u200C\u200D\u2060\uFEFF]/g;

export function sanitizeInput(raw: string): string {
  if (!raw || typeof raw !== "string") return "";

  let sanitized = raw;

  // 1. Remove zero-width characters
  sanitized = sanitized.replace(ZERO_WIDTH_REGEX, "");

  // 2. Remove hidden control characters (keep newline, carriage return, tab)
  sanitized = sanitized.replace(CONTROL_CHAR_REGEX, "");

  // 3. Normalize Cyrillic/unicode homoglyphs to Latin
  for (const [homoglyph, latin] of Object.entries(UNICODE_HOMOGLYPH_MAP)) {
    sanitized = sanitized.replaceAll(homoglyph, latin);
  }

  // 4. Normalize unicode (NFC)
  sanitized = sanitized.normalize("NFC");

  // 5. Strip prompt separators that could trick the model
  sanitized = sanitized.replace(PROMPT_SEPARATOR_REGEX, "[REMOVED_SEPARATOR]");

  // 6. Strip XML/tag injection
  sanitized = sanitized.replace(XML_INJECTION_REGEX, "[REMOVED_TAG]");

  // 7. Strip markdown jailbreak attempts
  sanitized = sanitized.replace(MARKDOWN_JAILBREAK_REGEX, "[REMOVED_BLOCK]");

  // 8. Trim excessive whitespace
  sanitized = sanitized.replace(EXCESSIVE_WHITESPACE_REGEX, "  ");
  sanitized = sanitized.replace(EXCESSIVE_NEWLINES_REGEX, "\n\n");

  // 9. Final trim
  sanitized = sanitized.trim();

  if (sanitized !== raw.trim()) {
    logger.security.warn("Input was sanitized", {
      originalLength: raw.length,
      sanitizedLength: sanitized.length,
    });
  }

  return sanitized;
}
