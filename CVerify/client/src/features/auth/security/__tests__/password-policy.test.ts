/**
 * White-box unit tests — password policy evaluator (branch & boundary coverage).
 */
import {
  evaluatePasswordStrength,
  passwordPoliciesRegistry,
  rulesRegistry,
  SPECIAL_CHARACTER_REGEX,
} from '../password-policy';

describe('password-policy (white-box)', () => {
  describe('rulesRegistry', () => {
    it.each([
      ['Abcdef1!', true],
      ['abcdef1!', false],
    ])('uppercase rule for "%s" => %s', (password, expectUpper) => {
      expect(rulesRegistry.uppercase.test(password)).toBe(expectUpper);
    });

    it('accepts backend-aligned special characters', () => {
      expect(SPECIAL_CHARACTER_REGEX.test('Aa1@')).toBe(true);
      expect(SPECIAL_CHARACTER_REGEX.test('Aa1_')).toBe(true);
    });
  });

  describe('evaluatePasswordStrength — default policy', () => {
    it('empty password → weak, 0%, all rules failed', () => {
      const result = evaluatePasswordStrength('');
      expect(result.level).toBe('weak');
      expect(result.percentage).toBe(0);
      expect(result.passedRules).toHaveLength(0);
      expect(result.failedRules).toHaveLength(passwordPoliciesRegistry.default.rules.length);
    });

    it('unknown policy id falls back to default', () => {
      const result = evaluatePasswordStrength('Aa1!aaaa', 'nonexistent-policy');
      expect(result.maxScore).toBe(passwordPoliciesRegistry.default.rules.length);
    });

    it('fully compliant password → excellent', () => {
      const result = evaluatePasswordStrength('Secure1!aa');
      expect(result.level).toBe('excellent');
      expect(result.percentage).toBe(100);
      expect(result.failedRules).toHaveLength(0);
    });

    it('partial compliance → fair or strong', () => {
      const result = evaluatePasswordStrength('abcdefgh');
      expect(['fair', 'weak', 'strong']).toContain(result.level);
      expect(result.percentage).toBeGreaterThan(0);
      expect(result.percentage).toBeLessThan(100);
    });

    it('boundary: 7 chars fails length rule', () => {
      const result = evaluatePasswordStrength('Aa1!aaa');
      expect(result.passedRules.map((r) => r.id)).not.toContain('length');
    });

    it('boundary: exactly 8 chars passes length', () => {
      const result = evaluatePasswordStrength('Aa1!aaaa');
      const passedIds = result.passedRules.map((r) => r.id);
      expect(passedIds).toContain('length');
    });
  });

  describe('evaluatePasswordStrength — enterprise policy', () => {
    it('11 chars fails enterprise length', () => {
      const result = evaluatePasswordStrength('Aa1!aaaaaa', 'enterprise');
      expect(result.passedRules.map((r) => r.id)).not.toContain('length');
    });

    it('12+ chars with full complexity → excellent', () => {
      const result = evaluatePasswordStrength('Enterprise1!x', 'enterprise');
      expect(result.level).toBe('excellent');
      expect(result.percentage).toBe(100);
    });
  });

  describe('edge cases', () => {
    it('unicode-only password fails ASCII rules', () => {
      const result = evaluatePasswordStrength('ĐăngNgữ');
      expect(result.level).not.toBe('excellent');
    });

    it('whitespace-only treated as non-empty but fails rules', () => {
      const result = evaluatePasswordStrength('        ');
      expect(result.percentage).toBeLessThan(100);
    });
  });
});
