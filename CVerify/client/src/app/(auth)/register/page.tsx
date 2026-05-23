"use client";

import React, { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Card, Typography, Button } from "@heroui/react";
import { Sparkles } from 'lucide-react';

export default function RegisterPage() {
  const router = useRouter();

  useEffect(() => {
    // SNAPPY redirect to unified sign-in flow after 1.5 seconds
    const timer = setTimeout(() => {
      router.push('/login');
    }, 1500);
    return () => clearTimeout(timer);
  }, [router]);

  return (
    <Card className="w-full bg-white dark:bg-zinc-900 border border-zinc-200 dark:border-zinc-800 p-8 shadow-xl rounded-2xl text-center select-none">
      <div className="w-12 h-12 bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center rounded-xl mb-6 mx-auto">
        <Sparkles className="size-6 text-zinc-900 dark:text-zinc-100 animate-pulse" />
      </div>

      <Typography.Heading level={3} className="text-2xl font-bold pb-2 text-zinc-900 dark:text-zinc-100 font-outfit">
        Unified Auth Flow
      </Typography.Heading>
      
      <Typography className="text-sm text-zinc-500 dark:text-zinc-400 mb-6 max-w-xs mx-auto">
        We have consolidated our signup and signin experience into a secure, single-step "Continue with Email" flow.
      </Typography>

      <Button
        className="h-12 rounded-xl bg-zinc-900 dark:bg-zinc-100 text-white dark:text-zinc-900 font-semibold px-6"
        onPress={() => router.push('/login')}
      >
        Go to Unified Sign In
      </Button>
    </Card>
  );
}
