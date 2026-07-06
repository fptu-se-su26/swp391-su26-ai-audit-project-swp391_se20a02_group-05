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
/// Unit tests for AttachmentService.GetAttachmentSignedUrlAsync — CVerify-48 (4 UTCIDs).
/// GET /api/v1/users/evidence/{id}/download [Authorize] — returns signed URL for file download.
/// Controller redirects (302) to the signed URL returned by the service.
/// </summary>
public sealed class CVerify48_DownloadEvidenceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AttachmentService _sut;

    public CVerify48_DownloadEvidenceTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://cdn.example.com/signed-download-url");

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
    // id: own evidence GUID → returns signed download URL (controller redirects 302)
    [Fact]
    public async Task CVerify48_UTCID01_DownloadEvidence_OwnAttachment_ReturnsSignedUrl()
    {
        var (userId, attachmentId) = await SeedAsync();

        var url = await _sut.GetAttachmentSignedUrlAsync(userId, attachmentId);

        url.Should().Be("https://cdn.example.com/signed-download-url");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // id: non-existent GUID → ResourceNotFoundException (404)
    [Fact]
    public async Task CVerify48_UTCID02_DownloadEvidence_NonExistentId_ThrowsResourceNotFoundException()
    {
        var (userId, _) = await SeedAsync();

        var act = async () => await _sut.GetAttachmentSignedUrlAsync(userId, Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // id: another user's evidence GUID → ResourceNotFoundException (404)
    // Service filters by userId, so other-user attachments are not found.
    [Fact]
    public async Task CVerify48_UTCID03_DownloadEvidence_OtherUsersAttachment_ThrowsResourceNotFoundException()
    {
        var (_, attachmentId) = await SeedAsync();
        var anotherUserId = Guid.NewGuid();

        var act = async () => await _sut.GetAttachmentSignedUrlAsync(anotherUserId, attachmentId);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId with any attachmentId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify48_UTCID04_DownloadEvidence_NoJwtControllerLevel_ServiceThrowsNotFound()
    {
        var act = async () => await _sut.GetAttachmentSignedUrlAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
