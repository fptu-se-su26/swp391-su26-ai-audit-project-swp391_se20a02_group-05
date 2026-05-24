import { useState, useEffect, useCallback, useRef } from 'react';

export type VerificationState =
  | 'idle'
  | 'typing'
  | 'submitting'
  | 'verified'
  | 'expired'
  | 'cooldown'
  | 'locked'
  | 'rate_limited';

interface UseVerificationSessionProps {
  initialCooldown?: number;
  maxAttempts?: number;
  onResend: () => Promise<boolean>;
  onSubmit: (code: string) => Promise<boolean>;
}

export function useVerificationSession({
  initialCooldown = 60,
  maxAttempts = 5,
  onResend,
  onSubmit,
}: UseVerificationSessionProps) {
  const [state, setState] = useState<VerificationState>('idle');
  const [cooldown, setCooldown] = useState(0);
  const [attempts, setAttempts] = useState(0);
  const [otpCode, setOtpCode] = useState('');
  const timerRef = useRef<NodeJS.Timeout | null>(null);

  // Initialize or decrease cooldown timer
  useEffect(() => {
    if (cooldown <= 0) {
      if (state === 'cooldown') {
        setTimeout(() => {
          setState('idle');
        }, 0);
      }
      return;
    }

    timerRef.current = setInterval(() => {
      setCooldown((prev) => {
        if (prev <= 1) {
          if (timerRef.current) clearInterval(timerRef.current);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, [cooldown, state]);

  // Handle setting/typing OTP
  const handleSetOtpCode = useCallback((code: string) => {
    setOtpCode(code);
    if (state === 'idle' || state === 'expired') {
      setState('typing');
    }
  }, [state]);

  // Trigger submission through state machine
  const submitSession = useCallback(async (code: string = otpCode) => {
    if (state === 'locked' || state === 'rate_limited' || state === 'submitting') return false;

    setState('submitting');
    const success = await onSubmit(code);

    if (success) {
      setState('verified');
      return true;
    } else {
      const nextAttempts = attempts + 1;
      setAttempts(nextAttempts);

      if (nextAttempts >= maxAttempts) {
        setState('locked');
      } else {
        setState('typing');
      }
      return false;
    }
  }, [otpCode, state, attempts, maxAttempts, onSubmit]);

  // Trigger resend through state machine
  const resendSession = useCallback(async () => {
    if (cooldown > 0 || state === 'submitting' || state === 'locked') return false;

    setState('submitting');
    const success = await onResend();

    if (success) {
      setCooldown(initialCooldown);
      setState('cooldown');
      setOtpCode('');
      return true;
    } else {
      setState('idle');
      return false;
    }
  }, [cooldown, state, initialCooldown, onResend]);

  const resetSession = useCallback(() => {
    setState('idle');
    setCooldown(0);
    setAttempts(0);
    setOtpCode('');
    if (timerRef.current) clearInterval(timerRef.current);
  }, []);

  return {
    state,
    cooldown,
    attempts,
    otpCode,
    setOtpCode: handleSetOtpCode,
    submit: submitSession,
    resend: resendSession,
    reset: resetSession,
    isPending: state === 'submitting',
    isDisabled: state === 'locked' || state === 'submitting',
  };
}
