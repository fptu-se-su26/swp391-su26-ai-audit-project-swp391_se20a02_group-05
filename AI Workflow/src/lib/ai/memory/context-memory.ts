// ============================================================================
// Context Memory — Server-side in-memory context for the AI pipeline
// Stores transient generation context (not persisted to localStorage)
// ============================================================================

interface ContextEntry {
  key: string;
  value: string;
  priority: number;
  timestamp: number;
}

class ContextMemoryStore {
  private entries: Map<string, ContextEntry> = new Map();

  set(key: string, value: string, priority = 5): void {
    this.entries.set(key, { key, value, priority, timestamp: Date.now() });
  }

  get(key: string): string | undefined {
    return this.entries.get(key)?.value;
  }

  remove(key: string): void {
    this.entries.delete(key);
  }

  /**
   * Get all context entries sorted by priority (highest first).
   */
  getAll(): ContextEntry[] {
    return Array.from(this.entries.values()).sort((a, b) => a.priority - b.priority);
  }

  /**
   * Compile all context into a single string for prompt injection.
   */
  compile(): string {
    const entries = this.getAll();
    if (entries.length === 0) return "";

    return [
      "CONTEXT MEMORY:",
      ...entries.map((e) => `[Priority ${e.priority}] ${e.key}: ${e.value}`),
    ].join("\n");
  }

  /**
   * Get total character count of all stored context.
   */
  getTokenEstimate(): number {
    let total = 0;
    for (const entry of this.entries.values()) {
      total += entry.value.length;
    }
    // Rough estimate: ~4 chars per token
    return Math.ceil(total / 4);
  }

  clear(): void {
    this.entries.clear();
  }
}

// Singleton for server-side usage
export const contextMemory = new ContextMemoryStore();
