import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { boardService } from '../services/boardService';
import { suggestionService } from '../services/suggestionService';
import { voteService } from '../services/voteService';
import { BoardDetail } from '../types';

export const BoardDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const [board, setBoard] = useState<BoardDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [suggestionText, setSuggestionText] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const { isAuthenticated, isAdmin, user } = useAuth();

  useEffect(() => {
    if (id) {
      loadBoard();
    }
  }, [id]);

  const loadBoard = async () => {
    try {
      setLoading(true);
      const data = await boardService.getBoardDetail(Number(id));
      setBoard(data);
      setError('');
    } catch (err: any) {
      setError('Failed to load board');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmitSuggestion = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!suggestionText.trim()) return;

    setSubmitting(true);
    try {
      await suggestionService.createSuggestion({
        boardId: Number(id),
        text: suggestionText,
      });
      setSuggestionText('');
      await loadBoard();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to submit suggestion');
    } finally {
      setSubmitting(false);
    }
  };

  const handleVote = async (suggestionId: number) => {
    try {
      await voteService.vote(suggestionId);
      await loadBoard();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to vote');
    }
  };

  const handleUnvote = async (suggestionId: number) => {
    try {
      await voteService.unvote(suggestionId);
      await loadBoard();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to remove vote');
    }
  };

  const handleApproveSuggestion = async (suggestionId: number) => {
    try {
      await suggestionService.approveSuggestion(suggestionId);
      await loadBoard();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to approve suggestion');
    }
  };

  const handleRejectSuggestion = async (suggestionId: number) => {
    try {
      await suggestionService.rejectSuggestion(suggestionId);
      await loadBoard();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to reject suggestion');
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-gray-600">Loading board...</div>
      </div>
    );
  }

  if (!board) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center">
          <p className="text-red-600">Board not found</p>
          <Link to="/boards" className="text-indigo-600 hover:text-indigo-500 mt-4 inline-block">
            Back to boards
          </Link>
        </div>
      </div>
    );
  }

  // Show visible suggestions + user's own pending suggestions
  const visibleSuggestions = board.suggestions.filter(
    (s) => s.isVisible || (user && s.submittedByUserId === user.id && s.status === 'Pending')
  );
  // For admins: show all pending suggestions in the approval section
  const pendingSuggestions = board.suggestions.filter(
    (s) => s.status === 'Pending' && !s.isVisible
  );

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-6">
        <Link to="/boards" className="text-indigo-600 hover:text-indigo-500">
          ← Back to boards
        </Link>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      <div className="bg-white rounded-lg shadow-md p-6 mb-8">
        <div className="flex justify-between items-start mb-4">
          <h1 className="text-3xl font-bold text-gray-900">{board.title}</h1>
          {isAdmin && (
            <Link
              to={`/admin/boards/${board.id}/edit`}
              className="px-4 py-2 text-sm bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
            >
              Edit Board
            </Link>
          )}
        </div>
        <p className="text-gray-700 mb-4">{board.description}</p>
        <div className="flex flex-wrap gap-2 mb-4">
          {board.isClosed && (
            <span className="px-3 py-1 text-sm bg-gray-200 text-gray-700 rounded">Closed</span>
          )}
          {!board.isSuggestionsOpen && (
            <span className="px-3 py-1 text-sm bg-yellow-100 text-yellow-800 rounded">
              Suggestions Closed
            </span>
          )}
          {!board.isVotingOpen && (
            <span className="px-3 py-1 text-sm bg-orange-100 text-orange-800 rounded">
              Voting Closed
            </span>
          )}
          {board.requireApproval && (
            <span className="px-3 py-1 text-sm bg-blue-100 text-blue-800 rounded">
              Requires Approval
            </span>
          )}
          <span className="px-3 py-1 text-sm bg-green-100 text-green-800 rounded">
            {board.votingType} Vote
            {board.maxVotes && ` (max ${board.maxVotes})`}
          </span>
        </div>
        <div className="text-sm text-gray-500">
          Created by {board.createdByUsername || 'Unknown'} on {formatDate(board.createdDate)}
        </div>
      </div>

      {/* Submit suggestion form */}
      {isAuthenticated && board.isSuggestionsOpen && !board.isClosed && (
        <div className="bg-white rounded-lg shadow-md p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Submit a Suggestion</h2>
          <form onSubmit={handleSubmitSuggestion}>
            <textarea
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
              rows={3}
              placeholder="Enter your suggestion..."
              value={suggestionText}
              onChange={(e) => setSuggestionText(e.target.value)}
              required
            />
            <button
              type="submit"
              disabled={submitting}
              className="mt-3 px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {submitting ? 'Submitting...' : 'Submit Suggestion'}
            </button>
          </form>
        </div>
      )}

      {/* Pending suggestions (admin only) */}
      {isAdmin && pendingSuggestions.length > 0 && (
        <div className="bg-yellow-50 rounded-lg shadow-md p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">
            Pending Approval ({pendingSuggestions.length})
          </h2>
          <div className="space-y-4">
            {pendingSuggestions.map((suggestion) => (
              <div key={suggestion.id} className="bg-white p-4 rounded border border-yellow-200">
                <p className="text-gray-900 mb-2">{suggestion.text}</p>
                <div className="text-sm text-gray-500 mb-3">
                  Submitted by {suggestion.submittedByUsername || 'Unknown'} on{' '}
                  {formatDate(suggestion.submittedDate)}
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={() => handleApproveSuggestion(suggestion.id)}
                    className="px-3 py-1 bg-green-600 text-white text-sm rounded hover:bg-green-700"
                  >
                    Approve
                  </button>
                  <button
                    onClick={() => handleRejectSuggestion(suggestion.id)}
                    className="px-3 py-1 bg-red-600 text-white text-sm rounded hover:bg-red-700"
                  >
                    Reject
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Approved suggestions */}
      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-xl font-semibold text-gray-900 mb-4">
          Suggestions ({visibleSuggestions.length})
        </h2>
        {visibleSuggestions.length === 0 ? (
          <p className="text-gray-600">No suggestions yet. Be the first to submit one!</p>
        ) : (
          <div className="space-y-4">
            {visibleSuggestions
              .sort((a, b) => b.voteCount - a.voteCount)
              .map((suggestion) => (
                <div
                  key={suggestion.id}
                  className={`border rounded-lg p-4 transition-colors ${
                    suggestion.status === 'Pending'
                      ? 'border-yellow-300 bg-yellow-50'
                      : 'border-gray-200 hover:border-gray-300'
                  }`}
                >
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-2">
                        <p className="text-gray-900">{suggestion.text}</p>
                        {suggestion.status === 'Pending' && (
                          <span className="px-2 py-1 text-xs bg-yellow-200 text-yellow-800 rounded">
                            Pending Approval
                          </span>
                        )}
                      </div>
                      <div className="text-sm text-gray-500">
                        Submitted by {suggestion.submittedByUsername || 'Unknown'} on{' '}
                        {formatDate(suggestion.submittedDate)}
                      </div>
                    </div>
                    <div className="ml-4 flex flex-col items-center">
                      <div className="text-2xl font-bold text-indigo-600">{suggestion.voteCount}</div>
                      <div className="text-xs text-gray-500">votes</div>
                      {isAuthenticated && board.isVotingOpen && !board.isClosed && suggestion.status === 'Approved' && (
                        <button
                          onClick={() =>
                            suggestion.userHasVoted
                              ? handleUnvote(suggestion.id)
                              : handleVote(suggestion.id)
                          }
                          className={`mt-2 px-4 py-1 text-sm rounded ${
                            suggestion.userHasVoted
                              ? 'bg-red-100 text-red-700 hover:bg-red-200'
                              : 'bg-indigo-600 text-white hover:bg-indigo-700'
                          }`}
                        >
                          {suggestion.userHasVoted ? 'Unvote' : 'Vote'}
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              ))}
          </div>
        )}
      </div>
    </div>
  );
};
