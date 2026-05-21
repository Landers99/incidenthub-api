using IncidentHub.Api.Models;

namespace IncidentHub.Api.Repository;

public interface ICommentRepository
{
    Task<Comment> AddAsync(Comment comment);
    Task<List<Comment>> GetByIncidentIdAsync(Guid incidentId);
    Task<Comment?> GetByIdAsync(Guid id);
    Task DeleteAsync(Comment comment);
}
