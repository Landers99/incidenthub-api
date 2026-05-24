using IncidentHub.Api.Data;
using IncidentHub.Api.DTOs.Common;
using IncidentHub.Api.DTOs.Incidents;
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

    public async Task<PagedResponse<IncidentListItemResponse>> GetPagedAsync(
            IncidentQueryRequest query)
    {
        var incidentsQuery = _dbContext.Incidents
            .Include(i => i.CreatedByUser)
            .Include(i => i.AssignedToUser)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (!Enum.TryParse<IncidentStatus>(query.Status, true, out var status))
            {
                throw new ArgumentException($"Invalid status '{query.Status}'.");
            }

            incidentsQuery = incidentsQuery.Where(i => i.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.Priority))
        {
            if (!Enum.TryParse<IncidentPriority>(query.Priority, true, out var priority))
            {
                throw new ArgumentException($"Invalid priority '{query.Priority}'.");
            }

            incidentsQuery = incidentsQuery.Where(i => i.Priority == priority);
        }

        if (query.AssignedToUserId.HasValue)
        {
            incidentsQuery = incidentsQuery.Where(
                    i => i.AssignedToUserId == query.AssignedToUserId.Value);
        }

        if (query.CreatedByUserId.HasValue)
        {
            incidentsQuery = incidentsQuery.Where(
                i => i.CreatedByUserId == query.CreatedByUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            incidentsQuery = incidentsQuery.Where(
                    i => i.Title.ToLower().Contains(search) ||
                    i.Description.ToLower().Contains(search));
        }

        incidentsQuery = ApplySorting(incidentsQuery, query);

        var totalCount = await incidentsQuery.CountAsync();

        var items = await incidentsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new IncidentListItemResponse
            {
                Id = i.Id,
                Title = i.Title,
                Status = i.Status.ToString(),
                Priority = i.Priority.ToString(),
                CreatedAtUtc = i.CreatedAtUtc,
                UpdatedAtUtc = i.UpdatedAtUtc,
                CreatedByEmail = i.CreatedByUser.Email,
                AssignedToEmail = i.AssignedToUser == null
                    ? null
                    : i.AssignedToUser.Email
            })
            .ToListAsync();

        return new PagedResponse<IncidentListItemResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
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

    private static IQueryable<Incident> ApplySorting(
            IQueryable<Incident> query,
            IncidentQueryRequest request)
    {
        var ascending = request.SortDirection.Equals(
                "asc",
                StringComparison.OrdinalIgnoreCase);

        return request.SortBy.ToLowerInvariant() switch
        {
            "title" => ascending
                ? query.OrderBy(i => i.Title)
                : query.OrderByDescending(i => i.Title),

            "status" => ascending
                ? query.OrderBy(i => i.Status)
                : query.OrderByDescending(i => i.Status),

            "priority" => ascending
                ? query.OrderBy(i => i.Priority)
                : query.OrderByDescending(i => i.Priority),

            "updatedat" => ascending
                ? query.OrderBy(i => i.UpdatedAtUtc)
                : query.OrderByDescending(i => i.UpdatedAtUtc),

            _ => ascending
                ? query.OrderBy(i => i.CreatedAtUtc)
                : query.OrderByDescending(i => i.CreatedAtUtc),
        };
    }
}
