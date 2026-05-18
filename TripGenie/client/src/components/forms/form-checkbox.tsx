"use client";

import React from 'react';
import { useFormContext, Controller } from 'react-hook-form';
import { Checkbox } from '@heroui/react';

interface FormCheckboxProps {
  name: string;
  children: React.ReactNode;
  disabled?: boolean;
}

export const FormCheckbox: React.FC<FormCheckboxProps> = ({
  name,
  children,
  disabled = false,
}) => {
  const { control, formState: { errors } } = useFormContext();
  const error = errors[name];
  const errorMessage = error?.message as string | undefined;

  return (
    <div className="flex flex-col gap-1 w-full">
      <Controller
        name={name}
        control={control}
        render={({ field: { value, onChange } }) => (
          <Checkbox
            isDisabled={disabled}
            isSelected={!!value}
            onChange={onChange}
            className="flex items-center gap-2.5 group cursor-pointer select-none text-zinc-600 dark:text-zinc-400 text-xs font-normal"
          >
            <Checkbox.Control className="w-4 h-4 rounded border border-zinc-300 dark:border-zinc-700 flex items-center justify-center bg-white dark:bg-zinc-950 group-data-[selected=true]:bg-zinc-950 dark:group-data-[selected=true]:bg-zinc-50 group-data-[selected=true]:border-zinc-950 dark:group-data-[selected=true]:border-zinc-50 transition-all shrink-0">
              <Checkbox.Indicator className="text-white dark:text-zinc-950 flex items-center justify-center">
                {/* Crisp premium Checkmark Icon */}
                <svg className="w-2.5 h-2.5 fill-none stroke-current stroke-[3]" viewBox="0 0 24 24">
                  <polyline points="20 6 9 17 4 12" />
                </svg>
              </Checkbox.Indicator>
            </Checkbox.Control>
            <Checkbox.Content className="selection:bg-transparent">
              {children}
            </Checkbox.Content>
          </Checkbox>
        )}
      />
      {errorMessage && (
        <span className="text-red-500 text-[10px] mt-1 pl-1 block font-medium" role="alert">
          {errorMessage}
        </span>
      )}
    </div>
  );
};
export default FormCheckbox;
