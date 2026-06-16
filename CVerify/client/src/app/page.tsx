"use client";

import React from 'react';
import {
  Compass,
  ArrowRight,
  ShieldCheck,
  FileSearch,
  Sparkles,
  BrainCircuit,
  GitBranch,
  BadgeCheck,
} from 'lucide-react';
import Link from 'next/link';
import { Card } from '../components/ui/card';
import { useAuth } from '../features/auth/hooks/use-auth';
import { AuthAvatar } from '../components/ui/auth-avatar';
import { Typography } from '@heroui/react';
import ShapeGrid from '../components/reactbits/ShapeGrid';

export default function Home() {
  const { isAuthenticated, user } = useAuth();

  return (
    <div className="dark relative min-h-screen w-full bg-background text-foreground flex flex-col overflow-hidden">

      {/* ReactBits ShapeGrid — full-page animated background */}
      <div className="fixed inset-0 pointer-events-none z-0">
        <ShapeGrid
          direction="diagonal"
          speed={0.4}
          borderColor="oklch(28% 0 0)"
          squareSize={48}
          hoverFillColor="oklch(25% 0 0)"
          hoverTrailAmount={8}
          className="w-full h-full"
        />
        {/* Radial vignette to focus attention on center */}
        <div className="absolute inset-0 bg-[radial-gradient(ellipse_80%_60%_at_50%_40%,transparent_30%,oklch(12%_0_0/90%)_100%)]" />
      </div>

      {/* Header */}
      <header className="relative z-10 w-full max-w-7xl mx-auto px-6 h-20 flex items-center justify-between select-none">
        <div className="flex items-center gap-2.5">
          <div className="w-9 h-9 rounded-xl bg-foreground text-background flex items-center justify-center shadow-lg">
            <Compass size={18} />
          </div>
          <span className="text-sm font-extrabold tracking-tight text-foreground">
            CVerify
          </span>
        </div>

        <nav className="hidden md:flex items-center gap-6 text-sm text-muted font-medium">
          <span className="hover:text-foreground transition-colors cursor-pointer">Platform</span>
          <span className="hover:text-foreground transition-colors cursor-pointer">Docs</span>
          <span className="hover:text-foreground transition-colors cursor-pointer">Pricing</span>
        </nav>

        <div className="flex items-center gap-4">
          {isAuthenticated ? (
            <div className="flex items-center gap-4">
              <Link href={`/${user?.role?.toLowerCase() || 'user'}`} className="text-sm font-semibold text-muted hover:text-foreground transition-colors">
                Dashboard
              </Link>
              <AuthAvatar />
            </div>
          ) : (
            <>
              <Link href="/login" className="text-sm font-semibold text-muted hover:text-foreground transition-colors">
                Sign In
              </Link>
              <Link href="/register">
                <button className="px-4 py-2 rounded-xl text-xs font-bold bg-foreground text-background hover:opacity-90 transition-all cursor-pointer">
                  Get Started
                </button>
              </Link>
            </>
          )}
        </div>
      </header>

      {/* Hero */}
      <main className="relative z-10 flex-1 flex flex-col items-center justify-center text-center px-6 py-24 space-y-8">

        <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full border border-border/60 bg-surface/40 backdrop-blur-md text-xs font-semibold text-muted">
          <Sparkles size={11} className="text-foreground/60" />
          AI-Powered Candidate Verification Platform
        </div>

        <h1 className="max-w-3xl text-5xl sm:text-7xl font-extrabold tracking-tight leading-[1.04] text-foreground">
          Verify Skills.{' '}
          <span className="text-muted font-light">
            Match Talent.
          </span>
          <br />
          Hire with Confidence.
        </h1>

        <p className="max-w-xl text-muted text-base leading-relaxed font-light">
          CVerify uses AI to audit candidate source code, validate CVs against job descriptions,
          and surface verified technical signals — so your hiring team spends time on people, not paperwork.
        </p>

        <div className="flex flex-col sm:flex-row gap-3 pt-2">
          <Link href={isAuthenticated ? `/${user?.role?.toLowerCase() || 'user'}` : '/register'}>
            <button className="h-12 px-8 rounded-xl text-sm font-bold bg-foreground text-background hover:opacity-90 transition-all flex items-center gap-2 group cursor-pointer">
              Start Verifying
              <ArrowRight size={15} className="transition-transform group-hover:translate-x-0.5" />
            </button>
          </Link>
          <Link href={isAuthenticated ? `/${user?.role?.toLowerCase() || 'user'}` : '/login'}>
            <button className="h-12 px-8 rounded-xl text-sm font-bold border border-border/60 bg-surface/30 backdrop-blur-sm hover:bg-surface/50 transition-all text-foreground cursor-pointer">
              {isAuthenticated ? 'Go to Dashboard' : 'Sign In'}
            </button>
          </Link>
        </div>

        {/* Trust strip */}
        <div className="flex items-center gap-6 pt-4 text-xs text-muted/60 font-medium select-none">
          <span className="flex items-center gap-1.5">
            <BadgeCheck size={12} className="text-success" />
            Source code audited
          </span>
          <span className="w-px h-3 bg-border/40" />
          <span className="flex items-center gap-1.5">
            <BadgeCheck size={12} className="text-success" />
            JD-to-CV matching
          </span>
          <span className="w-px h-3 bg-border/40" />
          <span className="flex items-center gap-1.5">
            <BadgeCheck size={12} className="text-success" />
            Workspace collaboration
          </span>
        </div>
      </main>

      {/* Features grid */}
      <section className="relative z-10 w-full max-w-7xl mx-auto px-6 pb-24">

        <div className="text-center mb-12 space-y-2 select-none">
          <Typography type="body-xs" className="uppercase tracking-widest font-extrabold text-muted/60">
            Platform Capabilities
          </Typography>
          <Typography type="h2" className="text-2xl font-bold text-foreground">
            Everything your team needs to hire smarter
          </Typography>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">

          <Card className="bg-surface/50 border-border/40 backdrop-blur-lg group hover:border-border/80 transition-colors" glow={false}>
            <div className="w-10 h-10 rounded-xl bg-surface-secondary flex items-center justify-center mb-5 text-foreground group-hover:bg-surface-tertiary transition-colors">
              <FileSearch size={18} />
            </div>
            <Typography type="h3" className="font-bold mb-2 text-foreground">
              CV Verification
            </Typography>
            <Typography type="body-xs" className="text-muted leading-relaxed">
              Parse and validate candidate resumes against standardized job descriptions.
              Surface skill gaps and highlight strengths with AI-powered analysis.
            </Typography>
          </Card>

          <Card className="bg-surface/50 border-border/40 backdrop-blur-lg group hover:border-border/80 transition-colors" glow={false}>
            <div className="w-10 h-10 rounded-xl bg-surface-secondary flex items-center justify-center mb-5 text-foreground group-hover:bg-surface-tertiary transition-colors">
              <GitBranch size={18} />
            </div>
            <Typography type="h3" className="font-bold mb-2 text-foreground">
              Source Code Audit
            </Typography>
            <Typography type="body-xs" className="text-muted leading-relaxed">
              Connect GitHub repositories and receive an AI-generated technical skill report —
              code quality, patterns, language proficiency, and real signal beyond the resume.
            </Typography>
          </Card>

          <Card className="bg-surface/50 border-border/40 backdrop-blur-lg group hover:border-border/80 transition-colors" glow={false}>
            <div className="w-10 h-10 rounded-xl bg-surface-secondary flex items-center justify-center mb-5 text-foreground group-hover:bg-surface-tertiary transition-colors">
              <BrainCircuit size={18} />
            </div>
            <Typography type="h3" className="font-bold mb-2 text-foreground">
              JD Matching Engine
            </Typography>
            <Typography type="body-xs" className="text-muted leading-relaxed">
              Define structured job descriptions and let the AI rank candidates by verified
              fit score — combining skills, experience, code quality, and role alignment.
            </Typography>
          </Card>

          <Card className="bg-surface/50 border-border/40 backdrop-blur-lg group hover:border-border/80 transition-colors" glow={false}>
            <div className="w-10 h-10 rounded-xl bg-surface-secondary flex items-center justify-center mb-5 text-foreground group-hover:bg-surface-tertiary transition-colors">
              <ShieldCheck size={18} />
            </div>
            <Typography type="h3" className="font-bold mb-2 text-foreground">
              Role-Based Access
            </Typography>
            <Typography type="body-xs" className="text-muted leading-relaxed">
              Granular permission model with recruiter, business, and admin roles.
              Control exactly who can view, evaluate, and decide on candidate pipelines.
            </Typography>
          </Card>

          <Card className="md:col-span-2 bg-surface/50 border-border/40 backdrop-blur-lg group hover:border-border/80 transition-colors" glow={false}>
            <div className="flex items-start gap-5">
              <div className="w-10 h-10 rounded-xl bg-surface-secondary flex items-center justify-center shrink-0 text-foreground group-hover:bg-surface-tertiary transition-colors">
                <Sparkles size={18} />
              </div>
              <div>
                <Typography type="h3" className="font-bold mb-2 text-foreground">
                  Workspace Collaboration
                </Typography>
                <Typography type="body-xs" className="text-muted leading-relaxed">
                  Invite your entire hiring team to a shared workspace. Create recruitment pipelines,
                  assign reviewers, and track candidates through every stage — from JD creation to
                  final hire decision — all in one place.
                </Typography>
              </div>
            </div>
          </Card>

        </div>
      </section>

      {/* Footer */}
      <footer className="relative z-10 w-full max-w-7xl mx-auto px-6 py-8 border-t border-border/20 flex items-center justify-between text-xs text-muted/50 select-none">
        <div className="flex items-center gap-2">
          <Compass size={13} />
          <span className="font-bold text-muted/60">CVerify</span>
        </div>
        <span>AI Recruitment Verification Platform · Built on Next.js + HeroUI</span>
      </footer>
    </div>
  );
}
