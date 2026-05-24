"use client";

import { useAuthStore } from '../store/use-auth-store';
import { authApi, SetupWorkspacePayload } from '@/features/auth/services/auth.service';
import { useState } from 'react';
import { normalizeError } from '@/services/axios-client';
import { mapLoginResponse } from './use-auth-helpers';

/**
 * Focused hook for the 3-step company onboarding flow + workspace setup.
 */
export const useOnboarding = () => {
  const store = useAuthStore();
  const [authError, setAuthError] = useState<string | null>(null);

  const verifyCompanyOnboarding = async (companyName: string, taxCode: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.verifyCompanyOnboarding(companyName, taxCode);
      store.setLoading(false);
      return { success: true, data: response };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  const verifyOnboardingOtp = async (challengeId: string, email: string, code: string, step1Token: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.verifyOnboardingOtp(challengeId, email, code, step1Token);
      store.setLoading(false);
      return { success: true, data: response };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  const verifyOnboardingGoogle = async (idToken: string, step1Token: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.verifyOnboardingGoogle(idToken, step1Token);
      store.setLoading(false);
      return { success: true, data: response };
    } catch (err: unknown) {
      const parsedError = normalizeError(err);
      setAuthError(parsedError.message);
      store.setLoading(false);
      return { success: false, error: parsedError };
    }
  };

  const completeOnboarding = async (
    payload: {
      step2Token: string;
      organizationUsername: string;
      password: string;
      confirmPassword: string;
      companyDisplayName: string;
    },
    idempotencyKey: string
  ) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.completeOnboarding(payload, idempotencyKey);
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

  const setupWorkspace = async (payload: SetupWorkspacePayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.setupWorkspace(payload);
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
    verifyCompanyOnboarding,
    verifyOnboardingOtp,
    verifyOnboardingGoogle,
    completeOnboarding,
    setupWorkspace,
  };
};
