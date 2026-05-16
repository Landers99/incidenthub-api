namespace IncidentHub.Api.Models;

public class Incident
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public IncidentPriority Priority { get; set; } = IncidentPriority.Medium;
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public List<Comment> Comments { get; set; } = new();
}
