"use client";

import React, { useState, useEffect } from 'react';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useRouter, useSearchParams } from 'next/navigation';
import { loginSchema } from '../../../features/auth/validators/auth.validator';
import { useAuth } from '../../../features/auth/hooks/use-auth';
import { FormInput } from '../../../components/forms/form-input';
import { FormCheckbox } from '../../../components/forms/form-checkbox';
import { Button } from '../../../components/ui/button';
import { Card } from '../../../components/ui/card';
import { z } from 'zod';
import Link from 'next/link';
import Script from 'next/script';
import { toast } from '@heroui/react';
import { Suspense } from 'react';
import { useTranslation } from 'react-i18next';

interface GoogleIdentityResponse {
  credential?: string;
}

interface CustomWindow extends Window {
  google?: {
    accounts?: {
      id?: {
        initialize: (options: { client_id: string; callback: (response: GoogleIdentityResponse) => void }) => void;
        renderButton: (parent: HTMLElement, options: { theme?: string; size?: string; width?: number; text?: string }) => void;
      };
    };
  };
  __googleIdentityListener?: (response: GoogleIdentityResponse) => void;
  __googleIdentityInitialized?: boolean;
}

type LoginFormValues = z.infer<typeof loginSchema>;

function LoginContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { login, loginWithGoogle, isLoading } = useAuth();
  const { t } = useTranslation(['auth', 'common']);
  
  // States for advanced security feedback
  const [cooldownSeconds, setCooldownSeconds] = useState<number>(0);
  
  // Callback URL for redirects
  const callbackUrl = searchParams.get('callbackUrl') || '/';
  const isSessionExpired = searchParams.get('session_expired') === 'true';
  const registered = searchParams.get('registered') === 'true';
  const resetSuccess = searchParams.get('reset_success') === 'true';

  const methods = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
      rememberMe: false,
    },
    mode: 'onChange',
  });

  const { handleSubmit, formState: { isValid } } = methods;

  // Trigger toasts on mount for query states
  useEffect(() => {
    if (isSessionExpired) {
      toast.warning(t('auth:toast.sessionExpiredTitle'), {
        description: t('auth:toast.sessionExpiredDesc'),
      });
    }
    if (registered) {
      toast.success(t('auth:toast.registerSuccessTitle'), {
        description: t('auth:toast.registerSuccessDesc'),
      });
    }
    if (resetSuccess) {
      toast.success(t('auth:toast.passwordResetSuccessTitle'), {
        description: t('auth:toast.passwordResetSuccessDesc'),
      });
    }
  }, [isSessionExpired, registered, resetSuccess, t]);

  // Rate Limiting Cooldown ticking timer
  useEffect(() => {
    if (cooldownSeconds <= 0) return;
    
    const interval = setInterval(() => {
      setCooldownSeconds((prev) => prev - 1);
    }, 1000);

    return () => clearInterval(interval);
  }, [cooldownSeconds]);

  const onSubmit = async (data: LoginFormValues) => {
    if (cooldownSeconds > 0) return;

    const result = await login(data);
    
    if (result.success) {
      if (result.isUnverified || result.nextStep === 'VERIFY_EMAIL') {
        toast.warning(t('auth:toast.verificationPendingTitle'), {
          description: t('auth:toast.verificationPendingDesc'),
        });
        router.push('/verify-email');
        return;
      }

      if (result.user) {
        toast.success(t('auth:toast.loginSuccessTitle'), {
          description: t('auth:toast.loginSuccessDesc'),
        });
        router.push(callbackUrl);
      }
    } else if (result.error) {
      const err = result.error;
      
      // Trigger Rate limiting lock
      if (err.code === 'RATE_LIMIT_EXCEEDED' && err.cooldownSeconds) {
        setCooldownSeconds(err.cooldownSeconds);
        toast.danger(t('auth:toast.rateLimitTitle'), {
          description: t('auth:toast.rateLimitDesc', { seconds: err.cooldownSeconds }),
        });
      } else {
        let toastDesc = err.message;
        if (err.remainingAttempts !== undefined) {
          if (err.remainingAttempts > 0) {
            toastDesc = t('auth:toast.authAlertRemainingAttempts', { attempts: err.remainingAttempts });
          } else {
            toastDesc = t('auth:toast.authAlertAccountLocked');
          }
        }
        toast.danger(t('auth:toast.authAlertTitle'), {
          description: toastDesc,
        });
      }

      // Propagate server-side validation errors to React Hook Form fields
      if (err.errors) {
        Object.entries(err.errors).forEach(([field, messages]) => {
          methods.setError(field as keyof LoginFormValues, {
            type: 'server',
            message: (messages as string[])[0],
          });
        });
      }
    }
  };

  const handleGoogleCredentialResponse = async (response: GoogleIdentityResponse) => {
    try {
      if (!response.credential) return;
      const result = await loginWithGoogle(response.credential);
      
      if (result.success) {
        if (result.isUnverified || result.nextStep === 'VERIFY_EMAIL') {
          toast.warning(t('auth:toast.verificationPendingTitle'), {
            description: t('auth:toast.verificationPendingDesc'),
          });
          router.push('/verify-email');
          return;
        }

        if (result.user) {
          toast.success(t('auth:toast.googleLoginSuccessTitle'), {
            description: t('auth:toast.googleLoginSuccessDesc'),
          });
          router.push(callbackUrl);
        }
      } else if (result.error) {
        toast.danger(t('auth:toast.googleLoginFailedTitle'), {
          description: result.error.message,
        });
      }
    } catch {
      toast.danger(t('auth:toast.googleLoginFailedTitle'), {
        description: t('auth:toast.googleLoginFailedDesc'),
      });
    }
  };

  const initializeGoogleSignIn = () => {
    const customWindow = typeof window !== 'undefined' ? (window as unknown as CustomWindow) : null;
    if (customWindow?.google?.accounts?.id) {
      // Set the dynamic listener to point to the current callback context
      customWindow.__googleIdentityListener = handleGoogleCredentialResponse;

      if (!customWindow.__googleIdentityInitialized) {
        customWindow.google.accounts.id.initialize({
          client_id: process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID || 'your_google_client_id_here',
          callback: (response: GoogleIdentityResponse) => {
            if (typeof customWindow.__googleIdentityListener === 'function') {
              customWindow.__googleIdentityListener(response);
            }
          },
        });
        customWindow.__googleIdentityInitialized = true;
      }

      const container = document.getElementById('google-signin-button');
      if (container) {
        customWindow.google.accounts.id.renderButton(
          container,
          { theme: 'outline', size: 'large', width: 350, text: 'continue_with' }
        );
      }
    }
  };

  // Sync Google script initialization
  useEffect(() => {
    initializeGoogleSignIn();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isLoading]);

  return (
    <>
      <Script
        src="https://accounts.google.com/gsi/client"
        strategy="lazyOnload"
        onLoad={initializeGoogleSignIn}
      />
      <Card className="shadow-2xl" glow={true}>
        <div className="text-center mb-6">
          <h2 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">
            {t('auth:title.login')}
          </h2>
          <p className="text-zinc-500 dark:text-zinc-400 text-sm mt-1">
            {t('auth:subtitle.login')}
          </p>
        </div>

        <FormProvider {...methods}>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <FormInput
              name="email"
              type="email"
              label={t('auth:labels.email')}
              placeholder={t('auth:placeholders.email')}
              disabled={isLoading || cooldownSeconds > 0}
              autoComplete="email"
            />

            <FormInput
              name="password"
              type="password"
              label={t('auth:labels.password')}
              placeholder={t('auth:placeholders.password')}
              disabled={isLoading || cooldownSeconds > 0}
              autoComplete="current-password"
            />

            <div className="flex items-center justify-between text-xs pt-1 select-none">
              <FormCheckbox name="rememberMe" disabled={isLoading || cooldownSeconds > 0}>
                {t('auth:labels.rememberMe')}
              </FormCheckbox>
              
              <Link
                href="/forgot-password"
                className="font-semibold text-zinc-950 dark:text-zinc-50 hover:underline shrink-0"
              >
                {t('auth:actions.forgot')}
              </Link>
            </div>

            <Button
              type="submit"
              className="w-full mt-2"
              isLoading={isLoading}
              disabled={!isValid || cooldownSeconds > 0 || isLoading}
            >
              {isLoading ? t('auth:actions.signingIn') : t('auth:actions.login')}
            </Button>
          </form>
        </FormProvider>

        {/* Visual Divider separator */}
        <div className="relative my-6 select-none">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-zinc-200 dark:border-zinc-800" />
          </div>
          <div className="relative flex justify-center text-xs uppercase font-bold tracking-wider">
            <span className="bg-white dark:bg-zinc-950 px-3 text-zinc-400 dark:text-zinc-600 text-[10px]">
              {t('auth:labels.orContinueWith')}
            </span>
          </div>
        </div>

        {/* Google OAuth Premium Overlay Button */}
        <div className="relative w-[350px] h-11 mx-auto select-none">
          {/* Beautiful Custom Premium Button */}
          <div className="absolute inset-0 flex items-center justify-center gap-3 w-full h-full bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 rounded-xl hover:bg-zinc-50 dark:hover:bg-zinc-850 active:scale-[0.98] transition-all duration-200 pointer-events-none shadow-sm">
            <svg className="w-5 h-5" viewBox="0 0 24 24">
              <path
                fill="#EA4335"
                d="M12 5.04c1.64 0 3.12.56 4.28 1.67l3.2-3.2C17.52 1.58 14.94 1 12 1 7.24 1 3.2 3.73 1.24 7.72l3.8 2.95C6 7.45 8.78 5.04 12 5.04z"
              />
              <path
                fill="#4285F4"
                d="M23.48 12.25c0-.82-.07-1.6-.2-2.35H12v4.45h6.44c-.28 1.47-1.1 2.71-2.35 3.55l3.65 2.83c2.13-1.97 3.74-4.87 3.74-8.48z"
              />
              <path
                fill="#FBBC05"
                d="M5.04 14.77C4.8 14.05 4.67 13.28 4.67 12.5s.13-1.55.37-2.27l-3.8-2.95C.44 8.73 0 10.56 0 12.5s.44 3.77 1.24 5.23l3.8-2.96z"
              />
              <path
                fill="#34A853"
                d="M12 23c3.24 0 5.97-1.07 7.96-2.92l-3.65-2.83c-1.2.8-2.73 1.28-4.31 1.28-3.22 0-6-2.41-6.96-5.63l-3.8 2.95C3.2 20.27 7.24 23 12 23z"
              />
            </svg>
            <span className="text-sm font-semibold text-zinc-700 dark:text-zinc-200">
              {t('auth:actions.googleSso')}
            </span>
          </div>

          {/* Google invisible GIS iframe trigger wrapper */}
          <div 
            id="google-signin-button" 
            className="absolute inset-0 opacity-0 cursor-pointer [&_iframe]:cursor-pointer [&_iframe]:w-full [&_iframe]:h-full"
          />
        </div>

        {/* Call to Register */}
        <div className="text-center text-xs text-zinc-500 dark:text-zinc-500 mt-6 font-outfit">
          {t('auth:actions.noAccount')}{' '}
          <Link
            href="/register"
            className="font-semibold text-zinc-950 dark:text-zinc-50 hover:underline"
          >
            {t('auth:actions.signUpNow')}
          </Link>
        </div>
      </Card>
    </>
  );
}

export default function LoginPage() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center p-8 min-h-[400px]">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-zinc-950 dark:border-zinc-50" />
      </div>
    }>
      <LoginContent />
    </Suspense>
  );
}
