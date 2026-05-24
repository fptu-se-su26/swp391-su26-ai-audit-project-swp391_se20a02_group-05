"use client";

import { useAuthStore } from '../store/use-auth-store';
import { authApi } from '@/features/auth/services/auth.service';
import { useState } from 'react';
import { normalizeError } from '@/services/axios-client';

/**
 * Focused hook for OTP and email identity resolution operations.
 */
export const useOtp = () => {
  const store = useAuthStore();
  const [authError, setAuthError] = useState<string | null>(null);

  const sendOtp = async (email: string, purpose: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.sendOtp(email, purpose);
      store.setLoading(false);
      return { success: true, data: response };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  const resolveEmailAuthState = async (email: string) => {
    setAuthError(null);
    try {
      const response = await authApi.resolveEmailAuthState(email);
      return { success: true as const, data: response };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      return { success: false as const, error: parsedError };
    }
  };

  const verifyOtp = async (challengeId: string, email: string, code: string, purpose: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.verifyOtp(challengeId, email, code, purpose);
      store.setLoading(false);
      return { success: true, data: response };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  return {
    authError,
    sendOtp,
    resolveEmailAuthState,
    verifyOtp,
  };
};
