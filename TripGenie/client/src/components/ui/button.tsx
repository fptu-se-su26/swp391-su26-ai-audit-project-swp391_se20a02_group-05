"use client";

import React from 'react';
import { Button as HeroButton, ButtonProps as HeroButtonProps } from '@heroui/react';

interface ButtonProps extends Omit<HeroButtonProps, 'variant' | 'children'> {
  variant?: 'solid' | 'bordered' | 'light' | 'flat' | 'ghost' | 'outline' | 'primary' | 'secondary' | 'danger';
  isLoading?: boolean;
  disabled?: boolean;
  children?: React.ReactNode;
}

export const Button: React.FC<ButtonProps> = ({
  children,
  variant = 'solid',
  className = '',
  isLoading = false,
  disabled = false,
  isDisabled = false,
  ...props
}) => {
  // Map our premium styles
  const baseClasses = "font-semibold transition-all duration-200 active:scale-[0.98] select-none flex items-center justify-center rounded-xl";
  
  let variantClasses = "";
  
  if (variant === 'solid') {
    variantClasses = [
      "bg-zinc-950 hover:bg-zinc-900 text-white dark:bg-zinc-50 dark:hover:bg-zinc-100 dark:text-zinc-950",
      "shadow-[0_4px_12px_rgba(0,0,0,0.08)] dark:shadow-[0_4px_20px_rgba(255,255,255,0.06)]",
      "border border-zinc-900 dark:border-zinc-100",
    ].join(' ');
  } else if (variant === 'bordered' || variant === 'outline') {
    variantClasses = [
      "bg-transparent border border-zinc-200 dark:border-zinc-800 text-zinc-800 dark:text-zinc-200",
      "hover:bg-zinc-50 dark:hover:bg-zinc-900/60 hover:border-zinc-300 dark:hover:border-zinc-700/80",
    ].join(' ');
  } else if (variant === 'flat' || variant === 'secondary') {
    variantClasses = [
      "bg-zinc-100 dark:bg-zinc-900 text-zinc-800 dark:text-zinc-200",
      "hover:bg-zinc-200/80 dark:hover:bg-zinc-800/80",
    ].join(' ');
  } else if (variant === 'danger') {
    variantClasses = [
      "bg-red-600 hover:bg-red-500 text-white dark:bg-red-500 dark:hover:bg-red-400 dark:text-zinc-950",
      "shadow-[0_4px_12px_rgba(220,38,38,0.15)]",
      "border border-red-600 dark:border-red-500",
    ].join(' ');
  }

  let mappedVariant: 'ghost' | 'outline' | 'primary' | 'secondary' | 'danger' | 'danger-soft' | 'tertiary' = 'primary';
  if (variant === 'solid') {
    mappedVariant = 'primary';
  } else if (variant === 'bordered' || variant === 'outline') {
    mappedVariant = 'outline';
  } else if (variant === 'flat' || variant === 'secondary') {
    mappedVariant = 'secondary';
  } else if (variant === 'light') {
    mappedVariant = 'tertiary';
  } else if (variant === 'ghost') {
    mappedVariant = 'ghost';
  } else if (variant === 'danger') {
    mappedVariant = 'danger';
  }

  return (
    <HeroButton
      {...props}
      variant={mappedVariant}
      className={`${baseClasses} ${variantClasses} ${className}`}
      isDisabled={disabled || isLoading || isDisabled}
    >
      {isLoading && (
        <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-current" fill="none" viewBox="0 0 24 24">
          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
        </svg>
      )}
      {children}
    </HeroButton>
  );
};
export default Button;
