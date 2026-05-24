"use client";

import { useAuthStore } from '../store/use-auth-store';
import { authApi, LoginPayload, CompanyLoginPayload } from '@/features/auth/services/auth.service';
import { useState } from 'react';
import { normalizeError } from '@/services/axios-client';
import { mapLoginResponse } from '@/modules/auth/hooks/use-auth-helpers';

/**
 * Focused hook for login operations (email/password, Google, company).
 */
export const useLogin = () => {
  const store = useAuthStore();
  const [authError, setAuthError] = useState<string | null>(null);

  const loginUser = async (credentials: LoginPayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.login(credentials);

      if (response.status === 'EMAIL_VERIFY_PENDING' || response.nextStep === 'VERIFY_EMAIL') {
        store.setPendingVerificationEmail(response.email);
        store.setAuthStatusAndNextStep(response.status, response.nextStep);
        store.setLoading(false);
        return { success: true, isUnverified: true, nextStep: response.nextStep, email: response.email };
      }

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

  const loginUserWithGoogle = async (idToken: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.loginWithGoogle(idToken);

      if (response.status === 'EMAIL_VERIFY_PENDING' || response.nextStep === 'VERIFY_EMAIL') {
        store.setPendingVerificationEmail(response.email);
        store.setAuthStatusAndNextStep(response.status, response.nextStep);
        store.setLoading(false);
        return { success: true, isUnverified: true, nextStep: response.nextStep, email: response.email };
      }

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

  const companyLogin = async (payload: CompanyLoginPayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.companyLogin(payload);
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
    login: loginUser,
    loginWithGoogle: loginUserWithGoogle,
    companyLogin,
  };
};
