using IncidentHub.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace IncidentHub.Api.DTOs.Incidents;

public class UpdateIncidentRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public IncidentStatus Status { get; set; }

    [Required]
    public IncidentPriority Priority { get; set; }

    public Guid? AssignedToUserId { get; set; }
}
