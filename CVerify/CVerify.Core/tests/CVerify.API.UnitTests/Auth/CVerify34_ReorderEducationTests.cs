using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for EducationService.ReorderEducationEntriesAsync — CVerify-34 (5 UTCIDs).
/// PUT /api/v1/users/education/reorder [Authorize] — reorders education entries by display order.
/// </summary>
public sealed class CVerify34_ReorderEducationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EducationService _sut;

    public CVerify34_ReorderEducationTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new EducationService(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, List<Guid> entryIds)> SeedEntriesAsync(int count = 2)
    {
        var userId = Guid.NewGuid();
        var ids = new List<Guid>();
        for (int i = 0; i < count; i++)
        {
            var id = Guid.NewGuid();
            ids.Add(id);
            _context.EducationEntries.Add(new EducationEntry
            {
                Id = id,
                UserId = userId,
                Label = $"Degree {i}",
                SchoolName = $"School {i}",
                IsCurrentlyStudying = false,
                DisplayOrder = i,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            });
        }
        await _context.SaveChangesAsync();
        return (userId, ids);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid orderedIds → entries reordered, display order updated (204)
    [Fact]
    public async Task CVerify34_UTCID01_ReorderEducation_ValidIds_UpdatesDisplayOrder()
    {
        var (userId, ids) = await SeedEntriesAsync(2);
        var reversed = new List<Guid> { ids[1], ids[0] };

        await _sut.ReorderEducationEntriesAsync(userId, reversed);

        var entry0 = await _context.EducationEntries.FindAsync(ids[1]);
        var entry1 = await _context.EducationEntries.FindAsync(ids[0]);
        entry0!.DisplayOrder.Should().Be(0, "first in list gets order 0");
        entry1!.DisplayOrder.Should().Be(1, "second in list gets order 1");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Invalid (non-matching) GUID in list → controller catches bad format at 400;
    // At service level, unknown GUIDs are silently ignored (no match in DB).
    [Fact]
    public async Task CVerify34_UTCID02_ReorderEducation_NonMatchingGuid_ServiceIgnoresSilently()
    {
        var (userId, _) = await SeedEntriesAsync(1);
        var unknownId = Guid.NewGuid();

        var act = async () => await _sut.ReorderEducationEntriesAsync(userId, new List<Guid> { unknownId });

        await act.Should().NotThrowAsync("service ignores GUIDs that don't match any entry for userId");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // List contains IDs belonging to another user → silently ignored (entries not found for userId)
    [Fact]
    public async Task CVerify34_UTCID03_ReorderEducation_OtherUserIds_ServiceIgnoresSilently()
    {
        var (userId, _) = await SeedEntriesAsync(1);
        var (_, otherUserEntryIds) = await SeedEntriesAsync(1);

        var act = async () => await _sut.ReorderEducationEntriesAsync(userId, otherUserEntryIds);

        await act.Should().NotThrowAsync("other-user IDs are filtered out by userId condition");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Empty orderedIds list → service returns immediately with no error (204)
    [Fact]
    public async Task CVerify34_UTCID04_ReorderEducation_EmptyList_ReturnsWithoutError()
    {
        var (userId, _) = await SeedEntriesAsync(1);

        var act = async () => await _sut.ReorderEducationEntriesAsync(userId, new List<Guid>());

        await act.Should().NotThrowAsync("service guards empty list with early return");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with empty DB → no error (empty list match).
    [Fact]
    public async Task CVerify34_UTCID05_ReorderEducation_NoJwtControllerLevel_ServiceSucceeds()
    {
        var ghostUserId = Guid.NewGuid();
        var ids = new List<Guid> { Guid.NewGuid() };

        var act = async () => await _sut.ReorderEducationEntriesAsync(ghostUserId, ids);

        await act.Should().NotThrowAsync("JWT auth is controller responsibility");
    }
}
