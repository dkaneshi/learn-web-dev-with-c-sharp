using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Core.Models;

public class Track
{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public required string Title { get; set; }
    
    [Required, MaxLength(100)]
    public required string Slug { get; set; }
    
    public int Order { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public bool IsPublished { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}