using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;
using CVerify.API.Modules.Intelligence.Services;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for CandidateAssessmentService.TriggerAssessmentAsync — CVerify-51 (5 UTCIDs).
/// POST /api/v1/candidate-assessments [Authorize] — triggers a new candidate assessment pipeline.
/// </summary>
public sealed class CVerify51_TriggerAssessmentTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICandidateAssessmentQueue> _queue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<IHmacSignatureService> _hmacService = new();
    private readonly Mock<IConnectionMultiplexer> _redis = new();
    private readonly Mock<ICandidateRepositoryProvider> _repositoryProvider = new();
    private readonly Mock<ILogger<CandidateAssessmentService>> _logger = new();
    private readonly Mock<ICandidateEvaluationService> _evaluationService = new();
    private readonly Mock<ISkillTreeValidationService> _validationService = new();
    private readonly Mock<IAiStreamingSessionService> _streamingSessionService = new();
    private readonly Mock<IAiCancellationManager> _cancellationManager = new();
    private readonly Mock<ICandidateRankingProjectionService> _rankingProjection = new();

    private readonly CandidateAssessmentService _sut;

    public CVerify51_TriggerAssessmentTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _hmacService
            .Setup(h => h.CreateSignatureHeaders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(("mock-sig", "12345", "nonce"));

        _repositoryProvider
            .Setup(r => r.GetLastRepositoryAnalysisAtAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DateTimeOffset.MinValue);

        _streamingSessionService
            .Setup(s => s.CreateSessionAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((CVerify.API.Modules.Shared.Domain.Entities.AiStreamingSession)null!);

        _queue
            .Setup(q => q.EnqueueAssessmentAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        _sut = new CandidateAssessmentService(
            _context, _queue.Object, _httpClientFactory.Object, _hmacService.Object,
            _redis.Object, _repositoryProvider.Object, _logger.Object,
            _evaluationService.Object, _validationService.Object,
            _streamingSessionService.Object, _cancellationManager.Object,
            _rankingProjection.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<Guid> SeedReadyUserAsync()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId, Email = $"{userId}@test.com", FullName = "Test User",
            Username = $"user{userId:N}", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        });
        _context.UserProfiles.Add(new UserProfile
        {
            UserId = userId, Username = $"user{userId:N}",
            Headline = "Software Engineer", Bio = "Experienced developer",
            ProfileVisibility = "public", RecruiterVisibility = true, AiTalentDiscovery = "disabled",
        });
        await _context.SaveChangesAsync();

        _repositoryProvider
            .Setup(r => r.HasCompletedRepositoriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return userId;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid JWT + profile ready + repos → 202 Accepted { jobId, status:'Queued' }
    [Fact]
    public async Task CVerify51_UTCID01_TriggerAssessment_ReadyProfile_ReturnsQueuedAssessment()
    {
        var userId = await SeedReadyUserAsync();

        var result = await _sut.TriggerAssessmentAsync(userId);

        result.Should().NotBeNull();
        result.Status.Should().Be("Queued");
        result.UserId.Should().Be(userId);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Profile incomplete (no repos) → 400 Bad Request (BusinessRuleException PROFILE_INCOMPLETE)
    [Fact]
    public async Task CVerify51_UTCID02_TriggerAssessment_IncompleteProfile_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId, Email = $"{userId}@test.com", FullName = "Test User",
            Username = $"user{userId:N}", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        });
        _context.UserProfiles.Add(new UserProfile
        {
            UserId = userId, Username = $"user{userId:N}",
            ProfileVisibility = "public", RecruiterVisibility = true, AiTalentDiscovery = "disabled",
        });
        await _context.SaveChangesAsync();

        _repositoryProvider
            .Setup(r => r.HasCompletedRepositoriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.TriggerAssessmentAsync(userId);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*repository*", "profile must have completed repos to trigger assessment");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Assessment already Queued/Running → 409 Conflict (BusinessRuleException ASSESSMENT_ALREADY_ACTIVE)
    [Fact]
    public async Task CVerify51_UTCID03_TriggerAssessment_AssessmentAlreadyActive_ThrowsConflict()
    {
        var userId = await SeedReadyUserAsync();

        _context.CandidateAssessments.Add(new CandidateAssessment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "Queued",
            PipelineVersion = "2.2.0",
            AssessmentSchemaVersion = "1.2.0",
            LastProfileUpdateAt = DateTimeOffset.UtcNow,
            LastRepositoryAnalysisAt = DateTimeOffset.MinValue,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.TriggerAssessmentAsync(userId);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already*");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId (no profile) → ResourceNotFoundException.
    [Fact]
    public async Task CVerify51_UTCID04_TriggerAssessment_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var ghostUserId = Guid.NewGuid();

        var act = async () => await _sut.TriggerAssessmentAsync(ghostUserId);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // Previous assessment Completed → new assessment queued (boundary case)
    [Fact]
    public async Task CVerify51_UTCID05_TriggerAssessment_AfterCompletedAssessment_ReturnsNewQueuedAssessment()
    {
        var userId = await SeedReadyUserAsync();

        _context.CandidateAssessments.Add(new CandidateAssessment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "Completed",
            PipelineVersion = "2.2.0",
            AssessmentSchemaVersion = "1.2.0",
            LastProfileUpdateAt = DateTimeOffset.UtcNow,
            LastRepositoryAnalysisAt = DateTimeOffset.MinValue,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            CompletedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            Version = 1,
        });
        await _context.SaveChangesAsync();

        var result = await _sut.TriggerAssessmentAsync(userId);

        result.Should().NotBeNull();
        result.Status.Should().Be("Queued");
        result.UserId.Should().Be(userId);
    }
}
