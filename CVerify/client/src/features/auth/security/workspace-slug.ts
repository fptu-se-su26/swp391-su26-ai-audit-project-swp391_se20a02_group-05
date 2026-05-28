/** Reserved workspace slugs that cannot be registered. */
export const RESERVED_SLUGS = [
  'admin',
  'root',
  'support',
  'system',
  'api',
  'cverify',
  'help',
  'billing',
  'status',
  'security',
] as const;

/** Typosquatting / lookalike brand detection for workspace slugs. */
export function isLookalikeSlug(slug: string): boolean {
  const normalized = slug
    .replace(/0/g, 'o')
    .replace(/1/g, 'i')
    .replace(/3/g, 'e')
    .replace(/vv/g, 'w')
    .replace(/l1/g, 'll');

  const criticalBrands = [
    'google',
    'facebook',
    'linkedin',
    'cverify',
    'admin',
    'microsoft',
    'github',
    'stripe',
  ];

  return criticalBrands.some(
    (brand) => normalized.includes(brand) && slug !== brand,
  );
}

export function isReservedSlug(slug: string): boolean {
  return RESERVED_SLUGS.includes(slug.toLowerCase() as (typeof RESERVED_SLUGS)[number]);
}

/** Normalize company name into a URL-safe workspace slug suggestion (Vietnamese-aware). */
export function generateSuggestedSlug(name: string): string {
  let slug = name
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd')
    .replace(/[^a-z0-9\s-]/g, '')
    .trim()
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-');

  if (slug.length < 4) {
    slug = slug.padEnd(4, '0');
  }
  return slug.substring(0, 32);
}
