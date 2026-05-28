import React from 'react';

export const Typography = {
  Heading: ({
    children,
    ...props
  }: React.PropsWithChildren<Record<string, unknown>>) =>
    React.createElement('h6', props, children),
  Prose: ({
    children,
    ...props
  }: React.PropsWithChildren<Record<string, unknown>>) =>
    React.createElement('div', props, children),
};

export const Button = ({
  children,
  ...props
}: React.PropsWithChildren<Record<string, unknown>>) =>
  React.createElement('button', { type: 'button', ...props }, children);

export const Link = ({
  children,
  href,
  ...props
}: React.PropsWithChildren<{ href?: string } & Record<string, unknown>>) =>
  React.createElement('a', { href, ...props }, children);
