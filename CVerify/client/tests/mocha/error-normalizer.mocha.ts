/**
 * Black-box behavioral tests — API error normalizer (Mocha + Chai).
 * Uses plain objects with isAxiosError: true instead of Jest mocks.
 * axios.isAxiosError() checks payload.isAxiosError === true, so no mocking needed.
 */
import { expect } from 'chai';
import { normalizeError } from '../../src/infrastructure/http/error-normalizer';

describe('normalizeError (black-box)', () => {
  describe('pre-normalized ApiError passthrough', () => {
    it('returns an existing ApiError object unchanged (same reference)', () => {
      const existing = {
        contractVersion: '1.0.0',
        status: 400,
        code: 'TEST_CODE',
        category: 'VALIDATION',
        severity: 'Error' as const,
        messageKey: 'test.key',
        message: 'test message',
        retryable: false,
        timestamp: new Date().toISOString(),
        uxSemantics: {
          displayMode: 'Toast' as const,
          resolutionStrategy: 'None' as const,
          userAction: '',
          targetPath: '',
        },
      };
      expect(normalizeError(existing)).to.equal(existing);
    });
  });

  describe('HTTP 400 validation error', () => {
    const validationError = {
      isAxiosError: true,
      response: {
        status: 400,
        data: {
          errors: { Email: ['Invalid format'], Password: ['Too short'] },
          detail: 'Validation failed',
        },
        headers: {},
      },
      message: 'Bad Request',
    };

    it('maps to code "VALIDATION_ERROR"', () => {
      expect(normalizeError(validationError).code).to.equal('VALIDATION_ERROR');
    });

    it('normalizes PascalCase "Email" key to camelCase "email"', () => {
      expect(normalizeError(validationError).errors?.email).to.deep.equal(['Invalid format']);
    });

    it('normalizes PascalCase "Password" key to camelCase "password"', () => {
      expect(normalizeError(validationError).errors?.password).to.deep.equal(['Too short']);
    });

    it('sets displayMode to "Inline" for validation errors', () => {
      expect(normalizeError(validationError).uxSemantics.displayMode).to.equal('Inline');
    });

    it('sets status to 400', () => {
      expect(normalizeError(validationError).status).to.equal(400);
    });
  });

  describe('HTTP 429 rate limit', () => {
    const rateError = {
      isAxiosError: true,
      response: {
        status: 429,
        headers: { 'retry-after': '120' },
        data: undefined,
      },
      message: 'Too Many Requests',
    };

    it('maps to code "RATE_LIMIT_EXCEEDED"', () => {
      expect(normalizeError(rateError).code).to.equal('RATE_LIMIT_EXCEEDED');
    });

    it('sets retryable to true', () => {
      expect(normalizeError(rateError).retryable).to.be.true;
    });

    it('extracts cooldownSeconds from retry-after header', () => {
      expect(normalizeError(rateError).cooldownSeconds).to.equal(120);
    });

    it('sets status to 429', () => {
      expect(normalizeError(rateError).status).to.equal(429);
    });
  });

  describe('network timeout (ECONNABORTED)', () => {
    const timeoutError = {
      isAxiosError: true,
      code: 'ECONNABORTED',
      message: 'timeout of 30000ms exceeded',
    };

    it('maps to code "NETWORK_TIMEOUT"', () => {
      expect(normalizeError(timeoutError).code).to.equal('NETWORK_TIMEOUT');
    });

    it('sets retryable to true', () => {
      expect(normalizeError(timeoutError).retryable).to.be.true;
    });

    it('sets status to 408', () => {
      expect(normalizeError(timeoutError).status).to.equal(408);
    });
  });

  describe('generic Error instance', () => {
    it('maps to code "UNKNOWN_ERROR"', () => {
      expect(normalizeError(new Error('boom'))).to.have.property('code', 'UNKNOWN_ERROR');
    });

    it('preserves the original error message', () => {
      expect(normalizeError(new Error('something broke')).message).to.equal('something broke');
    });

    it('sets retryable to false', () => {
      expect(normalizeError(new Error('boom')).retryable).to.be.false;
    });

    it('sets status to 500', () => {
      expect(normalizeError(new Error('boom')).status).to.equal(500);
    });
  });

  describe('unknown primitives / null', () => {
    it('null → code "UNKNOWN_ERROR"', () => {
      expect(normalizeError(null).code).to.equal('UNKNOWN_ERROR');
    });

    it('null → status 500', () => {
      expect(normalizeError(null).status).to.equal(500);
    });

    it('undefined → code "UNKNOWN_ERROR"', () => {
      expect(normalizeError(undefined).code).to.equal('UNKNOWN_ERROR');
    });

    it('number primitive → code "UNKNOWN_ERROR"', () => {
      expect(normalizeError(42).code).to.equal('UNKNOWN_ERROR');
    });
  });

  describe('response structure invariants', () => {
    it('every normalized error has contractVersion', () => {
      const result = normalizeError(new Error('test'));
      expect(result.contractVersion).to.be.a('string').and.to.not.be.empty;
    });

    it('every normalized error has a timestamp', () => {
      const result = normalizeError(null);
      expect(result.timestamp).to.be.a('string').and.to.not.be.empty;
    });

    it('every normalized error has uxSemantics with displayMode', () => {
      const result = normalizeError(null);
      expect(result.uxSemantics).to.have.property('displayMode');
    });
  });
});
