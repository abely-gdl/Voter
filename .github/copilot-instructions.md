# GitHub Copilot Instructions for Voter Application

## Project Overview
This is a full-stack voting application with a React frontend and ASP.NET Core backend. The application allows users to create and participate in polls/votes.

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 10.0 Web API
- **Language**: C# 13
- **Testing**: xUnit, Moq, FluentAssertions
- **Database**: Entity Framework Core (specify provider as needed)
- **API Style**: RESTful

### Frontend
- **Framework**: React 18+
- **Language**: TypeScript
- **Build Tool**: Vite or Create React App
- **Testing**: Jest, React Testing Library
- **State Management**: React Context API or Redux (specify when implemented)
- **HTTP Client**: Axios or Fetch API

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
- Follow **Repository Pattern** for data access
- Use **Dependency Injection** for all services
- Controller actions should return `ActionResult<T>` or `IActionResult`
- Use **DTOs** for API request/response, not direct entity models
- Apply **data annotations** for validation
- Follow C# naming conventions: PascalCase for classes, methods, properties
- Use **nullable reference types** (enabled by default in .NET 10)
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
- Use plural nouns for resource names: `/api/polls`, `/api/votes`
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
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;

    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PollDto>> GetPoll(int id)
    {
        var poll = await _pollService.GetPollByIdAsync(id);
        return poll == null ? NotFound() : Ok(poll);
    }
}
```

### Frontend Service Example
```typescript
export const pollService = {
  async getPolls(): Promise<Poll[]> {
    const response = await fetch('/api/polls');
    if (!response.ok) throw new Error('Failed to fetch polls');
    return response.json();
  }
};
```

### React Component Example
```typescript
interface PollProps {
  pollId: number;
}

export const PollComponent: React.FC<PollProps> = ({ pollId }) => {
  const [poll, setPoll] = useState<Poll | null>(null);
  
  useEffect(() => {
    // Fetch poll data
  }, [pollId]);

  return <div>{/* Render poll */}</div>;
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
- React Query or SWR (for data fetching)
- Formik or React Hook Form (for forms)
- Tailwind CSS or Material-UI (for styling)
