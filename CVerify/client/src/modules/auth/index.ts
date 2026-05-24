// Auth module — public API

// Hooks (monolithic facade — backward compatible)
export { useAuth } from './hooks/use-auth';
export { useSessionTimeout } from './hooks/use-session-timeout';

// Hooks (decomposed — for new feature development)
export { useLogin } from './hooks/use-login';
export { useRegister } from './hooks/use-register';
export { usePasswordReset } from './hooks/use-password-reset';
export { useOtp } from './hooks/use-otp';
export { useOnboarding } from './hooks/use-onboarding';
export { mapLoginResponse } from './hooks/use-auth-helpers';

// Store
export { useAuthStore } from './store/use-auth-store';

// Services
export { authApi } from './services/auth.service';
export { recoveryApi } from './services/recovery.service';

// Guards
export { AuthGuard } from './guards/auth-guard';
export { GuestGuard } from './guards/guest-guard';
export { PermissionGuard } from './guards/permission-guard';
export { RoleGuard } from './guards/role-guard';

// Components
export { AuthOrchestrator } from './components/auth-orchestrator';
export { SessionVerificationScreen } from './components/session-verification-screen';

// Types
export type * from './types/auth.types';

// Schemas
export * from './schemas/auth.schema';
