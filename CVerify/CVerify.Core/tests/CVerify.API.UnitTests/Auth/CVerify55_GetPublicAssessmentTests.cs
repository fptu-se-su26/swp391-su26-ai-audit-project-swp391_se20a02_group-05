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
/// Unit tests for CandidateAssessmentService.GetLatestPublicAssessmentAsync — CVerify-55 (4 UTCIDs).
/// GET /api/v1/candidate-assessments/public/{username} [AllowAnonymous] — returns public assessment.
/// Filters by profileVisibility=="public" and Status=="Completed".
/// </summary>
public sealed class CVerify55_GetPublicAssessmentTests : IDisposable
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

    public CVerify55_GetPublicAssessmentTests()
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

    private async Task<Guid> SeedPublicUserWithAssessmentAsync(string username)
    {
        var userId = Guid.NewGuid();
        _context.UserProfiles.Add(new UserProfile
        {
            UserId = userId,
            Username = username,
            ProfileVisibility = "public",
            RecruiterVisibility = true,
            AiTalentDiscovery = "disabled",
        });
        _context.CandidateAssessments.Add(new CandidateAssessment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "Completed",
            OverallScore = 82.0,
            PipelineVersion = "2.2.0",
            AssessmentSchemaVersion = "1.2.0",
            LastProfileUpdateAt = DateTimeOffset.UtcNow,
            LastRepositoryAnalysisAt = DateTimeOffset.MinValue,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            CompletedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
        });
        await _context.SaveChangesAsync();
        return userId;
    }

    private async Task<Guid> SeedPublicUserNoAssessmentAsync(string username)
    {
        var userId = Guid.NewGuid();
        _context.UserProfiles.Add(new UserProfile
        {
            UserId = userId,
            Username = username,
            ProfileVisibility = "public",
            RecruiterVisibility = true,
            AiTalentDiscovery = "disabled",
        });
        await _context.SaveChangesAsync();
        return userId;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // username: 'johndoe' (has public profile + completed assessment) → 200 OK – CandidateAssessmentDetailResponse
    [Fact]
    public async Task CVerify55_UTCID01_GetPublicAssessment_PublicUserWithAssessment_ReturnsDetailResponse()
    {
        await SeedPublicUserWithAssessmentAsync("johndoe");

        var result = await _sut.GetLatestPublicAssessmentAsync("johndoe");

        result.Should().NotBeNull();
        result!.Assessment.Status.Should().Be("Completed");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // username: 'janedoe' (public profile but no assessment) → 204 No Content (service returns null)
    [Fact]
    public async Task CVerify55_UTCID02_GetPublicAssessment_PublicUserNoAssessment_ReturnsNull()
    {
        await SeedPublicUserNoAssessmentAsync("janedoe");

        var result = await _sut.GetLatestPublicAssessmentAsync("janedoe");

        result.Should().BeNull("profile exists but has no completed assessment");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // username: 'nonexistentuser' → 404 Not Found (profile not found)
    [Fact]
    public async Task CVerify55_UTCID03_GetPublicAssessment_NonExistentUsername_ReturnsNull()
    {
        var result = await _sut.GetLatestPublicAssessmentAsync("nonexistentuser");

        result.Should().BeNull("no profile with that username exists");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No Authorization header – endpoint is [AllowAnonymous], service works without auth.
    // Service-level behavior is identical to UTCID01 — anon access returns the same result.
    [Fact]
    public async Task CVerify55_UTCID04_GetPublicAssessment_AnonymousAccess_ReturnsPublicAssessment()
    {
        await SeedPublicUserWithAssessmentAsync("johndoe2");

        // AllowAnonymous: no userId context needed — service method accepts username directly
        var result = await _sut.GetLatestPublicAssessmentAsync("johndoe2");

        result.Should().NotBeNull("AllowAnonymous endpoint; service returns public data regardless of auth header");
        result!.Assessment.Status.Should().Be("Completed");
    }
}
