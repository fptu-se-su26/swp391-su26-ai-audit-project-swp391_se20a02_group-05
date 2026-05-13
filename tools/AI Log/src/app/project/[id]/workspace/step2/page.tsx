"use client";

import Step2Form from "@/components/workspace/Step2Form";
import { useParams } from "next/navigation";

export default function Step2Page() {
  const params = useParams();
  const projectId = params.id as string;

  return (
    <div className="flex flex-col gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div>
        <h1 className="text-2xl font-bold">2. Changelog & Phase Updates</h1>
        <p className="text-default-500">Log your progress across different project phases.</p>
      </div>
      <Step2Form projectId={projectId} />
    </div>
  );
}
