"use client";

import React from "react";
import { useParams, useRouter } from "next/navigation";
import { useWorkspaceStore } from "@/features/workspace/store/use-workspace-store";
import { Card } from "@/components/ui/card";
import { Typography, Chip } from "@heroui/react";
import { Button } from "@/components/ui/button";
import { getTagLabel } from "@/features/workspace/types/workspace.types";

export default function WorkspaceHomeTab() {
  const params = useParams();
  const router = useRouter();
  const organizationSlug = typeof params?.organizationSlug === "string" ? params.organizationSlug : "";
  const workspaceDetails = useWorkspaceStore((s) => s.workspaces[organizationSlug]);

  if (!workspaceDetails) return null;

  // Mock Jobs for Preview
  const mockJobs = [
    {
      id: "job-1",
      title: "Senior Full-Stack Developer (.NET & React)",
      department: "Engineering",
      location: "Hanoi, Vietnam (Hybrid)",
      type: "Full-Time",
    },
    {
      id: "job-2",
      title: "Automated Verification QA Engineer",
      department: "Quality Assurance",
      location: "Remote",
      type: "Contract",
    },
  ];

  // Mock Updates for Preview
  const mockPosts = [
    {
      id: "post-1",
      title: "CVerify integration successfully deployed!",
      summary: "We are thrilled to announce that our development workflows have integrated credential hashing, achieving 100% automated skill verification.",
      date: "2 days ago",
    }
  ];

  const baseRoute = `/workspace/${organizationSlug}`;

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
      {/* Main Column */}
      <div className="lg:col-span-2 space-y-6">
        {/* Focus Areas & Industry Tags */}
        <Card className="p-5 md:p-6 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h3" className="font-semibold text-foreground text-sm">
            Focus Areas & Industry Tags
          </Typography>
          <div className="flex flex-wrap gap-2">
            {workspaceDetails.industryTags && workspaceDetails.industryTags.length > 0 ? (
              workspaceDetails.industryTags.map(tag => (
                <Chip
                  key={tag}
                  color="accent"
                  variant="soft"
                  size="sm"
                  className="font-medium text-xs py-1"
                >
                  {tag}
                </Chip>
              ))
            ) : (
              <span className="text-xs text-muted font-normal italic">No industry tags configured.</span>
            )}
          </div>
        </Card>

        {/* Employee Benefits */}
        <Card className="p-5 md:p-6 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h3" className="font-semibold text-foreground text-sm">
            Employee Benefits
          </Typography>
          <div className="flex flex-wrap gap-2">
            {workspaceDetails.benefitTags && workspaceDetails.benefitTags.length > 0 ? (
              workspaceDetails.benefitTags.map(tag => (
                <Chip
                  key={tag}
                  color="accent"
                  variant="soft"
                  size="sm"
                  className="font-medium text-xs py-1"
                >
                  {getTagLabel(tag)}
                </Chip>
              ))
            ) : (
              <span className="text-xs text-muted font-normal italic">No employee benefits listed.</span>
            )}
          </div>
        </Card>

        {/* Corporate Pillars */}
        <Card className="p-5 md:p-6 bg-surface border border-border rounded-xl space-y-5">
          <Typography type="h3" className="font-semibold text-foreground text-sm">
            Corporate Pillars
          </Typography>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="space-y-1">
              <span className="text-[10px] text-accent font-medium uppercase tracking-wider block">Mission</span>
              <Typography type="body-xs" className="text-muted text-xs leading-relaxed font-normal">
                {workspaceDetails.mission || "To establish a source of technical truth and enable seamless verification."}
              </Typography>
            </div>

            <div className="space-y-1">
              <span className="text-[10px] text-accent font-medium uppercase tracking-wider block">Vision</span>
              <Typography type="body-xs" className="text-muted text-xs leading-relaxed font-normal">
                {workspaceDetails.vision || "A world where skill validation is instant, verifiable, and free of bias."}
              </Typography>
            </div>

            <div className="space-y-1">
              <span className="text-[10px] text-accent font-medium uppercase tracking-wider block">Core Values</span>
              <Typography type="body-xs" className="text-muted text-xs leading-relaxed font-normal">
                {workspaceDetails.coreValues || "Trust, integrity, developers first, and continuous innovation."}
              </Typography>
            </div>
          </div>
        </Card>

        {/* Jobs Preview Card */}
        <Card className="p-5 md:p-6 bg-surface border border-border rounded-xl space-y-5">
          <div className="flex justify-between items-center select-none">
            <Typography type="h3" className="font-semibold text-foreground text-sm">
              Open Jobs Preview
            </Typography>
            <Button
              onClick={() => router.push(`${baseRoute}/jobs`)}
              variant="bordered"
              size="sm"
              className="font-medium text-[11px] h-8 border-border text-muted hover:text-foreground cursor-pointer px-3 rounded-lg"
            >
              See all jobs
            </Button>
          </div>

          <div className="space-y-3">
            {mockJobs.map((job) => (
              <div
                key={job.id}
                onClick={() => router.push(`${baseRoute}/jobs`)}
                className="p-3.5 rounded-xl border border-border bg-card/10 hover:bg-card/25 transition-colors cursor-pointer flex justify-between items-center gap-4"
              >
                <div className="space-y-0.5">
                  <Typography type="body-sm" className="font-medium text-foreground text-xs">
                    {job.title}
                  </Typography>
                  <div className="flex flex-wrap items-center gap-1.5 text-[10px] text-muted-foreground select-none">
                    <span>{job.department}</span>
                    <span>•</span>
                    <span>{job.location}</span>
                  </div>
                </div>
                <Chip size="sm" variant="soft" color="accent" className="text-[9px] font-medium h-5 px-1.5">
                  {job.type}
                </Chip>
              </div>
            ))}
          </div>
        </Card>

        {/* Latest Activity Preview */}
        <Card className="p-5 md:p-6 bg-surface border border-border rounded-xl space-y-5">
          <div className="flex justify-between items-center select-none">
            <Typography type="h3" className="font-semibold text-foreground text-sm">
              Latest Announcements
            </Typography>
            <Button
              onClick={() => router.push(`${baseRoute}/posts`)}
              variant="bordered"
              size="sm"
              className="font-medium text-[11px] h-8 border-border text-muted hover:text-foreground cursor-pointer px-3 rounded-lg"
            >
              See all posts
            </Button>
          </div>

          <div className="space-y-3">
            {mockPosts.map((post) => (
              <div
                key={post.id}
                onClick={() => router.push(`${baseRoute}/posts`)}
                className="p-4 rounded-xl border border-border bg-card/10 hover:bg-card/25 transition-colors cursor-pointer space-y-1.5"
              >
                <div className="flex justify-between items-start gap-4">
                  <Typography type="body-sm" className="font-medium text-foreground text-xs leading-tight">
                    {post.title}
                  </Typography>
                  <span className="text-[9px] text-muted-foreground select-none shrink-0">{post.date}</span>
                </div>
                <Typography type="body-xs" className="text-muted text-[11px] leading-relaxed line-clamp-2 font-normal">
                  {post.summary}
                </Typography>
              </div>
            ))}
          </div>
        </Card>
      </div>

      {/* Side Widget Column */}
      <div className="space-y-6">
        {/* Corporate Stats Card */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Company details
          </Typography>

          <div className="space-y-3 text-xs select-none font-normal">
            {workspaceDetails.website && (
              <div>
                <span className="text-[9px] text-muted-foreground uppercase block">Website</span>
                <a
                  href={workspaceDetails.website}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="font-medium text-accent hover:underline break-all text-xs"
                >
                  {workspaceDetails.website.replace("https://", "").replace("http://", "")}
                </a>
              </div>
            )}

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Company Type</span>
              <span className="font-medium text-foreground text-xs">{workspaceDetails.companyType || "Not specified"}</span>
            </div>

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Company size</span>
              <span className="font-medium text-foreground text-xs">
                {workspaceDetails.companySize && (
                  workspaceDetails.companySize.toLowerCase().includes("employee") || 
                  workspaceDetails.companySize.toLowerCase().includes("nhân viên")
                    ? workspaceDetails.companySize 
                    : `${workspaceDetails.companySize} employees`
                ) || "Not specified"}
              </span>
            </div>

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Branch offices</span>
              <span className="font-medium text-foreground text-xs">{workspaceDetails.branchCount || 0} branches</span>
            </div>

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Headquarters</span>
              <span className="font-medium text-foreground text-xs">
                {workspaceDetails.city 
                  ? (workspaceDetails.city.toLowerCase().includes("vietnam") || workspaceDetails.city.toLowerCase().includes("việt nam")
                    ? workspaceDetails.city
                    : `${workspaceDetails.city}, Vietnam`)
                  : workspaceDetails.location || "Not specified"}
              </span>
            </div>

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Founded</span>
              <span className="font-medium text-foreground text-xs">{workspaceDetails.founded || "Not specified"}</span>
            </div>

            {workspaceDetails.taxCode && (
              <div>
                <span className="text-[9px] text-muted-foreground uppercase block">Tax Registered Code</span>
                <span className="font-medium text-foreground text-xs font-mono">{workspaceDetails.taxCode}</span>
              </div>
            )}
          </div>
        </Card>

        {/* Social Links Card */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Social Coordinates
          </Typography>
          <div className="flex flex-col gap-2 font-normal">
            {workspaceDetails.linkedinUrl && (
              <a
                href={workspaceDetails.linkedinUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>LinkedIn</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {workspaceDetails.facebookUrl && (
              <a
                href={workspaceDetails.facebookUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>Facebook</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {workspaceDetails.twitterUrl && (
              <a
                href={workspaceDetails.twitterUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>Twitter / X</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {workspaceDetails.website && (
              <a
                href={workspaceDetails.website}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>Website Portal</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {!workspaceDetails.linkedinUrl && 
             !workspaceDetails.facebookUrl && 
             !workspaceDetails.twitterUrl && 
             !workspaceDetails.website && (
              <span className="text-xs text-muted font-normal italic">No social coordinates specified.</span>
            )}
          </div>
        </Card>

        {/* OfficeHQ Google Maps embed */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Office HQ Location
          </Typography>
          <div className="space-y-3 font-normal">
            <div className="text-xs text-foreground">
              <span>
                {workspaceDetails.detailAddress 
                  ? `${workspaceDetails.detailAddress}, ${workspaceDetails.city || ""}` 
                  : workspaceDetails.city || workspaceDetails.location || "No office locations registered."}
              </span>
            </div>
            {workspaceDetails.googleMapsEmbedUrl ? (
              <div className="h-48 rounded-xl overflow-hidden border border-border/80">
                <iframe
                  src={workspaceDetails.googleMapsEmbedUrl}
                  width="100%"
                  height="100%"
                  style={{ border: 0 }}
                  allowFullScreen={false}
                  loading="lazy"
                  title="Google Maps Location"
                />
              </div>
            ) : (
              <div className="h-28 border border-dashed border-border rounded-xl bg-surface-secondary/40 flex flex-col items-center justify-center text-muted select-none">
                <span className="text-xs font-medium italic">No interactive map location specified.</span>
              </div>
            )}
          </div>
        </Card>

        {/* Verification Highlights */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-3">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Verification Badging
          </Typography>
          <Typography type="body-xs" className="text-muted leading-relaxed font-normal text-xs">
            This workspace holds a Level 3 Domain & Ownership verification status. All corporate information has been cryptographic-hashed and signed by CVerify Authorities.
          </Typography>
          <div className="pt-1 select-none font-normal text-xs text-muted-foreground space-y-1.5">
            <div className="flex items-center gap-1.5">
              <span>•</span>
              Legal Authority Verified
            </div>
            <div className="flex items-center gap-1.5">
              <span>•</span>
              Representative Signature Matches
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
