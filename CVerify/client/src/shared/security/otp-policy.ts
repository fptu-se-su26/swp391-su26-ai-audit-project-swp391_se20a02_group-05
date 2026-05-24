export interface OtpPolicy {
  id: string;
  length: number;
  allowedCharacters: 'numeric' | 'alphanumeric';
  cooldownSeconds: number;
  expirationSeconds: number;
  maxRetries: number;
}

export const otpPoliciesRegistry: Record<string, OtpPolicy> = {
  default: {
    id: 'default',
    length: 6,
    allowedCharacters: 'numeric',
    cooldownSeconds: 60,
    expirationSeconds: 300,
    maxRetries: 3,
  },
};

export function validateOtp(
  code: string,
  policyId: string = 'default'
): { isValid: boolean; message?: string } {
  const policy = otpPoliciesRegistry[policyId] || otpPoliciesRegistry.default;

  if (!code) {
    return { isValid: false, message: 'Verification code is required.' };
  }

  if (code.length !== policy.length) {
    return {
      isValid: false,
      message: `Verification code must be exactly ${policy.length} characters long.`,
    };
  }

  if (policy.allowedCharacters === 'numeric' && !/^\d+$/.test(code)) {
    return {
      isValid: false,
      message: 'Verification code must contain digits only.',
    };
  }

  if (policy.allowedCharacters === 'alphanumeric' && !/^[a-zA-Z0-9]+$/.test(code)) {
    return {
      isValid: false,
      message: 'Verification code must contain alphanumeric characters only.',
    };
  }

  return { isValid: true };
}
