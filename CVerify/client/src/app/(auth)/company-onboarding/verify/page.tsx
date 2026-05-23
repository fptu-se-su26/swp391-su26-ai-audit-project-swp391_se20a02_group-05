"use client";

import React, { useState, useEffect } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { useAuth } from '../../../../features/auth/hooks/use-auth';
import { Card, Typography, Button, toast } from "@heroui/react";
import { ShieldAlert, ShieldCheck, RefreshCw } from 'lucide-react';
import { Suspense } from 'react';

function VerifyLinkContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { verifyCompanyLink } = useAuth();

  const token = searchParams.get('token') || '';

  // Processing state
  const [status, setStatus] = useState<'verifying' | 'success' | 'error'>('verifying');
  const [companyDetails, setCompanyDetails] = useState<{
    companyName: string;
    taxCode: string;
    companyEmail: string;
    verificationToken: string;
  } | null>(null);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    if (!token) {
      setStatus('error');
      setErrorMessage("No token provided. Please check the authorization link sent to your email.");
      return;
    }

    const runVerification = async () => {
      const result = await verifyCompanyLink(token);
      if (result.success && result.data) {
        setCompanyDetails(result.data);
        setStatus('success');
        toast.success("Authorization Verified", {
          description: "Company ownership proven successfully."
        });
      } else {
        setStatus('error');
        setErrorMessage(result.error?.message || "Verification link is invalid, expired, or already consumed.");
        toast.danger("Verification Failed");
      }
    };

    runVerification();
  }, [token]);

  const handleProceedToWorkspace = () => {
    if (!companyDetails) return;
    const query = new URLSearchParams({
      email: companyDetails.companyEmail,
      token: companyDetails.verificationToken
    }).toString();
    router.push(`/workspace-setup?${query}`);
  };

  return (
    <Card className="w-full bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 p-8 shadow-xl rounded-2xl">
      {status === 'verifying' && (
        <div className="w-full flex flex-col items-center py-8 text-center">
          <RefreshCw className="size-10 text-zinc-400 dark:text-zinc-600 animate-spin mb-6" />
          <Typography.Heading level={3} className="text-xl font-bold pb-2 text-zinc-900 dark:text-zinc-100">
            Verifying link...
          </Typography.Heading>
          <Typography className="text-sm text-zinc-500 dark:text-zinc-400">
            Cryptographically validating company ownership authorization.
          </Typography>
        </div>
      )}

      {status === 'success' && companyDetails && (
        <div className="w-full flex flex-col items-center text-center">
          <div className="w-16 h-16 bg-emerald-50 dark:bg-emerald-950 flex items-center justify-center rounded-2xl mb-6">
            <ShieldCheck className="size-8 text-emerald-600 dark:text-emerald-400" />
          </div>

          <Typography.Heading level={3} className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100">
            Verification Successful
          </Typography.Heading>
          
          <Typography className="text-sm text-zinc-500 dark:text-zinc-400 mb-8 max-w-sm">
            Company ownership has been proven. You are authorized to setup the company workspace.
          </Typography>

          <div className="w-full bg-zinc-50 dark:bg-zinc-850 border border-zinc-200 dark:border-zinc-800 rounded-xl p-5 mb-8 text-left space-y-3 font-outfit">
            <div>
              <span className="text-xs font-semibold uppercase text-zinc-450 dark:text-zinc-550 block">Company Name</span>
              <span className="text-sm font-bold text-zinc-800 dark:text-zinc-200">{companyDetails.companyName}</span>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <span className="text-xs font-semibold uppercase text-zinc-450 dark:text-zinc-550 block">Tax Code</span>
                <span className="text-sm font-bold text-zinc-800 dark:text-zinc-200">{companyDetails.taxCode}</span>
              </div>
              <div>
                <span className="text-xs font-semibold uppercase text-zinc-450 dark:text-zinc-550 block">Verified Email</span>
                <span className="text-sm font-bold text-zinc-800 dark:text-zinc-200 truncate block">{companyDetails.companyEmail}</span>
              </div>
            </div>
          </div>

          <Button
            fullWidth
            className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-semibold"
            onPress={handleProceedToWorkspace}
          >
            Set Up Workspace
          </Button>
        </div>
      )}

      {status === 'error' && (
        <div className="w-full flex flex-col items-center text-center py-4">
          <div className="w-16 h-16 bg-rose-50 dark:bg-rose-950 flex items-center justify-center rounded-2xl mb-6">
            <ShieldAlert className="size-8 text-rose-600 dark:text-rose-400" />
          </div>

          <Typography.Heading level={3} className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100">
            Authorization Failed
          </Typography.Heading>
          
          <Typography className="text-sm text-zinc-500 dark:text-zinc-400 mb-8 max-w-xs">
            {errorMessage}
          </Typography>

          <Button
            variant="secondary"
            className="h-12 rounded-xl text-zinc-800 dark:text-zinc-200 px-6"
            onPress={() => router.push('/login')}
          >
            Back to Sign In
          </Button>
        </div>
      )}
    </Card>
  );
}

export default function VerifyLinkPage() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center p-8 min-h-[400px]">
        <div className="w-8 h-8 border-2 border-t-zinc-900 border-zinc-200 dark:border-t-zinc-100 rounded-full animate-spin" />
      </div>
    }>
      <VerifyLinkContent />
    </Suspense>
  );
}
