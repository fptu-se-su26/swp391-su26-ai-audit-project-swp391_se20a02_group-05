import React from 'react';

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
  children: React.ReactNode;
  className?: string;
  glow?: boolean;
}

export const Card: React.FC<CardProps> = ({
  children,
  className = '',
  glow = true,
  ...props
}) => {
  return (
    <div
      {...props}
      className={[
        "relative overflow-hidden w-full rounded-2xl p-6 sm:p-8 transition-all duration-300",
        "bg-white/80 dark:bg-zinc-950/70",
        "backdrop-blur-xl",
        "border border-zinc-200/60 dark:border-zinc-900/80",
        // Premium drop shadows and glowing border effects
        glow
          ? "shadow-[0_8px_30px_rgb(0,0,0,0.04)] dark:shadow-[0_20px_50px_rgba(0,0,0,0.4)] before:absolute before:inset-0 before:rounded-2xl before:p-px before:bg-linear-to-b before:from-zinc-200/50 before:to-transparent dark:before:from-zinc-800/40 dark:before:to-transparent before:-z-10"
          : "shadow-sm",
        className
      ].join(' ')}
    >
      {/* Decorative subtle top glow inside the card */}
      {glow && (
        <div className="absolute top-0 left-1/4 right-1/4 h-px bg-linear-to-r from-transparent via-zinc-300/30 dark:via-zinc-700/20 to-transparent pointer-events-none" />
      )}

      <div className="relative z-10 flex flex-col h-full w-full">
        {children}
      </div>
    </div>
  );
};
