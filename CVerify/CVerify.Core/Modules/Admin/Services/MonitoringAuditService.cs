using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using CVerify.API.Modules.Admin.DTOs;
using CVerify.API.Modules.Admin.Hubs;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.Modules.Admin.Services;

public class MonitoringAuditService : IMonitoringAuditService
{
    private const string AdminGroup = "admins";
    private const string ClientMethod = "ReceiveMonitoringAlert";

    private readonly ApplicationDbContext _context;
    private readonly IHubContext<AdminHub> _adminHub;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<MonitoringAuditService> _logger;

    public MonitoringAuditService(
        ApplicationDbContext context,
        IHubContext<AdminHub> adminHub,
        TimeProvider timeProvider,
        ILogger<MonitoringAuditService> logger)
    {
        _context = context;
        _adminHub = adminHub;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<AdminMonitoringAlertDto> RecordAndBroadcastAsync(
        MonitoringEventIngestDto dto,
        CancellationToken cancellationToken = default)
    {
        var severity = NormalizeSeverity(dto.Severity);
        var source = string.IsNullOrWhiteSpace(dto.Source) ? "CVerify.AI" : dto.Source.Trim();
        var eventType = $"MONITORING_{dto.EventType.Trim().ToUpperInvariant()}";
        var createdAt = _timeProvider.GetUtcNow();

        var detailsPayload = new
        {
            severity,
            source,
            correlationId = dto.CorrelationId,
            occurredAt = dto.OccurredAt ?? createdAt,
            data = dto.Details
        };

        var log = new AuditLog
        {
            Id = Guid.CreateVersion7(),
            EventType = eventType,
            Description = dto.Message,
            ScopeType = "MONITORING",
            DetailsJson = JsonSerializer.Serialize(detailsPayload),
            CreatedAt = createdAt
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        var alert = new AdminMonitoringAlertDto(
            log.Id,
            eventType,
            severity,
            source,
            dto.Message,
            createdAt);

        try
        {
            await _adminHub.Clients
                .Group(AdminGroup)
                .SendAsync(ClientMethod, alert, cancellationToken);
        }
        catch (Exception ex)
        {
            // The event is already persisted; a broadcast failure must not fail the ingest.
            _logger.LogWarning(ex, "Failed to broadcast monitoring alert {EventType} to admins.", eventType);
        }

        return alert;
    }

    private static string NormalizeSeverity(string? severity)
    {
        if (string.IsNullOrWhiteSpace(severity))
        {
            return "info";
        }

        return severity.Trim().ToLowerInvariant() switch
        {
            "critical" or "fatal" => "critical",
            "error" or "err" => "error",
            "warning" or "warn" => "warning",
            _ => "info"
        };
    }
}
