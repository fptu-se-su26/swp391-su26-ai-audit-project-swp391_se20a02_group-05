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
/// Unit tests for SourceCodeProviderService.GetOrganizationsAsync — CVerify-68 (3 UTCIDs).
/// GET /api/v1/source-providers/organizations [Authorize] — returns linked GitHub/GitLab organizations.
/// </summary>
public sealed class CVerify68_GetOrganizationsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IRepositorySyncQueue> _syncQueue = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<SourceCodeProviderService>> _logger = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly SourceCodeProviderService _sut;

    public CVerify68_GetOrganizationsTests()
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

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // User has 1 active GitHub org → list with 1 ExternalOrganizationResponseDto.
    [Fact]
    public async Task CVerify68_UTCID01_GetOrganizations_HasOrg_ReturnsOrgList()
    {
        var (userId, providerId) = await SeedProviderAsync();
        _context.ExternalOrganizations.Add(new ExternalOrganization
        {
            Id = Guid.NewGuid(),
            AuthProviderId = providerId,
            ExternalId = "org-123",
            Name = "MyOrg",
            Login = "myorg",
            Type = "github",
            IsActive = true,
        });
        await _context.SaveChangesAsync();

        var result = await _sut.GetOrganizationsAsync(userId);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("MyOrg");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // User has no orgs → empty list.
    [Fact]
    public async Task CVerify68_UTCID02_GetOrganizations_NoOrgs_ReturnsEmptyList()
    {
        var (userId, _) = await SeedProviderAsync();

        var result = await _sut.GetOrganizationsAsync(userId);

        result.Should().BeEmpty("no external organizations linked");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → empty list.
    [Fact]
    public async Task CVerify68_UTCID03_GetOrganizations_NoJwtControllerLevel_ServiceReturnsEmptyForGhostUser()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetOrganizationsAsync(ghostUserId);

        result.Should().BeEmpty();
    }
}
