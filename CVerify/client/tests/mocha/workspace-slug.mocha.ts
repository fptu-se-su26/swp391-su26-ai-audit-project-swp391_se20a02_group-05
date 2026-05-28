/**
 * Black-box behavioral tests — workspace slug validation (Mocha + Chai).
 * Tests only the public API; no access to internal constants or implementation details.
 */
import { expect } from 'chai';
import {
  generateSuggestedSlug,
  isLookalikeSlug,
  isReservedSlug,
} from '../../src/features/auth/security/workspace-slug';

describe('workspace-slug (black-box)', () => {
  describe('isReservedSlug', () => {
    it('rejects "admin"', () => {
      expect(isReservedSlug('admin')).to.be.true;
    });

    it('rejects "cverify" (brand namespace protection)', () => {
      expect(isReservedSlug('cverify')).to.be.true;
    });

    it('rejects "support"', () => {
      expect(isReservedSlug('support')).to.be.true;
    });

    it('rejects "api"', () => {
      expect(isReservedSlug('api')).to.be.true;
    });

    it('rejects "system"', () => {
      expect(isReservedSlug('system')).to.be.true;
    });

    it('rejects "billing"', () => {
      expect(isReservedSlug('billing')).to.be.true;
    });

    it('is case-insensitive — "ADMIN" is reserved', () => {
      expect(isReservedSlug('ADMIN')).to.be.true;
    });

    it('is case-insensitive — "Support" is reserved', () => {
      expect(isReservedSlug('Support')).to.be.true;
    });

    it('allows a normal company slug "fpt-software"', () => {
      expect(isReservedSlug('fpt-software')).to.be.false;
    });

    it('allows "acme-corp"', () => {
      expect(isReservedSlug('acme-corp')).to.be.false;
    });

    it('allows "my-company-2024"', () => {
      expect(isReservedSlug('my-company-2024')).to.be.false;
    });
  });

  describe('isLookalikeSlug', () => {
    it('detects "g00gle" as a lookalike', () => {
      expect(isLookalikeSlug('g00gle')).to.be.true;
    });

    it('detects "faceb00k" as a lookalike', () => {
      expect(isLookalikeSlug('faceb00k')).to.be.true;
    });

    it('detects "cver1fy" as a CVerify lookalike', () => {
      expect(isLookalikeSlug('cver1fy')).to.be.true;
    });

    it('detects "adm1n-panel" as an admin lookalike', () => {
      expect(isLookalikeSlug('adm1n-panel')).to.be.true;
    });

    it('detects "m1crosoft" as a Microsoft lookalike', () => {
      expect(isLookalikeSlug('m1crosoft')).to.be.true;
    });

    it('allows "fpt-software" (not a brand lookalike)', () => {
      expect(isLookalikeSlug('fpt-software')).to.be.false;
    });

    it('allows "acme-corporation"', () => {
      expect(isLookalikeSlug('acme-corporation')).to.be.false;
    });

    it('allows "my-startup-2024"', () => {
      expect(isLookalikeSlug('my-startup-2024')).to.be.false;
    });
  });

  describe('generateSuggestedSlug', () => {
    it('strips Vietnamese diacritics from "Công Ty TNHH"', () => {
      expect(generateSuggestedSlug('Công Ty TNHH')).to.equal('cong-ty-tnhh');
    });

    it('converts spaces to hyphens', () => {
      const slug = generateSuggestedSlug('FPT Software');
      expect(slug).to.not.include(' ');
      expect(slug).to.include('-');
    });

    it('produces fully lowercase output', () => {
      const slug = generateSuggestedSlug('MyCompanyName');
      expect(slug).to.equal(slug.toLowerCase());
    });

    it('pads short names to minimum 4 characters', () => {
      expect(generateSuggestedSlug('AB').length).to.be.at.least(4);
    });

    it('truncates long names to maximum 32 characters', () => {
      const slug = generateSuggestedSlug('A'.repeat(100));
      expect(slug.length).to.be.at.most(32);
    });

    it('returns exactly 4 characters for empty string input', () => {
      expect(generateSuggestedSlug('').length).to.equal(4);
    });

    it('handles đ (Vietnamese d-stroke) — removes diacritic', () => {
      const slug = generateSuggestedSlug('Đặng Văn An');
      expect(slug).to.not.include('đ');
      expect(slug).to.not.include('Đ');
    });

    it('collapses multiple spaces into single hyphens', () => {
      const slug = generateSuggestedSlug('FPT   Software   Ltd');
      expect(slug).to.not.match(/--+/);
    });

    it('strips special characters that are not alphanumeric or hyphens', () => {
      const slug = generateSuggestedSlug('FPT & Software (Ltd.)');
      expect(slug).to.match(/^[a-z0-9-]+$/);
    });
  });
});
