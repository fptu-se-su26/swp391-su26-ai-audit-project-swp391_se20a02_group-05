"use client";

import { useState, useRef, useEffect } from "react";
import { ProjectMember } from "@/types/project";
import { Search, ChevronDown, Check, X, User } from "lucide-react";
import { Button, Input, Card } from "@heroui/react";

interface MemberSelectProps {
  members: ProjectMember[];
  selectedId: string | string[];
  onChange: (id: string | string[]) => void;
  placeholder?: string;
  isMulti?: boolean;
  label?: string;
}

export default function MemberSelect({
  members = [],
  selectedId,
  onChange,
  placeholder = "Select member...",
  isMulti = false,
  label
}: MemberSelectProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState("");
  const containerRef = useRef<HTMLDivElement>(null);

  // Close dropdown on click outside
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Helpers for deterministic HSL colors
  const getAvatarColor = (name: string) => {
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
      hash = name.charCodeAt(i) + ((hash << 5) - hash);
    }
    const h = Math.abs(hash) % 360;
    return `hsl(${h}, 65%, 42%)`;
  };

  const getInitials = (name: string) => {
    if (!name) return "?";
    const parts = name.trim().split(/\s+/);
    if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  };

  // Convert selectedId to normalized array
  const selectedIds = Array.isArray(selectedId)
    ? selectedId
    : selectedId
    ? [selectedId]
    : [];

  const selectedMembers = members.filter((m) => selectedIds.includes(m.id));

  const filteredMembers = members.filter((m) => {
    const term = search.toLowerCase();
    return (
      m.name.toLowerCase().includes(term) ||
      m.studentId.toLowerCase().includes(term)
    );
  });

  const handleSelect = (memberId: string) => {
    if (isMulti) {
      if (selectedIds.includes(memberId)) {
        onChange(selectedIds.filter((id) => id !== memberId));
      } else {
        onChange([...selectedIds, memberId]);
      }
    } else {
      onChange(memberId);
      setIsOpen(false);
    }
  };

  const handleRemove = (memberId: string, e: React.MouseEvent) => {
    e.stopPropagation();
    onChange(selectedIds.filter((id) => id !== memberId));
  };

  return (
    <div className="flex flex-col gap-1.5 w-full relative" ref={containerRef}>
      {label && <label className="text-sm font-medium text-default-700">{label}</label>}

      {/* Selector Trigger */}
      <div
        onClick={() => setIsOpen(!isOpen)}
        className="flex min-h-[44px] w-full items-center justify-between gap-2 rounded-xl border border-border bg-surface-secondary/40 px-3.5 py-2 text-sm shadow-sm transition-all duration-200 hover:border-default-400 hover:bg-surface-secondary/60 cursor-pointer focus-within:ring-2 focus-within:ring-primary focus-within:border-primary"
      >
        <div className="flex flex-wrap gap-1.5 items-center flex-1 overflow-hidden">
          {selectedMembers.length === 0 ? (
            <span className="text-default-400 select-none">{placeholder}</span>
          ) : isMulti ? (
            selectedMembers.map((member) => (
              <div
                key={member.id}
                className="flex items-center gap-1.5 bg-primary/10 text-primary text-xs font-semibold px-2 py-1 rounded-full border border-primary/20 transition-all duration-150 hover:bg-primary/20 shrink-0"
              >
                <div
                  className="w-4 h-4 rounded-full flex items-center justify-center text-[8px] font-black text-white shrink-0"
                  style={{ backgroundColor: getAvatarColor(member.name) }}
                >
                  {getInitials(member.name)}
                </div>
                <span>{member.name}</span>
                <button
                  onClick={(e) => handleRemove(member.id, e)}
                  className="hover:text-danger rounded-full transition-colors p-0.5 shrink-0"
                  aria-label={`Remove ${member.name}`}
                >
                  <X className="w-3 h-3" />
                </button>
              </div>
            ))
          ) : (
            <div className="flex items-center gap-2.5">
              <div
                className="w-6 h-6 rounded-full flex items-center justify-center text-[10px] font-black text-white shadow-sm shrink-0"
                style={{ backgroundColor: getAvatarColor(selectedMembers[0].name) }}
              >
                {getInitials(selectedMembers[0].name)}
              </div>
              <div className="flex flex-col text-left leading-tight">
                <span className="font-semibold text-default-800">{selectedMembers[0].name}</span>
                <span className="text-[10px] text-default-400">{selectedMembers[0].studentId}</span>
              </div>
            </div>
          )}
        </div>
        <ChevronDown className={`w-4 h-4 text-default-400 transition-transform duration-200 shrink-0 ${isOpen ? "rotate-180" : ""}`} />
      </div>

      {/* Searchable Dropdown List */}
      {isOpen && (
        <Card className="absolute top-[105%] left-0 right-0 z-50 p-2 shadow-2xl border border-border bg-surface flex flex-col gap-2 rounded-xl animate-in fade-in slide-in-from-top-2 duration-200 max-h-72">
          {/* Search Field */}
          <div className="flex items-center gap-2 border-b border-border pb-2 px-1">
            <Search className="w-4 h-4 text-default-400 shrink-0" />
            <input
              type="text"
              placeholder="Search member..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full bg-transparent text-sm text-default-800 outline-none placeholder:text-default-400 py-1"
              autoFocus
            />
            {search && (
              <button
                onClick={() => setSearch("")}
                className="text-xs text-default-400 hover:text-default-600 transition-colors p-0.5"
              >
                Clear
              </button>
            )}
          </div>

          {/* Members List */}
          <div className="overflow-y-auto flex flex-col gap-0.5 pr-1">
            {filteredMembers.length === 0 ? (
              <div className="text-center py-6 text-default-400 text-xs italic">
                No matching members found.
              </div>
            ) : (
              filteredMembers.map((member) => {
                const isSelected = selectedIds.includes(member.id);
                return (
                  <div
                    key={member.id}
                    onClick={() => handleSelect(member.id)}
                    className={`flex items-center justify-between gap-3 p-2 rounded-lg cursor-pointer transition-colors duration-150 ${
                      isSelected
                        ? "bg-primary/5 hover:bg-primary/10 text-primary-foreground"
                        : "hover:bg-default-100"
                    }`}
                  >
                    <div className="flex items-center gap-2.5 min-w-0">
                      <div
                        className="w-7 h-7 rounded-full flex items-center justify-center text-[10px] font-black text-white shadow-sm shrink-0"
                        style={{ backgroundColor: getAvatarColor(member.name) }}
                      >
                        {getInitials(member.name)}
                      </div>
                      <div className="flex flex-col text-left leading-tight min-w-0">
                        <span className={`text-sm font-semibold truncate ${isSelected ? "text-primary" : "text-default-800"}`}>
                          {member.name}
                        </span>
                        <span className="text-[10px] text-default-400 truncate">
                          {member.studentId}
                        </span>
                      </div>
                    </div>
                    {isSelected && (
                      <Check className="w-4 h-4 text-primary shrink-0 mr-1" />
                    )}
                  </div>
                );
              })
            )}
          </div>
        </Card>
      )}
    </div>
  );
}
