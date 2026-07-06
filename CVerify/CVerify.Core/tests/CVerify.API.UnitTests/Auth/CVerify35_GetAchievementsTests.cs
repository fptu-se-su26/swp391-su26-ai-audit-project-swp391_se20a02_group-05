using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AchievementService.GetAchievementsAsync — CVerify-35 (3 UTCIDs).
/// GET /api/v1/users/achievements [Authorize] — retrieves list of academic achievements.
/// </summary>
public sealed class CVerify35_GetAchievementsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AchievementService _sut;

    public CVerify35_GetAchievementsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync("https://cdn.example.com/signed");

        _sut = new AchievementService(_context, _storageService.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task SeedAchievementAsync(Guid userId, string title = "Best Paper Award")
    {
        _context.AcademicAchievements.Add(new AcademicAchievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Issuer = "IEEE",
            IssueDate = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero),
            Description = "Award for outstanding paper",
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // User has achievements → returns non-empty list (200)
    [Fact]
    public async Task CVerify35_UTCID01_GetAchievements_HasEntries_ReturnsPopulatedList()
    {
        var userId = Guid.NewGuid();
        await SeedAchievementAsync(userId, "Best Paper Award");
        await SeedAchievementAsync(userId, "Hackathon Winner");

        var result = await _sut.GetAchievementsAsync(userId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(a => a.Title == "Best Paper Award");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // User has no achievements → returns empty list (200)
    [Fact]
    public async Task CVerify35_UTCID02_GetAchievements_NoEntries_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetAchievementsAsync(userId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId has no achievements → empty list.
    [Fact]
    public async Task CVerify35_UTCID03_GetAchievements_NoJwtControllerLevel_ServiceReturnsEmpty()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetAchievementsAsync(ghostUserId);

        result.Should().NotBeNull();
        result.Should().BeEmpty("JWT auth is controller responsibility");
    }
}
