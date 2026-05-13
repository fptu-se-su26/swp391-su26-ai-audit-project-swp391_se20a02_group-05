"use client";

import Step4Form from "@/components/workspace/Step4Form";
import { useParams } from "next/navigation";

export default function Step4Page() {
  const params = useParams();
  const projectId = params.id as string;

  return (
    <div className="flex flex-col gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div>
        <h1 className="text-2xl font-bold">4. AI Audit Log</h1>
        <p className="text-default-500">Document the detailed usage, evaluation, and issues encountered with AI tools.</p>
      </div>
      <Step4Form projectId={projectId} />
    </div>
  );
}
