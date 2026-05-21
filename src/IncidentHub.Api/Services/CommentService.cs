using IncidentHub.Api.DTOs.Comments;
using IncidentHub.Api.Models;
using IncidentHub.Api.Repository;

namespace IncidentHub.Api.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IIncidentRepository _incidentRepository;

    public CommentService(ICommentRepository commentRepository, IIncidentRepository incidentRepository)
    {
        _commentRepository = commentRepository;
        _incidentRepository = incidentRepository;
    }

    public async Task<CommentResponse> AddCommentAsync(Guid incidentId, Guid userId, CreateCommentRequest request)
    {
        ValidateCreateCommentRequest(request);

        var incident = await _incidentRepository.GetByIdAsync(incidentId);

        if (incident is null)
        {
            throw new KeyNotFoundException($"Incident '{incidentId}' was not found.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            IncidentId = incidentId,
            UserId = userId,
            Body = request.Body.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        var saved = await _commentRepository.AddAsync(comment);

        var comments = await _commentRepository.GetByIncidentIdAsync(incidentId);
        var hydrated = comments.First(c => c.Id == saved.Id);

        return MapToResponse(hydrated);
    }

    public async Task<List<CommentResponse>> GetCommentsForIncidentAsync(Guid incidentId)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId);

        if (incident is null)
        {
            throw new KeyNotFoundException($"Incident '{incidentId}' was not found.");
        }

        var comments = await _commentRepository.GetByIncidentIdAsync(incidentId);

        return comments.Select(MapToResponse).ToList();
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId, bool isAdmin)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);

        if (comment is null)
        {
            throw new KeyNotFoundException($"Comment '{commentId}' was not found.");
        }

        var isOwner = comment.UserId == userId;

        if (!isOwner && !isAdmin)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this comment.");
        }

        await _commentRepository.DeleteAsync(comment);
    }

    private static CommentResponse MapToResponse(Comment comment)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            IncidentId = comment.IncidentId,
            UserId = comment.UserId,
            UserEmail = comment.User?.Email ?? string.Empty,
            Body = comment.Body,
            CreatedAtUtc = comment.CreatedAtUtc
        };
    }

    private static void ValidateCreateCommentRequest(CreateCommentRequest request)
    {
        if (request is null)
        {
            throw new ArgumentException("Reqeust body is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new ArgumentException("Comment body is required.");
        }

        if (request.Body.Length > 2000)
        {
            throw new ArgumentException("Comment body cannot exceed 2000 characters.");
        }
    }
}

