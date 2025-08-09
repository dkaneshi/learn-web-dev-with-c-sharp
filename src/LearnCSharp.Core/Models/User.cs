using Microsoft.AspNetCore.Identity;

namespace LearnCSharp.Core.Models;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int XpPoints { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;
    public DateTime? LastActivityDate { get; set; }
    
    public virtual ICollection<Progress> Progress { get; set; } = new List<Progress>();
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    public virtual ICollection<LabSubmission> LabSubmissions { get; set; } = new List<LabSubmission>();
}