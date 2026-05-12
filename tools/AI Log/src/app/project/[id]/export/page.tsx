"use client";

import ExportCenter from "@/components/export/ExportCenter";
import { useParams } from "next/navigation";

export default function ExportPage() {
  const params = useParams();
  const projectId = params.id as string;

  return <ExportCenter projectId={projectId} />;
}
