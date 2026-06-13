"use client";

import React, { useState } from "react";
import { useParams } from "next/navigation";
import { Card } from "@/components/ui/card";
import { Typography, Chip } from "@heroui/react";
import { Button } from "@/components/ui/button";


interface Job {
  id: string;
  title: string;
  department: string;
  location: string;
  type: string;
  posted: string;
  salary: string;
  description: string;
  requirements: string[];
}

export default function WorkspaceJobsTab() {
  const params = useParams();
  const organizationSlug = typeof params?.organizationSlug === "string" ? params.organizationSlug : "";

  // Mock Jobs list
  const mockJobs: Job[] = [
    {
      id: "job-1",
      title: "Senior Full-Stack Developer (.NET & React)",
      department: "Engineering",
      location: "Hanoi, Vietnam (Hybrid)",
      type: "Full-Time",
      posted: "2 days ago",
      salary: "Negotiable",
      description: "We are seeking a Senior Full-Stack Developer to lead architectural decisions across our verification platforms. You will work closely with .NET backend microservices and dynamic React single page applications.",
      requirements: [
        "5+ years of software development experience.",
        "Strong proficiency in C# .NET Core and Web API.",
        "Solid front-end skills in React, TypeScript, and modern state managers (Zustand, Redux).",
        "Experience designing and implementing RESTful APIs.",
      ],
    },
    {
      id: "job-2",
      title: "Automated Verification QA Engineer",
      department: "Quality Assurance",
      location: "Remote",
      type: "Contract",
      posted: "5 days ago",
      salary: "Competitive",
      description: "Join our QA team to design, write, and execute automated tests validating cryptographically hashed employee credential chains. You will build test suites and automate regression runs.",
      requirements: [
        "3+ years of automated testing experience (Playwright, Selenium, or Cypress).",
        "Familiarity with CI/CD tools and GitHub Actions workflows.",
        "Good understanding of security concepts (signatures, hashing).",
        "Strong debugging and logging analytical skills.",
      ],
    },
    {
      id: "job-3",
      title: "Lead UI/UX Product Designer",
      department: "Design",
      location: "Hanoi, Vietnam (On-site)",
      type: "Full-Time",
      posted: "1 week ago",
      salary: "Competitive",
      description: "We are looking for a visionary Lead Product Designer to guide the visual aesthetics of CVerify workspace panels. You will define and govern components, design systems, and visual guidelines.",
      requirements: [
        "4+ years designing high-fidelity dashboards and SaaS platforms.",
        "Expertise in Figma design system organization, auto-layouts, and design tokens.",
        "Deep understanding of user experience paradigms (UX research, wireframing).",
        "Ability to work closely with React developer teams to ensure pixel-perfect delivery.",
      ],
    },
  ];

  const [searchQuery, setSearchQuery] = useState("");
  const [selectedDept, setSelectedDept] = useState("All");
  const [selectedLoc, setSelectedLoc] = useState("All");
  const [selectedType, setSelectedType] = useState("All");

  const [activeJob, setActiveJob] = useState<Job | null>(null);
  const [appliedJobs, setAppliedJobs] = useState<string[]>([]);

  // Unique lists for filters
  const departments = ["All", ...Array.from(new Set(mockJobs.map((j) => j.department)))];
  const locations = ["All", ...Array.from(new Set(mockJobs.map((j) => j.location.split(" (")[0])))];
  const types = ["All", ...Array.from(new Set(mockJobs.map((j) => j.type)))];

  // Filtering Logic
  const filteredJobs = mockJobs.filter((job) => {
    const matchesSearch =
      job.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      job.description.toLowerCase().includes(searchQuery.toLowerCase());

    const matchesDept = selectedDept === "All" || job.department === selectedDept;
    const matchesLoc = selectedLoc === "All" || job.location.includes(selectedLoc);
    const matchesType = selectedType === "All" || job.type === selectedType;

    return matchesSearch && matchesDept && matchesLoc && matchesType;
  });

  const handleApply = (jobId: string) => {
    if (!appliedJobs.includes(jobId)) {
      setAppliedJobs([...appliedJobs, jobId]);
    }
  };

  return (
    <div className="space-y-6 relative">
      {/* Search and Filter Panel */}
      <Card className="p-6 bg-surface border border-border rounded-2xl space-y-4 select-none">
        {/* Search */}
        <div className="relative">
          <input
            type="text"
            placeholder="Search open positions..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full bg-card border border-border rounded-xl px-4 py-3 text-sm focus:outline-hidden focus:border-accent text-foreground font-outfit"
          />
        </div>

        {/* Filters */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="space-y-1">
            <span className="text-[10px] text-muted-foreground font-medium uppercase">Department</span>
            <select
              value={selectedDept}
              onChange={(e) => setSelectedDept(e.target.value)}
              className="w-full bg-card border border-border rounded-xl px-3 py-2.5 text-xs text-foreground focus:outline-hidden focus:border-accent font-outfit font-normal"
            >
              {departments.map((dept) => (
                <option key={dept} value={dept}>
                  {dept}
                </option>
              ))}
            </select>
          </div>

          <div className="space-y-1">
            <span className="text-[10px] text-muted-foreground font-medium uppercase">Location</span>
            <select
              value={selectedLoc}
              onChange={(e) => setSelectedLoc(e.target.value)}
              className="w-full bg-card border border-border rounded-xl px-3 py-2.5 text-xs text-foreground focus:outline-hidden focus:border-accent font-outfit font-normal"
            >
              {locations.map((loc) => (
                <option key={loc} value={loc}>
                  {loc}
                </option>
              ))}
            </select>
          </div>

          <div className="space-y-1">
            <span className="text-[10px] text-muted-foreground font-medium uppercase">Employment Type</span>
            <select
              value={selectedType}
              onChange={(e) => setSelectedType(e.target.value)}
              className="w-full bg-card border border-border rounded-xl px-3 py-2.5 text-xs text-foreground focus:outline-hidden focus:border-accent font-outfit font-normal"
            >
              {types.map((type) => (
                <option key={type} value={type}>
                  {type}
                </option>
              ))}
            </select>
          </div>
        </div>
      </Card>

      {/* Jobs Listing */}
      <div className="space-y-4">
        {filteredJobs.length === 0 ? (
          <div className="border border-dashed border-border/80 rounded-2xl p-12 text-center select-none bg-surface">
            <Typography type="h4" className="font-semibold text-foreground mb-1">
              No matching positions found
            </Typography>
            <Typography type="body-xs" className="text-muted max-w-md mx-auto font-normal">
              Try modifying your search keywords or adjusting the department, location, or type filters.
            </Typography>
          </div>
        ) : (
          filteredJobs.map((job) => (
            <Card
              key={job.id}
              onClick={() => setActiveJob(job)}
              className="p-6 bg-surface border border-border rounded-2xl hover:border-accent/40 transition-colors cursor-pointer flex justify-between items-center gap-4"
            >
              <div className="space-y-2">
                <div className="flex flex-wrap items-center gap-2 select-none">
                  <Chip size="sm" variant="soft" color="accent" className="text-[9px] font-medium py-0.5 px-2">
                    {job.department}
                  </Chip>
                  <Chip size="sm" variant="soft" color="warning" className="text-[9px] font-medium py-0.5 px-2">
                    {job.type}
                  </Chip>
                </div>

                <Typography type="body-sm" className="font-semibold text-foreground text-base hover:text-accent transition-colors">
                  {job.title}
                </Typography>

                <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-muted-foreground select-none font-normal">
                  <span className="flex items-center">
                    {job.location}
                  </span>
                  <span>•</span>
                  <span className="flex items-center">
                    {job.salary}
                  </span>
                  <span>•</span>
                  <span className="flex items-center">
                    {job.posted}
                  </span>
                </div>
              </div>

              <div className="flex items-center gap-2 select-none">
                {appliedJobs.includes(job.id) && (
                  <Chip size="sm" color="success" variant="soft" className="text-[10px] font-medium">
                    Applied
                  </Chip>
                )}
                <div className="w-8 h-8 rounded-lg border border-border flex items-center justify-center text-muted hover:text-foreground hover:bg-card/50 transition-colors text-xs font-normal">
                  →
                </div>
              </div>
            </Card>
          ))
        )}
      </div>

      {/* Details Side Drawer Modal Overlay */}
      {activeJob && (
        <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex justify-end">
          <div className="w-full max-w-xl bg-surface border-l border-border h-full flex flex-col p-6 shadow-2xl overflow-y-auto">
            {/* Header */}
            <div className="flex justify-between items-start pb-4 border-b border-border select-none">
              <div className="space-y-1">
                <Typography type="h3" className="font-semibold text-foreground text-lg leading-tight">
                  {activeJob.title}
                </Typography>
                <div className="flex flex-wrap gap-2 pt-1">
                  <Chip size="sm" variant="soft" color="accent" className="text-[9px] font-medium">
                    {activeJob.department}
                  </Chip>
                  <Chip size="sm" variant="soft" color="warning" className="text-[9px] font-medium">
                    {activeJob.type}
                  </Chip>
                </div>
              </div>
              <button
                onClick={() => setActiveJob(null)}
                className="w-8 h-8 rounded-lg border border-border hover:bg-card/50 flex items-center justify-center text-muted hover:text-foreground transition-colors cursor-pointer text-lg"
              >
                ×
              </button>
            </div>

            {/* Content */}
            <div className="flex-1 py-6 space-y-6">
              {/* Stats Block */}
              <div className="grid grid-cols-2 gap-4 p-4 rounded-xl border border-border bg-card/10 select-none">
                <div>
                  <span className="text-[9px] text-muted-foreground font-medium uppercase block">Location</span>
                  <span className="text-xs font-medium text-foreground">{activeJob.location}</span>
                </div>
                <div>
                  <span className="text-[9px] text-muted-foreground font-medium uppercase block">Compensation</span>
                  <span className="text-xs font-medium text-foreground">{activeJob.salary}</span>
                </div>
              </div>

              {/* Description */}
              <div className="space-y-2">
                <span className="text-[10px] text-muted font-medium uppercase select-none">Job Description</span>
                <Typography type="body-xs" className="text-muted leading-relaxed text-sm font-normal">
                  {activeJob.description}
                </Typography>
              </div>

              {/* Requirements */}
              <div className="space-y-2">
                <span className="text-[10px] text-muted font-medium uppercase select-none">Requirements</span>
                <ul className="list-disc pl-5 space-y-1.5 text-sm text-muted-foreground font-normal">
                  {activeJob.requirements.map((req, idx) => (
                    <li key={idx} className="leading-relaxed">
                      {req}
                    </li>
                  ))}
                </ul>
              </div>
            </div>

            {/* Footer */}
            <div className="pt-4 border-t border-border flex justify-end gap-2 select-none">
              <Button
                onClick={() => setActiveJob(null)}
                variant="bordered"
                size="sm"
                className="font-medium text-xs border-border text-muted hover:text-foreground cursor-pointer rounded-xl"
              >
                Close Details
              </Button>
              <Button
                onClick={() => handleApply(activeJob.id)}
                disabled={appliedJobs.includes(activeJob.id)}
                variant="solid"
                size="sm"
                className={`font-medium text-xs rounded-xl cursor-pointer ${
                  appliedJobs.includes(activeJob.id)
                    ? "bg-success/20 text-success border-none"
                    : "bg-accent text-background hover:bg-accent/90 border-none"
                }`}
              >
                {appliedJobs.includes(activeJob.id) ? "Applied Successfully" : "Apply for Job"}
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
