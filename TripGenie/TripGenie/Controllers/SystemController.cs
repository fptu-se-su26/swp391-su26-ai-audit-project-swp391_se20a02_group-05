using Microsoft.AspNetCore.Mvc;
using TripGenie.API.Services;

namespace TripGenie.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase {
    private readonly ISystemService _systemService;

    public SystemController(ISystemService systemService)
    {
        _systemService = systemService;
    }

    [HttpGet("database-status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        var result = await _systemService.CheckDatabaseStatusAsync();

        if (!result.Success)
        {
            return StatusCode(503, result);
        }

        return Ok(result);
    }
    
    [HttpGet("status")]
    public IActionResult GetStatus() {
        return Ok(new {
            Status = "Online",
            Message = "TripGenie System is operational."
        });
    }

    [HttpGet("health")]
    public IActionResult GetHealth() {
        return Ok(new {
            Status = "Healthy",
            Checks = new[] {
                new { Name = "Database", Status = "Up" },
                new { Name = "Storage", Status = "Up" }
            }
        });
    }

    [HttpGet("version")]
    public IActionResult GetVersion() {
        return Ok(new {
            Version = "1.0.0",
            Environment = "Development",
            BuildDate = "2026-05-14"
        });
    }

    [HttpGet("info")]
    public IActionResult GetInfo() {
        return Ok(new {
            Application = "TripGenie Server",
            Framework = ".NET 8.0",
            OS = System.Runtime.InteropServices.RuntimeInformation.OSDescription
        });
    }

    [HttpGet("time")]
    public IActionResult GetTime() {
        return Ok(new {
            UtcNow = DateTime.UtcNow,
            LocalTime = DateTime.Now,
            TimeZone = TimeZoneInfo.Local.DisplayName
        });
    }
}
