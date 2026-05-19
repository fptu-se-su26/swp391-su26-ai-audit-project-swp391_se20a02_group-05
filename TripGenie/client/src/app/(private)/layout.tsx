"use client";

import React from 'react';
import { useAuth } from '../../features/auth/hooks/use-auth';
import { useSessionTimeout } from '../../hooks/use-session-timeout';
import { SessionTimeoutModal } from '../../components/ui/session-timeout-modal';
import { useRouter, usePathname } from 'next/navigation';
import { Compass, Building2, ShieldAlert, LayoutDashboard, Sparkles, Users, Shield, FileText } from 'lucide-react';
import Link from 'next/link';
import { AuthAvatar } from '../../components/ui/auth-avatar';
import { useTranslation } from 'react-i18next';
import { Spinner, Typography } from '@heroui/react';
import { AppBreadcrumbs } from '../../components/ui/app-breadcrumbs';

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, logout, isInitialized, isAuthenticated, hasPermission } = useAuth();
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
            {t('common:misc.hydratingProfile')}
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

  const userRole = user?.role || 'USER';

  return (
    <div className="flex min-h-screen w-full bg-background text-foreground transition-colors duration-300">

      {/* 1. Sidebar Nav Gated (Hidden on Mobile) */}
      <aside className="hidden md:flex flex-col w-64 border-e border-border bg-background/70 backdrop-blur-xl shrink-0">
        <div className="p-6 flex items-center gap-2 select-none border-b border-separator">
          <div className="w-8 h-8 rounded-lg bg-foreground text-background flex items-center justify-center shadow-md">
            <Compass size={18} />
          </div>
          <Typography type="body-sm" className="font-bold bg-clip-text text-transparent bg-linear-to-r from-foreground to-muted font-outfit">
            {t('common:branding.title')}
          </Typography>
        </div>

        {/* Sidebar Nav Links */}
        <nav className="flex-1 p-4 space-y-1.5 select-none font-outfit">
          <Link
            href="/user"
            className={[
              "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200 focus-visible:ring-2 focus-visible:ring-focus focus-visible:outline-hidden",
              pathname === '/user'
                ? "bg-surface-secondary text-foreground"
                : "text-muted hover:bg-surface-secondary/40 hover:text-foreground",
            ].join(' ')}
          >
            <LayoutDashboard size={18} />
            {t('common:dashboard.travelerHub')}
          </Link>

          <Link
            href="/chat"
            className={[
              "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200 focus-visible:ring-2 focus-visible:ring-focus focus-visible:outline-hidden",
              pathname === '/chat'
                ? "bg-surface-secondary text-foreground"
                : "text-muted hover:bg-surface-secondary/40 hover:text-foreground",
            ].join(' ')}
          >
            <Sparkles size={18} />
            {t('common:dashboard.aiPlanner')}
          </Link>

          {/* Business Link (gated display or direct path) */}
          {(userRole === 'BUSINESS' || userRole === 'ADMIN') && (
            <Link
              href="/business"
              className={[
                "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200 focus-visible:ring-2 focus-visible:ring-focus focus-visible:outline-hidden",
                pathname === '/business'
                  ? "bg-surface-secondary text-foreground"
                  : "text-muted hover:bg-surface-secondary/40 hover:text-foreground",
            ].join(' ')}
          >
            <Building2 size={18} />
            {t('common:dashboard.partnerConsole')}
          </Link>
          )}

          {/* Admin Section (gated display or direct path) */}
          {userRole === 'ADMIN' && (
            <div className="space-y-1">
              <Link
                href="/admin"
                className={[
                  "flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all duration-200 focus-visible:ring-2 focus-visible:ring-focus focus-visible:outline-hidden",
                  pathname === '/admin'
                    ? "bg-surface-secondary text-foreground"
                    : "text-muted hover:bg-surface-secondary/40 hover:text-foreground",
                ].join(' ')}
              >
                <ShieldAlert size={18} />
                {t('common:dashboard.systemAdmin')}
              </Link>
              
              {/* Indented administrative permission-driven sub-routes */}
              <div className="ps-6 space-y-1">
                {hasPermission('users:view:list') && (
                  <Link
                    href="/admin/users"
                    className={[
                      "flex items-center gap-2.5 px-3 py-2 rounded-lg text-xs font-medium transition-all duration-200 focus-visible:ring-2 focus-visible:ring-focus focus-visible:outline-hidden",
                      pathname === '/admin/users'
                        ? "bg-surface-secondary text-foreground font-bold"
                        : "text-muted hover:bg-surface-secondary/40 hover:text-foreground",
                    ].join(' ')}
                  >
                    <Users size={14} />
                    <Typography type="body-xs" className="font-medium text-inherit">{t('common:admin.users')}</Typography>
                  </Link>
                )}

                {hasPermission('roles:view:list') && (
                  <Link
                    href="/admin/roles"
                    className={[
                      "flex items-center gap-2.5 px-3 py-2 rounded-lg text-xs font-medium transition-all duration-200 focus-visible:ring-2 focus-visible:ring-focus focus-visible:outline-hidden",
                      pathname === '/admin/roles'
                        ? "bg-surface-secondary text-foreground font-bold"
                        : "text-muted hover:bg-surface-secondary/40 hover:text-foreground",
                    ].join(' ')}
                  >
                    <Shield size={14} />
                    <Typography type="body-xs" className="font-medium text-inherit">{t('common:admin.rolesMatrix')}</Typography>
                  </Link>
                )}

                {hasPermission('ai:audit:view') && (
                  <Link
                    href="/admin/audit-logs"
                    className={[
                      "flex items-center gap-2.5 px-3 py-2 rounded-lg text-xs font-medium transition-all duration-200 focus-visible:ring-2 focus-visible:ring-focus focus-visible:outline-hidden",
                      pathname === '/admin/audit-logs'
                        ? "bg-surface-secondary text-foreground font-bold"
                        : "text-muted hover:bg-surface-secondary/40 hover:text-foreground",
                    ].join(' ')}
                  >
                    <FileText size={14} />
                    <Typography type="body-xs" className="font-medium text-inherit">{t('common:admin.auditTrail')}</Typography>
                  </Link>
                )}
              </div>
            </div>
          )}
        </nav>

        {/* Sidebar Footer User Info block */}
        <div className="p-4 border-t border-separator flex items-center select-none">
          <div className="flex items-center gap-3 min-w-0">
            <AuthAvatar />
            <div className="flex flex-col min-w-0">
              <Typography type="body-sm" className="font-bold truncate text-foreground font-outfit">
                {user?.fullName}
              </Typography>
              <Typography type="body-xs" className="text-muted text-[10px] uppercase font-extrabold tracking-wider font-outfit">
                {userRole}
              </Typography>
            </div>
          </div>
        </div>
      </aside>

      {/* 2. Main content viewport section */}
      <div className="flex flex-1 flex-col min-w-0">
        {/* Top Header navbar (Mobile responsive layout) */}
        <header className="flex h-16 w-full items-center justify-between px-6 border-b border-border bg-background/70 backdrop-blur-xl z-10">
          <div className="md:hidden flex items-center gap-2 select-none">
            <Compass size={20} />
            <Typography type="body-sm" className="font-bold tracking-tight font-outfit">
              {t('common:branding.title')}
            </Typography>
          </div>

          <AppBreadcrumbs />

          <div className="flex items-center gap-4">
            {/* Live activity indicator badge */}
            <span className="flex h-2 w-2 relative shrink-0">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2 w-2 bg-emerald-500"></span>
            </span>
            <Typography type="body-xs" className="text-muted select-none hidden sm:inline font-outfit font-semibold">
              {t('common:dashboard.sessionProtected')}
            </Typography>

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
