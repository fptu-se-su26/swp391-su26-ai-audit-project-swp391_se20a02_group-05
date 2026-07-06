using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
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
/// Unit tests for ProfileService.GetProfileByUserIdAsync — CVerify-07 (5 UTCIDs).
/// </summary>
public sealed class CVerify07_ViewProfileTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>           _cacheService           = new();
    private readonly Mock<IStorageService>         _storageService         = new();
    private readonly Mock<IUsernameService>        _usernameService        = new();
    private readonly Mock<IAppLogger>              _logger                 = new();
    private readonly Mock<IProjectService>         _projectService         = new();
    private readonly Mock<ICvRepositoryIndexer>    _cvRepositoryIndexer    = new();

    private readonly ProfileService _sut;

    public CVerify07_ViewProfileTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<System.Threading.CancellationToken>()))
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

    private async Task<(User user, UserProfile profile)> SeedUserWithProfileAsync(
        string? bio = "Hello World",
        string? headline = "Software Engineer",
        string? location = "Hanoi")
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = "user@example.com",
            FullName        = "Test User",
            Username        = "testuser",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        var profile = new UserProfile
        {
            UserId             = user.Id,
            Username           = user.Username,
            Bio                = bio,
            Headline           = headline,
            Location           = location,
            ProfileVisibility  = "public",
            RecruiterVisibility = true,
            AiTalentDiscovery  = "disabled",
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return (user, profile);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify07_UTCID01_GetProfile_ActiveUserWithFullProfile_ReturnsProfileResponse()
    {
        var (user, profile) = await SeedUserWithProfileAsync(bio: "My bio", headline: "Dev", location: "HCM");

        var result = await _sut.GetProfileByUserIdAsync(user.Id);

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.Bio.Should().Be("My bio");
        result.Headline.Should().Be("Dev");
        result.Location.Should().Be("HCM");
        result.Username.Should().Be("testuser");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify07_UTCID02_GetProfile_ActiveUserWithBioOnly_ReturnsProfileResponseWithNullOptionalFields()
    {
        var (user, _) = await SeedUserWithProfileAsync(bio: "Just a bio", headline: null, location: null);

        var result = await _sut.GetProfileByUserIdAsync(user.Id);

        result.Should().NotBeNull();
        result.Bio.Should().Be("Just a bio");
        result.Headline.Should().BeNull();
        result.Location.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401 before service is called.
    // At service level: a non-existent userId throws ResourceNotFoundException.
    [Fact]
    public async Task CVerify07_UTCID03_GetProfile_NoJwtControllerLevel_ServiceThrowsResourceNotFoundForUnknownUser()
    {
        var act = async () => await _sut.GetProfileByUserIdAsync(Guid.Empty);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Expired JWT → ASP.NET middleware returns 401 before service is called.
    // At service level: user exists but has no UserProfile → auto-provision.
    [Fact]
    public async Task CVerify07_UTCID04_GetProfile_UserExistsButNoProfile_AutoProvisionesAndReturnsProfileResponse()
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = "noprofile@example.com",
            FullName        = "No Profile",
            Username        = "noprofile",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _sut.GetProfileByUserIdAsync(user.Id);

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.Username.Should().Be("noprofile");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // Valid JWT but user deleted from DB → 404.
    // At service level: userId not found in DB → ResourceNotFoundException.
    [Fact]
    public async Task CVerify07_UTCID05_GetProfile_UserDeletedFromDB_ThrowsResourceNotFoundException()
    {
        var deletedUserId = Guid.NewGuid();

        var act = async () => await _sut.GetProfileByUserIdAsync(deletedUserId);

        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage("*not found*");
    }
}
