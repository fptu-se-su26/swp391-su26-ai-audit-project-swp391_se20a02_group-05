"use client";

import React, { useState } from "react";
import Link from "next/link";
import { ArrowLeft, FileText, Scale, ShieldAlert, CheckCircle } from "lucide-react";
import { PublicPageShell } from "@/components/ui/public-page-shell";
import { AuthFooter } from "@/features/auth/components/auth-footer";

export default function TermsOfServicePage() {
  const [accepted, setAccepted] = useState(false);

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
          <div className="flex items-center gap-3">
            <div className="p-3 rounded-2xl bg-primary/10 border border-primary/20 text-primary">
              <FileText size={32} />
            </div>
            <div>
              <h1 className="text-3xl font-extrabold tracking-tight">Terms of Service</h1>
              <p className="text-xs text-muted mt-1">Version 1.0.0 • Last Updated: July 18, 2026</p>
            </div>
          </div>
        </div>
      </div>

      <div className="bg-surface border border-border rounded-2xl p-6 sm:p-10 shadow-sm space-y-8 text-sm text-muted leading-relaxed">
        <section className="space-y-3">
          <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
            <Scale size={18} className="text-primary" />
            1. Acceptance of Terms
          </h2>
          <p>
            By accessing or using the CVerify platform, APIs, websites, and associated microservices (collectively, the "Service"), you agree to be bound by these Terms of Service ("Terms"). If you do not agree to these Terms, you must immediately cease all access and usage of the Service. CVerify reserves the right to modify, amend, or replace these Terms at any time, with notifications of significant changes posted on our dashboard and sent via registered email.
          </p>
          <p>
            These Terms constitute a legally binding agreement between you (whether as a registered Candidate, Recruiter, Organization Owner, or general visitor) and CVerify Joint Stock Company.
          </p>
        </section>

        <section className="space-y-3">
          <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
            <ShieldAlert size={18} className="text-primary" />
            2. Scope of Service & AI Disclaimers
          </h2>
          <p>
            CVerify provides automated developer capability profiling, static code analysis (via Lizard AST complexity engines), plagiarism/clone checking (via DataSketch MinHash algorithms), and generative evaluations (via Anthropic Claude API). 
          </p>
          <p>
            You explicitly acknowledge and agree that:
          </p>
          <ul className="list-disc pl-5 space-y-2">
            <li><strong>No Guarantee of Hiring:</strong> The Composite Trust Score, capability mappings, and AI personas are provided strictly as diagnostic indicators. CVerify does not make hiring choices or guarantee job placement.</li>
            <li><strong>Automated Assessment Calibration:</strong> AI evaluations are based on linked Git repository history and profile inputs. While CVerify strives for precision, we do not warrant that evaluations are error-free or represent absolute technical capability.</li>
            <li><strong>Third-party Services:</strong> Integrations with platforms like GitHub, GitLab, and Stripe are governed by their respective operational terms. CVerify is not responsible for API disruptions or failures on these external nodes.</li>
          </ul>
        </section>

        <section className="space-y-3">
          <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
            <Scale size={18} className="text-primary" />
            3. Account Registration & Namespace Rights
          </h2>
          <p>
            To use key features of CVerify, you must register a user account. You agree to provide accurate, current, and complete credentials. You are solely responsible for maintaining the confidentiality of your session parameters (JWT tokens, password hashes).
          </p>
          <p>
            CVerify reserves the right to block, reclaim, or modify any user account namespace (username) that violates trademarks, is deemed offensive, or conflicts with platform system reserved words. Registered namespaces are non-transferable without written authorization.
          </p>
        </section>

        <section className="space-y-3">
          <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
            <ShieldAlert size={18} className="text-primary" />
            4. Git Integration & Source Code Licensing
          </h2>
          <p>
            When you connect a GitHub or GitLab repository to CVerify, you grant us a temporary, non-exclusive, royalty-free, worldwide license to pull metadata, clone repository trees ephemerally to transient worker volumes, and parse file structures for static analysis. 
          </p>
          <p>
            You warrant that you hold the legal authority, copyright, or developer permissions to link all repositories connected to your profile, including any private codebases.
          </p>
        </section>

        <section className="space-y-3">
          <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
            <Scale size={18} className="text-primary" />
            5. Acceptable Use Policy & Platform Restrictions
          </h2>
          <p>
            You agree not to engage in any of the following prohibited behaviors:
          </p>
          <ul className="list-disc pl-5 space-y-2">
            <li>Attempting to spoof, falsify, or inflate commit counts or author attribution ratios.</li>
            <li>Uploading files containing malware, trojans, or corrupt data packages to Cloudflare R2 storage.</li>
            <li>Executing denial-of-service (DDoS) scripts against API routers or intercepting FastAPI HMAC payloads.</li>
            <li>Automated data scraping of CVerify public leaderboards or database records without permission.</li>
          </ul>
        </section>

        <section className="space-y-3">
          <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
            <ShieldAlert size={18} className="text-primary" />
            6. Limitation of Liability & Indemnification
          </h2>
          <p>
            In no event shall CVerify, its directors, employees, or sub-processors be liable for any direct, indirect, incidental, or consequential damages resulting from your use or inability to use the Service. You agree to indemnify and hold harmless CVerify J.S.C. from any legal claims, liabilities, or disputes arising from your portfolio evaluations or hiring outcomes.
          </p>
        </section>

        <div className="border-t border-separator/85 pt-6 flex flex-col sm:flex-row items-center justify-between gap-4">
          <p className="text-xs text-muted">Please read these Terms carefully before using CVerify.</p>
          <button
            onClick={() => setAccepted(true)}
            className={`px-6 h-10 rounded-xl text-xs font-semibold flex items-center gap-2 transition-all cursor-pointer ${
              accepted 
                ? "bg-success/20 text-success border border-success/30 cursor-default" 
                : "bg-foreground text-background hover:opacity-90"
            }`}
          >
            {accepted ? <CheckCircle size={14} /> : null}
            {accepted ? "Terms Accepted" : "I Agree to the Terms"}
          </button>
        </div>
      </div>
    </PublicPageShell>
  );
}
