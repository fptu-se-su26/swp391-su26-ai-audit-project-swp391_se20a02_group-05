import { axiosClient } from '@/services/axios-client';
import type { JdDetail, JdFormData, JdSummary, NormalizedJd, JdMatchRequest, JdMatchResponse } from '../types/jd.types';

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
    const response = await axiosClient.post<JdCreateResponse>('/jds', formData, {
      timeout: 300_000, // 5 min — AI pipeline (validate → generate → store) is slow
    });
    return response.data;
  },

  async listJds(): Promise<JdSummary[]> {
    const response = await axiosClient.get<JdSummary[]>('/jds');
    return response.data;
  },

  async getJd(jdId: string): Promise<JdDetail> {
    const response = await axiosClient.get<JdDetail>(`/jds/${jdId}`);
    return response.data;
  },

  async updateJd(jdId: string, formData: JdFormData, generatedJdText?: string): Promise<JdDetail> {
    const response = await axiosClient.put<JdDetail>(`/jds/${jdId}`, {
      normalizedJd: formData,
      generatedJdText,
    });
    return response.data;
  },

  async deleteJd(jdId: string): Promise<void> {
    await axiosClient.delete(`/jds/${jdId}`);
  },

  async matchCandidate(payload: JdMatchRequest): Promise<JdMatchResponse> {
    const response = await axiosClient.post<JdMatchResponse>('/jds/match', payload);
    return response.data;
  },
};
