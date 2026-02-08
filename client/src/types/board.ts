import { Suggestion } from './suggestion';

export interface Board {
  id: number;
  title: string;
  description: string;
  createdDate: string;
  createdByUserId: number;
  createdByUsername?: string;
  isSuggestionsOpen: boolean;
  isVotingOpen: boolean;
  requireApproval: boolean;
  votingType: 'Single' | 'Multiple';
  maxVotes?: number;
  isClosed: boolean;
  suggestionCount: number;
  totalVotes: number;
}

export interface BoardDetail extends Omit<Board, 'suggestionCount' | 'totalVotes'> {
  suggestions: Suggestion[];
}

export interface CreateBoardRequest {
  title: string;
  description: string;
  isSuggestionsOpen?: boolean;
  isVotingOpen?: boolean;
  requireApproval?: boolean;
  votingType: 'Single' | 'Multiple';
  maxVotes?: number;
}

export interface UpdateBoardRequest {
  title: string;
  description: string;
  isSuggestionsOpen: boolean;
  isVotingOpen: boolean;
  requireApproval: boolean;
  votingType: 'Single' | 'Multiple';
  maxVotes?: number;
}
