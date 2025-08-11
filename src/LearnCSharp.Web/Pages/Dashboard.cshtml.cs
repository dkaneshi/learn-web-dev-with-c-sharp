using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using LearnCSharp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Web.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly LearnCSharpDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IProgressService _progressService;

    public DashboardModel(LearnCSharpDbContext context, UserManager<User> userManager, IProgressService progressService)
    {
        _context = context;
        _userManager = userManager;
        _progressService = progressService;
    }

    public int TotalXp { get; set; }
    public int CurrentStreak { get; set; }
    public Lesson? NextLesson { get; set; }
    public List<UserBadge> UserBadges { get; set; } = new();
    public List<Badge> RecentBadges { get; set; } = new();
    public Dictionary<Track, int> TrackProgress { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return;

        var user = await _context.Users
            .Include(u => u.UserBadges)
            .ThenInclude(ub => ub.Badge)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            TotalXp = user.XpPoints;
            CurrentStreak = user.CurrentStreak;
            UserBadges = user.UserBadges.OrderByDescending(ub => ub.AwardedAt).ToList();
            RecentBadges = UserBadges
                .Where(ub => ub.AwardedAt > DateTime.UtcNow.AddDays(-7))
                .Select(ub => ub.Badge)
                .ToList();
        }

        var userProgress = await _progressService.GetUserProgressAsync(userId);
        var completedLessonIds = userProgress
            .Where(p => p.Status == ProgressStatus.Completed)
            .Select(p => p.LessonId)
            .ToHashSet();

        var tracks = await _context.Tracks
            .Include(t => t.Lessons)
            .Where(t => t.IsPublished)
            .OrderBy(t => t.Order)
            .ToListAsync();

        foreach (var track in tracks)
        {
            var completedCount = track.Lessons.Count(l => completedLessonIds.Contains(l.Id));
            TrackProgress[track] = completedCount;
        }

        NextLesson = await GetNextLessonAsync(userId, userProgress);
    }

    private async Task<Lesson?> GetNextLessonAsync(string userId, List<Progress> userProgress)
    {
        var completedLessonIds = userProgress
            .Where(p => p.Status == ProgressStatus.Completed)
            .Select(p => p.LessonId)
            .ToHashSet();

        var inProgressLessonIds = userProgress
            .Where(p => p.Status == ProgressStatus.InProgress)
            .Select(p => p.LessonId)
            .ToHashSet();

        // First priority: Return any lesson that's currently in progress
        if (inProgressLessonIds.Any())
        {
            var inProgressLesson = await _context.Lessons
                .Include(l => l.Track)
                .Where(l => l.IsPublished && inProgressLessonIds.Contains(l.Id))
                .OrderBy(l => l.Track.Order)
                .ThenBy(l => l.Order)
                .FirstOrDefaultAsync();
            
            if (inProgressLesson != null)
                return inProgressLesson;
        }

        // Get all lesson IDs where user has progress (completed or in-progress)
        var userProgressLessonIds = userProgress.Select(p => p.LessonId).ToHashSet();

        // Second priority: Find the next uncompleted lesson in a track where user has made progress
        if (userProgressLessonIds.Any())
        {
            // Get tracks that contain lessons where user has progress
            var tracksWithProgress = await _context.Tracks
                .Include(t => t.Lessons)
                .Where(t => t.IsPublished)
                .OrderBy(t => t.Order)
                .ToListAsync();

            // Filter tracks that have user progress
            var relevantTracks = tracksWithProgress
                .Where(t => t.Lessons.Any(l => userProgressLessonIds.Contains(l.Id)))
                .ToList();

            foreach (var track in relevantTracks)
            {
                var nextLessonInTrack = track.Lessons
                    .Where(l => l.IsPublished && !completedLessonIds.Contains(l.Id))
                    .OrderBy(l => l.Order)
                    .FirstOrDefault();
                
                if (nextLessonInTrack != null)
                {
                    // Load the track reference for the lesson
                    nextLessonInTrack.Track = track;
                    return nextLessonInTrack;
                }
            }
        }

        // Third priority: Find the first lesson of the first track (new user scenario)
        var firstTrack = await _context.Tracks
            .Include(t => t.Lessons)
            .Where(t => t.IsPublished)
            .OrderBy(t => t.Order)
            .FirstOrDefaultAsync();

        if (firstTrack != null)
        {
            var firstLesson = firstTrack.Lessons
                .Where(l => l.IsPublished)
                .OrderBy(l => l.Order)
                .FirstOrDefault();
            
            if (firstLesson != null)
            {
                firstLesson.Track = firstTrack;
                return firstLesson;
            }
        }

        return null;
    }
}