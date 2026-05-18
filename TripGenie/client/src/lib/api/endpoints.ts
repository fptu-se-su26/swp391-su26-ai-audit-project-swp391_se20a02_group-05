import { axiosClient } from './axios-client';
import {
  LoginResponseData,
  UserProfileResponseData,
  AuthSuccessResponse,
} from '../../types/auth.types';
import { z } from 'zod';
import {
  loginSchema,
  registerSchema,
  forgotPasswordSchema,
  resetPasswordSchema,
} from '../validators/auth.validator';

// Derive request payloads from schemas using Zod's infer capabilities
export type LoginPayload = z.infer<typeof loginSchema>;
export type RegisterPayload = z.infer<typeof registerSchema>;
export type ForgotPasswordPayload = z.infer<typeof forgotPasswordSchema>;
export type ResetPasswordPayload = z.infer<typeof resetPasswordSchema> & { token: string };

export const authApi = {
  /**
   * Log in user using email and password
   */
  login: async (payload: LoginPayload): Promise<LoginResponseData> => {
    const response = await axiosClient.post<LoginResponseData>('/auth/login', {
      email: payload.email,
      password: payload.password,
      rememberMe: payload.rememberMe,
    });
    return response.data;
  },

  /**
   * Authenticate with Google ID token
   */
  loginWithGoogle: async (idToken: string): Promise<LoginResponseData> => {
    const response = await axiosClient.post<LoginResponseData>('/auth/google', {
      idToken,
    });
    return response.data;
  },

  /**
   * Register a new traveler or business account
   */
  register: async (payload: RegisterPayload): Promise<AuthSuccessResponse> => {
    const response = await axiosClient.post<AuthSuccessResponse>('/auth/register', {
      fullName: payload.fullName,
      email: payload.email,
      password: payload.password,
      confirmPassword: payload.confirmPassword,
    });
    return response.data;
  },

  /**
   * Securely terminate current session on both client and backend
   */
  logout: async (): Promise<AuthSuccessResponse> => {
    const response = await axiosClient.post<AuthSuccessResponse>('/auth/logout');
    return response.data;
  },

  /**
   * Trigger token refresh manually (silent refresh handles this automatically in interceptors)
   */
  refreshToken: async (): Promise<LoginResponseData> => {
    const response = await axiosClient.post<LoginResponseData>('/auth/refresh-token');
    return response.data;
  },

  /**
   * Retrieve the active user's profile and roles/permissions
   */
  fetchMe: async (): Promise<UserProfileResponseData> => {
    const response = await axiosClient.get<UserProfileResponseData>('/auth/me');
    return response.data;
  },

  /**
   * Trigger forgot password reset email link
   */
  forgotPassword: async (payload: ForgotPasswordPayload): Promise<AuthSuccessResponse> => {
    const response = await axiosClient.post<AuthSuccessResponse>('/auth/forgot-password', payload);
    return response.data;
  },

  /**
   * Reset user password using the cryptographically verified token
   */
  resetPassword: async (payload: ResetPasswordPayload): Promise<LoginResponseData> => {
    const response = await axiosClient.post<LoginResponseData>('/auth/reset-password', {
      token: payload.token,
      password: payload.password,
      confirmPassword: payload.confirmPassword,
    });
    return response.data;
  },

  /**
   * Verify traveler or business partner email with verification token
   */
  verifyEmail: async (token: string): Promise<LoginResponseData> => {
    const response = await axiosClient.post<LoginResponseData>('/auth/verify-email', { token });
    return response.data;
  },

  /**
   * Resend the verification email to the user
   */
  resendVerification: async (email: string): Promise<AuthSuccessResponse> => {
    const response = await axiosClient.post<AuthSuccessResponse>('/auth/resend-verification', { email });
    return response.data;
  },
};
