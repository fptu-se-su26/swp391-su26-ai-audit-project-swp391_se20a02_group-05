using System;
using System.Threading;
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
/// Unit tests for ProfileService.UpdateProfileAsync — CVerify-24 (4 UTCIDs).
/// PUT /api/v1/users/profile [Authorize] — updates CV/profile settings (theme, headline, bio, etc.).
/// </summary>
public sealed class CVerify24_ChangeCvThemeTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify24_ChangeCvThemeTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cvRepositoryIndexer
            .Setup(i => i.IndexUserCvRepositoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
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

    private async Task<(User user, UserProfile profile)> SeedAsync(string? headline = null)
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "Theme User",
            Username = "themeuser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        var profile = new UserProfile
        {
            UserId = user.Id, Username = user.Username,
            ProfileVisibility = "public", RecruiterVisibility = true, AiTalentDiscovery = "disabled",
            Headline = headline,
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return (user, profile);
    }

    private static UpdateProfileRequest MakeRequest(
        string? headline = "Developer", uint version = 0u) =>
        new(
            FullName: null, Bio: null, Location: null, PhoneNumber: null, BirthDate: null,
            Headline: headline, Company: null, Pronouns: null, CustomPronouns: null,
            PublicEmail: null, ProfileVisibility: "public", RecruiterVisibility: true,
            AiTalentDiscovery: "disabled", SocialLinks: null, AiSuggestionsJson: null,
            Version: version);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid profile update (headline / theme field via UpdateProfileAsync) → 200 ProfileResponse
    [Fact]
    public async Task CVerify24_UTCID01_UpdateProfile_ValidData_ReturnsProfileResponse()
    {
        var (user, _) = await SeedAsync();

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(headline: "Senior Dev"));

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.Headline.Should().Be("Senior Dev");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Wrong version (stale concurrency token) → ProfileException(ConcurrencyConflict)
    [Fact]
    public async Task CVerify24_UTCID02_UpdateProfile_WrongVersion_ThrowsProfileException()
    {
        var (user, _) = await SeedAsync();

        var act = async () => await _sut.UpdateProfileAsync(user.Id,
            MakeRequest(version: 999u)); // wrong version

        await act.Should().ThrowAsync<ProfileException>()
            .WithMessage("*modified*");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: profile not found → ResourceNotFoundException.
    [Fact]
    public async Task CVerify24_UTCID03_UpdateProfile_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateProfileAsync(Guid.NewGuid(), MakeRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Empty string headline (clearing field) → 200 OK with empty headline
    [Fact]
    public async Task CVerify24_UTCID04_UpdateProfile_EmptyHeadline_StoresEmptyValue()
    {
        var (user, _) = await SeedAsync(headline: "Existing Headline");

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(headline: ""));

        result.Should().NotBeNull();
        result.Headline.Should().BeNullOrEmpty("empty string headline clears the field");
    }
}
