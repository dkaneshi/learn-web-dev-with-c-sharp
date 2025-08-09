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

        var nextLesson = await _context.Lessons
            .Include(l => l.Track)
            .Where(l => l.IsPublished)
            .Where(l => inProgressLessonIds.Contains(l.Id) || !completedLessonIds.Contains(l.Id))
            .OrderBy(l => l.Track.Order)
            .ThenBy(l => l.Order)
            .FirstOrDefaultAsync();

        return nextLesson;
    }
}