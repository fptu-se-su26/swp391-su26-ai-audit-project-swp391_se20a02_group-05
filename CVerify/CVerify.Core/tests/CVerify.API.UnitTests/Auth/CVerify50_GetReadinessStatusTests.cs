using System;
using System.Collections.Generic;
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
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for CandidateAssessmentService.GetReadinessStatusAsync — CVerify-50 (4 UTCIDs).
/// GET /api/v1/candidate-assessments/readiness [Authorize] — returns candidate readiness status.
/// isReady = true only when ALL required fields are satisfied (only Repositories is required).
/// </summary>
public sealed class CVerify50_GetReadinessStatusTests : IDisposable
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

    public CVerify50_GetReadinessStatusTests()
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

        _sut = new CandidateAssessmentService(
            _context, _queue.Object, _httpClientFactory.Object, _hmacService.Object,
            _redis.Object, _repositoryProvider.Object, _logger.Object,
            _evaluationService.Object, _validationService.Object,
            _streamingSessionService.Object, _cancellationManager.Object,
            _rankingProjection.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task SeedCompleteProfileAsync(Guid userId)
    {
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
        _context.UserSkills.Add(new UserSkill
        {
            Id = Guid.NewGuid(), UserId = userId, Skill = "C#",
            CreatedAt = DateTimeOffset.UtcNow,
        });
        _context.EducationEntries.Add(new EducationEntry
        {
            Id = Guid.NewGuid(), UserId = userId, Label = "Bachelor",
            SchoolName = "FPT University", IsCurrentlyStudying = false,
            DisplayOrder = 0, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow,
        });
        _context.WorkExperiences.Add(new WorkExperienceEntry
        {
            Id = Guid.NewGuid(), UserId = userId, JobTitle = "Engineer", Company = "Corp",
            ExperienceCategory = ExperienceCategory.ProfessionalWork, EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero), IsCurrentlyWorking = false,
            Description = "Work", DisplayOrder = 0, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Complete profile (education+work+headline+bio+skills) + repos available → isReady:true
    [Fact]
    public async Task CVerify50_UTCID01_GetReadiness_CompleteProfile_ReturnsIsReadyTrue()
    {
        var userId = Guid.NewGuid();
        await SeedCompleteProfileAsync(userId);

        _repositoryProvider
            .Setup(r => r.HasCompletedRepositoriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GetReadinessStatusAsync(userId);

        result.Should().NotBeNull();
        result.IsReady.Should().BeTrue("all required fields including repositories are satisfied");
        result.MissingFields.Should().BeEmpty("no missing required or optional fields");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Incomplete profile (no repos, no headline, no bio, no skills, no education, no work) →
    // isReady:false, multiple missing fields including required Repositories
    [Fact]
    public async Task CVerify50_UTCID02_GetReadiness_IncompleteProfile_ReturnsIsReadyFalse()
    {
        var userId = Guid.NewGuid();

        _repositoryProvider
            .Setup(r => r.HasCompletedRepositoriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.GetReadinessStatusAsync(userId);

        result.Should().NotBeNull();
        result.IsReady.Should().BeFalse("Repositories is a required field and is missing");
        result.MissingFields.Should().Contain(mf => mf.FieldKey == "Repositories");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → no profile, no repos → returns readiness with isReady=false.
    [Fact]
    public async Task CVerify50_UTCID03_GetReadiness_NoJwtControllerLevel_ServiceReturnsNotReady()
    {
        var ghostUserId = Guid.NewGuid();

        _repositoryProvider
            .Setup(r => r.HasCompletedRepositoriesAsync(ghostUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.GetReadinessStatusAsync(ghostUserId);

        result.Should().NotBeNull("JWT auth is controller responsibility — service runs regardless");
        result.IsReady.Should().BeFalse("ghost user has no completed repositories");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Profile at minimum threshold: has repos (required) but missing all optional fields →
    // isReady:true (repos satisfied), but low completeness score
    [Fact]
    public async Task CVerify50_UTCID04_GetReadiness_MinimumThreshold_ReturnsIsReadyTrueWithLowScore()
    {
        var userId = Guid.NewGuid();

        _repositoryProvider
            .Setup(r => r.HasCompletedRepositoriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GetReadinessStatusAsync(userId);

        result.Should().NotBeNull();
        result.IsReady.Should().BeTrue("repositories (the only required field) are present");
        result.CompletenessScore.Should().BeLessThan(100, "optional fields like headline, bio, skills are missing");
        result.MissingFields.Should().NotBeEmpty("optional fields are missing even though isReady is true");
    }
}
