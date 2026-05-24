"use client";

import { useAuthStore } from '../store/use-auth-store';
import { authApi, ResetPasswordPayload, CreatePasswordPayload } from '@/features/auth/services/auth.service';
import { useState } from 'react';
import { normalizeError } from '@/services/axios-client';
import { mapLoginResponse } from './use-auth-helpers';

/**
 * Focused hook for password reset and creation operations.
 */
export const usePasswordReset = () => {
  const store = useAuthStore();
  const [authError, setAuthError] = useState<string | null>(null);

  const resetPasswordUser = async (payload: ResetPasswordPayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.resetPassword(payload);
      const user = mapLoginResponse(response);
      store.login(user);
      store.setAuthStatusAndNextStep(response.status, response.nextStep);
      store.setLoading(false);
      return { success: true, user, nextStep: response.nextStep };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  const createPassword = async (payload: CreatePasswordPayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.createPassword(payload);
      const user = mapLoginResponse(response);
      store.login(user);
      store.setAuthStatusAndNextStep(response.status, response.nextStep);
      return { success: true, user, nextStep: response.nextStep };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  return {
    authError,
    resetPassword: resetPasswordUser,
    createPassword,
  };
};
