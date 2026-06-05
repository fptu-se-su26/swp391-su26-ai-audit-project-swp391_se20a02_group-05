"use client";

import React from "react";
import { Typography } from "@heroui/react";

interface TagChipMultiSelectProps {
  label?: string;
  description?: string;
  options: string[];
  value: string[];
  onChange: (value: string[]) => void;
  error?: string;
}

export const TagChipMultiSelect: React.FC<TagChipMultiSelectProps> = ({
  label,
  description,
  options,
  value = [],
  onChange,
  error,
}) => {
  const handleToggle = (option: string) => {
    const isSelected = value.includes(option);
    let newValue;
    if (isSelected) {
      newValue = value.filter((v) => v !== option);
    } else {
      newValue = [...value, option];
    }
    onChange(newValue);
  };

  return (
    <div className="flex flex-col gap-2 w-full text-left">
      {label && (
        <div className="flex flex-col gap-0.5 select-none">
          <Typography type="body-sm" className="font-bold text-foreground font-outfit">
            {label}
          </Typography>
          {description && (
            <Typography type="body-xs" className="text-muted max-w-xl">
              {description}
            </Typography>
          )}
        </div>
      )}

      <div className="flex flex-wrap gap-2 mt-1">
        {options.map((option) => {
          const isSelected = value.includes(option);
          return (
            <button
              key={option}
              type="button"
              onClick={() => handleToggle(option)}
              className={`px-3 py-1.5 rounded-full text-xs font-semibold border transition-all duration-150 cursor-pointer select-none
                ${
                  isSelected
                    ? "bg-accent text-accent-foreground border-accent shadow-xs scale-[1.02]"
                    : "bg-field text-foreground/80 border-field-border hover:border-border hover:bg-surface-secondary active:scale-95"
                }
              `}
            >
              {option}
            </button>
          );
        })}
      </div>

      {error && (
        <Typography type="body-xs" className="text-danger pl-1 font-semibold block" role="alert">
          {error}
        </Typography>
      )}
    </div>
  );
};

export default TagChipMultiSelect;
