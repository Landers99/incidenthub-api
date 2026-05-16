namespace IncidentHub.Api.Models;

public class User
{
   public Guid Id { get; set; } = Guid.NewGuid();
   public string Email { get; set; } = string.Empty;
   public string PasswordHash { get; set; } = string.Empty;
   public UserRole Role { get; set; } = UserRole.User;
   public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
   public List<Incident> CreatedIncidents { get; set; } = new();
   public List<Incident> AssignedIncidents { get; set; } = new();
   public List<Comment> Comments { get; set; } = new();
}
