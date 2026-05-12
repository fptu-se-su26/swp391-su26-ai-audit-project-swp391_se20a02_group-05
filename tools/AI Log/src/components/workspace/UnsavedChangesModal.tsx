"use client";

import { Modal, Button, Label } from "@heroui/react";
import { AlertCircle } from "lucide-react";

interface UnsavedChangesModalProps {
  isOpen: boolean;
  onSave: () => void;
  onDiscard: () => void;
  onCancel: () => void;
}

export default function UnsavedChangesModal({
  isOpen,
  onSave,
  onDiscard,
  onCancel,
}: UnsavedChangesModalProps) {
  return (
    <Modal isOpen={isOpen} onOpenChange={(open) => { if (!open) onCancel(); }}>
      <Label>Unsaved Changes</Label>
      <Modal.Backdrop
        isOpen={isOpen}
        onOpenChange={(open) => { if (!open) onCancel(); }}
        isDismissable={false}
        variant="blur"
      >
        <Modal.Container size="sm" placement="center">
          <Modal.Dialog>
            <Modal.Header>
              <div className="flex items-center gap-3">
                <div className="p-2 rounded-full bg-warning/10">
                  <AlertCircle className="w-5 h-5 text-warning" />
                </div>
                <Modal.Heading>Unsaved Changes</Modal.Heading>
              </div>
            </Modal.Header>
            <Modal.Body>
              <p className="text-default-600">
                You have unsaved changes. What would you like to do?
              </p>
            </Modal.Body>
            <Modal.Footer>
              <Button variant="ghost" className="text-danger" onPress={onDiscard}>
                Discard
              </Button>
              <Button variant="secondary" onPress={onCancel}>
                Cancel
              </Button>
              <Button onPress={onSave}>
                Save & Continue
              </Button>
            </Modal.Footer>
          </Modal.Dialog>
        </Modal.Container>
      </Modal.Backdrop>
    </Modal>
  );
}
