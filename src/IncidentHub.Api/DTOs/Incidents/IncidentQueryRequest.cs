namespace IncidentHub.Api.DTOs.Incidents;

public class IncidentQueryRequest
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? Search { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string SortBy { get; set; } = "createdAt";
    public string SortDirection { get; set; } = "desc";
}
