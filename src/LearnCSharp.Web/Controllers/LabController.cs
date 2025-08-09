using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using LearnCSharp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabController : ControllerBase
{
    private readonly LearnCSharpDbContext _context;
    private readonly ILabRunnerService _labRunnerService;
    private readonly IProgressService _progressService;
    private readonly UserManager<User> _userManager;

    public LabController(
        LearnCSharpDbContext context,
        ILabRunnerService labRunnerService,
        IProgressService progressService,
        UserManager<User> userManager)
    {
        _context = context;
        _labRunnerService = labRunnerService;
        _progressService = progressService;
        _userManager = userManager;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LabResponse>> GetLab(int id)
    {
        var lab = await _context.Labs
            .Include(l => l.Lesson)
            .ThenInclude(l => l.Track)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lab == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var lastSubmission = await _context.LabSubmissions
            .Where(ls => ls.LabId == id && ls.UserId == userId)
            .OrderByDescending(ls => ls.SubmittedAt)
            .FirstOrDefaultAsync();

        return new LabResponse
        {
            Id = lab.Id,
            Title = lab.Title,
            Prompt = lab.Prompt,
            MaxScore = lab.MaxScore,
            LessonTitle = lab.Lesson.Title,
            TrackTitle = lab.Lesson.Track.Title,
            LastSubmission = lastSubmission != null ? new LabSubmissionResponse
            {
                Code = lastSubmission.Code,
                Score = lastSubmission.Score,
                Passed = lastSubmission.Passed,
                TestResults = lastSubmission.TestResults,
                ErrorMessage = lastSubmission.ErrorMessage,
                SubmittedAt = lastSubmission.SubmittedAt
            } : null
        };
    }

    [HttpPost("{id}/submit")]
    public async Task<ActionResult<LabRunResponse>> SubmitLab(int id, [FromBody] SubmitLabRequest request)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var lab = await _context.Labs.FindAsync(id);
        if (lab == null)
        {
            return NotFound();
        }

        var result = await _labRunnerService.RunLabAsync(id, request.Code, userId);

        if (result.Success)
        {
            await _progressService.UpdateProgressAsync(
                userId, 
                lab.LessonId, 
                ProgressStatus.InProgress, 
                result.Score);
                
            if (result.Score >= lab.MaxScore * 0.8)
            {
                await _progressService.UpdateProgressAsync(
                    userId, 
                    lab.LessonId, 
                    ProgressStatus.Completed, 
                    result.Score);
            }
        }

        return new LabRunResponse
        {
            Success = result.Success,
            Score = result.Score,
            Output = result.Output,
            ErrorMessage = result.ErrorMessage,
            TestResults = result.TestResults,
            ExecutionTimeMs = (int)result.ExecutionTime.TotalMilliseconds
        };
    }
}

public class LabResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Prompt { get; set; }
    public int MaxScore { get; set; }
    public required string LessonTitle { get; set; }
    public required string TrackTitle { get; set; }
    public LabSubmissionResponse? LastSubmission { get; set; }
}

public class LabSubmissionResponse
{
    public required string Code { get; set; }
    public int Score { get; set; }
    public bool Passed { get; set; }
    public string? TestResults { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class SubmitLabRequest
{
    public required string Code { get; set; }
}

public class LabRunResponse
{
    public bool Success { get; set; }
    public int Score { get; set; }
    public string? Output { get; set; }
    public string? ErrorMessage { get; set; }
    public string? TestResults { get; set; }
    public int ExecutionTimeMs { get; set; }
}