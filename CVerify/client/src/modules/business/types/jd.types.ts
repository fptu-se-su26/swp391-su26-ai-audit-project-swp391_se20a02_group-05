export type Seniority = 'Junior' | 'Middle' | 'Senior' | 'Staff' | 'Principal';
export type WorkingModel = 'remote' | 'hybrid' | 'onsite';
export type Currency = 'USD' | 'VND';

export type JdFormData = {
  jobTitle: string;
  seniority: Seniority;
  requiredSkills: string[];
  preferredSkills: string[];
  responsibilities: string[];
  experienceYearsMin: number;
  experienceYearsMax: number;
  educationRequirement: string;
  englishLevel: string;
  salaryMin: number;
  salaryMax: number;
  currency: Currency;
  location: string;
  workingModel: WorkingModel;
};

export type NormalizedJd = JdFormData & {
  roleType?: string;
};

export type ValidationResult = {
  isValid: boolean;
  validationErrors: string[];
  normalizedJd: NormalizedJd;
};

export type JdGenerationResult = {
  generatedJdText: string;
  wordCount: number;
  sections: {
    aboutRole: string;
    keyResponsibilities: string[];
    requiredSkills: string[];
    preferredSkills: string[];
    whatWeOffer: string;
  };
};

export type StoredJd = {
  jdId: string;
  storedAt: string;
  structuredJson: NormalizedJd;
  humanReadableText: string;
  storageStatus: string;
};

export type CandidateSkillEvidence = {
  skill: string;
  proficiency: number;
  evidenceStrength?: string;
};

export type JdMatchRequest = {
  normalizedJd: JdFormData;
  candidateSkills: CandidateSkillEvidence[];
  candidateResponsibilities: string[];
  candidateLevel: string;
  desiredSalary?: number | null;
  minimumAcceptableSalary?: number | null;
  salaryCurrency: Currency;
  candidateRoleTendency?: string;
  candidateWorkingStyles?: string[];
};

export type ApplicationQualityGate = {
  qualityGateStatus: 'clear' | 'requires_confirmation';
  canApply: boolean;
  requiresExplicitConfirmation: boolean;
  confirmationRequiredReasons: string[];
  warnings: string[];
};

export type SkillMatchItem = {
  skill: string;
  matched: boolean;
  matchType: 'exact' | 'semantic' | 'partial' | 'none' | string;
  candidateProficiency: number;
  evidenceStrength: string;
};

export type GapAnalysis = {
  gapSeverity: 'critical' | 'significant' | 'minor' | 'none';
  skillGaps: string[];
  responsibilityGaps: string[];
  seniorityGap?: string | null;
  salaryMismatch?: string | null;
  improvementSuggestions: string[];
  overallGapSummary: string;
};

export type HiringRecommendation = {
  verdict: 'Yes' | 'Conditional' | 'No';
  confidence: number;
  oneParaSummary: string;
  keyReasons: string[];
  hiringRisk: 'low' | 'medium' | 'high';
};

export type JdMatchResponse = {
  matchScore: number;
  matchScorePercent: number;
  cappedMatchScorePercent: number;
  matchLabel: string;
  skillMatchScore: number;
  responsibilityMatchScore: number;
  seniorityMatchScore: number;
  salaryMatchScore: number;
  cultureFitScore: number;
  requiredSkillsMatch: SkillMatchItem[];
  preferredSkillsMatch: SkillMatchItem[];
  missingRequiredSkills: string[];
  uncoveredResponsibilities: string[];
  seniorityFlag: string;
  levelGap: number;
  salaryMatchType: string;
  activeFlags: string[];
  gapAnalysis: GapAnalysis;
  qualityGate: ApplicationQualityGate;
  hiringRecommendation: HiringRecommendation;
};

export type JdCreateState =
  | { step: 'form' }
  | { step: 'validating' }
  | { step: 'generating'; normalizedJd: NormalizedJd }
  | { step: 'preview'; normalizedJd: NormalizedJd; generatedText: string; wordCount: number }
  | { step: 'error'; message: string };
