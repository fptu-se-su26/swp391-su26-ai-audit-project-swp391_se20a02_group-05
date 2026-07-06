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
/// Unit tests for ProjectService.UpdateProjectAsync — CVerify-44 (4 UTCIDs).
/// PUT /api/v1/users/projects/{id} [Authorize] — updates a user project entry.
/// </summary>
public sealed class CVerify44_UpdateProjectTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly ProjectService _sut;

    public CVerify44_UpdateProjectTests()
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

    private async Task<(Guid userId, Guid projectId)> SeedAsync()
    {
        var userId = Guid.NewGuid();
        var project = new ProjectEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Old Project",
            Description = "Old description",
            VerificationLevel = ProjectVerificationLevel.Independent,
            VerificationStatus = ProjectVerificationStatus.Unverified,
            DisplayOrder = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.ProjectEntries.Add(project);
        await _context.SaveChangesAsync();
        return (userId, project.Id);
    }

    private static readonly DateTimeOffset Start2023 = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End2024 = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static ProjectEntryRequest BuildRequest(string name = "Updated Project") =>
        new(
            Name: name,
            Role: "Lead",
            Description: "Updated description",
            StartDate: Start2023,
            EndDate: End2024,
            IsCurrentlyWorking: false,
            VerificationLevel: ProjectVerificationLevel.Independent,
            LinkedRepositoryIds: null,
            Technologies: null,
            Contributions: null);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid project GUID, name:'Updated Project' → returns ProjectEntryResponse (200)
    [Fact]
    public async Task CVerify44_UTCID01_UpdateProject_ValidData_ReturnsUpdatedResponse()
    {
        var (userId, projectId) = await SeedAsync();

        var result = await _sut.UpdateProjectAsync(userId, projectId, BuildRequest("Updated Project"));

        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Project");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent GUID → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify44_UTCID02_UpdateProject_NonExistentId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.UpdateProjectAsync(userId, Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // name: null → controller [Required] returns 400 before reaching service.
    // Service level: UpdateProjectAsync does project.Name = request.Name (no .Trim()),
    // so null is accepted without exception. This documents controller-level validation.
    [Fact]
    public async Task CVerify44_UTCID03_UpdateProject_NullName_ServiceLevelAccepts()
    {
        var (userId, projectId) = await SeedAsync();
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

        var act = async () => await _sut.UpdateProjectAsync(userId, projectId, request);

        await act.Should().NotThrowAsync("Name=[Required] is controller-level DTO validation; service assigns directly without checking");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any projectId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify44_UTCID04_UpdateProject_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.UpdateProjectAsync(Guid.NewGuid(), Guid.NewGuid(), BuildRequest());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
