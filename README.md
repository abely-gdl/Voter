# Voter Application

A voting platform where users submit suggestions and vote on ideas. Perfect for feature requests, team decisions, and community feedback.

## What You Can Do

- **Create Voting Boards**: Set up boards with custom rules (single/multiple votes, vote limits)
- **Submit Suggestions**: Add ideas to any open board
- **Vote on Ideas**: Cast votes following board-specific rules
- **Moderate Content**: Admins can approve/reject suggestions before they go live
- **Manage Access**: Admin and User roles with different permissions
- **Track Results**: View vote counts and suggestion status in real-time

## Tech Stack

**Backend**: .NET 9 Web API, Entity Framework Core, SQLite  
**Frontend**: React 18, TypeScript, Vite, Tailwind CSS  
**Testing**: xUnit (backend), Vitest (frontend)

## Quick Start

### Prerequisites
- .NET 9 SDK
- Node.js 18+

### Setup

1. **Backend**
   ```bash
   cd server/API
   dotnet restore
   dotnet ef database update
   dotnet run
   ```
   API runs on `http://localhost:5076/`
   Swaggger    `http://localhost:5076/swagger`

2. **Frontend**
   ```bash
   cd client
   npm install
   npm run dev
   ```
   App runs on `http://localhost:5173`

### Default Login
- **Admin**: `admin` / `admin123`

## Testing

```bash
# Backend (18 tests)
cd server/Tests
dotnet test

# Frontend
cd client
npm test
```

## API Overview

**Auth**: `/api/auth/register`, `/api/auth/login`, `/api/auth/me`  
**Boards**: `/api/boards` - CRUD operations  
**Suggestions**: `/api/boards/{id}/suggestions` - Create and moderate  
**Votes**: `/api/votes` - Cast and remove votes

## Project Structure

```
├── client/          # React frontend
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   └── services/
│   └── tests/
└── server/
    ├── API/         # .NET Web API
    │   ├── Controllers/
    │   ├── Services/
    │   └── Data/
    └── Tests/       # xUnit tests
```

## License

Educational and demonstration purposes.
