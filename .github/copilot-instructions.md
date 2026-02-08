# GitHub Copilot Instructions for Voter Application

## Project Overview
This is a full-stack Voting Boards application with a React frontend and .NET backend. The application allows users to browse boards (voting spaces), submit suggestions, and vote on them. Admins can create/manage boards and moderate suggestions.

### Domain Model
- **User**: Authentication entity with username, hashed password, role (Admin/User)
- **Board**: A voting space with configurable rules (single/multi voting, max votes, suggestion approval, open/close status)
- **Suggestion**: User-submitted items that can be voted on, with approval workflow for admins
- **Vote**: User votes on suggestions, with rules enforcement (single vote vs multiple votes per board)

### Authentication
- **Simple authentication** with username/password stored in database
- Two roles: **Admin** and **User**
- Default admin user seeded in database (username: "admin", password: "admin123")
- Users can register as regular users or login with existing credentials
- JWT tokens or session-based auth for maintaining login state
- Password hashing (BCrypt or similar) for security

## Assistant Behavior
- **Act as Product Owner/Business Analyst**: Suggest use-cases, features, and user stories when appropriate
- **Keep explanations brief**: Provide concise responses for code changes; details only when requested
- **Keep it simple**: Avoid overcomplicating solutions; prefer straightforward, maintainable code

## Technology Stack

### Backend
- **Framework**: .NET 9 Web API
- **Language**: C# 13
- **Testing**: xUnit, Moq, FluentAssertions
- **Database**: Entity Framework Core with SQLite (repository pattern ensures DB interchangeability)
- **API Style**: RESTful

### Frontend
- **Framework**: React 18+
- **Language**: TypeScript
- **Build Tool**: Vite
- **Testing**: Vitest, React Testing Library
- **State Management**: React Context API
- **HTTP Client**: Axios
- **Styling**: Tailwind CSS

## Project Structure

```
Voter/
├── client/              # React frontend
│   ├── src/
│   │   ├── components/  # Reusable UI components
│   │   ├── pages/       # Page-level components
│   │   ├── services/    # API service layer
│   │   ├── hooks/       # Custom React hooks
│   │   ├── utils/       # Utility functions
│   │   └── types/       # TypeScript type definitions
│   ├── public/
│   └── tests/
├── server/              # .NET backend
│   ├── API/
│   │   ├── Controllers/ # API endpoints
│   │   ├── Models/      # Data models
│   │   ├── Services/    # Business logic
│   │   ├── Data/        # Database context and repositories
│   │   └── DTOs/        # Data transfer objects
│   └── Tests/           # Backend unit and integration tests
```

## Coding Conventions

### Backend (C#)
- Use **async/await** for all I/O operations
- Follow **Repository Pattern** for data access (ensures database provider can be swapped easily)
- Repositories must use interfaces (`IRepository<T>`, `IBoardRepository`, etc.)
- Use **Dependency Injection** for all services and repositories
- Controller actions should return `ActionResult<T>` or `IActionResult`
- Use **DTOs** for API request/response, not direct entity models
- Apply **data annotations** for validation
- Follow C# naming conventions: PascalCase for classes, methods, properties
- Use **nullable reference types** (enabled by default in .NET 9)
- Implement proper error handling with middleware
- Write XML documentation comments for public APIs

### Frontend (React/TypeScript)
- Use **functional components** with hooks (no class components)
- Use **TypeScript** for all components and type safety
- Follow **React best practices**: composition over inheritance
- Component naming: PascalCase for components, camelCase for functions/variables
- Use **custom hooks** for reusable logic
- Keep components small and focused (Single Responsibility)
- Use **props destructuring** for component parameters
- Implement **error boundaries** for error handling
- Use **async/await** for API calls
- Organize imports: React imports first, then third-party, then local

### API Design
- RESTful endpoints following conventions:
  - GET: Retrieve resources
  - POST: Create resources
  - PUT: Update entire resources
  - PATCH: Partial updates
  - DELETE: Remove resources
- Use plural nouns for resource names: `/api/boards`, `/api/suggestions`, `/api/votes`
- Return appropriate HTTP status codes
- Use consistent response structure for errors

### Testing
- **Backend**: Unit tests for services, integration tests for controllers
- **Frontend**: Component tests using React Testing Library
- Aim for meaningful test coverage on business logic
- Use descriptive test names: `Should_ReturnPoll_When_ValidIdProvided`
- Mock external dependencies in unit tests

## Common Patterns

### Backend API Controller Example
```csharp
[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardsController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BoardDto>> GetBoard(int id)
    {
        var board = await _boardService.GetBoardByIdAsync(id);
        return board == null ? NotFound() : Ok(board);
    }
}
```

### Frontend Service Example
```typescript
import axios from 'axios';
import { Board } from '../types';

export const boardService = {
  async getBoards(): Promise<Board[]> {
    const response = await axios.get<Board[]>('/api/boards');
    return response.data;
  }
};
```

### React Component Example
```typescript
interface BoardCardProps {
  boardId: number;
}

export const BoardCard: React.FC<BoardCardProps> = ({ boardId }) => {
  const [board, setBoard] = useState<Board | null>(null);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    // Fetch board data
  }, [boardId]);

  if (loading) return <LoadingSpinner />;
  return <div>{/* Render board */}</div>;
};
```

## Guidelines for Code Generation

1. **Always use TypeScript** for frontend code
2. **Include error handling** in all API calls and service methods
3. **Add validation** for user inputs and API parameters
4. **Use environment variables** for configuration (API URLs, secrets)
5. **Follow SOLID principles** in backend architecture
6. **Implement loading and error states** in UI components
7. **Add appropriate logging** for debugging
8. **Consider accessibility** (ARIA labels, keyboard navigation)
9. **Write self-documenting code** with clear variable names
10. **Add comments** for complex business logic only

## Security Considerations
- Validate all inputs on both client and server
- Use CORS configuration appropriately
- Implement authentication/authorization where needed
- Never expose sensitive data in API responses
- Use HTTPS in production
- Sanitize user inputs to prevent XSS attacks

## Performance
- Use pagination for large data sets
- Implement caching where appropriate
- Optimize database queries (use indexes, avoid N+1)
- Lazy load components and routes in React
- Minimize bundle size

## Preferred Libraries & Packages

### Backend
- AutoMapper (for DTO mapping)
- Serilog (for logging)
- FluentValidation (for validation)
- Swagger/OpenAPI (for API documentation)

### Frontend
- React Router (for routing)
- Axios (for HTTP requests)
- clsx (for conditional CSS classes)
- Tailwind CSS (for styling)
