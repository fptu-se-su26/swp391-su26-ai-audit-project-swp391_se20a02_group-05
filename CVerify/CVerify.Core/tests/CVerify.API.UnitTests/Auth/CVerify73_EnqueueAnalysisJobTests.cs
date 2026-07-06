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
/// Unit tests for RepositoryAnalysisService.EnqueueAnalysisJobAsync — CVerify-73 (5 UTCIDs).
/// POST /api/v1/repository-analysis/{repositoryId} [Authorize] — enqueues a repository analysis job.
/// Limit: max 2 concurrent active jobs per user.
/// </summary>
public sealed class CVerify73_EnqueueAnalysisJobTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IRepositoryAnalysisQueue> _queue = new();
    private readonly Mock<IConnectionMultiplexer> _redis = new();
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

    public CVerify73_EnqueueAnalysisJobTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _queue.Setup(q => q.EnqueueJobAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        _streamingSessionService.Setup(s => s.CreateSessionAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((CVerify.API.Modules.Shared.Domain.Entities.AiStreamingSession)null!);

        _sut = new RepositoryAnalysisService(
            _context, _queue.Object, _redis.Object, _httpClientFactory.Object,
            _hmacService.Object, new EnvConfiguration(), _logger.Object, TimeProvider.System,
            _storageProvider.Object, _assessmentService.Object, _scopeFactory.Object,
            _streamingSessionService.Object, _cancellationManager.Object, _outboxPublisher.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid providerId, Guid repoId)> SeedRepoAsync()
    {
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var repoId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider
        {
            Id = providerId, UserId = userId, ProviderName = "github",
            ProviderKey = $"key-{userId}", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Completed",
        });
        _context.SourceCodeRepositories.Add(new SourceCodeRepository
        {
            Id = repoId, AuthProviderId = providerId,
            ExternalRepositoryId = "ext-123", Name = "my-repo",
            Owner = "owner", OwnerLogin = "ownerlogin", OwnerType = "User", IsPrivate = false,
            IsAccessible = true, CreatedAtUtc = DateTimeOffset.UtcNow, LastSyncedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
        return (userId, providerId, repoId);
    }

    private void SeedActiveJob(Guid userId, Guid repoId, string status = "Queued")
    {
        _context.AnalysisJobs.Add(new AnalysisJob
        {
            Id = Guid.NewGuid(), RepositoryId = repoId, UserId = userId,
            Status = status, CreatedAtUtc = DateTimeOffset.UtcNow, LastUpdatedUtc = DateTimeOffset.UtcNow,
        });
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid user + own repo → Guid jobId returned, job persisted, queue called.
    [Fact]
    public async Task CVerify73_UTCID01_EnqueueAnalysis_ValidRepo_ReturnsJobId()
    {
        var (userId, _, repoId) = await SeedRepoAsync();

        var jobId = await _sut.EnqueueAnalysisJobAsync(userId, repoId);

        jobId.Should().NotBeEmpty();
        var job = await _context.AnalysisJobs.FirstOrDefaultAsync(j => j.Id == jobId);
        job.Should().NotBeNull("job should be persisted in DB");
        _queue.Verify(q => q.EnqueueJobAsync(jobId), Times.Once);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent repositoryId → KeyNotFoundException.
    [Fact]
    public async Task CVerify73_UTCID02_EnqueueAnalysis_RepoNotFound_ThrowsKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        var ghostRepoId = Guid.NewGuid();

        var act = async () => await _sut.EnqueueAnalysisJobAsync(userId, ghostRepoId);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // User has 2 active jobs → InvalidOperationException (limit exceeded).
    [Fact]
    public async Task CVerify73_UTCID03_EnqueueAnalysis_LimitExceeded_ThrowsInvalidOperationException()
    {
        var (userId, providerId, _) = await SeedRepoAsync();

        // Seed 2 more repos and their active jobs to hit the per-user limit
        var repo2Id = Guid.NewGuid();
        var repo3Id = Guid.NewGuid();
        _context.SourceCodeRepositories.Add(new SourceCodeRepository { Id = repo2Id, AuthProviderId = providerId, ExternalRepositoryId = "ext-2", Name = "repo2", Owner = "owner", OwnerLogin = "ownerlogin", OwnerType = "User", IsPrivate = false, IsAccessible = true, CreatedAtUtc = DateTimeOffset.UtcNow, LastSyncedAt = DateTimeOffset.UtcNow });
        _context.SourceCodeRepositories.Add(new SourceCodeRepository { Id = repo3Id, AuthProviderId = providerId, ExternalRepositoryId = "ext-3", Name = "repo3", Owner = "owner", OwnerLogin = "ownerlogin", OwnerType = "User", IsPrivate = false, IsAccessible = true, CreatedAtUtc = DateTimeOffset.UtcNow, LastSyncedAt = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();

        SeedActiveJob(userId, repo2Id, "Queued");
        SeedActiveJob(userId, repo3Id, "Preparing");
        await _context.SaveChangesAsync();

        // Now try to enqueue a 3rd repo analysis for this user
        var newRepoId = Guid.NewGuid();
        _context.SourceCodeRepositories.Add(new SourceCodeRepository { Id = newRepoId, AuthProviderId = providerId, ExternalRepositoryId = "ext-new", Name = "repo-new", Owner = "owner", OwnerLogin = "ownerlogin", OwnerType = "User", IsPrivate = false, IsAccessible = true, CreatedAtUtc = DateTimeOffset.UtcNow, LastSyncedAt = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.EnqueueAnalysisJobAsync(userId, newRepoId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*limit*", "user exceeded the 2-job concurrent analysis limit");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Repository belongs to another user → KeyNotFoundException.
    [Fact]
    public async Task CVerify73_UTCID04_EnqueueAnalysis_OtherUsersRepo_ThrowsKeyNotFoundException()
    {
        var (_, _, repoId) = await SeedRepoAsync();
        var differentUserId = Guid.NewGuid();

        var act = async () => await _sut.EnqueueAnalysisJobAsync(differentUserId, repoId);

        await act.Should().ThrowAsync<KeyNotFoundException>("repo ownership doesn't match");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // Active job already exists for this repo → returns existing jobId (idempotent).
    [Fact]
    public async Task CVerify73_UTCID05_EnqueueAnalysis_ActiveJobExists_ReturnsExistingJobId()
    {
        var (userId, _, repoId) = await SeedRepoAsync();
        var existingJobId = Guid.NewGuid();
        _context.AnalysisJobs.Add(new AnalysisJob
        {
            Id = existingJobId, RepositoryId = repoId, UserId = userId,
            Status = "Queued", CreatedAtUtc = DateTimeOffset.UtcNow, LastUpdatedUtc = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();

        var jobId = await _sut.EnqueueAnalysisJobAsync(userId, repoId);

        jobId.Should().Be(existingJobId, "existing active job should be returned (idempotent)");
        _queue.Verify(q => q.EnqueueJobAsync(It.IsAny<Guid>()), Times.Never, "no new job should be enqueued");
    }
}
