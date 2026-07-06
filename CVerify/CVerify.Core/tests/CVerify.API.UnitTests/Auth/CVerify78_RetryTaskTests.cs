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
/// Unit tests for RepositoryAnalysisService.RetryTaskAsync — CVerify-78 (5 UTCIDs).
/// POST /api/v1/repository-analysis/jobs/{jobId}/tasks/{taskId}/retry [Authorize].
/// Only terminal jobs (Completed/Failed/Cancelled/TimedOut) can have tasks retried.
/// </summary>
public sealed class CVerify78_RetryTaskTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IRepositoryAnalysisQueue> _queue = new();
    private readonly Mock<IConnectionMultiplexer> _redis = new();
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

    public CVerify78_RetryTaskTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _redis.Setup(r => r.GetSubscriber(It.IsAny<object>())).Returns(_redisSub.Object);
        _redisSub.Setup(s => s.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(0);

        _streamingSessionService
            .Setup(s => s.UpdateSessionStatusAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        _queue.Setup(q => q.EnqueueJobAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        _sut = new RepositoryAnalysisService(
            _context, _queue.Object, _redis.Object, _httpClientFactory.Object,
            _hmacService.Object, new EnvConfiguration(), _logger.Object, TimeProvider.System,
            _storageProvider.Object, _assessmentService.Object, _scopeFactory.Object,
            _streamingSessionService.Object, _cancellationManager.Object, _outboxPublisher.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid jobId, Guid taskId)> SeedFailedJobWithTaskAsync(string jobStatus = "Failed", string taskType = "SkillExtraction")
    {
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var repoId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider { Id = providerId, UserId = userId, ProviderName = "github", ProviderKey = "k", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Completed" });
        _context.SourceCodeRepositories.Add(new SourceCodeRepository { Id = repoId, AuthProviderId = providerId, ExternalRepositoryId = "ext", Name = "repo", Owner = "o", OwnerLogin = "o", OwnerType = "User", IsPrivate = false, IsAccessible = true, CreatedAtUtc = DateTimeOffset.UtcNow, LastSyncedAt = DateTimeOffset.UtcNow });
        _context.AnalysisJobs.Add(new AnalysisJob { Id = jobId, RepositoryId = repoId, UserId = userId, Status = jobStatus, Progress = 50.0, CreatedAtUtc = DateTimeOffset.UtcNow, LastUpdatedUtc = DateTimeOffset.UtcNow });
        _context.AnalysisTasks.Add(new AnalysisTask { Id = taskId, JobId = jobId, TaskType = taskType, Status = "Failed", Progress = 0, RetryCount = 0, CreatedAtUtc = DateTimeOffset.UtcNow, LastUpdatedUtc = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();
        return (userId, jobId, taskId);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Failed job + valid task → task reset to Queued, returns true.
    [Fact]
    public async Task CVerify78_UTCID01_RetryTask_FailedJobValidTask_ReturnsTrueAndResetsTask()
    {
        var (userId, jobId, taskId) = await SeedFailedJobWithTaskAsync("Failed");

        var result = await _sut.RetryTaskAsync(userId, jobId, taskId);

        result.Should().BeTrue();
        var task = await _context.AnalysisTasks.FindAsync(taskId);
        task!.Status.Should().Be("Queued", "task is reset to Queued for retry");
        task.RetryCount.Should().Be(1, "retry count incremented");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Active (non-terminal) job → retry not allowed, returns false.
    [Fact]
    public async Task CVerify78_UTCID02_RetryTask_ActiveJob_ReturnsFalse()
    {
        var (userId, jobId, taskId) = await SeedFailedJobWithTaskAsync("RunningAgents");

        var result = await _sut.RetryTaskAsync(userId, jobId, taskId);

        result.Should().BeFalse("only terminal jobs can have tasks retried");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Non-existent jobId → returns false.
    [Fact]
    public async Task CVerify78_UTCID03_RetryTask_JobNotFound_ReturnsFalse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.RetryTaskAsync(userId, Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeFalse();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Valid failed job but non-existent taskId → returns false.
    [Fact]
    public async Task CVerify78_UTCID04_RetryTask_TaskNotFound_ReturnsFalse()
    {
        var (userId, jobId, _) = await SeedFailedJobWithTaskAsync("Failed");

        var result = await _sut.RetryTaskAsync(userId, jobId, Guid.NewGuid());

        result.Should().BeFalse("task doesn't exist for this job");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller 401. Service level: ghost userId → returns false (job not found).
    [Fact]
    public async Task CVerify78_UTCID05_RetryTask_NoJwtControllerLevel_ServiceReturnsFalse()
    {
        var (_, jobId, taskId) = await SeedFailedJobWithTaskAsync("Failed");
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.RetryTaskAsync(ghostUserId, jobId, taskId);

        result.Should().BeFalse("ghost userId doesn't own the job");
    }
}
