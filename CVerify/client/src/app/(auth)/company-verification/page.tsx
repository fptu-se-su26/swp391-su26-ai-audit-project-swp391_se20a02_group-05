"use client";

import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '../../../features/auth/hooks/use-auth';
import {
  Card, Typography, Button, TextField, CardHeader, CardContent,
  Input, Form, Label, FieldError, Checkbox, toast, Spinner, Link
} from "@heroui/react";
import { Building2, ArrowLeft } from 'lucide-react';

export default function CompanyVerificationPage() {
  const router = useRouter();
  const { registerCompany } = useAuth();

  // Form states
  const [companyName, setCompanyName] = useState("");
  const [taxCode, setTaxCode] = useState("");
  const [companyEmail, setCompanyEmail] = useState("");
  const [emailTouched, setEmailTouched] = useState(false);
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  const validateEmail = (val: string) => {
    return val.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/);
  };
  const isEmailInvalid = emailTouched && companyEmail.length > 0 && !validateEmail(companyEmail);
  const isTaxCodeInvalid = taxCode.length > 0 && !taxCode.match(/^\d{10}$/);

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!companyName || !taxCode || !companyEmail || !acceptTerms) return;
    if (isEmailInvalid || isTaxCodeInvalid) return;

    setIsLoading(true);
    const result = await registerCompany({
      companyName,
      taxCode,
      companyEmail,
      agreeTerms: acceptTerms
    });
    setIsLoading(false);

    if (result.success) {
      setIsSuccess(true);
      toast.success("Registration Successful", {
        description: "An email verification link has been sent to your company email."
      });
    } else {
      toast.danger("Registration Failed", {
        description: result.error?.message || "Verify your details or check if tax code is already registered."
      });
    }
  };

  return (
    <Card className="w-full">
      {!isSuccess ? (
        <div className="w-full flex flex-col items-center">
          <div className="w-12 h-12 bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center rounded-xl mb-6">
            <Building2 className="size-6 text-zinc-900 dark:text-zinc-100" />
          </div>

          <CardHeader className="flex flex-col items-center text-center w-full">
            <Card.Title className="text-2xl pb-4 pt-6">Register your company</Card.Title>
            <Card.Description className="text-md pb-6">
              Enter your details below to request a CVerify business account.
            </Card.Description>
          </CardHeader>

          <CardContent className="w-full pb-3 px-6 lg:px-12 p-0">
            <Form className="flex flex-col gap-6" onSubmit={handleRegister}>
              <TextField
                isRequired
                name="companyName"
                type="text"
              >
                <Label>Company Name</Label>
                <Input
                  placeholder="Enter official company name"
                  className="h-12"
                  maxLength={10}
                  value={taxCode}
                  onChange={(e) => setTaxCode(e.target.value.replace(/\D/g, ''))}
                />
                <FieldError />
              </TextField>
              <TextField
                isRequired
                name="taxCode"
                type="text"
                value={taxCode}
                isInvalid={isTaxCodeInvalid}
                validate={() => isTaxCodeInvalid ? "Tax code must be exactly 10 digits." : null}
              >
                <Label>Tax Code</Label>
                <Input
                  placeholder="Enter your 10-digit tax code"
                  className="h-12"
                  value={taxCode}
                  onChange={(e) => setTaxCode(e.target.value)}
                />
                <FieldError />
              </TextField>
              <TextField
                isRequired
                name="companyEmail"
                type="email"
                isInvalid={isEmailInvalid}
                validate={() => isEmailInvalid ? "Please enter a valid company email address." : null}
              >
                <Label>Company Email</Label>
                <Input
                  placeholder="Enter company email"
                  className="h-12"
                  value={companyEmail}
                  onChange={(e) => {
                    setCompanyEmail(e.target.value);
                    setEmailTouched(true);
                  }}
                  onBlur={() => setEmailTouched(true)}
                />
                <FieldError />
              </TextField>
              <div className="flex items-center">
                <Checkbox
                  id="terms-agreement"
                  isSelected={acceptTerms}
                  onChange={setAcceptTerms}
                >
                  <Checkbox.Control>
                    <Checkbox.Indicator />
                  </Checkbox.Control>
                  <Checkbox.Content>
                    <Label htmlFor="terms-agreement" className="text-muted cursor-pointer">
                      I agree to CVerify's{" "}
                      <Link href="/terms-of-service" className="text-sm inline">
                        Terms of Service<Link.Icon />
                      </Link>{" "}
                      and{" "}
                      <Link href="/privacy-policy" className="text-sm inline">
                        Privacy Policy<Link.Icon />
                      </Link>
                    </Label>
                  </Checkbox.Content>
                </Checkbox>
              </div>
              <div className="flex gap-4 pb-6">
                <Button
                  type="button"
                  variant="secondary"
                  fullWidth
                  className="h-12 rounded-2xl"
                  onPress={() => router.push('/login')}
                >
                  Back to Sign In
                </Button>
                <Button
                  type="submit"
                  fullWidth
                  className="h-12 rounded-2xl"
                  isDisabled={isEmailInvalid || isTaxCodeInvalid || !companyName || !taxCode || !companyEmail || !acceptTerms || isLoading}
                  isPending={isLoading}
                >
                  Register
                </Button>
              </div>
            </Form>
          </CardContent>
        </div>
      ) : (
        <div className="w-full flex flex-col items-center py-6 text-center">
          <div className="w-16 h-16 bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center rounded-2xl mb-6">
            <Building2 className="size-8 text-zinc-900 dark:text-zinc-100" />
          </div>

          <Typography.Heading level={3} className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100">
            Verification link sent!
          </Typography.Heading>

          <Typography className="text-sm text-zinc-500 dark:text-zinc-400 mb-8 max-w-sm">
            We have sent a secure authorization link to <span className="font-semibold text-zinc-700 dark:text-zinc-300">{companyEmail}</span>. Please click the link inside to complete verification and set up your workspace.
          </Typography>

          <Button
            variant="secondary"
            className="h-12 rounded-xl text-zinc-800 dark:text-zinc-200 px-6"
            onPress={() => router.push('/login')}
          >
            <ArrowLeft className="size-4 mr-2" /> Back to Sign In
          </Button>
        </div>
      )}
    </Card>
  );
}
