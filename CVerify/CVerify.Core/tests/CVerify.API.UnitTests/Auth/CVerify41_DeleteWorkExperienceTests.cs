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
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkExperienceService.DeleteWorkExperienceAsync — CVerify-41 (3 UTCIDs).
/// DELETE /api/v1/users/work-experience/{id} [Authorize] — soft-deletes a work experience entry.
/// </summary>
public sealed class CVerify41_DeleteWorkExperienceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly WorkExperienceService _sut;

    public CVerify41_DeleteWorkExperienceTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _indexer
            .Setup(i => i.IndexUserCvRepositoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new WorkExperienceService(_context, _indexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid entryId)> SeedAsync()
    {
        var userId = Guid.NewGuid();
        var entry = new WorkExperienceEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            JobTitle = "Software Engineer",
            Company = "Tech Corp",
            ExperienceCategory = ExperienceCategory.ProfessionalWork,
            EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero),
            IsCurrentlyWorking = false,
            Description = "Backend dev",
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.WorkExperiences.Add(entry);
        await _context.SaveChangesAsync();
        return (userId, entry.Id);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid own work experience GUID → soft-deletes (sets DeletedAt), returns 204
    [Fact]
    public async Task CVerify41_UTCID01_DeleteWorkExperience_ValidEntry_SoftDeletesSuccessfully()
    {
        var (userId, entryId) = await SeedAsync();

        await _sut.DeleteWorkExperienceAsync(userId, entryId);

        var entry = await _context.WorkExperiences.FindAsync(entryId);
        entry!.DeletedAt.Should().NotBeNull("soft delete sets DeletedAt");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent GUID → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify41_UTCID02_DeleteWorkExperience_NonExistentId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.DeleteWorkExperienceAsync(userId, Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any entryId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify41_UTCID03_DeleteWorkExperience_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.DeleteWorkExperienceAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
