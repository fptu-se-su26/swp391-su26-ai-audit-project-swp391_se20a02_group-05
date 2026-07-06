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
using CVerify.API.Modules.Auth.Entities;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;
using CVerify.API.Modules.SourceCode.Clients;
using CVerify.API.Modules.SourceCode.Services;
using CVerify.API.Modules.Profiles.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for SourceCodeProviderService.EnqueueSyncJobAsync — CVerify-70 (3 UTCIDs).
/// POST /api/v1/source-providers/{providerId}/sync [Authorize] — enqueues a repo sync job.
/// Uses ICacheService (Redis) and IRepositorySyncQueue — both are mocked.
/// </summary>
public sealed class CVerify70_EnqueueSyncJobTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IRepositorySyncQueue> _syncQueue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SourceCodeProviderService>> _logger = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly SourceCodeProviderService _sut;

    public CVerify70_EnqueueSyncJobTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cacheService
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        _syncQueue
            .Setup(q => q.QueueSyncJob(It.IsAny<RepositorySyncJob>()));

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

    private async Task<(Guid userId, Guid providerId)> SeedProviderAsync()
    {
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider
        {
            Id = providerId, UserId = userId, ProviderName = "github",
            ProviderKey = $"key-{userId}", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Pending",
        });
        await _context.SaveChangesAsync();
        return (userId, providerId);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid userId + providerId → Guid jobId returned; job cached in Redis and enqueued.
    [Fact]
    public async Task CVerify70_UTCID01_EnqueueSyncJob_ValidProvider_ReturnsJobId()
    {
        var (userId, providerId) = await SeedProviderAsync();

        var jobId = await _sut.EnqueueSyncJobAsync(userId, providerId);

        jobId.Should().NotBeEmpty("a valid Guid job ID must be returned");
        _cacheService.Verify(c => c.SetAsync(It.Is<string>(k => k.StartsWith("repository:sync:job:")), It.IsAny<object>(), It.IsAny<TimeSpan?>()), Times.Once);
        _syncQueue.Verify(q => q.QueueSyncJob(It.IsAny<RepositorySyncJob>()), Times.Once);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent providerId → still returns a Guid jobId.
    // The service does NOT validate provider existence at this level; validation is deferred to worker.
    [Fact]
    public async Task CVerify70_UTCID02_EnqueueSyncJob_NonExistentProvider_StillReturnsJobId()
    {
        var userId = Guid.NewGuid();
        var ghostProviderId = Guid.NewGuid();

        var jobId = await _sut.EnqueueSyncJobAsync(userId, ghostProviderId);

        jobId.Should().NotBeEmpty("provider validation is deferred to the background worker");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: any userId → returns a Guid (service is provider-agnostic at enqueue time).
    [Fact]
    public async Task CVerify70_UTCID03_EnqueueSyncJob_NoJwtControllerLevel_ServiceAcceptsGhostUser()
    {
        var ghostUserId = Guid.NewGuid();
        var providerId = Guid.NewGuid();

        var jobId = await _sut.EnqueueSyncJobAsync(ghostUserId, providerId);

        jobId.Should().NotBeEmpty("service enqueues job without user validation");
    }
}
