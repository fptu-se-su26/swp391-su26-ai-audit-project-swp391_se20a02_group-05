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
/// Unit tests for CareerService.UpdateCareerPreferenceAsync (PreferredLanguage) — CVerify-25 (4 UTCIDs).
/// Updates the user's preferred display language via career preferences.
/// </summary>
public sealed class CVerify25_ChangeLanguageTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICareerReadinessEngine> _readinessEngine = new();
    private readonly CareerService _sut;

    public CVerify25_ChangeLanguageTests()
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
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "Lang User",
            Username = "languser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        var career = new CareerPreference
        {
            UserId = user.Id,
            PreferredLanguage = "en",
            AvailableForHire = true,
            OpenToWorkStatus = "casual",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.Users.Add(user);
        _context.CareerPreferences.Add(career);
        await _context.SaveChangesAsync();
        return (user, career);
    }

    private static UpdateCareerPreferenceRequest MakeRequest(string? language, uint version = 0u) =>
        new(
            AvailableForHire: null, PreferredLanguage: language, JobTitlePreferences: null,
            SalaryExpectations: null, RemotePreference: null, OpenToWorkStatus: null,
            OpenToRelocation: null, LeadershipTrack: null, CompanyStagePreferences: null,
            PreferredIndustries: null, TargetSkills: null, PreferredWorkEnvironments: null,
            WorkStyles: null, CompanyValues: null, DesiredJobPositions: null,
            Skills: null, PreferredLocations: null, EmploymentPreferences: null,
            ExpectedSalaryMin: null, ExpectedSalaryMax: null, ExpectedSalaryCurrency: null,
            ExpectedSalaryType: null, ExpectedSalaryNegotiable: null, IsExpectedSalaryVisible: null,
            WorkPreferenceNotes: null, Version: version);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify25_UTCID01_ChangeLanguage_ToEnglish_ReturnsUpdatedPrefs()
    {
        var (user, _) = await SeedAsync();

        var result = await _sut.UpdateCareerPreferenceAsync(user.Id, MakeRequest("en"));

        result.Should().NotBeNull();
        result.DeclaredPreferences.PreferredLanguage.Should().Be("en");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify25_UTCID02_ChangeLanguage_ToVietnamese_ReturnsUpdatedPrefs()
    {
        var (user, _) = await SeedAsync();

        var result = await _sut.UpdateCareerPreferenceAsync(user.Id, MakeRequest("vi"));

        result.Should().NotBeNull();
        result.DeclaredPreferences.PreferredLanguage.Should().Be("vi");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Unsupported language code — service has no language registry; value is stored as-is
    [Fact]
    public async Task CVerify25_UTCID03_ChangeLanguage_UnsupportedCode_ServiceAcceptsWithNoValidation()
    {
        var (user, _) = await SeedAsync();

        var result = await _sut.UpdateCareerPreferenceAsync(user.Id, MakeRequest("xx"));

        result.Should().NotBeNull("service does not validate language codes — controller/attribute responsibility");
        result.DeclaredPreferences.PreferredLanguage.Should().Be("xx");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: career prefs not found → ResourceNotFoundException.
    [Fact]
    public async Task CVerify25_UTCID04_ChangeLanguage_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateCareerPreferenceAsync(Guid.NewGuid(), MakeRequest("en"));

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
