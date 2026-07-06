"use client";

import React, { useState } from 'react';
import { AlertTriangle, CheckCircle, ShieldCheck } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import type { JdMatchResponse } from '../types/jd.types';

type Props = {
  match: JdMatchResponse;
  onConfirmApply: () => void;
  onCancel?: () => void;
};

export function ApplicationQualityGate({ match, onConfirmApply, onCancel }: Props) {
  const [confirmed, setConfirmed] = useState(false);
  const gate = match.qualityGate;
  const needsConfirmation = gate.requiresExplicitConfirmation;
  const canContinue = !needsConfirmation || confirmed;

  return (
    <Card glow={false}>
      <div className="flex items-start gap-3">
        {needsConfirmation ? (
          <AlertTriangle size={20} className="text-warning shrink-0 mt-0.5" />
        ) : (
          <CheckCircle size={20} className="text-success shrink-0 mt-0.5" />
        )}
        <div className="flex-1 space-y-4">
          <div>
            <p className="text-sm font-bold text-foreground">
              {needsConfirmation ? 'Review application warnings' : 'Application quality gate clear'}
            </p>
            <p className="text-xs text-muted mt-1">
              Displayed match score: {(match.cappedMatchScorePercent ?? 0).toFixed(1)}% ({match.matchLabel})
            </p>
          </div>

          {gate.warnings.length > 0 ? (
            <div className="space-y-2">
              {gate.warnings.map((warning, index) => (
                <div key={`${warning}-${index}`} className="rounded-xl border border-warning/30 bg-warning/10 px-3 py-2 text-xs text-warning">
                  {warning}
                </div>
              ))}
            </div>
          ) : (
            <div className="rounded-xl border border-success/30 bg-success/10 px-3 py-2 text-xs text-success">
              No salary, required skill, or seniority blockers were detected.
            </div>
          )}

          {match.gapAnalysis.improvementSuggestions.length > 0 && (
            <div className="space-y-2">
              <p className="text-xs font-bold uppercase tracking-wider text-muted">Gap improvement suggestions</p>
              <ul className="space-y-1 text-xs text-foreground/80">
                {match.gapAnalysis.improvementSuggestions.map((suggestion, index) => (
                  <li key={`${suggestion}-${index}`}>- {suggestion}</li>
                ))}
              </ul>
            </div>
          )}

          <div className="rounded-xl border border-separator bg-surface/60 px-3 py-2">
            <div className="flex items-center gap-2 text-xs font-semibold text-foreground">
              <ShieldCheck size={14} className="text-accent" />
              Hiring recommendation: {match.hiringRecommendation.verdict}
            </div>
            <p className="mt-1 text-xs text-muted">{match.hiringRecommendation.oneParaSummary}</p>
          </div>

          {needsConfirmation && (
            <label className="flex items-start gap-2 text-xs text-foreground cursor-pointer">
              <input
                type="checkbox"
                checked={confirmed}
                onChange={(event) => setConfirmed(event.currentTarget.checked)}
                className="mt-0.5"
              />
              I understand the salary, skill, or seniority warnings and still want to apply.
            </label>
          )}

          <div className="flex justify-end gap-2">
            {onCancel && (
              <Button variant="bordered" onClick={onCancel} className="cursor-pointer">
                Cancel
              </Button>
            )}
            <Button
              variant="solid"
              disabled={!canContinue || !gate.canApply}
              onClick={onConfirmApply}
              className="bg-accent hover:bg-accent/90 border-none cursor-pointer"
            >
              {needsConfirmation ? 'Confirm and Apply' : 'Apply'}
            </Button>
          </div>
        </div>
      </div>
    </Card>
  );
}
