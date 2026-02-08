# Voter Application

A full-stack voting application built with React and .NET.

## Project Structure

```
Voter/
├── client/           # React frontend application
│   ├── src/         # React source code
│   ├── public/      # Public assets
│   └── tests/       # Frontend tests
├── server/          # .NET backend application
│   ├── API/         # .NET Web API project
│   └── Tests/       # Backend tests
└── .gitignore
```

## Getting Started

### Prerequisites

- Node.js (v18 or higher)
- .NET SDK (v8.0 or higher)
- Git

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd Voter
   ```

2. Install frontend dependencies:
   ```bash
   cd client
   npm install
   ```

3. Restore backend dependencies:
   ```bash
   cd server
   dotnet restore
   ```

### Running the Application

#### Frontend
```bash
cd client
npm start
```

#### Backend
```bash
cd server/API
dotnet run
```

### Running Tests

#### Frontend Tests
```bash
cd client
npm test
```

#### Backend Tests
```bash
cd server/Tests
dotnet test
```

## Technologies Used

- **Frontend**: React, TypeScript
- **Backend**: .NET 8, ASP.NET Core Web API
- **Testing**: Jest (frontend), xUnit (backend)
