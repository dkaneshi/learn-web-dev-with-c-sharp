using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnCSharp.Core.Services;

public class ProgressService : IProgressService
{
    private readonly LearnCSharpDbContext _context;
    private readonly ILogger<ProgressService> _logger;

    public ProgressService(LearnCSharpDbContext context, ILogger<ProgressService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Progress?> GetProgressAsync(string userId, int lessonId)
    {
        return await _context.Progress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);
    }

    public async Task UpdateProgressAsync(string userId, int lessonId, ProgressStatus status, int score = 0)
    {
        var progress = await GetProgressAsync(userId, lessonId);
        
        if (progress == null)
        {
            progress = new Progress
            {
                UserId = userId,
                LessonId = lessonId,
                Status = status,
                Score = score,
                LastAttemptAt = DateTime.UtcNow
            };
            _context.Progress.Add(progress);
        }
        else
        {
            progress.Status = status;
            if (score > progress.Score)
            {
                progress.Score = score;
            }
            progress.LastAttemptAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        
        if (status == ProgressStatus.Completed)
        {
            await AwardXpAsync(userId, 10 + score / 10);
        }
    }

    public async Task<List<Progress>> GetUserProgressAsync(string userId)
    {
        return await _context.Progress
            .Include(p => p.Lesson)
            .ThenInclude(l => l.Track)
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Lesson.Track.Order)
            .ThenBy(p => p.Lesson.Order)
            .ToListAsync();
    }

    public async Task<int> GetUserXpAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.XpPoints ?? 0;
    }

    public async Task AwardXpAsync(string userId, int xp)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.XpPoints += xp;
            user.LastActivityDate = DateTime.UtcNow;
            
            var today = DateTime.UtcNow.Date;
            var lastActivity = user.LastActivityDate?.Date;
            
            if (lastActivity == today.AddDays(-1))
            {
                user.CurrentStreak++;
            }
            else if (lastActivity != today)
            {
                user.CurrentStreak = 1;
            }
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Badge>> CheckAndAwardBadgesAsync(string userId)
    {
        var newBadges = new List<Badge>();
        
        var user = await _context.Users
            .Include(u => u.UserBadges)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if (user == null) return newBadges;
        
        var earnedBadgeIds = user.UserBadges.Select(ub => ub.BadgeId).ToList();
        var availableBadges = await _context.Badges
            .Where(b => !earnedBadgeIds.Contains(b.Id))
            .ToListAsync();
        
        foreach (var badge in availableBadges)
        {
            if (await CheckBadgeCriteriaAsync(userId, badge))
            {
                var userBadge = new UserBadge
                {
                    UserId = userId,
                    BadgeId = badge.Id,
                    AwardedAt = DateTime.UtcNow
                };
                
                _context.UserBadges.Add(userBadge);
                newBadges.Add(badge);
            }
        }
        
        await _context.SaveChangesAsync();
        return newBadges;
    }

    private async Task<bool> CheckBadgeCriteriaAsync(string userId, Badge badge)
    {
        return badge.Name switch
        {
            "First Steps" => await HasCompletedLessonsAsync(userId, 1),
            "Getting Started" => await HasCompletedLessonsAsync(userId, 5),
            "Dedicated Learner" => await HasCompletedLessonsAsync(userId, 10),
            "Track Finisher" => await HasCompletedTrackAsync(userId),
            "Lab Master" => await HasCompletedLabsAsync(userId, 10),
            "Perfect Score" => await HasPerfectLabScoreAsync(userId),
            "Streak Master" => await HasStreakAsync(userId, 7),
            _ => false
        };
    }

    private async Task<bool> HasCompletedLessonsAsync(string userId, int count)
    {
        var completedCount = await _context.Progress
            .CountAsync(p => p.UserId == userId && p.Status == ProgressStatus.Completed);
        return completedCount >= count;
    }

    private async Task<bool> HasCompletedTrackAsync(string userId)
    {
        var tracks = await _context.Tracks.Include(t => t.Lessons).ToListAsync();
        
        foreach (var track in tracks)
        {
            var completedInTrack = await _context.Progress
                .CountAsync(p => p.UserId == userId && 
                                p.Status == ProgressStatus.Completed &&
                                track.Lessons.Select(l => l.Id).Contains(p.LessonId));
                                
            if (completedInTrack == track.Lessons.Count && track.Lessons.Count > 0)
            {
                return true;
            }
        }
        
        return false;
    }

    private async Task<bool> HasCompletedLabsAsync(string userId, int count)
    {
        var completedCount = await _context.LabSubmissions
            .CountAsync(ls => ls.UserId == userId && ls.Passed);
        return completedCount >= count;
    }

    private async Task<bool> HasPerfectLabScoreAsync(string userId)
    {
        return await _context.LabSubmissions
            .AnyAsync(ls => ls.UserId == userId && ls.Score == 100);
    }

    private async Task<bool> HasStreakAsync(string userId, int days)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.CurrentStreak >= days;
    }
}