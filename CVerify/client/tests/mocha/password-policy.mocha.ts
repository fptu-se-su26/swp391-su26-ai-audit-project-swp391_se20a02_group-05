/**
 * Black-box behavioral tests — password policy evaluator (Mocha + Chai).
 * Tests only the public contract of evaluatePasswordStrength; no access to internals.
 */
import { expect } from 'chai';
import { evaluatePasswordStrength } from '../../src/features/auth/security/password-policy';

describe('password-policy (black-box)', () => {
  describe('empty and whitespace inputs', () => {
    it('empty string → level "weak"', () => {
      expect(evaluatePasswordStrength('').level).to.equal('weak');
    });

    it('empty string → 0%', () => {
      expect(evaluatePasswordStrength('').percentage).to.equal(0);
    });

    it('empty string → no passed rules', () => {
      expect(evaluatePasswordStrength('').passedRules).to.have.length(0);
    });

    it('whitespace-only password → percentage below 100', () => {
      expect(evaluatePasswordStrength('        ').percentage).to.be.below(100);
    });
  });

  describe('default policy — length boundary', () => {
    it('7-character password does NOT pass the length rule', () => {
      const passedIds = evaluatePasswordStrength('Aa1!aaa').passedRules.map((r) => r.id);
      expect(passedIds).to.not.include('length');
    });

    it('exactly 8 characters DOES pass the length rule', () => {
      const passedIds = evaluatePasswordStrength('Aa1!aaaa').passedRules.map((r) => r.id);
      expect(passedIds).to.include('length');
    });
  });

  describe('default policy — fully compliant password', () => {
    it('"Valid1!pass" achieves level "excellent"', () => {
      expect(evaluatePasswordStrength('Valid1!pass').level).to.equal('excellent');
    });

    it('"Valid1!pass" achieves 100%', () => {
      expect(evaluatePasswordStrength('Valid1!pass').percentage).to.equal(100);
    });

    it('"Valid1!pass" has zero failed rules', () => {
      expect(evaluatePasswordStrength('Valid1!pass').failedRules).to.have.length(0);
    });
  });

  describe('default policy — partial compliance', () => {
    it('lowercase-only "abcdefgh" → not excellent and > 0%', () => {
      const result = evaluatePasswordStrength('abcdefgh');
      expect(result.level).to.not.equal('excellent');
      expect(result.percentage).to.be.greaterThan(0);
    });

    it('missing uppercase "aa1!aaaa" → not excellent', () => {
      expect(evaluatePasswordStrength('aa1!aaaa').level).to.not.equal('excellent');
    });

    it('missing digit "Aa!aaaaa" → has at least one failed rule', () => {
      expect(evaluatePasswordStrength('Aa!aaaaa').failedRules.length).to.be.greaterThan(0);
    });

    it('missing special character "Aa1aaaaa" → not excellent', () => {
      expect(evaluatePasswordStrength('Aa1aaaaa').level).to.not.equal('excellent');
    });
  });

  describe('enterprise policy — length boundary (12 chars)', () => {
    it('11-char "Valid1!shor" does NOT pass enterprise length rule', () => {
      const result = evaluatePasswordStrength('Valid1!shor', 'enterprise');
      expect(result.passedRules.map((r) => r.id)).to.not.include('length');
    });

    it('"Enterprise1!xx" (14 chars, full complexity) → 100%', () => {
      expect(evaluatePasswordStrength('Enterprise1!xx', 'enterprise').percentage).to.equal(100);
    });

    it('"Enterprise1!xx" → level "excellent"', () => {
      expect(evaluatePasswordStrength('Enterprise1!xx', 'enterprise').level).to.equal('excellent');
    });
  });

  describe('unknown policy fallback', () => {
    it('unknown policy id has same maxScore as default', () => {
      const def = evaluatePasswordStrength('Valid1!pass');
      const unk = evaluatePasswordStrength('Valid1!pass', 'nonexistent-policy');
      expect(unk.maxScore).to.equal(def.maxScore);
    });

    it('unknown policy still rates a compliant password as excellent', () => {
      expect(evaluatePasswordStrength('Valid1!pass', 'unknown-xyz').level).to.equal('excellent');
    });
  });

  describe('unicode edge cases', () => {
    it('unicode-only "ĐăngNgữ" is not excellent', () => {
      expect(evaluatePasswordStrength('ĐăngNgữ').level).to.not.equal('excellent');
    });

    it('unicode-only password has at least one failed rule', () => {
      expect(evaluatePasswordStrength('ĐăngNgữ').failedRules.length).to.be.greaterThan(0);
    });
  });
});
