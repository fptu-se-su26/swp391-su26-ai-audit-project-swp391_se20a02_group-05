import { z } from 'zod';

// Enforces: 1 uppercase, 1 lowercase, 1 number, 1 special character (non-alphanumeric), min 8 chars
const PASSWORD_STRENGTH_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$/;

export const passwordValidation = z
  .string()
  .min(8, { message: 'Password must be at least 8 characters long.' })
  .regex(PASSWORD_STRENGTH_REGEX, {
    message:
      'Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.',
  });

export const loginSchema = z.object({
  email: z
    .string()
    .min(1, { message: 'Email address is required.' })
    .email({ message: 'Please enter a valid RFC-compliant email address.' }),
  password: z
    .string()
    .min(1, { message: 'Password is required.' }),
  rememberMe: z.boolean(),
});

export const registerSchema = z
  .object({
    fullName: z
      .string()
      .min(2, { message: 'Full name must be at least 2 characters.' })
      .max(100, { message: 'Full name cannot exceed 100 characters.' })
      .trim()
      .regex(/^[\p{L}\p{M}\s'-]+$/u, {
        message: 'Full name can only contain letters, spaces, hyphens, and apostrophes.',
      }),
    email: z
      .string()
      .min(1, { message: 'Email address is required.' })
      .email({ message: 'Please enter a valid RFC-compliant email address.' })
      .toLowerCase(),
    password: passwordValidation,
    confirmPassword: z.string().min(1, { message: 'Confirm password is required.' }),
    agreeTerms: z.boolean().refine((val) => val === true, {
      message: 'You must agree to the Terms of Service and Privacy Policy.',
    }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'Passwords do not match.',
    path: ['confirmPassword'],
  });

export const forgotPasswordSchema = z.object({
  email: z
    .string()
    .min(1, { message: 'Email address is required.' })
    .email({ message: 'Please enter a valid RFC-compliant email address.' }),
});

export const resetPasswordSchema = z
  .object({
    password: passwordValidation,
    confirmPassword: z.string().min(1, { message: 'Confirm password is required.' }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'Passwords do not match.',
    path: ['confirmPassword'],
  });
