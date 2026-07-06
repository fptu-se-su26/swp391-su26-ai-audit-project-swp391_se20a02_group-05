using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
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
/// Unit tests for ProfileService.GetPublicProfileByUsernameAsync — CVerify-61 (4 UTCIDs).
/// GET /api/v1/profile/{username} — returns public profile by username.
/// UTCID01/UTCID04 (happy path) cannot be fully tested with InMemory DB because the method
/// executes a FromSqlRaw query for repositories; those cases require PostgreSQL integration tests.
/// UTCID02 (not found) and UTCID03 (private/connections profile) throw before FromSqlRaw.
/// </summary>
public sealed class CVerify61_GetPublicProfileTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IStorageService> _storageService = new();
    private readonly Mock<IUsernameService> _usernameService = new();
    private readonly Mock<IAppLogger> _appLogger = new();
    private readonly Mock<IProjectService> _projectService = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly ProfileService _sut;

    public CVerify61_GetPublicProfileTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        // Normalize returns the input unchanged in tests
        _usernameService
            .Setup(u => u.Normalize(It.IsAny<string>()))
            .Returns<string>(s => s);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);

        _projectService
            .Setup(p => p.UpgradeRepositoryLinkedProjectsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new ProfileService(
            _context,
            _cacheService.Object,
            _storageService.Object,
            _usernameService.Object,
            TimeProvider.System,
            _appLogger.Object,
            _projectService.Object,
            _cvRepositoryIndexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<Guid> SeedUserAsync(string username, string visibility = "public")
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId,
            Email = $"{username}@test.com",
            FullName = username,
            Username = username,
            Status = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        });
        _context.UserProfiles.Add(new UserProfile
        {
            UserId = userId,
            Username = username,
            ProfileVisibility = visibility,
            RecruiterVisibility = true,
            AiTalentDiscovery = "disabled",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
        return userId;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // username='private_user' with private visibility → 404 ResourceNotFoundException.
    // Documents the private-profile path that throws before the FromSqlRaw repo query.
    [Fact]
    public async Task CVerify61_UTCID01_GetPublicProfile_PrivateVisibility_ThrowsResourceNotFoundException()
    {
        await SeedUserAsync("private_user", "private");

        var act = async () => await _sut.GetPublicProfileByUsernameAsync("private_user");

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "private profiles are hidden from public lookup");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // username='ghost_user' not in DB → 404 ResourceNotFoundException.
    [Fact]
    public async Task CVerify61_UTCID02_GetPublicProfile_UsernameNotFound_ThrowsResourceNotFoundException()
    {
        var act = async () => await _sut.GetPublicProfileByUsernameAsync("ghost_user");

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "no user with this username exists");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // username='connections_user' with visibility='connections' → 404 ResourceNotFoundException.
    [Fact]
    public async Task CVerify61_UTCID03_GetPublicProfile_ConnectionsVisibility_ThrowsResourceNotFoundException()
    {
        await SeedUserAsync("connections_user", "connections");

        var act = async () => await _sut.GetPublicProfileByUsernameAsync("connections_user");

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "connections-only profiles are hidden from public lookup");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Empty/whitespace username → 404 ResourceNotFoundException (null-guard at service entry).
    [Fact]
    public async Task CVerify61_UTCID04_GetPublicProfile_EmptyUsername_ThrowsResourceNotFoundException()
    {
        var act = async () => await _sut.GetPublicProfileByUsernameAsync("  ");

        await act.Should().ThrowAsync<ResourceNotFoundException>(
            "null or whitespace username is an invalid lookup key");
    }
}
