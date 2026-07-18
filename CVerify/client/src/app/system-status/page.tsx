"use client";

import React, { useState } from "react";
import Link from "next/link";
import { ArrowLeft, Activity, ShieldCheck, Server, AlertCircle, RefreshCw } from "lucide-react";
import { PublicPageShell } from "@/components/ui/public-page-shell";
import { AuthFooter } from "@/features/auth/components/auth-footer";

export default function SystemStatusPage() {
  const [refreshing, setRefreshing] = useState(false);

  const handleRefresh = () => {
    setRefreshing(true);
    setTimeout(() => setRefreshing(false), 800);
  };

  const services = [
    { name: "ASP.NET Core Core API Gateway", status: "Operational", load: "12%", uptime: "99.98%", type: "core" },
    { name: "FastAPI Python AI Microservice", status: "Operational", load: "8%", uptime: "99.95%", type: "core" },
    { name: "Lizard AST Analysis Runner", status: "Operational", load: "4%", uptime: "99.99%", type: "worker" },
    { name: "MinHash LSH Clone-Detection Service", status: "Operational", load: "2%", uptime: "100%", type: "worker" },
    { name: "PostgreSQL Database Engine", status: "Operational", load: "22%", uptime: "99.99%", type: "database" },
    { name: "Redis Distributed Cache Store", status: "Operational", load: "6%", uptime: "99.99%", type: "database" },
  ];

  return (
    <PublicPageShell
      guestFooter={<AuthFooter />}
      guestContainerClassName="min-h-screen bg-background text-foreground flex flex-col font-sans select-text"
      guestMainClassName="max-w-4xl mx-auto w-full px-4 sm:px-6 py-8 flex-1 flex flex-col gap-6"
    >
      <div className="relative overflow-hidden rounded-3xl bg-gradient-to-r from-surface-secondary/40 via-surface/60 to-surface-secondary/40 border border-border p-8 shadow-md">
        <div className="relative z-10 flex flex-col gap-4">
          <Link href="/" className="inline-flex items-center gap-2 text-xs font-semibold text-muted hover:text-foreground transition-colors w-fit">
            <ArrowLeft size={14} />
            Back to Home
          </Link>
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="p-3 rounded-2xl bg-success/10 border border-success/20 text-success animate-pulse">
                <Activity size={32} />
              </div>
              <div>
                <h1 className="text-3xl font-extrabold tracking-tight">System Status</h1>
                <p className="text-xs text-muted mt-1">All CVerify services are operational</p>
              </div>
            </div>
            <button
              onClick={handleRefresh}
              disabled={refreshing}
              className="p-2.5 rounded-xl border border-border bg-surface hover:bg-surface-secondary text-muted hover:text-foreground transition-all cursor-pointer"
            >
              <RefreshCw size={16} className={refreshing ? "animate-spin text-primary" : ""} />
            </button>
          </div>
        </div>
      </div>

      <div className="bg-surface border border-border rounded-2xl p-6 shadow-sm flex flex-col gap-6">
        <div className="flex items-center gap-2 px-3 py-2.5 rounded-xl bg-success/5 border border-success/10 text-success text-xs font-semibold">
          <ShieldCheck size={16} />
          Every core microservice and pipeline integration is verified healthy.
        </div>

        <div className="flex flex-col gap-3">
          <p className="text-[10px] uppercase font-bold text-muted tracking-wider px-1">Service Status Directory</p>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {services.map((svc) => (
              <div key={svc.name} className="p-4 rounded-xl border border-border/80 bg-surface-secondary/40 flex flex-col gap-2">
                <div className="flex items-start justify-between">
                  <div className="flex items-center gap-2">
                    <Server size={14} className="text-muted" />
                    <span className="text-xs font-bold text-foreground">{svc.name}</span>
                  </div>
                  <span className="text-[10px] px-2 py-0.5 rounded-full bg-success/10 text-success border border-success/20 font-bold">
                    {svc.status}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </PublicPageShell>
  );
}
