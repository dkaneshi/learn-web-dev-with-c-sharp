using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnCSharp.Core.Services;

public class LabRunnerService : ILabRunnerService
{
    private readonly LearnCSharpDbContext _context;
    private readonly ILogger<LabRunnerService> _logger;
    private readonly string _workspaceRoot;

    public LabRunnerService(LearnCSharpDbContext context, ILogger<LabRunnerService> logger)
    {
        _context = context;
        _logger = logger;
        _workspaceRoot = Path.Combine(Path.GetTempPath(), "learncsharp-labs");
        
        if (!Directory.Exists(_workspaceRoot))
        {
            Directory.CreateDirectory(_workspaceRoot);
        }
    }

    public async Task<LabRunResult> RunLabAsync(int labId, string userCode, string userId)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var lab = await _context.Labs.FindAsync(labId);
            if (lab == null)
            {
                return new LabRunResult
                {
                    Success = false,
                    ErrorMessage = "Lab not found"
                };
            }

            var workspaceId = Guid.NewGuid().ToString();
            var workspacePath = Path.Combine(_workspaceRoot, workspaceId);
            
            try
            {
                Directory.CreateDirectory(workspacePath);
                
                if (!string.IsNullOrEmpty(lab.StarterZipPath) && File.Exists(lab.StarterZipPath))
                {
                    ZipFile.ExtractToDirectory(lab.StarterZipPath, workspacePath);
                }
                
                await File.WriteAllTextAsync(Path.Combine(workspacePath, "UserCode.cs"), userCode);
                
                var testResults = await RunTestsAsync(workspacePath, lab.TestsZipPath);
                
                var score = CalculateScore(testResults, lab.MaxScore);
                
                var submission = new LabSubmission
                {
                    LabId = labId,
                    UserId = userId,
                    Code = userCode,
                    Score = score,
                    Passed = testResults.Success,
                    TestResults = testResults.Output,
                    ErrorMessage = testResults.ErrorMessage,
                    SubmittedAt = DateTime.UtcNow
                };
                
                _context.LabSubmissions.Add(submission);
                await _context.SaveChangesAsync();
                
                return new LabRunResult
                {
                    Success = testResults.Success,
                    Score = score,
                    Output = testResults.Output,
                    ErrorMessage = testResults.ErrorMessage,
                    TestResults = testResults.TestResults,
                    ExecutionTime = stopwatch.Elapsed
                };
            }
            finally
            {
                if (Directory.Exists(workspacePath))
                {
                    Directory.Delete(workspacePath, true);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running lab {LabId} for user {UserId}", labId, userId);
            return new LabRunResult
            {
                Success = false,
                ErrorMessage = "An error occurred while running the lab",
                ExecutionTime = stopwatch.Elapsed
            };
        }
    }

    private async Task<TestExecutionResult> RunTestsAsync(string workspacePath, string? testsZipPath)
    {
        try
        {
            if (!string.IsNullOrEmpty(testsZipPath) && File.Exists(testsZipPath))
            {
                ZipFile.ExtractToDirectory(testsZipPath, workspacePath, true);
            }
            
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "test --no-build --logger:json",
                    WorkingDirectory = workspacePath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, args) => {
                if (args.Data != null) outputBuilder.AppendLine(args.Data);
            };
            
            process.ErrorDataReceived += (sender, args) => {
                if (args.Data != null) errorBuilder.AppendLine(args.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var completed = await process.WaitForExitAsync(TimeSpan.FromSeconds(30));
            
            if (!completed)
            {
                process.Kill();
                return new TestExecutionResult
                {
                    Success = false,
                    ErrorMessage = "Test execution timed out",
                    Output = outputBuilder.ToString()
                };
            }

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();
            
            return new TestExecutionResult
            {
                Success = process.ExitCode == 0,
                Output = output,
                ErrorMessage = process.ExitCode != 0 ? error : null,
                TestResults = output
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tests in workspace {WorkspacePath}", workspacePath);
            return new TestExecutionResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static int CalculateScore(TestExecutionResult testResult, int maxScore)
    {
        if (!testResult.Success || string.IsNullOrEmpty(testResult.Output))
            return 0;
            
        var passedTests = testResult.Output.Split('\n')
            .Count(line => line.Contains("Passed"));
            
        var totalTests = testResult.Output.Split('\n')
            .Count(line => line.Contains("Passed") || line.Contains("Failed"));
            
        if (totalTests == 0) return 0;
        
        return (int)((double)passedTests / totalTests * maxScore);
    }

    private class TestExecutionResult
    {
        public bool Success { get; set; }
        public string? Output { get; set; }
        public string? ErrorMessage { get; set; }
        public string? TestResults { get; set; }
    }
}

public static class ProcessExtensions
{
    public static async Task<bool> WaitForExitAsync(this Process process, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        var tcs = new TaskCompletionSource<bool>();
        
        void ProcessExited(object sender, EventArgs e) => tcs.TrySetResult(true);
        
        process.EnableRaisingEvents = true;
        process.Exited += ProcessExited;
        
        if (process.HasExited)
        {
            return true;
        }
        
        using (cancellationTokenSource.Token.Register(() => tcs.TrySetResult(false)))
        {
            return await tcs.Task;
        }
    }
}