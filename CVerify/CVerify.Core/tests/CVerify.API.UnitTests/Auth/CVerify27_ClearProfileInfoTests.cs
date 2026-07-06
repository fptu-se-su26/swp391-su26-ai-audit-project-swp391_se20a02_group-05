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
/// Unit tests for ProfileService.UpdateProfileAsync (clearing fields) — CVerify-27 (3 UTCIDs).
/// PUT /api/v1/users/profile [Authorize] — clears optional profile fields by sending null/empty.
/// </summary>
public sealed class CVerify27_ClearProfileInfoTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify27_ClearProfileInfoTests()
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

    private async Task<(User user, UserProfile profile)> SeedWithDataAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "Clear User",
            Username = "clearuser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        var profile = new UserProfile
        {
            UserId = user.Id, Username = user.Username,
            Bio = "My bio", Headline = "Engineer", Location = "HCM",
            ProfileVisibility = "public", RecruiterVisibility = true, AiTalentDiscovery = "disabled",
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return (user, profile);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // bio:'', headline:'' (empty strings) → fields cleared
    [Fact]
    public async Task CVerify27_UTCID01_ClearProfileInfo_EmptyStrings_ClearsFields()
    {
        var (user, _) = await SeedWithDataAsync();

        var result = await _sut.UpdateProfileAsync(user.Id, new UpdateProfileRequest(
            FullName: null, Bio: "", Location: null, PhoneNumber: null, BirthDate: null,
            Headline: "", Company: null, Pronouns: null, CustomPronouns: null, PublicEmail: null,
            ProfileVisibility: "public", RecruiterVisibility: true, AiTalentDiscovery: "disabled",
            SocialLinks: null, AiSuggestionsJson: null, Version: 0u));

        result.Should().NotBeNull();
        result.Bio.Should().BeNullOrEmpty("empty string clears the Bio field");
        result.Headline.Should().BeNullOrEmpty("empty string clears the Headline field");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // All optional fields set to null → profile returns with null optional fields
    [Fact]
    public async Task CVerify27_UTCID02_ClearProfileInfo_AllNullOptionals_ReturnsNullOptionals()
    {
        var (user, _) = await SeedWithDataAsync();

        var result = await _sut.UpdateProfileAsync(user.Id, new UpdateProfileRequest(
            FullName: null, Bio: null, Location: null, PhoneNumber: null, BirthDate: null,
            Headline: null, Company: null, Pronouns: null, CustomPronouns: null, PublicEmail: null,
            ProfileVisibility: "public", RecruiterVisibility: true, AiTalentDiscovery: "disabled",
            SocialLinks: null, AiSuggestionsJson: null, Version: 0u));

        result.Should().NotBeNull();
        result.Bio.Should().BeNull();
        result.Location.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: profile not found → ResourceNotFoundException.
    [Fact]
    public async Task CVerify27_UTCID03_ClearProfileInfo_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateProfileAsync(Guid.NewGuid(), new UpdateProfileRequest(
            FullName: null, Bio: null, Location: null, PhoneNumber: null, BirthDate: null,
            Headline: null, Company: null, Pronouns: null, CustomPronouns: null, PublicEmail: null,
            ProfileVisibility: "public", RecruiterVisibility: true, AiTalentDiscovery: "disabled",
            SocialLinks: null, AiSuggestionsJson: null, Version: 0u));

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
