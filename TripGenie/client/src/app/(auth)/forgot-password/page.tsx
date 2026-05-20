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
import { toast, Typography } from '@heroui/react';
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
          <div className="w-12 h-12 rounded-full bg-success/10 text-success flex items-center justify-center mb-4">
            <MailCheck size={24} />
          </div>
          
          <Typography type="h2" className="text-2xl font-bold tracking-tight text-foreground mb-2 font-outfit">
            {t('auth:screens.checkEmail')}
          </Typography>
          
          <Typography type="body-sm" className="text-muted leading-relaxed mb-6 font-outfit">
            {t('auth:toast.recoverySentDesc', { email: submittedEmail })}
          </Typography>

          <Link href="/login" className="w-full">
            <Button variant="solid" className="w-full">
              {t('auth:actions.backToLogin')}
            </Button>
          </Link>
          
          <button
            onClick={() => setSuccessText(null)}
            className="text-xs font-semibold text-muted hover:text-foreground transition-colors mt-5 hover:underline cursor-pointer"
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
        className="inline-flex items-center gap-1.5 text-xs font-semibold text-muted hover:text-foreground transition-colors mb-6 group w-fit select-none"
      >
        <ArrowLeft size={14} className="transition-transform group-hover:-translate-x-0.5" />
        {t('auth:actions.backToLogin')}
      </Link>

      <div className="mb-6 font-outfit">
        <Typography type="h2" className="text-2xl font-bold tracking-tight text-foreground">
          {t('auth:title.forgot')}
        </Typography>
        <Typography type="body-sm" className="text-muted mt-1">
          {t('auth:subtitle.forgot')}
        </Typography>
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
