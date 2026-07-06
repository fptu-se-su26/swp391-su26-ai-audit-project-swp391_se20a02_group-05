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
/// Unit tests for CareerService.UpdateCareerPreferenceAsync — CVerify-23 (5 UTCIDs).
/// PATCH /api/v1/users/career [Authorize] — creates or updates career preferences.
/// </summary>
public sealed class CVerify23_UpdateCareerPrefsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICareerReadinessEngine> _readinessEngine = new();
    private readonly CareerService _sut;

    public CVerify23_UpdateCareerPrefsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _readinessEngine
            .Setup(e => e.CalculateReadinessAsync(It.IsAny<CareerPreference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CareerReadinessReportDto(50, "Fair", 60, new List<CareerReadinessActionItem>()));

        _sut = new CareerService(_context, _readinessEngine.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(User user, CareerPreference career)> SeedAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "Career User",
            Username = "careeruser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        var career = new CareerPreference
        {
            UserId = user.Id,
            AvailableForHire = true,
            PreferredLanguage = "en",
            OpenToWorkStatus = "casual",
            ExpectedSalaryMin = 2000m,
            ExpectedSalaryMax = 5000m,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.Users.Add(user);
        _context.CareerPreferences.Add(career);
        await _context.SaveChangesAsync();
        return (user, career);
    }

    private static UpdateCareerPreferenceRequest MakeRequest(
        bool? availableForHire = null,
        decimal? salaryMin = null,
        decimal? salaryMax = null,
        string? language = null,
        uint version = 0u) =>
        new(
            AvailableForHire: availableForHire,
            PreferredLanguage: language,
            JobTitlePreferences: null,
            SalaryExpectations: null,
            RemotePreference: null,
            OpenToWorkStatus: null,
            OpenToRelocation: null,
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
    [Fact]
    public async Task CVerify23_UTCID01_UpdateCareerPrefs_AllFields_ReturnsDashboardResponse()
    {
        var (user, career) = await SeedAsync();

        var result = await _sut.UpdateCareerPreferenceAsync(user.Id,
            MakeRequest(availableForHire: true, salaryMin: 3000m, salaryMax: 5000m));

        result.Should().NotBeNull();
        result.DeclaredPreferences.Should().NotBeNull();
        result.DeclaredPreferences.AvailableForHire.Should().BeTrue();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Only salary changed (partial update)
    [Fact]
    public async Task CVerify23_UTCID02_UpdateCareerPrefs_OnlySalaryChanged_ReturnsUpdatedResponse()
    {
        var (user, career) = await SeedAsync();

        var result = await _sut.UpdateCareerPreferenceAsync(user.Id,
            MakeRequest(salaryMin: 2000m, salaryMax: 4000m));

        result.Should().NotBeNull();
        result.DeclaredPreferences.ExpectedSalaryMin.Should().Be(2000m);
        result.DeclaredPreferences.ExpectedSalaryMax.Should().Be(4000m);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: career prefs not found → ResourceNotFoundException.
    [Fact]
    public async Task CVerify23_UTCID03_UpdateCareerPrefs_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateCareerPreferenceAsync(Guid.NewGuid(), MakeRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // salaryMin > salaryMax → ValidationException
    [Fact]
    public async Task CVerify23_UTCID04_UpdateCareerPrefs_SalaryMinExceedsMax_ThrowsValidationException()
    {
        var (user, career) = await SeedAsync();

        var act = async () => await _sut.UpdateCareerPreferenceAsync(user.Id,
            MakeRequest(salaryMin: 5000m, salaryMax: 2000m));

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*salary*");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // All optional fields null → update is a no-op, returns current state
    [Fact]
    public async Task CVerify23_UTCID05_UpdateCareerPrefs_AllNullFields_ReturnsCurrentPrefs()
    {
        var (user, career) = await SeedAsync();

        var result = await _sut.UpdateCareerPreferenceAsync(user.Id, MakeRequest());

        result.Should().NotBeNull("all-null patch should succeed and return current preferences");
        result.DeclaredPreferences.PreferredLanguage.Should().Be("en");
    }
}
