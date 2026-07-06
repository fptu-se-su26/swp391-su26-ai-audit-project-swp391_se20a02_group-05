using System;
using System.Collections.Generic;
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
/// Unit tests for ProfileService.UpdateProfileAsync — CVerify-08 (7 UTCIDs).
/// </summary>
public sealed class CVerify08_UpdateProfileTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>         _cacheService         = new();
    private readonly Mock<IStorageService>       _storageService       = new();
    private readonly Mock<IUsernameService>      _usernameService      = new();
    private readonly Mock<IAppLogger>            _logger               = new();
    private readonly Mock<IProjectService>       _projectService       = new();
    private readonly Mock<ICvRepositoryIndexer>  _cvRepositoryIndexer  = new();

    private readonly ProfileService _sut;

    public CVerify08_UpdateProfileTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cvRepositoryIndexer
            .Setup(c => c.IndexUserCvRepositoriesAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
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

    private async Task<(User user, UserProfile profile)> SeedUserAndProfileAsync(
        string email = "user@example.com",
        string? bio = "Old bio",
        string? headline = "Old headline")
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = email,
            FullName        = "Test User",
            Username        = "testuser",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        var profile = new UserProfile
        {
            UserId              = user.Id,
            Username            = user.Username,
            Bio                 = bio,
            Headline            = headline,
            ProfileVisibility   = "public",
            RecruiterVisibility = true,
            AiTalentDiscovery   = "disabled",
            Version             = 0,
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return (user, profile);
    }

    private static UpdateProfileRequest BuildRequest(
        string? bio = null,
        string? headline = null,
        string? fullName = null,
        string? location = null,
        List<string>? socialLinks = null,
        uint version = 0) =>
        new(
            FullName:           fullName,
            Bio:                bio,
            Location:           location,
            PhoneNumber:        null,
            BirthDate:          null,
            Headline:           headline,
            Company:            null,
            Pronouns:           null,
            CustomPronouns:     null,
            PublicEmail:        null,
            ProfileVisibility:  "public",
            RecruiterVisibility: true,
            AiTalentDiscovery:  "disabled",
            SocialLinks:        socialLinks,
            AiSuggestionsJson:  null,
            Version:            version);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify08_UTCID01_UpdateProfile_AllFields_ReturnsUpdatedProfileResponse()
    {
        var (user, _) = await SeedUserAndProfileAsync();

        var request = BuildRequest(bio: "New bio", headline: "New headline", location: "Hanoi", fullName: "New Name", version: 0);
        var result = await _sut.UpdateProfileAsync(user.Id, request, "127.0.0.1", "TestAgent");

        result.Should().NotBeNull();
        result.Bio.Should().Be("New bio");
        result.Headline.Should().Be("New headline");
        result.Location.Should().Be("Hanoi");
        result.FullName.Should().Be("New Name");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify08_UTCID02_UpdateProfile_BioOnly_ReturnsUpdatedProfileResponse()
    {
        var (user, _) = await SeedUserAndProfileAsync();

        var request = BuildRequest(bio: "Updated bio only", version: 0);
        var result = await _sut.UpdateProfileAsync(user.Id, request);

        result.Should().NotBeNull();
        result.Bio.Should().Be("Updated bio only");
        result.Headline.Should().BeNull("only bio was provided in the update request");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] handles 401.
    // At service level: profile not found for userId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify08_UTCID03_UpdateProfile_NoJwtControllerLevel_ServiceThrowsForMissingProfile()
    {
        var request = BuildRequest(bio: "Some bio", version: 0);

        var act = async () => await _sut.UpdateProfileAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // bio > 1000 chars → controller model validation [MaxLength(1000)] returns 400.
    // At service level: service stores bio as-is (no service-level length check).
    [Fact]
    public async Task CVerify08_UTCID04_UpdateProfile_BioOver1000Chars_ServiceStoresBioWithoutValidation()
    {
        var (user, _) = await SeedUserAndProfileAsync();
        var longBio = new string('x', 1001);

        var request = BuildRequest(bio: longBio, version: 0);
        var result = await _sut.UpdateProfileAsync(user.Id, request);

        result.Bio.Should().Be(longBio, "service does not validate MaxLength — that is controller responsibility");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // SocialLinks with invalid URL format → controller would validate; service stores as-is.
    [Fact]
    public async Task CVerify08_UTCID05_UpdateProfile_InvalidUrlInSocialLinks_ServiceStoresWithoutValidation()
    {
        var (user, _) = await SeedUserAndProfileAsync();

        var request = BuildRequest(socialLinks: new List<string> { "not-a-valid-url" }, version: 0);
        var result = await _sut.UpdateProfileAsync(user.Id, request);

        result.SocialLinks.Should().Contain("not-a-valid-url", "service does not validate URL format — controller responsibility");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify08_UTCID06_UpdateProfile_EmptyStringBio_ReturnsProfileWith200()
    {
        var (user, _) = await SeedUserAndProfileAsync(bio: "Previous bio");

        var request = BuildRequest(bio: "", version: 0);
        var result = await _sut.UpdateProfileAsync(user.Id, request);

        result.Should().NotBeNull();
        result.Bio.Should().BeEmpty("empty string bio is a valid boundary input");
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    // Headline exactly 200 chars (controller [MaxLength(50)] rejects, but service stores at service level).
    [Fact]
    public async Task CVerify08_UTCID07_UpdateProfile_Headline200Chars_ServiceStoresBoundaryValue()
    {
        var (user, _) = await SeedUserAndProfileAsync();
        var headline200 = new string('H', 200);

        var request = BuildRequest(headline: headline200, version: 0);
        var result = await _sut.UpdateProfileAsync(user.Id, request);

        result.Headline.Should().Be(headline200, "service does not enforce MaxLength on headline — controller responsibility");
    }
}
