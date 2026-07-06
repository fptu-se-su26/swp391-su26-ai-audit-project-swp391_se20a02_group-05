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
/// Unit tests for ProfileService.FollowUserAsync — CVerify-64 (5 UTCIDs).
/// POST /api/v1/ranking/{username}/follow [Authorize] — follow another candidate.
/// </summary>
public sealed class CVerify64_FollowUserTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IStorageService> _storageService = new();
    private readonly Mock<IUsernameService> _usernameService = new();
    private readonly Mock<IAppLogger> _appLogger = new();
    private readonly Mock<IProjectService> _projectService = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly ProfileService _sut;

    public CVerify64_FollowUserTests()
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
    // Follower and followee both exist, not following → UserFollower record created.
    [Fact]
    public async Task CVerify64_UTCID01_FollowUser_ValidUsers_CreatesFollowRecord()
    {
        var followerId = await SeedUserAsync("alice");
        var followeeId = await SeedUserAsync("bob");

        var act = async () => await _sut.FollowUserAsync(followerId, "bob");

        await act.Should().NotThrowAsync();

        var record = await _context.UserFollowers
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FolloweeId == followeeId);
        record.Should().NotBeNull("UserFollower record should be persisted");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Already following → idempotent (no exception, no duplicate record).
    [Fact]
    public async Task CVerify64_UTCID02_FollowUser_AlreadyFollowing_IsIdempotent()
    {
        var followerId = await SeedUserAsync("alice2");
        var followeeId = await SeedUserAsync("bob2");
        _context.UserFollowers.Add(new UserFollower
        {
            FollowerId = followerId,
            FolloweeId = followeeId,
            FollowedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.FollowUserAsync(followerId, "bob2");

        await act.Should().NotThrowAsync("following an already-followed user is idempotent");

        var count = await _context.UserFollowers
            .CountAsync(uf => uf.FollowerId == followerId && uf.FolloweeId == followeeId);
        count.Should().Be(1, "no duplicate follow records should be created");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Self-follow → BusinessRuleException with code SELF_FOLLOW_NOT_ALLOWED.
    [Fact]
    public async Task CVerify64_UTCID03_FollowUser_SelfFollow_ThrowsBusinessRuleException()
    {
        var userId = await SeedUserAsync("selfuser");

        var act = async () => await _sut.FollowUserAsync(userId, "selfuser");

        await act.Should().ThrowAsync<BusinessRuleException>("self-follow is a domain rule violation");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Followee username does not exist → ResourceNotFoundException.
    [Fact]
    public async Task CVerify64_UTCID04_FollowUser_FolloweeNotFound_ThrowsResourceNotFoundException()
    {
        var followerId = await SeedUserAsync("alice3");

        var act = async () => await _sut.FollowUserAsync(followerId, "nonexistentuser");

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "no user with that username exists");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: followerId not in DB but followee exists → ResourceNotFoundException on follower lookup.
    [Fact]
    public async Task CVerify64_UTCID05_FollowUser_NoJwtControllerLevel_ServiceThrowsOnGhostFollower()
    {
        await SeedUserAsync("bob5");
        var ghostFollowerId = Guid.NewGuid(); // not in DB

        var act = async () => await _sut.FollowUserAsync(ghostFollowerId, "bob5");

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "follower user must exist in the DB");
    }
}
