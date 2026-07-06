using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AchievementService.UpdateAchievementAsync — CVerify-36 (4 UTCIDs).
/// PUT /api/v1/users/achievements/{id} [Authorize] — updates a user academic achievement.
/// </summary>
public sealed class CVerify36_UpdateAchievementTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AchievementService _sut;

    public CVerify36_UpdateAchievementTests()
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

    private async Task<(Guid userId, Guid achievementId)> SeedAsync()
    {
        var userId = Guid.NewGuid();
        var achievement = new AcademicAchievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = "Original Award",
            Issuer = "IEEE",
            IssueDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Description = "Original description",
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.AcademicAchievements.Add(achievement);
        await _context.SaveChangesAsync();
        return (userId, achievement.Id);
    }

    private static AcademicAchievementRequest BuildRequest(string title = "Updated Award") =>
        new(
            Title: title,
            Issuer: "ACM",
            IssueDate: new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero),
            Description: "Updated description",
            CredentialUrl: null,
            AttachmentId: null);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid update → returns updated AcademicAchievementResponse (200)
    [Fact]
    public async Task CVerify36_UTCID01_UpdateAchievement_ValidData_ReturnsUpdatedResponse()
    {
        var (userId, achievementId) = await SeedAsync();

        var result = await _sut.UpdateAchievementAsync(userId, achievementId, BuildRequest("Updated Award"));

        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Award");
        result.Issuer.Should().Be("ACM");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent achievementId → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify36_UTCID02_UpdateAchievement_NonExistentId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.UpdateAchievementAsync(userId, Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // title: null → service calls request.Title.Trim() → NullReferenceException (400)
    [Fact]
    public async Task CVerify36_UTCID03_UpdateAchievement_NullTitle_ThrowsOnTrim()
    {
        var (userId, achievementId) = await SeedAsync();
        var request = new AcademicAchievementRequest(
            Title: null!,
            Issuer: "IEEE",
            IssueDate: new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Description: "Some description",
            CredentialUrl: null,
            AttachmentId: null);

        var act = async () => await _sut.UpdateAchievementAsync(userId, achievementId, request);

        await act.Should().ThrowAsync<Exception>("service calls Title.Trim() — null throws NullReferenceException");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any achievementId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify36_UTCID04_UpdateAchievement_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateAchievementAsync(Guid.NewGuid(), Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
