"use client";

import { useAuthStore } from '../store/use-auth-store';
import { authApi, LoginPayload, RegisterPayload, ResetPasswordPayload } from '../services/auth.service';
import { User, UserRole, ResourceActionPermission } from '../../../types/auth.types';
import { useState } from 'react';
import { normalizeError } from '../../../services/axios-client';
import { normalizeRole } from '../../../lib/utils/auth-utils';

// Shared module-level bootstrap promise to deduplicate parallel mounts during app initialization
let bootstrapPromise: Promise<{ authenticated: boolean; user: User | null }> | null = null;

export const useAuth = () => {
  const store = useAuthStore();
  const [authError, setAuthError] = useState<string | null>(null);
  
  // Wrapper for Login operation
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

      const user: User = {
        id: response.id,
        email: response.email,
        fullName: response.fullName,
        avatarUrl: response.avatarUrl,
        role: normalizeRole(response.roles),
        permissions: response.permissions,
        isEmailVerified: response.isEmailVerified,
      };

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

  // Wrapper for Google Sign-in operation
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

      const user: User = {
        id: response.id,
        email: response.email,
        fullName: response.fullName,
        avatarUrl: response.avatarUrl,
        role: normalizeRole(response.roles),
        permissions: response.permissions,
        isEmailVerified: response.isEmailVerified,
      };

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

  // Wrapper for Registration operation
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

  // Wrapper for Email Verification operation
  const verifyEmailUser = async (token: string) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.verifyEmail(token);
      
      const user: User = {
        id: response.id,
        email: response.email,
        fullName: response.fullName,
        avatarUrl: response.avatarUrl,
        role: normalizeRole(response.roles),
        permissions: response.permissions,
        isEmailVerified: response.isEmailVerified,
      };

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

  // Wrapper for Password Reset operation
  const resetPasswordUser = async (payload: ResetPasswordPayload) => {
    store.setLoading(true);
    setAuthError(null);
    try {
      const response = await authApi.resetPassword(payload);
      
      const user: User = {
        id: response.id,
        email: response.email,
        fullName: response.fullName,
        avatarUrl: response.avatarUrl,
        role: normalizeRole(response.roles),
        permissions: response.permissions,
        isEmailVerified: response.isEmailVerified,
      };

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

  // Wrapper for Logout operation (calls API then clears state)
  const logoutUser = async (broadcast = true) => {
    store.setLoading(true);
    try {
      await authApi.logout();
    } catch (err) {
      console.warn('[Session System] Invalidation request on server failed or bypassed:', err);
    } finally {
      store.logout(broadcast);
    }
  };

  // Bootstraps local profile on app boot or token refresh, locking concurrent parallel calls
  const initializeUserSession = async () => {
    // If already READY, return cached session
    if (store.bootstrapState === 'READY') {
      return { authenticated: store.isAuthenticated, user: store.user };
    }
    
    // If already running (lock active), wait on the promise or return current state
    if (store.bootstrapState === 'BOOTSTRAPPING' || store.bootstrapState === 'VALIDATING') {
      if (bootstrapPromise) {
        return bootstrapPromise;
      }
      return { authenticated: store.isAuthenticated, user: store.user };
    }

    // Acquire lock and transition to bootstrapping
    store.setBootstrapState('BOOTSTRAPPING');

    bootstrapPromise = (async () => {
      store.setBootstrapState('VALIDATING');
      store.setLoading(true);
      console.log('[Auth System] Session bootstrap validation started.');
      try {
        const response = await authApi.fetchMe();
        
        if (response.status === 'EMAIL_VERIFY_PENDING' || response.nextStep === 'VERIFY_EMAIL') {
          store.setPendingVerificationEmail(response.email);
          store.setAuthStatusAndNextStep(response.status, response.nextStep);
          store.logout(false);
          console.log('[Auth System] Session bootstrap complete. Status: EMAIL_VERIFY_PENDING');
          return { authenticated: false, user: null, isUnverified: true, nextStep: response.nextStep };
        }

        const user: User = {
          id: response.id,
          email: response.email,
          fullName: response.fullName,
          avatarUrl: response.avatarUrl,
          role: normalizeRole(response.roles),
          permissions: response.permissions,
          isEmailVerified: response.isEmailVerified,
        };

        store.login(user);
        store.setAuthStatusAndNextStep(response.status, response.nextStep);
        console.log(`[Auth System] Session bootstrap complete. User authenticated. Role: ${user.role}`);
        return { authenticated: true, user };
      } catch (err) {
        console.warn('[Auth System] Session bootstrap validation failed. Cleaning local session.', err);
        store.logout(false);
        return { authenticated: false, user: null };
      } finally {
        store.setInitialized(true);
        store.setBootstrapState('READY');
        store.setLoading(false);
        bootstrapPromise = null;
      }
    })();

    return bootstrapPromise;
  };

  return {
    // Zustand States
    user: store.user,
    isAuthenticated: store.isAuthenticated,
    isLoading: store.isLoading,
    isInitialized: store.isInitialized,
    bootstrapState: store.bootstrapState,
    authError,
    
    // Auth Actions
    login: loginUser,
    loginWithGoogle: loginUserWithGoogle,
    register: registerUser,
    logout: logoutUser,
    verifyEmail: verifyEmailUser,
    resetPassword: resetPasswordUser,
    initializeSession: initializeUserSession,
    updateProfile: store.updateUser,

    // Guards Facades
    hasRole: (role: UserRole) => store.hasRole(role),
    hasPermission: (permission: ResourceActionPermission) => store.hasPermission(permission),
  };
};
