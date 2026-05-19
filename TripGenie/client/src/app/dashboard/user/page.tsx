"use client";

import React from 'react';
import { useAuth } from '../../../features/auth/hooks/use-auth';
import { Card } from '../../../components/ui/card';
import { Button } from '../../../components/ui/button';
import { PermissionGuard } from '../../../features/auth/guards/permission-guard';
import { User, ShieldCheck, Plus, Sparkles, PlaneTakeoff, Compass } from 'lucide-react';
import { useTranslation } from 'react-i18next';

export default function UserDashboardPage() {
  const { user } = useAuth();
  const { t } = useTranslation(['dashboard-user', 'common']);

  return (
    <div className="space-y-6 font-outfit">
      {/* Top Banner Message */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-linear-to-r from-zinc-950 via-zinc-900 to-zinc-800 text-white select-none">
        <div className="space-y-1">
          <h2 className="text-xl font-bold flex items-center gap-2">
            {t('dashboard-user:banner.welcome', { name: user?.fullName || 'Traveler' })}{' '}
            <Sparkles size={18} className="text-indigo-400 fill-indigo-400" />
          </h2>
          <p className="text-zinc-400 text-xs font-light">
            {t('dashboard-user:banner.subtitle')}
          </p>
        </div>
        <Button variant="solid" className="w-fit self-start shrink-0 cursor-pointer">
          <PlaneTakeoff size={16} />
          {t('dashboard-user:banner.createPrompt')}
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
              <h3 className="font-bold text-zinc-900 dark:text-zinc-50">{t('dashboard-user:profile.title')}</h3>
              <p className="text-zinc-400 text-xs">{t('dashboard-user:profile.subtitle')}</p>
            </div>
          </div>

          <div className="space-y-3.5 text-sm select-none font-outfit">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-[10px] block font-extrabold tracking-wider mb-0.5">{t('dashboard-user:profile.fullName')}</span>
              <span className="font-semibold text-zinc-800 dark:text-zinc-200">{user?.fullName}</span>
            </div>
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-[10px] block font-extrabold tracking-wider mb-0.5">{t('dashboard-user:profile.email')}</span>
              <span className="font-semibold text-zinc-800 dark:text-zinc-200">{user?.email}</span>
            </div>
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-[10px] block font-extrabold tracking-wider mb-0.5">{t('dashboard-user:profile.accountId')}</span>
              <span className="font-mono text-zinc-400 dark:text-zinc-600 text-xs truncate block">{user?.id}</span>
            </div>
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-[10px] block font-extrabold tracking-wider mb-0.5">{t('dashboard-user:profile.status')}</span>
              <span className="inline-flex items-center px-2 py-0.5 rounded-full text-[10px] font-extrabold tracking-wider uppercase bg-emerald-50 dark:bg-emerald-950/20 text-emerald-600 dark:text-emerald-400 border border-emerald-200/50 dark:border-emerald-900/30 select-none">
                {user?.isEmailVerified ? t('dashboard-user:profile.verified') : t('dashboard-user:profile.pending')}
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
              <h3 className="font-bold text-zinc-900 dark:text-zinc-50">{t('dashboard-user:security.title')}</h3>
              <p className="text-zinc-400 text-xs">{t('dashboard-user:security.subtitle')}</p>
            </div>
          </div>

          <div className="space-y-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-[10px] block font-extrabold tracking-wider mb-1">{t('dashboard-user:security.currentRole')}</span>
              <span className="inline-flex px-3 py-1 rounded-xl text-xs font-bold uppercase tracking-wider bg-zinc-100 dark:bg-zinc-900 text-zinc-800 dark:text-zinc-200">
                {user?.role}
              </span>
            </div>

            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-[10px] block font-extrabold tracking-wider mb-2">{t('dashboard-user:security.grantedAccess')}</span>
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
                    {t('dashboard-user:security.fullPrivilege')}
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
              <h3 className="font-bold text-zinc-900 dark:text-zinc-50">{t('dashboard-user:sandbox.title')}</h3>
              <p className="text-zinc-400 text-xs">{t('dashboard-user:sandbox.subtitle')}</p>
            </div>
          </div>

          <div className="space-y-4">
            {/* Feature gated on 'trips:create' */}
            <PermissionGuard
              permission="trips:create"
              fallback={
                <div className="p-4 rounded-xl border border-dashed border-zinc-200 dark:border-zinc-800 text-center select-none font-outfit">
                  <span className="text-xs text-zinc-400 dark:text-zinc-600 block mb-1 font-bold">{t('dashboard-user:sandbox.gatedComponent')}</span>
                  <p className="text-[11px] text-zinc-500 leading-normal">
                    {t('dashboard-user:sandbox.gatedComponentDesc')}
                  </p>
                </div>
              }
            >
              <div className="p-4 rounded-xl bg-indigo-50/50 dark:bg-indigo-950/10 border border-indigo-200/50 dark:border-indigo-900/30 space-y-3 font-outfit">
                <div className="flex items-center justify-between text-xs select-none">
                  <span className="font-bold text-indigo-700 dark:text-indigo-400">{t('dashboard-user:sandbox.creatorEnabled')}</span>
                  <span className="text-[10px] bg-indigo-600 text-white dark:bg-indigo-500 dark:text-zinc-950 px-1.5 py-0.5 rounded font-extrabold tracking-wider uppercase">Active</span>
                </div>
                <p className="text-xs text-zinc-500 dark:text-zinc-400 leading-relaxed select-none">
                  {t('dashboard-user:sandbox.creatorEnabledDesc')}
                </p>
                <Button size="sm" className="w-full gap-1 cursor-pointer">
                  <Plus size={14} />
                  {t('dashboard-user:sandbox.newItinerary')}
                </Button>
              </div>
            </PermissionGuard>

            {/* Feature gated on 'admin:system:view' */}
            <PermissionGuard
              permission="admin:system:view"
              fallback={
                <div className="p-4 rounded-xl border border-dashed border-zinc-200 dark:border-zinc-800 text-center select-none font-outfit">
                  <span className="text-xs text-zinc-400 dark:text-zinc-600 block mb-1 font-bold">{t('dashboard-user:sandbox.systemControls')}</span>
                  <p className="text-[11px] text-zinc-500 leading-normal">
                    {t('dashboard-user:sandbox.systemControlsDesc')}
                  </p>
                </div>
              }
            >
              <div className="p-4 rounded-xl bg-emerald-50/50 dark:bg-emerald-950/10 border border-emerald-200/50 dark:border-emerald-900/30 space-y-2 select-none font-outfit">
                <span className="font-bold text-xs text-emerald-700 dark:text-emerald-400 block">{t('dashboard-user:sandbox.systemUnlocked')}</span>
                <p className="text-[11px] text-zinc-500 dark:text-zinc-400 leading-relaxed">
                  {t('dashboard-user:sandbox.systemUnlockedDesc')}
                </p>
              </div>
            </PermissionGuard>
          </div>
        </Card>
      </div>
    </div>
  );
}
