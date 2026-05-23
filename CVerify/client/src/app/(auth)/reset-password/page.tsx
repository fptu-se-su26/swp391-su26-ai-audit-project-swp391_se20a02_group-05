"use client";

import React, { useState, useEffect } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import { useAuth } from '../../../features/auth/hooks/use-auth';
import {
    Card, Typography, Button, TextField,
    InputGroup, Input, Form, Label, toast, Spinner
} from "@heroui/react";
import { KeyRound, ShieldCheck, Eye, EyeOff } from 'lucide-react';
import { Suspense } from 'react';

function ResetPasswordContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token');
  const { resetPassword } = useAuth();

  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isVisible, setIsVisible] = useState(false);
  const [isConfirmVisible, setIsConfirmVisible] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  useEffect(() => {
    if (!token) {
      toast.danger("Invalid Link", {
        description: "Password reset token is missing. Please check your recovery email."
      });
    }
  }, [token]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token || !password || password.length < 8) return;
    if (password !== confirmPassword) {
      toast.danger("Mismatch", { description: "Passwords do not match." });
      return;
    }

    setIsLoading(true);
    const result = await resetPassword({
      token,
      password,
      confirmPassword,
    });
    setIsLoading(false);

    if (result.success) {
      setIsSuccess(true);
      toast.success("Password Updated", {
        description: "Your credential has been rotated. Redirecting to workspace..."
      });
      setTimeout(() => {
        router.push('/');
      }, 2000);
    } else {
      toast.danger("Reset Failed", {
        description: result.error?.message || "Failed to update your password."
      });
    }
  };

  return (
    <Card className="w-full bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 p-8 shadow-xl rounded-2xl">
      {!isSuccess ? (
        <div className="w-full flex flex-col items-center">
          <div className="w-12 h-12 bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center rounded-xl mb-6">
            <KeyRound className="size-6 text-zinc-900 dark:text-zinc-100" />
          </div>

          <div className="text-center w-full mb-8 font-outfit">
            <Typography.Heading level={3} className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100">
              Reset Password
            </Typography.Heading>
            <Typography className="text-sm text-zinc-500 dark:text-zinc-400">
              Establish a new password for your verified CVerify profile.
            </Typography>
          </div>

          <Form className="w-full flex flex-col gap-5" onSubmit={handleSubmit}>
            <TextField isRequired name="password" type="password">
              <Label className="text-sm font-medium text-zinc-700 dark:text-zinc-300 pb-1">New Password</Label>
              <InputGroup>
                <InputGroup.Input
                  className="h-12"
                  type={isVisible ? "text" : "password"}
                  placeholder="Enter new password (min 8 chars)"
                  value={password}
                  onChange={(e: any) => setPassword(e.target.value)}
                  disabled={isLoading || !token}
                />
                <InputGroup.Suffix>
                  <Button
                    isIconOnly
                    variant="ghost"
                    size="sm"
                    className="text-zinc-400 hover:bg-transparent"
                    onPress={() => setIsVisible(!isVisible)}
                    isDisabled={isLoading || !token}
                  >
                    {isVisible ? <Eye className="size-4" /> : <EyeOff className="size-4" />}
                  </Button>
                </InputGroup.Suffix>
              </InputGroup>
            </TextField>

            <TextField isRequired name="confirmPassword" type="password">
              <Label className="text-sm font-medium text-zinc-700 dark:text-zinc-300 pb-1">Confirm Password</Label>
              <InputGroup>
                <InputGroup.Input
                  className="h-12"
                  type={isConfirmVisible ? "text" : "password"}
                  placeholder="Repeat your new password"
                  value={confirmPassword}
                  onChange={(e: any) => setConfirmPassword(e.target.value)}
                  disabled={isLoading || !token}
                />
                <InputGroup.Suffix>
                  <Button
                    isIconOnly
                    variant="ghost"
                    size="sm"
                    className="text-zinc-400 hover:bg-transparent"
                    onPress={() => setIsConfirmVisible(!isConfirmVisible)}
                    isDisabled={isLoading || !token}
                  >
                    {isConfirmVisible ? <Eye className="size-4" /> : <EyeOff className="size-4" />}
                  </Button>
                </InputGroup.Suffix>
              </InputGroup>
            </TextField>

            <Button
              type="submit"
              fullWidth
              isPending={isLoading}
              isDisabled={!password || password.length < 8 || password !== confirmPassword || isLoading || !token}
              className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-semibold mt-2 flex items-center justify-center gap-2"
            >
              {isLoading && <Spinner color="current" size="sm" />}
              Update password
            </Button>
          </Form>
        </div>
      ) : (
        <div className="w-full flex flex-col items-center py-6 text-center">
          <div className="w-16 h-16 bg-emerald-50 dark:bg-emerald-950 flex items-center justify-center rounded-2xl mb-6">
            <ShieldCheck className="size-8 text-emerald-600 dark:text-emerald-400" />
          </div>

          <Typography.Heading level={3} className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100">
            Password Updated
          </Typography.Heading>
          
          <Typography className="text-sm text-zinc-500 dark:text-zinc-400 mb-8 max-w-sm">
            Your password credential has been rotated successfully. You are being navigated to your CVerify workspace...
          </Typography>

          <div className="w-8 h-8 border-2 border-t-zinc-900 border-zinc-200 dark:border-t-zinc-100 rounded-full animate-spin" />
        </div>
      )}
    </Card>
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center p-8 min-h-[400px]">
        <div className="w-8 h-8 border-2 border-t-zinc-900 border-zinc-200 dark:border-t-zinc-100 rounded-full animate-spin" />
      </div>
    }>
      <ResetPasswordContent />
    </Suspense>
  );
}
