import { useEffect, useState, useCallback } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { boardService } from '../services/boardService';
import { BoardDetail } from '../types';

export const EditBoardPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [board, setBoard] = useState<BoardDetail | null>(null);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    isSuggestionsOpen: true,
    isVotingOpen: true,
    requireApproval: false,
    votingType: 'Single' as 'Single' | 'Multiple',
    maxVotes: '',
  });
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const loadBoard = useCallback(async () => {
    try {
      setLoading(true);
      const data = await boardService.getBoardDetail(Number(id));
      setBoard(data);
      setFormData({
        title: data.title,
        description: data.description,
        isSuggestionsOpen: data.isSuggestionsOpen,
        isVotingOpen: data.isVotingOpen,
        requireApproval: data.requireApproval,
        votingType: data.votingType,
        maxVotes: data.maxVotes?.toString() || '',
      });
    } catch {
      setError('Failed to load board');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    if (id) {
      loadBoard();
    }
  }, [id, loadBoard]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);

    try {
      const updateData = {
        title: formData.title,
        description: formData.description,
        isSuggestionsOpen: formData.isSuggestionsOpen,
        isVotingOpen: formData.isVotingOpen,
        requireApproval: formData.requireApproval,
        votingType: formData.votingType,
        maxVotes: formData.votingType === 'Multiple' && formData.maxVotes
          ? Number(formData.maxVotes)
          : undefined,
      };

      await boardService.updateBoard(Number(id), updateData);
      navigate(`/boards/${id}`);
    } catch (err: unknown) {
      const errorMessage = 
        typeof err === 'object' && err !== null && 'response' in err && 
        typeof (err as { response?: { data?: { message?: string } } }).response === 'object' && 
        (err as { response?: { data?: { message?: string } } }).response?.data?.message
          ? (err as { response: { data: { message: string } } }).response.data.message
          : 'Failed to update board';
      setError(errorMessage);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this board? This action cannot be undone.')) {
      return;
    }

    try {
      await boardService.deleteBoard(Number(id));
      navigate('/boards');
    } catch (err: unknown) {
      const errorMessage = 
        typeof err === 'object' && err !== null && 'response' in err && 
        typeof (err as { response?: { data?: { message?: string } } }).response === 'object' && 
        (err as { response?: { data?: { message?: string } } }).response?.data?.message
          ? (err as { response: { data: { message: string } } }).response.data.message
          : 'Failed to delete board';
      alert(errorMessage);
    }
  };

  const handleToggleStatus = async () => {
    try {
      await boardService.toggleBoardStatus(Number(id));
      await loadBoard();
    } catch (err: unknown) {
      const errorMessage = 
        typeof err === 'object' && err !== null && 'response' in err && 
        typeof (err as { response?: { data?: { message?: string } } }).response === 'object' && 
        (err as { response?: { data?: { message?: string } } }).response?.data?.message
          ? (err as { response: { data: { message: string } } }).response.data.message
          : 'Failed to toggle board status';
      alert(errorMessage);
    }
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

  return (
    <div className="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-6">
        <Link to={`/boards/${id}`} className="text-indigo-600 hover:text-indigo-500">
          ← Back to board
        </Link>
      </div>

      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Edit Board</h1>
          <div className="flex gap-2">
            <button
              onClick={handleToggleStatus}
              className={`px-4 py-2 text-sm rounded ${
                board.isClosed
                  ? 'bg-green-600 text-white hover:bg-green-700'
                  : 'bg-yellow-600 text-white hover:bg-yellow-700'
              }`}
            >
              {board.isClosed ? 'Reopen Board' : 'Close Board'}
            </button>
            <button
              onClick={handleDelete}
              className="px-4 py-2 text-sm bg-red-600 text-white rounded hover:bg-red-700"
            >
              Delete Board
            </button>
          </div>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-2">
              Board Title *
            </label>
            <input
              type="text"
              id="title"
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
            />
          </div>

          <div>
            <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-2">
              Description *
            </label>
            <textarea
              id="description"
              required
              rows={4}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            />
          </div>

          <div>
            <label htmlFor="votingType" className="block text-sm font-medium text-gray-700 mb-2">
              Voting Type *
            </label>
            <select
              id="votingType"
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
              value={formData.votingType}
              onChange={(e) =>
                setFormData({ ...formData, votingType: e.target.value as 'Single' | 'Multiple' })
              }
            >
              <option value="Single">Single Vote (users can vote on one suggestion only)</option>
              <option value="Multiple">Multiple Votes (users can vote on multiple suggestions)</option>
            </select>
          </div>

          {formData.votingType === 'Multiple' && (
            <div>
              <label htmlFor="maxVotes" className="block text-sm font-medium text-gray-700 mb-2">
                Max Votes Per User (optional)
              </label>
              <input
                type="number"
                id="maxVotes"
                min="1"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
                value={formData.maxVotes}
                onChange={(e) => setFormData({ ...formData, maxVotes: e.target.value })}
                placeholder="Leave empty for unlimited votes"
              />
            </div>
          )}

          <div className="space-y-3">
            <div className="flex items-start">
              <input
                type="checkbox"
                id="isSuggestionsOpen"
                className="mt-1 h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
                checked={formData.isSuggestionsOpen}
                onChange={(e) => setFormData({ ...formData, isSuggestionsOpen: e.target.checked })}
              />
              <label htmlFor="isSuggestionsOpen" className="ml-2 block text-sm text-gray-700">
                Allow users to submit suggestions
              </label>
            </div>

            <div className="flex items-start">
              <input
                type="checkbox"
                id="isVotingOpen"
                className="mt-1 h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
                checked={formData.isVotingOpen}
                onChange={(e) => setFormData({ ...formData, isVotingOpen: e.target.checked })}
              />
              <label htmlFor="isVotingOpen" className="ml-2 block text-sm text-gray-700">
                Allow users to vote on suggestions
              </label>
            </div>

            <div className="flex items-start">
              <input
                type="checkbox"
                id="requireApproval"
                className="mt-1 h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
                checked={formData.requireApproval}
                onChange={(e) => setFormData({ ...formData, requireApproval: e.target.checked })}
              />
              <label htmlFor="requireApproval" className="ml-2 block text-sm text-gray-700">
                Require admin approval for suggestions before they become visible
              </label>
            </div>
          </div>

          <div className="flex gap-3 pt-4">
            <button
              type="submit"
              disabled={submitting}
              className="px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {submitting ? 'Saving...' : 'Save Changes'}
            </button>
            <Link
              to={`/boards/${id}`}
              className="px-6 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300"
            >
              Cancel
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
};
