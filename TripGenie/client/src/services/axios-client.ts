import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { useAuthStore } from '../features/auth/store/use-auth-store';
import { ApiError } from '../types/auth.types';
import { AUTH_KEYS } from '../lib/constants';

// Helper to extract a cookie value on the client side
export function getCookie(name: string): string | undefined {
  if (typeof document === 'undefined') return undefined;
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) {
    return parts.pop()?.split(';').shift();
  }
  return undefined;
}

// Helper to set a cookie value on the client side
export function setCookie(name: string, value: string, maxAgeSeconds: number = 31536000) {
  if (typeof document === 'undefined') return;
  document.cookie = `${name}=${value}; path=/; max-age=${maxAgeSeconds}; SameSite=Lax; Secure`;
}

// Configured base URL for API, using standard environment settings
const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

export const axiosClient: AxiosInstance = axios.create({
  baseURL: API_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
    'X-Requested-With': 'XMLHttpRequest', // Standard indicator for AJAX requests
  },
  withCredentials: true, // Crucial for HttpOnly cookies (session + refresh)
});

// CSRF Request Interceptor
axiosClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Inject CSRF protection token for state-mutating requests (POST, PUT, DELETE, PATCH)
    const method = config.method?.toUpperCase();
    if (method && ['POST', 'PUT', 'DELETE', 'PATCH'].includes(method)) {
      const csrfToken = getCookie(AUTH_KEYS.CSRF_COOKIE);
      if (csrfToken) {
        config.headers.set(AUTH_KEYS.CSRF_HEADER, csrfToken);
      }
    }
    return config;
  },
  (error: unknown) => Promise.reject(error)
);

// Single cached refresh promise to prevent concurrent token rotation under heavy traffic
let refreshPromise: Promise<unknown> | null = null;

// Response Interceptor
axiosClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };
    
    // Normalize our API error response structure
    const apiError = normalizeError(error);

    // 1. Session Expiry & Token Rotation Handler (401 Unauthorized)
    if (error.response?.status === 401 && !originalRequest._retry) {
      const isRefreshRequest = originalRequest.url?.includes('/auth/refresh-token');
      const isLogoutRequest = originalRequest.url?.includes('/auth/logout');
      const isAuthFlowRequest = originalRequest.url?.includes('/auth/') && !originalRequest.url?.includes('/auth/me');
      
      // If any auth flow request failed with a 401, do not attempt to refresh
      if (isAuthFlowRequest) {
        refreshPromise = null;
        if (isRefreshRequest || isLogoutRequest) {
          useAuthStore.getState().logout(true); // logout and broadcast to all tabs
        }
        return Promise.reject(apiError);
      }

      originalRequest._retry = true;

      try {
        // Enforce a single active refresh token rotation promise for all concurrent 401 failures
        if (!refreshPromise) {
          refreshPromise = axiosClient.post('/auth/refresh-token').finally(() => {
            refreshPromise = null;
          });
        }
        
        await refreshPromise;
        return axiosClient(originalRequest); // Retry the original failed request
      } catch (refreshErr) {
        refreshPromise = null;
        // Clear auth store and force redirects to login
        useAuthStore.getState().logout(true);
        
        if (typeof window !== 'undefined') {
          const currentPath = window.location.pathname;
          const isProtectedPage = ['/admin', '/business', '/user'].some(p => currentPath.startsWith(p));
          
          if (isProtectedPage) {
            window.location.href = `/login?session_expired=true&callbackUrl=${encodeURIComponent(
              window.location.pathname + window.location.search
            )}`;
          }
        }
        
        return Promise.reject(normalizeError(refreshErr));
      }
    }

    // 2. Access Gated (403 Forbidden)
    if (error.response?.status === 403) {
      console.warn('[Security Alert] 403 Forbidden - Insufficient Permissions');
    }

    return Promise.reject(apiError);
  }
);

/**
 * Normalizes all kinds of HTTP and network exceptions into a standardized ApiError.
 * Properly parses C# ProblemDetails, extensions.code, and lowercases validation dictionaries.
 */
export function normalizeError(error: unknown): ApiError {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as Record<string, unknown> | undefined;

    if (data && typeof data === 'object') {
      // 1. Map validation dictionaries from C# ProblemDetails (key: [messages]) to camelCase
      const normalizedErrors: Record<string, string[]> = {};
      let isValidationError = false;
      
      const errors = data.errors as Record<string, unknown> | undefined;
      if (errors && typeof errors === 'object') {
        isValidationError = true;
        Object.entries(errors).forEach(([key, value]) => {
          // Convert PascalCase C# properties (e.g. Email) to camelCase (e.g. email)
          const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
          normalizedErrors[camelKey] = Array.isArray(value) 
            ? (value as unknown[]).map(String) 
            : [String(value)];
        });
      }

      // 2. Extract detailed message
      const mainMessage = (data.detail as string) || 
                          (data.message as string) || 
                          (data.title as string) || 
                          error.message || 
                          'An unexpected error occurred.';

      // 3. Extract custom error code from extensions.code or direct property
      const extensions = data.extensions as Record<string, unknown> | undefined;
      const customCode = (extensions?.code as string) || 
                         (data.code as string) || 
                         (isValidationError ? 'VALIDATION_ERROR' : 'UNKNOWN_ERROR');

      return {
        code: customCode,
        message: mainMessage,
        errors: isValidationError ? normalizedErrors : undefined,
        remainingAttempts: typeof data.remainingAttempts === 'number' ? data.remainingAttempts : undefined,
        cooldownSeconds: typeof data.cooldownSeconds === 'number' ? data.cooldownSeconds : undefined,
      };
    }

    // Handle Rate Limiter responses (429 Too Many Requests)
    if (error.response?.status === 429) {
      const retryAfter = error.response.headers['retry-after'];
      const cooldown = retryAfter ? parseInt(retryAfter, 10) : 60;
      return {
        code: 'RATE_LIMIT_EXCEEDED',
        message: 'Too many requests. Please slow down and try again later.',
        cooldownSeconds: cooldown,
      };
    }

    // Handle Timeout
    if (error.code === 'ECONNABORTED') {
      return {
        code: 'NETWORK_TIMEOUT',
        message: 'Request timed out. Please check your network connection.',
      };
    }

    // General fallback
    return {
      code: 'NETWORK_ERROR',
      message: error.message || 'Unable to connect to the server.',
    };
  }

  // If it's a generic Error instance
  if (error instanceof Error) {
    return {
      code: 'UNKNOWN_ERROR',
      message: error.message,
    };
  }

  // Fallback
  return {
    code: 'UNKNOWN_ERROR',
    message: 'An unexpected error occurred.',
  };
}
