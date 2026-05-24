"use client";

import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { recoveryApi } from '@/features/auth/services/recovery.service';
import {
  Card,
  Typography,
  Button,
  TextField,
  InputOTP,
  InputGroup,
  Input,
  Form,
  Label,
  toast,
  Spinner,
  FieldError,
} from "@heroui/react";
import {
  Eye,
  EyeOff,
  ShieldCheck,
  Mail,
  ArrowLeft,
  Building2,
} from "lucide-react";
import axios from "axios";

// Step 1 Schema: Tax Code validation
const step1Schema = z.object({
  taxCode: z
    .string()
    .min(5, "Tax code must be at least 5 characters")
    .max(50, "Tax code cannot exceed 50 characters"),
});

// Step 2 Schema: OTP validation
const step2Schema = z.object({
  code: z.string().length(6, "Verification code must be exactly 6 digits"),
});

// Step 3 Schema: Password rotation validation
const step3Schema = z
  .object({
    password: z
      .string()
      .min(8, "Password must be at least 8 characters long")
      .regex(
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`])[A-Za-z\d@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`]{8,}$/,
        "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.",
      ),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

type Step1Input = z.infer<typeof step1Schema>;
type Step2Input = z.infer<typeof step2Schema>;
type Step3Input = z.infer<typeof step3Schema>;

export function OrgRecoveryView() {
  const router = useRouter();

  // Wizard steps: 1 = Enter Tax Code, 2 = Verify OTP, 3 = Reset Password
  const [step, setStep] = useState(1);
  const [isLoading, setIsLoading] = useState(false);
  const [cooldown, setCooldown] = useState(0);

  // Recovery credentials resolved on backend
  const [taxCode, setTaxCode] = useState("");
  const [challengeId, setChallengeId] = useState("");
  const [maskedEmail, setMaskedEmail] = useState("");
  const [verificationToken, setVerificationToken] = useState("");

  // Password visibility triggers
  const [isVisible, setIsVisible] = useState(false);
  const [isConfirmVisible, setIsConfirmVisible] = useState(false);

  // Forms setup with Zod resolvers
  const {
    register: reg1,
    handleSubmit: handleSub1,
    formState: { errors: err1 },
  } = useForm<Step1Input>({
    resolver: zodResolver(step1Schema),
  });

  const {
    control: control2,
    handleSubmit: handleSub2,
    formState: { errors: err2 },
  } = useForm<Step2Input>({
    resolver: zodResolver(step2Schema),
    defaultValues: { code: "" },
  });

  const {
    register: reg3,
    handleSubmit: handleSub3,
    formState: { errors: err3 },
  } = useForm<Step3Input>({
    resolver: zodResolver(step3Schema),
  });

  // OTP resend cooldown timer
  useEffect(() => {
    if (cooldown <= 0) return;
    const interval = setInterval(() => {
      setCooldown((prev) => prev - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [cooldown]);

  // Step 1: Submit Tax Code to generate OTP enqueued backend-side
  const onSubmitTaxCode = async (data: Step1Input) => {
    setIsLoading(true);
    try {
      const response = await recoveryApi.orgForgot(data.taxCode);
      setTaxCode(data.taxCode);
      setChallengeId(response.challengeId);
      setMaskedEmail(response.maskedEmail);
      setCooldown(response.cooldownSeconds);
      setStep(2);
      toast.success("Verification Code Dispatched", {
        description: `OTP code sent to registered corporate mailboxes matching ${response.maskedEmail}.`,
      });
    } catch (err) {
      const errorMessage =
        axios.isAxiosError(err) && err.response?.data?.message
          ? err.response.data.message
          : "Failed to trigger recovery. Ensure your Tax Code is correct.";
      toast.danger("Request Failed", {
        description: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  };

  // Resend OTP trigger
  const handleResendOtp = async () => {
    if (cooldown > 0 || !taxCode) return;

    setIsLoading(true);
    try {
      const response = await recoveryApi.orgForgot(taxCode);
      setChallengeId(response.challengeId);
      setCooldown(response.cooldownSeconds);
      toast.success("New OTP sent successfully.");
    } catch (err) {
      const errorMessage =
        axios.isAxiosError(err) && err.response?.data?.message
          ? err.response.data.message
          : "Failed to resend OTP.";
      toast.danger(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  // Step 2: Verify OTP
  const onSubmitOtp = async (data: Step2Input) => {
    setIsLoading(true);
    try {
      const response = await recoveryApi.orgVerifyOtp({
        taxCode,
        challengeId,
        code: data.code,
      });
      setVerificationToken(response.verificationToken);
      setStep(3);
      toast.success("OTP Verified Successfully", {
        description:
          "Your corporate administrative identity is linked. Please rotate your password.",
      });
    } catch (err) {
      const errorMessage =
        axios.isAxiosError(err) && err.response?.data?.message
          ? err.response.data.message
          : "The code entered is invalid or has expired.";
      toast.danger("Verification Failed", {
        description: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  };

  // Step 3: Rotate corporate admin password
  const onSubmitPassword = async (data: Step3Input) => {
    setIsLoading(true);
    try {
      await recoveryApi.orgResetPassword({
        token: verificationToken,
        newPassword: data.password,
        confirmPassword: data.confirmPassword,
      });
      toast.success("Password rotated successfully!", {
        description:
          "Your corporate credentials have been updated. Please sign in with your new password.",
      });
      router.push("/login?tab=bussiness");
    } catch (err) {
      const errorMessage =
        axios.isAxiosError(err) && err.response?.data?.message
          ? err.response.data.message
          : "Failed to update corporate credentials. Please try again.";
      toast.danger("Rotation Failed", {
        description: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Card className="w-full bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 p-8 shadow-xl rounded-2xl">
      {step === 1 && (
        <div className="w-full flex flex-col items-center">
          <div className="w-full flex justify-start mb-6">
            <button
              onClick={() => router.push("/login")}
              className="inline-flex items-center gap-1.5 text-xs font-semibold text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors group cursor-pointer bg-transparent border-0"
            >
              <ArrowLeft
                size={14}
                className="transition-transform group-hover:-translate-x-0.5"
              />{" "}
              Back to Sign In
            </button>
          </div>

          <div className="w-12 h-12 bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center rounded-xl mb-6">
            <Building2 className="size-6 text-zinc-900 dark:text-zinc-100" />
          </div>

          <div className="text-center w-full mb-8 font-outfit">
            <Typography.Heading
              level={3}
              className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100"
            >
              Corporate Recovery
            </Typography.Heading>
            <Typography className="text-sm text-zinc-500 dark:text-zinc-400">
              Enter your official Tax Code to verify corporate ownership and
              receive a challenge code.
            </Typography>
          </div>

          <Form
            className="w-full flex flex-col gap-4"
            onSubmit={handleSub1(onSubmitTaxCode)}
          >
            <TextField isRequired name="taxCode" isInvalid={!!err1.taxCode}>
              <Label className="text-sm font-medium text-zinc-700 dark:text-zinc-300 pb-1">
                Business Tax Code (MST)
              </Label>
              <Input
                placeholder="Enter company tax code"
                className="h-12"
                {...reg1("taxCode")}
              />
              {err1.taxCode && <FieldError>{err1.taxCode.message}</FieldError>}
            </TextField>

            <Button
              type="submit"
              fullWidth
              isPending={isLoading}
              isDisabled={isLoading}
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-semibold mt-2 flex items-center justify-center gap-2"
            >
              {isLoading && <Spinner color="current" size="sm" />}
              Request corporate OTP
            </Button>
          </Form>
        </div>
      )}

      {step === 2 && (
        <div className="w-full flex flex-col items-center">
          <div className="w-12 h-12 bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center rounded-xl mb-6">
            <Mail className="size-6 text-zinc-900 dark:text-zinc-100" />
          </div>

          <div className="text-center w-full mb-8 flex flex-col items-center">
            <Typography.Heading
              level={3}
              className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100"
            >
              Verify Corporate Mailbox
            </Typography.Heading>
            <Typography className="text-xs text-zinc-500 dark:text-zinc-400 leading-normal max-w-sm">
              We resolved your registered recovery contact email as{" "}
              <span className="font-semibold text-zinc-700 dark:text-zinc-300">
                {maskedEmail}
              </span>
              . Enter the 6-digit code sent there to confirm admin credentials
              access.
            </Typography>
          </div>

          <Form
            className="w-full flex flex-col gap-5 items-center"
            onSubmit={handleSub2(onSubmitOtp)}
          >
            <div className="flex flex-col gap-2 items-center w-full">
              <Label className="text-sm font-medium text-zinc-700 dark:text-zinc-300 pb-1">
                One-Time Code
              </Label>

              <Controller
                name="code"
                control={control2}
                render={({ field }) => (
                  <InputOTP
                    maxLength={6}
                    value={field.value}
                    onChange={field.onChange}
                  >
                    <InputOTP.Group>
                      <InputOTP.Slot index={0} />
                      <InputOTP.Slot index={1} />
                      <InputOTP.Slot index={2} />
                    </InputOTP.Group>
                    <InputOTP.Separator />
                    <InputOTP.Group>
                      <InputOTP.Slot index={3} />
                      <InputOTP.Slot index={4} />
                      <InputOTP.Slot index={5} />
                    </InputOTP.Group>
                  </InputOTP>
                )}
              />
              {err2.code && (
                <div className="text-danger text-xs font-semibold mt-1">
                  {err2.code.message}
                </div>
              )}
            </div>

            <Button
              type="submit"
              fullWidth
              isPending={isLoading}
              isDisabled={isLoading}
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-semibold flex items-center justify-center gap-2"
            >
              {isLoading && <Spinner color="current" size="sm" />}
              Verify corporate identity
            </Button>
          </Form>

          <div className="text-center text-xs font-medium text-zinc-500 dark:text-zinc-400 pt-6">
            Didn&apos;t receive the code?{" "}
            {cooldown > 0 ? (
              <span className="font-semibold text-zinc-400">
                Resend in {cooldown}s
              </span>
            ) : (
              <button
                onClick={handleResendOtp}
                className="font-semibold text-zinc-900 dark:text-zinc-100 hover:underline cursor-pointer bg-transparent border-0"
              >
                Resend code
              </button>
            )}
          </div>
        </div>
      )}

      {step === 3 && (
        <div className="w-full flex flex-col items-center">
          <div className="w-12 h-12 bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center rounded-xl mb-6">
            <ShieldCheck className="size-6 text-zinc-900 dark:text-zinc-100" />
          </div>

          <div className="text-center w-full mb-8">
            <Typography.Heading
              level={3}
              className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100"
            >
              Reset Administrator Password
            </Typography.Heading>
            <Typography className="text-sm text-zinc-500 dark:text-zinc-400">
              Establish your new secure administrative credential for this
              organization workspace.
            </Typography>
          </div>

          <Form
            className="w-full flex flex-col gap-5"
            onSubmit={handleSub3(onSubmitPassword)}
          >
            <TextField
              isRequired
              name="password"
              type="password"
              isInvalid={!!err3.password}
            >
              <Label className="text-sm font-medium text-zinc-700 dark:text-zinc-300 pb-1">
                New Password
              </Label>
              <InputGroup>
                <InputGroup.Input
                  className="h-12"
                  type={isVisible ? "text" : "password"}
                  placeholder="Enter new secure password"
                  {...reg3("password")}
                />
                <InputGroup.Suffix>
                  <Button
                    isIconOnly
                    variant="ghost"
                    size="sm"
                    className="text-zinc-400 hover:bg-transparent"
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
              {err3.password && (
                <FieldError>{err3.password.message}</FieldError>
              )}
            </TextField>

            <TextField
              isRequired
              name="confirmPassword"
              type="password"
              isInvalid={!!err3.confirmPassword}
            >
              <Label className="text-sm font-medium text-zinc-700 dark:text-zinc-300 pb-1">
                Confirm New Password
              </Label>
              <InputGroup>
                <InputGroup.Input
                  className="h-12"
                  type={isConfirmVisible ? "text" : "password"}
                  placeholder="Repeat your new password"
                  {...reg3("confirmPassword")}
                />
                <InputGroup.Suffix>
                  <Button
                    isIconOnly
                    variant="ghost"
                    size="sm"
                    className="text-zinc-400 hover:bg-transparent"
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
              {err3.confirmPassword && (
                <FieldError>{err3.confirmPassword.message}</FieldError>
              )}
            </TextField>

            <Button
              type="submit"
              fullWidth
              isPending={isLoading}
              isDisabled={isLoading}
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-semibold mt-2 flex items-center justify-center gap-2"
            >
              {isLoading && <Spinner color="current" size="sm" />}
              Update credentials
            </Button>
          </Form>
        </div>
      )}
    </Card>
  );
}
