using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Web.Pages;

[Authorize]
public class LessonModel : PageModel
{
    private readonly LearnCSharpDbContext _context;

    public LessonModel(LearnCSharpDbContext context)
    {
        _context = context;
    }

    public Lesson? Lesson { get; set; }
    public Track? Track { get; set; }
    public List<Lab> Labs { get; set; } = new();
    public List<Quiz> Quizzes { get; set; } = new();
    public Lesson? PreviousLesson { get; set; }
    public Lesson? NextLesson { get; set; }

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        Lesson = await _context.Lessons
            .Include(l => l.Track)
            .Include(l => l.Labs.OrderBy(lab => lab.Order))
            .Include(l => l.Quizzes.OrderBy(q => q.Order))
                .ThenInclude(q => q.Questions.OrderBy(qq => qq.Order))
            .FirstOrDefaultAsync(l => l.Slug == slug && l.IsPublished);

        if (Lesson == null)
        {
            return NotFound();
        }

        Track = Lesson.Track;
        Labs = Lesson.Labs.ToList();
        Quizzes = Lesson.Quizzes.ToList();

        // Get previous and next lessons in the track
        var allLessons = await _context.Lessons
            .Where(l => l.TrackId == Lesson.TrackId && l.IsPublished)
            .OrderBy(l => l.Order)
            .ToListAsync();

        var currentIndex = allLessons.FindIndex(l => l.Id == Lesson.Id);
        if (currentIndex > 0)
        {
            PreviousLesson = allLessons[currentIndex - 1];
        }
        if (currentIndex < allLessons.Count - 1)
        {
            NextLesson = allLessons[currentIndex + 1];
        }

        return Page();
    }
}