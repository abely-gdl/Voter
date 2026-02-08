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

export interface CreateSuggestionRequest {
  boardId: number;
  text: string;
}
