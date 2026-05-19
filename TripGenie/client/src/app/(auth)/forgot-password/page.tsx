"use client";

import React, { useState } from 'react';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { forgotPasswordSchema } from '../../../features/auth/validators/auth.validator';
import { authApi } from '../../../features/auth/services/auth.service';
import { FormInput } from '../../../components/forms/form-input';
import { Button } from '../../../components/ui/button';
import { Card } from '../../../components/ui/card';
import { z } from 'zod';
import Link from 'next/link';
import { MailCheck, ArrowLeft } from 'lucide-react';
import { normalizeError } from '../../../services/axios-client';
import { toast } from '@heroui/react';
import { useTranslation } from 'react-i18next';

type ForgotPasswordFormValues = z.infer<typeof forgotPasswordSchema>;

export default function ForgotPasswordPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [successText, setSuccessText] = useState<string | null>(null);
  const [submittedEmail, setSubmittedEmail] = useState('');
  const { t } = useTranslation(['auth', 'common']);

  const methods = useForm<ForgotPasswordFormValues>({
    resolver: zodResolver(forgotPasswordSchema),
    defaultValues: {
      email: '',
    },
    mode: 'onChange',
  });

  const { handleSubmit, formState: { isValid } } = methods;

  const onSubmit = async (data: ForgotPasswordFormValues) => {
    setIsLoading(true);
    setSuccessText(null);
    setSubmittedEmail(data.email);

    try {
      const response = await authApi.forgotPassword(data);
      setSuccessText(response.message || t('auth:screens.recoverySent'));
      toast.success(t('auth:toast.recoverySentTitle'), {
        description: t('auth:toast.recoverySentDesc', { email: data.email }),
      });
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      toast.danger(t('auth:toast.requestFailedTitle'), {
        description: parsedError.message || t('auth:toast.requestFailedDesc'),
      });
    } finally {
      setIsLoading(false);
    }
  };

  // Render Success confirmation screen
  if (successText) {
    return (
      <Card glow={true}>
        <div className="text-center py-4 flex flex-col items-center">
          <div className="w-12 h-12 rounded-full bg-emerald-50 dark:bg-emerald-950/30 text-emerald-500 flex items-center justify-center mb-4">
            <MailCheck size={24} />
          </div>
          
          <h2 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50 mb-2 font-outfit">
            {t('auth:screens.checkEmail')}
          </h2>
          
          <p className="text-zinc-500 dark:text-zinc-400 text-sm leading-relaxed mb-6 font-outfit">
            {t('auth:toast.recoverySentDesc', { email: submittedEmail })}
          </p>

          <Link href="/login" className="w-full">
            <Button variant="solid" className="w-full">
              {t('auth:actions.backToLogin')}
            </Button>
          </Link>
          
          <button
            onClick={() => setSuccessText(null)}
            className="text-xs font-semibold text-zinc-400 hover:text-zinc-600 dark:text-zinc-600 dark:hover:text-zinc-300 transition-colors mt-5 hover:underline cursor-pointer"
          >
            {t('auth:screens.didNotReceive')}
          </button>
        </div>
      </Card>
    );
  }

  return (
    <Card glow={true}>
      <Link
        href="/login"
        className="inline-flex items-center gap-1.5 text-xs font-semibold text-zinc-500 hover:text-zinc-950 dark:hover:text-zinc-50 transition-colors mb-6 group w-fit select-none"
      >
        <ArrowLeft size={14} className="transition-transform group-hover:-translate-x-0.5" />
        {t('auth:actions.backToLogin')}
      </Link>

      <div className="mb-6 font-outfit">
        <h2 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">
          {t('auth:title.forgot')}
        </h2>
        <p className="text-zinc-500 dark:text-zinc-400 text-sm mt-1">
          {t('auth:subtitle.forgot')}
        </p>
      </div>

      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <FormInput
            name="email"
            type="email"
            label={t('auth:labels.email')}
            placeholder={t('auth:placeholders.email')}
            disabled={isLoading}
            autoComplete="email"
          />

          <Button
            type="submit"
            className="w-full mt-2"
            isLoading={isLoading}
            disabled={!isValid || isLoading}
          >
            {isLoading ? t('auth:actions.sendingInstructions') : t('auth:actions.sendInstructions')}
          </Button>
        </form>
      </FormProvider>
    </Card>
  );
}
