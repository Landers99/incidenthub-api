using System.Diagnostics;
using BCrypt.Net;
using IncidentHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentHub.Api.Data.Seeders;

public class LargeDatasetSeeder
{
    private readonly AppDbContext _db;
    private readonly ILogger<LargeDatasetSeeder> _logger;

    public LargeDatasetSeeder(
            AppDbContext db,
            ILogger<LargeDatasetSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<LargeDatasetSeedResult> SeedAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var startedAt = DateTime.UtcNow;

        _logger.LogInformation("Starting large dataset seed");

        await ClearGeneratedDataAsync();

        var users = CreateUsers(100);
        await AddInBatchesAsync(users, 100);

        var incidents = CreateIncidents(users, 5000);
        await AddInBatchesAsync(incidents, 500);

        var comments = CreateComments(users, incidents, 20000);
        await AddInBatchesAsync(comments, 1000);

        stopwatch.Stop();

        _logger.LogInformation(
            "Large dataset seed complete. Users={Users}, Incidents={Incidents}, Comments={Comments}, ElapsedMs={ElapsedMs}",
            users.Count,
            incidents.Count,
            comments.Count,
            stopwatch.ElapsedMilliseconds);

        return new LargeDatasetSeedResult
        {
            UsersCreated = users.Count,
            IncidentsCreated = incidents.Count,
            CommentsCreated = comments.Count,
            StartedAtUtc = startedAt,
            FinishedAtUtc = DateTime.UtcNow,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
        };
    }

    private async Task ClearGeneratedDataAsync()
    {
        _logger.LogInformation("Clearing generated seed data");

        await _db.Comments.ExecuteDeleteAsync();
        await _db.Incidents.ExecuteDeleteAsync();

        var generatedUsers = await _db.Users
            .Where(u => u.Email.EndsWith("@seed.local"))
            .ToListAsync();

        _db.Users.RemoveRange(generatedUsers);

        await _db.SaveChangesAsync();
    }

    private List<User> CreateUsers(int count)
    {
        var users = new List<User>();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Pasword123!");

        users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@seed.locala",
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            CreatedAtUtc = DateTime.UtcNow
        });

        for (var i = 1; i < count; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = $"user{i:D3}@seed.local",
                PasswordHash = passwordHash,
                Role = UserRole.User,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365))
            });
        }

        return users;
    }

    private static readonly string[] IncidentTitles =
    [
        "Login API returning 401",
        "Dashboard loads slowly",
        "Payment webhook not processing",
        "User profile update fails",
        "Search endpoint timeout",
        "Database connection error",
        "File upload fails intermittently",
        "Background job not running",
        "Unexpected 500 from API",
        "Email notifaction not sent"
    ];

    private static readonly string[] IncidentDescriptions =
    [
        "Users reported that this issue started after thet latest deployment.",
        "The issue appears intermittent and affects only some requests.",
        "Initial logs show errors coming from the backend service.",
        "Customer support escalated this after multiple reports.",
        "The problem is reproducible in the staging environment."
    ];

    private List<Incident> CreateIncidents(List<User> users, int count)
    {
        var incidents = new List<Incident>();

        var normalUsers = users.Where(u => u.Role == UserRole.User).ToList();

        for (var i = 0; i < count; i++)
        {
            var createdBy = normalUsers[Random.Shared.Next(normalUsers.Count)];

            var assignedTo = Random.Shared.NextDouble() < 0.75
                ? normalUsers[Random.Shared.Next(normalUsers.Count)]
                : null;

            var createdAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 180))
                .AddMinutes(-Random.Shared.Next(0, 1440));

            var status = GetWeightedStatus();

            incidents.Add(new Incident
            {
                Id = Guid.NewGuid(),
                Title = $"{IncidentTitles[Random.Shared.Next(IncidentTitles.Length)]} #{i + 1}",
                Description = IncidentDescriptions[Random.Shared.Next(IncidentDescriptions.Length)],
                Status = status,
                Priority = GetWeightedPriority(),
                CreatedByUserId = createdBy.Id,
                AssignedToUserId = assignedTo?.Id,
                CreatedAtUtc = createdAt,
                UpdatedAtUtc = createdAt.AddHours(Random.Shared.Next(1, 72)),
                ResolvedAtUtc = status is IncidentStatus.Resolved or IncidentStatus.Closed
                    ? createdAt.AddDays(Random.Shared.Next(1, 14))
                    : null
            });
        }

        return incidents;
    }

    private IncidentStatus GetWeightedStatus()
    {
        var roll = Random.Shared.NextDouble();

        return roll switch
        {
            < 0.40 => IncidentStatus.Open,
            < 0.65 => IncidentStatus.InProgress,
            < 0.90 => IncidentStatus.Resolved,
            _ => IncidentStatus.Closed
        };
    }

    private IncidentPriority GetWeightedPriority()
    {
        var roll = Random.Shared.NextDouble();

        return roll switch
        {
            < 0.35 => IncidentPriority.Low,
            < 0.70 => IncidentPriority.Medium,
            < 0.92 => IncidentPriority.High,
            _ => IncidentPriority.Critical
        };
    }

    private static readonly string[] CommentBodies =
    [
        "I was able to reproduce this issue locally.",
        "Checking logs for the affected request path.",
        "This appears related to the latest deployment.",
        "Added more diagnostic logging to narrow this down.",
        "The issue seems isolated to a specific user flow.",
        "Database query timing looks suspicious.",
        "Auth middleware may be rejecting the request.",
        "This should be retested after the fix is deployed."
    ];

    private List<Comment> CreateComments(
            List<User> users,
            List<Incident> incidents,
            int count)
    {
        var comments = new List<Comment>();

        for (var i = 0; i < count; i++)
        {
            var incident = incidents[Random.Shared.Next(incidents.Count)];
            var user = users[Random.Shared.Next(users.Count)];

            comments.Add(new Comment
            {
                Id = Guid.NewGuid(),
                IncidentId = incident.Id,
                UserId = user.Id,
                Body = CommentBodies[Random.Shared.Next(CommentBodies.Length)],
                CreatedAtUtc = incident.CreatedAtUtc.AddMinutes(Random.Shared.Next(5, 10080))
            });
        }

        return comments;
    }

    private async Task AddInBatchesAsync<T>(List<T> entities, int batchSize)
        where T : class
    {
        for (var i = 0; i < entities.Count; i += batchSize)
        {
            var batch = entities.Skip(i).Take(batchSize).ToList();

            await _db.Set<T>().AddRangeAsync(batch);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Inserted batch {Start}-{End} of {Total} for {EntityType}",
                i + 1,
                Math.Min(i + batchSize, entities.Count),
                entities.Count,
                typeof(T).Name);
        }
    }
}
