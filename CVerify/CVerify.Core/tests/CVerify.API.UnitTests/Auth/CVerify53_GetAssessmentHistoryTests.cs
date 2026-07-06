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
/// Unit tests for CandidateAssessmentService.GetAssessmentHistoryAsync — CVerify-53 (3 UTCIDs).
/// GET /api/v1/candidate-assessments/history [Authorize] — returns all past assessments.
/// </summary>
public sealed class CVerify53_GetAssessmentHistoryTests : IDisposable
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

    public CVerify53_GetAssessmentHistoryTests()
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

    private CandidateAssessment MakeAssessment(Guid userId, string status, DateTimeOffset createdAt) =>
        new CandidateAssessment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = status,
            PipelineVersion = "2.2.0",
            AssessmentSchemaVersion = "1.2.0",
            LastProfileUpdateAt = DateTimeOffset.UtcNow,
            LastRepositoryAnalysisAt = DateTimeOffset.MinValue,
            CreatedAtUtc = createdAt,
            CompletedAtUtc = status == "Completed" ? createdAt.AddMinutes(30) : null,
        };

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid JWT – user has multiple assessments → 200 OK – CandidateAssessmentResponse[]
    [Fact]
    public async Task CVerify53_UTCID01_GetAssessmentHistory_MultipleAssessments_ReturnsAll()
    {
        var userId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        _context.CandidateAssessments.AddRange(
            MakeAssessment(userId, "Completed", now.AddDays(-2)),
            MakeAssessment(userId, "Completed", now.AddDays(-1)));
        await _context.SaveChangesAsync();

        var result = await _sut.GetAssessmentHistoryAsync(userId);

        result.Should().HaveCount(2);
        result[0].CreatedAtUtc.Should().BeAfter(result[1].CreatedAtUtc, "ordered descending by CreatedAtUtc");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Valid JWT – no assessment history → 200 OK – empty list
    [Fact]
    public async Task CVerify53_UTCID02_GetAssessmentHistory_NoHistory_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetAssessmentHistoryAsync(userId);

        result.Should().BeEmpty();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → returns empty list.
    [Fact]
    public async Task CVerify53_UTCID03_GetAssessmentHistory_NoJwtControllerLevel_ServiceReturnsEmpty()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetAssessmentHistoryAsync(ghostUserId);

        result.Should().BeEmpty("JWT auth is controller responsibility; service returns empty list for ghost user");
    }
}
