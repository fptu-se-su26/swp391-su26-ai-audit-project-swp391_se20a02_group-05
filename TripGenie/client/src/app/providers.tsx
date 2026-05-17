"use client";

import React, { useEffect } from 'react';
import { useAuth } from '../hooks/use-auth';
import { Compass } from 'lucide-react';
import { usePathname } from 'next/navigation';
import { Toast, toast } from '@heroui/react';

export function Providers({ children }: { children: React.ReactNode }) {
  const { initializeSession, isInitialized } = useAuth();
  const pathname = usePathname();

  // Run secure session hydration immediately on app boots
  useEffect(() => {
    initializeSession();
  }, []);

  // Clear toasts on navigation to decouple page contexts
  useEffect(() => {
    if (toast && typeof toast.clear === 'function') {
      toast.clear();
    }
  }, [pathname]);

  const isDashboardRoute = pathname.startsWith('/dashboard');
 
  // Hydration safety gate: only block UI on protected dashboard routes to prevent flashing protected layout
  if (isDashboardRoute && !isInitialized) {
    return (
      <div className="flex min-h-screen w-full items-center justify-center bg-zinc-50 dark:bg-zinc-950 transition-colors duration-300">
        <div className="flex flex-col items-center gap-4 text-center select-none animate-pulse">
          {/* Pulsing Brand Logo */}
          <div className="w-14 h-14 rounded-2xl bg-zinc-950 dark:bg-white text-white dark:text-zinc-950 flex items-center justify-center shadow-xl border border-zinc-200/20">
            <Compass size={32} className="text-white dark:text-zinc-950 animate-spin-slow" />
          </div>
          
          <div className="space-y-1">
            <h3 className="font-extrabold tracking-tight text-zinc-800 dark:text-zinc-100 text-base">
              TripGenie AI
            </h3>
            <p className="text-zinc-400 dark:text-zinc-500 text-xs font-medium">
              Establishing cryptographically secure session...
            </p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <>
      <Toast.Provider />
      {children}
    </>
  );
}
export default Providers;
