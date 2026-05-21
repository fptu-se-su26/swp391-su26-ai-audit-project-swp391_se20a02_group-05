"use client";

import React, { useState, useEffect } from 'react';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useSearchParams, useRouter } from 'next/navigation';
import { resetPasswordSchema } from '../../../features/auth/validators/auth.validator';
import { FormInput } from '../../../components/forms/form-input';
import { Button } from '../../../components/ui/button';
import { Card } from '../../../components/ui/card';
import { z } from 'zod';
import Link from 'next/link';
import { CheckCircle2, KeyRound } from 'lucide-react';
import { toast, Typography, Spinner } from '@heroui/react';
import { useAuth } from '../../../features/auth/hooks/use-auth';
import { Suspense } from 'react';
import { useTranslation } from 'react-i18next';

type ResetPasswordFormValues = z.infer<typeof resetPasswordSchema>;

function ResetPasswordContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token');
  const { resetPassword } = useAuth();
  const { t } = useTranslation(['auth', 'common']);

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
      toast.danger(t('auth:toast.tokenMissingTitle'), {
        description: t('auth:toast.tokenMissingDesc'),
      });
    }
  }, [token, t]);

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
      setSuccessText(t('auth:screens.resetSuccessDesc'));
      toast.success(t('auth:toast.passwordUpdatedSuccessTitle'), {
        description: t('auth:toast.passwordUpdatedSuccessDesc'),
      });
      methods.reset();
      
      // Auto-navigate to dashboard after 2 seconds
      setTimeout(() => {
        router.push('/user');
      }, 2000);
    } else {
      const error = result.error;
      toast.danger(t('auth:toast.passwordResetFailedTitle'), {
        description: error?.message || t('auth:toast.passwordResetFailedDesc'),
      });
      if (error?.errors) {
        Object.entries(error.errors).forEach(([field, messages]) => {
          methods.setError(field as keyof ResetPasswordFormValues, {
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
          <div className="w-12 h-12 rounded-full bg-success/10 text-success flex items-center justify-center mb-4">
            <CheckCircle2 size={24} />
          </div>
          
          <Typography type="h2" className="text-2xl font-bold tracking-tight text-foreground mb-2 font-outfit">
            {t('auth:screens.resetSuccessTitle')}
          </Typography>
          
          <Typography type="body-sm" className="text-muted leading-relaxed mb-6 font-outfit">
            {t('auth:screens.resetSuccessDesc')}
          </Typography>

          <Link href="/user" className="w-full">
            <Button variant="solid" className="w-full">
              {t('auth:actions.goToDashboard')}
            </Button>
          </Link>
        </div>
      </Card>
    );
  }

  return (
    <Card glow={true}>
      <div className="text-center mb-6">
        <div className="mx-auto w-10 h-10 rounded-xl bg-surface-secondary text-foreground flex items-center justify-center mb-3">
          <KeyRound size={20} />
        </div>
        <Typography type="h2" className="text-2xl font-bold tracking-tight text-foreground font-outfit">
          {t('auth:title.reset')}
        </Typography>
        <Typography type="body-sm" className="text-muted mt-1 font-outfit">
          {t('auth:subtitle.reset')}
        </Typography>
      </div>

      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <FormInput
            name="password"
            type="password"
            label={t('auth:labels.password')}
            placeholder="••••••••"
            disabled={isLoading || !token}
            autoComplete="new-password"
          />

          <FormInput
            name="confirmPassword"
            type="password"
            label={t('auth:labels.confirmPassword')}
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
            {isLoading ? t('auth:actions.updatingPassword') : t('auth:actions.updatePassword')}
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
        <Spinner size="md" color="accent" />
      </div>
    }>
      <ResetPasswordContent />
    </Suspense>
  );
}
