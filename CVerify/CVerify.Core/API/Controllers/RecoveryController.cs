using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CVerify.API.Application.DTOs;
using CVerify.API.Application.Interfaces;

namespace CVerify.API.API.Controllers;

[ApiController]
[Route("api/recovery")]
public class RecoveryController : ControllerBase
{
    private readonly IRecoveryService _recoveryService;

    public RecoveryController(IRecoveryService recoveryService)
    {
        _recoveryService = recoveryService;
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerifyOtpResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequest request,
        [FromQuery] string taxCode,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _recoveryService.VerifyRecoveryOtpAsync(request, taxCode, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("request")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubmitClaimResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitClaim(
        [FromForm] SubmitClaimRequest request,
        [FromForm] List<IFormFile> documents,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Recovery claim request payload is missing.");
        }

        var userAgent = Request.Headers.UserAgent.ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var docList = new List<(System.IO.Stream fileStream, string fileName, string contentType)>();
        foreach (var file in documents)
        {
            docList.Add((file.OpenReadStream(), file.FileName, file.ContentType));
        }

        try
        {
            var response = await _recoveryService.SubmitClaimAsync(request, docList, userAgent, ipAddress, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("claims")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClaimDetailsResponse>))]
    public async Task<IActionResult> GetClaims(CancellationToken cancellationToken)
    {
        var claims = await _recoveryService.GetPendingClaimsAsync(cancellationToken);
        return Ok(claims);
    }

    [HttpPost("claims/{id}/review")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReviewClaim(
        Guid id,
        [FromBody] ReviewClaimRequest request,
        CancellationToken cancellationToken)
    {
        var reviewerName = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? "system_admin";
        
        try
        {
            var result = await _recoveryService.ReviewClaimAsync(id, request, reviewerName, cancellationToken);
            return Ok(new { success = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("claims/{id}/document/{docId}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadDocument(
        Guid id,
        Guid docId,
        CancellationToken cancellationToken)
    {
        var reviewerName = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? "system_admin";

        try
        {
            var (fileStream, fileName, contentType) = await _recoveryService.DownloadDocumentAsync(docId, reviewerName, cancellationToken);
            return File(fileStream, contentType, fileName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("bootstrap/verify")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerifyBootstrapResponse))]
    public async Task<IActionResult> VerifyBootstrap([FromQuery] string token, CancellationToken cancellationToken)
    {
        var response = await _recoveryService.VerifyBootstrapTokenAsync(token, cancellationToken);
        if (!response.IsValid)
        {
            return BadRequest(new { message = "Recovery token is invalid, expired, or has already been used." });
        }

        return Ok(response);
    }

    [HttpPost("bootstrap/setup-credentials")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SetupRecoveryCredentialsResponse))]
    public async Task<IActionResult> SetupCredentials([FromBody] SetupRecoveryCredentialsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _recoveryService.SetupRecoveryCredentialsAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("bootstrap/execute")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    public async Task<IActionResult> ExecuteRecovery([FromBody] ExecuteRecoveryRequest request, CancellationToken cancellationToken)
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        try
        {
            var response = await _recoveryService.ExecuteRecoveryAsync(request, userAgent, ipAddress, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
