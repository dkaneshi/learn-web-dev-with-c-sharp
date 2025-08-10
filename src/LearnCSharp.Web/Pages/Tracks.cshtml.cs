using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Web.Pages;

public class TracksModel : PageModel
{
    private readonly LearnCSharpDbContext _context;

    public TracksModel(LearnCSharpDbContext context)
    {
        _context = context;
    }

    public List<Track> Tracks { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tracks = await _context.Tracks
            .Include(t => t.Lessons)
            .Where(t => t.IsPublished)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }
}