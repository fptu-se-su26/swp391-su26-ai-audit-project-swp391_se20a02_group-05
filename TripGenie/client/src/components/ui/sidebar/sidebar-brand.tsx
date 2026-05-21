"use client";

import React from 'react';
import { Compass } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Typography } from '@heroui/react';
interface SidebarBrandProps {
  collapsed: boolean;
}

export const SidebarBrand: React.FC<SidebarBrandProps> = ({ collapsed }) => {
  const { t } = useTranslation(['common']);

  return (
    <div className="flex h-16 items-center px-4 select-none border-b border-separator shrink-0 gap-2.5 overflow-hidden transition-all duration-300">
      {/* Brand Icon Box */}
      <div className="w-8 h-8 rounded-lg bg-foreground text-background flex items-center justify-center shadow-md shrink-0">
        <Compass size={18} />
      </div>

      {/* Brand Name Text */}
      <div
        className={[
          "flex flex-col min-w-0 transition-all duration-300 ease-in-out",
          collapsed ? "w-0 opacity-0 pointer-events-none" : "w-auto opacity-100"
        ].join(' ')}
      >
        <Typography
          type="body-sm"
          className="font-bold bg-clip-text text-transparent bg-linear-to-r from-foreground to-muted font-outfit truncate leading-none"
        >
          {t('common:branding.title', { defaultValue: 'CVerify' })}
        </Typography>
      </div>
    </div>
  );
};

export default SidebarBrand;
