"use client"

import ShapeGrid from "../../components/reactbits/ShapeGrid"
import { Google } from '@thesvg/react';
import { Card, CardFooter, CardHeader, Link, Tabs, Typography, Button, CardContent, TextField, Input, ErrorMessage } from "@heroui/react";
import { useState } from "react";

export default function TestingPage() {
    const validateEmail = (email: string) => {
        return email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/);
    };

    const [value, setValue] = useState("");
    const [touched, setTouched] = useState(false);

    const isInvalid = touched && value.length > 0 && !validateEmail(value);
    const errorMessage = isInvalid ? "" : "";

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
                    <div className="hidden xl:flex w-1/2 flex-col justify-center pl-12">
                        <div className="absolute top-12 left-12">
                            <img
                                src="/brand/logo&name.png"
                                alt="CVerify Logo"
                                className="h-10 w-auto"
                            />
                        </div>

                        <Typography.Prose>
                            <h2 className="text-[55px] font-bold mb-6">
                                Access Technical Truth
                            </h2>
                            <p className="text-2xl font-light tracking-tight mb-8 mr-24">
                                Secure infrastructure for verifying professional identity and
                                engineering excellence through cryptographically-backed contribution analysis.
                            </p>
                        </Typography.Prose>
                        <div className="flex gap-4">
                            <div className="border border-zinc-200 px-3 py-1 rounded">✓ SOC2 TYPE II</div>
                            <div className="border border-zinc-200 bg-white/50 px-3 py-1 rounded text-sm font-medium">🔒 END-TO-END ENCRYPTION</div>
                        </div>
                    </div>

                    <div className="flex flex-1 flex-col items-center justify-center pl-6 lg:ml-6">
                        <Card className="w-full max-w-[85%] lg:max-w-[80%]">
                            <Tabs className="w-full" variant="secondary" >
                                <Tabs.ListContainer>
                                    <Tabs.List aria-label="Options" className="flex items-center gap-4 h-10">
                                        <Tabs.Tab id="overview" className="flex items-center justify-center h-full pb-3">
                                            <Typography.Heading level={5}>Engineer</Typography.Heading>
                                            <Tabs.Indicator className="bottom-0!" />
                                        </Tabs.Tab>
                                        <Tabs.Tab id="analytics" className="flex items-center justify-center h-full pb-3">
                                            <Typography.Heading level={5}>Enterprise</Typography.Heading>
                                            <Tabs.Indicator className="!bottom-0!" />
                                        </Tabs.Tab>
                                    </Tabs.List>
                                </Tabs.ListContainer>
                                <Tabs.Panel className="pt-4 flex justify-center" id="overview">
                                    <Card variant="transparent" className="w-full max-w-[90%] flex flex-col items-center">
                                        <CardHeader className="flex flex-col items-center text-center">
                                            <Card.Title className="text-2xl pb-4">Proof over promises</Card.Title>
                                            <Card.Description className="text-md pb-12">
                                                Evidence-backed profiles for modern engineering hiring.
                                            </Card.Description>
                                            <CardContent className="w-full pb-6">
                                                <Button variant="tertiary" size="lg" fullWidth><Google /> Continue with Google</Button>
                                            </CardContent>
                                            <Typography type="body-sm" color="muted" className="pb-6">OR</Typography>
                                            <CardFooter className="w-full flex flex-col items-center gap-6">
                                                <TextField
                                                    fullWidth
                                                    isInvalid={isInvalid}
                                                    aria-label="Email Address"
                                                >
                                                    <Input
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
                                                        <div className="text-left w-full">
                                                            <ErrorMessage className="text-danger text-sm">
                                                                Please enter a valid email address.
                                                            </ErrorMessage>
                                                        </div>
                                                    )}
                                                </TextField>

                                                <Button
                                                    size="lg"
                                                    fullWidth
                                                    isDisabled={isInvalid}
                                                >
                                                    Continue with email
                                                </Button>
                                            </CardFooter>
                                        </CardHeader>
                                    </Card>
                                </Tabs.Panel>
                                <Tabs.Panel className="pt-4" id="analytics">
                                    <p>Track your metrics and analyze performance data.</p>
                                </Tabs.Panel>
                            </Tabs>
                        </Card>
                    </div>

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
                    <p>CVERIFY © 2026.  REAL COMMITS. REAL CAREER</p>
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