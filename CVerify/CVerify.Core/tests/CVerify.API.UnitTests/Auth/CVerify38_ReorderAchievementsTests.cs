using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AchievementService.ReorderAchievementsAsync — CVerify-38 (3 UTCIDs).
/// PUT /api/v1/users/achievements/reorder [Authorize] — reorders achievement display order.
/// </summary>
public sealed class CVerify38_ReorderAchievementsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AchievementService _sut;

    public CVerify38_ReorderAchievementsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new AchievementService(_context, _storageService.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, List<Guid> ids)> SeedAchievementsAsync(int count = 2)
    {
        var userId = Guid.NewGuid();
        var ids = new List<Guid>();
        for (int i = 0; i < count; i++)
        {
            var id = Guid.NewGuid();
            ids.Add(id);
            _context.AcademicAchievements.Add(new AcademicAchievement
            {
                Id = id,
                UserId = userId,
                Title = $"Award {i}",
                Issuer = "IEEE",
                IssueDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Description = $"Description {i}",
                DisplayOrder = i,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            });
        }
        await _context.SaveChangesAsync();
        return (userId, ids);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid orderedIds → achievements reordered, display order updated (204)
    [Fact]
    public async Task CVerify38_UTCID01_ReorderAchievements_ValidIds_UpdatesDisplayOrder()
    {
        var (userId, ids) = await SeedAchievementsAsync(2);
        var reversed = new List<Guid> { ids[1], ids[0] };

        await _sut.ReorderAchievementsAsync(userId, reversed);

        var first = await _context.AcademicAchievements.FindAsync(ids[1]);
        var second = await _context.AcademicAchievements.FindAsync(ids[0]);
        first!.DisplayOrder.Should().Be(0, "first in reversed list gets order 0");
        second!.DisplayOrder.Should().Be(1, "second in reversed list gets order 1");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-matching GUID in list → controller catches invalid format at 400;
    // Service level: silently ignores GUIDs not found in DB.
    [Fact]
    public async Task CVerify38_UTCID02_ReorderAchievements_NonMatchingGuid_ServiceIgnoresSilently()
    {
        var (userId, _) = await SeedAchievementsAsync(1);

        var act = async () => await _sut.ReorderAchievementsAsync(userId, new List<Guid> { Guid.NewGuid() });

        await act.Should().NotThrowAsync("service ignores GUIDs that don't match any achievement");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any IDs → silently succeeds (no matches found).
    [Fact]
    public async Task CVerify38_UTCID03_ReorderAchievements_NoJwtControllerLevel_ServiceSucceeds()
    {
        var ghostUserId = Guid.NewGuid();

        var act = async () => await _sut.ReorderAchievementsAsync(ghostUserId, new List<Guid> { Guid.NewGuid() });

        await act.Should().NotThrowAsync("JWT auth is controller responsibility");
    }
}
