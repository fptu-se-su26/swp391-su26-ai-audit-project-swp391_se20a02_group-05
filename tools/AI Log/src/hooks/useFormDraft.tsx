"use client";

import React, { useEffect, useState, useRef, useMemo } from "react";
import { UseFormWatch, UseFormReset } from "react-hook-form";
import { Sparkles, Check, Loader2, AlertCircle } from "lucide-react";

interface UseFormDraftOptions<TFieldValues extends Record<string, any>> {
  projectId: string;
  stepKey: string;
  watch: UseFormWatch<TFieldValues>;
  reset: UseFormReset<TFieldValues>;
  originalData: TFieldValues;
  schemaVersion?: string;
}

export type SaveStatus = "idle" | "saving" | "saved" | "restored";

export function useFormDraft<TFieldValues extends Record<string, any>>({
  projectId,
  stepKey,
  watch,
  reset,
  originalData,
  schemaVersion = "v1"
}: UseFormDraftOptions<TFieldValues>) {
  const [saveStatus, setSaveStatus] = useState<SaveStatus>("idle");
  const draftKey = `draft_${projectId}_${stepKey}_${schemaVersion}`;
  const isRestoring = useRef(false);
  const lastSavedValueRef = useRef<string>("");
  const debounceTimer = useRef<NodeJS.Timeout | null>(null);

  const currentValues = watch();
  const currentValuesString = useMemo(() => JSON.stringify(currentValues || {}), [currentValues]);
  const originalDataString = useMemo(() => JSON.stringify(originalData || {}), [originalData]);

  // represents whether current form state differs from database state in Zustand store
  const isActuallyDirty = useMemo(() => {
    return currentValuesString !== originalDataString;
  }, [currentValuesString, originalDataString]);

  // 1. Restore Unsaved Draft on Mount or when project changes
  useEffect(() => {
    if (typeof window === "undefined" || !projectId) return;

    const rawDraft = localStorage.getItem(draftKey);
    if (rawDraft) {
      try {
        const parsed = JSON.parse(rawDraft);
        if (parsed && parsed.data) {
          // Compare with database originalData to ensure there actually is a difference
          const hasDifference = JSON.stringify(parsed.data) !== originalDataString;
          if (hasDifference) {
            isRestoring.current = true;
            reset(parsed.data);
            setSaveStatus("restored");
            lastSavedValueRef.current = JSON.stringify(parsed.data);
            
            // Revert status to idle after a few seconds
            const timer = setTimeout(() => {
              setSaveStatus("idle");
              isRestoring.current = false;
            }, 3000);
            
            return () => clearTimeout(timer);
          }
        }
      } catch (e) {
        console.error("Failed to parse local draft data safely:", e);
      }
    }

    // If no draft is restored, initialize the form with original data from the project store
    reset(originalData);
    lastSavedValueRef.current = originalDataString;
  }, [projectId, draftKey, reset, originalDataString]);

  // 2. Watch for changes and Auto-save
  useEffect(() => {
    if (typeof window === "undefined" || isRestoring.current || !projectId) return;

    // If there is no difference from original data, clear draft from localStorage
    if (!isActuallyDirty) {
      if (debounceTimer.current) clearTimeout(debounceTimer.current);
      if (localStorage.getItem(draftKey)) {
        localStorage.removeItem(draftKey);
        setSaveStatus("idle");
      }
      return;
    }

    // Skip writing to localStorage if values haven't changed from the last written values
    if (currentValuesString === lastSavedValueRef.current) {
      return;
    }

    // Debounce saving to localStorage (~500ms)
    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    
    setSaveStatus("saving");

    debounceTimer.current = setTimeout(() => {
      try {
        const parsedValues = JSON.parse(currentValuesString);
        localStorage.setItem(
          draftKey,
          JSON.stringify({
            data: parsedValues,
            timestamp: Date.now(),
            version: schemaVersion
          })
        );
        lastSavedValueRef.current = currentValuesString;
        setSaveStatus("saved");
      } catch (e) {
        console.error("Failed to auto-save draft safely:", e);
        setSaveStatus("idle");
      }
    }, 500); // 500ms debounced auto-save

    return () => {
      if (debounceTimer.current) clearTimeout(debounceTimer.current);
    };
  }, [currentValuesString, isActuallyDirty, originalDataString, draftKey, projectId, schemaVersion]);

  // 3. Render Status Component
  const DraftStatusIndicator = () => {
    if (saveStatus === "restored") {
      return (
        <div className="flex items-center gap-1.5 bg-warning/10 text-warning border border-warning/20 px-3 py-1 rounded-full text-xs font-bold shadow-sm animate-pulse">
          <Sparkles className="w-3.5 h-3.5 shrink-0" />
          <span>Draft restored</span>
        </div>
      );
    }

    if (saveStatus === "saving") {
      return (
        <div className="flex items-center gap-1.5 bg-default-100 text-default-500 border border-border px-3 py-1 rounded-full text-xs font-medium">
          <Loader2 className="w-3.5 h-3.5 animate-spin shrink-0" />
          <span>Saving draft...</span>
        </div>
      );
    }

    if (saveStatus === "saved") {
      return (
        <div className="flex items-center gap-1.5 bg-success/15 text-success border border-success/30 px-3 py-1 rounded-full text-xs font-bold shadow-sm">
          <Check className="w-3.5 h-3.5 shrink-0" />
          <span>Draft saved locally</span>
        </div>
      );
    }

    if (isActuallyDirty) {
      return (
        <div className="flex items-center gap-1.5 bg-warning/15 text-warning border border-warning/30 px-3 py-1 rounded-full text-xs font-bold shadow-sm animate-pulse">
          <AlertCircle className="w-3.5 h-3.5 shrink-0" />
          <span>Unsaved changes</span>
        </div>
      );
    }

    return null;
  };

  return {
    saveStatus,
    DraftStatusIndicator,
    isActuallyDirty
  };
}
