using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CVerify.API.Modules.Auth.Entities;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;
using CVerify.API.Modules.SourceCode.Clients;
using CVerify.API.Modules.SourceCode.Entities;
using CVerify.API.Modules.SourceCode.Services;
using CVerify.API.Modules.Profiles.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for SourceCodeProviderService.GetRepositoriesAsync — CVerify-67 (4 UTCIDs).
/// GET /api/v1/source-providers/repositories [Authorize] — paginated repository list.
/// NOTE: Filters using EF.Functions.ILike (language, search) are not supported by InMemory;
/// only non-filtered and bool-filter cases are tested here.
/// </summary>
public sealed class CVerify67_GetRepositoriesTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IRepositorySyncQueue> _syncQueue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SourceCodeProviderService>> _logger = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly SourceCodeProviderService _sut;

    public CVerify67_GetRepositoriesTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new SourceCodeProviderService(
            _context,
            _cacheService.Object,
            _syncQueue.Object,
            new EnvConfiguration(),
            _httpClientFactory.Object,
            _logger.Object,
            TimeProvider.System,
            Enumerable.Empty<ISourceCodeClient>(),
            _cvRepositoryIndexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid providerId)> SeedProviderAsync()
    {
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider
        {
            Id = providerId, UserId = userId, ProviderName = "github",
            ProviderKey = $"key-{userId}", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Completed",
        });
        await _context.SaveChangesAsync();
        return (userId, providerId);
    }

    private async Task SeedRepoAsync(Guid providerId, bool isPrivate = false, bool isAccessible = true, string? classification = null)
    {
        _context.SourceCodeRepositories.Add(new SourceCodeRepository
        {
            Id = Guid.NewGuid(),
            AuthProviderId = providerId,
            ExternalRepositoryId = Guid.NewGuid().ToString(),
            Name = $"repo-{Guid.NewGuid():N}",
            Owner = "owner",
            OwnerLogin = "ownerlogin",
            OwnerType = "User",
            IsPrivate = isPrivate,
            IsAccessible = isAccessible,
            Classification = classification,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastSyncedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // No repos in DB → 200 OK with empty paginated result.
    [Fact]
    public async Task CVerify67_UTCID01_GetRepositories_NoRepos_ReturnsEmptyList()
    {
        var (userId, _) = await SeedProviderAsync();

        var result = await _sut.GetRepositoriesAsync(userId, null, null, null, null, null, null, null, null, null, 1, 20);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty("no repositories seeded");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // User has 2 repos, no filters → 200 OK with all 2.
    [Fact]
    public async Task CVerify67_UTCID02_GetRepositories_NoFilters_ReturnsAllRepos()
    {
        var (userId, providerId) = await SeedProviderAsync();
        await SeedRepoAsync(providerId);
        await SeedRepoAsync(providerId);

        var result = await _sut.GetRepositoriesAsync(userId, null, null, null, null, null, null, null, null, null, 1, 20);

        result.TotalCount.Should().Be(2, "two repositories exist for this user");
        result.Items.Should().HaveCount(2);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → empty paginated result.
    [Fact]
    public async Task CVerify67_UTCID03_GetRepositories_NoJwtControllerLevel_ServiceReturnsEmptyForGhostUser()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetRepositoriesAsync(ghostUserId, null, null, null, null, null, null, null, null, null, 1, 20);

        result.Items.Should().BeEmpty("ghost user has no repositories");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Boundary: page 999 with no repos → empty paginated result.
    [Fact]
    public async Task CVerify67_UTCID04_GetRepositories_BoundaryPage_ReturnsEmptyPage()
    {
        var (userId, providerId) = await SeedProviderAsync();
        await SeedRepoAsync(providerId);

        var result = await _sut.GetRepositoriesAsync(userId, null, null, null, null, null, null, null, null, null, 999, 20);

        result.Items.Should().BeEmpty("page 999 has no items when only 1 repo exists");
        result.TotalCount.Should().Be(1, "total count reflects all repos regardless of page");
    }
}
