using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for EducationService.CreateEducationEntryAsync — CVerify-20 (8 UTCIDs).
/// POST /api/v1/users/education [Authorize] — adds a new education entry to user profile.
/// </summary>
public sealed class CVerify20_AddEducationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EducationService _sut;

    public CVerify20_AddEducationTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _sut = new EducationService(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task<User> SeedUserAsync()
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = "user@example.com",
            FullName        = "Edu User",
            Username        = "eduuser",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    private static EducationEntryRequest BuildRequest(
        string label      = "Bachelor Degree",
        string schoolName = "FPT University",
        string? degree    = "Bachelor of Science",
        string? major     = "Software Engineering",
        decimal? gpa      = 3.5m,
        decimal? gpaScale = 4.0m,
        string? description = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool isCurrentlyStudying = false) =>
        new(
            Label:             label,
            SchoolName:        schoolName,
            Degree:            degree,
            Major:             major,
            GPA:               gpa,
            GPAScale:          gpaScale,
            Description:       description,
            StartDate:         startDate,
            EndDate:           endDate,
            IsCurrentlyStudying: isCurrentlyStudying);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify20_UTCID01_AddEducation_AllRequiredFields_ReturnsEducationEntryResponse()
    {
        var user = await SeedUserAsync();

        var result = await _sut.CreateEducationEntryAsync(user.Id, BuildRequest());

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.SchoolName.Should().Be("FPT University");
        result.Degree.Should().Be("Bachelor of Science");
        result.Major.Should().Be("Software Engineering");
        result.GPA.Should().Be(3.5m);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify20_UTCID02_AddEducation_RequiredFieldsOnlyNoOptionals_ReturnsResponse()
    {
        var user = await SeedUserAsync();

        var result = await _sut.CreateEducationEntryAsync(user.Id,
            BuildRequest(degree: null, major: null, gpa: null, gpaScale: null));

        result.Should().NotBeNull();
        result.Degree.Should().BeNull();
        result.GPA.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // At service level: non-existent userId → entry still gets created (service does not validate userId against DB).
    [Fact]
    public async Task CVerify20_UTCID03_AddEducation_NoJwtControllerLevel_ServiceCreatesEntryForAnyUserId()
    {
        var ghostUserId = Guid.NewGuid(); // not in DB

        var result = await _sut.CreateEducationEntryAsync(ghostUserId, BuildRequest());

        result.Should().NotBeNull("service does not check JWT — controller responsibility");
        result.UserId.Should().Be(ghostUserId);
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify20_UTCID04_AddEducation_WithStartAndEndDate_StoresDates()
    {
        var user = await SeedUserAsync();
        var start = new DateTimeOffset(2019, 9, 1, 0, 0, 0, TimeSpan.Zero);
        var end   = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero);

        var result = await _sut.CreateEducationEntryAsync(user.Id,
            BuildRequest(startDate: start, endDate: end));

        result.StartDate.Should().Be(start);
        result.EndDate.Should().Be(end);
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify20_UTCID05_AddEducation_IsCurrentlyStudying_StoresFlag()
    {
        var user = await SeedUserAsync();

        var result = await _sut.CreateEducationEntryAsync(user.Id,
            BuildRequest(isCurrentlyStudying: true, endDate: null));

        result.IsCurrentlyStudying.Should().BeTrue();
        result.EndDate.Should().BeNull();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify20_UTCID06_AddEducation_WithDescription_StoresDescription()
    {
        var user = await SeedUserAsync();
        const string desc = "Studied computer science fundamentals and software engineering.";

        var result = await _sut.CreateEducationEntryAsync(user.Id,
            BuildRequest(description: desc));

        result.Description.Should().Be(desc);
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify20_UTCID07_AddEducation_MultipleEntries_DisplayOrderIncrements()
    {
        var user = await SeedUserAsync();

        var first  = await _sut.CreateEducationEntryAsync(user.Id, BuildRequest(label: "First",  schoolName: "School A"));
        var second = await _sut.CreateEducationEntryAsync(user.Id, BuildRequest(label: "Second", schoolName: "School B"));

        first.DisplayOrder.Should().Be(0);
        second.DisplayOrder.Should().Be(1);
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify20_UTCID08_AddEducation_GpaZeroBoundary_StoresGpa()
    {
        var user = await SeedUserAsync();

        var result = await _sut.CreateEducationEntryAsync(user.Id, BuildRequest(gpa: 0m, gpaScale: 4.0m));

        result.GPA.Should().Be(0m);
    }
}
