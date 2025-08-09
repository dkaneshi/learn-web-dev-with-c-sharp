using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Core.Models;

public class Project
{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public required string Title { get; set; }
    
    [MaxLength(500)]
    public string? RepoUrl { get; set; }
    
    public required string Brief { get; set; }
    
    public string? RubricJson { get; set; }
    
    public int Order { get; set; }
    
    public bool IsCapstone { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}