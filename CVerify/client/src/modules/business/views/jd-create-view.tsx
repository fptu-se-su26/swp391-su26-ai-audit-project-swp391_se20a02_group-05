"use client";

import React, { useState } from 'react';
import { Typography } from '@heroui/react';
import { FileText, Sparkles } from 'lucide-react';
import { JdInputForm } from '../components/jd-input-form';
import { JdPreview } from '../components/jd-preview';
import { jdService } from '../services/jd.service';
import type { JdFormData, JdCreateState } from '../types/jd.types';

export function JdCreateView() {
  const [state, setState] = useState<JdCreateState>({ step: 'form' });
  const [savedJdId, setSavedJdId] = useState<string | undefined>();

  const handleFormSubmit = async (formData: JdFormData) => {
    setState({ step: 'validating' });
    try {
      setState({ step: 'generating', normalizedJd: formData as never });

      const result = await jdService.createJd(formData);

      if (!result.isValid) {
        setState({ step: 'error', message: result.validationErrors.join('\n') });
        return;
      }

      setSavedJdId(result.jdId);
      setState({
        step: 'preview',
        normalizedJd: result.normalizedJd as never,
        generatedText: result.generatedJdText ?? '',
        wordCount: result.wordCount,
      });
    } catch (err) {
      setState({
        step: 'error',
        message: err instanceof Error ? err.message : 'An unexpected error occurred.',
      });
    }
  };

  const handleBack = () => setState({ step: 'form' });
  const handleCreateNew = () => {
    setSavedJdId(undefined);
    setState({ step: 'form' });
  };

  const isLoading = state.step === 'validating' || state.step === 'generating';

  return (
    <div className="space-y-6 font-outfit">
      {/* Header */}
      <div className="dark flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 rounded-2xl bg-background border border-border text-foreground select-none">
        <div className="space-y-1">
          <Typography type="h2" className="text-xl font-bold flex items-center gap-2 text-foreground">
            Create Job Description
            <Sparkles size={18} className="text-accent" />
          </Typography>
          <Typography type="body-xs" className="text-muted font-light mt-0.5">
            Fill in the structured form and let AI generate a professional JD.
          </Typography>
        </div>
        <div className="flex items-center gap-2 px-3 py-1.5 rounded-xl bg-accent/10 border border-accent/20">
          <FileText size={14} className="text-accent" />
          <span className="text-xs font-semibold text-accent">
            {state.step === 'validating' && 'Validating fields…'}
            {state.step === 'generating' && 'Generating JD with AI…'}
            {state.step === 'form' && 'Step 1 of 3 — Fill Form'}
            {state.step === 'preview' && 'Step 3 of 3 — Preview'}
            {state.step === 'error' && 'Error occurred'}
          </span>
        </div>
      </div>

      {/* Progress Steps */}
      <StepIndicator currentStep={state.step} />

      {/* Error State */}
      {state.step === 'error' && (
        <div className="p-4 rounded-xl bg-danger/10 border border-danger/30">
          <p className="text-sm font-semibold text-danger mb-1">Validation Failed</p>
          <pre className="text-xs text-danger/80 whitespace-pre-wrap">{state.message}</pre>
          <button
            onClick={handleBack}
            className="mt-3 text-xs text-accent underline cursor-pointer"
          >
            ← Go back and fix
          </button>
        </div>
      )}

      {/* Form */}
      {(state.step === 'form' || state.step === 'validating' || state.step === 'generating') && (
        <JdInputForm onSubmit={handleFormSubmit} isLoading={isLoading} />
      )}

      {/* Preview */}
      {state.step === 'preview' && (
        <JdPreview
          normalizedJd={(state as Extract<JdCreateState, { step: 'preview' }>).normalizedJd}
          generatedText={(state as Extract<JdCreateState, { step: 'preview' }>).generatedText}
          wordCount={(state as Extract<JdCreateState, { step: 'preview' }>).wordCount}
          jdId={savedJdId}
          onBack={handleBack}
          onCreateNew={handleCreateNew}
        />
      )}
    </div>
  );
}

type Step = JdCreateState['step'];

function StepIndicator({ currentStep }: { currentStep: Step }) {
  const steps = [
    { key: 'form', label: 'Fill Form' },
    { key: 'validating', label: 'Validate' },
    { key: 'generating', label: 'Generate' },
    { key: 'preview', label: 'Preview' },
  ];

  const activeIndex = steps.findIndex((s) => s.key === currentStep);

  return (
    <div className="flex items-center gap-2">
      {steps.map((s, i) => (
        <React.Fragment key={s.key}>
          <div className="flex items-center gap-1.5">
            <div
              className={[
                'w-6 h-6 rounded-full flex items-center justify-center text-xs font-bold transition-colors',
                i <= activeIndex
                  ? 'bg-accent text-white'
                  : 'bg-separator text-muted',
              ].join(' ')}
            >
              {i + 1}
            </div>
            <span className={[
              'text-xs font-medium hidden sm:block',
              i <= activeIndex ? 'text-accent' : 'text-muted',
            ].join(' ')}>
              {s.label}
            </span>
          </div>
          {i < steps.length - 1 && (
            <div className={['h-px flex-1 transition-colors', i < activeIndex ? 'bg-accent' : 'bg-separator'].join(' ')} />
          )}
        </React.Fragment>
      ))}
    </div>
  );
}
