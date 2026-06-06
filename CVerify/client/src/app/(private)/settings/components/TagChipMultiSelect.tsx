"use client";
import React, { useState } from "react";
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
  const [customInput, setCustomInput] = useState("");
  const [localError, setLocalError] = useState("");

  const handleToggle = (option: string) => {
    setLocalError("");
    const isSelected = value.includes(option);
    let newValue;
    if (isSelected) {
      newValue = value.filter((v) => v !== option);
    } else {
      if (value.length >= 20) {
        setLocalError("You can select up to 20 options.");
        return;
      }
      newValue = [...value, option];
    }
    onChange(newValue);
  };

  const handleAddCustomTag = () => {
    setLocalError("");
    const trimmed = customInput.trim();
    if (!trimmed) return;

    if (value.length >= 20) {
      setLocalError("You can select up to 20 options.");
      return;
    }

    if (trimmed.length > 100) {
      setLocalError("Tag cannot exceed 100 characters.");
      return;
    }

    if (value.some((v) => v.toLowerCase() === trimmed.toLowerCase())) {
      setLocalError("Duplicate tag is not allowed.");
      return;
    }

    onChange([...value, trimmed]);
    setCustomInput("");
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleAddCustomTag();
    }
  };

  const allChips = Array.from(new Set([...options, ...value]));

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
        {allChips.map((option) => {
          const isSelected = value.includes(option);
          const isCustom = !options.includes(option);
          return (
            <button
              key={option}
              type="button"
              onClick={() => handleToggle(option)}
              className={`px-3 py-1.5 rounded-full text-xs font-semibold border transition-all duration-150 cursor-pointer select-none flex items-center gap-1
                ${
                  isSelected
                    ? "bg-accent text-accent-foreground border-accent shadow-xs scale-[1.02]"
                    : "bg-field text-foreground/80 border-field-border hover:border-border hover:bg-surface-secondary active:scale-95"
                }
              `}
            >
              {option}
              {isCustom && isSelected && (
                <span className="text-[10px] font-bold opacity-60 ml-0.5">×</span>
              )}
            </button>
          );
        })}
      </div>

      {/* Custom Tag Input */}
      <div className="flex gap-2 items-center mt-2 max-w-xs sm:max-w-sm">
        <input
          type="text"
          placeholder="Add custom option..."
          value={customInput}
          onChange={(e) => {
            setCustomInput(e.target.value);
            if (localError) setLocalError("");
          }}
          onKeyDown={handleKeyDown}
          className="flex-1 px-3 py-1.5 text-xs rounded-xl bg-field border border-field-border focus:border-accent outline-hidden transition-all text-foreground"
        />
        <button
          type="button"
          disabled={!customInput.trim()}
          onClick={handleAddCustomTag}
          className={`px-3 py-1.5 text-xs font-bold rounded-xl transition-all
            ${!customInput.trim()
              ? "bg-surface-secondary text-muted border border-border/40 cursor-not-allowed opacity-60"
              : "bg-accent text-accent-foreground hover:opacity-90 active:scale-95 cursor-pointer"
            }
          `}
        >
          Add
        </button>
      </div>

      {(localError || error) && (
        <Typography type="body-xs" className="text-danger pl-1 font-semibold block" role="alert">
          {localError || error}
        </Typography>
      )}
    </div>
  );
};

export default TagChipMultiSelect;
