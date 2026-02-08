import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { MainLayout } from './components/MainLayout';
import { ProtectedRoute } from './components/ProtectedRoute';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { BoardsListPage } from './pages/BoardsListPage';
import { BoardDetailPage } from './pages/BoardDetailPage';
import { CreateBoardPage } from './pages/CreateBoardPage';
import { EditBoardPage } from './pages/EditBoardPage';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <MainLayout>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/boards" element={<BoardsListPage />} />
            <Route path="/boards/:id" element={<BoardDetailPage />} />
            <Route
              path="/admin/boards/create"
              element={
                <ProtectedRoute adminOnly>
                  <CreateBoardPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/boards/:id/edit"
              element={
                <ProtectedRoute adminOnly>
                  <EditBoardPage />
                </ProtectedRoute>
              }
            />
          </Routes>
        </MainLayout>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
