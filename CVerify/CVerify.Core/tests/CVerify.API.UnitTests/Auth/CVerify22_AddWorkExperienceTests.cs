using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkExperienceService.CreateWorkExperienceAsync — CVerify-22 (7 UTCIDs).
/// POST /api/v1/users/work-experience [Authorize] — adds a work experience entry.
/// </summary>
public sealed class CVerify22_AddWorkExperienceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly WorkExperienceService _sut;

    public CVerify22_AddWorkExperienceTests()
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

    private static readonly DateTimeOffset Start2022 = new(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End2024   = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Start2023 = new(2023, 6, 1, 0, 0, 0, TimeSpan.Zero);

    private static WorkExperienceRequest BuildRequest(
        string jobTitle = "Software Engineer",
        string company = "Google",
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool isCurrentlyWorking = false,
        string description = "Developed features.") =>
        new(
            JobTitle: jobTitle,
            Company: company,
            ExperienceCategory: 0,
            EmploymentType: 0,
            Location: null,
            StartDate: startDate ?? Start2022,
            EndDate: endDate,
            IsCurrentlyWorking: isCurrentlyWorking,
            Description: description,
            Achievements: null,
            Technologies: null,
            Links: null);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify22_UTCID01_AddWorkExp_WithEndDate_ReturnsWorkExperienceResponse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateWorkExperienceAsync(userId,
            BuildRequest(company: "Google", jobTitle: "SWE",
                startDate: Start2022, endDate: End2024));

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Company.Should().Be("Google");
        result.JobTitle.Should().Be("SWE");
        result.EndDate.Should().Be(End2024);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // isCurrent=true → no endDate required
    [Fact]
    public async Task CVerify22_UTCID02_AddWorkExp_IsCurrentlyWorking_NoEndDateRequired()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateWorkExperienceAsync(userId,
            BuildRequest(company: "Startup", jobTitle: "Lead Dev",
                startDate: Start2023, endDate: null, isCurrentlyWorking: true));

        result.Should().NotBeNull();
        result.IsCurrentlyWorking.Should().BeTrue();
        result.EndDate.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // company: null → request.Company.Trim() throws at service level
    [Fact]
    public async Task CVerify22_UTCID03_AddWorkExp_NullCompany_ThrowsAtServiceLevel()
    {
        var userId = Guid.NewGuid();
        var request = BuildRequest(company: null!);

        var act = async () => await _sut.CreateWorkExperienceAsync(userId, request);

        await act.Should().ThrowAsync<Exception>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // title: null → request.JobTitle.Trim() throws at service level
    [Fact]
    public async Task CVerify22_UTCID04_AddWorkExp_NullJobTitle_ThrowsAtServiceLevel()
    {
        var userId = Guid.NewGuid();
        var request = BuildRequest(jobTitle: null!);

        var act = async () => await _sut.CreateWorkExperienceAsync(userId, request);

        await act.Should().ThrowAsync<Exception>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // endDate < startDate → BusinessRuleException (INVALID_DATE_CONSTRAINT)
    [Fact]
    public async Task CVerify22_UTCID05_AddWorkExp_EndDateBeforeStartDate_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        var startDate = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate   = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var act = async () => await _sut.CreateWorkExperienceAsync(userId,
            BuildRequest(startDate: startDate, endDate: endDate, isCurrentlyWorking: false));

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*End date cannot be before start date*");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: accepts any userId (creates entry for ghost user).
    [Fact]
    public async Task CVerify22_UTCID06_AddWorkExp_NoJwtControllerLevel_ServiceCreatesEntry()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.CreateWorkExperienceAsync(ghostUserId,
            BuildRequest(endDate: End2024, isCurrentlyWorking: false));

        result.Should().NotBeNull();
        result.UserId.Should().Be(ghostUserId);
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    // startDate = today's date (boundary) → success (no past-date constraint)
    [Fact]
    public async Task CVerify22_UTCID07_AddWorkExp_StartDateToday_ReturnsResponse()
    {
        var userId = Guid.NewGuid();
        var today  = DateTimeOffset.UtcNow.Date;
        var endDate = new DateTimeOffset(today.AddDays(1), TimeSpan.Zero);

        var result = await _sut.CreateWorkExperienceAsync(userId,
            BuildRequest(startDate: new DateTimeOffset(today, TimeSpan.Zero),
                endDate: endDate, isCurrentlyWorking: false));

        result.Should().NotBeNull();
        result.StartDate.Date.Should().Be(today);
    }
}
