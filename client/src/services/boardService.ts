import api from './api';
import { Board, BoardDetail, CreateBoardRequest, UpdateBoardRequest } from '../types';

export const boardService = {
  async getBoards(): Promise<Board[]> {
    const response = await api.get<Board[]>('/boards');
    return response.data;
  },

  async getBoardDetail(id: number): Promise<BoardDetail> {
    const response = await api.get<BoardDetail>(`/boards/${id}`);
    return response.data;
  },

  async createBoard(data: CreateBoardRequest): Promise<Board> {
    const response = await api.post<Board>('/boards', data);
    return response.data;
  },

  async updateBoard(id: number, data: UpdateBoardRequest): Promise<Board> {
    const response = await api.put<Board>(`/boards/${id}`, data);
    return response.data;
  },

  async deleteBoard(id: number): Promise<void> {
    await api.delete(`/boards/${id}`);
  },

  async toggleBoardStatus(id: number): Promise<Board> {
    const response = await api.put<Board>(`/boards/${id}/toggle-status`);
    return response.data;
  },

  async toggleSuggestionApproval(id: number): Promise<Board> {
    const response = await api.post<Board>(`/boards/${id}/toggle-approval`);
    return response.data;
  },
};
