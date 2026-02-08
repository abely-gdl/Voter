import api from './api';
import { Suggestion, CreateSuggestionRequest } from '../types';

export const suggestionService = {
  async createSuggestion(data: CreateSuggestionRequest): Promise<Suggestion> {
    const response = await api.post<Suggestion>(`/suggestions/boards/${data.boardId}`, { text: data.text });
    return response.data;
  },

  async approveSuggestion(id: number): Promise<Suggestion> {
    const response = await api.put<Suggestion>(`/suggestions/${id}/approve`);
    return response.data;
  },

  async rejectSuggestion(id: number): Promise<void> {
    await api.put(`/suggestions/${id}/reject`);
  },
};
