"use client";

import React from 'react';
import {
  Modal,
  ModalBackdrop,
  ModalContainer,
  ModalDialog,
  ModalBody,
  ModalFooter,
} from '@heroui/react';
import { Button } from './button';
import { AlertTriangle, Clock } from 'lucide-react';

interface SessionTimeoutModalProps {
  isOpen: boolean;
  countdown: number; // Seconds remaining
  onExtend: () => void;
  onLogout: () => void;
}

export const SessionTimeoutModal: React.FC<SessionTimeoutModalProps> = ({
  isOpen,
  countdown,
  onExtend,
  onLogout,
}) => {
  const formatTime = (secs: number) => {
    const minutes = Math.floor(secs / 60);
    const seconds = secs % 60;
    return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
  };

  const isCritical = countdown <= 30;

  return (
    <Modal isOpen={isOpen} onOpenChange={(open) => { if (!open) onLogout(); }}>
      {/* Cryptographically secure glassmorphic backdrop */}
      <ModalBackdrop 
        isDismissable={false} 
        className="fixed inset-0 bg-zinc-950/40 dark:bg-zinc-950/70 backdrop-blur-md z-50 flex items-center justify-center animate-fade-in"
      />
      
      {/* Floating Center Dialog Container */}
      <ModalContainer 
        placement="center" 
        className="fixed inset-0 flex items-center justify-center z-50 p-4"
      >
        <ModalDialog className="bg-white/95 dark:bg-zinc-950/95 border border-zinc-200 dark:border-zinc-900 shadow-2xl rounded-2xl max-w-sm w-full mx-auto overflow-hidden outline-none animate-scale-up">
          <ModalBody className="pt-6 px-6 pb-2 text-center flex flex-col items-center">
            {/* Animated Glowing Alert Icon */}
            <div
              className={[
                "w-12 h-12 rounded-full flex items-center justify-center mb-4 transition-all duration-300",
                isCritical
                  ? "bg-red-50 dark:bg-red-950/30 text-red-500 animate-pulse shadow-[0_0_15px_rgba(239,68,68,0.2)]"
                  : "bg-amber-50 dark:bg-amber-950/30 text-amber-500 shadow-[0_0_15px_rgba(245,158,11,0.15)]",
              ].join(' ')}
            >
              {isCritical ? <AlertTriangle size={24} /> : <Clock size={24} />}
            </div>

            <h3 className="text-zinc-900 dark:text-zinc-50 font-bold text-lg mb-1 tracking-tight">
              Session Expiring Soon
            </h3>
            
            <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-4">
              You have been inactive for a while. For security, your session will automatically lock in:
            </p>

            {/* Time Counter Ticker */}
            <div
              className={[
                "text-3xl font-extrabold tracking-tight tabular-nums transition-all duration-300 px-4 py-2 rounded-xl mb-3",
                isCritical
                  ? "text-red-600 dark:text-red-400 bg-red-50/50 dark:bg-red-950/20 scale-105"
                  : "text-zinc-800 dark:text-zinc-100 bg-zinc-50 dark:bg-zinc-900/50",
              ].join(' ')}
            >
              {formatTime(countdown)}
            </div>
          </ModalBody>
          
          <ModalFooter className="flex flex-col sm:flex-row gap-2 px-6 pb-6 pt-2 border-none">
            <Button
              variant="bordered"
              onClick={onLogout}
              className="w-full sm:order-1 text-zinc-500 dark:text-zinc-400 border-zinc-200 dark:border-zinc-800"
            >
              Sign Out
            </Button>
            <Button
              variant="solid"
              onClick={onExtend}
              className={[
                "w-full sm:order-2",
                isCritical
                  ? "bg-red-600 hover:bg-red-500 text-white dark:bg-red-500 dark:hover:bg-red-400 dark:text-zinc-950 border-none"
                  : "",
              ].join(' ')}
            >
              Extend Session
            </Button>
          </ModalFooter>
        </ModalDialog>
      </ModalContainer>
    </Modal>
  );
};
export default SessionTimeoutModal;
