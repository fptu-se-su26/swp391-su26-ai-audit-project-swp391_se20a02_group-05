using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkExperienceService.UpdateWorkExperienceAsync — CVerify-40 (5 UTCIDs).
/// PUT /api/v1/users/work-experience/{id} [Authorize] — updates a user work experience entry.
/// </summary>
public sealed class CVerify40_UpdateWorkExperienceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly WorkExperienceService _sut;

    public CVerify40_UpdateWorkExperienceTests()
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
            JobTitle = "Junior Developer",
            Company = "Old Corp",
            ExperienceCategory = ExperienceCategory.ProfessionalWork,
            EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2022, 12, 31, 0, 0, 0, TimeSpan.Zero),
            IsCurrentlyWorking = false,
            Description = "Old description",
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.WorkExperiences.Add(entry);
        await _context.SaveChangesAsync();
        return (userId, entry.Id);
    }

    private static readonly DateTimeOffset Start2022 = new(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End2023   = new(2023, 12, 31, 0, 0, 0, TimeSpan.Zero);

    private static WorkExperienceRequest BuildRequest(
        string jobTitle = "Senior Developer",
        string company = "New Corp",
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null) =>
        new(
            JobTitle: jobTitle,
            Company: company,
            ExperienceCategory: (int)ExperienceCategory.ProfessionalWork,
            EmploymentType: (int)EmploymentType.FullTime,
            Location: "Ho Chi Minh City",
            StartDate: startDate ?? Start2022,
            EndDate: endDate ?? End2023,
            IsCurrentlyWorking: false,
            Description: "Updated backend systems",
            Achievements: null,
            Technologies: null,
            Links: null);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid update → returns updated WorkExperienceResponse (200)
    [Fact]
    public async Task CVerify40_UTCID01_UpdateWorkExperience_ValidData_ReturnsUpdatedResponse()
    {
        var (userId, entryId) = await SeedAsync();

        var result = await _sut.UpdateWorkExperienceAsync(userId, entryId, BuildRequest(
            jobTitle: "Senior Developer", company: "New Corp"));

        result.Should().NotBeNull();
        result.JobTitle.Should().Be("Senior Developer");
        result.Company.Should().Be("New Corp");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent entryId → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify40_UTCID02_UpdateWorkExperience_NonExistentId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.UpdateWorkExperienceAsync(userId, Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // company: null → service calls request.Company.Trim() → NullReferenceException (400)
    [Fact]
    public async Task CVerify40_UTCID03_UpdateWorkExperience_NullCompany_ThrowsOnTrim()
    {
        var (userId, entryId) = await SeedAsync();
        var request = new WorkExperienceRequest(
            JobTitle: "Developer",
            Company: null!,
            ExperienceCategory: (int)ExperienceCategory.ProfessionalWork,
            EmploymentType: (int)EmploymentType.FullTime,
            Location: null,
            StartDate: Start2022,
            EndDate: End2023,
            IsCurrentlyWorking: false,
            Description: "Description",
            Achievements: null,
            Technologies: null,
            Links: null);

        var act = async () => await _sut.UpdateWorkExperienceAsync(userId, entryId, request);

        await act.Should().ThrowAsync<Exception>("service calls Company.Trim() — null throws NullReferenceException");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // EndDate before StartDate → BusinessRuleException (service validates dates) (400)
    [Fact]
    public async Task CVerify40_UTCID04_UpdateWorkExperience_InvalidDates_ThrowsBusinessRuleException()
    {
        var (userId, entryId) = await SeedAsync();
        var future = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var past   = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var act = async () => await _sut.UpdateWorkExperienceAsync(userId, entryId,
            BuildRequest(startDate: future, endDate: past));

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*End date cannot be before*");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any entryId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify40_UTCID05_UpdateWorkExperience_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateWorkExperienceAsync(Guid.NewGuid(), Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
