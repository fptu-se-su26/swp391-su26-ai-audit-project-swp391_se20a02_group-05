using System;
using System.Collections.Generic;
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
/// Unit tests for ProjectService.ReorderProjectsAsync — CVerify-46 (3 UTCIDs).
/// PUT /api/v1/users/projects/reorder [Authorize] — reorders project entries by display order.
/// NOTE: Unlike WorkExperience reorder, this service SILENTLY ignores non-matching IDs.
/// </summary>
public sealed class CVerify46_ReorderProjectsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly ProjectService _sut;

    public CVerify46_ReorderProjectsTests()
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

    private async Task<(Guid userId, List<Guid> ids)> SeedProjectsAsync(int count = 2)
    {
        var userId = Guid.NewGuid();
        var ids = new List<Guid>();
        for (int i = 0; i < count; i++)
        {
            var id = Guid.NewGuid();
            ids.Add(id);
            _context.ProjectEntries.Add(new ProjectEntry
            {
                Id = id,
                UserId = userId,
                Name = $"Project {i}",
                Description = $"Desc {i}",
                VerificationLevel = ProjectVerificationLevel.Independent,
                VerificationStatus = ProjectVerificationStatus.Unverified,
                DisplayOrder = i,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            });
        }
        await _context.SaveChangesAsync();
        return (userId, ids);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // orderedIds: all valid project GUIDs in new order → reorders, returns 204
    [Fact]
    public async Task CVerify46_UTCID01_ReorderProjects_ValidIds_UpdatesDisplayOrder()
    {
        var (userId, ids) = await SeedProjectsAsync(2);
        var reversed = new List<Guid> { ids[1], ids[0] };

        await _sut.ReorderProjectsAsync(userId, reversed);

        var first = await _context.ProjectEntries.FindAsync(ids[1]);
        var second = await _context.ProjectEntries.FindAsync(ids[0]);
        first!.DisplayOrder.Should().Be(0);
        second!.DisplayOrder.Should().Be(1);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // orderedIds contains non-existent GUID → service silently ignores it (400 is controller-level)
    [Fact]
    public async Task CVerify46_UTCID02_ReorderProjects_NonExistentGuid_ServiceIgnoresSilently()
    {
        var (userId, _) = await SeedProjectsAsync(1);

        var act = async () => await _sut.ReorderProjectsAsync(userId, new List<Guid> { Guid.NewGuid() });

        await act.Should().NotThrowAsync("ProjectService silently ignores non-matching IDs");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → no projects found, silently succeeds.
    [Fact]
    public async Task CVerify46_UTCID03_ReorderProjects_NoJwtControllerLevel_ServiceSucceeds()
    {
        var act = async () => await _sut.ReorderProjectsAsync(Guid.NewGuid(), new List<Guid> { Guid.NewGuid() });

        await act.Should().NotThrowAsync("JWT auth is controller responsibility");
    }
}
