namespace LearnCSharp.Core.Models;

public class Progress
{
    public int Id { get; set; }
    
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    
    public int LessonId { get; set; }
    public virtual Lesson Lesson { get; set; } = null!;
    
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    
    public int Score { get; set; } = 0;
    
    public DateTime? LastAttemptAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ProgressStatus
{
    NotStarted,
    InProgress, 
    Completed
}