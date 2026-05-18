export type UserRole = 'USER' | 'BUSINESS' | 'ADMIN';

// Scalable permission string format: 'resource:action' (e.g., 'bookings:create', 'admin:users:manage')
export type ResourceActionPermission = `${string}:${string}`;

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: UserRole;
  permissions: ResourceActionPermission[];
  avatarUrl?: string;
  isEmailVerified?: boolean;
}

export interface AuthSession {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  isInitialized: boolean;
}

// Enterprise standardized API error container
export interface ApiError {
  code: string; // E.g., 'AUTH_INVALID_CREDENTIALS', 'RATE_LIMIT_EXCEEDED', 'ACCOUNT_LOCKED'
  message: string;
  errors?: Record<string, string[]>; // Form validation errors (e.g., email: ['Invalid email format'])
  remainingAttempts?: number; // Lockout mitigation
  cooldownSeconds?: number; // Rate-limit countdown remaining
}

export interface LoginResponseData {
  id: string;
  email: string;
  fullName: string;
  avatarUrl?: string;
  roles: string[];
  permissions: ResourceActionPermission[];
  isEmailVerified: boolean;
  status: string;
  nextStep: string;
}

export interface UserProfileResponseData {
  id: string;
  email: string;
  fullName: string;
  avatarUrl?: string;
  roles: string[];
  permissions: ResourceActionPermission[];
  isEmailVerified: boolean;
  status: string;
  nextStep: string;
}

export interface AuthSuccessResponse {
  message: string;
  statusCode?: string;
  uiAction?: string;
}
