"use client";

import React from 'react';
import { Card } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { ShieldAlert, Compass } from 'lucide-react';
import { useRouter } from 'next/navigation';

export default function UnauthorizedPage() {
  const router = useRouter();

  return (
    <div className="flex min-h-screen w-full items-center justify-center bg-zinc-50 dark:bg-zinc-950 p-6 transition-colors duration-300">
      {/* Decorative background gradients */}
      <div className="absolute top-[10%] left-[10%] w-[300px] h-[300px] rounded-full bg-red-500/5 blur-[80px] pointer-events-none" />
      <div className="absolute bottom-[10%] right-[10%] w-[300px] h-[300px] rounded-full bg-indigo-500/5 blur-[80px] pointer-events-none" />

      <div className="w-full max-w-md">
        {/* Brand Header */}
        <div className="flex items-center justify-center gap-2 mb-8 select-none">
          <div className="w-8 h-8 rounded-lg bg-zinc-950 dark:bg-white text-white dark:text-zinc-950 flex items-center justify-center shadow-md">
            <Compass size={18} />
          </div>
          <span className="font-bold text-lg tracking-tight text-zinc-900 dark:text-white">
            TripGenie AI
          </span>
        </div>

        <Card className="text-center shadow-2xl" glow={true}>
          <div className="mx-auto w-14 h-14 bg-red-50 dark:bg-red-950/20 text-red-500 rounded-full flex items-center justify-center mb-5 shadow-[0_0_20px_rgba(239,68,68,0.15)]">
            <ShieldAlert size={28} />
          </div>
          
          <h2 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50 mb-2">
            Access Restricted
          </h2>
          
          <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-6">
            You do not have the necessary security clearance or permission level to access this dashboard. 
            Please contact your system administrator or log in with an authorized account.
          </p>
          
          <div className="flex flex-col sm:flex-row gap-3 w-full justify-center">
            <Button
              variant="bordered"
              onClick={() => router.back()}
              className="w-full"
            >
              Go Back
            </Button>
            <Button
              variant="solid"
              onClick={() => router.push('/login')}
              className="w-full animate-shimmer"
            >
              Change Account
            </Button>
          </div>
        </Card>
      </div>
    </div>
  );
}
