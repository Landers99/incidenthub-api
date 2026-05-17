using IncidentHub.Api.DTOs.Incidents;

namespace IncidentHub.Api.Services;

public interface IIncidentService
{
    Task<List<IncidentSummaryResponse>> GetAllAsync();
    Task<IncidentResponse?> GetByIdAsync(Guid id);
    Task<IncidentResponse> CreateAsync(CreateIncidentRequest request);
    Task<IncidentResponse?> UpdateAsync(Guid id, UpdateIncidentRequest reqeust);
    Task<bool> DeleteAsync(Guid id);
}
