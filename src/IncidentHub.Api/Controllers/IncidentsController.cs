using IncidentHub.Api.Services;
using IncidentHub.Api.DTOs.Incidents;
using Microsoft.AspNetCore.Mvc;

namespace IncidentHub.Api.Controllers;

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
        var created = await _incidentService.CreateAsync(request);

        return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
        );
    }

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
