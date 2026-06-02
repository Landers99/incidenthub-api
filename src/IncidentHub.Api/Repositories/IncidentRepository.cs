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
        var incidents = await _dbContext.Incidents
            .Include(i => i.CreatedByUser)
            .Include(i => i.AssignedToUser)
            .Include(i => i.Comments)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (!Enum.TryParse<IncidentStatus>(query.Status, true, out var status))
            {
                throw new ArgumentException($"Invalid status '{query.Status}'.");
            }

            incidents = incidents
                .Where(i => i.Status == status)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Priority))
        {
            if (!Enum.TryParse<IncidentPriority>(query.Priority, true, out var priority))
            {
                throw new ArgumentException($"Invalid priority '{query.Priority}'.");
            }

            incidents = incidents
                .Where(i => i.Priority == priority)
                .ToList();
        }

        if (query.AssignedToUserId.HasValue)
        {
            incidents = incidents
                .Where(i => i.AssignedToUserId == query.AssignedToUserId.Value)
                .ToList();
        }

        if (query.CreatedByUserId.HasValue)
        {
            incidents = incidents
                .Where(i => i.CreatedByUserId == query.CreatedByUserId.Value)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            incidents = incidents
                .Where(i =>
                    i.Title.ToLower().Contains(search) ||
                    i.Description.ToLower().Contains(search))
                .ToList();
        }

        incidents = ApplySortingInMemory(incidents, query).ToList();

        var totalCount = incidents.Count();

        var items = incidents
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
            .ToList();

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

        await _dbContext.Incidents
            .Include(i => i.CreatedByUser)
            .Include(i => i.AssignedToUser)
            .FirstOrDefaultAsync(i => i.Id == incident.Id);
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

    private static IEnumerable<Incident> ApplySortingInMemory(
            IEnumerable<Incident> incidents,
            IncidentQueryRequest query)
    {
        var descending = string.Equals(
                query.SortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

        return query.SortBy?.ToLower() switch
        {
            "title" => descending
                ? incidents.OrderByDescending(i => i.Title)
                : incidents.OrderBy(i => i.Title),

            "status" => descending
                ? incidents.OrderByDescending(i => i.Status)
                : incidents.OrderBy(i => i.Status),

            "priority" => descending
                ? incidents.OrderByDescending(i => i.Priority)
                : incidents.OrderBy(i => i.Priority),

            "updatedat" => descending
                ? incidents.OrderByDescending(i => i.UpdatedAtUtc)
                : incidents.OrderBy(i => i.UpdatedAtUtc),

            _ => descending
                ? incidents.OrderByDescending(i => i.CreatedAtUtc)
                : incidents.OrderBy(i => i.CreatedAtUtc)
        };
    }
}
