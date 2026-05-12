import { Project, DataProjectFile } from "@/types/project";

const APP_VERSION = "1.0.0";

// ─── Feature Detection ────────────────────────────────────────────

export type PersistenceMode = "native" | "fallback";

export function getPersistenceMode(): PersistenceMode {
  if (typeof window !== "undefined" && "showSaveFilePicker" in window) {
    return "native";
  }
  return "fallback";
}

// ─── File Name Helpers ────────────────────────────────────────────

export function generateFileName(projectName: string): string {
  const kebab = projectName
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-|-$/g, "");
  return `${kebab || "project"}.data.json`;
}

// ─── Serialization ────────────────────────────────────────────────

export function serializeProject(project: Project, fileName?: string): string {
  const dataFile: DataProjectFile = {
    schemaVersion: "1.0",
    appVersion: APP_VERSION,
    metadata: {
      createdAt: project.createdAt,
      updatedAt: new Date().toISOString(),
      fileName: fileName || generateFileName(project.metadata.name),
    },
    project,
  };
  return JSON.stringify(dataFile, null, 2);
}

// ─── Validation ───────────────────────────────────────────────────

export interface ValidationResult {
  valid: boolean;
  errors: string[];
  data?: DataProjectFile;
}

export function validateProjectFile(raw: unknown): ValidationResult {
  const errors: string[] = [];

  if (!raw || typeof raw !== "object") {
    return { valid: false, errors: ["File does not contain valid JSON data."] };
  }

  const data = raw as Record<string, unknown>;

  if (!data.schemaVersion) {
    errors.push("Missing schemaVersion field.");
  }

  if (!data.project || typeof data.project !== "object") {
    errors.push("Missing or invalid project data.");
  }

  const project = data.project as Record<string, unknown> | undefined;

  if (project) {
    if (!project.id || typeof project.id !== "string") {
      errors.push("Project is missing a valid ID.");
    }
    if (!project.metadata || typeof project.metadata !== "object") {
      errors.push("Project is missing metadata.");
    }
  }

  if (errors.length > 0) {
    return { valid: false, errors };
  }

  return { valid: true, errors: [], data: raw as DataProjectFile };
}

// ─── Ensure Backward Compatibility ───────────────────────────────

/** Patch older project data to include fields added in newer versions */
export function migrateProject(project: Project): Project {
  return {
    ...project,
    changelogSummary: project.changelogSummary || {
      completedFeatures: "",
      unfinishedFeatures: "",
      majorImprovements: "",
      overallSummary: "",
      futureImprovements: "",
    },
    prompts: (project.prompts || []).map((p) => ({
      ...p,
      importanceExplanation: p.importanceExplanation || "",
    })),
    aiAudit: {
      ...project.aiAudit,
      auditEntries: (project.aiAudit?.auditEntries || []).map((e) => ({
        ...e,
        linkedPromptIds: e.linkedPromptIds || [],
        evidence: (e.evidence || []).map((ev) => ({
          description: "",
          fileName: "",
          thumbnail: "",
          ...ev,
          timestamp: ev.timestamp || new Date().toISOString(),
        })),
      })),
    },
  };
}

// ─── File System Access API (Native Mode) ─────────────────────────

const FILE_TYPE_FILTER = {
  description: "AI Workflow Data Files",
  accept: { "application/json": [".json" as const] },
};

export async function saveProjectToFile(
  project: Project,
  existingHandle?: FileSystemFileHandle | null
): Promise<FileSystemFileHandle | null> {
  const fileName = generateFileName(project.metadata.name);
  const content = serializeProject(project, fileName);

  if (getPersistenceMode() === "native" && existingHandle) {
    try {
      const writable = await existingHandle.createWritable();
      await writable.write(content);
      await writable.close();
      return existingHandle;
    } catch {
      // If permission denied or handle stale, fall through to saveAs
    }
  }

  // Try native Save As
  if (getPersistenceMode() === "native") {
    try {
      const handle = await (window as unknown as { showSaveFilePicker: (opts: unknown) => Promise<FileSystemFileHandle> }).showSaveFilePicker({
        suggestedName: fileName,
        types: [FILE_TYPE_FILTER],
      });
      const writable = await handle.createWritable();
      await writable.write(content);
      await writable.close();
      return handle;
    } catch (err) {
      if ((err as Error).name === "AbortError") return null; // User cancelled
      throw err;
    }
  }

  // Fallback: download
  downloadFile(content, fileName);
  return null;
}

export async function saveProjectAs(
  project: Project
): Promise<FileSystemFileHandle | null> {
  const fileName = generateFileName(project.metadata.name);
  const content = serializeProject(project, fileName);

  if (getPersistenceMode() === "native") {
    try {
      const handle = await (window as unknown as { showSaveFilePicker: (opts: unknown) => Promise<FileSystemFileHandle> }).showSaveFilePicker({
        suggestedName: fileName,
        types: [FILE_TYPE_FILTER],
      });
      const writable = await handle.createWritable();
      await writable.write(content);
      await writable.close();
      return handle;
    } catch (err) {
      if ((err as Error).name === "AbortError") return null;
      throw err;
    }
  }

  downloadFile(content, fileName);
  return null;
}

export async function openProjectFile(): Promise<{
  project: Project;
  handle: FileSystemFileHandle | null;
} | null> {
  let content: string;
  let handle: FileSystemFileHandle | null = null;

  if (getPersistenceMode() === "native") {
    try {
      const [fileHandle] = await (window as unknown as { showOpenFilePicker: (opts: unknown) => Promise<FileSystemFileHandle[]> }).showOpenFilePicker({
        types: [FILE_TYPE_FILTER],
        multiple: false,
      });
      handle = fileHandle;
      const file = await fileHandle.getFile();
      content = await file.text();
    } catch (err) {
      if ((err as Error).name === "AbortError") return null;
      throw err;
    }
  } else {
    // Fallback: file input
    const result = await openFileViaInput();
    if (!result) return null;
    content = result;
  }

  let parsed: unknown;
  try {
    parsed = JSON.parse(content);
  } catch {
    throw new Error("File is not valid JSON.");
  }

  const validation = validateProjectFile(parsed);
  if (!validation.valid || !validation.data) {
    throw new Error(`Invalid project file: ${validation.errors.join(", ")}`);
  }

  const project = migrateProject(validation.data.project);
  return { project, handle };
}

// ─── Fallback Helpers ─────────────────────────────────────────────

function downloadFile(content: string, filename: string) {
  const blob = new Blob([content], { type: "application/json" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}

function openFileViaInput(): Promise<string | null> {
  return new Promise((resolve) => {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = ".json,.data.json";
    input.onchange = async () => {
      const file = input.files?.[0];
      if (!file) {
        resolve(null);
        return;
      }
      const text = await file.text();
      resolve(text);
    };
    input.click();
  });
}
