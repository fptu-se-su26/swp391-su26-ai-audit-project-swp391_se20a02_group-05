"use client";

import React, { useState, useEffect } from 'react';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useSearchParams, useRouter } from 'next/navigation';
import { resetPasswordSchema } from '../../../lib/validators/auth.validator';
import { FormInput } from '../../../components/forms/form-input';
import { Button } from '../../../components/ui/button';
import { Card } from '../../../components/ui/card';
import { z } from 'zod';
import Link from 'next/link';
import { CheckCircle2, ShieldAlert, KeyRound } from 'lucide-react';
import { toast } from '@heroui/react';
import { useAuth } from '../../../hooks/use-auth';
import { Suspense } from 'react';

type ResetPasswordFormValues = z.infer<typeof resetPasswordSchema>;

function ResetPasswordContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token');
  const { resetPassword } = useAuth();

  const [isLoading, setIsLoading] = useState(false);
  const [successText, setSuccessText] = useState<string | null>(null);

  const methods = useForm<ResetPasswordFormValues>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: {
      password: '',
      confirmPassword: '',
    },
    mode: 'onChange',
  });

  const { handleSubmit, formState: { isValid } } = methods;

  useEffect(() => {
    // Validate existence of recovery token
    if (!token) {
      toast.danger("Security Token Missing", {
        description: "Security Token is missing or invalid. Please check your recovery email or request a new reset link.",
      });
    }
  }, [token]);

  const onSubmit = async (data: ResetPasswordFormValues) => {
    if (!token) return;

    setIsLoading(true);
    setSuccessText(null);

    const result = await resetPassword({
      token,
      password: data.password,
      confirmPassword: data.confirmPassword,
    });

    if (result.success) {
      setSuccessText('Your password has been reset successfully.');
      toast.success("Password Updated", {
        description: "Your credentials have been successfully updated. Logging you in...",
      });
      methods.reset();
      
      // Auto-navigate to dashboard after 2 seconds
      setTimeout(() => {
        router.push('/dashboard/user');
      }, 2000);
    } else {
      const error = result.error;
      toast.danger("Password Reset Failed", {
        description: error?.message || "An error occurred while resetting your password.",
      });
      if (error?.errors) {
        Object.entries(error.errors).forEach(([field, messages]) => {
          methods.setError(field as any, {
            type: 'server',
            message: (messages as string[])[0],
          });
        });
      }
    }
    setIsLoading(false);
  };

  // Render success screen
  if (successText) {
    return (
      <Card glow={true}>
        <div className="text-center py-4 flex flex-col items-center">
          <div className="w-12 h-12 rounded-full bg-emerald-50 dark:bg-emerald-950/30 text-emerald-500 flex items-center justify-center mb-4">
            <CheckCircle2 size={24} />
          </div>
          
          <h2 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50 mb-2">
            Password updated
          </h2>
          
          <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-6">
            Your credentials have been successfully updated. You have been automatically logged in and are being redirected to your traveler dashboard.
          </p>

          <Link href="/dashboard/user" className="w-full">
            <Button variant="solid" className="w-full">
              Go to Dashboard
            </Button>
          </Link>
        </div>
      </Card>
    );
  }

  return (
    <Card glow={true}>
      <div className="text-center mb-6">
        <div className="mx-auto w-10 h-10 rounded-xl bg-zinc-50 dark:bg-zinc-900 text-zinc-800 dark:text-zinc-200 flex items-center justify-center mb-3">
          <KeyRound size={20} />
        </div>
        <h2 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">
          Enter new password
        </h2>
        <p className="text-zinc-500 dark:text-zinc-400 text-sm mt-1">
          Create a secure, complex password you haven&apos;t used before.
        </p>
      </div>



      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <FormInput
            name="password"
            type="password"
            label="New password"
            placeholder="••••••••"
            disabled={isLoading || !token}
            autoComplete="new-password"
          />

          <FormInput
            name="confirmPassword"
            type="password"
            label="Confirm new password"
            placeholder="••••••••"
            disabled={isLoading || !token}
            autoComplete="new-password"
          />

          <Button
            type="submit"
            className="w-full mt-2"
            isLoading={isLoading}
            disabled={!isValid || isLoading || !token}
          >
            {isLoading ? 'Updating password...' : 'Update password'}
          </Button>
        </form>
      </FormProvider>
    </Card>
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center p-8 min-h-[400px]">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-zinc-950 dark:border-zinc-50" />
      </div>
    }>
      <ResetPasswordContent />
    </Suspense>
  );
}
