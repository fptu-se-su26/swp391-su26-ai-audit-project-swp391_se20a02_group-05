"use client";

import { Button } from "@heroui/react";
import { Info, ListChecks, MessageSquare, ShieldCheck, Lightbulb, FileText, ChevronLeft } from "lucide-react";
import { usePathname, useRouter } from "next/navigation";

const steps = [
  { id: "step1", name: "1. Project Info", icon: Info, path: "/step1" },
  { id: "step2", name: "2. Changelog", icon: ListChecks, path: "/step2" },
  { id: "step3", name: "3. Prompt Log", icon: MessageSquare, path: "/step3" },
  { id: "step4", name: "4. AI Audit", icon: ShieldCheck, path: "/step4" },
  { id: "step5", name: "5. Reflection", icon: Lightbulb, path: "/step5" },
];

export default function WorkspaceSidebar({ projectId }: { projectId: string }) {
  const pathname = usePathname();
  const router = useRouter();
  const basePath = `/project/${projectId}/workspace`;
  
  return (
    <aside className="w-64 border-r border-border bg-surface flex flex-col">
      <div className="p-4 border-b border-border">
        <Button 
          variant="ghost" 
          onPress={() => router.push("/")}
          className="w-full justify-start text-default-500"
        >
          <ChevronLeft className="h-4 w-4 mr-2 inline" />
          Back to Dashboard
        </Button>
      </div>
      <nav className="flex-1 p-4 flex flex-col gap-2">
        <div className="text-xs font-semibold text-default-400 uppercase tracking-wider mb-2 px-2">
          Workflow Steps
        </div>
        {steps.map((step) => {
          const isActive = pathname === `${basePath}${step.path}`;
          const Icon = step.icon;
          return (
            <Button
              key={step.id}
              onPress={() => router.push(`${basePath}${step.path}`)}
              variant={isActive ? "secondary" : "ghost"}
              className={`w-full justify-start ${isActive ? "font-medium" : "text-default-500"}`}
            >
              <Icon className="h-4 w-4 mr-2 inline" />
              {step.name}
            </Button>
          );
        })}
        
        <div className="mt-8 text-xs font-semibold text-default-400 uppercase tracking-wider mb-2 px-2">
          Export
        </div>
        <Button
          onPress={() => router.push(`/project/${projectId}/export`)}
          variant="ghost"
          className="w-full justify-start text-success-600"
        >
          <FileText className="h-4 w-4 mr-2 inline" />
          Markdown Preview
        </Button>
      </nav>
    </aside>
  );
}
