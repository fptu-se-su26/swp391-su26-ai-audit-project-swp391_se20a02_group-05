"use client";

import React from 'react';
import { Accordion } from '@heroui/react';
import { ChevronDown } from 'lucide-react';

interface AccordionItemData {
  id: string;
  title: string;
  content: React.ReactNode;
  icon?: React.ReactNode;
}

interface AccordionWrapperProps {
  items: AccordionItemData[];
  className?: string;
  variant?: 'surface' | 'default';
  allowsMultipleExpanded?: boolean;
  expandedKeys?: Set<string | number>;
  onExpandedChange?: (keys: unknown) => void;
}

export const AccordionWrapper: React.FC<AccordionWrapperProps> = ({
  items,
  className = "w-full",
  variant = "surface",
  allowsMultipleExpanded = true,
  expandedKeys,
  onExpandedChange,
}) => {
  return (
    <Accordion
      className={className}
      variant={variant}
      allowsMultipleExpanded={allowsMultipleExpanded}
      expandedKeys={expandedKeys}
      onExpandedChange={onExpandedChange}
    >
      {items.map((item) => (
        <Accordion.Item key={item.id} id={item.id} className="border-zinc-200 dark:border-zinc-800">
          <Accordion.Heading>
            <Accordion.Trigger className="w-full flex items-center justify-between py-4 px-5 text-sm font-bold text-zinc-900 dark:text-zinc-550 select-none cursor-pointer outline-none focus:outline-none">
              <div className="flex items-center gap-3">
                {item.icon && <span className="text-zinc-500 dark:text-zinc-400 size-4">{item.icon}</span>}
                <span>{item.title}</span>
              </div>
              <Accordion.Indicator className="text-zinc-400 transition-transform duration-200 ease-out">
                <ChevronDown size={16} />
              </Accordion.Indicator>
            </Accordion.Trigger>
          </Accordion.Heading>
          <Accordion.Panel>
            <Accordion.Body className="px-5 pb-5 text-xs md:text-sm text-zinc-500 dark:text-zinc-400 leading-relaxed border-t border-zinc-100/50 dark:border-zinc-900/50 pt-4 bg-zinc-50/30 dark:bg-zinc-950/20">
              {item.content}
            </Accordion.Body>
          </Accordion.Panel>
        </Accordion.Item>
      ))}
    </Accordion>
  );
};

export default AccordionWrapper;
