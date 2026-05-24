namespace IncidentHub.Api.DTOs.Incidents;

public class UserSummaryResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
