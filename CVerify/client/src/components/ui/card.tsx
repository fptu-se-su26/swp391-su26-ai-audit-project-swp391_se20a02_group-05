import React from 'react';

interface CardProps extends React.HTMLAttributes<HTMLElement> {
  children: React.ReactNode;
  className?: string;
  glow?: boolean;
  as?: 'div' | 'button' | 'a';
  type?: 'button' | 'submit' | 'reset';
  href?: string;
}

export const Card: React.FC<CardProps> = ({
  children,
  className = '',
  glow = true,
  as = 'div',
  ...props
}) => {
  const Component = as as React.ElementType;
<<<<<<< Updated upstream
  return (
    <Component
      {...props}
      className={[
        "relative overflow-hidden w-full rounded-2xl p-6 sm:p-8 transition-all duration-300 text-left",
=======
  
  const roundedClass = `rounded-${rounded}`;
  const beforeRoundedClass = `before:rounded-${rounded}`;

  // Check if className contains any custom padding overrides
  const hasPadding = className.split(/\s+/).some(cls => 
    /^(?:[a-z0-9]+:)?p[xytrbl]?-/.test(cls)
  );

  return (
    <Component
      {...props}
      className={cn(
        "relative overflow-hidden w-full transition-all duration-300 text-left",
        hasPadding ? "" : "p-6 sm:p-8",
        roundedClass,
>>>>>>> Stashed changes
        "bg-surface text-foreground",
        "border border-border/60",
        // Premium drop shadows and glowing border effects
        glow
          ? "shadow-surface before:absolute before:inset-0 before:rounded-2xl before:p-px before:bg-linear-to-b before:from-border/40 before:to-transparent before:-z-10"
          : "shadow-sm",
        className
      ].join(' ')}
    >
      {/* Decorative subtle top glow inside the card */}
      {glow && (
        <div className="absolute top-0 left-1/4 right-1/4 h-px bg-linear-to-r from-transparent via-border/30 to-transparent pointer-events-none" />
      )}

      <div className="relative z-10 flex flex-col h-full w-full">
        {children}
      </div>
    </Component>
  );
};

