using Microsoft.AspNetCore.Mvc;
using IncidentHub.Api.Data.Seeders;

namespace IncidentHub.Api.Controllers;

[ApiController]
[Route("api/dev")]
public class DevDataController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly LargeDatasetSeeder _seeder;

    public DevDataController(
            IWebHostEnvironment environment,
            LargeDatasetSeeder seeder)
    {
        _environment = environment;
        _seeder = seeder;
    }

    [HttpPost("seed-large-dataset")]
    public async Task<IActionResult> SeedLargeDataset()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var result = await _seeder.SeedAsync();

        return Ok(result);
    }
}
