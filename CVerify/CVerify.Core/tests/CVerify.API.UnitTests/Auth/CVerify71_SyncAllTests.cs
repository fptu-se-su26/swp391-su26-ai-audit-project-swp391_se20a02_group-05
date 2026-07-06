using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;
using CVerify.API.Modules.SourceCode.Clients;
using CVerify.API.Modules.SourceCode.Services;
using CVerify.API.Modules.Profiles.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for SourceCodeProviderService.EnqueueSyncJobAsync(userId, null) — CVerify-71 (3 UTCIDs).
/// POST /api/v1/source-providers/sync-all [Authorize] — syncs all providers (providerId=null).
/// </summary>
public sealed class CVerify71_SyncAllTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IRepositorySyncQueue> _syncQueue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SourceCodeProviderService>> _logger = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly SourceCodeProviderService _sut;

    public CVerify71_SyncAllTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cacheService
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        _syncQueue.Setup(q => q.QueueSyncJob(It.IsAny<RepositorySyncJob>()));

        _sut = new SourceCodeProviderService(
            _context,
            _cacheService.Object,
            _syncQueue.Object,
            new EnvConfiguration(),
            _httpClientFactory.Object,
            _logger.Object,
            TimeProvider.System,
            Enumerable.Empty<ISourceCodeClient>(),
            _cvRepositoryIndexer.Object);
    }

    public void Dispose() => _context.Dispose();

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // SyncAll (providerId=null) → Guid jobId returned; RepositorySyncJob with null AuthProviderId enqueued.
    [Fact]
    public async Task CVerify71_UTCID01_SyncAll_ValidUser_ReturnsJobIdWithNullProvider()
    {
        var userId = Guid.NewGuid();

        var jobId = await _sut.EnqueueSyncJobAsync(userId, null);

        jobId.Should().NotBeEmpty("sync-all returns a valid job id");
        _syncQueue.Verify(q => q.QueueSyncJob(It.Is<RepositorySyncJob>(j => j.AuthProviderId == null)), Times.Once,
            "sync-all passes providerId=null to the queue");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // SyncAll cached in Redis with the correct key prefix.
    [Fact]
    public async Task CVerify71_UTCID02_SyncAll_CachesJobStatusInRedis()
    {
        var userId = Guid.NewGuid();

        var jobId = await _sut.EnqueueSyncJobAsync(userId, null);

        _cacheService.Verify(c => c.SetAsync(
            It.Is<string>(k => k == $"repository:sync:job:{jobId}"),
            It.IsAny<object>(),
            It.IsAny<TimeSpan?>()), Times.Once, "job status must be cached by jobId");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: EnqueueSyncJobAsync(ghostUserId, null) → still returns jobId.
    [Fact]
    public async Task CVerify71_UTCID03_SyncAll_NoJwtControllerLevel_ServiceAcceptsGhostUser()
    {
        var ghostUserId = Guid.NewGuid();

        var jobId = await _sut.EnqueueSyncJobAsync(ghostUserId, null);

        jobId.Should().NotBeEmpty("service does not validate userId at enqueue time");
    }
}
