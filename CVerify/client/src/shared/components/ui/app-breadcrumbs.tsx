"use client";

import React from 'react';
import { usePathname, useRouter } from 'next/navigation';
import { Breadcrumbs, BreadcrumbsItem } from '@heroui/react';
import { useTranslation } from 'react-i18next';
import { getRouteMetadata, getDynamicSegmentLabel } from '@/config/routes';

export const AppBreadcrumbs: React.FC = () => {
  const pathname = usePathname();
  const router = useRouter();
  const { t } = useTranslation(['common']);

  if (!pathname) return null;

  // Split path, remove empty strings, and ignore Next.js parenthesized folder route groups (e.g. (private))
  const segments = pathname
    .split('/')
    .filter((segment) => segment && !segment.startsWith('(') && !segment.endsWith(')'));

  // Build breadcrumb items based on accumulated path segments
  const breadcrumbItems = segments.map((segment, index) => {
    // Construct cumulative URL path up to current index
    const href = '/' + segments.slice(0, index + 1).join('/');
    const metadata = getRouteMetadata(href);
    
    let label = '';
    
    if (metadata) {
      // Resolve localized label using standard translation key if present
      label = t(metadata.translationKey, { defaultValue: metadata.fallbackLabel });
    } else {
      // If no route metadata exists, dynamically format the raw segment (e.g. UUID, number, slug)
      label = getDynamicSegmentLabel(segment);
    }

    const isLast = index === segments.length - 1;

    return {
      href,
      label,
      isLast,
    };
  });

  return (
    <nav aria-label="Breadcrumbs Navigation" className="hidden md:flex items-center font-outfit select-none">
      <Breadcrumbs
        separator={
          <svg className="h-3 w-3 text-muted/60 shrink-0" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
          </svg>
        }
        className="flex items-center"
      >
        {/* Static Root Node */}
        <BreadcrumbsItem
          href="/user"
          onClick={(e) => {
            e.preventDefault();
            router.push('/user');
          }}
          className="text-xs font-semibold text-muted hover:text-foreground transition-colors duration-150 focus-visible:outline-hidden focus-visible:ring-2 focus-visible:ring-focus rounded-lg px-1.5 py-0.5 cursor-pointer"
        >
          {t('common:dashboard.pages')}
        </BreadcrumbsItem>

        {/* Dynamic Nodes */}
        {breadcrumbItems.map((item) => (
          <BreadcrumbsItem
            key={item.href}
            href={item.isLast ? undefined : item.href}
            onClick={
              item.isLast
                ? undefined
                : (e) => {
                    e.preventDefault();
                    router.push(item.href);
                  }
            }
            className={
              item.isLast
                ? "text-xs font-extrabold text-foreground tracking-tight select-text px-1.5 py-0.5"
                : "text-xs font-semibold text-muted/80 hover:text-foreground transition-colors duration-150 focus-visible:outline-hidden focus-visible:ring-2 focus-visible:ring-focus rounded-lg px-1.5 py-0.5 cursor-pointer"
            }
          >
            {item.label}
          </BreadcrumbsItem>
        ))}
      </Breadcrumbs>
    </nav>
  );
};

export default AppBreadcrumbs;

