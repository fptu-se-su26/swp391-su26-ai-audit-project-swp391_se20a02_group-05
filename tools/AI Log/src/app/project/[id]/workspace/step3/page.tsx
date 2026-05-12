"use client";

import Step3Form from "@/components/workspace/Step3Form";
import { useParams } from "next/navigation";

export default function Step3Page() {
  const params = useParams();
  const projectId = params.id as string;

  return (
    <div className="flex flex-col gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div>
        <h1 className="text-2xl font-bold">3. Prompt Log</h1>
        <p className="text-default-500">Document the prompts you used and how you evaluated the AI&apos;s responses.</p>
      </div>
      <Step3Form projectId={projectId} />
    </div>
  );
}
