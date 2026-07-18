"use client";

import React, { useState } from "react";
import Link from "next/link";
import { ArrowLeft, Mail, Send, CheckCircle, Info } from "lucide-react";
import { PublicPageShell } from "@/components/ui/public-page-shell";
import { AuthFooter } from "@/features/auth/components/auth-footer";

export default function ContactPage() {
  const [formData, setFormData] = useState({ name: "", email: "", subject: "", message: "" });
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.name && formData.email && formData.message) {
      setSubmitted(true);
    }
  };

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
              <Mail size={32} />
            </div>
            <div>
              <h1 className="text-3xl font-extrabold tracking-tight">Contact Us</h1>
              <p className="text-xs text-muted mt-1">Get in touch with the CVerify Compliance & Support team</p>
            </div>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-12 gap-8">
        <div className="md:col-span-4 bg-surface border border-border rounded-2xl p-6 shadow-sm flex flex-col gap-6">
          <div>
            <h3 className="text-sm font-bold text-foreground mb-1 uppercase tracking-wider">Legal Inquiries</h3>
            <p className="text-xs text-muted">For data requests, GDPR/CCPA questions, or DPO coordination:</p>
            <a href="mailto:dpo@cverify.com" className="text-xs font-semibold text-primary hover:underline mt-2 inline-block">
              dpo@cverify.com
            </a>
          </div>

          <div className="border-t border-separator/85 pt-4">
            <h3 className="text-sm font-bold text-foreground mb-1 uppercase tracking-wider">Enterprise Sales</h3>
            <p className="text-xs text-muted">To scale up organization workspace limits and customize requirements:</p>
            <a href="mailto:sales@cverify.com" className="text-xs font-semibold text-primary hover:underline mt-2 inline-block">
              sales@cverify.com
            </a>
          </div>
        </div>

        <div className="md:col-span-8 bg-surface border border-border rounded-2xl p-6 sm:p-8 shadow-sm">
          {submitted ? (
            <div className="flex flex-col items-center text-center gap-4 py-12">
              <div className="p-4 rounded-full bg-success/10 border border-success/20 text-success">
                <CheckCircle size={40} />
              </div>
              <h2 className="text-xl font-bold">Message Sent Successfully</h2>
              <p className="text-sm text-muted max-w-sm">
                Thank you for contacting CVerify support. Our compliance officer or help desk will review your inquiry and get back to you within 2 business days.
              </p>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="flex flex-col gap-5">
              <div className="flex flex-col gap-2">
                <label className="text-xs font-bold uppercase tracking-wider text-muted">Your Name</label>
                <input
                  type="text"
                  required
                  placeholder="John Doe"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  className="w-full h-11 px-4 rounded-xl border border-border bg-surface-secondary text-sm focus:outline-none focus:border-primary transition-colors"
                />
              </div>

              <div className="flex flex-col gap-2">
                <label className="text-xs font-bold uppercase tracking-wider text-muted">Email Address</label>
                <input
                  type="email"
                  required
                  placeholder="john.doe@company.com"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full h-11 px-4 rounded-xl border border-border bg-surface-secondary text-sm focus:outline-none focus:border-primary transition-colors"
                />
              </div>

              <div className="flex flex-col gap-2">
                <label className="text-xs font-bold uppercase tracking-wider text-muted">Message</label>
                <textarea
                  required
                  rows={5}
                  placeholder="Describe your inquiry in detail..."
                  value={formData.message}
                  onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                  className="w-full p-4 rounded-xl border border-border bg-surface-secondary text-sm focus:outline-none focus:border-primary transition-colors resize-none"
                />
              </div>

              <button
                type="submit"
                className="w-full h-12 bg-foreground text-background hover:opacity-90 font-semibold rounded-xl text-sm flex items-center justify-center gap-2 cursor-pointer transition-all mt-2"
              >
                <Send size={16} />
                Send Message
              </button>
            </form>
          )}
        </div>
      </div>
    </PublicPageShell>
  );
}
