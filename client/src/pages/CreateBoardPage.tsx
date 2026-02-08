import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { boardService } from '../services/boardService';
import { getErrorMessage } from '../utils/errorUtils';

export const CreateBoardPage = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    requireApproval: false,
    votingType: 'Single' as 'Single' | 'Multiple',
    maxVotes: '',
  });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);

    try {
      const boardData = {
        title: formData.title,
        description: formData.description,
        requireApproval: formData.requireApproval,
        votingType: formData.votingType,
        maxVotes: formData.votingType === 'Multiple' && formData.maxVotes
          ? Number(formData.maxVotes)
          : undefined,
      };

      const newBoard = await boardService.createBoard(boardData);
      navigate(`/boards/${newBoard.id}`);
    } catch (err: unknown) {
      setError(getErrorMessage(err, 'Failed to create board'));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-6">
        <Link to="/boards" className="text-indigo-600 hover:text-indigo-500">
          ← Back to boards
        </Link>
      </div>

      <div className="bg-white rounded-lg shadow-md p-6">
        <h1 className="text-2xl font-bold text-gray-900 mb-6">Create New Board</h1>

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

          <div className="flex gap-3 pt-4">
            <button
              type="submit"
              disabled={submitting}
              className="px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {submitting ? 'Creating...' : 'Create Board'}
            </button>
            <Link
              to="/boards"
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
