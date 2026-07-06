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
/// Unit tests for SourceCodeProviderService.GetDistinctCategoriesAsync — CVerify-69 (3 UTCIDs).
/// GET /api/v1/source-providers/repositories/categories [Authorize] — returns distinct repo categories.
/// </summary>
public sealed class CVerify69_GetRepositoryCategoriesTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IRepositorySyncQueue> _syncQueue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SourceCodeProviderService>> _logger = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly SourceCodeProviderService _sut;

    public CVerify69_GetRepositoryCategoriesTests()
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

    private void SeedRepo(Guid providerId, string? classification, bool isAccessible = true)
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
            IsPrivate = false,
            IsAccessible = isAccessible,
            Classification = classification,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastSyncedAt = DateTimeOffset.UtcNow,
        });
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // User has 3 repos with 2 distinct categories → sorted distinct list.
    [Fact]
    public async Task CVerify69_UTCID01_GetCategories_HasReposWithCategories_ReturnsDistinctSortedList()
    {
        var (userId, providerId) = await SeedProviderAsync();
        SeedRepo(providerId, "Backend");
        SeedRepo(providerId, "Frontend");
        SeedRepo(providerId, "Backend"); // duplicate
        await _context.SaveChangesAsync();

        var result = await _sut.GetDistinctCategoriesAsync(userId);

        result.Should().BeEquivalentTo(new[] { "Backend", "Frontend" }, opts => opts.WithStrictOrdering(),
            "2 distinct categories in alphabetical order");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // No repos → empty list.
    [Fact]
    public async Task CVerify69_UTCID02_GetCategories_NoRepos_ReturnsEmptyList()
    {
        var (userId, _) = await SeedProviderAsync();

        var result = await _sut.GetDistinctCategoriesAsync(userId);

        result.Should().BeEmpty();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → empty list.
    [Fact]
    public async Task CVerify69_UTCID03_GetCategories_NoJwtControllerLevel_ServiceReturnsEmpty()
    {
        var result = await _sut.GetDistinctCategoriesAsync(Guid.NewGuid());

        result.Should().BeEmpty();
    }
}
