"use client";

import { useAuthStore } from '../store/use-auth-store';
import { authApi, RegisterPayload, RegisterCompanyPayload } from '@/features/auth/services/auth.service';
import { useState } from 'react';
import { normalizeError } from '@/services/axios-client';

/**
 * Focused hook for registration operations (user, company, company link verification).
 */
export const useRegister = () => {
  const store = useAuthStore();
  const [authError, setAuthError] = useState<string | null>(null);

  const registerUser = async (details: RegisterPayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.register(details);
      store.setLoading(false);
      return {
        success: true,
        message: response.message || 'Registration successful!',
        statusCode: response.statusCode,
        uiAction: response.uiAction,
      };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  const registerCompany = async (payload: RegisterCompanyPayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.registerCompany(payload);
      store.setLoading(false);
      return { success: true, data: response };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  const verifyCompanyLink = async (token: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.verifyCompanyLink(token);
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
    register: registerUser,
    registerCompany,
    verifyCompanyLink,
  };
};
