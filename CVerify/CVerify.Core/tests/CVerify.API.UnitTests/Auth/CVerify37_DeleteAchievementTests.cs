using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AchievementService.DeleteAchievementAsync — CVerify-37 (3 UTCIDs).
/// DELETE /api/v1/users/achievements/{id} [Authorize] — soft-deletes a user achievement.
/// </summary>
public sealed class CVerify37_DeleteAchievementTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AchievementService _sut;

    public CVerify37_DeleteAchievementTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new AchievementService(_context, _storageService.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid achievementId)> SeedAsync()
    {
        var userId = Guid.NewGuid();
        var achievement = new AcademicAchievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = "Best Paper Award",
            Issuer = "IEEE",
            IssueDate = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero),
            Description = "Awarded for paper",
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.AcademicAchievements.Add(achievement);
        await _context.SaveChangesAsync();
        return (userId, achievement.Id);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid userId and achievementId → soft-deletes (sets DeletedAt), returns 204
    [Fact]
    public async Task CVerify37_UTCID01_DeleteAchievement_ValidEntry_SoftDeletesSuccessfully()
    {
        var (userId, achievementId) = await SeedAsync();

        await _sut.DeleteAchievementAsync(userId, achievementId);

        var achievement = await _context.AcademicAchievements.FindAsync(achievementId);
        achievement!.DeletedAt.Should().NotBeNull("soft delete sets DeletedAt");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent achievementId → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify37_UTCID02_DeleteAchievement_NonExistentId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.DeleteAchievementAsync(userId, Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any achievementId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify37_UTCID03_DeleteAchievement_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.DeleteAchievementAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
