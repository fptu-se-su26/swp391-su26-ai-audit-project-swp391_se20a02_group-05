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
/// Unit tests for CareerService.UpdateCareerPreferenceAsync via PUT — CVerify-60 (3 UTCIDs).
/// PUT /api/v1/users/career [Authorize] — full replace of career preferences (PUT alias of PATCH).
/// Both PUT and PATCH map to the same service method UpdateCareerPreferenceAsync.
/// </summary>
public sealed class CVerify60_ReplaceCareerPreferencesTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICareerReadinessEngine> _readinessEngine = new();
    private readonly CareerService _sut;

    private static readonly CareerReadinessReportDto FakeReport = new(90, "Excellent", 95, new List<CareerReadinessActionItem>());

    public CVerify60_ReplaceCareerPreferencesTests()
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
            AvailableForHire = false,
            PreferredLanguage = "vi",
            OpenToWorkStatus = "passive",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Version = 0,
        };
        _context.CareerPreferences.Add(career);
        await _context.SaveChangesAsync();
        return (userId, career.Version);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // All fields populated → 200 OK – CareerPreferencesDashboardResponse with updated values
    [Fact]
    public async Task CVerify60_UTCID01_ReplaceCareerPreferences_AllFieldsPopulated_ReturnsUpdatedResponse()
    {
        var (userId, version) = await SeedCareerAsync();
        var request = new UpdateCareerPreferenceRequest(
            AvailableForHire: true,
            PreferredLanguage: "en",
            JobTitlePreferences: "Fullstack Engineer",
            SalaryExpectations: 8000m,
            RemotePreference: "fully-remote",
            OpenToWorkStatus: "ACTIVELY_LOOKING",
            OpenToRelocation: true,
            LeadershipTrack: "individual",
            CompanyStagePreferences: new List<string> { "Series B" },
            PreferredIndustries: new List<string> { "AI/ML" },
            TargetSkills: new List<string> { "C#", "React" },
            PreferredWorkEnvironments: null,
            WorkStyles: null,
            CompanyValues: null,
            DesiredJobPositions: new List<string> { "Fullstack Engineer" },
            Skills: null,
            PreferredLocations: new List<string> { "Ho Chi Minh" },
            EmploymentPreferences: new List<string> { "Full-time" },
            ExpectedSalaryMin: 6000m,
            ExpectedSalaryMax: 10000m,
            ExpectedSalaryCurrency: "USD",
            ExpectedSalaryType: "monthly",
            ExpectedSalaryNegotiable: true,
            IsExpectedSalaryVisible: true,
            WorkPreferenceNotes: "Prefer async-first teams",
            Version: version);

        var result = await _sut.UpdateCareerPreferenceAsync(userId, request);

        result.Should().NotBeNull();
        result.DeclaredPreferences.AvailableForHire.Should().BeTrue();
        result.DeclaredPreferences.OpenToWorkStatus.Should().Be("ACTIVELY_LOOKING");
        result.DeclaredPreferences.ExpectedSalaryMin.Should().Be(6000m);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → ResourceNotFoundException (career prefs not found).
    [Fact]
    public async Task CVerify60_UTCID02_ReplaceCareerPreferences_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var request = new UpdateCareerPreferenceRequest(
            AvailableForHire: true, PreferredLanguage: "en", JobTitlePreferences: null,
            SalaryExpectations: null, RemotePreference: null, OpenToWorkStatus: null,
            OpenToRelocation: null, LeadershipTrack: null, CompanyStagePreferences: null,
            PreferredIndustries: null, TargetSkills: null, PreferredWorkEnvironments: null,
            WorkStyles: null, CompanyValues: null, DesiredJobPositions: null,
            Skills: null, PreferredLocations: null, EmploymentPreferences: null,
            ExpectedSalaryMin: null, ExpectedSalaryMax: null, ExpectedSalaryCurrency: null,
            ExpectedSalaryType: null, ExpectedSalaryNegotiable: null, IsExpectedSalaryVisible: null,
            WorkPreferenceNotes: null, Version: 0);

        var act = async () => await _sut.UpdateCareerPreferenceAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // All optional fields null → 200 OK (only AvailableForHire + PreferredLanguage required;
    // nulls leave existing values or reset to defaults). Boundary case.
    [Fact]
    public async Task CVerify60_UTCID03_ReplaceCareerPreferences_AllOptionalFieldsNull_ReturnsResponse()
    {
        var (userId, version) = await SeedCareerAsync();
        var request = new UpdateCareerPreferenceRequest(
            AvailableForHire: true, PreferredLanguage: "en", JobTitlePreferences: null,
            SalaryExpectations: null, RemotePreference: null, OpenToWorkStatus: null,
            OpenToRelocation: null, LeadershipTrack: null, CompanyStagePreferences: null,
            PreferredIndustries: null, TargetSkills: null, PreferredWorkEnvironments: null,
            WorkStyles: null, CompanyValues: null, DesiredJobPositions: null,
            Skills: null, PreferredLocations: null, EmploymentPreferences: null,
            ExpectedSalaryMin: null, ExpectedSalaryMax: null, ExpectedSalaryCurrency: null,
            ExpectedSalaryType: null, ExpectedSalaryNegotiable: null, IsExpectedSalaryVisible: null,
            WorkPreferenceNotes: null, Version: version);

        var result = await _sut.UpdateCareerPreferenceAsync(userId, request);

        result.Should().NotBeNull("null optional fields are valid — service keeps existing values");
        result.DeclaredPreferences.AvailableForHire.Should().BeTrue();
    }
}
