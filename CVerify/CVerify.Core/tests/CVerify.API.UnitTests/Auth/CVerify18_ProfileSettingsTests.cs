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
/// Unit tests for ProfileService.GetProfileByUserIdAsync used in the Profile Settings page — CVerify-18 (4 UTCIDs).
/// GET /api/v1/users/profile [Authorize] — returns full profile data for the settings page.
/// </summary>
public sealed class CVerify18_ProfileSettingsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify18_ProfileSettingsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://cdn.example.com/signed-avatar");

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

    private async Task<User> SeedUserWithProfileAsync(
        string? bio = null, string? headline = null,
        string? location = null, string? avatarUrl = null)
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = "user@example.com",
            FullName        = "Settings User",
            Username        = "settingsuser",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
            AvatarUrl       = avatarUrl,
        };
        var profile = new UserProfile
        {
            UserId              = user.Id,
            Username            = user.Username,
            Bio                 = bio,
            Headline            = headline,
            Location            = location,
            ProfileVisibility   = "public",
            RecruiterVisibility = true,
            AiTalentDiscovery   = "disabled",
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify18_UTCID01_GetProfileSettings_UserWithFullProfile_ReturnsProfileResponse()
    {
        var user = await SeedUserWithProfileAsync(bio: "My bio", headline: "Engineer", location: "HCM");

        var result = await _sut.GetProfileByUserIdAsync(user.Id);

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.Bio.Should().Be("My bio");
        result.Headline.Should().Be("Engineer");
        result.Location.Should().Be("HCM");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify18_UTCID02_GetProfileSettings_UserWithMinimalProfile_ReturnsProfileResponse()
    {
        var user = await SeedUserWithProfileAsync(); // no optional fields

        var result = await _sut.GetProfileByUserIdAsync(user.Id);

        result.Should().NotBeNull();
        result.Bio.Should().BeNull();
        result.Headline.Should().BeNull();
        result.Location.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify18_UTCID03_GetProfileSettings_UserWithNoLinkedProviders_ReturnsProfileResponse()
    {
        var user = await SeedUserWithProfileAsync(bio: "Bio only");

        var result = await _sut.GetProfileByUserIdAsync(user.Id);

        result.Should().NotBeNull("user without linked providers still returns a valid profile");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // At service level: non-existent userId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify18_UTCID04_GetProfileSettings_NoJwtControllerLevel_ServiceThrowsForUnknownUser()
    {
        var act = async () => await _sut.GetProfileByUserIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
