using IncidentHub.Api.Models;
using IncidentHub.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentHub.Api.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _dbContext;

    public CommentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        return comment;
    }

    public async Task<List<Comment>> GetByIncidentIdAsync(Guid incidentId)
    {
        return await _dbContext.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.IncidentId == incidentId)
            .OrderBy(c => c.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task DeleteAsync(Comment comment)
    {
        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
}

