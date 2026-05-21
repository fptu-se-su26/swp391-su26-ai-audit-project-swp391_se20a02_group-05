"use client";

import React from 'react';
import { useAuth } from '../hooks/use-auth';
import { Compass } from 'lucide-react';
import { Typography } from '@heroui/react';
import { useTranslation } from 'react-i18next';

export const GuestGuard: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, bootstrapState } = useAuth();
  const { t } = useTranslation(['common']);

  const isLoading = bootstrapState !== 'READY';

  if (isLoading || isAuthenticated) {
    return (
      <div className="flex min-h-screen w-full items-center justify-center bg-zinc-950 transition-colors duration-300">
        <div className="flex flex-col items-center gap-4 text-center select-none animate-pulse">
          <div className="w-14 h-14 rounded-2xl bg-white text-zinc-950 flex items-center justify-center shadow-xl border border-white/10">
            <Compass size={32} className="text-zinc-950 animate-spin" style={{ animationDuration: '3s' }} />
          </div>

          <div className="space-y-1">
            <Typography className="font-extrabold tracking-tight text-white text-base font-outfit">
              {t('common:branding.title')}
            </Typography>
            <Typography className="text-zinc-500 font-medium text-xs">
              {t('common:misc.loadingSession')}
            </Typography>
          </div>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};

export default GuestGuard;
