"use client";

import React, { useState } from "react";
import Link from "next/link";
import { 
  Shield, 
  ArrowLeft, 
  ChevronRight, 
  Scale, 
  Database, 
  GitBranch, 
  Sparkles, 
  UserCheck, 
  Key, 
  AlertTriangle 
} from "lucide-react";
import { PublicPageShell } from "@/components/ui/public-page-shell";
import { AuthFooter } from "@/features/auth/components/auth-footer";

export default function PrivacyPolicyPage() {
  const [activeSection, setActiveSection] = useState("intro");

  const sections = [
    { id: "intro", title: "1. Introduction & Corporate Identity", icon: Scale },
    { id: "scope", title: "2. Scope & Applicability", icon: Shield },
    { id: "definitions", title: "3. Key Legal Definitions", icon: Scale },
    { id: "principles", title: "4. Core Privacy Principles", icon: Shield },
    { id: "dpo", title: "5. Data Protection Officer (DPO)", icon: UserCheck },
    { id: "categories", title: "6. Categories of Personal Data", icon: Database },
    { id: "sensitive", title: "7. Sensitive Data Disclosures", icon: AlertTriangle },
    { id: "git", title: "10. Git Analysis & Ephemeral Clones", icon: GitBranch },
    { id: "provenance", title: "11. Code Provenance & Plagiarism", icon: AlertTriangle },
    { id: "chat", title: "12. AI Career Counselor Chat", icon: Sparkles },
    { id: "automated", title: "13. Automated Decision Transparency", icon: Sparkles },
    { id: "visibility", title: "14. Profile Visibility Controls", icon: UserCheck },
    { id: "encryption", title: "23. Encryption & Hash Matrix", icon: Key },
    { id: "retention", title: "21. Data Retention & Erasure", icon: Database },
    { id: "rights", title: "30. Candidate Privacy Rights", icon: Scale },
  ];

  return (
    <PublicPageShell
      guestFooter={<AuthFooter />}
      guestContainerClassName="min-h-screen bg-background text-foreground flex flex-col font-sans select-text"
      guestMainClassName="max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-8 flex-1 flex flex-col gap-6"
    >
      {/* Header Banner */}
      <div className="relative overflow-hidden rounded-3xl bg-gradient-to-r from-surface-secondary/40 via-surface/60 to-surface-secondary/40 border border-border p-8 sm:p-12 shadow-lg backdrop-blur-sm select-none">
        <div className="absolute inset-0 bg-[radial-gradient(var(--separator)_1px,transparent_1px)] bg-size-[20px_20px] pointer-events-none opacity-20" />
        <div className="relative z-10 flex flex-col gap-4">
          <Link href="/" className="inline-flex items-center gap-2 text-xs font-semibold text-muted hover:text-foreground transition-colors w-fit">
            <ArrowLeft size={14} />
            Back to Home
          </Link>
          <div className="flex items-center gap-3">
            <div className="p-3 rounded-2xl bg-primary/10 border border-primary/20 text-primary">
              <Shield size={32} />
            </div>
            <div>
              <h1 className="text-3xl sm:text-4xl font-extrabold tracking-tight">Privacy Policy</h1>
              <p className="text-xs text-muted mt-1">Version 1.0.0-GA • Effective July 18, 2026</p>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        
        {/* Sticky Sidebar Navigation */}
        <nav className="lg:col-span-4 sticky top-6 bg-surface border border-border rounded-2xl p-4 shadow-sm flex flex-col gap-2 select-none">
          <p className="text-[10px] uppercase font-bold text-muted tracking-wider px-3 mb-2">Policy Navigation</p>
          <div className="flex flex-col gap-1 max-h-[60vh] overflow-y-auto custom-scrollbar">
            {sections.map((sec) => {
              const Icon = sec.icon;
              const isActive = activeSection === sec.id;
              return (
                <button
                  key={sec.id}
                  onClick={() => {
                    setActiveSection(sec.id);
                    document.getElementById(sec.id)?.scrollIntoView({ behavior: "smooth" });
                  }}
                  className={`w-full text-left px-3 py-2.5 rounded-xl text-xs font-medium flex items-center justify-between transition-all cursor-pointer ${
                    isActive 
                      ? "bg-foreground text-background font-semibold" 
                      : "hover:bg-surface-secondary text-muted hover:text-foreground"
                  }`}
                >
                  <span className="flex items-center gap-2.5 truncate">
                    <Icon size={14} />
                    {sec.title}
                  </span>
                  <ChevronRight size={12} className={isActive ? "opacity-100" : "opacity-0"} />
                </button>
              );
            })}
          </div>
        </nav>

        {/* Content Panel */}
        <div className="lg:col-span-8 bg-surface border border-border rounded-2xl p-6 sm:p-10 shadow-sm flex flex-col gap-10">
          
          {/* Section: Intro */}
          <section id="intro" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Scale size={20} className="text-primary" />
              1. Introduction & Corporate Identity
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                This Privacy Policy governs the processing of personal data by <strong>CVerify Joint Stock Company</strong> (&quot;CVerify&quot;, &quot;we&quot;, &quot;us&quot;, or &quot;our&quot;), a corporate entity registered under the laws of the Socialist Republic of Vietnam. CVerify acts as a Data Controller for candidate profile information created directly on our platform and as a Data Processor on behalf of employer organizations using CVerify workspaces.
              </p>
              <div className="p-4 rounded-xl bg-surface-secondary/50 border border-border text-xs space-y-2">
                <p className="font-semibold text-foreground">💡 Examples:</p>
                <ul className="list-disc pl-4 space-y-1">
                  <li>When a candidate registers at <code>cverify.com</code>, CVerify acts as the Data Controller.</li>
                  <li>When a candidate submits an application to a specific job vacancy posted by an employer under a workspace, the employer acts as the Data Controller, and CVerify acts as the Data Processor.</li>
                </ul>
              </div>
            </div>
          </section>

          {/* Section: Scope */}
          <section id="scope" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Shield size={20} className="text-primary" />
              2. Scope & Applicability
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                This policy applies to all visitors, registered candidates, organization owners, administrators, recruiters, interviewers, and partners interacting with the CVerify platform, websites, and applications.
              </p>
              <div className="p-4 rounded-xl bg-surface-secondary/50 border border-border text-xs space-y-2">
                <p className="font-semibold text-foreground">💡 Examples:</p>
                <ul className="list-disc pl-4 space-y-1">
                  <li>A developer visiting our homepage to read our blog.</li>
                  <li>A recruiter conducting an interview evaluation using our Workspace portal.</li>
                </ul>
              </div>
            </div>
          </section>

          {/* Section: Definitions */}
          <section id="definitions" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Scale size={20} className="text-primary" />
              3. Key Legal Definitions
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <ul className="list-disc pl-4 space-y-2">
                <li><strong>Candidate:</strong> A developer user registering to build a profile or verify source code.</li>
                <li><strong>Trust Score:</strong> An automated capability indicator calculated by CVerify&apos;s background ranking engine.</li>
                <li><strong>Lizard AST:</strong> The static code complexity analyzer utilized to evaluate source files.</li>
                <li><strong>MinHash LSH:</strong> The clone-detection algorithm used to identify code duplicates.</li>
              </ul>
            </div>
          </section>

          {/* Section: Principles */}
          <section id="principles" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Shield size={20} className="text-primary" />
              4. Core Privacy Principles
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                CVerify adheres to the principles of lawfulness, fairness, transparency, purpose limitation, data minimization, accuracy, storage limitation, integrity, and confidentiality. We design all features with Privacy by Design (PbD) configurations.
              </p>
            </div>
          </section>

          {/* Section: DPO */}
          <section id="dpo" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <UserCheck size={20} className="text-primary" />
              5. Data Protection Officer (DPO) & Governance
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                CVerify has appointed a Data Protection Officer (DPO) to oversee compliance with GDPR, CCPA, and PDPA. Users can contact the DPO directly regarding privacy matters via email at <code>dpo@cverify.com</code>.
              </p>
            </div>
          </section>

          {/* Section: Categories */}
          <section id="categories" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Database size={20} className="text-primary" />
              6. Categories of Personal Data Processed
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>We process the following data points to maintain secure and verified profiles:</p>
              <div className="overflow-x-auto">
                <table className="w-full text-xs text-left border-collapse border border-border">
                  <thead>
                    <tr className="bg-surface-secondary">
                      <th className="p-3 border border-border font-bold">Category</th>
                      <th className="p-3 border border-border font-bold">Specific Elements</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td className="p-3 border border-border font-medium">Identity Data</td>
                      <td className="p-3 border border-border text-muted">Name, email, password hashes, Google Subject ID.</td>
                    </tr>
                    <tr>
                      <td className="p-3 border border-border font-medium">Portfolio Data</td>
                      <td className="p-3 border border-border text-muted">Work experience logs, education history, skills, project titles, repository URLs.</td>
                    </tr>
                    <tr>
                      <td className="p-3 border border-border font-medium">Technical Telemetry</td>
                      <td className="p-3 border border-border text-muted">IP addresses, browser User-Agent strings, access logs, correlation IDs.</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </section>

          {/* Section: Sensitive */}
          <section id="sensitive" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <AlertTriangle size={20} className="text-primary" />
              7. Sensitive Personal Data Disclosures
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                CVerify does not collect sensitive personal data such as political opinions, religious beliefs, union memberships, health details, genetic profiles, or sexual orientation. Government registration IDs submitted during company validation are strictly restricted.
              </p>
            </div>
          </section>

          {/* Section: Git */}
          <section id="git" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <GitBranch size={20} className="text-primary" />
              10. Git Repository Analysis & Ephemeral Clones
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                CVerify analyzes connected repositories to verify developer contributions. The FastAPI AI microservice ephemerally clones selected repositories to a transient disk volume. Once the static analysis runs (calculating AST metrics via Lizard and plagiarism vectors), the code folder is permanently deleted.
              </p>
              <div className="p-4 rounded-xl bg-surface-secondary border border-border/80 font-mono text-xs select-none">
                [Git Repo] --&gt; [Secure Clone (tmpfs)] --&gt; [AST Engine Analysis] --&gt; [Cleanup & Delete Folder]
              </div>
            </div>
          </section>

          {/* Section: Provenance */}
          <section id="provenance" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <AlertTriangle size={20} className="text-primary" />
              11. Code Provenance & Plagiarism Analysis
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                CVerify calculates code signatures using the DataSketch MinHash LSH engine. This generates digital hashes of code fragments to identify files copied from public repositories or AI generation pools. Standard libraries are filtered out to minimize false positives.
              </p>
            </div>
          </section>

          {/* Section: Chat */}
          <section id="chat" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Sparkles size={20} className="text-primary" />
              12. Interactive AI Career Counselor Chat
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                CVerify offers a career counselor chat. The chat interface sends prompts and context window details (limited to the last 10 messages) to an external FastAPI parser connecting to Anthropic Claude. Conversations are encrypted in transit and signed with secure HMAC headers.
              </p>
            </div>
          </section>

          {/* Section: Automated */}
          <section id="automated" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Sparkles size={20} className="text-primary" />
              13. Automated Decision-Making & Trust Score Transparency
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                CVerify calculates a Composite Trust Score based on:
                <br />
                <code>CompositeScore = (AiScore * 0.35) + (TrustScore * 0.35) + (Completeness * 0.15) + (OssImpactScore * 0.15)</code>
              </p>
              <p>
                These scores represent automated evaluations. However, CVerify does not make hiring decisions; recruiters must review candidates manually, conforming with GDPR Article 22.
              </p>
            </div>
          </section>

          {/* Section: Visibility */}
          <section id="visibility" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <UserCheck size={20} className="text-primary" />
              14. Profile Visibility & Leaderboard Controls
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                Candidates can configure their profile visibility settings (Public, Private, or Restricted). profiles default to &quot;Private&quot; upon registration, giving users full control over their public search engine exposure and leaderboard projections.
              </p>
            </div>
          </section>

          {/* Section: Encryption */}
          <section id="encryption" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Key size={20} className="text-primary" />
              23. Data Encryption & Cryptographic Hash Matrix
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>We secure personal data using industry-standard hashing and encryption:</p>
              <ul className="list-disc pl-4 space-y-2">
                <li><strong>Passwords:</strong> Hashed using BCrypt.</li>
                <li><strong>Verification/Refresh Tokens:</strong> Encrypted using SHA-256 hashes.</li>
                <li><strong>Git Access Tokens:</strong> Encrypted symmetrically at rest via AES-256-GCM.</li>
                <li><strong>Transit:</strong> Enforced SSL/TLS 1.3 across all REST and SSE endpoints.</li>
              </ul>
            </div>
          </section>

          {/* Section: Retention */}
          <section id="retention" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Database size={20} className="text-primary" />
              21. Data Minimization & Retention Schedules
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                When you delete your account, it enters a deactivation state for 10 days to allow recovery. After the grace period, all resume, attachment files, and database entities are permanently hard-deleted or anonymized.
              </p>
            </div>
          </section>

          {/* Section: Rights */}
          <section id="rights" className="scroll-mt-6 flex flex-col gap-4">
            <h2 className="text-xl font-bold flex items-center gap-2 pb-2 border-b border-separator/85">
              <Scale size={20} className="text-primary" />
              30. Candidate Privacy Rights (GDPR/PDPA/CCPA)
            </h2>
            <div className="space-y-4 text-sm text-muted leading-relaxed">
              <p>
                You hold legal rights regarding your personal data under global regulations, including the rights to:
              </p>
              <ul className="list-disc pl-4 space-y-1">
                <li>Access all personal data categories stored about you.</li>
                <li>Rectify or update incomplete or incorrect portfolio attributes.</li>
                <li>Erase your data (Right to be Forgotten) completely.</li>
                <li>Port your profile data in a structured, machine-readable JSON format.</li>
                <li>Restrict or object to automated profiling.</li>
              </ul>
              <p className="mt-2 text-xs">
                To exercise any of these rights, please contact our team at <code>dpo@cverify.com</code>.
              </p>
            </div>
          </section>

        </div>
      </div>
    </PublicPageShell>
  );
}
