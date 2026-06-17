"use client";

import React, { useEffect, useState } from "react";
import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useWorkspaceStore } from "@/features/workspace/store/use-workspace-store";
import { Card } from "@/components/ui/card";
import { Typography, Chip } from "@heroui/react";
import { Button } from "@/components/ui/button";
import {
  Building2,
  FileText,
  ShieldCheck,
  AlertTriangle,
  ArrowLeft,
  Plus,
  Eye,
  Edit,
  Zap,
  Trash2,
  Loader2,
} from "lucide-react";
import { SkeletonLoader } from "@/components/ui/states";
import { jdService } from "@/modules/business/services/jd.service";
import type { JdSummary } from "@/modules/business/types/jd.types";

export default function WorkspaceJDManagementPage() {
  const params = useParams();
  const router = useRouter();
  const organizationSlug =
    typeof params?.organizationSlug === "string" ? params.organizationSlug : "";

  const fetchWorkspace = useWorkspaceStore((s) => s.fetchWorkspace);
  const workspaceDetails = useWorkspaceStore((s) => s.workspaces[organizationSlug]);
  const isDetailsLoading = useWorkspaceStore((s) => s.loading[organizationSlug]);
  const detailsError = useWorkspaceStore((s) => s.errors[organizationSlug]);

  const [jdList, setJdList] = useState<JdSummary[]>([]);
  const [jdLoading, setJdLoading] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    if (organizationSlug) fetchWorkspace(organizationSlug);
  }, [organizationSlug, fetchWorkspace]);

  useEffect(() => {
    let active = true;
    setJdLoading(true);
    jdService
      .listJds()
      .then((list) => { if (active) setJdList(list); })
      .catch(() => { if (active) setJdList([]); })
      .finally(() => { if (active) setJdLoading(false); });
    return () => { active = false; };
  }, []);

  const handleDelete = async (id: string) => {
    if (!window.confirm("Delete this job description?")) return;
    setDeletingId(id);
    try {
      await jdService.deleteJd(id);
      setJdList((prev) => prev.filter((j) => j.jdId !== id));
    } finally {
      setDeletingId(null);
    }
  };

  if (isDetailsLoading) {
    return (
      <div className="space-y-6 max-w-7xl mx-auto p-4 font-outfit text-foreground">
        <div className="h-10 w-48 bg-separator/50 animate-pulse rounded-lg mb-4" />
        <Card className="p-0 overflow-hidden">
          <SkeletonLoader rows={6} columns={4} />
        </Card>
      </div>
    );
  }

  if (detailsError || !workspaceDetails) {
    return (
      <div className="max-w-xl mx-auto py-20 font-outfit text-foreground">
        <Card className="p-8 border border-border bg-surface text-center">
          <div className="size-16 rounded-2xl bg-danger/10 flex items-center justify-center border border-danger/20 mx-auto mb-5 text-danger">
            <AlertTriangle size={28} />
          </div>
          <Typography type="h4" className="font-bold text-foreground mb-2">
            Workspace Loading Error
          </Typography>
          <Typography type="body-xs" className="text-muted leading-relaxed mb-6">
            {detailsError || "Organization not found"}
          </Typography>
        </Card>
      </div>
    );
  }

  const permissions = workspaceDetails.permissions || [];
  const isAuthorized =
    permissions.includes("ai:interview:configure") ||
    permissions.includes("ai:interview:conduct") ||
    permissions.includes("ai:interview:evaluate") ||
    workspaceDetails.userRole === "OWNER" ||
    workspaceDetails.userRole === "REPRESENTATIVE" ||
    workspaceDetails.userRole === "HR";

  if (!isAuthorized) {
    return (
      <div className="max-w-xl mx-auto py-20 font-outfit text-foreground">
        <Card className="p-8 border border-border bg-surface text-center">
          <div className="size-16 rounded-2xl bg-danger/10 flex items-center justify-center border border-danger/20 mx-auto mb-5 text-danger">
            <AlertTriangle size={28} />
          </div>
          <Typography type="h4" className="font-bold text-foreground mb-2">
            Access Denied
          </Typography>
          <Typography type="body-xs" className="text-muted leading-relaxed mb-6">
            You do not have permission to access the Job Description management
            console. This section is reserved for human resources and
            organization representatives.
          </Typography>
          <div className="flex gap-4 justify-center">
            <Button
              onClick={() =>
                router.push(`/workspace/${organizationSlug}/information`)
              }
              className="px-4 py-2 bg-foreground text-background font-bold rounded-xl text-xs cursor-pointer"
            >
              <ArrowLeft size={12} className="mr-1.5" />
              Back to Workspace
            </Button>
          </div>
        </Card>
      </div>
    );
  }

  return (
    <div className="space-y-6 font-outfit max-w-7xl mx-auto text-foreground">
      {/* Header Banner */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-surface border border-border text-foreground select-none">
        <div className="space-y-1">
          <Typography
            type="h2"
            className="text-2xl font-bold flex items-center gap-2 text-foreground"
          >
            <Building2 size={24} className="text-accent" />
            {workspaceDetails.organizationName}
          </Typography>
          <Typography type="body-xs" className="text-muted font-light mt-0.5">
            Workspace context:{" "}
            <span className="font-mono text-accent">
              @{workspaceDetails.organizationSlug}
            </span>{" "}
            • My Role:{" "}
            <span className="font-semibold text-foreground">
              {workspaceDetails.userRole}
            </span>
          </Typography>
        </div>
        <div className="flex items-center gap-3">
          <Chip
            color="success"
            variant="soft"
            size="sm"
            className="font-semibold text-xs py-1"
          >
            <ShieldCheck size={12} className="inline mr-1" />
            Verified Enterprise
          </Chip>
          <Link href="/jd/create">
            <Button
              size="sm"
              className="bg-accent text-white hover:bg-accent/90 flex items-center gap-1.5"
            >
              <Plus size={14} />
              Create JD
            </Button>
          </Link>
        </div>
      </div>

      {/* JD List Card */}
      <Card className="p-6 md:p-8 bg-surface border border-border rounded-2xl">
        <div className="flex items-center justify-between mb-6 select-none">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-accent/10 text-accent flex items-center justify-center">
              <FileText size={20} />
            </div>
            <div>
              <Typography type="h3" className="font-bold text-foreground">
                JD Management
              </Typography>
              <Typography type="body-xs" className="text-muted">
                Create, review, and organize job descriptions for automated
                evidence matching and screening.
              </Typography>
            </div>
          </div>
          <Link href="/jd/create">
            <Button
              size="sm"
              className="bg-accent text-white hover:bg-accent/90 hidden md:flex items-center gap-1.5"
            >
              <Plus size={14} />
              New JD
            </Button>
          </Link>
        </div>

        {jdLoading ? (
          <div className="flex items-center justify-center py-16 text-muted gap-2">
            <Loader2 size={18} className="animate-spin" />
            <span className="text-sm">Loading job descriptions…</span>
          </div>
        ) : jdList.length === 0 ? (
          <div className="border border-dashed border-border/80 rounded-2xl p-12 text-center select-none">
            <div className="w-14 h-14 rounded-2xl bg-accent/10 flex items-center justify-center mx-auto mb-4 text-accent">
              <FileText size={24} />
            </div>
            <Typography type="h4" className="font-bold text-foreground mb-2">
              No Job Descriptions Yet
            </Typography>
            <Typography
              type="body-xs"
              className="text-muted max-w-md mx-auto mb-6"
            >
              Begin hiring by creating your first AI-powered job description.
              Select required skills, experience, and salary — AI will generate
              a professional JD in seconds.
            </Typography>
            <Link href="/jd/create">
              <Button className="bg-accent text-white hover:bg-accent/90 inline-flex items-center gap-2">
                <Plus size={14} />
                Create Your First JD
              </Button>
            </Link>
          </div>
        ) : (
          <div className="space-y-3">
            {jdList.map((jd) => (
              <JdRow
                key={jd.jdId}
                jd={jd}
                deleting={deletingId === jd.jdId}
                onDelete={() => void handleDelete(jd.jdId)}
              />
            ))}
          </div>
        )}
      </Card>
    </div>
  );
}

function JdRow({
  jd,
  deleting,
  onDelete,
}: {
  jd: JdSummary;
  deleting: boolean;
  onDelete: () => void;
}) {
  const seniorityColor: Record<string, string> = {
    Junior: "bg-success/10 text-success border-success/30",
    Middle: "bg-accent/10 text-accent border-accent/30",
    Senior: "bg-warning/10 text-warning border-warning/30",
    Staff: "bg-warning/10 text-warning border-warning/30",
    Principal: "bg-danger/10 text-danger border-danger/30",
  };

  return (
    <div className="flex flex-col md:flex-row md:items-center justify-between gap-3 rounded-xl border border-separator bg-surface/50 px-4 py-3">
      <div className="flex-1 min-w-0">
        <div className="flex items-center gap-2 flex-wrap">
          <span className="font-semibold text-sm text-foreground truncate">
            {jd.jobTitle}
          </span>
          <span
            className={`text-xs font-semibold rounded-full border px-2 py-0.5 ${seniorityColor[jd.seniority] ?? "bg-separator text-muted border-separator"}`}
          >
            {jd.seniority}
          </span>
          {jd.hiringPriority && jd.hiringPriority !== "Medium" && (
            <span className="text-xs font-semibold rounded-full border px-2 py-0.5 bg-danger/10 text-danger border-danger/30">
              {jd.hiringPriority}
            </span>
          )}
        </div>
        <p className="text-xs text-muted mt-0.5">
          {jd.department} · {jd.location} · {jd.workMode} ·{" "}
          {jd.salaryMin.toLocaleString()}–{jd.salaryMax.toLocaleString()}{" "}
          {jd.currency}
        </p>
      </div>
      <div className="flex items-center gap-1.5 shrink-0">
        <Link href={`/jd/view/${jd.jdId}`}>
          <Button variant="bordered" size="sm" className="gap-1 text-xs">
            <Eye size={12} />
            View
          </Button>
        </Link>
        <Link href={`/jd/${jd.jdId}/match`}>
          <Button
            size="sm"
            className="gap-1 text-xs bg-accent text-white hover:bg-accent/90"
          >
            <Zap size={12} />
            Match
          </Button>
        </Link>
        <Link href={`/jd/edit/${jd.jdId}`}>
          <Button variant="bordered" size="sm" className="gap-1 text-xs">
            <Edit size={12} />
            Edit
          </Button>
        </Link>
        <Button
          variant="bordered"
          size="sm"
          disabled={deleting}
          onClick={onDelete}
          className="gap-1 text-xs text-danger border-danger/30 hover:bg-danger/10"
        >
          {deleting ? (
            <Loader2 size={12} className="animate-spin" />
          ) : (
            <Trash2 size={12} />
          )}
        </Button>
      </div>
    </div>
  );
}
