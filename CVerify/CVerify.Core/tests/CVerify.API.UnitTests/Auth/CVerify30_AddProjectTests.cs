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
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for ProjectService.CreateProjectAsync — CVerify-30 (7 UTCIDs).
/// POST /api/v1/users/projects [Authorize] — adds a personal/academic project to user profile.
/// </summary>
public sealed class CVerify30_AddProjectTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly ProjectService _sut;

    public CVerify30_AddProjectTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _indexer
            .Setup(i => i.IndexUserCvRepositoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new ProjectService(_context, _indexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private static readonly DateTimeOffset Start2023 = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End2024   = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static ProjectEntryRequest BuildRequest(
        string name = "E-Commerce App",
        string? role = "Lead Developer",
        string description = "Full stack e-commerce platform",
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool isCurrentlyWorking = false) =>
        new(
            Name: name,
            Role: role,
            Description: description,
            StartDate: startDate ?? Start2023,
            EndDate: endDate ?? End2024,
            IsCurrentlyWorking: isCurrentlyWorking,
            VerificationLevel: ProjectVerificationLevel.Independent,
            LinkedRepositoryIds: null,
            Technologies: null,
            Contributions: null);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify30_UTCID01_AddProject_AllFields_ReturnsProjectEntryResponse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateProjectAsync(userId, BuildRequest(
            name: "E-Commerce App",
            description: "Full stack platform",
            startDate: Start2023, endDate: End2024));

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Name.Should().Be("E-Commerce App");
        result.StartDate.Should().Be(Start2023);
        result.EndDate.Should().Be(End2024);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Name only (minimal required fields) → success
    [Fact]
    public async Task CVerify30_UTCID02_AddProject_MinimalRequiredFields_ReturnsResponse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateProjectAsync(userId, BuildRequest(name: "My API", role: null));

        result.Should().NotBeNull();
        result.Name.Should().Be("My API");
        result.Role.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // name: null → InMemory DB enforces [Required] on Name → DbUpdateException thrown.
    // Controller [Required] catches this first; at service level it throws on SaveChanges.
    [Fact]
    public async Task CVerify30_UTCID03_AddProject_NullName_ThrowsOnDatabaseSave()
    {
        var userId = Guid.NewGuid();
        var request = new ProjectEntryRequest(
            Name: null!,
            Role: null,
            Description: "description",
            StartDate: Start2023,
            EndDate: End2024,
            IsCurrentlyWorking: false,
            VerificationLevel: ProjectVerificationLevel.Independent,
            LinkedRepositoryIds: null,
            Technologies: null,
            Contributions: null);

        var act = async () => await _sut.CreateProjectAsync(userId, request);

        await act.Should().ThrowAsync<Exception>("null Name violates [Required] constraint on ProjectEntry");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // endDate < startDate → service does NOT validate date order; stores values as-is
    [Fact]
    public async Task CVerify30_UTCID04_AddProject_EndDateBeforeStartDate_ServiceAccepts()
    {
        var userId = Guid.NewGuid();
        var future = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var past   = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var result = await _sut.CreateProjectAsync(userId,
            BuildRequest(startDate: future, endDate: past));

        result.Should().NotBeNull("ProjectService does not enforce date ordering");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No linked repositories (no LinkedRepositoryIds) → success (no URL field in ProjectEntryRequest)
    [Fact]
    public async Task CVerify30_UTCID05_AddProject_NoLinkedRepositories_SucceedsWithEmptyRepoList()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateProjectAsync(userId, BuildRequest());

        result.Should().NotBeNull();
        result.RepositoryLinks.Should().BeEmpty();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: accepts any userId (creates entry for ghost user).
    [Fact]
    public async Task CVerify30_UTCID06_AddProject_NoJwtControllerLevel_ServiceCreatesEntry()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.CreateProjectAsync(ghostUserId, BuildRequest());

        result.Should().NotBeNull("JWT auth is controller responsibility");
        result.UserId.Should().Be(ghostUserId);
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    // name: 200-char string (max boundary) → success
    [Fact]
    public async Task CVerify30_UTCID07_AddProject_MaxBoundaryName200Chars_ReturnsResponse()
    {
        var userId = Guid.NewGuid();
        var longName = new string('X', 200);

        var result = await _sut.CreateProjectAsync(userId, BuildRequest(name: longName));

        result.Should().NotBeNull();
        result.Name.Should().HaveLength(200);
    }
}
