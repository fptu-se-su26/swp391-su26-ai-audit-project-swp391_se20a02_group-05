using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using FluentAssertions;
using Moq;
using Xunit;
using CVerify.API.Modules.Admin.DTOs;
using CVerify.API.Modules.Admin.Hubs;
using CVerify.API.Modules.Admin.Services;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Services;

public class MonitoringAuditServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IHubContext<AdminHub>> _hubContextMock;
    private readonly Mock<IClientProxy> _adminGroupProxyMock;
    private readonly FakeTimeProvider _timeProvider;
    private readonly MonitoringAuditService _service;

    public MonitoringAuditServiceTests()
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _context = new ApplicationDbContext(dbOptions);

        _adminGroupProxyMock = new Mock<IClientProxy>();
        var clientsMock = new Mock<IHubClients>();
        clientsMock.Setup(c => c.Group("admins")).Returns(_adminGroupProxyMock.Object);

        _hubContextMock = new Mock<IHubContext<AdminHub>>();
        _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

        _timeProvider = new FakeTimeProvider(DateTimeOffset.Parse("2026-07-18T10:00:00Z"));

        _service = new MonitoringAuditService(
            _context,
            _hubContextMock.Object,
            _timeProvider,
            Mock.Of<ILogger<MonitoringAuditService>>());
    }

    [Fact]
    public async Task RecordAndBroadcastAsync_PersistsAuditLogWithMonitoringScope()
    {
        var dto = new MonitoringEventIngestDto(
            EventType: "budget_exceeded",
            Message: "AI token budget exceeded for pipeline run.",
            Severity: "critical",
            Source: "ai-cost-tracker",
            CorrelationId: "corr-123",
            Details: null,
            OccurredAt: null);

        var alert = await _service.RecordAndBroadcastAsync(dto);

        var saved = _context.AuditLogs.Single();
        saved.EventType.Should().Be("MONITORING_BUDGET_EXCEEDED");
        saved.ScopeType.Should().Be("MONITORING");
        saved.Description.Should().Be(dto.Message);
        saved.CreatedAt.Should().Be(_timeProvider.GetUtcNow());

        alert.Id.Should().Be(saved.Id);
        alert.Severity.Should().Be("critical");
        alert.Source.Should().Be("ai-cost-tracker");
    }

    [Fact]
    public async Task RecordAndBroadcastAsync_BroadcastsToAdminsGroup()
    {
        var dto = new MonitoringEventIngestDto(
            "pipeline_error", "Line 2 pipeline failed.", "error", "orchestrator", null, null, null);

        await _service.RecordAndBroadcastAsync(dto);

        _adminGroupProxyMock.Verify(
            p => p.SendCoreAsync(
                "ReceiveMonitoringAlert",
                It.Is<object[]>(args => args.Length == 1 && args[0] is AdminMonitoringAlertDto),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("warn", "warning")]
    [InlineData("WARNING", "warning")]
    [InlineData("err", "error")]
    [InlineData("fatal", "critical")]
    [InlineData(null, "info")]
    [InlineData("something-else", "info")]
    public async Task RecordAndBroadcastAsync_NormalizesSeverity(string? input, string expected)
    {
        var dto = new MonitoringEventIngestDto("evt", "msg", input, "src", null, null, null);

        var alert = await _service.RecordAndBroadcastAsync(dto);

        alert.Severity.Should().Be(expected);
    }

    [Fact]
    public async Task RecordAndBroadcastAsync_DefaultsSourceWhenMissing()
    {
        var dto = new MonitoringEventIngestDto("evt", "msg", "info", null, null, null, null);

        var alert = await _service.RecordAndBroadcastAsync(dto);

        alert.Source.Should().Be("CVerify.AI");
    }

    [Fact]
    public async Task RecordAndBroadcastAsync_StillPersistsWhenBroadcastThrows()
    {
        _adminGroupProxyMock
            .Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("hub down"));

        var dto = new MonitoringEventIngestDto("evt", "msg", "info", "src", null, null, null);

        var act = async () => await _service.RecordAndBroadcastAsync(dto);

        await act.Should().NotThrowAsync();
        _context.AuditLogs.Should().HaveCount(1);
    }
}
