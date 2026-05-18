"use client";

import React, { useEffect } from 'react';
import { useAuth } from '../../hooks/use-auth';
import { useRouter } from 'next/navigation';
import { Compass } from 'lucide-react';

export default function DashboardResolutionPage() {
  const { user, isInitialized, isAuthenticated } = useAuth();
  const router = useRouter();

  useEffect(() => {
    // Wait until local session state is fully hydrated/initialized from cookies
    if (isInitialized) {
      if (!isAuthenticated) {
        // Safe unauthenticated fallback
        router.replace('/login');
        return;
      }

      // Dynamic client-side role dispatcher
      const role = user?.role || 'USER';
      
      if (role === 'ADMIN') {
        router.replace('/dashboard/admin');
      } else if (role === 'BUSINESS') {
        router.replace('/dashboard/business');
      } else {
        router.replace('/dashboard/user');
      }
    }
  }, [isInitialized, isAuthenticated, user, router]);

  // Premium, visually polished loading screen matching the design system
  return (
    <div className="flex min-h-screen w-full items-center justify-center bg-zinc-50 dark:bg-zinc-950 transition-colors duration-300">
      <div className="flex flex-col items-center gap-4 text-center select-none animate-pulse">
        {/* Beautiful Glassmorphic Compass Icon Container */}
        <div className="w-14 h-14 rounded-2xl bg-zinc-900 dark:bg-white text-white dark:text-zinc-950 flex items-center justify-center shadow-xl border border-zinc-200/20">
          <Compass size={32} className="text-white dark:text-zinc-950 animate-spin" style={{ animationDuration: '3s' }} />
        </div>
        
        <div className="space-y-1">
          <h3 className="font-extrabold tracking-tight text-zinc-800 dark:text-zinc-100 text-base font-outfit">
            TripGenie AI
          </h3>
          <p className="text-zinc-400 dark:text-zinc-500 text-xs font-medium">
            Routing to your travel hub...
          </p>
        </div>
      </div>
    </div>
  );
}
