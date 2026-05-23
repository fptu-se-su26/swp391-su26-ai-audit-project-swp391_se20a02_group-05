import React from 'react';
import { Typography, Button, Link } from '@heroui/react';

export const AuthShowcase: React.FC = () => {
  return (
    <div className="hidden xl:flex flex-col justify-center pl-12">
      <div className="absolute top-12 left-12">
        <Link href="/">
          <img
            src="/brand/logo&name.png"
            alt="CVerify Logo"
            className="h-10 w-auto"
          />
        </Link>
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
  );
};

export default AuthShowcase;
