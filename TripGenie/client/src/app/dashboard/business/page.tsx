"use client";

import React from 'react';
import { Card } from '../../../components/ui/card';
import { Button } from '../../../components/ui/button';
import { Building2, TrendingUp, HandCoins, Globe, Plus, Settings } from 'lucide-react';
import { useAuth } from '../../../hooks/use-auth';

export default function BusinessDashboardPage() {
  const { user } = useAuth();

  return (
    <div className="space-y-6">
      
      {/* Header Banner */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-zinc-900 border border-zinc-800 text-white select-none">
        <div className="space-y-1">
          <h2 className="text-xl font-bold flex items-center gap-2">
            Service Partner Console <Building2 size={20} className="text-indigo-400" />
          </h2>
          <p className="text-zinc-400 text-xs font-light">
            Manage your hospitality, travel products, listings, and monitor customer booking indexes.
          </p>
        </div>
        <div className="flex gap-2.5">
          <Button variant="solid" className="w-fit self-start bg-indigo-600 hover:bg-indigo-500 border-none shrink-0" size="sm">
            <Plus size={14} />
            Add Listing
          </Button>
        </div>
      </div>

      {/* KPI Cards Grid */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        
        {/* KPI 1: Active Listings */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-bold block mb-1">ACTIVE PRODUCTS</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">14</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-indigo-50 dark:bg-indigo-950/20 text-indigo-500 flex items-center justify-center">
              <Globe size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            <span className="text-emerald-500 font-bold font-mono">+2 new</span> listed this month
          </div>
        </Card>

        {/* KPI 2: Bookings index */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-bold block mb-1">TOTAL BOOKINGS</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">2,840</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-emerald-50 dark:bg-emerald-950/20 text-emerald-500 flex items-center justify-center">
              <TrendingUp size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            <span className="text-emerald-500 font-bold font-mono">+12.4%</span> vs previous quarter
          </div>
        </Card>

        {/* KPI 3: Commissions rate */}
        <Card glow={false}>
          <div className="flex justify-between items-start mb-4 select-none">
            <div>
              <span className="text-zinc-400 dark:text-zinc-500 text-xs font-bold block mb-1">PARTNER REVENUE</span>
              <span className="text-3xl font-extrabold tracking-tight tabular-nums text-zinc-900 dark:text-zinc-50">$48,250</span>
            </div>
            <div className="w-10 h-10 rounded-xl bg-amber-50 dark:bg-amber-950/20 text-amber-500 flex items-center justify-center">
              <HandCoins size={18} />
            </div>
          </div>
          <div className="text-xs text-zinc-400 dark:text-zinc-600 select-none">
            Average payout cycles: <span className="font-bold">14 days</span>
          </div>
        </Card>
      </div>

      {/* Listing Management Demonstration Layout */}
      <Card glow={false}>
        <div className="flex justify-between items-center mb-6 select-none">
          <div>
            <h3 className="font-bold text-zinc-900 dark:text-zinc-50">Current Managed Offerings</h3>
            <p className="text-zinc-400 text-xs">Itineraries & hospitality packages available for travelers</p>
          </div>
          <Button variant="bordered" size="sm">
            <Settings size={14} className="mr-1" />
            Manage Settings
          </Button>
        </div>

        <div className="overflow-x-auto w-full select-none">
          <table className="w-full text-sm text-left border-collapse">
            <thead>
              <tr className="border-b border-zinc-200 dark:border-zinc-800 text-zinc-400 dark:text-zinc-500 text-xs font-bold">
                <th className="py-3 px-4">Offering Title</th>
                <th className="py-3 px-4">Category</th>
                <th className="py-3 px-4">Price Rate</th>
                <th className="py-3 px-4">Booking Rate</th>
                <th className="py-3 px-4 text-right">Action</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-200/50 dark:divide-zinc-900 text-zinc-700 dark:text-zinc-300">
              <tr>
                <td className="py-3 px-4 font-semibold text-zinc-800 dark:text-zinc-200">Indochina Beach Retreat</td>
                <td className="py-3 px-4">Hotel Bundle</td>
                <td className="py-3 px-4 font-mono font-medium">$240/night</td>
                <td className="py-3 px-4">84% occupancy</td>
                <td className="py-3 px-4 text-right">
                  <button className="text-indigo-600 dark:text-indigo-400 font-semibold hover:underline">Edit</button>
                </td>
              </tr>
              <tr>
                <td className="py-3 px-4 font-semibold text-zinc-800 dark:text-zinc-200">Majestic Sapa trekking bundle</td>
                <td className="py-3 px-4">Itinerary Guide</td>
                <td className="py-3 px-4 font-mono font-medium">$45/person</td>
                <td className="py-3 px-4">120 bookings</td>
                <td className="py-3 px-4 text-right">
                  <button className="text-indigo-600 dark:text-indigo-400 font-semibold hover:underline">Edit</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
}
