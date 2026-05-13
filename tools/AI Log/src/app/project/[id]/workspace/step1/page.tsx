"use client";

import Step1Form from "@/components/workspace/Step1Form";
import { useParams } from "next/navigation";

export default function Step1Page() {
  const params = useParams();
  const projectId = params.id as string;

  return (
    <div className="flex flex-col gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div>
        <h1 className="text-2xl font-bold">1. Project Information</h1>
        <p className="text-default-500">Update the general metadata and team members for this project.</p>
      </div>
      <Step1Form projectId={projectId} />
    </div>
  );
}
