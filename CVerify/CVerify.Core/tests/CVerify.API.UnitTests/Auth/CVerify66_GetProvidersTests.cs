using System;
using System.Collections.Generic;
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
using CVerify.API.Modules.SourceCode.Services;
using CVerify.API.Modules.Profiles.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for SourceCodeProviderService.GetProvidersAsync — CVerify-66 (3 UTCIDs).
/// GET /api/v1/source-providers [Authorize] — returns connected GitHub/GitLab providers.
/// </summary>
public sealed class CVerify66_GetProvidersTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IRepositorySyncQueue> _syncQueue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SourceCodeProviderService>> _logger = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly SourceCodeProviderService _sut;

    public CVerify66_GetProvidersTests()
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

    private async Task<(Guid userId, Guid providerId)> SeedProviderAsync(string providerName = "github")
    {
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider
        {
            Id = providerId,
            UserId = userId,
            ProviderName = providerName,
            ProviderKey = $"{providerName}-key-{userId}",
            ProviderUsername = $"gh_user_{userId:N}",
            ScopeValidationStatus = ProviderScopeStatus.Valid,
            SyncStatus = "Completed",
        });
        await _context.SaveChangesAsync();
        return (userId, providerId);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // User with GitHub + GitLab providers → list of 2 SourceCodeProviderDto.
    [Fact]
    public async Task CVerify66_UTCID01_GetProviders_HasMultipleProviders_ReturnsList()
    {
        var userId = Guid.NewGuid();
        _context.AuthProviders.Add(new AuthProvider { Id = Guid.NewGuid(), UserId = userId, ProviderName = "github", ProviderKey = "gh-key", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Completed" });
        _context.AuthProviders.Add(new AuthProvider { Id = Guid.NewGuid(), UserId = userId, ProviderName = "gitlab", ProviderKey = "gl-key", ScopeValidationStatus = ProviderScopeStatus.Valid, SyncStatus = "Completed" });
        await _context.SaveChangesAsync();

        var result = await _sut.GetProvidersAsync(userId);

        result.Should().HaveCount(2, "one GitHub and one GitLab provider linked");
        result.Select(p => p.ProviderName).Should().Contain(new[] { "github", "gitlab" });
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // User has no providers → empty list (not an error).
    [Fact]
    public async Task CVerify66_UTCID02_GetProviders_NoProviders_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetProvidersAsync(userId);

        result.Should().BeEmpty("no providers linked to this user");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → empty list (no providers found for unknown user).
    [Fact]
    public async Task CVerify66_UTCID03_GetProviders_NoJwtControllerLevel_ServiceReturnsEmptyForGhostUser()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetProvidersAsync(ghostUserId);

        result.Should().BeEmpty("ghost user has no providers in DB");
    }
}
