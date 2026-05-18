"use client";

import React from 'react';
import { useAuth } from '../../../hooks/use-auth';
import { Card } from '../../../components/ui/card';
import { Button } from '../../../components/ui/button';
import { PermissionGuard } from '../../../features/auth/guards/permission-guard';
import { User, ShieldCheck, Plus, Sparkles, PlaneTakeoff, Compass } from 'lucide-react';

export default function UserDashboardPage() {
  const { user } = useAuth();

  return (
    <div className="space-y-6">
      {/* Top Banner Message */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-gradient-to-r from-zinc-950 via-zinc-900 to-zinc-800 text-white select-none">
        <div className="space-y-1">
          <h2 className="text-xl font-bold flex items-center gap-2">
            Welcome back, {user?.fullName || 'Traveler'}! <Sparkles size={18} className="text-indigo-400 fill-indigo-400" />
          </h2>
          <p className="text-zinc-400 text-xs font-light">
            Plan your next journey or search custom itineraries using our AI Travel Guide.
          </p>
        </div>
        <Button variant="solid" className="w-fit self-start shrink-0">
          <PlaneTakeoff size={16} />
          Create Prompt
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        
        {/* Card 1: Traveler Profile card */}
        <Card className="lg:col-span-1" glow={false}>
          <div className="flex items-center gap-3 mb-6 select-none">
            <div className="w-10 h-10 rounded-full bg-zinc-100 dark:bg-zinc-900 flex items-center justify-center text-zinc-800 dark:text-zinc-200">
              <User size={20} />
            </div>
            <div>
              <h3 className="font-bold text-zinc-900 dark:text-zinc-50">Profile Details</h3>
              <p className="text-zinc-400 text-xs">Your registered account details</p>
            </div>
          </div>

          <div className="space-y-3.5 text-sm select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs block font-semibold mb-0.5">FULL NAME</span>
              <span className="font-medium text-zinc-800 dark:text-zinc-200">{user?.fullName}</span>
            </div>
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs block font-semibold mb-0.5">EMAIL</span>
              <span className="font-medium text-zinc-800 dark:text-zinc-200">{user?.email}</span>
            </div>
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs block font-semibold mb-0.5">ACCOUNT ID</span>
              <span className="font-mono text-zinc-400 dark:text-zinc-600 text-xs truncate block">{user?.id}</span>
            </div>
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs block font-semibold mb-0.5">STATUS</span>
              <span className="inline-flex items-center px-2 py-0.5 rounded-full text-[10px] font-bold tracking-wide uppercase bg-emerald-50 dark:bg-emerald-950/20 text-emerald-600 dark:text-emerald-400 border border-emerald-200/50 dark:border-emerald-900/30 select-none">
                {user?.isEmailVerified ? 'Verified Account' : 'Pending Verification'}
              </span>
            </div>
          </div>
        </Card>

        {/* Card 2: Security & Permissions Indicator */}
        <Card className="lg:col-span-1" glow={false}>
          <div className="flex items-center gap-3 mb-6 select-none">
            <div className="w-10 h-10 rounded-full bg-zinc-100 dark:bg-zinc-900 flex items-center justify-center text-zinc-800 dark:text-zinc-200">
              <ShieldCheck size={20} />
            </div>
            <div>
              <h3 className="font-bold text-zinc-900 dark:text-zinc-50">Active Role & Permissions</h3>
              <p className="text-zinc-400 text-xs">Assigned RBAC capability limits</p>
            </div>
          </div>

          <div className="space-y-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs block font-semibold mb-1">CURRENT ROLE</span>
              <span className="inline-flex px-3 py-1 rounded-xl text-xs font-bold uppercase tracking-wider bg-zinc-100 dark:bg-zinc-900 text-zinc-800 dark:text-zinc-200">
                {user?.role}
              </span>
            </div>

            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs block font-semibold mb-2">GRANTED SYSTEM ACCESS</span>
              <div className="flex flex-wrap gap-1.5 max-h-40 overflow-y-auto">
                {user?.permissions.map((perm) => (
                  <span
                    key={perm}
                    className="px-2 py-1 rounded-lg bg-zinc-50 dark:bg-zinc-900 border border-zinc-200/60 dark:border-zinc-800 text-zinc-600 dark:text-zinc-400 text-xs font-mono font-medium"
                  >
                    {perm}
                  </span>
                ))}
                {user?.role === 'ADMIN' && (
                  <span className="px-2 py-1 rounded-lg bg-indigo-50 dark:bg-indigo-950/20 border border-indigo-200/40 dark:border-indigo-900/30 text-indigo-600 dark:text-indigo-400 text-xs font-mono font-semibold">
                    *:* (Full Privilege Bypass)
                  </span>
                )}
              </div>
            </div>
          </div>
        </Card>

        {/* Card 3: Element Level PermissionGating Showcase */}
        <Card className="lg:col-span-1" glow={false}>
          <div className="flex items-center gap-3 mb-6 select-none">
            <div className="w-10 h-10 rounded-full bg-zinc-100 dark:bg-zinc-900 flex items-center justify-center text-zinc-800 dark:text-zinc-200">
              <Compass size={20} />
            </div>
            <div>
              <h3 className="font-bold text-zinc-900 dark:text-zinc-50">Feature Sandbox</h3>
              <p className="text-zinc-400 text-xs">PermissionGuard demonstration</p>
            </div>
          </div>

          <div className="space-y-4">
            {/* Feature gated on 'trips:create' */}
            <PermissionGuard
              permission="trips:create"
              fallback={
                <div className="p-4 rounded-xl border border-dashed border-zinc-200 dark:border-zinc-800 text-center select-none">
                  <span className="text-xs text-zinc-400 dark:text-zinc-600 block mb-1">GATED COMPONENT</span>
                  <p className="text-[11px] text-zinc-500 leading-normal">
                    Requires `trips:create` permission. Add this permission or elevate role to test.
                  </p>
                </div>
              }
            >
              <div className="p-4 rounded-xl bg-indigo-50/50 dark:bg-indigo-950/10 border border-indigo-200/50 dark:border-indigo-900/30 space-y-3">
                <div className="flex items-center justify-between text-xs select-none">
                  <span className="font-bold text-indigo-700 dark:text-indigo-400">CREATOR PANEL ENABLED</span>
                  <span className="text-[10px] bg-indigo-600 text-white dark:bg-indigo-500 dark:text-zinc-950 px-1.5 py-0.5 rounded font-extrabold tracking-wider uppercase">Active</span>
                </div>
                <p className="text-xs text-zinc-500 dark:text-zinc-400 leading-relaxed select-none">
                  You are authorized to write custom itinerary configurations to the central TripGenie repository.
                </p>
                <Button size="sm" className="w-full gap-1">
                  <Plus size={14} />
                  New Itinerary
                </Button>
              </div>
            </PermissionGuard>
            
            {/* Feature gated on 'admin:system:view' */}
            <PermissionGuard
              permission="admin:system:view"
              fallback={
                <div className="p-4 rounded-xl border border-dashed border-zinc-200 dark:border-zinc-800 text-center select-none">
                  <span className="text-xs text-zinc-400 dark:text-zinc-600 block mb-1">SYSTEM CONTROLS</span>
                  <p className="text-[11px] text-zinc-500 leading-normal">
                    Requires `admin:system:view` permission. Locked for standard travelers.
                  </p>
                </div>
              }
            >
              <div className="p-4 rounded-xl bg-emerald-50/50 dark:bg-emerald-950/10 border border-emerald-200/50 dark:border-emerald-900/30 space-y-2 select-none">
                <span className="font-bold text-xs text-emerald-700 dark:text-emerald-400 block">SYSTEM CONSOLE UNLOCKED</span>
                <p className="text-[11px] text-zinc-500 dark:text-zinc-400 leading-relaxed">
                  You have administrative privileges to monitor platform CPU, database tables, and memory usage.
                </p>
              </div>
            </PermissionGuard>
          </div>
        </Card>
      </div>
    </div>
  );
}
