"use client";

import React, { useState } from 'react';
import { useFormContext, Controller } from 'react-hook-form';
import { TextField, Input, Label, FieldError } from '@heroui/react';
import { Eye, EyeOff } from 'lucide-react';

interface FormInputProps {
  name: string;
  type?: string;
  label?: string;
  placeholder?: string;
  disabled?: boolean;
  autoComplete?: string;
}

export const FormInput: React.FC<FormInputProps> = ({
  name,
  type = 'text',
  label,
  placeholder,
  disabled = false,
  autoComplete,
}) => {
  const { control, formState: { errors } } = useFormContext();
  const error = errors[name];
  const errorMessage = error?.message as string | undefined;

  const [showPassword, setShowPassword] = useState(false);

  const togglePasswordVisibility = () => {
    setShowPassword((prev) => !prev);
  };

  const isPassword = type === 'password';
  const currentType = isPassword && showPassword ? 'text' : type;

  return (
    <div className="flex flex-col gap-1.5 w-full text-left">
      <Controller
        name={name}
        control={control}
        render={({ field: { value, onChange, onBlur } }) => (
          <TextField
            isInvalid={!!errorMessage}
            className="flex flex-col gap-1.5 w-full"
          >
            {label && (
              <Label className="text-zinc-700 dark:text-zinc-300 text-xs font-semibold select-none">
                {label}
              </Label>
            )}
            
            <div className="relative flex items-center w-full">
              <Input
                type={currentType}
                value={value || ''}
                onChange={onChange}
                onBlur={onBlur}
                placeholder={placeholder}
                disabled={disabled}
                autoComplete={autoComplete}
                className={[
                  "w-full px-3.5 py-2.5 rounded-xl border transition-all text-xs outline-none bg-white dark:bg-zinc-950 text-zinc-900 dark:text-zinc-50 placeholder-zinc-400 dark:placeholder-zinc-600 font-medium",
                  errorMessage
                    ? "border-red-500 focus:border-red-500 focus:ring-1 focus:ring-red-500/50"
                    : "border-zinc-200 dark:border-zinc-800 focus:border-zinc-950 dark:focus:border-zinc-50 focus:ring-1 focus:ring-zinc-950/20 dark:focus:ring-white/20",
                  disabled ? "opacity-50 cursor-not-allowed bg-zinc-50 dark:bg-zinc-900/30" : "",
                ].join(' ')}
              />
              
              {/* Password visibility toggle */}
              {isPassword && (
                <button
                  type="button"
                  onClick={togglePasswordVisibility}
                  className="absolute right-3.5 text-zinc-400 hover:text-zinc-600 dark:hover:text-zinc-300 transition-colors cursor-pointer select-none"
                  tabIndex={-1}
                >
                  {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              )}
            </div>

            {errorMessage && (
              <FieldError className="text-red-500 text-[10px] pl-1 font-medium animate-fade-in block">
                {errorMessage}
              </FieldError>
            )}
          </TextField>
        )}
      />
    </div>
  );
};
export default FormInput;
