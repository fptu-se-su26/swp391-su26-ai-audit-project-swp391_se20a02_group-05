using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for EducationService.DeleteEducationEntryAsync — CVerify-33 (4 UTCIDs).
/// DELETE /api/v1/users/education/{id} [Authorize] — soft-deletes a user education entry.
/// </summary>
public sealed class CVerify33_DeleteEducationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EducationService _sut;

    public CVerify33_DeleteEducationTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new EducationService(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid entryId)> SeedAsync()
    {
        var userId = Guid.NewGuid();
        var entry = new EducationEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Label = "Bachelor",
            SchoolName = "FPT University",
            IsCurrentlyStudying = false,
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.EducationEntries.Add(entry);
        await _context.SaveChangesAsync();
        return (userId, entry.Id);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid userId and entryId → soft-deletes entry (sets DeletedAt), returns 204
    [Fact]
    public async Task CVerify33_UTCID01_DeleteEducation_ValidEntry_SoftDeletesSuccessfully()
    {
        var (userId, entryId) = await SeedAsync();

        await _sut.DeleteEducationEntryAsync(userId, entryId);

        var entry = await _context.EducationEntries.FindAsync(entryId);
        entry!.DeletedAt.Should().NotBeNull("soft delete sets DeletedAt");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent entryId → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify33_UTCID02_DeleteEducation_NonExistentEntryId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.DeleteEducationEntryAsync(userId, Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Other user's entryId → ResourceNotFoundException (403/404)
    [Fact]
    public async Task CVerify33_UTCID03_DeleteEducation_OtherUsersEntry_ThrowsResourceNotFoundException()
    {
        var (_, entryId) = await SeedAsync();
        var otherUserId = Guid.NewGuid();

        var act = async () => await _sut.DeleteEducationEntryAsync(otherUserId, entryId);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any entryId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify33_UTCID04_DeleteEducation_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.DeleteEducationEntryAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
