import axios, { AxiosError } from 'axios';
import { ApiError, UxSemantics } from '@/shared/types/api.types';

/**
 * Normalizes all HTTP, network, and unhandled exceptions into a standardized ApiError contract.
 * Preserves trace correlation IDs, versions, and UX semantics while remaining backward-compatible.
 */
export function normalizeError(error: unknown): ApiError {
  // If the error is already a normalized ApiError, return it directly
  if (error && typeof error === 'object' && 'code' in error && 'uxSemantics' in error) {
    return error as ApiError;
  }

  const defaultUxSemantics: UxSemantics = {
    displayMode: 'Toast',
    resolutionStrategy: 'None',
    userAction: '',
    targetPath: '',
  };

  if (axios.isAxiosError(error)) {
    const axiosErr = error as AxiosError;
    const data = axiosErr.response?.data as Record<string, unknown> | undefined;

    if (data && typeof data === 'object') {
      // 1. Extract validation dictionary if present
      const normalizedErrors: Record<string, string[]> = {};
      let isValidationError = false;
      
      const errors = data.errors as Record<string, unknown> | undefined;
      if (errors && typeof errors === 'object') {
        isValidationError = true;
        Object.entries(errors).forEach(([key, value]) => {
          // Normalize PascalCase properties to camelCase or keep path structure
          const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
          normalizedErrors[camelKey] = Array.isArray(value) 
            ? (value as unknown[]).map(String) 
            : [String(value)];
        });
      }

      // 2. Map standard versioned error structure
      const contractVersion = data.contractVersion || '1.0.0';
      const status = typeof data.status === 'number' ? data.status : (axiosErr.response?.status || 500);
      const code = data.code || (isValidationError ? 'VALIDATION_ERROR' : 'UNKNOWN_ERROR');
      const category = data.category || (isValidationError ? 'VALIDATION' : 'UNKNOWN');
      const severity = data.severity || 'Error';
      
      // The backend-provided localization key, or fallback to system category matching
      const messageKey = data.messageKey || `system.toast.error.${code.toLowerCase()}`;
      
      const mainMessage = data.detail || 
                          data.message || 
                          data.title || 
                          axiosErr.message || 
                          'An unexpected error occurred.';

      const retryable = typeof data.retryable === 'boolean' 
        ? data.retryable 
        : (status >= 500 || axiosErr.code === 'ECONNABORTED');

      const correlationId = data.correlationId || 
                            axiosErr.response?.headers['x-correlation-id'] || 
                            '';

      const timestamp = data.timestamp || new Date().toISOString();

      const uxSemantics: UxSemantics = data.uxSemantics 
        ? {
            displayMode: data.uxSemantics.displayMode || 'Toast',
            resolutionStrategy: data.uxSemantics.resolutionStrategy || 'None',
            userAction: data.uxSemantics.userAction || '',
            targetPath: data.uxSemantics.targetPath || '',
          }
        : {
            ...defaultUxSemantics,
            displayMode: isValidationError ? 'Inline' : 'Toast',
            resolutionStrategy: status === 401 ? 'Redirect' : 'None',
            targetPath: status === 401 ? '/login' : '',
          };

      // Extrapolate other details dictionary attributes safely
      const details: Record<string, unknown> = {};
      Object.keys(data).forEach((key) => {
        const skippedKeys = [
          'contractVersion', 'status', 'code', 'category', 'severity', 
          'messageKey', 'message', 'retryable', 'errors', 'correlationId', 
          'timestamp', 'uxSemantics', 'detail', 'title'
        ];
        if (!skippedKeys.includes(key)) {
          details[key] = data[key];
        }
      });

      return {
        contractVersion,
        status,
        code,
        category,
        severity,
        messageKey,
        message: mainMessage,
        retryable,
        errors: isValidationError ? normalizedErrors : undefined,
        correlationId,
        timestamp,
        uxSemantics,
        details,
        // Backward compatibility
        remainingAttempts: typeof data.remainingAttempts === 'number' ? data.remainingAttempts : details.remainingAttempts,
        cooldownSeconds: typeof data.cooldownSeconds === 'number' ? data.cooldownSeconds : details.cooldownSeconds,
      };
    }

    // Handle Rate Limiter responses (429 Too Many Requests)
    if (axiosErr.response?.status === 429) {
      const retryAfter = axiosErr.response.headers['retry-after'];
      const cooldown = retryAfter ? parseInt(retryAfter as string, 10) : 60;
      return {
        contractVersion: '1.0.0',
        status: 429,
        code: 'RATE_LIMIT_EXCEEDED',
        category: 'INFRASTRUCTURE',
        severity: 'Warning',
        messageKey: 'system.toast.error.rate_limited',
        message: 'Too many requests. Please slow down and try again later.',
        retryable: true,
        correlationId: axiosErr.response.headers['x-correlation-id'] || '',
        timestamp: new Date().toISOString(),
        uxSemantics: {
          displayMode: 'Toast',
          resolutionStrategy: 'Retry',
          userAction: '',
          targetPath: '',
        },
        cooldownSeconds: cooldown,
      };
    }

    // Handle Timeout
    if (axiosErr.code === 'ECONNABORTED') {
      return {
        contractVersion: '1.0.0',
        status: 408,
        code: 'NETWORK_TIMEOUT',
        category: 'NETWORK',
        severity: 'Error',
        messageKey: 'system.toast.error.network_timeout',
        message: 'Request timed out. Please check your network connection.',
        retryable: true,
        timestamp: new Date().toISOString(),
        uxSemantics: {
          displayMode: 'Toast',
          resolutionStrategy: 'Retry',
          userAction: '',
          targetPath: '',
        },
      };
    }

    // General fallback Axios network error
    return {
      contractVersion: '1.0.0',
      status: axiosErr.response?.status || 500,
      code: 'NETWORK_ERROR',
      category: 'NETWORK',
      severity: 'Error',
      messageKey: 'system.toast.error.unexpected',
      message: axiosErr.message || 'Unable to connect to the server.',
      retryable: true,
      timestamp: new Date().toISOString(),
      uxSemantics: defaultUxSemantics,
    };
  }

  // If it's a generic Error instance
  if (error instanceof Error) {
    return {
      contractVersion: '1.0.0',
      status: 500,
      code: 'UNKNOWN_ERROR',
      category: 'UNKNOWN',
      severity: 'Error',
      messageKey: 'system.toast.error.unexpected',
      message: error.message,
      retryable: false,
      timestamp: new Date().toISOString(),
      uxSemantics: defaultUxSemantics,
    };
  }

  // Fallback
  return {
    contractVersion: '1.0.0',
    status: 500,
    code: 'UNKNOWN_ERROR',
    category: 'UNKNOWN',
    severity: 'Error',
    messageKey: 'system.toast.error.unexpected',
    message: 'An unexpected error occurred.',
    retryable: false,
    timestamp: new Date().toISOString(),
    uxSemantics: defaultUxSemantics,
  };
}
