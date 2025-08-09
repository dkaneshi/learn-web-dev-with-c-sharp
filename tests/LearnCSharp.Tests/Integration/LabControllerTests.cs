using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using LearnCSharp.Core.Data;
using LearnCSharp.Core.Models;
using LearnCSharp.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LearnCSharp.Tests.Integration;

public class LabControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly LearnCSharpDbContext _context;
    private readonly UserManager<User> _userManager;

    public LabControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LearnCSharpDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<LearnCSharpDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<LearnCSharpDbContext>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        SeedTestDataAsync().Wait();
    }

    [Fact]
    public async Task GetLab_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/lab/1");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLab_WhenAuthenticated_ReturnsLab()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/lab/1");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var labResponse = await response.Content.ReadFromJsonAsync<LabResponse>();
        labResponse.Should().NotBeNull();
        labResponse!.Title.Should().Be("Test Lab");
    }

    [Fact]
    public async Task GetLab_WhenLabNotFound_ReturnsNotFound()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/lab/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SubmitLab_WhenAuthenticated_ProcessesSubmission()
    {
        await AuthenticateAsync();

        var request = new SubmitLabRequest { Code = "Console.WriteLine(\"Hello World\");" };
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/lab/1/submit", content);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var labRunResponse = await response.Content.ReadFromJsonAsync<LabRunResponse>();
        labRunResponse.Should().NotBeNull();
        labRunResponse!.Score.Should().BeGreaterOrEqualTo(0);
    }

    private async Task SeedTestDataAsync()
    {
        var track = new Track
        {
            Id = 1,
            Title = "Test Track",
            Slug = "test-track",
            Order = 1,
            Description = "Test track"
        };

        var lesson = new Lesson
        {
            Id = 1,
            TrackId = 1,
            Title = "Test Lesson",
            Slug = "test-lesson",
            Order = 1,
            Reading = "Test lesson content"
        };

        var lab = new Lab
        {
            Id = 1,
            LessonId = 1,
            Title = "Test Lab",
            Prompt = "Write a Hello World program",
            MaxScore = 100,
            Order = 1
        };

        _context.Tracks.Add(track);
        _context.Lessons.Add(lesson);
        _context.Labs.Add(lab);
        await _context.SaveChangesAsync();

        var user = new User
        {
            UserName = "test@example.com",
            Email = "test@example.com",
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, "Test123!");
        await _userManager.AddToRoleAsync(user, "Learner");
    }

    private async Task AuthenticateAsync()
    {
        var loginData = new
        {
            Email = "test@example.com",
            Password = "Test123!",
            RememberMe = false
        };

        var loginContent = new StringContent(
            JsonConvert.SerializeObject(loginData),
            Encoding.UTF8,
            "application/json");

        await _client.PostAsync("/Auth/Login", loginContent);
    }

    public void Dispose()
    {
        _scope?.Dispose();
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}