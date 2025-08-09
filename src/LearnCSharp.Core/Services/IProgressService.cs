using LearnCSharp.Core.Models;

namespace LearnCSharp.Core.Services;

public interface IProgressService
{
    Task<Progress?> GetProgressAsync(string userId, int lessonId);
    Task UpdateProgressAsync(string userId, int lessonId, ProgressStatus status, int score = 0);
    Task<List<Progress>> GetUserProgressAsync(string userId);
    Task<int> GetUserXpAsync(string userId);
    Task AwardXpAsync(string userId, int xp);
    Task<List<Badge>> CheckAndAwardBadgesAsync(string userId);
}