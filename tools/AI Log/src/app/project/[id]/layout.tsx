"use client";

import { useProjectStore } from "@/store/projectStore";
import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import WorkspaceSidebar from "@/components/workspace/WorkspaceSidebar";
import Navbar from "@/components/layout/Navbar";
import { Button } from "@heroui/react";

export default function WorkspaceLayout({ children }: { children: React.ReactNode }) {
  const { id } = useParams() as { id: string };
  const { projects, setActiveProject } = useProjectStore();
  const router = useRouter();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  useEffect(() => {
    if (mounted && id && projects[id]) {
      setActiveProject(id);
    }
  }, [id, projects, setActiveProject, mounted]);

  // Wait for client hydration
  if (!mounted) {
    return (
      <div className="min-h-screen bg-background flex flex-col">
        <Navbar />
        <div className="flex-1 flex items-center justify-center">
          <div className="animate-pulse flex flex-col items-center gap-4">
            <div className="h-8 w-8 rounded-full border-4 border-primary border-t-transparent animate-spin"></div>
            <p className="text-default-500">Loading workspace...</p>
          </div>
        </div>
      </div>
    );
  }

  // Handle invalid project
  if (!projects[id]) {
    return (
      <div className="min-h-screen bg-background flex flex-col">
        <Navbar />
        <div className="flex-1 flex items-center justify-center p-6">
          <div className="max-w-md w-full flex flex-col items-center text-center gap-6">
            <h1 className="text-4xl font-bold text-foreground">Project Not Found</h1>
            <p className="text-default-500">
              The project you are looking for does not exist or has been deleted.
            </p>
            <Button variant="primary" onPress={() => router.push("/")} className="w-full">
              Return to Dashboard
            </Button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <Navbar />
      <div className="flex-1 flex overflow-hidden">
        <WorkspaceSidebar projectId={id} />
        <main className="flex-1 overflow-y-auto bg-background p-6">
          <div className="max-w-4xl mx-auto">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
}
