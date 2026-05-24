"use client";

import React, { useState, useEffect, useCallback } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "../../../features/auth/hooks/use-auth";
import { Google } from "@thesvg/react";
import {
  Card,
  Tabs,
  Typography,
  Button,
  TextField,
  InputGroup,
  Input,
  ErrorMessage,
  Form,
  Label,
  FieldError,
  Checkbox,
  toast,
  Spinner,
  CardHeader,
  CardContent,
  Link,
} from "@heroui/react";
import { Eye, EyeOff } from "lucide-react";
import Script from "next/script";
import { Suspense } from "react";

interface GoogleIdentityResponse {
  credential?: string;
}

interface CustomWindow extends Window {
  google?: {
    accounts?: {
      id?: {
        initialize: (options: {
          client_id: string;
          callback: (response: GoogleIdentityResponse) => void;
        }) => void;
        renderButton: (
          parent: HTMLElement,
          options: {
            theme?: string;
            size?: string;
            width?: number;
            text?: string;
          },
        ) => void;
      };
    };
  };
  __googleIdentityListener?: (response: GoogleIdentityResponse) => void;
  __googleIdentityInitialized?: boolean;
}

function LoginContent() {
  const router = useRouter();
  const {
    loginWithGoogle,
    login,
    sendOtp,
    resolveEmailAuthState,
    companyLogin,
  } = useAuth();

  // Engineer state
  const [email, setEmail] = useState("");
  const [emailTouched, setEmailTouched] = useState(false);
  const [isEmailLoading, setIsEmailLoading] = useState(false);

  // Engineer identity flow phase: email input → password login (if credentials exist)
  type EmailFlowPhase = "EMAIL_INPUT" | "PASSWORD_LOGIN";
  const [emailFlowPhase, setEmailFlowPhase] =
    useState<EmailFlowPhase>("EMAIL_INPUT");
  const [engineerPassword, setEngineerPassword] = useState("");
  const [isEngineerPasswordVisible, setIsEngineerPasswordVisible] =
    useState(false);
  const [isPasswordLoginLoading, setIsPasswordLoginLoading] = useState(false);

  const validateEmail = (val: string) => {
    return val.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/);
  };
  const isEmailInvalid =
    emailTouched && email.length > 0 && !validateEmail(email);

  // Business state
  const [selectedTab, setSelectedTab] = useState("overview");
  const [businessUsername, setBusinessUsername] = useState("");
  const [businessPassword, setBusinessPassword] = useState("");
  const [isVisible, setIsVisible] = useState(false);
  const [isBusinessLoading, setIsBusinessLoading] = useState(false);

  // Google SSO logic
  const handleGoogleCredentialResponse = useCallback(
    async (response: GoogleIdentityResponse) => {
      try {
        if (!response.credential) return;
        const result = await loginWithGoogle(response.credential);

        if (result.success) {
          if (result.isUnverified || result.nextStep === "VERIFY_EMAIL") {
            toast.warning("Verification Pending", {
              description: "Please check your email to complete verification.",
            });
            router.push("/verify-email");
            return;
          }

          if (result.user) {
            toast.success("Welcome to CVerify!", {
              description: "Successfully logged in via Google SSO.",
            });
            // Navigation is handled by AuthOrchestrator (respects callbackUrl)
          }
        } else if (result.error) {
          toast.danger("Google Login Failed", {
            description: result.error.message,
          });
        }
      } catch {
        toast.danger("Google SSO Failed", {
          description:
            "An unexpected error occurred during Google authentication.",
        });
      }
    },
    [loginWithGoogle, router],
  );

  const initializeGoogleSignIn = useCallback(() => {
    const customWindow =
      typeof window !== "undefined"
        ? (window as unknown as CustomWindow)
        : null;
    if (customWindow?.google?.accounts?.id) {
      customWindow.__googleIdentityListener = handleGoogleCredentialResponse;

      if (!customWindow.__googleIdentityInitialized) {
        customWindow.google.accounts.id.initialize({
          client_id:
            process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID ||
            "your_google_client_id_here",
          callback: (response: GoogleIdentityResponse) => {
            if (typeof customWindow.__googleIdentityListener === "function") {
              customWindow.__googleIdentityListener(response);
            }
          },
        });
        customWindow.__googleIdentityInitialized = true;
      }

      const container = document.getElementById("google-signin-button");
      if (container) {
        container.innerHTML = "";
        customWindow.google.accounts.id.renderButton(container, {
          theme: "outline",
          size: "large",
          width: 390,
          text: "continue_with",
        });
      }
    }
  }, [handleGoogleCredentialResponse]);

  useEffect(() => {
    initializeGoogleSignIn();
  }, [initializeGoogleSignIn, selectedTab]);

  const handleContinueWithEmail = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email || !validateEmail(email)) return;

    setIsEmailLoading(true);

    // Phase 1: Resolve identity state from backend
    const stateResult = await resolveEmailAuthState(email);
    setIsEmailLoading(false);

    if (!stateResult.success || !stateResult.data) {
      toast.danger("Failed to resolve identity", {
        description: stateResult.error?.message || "An error occurred.",
      });
      return;
    }

    const { authState } = stateResult.data;

    switch (authState) {
      case "REQUIRES_AUTHENTICATION":
        // User has password credentials — show inline password form
        setEmailFlowPhase("PASSWORD_LOGIN");
        break;

      case "REQUIRES_ONBOARDING": {
        // New user or Google-only — trigger OTP onboarding
        setIsEmailLoading(true);
        const otpResult = await sendOtp(email, "Authentication");
        setIsEmailLoading(false);
        if (otpResult.success && otpResult.data) {
          toast.success("OTP Code Sent", {
            description: `Please check your email: ${email} for the 6-digit verification code.`,
          });
          router.push(
            `/continue-with-email?email=${encodeURIComponent(email)}&challengeId=${otpResult.data.challengeId}`,
          );
        } else {
          toast.danger("Failed to send OTP", {
            description: otpResult.error?.message || "An error occurred.",
          });
        }
        break;
      }

      case "REQUIRES_VERIFICATION":
        toast.warning("Verification Pending", {
          description: "Please check your email to complete verification.",
        });
        router.push("/verify-email");
        break;

      case "ACCOUNT_RESTRICTED":
        toast.danger("Account Restricted", {
          description:
            "This account has been restricted. Please contact support.",
        });
        break;
    }
  };

  const handlePasswordLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email || !engineerPassword) return;

    setIsPasswordLoginLoading(true);
    const result = await login({
      email,
      password: engineerPassword,
      rememberMe: false,
    });
    setIsPasswordLoginLoading(false);

    if (result.success) {
      if (result.isUnverified || result.nextStep === "VERIFY_EMAIL") {
        toast.warning("Verification Pending", {
          description: "Please check your email to complete verification.",
        });
        router.push("/verify-email");
        return;
      }
      toast.success("Welcome back!", {
        description: "Successfully logged in.",
      });
      // Navigation is handled by AuthOrchestrator (respects callbackUrl)
    } else {
      toast.danger("Login Failed", {
        description: result.error?.message || "Invalid email or password.",
      });
    }
  };

  const handleBusinessSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!businessUsername || !businessPassword) return;

    setIsBusinessLoading(true);
    const result = await companyLogin({
      organizationUsername: businessUsername,
      password: businessPassword,
    });
    setIsBusinessLoading(false);

    if (result.success) {
      toast.success("Workspace authenticated", {
        description: `Logged in to organization: ${businessUsername}`,
      });
      // Navigation is handled by AuthOrchestrator (respects callbackUrl)
    } else {
      toast.danger("Authentication Failed", {
        description:
          result.error?.message || "Invalid workspace username or password.",
      });
    }
  };

  const handleBusinessReset = () => {
    setBusinessUsername("");
    setBusinessPassword("");
  };

  return (
    <>
      <Script
        src="https://accounts.google.com/gsi/client"
        strategy="lazyOnload"
        onLoad={initializeGoogleSignIn}
      />
      <Card className="w-full">
        <Tabs
          className="w-full"
          variant="secondary"
          selectedKey={selectedTab}
          onSelectionChange={(key) => setSelectedTab(key as string)}
        >
          <Tabs.ListContainer>
            <Tabs.List
              aria-label="Options"
              className="flex items-center gap-4 h-10"
            >
              <Tabs.Tab
                id="overview"
                className="flex items-center justify-center h-full pb-3"
              >
                <Typography.Heading level={5}>Engineer</Typography.Heading>
                <Tabs.Indicator className="bottom-0!" />
              </Tabs.Tab>
              <Tabs.Tab
                id="bussiness"
                className="flex items-center justify-center h-full pb-3"
              >
                <Typography.Heading level={5}>Business</Typography.Heading>
                <Tabs.Indicator className="!bottom-0!" />
              </Tabs.Tab>
            </Tabs.List>
          </Tabs.ListContainer>

          <Tabs.Panel className="pt-6 flex justify-center w-full" id="overview">
            {selectedTab === "overview" && (
              <Card
                variant="transparent"
                className="w-full max-w-[90%] flex flex-col items-center"
              >
                <CardHeader className="flex flex-col items-center text-center w-full">
                  <Card.Title className="text-2xl pb-4">
                    Proof over promises
                  </Card.Title>
                  <Card.Description className="text-md pb-12">
                    Evidence-backed profiles for modern engineering hiring.
                  </Card.Description>
                </CardHeader>

                <div className="w-full pb-3 relative overflow-hidden rounded-2xl group">
                  {/* Invisible Google Sign In Button container overlay */}
                  <div
                    id="google-signin-button"
                    className="absolute inset-0 opacity-[0.01] z-10 cursor-pointer overflow-hidden flex justify-center items-center [&_iframe]:w-full [&_iframe]:h-full [&_iframe]:scale-[2.5] [&_iframe]:origin-center"
                    style={{ minHeight: "48px" }}
                  />
                  <Button
                    variant="tertiary"
                    size="lg"
                    fullWidth
                    className="h-12 rounded-2xl transition-all duration-200 group-hover:opacity-90 group-active:scale-[0.98]"
                  >
                    <Google /> Continue with Google
                  </Button>
                </div>

                <Typography type="body-sm" color="muted" className="pb-3">
                  OR
                </Typography>

                {emailFlowPhase === "EMAIL_INPUT" ? (
                  <Form
                    onSubmit={handleContinueWithEmail}
                    className="w-full flex flex-col items-center gap-6 p-0"
                  >
                    <TextField
                      fullWidth
                      isInvalid={isEmailInvalid}
                      aria-label="Email Address"
                    >
                      <Input
                        className="h-12"
                        id="email"
                        type="email"
                        placeholder="Enter your email"
                        value={email}
                        aria-label="Email Address"
                        onChange={(e) => {
                          setEmail(e.target.value);
                          setEmailTouched(true);
                        }}
                        onBlur={() => setEmailTouched(true)}
                      />
                      {isEmailInvalid && (
                        <div className="text-left w-full mt-1">
                          <ErrorMessage className="text-danger text-sm">
                            Please enter a valid email address.
                          </ErrorMessage>
                        </div>
                      )}
                    </TextField>

                    <Button
                      type="submit"
                      size="lg"
                      fullWidth
                      isDisabled={isEmailInvalid || !email || isEmailLoading}
                      isPending={isEmailLoading}
                      className="h-12 rounded-2xl"
                    >
                      {isEmailLoading && <Spinner color="current" size="sm" />}
                      Continue with email
                    </Button>
                  </Form>
                ) : (
                  <Form
                    onSubmit={handlePasswordLogin}
                    className="w-full flex flex-col items-center gap-6 p-0"
                  >
                    <TextField fullWidth aria-label="Email Address">
                      <Input
                        className="h-12 bg-zinc-50 dark:bg-zinc-900 opacity-70"
                        id="email-locked"
                        type="email"
                        value={email}
                        readOnly
                      />
                    </TextField>

                    <TextField
                      isRequired
                      name="password"
                      type="password"
                      fullWidth
                    >
                      <Label>Password</Label>
                      <InputGroup>
                        <InputGroup.Input
                          className="h-12"
                          type={isEngineerPasswordVisible ? "text" : "password"}
                          placeholder="Enter your password"
                          value={engineerPassword}
                          onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                            setEngineerPassword(e.target.value)
                          }
                          autoFocus
                        />
                        <InputGroup.Suffix>
                          <Button
                            isIconOnly
                            aria-label={
                              isEngineerPasswordVisible
                                ? "Hide password"
                                : "Show password"
                            }
                            size="sm"
                            variant="ghost"
                            onPress={() =>
                              setIsEngineerPasswordVisible(
                                !isEngineerPasswordVisible,
                              )
                            }
                          >
                            {isEngineerPasswordVisible ? (
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
                      size="lg"
                      fullWidth
                      isDisabled={!engineerPassword || isPasswordLoginLoading}
                      isPending={isPasswordLoginLoading}
                      className="h-12 rounded-2xl"
                    >
                      {isPasswordLoginLoading && (
                        <Spinner color="current" size="sm" />
                      )}
                      Sign In
                    </Button>

                    <div className="flex items-center justify-between w-full">
                      <button
                        type="button"
                        onClick={() => {
                          setEmailFlowPhase("EMAIL_INPUT");
                          setEngineerPassword("");
                        }}
                        className="text-sm text-muted hover:underline cursor-pointer bg-transparent border-0 p-0"
                      >
                        ← Use different email
                      </button>
                      <Link
                        href={`/forgot-password?email=${encodeURIComponent(email)}`}
                        className="text-sm text-muted hover:underline cursor-pointer"
                      >
                        Forgot password?
                      </Link>
                    </div>
                  </Form>
                )}
              </Card>
            )}
          </Tabs.Panel>

          <Tabs.Panel
            className="pt-4 flex justify-center w-full"
            id="bussiness"
          >
            {selectedTab === "bussiness" && (
              <Card
                variant="transparent"
                className="w-full max-w-[90%] flex flex-col items-center"
              >
                <CardHeader className="flex flex-col items-center text-center w-full">
                  <Card.Title className="text-2xl pb-4">
                    Hire beyond resumes
                  </Card.Title>
                  <Card.Description className="text-md pb-12 w-full">
                    Verify engineering talent through real technical evidence.
                  </Card.Description>
                </CardHeader>

                <CardContent className="w-full pb-3 p-0">
                  <Form
                    className="flex flex-col gap-6"
                    onSubmit={handleBusinessSubmit}
                    onReset={handleBusinessReset}
                  >
                    <TextField isRequired name="username" type="text">
                      <Label>Username</Label>
                      <Input
                        placeholder="Enter your username"
                        className="h-12"
                        value={businessUsername}
                        onChange={(e) => setBusinessUsername(e.target.value)}
                      />
                      <FieldError />
                    </TextField>

                    <TextField isRequired name="password" type="password">
                      <Label>Password</Label>
                      <InputGroup>
                        <InputGroup.Input
                          className="h-12"
                          type={isVisible ? "text" : "password"}
                          placeholder="Enter your password"
                          value={businessPassword}
                          onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                            setBusinessPassword(e.target.value)
                          }
                        />
                        <InputGroup.Suffix>
                          <Button
                            isIconOnly
                            aria-label={
                              isVisible ? "Hide password" : "Show password"
                            }
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
                      <FieldError />
                    </TextField>

                    <div className="flex items-center justify-between">
                      <Checkbox id="remember-me">
                        <Checkbox.Control>
                          <Checkbox.Indicator />
                        </Checkbox.Control>
                        <Checkbox.Content>
                          <Label
                            htmlFor="remember-me"
                            className="text-muted cursor-pointer"
                          >
                            Remember me
                          </Label>
                        </Checkbox.Content>
                      </Checkbox>

                      <Link
                        href="/forgot-password"
                        className="text-sm text-muted hover:underline cursor-pointer"
                      >
                        Forgot password?
                      </Link>
                    </div>

                    <div className="flex gap-2">
                      <Button
                        type="submit"
                        fullWidth
                        className="h-12 rounded-2xl"
                        isDisabled={
                          !businessUsername ||
                          !businessPassword ||
                          isBusinessLoading
                        }
                        isPending={isBusinessLoading}
                      >
                        Sign In
                      </Button>
                      <Button
                        type="reset"
                        variant="secondary"
                        fullWidth
                        className="h-12 rounded-2xl"
                      >
                        Reset
                      </Button>
                    </div>
                  </Form>
                </CardContent>

                <Typography type="body-sm" color="muted" className="pb-3 pt-3">
                  OR
                </Typography>

                <Typography type="body-sm" color="muted">
                  New to CVerify?{" "}
                  <Link
                    href="/company-verification"
                    className="cursor-pointer font-semibold text-zinc-900 dark:text-zinc-100 hover:underline"
                  >
                    Register your company
                    <Link.Icon className="pt-1" />
                  </Link>
                </Typography>
              </Card>
            )}
          </Tabs.Panel>
        </Tabs>
      </Card>
    </>
  );
}

export default function LoginPage() {
  return (
    <Suspense
      fallback={
        <div className="flex items-center justify-center p-8 min-h-[400px]">
          <div className="w-8 h-8 border-2 border-t-zinc-900 border-zinc-200 dark:border-t-zinc-100 rounded-full animate-spin" />
        </div>
      }
    >
      <LoginContent />
    </Suspense>
  );
}
