namespace LearnCSharp.Core.Services;

public interface ILabRunnerService
{
    Task<LabRunResult> RunLabAsync(int labId, string userCode, string userId);
}

public class LabRunResult
{
    public bool Success { get; set; }
    public int Score { get; set; }
    public string? Output { get; set; }
    public string? ErrorMessage { get; set; }
    public string? TestResults { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}