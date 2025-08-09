using FluentAssertions;
using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using LearnCSharp.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnCSharp.Tests.Services;

public class ProgressServiceTests : IDisposable
{
    private readonly LearnCSharpDbContext _context;
    private readonly Mock<ILogger<ProgressService>> _mockLogger;
    private readonly ProgressService _progressService;

    public ProgressServiceTests()
    {
        var options = new DbContextOptionsBuilder<LearnCSharpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new LearnCSharpDbContext(options);
        _mockLogger = new Mock<ILogger<ProgressService>>();
        _progressService = new ProgressService(_context, _mockLogger.Object);

        SeedTestData();
    }

    [Fact]
    public async Task UpdateProgressAsync_NewProgress_CreatesRecord()
    {
        const string userId = "test-user";
        const int lessonId = 1;

        await _progressService.UpdateProgressAsync(userId, lessonId, ProgressStatus.InProgress, 50);

        var progress = await _context.Progress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

        progress.Should().NotBeNull();
        progress!.Status.Should().Be(ProgressStatus.InProgress);
        progress.Score.Should().Be(50);
    }

    [Fact]
    public async Task UpdateProgressAsync_ExistingProgress_UpdatesRecord()
    {
        const string userId = "test-user";
        const int lessonId = 1;

        await _progressService.UpdateProgressAsync(userId, lessonId, ProgressStatus.InProgress, 50);
        await _progressService.UpdateProgressAsync(userId, lessonId, ProgressStatus.Completed, 100);

        var progress = await _context.Progress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

        progress.Should().NotBeNull();
        progress!.Status.Should().Be(ProgressStatus.Completed);
        progress.Score.Should().Be(100);
    }

    [Fact]
    public async Task UpdateProgressAsync_CompletedStatus_AwardsXp()
    {
        const string userId = "test-user-with-xp";
        var user = new User
        {
            Id = userId,
            UserName = "test@example.com",
            Email = "test@example.com",
            XpPoints = 0
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _progressService.UpdateProgressAsync(userId, 1, ProgressStatus.Completed, 100);

        var updatedUser = await _context.Users.FindAsync(userId);
        updatedUser!.XpPoints.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AwardXpAsync_ValidUser_UpdatesXpAndStreak()
    {
        const string userId = "xp-user";
        var user = new User
        {
            Id = userId,
            UserName = "xp@example.com",
            Email = "xp@example.com",
            XpPoints = 100,
            CurrentStreak = 2,
            LastActivityDate = DateTime.UtcNow.AddDays(-1)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _progressService.AwardXpAsync(userId, 50);

        var updatedUser = await _context.Users.FindAsync(userId);
        updatedUser!.XpPoints.Should().Be(150);
        updatedUser.CurrentStreak.Should().Be(3);
    }

    [Fact]
    public async Task CheckAndAwardBadgesAsync_FirstStepsCriteria_AwardsBadge()
    {
        const string userId = "badge-user";
        var user = new User
        {
            Id = userId,
            UserName = "badge@example.com",
            Email = "badge@example.com"
        };
        _context.Users.Add(user);
        
        var progress = new Progress
        {
            UserId = userId,
            LessonId = 1,
            Status = ProgressStatus.Completed,
            Score = 100
        };
        _context.Progress.Add(progress);
        
        await _context.SaveChangesAsync();

        var newBadges = await _progressService.CheckAndAwardBadgesAsync(userId);

        newBadges.Should().ContainSingle(b => b.Name == "First Steps");
    }

    private void SeedTestData()
    {
        var track = new Track
        {
            Id = 1,
            Title = "Test Track",
            Slug = "test-track",
            Order = 1
        };

        var lesson = new Lesson
        {
            Id = 1,
            TrackId = 1,
            Title = "Test Lesson",
            Slug = "test-lesson",
            Order = 1
        };

        var badges = new[]
        {
            new Badge { Id = 1, Name = "First Steps", Description = "Complete your first lesson", Icon = "🎯" },
            new Badge { Id = 2, Name = "Getting Started", Description = "Complete 5 lessons", Icon = "🚀" },
            new Badge { Id = 3, Name = "Dedicated Learner", Description = "Complete 10 lessons", Icon = "📚" }
        };

        _context.Tracks.Add(track);
        _context.Lessons.Add(lesson);
        _context.Badges.AddRange(badges);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}