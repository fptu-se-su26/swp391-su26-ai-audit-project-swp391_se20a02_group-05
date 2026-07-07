"use client";

import { Button, Tooltip } from "@heroui/react";
import { Save, HardDrive, Download, Clock, FileText, FileCode, ChevronDown } from "lucide-react";
import { useProjectStore } from "@/store/projectStore";
import { usePathname } from "next/navigation";
import { useState, useEffect, useCallback, useRef } from "react";
import { generateChangelog, generatePrompts, generateAiAudit, generateReflection } from "@/lib/markdown/generators";

function formatTimeAgo(dateStr: string | null): string {
  if (!dateStr) return "Not saved yet";
  const diff = Date.now() - new Date(dateStr).getTime();
  const seconds = Math.floor(diff / 1000);
  if (seconds < 60) return "Just now";
  const minutes = Math.floor(seconds / 60);
  if (minutes < 60) return `${minutes}m ago`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours}h ago`;
  return new Date(dateStr).toLocaleDateString();
}

export default function Navbar() {
  const pathname = usePathname();
  const { activeProjectId, projects, lastSavedAt, persistenceMode, saveProject, saveHandler } = useProjectStore();
  const [saving, setSaving] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [, setTick] = useState(0);

  const isInWorkspace = pathname?.includes("/project/") && activeProjectId;
  const project = activeProjectId ? projects[activeProjectId] : null;
  const lastSaved = activeProjectId ? lastSavedAt[activeProjectId] : null;
  const isNative = persistenceMode === "native";

  const [exportOpen, setExportOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Close dropdown on click outside
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setExportOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Refresh "time ago" every 30s
  useEffect(() => {
    const interval = setInterval(() => setTick((t) => t + 1), 30000);
    return () => clearInterval(interval);
  }, []);

  const handleSave = useCallback(async () => {
    if (!activeProjectId || saving) return;
    setSaving(true);
    try {
      let formSuccess = true;
      if (saveHandler) {
        formSuccess = await saveHandler();
      }
      if (!formSuccess) return;

      const success = await saveProject(activeProjectId);
      if (success) {
        setSaveSuccess(true);
        setTimeout(() => setSaveSuccess(false), 2000);

        const stepMatch = pathname?.match(/\/workspace\/(step[1-5])/);
        if (stepMatch && stepMatch[1]) {
          const stepKey = stepMatch[1];
          localStorage.removeItem(`draft_${activeProjectId}_${stepKey}_v1`);
          window.dispatchEvent(new Event("storage"));
        }
      }
    } finally {
      setSaving(false);
    }
  }, [activeProjectId, saving, saveProject, saveHandler, pathname]);

  const downloadFile = (content: string, filename: string, mimeType: string = "text/markdown") => {
    const element = document.createElement("a");
    const file = new Blob([content], { type: mimeType });
    element.href = URL.createObjectURL(file);
    element.download = filename;
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
  };

  const handleExport = useCallback((key: string) => {
    if (!project || !activeProjectId) return;

    const markdownFiles = [
      { name: "AI_AUDIT_LOG.md", content: generateAiAudit(project) },
      { name: "PROMPTS.md", content: generatePrompts(project) },
      { name: "CHANGELOG.md", content: generateChangelog(project) },
      { name: "REFLECTION.md", content: generateReflection(project) },
    ];

    const jsonFile = {
      name: `${project.metadata.name || "project"}_export.json`,
      content: JSON.stringify(project, null, 2),
      mime: "application/json"
    };

    if (key === "markdown") {
      markdownFiles.forEach((file, index) => {
        setTimeout(() => {
          downloadFile(file.content, file.name);
        }, index * 200);
      });
    } else if (key === "json") {
      downloadFile(jsonFile.content, jsonFile.name, jsonFile.mime);
    } else if (key === "all") {
      const allFiles = [
        ...markdownFiles,
        { name: jsonFile.name, content: jsonFile.content, mime: jsonFile.mime }
      ];
      allFiles.forEach((file, index) => {
        setTimeout(() => {
          downloadFile(file.content, file.name, (file as any).mime || "text/markdown");
        }, index * 200);
      });
    }
  }, [project, activeProjectId]);

  // Ctrl+S shortcut
  const handleKeyDown = useCallback((e: KeyboardEvent) => {
    if ((e.ctrlKey || e.metaKey) && e.key === "s") {
      e.preventDefault();
      if (activeProjectId) {
        handleSave();
      }
    }
  }, [activeProjectId, handleSave]);

  useEffect(() => {
    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [handleKeyDown]);

  return (
    <header className="h-14 border-b border-border bg-surface flex items-center px-6 justify-between shrink-0">
      <div className="flex items-center gap-3">
        <h1 className="text-lg font-bold tracking-tight">SE20A02 - Group 05</h1>
        {project && isInWorkspace && (
          <>
            <span className="text-default-400">/</span>
            <span className="text-sm text-default-600 truncate max-w-[200px]">
              {project.metadata.name || "Untitled Project"}
            </span>
          </>
        )}
      </div>

      {isInWorkspace && project && (
        <div className="flex items-center gap-2">
          {/* Last saved indicator */}
          <div className="flex items-center gap-1.5 text-xs text-default-400 mr-2">
            <Clock className="w-3 h-3" />
            <span className={saveSuccess ? "text-success font-medium" : ""}>
              {saveSuccess ? "Saved ✓" : formatTimeAgo(lastSaved ?? null)}
            </span>
          </div>

          {/* Persistence mode badge */}
          <Tooltip>
            <Button
              className="flex items-center gap-1 text-xs text-default-400 px-2 py-1 min-w-0 h-auto rounded-md bg-surface-secondary/50 cursor-help data-[hover=true]:bg-surface-secondary/70 transition-colors border-none"
              aria-label="View persistence mode"
            >
              {isNative ? (
                <HardDrive className="w-3 h-3" />
              ) : (
                <Download className="w-3 h-3" />
              )}
              <span className="hidden sm:inline">{isNative ? "Native" : "Download"}</span>
            </Button>
            <Tooltip.Content>
              {isNative
                ? "Native file access — saves directly to your file"
                : "Download mode — overwrite-in-place unavailable in this browser"}
            </Tooltip.Content>
          </Tooltip>

          {/* Save button */}
          <Button
            size="sm"
            variant="secondary"
            onPress={handleSave}
            isDisabled={saving}
          >
            <Save className="w-4 h-4 mr-1.5 inline" />
            Save
          </Button>

          {/* Export dropdown */}
          <div className="relative" ref={dropdownRef}>
            <Button
              size="sm"
              variant="ghost"
              className="flex items-center gap-1.5"
              onPress={() => setExportOpen(!exportOpen)}
            >
              <Download className="w-4 h-4" />
              Export
              <ChevronDown className={`w-3.5 h-3.5 opacity-60 transition-transform duration-200 ${exportOpen ? 'rotate-180' : ''}`} />
            </Button>

            {exportOpen && (
              <div className="absolute right-0 mt-2 w-52 rounded-xl border border-border bg-surface-secondary shadow-lg z-50 py-1.5 animate-in fade-in slide-in-from-top-2 duration-150">
                <button
                  type="button"
                  onClick={() => {
                    handleExport("markdown");
                    setExportOpen(false);
                  }}
                  className="w-full flex items-center gap-2 px-3 py-2 text-sm text-default-800 hover:bg-surface/60 transition-colors text-left"
                >
                  <FileText className="w-4 h-4 text-default-500" />
                  <span>Export as Markdown</span>
                </button>
                <button
                  type="button"
                  onClick={() => {
                    handleExport("json");
                    setExportOpen(false);
                  }}
                  className="w-full flex items-center gap-2 px-3 py-2 text-sm text-default-800 hover:bg-surface/60 transition-colors text-left"
                >
                  <FileCode className="w-4 h-4 text-default-500" />
                  <span>Export as JSON</span>
                </button>
                <button
                  type="button"
                  onClick={() => {
                    handleExport("all");
                    setExportOpen(false);
                  }}
                  className="w-full flex items-center gap-2 px-3 py-2 text-sm text-primary font-medium hover:bg-primary/5 transition-colors text-left border-t border-border mt-1 pt-2"
                >
                  <Download className="w-4 h-4 text-primary" />
                  <span>Export All Files</span>
                </button>
              </div>
            )}
          </div>
        </div>
      )}
    </header>
  );
}
