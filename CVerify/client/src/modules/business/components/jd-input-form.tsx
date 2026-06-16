"use client";

import React, { useState } from 'react';
import { Typography } from '@heroui/react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { TagInput } from './tag-input';
import type { JdFormData, Seniority, WorkingModel, Currency } from '../types/jd.types';

type Props = {
  onSubmit: (data: JdFormData) => void;
  isLoading: boolean;
};

const SENIORITY_OPTIONS: Seniority[] = ['Junior', 'Middle', 'Senior', 'Staff', 'Principal'];
const WORKING_MODEL_OPTIONS: WorkingModel[] = ['remote', 'hybrid', 'onsite'];
const ENGLISH_LEVEL_OPTIONS = ['Basic', 'Intermediate', 'Upper-Intermediate', 'Advanced', 'Fluent', 'Native'];
const EDUCATION_OPTIONS = ['High School', 'Associate Degree', "Bachelor's Degree", "Master's Degree", 'PhD', 'No Requirement'];
const CURRENCY_OPTIONS: Currency[] = ['USD', 'VND'];

const DEFAULT_FORM: JdFormData = {
  jobTitle: '',
  seniority: 'Middle',
  requiredSkills: [],
  preferredSkills: [],
  responsibilities: [],
  experienceYearsMin: 0,
  experienceYearsMax: 3,
  educationRequirement: "Bachelor's Degree",
  englishLevel: 'Intermediate',
  salaryMin: 1000,
  salaryMax: 2000,
  currency: 'USD',
  location: '',
  workingModel: 'hybrid',
};

export function JdInputForm({ onSubmit, isLoading }: Props) {
  const [form, setForm] = useState<JdFormData>(DEFAULT_FORM);
  const [errors, setErrors] = useState<Partial<Record<keyof JdFormData, string>>>({});

  const set = <K extends keyof JdFormData>(key: K, value: JdFormData[K]) => {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => ({ ...prev, [key]: undefined }));
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof JdFormData, string>> = {};
    if (!form.jobTitle.trim()) newErrors.jobTitle = 'Job title is required';
    if (form.requiredSkills.length === 0) newErrors.requiredSkills = 'At least one required skill';
    if (form.responsibilities.length === 0) newErrors.responsibilities = 'At least one responsibility';
    if (!form.location.trim()) newErrors.location = 'Location is required';
    if (form.salaryMin > form.salaryMax) newErrors.salaryMax = 'Salary max must be ≥ salary min';
    if (form.experienceYearsMin > form.experienceYearsMax) newErrors.experienceYearsMax = 'Max experience must be ≥ min';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validate()) onSubmit(form);
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      {/* Section: Basic Info */}
      <Card glow={false}>
        <Typography type="h3" className="font-bold text-foreground mb-4">Basic Information</Typography>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Field label="Job Title" required error={errors.jobTitle}>
            <input
              type="text"
              value={form.jobTitle}
              onChange={(e) => set('jobTitle', e.target.value)}
              placeholder="e.g. Senior Backend Engineer"
              className={fieldClass(!!errors.jobTitle)}
            />
          </Field>

          <Field label="Seniority Level" required>
            <select
              value={form.seniority}
              onChange={(e) => set('seniority', e.target.value as Seniority)}
              className={fieldClass(false)}
            >
              {SENIORITY_OPTIONS.map((s) => (
                <option key={s} value={s}>{s}</option>
              ))}
            </select>
          </Field>

          <Field label="Location" required error={errors.location}>
            <input
              type="text"
              value={form.location}
              onChange={(e) => set('location', e.target.value)}
              placeholder="e.g. Ho Chi Minh City, Vietnam"
              className={fieldClass(!!errors.location)}
            />
          </Field>

          <Field label="Working Model" required>
            <select
              value={form.workingModel}
              onChange={(e) => set('workingModel', e.target.value as WorkingModel)}
              className={fieldClass(false)}
            >
              {WORKING_MODEL_OPTIONS.map((m) => (
                <option key={m} value={m}>{m.charAt(0).toUpperCase() + m.slice(1)}</option>
              ))}
            </select>
          </Field>
        </div>
      </Card>

      {/* Section: Skills */}
      <Card glow={false}>
        <Typography type="h3" className="font-bold text-foreground mb-4">Skills</Typography>
        <div className="space-y-4">
          <TagInput
            label="Required Skills"
            placeholder="e.g. React, Node.js, PostgreSQL"
            tags={form.requiredSkills}
            onChange={(tags) => set('requiredSkills', tags)}
            required
          />
          {errors.requiredSkills && <p className="text-xs text-danger">{errors.requiredSkills}</p>}

          <TagInput
            label="Preferred Skills"
            placeholder="e.g. Docker, Kubernetes, GraphQL"
            tags={form.preferredSkills}
            onChange={(tags) => set('preferredSkills', tags)}
          />
        </div>
      </Card>

      {/* Section: Responsibilities */}
      <Card glow={false}>
        <Typography type="h3" className="font-bold text-foreground mb-4">Responsibilities</Typography>
        <TagInput
          label="Key Responsibilities"
          placeholder="e.g. Design and implement REST APIs"
          tags={form.responsibilities}
          onChange={(tags) => set('responsibilities', tags)}
          required
        />
        {errors.responsibilities && <p className="text-xs text-danger">{errors.responsibilities}</p>}
      </Card>

      {/* Section: Requirements */}
      <Card glow={false}>
        <Typography type="h3" className="font-bold text-foreground mb-4">Requirements</Typography>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Field label="Experience Min (years)">
            <input
              type="number"
              min={0}
              max={20}
              value={form.experienceYearsMin}
              onChange={(e) => set('experienceYearsMin', Number(e.target.value))}
              className={fieldClass(false)}
            />
          </Field>

          <Field label="Experience Max (years)" error={errors.experienceYearsMax}>
            <input
              type="number"
              min={0}
              max={20}
              value={form.experienceYearsMax}
              onChange={(e) => set('experienceYearsMax', Number(e.target.value))}
              className={fieldClass(!!errors.experienceYearsMax)}
            />
          </Field>

          <Field label="Education">
            <select
              value={form.educationRequirement}
              onChange={(e) => set('educationRequirement', e.target.value)}
              className={fieldClass(false)}
            >
              {EDUCATION_OPTIONS.map((e) => (
                <option key={e} value={e}>{e}</option>
              ))}
            </select>
          </Field>

          <Field label="English Level">
            <select
              value={form.englishLevel}
              onChange={(e) => set('englishLevel', e.target.value)}
              className={fieldClass(false)}
            >
              {ENGLISH_LEVEL_OPTIONS.map((l) => (
                <option key={l} value={l}>{l}</option>
              ))}
            </select>
          </Field>
        </div>
      </Card>

      {/* Section: Salary */}
      <Card glow={false}>
        <Typography type="h3" className="font-bold text-foreground mb-4">Compensation</Typography>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Field label="Currency">
            <select
              value={form.currency}
              onChange={(e) => set('currency', e.target.value as Currency)}
              className={fieldClass(false)}
            >
              {CURRENCY_OPTIONS.map((c) => (
                <option key={c} value={c}>{c}</option>
              ))}
            </select>
          </Field>

          <Field label={`Salary Min (${form.currency})`}>
            <input
              type="number"
              min={0}
              value={form.salaryMin}
              onChange={(e) => set('salaryMin', Number(e.target.value))}
              className={fieldClass(false)}
            />
          </Field>

          <Field label={`Salary Max (${form.currency})`} error={errors.salaryMax}>
            <input
              type="number"
              min={0}
              value={form.salaryMax}
              onChange={(e) => set('salaryMax', Number(e.target.value))}
              className={fieldClass(!!errors.salaryMax)}
            />
          </Field>
        </div>
      </Card>

      <div className="flex justify-end">
        <Button
          type="submit"
          variant="solid"
          disabled={isLoading}
          className="bg-accent hover:bg-accent/90 border-none min-w-[180px]"
        >
          {isLoading ? 'Generating JD...' : 'Validate & Generate JD'}
        </Button>
      </div>
    </form>
  );
}

function Field({ label, required, error, children }: {
  label: string;
  required?: boolean;
  error?: string;
  children: React.ReactNode;
}) {
  return (
    <div className="flex flex-col gap-1.5">
      <label className="text-xs font-semibold text-muted uppercase tracking-wider">
        {label}
        {required && <span className="text-danger ml-1">*</span>}
      </label>
      {children}
      {error && <p className="text-xs text-danger">{error}</p>}
    </div>
  );
}

function fieldClass(hasError: boolean) {
  return [
    'w-full px-3 py-2 rounded-xl border text-sm text-foreground bg-surface outline-none transition-colors',
    'focus:border-accent',
    hasError ? 'border-danger' : 'border-separator',
  ].join(' ');
}
