using FluentAssertions;
using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using LearnCSharp.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnCSharp.Tests.Services;

public class LabRunnerServiceTests : IDisposable
{
    private readonly LearnCSharpDbContext _context;
    private readonly Mock<ILogger<LabRunnerService>> _mockLogger;
    private readonly LabRunnerService _labRunnerService;

    public LabRunnerServiceTests()
    {
        var options = new DbContextOptionsBuilder<LearnCSharpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new LearnCSharpDbContext(options);
        _mockLogger = new Mock<ILogger<LabRunnerService>>();
        _labRunnerService = new LabRunnerService(_context, _mockLogger.Object);

        SeedTestData();
    }

    [Fact]
    public async Task RunLabAsync_WhenLabNotFound_ReturnsFailure()
    {
        var result = await _labRunnerService.RunLabAsync(999, "some code", "user1");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Lab not found");
    }

    [Fact]
    public async Task RunLabAsync_WithValidCode_CreatesSubmission()
    {
        const string userCode = "Console.WriteLine(\"Hello World\");";
        const string userId = "test-user-id";

        var result = await _labRunnerService.RunLabAsync(1, userCode, userId);

        var submission = await _context.LabSubmissions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.LabId == 1);

        submission.Should().NotBeNull();
        submission!.Code.Should().Be(userCode);
        submission.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task RunLabAsync_WithInvalidCode_ReturnsError()
    {
        const string invalidCode = "This is not valid C# code!!!";
        const string userId = "test-user-id";

        var result = await _labRunnerService.RunLabAsync(1, invalidCode, userId);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    private void SeedTestData()
    {
        var track = new Track
        {
            Id = 1,
            Title = "Test Track",
            Slug = "test-track",
            Order = 1,
            Description = "Test track for unit testing"
        };

        var lesson = new Lesson
        {
            Id = 1,
            TrackId = 1,
            Title = "Test Lesson",
            Slug = "test-lesson",
            Order = 1,
            Reading = "Test lesson content"
        };

        var lab = new Lab
        {
            Id = 1,
            LessonId = 1,
            Title = "Test Lab",
            Prompt = "Write a simple Hello World program",
            MaxScore = 100,
            Order = 1
        };

        _context.Tracks.Add(track);
        _context.Lessons.Add(lesson);
        _context.Labs.Add(lab);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}