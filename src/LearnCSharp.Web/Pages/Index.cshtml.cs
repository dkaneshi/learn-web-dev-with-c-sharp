using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Web.Pages;

public class IndexModel : PageModel
{
    private readonly LearnCSharpDbContext _context;

    public IndexModel(LearnCSharpDbContext context)
    {
        _context = context;
    }

    public List<Track> FeaturedTracks { get; set; } = new();

    public async Task OnGetAsync()
    {
        FeaturedTracks = await _context.Tracks
            .Include(t => t.Lessons)
            .Where(t => t.IsPublished)
            .OrderBy(t => t.Order)
            .Take(4)
            .ToListAsync();
    }
}