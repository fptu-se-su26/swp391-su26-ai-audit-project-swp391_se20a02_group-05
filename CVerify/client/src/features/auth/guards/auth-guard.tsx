"use client";

import React from 'react';
import { useAuth } from '../hooks/use-auth';
import { Spinner } from '@heroui/react';

export const AuthGuard: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, bootstrapState } = useAuth();

  const isLoading = bootstrapState !== 'READY';

  if (isLoading || !isAuthenticated) {
    return (
      <div className="flex min-h-screen w-full items-center justify-center bg-zinc-950 text-white select-none">
        <div className="flex flex-col items-center gap-4 text-center select-none animate-pulse">
          <Spinner size="lg" color="current" />
          <p className="text-zinc-400 font-medium font-outfit text-sm">
            Establishing secure session...
          </p>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};

export default AuthGuard;
