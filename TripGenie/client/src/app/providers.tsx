"use client";

import React, { useEffect } from 'react';
import { useAuth } from '../features/auth/hooks/use-auth';
import { Compass } from 'lucide-react';
import { usePathname } from 'next/navigation';
import { Toast, toast, Typography } from '@heroui/react';
import i18n from '../lib/i18n';
import { useThemeStore } from '../hooks/use-theme-store';

export function Providers({ children, locale }: { children: React.ReactNode; locale: string }) {
  const { initializeSession, isInitialized } = useAuth();
  const initializeTheme = useThemeStore(state => state.initializeTheme);
  const pathname = usePathname();

  // Synchronize server-resolved locale to client i18n instance before hydration (Server only)
  if (typeof window === 'undefined') {
    if (i18n.language !== locale) {
      i18n.changeLanguage(locale);
    }
  }

  // Initialize theme on client-side boot to align storage and cookies
  useEffect(() => {
    initializeTheme();
  }, [initializeTheme]);

  // Handle client-side changes post-hydration cleanly to satisfy React Compiler constraints
  useEffect(() => {
    if (i18n.language !== locale) {
      i18n.changeLanguage(locale);
    }
  }, [locale]);

  // Run secure session hydration immediately on app boots
  useEffect(() => {
    initializeSession();
  }, [initializeSession]);

  // Clear toasts on navigation to decouple page contexts
  useEffect(() => {
    if (toast && typeof toast.clear === 'function') {
      toast.clear();
    }
  }, [pathname]);

  const isDashboardRoute = ['/admin', '/business', '/user'].some(p => pathname.startsWith(p));
 
  // Hydration safety gate: only block UI on protected dashboard routes to prevent flashing protected layout
  if (isDashboardRoute && !isInitialized) {
    return (
      <div className="flex min-h-screen w-full items-center justify-center bg-background transition-colors duration-300">
        <div className="flex flex-col items-center gap-4 text-center select-none animate-pulse">
          {/* Pulsing Brand Logo */}
          <div className="w-14 h-14 rounded-2xl bg-foreground text-background flex items-center justify-center shadow-surface border border-border">
            <Compass size={32} className="text-background animate-spin-slow" />
          </div>
          
          <div className="space-y-1">
            <Typography type="body-sm" className="font-extrabold tracking-tight text-foreground">
              TripGenie AI
            </Typography>
            <Typography type="body-xs" className="text-muted font-medium mt-1">
              {locale === 'vi' ? 'Đang khởi tạo phiên làm việc bảo mật...' : 'Establishing secure session...'}
            </Typography>
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
