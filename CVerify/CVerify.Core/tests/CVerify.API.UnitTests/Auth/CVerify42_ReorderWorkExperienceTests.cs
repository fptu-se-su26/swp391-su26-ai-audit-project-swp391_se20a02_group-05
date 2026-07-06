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
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkExperienceService.ReorderWorkExperiencesAsync — CVerify-42 (3 UTCIDs).
/// PUT /api/v1/users/work-experience/reorder [Authorize] — reorders work experience entries.
/// NOTE: Unlike Education/Achievement reorder, this service THROWS BusinessRuleException if
/// any ID in orderedIds does not match an existing entry for userId.
/// </summary>
public sealed class CVerify42_ReorderWorkExperienceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly WorkExperienceService _sut;

    public CVerify42_ReorderWorkExperienceTests()
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

    private async Task<(Guid userId, List<Guid> ids)> SeedEntriesAsync(int count = 3)
    {
        var userId = Guid.NewGuid();
        var ids = new List<Guid>();
        for (int i = 0; i < count; i++)
        {
            var id = Guid.NewGuid();
            ids.Add(id);
            _context.WorkExperiences.Add(new WorkExperienceEntry
            {
                Id = id,
                UserId = userId,
                JobTitle = $"Job {i}",
                Company = $"Corp {i}",
                ExperienceCategory = ExperienceCategory.ProfessionalWork,
                EmploymentType = EmploymentType.FullTime,
                StartDate = new DateTimeOffset(2020 + i, 1, 1, 0, 0, 0, TimeSpan.Zero),
                IsCurrentlyWorking = false,
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
    // orderedIds: [valid_id2, valid_id1, valid_id3] → reorders entries, returns 204
    [Fact]
    public async Task CVerify42_UTCID01_ReorderWorkExperience_ValidIds_UpdatesDisplayOrder()
    {
        var (userId, ids) = await SeedEntriesAsync(3);
        var reordered = new List<Guid> { ids[2], ids[0], ids[1] };

        await _sut.ReorderWorkExperiencesAsync(userId, reordered);

        var entry0 = await _context.WorkExperiences.FindAsync(ids[2]);
        var entry1 = await _context.WorkExperiences.FindAsync(ids[0]);
        entry0!.DisplayOrder.Should().Be(0);
        entry1!.DisplayOrder.Should().Be(1);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // orderedIds contains non-existent GUID → service throws BusinessRuleException (400)
    // because entries.Count != orderedIds.Count (ownership check).
    [Fact]
    public async Task CVerify42_UTCID02_ReorderWorkExperience_NonExistentGuid_ThrowsBusinessRuleException()
    {
        var (userId, ids) = await SeedEntriesAsync(2);
        var withUnknown = new List<Guid> { ids[0], Guid.NewGuid() };

        var act = async () => await _sut.ReorderWorkExperiencesAsync(userId, withUnknown);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*invalid*");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any IDs → entries.Count=0 != orderedIds.Count
    //   → BusinessRuleException thrown.
    [Fact]
    public async Task CVerify42_UTCID03_ReorderWorkExperience_NoJwtControllerLevel_ServiceThrowsBusinessRuleException()
    {
        var ghostUserId = Guid.NewGuid();
        var ids = new List<Guid> { Guid.NewGuid() };

        var act = async () => await _sut.ReorderWorkExperiencesAsync(ghostUserId, ids);

        await act.Should().ThrowAsync<BusinessRuleException>("ghost userId has no matching entries");
    }
}
