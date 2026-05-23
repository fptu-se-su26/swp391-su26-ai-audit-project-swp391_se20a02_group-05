"use client"

import ShapeGrid from "../../components/reactbits/ShapeGrid"
import { Google } from '@thesvg/react';
import {
    Card, CardFooter, CardHeader, Link, Tabs,
    Typography, Button, CardContent, TextField,
    InputGroup, Input, ErrorMessage, Form, Label,
    FieldError, Checkbox
} from "@heroui/react";
import { Eye, EyeOff } from 'lucide-react';
import { useState } from "react";

export default function TestingPage() {
    const validateEmail = (email: string) => {
        return email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/);
    };

    const [value, setValue] = useState("");
    const [touched, setTouched] = useState(false);

    const isInvalid = touched && value.length > 0 && !validateEmail(value);
    const [isVisible, setIsVisible] = useState(false);

    // Selected Tab State
    const [selectedTab, setSelectedTab] = useState("overview");

    // Business Form State
    const [businessUsername, setBusinessUsername] = useState("");
    const [businessPassword, setBusinessPassword] = useState("");

    // Company Registration State
    const [showRegistration, setShowRegistration] = useState(false);
    const [companyName, setCompanyName] = useState("");
    const [taxCode, setTaxCode] = useState("");
    const [companyEmail, setCompanyEmail] = useState("");
    const [companyEmailTouched, setCompanyEmailTouched] = useState(false);
    const [acceptTerms, setAcceptTerms] = useState(false);

    const isCompanyEmailInvalid = companyEmailTouched && companyEmail.length > 0 && !validateEmail(companyEmail);

    const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const formData = new FormData(e.currentTarget);
        const data: Record<string, string> = {};
        formData.forEach((value, key) => {
            data[key] = value.toString();
        });
        alert(`Form submitted with: ${JSON.stringify(data, null, 2)}`);
    };

    const onReset = () => {
        setBusinessUsername("");
        setBusinessPassword("");
    };

    const onRegisterSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        alert(`Registering company: ${companyName}, Tax Code: ${taxCode}, Email: ${companyEmail}, Terms Accepted: ${acceptTerms}`);
        // Reset form fields
        setCompanyName("");
        setTaxCode("");
        setCompanyEmail("");
        setCompanyEmailTouched(false);
        setAcceptTerms(false);
        setSelectedTab("analytics");
        setShowRegistration(false);
    };

    return (
        <div className="flex flex-col h-screen w-screen overflow-hidden bg-[#f5f5f5]">
            <div className="relative flex-1 w-full overflow-hidden">
                <div className="absolute inset-0">
                    <ShapeGrid
                        speed={0.1}
                        squareSize={40}
                        direction="diagonal"
                        borderColor="#e6e6e6ff"
                        hoverFillColor="#f0f0f0ff"
                        shape="square"
                        hoverTrailAmount={5}
                    />
                </div>

                <div className="relative z-10 flex h-full w-full">
                    {/* Left Column */}
                    <div className="hidden xl:flex w-1/2 flex-col justify-center pl-12">
                        <div className="absolute top-12 left-12">
                            <img
                                src="/brand/logo&name.png"
                                alt="CVerify Logo"
                                className="h-10 w-auto"
                            />
                        </div>

                        <Typography.Prose>
                            <h2 className="text-[55px] font-bold mb-6 text-foreground">
                                Access Technical Truth
                            </h2>
                            <p className="text-2xl font-light tracking-tight mb-8 mr-24 text-muted">
                                Secure infrastructure for verifying professional identity and
                                engineering excellence through cryptographically-backed contribution analysis.
                            </p>
                        </Typography.Prose>
                    </div>

                    {/* Right Column */}
                    <div className="flex flex-1 flex-col items-center justify-center pl-6 lg:ml-6 mt-10 w-full relative">
                        <div className="w-full flex justify-center max-w-[85%] lg:max-w-[80%]">
                            {!showRegistration ? (
                                <Card className="w-full">
                                    <Tabs
                                        className="w-full"
                                        variant="secondary"
                                        selectedKey={selectedTab}
                                        onSelectionChange={(key) => setSelectedTab(key as string)}
                                    >
                                        <Tabs.ListContainer>
                                            <Tabs.List aria-label="Options" className="flex items-center gap-4 h-10">
                                                <Tabs.Tab id="overview" className="flex items-center justify-center h-full pb-3">
                                                    <Typography.Heading level={5}>Engineer</Typography.Heading>
                                                    <Tabs.Indicator className="bottom-0!" />
                                                </Tabs.Tab>
                                                <Tabs.Tab id="analytics" className="flex items-center justify-center h-full pb-3">
                                                    <Typography.Heading level={5}>Business</Typography.Heading>
                                                    <Tabs.Indicator className="!bottom-0!" />
                                                </Tabs.Tab>
                                            </Tabs.List>
                                        </Tabs.ListContainer>

                                        <Tabs.Panel className="pt-4 flex justify-center w-full" id="overview">
                                            {selectedTab === "overview" && (
                                                <Card variant="transparent" className="w-full max-w-[90%] flex flex-col items-center">
                                                    <CardHeader className="flex flex-col items-center text-center w-full">
                                                        <Card.Title className="text-2xl pb-4">Proof over promises</Card.Title>
                                                        <Card.Description className="text-md pb-12">
                                                            Evidence-backed profiles for modern engineering hiring.
                                                        </Card.Description>
                                                    </CardHeader>

                                                    <div className="w-full pb-3">
                                                        <Button variant="tertiary" size="lg" fullWidth className="h-12 rounded-2xl">
                                                            <Google /> Continue with Google
                                                        </Button>
                                                    </div>

                                                    <Typography type="body-sm" color="muted" className="pb-3">OR</Typography>

                                                    <CardFooter className="w-full flex flex-col items-center gap-6 p-0">
                                                        <TextField
                                                            fullWidth
                                                            isInvalid={isInvalid}
                                                            aria-label="Email Address"
                                                        >
                                                            <Input
                                                                className="h-12"
                                                                id="email"
                                                                type="email"
                                                                placeholder="Enter your email"
                                                                value={value}
                                                                aria-label="Email Address"
                                                                onChange={(e) => {
                                                                    setValue(e.target.value);
                                                                    setTouched(true);
                                                                }}
                                                                onBlur={() => setTouched(true)}
                                                            />
                                                            {isInvalid && (
                                                                <div className="text-left w-full mt-1">
                                                                    <ErrorMessage className="text-danger text-sm">
                                                                        Please enter a valid email address.
                                                                    </ErrorMessage>
                                                                </div>
                                                            )}
                                                        </TextField>

                                                        <Button
                                                            size="lg"
                                                            fullWidth
                                                            isDisabled={isInvalid || !value}
                                                            className="h-12 rounded-2xl"
                                                        >
                                                            Continue with email
                                                        </Button>
                                                    </CardFooter>
                                                </Card>
                                            )}
                                        </Tabs.Panel>

                                        <Tabs.Panel className="pt-4 flex justify-center w-full" id="analytics">
                                            {selectedTab === "analytics" && (
                                                <Card variant="transparent" className="w-full max-w-[90%] flex flex-col items-center">
                                                    <CardHeader className="flex flex-col items-center text-center w-full">
                                                        <Card.Title className="text-2xl pb-4">Hire beyond resumes</Card.Title>
                                                        <Card.Description className="text-md pb-12 w-full">
                                                            Verify engineering talent through real technical evidence.
                                                        </Card.Description>
                                                    </CardHeader>

                                                    <CardContent className="w-full pb-3 p-0">
                                                        <Form className="flex flex-col gap-6" onSubmit={onSubmit} onReset={onReset}>
                                                            <TextField
                                                                isRequired
                                                                name="username"
                                                                type="text"
                                                            >
                                                                <Label>Username</Label>
                                                                <Input
                                                                    placeholder="Enter your username"
                                                                    className="h-12"
                                                                    value={businessUsername}
                                                                    onChange={(e) => setBusinessUsername(e.target.value)}
                                                                />
                                                                <FieldError />
                                                            </TextField>

                                                            <TextField
                                                                isRequired
                                                                name="password"
                                                                type="password"
                                                            >
                                                                <Label>Password</Label>
                                                                <InputGroup>
                                                                    <InputGroup.Input
                                                                        className="h-12"
                                                                        type={isVisible ? "text" : "password"}
                                                                        placeholder="Enter your password"
                                                                        value={businessPassword}
                                                                        onChange={(e: any) => setBusinessPassword(e.target.value)}
                                                                    />
                                                                    <InputGroup.Suffix>
                                                                        <Button
                                                                            isIconOnly
                                                                            aria-label={isVisible ? "Hide password" : "Show password"}
                                                                            size="sm"
                                                                            variant="ghost"
                                                                            onPress={() => setIsVisible(!isVisible)}
                                                                        >
                                                                            {isVisible ? <Eye className="size-4" /> : <EyeOff className="size-4" />}
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
                                                                        <Label htmlFor="remember-me" className="text-muted">Remember me</Label>
                                                                    </Checkbox.Content>
                                                                </Checkbox>

                                                                <Link className="text-sm text-muted">
                                                                    Forgot password?
                                                                </Link>
                                                            </div>

                                                            <div className="flex gap-2">
                                                                <Button
                                                                    type="submit"
                                                                    fullWidth
                                                                    className="h-12 rounded-2xl"
                                                                    isDisabled={!businessUsername || !businessPassword}
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

                                                    <Typography type="body-sm" color="muted" className="pb-3 pt-3">OR</Typography>

                                                    <Typography
                                                        type="body-sm"
                                                        color="muted"
                                                    >
                                                        New to CVerify?{" "}
                                                        <Link
                                                            onPress={() => setShowRegistration(true)}
                                                            className="cursor-pointer"
                                                        >
                                                            Register your company<Link.Icon className="pt-1" />
                                                        </Link>
                                                    </Typography>
                                                </Card>
                                            )}
                                        </Tabs.Panel>
                                    </Tabs>
                                </Card>
                            ) : (
                                <Card className="w-full">
                                    <div className="w-full flex flex-col items-center">
                                        <CardHeader className="flex flex-col items-center text-center w-full">
                                            <Card.Title className="text-2xl pb-4 pt-6">Register your company</Card.Title>
                                            <Card.Description className="text-md pb-6">
                                                Enter your details below to request a CVerify business account.
                                            </Card.Description>
                                        </CardHeader>
                                        <CardContent className="w-full pb-3 px-6 lg:px-12 p-0">
                                            <Form className="flex flex-col gap-6" onSubmit={onRegisterSubmit}>
                                                <TextField
                                                    isRequired
                                                    name="companyName"
                                                    type="text"
                                                >
                                                    <Label>Company Name</Label>
                                                    <Input
                                                        placeholder="Enter company name"
                                                        className="h-12"
                                                        value={companyName}
                                                        onChange={(e) => setCompanyName(e.target.value)}
                                                    />
                                                    <FieldError />
                                                </TextField>

                                                <TextField
                                                    isRequired
                                                    name="taxCode"
                                                    type="text"
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
                                                    isInvalid={isCompanyEmailInvalid}
                                                >
                                                    <Label>Company Email</Label>
                                                    <Input
                                                        placeholder="Enter company email"
                                                        className="h-12"
                                                        value={companyEmail}
                                                        onChange={(e) => {
                                                            setCompanyEmail(e.target.value);
                                                            setCompanyEmailTouched(true);
                                                        }}
                                                        onBlur={() => setCompanyEmailTouched(true)}
                                                    />
                                                    {isCompanyEmailInvalid && (
                                                        <div className="text-left w-full mt-1">
                                                            <ErrorMessage className="text-danger text-sm">
                                                                Please enter a valid company email address.
                                                            </ErrorMessage>
                                                        </div>
                                                    )}
                                                </TextField>

                                                <div className="flex items-center pb-2">
                                                    <Checkbox
                                                        id="terms-agreement"
                                                        isSelected={acceptTerms}
                                                        onChange={setAcceptTerms}
                                                    >
                                                        <Checkbox.Control>
                                                            <Checkbox.Indicator />
                                                        </Checkbox.Control>
                                                        <Checkbox.Content>
                                                            <Label htmlFor="terms-agreement" className="text-muted">
                                                                I agree to the{" "}
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

                                                <div className="flex gap-4 pt-2 pb-6">
                                                    <Button
                                                        type="button"
                                                        variant="secondary"
                                                        fullWidth
                                                        className="h-12 rounded-2xl"
                                                        onPress={() => {
                                                            setSelectedTab("analytics");
                                                            setShowRegistration(false);
                                                        }}
                                                    >
                                                        Back to Sign In
                                                    </Button>
                                                    <Button
                                                        type="submit"
                                                        fullWidth
                                                        className="h-12 rounded-2xl"
                                                        isDisabled={isCompanyEmailInvalid || !companyName || !taxCode || !companyEmail || !acceptTerms}
                                                    >
                                                        Register
                                                    </Button>
                                                </div>
                                            </Form>
                                        </CardContent>
                                    </div>
                                </Card>
                            )}
                        </div>
                    </div>

                    {/* Protocol Tag Badge */}
                    <div className="absolute top-12 right-12">
                        <Typography.Heading level={6} color="muted" className="tracking-widest font-mono">
                            PROTOCOL V1.0.0
                        </Typography.Heading>
                    </div>
                </div>
            </div>

            <footer
                className="w-full border-t border-divider flex items-center justify-between px-12 shrink-0 bg-[#f5f5f5]"
                style={{ height: '10vh' }}
            >
                <div className="text-foreground text-sm">
                    <p>CVERIFY © 2026. REAL COMMITS. REAL CAREER</p>
                </div>
                <div className="flex gap-8 text-sm">
                    <Link className="text-foreground" href="/privacy-policy">Privacy Policy<Link.Icon /></Link>
                    <Link className="text-foreground" href="/terms-of-service">Terms of Service<Link.Icon /></Link>
                    <Link className="text-foreground" href="/contact">Contact<Link.Icon /></Link>
                    <Link className="text-foreground" href="/system-status">System Status<Link.Icon /></Link>
                </div>
            </footer>
        </div>
    )
}