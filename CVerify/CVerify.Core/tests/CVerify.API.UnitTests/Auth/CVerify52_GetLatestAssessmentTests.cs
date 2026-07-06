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
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for CandidateAssessmentService.GetLatestAssessmentAsync — CVerify-52 (4 UTCIDs).
/// GET /api/v1/candidate-assessments/latest [Authorize] — returns the most recent assessment or null.
/// </summary>
public sealed class CVerify52_GetLatestAssessmentTests : IDisposable
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

    public CVerify52_GetLatestAssessmentTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _hmacService
            .Setup(h => h.CreateSignatureHeaders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(("mock-sig", "12345", "nonce"));

        _sut = new CandidateAssessmentService(
            _context, _queue.Object, _httpClientFactory.Object, _hmacService.Object,
            _redis.Object, _repositoryProvider.Object, _logger.Object,
            _evaluationService.Object, _validationService.Object,
            _streamingSessionService.Object, _cancellationManager.Object,
            _rankingProjection.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid assessmentId)> SeedAssessmentAsync(string status = "Completed")
    {
        var userId = Guid.NewGuid();
        var assessment = new CandidateAssessment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = status,
            PipelineVersion = "2.2.0",
            AssessmentSchemaVersion = "1.2.0",
            LastProfileUpdateAt = DateTimeOffset.UtcNow,
            LastRepositoryAnalysisAt = DateTimeOffset.MinValue,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CompletedAtUtc = status == "Completed" ? DateTimeOffset.UtcNow : null,
        };
        _context.CandidateAssessments.Add(assessment);
        await _context.SaveChangesAsync();
        return (userId, assessment.Id);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid JWT – user has completed assessment → 200 CandidateAssessmentResponse
    [Fact]
    public async Task CVerify52_UTCID01_GetLatestAssessment_CompletedAssessment_ReturnsResponse()
    {
        var (userId, assessmentId) = await SeedAssessmentAsync("Completed");

        var result = await _sut.GetLatestAssessmentAsync(userId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(assessmentId);
        result.Status.Should().Be("Completed");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Valid JWT – assessment in Processing/Running state → 200 CandidateAssessmentResponse
    [Fact]
    public async Task CVerify52_UTCID02_GetLatestAssessment_RunningAssessment_ReturnsResponse()
    {
        var (userId, assessmentId) = await SeedAssessmentAsync("Running");

        var result = await _sut.GetLatestAssessmentAsync(userId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(assessmentId);
        result.Status.Should().Be("Running");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Valid JWT – no assessments ever triggered → 204 No Content (service returns null)
    [Fact]
    public async Task CVerify52_UTCID03_GetLatestAssessment_NoAssessments_ReturnsNull()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetLatestAssessmentAsync(userId);

        result.Should().BeNull("no assessments exist for this user");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → no assessments → returns null.
    [Fact]
    public async Task CVerify52_UTCID04_GetLatestAssessment_NoJwtControllerLevel_ServiceReturnsNull()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetLatestAssessmentAsync(ghostUserId);

        result.Should().BeNull("JWT auth is controller responsibility; service returns null for ghost user");
    }
}
