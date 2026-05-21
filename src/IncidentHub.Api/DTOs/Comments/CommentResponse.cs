namespace IncidentHub.Api.DTOs.Comments;

public class CommentResponse
{
    public Guid Id { get; set; }
    public Guid IncidentId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
