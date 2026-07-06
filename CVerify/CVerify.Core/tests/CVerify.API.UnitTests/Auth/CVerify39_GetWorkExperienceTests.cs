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
/// Unit tests for WorkExperienceService.GetWorkExperiencesAsync — CVerify-39 (3 UTCIDs).
/// GET /api/v1/users/work-experience [Authorize] — retrieves list of user work experiences.
/// </summary>
public sealed class CVerify39_GetWorkExperienceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly WorkExperienceService _sut;

    public CVerify39_GetWorkExperienceTests()
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

    private async Task SeedWorkExperienceAsync(Guid userId, string jobTitle = "Software Engineer")
    {
        _context.WorkExperiences.Add(new WorkExperienceEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            JobTitle = jobTitle,
            Company = "Tech Corp",
            ExperienceCategory = ExperienceCategory.ProfessionalWork,
            EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero),
            IsCurrentlyWorking = false,
            Description = "Worked on backend services",
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // User has work experiences → returns non-empty list (200)
    [Fact]
    public async Task CVerify39_UTCID01_GetWorkExperience_HasEntries_ReturnsPopulatedList()
    {
        var userId = Guid.NewGuid();
        await SeedWorkExperienceAsync(userId, "Software Engineer");
        await SeedWorkExperienceAsync(userId, "Tech Lead");

        var result = await _sut.GetWorkExperiencesAsync(userId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.JobTitle == "Software Engineer");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // User has no work experiences → returns empty list (200)
    [Fact]
    public async Task CVerify39_UTCID02_GetWorkExperience_NoEntries_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.GetWorkExperiencesAsync(userId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId has no entries → empty list.
    [Fact]
    public async Task CVerify39_UTCID03_GetWorkExperience_NoJwtControllerLevel_ServiceReturnsEmpty()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.GetWorkExperiencesAsync(ghostUserId);

        result.Should().NotBeNull();
        result.Should().BeEmpty("JWT auth is controller responsibility");
    }
}
