namespace IncidentHub.Api.Models;

public enum UserRole
{
    User = 1,
    Admin = 2
}

public enum IncidentStatus
{
    Open = 1,
    InProgress = 2,
    Resolved = 3,
    Closed = 4
}

public enum IncidentPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}
