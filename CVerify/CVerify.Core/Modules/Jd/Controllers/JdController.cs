using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CVerify.API.Modules.Jd.DTOs;
using CVerify.API.Modules.Jd.Services;

namespace CVerify.API.Modules.Jd.Controllers;

[ApiController]
[Route("api/jd")]
[Authorize]
public sealed class JdController : ControllerBase
{
    private readonly IJdService _jdService;
    private readonly ILogger<JdController> _logger;

    public JdController(IJdService jdService, ILogger<JdController> logger)
    {
        _jdService = jdService;
        _logger = logger;
    }

    private Guid CurrentUserId
    {
        get
        {
            var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(raw) || !Guid.TryParse(raw, out var id))
                throw new UnauthorizedAccessException("Unauthenticated or invalid user ID.");
            return id;
        }
    }

    /// <summary>
    /// L3-001/L3-002/L3-003/L3-004: Create a standardized JD via AI pipeline.
    /// Validates form data, normalizes skills, generates professional JD text, and stores the result.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(JdCreateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateJd([FromBody] JdFormRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.JobTitle))
            return BadRequest(new { error = "jobTitle is required" });

        if (request.RequiredSkills == null || request.RequiredSkills.Count == 0)
            return BadRequest(new { error = "At least one required skill must be provided" });

        try
        {
            var result = await _jdService.CreateJdAsync(CurrentUserId, request, cancellationToken);

            if (!result.IsValid)
                return BadRequest(new { errors = result.ValidationErrors });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JD creation failed for user {UserId}", CurrentUserId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "JD creation failed. Please try again." });
        }
    }
}
