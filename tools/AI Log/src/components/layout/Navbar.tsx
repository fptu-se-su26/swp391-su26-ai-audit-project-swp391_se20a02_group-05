"use client";

import { Button, Tooltip } from "@heroui/react";
import { Save, SaveAll, HardDrive, Download, Clock } from "lucide-react";
import { useProjectStore } from "@/store/projectStore";
import { usePathname } from "next/navigation";
import { useState, useEffect, useCallback } from "react";

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
  const { activeProjectId, projects, lastSavedAt, persistenceMode, saveProject, saveProjectAs } = useProjectStore();
  const [saving, setSaving] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [, setTick] = useState(0);

  const isInWorkspace = pathname?.includes("/project/") && activeProjectId;
  const project = activeProjectId ? projects[activeProjectId] : null;
  const lastSaved = activeProjectId ? lastSavedAt[activeProjectId] : null;
  const isNative = persistenceMode === "native";

  // Refresh "time ago" every 30s
  useEffect(() => {
    const interval = setInterval(() => setTick((t) => t + 1), 30000);
    return () => clearInterval(interval);
  }, []);

  const handleSave = useCallback(async () => {
    if (!activeProjectId || saving) return;
    setSaving(true);
    try {
      const success = await saveProject(activeProjectId);
      if (success) {
        setSaveSuccess(true);
        setTimeout(() => setSaveSuccess(false), 2000);
      }
    } finally {
      setSaving(false);
    }
  }, [activeProjectId, saving, saveProject]);

  const handleSaveAs = useCallback(async () => {
    if (!activeProjectId || saving) return;
    setSaving(true);
    try {
      const success = await saveProjectAs(activeProjectId);
      if (success) {
        setSaveSuccess(true);
        setTimeout(() => setSaveSuccess(false), 2000);
      }
    } finally {
      setSaving(false);
    }
  }, [activeProjectId, saving, saveProjectAs]);

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
        <h1 className="text-lg font-bold tracking-tight">Group 05</h1>
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

          {/* Save As button */}
          <Button
            size="sm"
            variant="ghost"
            onPress={handleSaveAs}
            isDisabled={saving}
          >
            <SaveAll className="w-4 h-4 mr-1.5 inline" />
            <span className="hidden sm:inline">Save As</span>
          </Button>
        </div>
      )}
    </header>
  );
}
