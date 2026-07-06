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
/// Unit tests for ProfileService.UpdateUsernameAsync — CVerify-15 (11 UTCIDs).
/// PUT /api/v1/users/profile/username — updates username with 30-day cooldown and validation.
/// </summary>
public sealed class CVerify15_UpdateUsernameTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify15_UpdateUsernameTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        // Default: normalize returns lowercase, no cooldown, no reserved
        _usernameService.Setup(u => u.Normalize(It.IsAny<string>()))
            .Returns<string>(s => s.Trim().ToLowerInvariant());
        _usernameService.Setup(u => u.CheckChangeCooldownAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
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

    private async Task<(User user, UserProfile profile)> SeedAsync(
        string username = "oldusername",
        DateTimeOffset? lastChange = null)
    {
        var user = new User
        {
            Id                   = Guid.NewGuid(),
            Email                = "user@example.com",
            FullName             = "Test User",
            Username             = username,
            Status               = UserStatus.ACTIVE,
            EmailVerifiedAt      = DateTime.UtcNow,
            LastUsernameChangeAt = lastChange,
        };
        var profile = new UserProfile
        {
            UserId              = user.Id,
            Username            = username,
            ProfileVisibility   = "public",
            RecruiterVisibility = true,
            AiTalentDiscovery   = "disabled",
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return (user, profile);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID01_UpdateUsername_ValidAlphanumericUsername_Succeeds()
    {
        var (user, _) = await SeedAsync();

        await _sut.UpdateUsernameAsync(user.Id, "john_doe_123");

        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser!.Username.Should().Be("john_doe_123");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID02_UpdateUsername_UsernameTakenByAnotherUser_ThrowsProfileException()
    {
        var (user, _) = await SeedAsync();

        // Seed another user with the target username
        _context.Users.Add(new User
        {
            Id     = Guid.NewGuid(), Email = "other@example.com", FullName = "Other",
            Username = "taken_user", Status = UserStatus.ACTIVE,
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.UpdateUsernameAsync(user.Id, "taken_user");

        await act.Should().ThrowAsync<ProfileException>()
            .Where(e => e.ErrorCode == ProfileErrorCodes.UsernameAlreadyExists);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID03_UpdateUsername_ReservedKeyword_ThrowsValidationException()
    {
        var (user, _) = await SeedAsync();
        _usernameService
            .Setup(u => u.ValidateUsername("admin"))
            .Throws(new ValidationException("The username 'admin' is reserved."));

        var act = async () => await _sut.UpdateUsernameAsync(user.Id, "admin");

        await act.Should().ThrowAsync<ValidationException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID04_UpdateUsername_TwoCharsTooShort_ThrowsValidationException()
    {
        var (user, _) = await SeedAsync();
        _usernameService
            .Setup(u => u.ValidateUsername("ab"))
            .Throws(new ValidationException("Username must be at least 3 characters long."));

        var act = async () => await _sut.UpdateUsernameAsync(user.Id, "ab");

        await act.Should().ThrowAsync<ValidationException>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID05_UpdateUsername_ThirtyOneCharsTooLong_ThrowsValidationException()
    {
        var (user, _) = await SeedAsync();
        var longName = new string('a', 31);
        _usernameService
            .Setup(u => u.ValidateUsername(longName))
            .Throws(new ValidationException("Username cannot exceed 30 characters."));

        var act = async () => await _sut.UpdateUsernameAsync(user.Id, longName);

        await act.Should().ThrowAsync<ValidationException>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID06_UpdateUsername_UsernameWithSpace_ThrowsValidationException()
    {
        var (user, _) = await SeedAsync();
        _usernameService
            .Setup(u => u.ValidateUsername("user name"))
            .Throws(new ValidationException("Username can only contain alphanumeric characters, underscores, hyphens, and periods."));

        var act = async () => await _sut.UpdateUsernameAsync(user.Id, "user name");

        await act.Should().ThrowAsync<ValidationException>();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID07_UpdateUsername_WithinThirtyDayCooldown_ThrowsValidationException()
    {
        var (user, _) = await SeedAsync(lastChange: DateTimeOffset.UtcNow.AddDays(-10));
        _usernameService
            .Setup(u => u.CheckChangeCooldownAsync(user.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("You can only change your username once every 30 days."));

        var act = async () => await _sut.UpdateUsernameAsync(user.Id, "valid_new");

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*30 days*");
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // At service level: no profile found → ResourceNotFoundException.
    [Fact]
    public async Task CVerify15_UTCID08_UpdateUsername_NoJwtControllerLevel_ServiceThrowsForMissingProfile()
    {
        var act = async () => await _sut.UpdateUsernameAsync(Guid.NewGuid(), "anyname");

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID09_UpdateUsername_ExactlyThreeChars_Succeeds()
    {
        var (user, _) = await SeedAsync();

        await _sut.UpdateUsernameAsync(user.Id, "abc");

        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser!.Username.Should().Be("abc");
    }

    // ── UTCID10 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID10_UpdateUsername_ExactlyThirtyChars_Succeeds()
    {
        var (user, _) = await SeedAsync();
        var thirtyChar = new string('z', 30);

        await _sut.UpdateUsernameAsync(user.Id, thirtyChar);

        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser!.Username.Should().Be(thirtyChar);
    }

    // ── UTCID11 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify15_UTCID11_UpdateUsername_ReservedKeywordLoginOrSettings_ThrowsValidationException()
    {
        var (user, _) = await SeedAsync();
        _usernameService
            .Setup(u => u.ValidateUsername(It.IsIn("login", "settings")))
            .Throws(new ValidationException("The username is reserved and cannot be used."));

        var act = async () => await _sut.UpdateUsernameAsync(user.Id, "login");

        await act.Should().ThrowAsync<ValidationException>();
    }
}
