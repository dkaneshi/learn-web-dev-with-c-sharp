using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Web.Pages;

public class TrackModel : PageModel
{
    private readonly LearnCSharpDbContext _context;

    public TrackModel(LearnCSharpDbContext context)
    {
        _context = context;
    }

    public Track? Track { get; set; }
    public List<Lesson> Lessons { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        Track = await _context.Tracks
            .Include(t => t.Lessons.OrderBy(l => l.Order))
            .FirstOrDefaultAsync(t => t.Slug == slug && t.IsPublished);

        if (Track == null)
        {
            return NotFound();
        }

        Lessons = Track.Lessons.ToList();

        return Page();
    }
}