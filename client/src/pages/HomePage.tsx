import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export const HomePage = () => {
  const { isAuthenticated } = useAuth();

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-100 to-purple-100">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="text-center">
          <h1 className="text-5xl font-extrabold text-gray-900 sm:text-6xl md:text-7xl">
            <span className="block">Voting Boards</span>
            <span className="block text-indigo-600 mt-2">Collect ideas, vote together</span>
          </h1>
          <p className="mt-6 max-w-2xl mx-auto text-xl text-gray-600">
            Create voting boards, submit suggestions, and let your community decide what matters most.
          </p>
          <div className="mt-10 flex justify-center gap-4">
            <Link
              to="/boards"
              className="px-8 py-3 border border-transparent text-base font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 md:py-4 md:text-lg md:px-10"
            >
              Browse Boards
            </Link>
            {!isAuthenticated && (
              <>
                <Link
                  to="/login"
                  className="px-8 py-3 border border-transparent text-base font-medium rounded-md text-indigo-600 bg-white hover:bg-gray-50 md:py-4 md:text-lg md:px-10"
                >
                  Sign In
                </Link>
              </>
            )}
          </div>
        </div>

        <div className="mt-20">
          <h2 className="text-3xl font-bold text-gray-900 text-center mb-12">How it works</h2>
          <div className="grid grid-cols-1 gap-8 md:grid-cols-3">
            <div className="bg-white rounded-lg shadow-md p-6">
              <div className="text-indigo-600 text-4xl mb-4">📋</div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">Create Boards</h3>
              <p className="text-gray-600">
                Set up voting boards for any topic. Configure voting rules, approval workflows, and more.
              </p>
            </div>
            <div className="bg-white rounded-lg shadow-md p-6">
              <div className="text-indigo-600 text-4xl mb-4">💡</div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">Submit Ideas</h3>
              <p className="text-gray-600">
                Anyone can submit suggestions to open boards. Ideas can be approved by admins before voting.
              </p>
            </div>
            <div className="bg-white rounded-lg shadow-md p-6">
              <div className="text-indigo-600 text-4xl mb-4">🗳️</div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">Vote & Decide</h3>
              <p className="text-gray-600">
                Cast your votes on suggestions. Support single-vote or multi-vote systems with customizable limits.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
