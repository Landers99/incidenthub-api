namespace IncidentHub.Api.Common;

public class ApiErrorResponse
{
    public string Error { get; init; } = "";
    public string Message { get; init; } = "";
    public string CorrelationId { get; init; } = "";
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
}
