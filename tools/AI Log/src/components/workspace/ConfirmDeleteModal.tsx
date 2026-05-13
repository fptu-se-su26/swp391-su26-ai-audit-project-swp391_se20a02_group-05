"use client";

import { Modal, Button } from "@heroui/react";
import { AlertTriangle } from "lucide-react";

interface ConfirmDeleteModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title?: string;
  description?: string;
  itemName?: string;
}

export default function ConfirmDeleteModal({
  isOpen,
  onClose,
  onConfirm,
  title = "Confirm Delete",
  description,
  itemName,
}: ConfirmDeleteModalProps) {
  const handleConfirm = () => {
    onConfirm();
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onOpenChange={(open) => { if (!open) onClose(); }}>
      <Modal.Backdrop 
        isOpen={isOpen} 
        onOpenChange={(open) => { if (!open) onClose(); }}
        variant="blur"
      >
        <Modal.Container size="sm" placement="center">
          <Modal.Dialog>
            <Modal.Header>
              <div className="flex items-center gap-3">
                <div className="p-2 rounded-full bg-danger/10">
                  <AlertTriangle className="w-5 h-5 text-danger" />
                </div>
                <Modal.Heading>{title}</Modal.Heading>
              </div>
            </Modal.Header>
            <Modal.Body>
              <p className="text-default-600">
                {description || (
                  <>
                    This action cannot be undone. Are you sure you want to delete
                    {itemName ? <strong className="text-foreground"> &quot;{itemName}&quot;</strong> : " this item"}?
                  </>
                )}
              </p>
            </Modal.Body>
            <Modal.Footer>
              <Button variant="ghost" onPress={onClose}>
                Cancel
              </Button>
              <Button variant="primary" className="bg-danger text-white hover:bg-danger/90" onPress={handleConfirm}>
                Delete
              </Button>
            </Modal.Footer>
          </Modal.Dialog>
        </Modal.Container>
      </Modal.Backdrop>
    </Modal>
  );
}
