using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Core.Models;

public class LabSubmission
{
    public int Id { get; set; }
    
    public int LabId { get; set; }
    public virtual Lab Lab { get; set; } = null!;
    
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    
    public required string Code { get; set; }
    
    public int Score { get; set; } = 0;
    
    public bool Passed { get; set; } = false;
    
    public string? TestResults { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}