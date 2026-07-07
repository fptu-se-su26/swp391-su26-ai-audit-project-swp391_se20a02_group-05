"use client";

import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { Typography } from '@heroui/react';
import { ArrowLeft, Edit, Trash2, Zap } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { jdService } from '../services/jd.service';
import type { JdDetail } from '../types/jd.types';

export function JdDetailView() {
  const params = useParams();
  const router = useRouter();
  const jdId = typeof params?.id === 'string' ? params.id : '';
  const [detail, setDetail] = useState<JdDetail | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!jdId) return;

    let isActive = true;
    jdService.getJd(jdId)
      .then((data) => {
        if (isActive) setDetail(data);
      })
      .catch((err) => {
        if (isActive) setError(err instanceof Error ? err.message : 'Failed to load job description.');
      });

    return () => {
      isActive = false;
    };
  }, [jdId]);

  const handleDelete = async () => {
    if (!jdId || !window.confirm('Delete this job description?')) return;
    await jdService.deleteJd(jdId);
    router.push('/jd');
  };

  if (error) return <Card className="p-4 text-sm text-danger">{error}</Card>;
  if (!detail) return <Card className="p-6 text-sm text-muted">Loading job description...</Card>;

  const jd = detail.normalizedJd;

  return (
    <div className="space-y-6 font-outfit">
      <div className="flex flex-col gap-3 rounded-2xl border border-border bg-surface p-6 md:flex-row md:items-start md:justify-between">
        <div>
          <Link href="/jd" className="mb-3 inline-flex items-center gap-1 text-xs text-muted hover:text-accent">
            <ArrowLeft size={14} />
            Back to JD list
          </Link>
          <Typography type="h2" className="text-2xl font-bold text-foreground">{jd.jobTitle}</Typography>
          <p className="mt-1 text-sm text-muted">
            {jd.department} · {jd.seniority} · {jd.employmentType} · {jd.location} · {jd.workMode ?? jd.workingModel}
          </p>
        </div>
        <div className="flex gap-2">
          <Link href={`/jd/${detail.jdId}/match`}>
            <Button size="sm" className="bg-accent text-white hover:bg-accent/90">
              <Zap size={14} className="mr-1" />
              Match Candidate
            </Button>
          </Link>
          <Link href={`/jd/edit/${detail.jdId}`}>
            <Button variant="bordered" size="sm">
              <Edit size={14} className="mr-1" />
              Edit
            </Button>
          </Link>
          <Button variant="bordered" size="sm" onClick={() => void handleDelete()}>
            <Trash2 size={14} className="mr-1" />
            Delete
          </Button>
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-4">
        <SummaryCard label="Salary" value={`${jd.salaryMin.toLocaleString()}-${jd.salaryMax.toLocaleString()} ${jd.currency}`} />
        <SummaryCard label="Experience" value={`${jd.experienceYearsMin}-${jd.experienceYearsMax} years`} />
        <SummaryCard label="Industry" value={jd.industry || 'Not set'} />
        <SummaryCard label="Priority" value={jd.hiringPriority || 'Medium'} />
      </div>

      <Card className="p-6">
        <Typography type="h3" className="mb-4 font-bold text-foreground">Structured Requirements</Typography>
        <DetailRow label="Required Skills" value={jd.requiredSkills.join(', ')} />
        <DetailRow label="Preferred Skills" value={jd.preferredSkills.join(', ') || 'None'} />
        <DetailRow label="Tech Stack" value={jd.techStack?.join(', ') || 'None'} />
        <DetailRow label="Must Have" value={jd.mustHave?.join('; ') || 'None'} />
        <DetailRow label="Nice To Have" value={jd.niceToHave?.join('; ') || 'None'} />
        <DetailRow label="Education" value={jd.educationRequirement} />
        <DetailRow label="Languages" value={jd.languages?.join(', ') || jd.englishLevel} />
      </Card>

      <Card className="p-6">
        <Typography type="h3" className="mb-4 font-bold text-foreground">Responsibilities</Typography>
        <ul className="list-disc space-y-2 pl-5 text-sm text-foreground/85">
          {jd.responsibilities.map((item) => <li key={item}>{item}</li>)}
        </ul>
      </Card>

      <Card className="p-6">
        <Typography type="h3" className="mb-4 font-bold text-foreground">Generated JD Text</Typography>
        <pre className="max-h-[560px] overflow-auto whitespace-pre-wrap rounded-xl border border-separator bg-surface/50 p-4 font-sans text-sm leading-relaxed text-foreground/85">
          {detail.generatedJdText || 'No generated JD text stored.'}
        </pre>
      </Card>
    </div>
  );
}

function SummaryCard({ label, value }: { label: string; value: string }) {
  return (
    <Card className="p-4">
      <p className="text-xs font-bold uppercase tracking-wider text-muted">{label}</p>
      <p className="mt-1 text-sm font-semibold text-foreground">{value}</p>
    </Card>
  );
}

function DetailRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="grid gap-1 border-b border-separator py-3 text-sm last:border-0 md:grid-cols-[180px_1fr]">
      <span className="font-semibold text-muted">{label}</span>
      <span className="text-foreground">{value}</span>
    </div>
  );
}
