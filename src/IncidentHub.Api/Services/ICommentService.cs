using IncidentHub.Api.DTOs.Comments;

namespace IncidentHub.Api.Services;

public interface ICommentService
{
    Task<CommentResponse> AddCommentAsync(Guid incidentId, Guid userId, CreateCommentRequest request);
    Task<List<CommentResponse>> GetCommentsForIncidentAsync(Guid incidentId);
    Task DeleteCommentAsync(Guid commentId, Guid userId, bool isAdmin);
}
