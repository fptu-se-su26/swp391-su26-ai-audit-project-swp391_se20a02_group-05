"use client";

import { Check } from "lucide-react";

interface TagSelectorProps {
  options: string[];
  selected: string[];
  onChange: (selected: string[]) => void;
  isMulti?: boolean;
  label?: string;
  className?: string;
}

export default function TagSelector({
  options = [],
  selected = [],
  onChange,
  isMulti = true,
  label,
  className = ""
}: TagSelectorProps) {
  
  const handleToggle = (option: string) => {
    if (isMulti) {
      if (selected.includes(option)) {
        onChange(selected.filter((item) => item !== option));
      } else {
        onChange([...selected, option]);
      }
    } else {
      if (selected.includes(option)) {
        onChange([]);
      } else {
        onChange([option]);
      }
    }
  };

  return (
    <div className={`flex flex-col gap-2 w-full ${className}`}>
      {label && <span className="text-sm font-medium text-default-700">{label}</span>}
      <div className="flex flex-wrap gap-2">
        {options.map((option) => {
          const isSelected = selected.includes(option);
          return (
            <button
              key={option}
              type="button"
              onClick={() => handleToggle(option)}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-semibold border transition-all duration-200 select-none cursor-pointer transform active:scale-95 ${
                isSelected
                  ? "bg-primary/10 border-primary text-primary shadow-sm ring-1 ring-primary/20 scale-[1.02]"
                  : "bg-surface-secondary/30 border-border text-default-600 hover:bg-surface-secondary hover:border-default-400 hover:text-default-800"
              }`}
            >
              {isSelected && <Check className="w-3.5 h-3.5 shrink-0 transition-transform duration-200 scale-100" />}
              <span>{option}</span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
