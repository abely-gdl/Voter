import api from './api';
import { Vote } from '../types';

export const voteService = {
  async vote(suggestionId: number): Promise<Vote> {
    const response = await api.post<Vote>('/votes', { suggestionId });
    return response.data;
  },

  async unvote(suggestionId: number): Promise<void> {
    await api.delete(`/votes/${suggestionId}`);
  },

  async getVotesForBoard(boardId: number): Promise<Vote[]> {
    const response = await api.get<Vote[]>(`/votes/board/${boardId}`);
    return response.data;
  },
};
