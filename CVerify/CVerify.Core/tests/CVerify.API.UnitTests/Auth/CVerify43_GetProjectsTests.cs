using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for ProjectService.GetProjectsAsync — CVerify-43 (3 UTCIDs).
/// GET /api/v1/users/projects [Authorize] — retrieves list of user project entries.
/// </summary>
public sealed class CVerify43_GetProjectsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly ProjectService _sut;

    public CVerify43_GetProjectsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _indexer
            .Setup(i => i.IndexUserCvRepositoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new ProjectService(_context, _indexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task SeedProjectAsync(Guid userId, string name = "My Project")
    {
        _context.ProjectEntries.Add(new ProjectEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Description = "Project description",
            VerificationLevel = ProjectVerificationLevel.Independent,
            VerificationStatus = ProjectVerificationStatus.Unverified,
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // User has projects → returns non-empty list (200)
    [Fact]
    public async Task CVerify43_UTCID01_GetProjects_HasProjects_ReturnsPopulatedList()
    {
        var userId = Guid.NewGuid();
        await SeedProjectAsync(userId, "E-Commerce App");
        await SeedProjectAsync(userId, "Portfolio Site");

        var result = await _sut.GetProjectsAsync(userId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "E-Commerce App");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // User has no projects → returns empty list (200)
    [Fact]
    public async Task CVerify43_UTCID02_GetProjects_NoProjects_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetProjectsAsync(userId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId has no projects → empty list.
    [Fact]
    public async Task CVerify43_UTCID03_GetProjects_NoJwtControllerLevel_ServiceReturnsEmpty()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetProjectsAsync(ghostUserId);

        result.Should().NotBeNull();
        result.Should().BeEmpty("JWT auth is controller responsibility");
    }
}
