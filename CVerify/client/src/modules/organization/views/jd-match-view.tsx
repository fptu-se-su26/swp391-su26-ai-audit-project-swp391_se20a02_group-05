"use client";

import Link from 'next/link';
import { useParams } from 'next/navigation';
import { useEffect, useState } from 'react';
import { Typography } from '@heroui/react';
import { ArrowLeft, Zap } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { TagInput } from '../components/tag-input';
import { ApplicationQualityGate } from '../components/application-quality-gate';
import { jdService } from '../services/jd.service';
import type { JdDetail, JdMatchRequest, JdMatchResponse, CandidateSkillEvidence } from '../types/jd.types';

const SENIORITY_OPTIONS = ['Junior', 'Middle', 'Senior', 'Staff', 'Principal'];

export function JdMatchView() {
  const params = useParams();
  const jdId = typeof params?.id === 'string' ? params.id : '';

  const [detail, setDetail] = useState<JdDetail | null>(null);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [candidateLevel, setCandidateLevel] = useState('Middle');
  const [skillLines, setSkillLines] = useState<string[]>([]);
  const [responsibilities, setResponsibilities] = useState<string[]>([]);
  const [desiredSalary, setDesiredSalary] = useState('');
  const [minSalary, setMinSalary] = useState('');
  const [currency, setCurrency] = useState('USD');

  const [isMatching, setIsMatching] = useState(false);
  const [matchError, setMatchError] = useState<string | null>(null);
  const [result, setResult] = useState<JdMatchResponse | null>(null);

  useEffect(() => {
    if (!jdId) return;
    let active = true;
    jdService.getJd(jdId)
      .then((d) => { if (active) setDetail(d); })
      .catch((e) => { if (active) setLoadError(e instanceof Error ? e.message : 'Failed to load JD.'); });
    return () => { active = false; };
  }, [jdId]);

  const handleMatch = async () => {
    if (!detail) return;
    setIsMatching(true);
    setMatchError(null);
    setResult(null);

    const candidateSkills: CandidateSkillEvidence[] = skillLines.map((line) => {
      const parts = line.split(':');
      const skill = parts[0]?.trim() ?? line.trim();
      const proficiency = parts[1] ? parseFloat(parts[1].trim()) : 3;
      return { skill, proficiency: isNaN(proficiency) ? 3 : proficiency, evidenceStrength: 'self-reported' };
    });

    const jd = detail.normalizedJd;
    const payload: JdMatchRequest = {
      normalizedJd: jd,
      candidateSkills,
      candidateResponsibilities: responsibilities,
      candidateLevel,
      desiredSalary: desiredSalary ? parseFloat(desiredSalary) : null,
      minimumAcceptableSalary: minSalary ? parseFloat(minSalary) : null,
      salaryCurrency: currency as import('../types/jd.types').Currency,
    };

    try {
      const res = await jdService.matchCandidate(payload);
      setResult(res);
    } catch (e) {
      setMatchError(e instanceof Error ? e.message : 'Matching failed.');
    } finally {
      setIsMatching(false);
    }
  };

  if (loadError) return <Card className="p-4 text-sm text-danger">{loadError}</Card>;
  if (!detail) return <Card className="p-6 text-sm text-muted">Loading job description...</Card>;

  const jd = detail.normalizedJd;

  return (
    <div className="space-y-6 font-outfit">
      <div className="flex flex-col gap-3 rounded-2xl border border-border bg-surface p-6 md:flex-row md:items-start md:justify-between">
        <div>
          <Link href={`/jd/view/${jdId}`} className="mb-3 inline-flex items-center gap-1 text-xs text-muted hover:text-accent">
            <ArrowLeft size={14} />
            Back to JD
          </Link>
          <Typography type="h2" className="flex items-center gap-2 text-xl font-bold text-foreground">
            <Zap size={18} className="text-accent" />
            AI Candidate Matching
          </Typography>
          <p className="mt-1 text-sm text-muted">
            Test how a candidate profile scores against <strong>{jd.jobTitle}</strong> ({jd.seniority}).
          </p>
        </div>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        {/* JD Summary */}
        <Card className="p-5">
          <Typography type="h3" className="mb-3 font-bold text-foreground">JD Requirements</Typography>
          <div className="space-y-2 text-sm">
            <InfoRow label="Required Skills" value={jd.requiredSkills.join(', ')} />
            <InfoRow label="Seniority" value={jd.seniority} />
            <InfoRow label="Location" value={jd.location || '—'} />
            <InfoRow label="Salary" value={`${jd.salaryMin.toLocaleString()}–${jd.salaryMax.toLocaleString()} ${jd.currency}`} />
          </div>
        </Card>

        {/* Candidate Input */}
        <Card className="p-5 space-y-4">
          <Typography type="h3" className="font-bold text-foreground">Candidate Profile</Typography>

          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-semibold uppercase tracking-wider text-muted">Seniority Level</label>
            <select
              value={candidateLevel}
              onChange={(e) => setCandidateLevel(e.target.value)}
              className="w-full rounded-xl border border-separator bg-surface px-3 py-2 text-sm text-foreground outline-none focus:border-accent"
            >
              {SENIORITY_OPTIONS.map((s) => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>

          <TagInput
            label='Skills (format: "React:4" or just "React")'
            placeholder="React:4, TypeScript:3, Node.js:3"
            tags={skillLines}
            onChange={setSkillLines}
            maxTags={50}
          />

          <TagInput
            label="Key Responsibilities / Experience"
            placeholder="Built REST APIs, Led frontend team..."
            tags={responsibilities}
            onChange={setResponsibilities}
            maxTags={30}
          />

          <div className="grid grid-cols-3 gap-3">
            <div className="flex flex-col gap-1.5">
              <label className="text-xs font-semibold uppercase tracking-wider text-muted">Currency</label>
              <select
                value={currency}
                onChange={(e) => setCurrency(e.target.value)}
                className="w-full rounded-xl border border-separator bg-surface px-3 py-2 text-sm text-foreground outline-none focus:border-accent"
              >
                <option>USD</option>
                <option>VND</option>
              </select>
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-xs font-semibold uppercase tracking-wider text-muted">Desired Salary</label>
              <input
                type="number"
                value={desiredSalary}
                onChange={(e) => setDesiredSalary(e.target.value)}
                placeholder="e.g. 3000"
                className="w-full rounded-xl border border-separator bg-surface px-3 py-2 text-sm text-foreground outline-none focus:border-accent"
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-xs font-semibold uppercase tracking-wider text-muted">Minimum Accept</label>
              <input
                type="number"
                value={minSalary}
                onChange={(e) => setMinSalary(e.target.value)}
                placeholder="e.g. 2500"
                className="w-full rounded-xl border border-separator bg-surface px-3 py-2 text-sm text-foreground outline-none focus:border-accent"
              />
            </div>
          </div>

          {matchError && <p className="text-xs text-danger">{matchError}</p>}

          <Button
            onClick={() => void handleMatch()}
            disabled={isMatching || skillLines.length === 0}
            className="w-full bg-accent text-white hover:bg-accent/90 disabled:opacity-50"
          >
            {isMatching ? 'Running AI Match…' : 'Run Matching'}
          </Button>
        </Card>
      </div>

      {result && <MatchResult result={result} />}
    </div>
  );
}

function MatchResult({ result }: { result: JdMatchResponse }) {
  const scoreColor = result.cappedMatchScorePercent >= 75
    ? 'text-success'
    : result.cappedMatchScorePercent >= 50
      ? 'text-warning'
      : 'text-danger';

  return (
    <div className="space-y-4">
      {/* Score Header */}
      <Card className="p-6">
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="text-xs font-bold uppercase tracking-wider text-muted">Overall Match Score</p>
            <p className={`text-4xl font-bold ${scoreColor}`}>{result.cappedMatchScorePercent.toFixed(1)}%</p>
            <p className="mt-1 text-sm font-semibold text-foreground">{result.matchLabel}</p>
          </div>
          <div className="grid grid-cols-2 gap-3 md:grid-cols-4">
            <ScoreCard label="Skills" value={result.skillMatch} />
            <ScoreCard label="Experience" value={result.experienceMatch} />
            <ScoreCard label="Projects" value={result.projectRelevance} />
            <ScoreCard label="Trust" value={result.trustWeightedScore} />
          </div>
        </div>
        {result.activeFlags.length > 0 && (
          <div className="mt-4 flex flex-wrap gap-2">
            {result.activeFlags.map((flag) => (
              <span key={flag} className="rounded-full border border-warning/30 bg-warning/10 px-2 py-0.5 text-xs font-semibold text-warning">
                {flag}
              </span>
            ))}
          </div>
        )}
      </Card>

      {/* Recommendation */}
      <Card className="p-5">
        <Typography type="h3" className="mb-3 font-bold text-foreground">Hiring Recommendation</Typography>
        <VerdictBadge verdict={result.hiringRecommendation.verdict} />
        <p className="mt-3 text-sm text-foreground/85">{result.hiringRecommendation.oneParaSummary}</p>
        <p className="mt-2 text-xs text-muted">
          Confidence: {(result.hiringRecommendation.confidence * 100).toFixed(0)}% · Risk: {result.hiringRecommendation.hiringRisk}
        </p>
      </Card>

      {/* Quality Gate */}
      <ApplicationQualityGate match={result} onConfirmApply={() => undefined} />

      {/* Gap Analysis */}
      <Card className="p-5">
        <Typography type="h3" className="mb-3 font-bold text-foreground">Gap Analysis</Typography>
        <SeverityBadge severity={result.gapAnalysis.gapSeverity} />
        <p className="mt-2 text-sm text-foreground/85">{result.gapAnalysis.overallGapSummary}</p>

        {result.gapAnalysis.skillGaps.length > 0 && (
          <div className="mt-3">
            <p className="mb-1 text-xs font-bold text-muted">Missing Skills</p>
            <div className="flex flex-wrap gap-1.5">
              {result.gapAnalysis.skillGaps.map((s) => (
                <span key={s} className="rounded-full border border-danger/30 bg-danger/10 px-2 py-0.5 text-xs text-danger">{s}</span>
              ))}
            </div>
          </div>
        )}

        {result.gapAnalysis.improvementSuggestions.length > 0 && (
          <div className="mt-3">
            <p className="mb-1 text-xs font-bold text-muted">Suggestions</p>
            <ul className="list-disc space-y-1 pl-4 text-xs text-foreground/75">
              {result.gapAnalysis.improvementSuggestions.slice(0, 5).map((s, i) => (
                <li key={i}>{s}</li>
              ))}
            </ul>
          </div>
        )}
      </Card>

      {/* Strengths & Weaknesses */}
      <div className="grid gap-4 md:grid-cols-2">
        <Card className="p-5">
          <p className="mb-2 text-xs font-bold uppercase tracking-wider text-success">Strengths</p>
          <ul className="space-y-1 text-sm text-foreground/85">
            {result.strengths.map((s, i) => <li key={i}>✓ {s}</li>)}
          </ul>
        </Card>
        <Card className="p-5">
          <p className="mb-2 text-xs font-bold uppercase tracking-wider text-warning">Weaknesses</p>
          <ul className="space-y-1 text-sm text-foreground/85">
            {result.weaknesses.map((w, i) => <li key={i}>⚠ {w}</li>)}
          </ul>
        </Card>
      </div>

      {/* Skill Match Detail */}
      {result.requiredSkillsMatch.length > 0 && (
        <Card className="p-5">
          <Typography type="h3" className="mb-3 font-bold text-foreground">Required Skills Breakdown</Typography>
          <div className="space-y-2">
            {result.requiredSkillsMatch.map((item) => (
              <div key={item.skill} className="flex items-center justify-between rounded-xl border border-separator bg-surface/50 px-3 py-2 text-sm">
                <div className="flex items-center gap-2">
                  <span className={item.matched ? 'text-success' : 'text-danger'}>{item.matched ? '✓' : '✗'}</span>
                  <span className="font-medium text-foreground">{item.skill}</span>
                </div>
                <div className="flex items-center gap-3 text-xs text-muted">
                  <span>{item.matchType}</span>
                  {item.matched && <span>proficiency {(item.candidateProficiency * 5).toFixed(1)}/5</span>}
                </div>
              </div>
            ))}
          </div>
        </Card>
      )}
    </div>
  );
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex gap-2">
      <span className="w-28 shrink-0 text-muted">{label}:</span>
      <span className="text-foreground">{value}</span>
    </div>
  );
}

function ScoreCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-xl border border-separator bg-surface p-3 text-center">
      <p className="text-lg font-bold text-foreground">{value.toFixed(0)}%</p>
      <p className="text-xs text-muted">{label}</p>
    </div>
  );
}

function VerdictBadge({ verdict }: { verdict: string }) {
  const colorMap: Record<string, string> = {
    Yes: 'bg-success/10 text-success border-success/30',
    Conditional: 'bg-warning/10 text-warning border-warning/30',
    No: 'bg-danger/10 text-danger border-danger/30',
  };
  return (
    <span className={`inline-flex rounded-full border px-3 py-1 text-sm font-bold ${colorMap[verdict] ?? 'bg-separator text-muted border-separator'}`}>
      {verdict === 'Yes' ? '✓ Hire' : verdict === 'Conditional' ? '? Conditional' : '✗ No Hire'}
    </span>
  );
}

function SeverityBadge({ severity }: { severity: string }) {
  const colorMap: Record<string, string> = {
    none: 'bg-success/10 text-success border-success/30',
    minor: 'bg-success/10 text-success border-success/30',
    significant: 'bg-warning/10 text-warning border-warning/30',
    critical: 'bg-danger/10 text-danger border-danger/30',
  };
  return (
    <span className={`inline-flex rounded-full border px-3 py-1 text-xs font-bold capitalize ${colorMap[severity] ?? 'bg-separator'}`}>
      {severity} gap
    </span>
  );
}
