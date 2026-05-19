"use client";

import React from 'react';
import { useAuth } from '../../features/auth/hooks/use-auth';
import { useSessionTimeout } from '../../hooks/use-session-timeout';
import { SessionTimeoutModal } from '../../components/ui/session-timeout-modal';
import { useRouter, usePathname } from 'next/navigation';
import { useTranslation } from 'react-i18next';
import { Spinner, Typography } from '@heroui/react';
import { AppBreadcrumbs } from '../../components/ui/app-breadcrumbs';
import { Sidebar } from '../../components/ui/sidebar';
import { Header } from '../../components/ui/header';

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { logout, isInitialized, isAuthenticated } = useAuth();
  const { showWarning, secondsRemaining, extendSession } = useSessionTimeout();
  const router = useRouter();
  const pathname = usePathname();
  const { t } = useTranslation(['common']);

  // Handle logout with redirection
  const handleSignOut = async () => {
    await logout(true);
    router.push('/login');
  };

  // If loading or session is initializing, render clean central loader
  if (!isInitialized) {
    return (
      <div className="flex min-h-screen w-full items-center justify-center bg-background transition-colors duration-300">
        <div className="flex flex-col items-center gap-3">
          <Spinner size="lg" color="accent" />
          <Typography type="body-sm" className="text-muted font-medium font-outfit select-none">
            {t('common:misc.hydratingProfile', { defaultValue: 'Hydrating profile...' })}
          </Typography>
        </div>
      </div>
    );
  }

  // Double security guard redirect if somehow accessed unauthenticated
  if (isInitialized && !isAuthenticated) {
    if (typeof window !== 'undefined') {
      router.replace(`/login?callbackUrl=${encodeURIComponent(pathname)}`);
    }
    return null;
  }

  return (
    <div className="grid min-h-screen w-full grid-cols-1 md:grid-cols-[auto_1fr] bg-background text-foreground transition-colors duration-300">

      {/* 1. Dynamic Recursive Sidebar (Desktop + Mobile overlay drawer) */}
      <Sidebar />

      {/* 2. Main content viewport section */}
      <div className="flex flex-col min-w-0 h-screen overflow-hidden">
        {/* Global Header shell */}
        <Header />

        {/* Dynamic page contents wrapper */}
        <main className="flex-1 p-6 md:p-8 overflow-y-auto max-w-7xl w-full mx-auto">
          {/* Shifted page-level Breadcrumbs */}
          <div className="mb-6">
            <AppBreadcrumbs />
          </div>
          {children}
        </main>
      </div>

      {/* 3. Session Inactivity Countdown Modal Overlay */}
      <SessionTimeoutModal
        isOpen={showWarning}
        countdown={secondsRemaining}
        onExtend={extendSession}
        onLogout={handleSignOut}
      />
    </div>
  );
}

