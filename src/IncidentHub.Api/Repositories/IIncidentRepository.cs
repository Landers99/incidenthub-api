using IncidentHub.Api.Models;
using IncidentHub.Api.DTOs.Common;
using IncidentHub.Api.DTOs.Incidents;

namespace IncidentHub.Api.Repository;

public interface IIncidentRepository
{
    Task<List<Incident>> GetAllAsync();
    Task<Incident?> GetByIdAsync(Guid id);
    Task<PagedResponse<IncidentListItemResponse>> GetPagedAsync(IncidentQueryRequest query);
    Task AddAsync(Incident incident);
    Task UpdateAsync(Incident incident);
    Task DeleteAsync(Incident incident);
}
