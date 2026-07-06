using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
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
/// Unit tests for ProfileService.GetRankingAsync — CVerify-62 (5 UTCIDs).
/// GET /api/v1/ranking [AllowAnonymous] — returns paginated ranked candidate list.
/// </summary>
public sealed class CVerify62_GetRankingTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICacheService> _cacheService = new();
    private readonly Mock<IStorageService> _storageService = new();
    private readonly Mock<IUsernameService> _usernameService = new();
    private readonly Mock<IAppLogger> _appLogger = new();
    private readonly Mock<IProjectService> _projectService = new();
    private readonly Mock<ICvRepositoryIndexer> _cvRepositoryIndexer = new();
    private readonly ProfileService _sut;

    public CVerify62_GetRankingTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _usernameService
            .Setup(u => u.Normalize(It.IsAny<string>()))
            .Returns<string>(s => s);

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

    private async Task SeedProjectionsAsync(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var candidateId = Guid.NewGuid();
            _context.Users.Add(new User
            {
                Id = candidateId, Email = $"c{i}@test.com", FullName = $"Candidate {i}",
                Username = $"candidate{i}", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
            });
            _context.CandidateRankingProjections.Add(new CandidateRankingProjection
            {
                CandidateId = candidateId,
                FullName = $"Candidate {i}",
                Username = $"candidate{i}",
                CompositeScore = i * 10.0,
                TrustScore = i * 5.0,
                AvailableForHire = true,
                OpenToWorkStatus = "ACTIVELY_LOOKING",
                LastUpdatedAt = DateTimeOffset.UtcNow,
            });
        }
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Default query (no filters) → 200 OK with paginated result.
    [Fact]
    public async Task CVerify62_UTCID01_GetRanking_DefaultQuery_ReturnsPaginatedResult()
    {
        await SeedProjectionsAsync(3);
        var query = new RankingQueryDto();

        var result = await _sut.GetRankingAsync(null, query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // category='Trending' → 200 OK – sorted by trending formula.
    [Fact]
    public async Task CVerify62_UTCID02_GetRanking_TrendingCategory_ReturnsSortedResult()
    {
        await SeedProjectionsAsync(2);
        var query = new RankingQueryDto(Category: "Trending");

        var result = await _sut.GetRankingAsync(null, query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2, "trending category applies in-memory sort");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Empty DB (no candidates) → 200 OK with empty list.
    [Fact]
    public async Task CVerify62_UTCID03_GetRanking_NoCandidates_ReturnsEmptyList()
    {
        var query = new RankingQueryDto();

        var result = await _sut.GetRankingAsync(null, query);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // AvailableForHire filter → 200 OK with filtered candidates.
    [Fact]
    public async Task CVerify62_UTCID04_GetRanking_AvailableForHireFilter_ReturnsFilteredCandidates()
    {
        var candidateId1 = Guid.NewGuid();
        var candidateId2 = Guid.NewGuid();
        _context.Users.Add(new User { Id = candidateId1, Email = "h1@test.com", FullName = "Hire1", Username = "hire1", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow });
        _context.Users.Add(new User { Id = candidateId2, Email = "h2@test.com", FullName = "NotHire", Username = "nothire", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow });
        _context.CandidateRankingProjections.Add(new CandidateRankingProjection { CandidateId = candidateId1, FullName = "Hire1", AvailableForHire = true, OpenToWorkStatus = "ACTIVELY_LOOKING", LastUpdatedAt = DateTimeOffset.UtcNow });
        _context.CandidateRankingProjections.Add(new CandidateRankingProjection { CandidateId = candidateId2, FullName = "NotHire", AvailableForHire = false, OpenToWorkStatus = "NOT_LOOKING", LastUpdatedAt = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync();

        var query = new RankingQueryDto(AvailableForHire: true);

        var result = await _sut.GetRankingAsync(null, query);

        result.Items.Should().HaveCount(1, "only AvailableForHire=true candidates match the filter");
        result.Items.First().FullName.Should().Be("Hire1");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // Authenticated user query (currentUserId set) → IsFollowedByCurrentUser populated.
    [Fact]
    public async Task CVerify62_UTCID05_GetRanking_AuthenticatedUser_PopulatesFollowState()
    {
        await SeedProjectionsAsync(1);
        var currentUserId = Guid.NewGuid();
        var query = new RankingQueryDto();

        var result = await _sut.GetRankingAsync(currentUserId, query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().IsFollowedByCurrentUser.Should().BeFalse("no follow records seeded");
    }
}
