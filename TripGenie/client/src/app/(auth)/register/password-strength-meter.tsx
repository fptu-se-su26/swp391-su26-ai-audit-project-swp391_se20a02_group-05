"use client";

import React from 'react';
import { useWatch, Control, FieldValues, Path } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Typography } from '@heroui/react';

interface PasswordStrengthMeterProps<TFieldValues extends FieldValues = FieldValues> {
  control: Control<TFieldValues>;
}

export const PasswordStrengthMeter = <TFieldValues extends FieldValues = FieldValues>({
  control,
}: PasswordStrengthMeterProps<TFieldValues>) => {
  const { t } = useTranslation(['auth']);
  
  // Isolated watch subscription to avoid re-rendering parent form component
  const watchedPassword = (useWatch({
    control,
    name: 'password' as Path<TFieldValues>,
  }) as string | undefined) || '';

  if (watchedPassword.length === 0) {
    return null;
  }

  // 100% Pure derived calculations for password strength score
  let score = 0;
  if (watchedPassword.length >= 8) score += 1;
  if (/[A-Z]/.test(watchedPassword)) score += 1;
  if (/[a-z]/.test(watchedPassword)) score += 1;
  if (/[0-9]/.test(watchedPassword)) score += 1;
  if (/[@$!%*?&]/.test(watchedPassword)) score += 1;

  const strengthScore = Math.min(4, Math.floor(score * 0.8));

  let strengthLabel = '';
  let strengthColor = 'bg-zinc-200';

  switch (strengthScore) {
    case 0:
      strengthLabel = t('auth:passwordStrength.tooWeak', { defaultValue: 'Too Weak' });
      strengthColor = 'bg-danger';
      break;
    case 1:
      strengthLabel = t('auth:passwordStrength.weak', { defaultValue: 'Weak' });
      strengthColor = 'bg-danger';
      break;
    case 2:
      strengthLabel = t('auth:passwordStrength.fair', { defaultValue: 'Fair' });
      strengthColor = 'bg-warning';
      break;
    case 3:
      strengthLabel = t('auth:passwordStrength.strong', { defaultValue: 'Strong' });
      strengthColor = 'bg-accent';
      break;
    case 4:
      strengthLabel = t('auth:passwordStrength.excellent', { defaultValue: 'Excellent' });
      strengthColor = 'bg-success';
      break;
  }

  return (
    <div className="space-y-1.5 px-1 py-0.5 select-none">
      <div className="flex justify-between items-center text-xs">
        <Typography type="body-xs" className="text-muted font-medium">
          {t('auth:passwordStrength.label', { defaultValue: 'Password Strength' })}
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
          {t('auth:passwordStrength.minChars', { defaultValue: 'At least 8 characters' })}
        </span>
        <span className={/[A-Z]/.test(watchedPassword) ? 'text-foreground font-medium' : ''}>
          {t('auth:passwordStrength.uppercase', { defaultValue: 'One uppercase letter' })}
        </span>
        <span className={/[a-z]/.test(watchedPassword) ? 'text-foreground font-medium' : ''}>
          {t('auth:passwordStrength.lowercase', { defaultValue: 'One lowercase letter' })}
        </span>
        <span className={/[0-9]/.test(watchedPassword) ? 'text-foreground font-medium' : ''}>
          {t('auth:passwordStrength.numberSpecial', { defaultValue: 'One number or symbol' })}
        </span>
      </div>
    </div>
  );
};

export default PasswordStrengthMeter;
