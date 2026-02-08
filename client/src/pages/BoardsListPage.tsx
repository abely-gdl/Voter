import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { boardService } from '../services/boardService';
import { Board } from '../types';

export const BoardsListPage = () => {
  const [boards, setBoards] = useState<Board[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const { isAdmin } = useAuth();

  useEffect(() => {
    loadBoards();
  }, []);

  const loadBoards = async () => {
    try {
      setLoading(true);
      const data = await boardService.getBoards();
      setBoards(data);
      setError('');
    } catch (err: unknown) {
      setError('Failed to load boards');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-gray-600">Loading boards...</div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Voting Boards</h1>
        {isAdmin && (
          <Link
            to="/admin/boards/create"
            className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700"
          >
            + Create Board
          </Link>
        )}
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {boards.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-600 text-lg">No boards available yet.</p>
          {isAdmin && (
            <Link
              to="/admin/boards/create"
              className="mt-4 inline-block text-indigo-600 hover:text-indigo-500"
            >
              Create the first board
            </Link>
          )}
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {boards.map((board) => (
            <Link
              key={board.id}
              to={`/boards/${board.id}`}
              className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow p-6"
            >
              <div className="flex justify-between items-start mb-3">
                <h2 className="text-xl font-semibold text-gray-900">{board.title}</h2>
                {board.isClosed && (
                  <span className="px-2 py-1 text-xs font-semibold text-gray-500 bg-gray-200 rounded">
                    Closed
                  </span>
                )}
              </div>
              <p className="text-gray-600 mb-4 line-clamp-2">{board.description}</p>
              <div className="flex justify-between items-center text-sm text-gray-500 mb-3">
                <span>Created {formatDate(board.createdDate)}</span>
              </div>
              <div className="flex justify-between items-center text-sm">
                <span className="text-gray-700">
                  {board.suggestionCount} suggestion{board.suggestionCount !== 1 ? 's' : ''}
                </span>
                <span className="text-gray-700">
                  {board.totalVotes} vote{board.totalVotes !== 1 ? 's' : ''}
                </span>
              </div>
              <div className="mt-4 flex flex-wrap gap-2">
                {!board.isSuggestionsOpen && (
                  <span className="px-2 py-1 text-xs bg-yellow-100 text-yellow-800 rounded">
                    Suggestions Closed
                  </span>
                )}
                {!board.isVotingOpen && (
                  <span className="px-2 py-1 text-xs bg-orange-100 text-orange-800 rounded">
                    Voting Closed
                  </span>
                )}
                {board.requireApproval && (
                  <span className="px-2 py-1 text-xs bg-blue-100 text-blue-800 rounded">
                    Requires Approval
                  </span>
                )}
                <span className="px-2 py-1 text-xs bg-green-100 text-green-800 rounded">
                  {board.votingType} Vote
                </span>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
};
