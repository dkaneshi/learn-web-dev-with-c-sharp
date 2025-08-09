using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Web.Services;

public class SeedDataService
{
    private readonly LearnCSharpDbContext _context;

    public SeedDataService(LearnCSharpDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedBadgesAsync();
        await SeedTracksAndLessonsAsync();
        await SeedProjectsAsync();
    }

    private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "Instructor", "Learner" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task SeedUsersAsync(UserManager<User> userManager)
    {
        if (!await _context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                UserName = "admin@learncsharp.com",
                Email = "admin@learncsharp.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var learnerUser = new User
            {
                UserName = "learner@example.com",
                Email = "learner@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            result = await userManager.CreateAsync(learnerUser, "Learner123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(learnerUser, "Learner");
            }
        }
    }

    private async Task SeedBadgesAsync()
    {
        if (!await _context.Badges.AnyAsync())
        {
            var badges = new[]
            {
                new Badge { Name = "First Steps", Description = "Complete your first lesson", Icon = "🎯" },
                new Badge { Name = "Getting Started", Description = "Complete 5 lessons", Icon = "🚀" },
                new Badge { Name = "Dedicated Learner", Description = "Complete 10 lessons", Icon = "📚" },
                new Badge { Name = "Track Finisher", Description = "Complete an entire track", Icon = "🏆" },
                new Badge { Name = "Lab Master", Description = "Complete 10 labs", Icon = "⚡" },
                new Badge { Name = "Perfect Score", Description = "Get 100% on a lab", Icon = "💯" },
                new Badge { Name = "Streak Master", Description = "Maintain a 7-day streak", Icon = "🔥" }
            };

            _context.Badges.AddRange(badges);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedTracksAndLessonsAsync()
    {
        if (!await _context.Tracks.AnyAsync())
        {
            var tracks = new[]
            {
                new Track
                {
                    Title = "C# Fundamentals for Web",
                    Slug = "csharp-fundamentals",
                    Order = 1,
                    Description = "Learn C# basics including types, nullable reference types, records, async/await, and LINQ"
                },
                new Track
                {
                    Title = "ASP.NET Core Basics",
                    Slug = "aspnet-core-basics",
                    Order = 2,
                    Description = "Master Program.cs, dependency injection, configuration, logging, middleware, and routing"
                },
                new Track
                {
                    Title = "Razor Pages & MVC",
                    Slug = "razor-pages-mvc",
                    Order = 3,
                    Description = "Build web applications with ViewModels, validation attributes, tag helpers, and partials"
                },
                new Track
                {
                    Title = "Data & Entity Framework Core",
                    Slug = "data-ef-core",
                    Order = 4,
                    Description = "Work with databases using DbContext, migrations, relationships, and tracking vs no-tracking"
                },
                new Track
                {
                    Title = "Minimal APIs",
                    Slug = "minimal-apis",
                    Order = 5,
                    Description = "Create APIs with endpoints, filters, FluentValidation, and Swagger documentation"
                },
                new Track
                {
                    Title = "Authentication & Security",
                    Slug = "auth-security",
                    Order = 6,
                    Description = "Implement Identity, cookie vs JWT, policies, anti-forgery, and data protection"
                },
                new Track
                {
                    Title = "Testing",
                    Slug = "testing",
                    Order = 7,
                    Description = "Write unit and integration tests with xUnit, TestServer, and mocking"
                },
                new Track
                {
                    Title = "Deployment",
                    Slug = "deployment",
                    Order = 8,
                    Description = "Deploy with Docker, Azure App Service, and manage connection strings"
                }
            };

            _context.Tracks.AddRange(tracks);
            await _context.SaveChangesAsync();

            await SeedLessonsAsync();
        }
    }

    private async Task SeedLessonsAsync()
    {
        var fundamentalsTrack = await _context.Tracks.FirstAsync(t => t.Slug == "csharp-fundamentals");
        var aspNetTrack = await _context.Tracks.FirstAsync(t => t.Slug == "aspnet-core-basics");
        var minimalApisTrack = await _context.Tracks.FirstAsync(t => t.Slug == "minimal-apis");

        var lessons = new[]
        {
            new Lesson
            {
                TrackId = fundamentalsTrack.Id,
                Title = "C# Types and Variables",
                Slug = "csharp-types-variables",
                Order = 1,
                VideoUrl = "https://example.com/video1",
                Reading = "# C# Types and Variables\n\nLearn about value types, reference types, and nullable reference types in C#.",
                DurationMinutes = 8
            },
            new Lesson
            {
                TrackId = fundamentalsTrack.Id,
                Title = "Control Flow and Loops",
                Slug = "control-flow-loops",
                Order = 2,
                VideoUrl = "https://example.com/video2",
                Reading = "# Control Flow and Loops\n\nMaster if statements, switch expressions, for loops, and foreach loops.",
                DurationMinutes = 10
            },
            new Lesson
            {
                TrackId = fundamentalsTrack.Id,
                Title = "LINQ Fundamentals",
                Slug = "linq-fundamentals",
                Order = 3,
                VideoUrl = "https://example.com/video3",
                Reading = "# LINQ Fundamentals\n\nDiscover the power of LINQ for querying collections.",
                DurationMinutes = 12
            },
            new Lesson
            {
                TrackId = aspNetTrack.Id,
                Title = "Program.cs and Startup",
                Slug = "program-startup",
                Order = 1,
                VideoUrl = "https://example.com/video4",
                Reading = "# Program.cs and Startup\n\nLearn how to configure your ASP.NET Core application.",
                DurationMinutes = 9
            },
            new Lesson
            {
                TrackId = aspNetTrack.Id,
                Title = "Dependency Injection",
                Slug = "dependency-injection",
                Order = 2,
                VideoUrl = "https://example.com/video5",
                Reading = "# Dependency Injection\n\nUnderstand the built-in DI container in ASP.NET Core.",
                DurationMinutes = 11
            },
            new Lesson
            {
                TrackId = minimalApisTrack.Id,
                Title = "Creating Your First API",
                Slug = "first-api",
                Order = 1,
                VideoUrl = "https://example.com/video6",
                Reading = "# Creating Your First API\n\nBuild a simple GET endpoint using Minimal APIs.",
                DurationMinutes = 7
            }
        };

        _context.Lessons.AddRange(lessons);
        await _context.SaveChangesAsync();

        await SeedLabsAndQuizzesAsync();
    }

    private async Task SeedLabsAndQuizzesAsync()
    {
        var firstApiLesson = await _context.Lessons.FirstAsync(l => l.Slug == "first-api");

        var lab = new Lab
        {
            LessonId = firstApiLesson.Id,
            Title = "Create a GET /api/todos endpoint (Minimal API)",
            Prompt = @"Implement a GET /api/todos endpoint that returns a 200 OK response with a JSON array from an in-memory repository. 

The endpoint should:
1. Accept an optional 'status' query parameter that filters todos by status
2. Valid status values are: 'all', 'open', 'done'
3. Return 400 Bad Request with problem details for invalid status values
4. Return all todos when status is 'all' or not provided

Use the provided Todo record and in-memory repository.",
            MaxScore = 100,
            Order = 1
        };

        _context.Labs.Add(lab);

        var quiz = new Quiz
        {
            LessonId = firstApiLesson.Id,
            Title = "Minimal API Basics",
            Order = 1
        };

        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();

        var questions = new[]
        {
            new QuizQuestion
            {
                QuizId = quiz.Id,
                Type = "multiple-choice",
                Prompt = "What HTTP status code should be returned for a successful GET request?",
                ChoicesJson = """["200", "201", "204", "400"]""",
                AnswerKey = "200",
                Order = 1
            },
            new QuizQuestion
            {
                QuizId = quiz.Id,
                Type = "multiple-choice", 
                Prompt = "Which method is used to define a GET endpoint in Minimal APIs?",
                ChoicesJson = """["app.MapGet", "app.Get", "app.AddGet", "app.UseGet"]""",
                AnswerKey = "app.MapGet",
                Order = 2
            }
        };

        _context.QuizQuestions.AddRange(questions);
        await _context.SaveChangesAsync();
    }

    private async Task SeedProjectsAsync()
    {
        if (!await _context.Projects.AnyAsync())
        {
            var projects = new[]
            {
                new Project
                {
                    Title = "TaskBoard Application",
                    Brief = "Build a task management application using Razor Pages, Entity Framework Core, and CRUD operations. Implement user authentication and task assignment.",
                    RepoUrl = "https://github.com/example/taskboard-starter",
                    Order = 1,
                    RubricJson = """
                    {
                        "criteria": [
                            { "name": "Database Design", "points": 25 },
                            { "name": "CRUD Operations", "points": 25 },
                            { "name": "User Interface", "points": 25 },
                            { "name": "Authentication", "points": 25 }
                        ]
                    }
                    """
                },
                new Project
                {
                    Title = "Northwind API",
                    Brief = "Create a RESTful API for the Northwind database using Minimal APIs, Swagger documentation, and DTO validation.",
                    RepoUrl = "https://github.com/example/northwind-api-starter",
                    Order = 2,
                    RubricJson = """
                    {
                        "criteria": [
                            { "name": "API Design", "points": 30 },
                            { "name": "Data Validation", "points": 25 },
                            { "name": "Documentation", "points": 20 },
                            { "name": "Error Handling", "points": 25 }
                        ]
                    }
                    """
                },
                new Project
                {
                    Title = "Meal Planner Application",
                    Brief = "Build a comprehensive meal planning application with MVC architecture, Entity Framework Core, caching, logging, and comprehensive testing.",
                    RepoUrl = "https://github.com/example/meal-planner-starter",
                    Order = 3,
                    IsCapstone = true,
                    RubricJson = """
                    {
                        "criteria": [
                            { "name": "Architecture", "points": 20 },
                            { "name": "Data Management", "points": 20 },
                            { "name": "User Experience", "points": 20 },
                            { "name": "Performance", "points": 20 },
                            { "name": "Testing", "points": 20 }
                        ]
                    }
                    """
                }
            };

            _context.Projects.AddRange(projects);
            await _context.SaveChangesAsync();
        }
    }
}