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
/// Unit tests for ProjectService.DeleteProjectAsync — CVerify-45 (3 UTCIDs).
/// DELETE /api/v1/users/projects/{id} [Authorize] — hard-deletes a project entry.
/// NOTE: DeleteProjectAsync is IDEMPOTENT — returns success even if project not found.
/// </summary>
public sealed class CVerify45_DeleteProjectTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICvRepositoryIndexer> _indexer = new();
    private readonly ProjectService _sut;

    public CVerify45_DeleteProjectTests()
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
            Name = "My Project",
            Description = "Project desc",
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

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid own project GUID → hard-deletes project, returns 204
    [Fact]
    public async Task CVerify45_UTCID01_DeleteProject_ValidProject_DeletesSuccessfully()
    {
        var (userId, projectId) = await SeedAsync();

        await _sut.DeleteProjectAsync(userId, projectId);

        var project = await _context.ProjectEntries.FindAsync(projectId);
        project.Should().BeNull("project is hard-deleted from DB");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent GUID → idempotent (no exception), returns 204
    [Fact]
    public async Task CVerify45_UTCID02_DeleteProject_NonExistentId_IdempotentSuccess()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.DeleteProjectAsync(userId, Guid.NewGuid());

        await act.Should().NotThrowAsync("DeleteProjectAsync is idempotent — silently succeeds if not found");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any projectId → idempotent, no exception.
    [Fact]
    public async Task CVerify45_UTCID03_DeleteProject_NoJwtControllerLevel_ServiceIdempotent()
    {
        var act = async () => await _sut.DeleteProjectAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().NotThrowAsync("JWT auth is controller responsibility; service is idempotent");
    }
}
