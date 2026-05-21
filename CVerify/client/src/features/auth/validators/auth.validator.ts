import { z } from 'zod';

// Enforces: 1 uppercase, 1 lowercase, 1 number, 1 special character (non-alphanumeric), min 8 chars
const PASSWORD_STRENGTH_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$/;

export const passwordValidation = z
  .string()
  .min(8, { message: 'auth:validation.passwordMin' })
  .regex(PASSWORD_STRENGTH_REGEX, {
    message: 'auth:validation.passwordStrength',
  });

export const loginSchema = z.object({
  email: z
    .string()
    .min(1, { message: 'auth:validation.emailRequired' })
    .email({ message: 'auth:validation.emailInvalid' }),
  password: z
    .string()
    .min(1, { message: 'auth:validation.passwordRequired' }),
  rememberMe: z.boolean(),
});

export const registerSchema = z
  .object({
    fullName: z
      .string()
      .min(2, { message: 'auth:validation.fullNameMin' })
      .max(100, { message: 'auth:validation.fullNameMax' })
      .trim()
      .regex(/^[\p{L}\p{M}\s'-]+$/u, {
        message: 'auth:validation.fullNameInvalid',
      }),
    email: z
      .string()
      .min(1, { message: 'auth:validation.emailRequired' })
      .email({ message: 'auth:validation.emailInvalid' })
      .toLowerCase(),
    password: passwordValidation,
    confirmPassword: z.string().min(1, { message: 'auth:validation.confirmPasswordRequired' }),
    agreeTerms: z.boolean().refine((val) => val === true, {
      message: 'auth:validation.agreeTermsRequired',
    }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'auth:validation.passwordsMismatch',
    path: ['confirmPassword'],
  });

export const forgotPasswordSchema = z.object({
  email: z
    .string()
    .min(1, { message: 'auth:validation.emailRequired' })
    .email({ message: 'auth:validation.emailInvalid' }),
});

export const resetPasswordSchema = z
  .object({
    password: passwordValidation,
    confirmPassword: z.string().min(1, { message: 'auth:validation.confirmPasswordRequired' }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'auth:validation.passwordsMismatch',
    path: ['confirmPassword'],
  });
