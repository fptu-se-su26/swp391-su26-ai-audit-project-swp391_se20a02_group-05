"use client";

import React, { useState, useEffect } from 'react';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useRouter } from 'next/navigation';
import { registerSchema } from '../../../lib/validators/auth.validator';
import { useAuth } from '../../../hooks/use-auth';
import { FormInput } from '../../../components/forms/form-input';
import { FormCheckbox } from '../../../components/forms/form-checkbox';
import { Button } from '../../../components/ui/button';
import { Card } from '../../../components/ui/card';
import { z } from 'zod';
import Link from 'next/link';
import { Check, ShieldAlert, Sparkles } from 'lucide-react';
import { toast } from '@heroui/react';
import Script from 'next/script';

import { useAuthStore } from '../../../store/use-auth-store';

type RegisterFormValues = z.infer<typeof registerSchema>;

export default function RegisterPage() {
  const router = useRouter();
  const { register: registerUser, isLoading, loginWithGoogle } = useAuth();
  const { setPendingVerificationEmail } = useAuthStore();

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

  const { handleSubmit, watch, formState: { isValid, errors } } = methods;

  // Watch the password field to calculate strength in real-time
  const watchedPassword = watch('password') || '';

  // States for password strength calculations
  const [strengthScore, setStrengthScore] = useState(0);
  const [strengthLabel, setStrengthLabel] = useState('Too Weak');
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
        setStrengthLabel('Too Weak');
        setStrengthColor('bg-red-500');
        break;
      case 1:
        setStrengthLabel('Weak');
        setStrengthColor('bg-red-500');
        break;
      case 2:
        setStrengthLabel('Fair');
        setStrengthColor('bg-amber-500');
        break;
      case 3:
        setStrengthLabel('Strong');
        setStrengthColor('bg-indigo-500');
        break;
      case 4:
        setStrengthLabel('Excellent');
        setStrengthColor('bg-emerald-500');
        break;
      default:
        break;
    }
  }, [watchedPassword]);

  const handleGoogleCredentialResponse = async (response: any) => {
    try {
      const result = await loginWithGoogle(response.credential);

      if (result.success) {
        if (result.isUnverified || result.nextStep === 'VERIFY_EMAIL') {
          toast.warning("Verification Pending", {
            description: "Your email has not been verified yet. Please verify to continue.",
          });
          router.push('/verify-email');
          return;
        }

        if (result.user) {
          toast.success("Welcome!", {
            description: "Google Sign-in successful.",
          });
          router.push('/dashboard');
        }
      } else if (result.error) {
        toast.danger("Google Sign-in Failed", {
          description: result.error.message,
        });
      }
    } catch (err: any) {
      toast.danger("Google Sign-in Failed", {
        description: 'An unexpected error occurred during Google Sign-in.',
      });
    }
  };

  const initializeGoogleSignIn = () => {
    if (typeof window !== 'undefined' && (window as any).google?.accounts?.id) {
      // Set the dynamic listener to point to the current callback context
      (window as any).__googleIdentityListener = handleGoogleCredentialResponse;

      if (!(window as any).__googleIdentityInitialized) {
        (window as any).google.accounts.id.initialize({
          client_id: process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID || 'your_google_client_id_here',
          callback: (response: any) => {
            if (typeof (window as any).__googleIdentityListener === 'function') {
              (window as any).__googleIdentityListener(response);
            }
          },
        });
        (window as any).__googleIdentityInitialized = true;
      }

      const container = document.getElementById('google-signin-button');
      if (container) {
        (window as any).google.accounts.id.renderButton(
          container,
          { theme: 'outline', size: 'large', width: 350, text: 'continue_with' }
        );
      }
    }
  };

  // Sync Google script initialization
  useEffect(() => {
    initializeGoogleSignIn();
  }, [isLoading]);

  const onSubmit = async (data: RegisterFormValues) => {
    const result = await registerUser(data);

    if (result.success) {
      // In-memory tracking of the email to verify
      setPendingVerificationEmail(data.email);

      if (result.uiAction === 'SHOW_WARNING_TOAST') {
        toast.warning("Verification Pending", {
          description: result.message || "An unverified registration already exists for this email. We have resent your verification link.",
        });
      } else {
        toast.success("Account Created", {
          description: result.message || "Please check your email inbox to verify your account.",
        });
      }
      methods.reset();

      // SNAPPY redirect directly to /verify-email - decoupled from query parameters
      router.push('/verify-email');
    } else if (result.error) {
      const err = result.error;

      if (err.code === 'AUTH_EMAIL_ALREADY_EXISTS') {
        toast.danger("Account Already Exists", {
          description: "This email is already registered and verified. Please sign in to continue.",
        });
        router.push('/login');
      } else {
        toast.danger("Registration Failed", {
          description: err.message || "An unexpected error occurred during account creation.",
        });
      }

      // Propagate server-side validation errors to React Hook Form fields
      if (err.errors) {
        Object.entries(err.errors).forEach(([field, messages]) => {
          methods.setError(field as any, {
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
        <h2 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">
          Create an account
        </h2>
        <p className="text-zinc-500 dark:text-zinc-400 text-sm mt-1">
          Join TripGenie AI to unleash smart travels
        </p>
      </div>

      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <FormInput
            name="fullName"
            type="text"
            label="Full name"
            placeholder="John Doe"
            disabled={isLoading}
            autoComplete="name"
          />

          <FormInput
            name="email"
            type="email"
            label="Email address"
            placeholder="john.doe@example.com"
            disabled={isLoading}
            autoComplete="email"
          />

          <FormInput
            name="password"
            type="password"
            label="Password"
            placeholder="••••••••"
            disabled={isLoading}
            autoComplete="new-password"
          />

          {/* Real-time Password Strength Progress Meter */}
          {watchedPassword.length > 0 && (
            <div className="space-y-1.5 px-1 py-0.5 select-none">
              <div className="flex justify-between items-center text-xs">
                <span className="text-zinc-500 dark:text-zinc-500 font-medium">Password strength:</span>
                <span className={[
                  "font-bold transition-colors",
                  strengthScore <= 1 ? "text-red-500" : "",
                  strengthScore === 2 ? "text-amber-500" : "",
                  strengthScore === 3 ? "text-indigo-500" : "",
                  strengthScore === 4 ? "text-emerald-500" : "",
                ].join(' ')}>
                  {strengthLabel}
                </span>
              </div>

              <div className="flex gap-1 h-1.5 w-full bg-zinc-100 dark:bg-zinc-900 rounded-full overflow-hidden">
                <div
                  className={[
                    "h-full rounded-full transition-all duration-300",
                    strengthColor,
                  ].join(' ')}
                  style={{ width: `${(strengthScore / 4) * 100}%` }}
                />
              </div>

              {/* Password checks guidance list */}
              <div className="grid grid-cols-2 gap-x-3 gap-y-0.5 text-[10px] text-zinc-400 dark:text-zinc-600 mt-1">
                <span className={watchedPassword.length >= 8 ? 'text-zinc-800 dark:text-zinc-300 font-medium' : ''}>
                  • Min 8 characters
                </span>
                <span className={/[A-Z]/.test(watchedPassword) ? 'text-zinc-800 dark:text-zinc-300 font-medium' : ''}>
                  • 1 uppercase letter
                </span>
                <span className={/[a-z]/.test(watchedPassword) ? 'text-zinc-800 dark:text-zinc-300 font-medium' : ''}>
                  • 1 lowercase letter
                </span>
                <span className={/[0-9]/.test(watchedPassword) ? 'text-zinc-800 dark:text-zinc-300 font-medium' : ''}>
                  • 1 number & special
                </span>
              </div>
            </div>
          )}

          <FormInput
            name="confirmPassword"
            type="password"
            label="Confirm password"
            placeholder="••••••••"
            disabled={isLoading}
            autoComplete="new-password"
          />

          <div className="pt-1 select-none">
            <FormCheckbox name="agreeTerms" disabled={isLoading}>
              <span className="flex items-center gap-1 whitespace-nowrap">
                I agree to the{' '}
                <a href="#" className="font-semibold text-zinc-950 dark:text-zinc-50 hover:underline">
                  Terms of Service
                </a>
                {' '}and{' '}
                <a href="#" className="font-semibold text-zinc-950 dark:text-zinc-50 hover:underline">
                  Privacy Policy.
                </a>
              </span>
            </FormCheckbox>
          </div>

          <Button
            type="submit"
            className="w-full mt-2"
            isLoading={isLoading}
            disabled={!isValid || isLoading}
          >
            {isLoading ? 'Creating Account...' : 'Sign Up'}
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
            Or continue with
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
            Continue with Google
          </span>
        </div>

        {/* Google invisible GIS iframe trigger wrapper */}
        <div
          id="google-signin-button"
          className="absolute inset-0 opacity-0 cursor-pointer [&_iframe]:cursor-pointer [&_iframe]:w-full [&_iframe]:h-full"
        />
      </div>

      {/* Call to Login */}
      <div className="text-center text-xs text-zinc-500 dark:text-zinc-500 mt-6 select-none">
        Already have an account?{' '}
        <Link
          href="/login"
          className="font-semibold text-zinc-950 dark:text-zinc-50 hover:underline"
        >
          Sign in
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
