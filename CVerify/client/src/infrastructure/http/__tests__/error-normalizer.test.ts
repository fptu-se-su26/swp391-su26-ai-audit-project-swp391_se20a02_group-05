/**
 * White-box unit tests — API error normalization branches.
 */
import axios, { AxiosError } from 'axios';
import { normalizeError } from '../error-normalizer';
import type { ApiError } from '@/types/api.types';

describe('normalizeError (white-box)', () => {
  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('returns existing ApiError unchanged', () => {
    const existing: ApiError = {
      contractVersion: '1.0.0',
      status: 400,
      code: 'TEST',
      category: 'VALIDATION',
      severity: 'Error',
      messageKey: 'test',
      message: 'msg',
      retryable: false,
      timestamp: new Date().toISOString(),
      uxSemantics: {
        displayMode: 'Toast',
        resolutionStrategy: 'None',
        userAction: '',
        targetPath: '',
      },
    };
    expect(normalizeError(existing)).toBe(existing);
  });

  it('maps validation error payload with PascalCase keys', () => {
    const error = {
      isAxiosError: true,
      response: {
        status: 400,
        data: {
          errors: { Email: ['Invalid format'] },
          detail: 'Validation failed',
        },
        headers: {},
      },
      message: 'Bad Request',
    } as AxiosError;

    jest.spyOn(axios, 'isAxiosError').mockReturnValue(true);
    const result = normalizeError(error);
    expect(result.code).toBe('VALIDATION_ERROR');
    expect(result.errors?.email).toEqual(['Invalid format']);
    expect(result.uxSemantics.displayMode).toBe('Inline');
  });

  it('maps 429 rate limit with retry-after header', () => {
    const error = {
      isAxiosError: true,
      response: {
        status: 429,
        headers: { 'retry-after': '120' },
        data: undefined,
      },
      message: 'Too Many Requests',
    } as AxiosError;

    jest.spyOn(axios, 'isAxiosError').mockReturnValue(true);
    const result = normalizeError(error);
    expect(result.code).toBe('RATE_LIMIT_EXCEEDED');
    expect(result.cooldownSeconds).toBe(120);
    expect(result.retryable).toBe(true);
  });

  it('maps network timeout (ECONNABORTED)', () => {
    const error = {
      isAxiosError: true,
      code: 'ECONNABORTED',
      message: 'timeout',
    } as AxiosError;

    jest.spyOn(axios, 'isAxiosError').mockReturnValue(true);
    const result = normalizeError(error);
    expect(result.code).toBe('NETWORK_TIMEOUT');
    expect(result.retryable).toBe(true);
  });

  it('maps generic Error instance', () => {
    jest.spyOn(axios, 'isAxiosError').mockReturnValue(false);
    const result = normalizeError(new Error('boom'));
    expect(result.code).toBe('UNKNOWN_ERROR');
    expect(result.message).toBe('boom');
  });

  it('maps unknown primitives', () => {
    jest.spyOn(axios, 'isAxiosError').mockReturnValue(false);
    const result = normalizeError(null);
    expect(result.status).toBe(500);
    expect(result.code).toBe('UNKNOWN_ERROR');
  });
});
