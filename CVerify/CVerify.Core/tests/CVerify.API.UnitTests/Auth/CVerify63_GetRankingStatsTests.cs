using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Diagnostics;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for ProfileService.GetRankingStatsAsync — CVerify-63 (3 UTCIDs).
/// GET /api/v1/ranking/stats [AllowAnonymous] — returns aggregate platform statistics.
/// </summary>
public sealed class CVerify63_GetRankingStatsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IStorageService> _storageService = new();
    private readonly Mock<IUsernameService> _usernameService = new();
    private readonly Mock<IAppLogger> _appLogger = new();
    private readonly Mock<IProjectService> _projectService = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly ProfileService _sut;

    public CVerify63_GetRankingStatsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _usernameService.Setup(u => u.Normalize(It.IsAny<string>())).Returns<string>(s => s);
        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);

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

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Empty DB → 200 OK with zeroed stats and empty trending engineers.
    [Fact]
    public async Task CVerify63_UTCID01_GetRankingStats_EmptyDatabase_ReturnsZeroStats()
    {
        var result = await _sut.GetRankingStatsAsync();

        result.Should().NotBeNull();
        result.TotalTalents.Should().Be(0, "no candidates in DB");
        result.TotalRepositories.Should().Be(0, "no verified repositories in DB");
        result.TotalCountries.Should().Be(0, "no location data");
        result.TrendingEngineers.Should().BeEmpty("no candidates to trend");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // DB with 3 candidates in different locations → TotalTalents=3, TotalCountries=2.
    [Fact]
    public async Task CVerify63_UTCID02_GetRankingStats_WithCandidates_ReturnsCorrectAggregates()
    {
        for (int i = 0; i < 3; i++)
        {
            var id = Guid.NewGuid();
            _context.Users.Add(new User { Id = id, Email = $"u{i}@test.com", FullName = $"U{i}", Username = $"u{i}", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow });
            _context.CandidateRankingProjections.Add(new CandidateRankingProjection
            {
                CandidateId = id, FullName = $"U{i}", Username = $"u{i}",
                Location = i < 2 ? "Vietnam" : "Singapore", // 2 unique locations
                CompositeScore = i * 10.0, AvailableForHire = true,
                OpenToWorkStatus = "casual", LastUpdatedAt = DateTimeOffset.UtcNow,
            });
        }
        await _context.SaveChangesAsync();

        var result = await _sut.GetRankingStatsAsync();

        result.TotalTalents.Should().Be(3);
        result.TotalCountries.Should().Be(2, "Vietnam and Singapore are distinct locations");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [AllowAnonymous] — service takes no userId, always succeeds.
    [Fact]
    public async Task CVerify63_UTCID03_GetRankingStats_NoAuth_ServiceStillReturnsStats()
    {
        // GetRankingStatsAsync has no auth dependency — it does not accept a userId.
        var result = await _sut.GetRankingStatsAsync(CancellationToken.None);

        result.Should().NotBeNull("endpoint is AllowAnonymous and service takes no userId");
    }
}
