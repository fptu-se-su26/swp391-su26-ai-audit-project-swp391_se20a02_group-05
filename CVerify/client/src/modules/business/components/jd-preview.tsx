"use client";

import React from 'react';
import { Typography } from '@heroui/react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { CheckCircle, FileText, ArrowLeft, Download } from 'lucide-react';
import type { NormalizedJd } from '../types/jd.types';

type Props = {
  normalizedJd: NormalizedJd;
  generatedText: string;
  wordCount: number;
  jdId?: string;
  onBack: () => void;
  onCreateNew: () => void;
};

export function JdPreview({ normalizedJd, generatedText, wordCount, jdId, onBack, onCreateNew }: Props) {
  const handleDownload = () => {
    const blob = new Blob([generatedText], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${normalizedJd.jobTitle.replace(/\s+/g, '_')}_JD.txt`;
    a.click();
    URL.revokeObjectURL(url);
  };

  return (
    <div className="space-y-6">
      {/* Success Banner */}
      <div className="flex items-center gap-3 p-4 rounded-xl bg-success/10 border border-success/30">
        <CheckCircle size={20} className="text-success shrink-0" />
        <div>
          <p className="text-sm font-semibold text-success">Job Description Generated Successfully</p>
          <p className="text-xs text-muted mt-0.5">
            {wordCount} words · {jdId ? `ID: ${jdId}` : 'Ready for use'}
          </p>
        </div>
      </div>

      {/* Summary cards */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
        <SummaryCard label="Position" value={normalizedJd.jobTitle} />
        <SummaryCard label="Seniority" value={normalizedJd.seniority} />
        <SummaryCard label="Working Model" value={normalizedJd.workingModel} />
        <SummaryCard
          label="Salary Range"
          value={`${normalizedJd.salaryMin.toLocaleString()}–${normalizedJd.salaryMax.toLocaleString()} ${normalizedJd.currency}`}
        />
      </div>

      {/* Generated JD Text */}
      <Card glow={false}>
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-2">
            <FileText size={16} className="text-accent" />
            <Typography type="h3" className="font-bold text-foreground">Generated Job Description</Typography>
          </div>
          <Button variant="bordered" size="sm" onClick={handleDownload} className="cursor-pointer">
            <Download size={14} className="mr-1" />
            Download
          </Button>
        </div>
        <pre className="whitespace-pre-wrap text-sm text-foreground/85 font-sans leading-relaxed bg-surface/50 p-4 rounded-xl border border-separator max-h-[500px] overflow-y-auto">
          {generatedText}
        </pre>
      </Card>

      {/* Normalized JD Detail */}
      <Card glow={false}>
        <Typography type="h3" className="font-bold text-foreground mb-4">Structured JD Data</Typography>
        <div className="space-y-3">
          <DetailRow label="Required Skills" value={normalizedJd.requiredSkills.join(', ')} />
          {normalizedJd.preferredSkills.length > 0 && (
            <DetailRow label="Preferred Skills" value={normalizedJd.preferredSkills.join(', ')} />
          )}
          <DetailRow label="Experience" value={`${normalizedJd.experienceYearsMin}–${normalizedJd.experienceYearsMax} years`} />
          <DetailRow label="Education" value={normalizedJd.educationRequirement} />
          <DetailRow label="English Level" value={normalizedJd.englishLevel} />
          <DetailRow label="Location" value={normalizedJd.location} />
        </div>
      </Card>

      {/* Actions */}
      <div className="flex gap-3 justify-end">
        <Button variant="bordered" onClick={onBack} className="cursor-pointer">
          <ArrowLeft size={14} className="mr-1" />
          Edit Form
        </Button>
        <Button
          variant="solid"
          onClick={onCreateNew}
          className="bg-accent hover:bg-accent/90 border-none cursor-pointer"
        >
          Create Another JD
        </Button>
      </div>
    </div>
  );
}

function SummaryCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="p-3 rounded-xl border border-separator bg-surface">
      <p className="text-xs text-muted uppercase font-bold tracking-wider mb-1">{label}</p>
      <p className="text-sm font-semibold text-foreground truncate">{value}</p>
    </div>
  );
}

function DetailRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex gap-3 text-sm">
      <span className="text-muted font-medium w-36 shrink-0">{label}</span>
      <span className="text-foreground">{value}</span>
    </div>
  );
}
