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
/// Unit tests for CandidateAssessmentService.GetAssessmentDetailsAsync — CVerify-54 (4 UTCIDs).
/// GET /api/v1/candidate-assessments/{assessmentId}/details [Authorize] — returns assessment details.
/// Returns null (404) when not found or when accessing another user's assessment.
/// </summary>
public sealed class CVerify54_GetAssessmentDetailsTests : IDisposable
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

    public CVerify54_GetAssessmentDetailsTests()
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

    private async Task<(Guid userId, Guid assessmentId)> SeedAsync()
    {
        var userId = Guid.NewGuid();
        var assessment = new CandidateAssessment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "Completed",
            OverallScore = 78.5,
            PipelineVersion = "2.2.0",
            AssessmentSchemaVersion = "1.2.0",
            LastProfileUpdateAt = DateTimeOffset.UtcNow,
            LastRepositoryAnalysisAt = DateTimeOffset.MinValue,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CompletedAtUtc = DateTimeOffset.UtcNow,
        };
        _context.CandidateAssessments.Add(assessment);
        await _context.SaveChangesAsync();
        return (userId, assessment.Id);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // id: own completed assessment GUID → 200 OK – CandidateAssessmentDetailResponse
    [Fact]
    public async Task CVerify54_UTCID01_GetAssessmentDetails_OwnAssessment_ReturnsDetailResponse()
    {
        var (userId, assessmentId) = await SeedAsync();

        var result = await _sut.GetAssessmentDetailsAsync(userId, assessmentId);

        result.Should().NotBeNull();
        result!.Assessment.Id.Should().Be(assessmentId);
        result.Assessment.Status.Should().Be("Completed");
        result.Artifacts.Should().NotBeNull();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // id: non-existent GUID → 404 Not Found (service returns null)
    [Fact]
    public async Task CVerify54_UTCID02_GetAssessmentDetails_NonExistentId_ReturnsNull()
    {
        var (userId, _) = await SeedAsync();

        var result = await _sut.GetAssessmentDetailsAsync(userId, Guid.NewGuid());

        result.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // id: another user's assessment GUID → 404 Not Found (service filters by userId)
    [Fact]
    public async Task CVerify54_UTCID03_GetAssessmentDetails_OtherUsersAssessment_ReturnsNull()
    {
        var (_, assessmentId) = await SeedAsync();
        var anotherUserId = Guid.NewGuid();

        var result = await _sut.GetAssessmentDetailsAsync(anotherUserId, assessmentId);

        result.Should().BeNull("service filters by userId AND assessmentId");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → returns null.
    [Fact]
    public async Task CVerify54_UTCID04_GetAssessmentDetails_NoJwtControllerLevel_ServiceReturnsNull()
    {
        var result = await _sut.GetAssessmentDetailsAsync(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeNull("JWT auth is controller responsibility; service returns null for ghost user");
    }
}
