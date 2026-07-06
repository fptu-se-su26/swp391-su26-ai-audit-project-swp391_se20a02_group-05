using System;
using System.Net.Http;
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
/// Unit tests for RepositoryAnalysisService.GetJobStatusAsync — CVerify-74 (5 UTCIDs).
/// GET /api/v1/repository-analysis/jobs/{jobId} [Authorize] — returns analysis job status.
/// </summary>
public sealed class CVerify74_GetJobStatusTests : IDisposable
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

    public CVerify74_GetJobStatusTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

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
        _context.AnalysisJobs.Add(new AnalysisJob { Id = jobId, RepositoryId = repoId, UserId = userId, Status = status, CreatedAtUtc = DateTimeOffset.UtcNow, LastUpdatedUtc = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();
        return (userId, repoId, jobId);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Job with status='Queued' → AnalysisJobDto returned with matching status.
    [Fact]
    public async Task CVerify74_UTCID01_GetJobStatus_QueuedJob_ReturnsDto()
    {
        var (userId, _, jobId) = await SeedJobAsync("Queued");

        var result = await _sut.GetJobStatusAsync(userId, jobId);

        result.Should().NotBeNull();
        result!.Status.Should().Be("Queued");
        result.Id.Should().Be(jobId);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Job with status='CloningRepository' → AnalysisJobDto with correct status.
    [Fact]
    public async Task CVerify74_UTCID02_GetJobStatus_CloningStatus_ReturnsDto()
    {
        var (userId, _, jobId) = await SeedJobAsync("CloningRepository");

        var result = await _sut.GetJobStatusAsync(userId, jobId);

        result.Should().NotBeNull();
        result!.Status.Should().Be("CloningRepository");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Job with status='Completed' → AnalysisJobDto with Completed status.
    [Fact]
    public async Task CVerify74_UTCID03_GetJobStatus_CompletedJob_ReturnsDto()
    {
        var (userId, _, jobId) = await SeedJobAsync("Completed");

        var result = await _sut.GetJobStatusAsync(userId, jobId);

        result.Should().NotBeNull();
        result!.Status.Should().Be("Completed");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Non-existent jobId → null returned.
    [Fact]
    public async Task CVerify74_UTCID04_GetJobStatus_JobNotFound_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var ghostJobId = Guid.NewGuid();

        var result = await _sut.GetJobStatusAsync(userId, ghostJobId);

        result.Should().BeNull("no job with that ID exists for this user");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // Correct jobId but different userId → null returned (ownership enforced).
    [Fact]
    public async Task CVerify74_UTCID05_GetJobStatus_WrongOwner_ReturnsNull()
    {
        var (realOwnerId, _, jobId) = await SeedJobAsync("Running");
        var intruder = Guid.NewGuid();

        var result = await _sut.GetJobStatusAsync(intruder, jobId);

        result.Should().BeNull("job belongs to a different user");
    }
}
