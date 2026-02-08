# Plan: Voting Boards Application with .NET 9 API & React

A full-stack voting application where users can browse boards, submit suggestions, and vote on them. Admins can create/manage boards and moderate suggestions. Built with .NET 9 Web API backend, React 18+ with Vite and TypeScript frontend, EF Core with SQLite (repository pattern for DB interchangeability), Tailwind CSS styling, and simple username/password authentication.

**Key Architectural Decisions:**
- Repository pattern for data access ensures future DB provider swapping
- Simple authentication with JWT tokens and two roles (Admin/User)
- Default admin user seeded in database (username: "admin", password: "admin123")
- Multiple layouts: MainLayout (public), AdminLayout with admin navigation
- Multi-page routing with React Router v6+
- RESTful API design with DTOs for all responses

---

**Steps**

### Phase 1: Backend Infrastructure Setup

1. Create .NET 9 Web API project in [server/API](server/API) using `dotnet new webapi` with .NET 9 SDK
2. Create xUnit test project in [server/Tests](server/Tests) using `dotnet new xunit`
3. Add solution file at repository root to manage both projects
4. Install NuGet packages in API project: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`, `AutoMapper.Extensions.Microsoft.DependencyInjection`, `FluentValidation.AspNetCore`, `Swashbuckle.AspNetCore`, `BCrypt.Net-Next` (password hashing), `Microsoft.AspNetCore.Authentication.JwtBearer` (JWT auth)
5. Install test packages: `Moq`, `FluentAssertions`, `Microsoft.EntityFrameworkCore.InMemory` (for testing)
6. Configure CORS in [Program.cs](server/API/Program.cs) to allow React dev server (`http://localhost:5173` for Vite)
7. Configure Swagger/OpenAPI in [Program.cs](server/API/Program.cs) for API documentation with JWT bearer token support
8. Add `appsettings.json` and `appsettings.Development.json` with SQLite connection string and JWT settings (secret key, issuer, audience, expiration)

### Phase 2: Backend Domain Models & Database

9. Create entity models in [server/API/Models](server/API/Models):
   - `User` entity: Id, Username (unique), PasswordHash, Role (enum: Admin/User), CreatedDate
   - `Board` entity: Id, Title, Description, CreatedDate, CreatedByUserId (FK), IsSuggestionsOpen, IsVotingOpen, RequireApproval, VotingType (enum: Single/Multi), MaxVotes (nullable int), IsClosed
   - `Suggestion` entity: Id, BoardId (FK), Text, SubmittedByUserId (FK), SubmittedDate, Status (enum: Pending/Approved/Rejected), IsVisible
   - `Vote` entity: Id, SuggestionId (FK), UserId (FK), VotedDate

10. Create enums in [server/API/Models/Enums.cs](server/API/Models/Enums.cs): `UserRole`, `VotingType`, `SuggestionStatus`
11. Create `VoterDbContext` in [server/API/Data/VoterDbContext.cs](server/API/Data/VoterDbContext.cs) with DbSets for entities, configure relationships and constraints (unique username index, cascade deletes)
12. Create repository interfaces in [server/API/Data/Repositories](server/API/Data/Repositories): `IRepository<T>`, `IUserRepository`, `IBoardRepository`, `ISuggestionRepository`, `IVoteRepository`
13. Implement generic repository and specific repositories in same folder with EF Core implementations
14. Create and apply initial EF Core migration: `dotnet ef migrations add Initial` in API project
15. Configure database seeding in `VoterDbContext.OnModelCreating` with default admin user (username: "admin", password hashed: "admin123") and 2-3 sample boards for development

### Phase 3: Backend Authentication & JWT

16. Create JWT helper/service in [server/API/Services/JwtService.cs](server/API/Services/JwtService.cs) with methods: `GenerateToken(User user)`, `ValidateToken(string token)`
17. Create password hashing utility in [server/API/Utils/PasswordHasher.cs](server/API/Utils/PasswordHasher.cs) using BCrypt: `HashPassword(string password)`, `VerifyPassword(string password, string hash)`
18. Configure JWT authentication in [Program.cs](server/API/Program.cs) with bearer token validation
19. Add authorization policies in [Program.cs](server/API/Program.cs): "AdminOnly" policy requiring Admin role
20. Create authentication DTOs: `LoginRequestDto`, `LoginResponseDto`, `RegisterRequestDto`, `UserDto`

### Phase 4: Backend DTOs & Mapping

21. Create DTOs in [server/API/DTOs](server/API/DTOs):
    - `BoardDto`, `BoardCreateDto`, `BoardUpdateDto`
    - `SuggestionDto`, `SuggestionCreateDto`, `SuggestionUpdateDto`
    - `VoteDto`, `VoteCreateDto`
    - `BoardDetailDto` (includes suggestions collection and vote counts)
    - `SuggestionWithVotesDto` (includes vote count and user's vote status)

22. Create AutoMapper profile in [server/API/Mapping/MappingProfile.cs](server/API/Mapping/MappingProfile.cs) to map entities to DTOs
23. Register AutoMapper in [Program.cs](server/API/Program.cs) DI container

### Phase 5: Backend Business Logic & Validation

24. Create service interfaces in [server/API/Services](server/API/Services): `IAuthService`, `IBoardService`, `ISuggestionService`, `IVoteService`
25. Implement `AuthService` with methods: `Login(username, password)`, `Register(username, password)`, `GetCurrentUser(userId)`
26. Implement `BoardService` with methods: GetAllBoards, GetBoardById, GetBoardWithDetails, CreateBoard, UpdateBoard, ToggleVoting, ToggleSuggestions, CloseBoard
27. Implement `SuggestionService` with methods: GetSuggestionsByBoardId, CreateSuggestion, ApproveSuggestion, RejectSuggestion, GetPendingSuggestions (admin)
28. Implement `VoteService` with methods: AddVote, GetVotesBySuggestionId, GetUserVotesByBoardId, RemoveVote, ValidateVotingRules (check multi/single, max votes)
29. Create FluentValidation validators in [server/API/Validators](server/API/Validators): `LoginRequestDtoValidator`, `RegisterRequestDtoValidator`, `BoardCreateDtoValidator`, `SuggestionCreateDtoValidator`, `VoteCreateDtoValidator`
30. Register services and repositories in [Program.cs](server/API/Program.cs) DI container with scoped lifetime

### Phase 6: Backend API Controllers

31. Create `AuthController` in [server/API/Controllers](server/API/Controllers) with endpoints:
    - `POST /api/auth/login` - authenticate user and return JWT token
    - `POST /api/auth/register` - register new user (regular User role)
    - `GET /api/auth/me` - get current user info (requires authentication)

32. Create `BoardsController` with endpoints:
    - `GET /api/boards` - list all boards (public)
    - `GET /api/boards/{id}` - get board details with suggestions and votes (public)
    - `POST /api/boards` - create board (requires Admin role)
    - `PUT /api/boards/{id}` - update board (requires Admin role)
    - `PUT /api/boards/{id}/toggle-voting` - toggle voting status (requires Admin role)
    - `PUT /api/boards/{id}/toggle-suggestions` - toggle suggestions status (requires Admin role)
    - `PUT /api/boards/{id}/close` - close board (requires Admin role)

33. Create `SuggestionsController` with endpoints:
    - `GET /api/boards/{boardId}/suggestions` - get approved suggestions for a board (public)
    - `GET /api/suggestions/pending` - get pending suggestions (requires Admin role)
    - `POST /api/boards/{boardId}/suggestions` - create suggestion (requires authentication)
    - `PUT /api/suggestions/{id}/approve` - approve suggestion (requires Admin role)
    - `PUT /api/suggestions/{id}/reject` - reject suggestion (requires Admin role)

34. Create `VotesController` with endpoints:
    - `POST /api/suggestions/{suggestionId}/votes` - add vote (requires authentication, uses userId from JWT claims)
    - `DELETE /api/suggestions/{suggestionId}/votes` - remove vote (requires authentication)
    - `GET /api/boards/{boardId}/votes/me` - get current user's votes for a board (requires authentication)

35. Add `[Authorize]` and `[Authorize(Policy = "AdminOnly")]` attributes to controllers/actions as appropriate
36. Add error handling middleware in [server/API/Middleware/ErrorHandlingMiddleware.cs](server/API/Middleware/ErrorHandlingMiddleware.cs) for consistent error responses
37. Register middleware in [Program.cs](server/API/Program.cs) pipeline

### Phase 7: Backend Testing

38. Create unit tests for `AuthService` in [server/Tests/Services/AuthServiceTests.cs](server/Tests/Services/AuthServiceTests.cs) testing login, registration, password hashing
39. Create unit tests for `BoardService`, `SuggestionService`, `VoteService` using Moq for repository mocks
40. Create integration tests for controllers in [server/Tests/Controllers](server/Tests/Controllers) using `WebApplicationFactory` and InMemory database
41. Test JWT token generation and validation
42. Test authorization policies (admin-only endpoints)
43. Test voting rules validation (single vs multi, max votes enforcement)
44. Test suggestion approval workflow

### Phase 8: Frontend Infrastructure Setup

45. Initialize React + TypeScript + Vite project in [client](client) folder using `npm create vite@latest . -- --template react-ts`
46. Install dependencies: `react-router-dom`, `axios`, `clsx`, `jwt-decode` (for decoding JWT tokens)
47. Install Tailwind CSS: `tailwindcss`, `postcss`, `autoprefixer`, configure [tailwind.config.js](client/tailwind.config.js) and [postcss.config.js](client/postcss.config.js)
48. Create Tailwind CSS imports in [client/src/index.css](client/src/index.css)
49. Update [vite.config.ts](client/vite.config.ts) to proxy API requests to `http://localhost:5000` (or backend port)
50. Configure `tsconfig.json` with path aliases (e.g., `@/` for `src/`)
51. Create folder structure: [src/components](client/src/components), [src/pages](client/src/pages), [src/layouts](client/src/layouts), [src/services](client/src/services), [src/hooks](client/src/hooks), [src/types](client/src/types), [src/utils](client/src/utils)

### Phase 9: Frontend Type Definitions & API Client

52. Create TypeScript types in [client/src/types/index.ts](client/src/types/index.ts) matching backend DTOs: `User`, `UserRole`, `LoginRequest`, `LoginResponse`, `RegisterRequest`, `Board`, `BoardDetail`, `Suggestion`, `SuggestionWithVotes`, `Vote`, `VotingType`, `SuggestionStatus`
53. Create API service in [client/src/services/api.ts](client/src/services/api.ts) with axios instance configured with base URL and JWT token interceptor (add Authorization header from localStorage)
54. Create `authService` in [client/src/services/authService.ts](client/src/services/authService.ts) with functions: `login(username, password)`, `register(username, password)`, `logout()`, `getCurrentUser()`, `getStoredToken()`, `setStoredToken(token)`
55. Create `boardService` in [client/src/services/boardService.ts](client/src/services/boardService.ts) with functions: `getBoards()`, `getBoardDetails(id)`, `createBoard(data)`, `updateBoard(id, data)`, `toggleVoting(id)`, `toggleSuggestions(id)`, `closeBoard(id)`
56. Create `suggestionService` in [client/src/services/suggestionService.ts](client/src/services/suggestionService.ts) with functions: `getSuggestions(boardId)`, `createSuggestion(boardId, data)`, `getPendingSuggestions()`, `approveSuggestion(id)`, `rejectSuggestion(id)`
57. Create `voteService` in [client/src/services/voteService.ts](client/src/services/voteService.ts) with functions: `addVote(suggestionId)`, `removeVote(suggestionId)`, `getMyVotes(boardId)`

### Phase 10: Frontend Auth Context & Hooks

58. Create auth context in [client/src/contexts/AuthContext.tsx](client/src/contexts/AuthContext.tsx) with user state, `login()`, `register()`, `logout()`, `isAuthenticated`, `isAdmin` computed properties
59. Load user from JWT token on app initialization (decode token to get user info, verify not expired)
60. Create `useAuth` hook in [client/src/hooks/useAuth.ts](client/src/hooks/useAuth.ts) to consume AuthContext
61. Create custom hooks in [client/src/hooks](client/src/hooks): `useBoards`, `useBoardDetails`, `useSuggestions`, `useVotes` for data fetching with loading/error states
62. Wrap app with `AuthProvider` in [client/src/main.tsx](client/src/main.tsx)
63. Create axios interceptor to handle 401 responses (clear token, redirect to login)

### Phase 11: Frontend Layouts & Navigation

64. Create `MainLayout` component in [client/src/layouts/MainLayout.tsx](client/src/layouts/MainLayout.tsx) with header (logo, navigation links, user info, login/logout button), main content area, and footer
65. Create `AdminLayout` component in [client/src/layouts/AdminLayout.tsx](client/src/layouts/AdminLayout.tsx) extending MainLayout with admin-specific navigation (Create Board, Pending Suggestions, Manage Boards)
66. Create `Navigation` component in [client/src/components/Navigation.tsx](client/src/components/Navigation.tsx) with conditional links based on authentication status and user role
67. Display username in header when logged in
68. Style layouts with Tailwind CSS using responsive design patterns

### Phase 12: Frontend Auth Pages

69. Create `LoginPage` in [client/src/pages/LoginPage.tsx](client/src/pages/LoginPage.tsx) with username/password form, form validation, error display, link to register
70. Create `RegisterPage` in [client/src/pages/RegisterPage.tsx](client/src/pages/RegisterPage.tsx) with username/password form, password confirmation, form validation, link to login
71. Add loading states during login/register API calls
72. Redirect to home/boards page after successful login
73. Show friendly error messages for invalid credentials or registration errors

### Phase 13: Frontend Core Components

74. Create `BoardCard` component in [client/src/components/BoardCard.tsx](client/src/components/BoardCard.tsx) to display board summary with title, description, vote count, status badges
75. Create `SuggestionCard` component in [client/src/components/SuggestionCard.tsx](client/src/components/SuggestionCard.tsx) to display suggestion with text, vote count, vote button, status indicator
76. Create `VoteButton` component in [client/src/components/VoteButton.tsx](client/src/components/VoteButton.tsx) with vote/unvote logic, disabled states for unauthenticated users, vote count display
77. Create `BoardForm` component in [client/src/components/BoardForm.tsx](client/src/components/BoardForm.tsx) for creating/editing boards with form validation
78. Create `SuggestionForm` component in [client/src/components/SuggestionForm.tsx](client/src/components/SuggestionForm.tsx) for submitting suggestions (requires authentication)
79. Create reusable UI components: `Button`, `Card`, `Badge`, `Input`, `Textarea`, `Select`, `Toggle` in [client/src/components/ui](client/src/components/ui)
80. Create `LoadingSpinner` and `ErrorMessage` components for async states

### Phase 14: Frontend Pages - User Views

81. Create `BoardsListPage` in [client/src/pages/BoardsListPage.tsx](client/src/pages/BoardsListPage.tsx) - displays grid of BoardCards, filters (open/closed/all), uses `useBoards` hook
82. Create `BoardDetailPage` in [client/src/pages/BoardDetailPage.tsx](client/src/pages/BoardDetailPage.tsx) - shows board info, suggestion list with voting, suggestion form (if authenticated and open), real-time results, uses `useBoardDetails` hook
83. Create `HomePage` in [client/src/pages/HomePage.tsx](client/src/pages/HomePage.tsx) - landing page with app description, link to browse boards, login prompt if not authenticated
84. Implement voting logic in BoardDetailPage: check user's existing votes, enforce voting rules (single/multi, max votes), optimistic UI updates, require authentication
85. Show/hide suggestion form based on `isSuggestionsOpen` board property and authentication status
86. Display voting results with vote counts and percentage bars
87. Show "Login to vote" message for unauthenticated users

### Phase 15: Frontend Pages - Admin Views

88. Create `CreateBoardPage` in [client/src/pages/admin/CreateBoardPage.tsx](client/src/pages/admin/CreateBoardPage.tsx) - form to create new board with all settings (requires Admin role)
89. Create `EditBoardPage` in [client/src/pages/admin/EditBoardPage.tsx](client/src/pages/admin/EditBoardPage.tsx) - form to update board settings, quick toggles for voting/suggestions/close status (requires Admin role)
90. Create `ManageBoardsPage` in [client/src/pages/admin/ManageBoardsPage.tsx](client/src/pages/admin/ManageBoardsPage.tsx) - table/list of all boards with edit/delete actions, status indicators (requires Admin role)
91. Create `PendingSuggestionsPage` in [client/src/pages/admin/PendingSuggestionsPage.tsx](client/src/pages/admin/PendingSuggestionsPage.tsx) - list of pending suggestions with approve/reject buttons (requires Admin role)
92. Create `AdminDashboardPage` in [client/src/pages/admin/AdminDashboardPage.tsx](client/src/pages/admin/AdminDashboardPage.tsx) - overview with stats (total boards, pending suggestions, total votes) (requires Admin role)

### Phase 16: Frontend Routing & Protected Routes

93. Create `ProtectedRoute` component in [client/src/components/ProtectedRoute.tsx](client/src/components/ProtectedRoute.tsx) to check authentication and optionally admin role, redirect to login if not authenticated
94. Set up React Router in [client/src/App.tsx](client/src/App.tsx) with routes:
    - `/` - HomePage (MainLayout)
    - `/login` - LoginPage (no layout/minimal layout)
    - `/register` - RegisterPage (no layout/minimal layout)
    - `/boards` - BoardsListPage (MainLayout)
    - `/boards/:id` - BoardDetailPage (MainLayout)
    - `/admin` - AdminDashboardPage (AdminLayout, protected, admin only)
    - `/admin/boards/create` - CreateBoardPage (AdminLayout, protected, admin only)
    - `/admin/boards/:id/edit` - EditBoardPage (AdminLayout, protected, admin only)
    - `/admin/boards/manage` - ManageBoardsPage (AdminLayout, protected, admin only)
    - `/admin/suggestions/pending` - PendingSuggestionsPage (AdminLayout, protected, admin only)
    - `*` - NotFoundPage (MainLayout)
95. Create `NotFoundPage` component for 404 errors
96. Hide admin navigation links from non-admin users

### Phase 17: Frontend Polish & User Experience

97. Add loading states with `LoadingSpinner` component in all pages during data fetching
98. Add error handling with `ErrorMessage` component for API failures
99. Implement success/error toast notifications using a simple toast utility or library
100. Add form validation with helpful error messages in all forms
101. Add confirmation dialogs for destructive actions (close board, reject suggestion, logout)
102. Implement responsive design breakpoints for mobile, tablet, desktop views in all components
103. Add accessibility attributes (ARIA labels, keyboard navigation) to interactive elements
104. Show appropriate feedback when user is not authenticated (login prompts, disabled buttons)
105. Auto-refresh board details after voting to show updated vote counts

### Phase 18: Testing & Documentation

106. Install testing dependencies: `@testing-library/react`, `@testing-library/jest-dom`, `@testing-library/user-event`, `vitest`, `jsdom`
107. Configure Vitest in [vite.config.ts](client/vite.config.ts) for testing
108. Create component tests for `BoardCard`, `SuggestionCard`, `VoteButton` in [client/tests/components](client/tests/components)
109. Create integration tests for `BoardsListPage` and `BoardDetailPage` mocking API calls
110. Create tests for authentication flow (login, logout, token expiration)
111. Create tests for voting rules enforcement and suggestion approval workflow
112. Update [README.md](README.md) with setup instructions, default admin credentials, API documentation, and usage guide
113. Add API documentation comments in controller methods for Swagger
114. Create [client/README.md](client/README.md) with frontend-specific documentation

---

**Verification**

**Backend:**
- Run `dotnet build` in [server/API](server/API) - should compile without errors
- Run `dotnet test` in [server/Tests](server/Tests) - all tests should pass
- Run `dotnet run` in [server/API](server/API) and navigate to Swagger UI at `/swagger` - should see all API endpoints documented with JWT authentication
- Test authentication flow with Swagger:
  - Login with default admin (username: "admin", password: "admin123") → receive JWT token
  - Use "Authorize" button in Swagger to add token
  - Test protected endpoints
- Test API endpoints:
  - Create a board (as admin)
  - Add suggestions (as authenticated user)
  - Vote on suggestions (as authenticated user)
  - Approve/reject suggestions (as admin)
  - Test voting rules (single vote limit, multi-vote with max votes)
  - Test authorization (non-admin cannot access admin endpoints)

**Frontend:**
- Run `npm install` in [client](client)
- Run `npm run dev` in [client](client) - should start dev server on `http://localhost:5173`
- Run `npm run build` - should build production bundle without errors
- Run `npm test` - all tests should pass
- Manual testing:
  - Visit app without logging in → limited functionality
  - Register new user → login successful
  - Browse boards list
  - View board details and suggestions
  - Try to vote → requires login
  - Login with regular user → can vote and submit suggestions
  - Submit suggestion and vote on suggestions
  - Verify voting rules enforcement
  - Logout and login as admin (username: "admin", password: "admin123")
  - Verify admin navigation appears
  - Create/edit boards (admin mode)
  - Approve/reject suggestions (admin mode)
  - Verify layouts switch between MainLayout and AdminLayout
  - Test responsive design on mobile viewport
  - Test all routing and navigation
  - Test token persists across page refreshes
  - Test token expiration handling

**Integration:**
- Start both backend and frontend
- Verify CORS configuration allows frontend to call API
- Test full user workflow: register → login → browse → vote → see results → logout
- Test full admin workflow: login as admin → create board → approve suggestions → close voting
- Verify JWT token is sent in Authorization header for protected requests
- Verify 401 responses redirect to login page
- Test password hashing (cannot login with wrong password)

---

**Decisions**

- **Authentication**: Simple username/password with JWT tokens (not fake localStorage)
- **Password Security**: BCrypt for password hashing
- **User Roles**: Two roles - Admin and User (stored in database)
- **Default Credentials**: Admin user seeded with username "admin" and password "admin123"
- **Token Storage**: JWT stored in localStorage on frontend
- **Database**: SQLite with repository pattern for easy provider swap in future
- **Build tool**: Vite for faster development and build times
- **.NET version**: .NET 9 (latest)
- **Styling**: Tailwind CSS for utility-first, flexible styling
- **State management**: React hooks + Context API (simpler, sufficient for app scope - can migrate to Redux later if needed)
- **HTTP client**: Axios for frontend (better error handling, interceptors for JWT)
- **Authorization**: Role-based with [Authorize] attributes and policies on backend, ProtectedRoute component on frontend
- **Voting rules enforcement**: Backend validation + frontend UI prevention (defensive programming)
- **Layouts**: Two distinct layouts with layout switching via routes - MainLayout for public pages, AdminLayout for admin pages
