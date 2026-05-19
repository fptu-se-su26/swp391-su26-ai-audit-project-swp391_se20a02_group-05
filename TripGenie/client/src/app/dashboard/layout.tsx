"use client";

import React from 'react';
import { useAuth } from '../../features/auth/hooks/use-auth';
import { useSessionTimeout } from '../../hooks/use-session-timeout';
import { SessionTimeoutModal } from '../../components/ui/session-timeout-modal';
import { useRouter, usePathname } from 'next/navigation';
import { Compass, Building2, ShieldAlert, LayoutDashboard, Sparkles } from 'lucide-react';
import Link from 'next/link';
import { AuthAvatar } from '../../components/ui/auth-avatar';
import { useTranslation } from 'react-i18next';

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, logout, isInitialized, isAuthenticated } = useAuth();
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
      <div className="flex min-h-screen w-full items-center justify-center bg-zinc-50 dark:bg-zinc-950 transition-colors duration-300">
        <div className="flex flex-col items-center gap-3">
          <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-zinc-950 dark:border-zinc-50" />
          <span className="text-sm font-medium text-zinc-500 dark:text-zinc-500 font-outfit">
            {t('common:misc.hydratingProfile')}
          </span>
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

  const userRole = user?.role || 'USER';

  return (
    <div className="flex min-h-screen w-full bg-zinc-50 dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100 transition-colors duration-300">
      
      {/* 1. Sidebar Nav Gated (Hidden on Mobile) */}
      <aside className="hidden md:flex flex-col w-64 border-r border-zinc-200/80 dark:border-zinc-900 bg-white/70 dark:bg-zinc-950/60 backdrop-blur-xl shrink-0">
        <div className="p-6 flex items-center gap-2 select-none border-b border-zinc-200/50 dark:border-zinc-900/50">
          <div className="w-8 h-8 rounded-lg bg-zinc-950 dark:bg-white text-white dark:text-zinc-950 flex items-center justify-center shadow-md">
            <Compass size={18} />
          </div>
          <span className="font-bold text-lg tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-zinc-950 to-zinc-600 dark:from-white dark:to-zinc-400 font-outfit">
            {t('common:branding.title')}
          </span>
        </div>

        {/* Sidebar Nav Links */}
        <nav className="flex-1 p-4 space-y-1.5 select-none font-outfit">
          <Link
            href="/dashboard/user"
            className={[
              "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200",
              pathname === '/dashboard/user'
                ? "bg-zinc-100 dark:bg-zinc-900 text-zinc-950 dark:text-zinc-50"
                : "text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-900/40 hover:text-zinc-800 dark:hover:text-zinc-200",
            ].join(' ')}
          >
            <LayoutDashboard size={18} />
            {t('common:dashboard.travelerHub')}
          </Link>

          <Link
            href="/dashboard/chat"
            className={[
              "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200",
              pathname === '/dashboard/chat'
                ? "bg-zinc-100 dark:bg-zinc-900 text-zinc-950 dark:text-zinc-50"
                : "text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-900/40 hover:text-zinc-800 dark:hover:text-zinc-200",
            ].join(' ')}
          >
            <Sparkles size={18} />
            {t('common:dashboard.aiPlanner')}
          </Link>

          {/* Business Link (gated display or direct path) */}
          {(userRole === 'BUSINESS' || userRole === 'ADMIN') && (
            <Link
              href="/dashboard/business"
              className={[
                "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200",
                pathname === '/dashboard/business'
                  ? "bg-zinc-100 dark:bg-zinc-900 text-zinc-950 dark:text-zinc-50"
                  : "text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-900/40 hover:text-zinc-800 dark:hover:text-zinc-200",
              ].join(' ')}
            >
              <Building2 size={18} />
              {t('common:dashboard.partnerConsole')}
            </Link>
          )}

          {/* Admin Link (gated display or direct path) */}
          {userRole === 'ADMIN' && (
            <Link
              href="/dashboard/admin"
              className={[
                "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200",
                pathname === '/dashboard/admin'
                  ? "bg-zinc-100 dark:bg-zinc-900 text-zinc-950 dark:text-zinc-50"
                  : "text-zinc-500 hover:bg-zinc-50 dark:hover:bg-zinc-900/40 hover:text-zinc-800 dark:hover:text-zinc-200",
              ].join(' ')}
            >
              <ShieldAlert size={18} />
              {t('common:dashboard.systemAdmin')}
            </Link>
          )}
        </nav>

        {/* Sidebar Footer User Info block */}
        <div className="p-4 border-t border-zinc-200/50 dark:border-zinc-900/50 flex items-center select-none">
          <div className="flex items-center gap-3 min-w-0">
            <AuthAvatar />
            <div className="flex flex-col min-w-0">
              <span className="font-bold text-sm truncate text-zinc-800 dark:text-zinc-200 font-outfit">
                {user?.fullName}
              </span>
              <span className="text-zinc-400 dark:text-zinc-600 text-[10px] uppercase font-extrabold tracking-wider font-outfit">
                {userRole}
              </span>
            </div>
          </div>
        </div>
      </aside>

      {/* 2. Main content viewport section */}
      <div className="flex flex-1 flex-col min-w-0">
        {/* Top Header navbar (Mobile responsive layout) */}
        <header className="flex h-16 w-full items-center justify-between px-6 border-b border-zinc-200/80 dark:border-zinc-900 bg-white/70 dark:bg-zinc-950/60 backdrop-blur-xl">
          <div className="md:hidden flex items-center gap-2 select-none">
            <Compass size={20} />
            <span className="font-bold text-base tracking-tight font-outfit">
              {t('common:branding.title')}
            </span>
          </div>

          <div className="hidden md:flex items-center gap-1.5 text-xs text-zinc-400 dark:text-zinc-500 font-semibold select-none font-outfit">
            <span>{t('common:dashboard.pages')}</span>
            <span>/</span>
            <span className="capitalize text-zinc-700 dark:text-zinc-300 font-bold">
              {pathname.split('/').pop()?.replace('-', ' ')}
            </span>
          </div>

          <div className="flex items-center gap-4">
            {/* Live activity indicator badge */}
            <span className="flex h-2 w-2 relative shrink-0">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2 w-2 bg-emerald-500"></span>
            </span>
            <span className="text-xs text-zinc-400 dark:text-zinc-600 select-none hidden sm:inline font-outfit font-semibold">
              {t('common:dashboard.sessionProtected')}
            </span>
            
            {/* Mobile Header Dropdown */}
            <div className="md:hidden">
              <AuthAvatar />
            </div>
          </div>
        </header>

        {/* Dynamic page contents wrapper */}
        <main className="flex-1 p-6 md:p-8 overflow-y-auto max-w-7xl w-full mx-auto">
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
