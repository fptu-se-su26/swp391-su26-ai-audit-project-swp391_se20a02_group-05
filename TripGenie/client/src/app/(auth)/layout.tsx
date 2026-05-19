"use client";

import React, { useEffect } from 'react';
import { Compass } from 'lucide-react';
import { useAuth } from '../../features/auth/hooks/use-auth';
import { useRouter, usePathname } from 'next/navigation';
import { useTranslation } from 'react-i18next';
import { Typography } from '@heroui/react';

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { isAuthenticated, isInitialized, user } = useAuth();
  const router = useRouter();
  const pathname = usePathname();
  const { t } = useTranslation(['common']);

  useEffect(() => {
    if (isInitialized && isAuthenticated) {
      const isEmailVerified = user?.isEmailVerified;

      if (isEmailVerified) {
        // Logged-in verified users are blocked from auth routes and routed to dashboard
        router.replace('/');
      } else {
        // Logged-in unverified users are forced to verify their email
        if (pathname !== '/verify-email') {
          router.replace('/verify-email');
        }
      }
    }
  }, [isInitialized, isAuthenticated, user, pathname, router]);

  // Prevent flashing unauthenticated layouts briefly (FOUC prevention)
  const shouldShowLoader = !isInitialized || (isAuthenticated && (user?.isEmailVerified ? true : pathname !== '/verify-email'));

  if (shouldShowLoader) {
    return (
      <div className="flex min-h-screen w-full items-center justify-center bg-background transition-colors duration-300">
        <div className="flex flex-col items-center gap-4 text-center select-none animate-pulse">
          {/* Beautiful Pulsing Compass Icon Container */}
          <div className="w-14 h-14 rounded-2xl bg-foreground text-background flex items-center justify-center shadow-xl border border-border/20">
            <Compass size={32} className="text-background animate-spin" style={{ animationDuration: '3s' }} />
          </div>

          <div className="space-y-1">
            <Typography type="h3" className="font-extrabold tracking-tight text-foreground text-base font-outfit">
              {t('common:branding.title')}
            </Typography>
            <Typography type="body-xs" className="text-muted font-medium">
              {t('common:misc.loadingSession')}
            </Typography>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen w-full bg-background transition-colors duration-300">
      {/* Split column 1: Visual Branding Panel (Hidden on Mobile) */}
      <div className="hidden lg:flex relative w-1/2 flex-col justify-between p-12 overflow-hidden bg-zinc-950 text-white select-none">
        {/* Subtle decorative radial lights */}
        <div className="absolute top-[-10%] right-[-10%] w-[80%] h-[80%] rounded-full bg-indigo-500/10 blur-[120px] pointer-events-none" />
        <div className="absolute bottom-[-10%] left-[-10%] w-[60%] h-[60%] rounded-full bg-emerald-500/10 blur-[100px] pointer-events-none" />

        {/* Glowing grid backdrop pattern */}
        <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_center,rgba(255,255,255,0.02)_1px,transparent_1px)] bg-size-[24px_24px] pointer-events-none opacity-60" />

        {/* Top: Logo */}
        <div className="relative z-10 flex items-center gap-2.5">
          <div className="w-10 h-10 rounded-xl bg-white text-zinc-950 flex items-center justify-center shadow-lg border border-white/10">
            <Compass size={22} className="text-zinc-950" />
          </div>
          <Typography type="body-sm" className="font-extrabold tracking-tight bg-clip-text text-transparent bg-linear-to-r from-white via-zinc-100 to-zinc-400">
            {t('common:branding.title')}
          </Typography>
        </div>

        {/* Center: Hero Intro */}
        <div className="relative z-10 my-auto max-w-md space-y-4">
          <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold bg-white/5 border border-white/10 text-zinc-300 backdrop-blur-md">
            ✨ {t('common:branding.tagline')}
          </span>
          <Typography type="h1" className="text-4xl font-extrabold tracking-tight leading-[1.1] bg-linear-to-b from-white to-zinc-400 bg-clip-text text-transparent">
            {t('common:branding.journeyPrompt')}
          </Typography>
          <Typography type="body-sm" className="text-zinc-400 leading-relaxed font-light font-outfit">
            {t('common:branding.description')}
          </Typography>
        </div>

        {/* Bottom: Footer Info */}
        <div className="relative z-10 flex items-center justify-between text-xs text-zinc-500">
          <Typography type="body-xs" className="text-zinc-500">
            &copy; {new Date().getFullYear()} {t('common:branding.title')} Inc.
          </Typography>
          <div className="flex gap-4">
            <a href="#" className="hover:text-zinc-300 transition-colors">{t('common:navigation.terms')}</a>
            <a href="#" className="hover:text-zinc-300 transition-colors">{t('common:navigation.privacy')}</a>
          </div>
        </div>
      </div>

      {/* Split column 2: Interactive Form Container */}
      <div className="flex flex-1 flex-col items-center justify-center p-6 md:p-12 relative overflow-hidden">
        {/* Mobile Logo Header (Hidden on Large Screen) */}
        <div className="lg:hidden absolute top-8 left-8 flex items-center gap-2 select-none">
          <div className="w-8 h-8 rounded-lg bg-foreground text-background flex items-center justify-center shadow-md">
            <Compass size={18} />
          </div>
          <Typography type="body-sm" className="font-bold tracking-tight text-foreground">
            {t('common:branding.title')}
          </Typography>
        </div>

        <div className="w-full max-w-[440px]">
          {children}
        </div>
      </div>
    </div>
  );
}
