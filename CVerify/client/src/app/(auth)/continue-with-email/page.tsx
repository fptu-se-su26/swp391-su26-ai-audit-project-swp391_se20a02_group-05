"use client";

import React, { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useAuth } from "../../../features/auth/hooks/use-auth";
import {
  Card,
  Typography,
  Button,
  TextField,
  InputOTP,
  InputGroup,
  Form,
  Label,
  FieldError,
  toast,
  Spinner,
} from "@heroui/react";
import { Eye, EyeOff, ShieldCheck, Mail } from "lucide-react";
import { Suspense } from "react";

function PasswordStrengthMeter({ value }: { value: string }) {
  if (!value) return null;

  const checks = {
    length: value.length >= 8,
    uppercase: /[A-Z]/.test(value),
    lowercase: /[a-z]/.test(value),
    digit: /\d/.test(value),
    special: /[@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`]/.test(value),
  };

  const score = Object.values(checks).filter(Boolean).length;

  let label = "Very Weak";
  let color = "bg-danger";
  let textColor = "text-danger";

  if (score === 5) {
    label = "Strong";
    color = "bg-success";
    textColor = "text-success";
  } else if (score >= 3) {
    label = "Fair";
    color = "bg-warning";
    textColor = "text-warning";
  } else if (score >= 1) {
    label = "Weak";
    color = "bg-danger";
    textColor = "text-danger";
  }

  return (
    <div className="space-y-2 mt-2 px-1 select-none">
      <div className="flex justify-between items-center text-xs">
        <span className="text-zinc-500 dark:text-zinc-400 text-[11px] font-medium font-sans">
          Password Strength
        </span>
        <span
          className={`font-bold text-[11px] transition-colors ${textColor}`}
        >
          {label}
        </span>
      </div>

      <div className="flex gap-1 h-1.5 w-full bg-zinc-100 dark:bg-zinc-800 rounded-full overflow-hidden">
        <div
          className={`h-full rounded-full transition-all duration-300 ${color}`}
          style={{ width: `${(score / 5) * 100}%` }}
        />
      </div>

      <div className="grid grid-cols-2 gap-x-3 gap-y-1 text-[11px] text-zinc-500 dark:text-zinc-400 mt-1.5">
        <span
          className={`flex items-center gap-1 transition-colors ${checks.length ? "text-success font-medium" : "text-zinc-400 dark:text-zinc-500"}`}
        >
          {checks.length ? "✓" : "○"} At least 8 characters
        </span>
        <span
          className={`flex items-center gap-1 transition-colors ${checks.uppercase ? "text-success font-medium" : "text-zinc-400 dark:text-zinc-500"}`}
        >
          {checks.uppercase ? "✓" : "○"} One uppercase letter
        </span>
        <span
          className={`flex items-center gap-1 transition-colors ${checks.lowercase ? "text-success font-medium" : "text-zinc-400 dark:text-zinc-500"}`}
        >
          {checks.lowercase ? "✓" : "○"} One lowercase letter
        </span>
        <span
          className={`flex items-center gap-1 transition-colors ${checks.digit ? "text-success font-medium" : "text-zinc-400 dark:text-zinc-500"}`}
        >
          {checks.digit ? "✓" : "○"} One number
        </span>
        <span
          className={`flex items-center gap-1 transition-colors ${checks.special ? "text-success font-medium" : "text-zinc-400 dark:text-zinc-500"}`}
        >
          {checks.special ? "✓" : "○"} One special character
        </span>
      </div>
    </div>
  );
}

function ContinueWithEmailContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { sendOtp, verifyOtp, createPassword } = useAuth();

  const email = searchParams.get("email") || "";
  const initialChallengeId = searchParams.get("challengeId") || "";
  const callbackUrl = searchParams.get("callbackUrl") || "/";

  // Step state: 1 = OTP verification, 2 = Create Password
  const [step, setStep] = useState(1);
  const [challengeId, setChallengeId] = useState(initialChallengeId);
  const [verificationToken, setVerificationToken] = useState("");

  // Step 1: OTP states
  const [otpCode, setOtpCode] = useState("");
  const [isOtpLoading, setIsOtpLoading] = useState(false);
  const [cooldown, setCooldown] = useState(60);

  // Step 2: Password setup states
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isVisible, setIsVisible] = useState(false);
  const [isConfirmVisible, setIsConfirmVisible] = useState(false);
  const [isPasswordLoading, setIsPasswordLoading] = useState(false);

  // Timer cooldown ticking
  useEffect(() => {
    if (cooldown <= 0) return;
    const interval = setInterval(() => {
      setCooldown((prev) => prev - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [cooldown]);

  const handleResendOtp = async () => {
    if (cooldown > 0) return;

    const result = await sendOtp(email, "Authentication");
    if (result.success && result.data) {
      setChallengeId(result.data.challengeId);
      setCooldown(60);
      toast.success("New OTP sent successfully.");
    } else {
      toast.danger(
        result.error?.message || "Failed to resend OTP. Please try again.",
      );
    }
  };

  const handleVerifyOtp = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!otpCode || otpCode.length < 6) {
      toast.danger("Invalid Code", {
        description: "Please enter the full 6-digit OTP code.",
      });
      return;
    }

    setIsOtpLoading(true);
    const result = await verifyOtp(
      challengeId,
      email,
      otpCode,
      "Authentication",
    );
    setIsOtpLoading(false);

    if (result.success && result.data) {
      setVerificationToken(result.data.verificationToken);
      setStep(2);
      toast.success("OTP Verified", {
        description: "Email ownership proven. Please set your password.",
      });
    } else {
      toast.danger("Verification Failed", {
        description:
          result.error?.message ||
          "The OTP code entered is incorrect or expired.",
      });
    }
  };

  const isPasswordValid =
    password.length >= 8 &&
    /[A-Z]/.test(password) &&
    /[a-z]/.test(password) &&
    /\d/.test(password) &&
    /[@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`]/.test(password);

  const handlePasswordSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isPasswordValid) {
      toast.danger("Weak Password", {
        description:
          "Password must contain at least 8 characters, 1 uppercase, 1 lowercase, 1 number, and 1 special character.",
      });
      return;
    }
    if (password !== confirmPassword) {
      toast.danger("Mismatch", { description: "Passwords do not match." });
      return;
    }

    setIsPasswordLoading(true);
    const result = await createPassword({
      challengeId,
      email,
      verificationToken,
      password,
      confirmPassword,
    });
    setIsPasswordLoading(false);

    if (result.success) {
      toast.success("Identity Setup Complete", {
        description:
          "Welcome to CVerify! You have been logged in successfully.",
      });
      router.push(callbackUrl);
    } else {
      let errorMsg =
        result.error?.message || "Failed to establish password credential.";
      if (result.error?.errors) {
        const details = Object.values(result.error.errors).flat().join(" ");
        if (details) {
          errorMsg = details;
        }
      }
      toast.danger("Setup Failed", {
        description: errorMsg,
      });
    }
  };

  return (
    <Card className="w-full">
      {step === 1 ? (
        <div className="w-full flex flex-col items-center">
          <div className="w-12 h-12 bg-background flex items-center justify-center rounded-xl my-6">
            <Mail className="size-6" />
          </div>

          <div className="text-center w-full mb-6 flex flex-col items-center gap-2">
            <Typography.Heading level={3} className="text-2xl font-bold">
              Confirm email ownership
            </Typography.Heading>
            <Typography className="text-sm text-muted">
              We&apos;ve sent a 6-digit verification code to{" "}
              <span className="font-bold">{email}</span>.
            </Typography>
          </div>

          <Form
            className="w-full flex flex-col gap-6 items-center px-12"
            onSubmit={handleVerifyOtp}
          >
            <div className="flex flex-col gap-2 items-center w-full">
              <Label className="text-muted">One-Time Code</Label>
              <InputOTP maxLength={6} value={otpCode} onChange={setOtpCode}>
                <InputOTP.Group>
                  <InputOTP.Slot
                    className="border rounded-2xl h-12 w-12"
                    index={0}
                  />
                  <InputOTP.Slot
                    className="border rounded-2xl h-12 w-12"
                    index={1}
                  />
                  <InputOTP.Slot
                    className="border rounded-2xl h-12 w-12"
                    index={2}
                  />
                </InputOTP.Group>
                <InputOTP.Separator />
                <InputOTP.Group>
                  <InputOTP.Slot
                    className="border rounded-2xl h-12 w-12"
                    index={3}
                  />
                  <InputOTP.Slot
                    className="border rounded-2xl h-12 w-12"
                    index={4}
                  />
                  <InputOTP.Slot
                    className="border rounded-2xl h-12 w-12"
                    index={5}
                  />
                </InputOTP.Group>
              </InputOTP>
            </div>

            <Button
              type="submit"
              fullWidth
              isPending={isOtpLoading}
              isDisabled={otpCode.length < 6 || isOtpLoading}
              className="h-12 rounded-2xl"
            >
              {isOtpLoading && <Spinner color="current" size="sm" />}
              Verify code
            </Button>
          </Form>

          <div className="text-center text-xs font-medium text-muted my-6">
            Didn&apos;t receive the email?{" "}
            {cooldown > 0 ? (
              <span className="font-bold">Resend in {cooldown}s</span>
            ) : (
              <button
                onClick={handleResendOtp}
                className="font-bold text-foreground hover:underline cursor-pointer bg-transparent border-0"
              >
                Resend code
              </button>
            )}
          </div>
        </div>
      ) : (
        <div className="w-full flex flex-col items-center">
          <div className="w-12 h-12 bg-background flex items-center justify-center rounded-xl my-6">
            <ShieldCheck className="size-6" />
          </div>

          <div className="text-center w-full mb-6 flex flex-col items-center gap-2">
            <Typography.Heading level={3} className="text-2xl font-bold">
              Establish Password
            </Typography.Heading>
            <Typography className="text-sm text-muted">
              Secure your new CVerify engineer profile credential.
            </Typography>
          </div>

          <Form
            className="w-full flex flex-col gap-6 p-12"
            onSubmit={handlePasswordSubmit}
          >
            <TextField isRequired name="password" type="password">
              <Label>Password</Label>
              <InputGroup>
                <InputGroup.Input
                  className="h-12"
                  type={isVisible ? "text" : "password"}
                  placeholder="Create password"
                  value={password}
                  onChange={(e: React.ChangeEvent<HTMLInputElement>) => setPassword(e.target.value)}
                />
                <InputGroup.Suffix>
                  <Button
                    isIconOnly
                    aria-label={isVisible ? "Hide password" : "Show password"}
                    size="sm"
                    variant="ghost"
                    onPress={() => setIsVisible(!isVisible)}
                  >
                    {isVisible ? (
                      <Eye className="size-4" />
                    ) : (
                      <EyeOff className="size-4" />
                    )}
                  </Button>
                </InputGroup.Suffix>
              </InputGroup>
              <PasswordStrengthMeter value={password} />
              <FieldError />
            </TextField>

            <TextField isRequired name="confirmPassword" type="password">
              <Label>Confirm Password</Label>
              <InputGroup>
                <InputGroup.Input
                  className="h-12"
                  type={isConfirmVisible ? "text" : "password"}
                  placeholder="Confirm your password"
                  value={confirmPassword}
                  onChange={(e: React.ChangeEvent<HTMLInputElement>) => setConfirmPassword(e.target.value)}
                />
                <InputGroup.Suffix>
                  <Button
                    isIconOnly
                    aria-label={isVisible ? "Hide password" : "Show password"}
                    size="sm"
                    variant="ghost"
                    onPress={() => setIsConfirmVisible(!isConfirmVisible)}
                  >
                    {isConfirmVisible ? (
                      <Eye className="size-4" />
                    ) : (
                      <EyeOff className="size-4" />
                    )}
                  </Button>
                </InputGroup.Suffix>
              </InputGroup>
              <FieldError />
            </TextField>

            <Button
              type="submit"
              fullWidth
              isPending={isPasswordLoading}
              isDisabled={
                !isPasswordValid ||
                password !== confirmPassword ||
                isPasswordLoading
              }
              className="h-12 rounded-2xl"
            >
              {({ isPending }) => (
                <>
                  {isPending && <Spinner color="current" size="sm" />}
                  {isPending
                    ? "Complete Registration..."
                    : "Complete Registration"}
                </>
              )}
            </Button>
          </Form>
        </div>
      )}
    </Card>
  );
}

export default function ContinueWithEmailPage() {
  return (
    <Suspense
      fallback={
        <div className="flex items-center justify-center p-8 min-h-[400px]">
          <div className="w-8 h-8 border-2 border-t-zinc-900 border-zinc-200 dark:border-t-zinc-100 rounded-full animate-spin" />
        </div>
      }
    >
      <ContinueWithEmailContent />
    </Suspense>
  );
}
