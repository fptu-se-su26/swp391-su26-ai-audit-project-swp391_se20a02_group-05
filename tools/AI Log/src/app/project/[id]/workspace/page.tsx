import { redirect } from "next/navigation";

export default async function WorkspaceRoot({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  // Redirect to the first step by default
  redirect(`/project/${id}/workspace/step1`);
}
