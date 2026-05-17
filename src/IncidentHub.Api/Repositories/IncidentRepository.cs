using IncidentHub.Api.Data;
using IncidentHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentHub.Api.Repository;

public class IncidentRepository : IIncidentRepository
{
    private readonly AppDbContext _dbContext;

    public IncidentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Incident>> GetAllAsync()
    {
        return await _dbContext.Incidents
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Incident?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Incidents
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task AddAsync(Incident incident)
    {
        await _dbContext.Incidents.AddAsync(incident);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Incident incident)
    {
        _dbContext.Incidents.Update(incident);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Incident incident)
    {
        _dbContext.Incidents.Remove(incident);
        await _dbContext.SaveChangesAsync();
    }
}
