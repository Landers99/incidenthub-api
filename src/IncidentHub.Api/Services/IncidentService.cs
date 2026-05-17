using IncidentHub.Api.DTOs.Incidents;
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
            ResolvedAtUtc = incident.ResolvedAtUtc
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
}
