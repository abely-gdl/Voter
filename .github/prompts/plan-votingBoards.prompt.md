# Plan: Voting Boards Application with .NET 9 API & React

A full-stack voting application where users can browse boards, submit suggestions, and vote on them. Admins can create/manage boards and moderate suggestions. Built with .NET 9 Web API backend, React 18+ with Vite and TypeScript frontend, EF Core with SQLite (repository pattern for DB interchangeability), Tailwind CSS styling, and fake localStorage-based authentication.

**Key Architectural Decisions:**
- Repository pattern for data access ensures future DB provider swapping
- Fake auth via localStorage flag (`isAdmin`) - toggle in UI
- Multiple layouts: MainLayout (public), AdminLayout with admin navigation
- Multi-page routing with React Router v6+
- RESTful API design with DTOs for all responses

---

**Steps**

### Phase 1: Backend Infrastructure Setup

1. Create .NET 9 Web API project in [server/API](server/API) using `dotnet new webapi` with .NET 9 SDK
2. Create xUnit test project in [server/Tests](server/Tests) using `dotnet new xunit`
3. Add solution file at repository root to manage both projects
4. Install NuGet packages in API project: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`, `AutoMapper.Extensions.Microsoft.DependencyInjection`, `FluentValidation.AspNetCore`, `Swashbuckle.AspNetCore` (Swagger)
5. Install test packages: `Moq`, `FluentAssertions`, `Microsoft.EntityFrameworkCore.InMemory` (for testing)
6. Configure CORS in [Program.cs](server/API/Program.cs) to allow React dev server (`http://localhost:5173` for Vite)
7. Configure Swagger/OpenAPI in [Program.cs](server/API/Program.cs) for API documentation
8. Add `appsettings.json` and `appsettings.Development.json` with SQLite connection string

### Phase 2: Backend Domain Models & Database

9. Create entity models in [server/API/Models](server/API/Models):
   - `Board` entity: Id, Title, Description, CreatedDate, IsSuggestionsOpen, IsVotingOpen, RequireApproval, VotingType (enum: Single/Multi), MaxVotes (nullable int), IsClosed
   - `Suggestion` entity: Id, BoardId (FK), Text, SubmittedBy, SubmittedDate, Status (enum: Pending/Approved/Rejected), IsVisible
   - `Vote` entity: Id, SuggestionId (FK), UserId (string), VotedDate

10. Create enums in [server/API/Models/Enums.cs](server/API/Models/Enums.cs): `VotingType`, `SuggestionStatus`
11. Create `VoterDbContext` in [server/API/Data/VoterDbContext.cs](server/API/Data/VoterDbContext.cs) with DbSets for entities, configure relationships and constraints
12. Create repository interfaces in [server/API/Data/Repositories](server/API/Data/Repositories): `IRepository<T>`, `IBoardRepository`, `ISuggestionRepository`, `IVoteRepository`
13. Implement generic repository and specific repositories in same folder with EF Core implementations
14. Create and apply initial EF Core migration: `dotnet ef migrations add Initial` in API project
15. Configure database seeding in `VoterDbContext.OnModelCreating` with 2-3 sample boards for development

### Phase 3: Backend DTOs & Mapping

16. Create DTOs in [server/API/DTOs](server/API/DTOs):
    - `BoardDto`, `BoardCreateDto`, `BoardUpdateDto`
    - `SuggestionDto`, `SuggestionCreateDto`, `SuggestionUpdateDto`
    - `VoteDto`, `VoteCreateDto`
    - `BoardDetailDto` (includes suggestions collection and vote counts)
    - `SuggestionWithVotesDto` (includes vote count)

17. Create AutoMapper profile in [server/API/Mapping/MappingProfile.cs](server/API/Mapping/MappingProfile.cs) to map entities to DTOs
18. Register AutoMapper in [Program.cs](server/API/Program.cs) DI container

### Phase 4: Backend Business Logic & Validation

19. Create service interfaces in [server/API/Services](server/API/Services): `IBoardService`, `ISuggestionService`, `IVoteService`
20. Implement `BoardService` with methods: GetAllBoards, GetBoardById, GetBoardWithDetails, CreateBoard, UpdateBoard, ToggleVoting, ToggleSuggestions, CloseBoard
21. Implement `SuggestionService` with methods: GetSuggestionsByBoardId, CreateSuggestion, ApproveSuggestion, RejectSuggestion, GetPendingSuggestions (admin)
22. Implement `VoteService` with methods: AddVote, GetVotesBySuggestionId, GetUserVotesByBoardId, RemoveVote, ValidateVotingRules (check multi/single, max votes)
23. Create FluentValidation validators in [server/API/Validators](server/API/Validators): `BoardCreateDtoValidator`, `SuggestionCreateDtoValidator`, `VoteCreateDtoValidator`
24. Register services and repositories in [Program.cs](server/API/Program.cs) DI container with scoped lifetime

### Phase 5: Backend API Controllers

25. Create `BoardsController` in [server/API/Controllers](server/API/Controllers) with endpoints:
    - `GET /api/boards` - list all boards
    - `GET /api/boards/{id}` - get board details with suggestions and votes
    - `POST /api/boards` - create board (admin)
    - `PUT /api/boards/{id}` - update board (admin)
    - `PUT /api/boards/{id}/toggle-voting` - toggle voting status (admin)
    - `PUT /api/boards/{id}/toggle-suggestions` - toggle suggestions status (admin)
    - `PUT /api/boards/{id}/close` - close board (admin)

26. Create `SuggestionsController` with endpoints:
    - `GET /api/boards/{boardId}/suggestions` - get approved suggestions for a board
    - `GET /api/suggestions/pending` - get pending suggestions (admin)
    - `POST /api/boards/{boardId}/suggestions` - create suggestion
    - `PUT /api/suggestions/{id}/approve` - approve suggestion (admin)
    - `PUT /api/suggestions/{id}/reject` - reject suggestion (admin)

27. Create `VotesController` with endpoints:
    - `POST /api/suggestions/{suggestionId}/votes` - add vote (includes userId in body)
    - `DELETE /api/suggestions/{suggestionId}/votes` - remove vote (includes userId in body)
    - `GET /api/boards/{boardId}/votes/user/{userId}` - get user's votes for a board

28. Add error handling middleware in [server/API/Middleware/ErrorHandlingMiddleware.cs](server/API/Middleware/ErrorHandlingMiddleware.cs) for consistent error responses
29. Register middleware in [Program.cs](server/API/Program.cs) pipeline

### Phase 6: Backend Testing

30. Create unit tests for `BoardService` in [server/Tests/Services/BoardServiceTests.cs](server/Tests/Services/BoardServiceTests.cs) using Moq for repository mocks
31. Create unit tests for `SuggestionService` and `VoteService` in respective test files
32. Create integration tests for controllers in [server/Tests/Controllers](server/Tests/Controllers) using `WebApplicationFactory` and InMemory database
33. Test voting rules validation (single vs multi, max votes enforcement)
34. Test suggestion approval workflow

### Phase 7: Frontend Infrastructure Setup

35. Initialize React + TypeScript + Vite project in [client](client) folder using `npm create vite@latest . -- --template react-ts`
36. Install dependencies: `react-router-dom`, `axios`, `clsx` (for conditional classes)
37. Install Tailwind CSS: `tailwindcss`, `postcss`, `autoprefixer`, configure [tailwind.config.js](client/tailwind.config.js) and [postcss.config.js](client/postcss.config.js)
38. Create Tailwind CSS imports in [client/src/index.css](client/src/index.css)
39. Update [vite.config.ts](client/vite.config.ts) to proxy API requests to `http://localhost:5000` (or backend port)
40. Configure `tsconfig.json` with path aliases (e.g., `@/` for `src/`)
41. Create folder structure: [src/components](client/src/components), [src/pages](client/src/pages), [src/layouts](client/src/layouts), [src/services](client/src/services), [src/hooks](client/src/hooks), [src/types](client/src/types), [src/utils](client/src/utils)

### Phase 8: Frontend Type Definitions & API Client

42. Create TypeScript types in [client/src/types/index.ts](client/src/types/index.ts) matching backend DTOs: `Board`, `BoardDetail`, `Suggestion`, `SuggestionWithVotes`, `Vote`, `VotingType`, `SuggestionStatus`
43. Create API service in [client/src/services/api.ts](client/src/services/api.ts) with axios instance configured with base URL
44. Create `boardService` in [client/src/services/boardService.ts](client/src/services/boardService.ts) with functions: `getBoards()`, `getBoardDetails(id)`, `createBoard(data)`, `updateBoard(id, data)`, `toggleVoting(id)`, `toggleSuggestions(id)`, `closeBoard(id)`
45. Create `suggestionService` in [client/src/services/suggestionService.ts](client/src/services/suggestionService.ts) with functions: `getSuggestions(boardId)`, `createSuggestion(boardId, data)`, `getPendingSuggestions()`, `approveSuggestion(id)`, `rejectSuggestion(id)`
46. Create `voteService` in [client/src/services/voteService.ts](client/src/services/voteService.ts) with functions: `addVote(suggestionId, userId)`, `removeVote(suggestionId, userId)`, `getUserVotes(boardId, userId)`

### Phase 9: Frontend Auth Context & Hooks

47. Create auth context in [client/src/contexts/AuthContext.tsx](client/src/contexts/AuthContext.tsx) with `isAdmin` and `userId` state, load/save from localStorage
48. Create `useAuth` hook in [client/src/hooks/useAuth.ts](client/src/hooks/useAuth.ts) to consume AuthContext
49. Create `toggleAdminMode()` and `setUserId()` functions in AuthContext
50. Create custom hooks in [client/src/hooks](client/src/hooks): `useBoards`, `useBoardDetails`, `useSuggestions`, `useVotes` for data fetching with loading/error states
51. Wrap app with `AuthProvider` in [client/src/main.tsx](client/src/main.tsx)

### Phase 10: Frontend Layouts & Navigation

52. Create `MainLayout` component in [client/src/layouts/MainLayout.tsx](client/src/layouts/MainLayout.tsx) with header (logo, navigation links, admin toggle switch), main content area, and footer
53. Create `AdminLayout` component in [client/src/layouts/AdminLayout.tsx](client/src/layouts/AdminLayout.tsx) extending MainLayout with admin-specific navigation (Create Board, Pending Suggestions, Manage Boards)
54. Create `Navigation` component in [client/src/components/Navigation.tsx](client/src/components/Navigation.tsx) with conditional links based on `isAdmin`
55. Style layouts with Tailwind CSS using responsive design patterns

### Phase 11: Frontend Core Components

56. Create `BoardCard` component in [client/src/components/BoardCard.tsx](client/src/components/BoardCard.tsx) to display board summary with title, description, vote count, status badges
57. Create `SuggestionCard` component in [client/src/components/SuggestionCard.tsx](client/src/components/SuggestionCard.tsx) to display suggestion with text, vote count, vote button, status indicator
58. Create `VoteButton` component in [client/src/components/VoteButton.tsx](client/src/components/VoteButton.tsx) with vote/unvote logic, disabled states, vote count display
59. Create `BoardForm` component in [client/src/components/BoardForm.tsx](client/src/components/BoardForm.tsx) for creating/editing boards with form validation
60. Create `SuggestionForm` component in [client/src/components/SuggestionForm.tsx](client/src/components/SuggestionForm.tsx) for submitting suggestions
61. Create reusable UI components: `Button`, `Card`, `Badge`, `Input`, `Textarea`, `Select`, `Toggle` in [client/src/components/ui](client/src/components/ui)
62. Create `LoadingSpinner` and `ErrorMessage` components for async states

### Phase 12: Frontend Pages - User Views

63. Create `BoardsListPage` in [client/src/pages/BoardsListPage.tsx](client/src/pages/BoardsListPage.tsx) - displays grid of BoardCards, filters (open/closed/all), uses `useBoards` hook
64. Create `BoardDetailPage` in [client/src/pages/BoardDetailPage.tsx](client/src/pages/BoardDetailPage.tsx) - shows board info, suggestion list with voting, suggestion form (if open), real-time results, uses `useBoardDetails` hook
65. Create `HomePage` in [client/src/pages/HomePage.tsx](client/src/pages/HomePage.tsx) - landing page with app description, link to browse boards
66. Implement voting logic in BoardDetailPage: check user's existing votes, enforce voting rules (single/multi, max votes), optimistic UI updates
67. Show/hide suggestion form based on `isSuggestionsOpen` board property
68. Display voting results with vote counts and percentage bars

### Phase 13: Frontend Pages - Admin Views

69. Create `CreateBoardPage` in [client/src/pages/admin/CreateBoardPage.tsx](client/src/pages/admin/CreateBoardPage.tsx) - form to create new board with all settings
70. Create `EditBoardPage` in [client/src/pages/admin/EditBoardPage.tsx](client/src/pages/admin/EditBoardPage.tsx) - form to update board settings, quick toggles for voting/suggestions/close status
71. Create `ManageBoardsPage` in [client/src/pages/admin/ManageBoardsPage.tsx](client/src/pages/admin/ManageBoardsPage.tsx) - table/list of all boards with edit/delete actions, status indicators
72. Create `PendingSuggestionsPage` in [client/src/pages/admin/PendingSuggestionsPage.tsx](client/src/pages/admin/PendingSuggestionsPage.tsx) - list of pending suggestions with approve/reject buttons
73. Create `AdminDashboardPage` in [client/src/pages/admin/AdminDashboardPage.tsx](client/src/pages/admin/AdminDashboardPage.tsx) - overview with stats (total boards, pending suggestions, total votes)

### Phase 14: Frontend Routing & Protected Routes

74. Create `ProtectedRoute` component in [client/src/components/ProtectedRoute.tsx](client/src/components/ProtectedRoute.tsx) to check `isAdmin` and redirect if not authorized
75. Set up React Router in [client/src/App.tsx](client/src/App.tsx) with routes:
    - `/` - HomePage (MainLayout)
    - `/boards` - BoardsListPage (MainLayout)
    - `/boards/:id` - BoardDetailPage (MainLayout)
    - `/admin` - AdminDashboardPage (AdminLayout, protected)
    - `/admin/boards/create` - CreateBoardPage (AdminLayout, protected)
    - `/admin/boards/:id/edit` - EditBoardPage (AdminLayout, protected)
    - `/admin/boards/manage` - ManageBoardsPage (AdminLayout, protected)
    - `/admin/suggestions/pending` - PendingSuggestionsPage (AdminLayout, protected)
    - `*` - NotFoundPage (MainLayout)
76. Create `NotFoundPage` component for 404 errors

### Phase 15: Frontend Polish & User Experience

77. Add loading states with `LoadingSpinner` component in all pages during data fetching
78. Add error handling with `ErrorMessage` component for API failures
79. Implement success/error toast notifications using a simple toast utility or library
80. Add form validation with helpful error messages in BoardForm and SuggestionForm
81. Add confirmation dialogs for destructive actions (close board, reject suggestion)
82. Implement responsive design breakpoints for mobile, tablet, desktop views in all components
83. Add accessibility attributes (ARIA labels, keyboard navigation) to interactive elements
84. Add "Admin Mode" toggle in header (prominent switch/button visible to all users for demo purposes)
85. Create user ID input/selector in header (simple text input or dropdown for demo purposes)

### Phase 16: Testing & Documentation

86. Install testing dependencies: `@testing-library/react`, `@testing-library/jest-dom`, `@testing-library/user-event`, `vitest`, `jsdom`
87. Configure Vitest in [vite.config.ts](client/vite.config.ts) for testing
88. Create component tests for `BoardCard`, `SuggestionCard`, `VoteButton` in [client/tests/components](client/tests/components)
89. Create integration tests for `BoardsListPage` and `BoardDetailPage` mocking API calls
90. Create tests for voting rules enforcement and suggestion approval workflow
91. Update [README.md](README.md) with setup instructions, API documentation, and usage guide
92. Add API documentation comments in controller methods for Swagger
93. Create [client/README.md](client/README.md) with frontend-specific documentation

---

**Verification**

**Backend:**
- Run `dotnet build` in [server/API](server/API) - should compile without errors
- Run `dotnet test` in [server/Tests](server/Tests) - all tests should pass
- Run `dotnet run` in [server/API](server/API) and navigate to Swagger UI at `/swagger` - should see all API endpoints documented
- Test API endpoints with Swagger or Postman:
  - Create a board
  - Add suggestions
  - Vote on suggestions
  - Approve/reject suggestions
  - Test voting rules (single vote limit, multi-vote with max votes)

**Frontend:**
- Run `npm install` in [client](client)
- Run `npm run dev` in [client](client) - should start dev server on `http://localhost:5173`
- Run `npm run build` - should build production bundle without errors
- Run `npm test` - all tests should pass
- Manual testing:
  - Browse boards list
  - View board details and suggestions
  - Submit suggestion (user mode)
  - Vote on suggestions, verify voting rules enforcement
  - Toggle admin mode
  - Create/edit boards (admin mode)
  - Approve/reject suggestions (admin mode)
  - Verify layouts switch between MainLayout and AdminLayout
  - Test responsive design on mobile viewport
  - Test all routing and navigation

**Integration:**
- Start both backend and frontend
- Verify CORS configuration allows frontend to call API
- Test full user workflow: browse → vote → see results
- Test full admin workflow: create board → approve suggestions → close voting
- Verify fake auth persists in localStorage across page refreshes

---

**Decisions**

- **Database**: SQLite with repository pattern for easy provider swap in future
- **Build tool**: Vite for faster development and build times
- **.NET version**: .NET 9 (latest)
- **Styling**: Tailwind CSS for utility-first, flexible styling
- **State management**: React hooks + Context API (simpler, sufficient for app scope - can migrate to Redux later if needed)
- **HTTP client**: Axios for frontend (better error handling, interceptors)
- **Fake auth**: localStorage-based with UI toggles - `isAdmin` boolean flag and `userId` string, both editable in UI for demo purposes
- **Voting rules enforcement**: Backend validation + frontend UI prevention (defensive programming)
- **Layouts**: Two distinct layouts with layout switching via routes - MainLayout for public pages, AdminLayout for admin pages
