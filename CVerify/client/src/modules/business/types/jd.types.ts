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

export type JdCreateState =
  | { step: 'form' }
  | { step: 'validating' }
  | { step: 'generating'; normalizedJd: NormalizedJd }
  | { step: 'preview'; normalizedJd: NormalizedJd; generatedText: string; wordCount: number }
  | { step: 'error'; message: string };
