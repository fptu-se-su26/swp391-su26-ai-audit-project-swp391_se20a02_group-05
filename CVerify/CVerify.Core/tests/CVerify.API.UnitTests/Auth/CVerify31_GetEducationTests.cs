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
/// Unit tests for EducationService.GetEducationEntriesAsync — CVerify-31 (3 UTCIDs).
/// GET /api/v1/users/education [Authorize] — retrieves list of user education entries.
/// </summary>
public sealed class CVerify31_GetEducationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EducationService _sut;

    public CVerify31_GetEducationTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new EducationService(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task SeedEntryAsync(Guid userId, string schoolName = "FPT University")
    {
        _context.EducationEntries.Add(new EducationEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Label = "Bachelor",
            SchoolName = schoolName,
            IsCurrentlyStudying = false,
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // User has education entries → returns non-empty list (200)
    [Fact]
    public async Task CVerify31_UTCID01_GetEducation_HasEntries_ReturnsPopulatedList()
    {
        var userId = Guid.NewGuid();
        await SeedEntryAsync(userId, "FPT University");
        await SeedEntryAsync(userId, "RMIT Vietnam");

        var result = await _sut.GetEducationEntriesAsync(userId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.SchoolName == "FPT University");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // User has no education entries → returns empty list (200)
    [Fact]
    public async Task CVerify31_UTCID02_GetEducation_NoEntries_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetEducationEntriesAsync(userId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId has no entries → empty list returned.
    [Fact]
    public async Task CVerify31_UTCID03_GetEducation_NoJwtControllerLevel_ServiceReturnsEmpty()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetEducationEntriesAsync(ghostUserId);

        result.Should().NotBeNull();
        result.Should().BeEmpty("JWT auth is controller responsibility — service returns empty for unknown userId");
    }
}
