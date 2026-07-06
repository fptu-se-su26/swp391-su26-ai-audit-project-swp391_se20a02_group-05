using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;
using CVerify.API.Modules.Auth.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.System.Services;
using CVerify.API.Modules.SourceCode.Entities;
using CVerify.API.Modules.SourceCode.Services;
using CVerify.API.Pipelines.Shared.Storage;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for RepositoryAnalysisService.CancelJobAsync — CVerify-77 (5 UTCIDs).
/// DELETE /api/v1/repository-analysis/jobs/{jobId} [Authorize] — cancels an active analysis job.
/// Returns true for active jobs, false for non-active or non-existent jobs.
/// </summary>
public sealed class CVerify77_CancelJobTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IRepositoryAnalysisQueue> _queue = new();
    private readonly Mock<IConnectionMultiplexer> _redis = new();
    private readonly Mock<IDatabase> _redisDb = new();
    private readonly Mock<ISubscriber> _redisSub = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<IHmacSignatureService> _hmacService = new();
    private readonly Mock<ILogger<RepositoryAnalysisService>> _logger = new();
    private readonly Mock<IArtifactStorageProvider> _storageProvider = new();
    private readonly Mock<ICandidateAssessmentService> _assessmentService = new();
    private readonly Mock<IServiceScopeFactory> _scopeFactory = new();
    private readonly Mock<IAiStreamingSessionService> _streamingSessionService = new();
    private readonly Mock<IAiCancellationManager> _cancellationManager = new();
    private readonly Mock<IOutboxPublisher> _outboxPublisher = new();
    private readonly RepositoryAnalysisService _sut;

    public CVerify77_CancelJobTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _redis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDb.Object);
        _redis.Setup(r => r.GetSubscriber(It.IsAny<object>())).Returns(_redisSub.Object);
        _redisDb.Setup(d => d.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
        _redisSub.Setup(s => s.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(0);

        _streamingSessionService
            .Setup(s => s.UpdateSessionStatusAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        _sut = new RepositoryAnalysisService(
            _context, _queue.Object, _redis.Object, _httpClientFactory.Object,
            _hmacService.Object, new EnvConfiguration(), _logger.Object, TimeProvider.System,
            _storageProvider.Object, _assessmentService.Object, _scopeFactory.Object,
            _streamingSessionService.Object, _cancellationManager.Object, _outboxPublisher.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid repoId, Guid jobId)> SeedJobAsync(string status)
    {
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var repoId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider { Id = providerId, UserId = userId, ProviderName = "github", ProviderKey = "k", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Completed" });
        _context.SourceCodeRepositories.Add(new SourceCodeRepository { Id = repoId, AuthProviderId = providerId, ExternalRepositoryId = "ext", Name = "repo", Owner = "o", OwnerLogin = "o", OwnerType = "User", IsPrivate = false, IsAccessible = true, CreatedAtUtc = DateTimeOffset.UtcNow, LastSyncedAt = DateTimeOffset.UtcNow });
        _context.AnalysisJobs.Add(new AnalysisJob { Id = jobId, RepositoryId = repoId, UserId = userId, Status = status, Progress = 30.0, CreatedAtUtc = DateTimeOffset.UtcNow, LastUpdatedUtc = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();
        return (userId, repoId, jobId);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Job in active 'Queued' state → cancelled, returns true.
    [Fact]
    public async Task CVerify77_UTCID01_CancelJob_QueuedJob_ReturnsTrueAndCancels()
    {
        var (userId, _, jobId) = await SeedJobAsync("Queued");

        var result = await _sut.CancelJobAsync(userId, jobId);

        result.Should().BeTrue();
        var job = await _context.AnalysisJobs.FindAsync(jobId);
        job!.Status.Should().Be("Cancelled");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Job in terminal 'Completed' state → not cancellable, returns false.
    [Fact]
    public async Task CVerify77_UTCID02_CancelJob_CompletedJob_ReturnsFalse()
    {
        var (userId, _, jobId) = await SeedJobAsync("Completed");

        var result = await _sut.CancelJobAsync(userId, jobId);

        result.Should().BeFalse("completed jobs cannot be cancelled");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Non-existent jobId → returns false.
    [Fact]
    public async Task CVerify77_UTCID03_CancelJob_JobNotFound_ReturnsFalse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CancelJobAsync(userId, Guid.NewGuid());

        result.Should().BeFalse("no job found → safe false return");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Different userId (ownership mismatch) → returns false.
    [Fact]
    public async Task CVerify77_UTCID04_CancelJob_WrongOwner_ReturnsFalse()
    {
        var (_, _, jobId) = await SeedJobAsync("Queued");
        var intruder = Guid.NewGuid();

        var result = await _sut.CancelJobAsync(intruder, jobId);

        result.Should().BeFalse("job belongs to a different user");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // Job in active 'RunningAgents' state → cancelled, cancellation manager notified.
    [Fact]
    public async Task CVerify77_UTCID05_CancelJob_RunningJob_ReturnsTrueAndNotifiesManager()
    {
        var (userId, _, jobId) = await SeedJobAsync("RunningAgents");

        var result = await _sut.CancelJobAsync(userId, jobId);

        result.Should().BeTrue();
        _cancellationManager.Verify(m => m.Cancel(jobId), Times.Once,
            "cancellation manager must be notified to stop the running analysis");
    }
}
