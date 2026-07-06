using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for CareerService.AcceptAiSuggestionsAsync — CVerify-59 (4 UTCIDs).
/// POST /api/v1/users/career/accept-suggestions [Authorize] — merges AI suggestions into career prefs.
/// Throws ResourceNotFoundException when no AI suggestions exist.
/// </summary>
public sealed class CVerify59_AcceptAiSuggestionsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICareerReadinessEngine> _readinessEngine = new();
    private readonly CareerService _sut;

    private static readonly CareerReadinessReportDto FakeReport = new(80, "Good", 85, new List<CareerReadinessActionItem>());

    public CVerify59_AcceptAiSuggestionsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _readinessEngine
            .Setup(e => e.CalculateReadinessAsync(It.IsAny<CareerPreference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeReport);

        _sut = new CareerService(_context, _readinessEngine.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, uint careerVersion)> SeedCareerWithAiSuggestionsAsync()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId, Email = $"{userId}@test.com", FullName = "Test User",
            Username = $"user{userId:N}", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        });
        var career = new CareerPreference
        {
            UserId = userId,
            AvailableForHire = true,
            PreferredLanguage = "en",
            OpenToWorkStatus = "casual",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Version = 0,
        };
        _context.CareerPreferences.Add(career);
        _context.AiInferredPreferences.Add(new AiInferredPreference
        {
            UserId = userId,
            InferredPrimaryRole = "Backend Engineer",
            InferredSeniority = "Mid",
            InferredSkills = new List<string> { "C#", "PostgreSQL" },
            ConfidenceScore = 0.85m,
            LastAnalyzedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
        return (userId, career.Version);
    }

    private async Task<(Guid userId, uint careerVersion)> SeedCareerWithoutAiSuggestionsAsync()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId, Email = $"{userId}@test.com", FullName = "Test2",
            Username = $"user{userId:N}", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        });
        var career = new CareerPreference
        {
            UserId = userId,
            AvailableForHire = true,
            PreferredLanguage = "en",
            OpenToWorkStatus = "casual",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Version = 0,
        };
        _context.CareerPreferences.Add(career);
        await _context.SaveChangesAsync();
        return (userId, career.Version);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // acceptRoles:true, acceptSkills:true + AI suggestions exist → 200 OK
    [Fact]
    public async Task CVerify59_UTCID01_AcceptAiSuggestions_AcceptAll_ReturnsUpdatedResponse()
    {
        var (userId, version) = await SeedCareerWithAiSuggestionsAsync();
        var request = new AcceptAiSuggestionsRequest(AcceptRoles: true, AcceptSkills: true, Version: version);

        var result = await _sut.AcceptAiSuggestionsAsync(userId, request);

        result.Should().NotBeNull();
        result.DeclaredPreferences.DesiredJobPositions.Should()
            .Contain("Backend Engineer", "inferred role was merged into declared positions");
        result.DeclaredPreferences.TargetSkills.Should()
            .Contain("C#", "inferred skills were merged into target skills");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // No AI suggestions exist (AiInferredPreference not found) → 400 Bad Request (ResourceNotFoundException)
    [Fact]
    public async Task CVerify59_UTCID02_AcceptAiSuggestions_NoSuggestionsExist_ThrowsResourceNotFoundException()
    {
        var (userId, version) = await SeedCareerWithoutAiSuggestionsAsync();
        var request = new AcceptAiSuggestionsRequest(AcceptRoles: true, AcceptSkills: true, Version: version);

        var act = async () => await _sut.AcceptAiSuggestionsAsync(userId, request);

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "AiInferredPreference not found — no suggestions available");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId (no career prefs) → ResourceNotFoundException.
    [Fact]
    public async Task CVerify59_UTCID03_AcceptAiSuggestions_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var request = new AcceptAiSuggestionsRequest(AcceptRoles: true, AcceptSkills: true, Version: 0);

        var act = async () => await _sut.AcceptAiSuggestionsAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // acceptRoles:true, acceptSkills:false → only roles merged (boundary)
    [Fact]
    public async Task CVerify59_UTCID04_AcceptAiSuggestions_AcceptRolesOnly_MergesRolesOnly()
    {
        var (userId, version) = await SeedCareerWithAiSuggestionsAsync();
        var request = new AcceptAiSuggestionsRequest(AcceptRoles: true, AcceptSkills: false, Version: version);

        var result = await _sut.AcceptAiSuggestionsAsync(userId, request);

        result.Should().NotBeNull();
        result.DeclaredPreferences.DesiredJobPositions.Should()
            .Contain("Backend Engineer", "role was accepted");
        result.DeclaredPreferences.TargetSkills.Should()
            .BeEmpty("skills were not accepted");
    }
}
