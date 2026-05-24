using IncidentHub.Api.DTOs.Incidents;
using IncidentHub.Api.DTOs.Common;
using IncidentHub.Api.Repository;
using IncidentHub.Api.Models;

namespace IncidentHub.Api.Services;

public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository;

    public IncidentService(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<List<IncidentSummaryResponse>> GetAllAsync()
    {
        var incidents = await _incidentRepository.GetAllAsync();

        return incidents.Select(ToSummaryResponse).ToList();
    }

    public async Task<IncidentResponse?> GetByIdAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        return incident is null ? null : ToResponse(incident);
    }

    public async Task<PagedResponse<IncidentListItemResponse>> GetPagedAsync(IncidentQueryRequest query)
    {
        ValidateQuery(query);

        return await _incidentRepository.GetPagedAsync(query);
    }

    public async Task<IncidentResponse> CreateAsync(CreateIncidentRequest request, Guid userId)
    {
        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Status = IncidentStatus.Open,
            AssignedToUserId = request.AssignedToUserId,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await _incidentRepository.AddAsync(incident);

        return ToResponse(incident);
    }

    public async Task<IncidentResponse?> UpdateAsync(Guid id, UpdateIncidentRequest request)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
        {
            return null;
        }

        incident.Title = request.Title.Trim();
        incident.Description = request.Description.Trim();
        incident.Priority = request.Priority;
        incident.Status = request.Status;
        incident.AssignedToUserId = request.AssignedToUserId;
        incident.UpdatedAtUtc = DateTime.UtcNow;

        if (request.Status == IncidentStatus.Resolved && incident.ResolvedAtUtc is null)
        {
            incident.ResolvedAtUtc = DateTime.UtcNow;
        }

        if (request.Status != IncidentStatus.Resolved)
        {
            incident.ResolvedAtUtc = null;
        }

        await _incidentRepository.UpdateAsync(incident);

        return ToResponse(incident);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
        {
            return false;
        }

        await _incidentRepository.DeleteAsync(incident);

        return true;
    }

    private static IncidentResponse ToResponse(Incident incident)
    {
        return new IncidentResponse
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            Status = incident.Status,
            Priority = incident.Priority,
            CreatedByUserId = incident.CreatedByUserId,
            AssignedToUserId = incident.AssignedToUserId,
            CreatedAtUtc = incident.CreatedAtUtc,
            UpdatedAtUtc = incident.UpdatedAtUtc,
            ResolvedAtUtc = incident.ResolvedAtUtc,

            CreatedBy = new UserSummaryResponse
            {
                Id = incident.CreatedByUser.Id,
                Email = incident.CreatedByUser.Email
            },
            
            AssignedTo = new UserSummaryResponse
            {
                Id = incident.AssignedToUser!.Id,
                Email = incident.AssignedToUser.Email
            }
        };
    }

    private static IncidentSummaryResponse ToSummaryResponse(Incident incident)
    {
        return new IncidentSummaryResponse
        {
            Id = incident.Id,
            Title = incident.Title,
            Status = incident.Status,
            Priority = incident.Priority,
            AssignedToUserId = incident.AssignedToUserId,
            CreatedAtUtc = incident.CreatedAtUtc
        };
    }

    private static void ValidateQuery(IncidentQueryRequest query)
    {
        if (query.Page < 1)
        {
            throw new ArgumentException("Page must be greater than or equal to 1.");
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            throw new ArgumentException("PageSize must be between 1 and 100.");
        }

        var allowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "createdAt",
            "updatedAt",
            "priority",
            "status",
            "title"
        };

        if (!allowedSortFields.Contains(query.SortBy))
        {
            throw new ArgumentException($"Invalid sort field '{query.SortBy}'. Allowed values: 'createdAt', 'updatedAt', 'priority', 'status', 'title'.");
        }

        var allowedDirections = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "asc",
            "desc"
        };

        if (!allowedDirections.Contains(query.SortDirection))
        {
            throw new ArgumentException($"Invalid sort direction '{query.SortDirection}'. Allowed values: 'asc', 'desc'.");
        }

        if (!string.IsNullOrWhiteSpace(query.Search) && query.Search.Length > 100)
        {
            throw new ArgumentException("Search cannot exceed 100 characters.");
        }
    }
}
