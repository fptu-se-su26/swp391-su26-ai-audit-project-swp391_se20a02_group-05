"use client";

import React, { useState, useEffect, useRef } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import { authApi } from '../../../features/auth/services/auth.service';
import { Card } from '../../../components/ui/card';
import { Button } from '../../../components/ui/button';
import { useAuth } from '../../../features/auth/hooks/use-auth';
import { useAuthStore } from '../../../features/auth/store/use-auth-store';
import Link from 'next/link';
import {
  ShieldCheck,
  ShieldAlert,
  Shield,
  Mail,
  ArrowRight,
  CheckCircle2,
  ChevronLeft,
  RefreshCw
} from 'lucide-react';
import { normalizeError } from '../../../services/axios-client';
import { toast } from '@heroui/react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useTranslation } from 'react-i18next';
import { Suspense } from 'react';

type ResendFormValues = {
  email: string;
};

type VerifyState = 'pending' | 'verifying' | 'success' | 'failed' | 'expired';

function VerifyEmailContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token');
  const emailFromUrl = searchParams.get('email');
  const effectRan = useRef(false);
  const { user, verifyEmail } = useAuth();
  const { pendingVerificationEmail } = useAuthStore();
  const { t } = useTranslation(['auth', 'common']);

  const resendSchema = z.object({
    email: z.string().email(t('auth:validation.emailInvalid')).max(255),
  });

  const targetEmail = pendingVerificationEmail || emailFromUrl || '';

  // Determine initial state based on presence of a verification token
  const [state, setState] = useState<VerifyState>(token ? 'verifying' : 'pending');
  const [message, setMessage] = useState(token ? t('auth:screens.verifyPending') : t('auth:subtitle.verify'));
  const [errorText, setErrorText] = useState<string | null>(null);
  const [showManualForm, setShowManualForm] = useState(!targetEmail);
  const [resendLoading, setResendLoading] = useState(false);

  const { register, handleSubmit, setValue, formState: { errors, isValid } } = useForm<ResendFormValues>({
    resolver: zodResolver(resendSchema),
    defaultValues: {
      email: targetEmail || user?.email || '',
    },
    mode: 'onChange',
  });

  // Keep email input in sync if user or store state hydrates late
  useEffect(() => {
    const activeEmail = targetEmail || user?.email || '';
    if (activeEmail) {
      setValue('email', activeEmail);
      if (showManualForm && pendingVerificationEmail) {
        queueMicrotask(() => {
          setShowManualForm(false);
        });
      }
    }
  }, [user, targetEmail, setValue, showManualForm, pendingVerificationEmail]);

  // Execute verification immediately if token is present
  useEffect(() => {
    if (!token) return;
    if (effectRan.current) return;

    const verify = async () => {
      const result = await verifyEmail(token);
      if (result.success) {
        setState('success');
        setMessage(t('auth:screens.verifySuccess'));

        toast.success(t('auth:toast.verifiedSuccessTitle'), {
          description: t('auth:toast.verifiedSuccessDesc'),
        });

        setTimeout(() => {
          router.push('/dashboard');
        }, 2000);
      } else {
        const error = result.error;
        if (error?.code === 'AUTH_EXPIRED_TOKEN') {
          setState('expired');
          setMessage(t('auth:screens.verifyExpired'));
        } else {
          setState('failed');
          setMessage(t('auth:screens.verifyFailed'));
        }
        setErrorText(error?.message || t('auth:screens.verifyFailed'));

        toast.danger(t('auth:toast.verifiedFailedTitle'), {
          description: error?.message || t('auth:toast.verifiedFailedDesc'),
        });
      }
    };

    verify();
    effectRan.current = true;
  }, [token, verifyEmail, router, t]);

  // Handler for resending the verification email
  const onResend = async (data: ResendFormValues) => {
    setResendLoading(true);
    try {
      await authApi.resendVerification(data.email);
      toast.success(t('auth:toast.linkSentTitle'), {
        description: t('auth:toast.linkSentDesc', { email: data.email }),
      });
      // Return state to pending since a new link was sent
      setState('pending');
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      toast.danger(t('auth:toast.linkSentFailedTitle'), {
        description: parsedError.message || t('auth:toast.linkSentFailedDesc'),
      });
    } finally {
      setResendLoading(false);
    }
  };

  return (
    <Card glow={true} className="transition-all duration-300">
      {/* 1. VERIFYING STATE */}
      {state === 'verifying' && (
        <div className="text-center py-6 flex flex-col items-center select-none">
          <div className="relative mb-6">
            <div className="w-16 h-16 rounded-full bg-zinc-950 text-indigo-500 flex items-center justify-center border border-indigo-500/20 shadow-[0_0_20px_rgba(99,102,241,0.15)] animate-pulse">
              <Shield size={32} className="animate-spin-slow" />
            </div>
            <div className="absolute inset-0 rounded-full border-t-2 border-indigo-500 animate-spin" />
          </div>

          <h2 className="text-2xl font-extrabold tracking-tight text-zinc-900 dark:text-zinc-50 mb-3 font-outfit">
            {t('auth:screens.verifyingTitle')}
          </h2>
          <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-8 max-w-sm">
            {message}
          </p>
        </div>
      )}

      {/* 2. SUCCESS STATE */}
      {state === 'success' && (
        <div className="text-center py-6 flex flex-col items-center select-none">
          <div className="w-16 h-16 rounded-full bg-emerald-950/30 text-emerald-500 flex items-center justify-center mb-6 shadow-[0_0_25px_rgba(16,185,129,0.3)] border border-emerald-500/20 animate-scale-up">
            <ShieldCheck size={36} className="text-emerald-400" />
          </div>

          <h2 className="text-2xl font-extrabold tracking-tight text-zinc-900 dark:text-zinc-50 mb-3 font-outfit">
            {t('auth:toast.verifiedSuccessTitle')}
          </h2>

          <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-8 max-w-sm">
            {t('auth:screens.verifySuccess')}
          </p>

          <Button
            variant="solid"
            className="w-full py-6 text-sm font-semibold rounded-xl bg-linear-to-r from-zinc-950 to-zinc-900 dark:from-white dark:to-zinc-100 hover:opacity-90 active:scale-[0.98] transition-all duration-200"
            onClick={() => router.push('/dashboard')}
          >
            {t('auth:actions.goToDashboard')}
            <ArrowRight className="ml-2 w-4.5 h-4.5" />
          </Button>
        </div>
      )}

      {/* 3. EXPIRED STATE */}
      {state === 'expired' && (
        <div className="text-center py-6 flex flex-col items-center select-none">
          <div className="w-16 h-16 rounded-full bg-amber-950/30 text-amber-500 flex items-center justify-center mb-6 shadow-[0_0_25px_rgba(245,158,11,0.3)] border border-amber-500/20 animate-bounce-short">
            <RefreshCw size={32} className="text-amber-400" />
          </div>

          <h2 className="text-2xl font-extrabold tracking-tight text-zinc-900 dark:text-zinc-50 mb-3 font-outfit">
            {t('auth:screens.verifyExpiredTitle')}
          </h2>

          <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-8 max-w-sm font-outfit">
            {t('auth:screens.verifyExpired')}
          </p>

          <div className="flex flex-col gap-3 w-full">
            <Button
              variant="solid"
              className="w-full py-6 text-sm font-semibold bg-zinc-900 hover:bg-zinc-800 text-white dark:bg-zinc-50 dark:hover:bg-zinc-100 dark:text-zinc-950"
              onClick={() => {
                if (targetEmail) {
                  onResend({ email: targetEmail });
                } else {
                  setShowManualForm(true);
                  setState('pending');
                }
              }}
            >
              {t('auth:toast.linkSentTitle')}
            </Button>
            <Button
              variant="bordered"
              className="w-full py-6 text-sm font-semibold font-outfit"
              onClick={() => router.push('/login')}
            >
              {t('auth:actions.backToLogin')}
            </Button>
          </div>
        </div>
      )}

      {/* 4. FAILED STATE */}
      {state === 'failed' && (
        <div className="text-center py-6 flex flex-col items-center select-none">
          <div className="w-16 h-16 rounded-full bg-red-950/30 text-red-500 flex items-center justify-center mb-6 shadow-[0_0_25px_rgba(239,68,68,0.3)] border border-red-500/20 animate-bounce-short">
            <ShieldAlert size={36} className="text-red-400" />
          </div>

          <h2 className="text-2xl font-extrabold tracking-tight text-zinc-900 dark:text-zinc-50 mb-3 font-outfit">
            {t('auth:screens.verifyFailedTitle')}
          </h2>

          <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-8 max-w-sm font-outfit">
            {errorText || t('auth:screens.verifyFailed')}
          </p>

          <div className="flex flex-col gap-3 w-full">
            <Button
              variant="solid"
              className="w-full py-6 text-sm font-semibold bg-zinc-900 hover:bg-zinc-800 text-white dark:bg-zinc-50 dark:hover:bg-zinc-100 dark:text-zinc-950 animate-scale-up"
              onClick={() => {
                setState('pending');
                setShowManualForm(true);
              }}
            >
              {t('auth:toast.linkSentTitle')}
            </Button>
            <Button
              variant="bordered"
              className="w-full py-6 text-sm font-semibold font-outfit"
              onClick={() => router.push('/login')}
            >
              {t('auth:actions.backToLogin')}
            </Button>
          </div>
        </div>
      )}

      {/* 5. PENDING STATE */}
      {state === 'pending' && (
        <div className="py-2 flex flex-col select-none">
          <div className="text-center mb-6">
            <div className="inline-flex w-12 h-12 rounded-full bg-zinc-100 dark:bg-zinc-900 text-zinc-600 dark:text-zinc-400 items-center justify-center mb-4 border border-zinc-200/20">
              <Mail size={22} />
            </div>
            <h2 className="text-2xl font-extrabold tracking-tight text-zinc-900 dark:text-zinc-50 font-outfit">
              {t('auth:title.verify')}
            </h2>
            <p className="text-zinc-500 dark:text-zinc-400 text-sm mt-2 max-w-xs mx-auto font-outfit">
              {t('auth:subtitle.verify')}
            </p>
          </div>

          {targetEmail && !showManualForm && (
            <div className="mb-6 p-4 rounded-2xl bg-zinc-50/50 dark:bg-zinc-900/30 border border-zinc-200/50 dark:border-zinc-800/40 flex flex-col items-center text-center gap-2 font-outfit animate-scale-up">
              <div className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border border-emerald-500/20">
                <CheckCircle2 size={12} />
                <span>{t('auth:labels.verificationLinkSent')}</span>
              </div>
              <p className="text-xs text-zinc-400 dark:text-zinc-500 mt-1 max-w-[280px]">
                {t('auth:screens.verifyPending').split(':')[0]}:
              </p>
              <span className="font-bold text-sm text-zinc-800 dark:text-zinc-200 bg-zinc-200/40 dark:bg-zinc-800/60 px-3 py-1 rounded-lg select-all">
                {targetEmail}
              </span>
              <p className="text-[11px] text-zinc-400 dark:text-zinc-500 mt-2 max-w-[300px] leading-relaxed">
                {t('auth:screens.verifyPendingInstructions')}
              </p>
            </div>
          )}

          <form onSubmit={handleSubmit(onResend)} className="space-y-4 font-outfit">
            {showManualForm && (
              <div className="flex flex-col gap-1.5 animate-scale-up">
                <label className="text-xs font-semibold text-zinc-500 dark:text-zinc-400 mb-0.5">
                  {t('auth:labels.email')}
                </label>
                <div className="relative">
                  <input
                    type="email"
                    placeholder="name@example.com"
                    {...register('email')}
                    disabled={resendLoading}
                    className={[
                      "w-full px-4 py-3 rounded-xl text-sm transition-all duration-200 bg-zinc-50/30 dark:bg-zinc-900/20 border outline-none",
                      errors.email
                        ? "border-red-500 focus:border-red-500 focus:ring-1 focus:ring-red-500"
                        : "border-zinc-200 dark:border-zinc-800 focus:border-zinc-900 dark:focus:border-zinc-100 focus:ring-1 focus:ring-zinc-950 dark:focus:ring-zinc-50"
                    ].join(' ')}
                  />
                  {errors.email && (
                    <span className="text-xs text-red-500 font-medium mt-1.5 flex items-center gap-1.5">
                      <ShieldAlert size={14} className="shrink-0" />
                      {errors.email.message}
                    </span>
                  )}
                </div>
              </div>
            )}

            <Button
              type="submit"
              className="w-full mt-2 py-6 text-sm font-semibold rounded-xl bg-linear-to-r from-zinc-950 to-zinc-900 dark:from-white dark:to-zinc-100 text-white dark:text-zinc-950 hover:opacity-90 active:scale-[0.98] transition-all duration-200"
              isLoading={resendLoading}
              disabled={(!isValid && showManualForm) || resendLoading}
            >
              {resendLoading ? t('auth:actions.sendingVerification') : t('auth:actions.resendVerification')}
            </Button>
          </form>

          {/* Action Footer */}
          <div className="text-center mt-6 flex flex-col gap-3 font-outfit">
            {targetEmail && !showManualForm && (
              <button
                type="button"
                onClick={() => setShowManualForm(true)}
                className="text-xs font-semibold text-zinc-500 dark:text-zinc-400 hover:text-zinc-950 dark:hover:text-zinc-50 hover:underline transition-colors inline-flex items-center justify-center gap-1.5 cursor-pointer"
              >
                {t('auth:actions.useDifferentEmail')}
              </button>
            )}

            {showManualForm && targetEmail && (
              <button
                type="button"
                onClick={() => setShowManualForm(false)}
                className="text-xs font-semibold text-zinc-500 dark:text-zinc-400 hover:text-zinc-950 dark:hover:text-zinc-50 hover:underline transition-colors inline-flex items-center justify-center gap-1.5 cursor-pointer"
              >
                <ChevronLeft size={14} />
                {t('auth:actions.backToSentInfo')}
              </button>
            )}

            <div className="text-xs text-zinc-500 dark:text-zinc-400">
              {t('auth:actions.haveAccount')}{' '}
              <Link
                href="/login"
                className="font-semibold text-zinc-950 dark:text-zinc-50 hover:underline"
              >
                {t('auth:actions.signInNow')}
              </Link>
            </div>
          </div>
        </div>
      )}
    </Card>
  );
}

export default function VerifyEmailPage() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center p-8 min-h-[400px]">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-zinc-950 dark:border-zinc-50" />
      </div>
    }>
      <VerifyEmailContent />
    </Suspense>
  );
}
