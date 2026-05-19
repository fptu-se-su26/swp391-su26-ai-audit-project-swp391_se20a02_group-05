"use client";

import React, { useState, useEffect } from 'react';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useRouter } from 'next/navigation';
import { registerSchema } from '../../../features/auth/validators/auth.validator';
import { useAuth } from '../../../features/auth/hooks/use-auth';
import { FormInput } from '../../../components/forms/form-input';
import { FormCheckbox } from '../../../components/forms/form-checkbox';
import { Button } from '../../../components/ui/button';
import { Card } from '../../../components/ui/card';
import { z } from 'zod';
import Link from 'next/link';
import { toast, Typography } from '@heroui/react';
import Script from 'next/script';
import { useTranslation } from 'react-i18next';

import { useAuthStore } from '../../../features/auth/store/use-auth-store';

interface GoogleIdentityResponse {
  credential?: string;
  select_by?: string;
}

interface CustomWindow extends Window {
  google?: {
    accounts: {
      id: {
        initialize: (config: {
          client_id: string;
          callback: (response: GoogleIdentityResponse) => void;
        }) => void;
        renderButton: (
          parent: HTMLElement,
          options: { theme: string; size: string; width: number; text: string }
        ) => void;
      };
    };
  };
  __googleIdentityListener?: (response: GoogleIdentityResponse) => void;
  __googleIdentityInitialized?: boolean;
}

type RegisterFormValues = z.infer<typeof registerSchema>;

export default function RegisterPage() {
  const router = useRouter();
  const { register: registerUser, isLoading, loginWithGoogle } = useAuth();
  const { setPendingVerificationEmail } = useAuthStore();
  const { t } = useTranslation(['auth', 'common']);

  const methods = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      fullName: '',
      email: '',
      password: '',
      confirmPassword: '',
      agreeTerms: false,
    },
    mode: 'onChange',
  });

  const { handleSubmit, watch, formState: { isValid } } = methods;

  // Watch the password field to calculate strength in real-time
  const watchedPassword = watch('password') || '';

  // States for password strength calculations
  const [strengthScore, setStrengthScore] = useState(0);
  const [strengthLabel, setStrengthLabel] = useState(t('auth:passwordStrength.tooWeak'));
  const [strengthColor, setStrengthColor] = useState('bg-zinc-200');

  useEffect(() => {
    let score = 0;
    if (watchedPassword.length >= 8) score += 1;
    if (/[A-Z]/.test(watchedPassword)) score += 1;
    if (/[a-z]/.test(watchedPassword)) score += 1;
    if (/[0-9]/.test(watchedPassword)) score += 1;
    if (/[@$!%*?&]/.test(watchedPassword)) score += 1;

    // Standardize score to max of 4 steps
    const finalScore = watchedPassword.length === 0 ? 0 : Math.min(4, Math.floor(score * 0.8));
    setStrengthScore(finalScore);

    switch (finalScore) {
      case 0:
        setStrengthLabel(t('auth:passwordStrength.tooWeak'));
        setStrengthColor('bg-danger');
        break;
      case 1:
        setStrengthLabel(t('auth:passwordStrength.weak'));
        setStrengthColor('bg-danger');
        break;
      case 2:
        setStrengthLabel(t('auth:passwordStrength.fair'));
        setStrengthColor('bg-warning');
        break;
      case 3:
        setStrengthLabel(t('auth:passwordStrength.strong'));
        setStrengthColor('bg-accent');
        break;
      case 4:
        setStrengthLabel(t('auth:passwordStrength.excellent'));
        setStrengthColor('bg-success');
        break;
      default:
        break;
    }
  }, [watchedPassword, t]);

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
          toast.success(t('auth:toast.googleLoginWelcome'), {
            description: t('auth:toast.googleLoginSuccessDesc'),
          });
          router.push('/');
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

  const onSubmit = async (data: RegisterFormValues) => {
    const result = await registerUser(data);

    if (result.success) {
      // In-memory tracking of the email to verify
      setPendingVerificationEmail(data.email);

      if (result.uiAction === 'SHOW_WARNING_TOAST') {
        toast.warning(t('auth:toast.verificationPendingTitle'), {
          description: result.message || t('auth:toast.verificationPendingDesc'),
        });
      } else {
        toast.success(t('auth:toast.registerSuccessTitle'), {
          description: result.message || t('auth:toast.registerSuccessDesc'),
        });
      }
      methods.reset();

      // SNAPPY redirect directly to /verify-email - decoupled from query parameters
      router.push('/verify-email');
    } else if (result.error) {
      const err = result.error;

      if (err.code === 'AUTH_EMAIL_ALREADY_EXISTS') {
        toast.danger(t('auth:toast.accountAlreadyExistsTitle'), {
          description: t('auth:toast.accountAlreadyExistsDesc'),
        });
        router.push('/login');
      } else {
        toast.danger(t('auth:toast.registrationFailedTitle'), {
          description: err.message || t('auth:toast.registrationFailedDesc'),
        });
      }

      // Propagate server-side validation errors to React Hook Form fields
      if (err.errors) {
        Object.entries(err.errors).forEach(([field, messages]) => {
          methods.setError(field as keyof RegisterFormValues, {
            type: 'server',
            message: (messages as string[])[0],
          });
        });
      }
    }
  };

  return (
    <Card className="shadow-2xl" glow={true}>
      <div className="text-center mb-6">
        <Typography type="h2" className="text-2xl font-bold tracking-tight text-foreground font-outfit">
          {t('auth:title.register')}
        </Typography>
        <Typography type="body-sm" className="text-muted mt-1">
          {t('auth:subtitle.register')}
        </Typography>
      </div>

      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <FormInput
            name="fullName"
            type="text"
            label={t('auth:labels.fullName')}
            placeholder={t('auth:placeholders.fullName')}
            disabled={isLoading}
            autoComplete="name"
          />

          <FormInput
            name="email"
            type="email"
            label={t('auth:labels.email')}
            placeholder={t('auth:placeholders.email')}
            disabled={isLoading}
            autoComplete="email"
          />

          <FormInput
            name="password"
            type="password"
            label={t('auth:labels.password')}
            placeholder={t('auth:placeholders.password')}
            disabled={isLoading}
            autoComplete="new-password"
          />

          {/* Real-time Password Strength Progress Meter */}
          {watchedPassword.length > 0 && (
            <div className="space-y-1.5 px-1 py-0.5 select-none">
              <div className="flex justify-between items-center text-xs">
                <Typography type="body-xs" className="text-muted font-medium">
                  {t('auth:passwordStrength.label')}
                </Typography>
                <span className={[
                  "font-bold transition-colors",
                  strengthScore <= 1 ? "text-danger" : "",
                  strengthScore === 2 ? "text-warning" : "",
                  strengthScore === 3 ? "text-accent" : "",
                  strengthScore === 4 ? "text-success" : "",
                ].join(' ')}>
                  {strengthLabel}
                </span>
              </div>

              <div className="flex gap-1 h-1.5 w-full bg-surface-secondary rounded-full overflow-hidden">
                <div
                  className={[
                    "h-full rounded-full transition-all duration-300",
                    strengthColor,
                  ].join(' ')}
                  style={{ width: `${(strengthScore / 4) * 100}%` }}
                />
              </div>

              {/* Password checks guidance list */}
              <div className="grid grid-cols-2 gap-x-3 gap-y-0.5 text-[10px] text-muted mt-1">
                <span className={watchedPassword.length >= 8 ? 'text-foreground font-medium' : ''}>
                  {t('auth:passwordStrength.minChars')}
                </span>
                <span className={/[A-Z]/.test(watchedPassword) ? 'text-foreground font-medium' : ''}>
                  {t('auth:passwordStrength.uppercase')}
                </span>
                <span className={/[a-z]/.test(watchedPassword) ? 'text-foreground font-medium' : ''}>
                  {t('auth:passwordStrength.lowercase')}
                </span>
                <span className={/[0-9]/.test(watchedPassword) ? 'text-foreground font-medium' : ''}>
                  {t('auth:passwordStrength.numberSpecial')}
                </span>
              </div>
            </div>
          )}

          <FormInput
            name="confirmPassword"
            type="password"
            label={t('auth:labels.confirmPassword')}
            placeholder={t('auth:placeholders.confirmPassword')}
            disabled={isLoading}
            autoComplete="new-password"
          />

          <div className="pt-1 select-none font-outfit">
            <FormCheckbox name="agreeTerms" disabled={isLoading}>
              <span className="flex items-center gap-1 whitespace-nowrap">
                {t('auth:labels.acceptTerms')}
              </span>
            </FormCheckbox>
          </div>

          <Button
            type="submit"
            className="w-full mt-2"
            isLoading={isLoading}
            disabled={!isValid || isLoading}
          >
            {isLoading ? t('auth:actions.creatingAccount') : t('auth:actions.register')}
          </Button>
        </form>
      </FormProvider>
      
      {/* Visual Divider separator */}
      <div className="relative my-6 select-none">
        <div className="absolute inset-0 flex items-center">
          <div className="w-full border-t border-separator" />
        </div>
        <div className="relative flex justify-center text-xs uppercase font-bold tracking-wider">
          <span className="bg-background px-3 text-muted text-[10px]">
            {t('auth:labels.orContinueWith')}
          </span>
        </div>
      </div>

      {/* Google OAuth Premium Overlay Button */}
      <div className="relative w-[350px] h-11 mx-auto select-none">
        {/* Beautiful Custom Premium Button */}
        <div className="absolute inset-0 flex items-center justify-center gap-3 w-full h-full bg-surface border border-border rounded-xl hover:bg-surface-secondary active:scale-[0.98] transition-all duration-200 pointer-events-none shadow-sm">
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
          <span className="text-sm font-semibold text-foreground">
            {t('auth:actions.googleSso')}
          </span>
        </div>

        {/* Google invisible GIS iframe trigger wrapper */}
        <div
          id="google-signin-button"
          className="absolute inset-0 opacity-0 cursor-pointer [&_iframe]:cursor-pointer [&_iframe]:w-full [&_iframe]:h-full"
        />
      </div>

      {/* Call to Login */}
      <div className="text-center text-xs text-muted mt-6 select-none font-outfit">
        {t('auth:actions.haveAccount')}{' '}
        <Link
          href="/login"
          className="font-semibold text-foreground hover:underline"
        >
          {t('auth:actions.signInNow')}
        </Link>
      </div>

      <Script
        src="https://accounts.google.com/gsi/client"
        strategy="lazyOnload"
        onLoad={initializeGoogleSignIn}
      />
    </Card>
  );
}
