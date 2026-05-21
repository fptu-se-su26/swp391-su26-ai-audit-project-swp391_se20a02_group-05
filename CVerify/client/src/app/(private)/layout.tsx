"use client";

import React from 'react';
import { useAuth } from '../../features/auth/hooks/use-auth';
import { useSessionTimeout } from '../../hooks/use-session-timeout';
import { SessionTimeoutModal } from '../../components/ui/session-timeout-modal';
import { AppBreadcrumbs } from '../../components/ui/app-breadcrumbs';
import { Sidebar } from '../../components/ui/sidebar';
import { Header } from '../../components/ui/header';
import { AuthGuard } from '../../features/auth/guards/auth-guard';

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { logout } = useAuth();
  const { showWarning, secondsRemaining, extendSession } = useSessionTimeout();

  // Handle logout with redirection
  const handleSignOut = async () => {
    await logout(true);
  };

  return (
    <AuthGuard>
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
    </AuthGuard>
  );
}

