"use client";

import React from 'react';
import { AlertTriangle, RefreshCcw, FolderOpen } from 'lucide-react';

// SKELETON TABLE ROW LOADER
interface SkeletonLoaderProps {
  rows?: number;
  columns?: number;
}

export const SkeletonLoader: React.FC<SkeletonLoaderProps> = ({
  rows = 5,
  columns = 5,
}) => {
  return (
    <div className="w-full space-y-4 p-5 animate-pulse">
      {/* Table Header skeleton */}
      <div className="flex items-center gap-4 py-2 border-b border-zinc-200 dark:border-zinc-800">
        {Array.from({ length: columns }).map((_, idx) => (
          <div key={`h-${idx}`} className="h-4 bg-zinc-200 dark:bg-zinc-850 rounded-lg flex-1" />
        ))}
      </div>
      {/* Table Body rows */}
      {Array.from({ length: rows }).map((_, rowIdx) => (
        <div
          key={`r-${rowIdx}`}
          className="flex items-center gap-4 py-4 border-b border-zinc-150/50 dark:border-zinc-900/50"
        >
          {Array.from({ length: columns }).map((_, colIdx) => (
            <div
              key={`c-${rowIdx}-${colIdx}`}
              className={`h-3 bg-zinc-100 dark:bg-zinc-900 rounded-lg flex-1 ${
                colIdx === 0 ? "w-3/4" : colIdx === columns - 1 ? "w-1/2" : "w-full"
              }`}
            />
          ))}
        </div>
      ))}
    </div>
  );
};

// PREMIER EMPTY DIRECTORY STATE
interface EmptyStateProps {
  title: string;
  description: string;
  action?: React.ReactNode;
}

export const EmptyState: React.FC<EmptyStateProps> = ({
  title,
  description,
  action,
}) => {
  return (
    <div className="flex flex-col items-center justify-center text-center p-12 md:p-20 max-w-md mx-auto select-none">
      <div className="size-16 rounded-2xl bg-indigo-50 dark:bg-indigo-950/30 flex items-center justify-center border border-indigo-150/30 dark:border-indigo-900/30 mb-5 text-indigo-500 animate-bounce duration-[2000ms]">
        <FolderOpen size={28} />
      </div>
      <h3 className="text-base font-extrabold text-zinc-900 dark:text-zinc-50 tracking-tight mb-2">
        {title}
      </h3>
      <p className="text-xs text-zinc-500 dark:text-zinc-400 leading-relaxed mb-6">
        {description}
      </p>
      {action && <div className="flex justify-center">{action}</div>}
    </div>
  );
};

// PREMIER DATA FETCH ERROR / DISCONNECT STATE
interface ErrorStateProps {
  message: string;
  onRetry?: () => void;
}

export const ErrorState: React.FC<ErrorStateProps> = ({
  message,
  onRetry,
}) => {
  return (
    <div className="flex flex-col items-center justify-center text-center p-12 md:p-20 max-w-md mx-auto select-none">
      <div className="size-16 rounded-2xl bg-rose-50 dark:bg-rose-950/30 flex items-center justify-center border border-rose-150/30 dark:border-rose-900/30 mb-5 text-rose-500">
        <AlertTriangle size={28} className="animate-pulse" />
      </div>
      <h3 className="text-base font-extrabold text-zinc-900 dark:text-zinc-50 tracking-tight mb-2">
        Database Synchronization Failure
      </h3>
      <p className="text-xs text-zinc-500 dark:text-zinc-400 leading-relaxed mb-6">
        {message || "We encountered a network timeout while attempting to update your administrative data feeds."}
      </p>
      {onRetry && (
        <button
          onClick={onRetry}
          className="px-4 py-2.5 bg-zinc-900 text-white dark:bg-zinc-100 dark:text-zinc-950 text-xs font-bold rounded-xl flex items-center gap-2 hover:opacity-90 transition-opacity cursor-pointer shadow-sm"
        >
          <RefreshCcw size={14} />
          Retry Connection
        </button>
      )}
    </div>
  );
};

const States = {
  SkeletonLoader,
  EmptyState,
  ErrorState,
};

export default States;
