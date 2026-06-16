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
    private readonly IJdMatchingService _matchingService;
    private readonly ILogger<JdController> _logger;

    public JdController(IJdService jdService, IJdMatchingService matchingService, ILogger<JdController> logger)
    {
        _jdService = jdService;
        _matchingService = matchingService;
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

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<JdSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListJds(CancellationToken cancellationToken)
    {
        var result = await _jdService.ListJdsAsync(CurrentUserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{jdId}")]
    [ProducesResponseType(typeof(JdDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJd([FromRoute] string jdId, CancellationToken cancellationToken)
    {
        var result = await _jdService.GetJdAsync(CurrentUserId, jdId, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("{jdId}")]
    [ProducesResponseType(typeof(JdDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJd([FromRoute] string jdId, [FromBody] JdUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await _jdService.UpdateJdAsync(CurrentUserId, jdId, request, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{jdId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteJd([FromRoute] string jdId, CancellationToken cancellationToken)
    {
        var deleted = await _jdService.DeleteJdAsync(CurrentUserId, jdId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("match")]
    [ProducesResponseType(typeof(MatchScoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult MatchCandidate([FromBody] JdMatchRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.NormalizedJd is null)
            return BadRequest(new { error = "normalizedJd is required" });

        if (request.CandidateSkills is null)
            return BadRequest(new { error = "candidateSkills is required" });

        if (request.CandidateResponsibilities is null)
            return BadRequest(new { error = "candidateResponsibilities is required" });

        if (request.CandidateSkills.Count > 200)
            return BadRequest(new { error = "candidateSkills cannot contain more than 200 items" });

        if (request.CandidateResponsibilities.Count > 100)
            return BadRequest(new { error = "candidateResponsibilities cannot contain more than 100 items" });

        if (request.NormalizedJd.RequiredSkills is not { Count: > 0 })
            return BadRequest(new { error = "normalizedJd.requiredSkills must contain at least one skill" });

        if (request.NormalizedJd.Responsibilities is null)
            return BadRequest(new { error = "normalizedJd.responsibilities is required" });

        if (request.NormalizedJd.SalaryMin > request.NormalizedJd.SalaryMax && request.NormalizedJd.SalaryMax > 0)
            return BadRequest(new { error = "normalizedJd.salaryMin must be less than or equal to salaryMax" });

        var desiredSalary = request.DesiredSalary ?? 0m;
        var minimumAcceptableSalary = request.MinimumAcceptableSalary ?? 0m;

        if (desiredSalary == 0 && minimumAcceptableSalary > 0)
            return BadRequest(new { error = "minimumAcceptableSalary cannot be set when desiredSalary is 0" });

        if (minimumAcceptableSalary > desiredSalary)
            return BadRequest(new { error = "minimumAcceptableSalary must be less than or equal to desiredSalary" });

        var result = _matchingService.CalculateMatch(request);
        return Ok(result);
    }
}
