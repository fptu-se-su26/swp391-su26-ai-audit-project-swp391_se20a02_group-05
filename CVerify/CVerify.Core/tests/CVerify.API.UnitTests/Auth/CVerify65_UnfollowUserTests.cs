using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Diagnostics;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for ProfileService.UnfollowUserAsync — CVerify-65 (4 UTCIDs).
/// DELETE /api/v1/ranking/{username}/follow [Authorize] — unfollow a candidate.
/// </summary>
public sealed class CVerify65_UnfollowUserTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IStorageService> _storageService = new();
    private readonly Mock<IUsernameService> _usernameService = new();
    private readonly Mock<IAppLogger> _appLogger = new();
    private readonly Mock<IProjectService> _projectService = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly ProfileService _sut;

    public CVerify65_UnfollowUserTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _usernameService
            .Setup(u => u.Normalize(It.IsAny<string>()))
            .Returns<string>(s => s.ToLower());

        _sut = new ProfileService(
            _context,
            _cacheService.Object,
            _storageService.Object,
            _usernameService.Object,
            TimeProvider.System,
            _appLogger.Object,
            _projectService.Object,
            _cvRepositoryIndexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<Guid> SeedUserAsync(string username)
    {
        var id = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = id, Email = $"{username}@test.com", FullName = username,
            Username = username.ToLower(), Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
        return id;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Currently following → UserFollower record removed.
    [Fact]
    public async Task CVerify65_UTCID01_UnfollowUser_CurrentlyFollowing_RemovesFollowRecord()
    {
        var followerId = await SeedUserAsync("alice");
        var followeeId = await SeedUserAsync("bob");
        _context.UserFollowers.Add(new UserFollower
        {
            FollowerId = followerId,
            FolloweeId = followeeId,
            FollowedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.UnfollowUserAsync(followerId, "bob");

        await act.Should().NotThrowAsync();

        var record = await _context.UserFollowers
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FolloweeId == followeeId);
        record.Should().BeNull("follow record should be deleted");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Not following → idempotent (no exception).
    [Fact]
    public async Task CVerify65_UTCID02_UnfollowUser_NotFollowing_IsIdempotent()
    {
        var followerId = await SeedUserAsync("alice2");
        await SeedUserAsync("bob2");

        var act = async () => await _sut.UnfollowUserAsync(followerId, "bob2");

        await act.Should().NotThrowAsync("unfollowing someone not followed is idempotent");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Followee username does not exist → ResourceNotFoundException.
    [Fact]
    public async Task CVerify65_UTCID03_UnfollowUser_FolloweeNotFound_ThrowsResourceNotFoundException()
    {
        var followerId = await SeedUserAsync("alice3");

        var act = async () => await _sut.UnfollowUserAsync(followerId, "nonexistentuser");

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "no user with that username exists");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost followerId + valid followee → returns without error (no follow record found → idempotent).
    [Fact]
    public async Task CVerify65_UTCID04_UnfollowUser_NoJwtControllerLevel_ServiceIsIdempotentForGhostFollower()
    {
        await SeedUserAsync("bob4");
        var ghostFollowerId = Guid.NewGuid(); // not in DB

        var act = async () => await _sut.UnfollowUserAsync(ghostFollowerId, "bob4");

        await act.Should().NotThrowAsync("no follow record exists for ghost follower → idempotent success");
    }
}
