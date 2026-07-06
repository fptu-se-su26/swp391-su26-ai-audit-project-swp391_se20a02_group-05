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
/// Unit tests for CareerService.GetCareerDashboardAsync — CVerify-57 (3 UTCIDs).
/// GET /api/v1/users/career [Authorize] — returns current career preferences.
/// When no preferences exist but user exists, creates default preferences on the fly.
/// </summary>
public sealed class CVerify57_GetCareerPreferencesTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICareerReadinessEngine> _readinessEngine = new();
    private readonly CareerService _sut;

    private static readonly CareerReadinessReportDto FakeReport = new(75, "Good", 80, new List<CareerReadinessActionItem>());

    public CVerify57_GetCareerPreferencesTests()
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

    private async Task<Guid> SeedUserAsync()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId, Email = $"{userId}@test.com", FullName = "Test User",
            Username = $"user{userId:N}", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
        return userId;
    }

    private async Task<Guid> SeedUserWithCareerAsync()
    {
        var userId = await SeedUserAsync();
        _context.CareerPreferences.Add(new CareerPreference
        {
            UserId = userId,
            AvailableForHire = true,
            PreferredLanguage = "en",
            OpenToWorkStatus = "ACTIVELY_LOOKING",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
        return userId;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid JWT – user has career preferences set → 200 OK – CareerPreferencesDashboardResponse
    [Fact]
    public async Task CVerify57_UTCID01_GetCareerPreferences_UserHasPrefs_ReturnsDashboardResponse()
    {
        var userId = await SeedUserWithCareerAsync();

        var result = await _sut.GetCareerDashboardAsync(userId);

        result.Should().NotBeNull();
        result.DeclaredPreferences.Should().NotBeNull();
        result.ReadinessReport.Should().NotBeNull();
        result.DeclaredPreferences.OpenToWorkStatus.Should().Be("ACTIVELY_LOOKING");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Valid JWT – user has no career prefs (but user exists) →
    // service creates default prefs and returns 200 OK
    [Fact]
    public async Task CVerify57_UTCID02_GetCareerPreferences_NoPrefsUserExists_CreatesDefaultAndReturns()
    {
        var userId = await SeedUserAsync();

        var result = await _sut.GetCareerDashboardAsync(userId);

        result.Should().NotBeNull();
        result.DeclaredPreferences.Should().NotBeNull();
        result.DeclaredPreferences.OpenToWorkStatus.Should().Be("casual", "default preferences have OpenToWorkStatus='casual'");

        var createdPref = await _context.CareerPreferences.FindAsync(userId);
        createdPref.Should().NotBeNull("service persists default preferences");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId (user not in DB) → ResourceNotFoundException.
    [Fact]
    public async Task CVerify57_UTCID03_GetCareerPreferences_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var ghostUserId = Guid.NewGuid();

        var act = async () => await _sut.GetCareerDashboardAsync(ghostUserId);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
