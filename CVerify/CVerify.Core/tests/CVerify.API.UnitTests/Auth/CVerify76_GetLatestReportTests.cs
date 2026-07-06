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
/// Unit tests for RepositoryAnalysisService.GetLatestReportAsync — CVerify-76 (4 UTCIDs).
/// GET /api/v1/repository-analysis/repositories/{repositoryId}/report [Authorize] — latest analysis report.
/// </summary>
public sealed class CVerify76_GetLatestReportTests : IDisposable
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

    public CVerify76_GetLatestReportTests()
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

    private async Task<(Guid userId, Guid repoId)> SeedRepoAsync()
    {
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var repoId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider { Id = providerId, UserId = userId, ProviderName = "github", ProviderKey = "k", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Completed" });
        _context.SourceCodeRepositories.Add(new SourceCodeRepository { Id = repoId, AuthProviderId = providerId, ExternalRepositoryId = "ext", Name = "repo", Owner = "o", OwnerLogin = "o", OwnerType = "User", IsPrivate = false, IsAccessible = true, CreatedAtUtc = DateTimeOffset.UtcNow, LastSyncedAt = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();
        return (userId, repoId);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Repo with completed analysis → latest report data string returned.
    [Fact]
    public async Task CVerify76_UTCID01_GetLatestReport_RepoWithReport_ReturnsReportData()
    {
        var (userId, repoId) = await SeedRepoAsync();
        var jobId = Guid.NewGuid();
        _context.AnalysisJobs.Add(new AnalysisJob { Id = jobId, RepositoryId = repoId, UserId = userId, Status = "Completed", CreatedAtUtc = DateTimeOffset.UtcNow, LastUpdatedUtc = DateTimeOffset.UtcNow });
        _context.AnalysisReports.Add(new AnalysisReport { Id = Guid.NewGuid(), JobId = jobId, RepositoryId = repoId, ReportData = "{\"trust\":90}", CreatedAtUtc = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();

        var result = await _sut.GetLatestReportAsync(userId, repoId);

        result.Should().Be("{\"trust\":90}");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Repo exists but no analysis report → null returned.
    [Fact]
    public async Task CVerify76_UTCID02_GetLatestReport_RepoNoReport_ReturnsNull()
    {
        var (userId, repoId) = await SeedRepoAsync();

        var result = await _sut.GetLatestReportAsync(userId, repoId);

        result.Should().BeNull("no analysis report exists yet");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Non-existent repositoryId → KeyNotFoundException.
    [Fact]
    public async Task CVerify76_UTCID03_GetLatestReport_RepoNotFound_ThrowsKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        var ghostRepoId = Guid.NewGuid();

        var act = async () => await _sut.GetLatestReportAsync(userId, ghostRepoId);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Repo belongs to a different user → KeyNotFoundException (access denied).
    [Fact]
    public async Task CVerify76_UTCID04_GetLatestReport_WrongUser_ThrowsKeyNotFoundException()
    {
        var (_, repoId) = await SeedRepoAsync();
        var intruder = Guid.NewGuid();

        var act = async () => await _sut.GetLatestReportAsync(intruder, repoId);

        await act.Should().ThrowAsync<KeyNotFoundException>("repo ownership doesn't match");
    }
}
