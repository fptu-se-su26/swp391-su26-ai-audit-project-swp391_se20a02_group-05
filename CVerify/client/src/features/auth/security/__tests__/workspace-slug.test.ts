/**
 * White-box unit tests — workspace slug validation & generation.
 */
import {
  generateSuggestedSlug,
  isLookalikeSlug,
  isReservedSlug,
  RESERVED_SLUGS,
} from '../workspace-slug';

describe('workspace-slug (white-box)', () => {
  describe('isReservedSlug', () => {
    it.each(RESERVED_SLUGS)('blocks reserved slug "%s"', (slug) => {
      expect(isReservedSlug(slug)).toBe(true);
      expect(isReservedSlug(slug.toUpperCase())).toBe(true);
    });

    it('allows non-reserved slugs', () => {
      expect(isReservedSlug('acme-corp')).toBe(false);
    });
  });

  describe('isLookalikeSlug', () => {
    it('allows exact brand match (not impersonation)', () => {
      expect(isLookalikeSlug('google')).toBe(false);
    });

    it.each([
      ['g00gle'],
      ['faceb00k'],
      ['cver1fy'],
      ['adm1n-panel'],
    ])('detects lookalike "%s"', (slug) => {
      expect(isLookalikeSlug(slug)).toBe(true);
    });

    it('allows unrelated slugs', () => {
      expect(isLookalikeSlug('fpt-software')).toBe(false);
    });
  });

  describe('generateSuggestedSlug', () => {
    it('strips Vietnamese diacritics', () => {
      expect(generateSuggestedSlug('Công Ty TNHH')).toBe('cong-ty-tnhh');
    });

    it('pads short names to minimum 4 characters', () => {
      expect(generateSuggestedSlug('AB').length).toBeGreaterThanOrEqual(4);
    });

    it('truncates to 32 characters', () => {
      const long = 'A'.repeat(100);
      expect(generateSuggestedSlug(long).length).toBeLessThanOrEqual(32);
    });

    it('handles empty string', () => {
      expect(generateSuggestedSlug('').length).toBe(4);
    });
  });
});
