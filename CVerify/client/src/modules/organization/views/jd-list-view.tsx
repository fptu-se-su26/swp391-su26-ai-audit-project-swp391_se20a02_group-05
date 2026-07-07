"use client";

import Link from 'next/link';
import { useEffect, useState } from 'react';
import { Typography } from '@heroui/react';
import { FileText, Plus, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { jdService } from '../services/jd.service';
import type { JdSummary } from '../types/jd.types';

export function JdListView() {
  const [jds, setJds] = useState<JdSummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    jdService.listJds()
      .then((data) => {
        if (isActive) setJds(data);
      })
      .catch((err) => {
        if (isActive) setError(err instanceof Error ? err.message : 'Failed to load job descriptions.');
      })
      .finally(() => {
        if (isActive) setIsLoading(false);
      });

    return () => {
      isActive = false;
    };
  }, []);

  const handleDelete = async (jdId: string) => {
    if (!window.confirm('Delete this job description?')) return;
    await jdService.deleteJd(jdId);
    setJds((current) => current.filter((jd) => jd.jdId !== jdId));
  };

  return (
    <div className="space-y-6 font-outfit">
      <div className="flex flex-col gap-4 rounded-2xl border border-border bg-surface p-6 md:flex-row md:items-center md:justify-between">
        <div>
          <Typography type="h2" className="flex items-center gap-2 text-xl font-bold text-foreground">
            <FileText size={20} className="text-accent" />
            JD Management
          </Typography>
          <p className="mt-1 text-sm text-muted">Create, edit, delete, and review structured job descriptions for AI matching.</p>
        </div>
        <Link href="/jd/create">
          <Button className="bg-accent text-white hover:bg-accent/90">
            <Plus size={16} className="mr-2" />
            Create JD
          </Button>
        </Link>
      </div>

      {error ? (
        <Card className="border-danger/30 bg-danger/10 p-4 text-sm text-danger">{error}</Card>
      ) : null}

      {isLoading ? (
        <Card className="p-6 text-sm text-muted">Loading job descriptions...</Card>
      ) : jds.length === 0 ? (
        <Card className="p-10 text-center">
          <Typography type="h3" className="font-bold text-foreground">No job descriptions yet</Typography>
          <p className="mt-2 text-sm text-muted">Start by creating a structured JD for recruiter matching.</p>
        </Card>
      ) : (
        <div className="grid gap-4">
          {jds.map((jd) => (
            <Card key={jd.jdId} className="p-5">
              <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
                <div className="space-y-2">
                  <Link href={`/jd/view/${jd.jdId}`} className="text-lg font-bold text-foreground hover:text-accent">
                    {jd.jobTitle}
                  </Link>
                  <div className="flex flex-wrap gap-2 text-xs text-muted">
                    <span>{jd.department || 'Department not set'}</span>
                    <span>•</span>
                    <span>{jd.seniority}</span>
                    <span>•</span>
                    <span>{jd.employmentType || 'Employment type not set'}</span>
                    <span>•</span>
                    <span>{jd.location || 'Location not set'}</span>
                    <span>•</span>
                    <span>{jd.workMode || 'Work mode not set'}</span>
                  </div>
                  <p className="text-xs text-muted">
                    {jd.salaryMin.toLocaleString()}-{jd.salaryMax.toLocaleString()} {jd.currency}
                    {jd.hiringPriority ? ` · ${jd.hiringPriority} priority` : ''}
                  </p>
                </div>
                <div className="flex gap-2">
                  <Link href={`/jd/edit/${jd.jdId}`}>
                    <Button variant="bordered" size="sm">Edit</Button>
                  </Link>
                  <Link href={`/jd/view/${jd.jdId}`}>
                    <Button variant="bordered" size="sm">View</Button>
                  </Link>
                  <Button variant="bordered" size="sm" onClick={() => void handleDelete(jd.jdId)}>
                    <Trash2 size={14} className="mr-1" />
                    Delete
                  </Button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
