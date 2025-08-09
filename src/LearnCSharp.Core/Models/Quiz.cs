using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Core.Models;

public class Quiz
{
    public int Id { get; set; }
    
    public int LessonId { get; set; }
    public virtual Lesson Lesson { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public required string Title { get; set; }
    
    public int Order { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
}

public class QuizQuestion
{
    public int Id { get; set; }
    
    public int QuizId { get; set; }
    public virtual Quiz Quiz { get; set; } = null!;
    
    [Required]
    public required string Type { get; set; }
    
    [Required]
    public required string Prompt { get; set; }
    
    public string? ChoicesJson { get; set; }
    
    [Required]
    public required string AnswerKey { get; set; }
    
    public int Order { get; set; }
}