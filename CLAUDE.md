# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Quick Start
- `./scripts/dev.sh` - Start development server (builds, creates DB if needed, runs on https://localhost:5001)
- `./scripts/test.sh` - Run all tests with detailed output
- `docker-compose up --build` - Start with Docker (recommended for consistent environment)

### Core Commands
- `dotnet build` - Build the solution
- `dotnet test` - Run all tests
- `dotnet run --project src/LearnCSharp.Web` - Run web application
- `make dev` - Alternative development startup using Makefile
- `make test` - Run tests via Makefile
- `make format` - Format code using dotnet format
- `make lint` - Verify code formatting

### Database Operations
- `dotnet ef database update -s src/LearnCSharp.Web` - Apply migrations
- `dotnet ef migrations add MigrationName -p src/LearnCSharp.Core -s src/LearnCSharp.Web` - Create migration
- `make reset-db` - Delete and recreate database (dev only)

### Testing
- `dotnet test --collect:"XPlat Code Coverage"` - Run tests with coverage
- `dotnet watch test --project tests/LearnCSharp.Tests` - Watch mode testing

## Architecture Overview

This is a full-stack C# learning platform with a 3-layer architecture:

### Project Structure
- **LearnCSharp.Core** - Domain models, Entity Framework DbContext, business services
- **LearnCSharp.Web** - ASP.NET Core web application with Razor Pages + API controllers
- **LearnCSharp.Tests** - Unit and integration tests

### Key Components

#### Database Layer (`LearnCSharp.Core.Data`)
- Uses Entity Framework Core with SQLite (dev) / configurable for production
- Identity integration with custom User model extending IdentityUser
- Main entities: Track, Lesson, Lab, LabSubmission, Progress, Badge, Quiz

#### Business Services (`LearnCSharp.Core.Services`)
- `LabRunnerService` - Secure code execution engine that:
  - Creates temporary workspaces for each lab submission
  - Extracts starter code from zip files
  - Compiles and runs user C# code in sandboxed environment
  - Executes unit tests with 30-second timeout
  - Calculates scores based on test pass rate
- `ProgressService` - Handles learning progress tracking and gamification

#### Web Layer (`LearnCSharp.Web`)
- Razor Pages for UI (auth, dashboard, lessons)
- API controllers for lab execution and data operations
- ASP.NET Core Identity for authentication with custom policies
- Swagger/OpenAPI integration for API documentation

### Security Features
- Sandboxed code execution in temporary directories
- Process timeouts and resource limits for lab execution
- Role-based authorization (Admin, Instructor policies)
- Anti-forgery tokens and secure cookie configuration

### Data Flow for Lab Execution
1. User submits C# code via web interface
2. `LabController` receives submission and calls `LabRunnerService`
3. Service creates isolated workspace, extracts starter files
4. User code is compiled and tested against unit tests
5. Results are scored and persisted to database
6. Temporary workspace is cleaned up

## Development Practices

### Authentication
- Default seeded accounts: admin@learncsharp.com/Admin123!, learner@example.com/Learner123!
- Custom password policy (minimum 6 chars, flexible requirements for learning environment)

### Database
- Migrations are in LearnCSharp.Core project
- Use Entity Framework conventions for relationships
- Unique constraints on Track.Slug, Lesson.Slug, and composite keys

### Lab Content Structure
- Starter code and tests are stored as zip files
- Lab content goes in `lab-content/` directory
- Each lab has associated unit tests for auto-grading

### URLs and Ports
- Development: https://localhost:5001, http://localhost:5000
- Docker: http://localhost:5173 (maps to container port 8080)
- Health check endpoint: `/api/health`