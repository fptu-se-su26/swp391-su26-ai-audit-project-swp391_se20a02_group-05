import { axiosClient } from '@/services/axios-client';
import type { JdFormData, NormalizedJd, JdMatchRequest, JdMatchResponse } from '../types/jd.types';

export type JdCreateResponse = {
  jdId: string;
  isValid: boolean;
  validationErrors: string[];
  normalizedJd: NormalizedJd | null;
  generatedJdText: string | null;
  wordCount: number;
  storedAt: string | null;
};

export const jdService = {
  async createJd(formData: JdFormData): Promise<JdCreateResponse> {
    const response = await axiosClient.post<JdCreateResponse>('/api/jd', formData);
    return response.data;
  },

  async matchCandidate(payload: JdMatchRequest): Promise<JdMatchResponse> {
    const response = await axiosClient.post<JdMatchResponse>('/api/jd/match', payload);
    return response.data;
  },
};
