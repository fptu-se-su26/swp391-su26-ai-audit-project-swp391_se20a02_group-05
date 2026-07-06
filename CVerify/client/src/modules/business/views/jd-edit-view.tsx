"use client";

import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { Typography } from '@heroui/react';
import { ArrowLeft } from 'lucide-react';
import { Card } from '@/components/ui/card';
import { JdInputForm } from '../components/jd-input-form';
import { jdService } from '../services/jd.service';
import type { JdDetail, JdFormData } from '../types/jd.types';

export function JdEditView() {
  const params = useParams();
  const router = useRouter();
  const jdId = typeof params?.id === 'string' ? params.id : '';
  const [detail, setDetail] = useState<JdDetail | null>(null);
  const [isSaving, setIsSaving] = useState(false);
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

  const handleSubmit = async (formData: JdFormData) => {
    if (!detail) return;
    setIsSaving(true);
    setError(null);
    try {
      await jdService.updateJd(jdId, formData, detail.generatedJdText);
      router.push(`/jd/view/${jdId}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update job description.');
    } finally {
      setIsSaving(false);
    }
  };

  if (error && !detail) return <Card className="p-4 text-sm text-danger">{error}</Card>;
  if (!detail) return <Card className="p-6 text-sm text-muted">Loading job description...</Card>;

  return (
    <div className="space-y-6 font-outfit">
      <div className="rounded-2xl border border-border bg-surface p-6">
        <Link href={`/jd/view/${jdId}`} className="mb-3 inline-flex items-center gap-1 text-xs text-muted hover:text-accent">
          <ArrowLeft size={14} />
          Back to JD detail
        </Link>
        <Typography type="h2" className="text-xl font-bold text-foreground">Edit Job Description</Typography>
        <p className="mt-1 text-sm text-muted">Update structured recruiter fields used by JD matching.</p>
      </div>

      {error ? <Card className="border-danger/30 bg-danger/10 p-4 text-sm text-danger">{error}</Card> : null}

      <JdInputForm
        initialData={detail.normalizedJd}
        isLoading={isSaving}
        onSubmit={handleSubmit}
        submitLabel="Save JD"
      />
    </div>
  );
}
