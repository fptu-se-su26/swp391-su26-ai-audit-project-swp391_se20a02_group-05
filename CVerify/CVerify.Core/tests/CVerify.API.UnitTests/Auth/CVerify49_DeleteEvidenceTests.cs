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
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AttachmentService.DeleteAttachmentAsync — CVerify-49 (4 UTCIDs).
/// DELETE /api/v1/users/evidence/{id} [Authorize] — soft-deletes an evidence attachment.
/// </summary>
public sealed class CVerify49_DeleteEvidenceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AttachmentService _sut;

    public CVerify49_DeleteEvidenceTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new AttachmentService(_context, _storageService.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<(Guid userId, Guid attachmentId)> SeedAsync()
    {
        var userId = Guid.NewGuid();
        var attachment = new ProfileAttachment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EntityType = "AcademicAchievement",
            EntityId = null,
            FileName = "certificate.pdf",
            FilePath = "evidence/cert-key-123",
            FileSize = 512_000L,
            FileType = "application/pdf",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.ProfileAttachments.Add(attachment);
        await _context.SaveChangesAsync();
        return (userId, attachment.Id);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // id: own evidence GUID → soft-deletes (sets DeletedAt), returns 204
    [Fact]
    public async Task CVerify49_UTCID01_DeleteEvidence_OwnAttachment_SoftDeletesSuccessfully()
    {
        var (userId, attachmentId) = await SeedAsync();

        await _sut.DeleteAttachmentAsync(userId, attachmentId);

        var attachment = await _context.ProfileAttachments.FindAsync(attachmentId);
        attachment!.DeletedAt.Should().NotBeNull("soft delete sets DeletedAt");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // id: non-existent GUID → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify49_UTCID02_DeleteEvidence_NonExistentId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.DeleteAttachmentAsync(userId, Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // id: another user's evidence GUID → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify49_UTCID03_DeleteEvidence_OtherUsersAttachment_ThrowsResourceNotFoundException()
    {
        var (_, attachmentId) = await SeedAsync();
        var anotherUserId = Guid.NewGuid();

        var act = async () => await _sut.DeleteAttachmentAsync(anotherUserId, attachmentId);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any attachmentId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify49_UTCID04_DeleteEvidence_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.DeleteAttachmentAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
