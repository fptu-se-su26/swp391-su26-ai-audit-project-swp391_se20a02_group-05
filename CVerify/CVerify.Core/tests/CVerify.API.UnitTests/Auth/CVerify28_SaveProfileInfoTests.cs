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
/// Unit tests for ProfileService.UpdateProfileAsync (persist/save) — CVerify-28 (4 UTCIDs).
/// PUT /api/v1/users/profile [Authorize] — persists profile changes with full validation.
/// </summary>
public sealed class CVerify28_SaveProfileInfoTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify28_SaveProfileInfoTests()
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

    private async Task<(User user, UserProfile profile)> SeedAsync(string? bio = null)
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "Save User",
            Username = "saveuser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        var profile = new UserProfile
        {
            UserId = user.Id, Username = user.Username,
            Bio = bio,
            ProfileVisibility = "public", RecruiterVisibility = true, AiTalentDiscovery = "disabled",
        };
        _context.Users.Add(user);
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return (user, profile);
    }

    private static UpdateProfileRequest MakeRequest(
        string? bio = "Updated bio", uint version = 0u) =>
        new(
            FullName: null, Bio: bio, Location: null, PhoneNumber: null, BirthDate: null,
            Headline: null, Company: null, Pronouns: null, CustomPronouns: null, PublicEmail: null,
            ProfileVisibility: "public", RecruiterVisibility: true, AiTalentDiscovery: "disabled",
            SocialLinks: null, AiSuggestionsJson: null, Version: version);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify28_UTCID01_SaveProfileInfo_ValidData_ReturnsProfileResponse()
    {
        var (user, _) = await SeedAsync();

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(bio: "My updated bio"));

        result.Should().NotBeNull();
        result.Bio.Should().Be("My updated bio");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Concurrency conflict (stale version token) → ProfileException
    [Fact]
    public async Task CVerify28_UTCID02_SaveProfileInfo_StaleVersion_ThrowsConcurrencyException()
    {
        var (user, _) = await SeedAsync(bio: "Original bio");

        var act = async () => await _sut.UpdateProfileAsync(user.Id, MakeRequest(version: 42u));

        await act.Should().ThrowAsync<ProfileException>()
            .WithMessage("*modified*");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: profile not found → ResourceNotFoundException.
    [Fact]
    public async Task CVerify28_UTCID03_SaveProfileInfo_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateProfileAsync(Guid.NewGuid(), MakeRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Same data as existing → idempotent (no error, no actual change)
    [Fact]
    public async Task CVerify28_UTCID04_SaveProfileInfo_SameDataAsExisting_SucceedsIdempotently()
    {
        var (user, _) = await SeedAsync(bio: "My bio");

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(bio: "My bio"));

        result.Should().NotBeNull();
        result.Bio.Should().Be("My bio", "re-saving the same data should succeed without error");
    }
}
