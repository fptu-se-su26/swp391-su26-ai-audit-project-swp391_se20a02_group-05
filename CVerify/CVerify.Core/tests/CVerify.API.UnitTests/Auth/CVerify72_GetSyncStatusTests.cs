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
using CVerify.API.Modules.SourceCode.DTOs;
using CVerify.API.Modules.SourceCode.Services;
using CVerify.API.Modules.Profiles.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for SourceCodeProviderService.GetSyncStatusAsync — CVerify-72 (4 UTCIDs).
/// GET /api/v1/source-providers/sync/status/{jobId} [Authorize] — returns sync job status from Redis cache.
/// </summary>
public sealed class CVerify72_GetSyncStatusTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IRepositorySyncQueue> _syncQueue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SourceCodeProviderService>> _logger = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly SourceCodeProviderService _sut;

    public CVerify72_GetSyncStatusTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

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

    private void MockCachedStatus(Guid jobId, Guid userId, string status)
    {
        var cached = new RepositorySyncJobStatus
        {
            JobId = jobId,
            UserId = userId,
            Status = status,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _cacheService
            .Setup(c => c.GetAsync<RepositorySyncJobStatus>($"repository:sync:job:{jobId}"))
            .ReturnsAsync(cached);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Job running (status='Syncing') → RepositorySyncJobStatus returned.
    [Fact]
    public async Task CVerify72_UTCID01_GetSyncStatus_SyncingJob_ReturnsStatus()
    {
        var userId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        MockCachedStatus(jobId, userId, "Syncing");

        var result = await _sut.GetSyncStatusAsync(userId, jobId);

        result.Should().NotBeNull();
        result!.Status.Should().Be("Syncing");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Job completed (status='Completed') → RepositorySyncJobStatus returned.
    [Fact]
    public async Task CVerify72_UTCID02_GetSyncStatus_CompletedJob_ReturnsCompletedStatus()
    {
        var userId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        MockCachedStatus(jobId, userId, "Completed");

        var result = await _sut.GetSyncStatusAsync(userId, jobId);

        result.Should().NotBeNull();
        result!.Status.Should().Be("Completed");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Non-existent jobId (not in Redis cache) → null returned.
    [Fact]
    public async Task CVerify72_UTCID03_GetSyncStatus_JobNotFound_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var ghostJobId = Guid.NewGuid();

        _cacheService
            .Setup(c => c.GetAsync<RepositorySyncJobStatus>(It.IsAny<string>()))
            .ReturnsAsync((RepositorySyncJobStatus?)null);

        var result = await _sut.GetSyncStatusAsync(userId, ghostJobId);

        result.Should().BeNull("no cached status for unknown job");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Correct job exists but wrong userId (ownership check fails) → null returned.
    [Fact]
    public async Task CVerify72_UTCID04_GetSyncStatus_WrongOwner_ReturnsNull()
    {
        var realOwnerId = Guid.NewGuid();
        var intruderUserId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        MockCachedStatus(jobId, realOwnerId, "Completed"); // cached under realOwnerId

        var result = await _sut.GetSyncStatusAsync(intruderUserId, jobId);

        result.Should().BeNull("job ownership mismatch must return null");
    }
}
