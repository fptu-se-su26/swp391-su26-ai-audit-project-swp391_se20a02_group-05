using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for EducationService.UpdateEducationEntryAsync — CVerify-32 (6 UTCIDs).
/// PUT /api/v1/users/education/{id} [Authorize] — updates a user education entry.
/// </summary>
public sealed class CVerify32_UpdateEducationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EducationService _sut;

    public CVerify32_UpdateEducationTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new EducationService(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid entryId)> SeedAsync(string label = "Bachelor", string school = "FPT University")
    {
        var userId = Guid.NewGuid();
        var entry = new EducationEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Label = label,
            SchoolName = school,
            IsCurrentlyStudying = false,
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.EducationEntries.Add(entry);
        await _context.SaveChangesAsync();
        return (userId, entry.Id);
    }

    private static EducationEntryRequest BuildRequest(string label = "Master", string school = "RMIT Vietnam") =>
        new(
            Label: label,
            SchoolName: school,
            Degree: "M.Sc.",
            Major: "Software Engineering",
            GPA: null,
            GPAScale: null,
            Description: "Updated description",
            StartDate: new DateTimeOffset(2021, 9, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate: new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero),
            IsCurrentlyStudying: false);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid update → returns updated EducationEntryResponse (200)
    [Fact]
    public async Task CVerify32_UTCID01_UpdateEducation_ValidData_ReturnsUpdatedResponse()
    {
        var (userId, entryId) = await SeedAsync();

        var result = await _sut.UpdateEducationEntryAsync(userId, entryId, BuildRequest(school: "RMIT Vietnam"));

        result.Should().NotBeNull();
        result.SchoolName.Should().Be("RMIT Vietnam");
        result.Label.Should().Be("Master");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent entryId → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify32_UTCID02_UpdateEducation_NonExistentEntryId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.UpdateEducationEntryAsync(userId, Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Wrong userId (entry belongs to another user) → ResourceNotFoundException (404/403)
    [Fact]
    public async Task CVerify32_UTCID03_UpdateEducation_WrongUserId_ThrowsResourceNotFoundException()
    {
        var (_, entryId) = await SeedAsync();
        var anotherUserId = Guid.NewGuid();

        var act = async () => await _sut.UpdateEducationEntryAsync(anotherUserId, entryId, BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // SchoolName null → service calls .Trim() on it → NullReferenceException (400)
    [Fact]
    public async Task CVerify32_UTCID04_UpdateEducation_NullSchoolName_ThrowsOnTrim()
    {
        var (userId, entryId) = await SeedAsync();
        var request = new EducationEntryRequest(
            Label: "Bachelor",
            SchoolName: null!,
            Degree: null,
            Major: null,
            GPA: null,
            GPAScale: null,
            Description: null,
            StartDate: null,
            EndDate: null,
            IsCurrentlyStudying: false);

        var act = async () => await _sut.UpdateEducationEntryAsync(userId, entryId, request);

        await act.Should().ThrowAsync<Exception>("service calls SchoolName.Trim() — null throws NullReferenceException");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any entryId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify32_UTCID05_UpdateEducation_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateEducationEntryAsync(Guid.NewGuid(), Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // Same data as existing → idempotent, no error (200)
    [Fact]
    public async Task CVerify32_UTCID06_UpdateEducation_SameDataAsExisting_SucceedsIdempotently()
    {
        var (userId, entryId) = await SeedAsync(label: "Bachelor", school: "FPT University");
        var sameDataRequest = new EducationEntryRequest(
            Label: "Bachelor",
            SchoolName: "FPT University",
            Degree: null,
            Major: null,
            GPA: null,
            GPAScale: null,
            Description: null,
            StartDate: null,
            EndDate: null,
            IsCurrentlyStudying: false);

        var result = await _sut.UpdateEducationEntryAsync(userId, entryId, sameDataRequest);

        result.Should().NotBeNull();
        result.SchoolName.Should().Be("FPT University");
    }
}
