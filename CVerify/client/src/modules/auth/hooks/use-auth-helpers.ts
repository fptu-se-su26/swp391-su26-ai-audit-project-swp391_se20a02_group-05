import { LoginResponseData, User } from '@/types/auth.types';
import { normalizeRole } from '@/lib/utils/auth-utils';

/**
 * Maps a raw login/auth API response to the domain User model.
 * Shared across all auth hooks that receive LoginResponseData.
 */
export function mapLoginResponse(response: LoginResponseData): User {
  return {
    id: response.id,
    email: response.email,
    fullName: response.fullName,
    avatarUrl: response.avatarUrl,
    role: normalizeRole(response.roles),
    permissions: response.permissions,
    isEmailVerified: response.isEmailVerified,
  };
}
