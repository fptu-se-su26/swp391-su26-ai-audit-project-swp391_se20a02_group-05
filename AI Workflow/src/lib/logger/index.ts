// ============================================================================
// Enterprise Logger — AI Travel Planner
// Production-safe structured logging with sensitive data redaction
// ============================================================================

type LogLevel = "debug" | "info" | "warn" | "error";

interface LogEntry {
  timestamp: string;
  level: LogLevel;
  category: string;
  message: string;
  data?: Record<string, unknown>;
  duration?: number;
}

const SENSITIVE_KEYS = [
  "apiKey",
  "api_key",
  "GOOGLE_API_KEY",
  "password",
  "secret",
  "token",
  "authorization",
  "cookie",
  "ssn",
  "creditCard",
];

function redact(obj: unknown): unknown {
  if (obj === null || obj === undefined) return obj;
  if (typeof obj === "string") return obj;
  if (Array.isArray(obj)) return obj.map(redact);
  if (typeof obj === "object") {
    const result: Record<string, unknown> = {};
    for (const [key, value] of Object.entries(obj as Record<string, unknown>)) {
      if (SENSITIVE_KEYS.some((sk) => key.toLowerCase().includes(sk.toLowerCase()))) {
        result[key] = "[REDACTED]";
      } else {
        result[key] = redact(value);
      }
    }
    return result;
  }
  return obj;
}

const IS_PRODUCTION = process.env.NODE_ENV === "production";

function formatEntry(entry: LogEntry): string {
  const dur = entry.duration !== undefined ? ` (${entry.duration}ms)` : "";
  return `[${entry.timestamp}] [${entry.level.toUpperCase()}] [${entry.category}] ${entry.message}${dur}`;
}

function shouldLog(level: LogLevel): boolean {
  if (IS_PRODUCTION && level === "debug") return false;
  return true;
}

function log(level: LogLevel, category: string, message: string, data?: Record<string, unknown>, duration?: number) {
  if (!shouldLog(level)) return;

  const entry: LogEntry = {
    timestamp: new Date().toISOString(),
    level,
    category,
    message,
    data: data ? (redact(data) as Record<string, unknown>) : undefined,
    duration,
  };

  const formatted = formatEntry(entry);

  switch (level) {
    case "debug":
      console.debug(formatted, entry.data ?? "");
      break;
    case "info":
      console.info(formatted, entry.data ?? "");
      break;
    case "warn":
      console.warn(formatted, entry.data ?? "");
      break;
    case "error":
      console.error(formatted, entry.data ?? "");
      break;
  }
}

// ---- Category-specific loggers ----

export const logger = {
  debug: (category: string, message: string, data?: Record<string, unknown>) =>
    log("debug", category, message, data),
  info: (category: string, message: string, data?: Record<string, unknown>) =>
    log("info", category, message, data),
  warn: (category: string, message: string, data?: Record<string, unknown>) =>
    log("warn", category, message, data),
  error: (category: string, message: string, data?: Record<string, unknown>) =>
    log("error", category, message, data),

  // Pre-built domain loggers
  ai: {
    request: (message: string, data?: Record<string, unknown>) => log("info", "AI:Request", message, data),
    response: (message: string, data?: Record<string, unknown>, duration?: number) =>
      log("info", "AI:Response", message, data, duration),
    error: (message: string, data?: Record<string, unknown>) => log("error", "AI:Error", message, data),
    retry: (attempt: number, reason: string) =>
      log("warn", "AI:Retry", `Attempt ${attempt}: ${reason}`),
    stream: (message: string) => log("debug", "AI:Stream", message),
  },

  validation: {
    success: (schema: string) => log("debug", "Validation", `Schema "${schema}" passed`),
    failure: (schema: string, errors: string[]) =>
      log("warn", "Validation", `Schema "${schema}" failed`, { errors }),
  },

  security: {
    clean: (message: string) => log("info", "Security", message),
    warn: (message: string, data?: Record<string, unknown>) => log("warn", "Security", message, data),
    block: (message: string, data?: Record<string, unknown>) => log("error", "Security:Block", message, data),
    injection: (message: string, data?: Record<string, unknown>) =>
      log("error", "Security:Injection", message, data),
  },

  mcp: {
    toolCall: (tool: string, data?: Record<string, unknown>) => log("info", "MCP:Tool", `Calling ${tool}`, data),
    toolResult: (tool: string, duration: number) =>
      log("info", "MCP:Tool", `${tool} completed`, undefined, duration),
    toolError: (tool: string, error: string) => log("error", "MCP:Tool", `${tool} failed: ${error}`),
  },

  workflow: {
    step: (step: string, message: string) => log("info", "Workflow", `[${step}] ${message}`),
    complete: (duration: number) => log("info", "Workflow", "Pipeline completed", undefined, duration),
    error: (step: string, error: string) => log("error", "Workflow", `[${step}] ${error}`),
  },
};
