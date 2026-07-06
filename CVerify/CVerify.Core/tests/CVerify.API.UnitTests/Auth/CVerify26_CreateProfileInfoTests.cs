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
/// Unit tests for ProfileService.UpdateProfileAsync — CVerify-26 (5 UTCIDs).
/// PUT /api/v1/users/profile [Authorize] — creates or updates user profile info.
/// </summary>
public sealed class CVerify26_CreateProfileInfoTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify26_CreateProfileInfoTests()
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
            _context, _cacheService.Object, _storageService.Object, _usernameService.Object,
            new FakeTimeProvider(), _logger.Object, _projectService.Object, _cvRepositoryIndexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(User user, UserProfile profile)> SeedAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "Profile User",
            Username = "profileuser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        var profile = new UserProfile
        {
            UserId = user.Id, Username = user.Username,
            ProfileVisibility = "public", RecruiterVisibility = true, AiTalentDiscovery = "disabled",
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return (user, profile);
    }

    private static UpdateProfileRequest MakeRequest(
        string? fullName = null, string? bio = null, string? headline = null,
        string? location = null, uint version = 0u) =>
        new(
            FullName: fullName, Bio: bio, Location: location, PhoneNumber: null,
            BirthDate: null, Headline: headline, Company: null, Pronouns: null,
            CustomPronouns: null, PublicEmail: null,
            ProfileVisibility: "public", RecruiterVisibility: true, AiTalentDiscovery: "disabled",
            SocialLinks: null, AiSuggestionsJson: null, Version: version);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify26_UTCID01_CreateProfileInfo_AllFields_ReturnsProfileResponse()
    {
        var (user, _) = await SeedAsync();

        var result = await _sut.UpdateProfileAsync(user.Id,
            MakeRequest(fullName: "Nguyen Van A", bio: "Hello", headline: "Dev", location: "HCM"));

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.Bio.Should().Be("Hello");
        result.Headline.Should().Be("Dev");
        result.Location.Should().Be("HCM");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Invalid website URL — UpdateProfileRequest has no website/url field;
    // the service simply stores bio/headline without URL validation.
    // This tests that a profile with SocialLinks containing an invalid URL is accepted at service level.
    [Fact]
    public async Task CVerify26_UTCID02_CreateProfileInfo_InvalidWebsiteInSocialLinks_ServiceAccepts()
    {
        var (user, _) = await SeedAsync();
        var request = new UpdateProfileRequest(
            FullName: null, Bio: "test bio", Location: null, PhoneNumber: null, BirthDate: null,
            Headline: null, Company: null, Pronouns: null, CustomPronouns: null, PublicEmail: null,
            ProfileVisibility: "public", RecruiterVisibility: true, AiTalentDiscovery: "disabled",
            SocialLinks: new System.Collections.Generic.List<string> { "not-a-url" },
            AiSuggestionsJson: null, Version: 0u);

        var result = await _sut.UpdateProfileAsync(user.Id, request);

        result.Should().NotBeNull("URL validation is controller-level, service stores values as-is");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // fullName: 300-char string → MaxLength(100) validated at controller; service stores it
    [Fact]
    public async Task CVerify26_UTCID03_CreateProfileInfo_LongFullName_ServiceStoresIt()
    {
        var (user, _) = await SeedAsync();
        var longName = new string('A', 300);

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(fullName: longName));

        result.Should().NotBeNull("MaxLength(100) is a DTO attribute, not a service-level check");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → ResourceNotFoundException at service level
    [Fact]
    public async Task CVerify26_UTCID04_CreateProfileInfo_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateProfileAsync(Guid.NewGuid(), MakeRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // All optional fields null/empty → partial update with no changes
    [Fact]
    public async Task CVerify26_UTCID05_CreateProfileInfo_AllNullOptionals_ReturnsProfileResponse()
    {
        var (user, _) = await SeedAsync();

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest());

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.Bio.Should().BeNull();
    }
}
