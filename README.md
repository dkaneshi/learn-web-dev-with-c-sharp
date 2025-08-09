# Learn C# for Web Development

A modern, full-stack web application that teaches C# for web development from zero to job-ready, emphasizing ASP.NET Core, Entity Framework Core, and hands-on learning through interactive coding challenges.

## 🎯 Features

- **Video-First Learning**: Short, focused video lessons (5-10 minutes)
- **Interactive Coding Labs**: Auto-graded coding challenges with instant feedback
- **Portfolio Projects**: Real-world projects including TaskBoard, Northwind API, and Meal Planner
- **Gamification**: XP points, badges, streaks, and progress tracking
- **Comprehensive Curriculum**: From C# basics to deployment

## 🚀 Quick Start

### Using Docker (Recommended)

```bash
# Clone the repository
git clone <your-repo-url>
cd web-dev-with-c-sharp

# Start with Docker Compose
docker-compose up --build

# Visit http://localhost:5173
```

### Local Development

**Prerequisites:**
- .NET 8.0 SDK
- Node.js (for Tailwind CSS)

**Windows:**
```powershell
./scripts/dev.ps1
```

**macOS/Linux:**
```bash
chmod +x scripts/dev.sh
./scripts/dev.sh
```

### Manual Setup

```bash
# Restore packages
dotnet restore

# Update database
cd src/LearnCSharp.Web
dotnet ef database update

# Run the application
dotnet run --urls="https://localhost:5001;http://localhost:5000"
```

## 📚 Curriculum Overview

### Learning Tracks

1. **C# Fundamentals for Web**
   - Types, nullable reference types, records
   - async/await, LINQ
   - Object-oriented programming

2. **ASP.NET Core Basics**
   - Program.cs, DI, configuration
   - Logging, middleware, routing

3. **Razor Pages & MVC**
   - ViewModels, validation attributes
   - Tag helpers, partials

4. **Data & Entity Framework Core**
   - DbContext, migrations, relationships
   - Tracking vs no-tracking queries

5. **Minimal APIs**
   - Endpoints, filters, FluentValidation
   - Swagger documentation

6. **Authentication & Security**
   - Identity, cookie vs JWT
   - Policies, anti-forgery, data protection

7. **Testing**
   - xUnit, integration tests with TestServer
   - Mocking strategies

8. **Deployment**
   - Docker, Azure App Service
   - Connection strings, configuration

### Project Portfolio

- **TaskBoard**: Razor Pages + EF Core + CRUD operations
- **Northwind API**: Minimal APIs + Swagger + DTO validation  
- **AuthN/AuthZ Demo**: Identity + Roles + Policy-based auth
- **Meal Planner** (Capstone): Full MVC + EF Core + caching + logging + tests

## 🏗️ Architecture

```
src/
├── LearnCSharp.Core/           # Domain models and services
│   ├── Models/                 # Entity models
│   ├── Data/                   # DbContext and configurations
│   └── Services/               # Business logic services
└── LearnCSharp.Web/            # Web application
    ├── Pages/                  # Razor Pages
    ├── Controllers/            # API controllers
    ├── Services/               # Web-specific services
    └── wwwroot/                # Static files

tests/
└── LearnCSharp.Tests/          # Unit and integration tests

lab-content/                    # Lab starter code and tests
```

## 🛠️ Technology Stack

- **Backend**: .NET 8 LTS, ASP.NET Core, EF Core
- **Frontend**: Razor Pages, Tailwind CSS, Alpine.js
- **Database**: SQLite (dev), PostgreSQL/Azure SQL (production)
- **Authentication**: ASP.NET Core Identity
- **Testing**: xUnit, FluentAssertions, WebApplicationFactory
- **CI/CD**: GitHub Actions
- **Containerization**: Docker & Docker Compose

## 🧪 Testing

Run all tests:
```bash
./scripts/test.sh
# or
dotnet test
```

Run with coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Development Scripts

| Script | Purpose |
|--------|---------|
| `scripts/dev.sh` / `scripts/dev.ps1` | Start development server |
| `scripts/test.sh` | Run all tests |
| `make install` | Install dependencies |
| `make build` | Build the application |
| `make seed` | Seed the database with sample data |

## 📊 Database

The application uses Entity Framework Core with the following key entities:

- **User**: Extended IdentityUser with XP, streaks
- **Track**: Learning paths (e.g., "C# Fundamentals")
- **Lesson**: Individual lessons with video/content
- **Lab**: Coding challenges with auto-grading
- **Progress**: User progress through lessons
- **Badge**: Achievement system

### Database Commands

```bash
# Create new migration
dotnet ef migrations add MigrationName -p src/LearnCSharp.Core -s src/LearnCSharp.Web

# Update database
dotnet ef database update -s src/LearnCSharp.Web

# Reset database (development only)
rm data/learncsharp.db
dotnet ef database update -s src/LearnCSharp.Web
```

## 🚀 Deployment

### Docker Production

```bash
# Build production image
docker build -t learncsharp:latest .

# Run with environment variables
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Your-Production-Connection-String" \
  -e ASPNETCORE_ENVIRONMENT=Production \
  learncsharp:latest
```

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Database connection | SQLite local file |
| `ASPNETCORE_ENVIRONMENT` | Environment | Development |
| `Serilog__MinimumLevel` | Logging level | Information |

## 🎮 Lab System

The interactive lab system provides:

- **Secure Code Execution**: Sandboxed .NET compilation and execution
- **Auto-Grading**: Unit test-based scoring
- **Instant Feedback**: Real-time compilation errors and test results
- **Progress Tracking**: Automatic progress updates

### Example Lab: Minimal API Todos

Students implement a GET `/api/todos` endpoint with:
- Optional status filtering (`all`, `open`, `done`)
- Proper HTTP status codes
- Problem details for validation errors

## 🏆 Gamification Features

- **XP Points**: Earned by completing lessons and labs
- **Badges**: Achievement system (First Steps, Lab Master, Streak Master, etc.)
- **Streaks**: Daily activity tracking
- **Progress Visualization**: Track completion across learning paths

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Write tests for new features
- Update documentation for public APIs
- Ensure all tests pass before submitting PRs

## 📋 Default Accounts

The application seeds with default accounts:

- **Admin**: `admin@learncsharp.com` / `Admin123!`
- **Learner**: `learner@example.com` / `Learner123!`

## 🐛 Troubleshooting

### Common Issues

**Database Issues**
```bash
# Delete and recreate database
rm data/learncsharp.db
dotnet ef database update -s src/LearnCSharp.Web
```

**Port Already in Use**
```bash
# Check what's using the port
lsof -i :5000
# or change port in launchSettings.json
```

**Docker Build Issues**
```bash
# Clean Docker cache
docker system prune -a
docker-compose build --no-cache
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋‍♂️ Support

- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)
- **Documentation**: See `/docs` folder

## 🚀 Roadmap

- [ ] **Blazor WebAssembly Module**: Interactive client-side labs
- [ ] **AI Tutor Integration**: Context-aware help system  
- [ ] **Mobile App**: React Native companion app
- [ ] **Video Recording Studio**: Built-in lesson creation tools
- [ ] **Advanced Analytics**: Detailed learning analytics dashboard
- [ ] **Collaborative Features**: Code review and peer learning
- [ ] **Enterprise Features**: Team management and reporting

---

**Built with ❤️ using ASP.NET Core 8, Entity Framework Core, and Tailwind CSS**