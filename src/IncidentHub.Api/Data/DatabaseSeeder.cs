using IncidentHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentHub.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var admin = new User
        {
            Email = "admin@example.com",
            PasswordHash = "placeholder_hash",
            Role = UserRole.Admin
        };

        var alex = new User
        {
            Email = "alex@example.com",
            PasswordHash = "placeholder_hash",
            Role = UserRole.User
        };

        var sam = new User
        {
            Email = "sam@example.com",
            PasswordHash = "placeholder_hash",
            Role = UserRole.User
        };

        db.Users.AddRange(admin, alex, sam);

        var incident1 = new Incident
        {
            Title = "Login endpoint returning 401",
            Description = "Users report being unable to access protected routes after logging in.",
            Priority = IncidentPriority.High,
            Status = IncidentStatus.Open,
            CreatedByUser = alex,
            AssignedToUser = admin
        };


        var incident2 = new Incident
        {
            Title = "Incident list endpoint is slow",
            Description = "The incident list takes over one second to return under load.",
            Priority = IncidentPriority.Medium,
            Status = IncidentStatus.InProgress,
            CreatedByUser = sam,
            AssignedToUser = null
        };

        db.Incidents.AddRange(incident1, incident2);

        db.Comments.AddRange(
                new Comment
                {
                    Incident = incident1,
                    User = alex,
                    Body = "This started happending after the last deployment."
                },
                new Comment
                {
                    Incident = incident2,
                    User = sam,
                    Body = "The endpoint slows down when filters are applied."
                }
            );

        await db.SaveChangesAsync();
    }
}
