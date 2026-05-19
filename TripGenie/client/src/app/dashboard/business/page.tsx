"use client";

import React from 'react';
import { Card } from '../../../components/ui/card';
import { Button } from '../../../components/ui/button';
import { Building2, TrendingUp, HandCoins, Globe, Plus, Settings } from 'lucide-react';
import { useTranslation } from 'react-i18next';

export default function BusinessDashboardPage() {
  const { t } = useTranslation(['dashboard-business', 'common']);

  return (
    <div className="space-y-6 font-outfit">
      
      {/* Header Banner */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-zinc-900 border border-zinc-800 text-white select-none">
        <div className="space-y-1">
          <h2 className="text-xl font-bold flex items-center gap-2">
            {t('dashboard-business:banner.title')}{' '}
            <Building2 size={20} className="text-indigo-400" />
          </h2>
          <p className="text-zinc-400 text-xs font-light">
            {t('dashboard-business:banner.subtitle')}
          </p>
        </div>
        <div className="flex gap-2.5">
          <Button variant="solid" className="w-fit self-start bg-indigo-600 hover:bg-indigo-500 border-none shrink-0 cursor-pointer" size="sm">
            <Plus size={14} />
            {t('dashboard-business:banner.addListing')}
          </Button>
        </div>
      </div>

      {/* KPI Cards Grid */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        
        {/* KPI 1: Active Listings */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-extrabold block mb-1 uppercase tracking-wider">{t('dashboard-business:kpis.activeProducts')}</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">14</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-indigo-50 dark:bg-indigo-950/20 text-indigo-500 flex items-center justify-center">
              <Globe size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            {t('dashboard-business:kpis.activeProductsSub')}
          </div>
        </Card>

        {/* KPI 2: Bookings index */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-extrabold block mb-1 uppercase tracking-wider">{t('dashboard-business:kpis.totalBookings')}</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">2,840</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-emerald-50 dark:bg-emerald-950/20 text-emerald-500 flex items-center justify-center">
              <TrendingUp size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none font-medium">
            {t('dashboard-business:kpis.totalBookingsSub')}
          </div>
        </Card>

        {/* KPI 3: Commissions rate */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-extrabold block mb-1 uppercase tracking-wider">{t('dashboard-business:kpis.partnerRevenue')}</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">$48,250</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-amber-50 dark:bg-amber-950/20 text-amber-500 flex items-center justify-center">
              <HandCoins size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            {t('dashboard-business:kpis.partnerRevenueSub', { days: 14 })}
          </div>
        </Card>
      </div>

      {/* Listing Management Demonstration Layout */}
      <Card glow={false}>
        <div className="flex justify-between items-center mb-6 select-none">
          <div>
            <h3 className="font-bold text-zinc-900 dark:text-zinc-50">{t('dashboard-business:offerings.title')}</h3>
            <p className="text-zinc-400 text-xs">{t('dashboard-business:offerings.subtitle')}</p>
          </div>
          <Button variant="bordered" size="sm" className="cursor-pointer">
            <Settings size={14} className="mr-1" />
            {t('dashboard-business:offerings.manageSettings')}
          </Button>
        </div>

        <div className="overflow-x-auto w-full select-none">
          <table className="w-full text-sm text-left border-collapse">
            <thead>
              <tr className="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 dark:text-zinc-500 text-xs font-bold font-outfit uppercase tracking-wider">
                <th className="py-3 px-4">{t('dashboard-business:offerings.table.title')}</th>
                <th className="py-3 px-4">{t('dashboard-business:offerings.table.category')}</th>
                <th className="py-3 px-4">{t('dashboard-business:offerings.table.price')}</th>
                <th className="py-3 px-4">{t('dashboard-business:offerings.table.bookingRate')}</th>
                <th className="py-3 px-4 text-right">{t('dashboard-business:offerings.table.action')}</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-200/50 dark:divide-zinc-900 text-zinc-700 dark:text-zinc-300 font-medium">
              <tr>
                <td className="py-3 px-4 font-semibold text-zinc-800 dark:text-zinc-200">Indochina Beach Retreat</td>
                <td className="py-3 px-4">{t('dashboard-business:offerings.table.hotelBundle')}</td>
                <td className="py-3 px-4 font-mono font-medium">$240/night</td>
                <td className="py-3 px-4">{t('dashboard-business:offerings.table.occupancy', { percent: 84 })}</td>
                <td className="py-3 px-4 text-right">
                  <button className="text-indigo-600 dark:text-indigo-400 font-bold hover:underline cursor-pointer">
                    {t('dashboard-business:offerings.table.edit')}
                  </button>
                </td>
              </tr>
              <tr>
                <td className="py-3 px-4 font-semibold text-zinc-800 dark:text-zinc-200">Majestic Sapa trekking bundle</td>
                <td className="py-3 px-4">{t('dashboard-business:offerings.table.itineraryGuide')}</td>
                <td className="py-3 px-4 font-mono font-medium">$45/person</td>
                <td className="py-3 px-4">{t('dashboard-business:offerings.table.bookings', { count: 120 })}</td>
                <td className="py-3 px-4 text-right">
                  <button className="text-indigo-600 dark:text-indigo-400 font-bold hover:underline cursor-pointer">
                    {t('dashboard-business:offerings.table.edit')}
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
}
