export interface User {
  id: number;
  username: string;
  role: string;
  createdDate: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

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

export interface Suggestion {
  id: number;
  boardId: number;
  text: string;
  submittedByUserId: number;
  submittedByUsername?: string;
  submittedDate: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  isVisible: boolean;
  voteCount: number;
  userHasVoted: boolean;
}

export interface BoardDetail extends Omit<Board, 'suggestionCount' | 'totalVotes'> {
  suggestions: Suggestion[];
}
