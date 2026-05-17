using IncidentHub.Api.DTOs.Incidents;

namespace IncidentHub.Api.Services;

public interface IIncidentService
{
    Task<List<IncidentSummaryResponse>> GetAllAsync();
    Task<IncidentResponse?> GetByIdAsync(Guid id);
    Task<IncidentResponse> CreateAsync(CreateIncidentRequest request, Guid userId);
    Task<IncidentResponse?> UpdateAsync(Guid id, UpdateIncidentRequest reqeust);
    Task<bool> DeleteAsync(Guid id);
}
