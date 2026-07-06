using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Entities;
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
/// Unit tests for ProfileService.DeleteAvatarAsync — CVerify-19 (3 UTCIDs).
/// DELETE /api/v1/users/profile/avatar [Authorize] — removes the user's avatar.
/// </summary>
public sealed class CVerify19_DeleteAvatarTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify19_DeleteAvatarTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        // DeleteFileAsync mock — returns success
        _storageService
            .Setup(s => s.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new ProfileService(
            _context,
            _cacheService.Object,
            _storageService.Object,
            _usernameService.Object,
            new FakeTimeProvider(),
            _logger.Object,
            _projectService.Object,
            _cvRepositoryIndexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<User> SeedUserAsync(string? avatarUrl = null)
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = "user@example.com",
            FullName        = "Avatar User",
            Username        = "avataruser",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
            AvatarUrl       = avatarUrl,
            AvatarSource    = avatarUrl != null ? AvatarSource.Uploaded : AvatarSource.Default,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify19_UTCID01_DeleteAvatar_UserWithUploadedAvatar_ClearsAvatarUrlAndSetsDefault()
    {
        var user = await SeedUserAsync(avatarUrl: "profiles/avatar-key-abc.jpg");

        await _sut.DeleteAvatarAsync(user.Id);

        var updated = await _context.Users.FindAsync(user.Id);
        updated!.AvatarUrl.Should().BeNull();
        updated.AvatarSource.Should().Be(AvatarSource.Default);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // At service level: non-existent userId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify19_UTCID02_DeleteAvatar_NoJwtControllerLevel_ServiceThrowsForUnknownUser()
    {
        var act = async () => await _sut.DeleteAvatarAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // User with no existing avatar — delete is idempotent.
    [Fact]
    public async Task CVerify19_UTCID03_DeleteAvatar_UserWithNoAvatar_SucceedsIdempotently()
    {
        var user = await SeedUserAsync(avatarUrl: null);

        var act = async () => await _sut.DeleteAvatarAsync(user.Id);

        await act.Should().NotThrowAsync("deleting when no avatar exists is a no-op");
        var updated = await _context.Users.FindAsync(user.Id);
        updated!.AvatarUrl.Should().BeNull();
        updated.AvatarSource.Should().Be(AvatarSource.Default);
    }
}
