using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CVerify.API.Modules.Admin.DTOs;
using CVerify.API.Modules.Admin.Services;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.Modules.Admin.Controllers;

/// <summary>
/// Internal, HMAC-authenticated ingest endpoint for monitoring events emitted by
/// CVerify.AI. Events are persisted as audit logs and broadcast to admins in realtime.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/admin/monitoring")]
public class MonitoringController : ControllerBase
{
    private const string ExpectedClientId = "cverify-ai";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IMonitoringAuditService _monitoringAuditService;
    private readonly IHmacSignatureService _hmacService;
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(
        IMonitoringAuditService monitoringAuditService,
        IHmacSignatureService hmacService,
        ILogger<MonitoringController> logger)
    {
        _monitoringAuditService = monitoringAuditService;
        _hmacService = hmacService;
        _logger = logger;
    }

    [HttpPost("events")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AdminMonitoringAlertDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IngestEvent(CancellationToken cancellationToken)
    {
        // Read the raw body so the HMAC signature is verified against the exact bytes signed.
        string rawBody;
        using (var reader = new StreamReader(Request.Body))
        {
            rawBody = await reader.ReadToEndAsync(cancellationToken);
        }

        var clientId = Request.Headers["X-Client-Id"].ToString();
        var timestamp = Request.Headers["X-Timestamp"].ToString();
        var nonce = Request.Headers["X-Nonce"].ToString();
        var signature = Request.Headers["X-Signature"].ToString();

        if (!string.Equals(clientId, ExpectedClientId, StringComparison.Ordinal))
        {
            _logger.LogWarning("Monitoring ingest rejected: unexpected client id '{ClientId}'.", clientId);
            return Unauthorized();
        }

        var path = Request.Path.Value ?? string.Empty;
        var isValid = _hmacService.VerifySignature("POST", path, rawBody, timestamp, nonce, signature);
        if (!isValid)
        {
            _logger.LogWarning("Monitoring ingest rejected: invalid HMAC signature.");
            return Unauthorized();
        }

        MonitoringEventIngestDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<MonitoringEventIngestDto>(rawBody, JsonOptions);
        }
        catch (JsonException)
        {
            return BadRequest("Malformed monitoring event payload.");
        }

        if (dto is null || string.IsNullOrWhiteSpace(dto.EventType) || string.IsNullOrWhiteSpace(dto.Message))
        {
            return BadRequest("Monitoring event requires 'eventType' and 'message'.");
        }

        var alert = await _monitoringAuditService.RecordAndBroadcastAsync(dto, cancellationToken);
        return Accepted(alert);
    }
}
