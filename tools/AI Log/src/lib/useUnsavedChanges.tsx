"use client";

import { useEffect, useCallback, useState, useRef } from "react";
import { useRouter } from "next/navigation";
import UnsavedChangesModal from "@/components/workspace/UnsavedChangesModal";

interface UseUnsavedChangesOptions {
  isDirty: boolean;
  onSave: () => void | Promise<void>;
}

/**
 * Hook to warn users about unsaved changes before navigating away.
 *
 * Returns:
 * - `UnsavedModal` — render this component in your form
 * - `guardNavigation(url)` — call this instead of `router.push(url)`
 */
export function useUnsavedChanges({ isDirty, onSave }: UseUnsavedChangesOptions) {
  const router = useRouter();
  const [showModal, setShowModal] = useState(false);
  const pendingUrl = useRef<string | null>(null);

  // Browser beforeunload
  useEffect(() => {
    if (!isDirty) return;

    const handler = (e: BeforeUnloadEvent) => {
      e.preventDefault();
    };

    window.addEventListener("beforeunload", handler);
    return () => window.removeEventListener("beforeunload", handler);
  }, [isDirty]);

  const guardNavigation = useCallback((url: string) => {
    if (isDirty) {
      pendingUrl.current = url;
      setShowModal(true);
    } else {
      router.push(url);
    }
  }, [isDirty, router]);

  const handleSave = useCallback(async () => {
    await onSave();
    setShowModal(false);
    if (pendingUrl.current) {
      router.push(pendingUrl.current);
      pendingUrl.current = null;
    }
  }, [onSave, router]);

  const handleDiscard = useCallback(() => {
    setShowModal(false);
    if (pendingUrl.current) {
      router.push(pendingUrl.current);
      pendingUrl.current = null;
    }
  }, [router]);

  const handleCancel = useCallback(() => {
    setShowModal(false);
    pendingUrl.current = null;
  }, []);

  const Modal = useCallback(() => (
    <UnsavedChangesModal
      isOpen={showModal}
      onSave={handleSave}
      onDiscard={handleDiscard}
      onCancel={handleCancel}
    />
  ), [showModal, handleSave, handleDiscard, handleCancel]);

  return {
    UnsavedModal: Modal,
    guardNavigation,
  };
}
