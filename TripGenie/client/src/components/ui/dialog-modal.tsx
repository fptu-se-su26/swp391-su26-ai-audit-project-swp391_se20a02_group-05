"use client";

import React, { useEffect, useState } from 'react';
import { Modal } from '@heroui/react';

interface DialogModalProps {
  isOpen: boolean;
  onOpenChange: (isOpen: boolean) => void;
  title: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'cover' | 'full';
  isDismissable?: boolean;
  isKeyboardDismissDisabled?: boolean;
}

export const DialogModal: React.FC<DialogModalProps> = ({
  isOpen,
  onOpenChange,
  title,
  children,
  footer,
  size = 'md',
  isDismissable = true,
  isKeyboardDismissDisabled = false,
}) => {
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    let active = true;
    Promise.resolve().then(() => {
      if (active) {
        setIsMounted(true);
      }
    });
    return () => {
      active = false;
    };
  }, []);

  if (!isMounted) return null;

  return (
    <Modal.Backdrop
      isOpen={isOpen}
      onOpenChange={onOpenChange}
      isDismissable={isDismissable}
      isKeyboardDismissDisabled={isKeyboardDismissDisabled}
      className="bg-zinc-950/80 backdrop-blur-sm animate-in fade-in duration-200"
    >
      <Modal.Container size={size}>
        <Modal.Dialog className="w-full max-w-2xl bg-white dark:bg-zinc-950 border border-zinc-200 dark:border-zinc-900 rounded-2xl shadow-2xl p-6 text-left relative focus-visible:outline-none focus:outline-none">
          <Modal.CloseTrigger className="absolute right-4 top-4 p-1 rounded-full hover:bg-zinc-100 dark:hover:bg-zinc-900 text-zinc-400 hover:text-zinc-500 cursor-pointer transition-colors" />
          <Modal.Header className="mb-4">
            <Modal.Heading className="font-extrabold text-lg text-zinc-900 dark:text-zinc-50">
              {title}
            </Modal.Heading>
          </Modal.Header>
          <Modal.Body className="space-y-4 py-2 text-sm leading-relaxed text-zinc-600 dark:text-zinc-400">
            {children}
          </Modal.Body>
          {footer && (
            <Modal.Footer className="flex justify-end gap-3 pt-4 mt-4 border-t border-zinc-100 dark:border-zinc-900">
              {footer}
            </Modal.Footer>
          )}
        </Modal.Dialog>
      </Modal.Container>
    </Modal.Backdrop>
  );
};

export default DialogModal;
