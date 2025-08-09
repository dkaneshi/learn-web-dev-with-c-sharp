using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Core.Models;

public class Lesson
{
    public int Id { get; set; }
    
    public int TrackId { get; set; }
    public virtual Track Track { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public required string Title { get; set; }
    
    [Required, MaxLength(100)]
    public required string Slug { get; set; }
    
    public int Order { get; set; }
    
    [MaxLength(500)]
    public string? VideoUrl { get; set; }
    
    public string? Transcript { get; set; }
    
    public string? Reading { get; set; }
    
    public int DurationMinutes { get; set; } = 0;
    
    public bool IsPublished { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public virtual ICollection<Progress> Progress { get; set; } = new List<Progress>();
}