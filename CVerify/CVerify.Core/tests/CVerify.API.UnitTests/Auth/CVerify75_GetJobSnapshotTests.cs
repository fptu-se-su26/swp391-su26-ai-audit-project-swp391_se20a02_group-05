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
/// Unit tests for RepositoryAnalysisService.GetJobSnapshotAsync — CVerify-75 (4 UTCIDs).
/// GET /api/v1/repository-analysis/jobs/{jobId}/snapshot [Authorize] — returns current job snapshot.
/// Completed jobs return the persisted report; incomplete jobs return a progress report.
/// </summary>
public sealed class CVerify75_GetJobSnapshotTests : IDisposable
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

    public CVerify75_GetJobSnapshotTests()
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
    // Non-existent jobId → null returned.
    [Fact]
    public async Task CVerify75_UTCID01_GetJobSnapshot_JobNotFound_ReturnsNull()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetJobSnapshotAsync(userId, Guid.NewGuid());

        result.Should().BeNull("no job exists for this user");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Job exists but status='Queued' and no RepoStructure task completed → default progress report returned.
    [Fact]
    public async Task CVerify75_UTCID02_GetJobSnapshot_QueuedJob_ReturnsDefaultProgressReport()
    {
        var (userId, _, jobId) = await SeedJobAsync("Queued");

        var result = await _sut.GetJobSnapshotAsync(userId, jobId);

        result.Should().NotBeNull("a default progress report is returned for unstarted jobs");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Completed job with persisted AnalysisReport → report data string returned.
    [Fact]
    public async Task CVerify75_UTCID03_GetJobSnapshot_CompletedJob_ReturnsFinalReport()
    {
        var (userId, repoId, jobId) = await SeedJobAsync("Completed");
        var reportData = "{\"score\": 90}";
        _context.AnalysisReports.Add(new AnalysisReport
        {
            Id = Guid.NewGuid(), JobId = jobId, RepositoryId = repoId,
            ReportData = reportData, CreatedAtUtc = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();

        var result = await _sut.GetJobSnapshotAsync(userId, jobId);

        result.Should().Be(reportData, "completed job with persisted report returns report data directly");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Wrong userId (ownership check) → null returned.
    [Fact]
    public async Task CVerify75_UTCID04_GetJobSnapshot_WrongOwner_ReturnsNull()
    {
        var (_, _, jobId) = await SeedJobAsync("Completed");
        var intruder = Guid.NewGuid();

        var result = await _sut.GetJobSnapshotAsync(intruder, jobId);

        result.Should().BeNull("job belongs to a different user");
    }
}
