using IncidentHub.Api.DTOs.Incidents;
using IncidentHub.Api.DTOs.Common;

namespace IncidentHub.Api.Services;

public interface IIncidentService
{
    Task<List<IncidentSummaryResponse>> GetAllAsync();
    Task<IncidentResponse?> GetByIdAsync(Guid id);
    Task<PagedResponse<IncidentListItemResponse>> GetPagedAsync(IncidentQueryRequest query);
    Task<IncidentResponse> CreateAsync(CreateIncidentRequest request, Guid userId);
    Task<IncidentResponse?> UpdateAsync(Guid id, UpdateIncidentRequest reqeust);
    Task<bool> DeleteAsync(Guid id);
}
