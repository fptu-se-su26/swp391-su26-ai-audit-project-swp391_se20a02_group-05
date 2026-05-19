"use client";

import React from 'react';
import { Card } from '../../../components/ui/card';
import { Button } from '../../../components/ui/button';
import { ShieldAlert, Server, Activity, Users, Lock, Eye } from 'lucide-react';
import { useTranslation } from 'react-i18next';

export default function AdminDashboardPage() {
  const { t } = useTranslation(['dashboard-admin', 'common']);

  return (
    <div className="space-y-6 font-outfit">
      
      {/* Header Banner */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-zinc-950 border border-zinc-900 text-white select-none">
        <div className="space-y-1">
          <h2 className="text-xl font-bold flex items-center gap-2">
            {t('dashboard-admin:banner.title')}{' '}
            <ShieldAlert size={20} className="text-red-500 animate-pulse" />
          </h2>
          <p className="text-zinc-400 text-xs font-light">
            {t('dashboard-admin:banner.subtitle')}
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="solid" className="w-fit self-start shrink-0 text-red-600 dark:text-red-400 bg-red-500/10 hover:bg-red-500/20 border-red-500/20 cursor-pointer" size="sm">
            <Lock size={14} className="mr-1" />
            {t('dashboard-admin:banner.lockApi')}
          </Button>
        </div>
      </div>

      {/* Admin KPIs Grid */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        
        {/* Metric 1: CPU/RAM */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-extrabold block mb-1 uppercase tracking-wider">{t('dashboard-admin:kpis.platformLoad')}</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">18.4%</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-indigo-50 dark:bg-indigo-950/20 text-indigo-500 flex items-center justify-center">
              <Server size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            {t('dashboard-admin:kpis.platformLoadSub', { used: "4.2", total: "16" })}
          </div>
        </Card>

        {/* Metric 2: API Request index */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-extrabold block mb-1 uppercase tracking-wider">{t('dashboard-admin:kpis.apiHealth')}</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">99.98%</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-emerald-50 dark:bg-emerald-950/20 text-emerald-500 flex items-center justify-center">
              <Activity size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            {t('dashboard-admin:kpis.apiHealthSub', { ms: 140 })}
          </div>
        </Card>

        {/* Metric 3: Active user accounts count */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-extrabold block mb-1 uppercase tracking-wider">{t('dashboard-admin:kpis.totalUsers')}</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">142,500</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-amber-50 dark:bg-amber-950/20 text-amber-500 flex items-center justify-center">
              <Users size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            {t('dashboard-admin:kpis.totalUsersSub', { count: "1,820" })}
          </div>
        </Card>
      </div>

      {/* Live simulated console security log stream */}
      <Card glow={false}>
        <div className="flex justify-between items-center mb-5 select-none">
          <div>
            <h3 className="font-bold text-zinc-900 dark:text-zinc-50">{t('dashboard-admin:console.title')}</h3>
            <p className="text-zinc-400 text-xs">{t('dashboard-admin:console.subtitle')}</p>
          </div>
          <Button variant="bordered" size="sm" className="cursor-pointer">
            <Eye size={14} className="mr-1" />
            {t('dashboard-admin:console.auditTrail')}
          </Button>
        </div>

        <div className="p-4 rounded-xl bg-zinc-900 dark:bg-black font-mono text-xs text-zinc-300 space-y-2 max-h-56 overflow-y-auto shadow-inner border border-zinc-800 select-none">
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:48:10]</span>
            <span className="text-emerald-400 font-bold">[SUCCESS]</span>
            <span>{t('dashboard-admin:console.logs.successRotation', { email: 'admin@tripgenie.ai' })}</span>
          </div>
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:44:22]</span>
            <span className="text-red-400 font-bold">[WARN]</span>
            <span>{t('dashboard-admin:console.logs.warnBruteForce', { ip: '192.168.1.42' })}</span>
          </div>
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:39:15]</span>
            <span className="text-emerald-400 font-bold">[SUCCESS]</span>
            <span>{t('dashboard-admin:console.logs.successOAuth', { email: 'traveler_4892@gmail.com' })}</span>
          </div>
          <div className="flex items-start gap-2.5">
            <span className="text-zinc-500 font-semibold">[15:32:04]</span>
            <span className="text-amber-400 font-bold">[INFO]</span>
            <span>{t('dashboard-admin:console.logs.infoJwt')}</span>
          </div>
        </div>
      </Card>
    </div>
  );
}
