"use client";

import React from 'react';
import { useTranslation } from 'react-i18next';
import { Separator, Typography } from '@heroui/react';
import { NavigationSectionItem } from '@/types/navigation.types';
import SidebarLink from './sidebar-link';
import SidebarGroup from './sidebar-group';

interface SidebarSectionProps {
  section: NavigationSectionItem;
  collapsed: boolean;
  isMobile: boolean;
  depth?: number;
}

export const SidebarSection: React.FC<SidebarSectionProps> = ({ section, collapsed, isMobile, depth = 0 }) => {
  const { t } = useTranslation(['common']);

  // Localized section title with fallback
  const label = section.translationKey
    ? t(section.translationKey, { defaultValue: section.label })
    : section.label;

  return (
    <div className="flex flex-col gap-1 w-full select-none">
      {/* Visual Section Header Label */}
      {collapsed ? (
        <div className="my-2 shrink-0">
          <Separator variant="tertiary" />
        </div>
      ) : (
        <div className="px-4 pt-4 pb-1 shrink-0">
          <Typography
            type="body-xs"
            className="text-muted/65 text-[10px] font-extrabold uppercase tracking-wider font-outfit select-none truncate"
          >
            {label}
          </Typography>
        </div>
      )}

      {/* Render nested children inside section */}
      <div className="flex flex-col gap-1.5 w-full">
        {section.children.map((child) => {
          if (child.type === 'item') {
            return (
              <SidebarLink
                key={child.id}
                item={child}
                collapsed={collapsed}
                isMobile={isMobile}
                depth={depth}
              />
            );
          }
          if (child.type === 'group') {
            return (
              <SidebarGroup
                key={child.id}
                group={child}
                collapsed={collapsed}
                isMobile={isMobile}
                depth={depth}
              />
            );
          }
          return null;
        })}
      </div>
    </div>
  );
};

export default SidebarSection;
