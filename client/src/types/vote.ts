export interface Vote {
  id: number;
  suggestionId: number;
  userId: number;
  username?: string;
  votedDate: string;
}
