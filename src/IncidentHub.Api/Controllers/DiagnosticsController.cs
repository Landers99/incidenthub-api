using Microsoft.AspNetCore.Mvc;

namespace IncidentHub.Api.Controllers;

[ApiController]
[Route("api")]
public class DiagnosticsController : ControllerBase
{
    public DiagnosticsController()
    {
    }

    [HttpGet("diagnostics/test-error")]
    public IActionResult TestError()
    {
        throw new InvalidOperationException("This is a test exception.");
    }
}

