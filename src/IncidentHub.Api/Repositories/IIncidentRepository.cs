using IncidentHub.Api.Models;

namespace IncidentHub.Api.Repository;

public interface IIncidentRepository
{
    Task<List<Incident>> GetAllAsync();
    Task<Incident?> GetByIdAsync(Guid id);
    Task AddAsync(Incident incident);
    Task UpdateAsync(Incident incident);
    Task DeleteAsync(Incident incident);
}
