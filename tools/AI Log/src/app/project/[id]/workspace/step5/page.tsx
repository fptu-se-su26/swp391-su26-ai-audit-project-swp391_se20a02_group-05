"use client";

import Step5Form from "@/components/workspace/Step5Form";
import { useParams } from "next/navigation";

export default function Step5Page() {
  const params = useParams();
  const projectId = params.id as string;

  return (
    <div className="flex flex-col gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div>
        <h1 className="text-2xl font-bold">5. Reflection</h1>
        <p className="text-default-500">Self-evaluate your use of AI in this project and the lessons you&apos;ve learned.</p>
      </div>
      <Step5Form projectId={projectId} />
    </div>
  );
}
