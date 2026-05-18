"use client";

import React from 'react';
import { Card } from '../../../components/ui/card';
import { Button } from '../../../components/ui/button';
import { ShieldAlert, Server, Activity, Users, Lock, Unlock, Eye } from 'lucide-react';

export default function AdminDashboardPage() {
  return (
    <div className="space-y-6">
      
      {/* Header Banner */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-zinc-950 border border-zinc-900 text-white select-none">
        <div className="space-y-1">
          <h2 className="text-xl font-bold flex items-center gap-2">
            System Administrator Console <ShieldAlert size={20} className="text-red-500 animate-pulse" />
          </h2>
          <p className="text-zinc-400 text-xs font-light">
            Platform control center. Monitor server health, manage users database, and enforce auth policies.
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="solid" className="w-fit self-start shrink-0 text-red-600 dark:text-red-400 bg-red-500/10 hover:bg-red-500/20 border-red-500/20" size="sm">
            <Lock size={14} className="mr-1" />
            Lock API
          </Button>
        </div>
      </div>

      {/* Admin KPIs Grid */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        
        {/* Metric 1: CPU/RAM */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-bold block mb-1">PLATFORM LOAD</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">18.4%</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-indigo-50 dark:bg-indigo-950/20 text-indigo-500 flex items-center justify-center">
              <Server size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            Memory usage: <span className="font-bold">4.2GB / 16GB</span>
          </div>
        </Card>

        {/* Metric 2: API Request index */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-bold block mb-1">API HEALTH</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">99.98%</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-emerald-50 dark:bg-emerald-950/20 text-emerald-500 flex items-center justify-center">
              <Activity size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            Mean latency: <span className="text-emerald-500 font-bold font-mono">140ms</span>
          </div>
        </Card>

        {/* Metric 3: Active user accounts count */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-bold block mb-1">TOTAL USER RECORDS</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">142,500</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-amber-50 dark:bg-amber-950/20 text-amber-500 flex items-center justify-center">
              <Users size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            Active sessions right now: <span className="font-bold">1,820</span>
          </div>
        </Card>
      </div>

      {/* Live simulated console security log stream */}
      <Card glow={false}>
        <div className="flex justify-between items-center mb-5 select-none">
          <div>
            <h3 className="font-bold text-zinc-900 dark:text-zinc-50">Recent System Security Logs</h3>
            <p className="text-zinc-400 text-xs">Simulated live edge-guard authentication status events</p>
          </div>
          <Button variant="bordered" size="sm">
            <Eye size={14} className="mr-1" />
            Audit trail
          </Button>
        </div>

        <div className="p-4 rounded-xl bg-zinc-900 dark:bg-black font-mono text-xs text-zinc-300 space-y-2 max-h-56 overflow-y-auto shadow-inner border border-zinc-800 select-none">
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:48:10]</span>
            <span className="text-emerald-400 font-bold">[SUCCESS]</span>
            <span>Silent Token Rotation completed. Session Extended for admin@tripgenie.ai</span>
          </div>
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:44:22]</span>
            <span className="text-red-400 font-bold">[WARN]</span>
            <span>Brute-force block: 429 RateLimit lock engaged on IP 192.168.1.42 (Login Endpoint)</span>
          </div>
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:39:15]</span>
            <span className="text-emerald-400 font-bold">[SUCCESS]</span>
            <span>Google OAuth Login processed for user traveler_4892@gmail.com</span>
          </div>
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:32:04]</span>
            <span className="text-amber-400 font-bold">[INFO]</span>
            <span>Edge verified & decoded signed JWT. Access granted to /dashboard/business (Partner role)</span>
          </div>
        </div>
      </Card>
    </div>
  );
}
