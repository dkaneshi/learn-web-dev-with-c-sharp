using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Core.Models;

public class Lab
{
    public int Id { get; set; }
    
    public int LessonId { get; set; }
    public virtual Lesson Lesson { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public required string Title { get; set; }
    
    public required string Prompt { get; set; }
    
    [MaxLength(500)]
    public string? StarterZipPath { get; set; }
    
    [MaxLength(500)]
    public string? TestsZipPath { get; set; }
    
    public int MaxScore { get; set; } = 100;
    
    public int Order { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<LabSubmission> Submissions { get; set; } = new List<LabSubmission>();
}