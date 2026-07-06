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
/// Unit tests for CareerService.UpdateCareerPreferenceAsync — CVerify-58 (4 UTCIDs).
/// PATCH /api/v1/users/career [Authorize] — partially updates career preferences.
/// Throws ValidationException when salaryMin > salaryMax.
/// </summary>
public sealed class CVerify58_UpdateCareerPreferencesTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICareerReadinessEngine> _readinessEngine = new();
    private readonly CareerService _sut;

    private static readonly CareerReadinessReportDto FakeReport = new(75, "Good", 80, new List<CareerReadinessActionItem>());

    public CVerify58_UpdateCareerPreferencesTests()
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

    private async Task<(Guid userId, uint version)> SeedCareerAsync()
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
        await _context.SaveChangesAsync();
        return (userId, career.Version);
    }

    private static UpdateCareerPreferenceRequest BuildRequest(
        string? openToWorkStatus = "ACTIVELY_LOOKING",
        decimal? salaryMin = null,
        decimal? salaryMax = null,
        uint version = 0) =>
        new(
            AvailableForHire: true,
            PreferredLanguage: "en",
            JobTitlePreferences: null,
            SalaryExpectations: null,
            RemotePreference: "remote",
            OpenToWorkStatus: openToWorkStatus,
            OpenToRelocation: false,
            LeadershipTrack: null,
            CompanyStagePreferences: null,
            PreferredIndustries: null,
            TargetSkills: null,
            PreferredWorkEnvironments: null,
            WorkStyles: null,
            CompanyValues: null,
            DesiredJobPositions: null,
            Skills: null,
            PreferredLocations: null,
            EmploymentPreferences: null,
            ExpectedSalaryMin: salaryMin,
            ExpectedSalaryMax: salaryMax,
            ExpectedSalaryCurrency: null,
            ExpectedSalaryType: null,
            ExpectedSalaryNegotiable: null,
            IsExpectedSalaryVisible: null,
            WorkPreferenceNotes: null,
            Version: version);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // availabilityStatus:'ACTIVELY_LOOKING' → 200 OK – CareerPreferencesDashboardResponse
    [Fact]
    public async Task CVerify58_UTCID01_UpdateCareerPreferences_ActivelyLooking_ReturnsUpdatedResponse()
    {
        var (userId, version) = await SeedCareerAsync();
        var request = BuildRequest(openToWorkStatus: "ACTIVELY_LOOKING", version: version);

        var result = await _sut.UpdateCareerPreferenceAsync(userId, request);

        result.Should().NotBeNull();
        result.DeclaredPreferences.OpenToWorkStatus.Should().Be("ACTIVELY_LOOKING");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // All non-null fields populated with valid data → 200 OK
    [Fact]
    public async Task CVerify58_UTCID02_UpdateCareerPreferences_AllFieldsPopulated_ReturnsResponse()
    {
        var (userId, version) = await SeedCareerAsync();
        var request = new UpdateCareerPreferenceRequest(
            AvailableForHire: true,
            PreferredLanguage: "en",
            JobTitlePreferences: "Backend Engineer",
            SalaryExpectations: 5000m,
            RemotePreference: "hybrid",
            OpenToWorkStatus: "OPENLY_AVAILABLE",
            OpenToRelocation: true,
            LeadershipTrack: "individual",
            CompanyStagePreferences: new List<string> { "Series A" },
            PreferredIndustries: new List<string> { "Fintech" },
            TargetSkills: null,
            PreferredWorkEnvironments: null,
            WorkStyles: null,
            CompanyValues: null,
            DesiredJobPositions: new List<string> { "Backend Engineer" },
            Skills: null,
            PreferredLocations: new List<string> { "Ho Chi Minh" },
            EmploymentPreferences: null,
            ExpectedSalaryMin: 3000m,
            ExpectedSalaryMax: 7000m,
            ExpectedSalaryCurrency: "USD",
            ExpectedSalaryType: "monthly",
            ExpectedSalaryNegotiable: true,
            IsExpectedSalaryVisible: true,
            WorkPreferenceNotes: "Open to remote",
            Version: version);

        var result = await _sut.UpdateCareerPreferenceAsync(userId, request);

        result.Should().NotBeNull();
        result.DeclaredPreferences.ExpectedSalaryMin.Should().Be(3000m);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // salaryMin:10000, salaryMax:1000 (min > max) → 400 Bad Request – ValidationException
    [Fact]
    public async Task CVerify58_UTCID03_UpdateCareerPreferences_MinSalaryExceedsMax_ThrowsValidationException()
    {
        var (userId, version) = await SeedCareerAsync();
        var request = BuildRequest(salaryMin: 10000m, salaryMax: 1000m, version: version);

        var act = async () => await _sut.UpdateCareerPreferenceAsync(userId, request);

        await act.Should().ThrowAsync<CVerify.API.Modules.Shared.Exceptions.ValidationException>(
            "minimum salary cannot exceed maximum salary");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId (no career prefs) → ResourceNotFoundException.
    [Fact]
    public async Task CVerify58_UTCID04_UpdateCareerPreferences_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var request = BuildRequest(version: 0);

        var act = async () => await _sut.UpdateCareerPreferenceAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
