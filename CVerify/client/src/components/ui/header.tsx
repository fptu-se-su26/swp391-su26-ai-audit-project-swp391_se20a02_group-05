"use client";

import React from 'react';
import { Menu, ChevronLeft, ChevronRight, Bell, Compass } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { useSidebarStore } from '../../stores/use-sidebar-store';
import { AuthAvatar } from './auth-avatar';
import { Typography } from '@heroui/react';

export const Header: React.FC = () => {
  const { t } = useTranslation(['common']);
  const { isCollapsed, toggleCollapsed, setMobileOpen } = useSidebarStore();

  return (
    <header className="flex h-16 w-full items-center justify-between px-6 border-b border-border bg-background/70 backdrop-blur-xl z-20 shrink-0 select-none">
      {/* Left side: toggle controls & mobile brand logo */}
      <div className="flex items-center gap-3">
        {/* Mobile: Hamburger toggle button */}
        <button
          onClick={() => setMobileOpen(true)}
          aria-label="Open Menu"
          className="md:hidden flex h-9 w-9 items-center justify-center rounded-lg hover:bg-surface-secondary text-muted hover:text-foreground cursor-pointer transition-colors outline-hidden focus-visible:ring-2 focus-visible:ring-focus"
        >
          <Menu size={20} />
        </button>

        {/* Desktop: Collapse toggle button */}
        <button
          onClick={toggleCollapsed}
          aria-label={isCollapsed ? "Expand Sidebar" : "Collapse Sidebar"}
          className="hidden md:flex h-9 w-9 items-center justify-center rounded-lg hover:bg-surface-secondary text-muted hover:text-foreground cursor-pointer transition-colors outline-hidden focus-visible:ring-2 focus-visible:ring-focus"
        >
          {isCollapsed ? (
            <ChevronRight size={18} />
          ) : (
            <ChevronLeft size={18} />
          )}
        </button>

        {/* Mobile-only logo */}
        <div className="md:hidden flex items-center gap-2">
          <div className="w-7 h-7 rounded-lg bg-foreground text-background flex items-center justify-center shadow-sm shrink-0">
            <Compass size={16} />
          </div>
          <Typography type="body-sm" className="font-bold tracking-tight font-outfit text-foreground leading-none">
            {t('common:branding.title', { defaultValue: 'CVerify' })}
          </Typography>
        </div>
      </div>

      {/* Right side: Notifications, Session status, User profile avatar */}
      <div className="flex items-center gap-4">
        {/* Session protected indicator */}
        <div className="hidden sm:flex items-center gap-2 bg-success/10 border border-success/25 px-2.5 py-1 rounded-full select-none">
          <span className="flex h-2 w-2 relative shrink-0">
            <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-success/80 opacity-75"></span>
            <span className="relative inline-flex rounded-full h-2 w-2 bg-success"></span>
          </span>
          <Typography type="body-xs" className="text-success font-outfit font-bold text-[10px] tracking-wide uppercase leading-none">
            {t('common:dashboard.sessionProtected', { defaultValue: 'Protected' })}
          </Typography>
        </div>

        {/* Action Button: Notifications (Placeholder for Page-level actions) */}
        <button
          aria-label="Notifications"
          className="flex h-9 w-9 items-center justify-center rounded-lg hover:bg-surface-secondary text-muted hover:text-foreground cursor-pointer transition-colors outline-hidden focus-visible:ring-2 focus-visible:ring-focus"
        >
          <Bell size={18} />
        </button>

        {/* User profile dropdown avatar (always in Header now!) */}
        <AuthAvatar />
      </div>
    </header>
  );
};

export default Header;
