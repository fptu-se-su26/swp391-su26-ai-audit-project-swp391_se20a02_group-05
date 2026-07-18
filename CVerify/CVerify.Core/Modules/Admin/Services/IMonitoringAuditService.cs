using System.Threading;
using System.Threading.Tasks;
using CVerify.API.Modules.Admin.DTOs;

namespace CVerify.API.Modules.Admin.Services;

public interface IMonitoringAuditService
{
    /// <summary>
    /// Persists an inbound monitoring event as an audit log and broadcasts a realtime
    /// alert to connected admins. Returns the alert payload that was broadcast.
    /// </summary>
    Task<AdminMonitoringAlertDto> RecordAndBroadcastAsync(
        MonitoringEventIngestDto dto,
        CancellationToken cancellationToken = default);
}
