using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Core.Models;

public class Badge
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public required string Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? Icon { get; set; }
    
    public string? CriteriaJson { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}

public class UserBadge
{
    public int Id { get; set; }
    
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    
    public int BadgeId { get; set; }
    public virtual Badge Badge { get; set; } = null!;
    
    public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
}