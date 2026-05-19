"use client";

import React, { useEffect, useState } from 'react';
import { Select, Label, ListBox } from '@heroui/react';

interface Option {
  value: string;
  label: string;
}

interface SelectDropdownProps {
  value: string;
  onChange: (value: string) => void;
  options: Option[];
  placeholder?: string;
  label?: string;
  className?: string;
  isDisabled?: boolean;
}

export const SelectDropdown: React.FC<SelectDropdownProps> = ({
  value,
  onChange,
  options,
  placeholder = "Select option...",
  label,
  className = "w-full",
  isDisabled = false,
}) => {
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    let active = true;
    Promise.resolve().then(() => {
      if (active) {
        setIsMounted(true);
      }
    });
    return () => {
      active = false;
    };
  }, []);

  if (!isMounted) return null;

  return (
    <Select
      value={value}
      onChange={(val) => onChange(val as string)}
      isDisabled={isDisabled}
      className={className}
      placeholder={placeholder}
      variant="secondary"
    >
      {label && <Label className="text-xs font-bold text-zinc-500 dark:text-zinc-400 mb-1.5 block">{label}</Label>}
      <Select.Trigger className="w-full flex items-center justify-between px-3.5 py-2.5 rounded-xl border border-zinc-200/60 dark:border-zinc-800 bg-white/50 dark:bg-zinc-900/50 text-xs font-semibold focus:outline-none cursor-pointer hover:border-zinc-300 dark:hover:border-zinc-700 transition-all select-none">
        <Select.Value />
        <Select.Indicator className="size-4 text-zinc-400" />
      </Select.Trigger>
      <Select.Popover className="border border-zinc-200 dark:border-zinc-800 rounded-xl bg-white dark:bg-zinc-950 shadow-xl overflow-hidden min-w-[200px] z-50">
        <ListBox className="p-1 max-h-60 overflow-y-auto outline-none focus:outline-none">
          {options.map((option) => (
            <ListBox.Item
              key={option.value}
              id={option.value}
              textValue={option.label}
              className="flex items-center justify-between px-3 py-2 text-xs font-medium text-zinc-700 dark:text-zinc-300 hover:bg-zinc-50/50 dark:hover:bg-zinc-900/50 rounded-lg cursor-pointer transition-colors outline-none focus:bg-zinc-50/50 dark:focus:bg-zinc-900/50"
            >
              <span>{option.label}</span>
              <ListBox.ItemIndicator className="size-3 text-indigo-500 dark:text-indigo-400" />
            </ListBox.Item>
          ))}
        </ListBox>
      </Select.Popover>
    </Select>
  );
};

export default SelectDropdown;
