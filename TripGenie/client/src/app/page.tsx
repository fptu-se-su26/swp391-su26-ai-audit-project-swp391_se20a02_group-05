"use client";

import React from 'react';
import { Compass, Sparkles, User, Building2, ShieldAlert, ArrowRight } from 'lucide-react';
import Link from 'next/link';
import { Card } from '../components/ui/card';
import { useAuth } from '../hooks/use-auth';
import { AuthAvatar } from '../components/ui/auth-avatar';

export default function Home() {
  const { isAuthenticated, user } = useAuth();

  return (
    <div className="relative min-h-screen w-full bg-zinc-950 text-white flex flex-col justify-between overflow-hidden">
      
      {/* 1. Stunning Background Glow Highlights */}
      <div className="absolute top-[-20%] left-[-10%] w-[600px] h-[600px] rounded-full bg-indigo-500/10 blur-[150px] pointer-events-none" />
      <div className="absolute bottom-[-10%] right-[-10%] w-[500px] h-[500px] rounded-full bg-emerald-500/10 blur-[150px] pointer-events-none" />
      
      {/* Subtle grid backdrop overlay */}
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_center,rgba(255,255,255,0.015)_1px,transparent_1px)] bg-[size:32px_32px] pointer-events-none opacity-80" />
 
      {/* 2. Top Header Navbar */}
      <header className="relative z-10 w-full max-w-7xl mx-auto px-6 h-20 flex items-center justify-between border-b border-white/5 backdrop-blur-md bg-zinc-950/20 select-none">
        <div className="flex items-center gap-2.5">
          <div className="w-9 h-9 rounded-xl bg-white text-zinc-950 flex items-center justify-center shadow-lg font-bold">
            <Compass size={20} />
          </div>
          <span className="font-extrabold text-lg tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-white to-zinc-400">
            TripGenie AI
          </span>
        </div>
 
        <div className="flex items-center gap-4">
          {isAuthenticated ? (
            <div className="flex items-center gap-4">
              <Link href={`/dashboard/${user?.role?.toLowerCase() || 'user'}`} className="text-sm font-semibold text-zinc-400 hover:text-white transition-colors">
                Dashboard
              </Link>
              <AuthAvatar />
            </div>
          ) : (
            <>
              <Link href="/login" className="text-sm font-semibold text-zinc-400 hover:text-white transition-colors">
                Sign In
              </Link>
              <Link href="/register">
                <button className="px-4 py-2 rounded-xl text-xs font-bold bg-white text-zinc-950 hover:bg-zinc-100 transition-all select-none cursor-pointer">
                  Sign Up
                </button>
              </Link>
            </>
          )}
        </div>
      </header>
 
      {/* 3. Hero Visual Container */}
      <main className="relative z-10 w-full max-w-4xl mx-auto px-6 py-20 flex flex-col items-center text-center space-y-8 my-auto">
        
        {/* Dynamic Badge */}
        <div className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold bg-white/5 border border-white/10 text-zinc-300 backdrop-blur-md select-none">
          <Sparkles size={12} className="text-indigo-400 fill-indigo-400" />
          <span>Production-grade AuthN & AuthZ Module Live</span>
        </div>
 
        {/* Headline */}
        <h1 className="text-4xl sm:text-6xl font-extrabold tracking-tight leading-[1.05] bg-gradient-to-b from-white via-zinc-100 to-zinc-400 bg-clip-text text-transparent">
          Decoupled Role-Based Security for modern TravelTech.
        </h1>
 
        {/* Supporting description */}
        <p className="max-w-2xl text-zinc-400 text-base sm:text-lg leading-relaxed font-light select-none">
          TripGenie AI integrates Zustand state hydration, edge JWT cryptography, anti-CSRF protections, Broadcast session syncing, and inactivity warning locks.
        </p>
 
        {/* Action Buttons */}
        <div className="flex flex-col sm:flex-row gap-4 pt-4 select-none w-full max-w-md justify-center">
          <Link href={isAuthenticated ? `/dashboard/${user?.role?.toLowerCase() || 'user'}` : "/dashboard/user"} className="w-full sm:w-auto">
            <button className="w-full sm:w-[200px] h-12 rounded-xl text-sm font-bold bg-white text-zinc-950 hover:bg-zinc-100 transition-all flex items-center justify-center gap-2 group shadow-[0_4px_20px_rgba(255,255,255,0.06)] border border-white/10 cursor-pointer">
              Enter Traveler Hub
              <ArrowRight size={16} className="transition-transform group-hover:translate-x-0.5" />
            </button>
          </Link>
          <Link href={isAuthenticated ? `/dashboard/${user?.role?.toLowerCase() || 'user'}` : "/login"} className="w-full sm:w-auto">
            <button className="w-full sm:w-[200px] h-12 rounded-xl text-sm font-bold bg-white/5 hover:bg-white/10 transition-all border border-white/10 text-white backdrop-blur-sm flex items-center justify-center gap-2 cursor-pointer">
              {isAuthenticated ? 'Go to Console' : 'Access Console'}
            </button>
          </Link>
        </div>
      </main>

      {/* 4. Feature Roles Overview Grid */}
      <section className="relative z-10 w-full max-w-7xl mx-auto px-6 pb-20 select-none">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          
          <Card className="bg-zinc-950/40 border-white/5 backdrop-blur-lg" glow={false}>
            <div className="w-10 h-10 rounded-xl bg-white/5 text-zinc-300 flex items-center justify-center mb-4">
              <User size={20} />
            </div>
            <h3 className="font-bold text-base mb-1.5 text-zinc-100">Traveler (USER)</h3>
            <p className="text-zinc-500 text-xs leading-relaxed">
              Standard traveler dashboard giving full access to planning, searching prompts, and reviewing personal travel history details.
            </p>
          </Card>

          <Card className="bg-zinc-950/40 border-white/5 backdrop-blur-lg" glow={false}>
            <div className="w-10 h-10 rounded-xl bg-white/5 text-zinc-300 flex items-center justify-center mb-4">
              <Building2 size={20} />
            </div>
            <h3 className="font-bold text-base mb-1.5 text-zinc-100">Service Partner (BUSINESS)</h3>
            <p className="text-zinc-500 text-xs leading-relaxed">
              Partner dashboard to manage hotel packages, list excursions, and evaluate incoming travelers reservation revenue pools.
            </p>
          </Card>

          <Card className="bg-zinc-950/40 border-white/5 backdrop-blur-lg" glow={false}>
            <div className="w-10 h-10 rounded-xl bg-white/5 text-zinc-300 flex items-center justify-center mb-4">
              <ShieldAlert size={20} />
            </div>
            <h3 className="font-bold text-base mb-1.5 text-zinc-100">System Admin (ADMIN)</h3>
            <p className="text-zinc-500 text-xs leading-relaxed">
              Global system control dashboard offering server health diagnostic analytics, account registries, and direct API lock gates.
            </p>
          </Card>
        </div>
      </section>

      {/* 5. Minimalist footer */}
      <footer className="relative z-10 w-full max-w-7xl mx-auto px-6 py-8 border-t border-white/5 text-center text-xs text-zinc-600 select-none">
        <span>TripGenie AI Auth Portal Integration. Verified for Next.js 16 + HeroUI v3 + Tailwind v4.</span>
      </footer>
    </div>
  );
}
