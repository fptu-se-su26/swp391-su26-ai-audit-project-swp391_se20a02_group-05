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
/// Unit tests for ProfileService.UpdateProfileAsync (dateOfBirth) — CVerify-29 (7 UTCIDs).
/// PUT /api/v1/users/profile [Authorize] — updates user's date of birth via BirthDate field.
/// NOTE: Service does not validate age constraints — that is controller/DTO-level responsibility.
/// </summary>
public sealed class CVerify29_UpdateDateOfBirthTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>        _cacheService        = new();
    private readonly Mock<IStorageService>      _storageService      = new();
    private readonly Mock<IUsernameService>     _usernameService     = new();
    private readonly Mock<IAppLogger>           _logger              = new();
    private readonly Mock<IProjectService>      _projectService      = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();

    private readonly ProfileService _sut;

    public CVerify29_UpdateDateOfBirthTests()
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
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "DOB User",
            Username = "dobuser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
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

    private static UpdateProfileRequest MakeRequest(DateTimeOffset? birthDate, uint version = 0u) =>
        new(
            FullName: null, Bio: null, Location: null, PhoneNumber: null,
            BirthDate: birthDate, Headline: null, Company: null, Pronouns: null,
            CustomPronouns: null, PublicEmail: null,
            ProfileVisibility: "public", RecruiterVisibility: true, AiTalentDiscovery: "disabled",
            SocialLinks: null, AiSuggestionsJson: null, Version: version);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid DOB (1999-05-15) → success
    [Fact]
    public async Task CVerify29_UTCID01_UpdateDOB_ValidDate1999_StoresBirthDate()
    {
        var (user, _) = await SeedAsync();
        var dob = new DateTimeOffset(1999, 5, 15, 0, 0, 0, TimeSpan.Zero);

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(dob));

        result.Should().NotBeNull();
        result.BirthDate.Should().Be(dob);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Today's date — service stores without validation (controller validates age)
    [Fact]
    public async Task CVerify29_UTCID02_UpdateDOB_TodaysDate_ServiceAcceptsWithoutValidation()
    {
        var (user, _) = await SeedAsync();
        var today = DateTimeOffset.UtcNow;

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(today));

        result.Should().NotBeNull("service does not validate DOB age constraints");
        result.BirthDate.Should().NotBeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Future date — service stores without validation
    [Fact]
    public async Task CVerify29_UTCID03_UpdateDOB_FutureDate_ServiceAcceptsWithoutValidation()
    {
        var (user, _) = await SeedAsync();
        var future = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(future));

        result.Should().NotBeNull("service does not block future dates");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // 12 years ago — service stores without age validation
    [Fact]
    public async Task CVerify29_UTCID04_UpdateDOB_Under13Years_ServiceAcceptsWithoutValidation()
    {
        var (user, _) = await SeedAsync();
        var twelveYearsAgo = DateTimeOffset.UtcNow.AddYears(-12);

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(twelveYearsAgo));

        result.Should().NotBeNull("minimum-age validation is controller-level, not service-level");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: profile not found → ResourceNotFoundException.
    [Fact]
    public async Task CVerify29_UTCID05_UpdateDOB_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateProfileAsync(Guid.NewGuid(),
            MakeRequest(new DateTimeOffset(1999, 5, 15, 0, 0, 0, TimeSpan.Zero)));

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // Exactly 13 years ago (boundary) → service stores without age validation
    [Fact]
    public async Task CVerify29_UTCID06_UpdateDOB_Exactly13YearsAgo_ServiceAccepts()
    {
        var (user, _) = await SeedAsync();
        var exactly13YearsAgo = DateTimeOffset.UtcNow.AddYears(-13);

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(exactly13YearsAgo));

        result.Should().NotBeNull();
        result.BirthDate.Should().NotBeNull();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    // 1900-01-01 (very old date) → service stores without constraint
    [Fact]
    public async Task CVerify29_UTCID07_UpdateDOB_Year1900_ServiceAcceptsAncientDate()
    {
        var (user, _) = await SeedAsync();
        var ancient = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var result = await _sut.UpdateProfileAsync(user.Id, MakeRequest(ancient));

        result.Should().NotBeNull();
        result.BirthDate.Should().Be(ancient);
    }
}
