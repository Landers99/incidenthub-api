using IncidentHub.Api.Services;
using IncidentHub.Api.DTOs.Incidents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IncidentHub.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentService _incidentService;

    public IncidentsController(IIncidentService incidentService)
    {
        _incidentService = incidentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<IncidentSummaryResponse>>> GetAll()
    {
        var incidents = await _incidentService.GetAllAsync();
        return Ok(incidents);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IncidentResponse>> GetById(Guid id)
    {
        var incident = await _incidentService.GetByIdAsync(id);

        if (incident is null)
        {
            return NotFound();
        }

        return Ok(incident);
    }

    [HttpPost]
    public async Task<ActionResult<IncidentResponse>> Create(CreateIncidentRequest request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var incident = await _incidentService.CreateAsync(request, userId);

        return CreatedAtAction(
                nameof(GetById),
                new { id = incident.Id },
                incident
        );
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IncidentResponse>> Update(Guid id, UpdateIncidentRequest request)
    {
        var updated = await _incidentService.UpdateAsync(id, request);

        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _incidentService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
