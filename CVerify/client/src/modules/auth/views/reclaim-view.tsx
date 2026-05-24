"use client";

import React, { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { recoveryApi } from '@/features/auth/services/recovery.service';
import {
  Card,
  Typography,
  Button,
  TextField,
  InputOTP,
  Input,
  Form,
  Label,
  FieldError,
  toast,
  Spinner,
  Description,
} from "@heroui/react";
import {
  Building2,
  ArrowLeft,
  Mail,
  FileText,
  Upload,
  AlertTriangle,
  ShieldCheck,
  X,
  FileCheck2,
} from "lucide-react";
import axios from "axios";

export function ReclaimView() {
  const router = useRouter();
  const searchParams = useSearchParams();

  // Route parameters
  const taxCode = searchParams.get("taxCode") || "";
  const companyName = searchParams.get("companyName") || "";

  // Step state: 1 = Representative Info & OTP Send, 2 = Verify OTP, 3 = Legal Documents Upload, 4 = Success Receipt
  const [step, setStep] = useState(1);
  const [isLoading, setIsLoading] = useState(false);

  // Form State
  const [fullName, setFullName] = useState("");
  const [position, setPosition] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [recoveryEmail, setRecoveryEmail] = useState("");

  // Touched Validation States
  const [fullNameTouched, setFullNameTouched] = useState(false);
  const [positionTouched, setPositionTouched] = useState(false);
  const [phoneTouched, setPhoneTouched] = useState(false);
  const [emailTouched, setEmailTouched] = useState(false);

  // OTP State
  const [challengeId, setChallengeId] = useState("");
  const [otpCode, setOtpCode] = useState("");
  const [cooldown, setCooldown] = useState(0);
  const [emailVerificationToken, setEmailVerificationToken] = useState("");

  // Document Upload State
  const [files, setFiles] = useState<File[]>([]);
  const [uploadError, setUploadError] = useState<string | null>(null);

  // Recovery Receipt Data
  const [receipt, setReceipt] = useState<{
    claimId: string;
    riskScore: number;
    riskLevel: string;
    status: string;
  } | null>(null);

  // Validation formulas
  const isFullNameValid = fullName.trim().length >= 3;
  const isPositionValid = position.trim().length >= 2;
  const isPhoneValid = /^[0-9+]{9,15}$/.test(phoneNumber);
  const isEmailValid = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(recoveryEmail);

  const isStep1Valid =
    isFullNameValid && isPositionValid && isPhoneValid && isEmailValid;

  // Timer cooldown ticking for OTP
  useEffect(() => {
    if (cooldown <= 0) return;
    const interval = setInterval(() => {
      setCooldown((prev) => prev - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [cooldown]);

  // Dispatch OTP code securely to verify corporate email domain ownership before claiming
  const handleSendOtp = async () => {
    setEmailTouched(true);
    if (!isEmailValid) return;

    setIsLoading(true);
    try {
      const result = await recoveryApi.orgForgot(taxCode);
      setChallengeId(result.challengeId);
      setCooldown(result.cooldownSeconds);
      setStep(2); // Go to OTP verification step
      toast.success("Security OTP sent!", {
        description: `Verification code dispatched to the registered company recovery email ${result.maskedEmail}.`,
      });
    } catch (err) {
      const errorMessage =
        axios.isAxiosError(err) && err.response?.data?.message
          ? err.response.data.message
          : "Please verify that the Tax Code is correct and try again.";
      toast.danger("Verification code failed to send", {
        description: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  };

  // Verify OTP & Generate signed verification token
  const handleVerifyOtp = async (e: React.FormEvent) => {
    e.preventDefault();
    if (otpCode.length < 6 || !challengeId) return;

    setIsLoading(true);
    try {
      const response = await recoveryApi.orgVerifyOtp({
        taxCode,
        challengeId,
        code: otpCode,
      });

      setEmailVerificationToken(response.verificationToken);
      setStep(3); // Advance to upload step
      toast.success("OTP verified successfully!", {
        description: "Your claimant corporate identity is securely verified.",
      });
    } catch (err) {
      const errorMessage =
        axios.isAxiosError(err) && err.response?.data?.message
          ? err.response.data.message
          : "The OTP code entered is incorrect or has expired.";
      toast.danger("OTP Verification failed", {
        description: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  };

  // Document management
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files) return;
    const selectedFiles = Array.from(e.target.files);

    // File validation: PDF/JPG/PNG only, max 10MB per file
    const invalidFile = selectedFiles.find(
      (f) =>
        !["application/pdf", "image/jpeg", "image/png"].includes(f.type) ||
        f.size > 10 * 1024 * 1024,
    );

    if (invalidFile) {
      setUploadError(
        "Files must be PDF, JPG, or PNG, and smaller than 10MB each.",
      );
      return;
    }

    setUploadError(null);
    setFiles((prev) => [...prev, ...selectedFiles]);
  };

  const removeFile = (index: number) => {
    setFiles((prev) => prev.filter((_, i) => i !== index));
  };

  // Submit Claim & Upload files
  const handleStep3Submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (files.length === 0) {
      setUploadError(
        "You must upload at least one legal verification document.",
      );
      return;
    }

    setIsLoading(true);
    try {
      const response = await recoveryApi.submitClaim({
        representativeFullName: fullName,
        representativePosition: position,
        phoneNumber,
        recoveryEmail,
        taxCode,
        emailVerificationToken,
        documents: files,
      });

      setReceipt(response);
      setStep(4); // Display success receipt
      toast.success("Disputed Reclaim Claim Submitted!", {
        description: "Our compliance managers have enqueued your legal business evidence for audit.",
      });
    } catch (err) {
      const errorMessage =
        axios.isAxiosError(err) && err.response?.data?.message
          ? err.response.data.message
          : "An unexpected error occurred during document submission.";
      toast.danger("Failed to submit claim", {
        description: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  };

  // STEP 4: Success Receipt
  if (step === 4 && receipt) {
    return (
      <Card className="w-full relative overflow-hidden max-h-[85vh] flex flex-col premium-glass border-accent/20">
        <div className="absolute top-0 left-0 w-full h-1.5 bg-success shrink-0" />
        <div className="flex flex-col items-center p-8 overflow-y-auto">
          <div className="w-16 h-16 bg-success/15 flex items-center justify-center rounded-2xl mb-6 border border-success/35 animate-bounce">
            <ShieldCheck className="size-8 text-success" />
          </div>

          <Typography.Heading
            level={3}
            className="text-2xl font-extrabold text-foreground text-center"
          >
            Disputed Ownership Reclaim Registered
          </Typography.Heading>

          <Typography className="text-sm text-muted text-center mt-2 max-w-md leading-relaxed">
            Your enterprise ownership reclaim has been registered. The anti-fraud verification engine has queued your legal proofs for manual auditor sign-off.
          </Typography>

          <div className="w-full mt-6 space-y-4 p-5 rounded-2xl bg-surface-secondary border border-border">
            <div className="flex justify-between items-center text-xs">
              <span className="text-muted">Claim Reference ID</span>
              <span className="font-mono font-semibold text-foreground select-all">
                {receipt.claimId}
              </span>
            </div>

            <div className="border-t border-border/60 my-2" />

            <div className="flex justify-between items-center text-xs">
              <span className="text-muted">Target Tax Code</span>
              <span className="font-semibold text-foreground">{taxCode}</span>
            </div>

            <div className="flex justify-between items-center text-xs">
              <span className="text-muted">Official Company Name</span>
              <span className="font-semibold text-foreground">
                {companyName}
              </span>
            </div>

            <div className="flex justify-between items-center text-xs">
              <span className="text-muted">Representative Claimant</span>
              <span className="font-semibold text-foreground">
                {fullName} ({position})
              </span>
            </div>

            <div className="flex justify-between items-center text-xs">
              <span className="text-muted">Risk Assessment Queue</span>
              <span className="font-semibold text-warning">
                Compliance Dual-Review Enforced
              </span>
            </div>

            <div className="flex justify-between items-center text-xs">
              <span className="text-muted">Review Status</span>
              <span className="px-2 py-0.5 rounded-full text-[10px] font-bold bg-warning/15 text-warning border border-warning/20">
                {receipt.status}
              </span>
            </div>
          </div>

          <div className="w-full mt-8 flex flex-col gap-3">
            <Button
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-bold w-full"
              onPress={() => router.push("/login")}
            >
              Return to Login
            </Button>
            <Typography className="text-[10px] text-muted text-center leading-normal">
              You will receive an administrative bootstrap link at{" "}
              <strong className="text-foreground">{recoveryEmail}</strong> once
              compliance validation is approved (typically under 24 hours).
            </Typography>
          </div>
        </div>
      </Card>
    );
  }

  return (
    <Card className="w-full relative overflow-hidden max-h-[85vh] flex flex-col">
      <div className="absolute top-0 left-0 w-full h-1.5 bg-accent shrink-0" />

      {/* Premium Wizard Header */}
      <div className="w-full flex items-center justify-between px-6 py-4 border-b border-border shrink-0 select-none">
        <div className="flex items-center gap-2.5">
          <div className="w-8 h-8 rounded-lg bg-accent/15 flex items-center justify-center border border-accent/20">
            <Building2 className="size-4 text-accent" />
          </div>
          <div>
            <Typography className="text-sm font-bold text-foreground leading-none">
              Reclaim Organization Ownership
            </Typography>
            <Typography className="text-[10px] text-muted font-medium mt-1 leading-none">
              MST: {taxCode} | Exceptional Recovery
            </Typography>
          </div>
        </div>
        <Button
          variant="ghost"
          size="sm"
          className="text-muted text-[11px]"
          onPress={() => {
            if (step > 1) {
              setStep((s) => s - 1);
            } else {
              router.push("/company-verification");
            }
          }}
        >
          <ArrowLeft className="size-3.5 mr-1" />
          {step > 1 ? "Previous" : "Cancel"}
        </Button>
      </div>

      {/* PROGRESS TIMELINE */}
      <div className="w-full h-1 bg-surface-secondary shrink-0">
        <div
          className="h-full bg-accent transition-all duration-300"
          style={{ width: `${(step / 3) * 100}%` }}
        />
      </div>

      {/* STEP 1: CLAIMANT POSITION & EMAIL CHALLENGE */}
      {step === 1 && (
        <Form
          className="w-full flex flex-col flex-1 overflow-hidden"
          onSubmit={(e) => {
            e.preventDefault();
            handleSendOtp();
          }}
        >
          <div className="flex-1 overflow-y-auto px-6 py-6 space-y-5">
            <div className="p-4 rounded-xl bg-surface-secondary border border-border mb-2 select-none text-center">
              <Typography className="text-[11px] text-muted">
                Target Disputed Entity
              </Typography>
              <Typography className="font-bold text-foreground mt-0.5">
                {companyName}
              </Typography>
            </div>

            <TextField
              isRequired
              name="fullName"
              isInvalid={fullNameTouched && !isFullNameValid}
            >
              <Label>Claimant Full Name</Label>
              <Input
                placeholder="Enter your official full name"
                className="h-11 rounded-xl"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                onBlur={(e) => setFullNameTouched(e.target.value.length > 0)}
              />
              <FieldError>Full name must be at least 3 characters.</FieldError>
            </TextField>

            <div className="grid grid-cols-2 gap-4">
              <TextField
                isRequired
                name="position"
                isInvalid={positionTouched && !isPositionValid}
              >
                <Label>Your Position / Job Title</Label>
                <Input
                  placeholder="e.g. CEO, Director"
                  className="h-11 rounded-xl"
                  value={position}
                  onChange={(e) => setPosition(e.target.value)}
                  onBlur={(e) => setPositionTouched(e.target.value.length > 0)}
                />
                <FieldError>
                  Job title must be at least 2 characters.
                </FieldError>
              </TextField>

              <TextField
                isRequired
                name="phoneNumber"
                isInvalid={phoneTouched && !isPhoneValid}
              >
                <Label>Contact Phone Number</Label>
                <Input
                  placeholder="e.g. +84901234567"
                  className="h-11 rounded-xl"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                  onBlur={(e) => setPhoneTouched(e.target.value.length > 0)}
                />
                <FieldError>Phone format is invalid.</FieldError>
              </TextField>
            </div>

            <TextField
              isRequired
              name="recoveryEmail"
              isInvalid={emailTouched && !isEmailValid}
            >
              <Label>Secure Recovery Email Address</Label>
              <Input
                placeholder="representative@yourcompany.com"
                className="h-11 rounded-xl"
                value={recoveryEmail}
                onChange={(e) => setRecoveryEmail(e.target.value)}
                onBlur={(e) => setEmailTouched(e.target.value.length > 0)}
              />
              <Description>
                Must be an active corporate domain matching official business registry details.
              </Description>
              <FieldError>Invalid email address format.</FieldError>
            </TextField>
          </div>

          <div className="p-4 border-t border-border shrink-0 w-full bg-surface">
            <Button
              type="submit"
              fullWidth
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-bold"
              isDisabled={!isStep1Valid || isLoading}
              isPending={isLoading}
            >
              {isLoading ? (
                <Spinner color="current" size="sm" />
              ) : (
                <Mail className="size-4 mr-2" />
              )}
              Send Email OTP Challenge
            </Button>
          </div>
        </Form>
      )}

      {/* STEP 2: OTP VERIFICATION */}
      {step === 2 && (
        <Form
          className="w-full flex flex-col flex-1 overflow-hidden"
          onSubmit={handleVerifyOtp}
        >
          <div className="flex-1 overflow-y-auto px-6 py-6 flex flex-col items-center justify-center text-center space-y-6">
            <div className="w-12 h-12 bg-accent/10 flex items-center justify-center rounded-xl border border-accent/20">
              <Mail className="size-5 text-accent" />
            </div>

            <div>
              <Typography.Heading
                level={4}
                className="text-lg font-bold text-foreground"
              >
                Confirm Claimant Email Ownership
              </Typography.Heading>
              <Typography className="text-xs text-muted mt-1.5 max-w-xs leading-normal">
                We sent a 6-digit OTP code to verify ownership of <strong className="text-foreground">{recoveryEmail}</strong>. Enter it below to unlock document uploading.
              </Typography>
            </div>

            <InputOTP
              maxLength={6}
              value={otpCode}
              onChange={setOtpCode}
              className="font-sans font-extrabold select-none"
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

            <div className="flex flex-col items-center gap-2">
              <Button
                variant="ghost"
                size="sm"
                className="text-xs text-accent font-semibold"
                isDisabled={cooldown > 0 || isLoading}
                onPress={handleSendOtp}
              >
                {cooldown > 0
                  ? `Resend OTP in {cooldown}s`
                  : "Resend Verification Code"}
              </Button>
            </div>
          </div>

          <div className="p-4 border-t border-border shrink-0 w-full bg-surface">
            <Button
              type="submit"
              fullWidth
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-bold"
              isDisabled={otpCode.length < 6 || isLoading}
              isPending={isLoading}
            >
              {isLoading ? (
                <Spinner color="current" size="sm" />
              ) : (
                "Verify claimant Identity"
              )}
            </Button>
          </div>
        </Form>
      )}

      {/* STEP 3: DISPUTED EVIDENCE PDF UPLOADER */}
      {step === 3 && (
        <Form
          className="w-full flex flex-col flex-1 overflow-hidden"
          onSubmit={handleStep3Submit}
        >
          <div className="flex-1 overflow-y-auto px-6 py-6 space-y-5 flex flex-col">
            <div className="text-center shrink-0 mb-1">
              <Typography className="text-xs font-semibold text-foreground">
                Provide Legal Business Ownership Evidence
              </Typography>
              <Typography className="text-[10px] text-muted mt-0.5 leading-normal">
                Upload tax extracts, active business licenses, or legal representation credentials.
              </Typography>
            </div>

            <div className="relative border-2 border-dashed border-border hover:border-accent/40 rounded-xl p-6 flex flex-col items-center justify-center bg-surface-secondary/40 select-none cursor-pointer group transition-colors min-h-[140px]">
              <input
                type="file"
                multiple
                accept=".pdf,image/jpeg,image/png"
                className="absolute inset-0 opacity-0 cursor-pointer"
                onChange={handleFileChange}
              />
              <Upload className="size-8 text-muted group-hover:text-accent transition-colors mb-2.5" />
              <Typography className="text-xs font-bold text-foreground">
                Drag & drop files or click to browse
              </Typography>
              <Typography className="text-[10px] text-muted mt-1">
                PDF, JPG, PNG up to 10MB per file
              </Typography>
            </div>

            {uploadError && (
              <div className="p-3 rounded-lg bg-danger/10 border border-danger/25 flex items-start gap-2 select-none text-[11px] text-danger font-medium leading-normal animate-pulse">
                <AlertTriangle className="size-4 shrink-0 text-danger mt-0.5" />
                <span>{uploadError}</span>
              </div>
            )}

            {files.length > 0 && (
              <div className="space-y-2 flex-1 overflow-y-auto min-h-[100px] border border-border/80 rounded-xl p-3 bg-surface-secondary/15">
                <Typography className="text-[10px] font-bold text-muted select-none">
                  Uploaded Proofs ({files.length})
                </Typography>
                <div className="space-y-2">
                  {files.map((file, idx) => (
                    <div
                      key={idx}
                      className="flex items-center justify-between p-2 rounded-lg bg-surface border border-border/70 hover:border-border text-xs group"
                    >
                      <div className="flex items-center gap-2 overflow-hidden mr-2">
                        <FileText className="size-4 text-accent shrink-0" />
                        <span className="truncate font-semibold text-foreground">
                          {file.name}
                        </span>
                        <span className="text-[9px] text-muted shrink-0">
                          ({(file.size / (1024 * 1024)).toFixed(2)} MB)
                        </span>
                      </div>
                      <Button
                        variant="ghost"
                        isIconOnly
                        size="sm"
                        className="text-muted hover:text-danger min-w-0 p-1"
                        onPress={() => removeFile(idx)}
                      >
                        <X className="size-3.5" />
                      </Button>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {files.length === 0 && (
              <div className="flex-1 flex flex-col items-center justify-center border border-border/50 rounded-xl bg-surface-secondary/10 py-6 select-none text-muted text-xs">
                <FileCheck2 className="size-6 mb-1 text-muted/60" />
                No documents uploaded yet
              </div>
            )}
          </div>

          <div className="p-4 border-t border-border shrink-0 w-full bg-surface">
            <Button
              type="submit"
              fullWidth
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-bold"
              isDisabled={files.length === 0 || isLoading}
              isPending={isLoading}
            >
              {isLoading ? (
                <Spinner color="current" size="sm" />
              ) : (
                "Submit Reclaim for legal compliance audit"
              )}
            </Button>
          </div>
        </Form>
      )}
    </Card>
  );
}
