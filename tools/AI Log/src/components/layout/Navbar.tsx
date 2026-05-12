"use client";

import { Button } from "@heroui/react";
import { Bot } from "lucide-react";
import Link from "next/link";

export default function Navbar() {
  return (
    <nav className="w-full flex justify-center border-b border-border bg-background/70 backdrop-blur-md sticky top-0 z-50">
      <div className="w-full max-w-7xl px-6 h-16 flex items-center justify-between">
        <Link href="/" className="flex items-center gap-2 text-inherit">
          <Bot className="h-6 w-6 text-accent" />
          <p className="font-bold text-inherit tracking-tight">AI Workflow Logger</p>
        </Link>
        <div className="flex items-center">
          <Button
            onPress={() => window.open("https://github.com", "_blank", "noopener,noreferrer")}
            className="bg-transparent text-foreground data-[hover=true]:bg-surface-secondary"
            variant="ghost"
          >
            GitHub
          </Button>
        </div>
      </div>
    </nav>
  );
}
